// Decompiled with JetBrains decompiler
// Type: Nest.ElasticsearchCorePropertyAttributeBase
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
  public abstract class ElasticsearchCorePropertyAttributeBase : 
    ElasticsearchPropertyAttributeBase,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    protected ElasticsearchCorePropertyAttributeBase(FieldType type)
      : base(type)
    {
    }

    public string Similarity
    {
      set => this.Self.Similarity = value;
      get => this.Self.Similarity;
    }

    public bool Store
    {
      get => this.Self.Store.GetValueOrDefault();
      set => this.Self.Store = new bool?(value);
    }

    Fields ICoreProperty.CopyTo { get; set; }

    IProperties ICoreProperty.Fields { get; set; }

    private ICoreProperty Self => (ICoreProperty) this;

    string ICoreProperty.Similarity { get; set; }

    bool? ICoreProperty.Store { get; set; }

    public static ElasticsearchCorePropertyAttributeBase From(MemberInfo memberInfo) => memberInfo.GetCustomAttribute<ElasticsearchCorePropertyAttributeBase>(true);
  }
}
