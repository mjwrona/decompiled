// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.DataMemberAttributeEdmPropertyConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class DataMemberAttributeEdmPropertyConvention : 
    AttributeEdmPropertyConvention<PropertyConfiguration>
  {
    public DataMemberAttributeEdmPropertyConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (DataMemberAttribute)), false)
    {
    }

    public override void Apply(
      PropertyConfiguration edmProperty,
      StructuralTypeConfiguration structuralTypeConfiguration,
      Attribute attribute,
      ODataConventionModelBuilder model)
    {
      if (structuralTypeConfiguration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (structuralTypeConfiguration));
      if (edmProperty == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmProperty));
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      int num = ((IEnumerable<object>) TypeHelper.AsMemberInfo(structuralTypeConfiguration.ClrType).GetCustomAttributes(typeof (DataContractAttribute), true)).Any<object>() ? 1 : 0;
      DataMemberAttribute dataMemberAttribute = attribute as DataMemberAttribute;
      if (num == 0 || dataMemberAttribute == null || edmProperty.AddedExplicitly)
        return;
      if (model.ModelAliasingEnabled && !string.IsNullOrWhiteSpace(dataMemberAttribute.Name))
        edmProperty.Name = dataMemberAttribute.Name;
      if (edmProperty is StructuralPropertyConfiguration propertyConfiguration1)
        propertyConfiguration1.OptionalProperty = !dataMemberAttribute.IsRequired;
      if (!(edmProperty is NavigationPropertyConfiguration propertyConfiguration2) || propertyConfiguration2.Multiplicity == EdmMultiplicity.Many)
        return;
      if (dataMemberAttribute.IsRequired)
        propertyConfiguration2.Required();
      else
        propertyConfiguration2.Optional();
    }
  }
}
