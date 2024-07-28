// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PushNotificationDetails
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class PushNotificationDetails : GitNotification
  {
    private Dictionary<string, TfsGitRefUpdateResult> m_activeRefs;

    internal PushNotificationDetails(
      string teamProjectUri,
      Guid repositoryId,
      string repositoryName,
      IEnumerable<TfsGitRefUpdateResult> refUpdateResults,
      IEnumerable<Sha1Id> includedCommitIds,
      int pushId)
      : base(teamProjectUri, repositoryId, repositoryName)
    {
      this.RefUpdateResults = refUpdateResults;
      this.IncludedCommits = includedCommitIds;
      this.PushId = pushId;
    }

    public IEnumerable<TfsGitRefUpdateResult> RefUpdateResults { get; private set; }

    public IEnumerable<Sha1Id> IncludedCommits { get; private set; }

    public int PushId { get; private set; }

    [JsonIgnore]
    public Dictionary<string, TfsGitRefUpdateResult> ActiveRefs
    {
      get
      {
        if (this.m_activeRefs == null)
        {
          this.m_activeRefs = new Dictionary<string, TfsGitRefUpdateResult>();
          foreach (TfsGitRefUpdateResult refUpdateResult in this.RefUpdateResults)
          {
            if (refUpdateResult.Succeeded)
              this.m_activeRefs.Add(refUpdateResult.Name, refUpdateResult);
          }
        }
        return this.m_activeRefs;
      }
    }
  }
}
