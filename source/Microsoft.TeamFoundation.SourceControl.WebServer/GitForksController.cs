// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitForksController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "forks")]
  public class GitForksController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("158C0340-BF6F-489C-9625-D572A1480D57")]
    [ClientResponseType(typeof (IEnumerable<GitRepositoryRef>), null, null)]
    public HttpResponseMessage GetForks(
      [ClientParameterType(typeof (Guid), true)] string repositoryNameOrId,
      Guid collectionId,
      bool includeLinks = false)
    {
      RepoKey repoKey = this.ResolveRepoKey(repositoryNameOrId);
      return this.GenerateResponse<GitRepositoryRef>(this.TfsRequestContext.GetService<IGitForkService>().QueryChildren(this.TfsRequestContext, repoKey, collectionId));
    }
  }
}
