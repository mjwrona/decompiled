// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.MaxLengthAttributeEdmPropertyConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class MaxLengthAttributeEdmPropertyConvention : 
    AttributeEdmPropertyConvention<StructuralPropertyConfiguration>
  {
    public MaxLengthAttributeEdmPropertyConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (MaxLengthAttribute)), false)
    {
    }

    public override void Apply(
      StructuralPropertyConfiguration edmProperty,
      StructuralTypeConfiguration structuralTypeConfiguration,
      Attribute attribute,
      ODataConventionModelBuilder model)
    {
      if (edmProperty == null)
        throw Error.ArgumentNull(nameof (edmProperty));
      MaxLengthAttribute maxLengthAttribute = attribute as MaxLengthAttribute;
      if (!(edmProperty is LengthPropertyConfiguration propertyConfiguration) || maxLengthAttribute == null)
        return;
      propertyConfiguration.MaxLength = new int?(maxLengthAttribute.Length);
    }
  }
}
