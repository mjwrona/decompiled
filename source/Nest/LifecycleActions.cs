// Decompiled with JetBrains decompiler
// Type: Nest.LifecycleActions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class LifecycleActions : 
    IsADictionaryBase<string, ILifecycleAction>,
    ILifecycleActions,
    IIsADictionary<string, ILifecycleAction>,
    IDictionary<string, ILifecycleAction>,
    ICollection<KeyValuePair<string, ILifecycleAction>>,
    IEnumerable<KeyValuePair<string, ILifecycleAction>>,
    IEnumerable,
    IIsADictionary
  {
    public LifecycleActions()
    {
    }

    public LifecycleActions(IDictionary<string, ILifecycleAction> container)
      : base(container)
    {
    }

    public void Add(IAllocateLifecycleAction action) => this.BackingDictionary.Add("allocate", (ILifecycleAction) action);

    public void Add(IDeleteLifecycleAction action) => this.BackingDictionary.Add("delete", (ILifecycleAction) action);

    public void Add(IForceMergeLifecycleAction action) => this.BackingDictionary.Add("forcemerge", (ILifecycleAction) action);

    public void Add(IFreezeLifecycleAction action) => this.BackingDictionary.Add("freeze", (ILifecycleAction) action);

    public void Add(IReadOnlyLifecycleAction action) => this.BackingDictionary.Add("readonly", (ILifecycleAction) action);

    public void Add(IRolloverLifecycleAction action) => this.BackingDictionary.Add("rollover", (ILifecycleAction) action);

    public void Add(ISearchableSnapshotAction action) => this.BackingDictionary.Add("searchable_snapshot", (ILifecycleAction) action);

    public void Add(ISetPriorityLifecycleAction action) => this.BackingDictionary.Add("set_priority", (ILifecycleAction) action);

    public void Add(IShrinkLifecycleAction action) => this.BackingDictionary.Add("shrink", (ILifecycleAction) action);

    public void Add(IUnfollowLifecycleAction action) => this.BackingDictionary.Add("unfollow", (ILifecycleAction) action);

    public void Add(IWaitForSnapshotLifecycleAction action) => this.BackingDictionary.Add("wait_for_snapshot", (ILifecycleAction) action);
  }
}
