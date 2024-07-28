// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitRefs2Controller
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(7.2)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "refs", ResourceVersion = 2)]
  public class GitRefs2Controller : GitRefsController
  {
    [HttpGet]
    [ClientLocationId("2D874A60-A811-4F62-9C9F-963A6EA0A55B")]
    [ClientResponseType(typeof (IPagedList<GitRef>), null, null)]
    [ClientExample("GET__git_repositories__repositoryId__refs.json", "Refs", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__refs_heads.json", "Refs heads", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__refs_heads_statuses.json", "Refs heads statuses", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__refs_tags.json", "Refs tags", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__refs_heads_contains_replacer.json", "Refs heads that contain a word", null, null)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetRefs(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      [ClientQueryParameter] string filter = null,
      bool includeLinks = false,
      bool includeStatuses = false,
      bool includeMyBranches = false,
      bool latestStatusesOnly = false,
      bool peelTags = false,
      string filterContains = null,
      [ClientInclude(RestClientLanguages.CSharp | RestClientLanguages.Swagger2 | RestClientLanguages.Python | RestClientLanguages.TypeScriptWebPlatform | RestClientLanguages.Go), FromUri(Name = "$top")] int? top = null,
      [ClientInclude(RestClientLanguages.CSharp | RestClientLanguages.Swagger2 | RestClientLanguages.Python | RestClientLanguages.TypeScriptWebPlatform | RestClientLanguages.Go)] string continuationToken = null)
    {
      ISecuredObject securedObject;
      string nextToken;
      HttpResponseMessage response = this.GenerateResponse<GitRef>(this.GetRefsInternal(out securedObject, out nextToken, repositoryId, projectId, filter, includeLinks, includeStatuses, includeMyBranches, latestStatusesOnly, peelTags, filterContains, top, continuationToken), securedObject);
      if (nextToken != null)
        response.Headers.Add("x-ms-continuationtoken", nextToken);
      return response;
    }

    [HttpPatch]
    [ClientLocationId("2D874A60-A811-4F62-9C9F-963A6EA0A55B")]
    [ClientExample("PATCH__git_repositories__repositoryId__refs.json", "Lock/unlock branch", null, null)]
    public override GitRef UpdateRef(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientQueryParameter] string filter,
      GitRefUpdate newRefInfo,
      string projectId = null)
    {
      return this.UpdateRefInternal(repositoryId, filter, newRefInfo, projectId);
    }
  }
}
