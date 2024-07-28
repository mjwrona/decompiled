// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.NotMappedAttributeConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class NotMappedAttributeConvention : AttributeEdmPropertyConvention<PropertyConfiguration>
  {
    private const string EntityFrameworkNotMappedAttributeTypeName = "System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute";
    private static Func<Attribute, bool> _filter = (Func<Attribute, bool>) (attribute => attribute.GetType().FullName.Equals("System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute", StringComparison.Ordinal));

    public NotMappedAttributeConvention()
      : base(NotMappedAttributeConvention._filter, false)
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
      if (structuralTypeConfiguration == null)
        throw Error.ArgumentNull(nameof (structuralTypeConfiguration));
      if (edmProperty.AddedExplicitly)
        return;
      structuralTypeConfiguration.RemoveProperty(edmProperty.PropertyInfo);
    }
  }
}
