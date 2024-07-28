// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.CountAttributeEdmPropertyConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query;
using System;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class CountAttributeEdmPropertyConvention : 
    AttributeEdmPropertyConvention<PropertyConfiguration>
  {
    public CountAttributeEdmPropertyConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (CountAttribute)), false)
    {
    }

    public override void Apply(
      PropertyConfiguration edmProperty,
      StructuralTypeConfiguration structuralTypeConfiguration,
      Attribute attribute,
      ODataConventionModelBuilder model)
    {
      if (edmProperty == null)
        throw Error.ArgumentNull(nameof (edmProperty));
      if (edmProperty.AddedExplicitly)
        return;
      if ((attribute as CountAttribute).Disabled)
        edmProperty.QueryConfiguration.GetModelBoundQuerySettingsOrDefault().Countable = new bool?(false);
      else
        edmProperty.QueryConfiguration.GetModelBoundQuerySettingsOrDefault().Countable = new bool?(true);
    }
  }
}
