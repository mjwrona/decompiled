// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.FilterAttributeEdmTypeConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query;
using System;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class FilterAttributeEdmTypeConvention : 
    AttributeEdmTypeConvention<StructuralTypeConfiguration>
  {
    public FilterAttributeEdmTypeConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (FilterAttribute)), true)
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
      if (edmTypeConfiguration.AddedExplicitly)
        return;
      FilterAttribute filterAttribute = attribute as FilterAttribute;
      ModelBoundQuerySettings settingsOrDefault = edmTypeConfiguration.QueryConfiguration.GetModelBoundQuerySettingsOrDefault();
      if (settingsOrDefault.FilterConfigurations.Count == 0)
      {
        settingsOrDefault.CopyFilterConfigurations(filterAttribute.FilterConfigurations);
      }
      else
      {
        foreach (string key in filterAttribute.FilterConfigurations.Keys)
          settingsOrDefault.FilterConfigurations[key] = filterAttribute.FilterConfigurations[key];
      }
      if (filterAttribute.FilterConfigurations.Count != 0)
        return;
      settingsOrDefault.DefaultEnableFilter = filterAttribute.DefaultEnableFilter;
    }
  }
}
