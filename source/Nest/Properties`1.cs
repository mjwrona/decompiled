// Decompiled with JetBrains decompiler
// Type: Nest.Properties`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class Properties<T> : 
    IsADictionaryBase<PropertyName, IProperty>,
    IProperties,
    IIsADictionary<PropertyName, IProperty>,
    IDictionary<PropertyName, IProperty>,
    ICollection<KeyValuePair<PropertyName, IProperty>>,
    IEnumerable<KeyValuePair<PropertyName, IProperty>>,
    IEnumerable,
    IIsADictionary
  {
    public Properties()
    {
    }

    public Properties(IDictionary<PropertyName, IProperty> container)
      : base(container)
    {
    }

    public Properties(IProperties properties)
      : base((IDictionary<PropertyName, IProperty>) properties)
    {
    }

    public Properties(Dictionary<PropertyName, IProperty> container)
      : base((IDictionary<PropertyName, IProperty>) container)
    {
    }

    public void Add(PropertyName name, IProperty property) => this.BackingDictionary.Add(name, property);

    public void Add<TValue>(Expression<Func<T, TValue>> name, IProperty property) => this.BackingDictionary.Add((PropertyName) (Expression) name, property);
  }
}
