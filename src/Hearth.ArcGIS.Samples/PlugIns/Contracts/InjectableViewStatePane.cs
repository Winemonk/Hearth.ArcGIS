using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Core;
using DryIoc;

namespace Hearth.ArcGIS.Samples.PlugIns.Contracts
{
    public class InjectableViewStatePane : ViewStatePane, IScopeInjectable
    {
        public InjectableViewStatePane(CIMView cimView) : base(cimView)
        {
            this.InjectServices();
        }

        public override CIMView ViewState
        {
            get
            {
                _cimView.InstanceID = (int)InstanceID;
                return _cimView;
            }
        }

        public IResolverContext Scope { get; set; }

        ~InjectableViewStatePane() => Scope?.Dispose();
    }
}
