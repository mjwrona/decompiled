// Decompiled with JetBrains decompiler
// Type: Nest.JoinAttribute
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class JoinAttribute : 
    ElasticsearchPropertyAttributeBase,
    IJoinProperty,
    IProperty,
    IFieldMapping
  {
    public JoinAttribute(
      RelationName parent,
      RelationName child,
      params RelationName[] moreChildren)
      : base(FieldType.Join)
    {
      this.Self.Relations = (IRelations) new Relations()
      {
        {
          parent,
          new Children(child, moreChildren)
        }
      };
    }

    public JoinAttribute(Type parent, Type child, params Type[] moreChildren)
      : base(FieldType.Join)
    {
      Children children1 = new Children();
      children1.Add((RelationName) child);
      Children children2 = children1;
      children2.AddRange(((IEnumerable<Type>) moreChildren).Select<Type, RelationName>((Func<Type, RelationName>) (c => (RelationName) c)));
      this.Self.Relations = (IRelations) new Relations()
      {
        {
          (RelationName) parent,
          children2
        }
      };
    }

    public JoinAttribute(string parent, string child, params string[] moreChildren)
      : base(FieldType.Join)
    {
      Children children1 = new Children();
      children1.Add((RelationName) child);
      Children children2 = children1;
      children2.AddRange(((IEnumerable<string>) moreChildren).Select<string, RelationName>((Func<string, RelationName>) (c => (RelationName) c)));
      this.Self.Relations = (IRelations) new Relations()
      {
        {
          (RelationName) parent,
          children2
        }
      };
    }

    IRelations IJoinProperty.Relations { get; set; }

    private IJoinProperty Self => (IJoinProperty) this;
  }
}
