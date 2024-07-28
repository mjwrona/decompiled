// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.AttributeEdmTypeConvention`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal abstract class AttributeEdmTypeConvention<TEdmTypeConfiguration> : 
    AttributeConvention,
    IEdmTypeConvention,
    IConvention
    where TEdmTypeConfiguration : class, IEdmTypeConfiguration
  {
    protected AttributeEdmTypeConvention(Func<Attribute, bool> attributeFilter, bool allowMultiple)
      : base(attributeFilter, allowMultiple)
    {
    }

    public void Apply(IEdmTypeConfiguration edmTypeConfiguration, ODataConventionModelBuilder model)
    {
      if (!(edmTypeConfiguration is TEdmTypeConfiguration edmTypeConfiguration1))
        return;
      this.Apply(edmTypeConfiguration1, model);
    }

    public void Apply(TEdmTypeConfiguration edmTypeConfiguration, ODataConventionModelBuilder model)
    {
      if ((object) edmTypeConfiguration == null)
        throw Error.ArgumentNull(nameof (edmTypeConfiguration));
      foreach (Attribute attribute in this.GetAttributes(TypeHelper.AsMemberInfo(edmTypeConfiguration.ClrType)))
        this.Apply(edmTypeConfiguration, model, attribute);
    }

    public abstract void Apply(
      TEdmTypeConfiguration edmTypeConfiguration,
      ODataConventionModelBuilder model,
      Attribute attribute);
  }
}
