﻿using DryIoc;

namespace Hearth.ArcGIS.Samples
{
    public class CustomContainerBuilder : ContainerBuilder
    {
        public override Container Build()
        {
            Container container = new Container(
                rules => rules
                    .With(
                        FactoryMethod.ConstructorWithResolvableArgumentsIncludingNonPublic,
                        null,
                        PropertiesAndFields.All()));
            AddNlog(container);
            AddAutoMapper(container);
            AddViewModelLocationProvider(container);
            return container;
        }
    }
}
