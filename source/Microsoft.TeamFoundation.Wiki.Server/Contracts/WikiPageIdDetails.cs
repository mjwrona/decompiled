// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.Contracts.WikiPageIdDetails
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;

namespace Microsoft.TeamFoundation.Wiki.Server.Contracts
{
  public class WikiPageIdDetails
  {
    public int PageId { get; private set; }

    public Guid WikiId { get; private set; }

    public GitVersionDescriptor WikiVersion { get; private set; }

    public string GitFriendlyPagePath { get; private set; }

    public int AssociatedPushId { get; private set; }

    public WikiPageIdDetails(
      int pageId,
      Guid wikiId,
      string wikiVersion,
      string gitFriendlyPagePath,
      int associatedPushId)
    {
      if (!wikiVersion.StartsWith("refs/heads/"))
        throw new ArgumentException("Only version type 'branch' is supported");
      this.PageId = pageId;
      this.WikiId = wikiId;
      this.WikiVersion = new GitVersionDescriptor()
      {
        VersionType = GitVersionType.Branch,
        Version = GitUtils.GetFriendlyBranchName(wikiVersion)
      };
      this.AssociatedPushId = associatedPushId;
      this.GitFriendlyPagePath = gitFriendlyPagePath;
    }

    internal WikiPageIdDetails(WikiPageIdInfo info)
      : this(info.PageId, info.WikiId, info.WikiVersion, info.GitFriendlyPagePath, info.AssociatedPushId)
    {
    }

    internal WikiPageIdDetails(WikiPageWithId pageWithId, Guid wikiId, string wikiVersion)
      : this(pageWithId.PageId, wikiId, wikiVersion, pageWithId.GitFriendlyPagePath, pageWithId.AssociatedPushId)
    {
    }
  }
}
