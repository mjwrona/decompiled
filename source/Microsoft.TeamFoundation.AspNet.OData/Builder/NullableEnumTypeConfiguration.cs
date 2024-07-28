// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.NullableEnumTypeConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.AspNet.OData.Builder
{
  internal class NullableEnumTypeConfiguration : IEdmTypeConfiguration
  {
    internal NullableEnumTypeConfiguration(EnumTypeConfiguration enumTypeConfiguration)
    {
      this.ClrType = typeof (Nullable<>).MakeGenericType(enumTypeConfiguration.ClrType);
      this.FullName = enumTypeConfiguration.FullName;
      this.Namespace = enumTypeConfiguration.Namespace;
      this.Name = enumTypeConfiguration.Name;
      this.Kind = enumTypeConfiguration.Kind;
      this.ModelBuilder = enumTypeConfiguration.ModelBuilder;
      this.EnumTypeConfiguration = enumTypeConfiguration;
    }

    public Type ClrType { get; private set; }

    public string FullName { get; private set; }

    public string Namespace { get; private set; }

    public string Name { get; private set; }

    public EdmTypeKind Kind { get; private set; }

    public ODataModelBuilder ModelBuilder { get; private set; }

    internal EnumTypeConfiguration EnumTypeConfiguration { get; private set; }
  }
}
