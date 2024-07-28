// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.AdvSecEstimateProjectController
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
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "advsecEstimateProject")]
  public class AdvSecEstimateProjectController : GitApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (IList<BillablePusher>), null, null)]
    [ClientLocationId("1DF7833E-1EED-447B-81A3-390C74923900")]
    [ClientInternalUseOnly(true)]
    public List<BillablePusher> GetEstimatedBillablePushersProject()
    {
      IGitAdvSecService service = this.TfsRequestContext.GetService<IGitAdvSecService>();
      if (!service.CheckS2SCall(this.TfsRequestContext))
        throw new HttpException(403, "AdvSec calls are S2S only.");
      return service.EstimateBillablePushers(this.TfsRequestContext, new Guid?(this.ProjectId)).Select<GitBillablePusher, BillablePusher>((Func<GitBillablePusher, BillablePusher>) (x => new BillablePusher()
      {
        ProjectId = x.ProjectId,
        RepoId = x.RepoId,
        VSID = x.VSID
      })).ToList<BillablePusher>();
    }
  }
}
