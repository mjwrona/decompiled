// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutResolverSimple
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using System;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public sealed class LayoutResolverSimple : LayoutResolver
  {
    private readonly Func<SchemaId, Layout> resolver;

    public LayoutResolverSimple(Func<SchemaId, Layout> resolver) => this.resolver = resolver;

    public override Layout Resolve(SchemaId schemaId) => this.resolver(schemaId);
  }
}
