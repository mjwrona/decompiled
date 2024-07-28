// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.DataContractAttributeEdmTypeConvention
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
  internal class DataContractAttributeEdmTypeConvention : 
    AttributeEdmTypeConvention<StructuralTypeConfiguration>
  {
    public DataContractAttributeEdmTypeConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (DataContractAttribute)), false)
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
      if (!edmTypeConfiguration.AddedExplicitly && model.ModelAliasingEnabled)
      {
        if (attribute is DataContractAttribute contractAttribute)
        {
          if (contractAttribute.Name != null)
            edmTypeConfiguration.Name = contractAttribute.Name;
          if (contractAttribute.Namespace != null)
            edmTypeConfiguration.Namespace = contractAttribute.Namespace;
        }
        edmTypeConfiguration.AddedExplicitly = false;
      }
      foreach (PropertyConfiguration propertyConfiguration in (IEnumerable<PropertyConfiguration>) edmTypeConfiguration.Properties.ToArray<PropertyConfiguration>())
      {
        if (!((IEnumerable<object>) propertyConfiguration.PropertyInfo.GetCustomAttributes(typeof (DataMemberAttribute), true)).Any<object>() && !propertyConfiguration.AddedExplicitly)
          edmTypeConfiguration.RemoveProperty(propertyConfiguration.PropertyInfo);
      }
    }
  }
}
