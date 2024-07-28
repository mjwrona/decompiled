// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.DefaultValueAttributeEdmPropertyConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.ComponentModel;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class DefaultValueAttributeEdmPropertyConvention : 
    AttributeEdmPropertyConvention<PropertyConfiguration>
  {
    public DefaultValueAttributeEdmPropertyConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (DefaultValueAttribute)), false)
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
      DefaultValueAttribute defaultValueAttribute = attribute as DefaultValueAttribute;
      if (edmProperty.AddedExplicitly || defaultValueAttribute == null || defaultValueAttribute.Value == null)
        return;
      if (edmProperty.Kind == PropertyKind.Primitive)
        (edmProperty as PrimitivePropertyConfiguration).DefaultValueString = defaultValueAttribute.Value.ToString();
      if (edmProperty.Kind != PropertyKind.Enum)
        return;
      (edmProperty as EnumPropertyConfiguration).DefaultValueString = defaultValueAttribute.Value.ToString();
    }
  }
}
