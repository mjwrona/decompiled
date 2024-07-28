// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitCommitFilters
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class TfsGitCommitFilters
  {
    public static IEnumerable<TfsGitCommit> FilterByAuthor(
      this IEnumerable<TfsGitCommit> results,
      string author)
    {
      return results.Where<TfsGitCommit>((Func<TfsGitCommit, bool>) (commit => commit.GetAuthor().NameAndEmail.IndexOf(author, StringComparison.CurrentCultureIgnoreCase) >= 0));
    }

    public static IEnumerable<TfsGitCommit> FilterByCommitter(
      this IEnumerable<TfsGitCommit> results,
      string committer)
    {
      return results.Where<TfsGitCommit>((Func<TfsGitCommit, bool>) (commit => commit.GetCommitter().NameAndEmail.IndexOf(committer, StringComparison.CurrentCultureIgnoreCase) >= 0));
    }

    public static IEnumerable<TfsGitCommit> FilterByDateRange(
      this IEnumerable<TfsGitCommit> results,
      DateTime? fromDate,
      DateTime? toDate,
      int maxAfterLastComparison = 2147483647)
    {
      if (!fromDate.HasValue)
        fromDate = new DateTime?(DateTime.MinValue);
      if (!toDate.HasValue)
        toDate = new DateTime?(DateTime.MaxValue);
      DateTime? nullable1 = fromDate;
      DateTime? nullable2 = toDate;
      if ((nullable1.HasValue & nullable2.HasValue ? (nullable1.GetValueOrDefault() > nullable2.GetValueOrDefault() ? 1 : 0) : 0) == 0)
      {
        int numberSinceLastComparison = 0;
        foreach (TfsGitCommit result in results)
        {
          DateTime time = result.GetCommitter().Time;
          DateTime dateTime1 = time;
          nullable2 = fromDate;
          if ((nullable2.HasValue ? (dateTime1 < nullable2.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          {
            ++numberSinceLastComparison;
            if (numberSinceLastComparison >= maxAfterLastComparison)
              break;
          }
          else
          {
            numberSinceLastComparison = 0;
            DateTime dateTime2 = time;
            nullable2 = toDate;
            if ((nullable2.HasValue ? (dateTime2 <= nullable2.GetValueOrDefault() ? 1 : 0) : 0) != 0)
              yield return result;
          }
        }
      }
    }

    public static IEnumerable<GitLogEntry> FilterBySha1IdRange(
      this IEnumerable<GitLogEntry> results,
      Sha1Id? fromId,
      Sha1Id? toId)
    {
      return fromId.HasValue ? (toId.HasValue ? results.Where<GitLogEntry>((Func<GitLogEntry, bool>) (entry => entry.Commit.ObjectId.CompareTo(fromId.Value) >= 0 && entry.Commit.ObjectId.CompareTo(toId.Value) <= 0)) : results.Where<GitLogEntry>((Func<GitLogEntry, bool>) (entry => entry.Commit.ObjectId.CompareTo(fromId.Value) >= 0))) : (toId.HasValue ? results.Where<GitLogEntry>((Func<GitLogEntry, bool>) (entry => entry.Commit.ObjectId.CompareTo(toId.Value) <= 0)) : results);
    }
  }
}
