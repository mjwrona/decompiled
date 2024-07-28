// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.ColumnAttributeEdmPropertyConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class ColumnAttributeEdmPropertyConvention : 
    AttributeEdmPropertyConvention<PropertyConfiguration>
  {
    public ColumnAttributeEdmPropertyConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (ColumnAttribute)), false)
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
      if (attribute is ColumnAttribute columnAttribute && columnAttribute.Order > 0)
        edmProperty.Order = columnAttribute.Order;
      if (!(edmProperty is PrimitivePropertyConfiguration property) || columnAttribute == null || columnAttribute.TypeName == null)
        return;
      string typeName = columnAttribute.TypeName;
      if (string.Compare(typeName, "date", StringComparison.OrdinalIgnoreCase) == 0)
      {
        property.AsDate();
      }
      else
      {
        if (string.Compare(typeName, "time", StringComparison.OrdinalIgnoreCase) != 0)
          return;
        property.AsTimeOfDay();
      }
    }
  }
}
