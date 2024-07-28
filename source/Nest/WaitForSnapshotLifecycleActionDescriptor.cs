// Decompiled with JetBrains decompiler
// Type: Nest.WaitForSnapshotLifecycleActionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class WaitForSnapshotLifecycleActionDescriptor : 
    DescriptorBase<WaitForSnapshotLifecycleActionDescriptor, IWaitForSnapshotLifecycleAction>,
    IWaitForSnapshotLifecycleAction,
    ILifecycleAction
  {
    string IWaitForSnapshotLifecycleAction.Policy { get; set; }

    public WaitForSnapshotLifecycleActionDescriptor Policy(string policy) => this.Assign<string>(policy, (Action<IWaitForSnapshotLifecycleAction, string>) ((a, v) => a.Policy = policy));
  }
}
