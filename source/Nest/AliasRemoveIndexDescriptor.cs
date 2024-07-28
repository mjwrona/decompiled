// Decompiled with JetBrains decompiler
// Type: Nest.AliasRemoveIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class AliasRemoveIndexDescriptor : 
    DescriptorBase<AliasRemoveIndexDescriptor, IAliasRemoveIndexAction>,
    IAliasRemoveIndexAction,
    IAliasAction
  {
    public AliasRemoveIndexDescriptor() => this.Self.RemoveIndex = new AliasRemoveIndexOperation();

    AliasRemoveIndexOperation IAliasRemoveIndexAction.RemoveIndex { get; set; }

    public AliasRemoveIndexDescriptor Index(IndexName index)
    {
      this.Self.RemoveIndex.Index = index;
      return this;
    }

    public AliasRemoveIndexDescriptor Index(Type index)
    {
      this.Self.RemoveIndex.Index = (IndexName) index;
      return this;
    }

    public AliasRemoveIndexDescriptor Index<T>() where T : class
    {
      this.Self.RemoveIndex.Index = (IndexName) typeof (T);
      return this;
    }
  }
}
