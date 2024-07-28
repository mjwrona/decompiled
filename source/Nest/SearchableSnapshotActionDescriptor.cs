// Decompiled with JetBrains decompiler
// Type: Nest.SearchableSnapshotActionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SearchableSnapshotActionDescriptor : 
    DescriptorBase<SearchableSnapshotActionDescriptor, ISearchableSnapshotAction>,
    ISearchableSnapshotAction,
    ILifecycleAction
  {
    string ISearchableSnapshotAction.SnapshotRepository { get; set; }

    bool? ISearchableSnapshotAction.ForceMergeIndex { get; set; }

    public SearchableSnapshotActionDescriptor SnapshotRepository(string snapshotRespository) => this.Assign<string>(snapshotRespository, (Action<ISearchableSnapshotAction, string>) ((a, v) => a.SnapshotRepository = snapshotRespository));

    public SearchableSnapshotActionDescriptor ForceMergeIndex(bool? forceMergeIndex = true) => this.Assign<bool?>(forceMergeIndex, (Action<ISearchableSnapshotAction, bool?>) ((a, v) => a.ForceMergeIndex = forceMergeIndex));
  }
}
