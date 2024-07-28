// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcBranchesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class TfvcBranchesController : TfvcApiController
  {
    [ClientExample("GET__tfvc_branches__path_.json", "GET a branch", null, null)]
    [ClientExample("GET__tfvc_branches__path__includeChildren-true.json", "GET a branch with children", null, null)]
    [ClientExample("GET__tfvc_branches__path__includeParent-true.json", "GET a branch with parents", null, null)]
    [ClientExample("GET__tfvc_branches__deleted__includeDeleted-true.json", "GET a branch with deleted", null, null)]
    [HttpGet]
    [ClientResponseType(typeof (TfvcBranch), null, null)]
    public HttpResponseMessage GetBranch([ClientQueryParameter] string path, bool includeParent = false, bool includeChildren = false)
    {
      if (this.ProjectId != Guid.Empty)
        path = this.ProjectScopedPath(path);
      if (string.IsNullOrWhiteSpace(path))
        return this.GetBranches(includeParent, includeChildren);
      TfvcBranch branch = TfvcBranchesUtility.GetBranch(this.TfsRequestContext, this.Url, path, includeParent, includeChildren);
      branch.Links = branch.GetBranchReferenceLinks(this.TfsRequestContext, this.Url);
      return this.Request.CreateResponse<TfvcBranch>(HttpStatusCode.OK, branch);
    }

    [ClientExample("GET__tfvc_branches.json", "GET a list of root branches", null, null)]
    [ClientExample("GET__tfvc_branches_includeChildren-true.json", "GET a list of root branches with children", null, null)]
    [ClientExample("GET__tfvc_branches_includeDeleted-true.json", "GET a list of root branches with deleted", null, null)]
    [ClientResourceOperation(ClientResourceOperationName.List)]
    [HttpGet]
    [ClientResponseType(typeof (List<TfvcBranch>), null, null)]
    public HttpResponseMessage GetBranches(
      bool includeParent = false,
      bool includeChildren = false,
      bool includeDeleted = false,
      bool includeLinks = false)
    {
      return this.Request.CreateResponse<TfvcBranchesCollection>(HttpStatusCode.OK, TfvcBranchesUtility.QueryRootBranches(this.TfsRequestContext, this.Url, this.ProjectId != Guid.Empty ? this.ProjectScopedPath(string.Empty) : string.Empty, includeParent, includeChildren, includeDeleted, includeLinks));
    }

    [ClientExample("GET__tfvc_branchRefs.json", "GET a list of branch refs", null, null)]
    [ClientExample("GET__tfvc_branchRefs_includeDeleted.json", "GET a list of branch refs including deleted", null, null)]
    [ClientExample("GET__tfvc_branchRefs_includeLinks.json", "GET a list of branch refs with links", null, null)]
    [HttpGet]
    [ClientResponseType(typeof (List<TfvcBranchRef>), null, null)]
    public HttpResponseMessage GetBranchRefs(
      string scopePath,
      bool includeDeleted = false,
      bool includeLinks = false)
    {
      if (this.ProjectId != Guid.Empty)
        scopePath = this.ProjectScopedPath(scopePath);
      if (string.IsNullOrWhiteSpace(scopePath))
        return this.GetBranches(includeDeleted: includeDeleted, includeLinks: includeLinks);
      if (scopePath == "$")
        scopePath = "$/";
      return this.Request.CreateResponse<TfvcBranchRefsCollection>(HttpStatusCode.OK, TfvcBranchesUtility.QueryBranches(this.TfsRequestContext, this.Url, scopePath, (VersionSpec) new LatestVersionSpec(), includeDeleted, includeLinks));
    }
  }
}
