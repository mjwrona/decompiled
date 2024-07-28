// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.SelectAttributeEdmTypeConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query;
using System;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class SelectAttributeEdmTypeConvention : 
    AttributeEdmTypeConvention<StructuralTypeConfiguration>
  {
    public SelectAttributeEdmTypeConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (SelectAttribute)), true)
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
      SelectAttribute selectAttribute = attribute as SelectAttribute;
      ModelBoundQuerySettings settingsOrDefault = edmTypeConfiguration.QueryConfiguration.GetModelBoundQuerySettingsOrDefault();
      if (settingsOrDefault.SelectConfigurations.Count == 0)
      {
        settingsOrDefault.CopySelectConfigurations(selectAttribute.SelectConfigurations);
      }
      else
      {
        foreach (string key in selectAttribute.SelectConfigurations.Keys)
          settingsOrDefault.SelectConfigurations[key] = selectAttribute.SelectConfigurations[key];
      }
      if (selectAttribute.SelectConfigurations.Count != 0)
        return;
      settingsOrDefault.DefaultSelectType = selectAttribute.DefaultSelectType;
    }
  }
}
