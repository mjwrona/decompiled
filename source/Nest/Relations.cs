// Decompiled with JetBrains decompiler
// Type: Nest.Relations
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class Relations : 
    IsADictionaryBase<RelationName, Children>,
    IRelations,
    IIsADictionary<RelationName, Children>,
    IDictionary<RelationName, Children>,
    ICollection<KeyValuePair<RelationName, Children>>,
    IEnumerable<KeyValuePair<RelationName, Children>>,
    IEnumerable,
    IIsADictionary
  {
    public Relations()
    {
    }

    public Relations(IDictionary<RelationName, Children> container)
      : base(container)
    {
    }

    public Relations(Dictionary<RelationName, Children> container)
      : base((IDictionary<RelationName, Children>) container)
    {
    }

    public void Add(RelationName type, Children children)
    {
      if (this.BackingDictionary.ContainsKey(type))
        throw new ArgumentException(string.Format("{0} is already mapped as parent, you have to map all it's children as a single entry", (object) type));
      this.BackingDictionary.Add(type, children);
    }

    public void Add(RelationName type, RelationName child, params RelationName[] moreChildren)
    {
      if (this.BackingDictionary.ContainsKey(type))
        throw new ArgumentException(string.Format("{0} is already mapped as parent, you have to map all it's children as a single entry", (object) type));
      this.BackingDictionary.Add(type, new Children(child, moreChildren));
    }
  }
}
