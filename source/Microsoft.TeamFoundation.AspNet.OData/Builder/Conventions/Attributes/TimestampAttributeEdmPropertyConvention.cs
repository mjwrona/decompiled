// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.TimestampAttributeEdmPropertyConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class TimestampAttributeEdmPropertyConvention : 
    AttributeEdmPropertyConvention<PropertyConfiguration>
  {
    public TimestampAttributeEdmPropertyConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (TimestampAttribute)), false)
    {
    }

    public override void Apply(
      PropertyConfiguration edmProperty,
      StructuralTypeConfiguration structuralTypeConfiguration,
      Attribute attribute,
      ODataConventionModelBuilder model)
    {
      if (!(structuralTypeConfiguration is EntityTypeConfiguration config))
        return;
      PrimitivePropertyConfiguration[] propertiesWithTimestamp = TimestampAttributeEdmPropertyConvention.GetPropertiesWithTimestamp(config);
      if (propertiesWithTimestamp.Length != 1)
        return;
      propertiesWithTimestamp[0].IsConcurrencyToken();
    }

    private static PrimitivePropertyConfiguration[] GetPropertiesWithTimestamp(
      EntityTypeConfiguration config)
    {
      return config.ThisAndBaseTypes().SelectMany<StructuralTypeConfiguration, PropertyConfiguration>((Func<StructuralTypeConfiguration, IEnumerable<PropertyConfiguration>>) (p => p.Properties)).OfType<PrimitivePropertyConfiguration>().Where<PrimitivePropertyConfiguration>((Func<PrimitivePropertyConfiguration, bool>) (pc => ((IEnumerable<object>) pc.PropertyInfo.GetCustomAttributes(typeof (TimestampAttribute), true)).Any<object>())).ToArray<PrimitivePropertyConfiguration>();
    }
  }
}
