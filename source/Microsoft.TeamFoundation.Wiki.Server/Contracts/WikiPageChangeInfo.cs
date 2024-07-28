// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.Contracts.WikiPageChangeInfo
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Git.Server;

namespace Microsoft.TeamFoundation.Wiki.Server.Contracts
{
  public class WikiPageChangeInfo
  {
    public int PageId { get; set; }

    public string GitFilePath { get; set; }

    public string OldGitFilePath { get; set; }

    public TfsGitChangeType ChangeType { get; set; }

    public WikiPageChangeInfo(
      int pageId,
      string gitFriendlyPagePath,
      TfsGitChangeType changeType,
      string oldGitFriendlyPagePath)
    {
      this.PageId = pageId;
      this.GitFilePath = gitFriendlyPagePath;
      this.ChangeType = changeType;
      this.OldGitFilePath = oldGitFriendlyPagePath;
    }

    public WikiPageChangeInfo()
    {
    }
  }
}
