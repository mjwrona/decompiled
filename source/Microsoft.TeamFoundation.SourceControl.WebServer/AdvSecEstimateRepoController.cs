// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.AdvSecEstimateRepoController
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
using System.Web;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "advsecEstimateRepo")]
  public class AdvSecEstimateRepoController : GitApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (IList<BillableCommitter>), null, null)]
    [ClientLocationId("5DCEC07B-A844-4EFB-9FC1-968FD1F149DB")]
    [ClientInternalUseOnly(true)]
    public List<BillableCommitter> GetEstimatedBillableCommittersRepo([ClientParameterType(typeof (Guid), true)] string repositoryId)
    {
      IGitAdvSecService service = this.TfsRequestContext.GetService<IGitAdvSecService>();
      if (!service.CheckS2SCall(this.TfsRequestContext))
        throw new HttpException(403, "AdvSec calls are S2S only.");
      return service.EstimateBillableCommitters(this.TfsRequestContext, this.ProjectId, new Guid?(Guid.Parse(repositoryId))).Select<GitBillableCommitter, BillableCommitter>((Func<GitBillableCommitter, BillableCommitter>) (x => new BillableCommitter()
      {
        VSID = x.VSID,
        RepoId = x.RepoId
      })).ToList<BillableCommitter>();
    }
  }
}
