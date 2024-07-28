// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CommitIdSet
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class CommitIdSet : IReadOnlyCollection<Sha1Id>, IEnumerable<Sha1Id>, IEnumerable
  {
    private Sha1Id m_lastAdded;
    private Sha1Id m_oldest;
    private DateTime m_oldestTime;
    private readonly HashSet<Sha1Id> m_objectIds;

    public CommitIdSet()
    {
      this.m_lastAdded = Sha1Id.Empty;
      this.m_oldest = Sha1Id.Empty;
      this.m_oldestTime = DateTime.MaxValue;
      this.m_objectIds = new HashSet<Sha1Id>();
    }

    public void Add(Sha1Id toAdd, DateTime commitTimeIfCommit)
    {
      this.m_objectIds.Add(toAdd);
      this.m_lastAdded = toAdd;
      if (!(commitTimeIfCommit < this.m_oldestTime))
        return;
      this.m_oldestTime = commitTimeIfCommit;
      this.m_oldest = toAdd;
    }

    public Sha1Id LastAdded
    {
      get
      {
        if (this.Count == 0)
          throw new InvalidOperationException(Resources.Get("CommitIdSetEmpty"));
        return this.m_lastAdded;
      }
    }

    public Sha1Id Oldest
    {
      get
      {
        if (this.Count == 0)
          throw new InvalidOperationException(Resources.Get("CommitIdSetEmpty"));
        return this.m_oldest;
      }
    }

    public DateTime OldestCommitTime
    {
      get
      {
        if (this.Count == 0)
          throw new InvalidOperationException(Resources.Get("CommitIdSetEmpty"));
        return this.m_oldestTime;
      }
    }

    public int Count => this.m_objectIds.Count;

    internal HashSet<Sha1Id> InternalSet => this.m_objectIds;

    public void Clear()
    {
      this.m_objectIds.Clear();
      this.m_lastAdded = Sha1Id.Empty;
      this.m_oldest = Sha1Id.Empty;
      this.m_oldestTime = DateTime.MaxValue;
    }

    public bool Contains(Sha1Id toCheck) => this.m_objectIds.Contains(toCheck);

    public IEnumerator<Sha1Id> GetEnumerator() => (IEnumerator<Sha1Id>) this.m_objectIds.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_objectIds.GetEnumerator();
  }
}
