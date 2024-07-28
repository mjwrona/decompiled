// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.PageAttributeEdmTypeConvention
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query;
using System;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal class PageAttributeEdmTypeConvention : 
    AttributeEdmTypeConvention<StructuralTypeConfiguration>
  {
    public PageAttributeEdmTypeConvention()
      : base((Func<Attribute, bool>) (attribute => attribute.GetType() == typeof (PageAttribute)), false)
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
      PageAttribute pageAttribute = attribute as PageAttribute;
      ModelBoundQuerySettings settingsOrDefault = edmTypeConfiguration.QueryConfiguration.GetModelBoundQuerySettingsOrDefault();
      settingsOrDefault.MaxTop = pageAttribute.MaxTop >= 0 ? new int?(pageAttribute.MaxTop) : new int?();
      if (pageAttribute.PageSize <= 0)
        return;
      settingsOrDefault.PageSize = new int?(pageAttribute.PageSize);
    }
  }
}
