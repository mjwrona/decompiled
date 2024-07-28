// Decompiled with JetBrains decompiler
// Type: Nest.CancelClusterRerouteCommandDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class CancelClusterRerouteCommandDescriptor : 
    DescriptorBase<CancelClusterRerouteCommandDescriptor, ICancelClusterRerouteCommand>,
    ICancelClusterRerouteCommand,
    IClusterRerouteCommand
  {
    bool? ICancelClusterRerouteCommand.AllowPrimary { get; set; }

    IndexName ICancelClusterRerouteCommand.Index { get; set; }

    string IClusterRerouteCommand.Name => "cancel";

    string ICancelClusterRerouteCommand.Node { get; set; }

    int? ICancelClusterRerouteCommand.Shard { get; set; }

    public CancelClusterRerouteCommandDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<ICancelClusterRerouteCommand, IndexName>) ((a, v) => a.Index = v));

    public CancelClusterRerouteCommandDescriptor Index<T>() where T : class => this.Assign<Type>(typeof (T), (Action<ICancelClusterRerouteCommand, Type>) ((a, v) => a.Index = (IndexName) v));

    public CancelClusterRerouteCommandDescriptor Shard(int? shard) => this.Assign<int?>(shard, (Action<ICancelClusterRerouteCommand, int?>) ((a, v) => a.Shard = v));

    public CancelClusterRerouteCommandDescriptor Node(string node) => this.Assign<string>(node, (Action<ICancelClusterRerouteCommand, string>) ((a, v) => a.Node = v));

    public CancelClusterRerouteCommandDescriptor AllowPrimary(bool? allowPrimary = true) => this.Assign<bool?>(allowPrimary, (Action<ICancelClusterRerouteCommand, bool?>) ((a, v) => a.AllowPrimary = v));
  }
}
