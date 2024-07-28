// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutResolverNamespace
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using System.Collections.Concurrent;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public sealed class LayoutResolverNamespace : LayoutResolver
  {
    private readonly ConcurrentDictionary<int, Layout> layoutCache;
    private readonly LayoutResolver parent;
    private readonly Namespace schemaNamespace;

    public LayoutResolverNamespace(Namespace schemaNamespace, LayoutResolver parent = null)
    {
      this.schemaNamespace = schemaNamespace;
      this.parent = parent;
      this.layoutCache = new ConcurrentDictionary<int, Layout>();
    }

    public Namespace Namespace => this.schemaNamespace;

    public override Layout Resolve(SchemaId schemaId)
    {
      Layout layout1;
      if (this.layoutCache.TryGetValue(schemaId.Id, out layout1))
        return layout1;
      foreach (Schema schema in this.schemaNamespace.Schemas)
      {
        if (schema.SchemaId == schemaId)
        {
          Layout layout2 = schema.Compile(this.schemaNamespace);
          return this.layoutCache.GetOrAdd(schemaId.Id, layout2);
        }
      }
      Layout layout3 = this.parent?.Resolve(schemaId);
      if (layout3 != null)
      {
        this.layoutCache.TryAdd(schemaId.Id, layout3);
        return layout3;
      }
      Contract.Fail(string.Format("Failed to resolve schema {0}", (object) schemaId));
      return (Layout) null;
    }
  }
}
