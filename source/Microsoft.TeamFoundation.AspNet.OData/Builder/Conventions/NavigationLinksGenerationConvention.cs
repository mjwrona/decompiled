// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.NavigationLinksGenerationConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.AspNet.OData.Builder.Conventions
{
  internal class NavigationLinksGenerationConvention : INavigationSourceConvention, IConvention
  {
    public void Apply(NavigationSourceConfiguration configuration, ODataModelBuilder model)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      foreach (StructuralTypeConfiguration thisAndBaseType in configuration.EntityType.ThisAndBaseTypes())
      {
        foreach (NavigationPropertyConfiguration navigationProperty1 in thisAndBaseType.NavigationProperties)
        {
          if (configuration.GetNavigationPropertyLink(navigationProperty1) == null)
            configuration.HasNavigationPropertyLink(navigationProperty1, new NavigationLinkBuilder((Func<ResourceContext, IEdmNavigationProperty, Uri>) ((entityContext, navigationProperty) => entityContext.GenerateNavigationPropertyLink(navigationProperty, false)), true));
        }
      }
      foreach (StructuralTypeConfiguration derivedType in EdmTypeConfigurationExtensions.DerivedTypes(model, configuration.EntityType))
      {
        foreach (NavigationPropertyConfiguration navigationProperty2 in derivedType.NavigationProperties)
        {
          if (configuration.GetNavigationPropertyLink(navigationProperty2) == null)
            configuration.HasNavigationPropertyLink(navigationProperty2, new NavigationLinkBuilder((Func<ResourceContext, IEdmNavigationProperty, Uri>) ((entityContext, navigationProperty) => entityContext.GenerateNavigationPropertyLink(navigationProperty, true)), true));
        }
      }
    }
  }
}
