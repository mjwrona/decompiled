// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.RequiredAttributeEdmPropertyConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class RequiredAttributeEdmPropertyConvention : 
    AttributeEdmPropertyConvention<PropertyConfiguration>
  {
    public RequiredAttributeEdmPropertyConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (RequiredAttribute)), false)
    {
    }

    public override void Apply(
      PropertyConfiguration edmProperty,
      StructuralTypeConfiguration structuralTypeConfiguration,
      Attribute attribute,
      ODataConventionModelBuilder model)
    {
      if (edmProperty == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmProperty));
      if (edmProperty.AddedExplicitly)
        return;
      if (edmProperty is StructuralPropertyConfiguration propertyConfiguration1)
        propertyConfiguration1.OptionalProperty = false;
      if (!(edmProperty is NavigationPropertyConfiguration propertyConfiguration2) || propertyConfiguration2.Multiplicity == EdmMultiplicity.Many)
        return;
      propertyConfiguration2.Required();
    }
  }
}
