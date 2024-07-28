// Decompiled with JetBrains decompiler
// Type: Nest.AliasRemoveDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class AliasRemoveDescriptor : 
    DescriptorBase<AliasRemoveDescriptor, IAliasRemoveAction>,
    IAliasRemoveAction,
    IAliasAction
  {
    public AliasRemoveDescriptor() => this.Self.Remove = new AliasRemoveOperation();

    AliasRemoveOperation IAliasRemoveAction.Remove { get; set; }

    public AliasRemoveDescriptor Index(string index)
    {
      this.Self.Remove.Index = (IndexName) index;
      return this;
    }

    public AliasRemoveDescriptor Index(Type index)
    {
      this.Self.Remove.Index = (IndexName) index;
      return this;
    }

    public AliasRemoveDescriptor Index<T>() where T : class
    {
      this.Self.Remove.Index = (IndexName) typeof (T);
      return this;
    }

    public AliasRemoveDescriptor Indices(Nest.Indices indices)
    {
      this.Self.Remove.Indices = indices;
      return this;
    }

    public AliasRemoveDescriptor Alias(string alias)
    {
      this.Self.Remove.Alias = alias;
      return this;
    }

    public AliasRemoveDescriptor Aliases(IEnumerable<string> aliases)
    {
      this.Self.Remove.Aliases = aliases;
      return this;
    }

    public AliasRemoveDescriptor Aliases(params string[] aliases)
    {
      this.Self.Remove.Aliases = (IEnumerable<string>) aliases;
      return this;
    }
  }
}
