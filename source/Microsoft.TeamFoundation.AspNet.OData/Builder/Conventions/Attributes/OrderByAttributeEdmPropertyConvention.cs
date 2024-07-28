// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.OrderByAttributeEdmPropertyConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query;
using System;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class OrderByAttributeEdmPropertyConvention : 
    AttributeEdmPropertyConvention<PropertyConfiguration>
  {
    public OrderByAttributeEdmPropertyConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (OrderByAttribute)), true)
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
      OrderByAttribute orderByAttribute = attribute as OrderByAttribute;
      ModelBoundQuerySettings settingsOrDefault = edmProperty.QueryConfiguration.GetModelBoundQuerySettingsOrDefault();
      if (settingsOrDefault.OrderByConfigurations.Count == 0)
      {
        settingsOrDefault.CopyOrderByConfigurations(orderByAttribute.OrderByConfigurations);
      }
      else
      {
        foreach (string key in orderByAttribute.OrderByConfigurations.Keys)
          settingsOrDefault.OrderByConfigurations[key] = orderByAttribute.OrderByConfigurations[key];
      }
      if (orderByAttribute.OrderByConfigurations.Count != 0)
        return;
      settingsOrDefault.DefaultEnableOrderBy = orderByAttribute.DefaultEnableOrderBy;
    }
  }
}
