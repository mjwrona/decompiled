// Decompiled with JetBrains decompiler
// Type: Nest.RelationsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RelationsDescriptor : 
    IsADictionaryDescriptorBase<RelationsDescriptor, IRelations, RelationName, Children>
  {
    public RelationsDescriptor()
      : base((IRelations) new Relations())
    {
    }

    internal RelationsDescriptor(IRelations relations)
      : base(relations)
    {
    }

    public RelationsDescriptor Join(
      RelationName parent,
      RelationName child,
      params RelationName[] moreChildren)
    {
      return this.Assign(parent, new Children(child, moreChildren));
    }

    public RelationsDescriptor Join<TParent>(RelationName child, params RelationName[] moreChildren)
    {
      if (this.PromisedValue.ContainsKey((RelationName) typeof (TParent)))
        throw new ArgumentException(RelationsDescriptor.Message(typeof (TParent)));
      return this.Assign((RelationName) typeof (TParent), new Children(child, moreChildren));
    }

    public RelationsDescriptor Join<TParent, TChild>()
    {
      if (this.PromisedValue.ContainsKey((RelationName) typeof (TParent)))
        throw new ArgumentException(RelationsDescriptor.Message(typeof (TParent)));
      return this.Assign((RelationName) typeof (TParent), (Children) typeof (TChild));
    }

    private static string Message(Type t) => t.Name + " is already mapped. Use Join<TParent>(typeof(ChildA), typeof(ChildB), ..) to add multiple children in one go";
  }
}
