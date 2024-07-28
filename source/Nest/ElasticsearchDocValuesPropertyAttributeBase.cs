// Decompiled with JetBrains decompiler
// Type: Nest.ElasticsearchDocValuesPropertyAttributeBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Nest
{
  [AttributeUsage(AttributeTargets.Property)]
  [DataContract]
  public abstract class ElasticsearchDocValuesPropertyAttributeBase : 
    ElasticsearchCorePropertyAttributeBase,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    protected ElasticsearchDocValuesPropertyAttributeBase(FieldType type)
      : base(type)
    {
    }

    public bool DocValues
    {
      get => this.Self.DocValues.GetValueOrDefault(true);
      set => this.Self.DocValues = new bool?(value);
    }

    bool? IDocValuesProperty.DocValues { get; set; }

    private IDocValuesProperty Self => (IDocValuesProperty) this;

    public static ElasticsearchDocValuesPropertyAttributeBase From(MemberInfo memberInfo) => memberInfo.GetCustomAttribute<ElasticsearchDocValuesPropertyAttributeBase>(true);
  }
}
