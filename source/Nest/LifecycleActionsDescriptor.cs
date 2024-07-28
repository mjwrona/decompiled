// Decompiled with JetBrains decompiler
// Type: Nest.LifecycleActionsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class LifecycleActionsDescriptor : 
    IsADictionaryDescriptorBase<LifecycleActionsDescriptor, LifecycleActions, string, ILifecycleAction>
  {
    public LifecycleActionsDescriptor()
      : base(new LifecycleActions())
    {
    }

    public LifecycleActionsDescriptor Allocate(
      Func<AllocateLifecycleActionDescriptor, IAllocateLifecycleAction> selector)
    {
      return this.Assign("allocate", (ILifecycleAction) selector.InvokeOrDefault<AllocateLifecycleActionDescriptor, IAllocateLifecycleAction>(new AllocateLifecycleActionDescriptor()));
    }

    public LifecycleActionsDescriptor Delete(
      Func<DeleteLifecycleActionDescriptor, IDeleteLifecycleAction> selector)
    {
      return this.Assign("delete", (ILifecycleAction) selector.InvokeOrDefault<DeleteLifecycleActionDescriptor, IDeleteLifecycleAction>(new DeleteLifecycleActionDescriptor()));
    }

    public LifecycleActionsDescriptor ForceMerge(
      Func<ForceMergeLifecycleActionDescriptor, IForceMergeLifecycleAction> selector)
    {
      return this.Assign("forcemerge", (ILifecycleAction) selector.InvokeOrDefault<ForceMergeLifecycleActionDescriptor, IForceMergeLifecycleAction>(new ForceMergeLifecycleActionDescriptor()));
    }

    public LifecycleActionsDescriptor Freeze(
      Func<FreezeLifecycleActionDescriptor, IFreezeLifecycleAction> selector)
    {
      return this.Assign("freeze", (ILifecycleAction) selector.InvokeOrDefault<FreezeLifecycleActionDescriptor, IFreezeLifecycleAction>(new FreezeLifecycleActionDescriptor()));
    }

    public LifecycleActionsDescriptor ReadOnly(
      Func<ReadOnlyLifecycleActionDescriptor, IReadOnlyLifecycleAction> selector)
    {
      return this.Assign("readonly", (ILifecycleAction) selector.InvokeOrDefault<ReadOnlyLifecycleActionDescriptor, IReadOnlyLifecycleAction>(new ReadOnlyLifecycleActionDescriptor()));
    }

    public LifecycleActionsDescriptor Rollover(
      Func<RolloverLifecycleActionDescriptor, IRolloverLifecycleAction> selector)
    {
      return this.Assign("rollover", (ILifecycleAction) selector.InvokeOrDefault<RolloverLifecycleActionDescriptor, IRolloverLifecycleAction>(new RolloverLifecycleActionDescriptor()));
    }

    public LifecycleActionsDescriptor SearchableSnapshot(
      Func<SearchableSnapshotAction, ISearchableSnapshotAction> selector)
    {
      return this.Assign("searchable_snapshot", (ILifecycleAction) selector.InvokeOrDefault<SearchableSnapshotAction, ISearchableSnapshotAction>(new SearchableSnapshotAction()));
    }

    public LifecycleActionsDescriptor SetPriority(
      Func<SetPriorityLifecycleActionDescriptor, ISetPriorityLifecycleAction> selector)
    {
      return this.Assign("set_priority", (ILifecycleAction) selector.InvokeOrDefault<SetPriorityLifecycleActionDescriptor, ISetPriorityLifecycleAction>(new SetPriorityLifecycleActionDescriptor()));
    }

    public LifecycleActionsDescriptor Shrink(
      Func<ShrinkLifecycleActionDescriptor, IShrinkLifecycleAction> selector)
    {
      return this.Assign("shrink", (ILifecycleAction) selector.InvokeOrDefault<ShrinkLifecycleActionDescriptor, IShrinkLifecycleAction>(new ShrinkLifecycleActionDescriptor()));
    }

    public LifecycleActionsDescriptor Unfollow(
      Func<UnfollowLifecycleActionDescriptor, IUnfollowLifecycleAction> selector)
    {
      return this.Assign("unfollow", (ILifecycleAction) selector.InvokeOrDefault<UnfollowLifecycleActionDescriptor, IUnfollowLifecycleAction>(new UnfollowLifecycleActionDescriptor()));
    }

    public LifecycleActionsDescriptor WaitForSnapshot(
      Func<WaitForSnapshotLifecycleActionDescriptor, IWaitForSnapshotLifecycleAction> selector)
    {
      return this.Assign("wait_for_snapshot", (ILifecycleAction) selector.InvokeOrDefault<WaitForSnapshotLifecycleActionDescriptor, IWaitForSnapshotLifecycleAction>(new WaitForSnapshotLifecycleActionDescriptor()));
    }
  }
}
