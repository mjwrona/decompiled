// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki.WikiSearchHit
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Utils;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki
{
  public class WikiSearchHit : SearchHit
  {
    public WikiContract Source { get; private set; }

    public WikiSearchHit()
    {
      this.Hits = Enumerable.Empty<WikiHitSnippet>();
      this.Source = new WikiContract();
    }

    public WikiSearchHit(IEnumerable<WikiHitSnippet> hits, WikiContract source)
    {
      this.Hits = hits;
      this.Source = source;
    }

    public IEnumerable<WikiHitSnippet> Hits { get; }

    public static WikiResult CreateWikiSearchResult(WikiSearchHit platformHit)
    {
      string collectionName = platformHit.Source.CollectionName;
      string collectionUrl = platformHit.Source.CollectionUrl;
      string projectName = platformHit.Source.ProjectName;
      string projectId = platformHit.Source.ProjectId;
      string repoName = platformHit.Source.RepoName;
      string repositoryId = platformHit.Source.RepositoryId;
      string wikiName = platformHit.Source.WikiName;
      string wikiId = platformHit.Source.WikiId;
      string branchName = platformHit.Source.BranchName;
      string mappedPath = platformHit.Source.MappedPath;
      IEnumerable<WikiHitSnippet> hits = platformHit.Hits;
      string filePath = platformHit.Source.FilePath;
      return new WikiResult(FilePathUtils.GetFileName(platformHit.Source.FilePath), filePath, hits, collectionName, collectionUrl, projectId, projectName, repoName, repositoryId, wikiName, wikiId, branchName, mappedPath, platformHit.Source.ContentId, platformHit.Source.LastUpdatedTime, platformHit.Source.ProjectVisibility);
    }
  }
}
