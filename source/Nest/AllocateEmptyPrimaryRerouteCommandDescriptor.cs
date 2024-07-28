// Decompiled with JetBrains decompiler
// Type: Nest.AllocateEmptyPrimaryRerouteCommandDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class AllocateEmptyPrimaryRerouteCommandDescriptor : 
    AllocateClusterRerouteCommandDescriptorBase<AllocateEmptyPrimaryRerouteCommandDescriptor, IAllocateEmptyPrimaryRerouteCommand>,
    IAllocateEmptyPrimaryRerouteCommand,
    IAllocateClusterRerouteCommand,
    IClusterRerouteCommand
  {
    public override string Name => "allocate_empty_primary";

    bool? IAllocateEmptyPrimaryRerouteCommand.AcceptDataLoss { get; set; }

    public AllocateEmptyPrimaryRerouteCommandDescriptor AcceptDataLoss(bool? acceptDataLoss = true) => this.Assign<bool?>(acceptDataLoss, (Action<IAllocateEmptyPrimaryRerouteCommand, bool?>) ((a, v) => a.AcceptDataLoss = v));
  }
}
