// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitRefsBatchController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "refsBatch")]
  public class GitRefsBatchController : GitApiController
  {
    [HttpPost]
    [ClientIgnore]
    [ClientLocationId("D5E42319-9C64-4ACD-A906-F524A578A7FE")]
    public HttpResponseMessage GetRefsBatch(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [FromBody] GitQueryRefsCriteria searchCriteria,
      [ClientIgnore] string projectId = null,
      bool includeLinks = false)
    {
      if (searchCriteria == null)
        searchCriteria = new GitQueryRefsCriteria();
      List<Sha1Id> commitIds = new List<Sha1Id>();
      if (searchCriteria.CommitIds != null && searchCriteria.CommitIds.Any<string>())
      {
        foreach (string commitId in searchCriteria.CommitIds)
        {
          Sha1Id id;
          if (Sha1Id.TryParse(commitId, out id))
            commitIds.Add(id);
        }
      }
      List<TfsGitRefWithResolvedCommit> source;
      RepoKey repoKey;
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        repoKey = tfsGitRepository.Key;
        source = tfsGitRepository.Refs.QueryGitRefsBySearchCriteria(searchCriteria.RefNames, searchCriteria.SearchType, (IEnumerable<Sha1Id>) commitIds);
      }
      return this.GenerateResponse<GitRef>(source.Select<TfsGitRefWithResolvedCommit, GitRef>((Func<TfsGitRefWithResolvedCommit, GitRef>) (r => r.ToWebApiItem(this.TfsRequestContext, repoKey, includeLinks, (ILookup<Sha1Id, GitStatus>) null, (ISecuredObject) null))));
    }
  }
}
