// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ChangeListHelpers
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  public static class ChangeListHelpers
  {
    public static Dictionary<VersionControlChangeType, int> ComputeChangeCounts(
      IEnumerable<Change> changes)
    {
      Dictionary<VersionControlChangeType, int> changeCounts = new Dictionary<VersionControlChangeType, int>();
      foreach (VersionControlChangeType changeType in changes.Where<Change>((Func<Change, bool>) (change => !change.Item.IsFolder)).Select<Change, VersionControlChangeType>((Func<Change, VersionControlChangeType>) (change => change.ChangeType)))
        ChangeListHelpers.IncrementChangeCounts(changeCounts, changeType, 1);
      return changeCounts;
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

    private static void IncrementChangeFlagCount(
      Dictionary<VersionControlChangeType, int> changeCounts,
      VersionControlChangeType changeFlag,
      int count)
    {
      if (changeCounts.ContainsKey(changeFlag))
        changeCounts[changeFlag] += count;
      else
        changeCounts[changeFlag] = count;
    }
  }
}
