// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.Conventions.IEdmPropertyConvention`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

namespace Microsoft.AspNet.OData.Builder.Conventions
{
  internal interface IEdmPropertyConvention<TPropertyConfiguration> : 
    IEdmPropertyConvention,
    IConvention
    where TPropertyConfiguration : PropertyConfiguration
  {
    void Apply(
      TPropertyConfiguration edmProperty,
      StructuralTypeConfiguration structuralTypeConfiguration,
      ODataConventionModelBuilder model);
  }
}
