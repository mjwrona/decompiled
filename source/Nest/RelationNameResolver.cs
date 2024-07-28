// Decompiled with JetBrains decompiler
// Type: Nest.RelationNameResolver
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Concurrent;

namespace Nest
{
  public class RelationNameResolver
  {
    private readonly IConnectionSettingsValues _connectionSettings;
    private readonly ConcurrentDictionary<Type, string> _relationNames = new ConcurrentDictionary<Type, string>();

    public RelationNameResolver(IConnectionSettingsValues connectionSettings)
    {
      connectionSettings.ThrowIfNull<IConnectionSettingsValues>(nameof (connectionSettings));
      this._connectionSettings = connectionSettings;
    }

    public string Resolve<T>() where T : class => this.Resolve((RelationName) typeof (T));

    public string Resolve(RelationName t) => t?.Name ?? this.ResolveType(t?.Type);

    private string ResolveType(Type type)
    {
      if (type == (Type) null)
        return (string) null;
      string str;
      if (this._relationNames.TryGetValue(type, out str))
        return str;
      if (this._connectionSettings.DefaultRelationNames.TryGetValue(type, out str))
      {
        this._relationNames.TryAdd(type, str);
        return str;
      }
      ElasticsearchTypeAttribute elasticsearchTypeAttribute = ElasticsearchTypeAttribute.From(type);
      str = elasticsearchTypeAttribute == null || elasticsearchTypeAttribute.RelationName.IsNullOrEmpty() ? type.Name.ToLowerInvariant() : elasticsearchTypeAttribute.RelationName;
      this._relationNames.TryAdd(type, str);
      return str;
    }
  }
}
