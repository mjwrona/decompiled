// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.IgnoreDataMemberAttributeEdmPropertyConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class IgnoreDataMemberAttributeEdmPropertyConvention : 
    AttributeEdmPropertyConvention<PropertyConfiguration>
  {
    public IgnoreDataMemberAttributeEdmPropertyConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (IgnoreDataMemberAttribute)), false)
    {
    }

    public override void Apply(
      PropertyConfiguration edmProperty,
      StructuralTypeConfiguration structuralTypeConfiguration,
      Attribute attribute,
      ODataConventionModelBuilder model)
    {
      if (structuralTypeConfiguration == null)
        throw Error.ArgumentNull(nameof (structuralTypeConfiguration));
      if (edmProperty == null)
        throw Error.ArgumentNull(nameof (edmProperty));
      if (edmProperty.AddedExplicitly || ((IEnumerable<object>) TypeHelper.AsMemberInfo(structuralTypeConfiguration.ClrType).GetCustomAttributes(typeof (DataContractAttribute), true)).Any<object>() & ((IEnumerable<object>) edmProperty.PropertyInfo.GetCustomAttributes(typeof (DataMemberAttribute), true)).Any<object>())
        return;
      structuralTypeConfiguration.RemoveProperty(edmProperty.PropertyInfo);
    }
  }
}
