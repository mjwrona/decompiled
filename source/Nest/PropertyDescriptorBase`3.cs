// Decompiled with JetBrains decompiler
// Type: Nest.PropertyDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public abstract class PropertyDescriptorBase<TDescriptor, TInterface, T> : 
    DescriptorBase<TDescriptor, TInterface>,
    IProperty,
    IFieldMapping
    where TDescriptor : PropertyDescriptorBase<TDescriptor, TInterface, T>, TInterface
    where TInterface : class, IProperty
    where T : class
  {
    private string _type;

    protected PropertyDescriptorBase(FieldType type) => this.Self.Type = type.GetStringValue();

    protected string DebugDisplay => "Type: " + (this.Self.Type ?? "<empty>") + ", Name: " + this.Self.Name.DebugDisplay + " ";

    protected string TypeOverride
    {
      set => this._type = value;
    }

    IDictionary<string, object> IProperty.LocalMetadata { get; set; }

    PropertyName IProperty.Name { get; set; }

    IDictionary<string, string> IProperty.Meta { get; set; }

    string IProperty.Type
    {
      get => this._type;
      set => this._type = value;
    }

    public TDescriptor Name(PropertyName name) => this.Assign<PropertyName>(name, (Action<TInterface, PropertyName>) ((a, v) => a.Name = v));

    public TDescriptor Name<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<TInterface, Expression<Func<T, TValue>>>) ((a, v) => a.Name = (PropertyName) (Expression) v));

    public TDescriptor LocalMetadata(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<TInterface, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.LocalMetadata = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }

    public TDescriptor Meta(
      Func<FluentDictionary<string, string>, FluentDictionary<string, string>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, string>, FluentDictionary<string, string>>>(selector, (Action<TInterface, Func<FluentDictionary<string, string>, FluentDictionary<string, string>>>) ((a, v) => a.Meta = v != null ? (IDictionary<string, string>) v(new FluentDictionary<string, string>()) : (IDictionary<string, string>) null));
    }
  }
}
