// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryMerges
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.VersionControl.Common.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryMerges : CommandQueryMergesWithDetails
  {
    private List<Changeset> m_changesets;
    private List<ChangesetMerge> m_mergedChangesets;

    public CommandQueryMerges(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public override void Execute(
      Workspace workspace,
      ItemSpec source,
      VersionSpec versionSource,
      ItemSpec target,
      VersionSpec versionTarget,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      int maxChangesets,
      bool showAll)
    {
      this.m_mergedChangesets = new List<ChangesetMerge>();
      base.Execute(workspace, source, versionSource, target, versionTarget, versionFrom, versionTo, maxChangesets, showAll);
    }

    public override void ContinueExecution()
    {
      if (this.m_state == CommandQueryMergesWithDetails.State.Complete)
        return;
      List<Changeset> items = this.m_results.GetCurrent<Changeset>().Items;
      this.m_results.NextResult();
      Set<long> set1 = new Set<long>();
      while (this.m_results.GetCurrent<ItemMerge>().MoveNext())
      {
        ItemMerge current = this.m_results.GetCurrent<ItemMerge>().Current;
        long num = (long) current.SourceVersionFrom << 32 | (long) current.TargetVersionFrom & (long) uint.MaxValue;
        if (!set1.Contains(num) && current.HasPermission(this.m_versionControlRequestContext))
        {
          set1.Add(num);
          this.m_mergedChangesets.Add(new ChangesetMerge(current));
          if (!this.m_validChangesets.ContainsKey(current.SourceVersionFrom))
            this.m_validChangesets.Add(current.SourceVersionFrom, current.SourceVersionFrom);
          if (!this.m_validChangesets.ContainsKey(current.TargetVersionFrom))
            this.m_validChangesets.Add(current.TargetVersionFrom, current.TargetVersionFrom);
          if (this.m_maxChangesets > 0 && this.m_mergedChangesets.Count >= this.m_maxChangesets)
            break;
        }
      }
      Set<int> set2 = new Set<int>();
      this.m_results.NextResult();
      while (this.m_results.GetCurrent<ItemMerge>().MoveNext())
      {
        ItemMerge current = this.m_results.GetCurrent<ItemMerge>().Current;
        if (!set2.Contains(current.SourceVersionFrom) && current.HasPermission(this.m_versionControlRequestContext))
        {
          set2.Add(current.SourceVersionFrom);
          for (int index = 0; index < this.m_mergedChangesets.Count; ++index)
          {
            if (current.SourceVersionFrom == this.m_mergedChangesets[index].SourceVersion)
            {
              this.m_mergedChangesets[index].Partial = true;
              break;
            }
          }
        }
      }
      this.m_changesets = new List<Changeset>(items.Count);
      foreach (Changeset changeset in items)
      {
        if (this.m_validChangesets.ContainsKey(changeset.ChangesetId))
        {
          changeset.LookupDisplayNames(this.m_versionControlRequestContext);
          this.m_changesets.Add(changeset);
        }
      }
      this.m_state = CommandQueryMergesWithDetails.State.Complete;
    }

    public List<Changeset> Changesets => this.m_changesets;

    public List<ChangesetMerge> MergedChangesets => this.m_mergedChangesets;

    public override ChangesetMergeDetails Details => throw new NotSupportedException();
  }
}
