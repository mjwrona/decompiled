// Decompiled with JetBrains decompiler
// Type: Nest.MoveClusterRerouteCommandDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MoveClusterRerouteCommandDescriptor : 
    DescriptorBase<MoveClusterRerouteCommandDescriptor, IMoveClusterRerouteCommand>,
    IMoveClusterRerouteCommand,
    IClusterRerouteCommand
  {
    string IMoveClusterRerouteCommand.FromNode { get; set; }

    IndexName IMoveClusterRerouteCommand.Index { get; set; }

    string IClusterRerouteCommand.Name => "move";

    int? IMoveClusterRerouteCommand.Shard { get; set; }

    string IMoveClusterRerouteCommand.ToNode { get; set; }

    public MoveClusterRerouteCommandDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IMoveClusterRerouteCommand, IndexName>) ((a, v) => a.Index = v));

    public MoveClusterRerouteCommandDescriptor Index<T>() where T : class => this.Assign<Type>(typeof (T), (Action<IMoveClusterRerouteCommand, Type>) ((a, v) => a.Index = (IndexName) v));

    public MoveClusterRerouteCommandDescriptor Shard(int? shard) => this.Assign<int?>(shard, (Action<IMoveClusterRerouteCommand, int?>) ((a, v) => a.Shard = v));

    public MoveClusterRerouteCommandDescriptor FromNode(string fromNode) => this.Assign<string>(fromNode, (Action<IMoveClusterRerouteCommand, string>) ((a, v) => a.FromNode = v));

    public MoveClusterRerouteCommandDescriptor ToNode(string toNode) => this.Assign<string>(toNode, (Action<IMoveClusterRerouteCommand, string>) ((a, v) => a.ToNode = v));
  }
}
