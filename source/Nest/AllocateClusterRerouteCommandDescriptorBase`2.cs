// Decompiled with JetBrains decompiler
// Type: Nest.AllocateClusterRerouteCommandDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class AllocateClusterRerouteCommandDescriptorBase<TDescriptor, TInterface> : 
    DescriptorBase<TDescriptor, TInterface>,
    IAllocateClusterRerouteCommand,
    IClusterRerouteCommand
    where TDescriptor : AllocateClusterRerouteCommandDescriptorBase<TDescriptor, TInterface>, TInterface, IAllocateClusterRerouteCommand
    where TInterface : class, IAllocateClusterRerouteCommand
  {
    public abstract string Name { get; }

    IndexName IAllocateClusterRerouteCommand.Index { get; set; }

    string IClusterRerouteCommand.Name => this.Name;

    string IAllocateClusterRerouteCommand.Node { get; set; }

    int? IAllocateClusterRerouteCommand.Shard { get; set; }

    public TDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<TInterface, IndexName>) ((a, v) => a.Index = v));

    public TDescriptor Index<T>() where T : class => this.Assign<Type>(typeof (T), (Action<TInterface, Type>) ((a, v) => a.Index = (IndexName) v));

    public TDescriptor Shard(int? shard) => this.Assign<int?>(shard, (Action<TInterface, int?>) ((a, v) => a.Shard = v));

    public TDescriptor Node(string node) => this.Assign<string>(node, (Action<TInterface, string>) ((a, v) => a.Node = v));
  }
}
