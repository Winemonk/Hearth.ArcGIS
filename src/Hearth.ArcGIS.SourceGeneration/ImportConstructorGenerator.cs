using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DryIoc.Hearth
{
    [Generator]
    public class ImportConstructorGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // 步骤1：筛选带有 [InjectParam] 特性的字段
            IncrementalValuesProvider<FieldDeclarationSyntax> fieldDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: (s, _) => IsFieldWithImportAttribute(s),
                    transform: (ctx, _) => GetFieldDeclaration(ctx))
                .Where(m => m != null);

            // 步骤2：按所属类分组
            IncrementalValueProvider<(Compilation, ImmutableArray<FieldDeclarationSyntax>)> compilationAndFields
                = context.CompilationProvider.Combine(fieldDeclarations.Collect());

            // 步骤3：生成代码
            context.RegisterSourceOutput(compilationAndFields,
                (spc, source) => Execute(source.Item1, source.Item2, spc));
        }

        private static bool IsFieldWithImportAttribute(SyntaxNode node)
        {
            // 检查是否是字段声明且带有特性
            if (node is FieldDeclarationSyntax field && field.AttributeLists.Count > 0)
            {
                foreach (AttributeListSyntax attrList in field.AttributeLists)
                {
                    foreach (AttributeSyntax attr in attrList.Attributes)
                    {
                        // 检查特性名称是否为 InjectParam（允许带或不带 Attribute 后缀）
                        string attrName = attr.Name.ToString();
                        if (attrName == "InjectParam" || attrName == "InjectParamAttribute")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static FieldDeclarationSyntax GetFieldDeclaration(GeneratorSyntaxContext context)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            // 验证特性是否来自 Hearth.ArcGIS 命名空间
            foreach (AttributeListSyntax attrList in fieldDeclaration.AttributeLists)
            {
                foreach (AttributeSyntax attr in attrList.Attributes)
                {
                    if (context.SemanticModel.GetSymbolInfo(attr).Symbol is IMethodSymbol attrSymbol)
                    {
                        string fullName = attrSymbol.ContainingType.ToDisplayString();
                        if (fullName == "Hearth.ArcGIS.InjectParamAttribute")
                        {
                            return fieldDeclaration;
                        }
                    }
                }
            }
            return null;
        }

        private static void Execute(
            Compilation compilation,
            ImmutableArray<FieldDeclarationSyntax> fields,
            SourceProductionContext context)
        {
            if (fields.IsDefaultOrEmpty)
                return;

            // 按类分组字段
            var fieldsByClass = fields
                .GroupBy(f => f.Parent as ClassDeclarationSyntax)
                .Where(g => g.Key != null);

            foreach (var group in fieldsByClass)
            {
                ClassDeclarationSyntax classDecl = group.Key;
                SemanticModel model = compilation.GetSemanticModel(classDecl.SyntaxTree);
                INamedTypeSymbol classSymbol = model.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;

                if (classSymbol == null) continue;

                // 收集需要注入的字段
                var injectionFields = new List<IFieldSymbol>();
                foreach (FieldDeclarationSyntax fieldDecl in group)
                {
                    foreach (VariableDeclaratorSyntax variable in fieldDecl.Declaration.Variables)
                    {
                        IFieldSymbol fieldSymbol = model.GetDeclaredSymbol(variable) as IFieldSymbol;
                        if (fieldSymbol != null)
                        {
                            injectionFields.Add(fieldSymbol);
                        }
                    }
                }

                if (injectionFields.Count > 0)
                {
                    string source = GenerateConstructor(classSymbol, injectionFields);
                    context.AddSource($"{classSymbol.Name}_ImportConstructor.g.cs",
                        SourceText.From(source, Encoding.UTF8));
                }
            }
        }

        private static string GenerateConstructor(INamedTypeSymbol classSymbol, List<IFieldSymbol> fields)
        {
            string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
            string className = classSymbol.Name;
            string accessibility = classSymbol.DeclaredAccessibility.ToString().ToLowerInvariant();

            StringBuilder src = new StringBuilder();
            src.AppendLine($"namespace {namespaceName}");
            src.AppendLine("{");
            src.AppendLine($"    {accessibility} partial class {className}");
            src.AppendLine("    {");

            // 构造函数签名
            src.Append($"        public {className}(");
            src.Append(string.Join(", ", fields.Select(GenerateParameter)));
            src.AppendLine(")");
            src.AppendLine("        {");

            // 字段赋值
            foreach (var field in fields)
            {
                src.AppendLine($"            this.{field.Name} = {field.Name};");
            }

            src.AppendLine("        }");
            src.AppendLine("    }");
            src.AppendLine("}");

            return src.ToString();
        }

        private static string GenerateParameter(IFieldSymbol field)
        {
            AttributeData importAttr = field.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == "InjectParamAttribute");

            if (importAttr == null)
                return $"{field.Type} {field.Name}";

            // 处理 ServiceKey 参数
            string serviceKeyArg = "";
            if (importAttr.ConstructorArguments.Length > 0)
            {
                var arg = importAttr.ConstructorArguments[0];
                if (!arg.IsNull)
                {
                    serviceKeyArg = FormatArgument(arg.Value);
                }
            }

            string attribute = string.IsNullOrEmpty(serviceKeyArg)
                ? "[Hearth.ArcGIS.InjectParam]"
                : $"[Hearth.ArcGIS.InjectParam({serviceKeyArg})]";

            return $"{attribute} {field.Type.ToDisplayString()} {field.Name}";
        }

        private static string FormatArgument(object value)
        {
            switch (value)
            {
                case string s:
                    return $"\"{s}\"";
                case int i:
                    return i.ToString();
                case bool b:
                    return b.ToString().ToLowerInvariant();
            }
            if (value.GetType().IsEnum)
            {
                return $"{value.GetType().FullName}.{value}";
            }
            return value.ToString() ?? "null";
        }
    }
}