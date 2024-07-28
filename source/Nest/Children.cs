// Decompiled with JetBrains decompiler
// Type: Nest.Children
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  [JsonFormatter(typeof (ChildrenFormatter))]
  public class Children : List<RelationName>
  {
    public Children()
    {
    }

    public Children(RelationName child, params RelationName[] moreChildren)
    {
      if (child != (RelationName) null)
        this.Add(child);
      if (moreChildren == null || moreChildren.Length == 0)
        return;
      this.AddRange((IEnumerable<RelationName>) moreChildren);
    }

    public static implicit operator Children(RelationName child)
    {
      if (child == (RelationName) null)
        return (Children) null;
      Children children = new Children();
      children.Add(child);
      return children;
    }

    public static implicit operator Children(Type type)
    {
      if (type == (Type) null)
        return (Children) null;
      Children children = new Children();
      children.Add((RelationName) type);
      return children;
    }

    public static implicit operator Children(string type)
    {
      if (type == null)
        return (Children) null;
      Children children = new Children();
      children.Add((RelationName) type);
      return children;
    }
  }
}
