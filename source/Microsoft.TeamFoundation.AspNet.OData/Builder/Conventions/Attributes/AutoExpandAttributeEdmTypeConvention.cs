// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.AutoExpandAttributeEdmTypeConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class AutoExpandAttributeEdmTypeConvention : 
    AttributeEdmTypeConvention<StructuralTypeConfiguration>
  {
    public AutoExpandAttributeEdmTypeConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (AutoExpandAttribute)), false)
    {
    }

    public override void Apply(
      StructuralTypeConfiguration edmTypeConfiguration,
      ODataConventionModelBuilder model,
      Attribute attribute)
    {
      if (edmTypeConfiguration == null)
        throw Error.ArgumentNull(nameof (edmTypeConfiguration));
      if (model == null)
        throw Error.ArgumentNull(nameof (model));
      EntityTypeConfiguration typeConfiguration = edmTypeConfiguration as EntityTypeConfiguration;
      AutoExpandAttribute autoExpandAttribute = attribute as AutoExpandAttribute;
      foreach (NavigationPropertyConfiguration navigationProperty in typeConfiguration.NavigationProperties)
      {
        if (!navigationProperty.AddedExplicitly)
          navigationProperty.AutomaticallyExpand(autoExpandAttribute.DisableWhenSelectPresent);
      }
    }
  }
}
