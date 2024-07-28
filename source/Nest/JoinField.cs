// Decompiled with JetBrains decompiler
// Type: Nest.JoinField
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  [JsonFormatter(typeof (JoinFieldFormatter))]
  public class JoinField
  {
    internal JoinField.Child ChildOption { get; }

    internal JoinField.Parent ParentOption { get; }

    internal int Tag { get; }

    public JoinField(JoinField.Parent parentName)
    {
      this.ParentOption = parentName;
      this.Tag = 0;
    }

    public JoinField(JoinField.Child child)
    {
      this.ChildOption = child;
      this.Tag = 1;
    }

    public static JoinField Root<TParent>() => (JoinField) new JoinField.Parent((RelationName) typeof (TParent));

    public static JoinField Root(RelationName parent) => (JoinField) new JoinField.Parent(parent);

    public static JoinField Link(RelationName child, Id parentId) => (JoinField) new JoinField.Child(child, parentId);

    public static JoinField Link<TChild, TParentDocument>(TParentDocument parent) where TParentDocument : class => (JoinField) new JoinField.Child((RelationName) typeof (TChild), Id.From<TParentDocument>(parent));

    public static JoinField Link<TChild>(Id parentId) => (JoinField) new JoinField.Child((RelationName) typeof (TChild), parentId);

    public static implicit operator JoinField(JoinField.Parent parent) => new JoinField(parent);

    public static implicit operator JoinField(string parentName) => new JoinField(new JoinField.Parent((RelationName) parentName));

    public static implicit operator JoinField(Type parentType) => new JoinField(new JoinField.Parent((RelationName) parentType));

    public static implicit operator JoinField(JoinField.Child child) => new JoinField(child);

    public T Match<T>(Func<JoinField.Parent, T> first, Func<JoinField.Child, T> second)
    {
      switch (this.Tag)
      {
        case 0:
          return first(this.ParentOption);
        case 1:
          return second(this.ChildOption);
        default:
          throw new Exception(string.Format("Unrecognized tag value: {0}", (object) this.Tag));
      }
    }

    public void Match(Action<JoinField.Parent> first, Action<JoinField.Child> second)
    {
      switch (this.Tag)
      {
        case 0:
          first(this.ParentOption);
          break;
        case 1:
          second(this.ChildOption);
          break;
        default:
          throw new Exception(string.Format("Unrecognized tag value: {0}", (object) this.Tag));
      }
    }

    public class Parent
    {
      public Parent(RelationName name) => this.Name = name;

      public RelationName Name { get; }
    }

    public class Child
    {
      public Child(RelationName name, Id parent)
      {
        this.Name = name;
        this.ParentId = parent;
      }

      public RelationName Name { get; }

      public Id ParentId { get; }
    }
  }
}
