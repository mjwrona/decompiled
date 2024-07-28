// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.Contracts.WikiPageChangeInfoAndCommits
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Wiki.Server.Contracts
{
  public class WikiPageChangeInfoAndCommits
  {
    public WikiPageChangeInfo PageChangeInfo { get; set; }

    public List<Microsoft.TeamFoundation.Wiki.Server.CommitDetails> CommitDetails { get; set; }

    public string RemoteUrl { get; set; }

    public WikiPageChangeInfoAndCommits() => this.CommitDetails = new List<Microsoft.TeamFoundation.Wiki.Server.CommitDetails>();

    public WikiPageChangeInfoAndCommits(WikiPageChangeInfo pageChangeInfo, string remoteUrl)
    {
      this.PageChangeInfo = pageChangeInfo;
      this.RemoteUrl = remoteUrl;
      this.CommitDetails = new List<Microsoft.TeamFoundation.Wiki.Server.CommitDetails>();
    }
  }
}
