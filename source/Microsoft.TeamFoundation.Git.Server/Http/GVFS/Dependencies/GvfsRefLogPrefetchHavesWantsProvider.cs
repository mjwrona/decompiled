// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Http.GVFS.Dependencies.GvfsRefLogPrefetchHavesWantsProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Http.GVFS.Dependencies
{
  internal class GvfsRefLogPrefetchHavesWantsProvider : IGvfsPrefetchHavesWantsProvider
  {
    private readonly IGvfsRefLogProvider m_refLog;

    public GvfsRefLogPrefetchHavesWantsProvider(IGvfsRefLogProvider refLog) => this.m_refLog = refLog;

    public (HashSet<Sha1Id> haves, HashSet<Sha1Id> wants) ReadHavesWantsSince(long timestamp)
    {
      IEnumerable<TfsGitRefLogEntry> source = this.m_refLog.ReadRefLogSince(timestamp);
      Dictionary<string, GvfsRefLogPrefetchHavesWantsProvider.HaveWant> dictionary = new Dictionary<string, GvfsRefLogPrefetchHavesWantsProvider.HaveWant>();
      foreach (TfsGitRefLogEntry tfsGitRefLogEntry in (IEnumerable<TfsGitRefLogEntry>) source.OrderBy<TfsGitRefLogEntry, int>((Func<TfsGitRefLogEntry, int>) (x => x.PushId)))
      {
        if (!dictionary.ContainsKey(tfsGitRefLogEntry.Name))
        {
          GvfsRefLogPrefetchHavesWantsProvider.HaveWant haveWant = new GvfsRefLogPrefetchHavesWantsProvider.HaveWant()
          {
            Have = tfsGitRefLogEntry.OldObjectId,
            Want = tfsGitRefLogEntry.NewObjectId
          };
          dictionary.Add(tfsGitRefLogEntry.Name, haveWant);
        }
        else
          dictionary[tfsGitRefLogEntry.Name].Want = tfsGitRefLogEntry.NewObjectId;
      }
      HashSet<Sha1Id> sha1IdSet1 = new HashSet<Sha1Id>(dictionary.Values.Select<GvfsRefLogPrefetchHavesWantsProvider.HaveWant, Sha1Id>((Func<GvfsRefLogPrefetchHavesWantsProvider.HaveWant, Sha1Id>) (x => x.Have)));
      sha1IdSet1.Remove(Sha1Id.Empty);
      HashSet<Sha1Id> sha1IdSet2 = new HashSet<Sha1Id>(dictionary.Values.Select<GvfsRefLogPrefetchHavesWantsProvider.HaveWant, Sha1Id>((Func<GvfsRefLogPrefetchHavesWantsProvider.HaveWant, Sha1Id>) (x => x.Want)));
      sha1IdSet2.Remove(Sha1Id.Empty);
      return (sha1IdSet1, sha1IdSet2);
    }

    private class HaveWant
    {
      public Sha1Id Have { get; set; }

      public Sha1Id Want { get; set; }
    }
  }
}
