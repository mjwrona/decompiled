// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendingMergeCollection
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PendingMergeCollection : 
    IEnumerable<PendingMerge>,
    IEnumerable,
    IEnumerator<PendingMerge>,
    IDisposable,
    IEnumerator
  {
    private List<PendingChange> m_pendingChanges;
    private int m_index = -1;

    public PendingMergeCollection(List<PendingChange> pendingChanges) => this.m_pendingChanges = pendingChanges;

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerator<PendingMerge> GetEnumerator() => (IEnumerator<PendingMerge>) this;

    public bool MoveNext()
    {
      if (this.m_index >= this.m_pendingChanges.Count * 2)
        return false;
      ++this.m_index;
      if (this.m_index % 2 == 1 && this.m_pendingChanges[this.m_index / 2].Version == 0)
        ++this.m_index;
      return this.m_index < this.m_pendingChanges.Count * 2;
    }

    public void Reset() => this.m_index = -1;

    object IEnumerator.Current => (object) this.Current;

    public PendingMerge Current
    {
      get
      {
        if (this.m_index >= this.m_pendingChanges.Count * 2)
          throw new InvalidOperationException();
        PendingChange pendingChange = this.m_pendingChanges[this.m_index / 2];
        PendingMerge current = new PendingMerge();
        current.WorkspaceId = pendingChange.pendingSet.workspaceId;
        current.IsCommitted = pendingChange.Version > 0;
        if (this.m_index % 2 == 1)
        {
          current.TargetServerItem = pendingChange.SourceServerItem ?? pendingChange.ServerItem;
          current.IsRenameSource = true;
        }
        else
        {
          current.TargetServerItem = pendingChange.ServerItem;
          current.IsRenameSource = false;
        }
        return current;
      }
    }

    public void Dispose()
    {
    }
  }
}
