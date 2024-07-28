// Decompiled with JetBrains decompiler
// Type: Nest.AliasAddDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class AliasAddDescriptor : 
    DescriptorBase<AliasAddDescriptor, IAliasAddAction>,
    IAliasAddAction,
    IAliasAction
  {
    public AliasAddDescriptor() => this.Self.Add = new AliasAddOperation();

    AliasAddOperation IAliasAddAction.Add { get; set; }

    public AliasAddDescriptor Index(string index)
    {
      this.Self.Add.Index = (IndexName) index;
      return this;
    }

    public AliasAddDescriptor Index(Type index)
    {
      this.Self.Add.Index = (IndexName) index;
      return this;
    }

    public AliasAddDescriptor Index<T>() where T : class
    {
      this.Self.Add.Index = (IndexName) typeof (T);
      return this;
    }

    public AliasAddDescriptor Indices(Nest.Indices indices)
    {
      this.Self.Add.Indices = indices;
      return this;
    }

    public AliasAddDescriptor Alias(string alias)
    {
      this.Self.Add.Alias = alias;
      return this;
    }

    public AliasAddDescriptor Aliases(IEnumerable<string> aliases)
    {
      this.Self.Add.Aliases = aliases;
      return this;
    }

    public AliasAddDescriptor Aliases(params string[] aliases)
    {
      this.Self.Add.Aliases = (IEnumerable<string>) aliases;
      return this;
    }

    public AliasAddDescriptor Routing(string routing)
    {
      this.Self.Add.Routing = routing;
      return this;
    }

    public AliasAddDescriptor IndexRouting(string indexRouting)
    {
      this.Self.Add.IndexRouting = indexRouting;
      return this;
    }

    public AliasAddDescriptor SearchRouting(string searchRouting)
    {
      this.Self.Add.SearchRouting = searchRouting;
      return this;
    }

    public AliasAddDescriptor IsWriteIndex(bool? isWriteIndex = true)
    {
      this.Self.Add.IsWriteIndex = isWriteIndex;
      return this;
    }

    public AliasAddDescriptor IsHidden(bool? isHidden = true)
    {
      this.Self.Add.IsHidden = isHidden;
      return this;
    }

    public AliasAddDescriptor Filter<T>(
      Func<QueryContainerDescriptor<T>, QueryContainer> filterSelector)
      where T : class
    {
      this.Self.Add.Filter = filterSelector != null ? filterSelector(new QueryContainerDescriptor<T>()) : (QueryContainer) null;
      return this;
    }
  }
}
