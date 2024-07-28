// Decompiled with JetBrains decompiler
// Type: Nest.IndexNameResolver
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IndexNameResolver
  {
    private readonly IConnectionSettingsValues _connectionSettings;

    public IndexNameResolver(IConnectionSettingsValues connectionSettings)
    {
      connectionSettings.ThrowIfNull<IConnectionSettingsValues>(nameof (connectionSettings));
      this._connectionSettings = connectionSettings;
    }

    public string Resolve<T>() where T : class => this.Resolve(typeof (T));

    public string Resolve(IndexName i)
    {
      if (string.IsNullOrEmpty(i?.Name))
        return IndexNameResolver.PrefixClusterName(i, this.Resolve(i?.Type));
      IndexNameResolver.ValidateIndexName(i.Name);
      return IndexNameResolver.PrefixClusterName(i, i.Name);
    }

    public string Resolve(Type type)
    {
      string indexName = this._connectionSettings.DefaultIndex;
      FluentDictionary<Type, string> defaultIndices = this._connectionSettings.DefaultIndices;
      string str;
      if (defaultIndices != null && type != (Type) null && defaultIndices.TryGetValue(type, out str) && !string.IsNullOrEmpty(str))
        indexName = str;
      IndexNameResolver.ValidateIndexName(indexName);
      return indexName;
    }

    private static string PrefixClusterName(IndexName i, string name) => !i.Cluster.IsNullOrEmpty() ? i.Cluster + ":" + name : name;

    private static void ValidateIndexName(string indexName)
    {
      if (string.IsNullOrWhiteSpace(indexName))
        throw new ArgumentException("Index name is null for the given type and no default index is set. Map an index name using ConnectionSettings.DefaultMappingFor<TDocument>() or set a default index using ConnectionSettings.DefaultIndex().");
    }
  }
}
