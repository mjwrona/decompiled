// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.DataContractAttributeEnumTypeConvention
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
  internal class DataContractAttributeEnumTypeConvention : 
    AttributeEdmTypeConvention<EnumTypeConfiguration>
  {
    public DataContractAttributeEnumTypeConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (DataContractAttribute)), false)
    {
    }

    public override void Apply(
      EnumTypeConfiguration enumTypeConfiguration,
      ODataConventionModelBuilder model,
      Attribute attribute)
    {
      if (enumTypeConfiguration == null)
        throw Error.ArgumentNull(nameof (enumTypeConfiguration));
      if (model == null)
        throw Error.ArgumentNull(nameof (model));
      if (!enumTypeConfiguration.AddedExplicitly && model.ModelAliasingEnabled)
      {
        if (attribute is DataContractAttribute contractAttribute)
        {
          if (contractAttribute.Name != null)
            enumTypeConfiguration.Name = contractAttribute.Name;
          if (contractAttribute.Namespace != null)
            enumTypeConfiguration.Namespace = contractAttribute.Namespace;
        }
        enumTypeConfiguration.AddedExplicitly = false;
      }
      foreach (EnumMemberConfiguration memberConfiguration in (IEnumerable<EnumMemberConfiguration>) enumTypeConfiguration.Members.ToArray<EnumMemberConfiguration>())
      {
        EnumMemberAttribute enumMemberAttribute = ((IEnumerable<object>) enumTypeConfiguration.ClrType.GetField(memberConfiguration.Name).GetCustomAttributes(typeof (EnumMemberAttribute), true)).FirstOrDefault<object>() as EnumMemberAttribute;
        if (!memberConfiguration.AddedExplicitly)
        {
          if (model.ModelAliasingEnabled && enumMemberAttribute != null)
          {
            if (!string.IsNullOrWhiteSpace(enumMemberAttribute.Value))
              memberConfiguration.Name = enumMemberAttribute.Value;
          }
          else
            enumTypeConfiguration.RemoveMember(memberConfiguration.MemberInfo);
        }
      }
    }
  }
}
