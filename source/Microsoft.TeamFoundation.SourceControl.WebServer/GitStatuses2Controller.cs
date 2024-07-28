// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitStatuses2Controller
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(7.2)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "Statuses", ResourceVersion = 2)]
  public class GitStatuses2Controller : GitStatusesController
  {
    [HttpGet]
    [ClientLocationId("428DD4FB-FDA5-4722-AF02-9313B80305DA")]
    [ClientResponseType(typeof (IList<GitStatus>), null, null)]
    [ClientExample("GET__git_repositories__repositoryId__commits__commitId__statuses.json", null, null, null)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetStatuses(
      string commitId,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      int top = 1000,
      int skip = 0,
      bool latestOnly = false)
    {
      ISecuredObject securedObject;
      List<GitStatus> statusesInternal = this.GetStatusesInternal(commitId, repositoryId, projectId, top, skip, latestOnly, out securedObject);
      return statusesInternal == null ? this.Request.CreateResponse<IList<GitStatus>>(HttpStatusCode.OK, (IList<GitStatus>) Array.Empty<GitStatus>()) : this.Request.CreateResponse<IList<GitStatus>>(HttpStatusCode.OK, (IList<GitStatus>) statusesInternal.Select<GitStatus, GitStatus>((Func<GitStatus, GitStatus>) (x =>
      {
        x.SetSecuredObject(securedObject);
        return x;
      })).ToList<GitStatus>());
    }
  }
}
