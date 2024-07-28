// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.ChangeListHelpers
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  public static class ChangeListHelpers
  {
    public static Dictionary<VersionControlChangeType, int> ComputeChangeCounts(
      IEnumerable<TfvcChange> changes)
    {
      return ChangeListHelpers.ComputeChangeCountsInternal<TfvcItem>((IEnumerable<Change<TfvcItem>>) changes);
    }

    public static Dictionary<VersionControlChangeType, int> ComputeChangeCounts(
      IEnumerable<GitChange> changes)
    {
      return ChangeListHelpers.ComputeChangeCountsInternal<GitItem>((IEnumerable<Change<GitItem>>) changes);
    }

    public static void IncrementChangeCounts(
      Dictionary<VersionControlChangeType, int> changeCounts,
      VersionControlChangeType changeType,
      int count)
    {
      if (changeType.HasFlag((Enum) VersionControlChangeType.Add))
        ChangeListHelpers.IncrementChangeFlagCount(changeCounts, VersionControlChangeType.Add, count);
      else if (changeType.HasFlag((Enum) VersionControlChangeType.Delete) && !changeType.HasFlag((Enum) VersionControlChangeType.SourceRename))
      {
        ChangeListHelpers.IncrementChangeFlagCount(changeCounts, VersionControlChangeType.Delete, count);
      }
      else
      {
        if (!changeType.HasFlag((Enum) VersionControlChangeType.Edit))
          return;
        ChangeListHelpers.IncrementChangeFlagCount(changeCounts, VersionControlChangeType.Edit, count);
      }
    }

    public static void IncrementChangeFlagCount(
      Dictionary<VersionControlChangeType, int> changeCounts,
      VersionControlChangeType changeFlag,
      int count)
    {
      if (changeCounts.ContainsKey(changeFlag))
        changeCounts[changeFlag] += count;
      else
        changeCounts[changeFlag] = count;
    }

    private static Dictionary<VersionControlChangeType, int> ComputeChangeCountsInternal<T>(
      IEnumerable<Change<T>> changes)
      where T : ItemModel
    {
      Dictionary<VersionControlChangeType, int> changeCounts = new Dictionary<VersionControlChangeType, int>();
      foreach (VersionControlChangeType changeType in changes.Where<Change<T>>((Func<Change<T>, bool>) (change => !change.Item.IsFolder)).Select<Change<T>, VersionControlChangeType>((Func<Change<T>, VersionControlChangeType>) (change => change.ChangeType)))
        ChangeListHelpers.IncrementChangeCounts(changeCounts, changeType, 1);
      return changeCounts;
    }
  }
}
