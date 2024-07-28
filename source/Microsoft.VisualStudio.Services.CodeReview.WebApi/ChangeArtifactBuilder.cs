// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.ChangeArtifactBuilder
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  internal class ChangeArtifactBuilder
  {
    private readonly Dictionary<ChangeArtifact, ChangeEntry> m_artifacts;
    private readonly Dictionary<string, List<ChangeArtifact>> m_filePathArtifactsMap;
    private int m_highestChangeTrackingId;
    private int m_localLowestChangeTrackingId;

    internal ChangeArtifactBuilder()
    {
      this.m_filePathArtifactsMap = new Dictionary<string, List<ChangeArtifact>>((IEqualityComparer<string>) StringComparer.Ordinal);
      this.m_artifacts = new Dictionary<ChangeArtifact, ChangeEntry>();
      this.m_highestChangeTrackingId = 0;
      this.m_localLowestChangeTrackingId = 2147483646;
    }

    internal void BuildMap(IList<ChangeEntry> priorChangeEntries) => this.Append(priorChangeEntries, false, true);

    internal bool Compute(IList<ChangeEntry> changeEntries, bool continueIfMatchNotFound = true) => this.Append(changeEntries, true, continueIfMatchNotFound);

    private bool Append(
      IList<ChangeEntry> changeEntries,
      bool computeChangeTrackingId,
      bool continueIfMatchNotFound)
    {
      foreach (ChangeEntry changeEntry in (IEnumerable<ChangeEntry>) changeEntries)
      {
        string path1 = changeEntry.Base?.Path;
        string path2 = changeEntry.Modified?.Path;
        if (computeChangeTrackingId)
        {
          ChangeEntry matchingChangeEntry = this.GetMatchingChangeEntry(path1, path2, changeEntry.Type);
          if (matchingChangeEntry == null && !continueIfMatchNotFound)
            return false;
          changeEntry.ChangeTrackingId = this.ComputeChangeTrackingId(matchingChangeEntry);
        }
        else
        {
          ChangeArtifact changeArtifact = new ChangeArtifact(changeEntry.IterationId.Value, changeEntry.ChangeTrackingId);
          this.AddChangeEntryToArtifactMap(path1, changeArtifact, changeEntry);
          this.AddChangeEntryToArtifactMap(path2, changeArtifact, changeEntry);
        }
        this.m_highestChangeTrackingId = Math.Max(changeEntry.ChangeTrackingId, this.m_highestChangeTrackingId);
        if (changeEntry.ChangeTrackingId > 1073741823)
          this.m_localLowestChangeTrackingId = Math.Min(changeEntry.ChangeTrackingId, this.m_localLowestChangeTrackingId);
      }
      return true;
    }

    private ChangeEntry GetMatchingChangeEntry(
      string basePath,
      string modifiedPath,
      ChangeType changeType)
    {
      List<ChangeArtifact> changeArtifactList1 = new List<ChangeArtifact>();
      if (basePath != null && this.m_filePathArtifactsMap.ContainsKey(basePath))
        changeArtifactList1 = this.m_filePathArtifactsMap[basePath];
      List<ChangeArtifact> changeArtifactList2 = new List<ChangeArtifact>();
      if (modifiedPath != null && this.m_filePathArtifactsMap.ContainsKey(modifiedPath))
        changeArtifactList2 = this.m_filePathArtifactsMap[modifiedPath];
      ChangeEntry matchingChangeEntry = (ChangeEntry) null;
      int num1 = (changeType & ChangeType.Rename) != 0 ? 1 : 0;
      bool flag1 = (changeType & ChangeType.Move) != 0;
      bool flag2 = (changeType & ChangeType.Add) != 0;
      bool flag3 = (changeType & ChangeType.Delete) != 0;
      bool flag4 = (changeType & ChangeType.Edit) != 0;
      int num2 = flag1 ? 1 : 0;
      if ((num1 | num2) != 0)
      {
        foreach (ChangeArtifact key in changeArtifactList1)
        {
          matchingChangeEntry = this.m_artifacts[key];
          if (changeArtifactList2.Contains(key))
            return matchingChangeEntry;
        }
        return matchingChangeEntry;
      }
      if (flag2)
      {
        foreach (ChangeArtifact key in changeArtifactList2)
        {
          if (this.m_artifacts.ContainsKey(key))
          {
            matchingChangeEntry = this.m_artifacts[key];
            if (matchingChangeEntry.Base == null)
              return matchingChangeEntry;
          }
        }
        return matchingChangeEntry;
      }
      if (flag3)
      {
        foreach (ChangeArtifact key in changeArtifactList1)
        {
          if (this.m_artifacts.ContainsKey(key))
          {
            matchingChangeEntry = this.m_artifacts[key];
            if (matchingChangeEntry.Modified == null)
              return matchingChangeEntry;
          }
        }
        return matchingChangeEntry;
      }
      if (flag4)
      {
        foreach (ChangeArtifact key in changeArtifactList1)
        {
          if (changeArtifactList2.Contains(key))
            return this.m_artifacts[key];
        }
      }
      return (ChangeEntry) null;
    }

    private void AddChangeEntryToArtifactMap(
      string filePath,
      ChangeArtifact changeArtifact,
      ChangeEntry entry)
    {
      if (string.IsNullOrEmpty(filePath))
        return;
      if (!this.m_filePathArtifactsMap.ContainsKey(filePath))
      {
        this.m_filePathArtifactsMap.Add(filePath, new List<ChangeArtifact>()
        {
          changeArtifact
        });
      }
      else
      {
        List<ChangeArtifact> filePathArtifacts = this.m_filePathArtifactsMap[filePath];
        if (!filePathArtifacts.Contains(changeArtifact))
          filePathArtifacts.Add(changeArtifact);
      }
      if (this.m_artifacts.ContainsKey(changeArtifact))
        return;
      this.m_artifacts.Add(changeArtifact, entry);
    }

    private int ComputeChangeTrackingId(ChangeEntry storedChangeTrackinEntry)
    {
      if (storedChangeTrackinEntry != null)
        return storedChangeTrackinEntry.ChangeTrackingId;
      return this.m_highestChangeTrackingId != 2147483646 ? ++this.m_highestChangeTrackingId : --this.m_localLowestChangeTrackingId;
    }
  }
}
