// Decompiled with JetBrains decompiler
// Type: Nest.ElasticsearchPropertyAttributeBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Nest
{
  [AttributeUsage(AttributeTargets.Property)]
  [DataContract]
  public abstract class ElasticsearchPropertyAttributeBase : 
    Attribute,
    IProperty,
    IFieldMapping,
    IPropertyMapping,
    IJsonProperty
  {
    protected ElasticsearchPropertyAttributeBase(FieldType type) => this.Self.Type = type.GetStringValue();

    public bool? AllowPrivate { get; set; } = new bool?(true);

    public bool Ignore { get; set; }

    public string Name { get; set; }

    public int Order { get; } = -2;

    IDictionary<string, object> IProperty.LocalMetadata { get; set; }

    IDictionary<string, string> IProperty.Meta { get; set; }

    PropertyName IProperty.Name { get; set; }

    private IProperty Self => (IProperty) this;

    string IProperty.Type { get; set; }

    public static ElasticsearchPropertyAttributeBase From(MemberInfo memberInfo) => memberInfo.GetCustomAttribute<ElasticsearchPropertyAttributeBase>(true);
  }
}
