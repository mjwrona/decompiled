// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.ExpandAttributeEdmTypeConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query;
using System;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class ExpandAttributeEdmTypeConvention : 
    AttributeEdmTypeConvention<StructuralTypeConfiguration>
  {
    public ExpandAttributeEdmTypeConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (ExpandAttribute)), true)
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
      ExpandAttribute expandAttribute = attribute as ExpandAttribute;
      ModelBoundQuerySettings settingsOrDefault = edmTypeConfiguration.QueryConfiguration.GetModelBoundQuerySettingsOrDefault();
      if (settingsOrDefault.ExpandConfigurations.Count == 0)
      {
        settingsOrDefault.CopyExpandConfigurations(expandAttribute.ExpandConfigurations);
      }
      else
      {
        foreach (string key in expandAttribute.ExpandConfigurations.Keys)
          settingsOrDefault.ExpandConfigurations[key] = expandAttribute.ExpandConfigurations[key];
      }
      if (expandAttribute.ExpandConfigurations.Count != 0)
        return;
      settingsOrDefault.DefaultExpandType = expandAttribute.DefaultExpandType;
      settingsOrDefault.DefaultMaxDepth = expandAttribute.DefaultMaxDepth ?? 2;
    }
  }
}
