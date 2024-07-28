// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitBillableCommittersController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "billableCommitters")]
  public class GitBillableCommittersController : GitApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (IList<BillableCommitter>), null, null)]
    [ClientLocationId("5C5E3EBC-37B0-4547-A957-945912D44922")]
    [ClientInternalUseOnly(true)]
    public List<BillableCommitter> GetBillableCommitters(
      [ClientIgnore] string projectId = null,
      [FromUri(Name = "$billingDate")] DateTime? billingDate = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$take")] int? take = null)
    {
      IGitAdvSecService service = this.TfsRequestContext.GetService<IGitAdvSecService>();
      if (!service.CheckS2SCall(this.TfsRequestContext))
        throw new HttpException(403, "AdvSec calls are S2S only.");
      return service.QueryBillableCommitters(this.TfsRequestContext, this.GetProjectFilter(projectId, out ProjectInfo _), billingDate, skip, take).Select<GitBillableCommitter, BillableCommitter>((Func<GitBillableCommitter, BillableCommitter>) (x => new BillableCommitter()
      {
        VSID = x.VSID,
        RepoId = x.RepoId
      })).ToList<BillableCommitter>();
    }

    [HttpGet]
    [ClientResponseType(typeof (IList<BillableCommitterDetail>), null, null)]
    [ClientLocationId("5C5E3EBC-37B0-4547-A957-945912D44922")]
    [ClientInternalUseOnly(true)]
    public List<BillableCommitterDetail> GetBillableCommittersDetail(
      [FromUri(Name = "$includeDetails")] string includeDetails,
      [ClientIgnore] string projectId = null,
      [FromUri(Name = "$billingDate")] DateTime? billingDate = null)
    {
      IGitAdvSecService service = this.TfsRequestContext.GetService<IGitAdvSecService>();
      if (!service.CheckS2SCall(this.TfsRequestContext))
        throw new HttpException(403, "AdvSec calls are S2S only.");
      return service.QueryBillableCommittersDetailed(this.TfsRequestContext, this.GetProjectFilter(projectId, out ProjectInfo _), billingDate).Select<GitBillableCommitterDetail, BillableCommitterDetail>((Func<GitBillableCommitterDetail, BillableCommitterDetail>) (x =>
      {
        return new BillableCommitterDetail()
        {
          ProjectId = x.ProjectId,
          ProjectName = x.ProjectName,
          RepoId = x.RepoId,
          RepoName = x.RepoName,
          CommitId = x.CommitId.ToString(),
          VSID = x.VSID,
          CommitterEmail = x.CommitterEmail,
          CommitTime = x.CommitTime,
          PushId = x.PushId,
          PushedTime = x.PushedTime,
          PusherId = x.Pusherid,
          SamAccountName = x.SamAccountName,
          MailNickName = x.MailNickName,
          DisplayName = x.DisplayName
        };
      })).ToList<BillableCommitterDetail>();
    }
  }
}
