// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitCherryPickRelationshipsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [FeatureEnabled("Git.CherryPickRelationship")]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "cherryPickRelationships")]
  [ClientInternalUseOnly(true)]
  public class GitCherryPickRelationshipsController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("8AF142A4-27C2-4168-9E82-46B8629AAA0D")]
    [ClientResponseType(typeof (IEnumerable<GitCommitRef>), null, null)]
    public HttpResponseMessage GetCherryPickRelationships(
      [ClientParameterType(typeof (Guid), true)] string repositoryNameOrId,
      string commitId,
      bool includeLinks = false)
    {
      Sha1Id sha1Id = GitCommitUtility.ParseSha1Id(commitId);
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryNameOrId))
        return this.GenerateResponse<GitCommitRef>(tfsGitRepository.CherryPickRelationships.GetFamily(sha1Id).Select<Sha1Id, GitCommitRef>((Func<Sha1Id, GitCommitRef>) (oid => new GitCommitRef()
        {
          CommitId = oid.ToString()
        })));
    }
  }
}
