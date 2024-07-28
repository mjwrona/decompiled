// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.Attributes.AttributeEdmPropertyConvention`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder.Conventions.Attributes
{
  internal abstract class AttributeEdmPropertyConvention<TPropertyConfiguration> : 
    AttributeConvention,
    IEdmPropertyConvention<TPropertyConfiguration>,
    IEdmPropertyConvention,
    IConvention
    where TPropertyConfiguration : PropertyConfiguration
  {
    protected AttributeEdmPropertyConvention(
      Func<Attribute, bool> attributeFilter,
      bool allowMultiple)
      : base(attributeFilter, allowMultiple)
    {
    }

    public void Apply(
      PropertyConfiguration edmProperty,
      StructuralTypeConfiguration structuralTypeConfiguration,
      ODataConventionModelBuilder model)
    {
      if (edmProperty == null)
        throw Error.ArgumentNull(nameof (edmProperty));
      if (structuralTypeConfiguration == null)
        throw Error.ArgumentNull(nameof (structuralTypeConfiguration));
      if (!(edmProperty is TPropertyConfiguration edmProperty1))
        return;
      this.Apply(edmProperty1, structuralTypeConfiguration, model);
    }

    public void Apply(
      TPropertyConfiguration edmProperty,
      StructuralTypeConfiguration structuralTypeConfiguration,
      ODataConventionModelBuilder model)
    {
      if ((object) edmProperty == null)
        throw Error.ArgumentNull(nameof (edmProperty));
      if (structuralTypeConfiguration == null)
        throw Error.ArgumentNull(nameof (structuralTypeConfiguration));
      foreach (Attribute attribute in this.GetAttributes((MemberInfo) edmProperty.PropertyInfo))
        this.Apply(edmProperty, structuralTypeConfiguration, attribute, model);
    }

    public abstract void Apply(
      TPropertyConfiguration edmProperty,
      StructuralTypeConfiguration structuralTypeConfiguration,
      Attribute attribute,
      ODataConventionModelBuilder model);
  }
}
