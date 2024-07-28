// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.MergeChange
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.Diff;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  internal class MergeChange : IMergeChange
  {
    private MergeChangeType m_changeType;
    private List<IDiffChange> m_modifiedChanges;
    private List<IDiffChange> m_latestChanges;
    private IDiffChange m_latestChange;
    private IDiffChange m_modifiedChange;

    public MergeChange(MergeChangeType changeType)
    {
      this.m_changeType = changeType;
      this.m_modifiedChanges = new List<IDiffChange>();
      this.m_latestChanges = new List<IDiffChange>();
    }

    public MergeChange(
      MergeChangeType changeType,
      IDiffChange modifiedChange,
      IDiffChange latestChange)
    {
      this.m_changeType = changeType;
      this.m_modifiedChanges = new List<IDiffChange>();
      this.m_latestChanges = new List<IDiffChange>();
      if (modifiedChange != null)
        this.m_modifiedChanges.Add(modifiedChange);
      if (latestChange == null)
        return;
      this.m_latestChanges.Add(latestChange);
    }

    public MergeChangeType ChangeType => this.m_changeType;

    public IDiffChange[] ModifiedChanges => this.m_modifiedChanges.ToArray();

    public IDiffChange[] LatestChanges => this.m_latestChanges.ToArray();

    public IDiffChange ModifiedChange
    {
      get
      {
        if (this.m_modifiedChange == null)
          this.m_modifiedChange = MergeChange.BuildConsolidatedChange(this.m_modifiedChanges);
        return this.m_modifiedChange;
      }
    }

    public IDiffChange LatestChange
    {
      get
      {
        if (this.m_latestChange == null)
          this.m_latestChange = MergeChange.BuildConsolidatedChange(this.m_latestChanges);
        return this.m_latestChange;
      }
    }

    public void AddModifiedChange(IDiffChange modifiedChange)
    {
      this.m_modifiedChanges.Add(modifiedChange);
      this.m_modifiedChange = (IDiffChange) null;
      if (this.m_latestChanges.Count <= 0)
        return;
      this.m_changeType = MergeChangeType.Conflict;
    }

    public void AddLatestChange(IDiffChange latestChange)
    {
      this.m_latestChanges.Add(latestChange);
      this.m_latestChange = (IDiffChange) null;
      if (this.m_modifiedChanges.Count <= 0)
        return;
      this.m_changeType = MergeChangeType.Conflict;
    }

    private static IDiffChange BuildConsolidatedChange(List<IDiffChange> diffChanges)
    {
      if (diffChanges.Count == 0)
        return (IDiffChange) null;
      if (diffChanges.Count == 1)
        return diffChanges[0];
      IDiffChange consolidatedDiffChange = diffChanges[0];
      diffChanges.ForEach((Action<IDiffChange>) (diffChange => consolidatedDiffChange = consolidatedDiffChange.Add(diffChange)));
      return consolidatedDiffChange;
    }
  }
}
