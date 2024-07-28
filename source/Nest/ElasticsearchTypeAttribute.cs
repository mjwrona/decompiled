// Decompiled with JetBrains decompiler
// Type: Nest.ElasticsearchTypeAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class ElasticsearchTypeAttribute : Attribute
  {
    private static readonly ConcurrentDictionary<Type, ElasticsearchTypeAttribute> CachedTypeLookups = new ConcurrentDictionary<Type, ElasticsearchTypeAttribute>();

    public string IdProperty { get; set; }

    public string RelationName { get; set; }

    [Obsolete("Deprecated. Please use RelationName")]
    public string Name
    {
      get => this.RelationName;
      set => this.RelationName = value;
    }

    public static ElasticsearchTypeAttribute From(Type type)
    {
      ElasticsearchTypeAttribute elasticsearchTypeAttribute;
      if (ElasticsearchTypeAttribute.CachedTypeLookups.TryGetValue(type, out elasticsearchTypeAttribute))
        return elasticsearchTypeAttribute;
      object[] customAttributes = type.GetCustomAttributes(typeof (ElasticsearchTypeAttribute), true);
      if (((IEnumerable<object>) customAttributes).HasAny<object>())
        elasticsearchTypeAttribute = (ElasticsearchTypeAttribute) ((IEnumerable<object>) customAttributes).First<object>();
      ElasticsearchTypeAttribute.CachedTypeLookups.TryAdd(type, elasticsearchTypeAttribute);
      return elasticsearchTypeAttribute;
    }
  }
}
