// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.CommitDetails
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class CommitDetails
  {
    [DataMember]
    public string CommitId { get; set; }

    [DataMember]
    public string Committer { get; set; }

    [DataMember]
    public string CommitMessage { get; set; }

    [DataMember]
    public string CommitDiffUrl { get; set; }

    [DataMember]
    public DateTime CommitTime { get; set; }

    public CommitDetails()
    {
    }

    public CommitDetails(
      string commitId,
      string committer,
      string commitMessage,
      string commitDiffUrl,
      DateTime commitTime)
    {
      this.CommitId = commitId;
      this.Committer = committer;
      this.CommitMessage = commitMessage;
      this.CommitDiffUrl = commitDiffUrl;
      this.CommitTime = commitTime;
    }
  }
}
