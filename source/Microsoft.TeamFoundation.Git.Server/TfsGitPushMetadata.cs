// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitPushMetadata
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitPushMetadata
  {
    public TfsGitPushMetadata(Guid repoId, int pushId, Guid pusherId, DateTime pushTime)
    {
      this.RepoId = repoId;
      this.PushId = pushId;
      this.PusherId = pusherId;
      this.PushTime = pushTime;
    }

    internal static void AssignRefLogsToMetadata(
      List<TfsGitPushMetadata> pushMetadata,
      IEnumerable<TfsGitRefLogEntry> refLogEntries)
    {
      Dictionary<string, List<TfsGitRefLogEntry>> dictionary = refLogEntries.GroupBy<TfsGitRefLogEntry, string>((Func<TfsGitRefLogEntry, string>) (rle => rle.RepositoryId.ToString() + rle.PushId.ToString())).ToDictionary<IGrouping<string, TfsGitRefLogEntry>, string, List<TfsGitRefLogEntry>>((Func<IGrouping<string, TfsGitRefLogEntry>, string>) (g => g.Key), (Func<IGrouping<string, TfsGitRefLogEntry>, List<TfsGitRefLogEntry>>) (g => g.ToList<TfsGitRefLogEntry>()));
      foreach (TfsGitPushMetadata tfsGitPushMetadata in pushMetadata)
      {
        string key = tfsGitPushMetadata.RepoId.ToString() + tfsGitPushMetadata.PushId.ToString();
        List<TfsGitRefLogEntry> tfsGitRefLogEntryList;
        if (dictionary.TryGetValue(key, out tfsGitRefLogEntryList))
          tfsGitPushMetadata.RefLog = tfsGitRefLogEntryList;
      }
    }

    public Guid RepoId { get; }

    public int PushId { get; }

    public Guid PusherId { get; }

    public DateTime PushTime { get; }

    public List<TfsGitRefLogEntry> RefLog { get; private set; }
  }
}
