// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.AdvSecEnablementController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "advsecEnablement")]
  public class AdvSecEnablementController : TfsApiController
  {
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("B43DD56F-A1B4-47A5-A857-73FC1B6C700C")]
    [ClientInternalUseOnly(true)]
    public HttpResponseMessage DeleteEnablementStatus(
      [FromUri(Name = "$allProjects")] bool allProjects,
      [FromUri(Name = "$includeBillableCommitters")] bool includeBillableCommitters = true,
      [FromUri(Name = "projectIds")] IEnumerable<Guid> projectIds = null)
    {
      IGitAdvSecService service = this.TfsRequestContext.GetService<IGitAdvSecService>();
      if (!service.CheckS2SCall(this.TfsRequestContext))
        throw new HttpException(403, "AdvSec calls are S2S only.");
      if (!allProjects && projectIds.IsNullOrEmpty<Guid>())
        throw new InvalidArgumentValueException(nameof (allProjects), "If allProjects is false, there must be at least 1 projectIds");
      if (allProjects && !projectIds.IsNullOrEmpty<Guid>())
        throw new InvalidArgumentValueException(nameof (allProjects), "allProjects is true, projectIds should not be populated");
      if (allProjects)
        projectIds = Enumerable.Empty<Guid>();
      service.DeleteEnablementData(this.TfsRequestContext, allProjects, includeBillableCommitters, projectIds);
      return new HttpResponseMessage(HttpStatusCode.OK);
    }

    [HttpGet]
    [ClientResponseType(typeof (IList<AdvSecEnablementStatus>), null, null)]
    [ClientLocationId("B43DD56F-A1B4-47A5-A857-73FC1B6C700C")]
    [ClientInternalUseOnly(true)]
    public List<AdvSecEnablementStatus> GetEnablementStatus(
      [FromUri(Name = "projectIds")] IEnumerable<Guid> projectIds = null,
      [FromUri(Name = "$billingDate")] DateTime? billingDate = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$take")] int? take = null)
    {
      IGitAdvSecService service = this.TfsRequestContext.GetService<IGitAdvSecService>();
      if (!service.CheckS2SCall(this.TfsRequestContext))
        throw new HttpException(403, "AdvSec calls are S2S only.");
      if (projectIds == null)
        projectIds = (IEnumerable<Guid>) new List<Guid>();
      return service.QueryEnablementStatus(this.TfsRequestContext, projectIds, false, billingDate, skip, take).Select<GitAdvSecEnablementStatus, AdvSecEnablementStatus>((Func<GitAdvSecEnablementStatus, AdvSecEnablementStatus>) (x => new AdvSecEnablementStatus()
      {
        ProjectId = x.ProjectId,
        RepositoryId = x.RepositoryId,
        Enabled = x.Enabled,
        EnabledChangedOnDate = x.EnabledChangedOnDate,
        ChangedOnDate = x.ChangedOnDate,
        ChangedById = x.ChangedBy
      })).ToList<AdvSecEnablementStatus>();
    }

    [HttpGet]
    [ClientResponseType(typeof (bool), null, null)]
    [ClientLocationId("B43DD56F-A1B4-47A5-A857-73FC1B6C700C")]
    [ClientInternalUseOnly(true)]
    public bool GetEnableOnCreateHost([FromUri(Name = "$enableOnCreateHost")] bool enableOnCreateHost) => this.TfsRequestContext.GetService<IGitAdvSecService>().QueryEnableOnCreateHostRegKey(this.TfsRequestContext);

    [HttpGet]
    [ClientResponseType(typeof (bool), null, null)]
    [ClientLocationId("B43DD56F-A1B4-47A5-A857-73FC1B6C700C")]
    [ClientInternalUseOnly(true)]
    public bool GetEnableOnCreateProject([FromUri(Name = "$enableOnCreateProjectId")] Guid projectId) => this.TfsRequestContext.GetService<IGitAdvSecService>().QueryEnableOnCreateProjectRegKey(this.TfsRequestContext, projectId);

    [HttpPut]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("B43DD56F-A1B4-47A5-A857-73FC1B6C700C")]
    [ClientInternalUseOnly(true)]
    public void SetEnableOnCreateHost([FromUri(Name = "$enableOnCreateHost")] bool enableOnCreateHost) => this.TfsRequestContext.GetService<IGitAdvSecService>().UpdateEnableOnCreateHostRegKey(this.TfsRequestContext, enableOnCreateHost);

    [HttpPut]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("B43DD56F-A1B4-47A5-A857-73FC1B6C700C")]
    [ClientInternalUseOnly(true)]
    public void SetEnableOnCreateProject([FromUri(Name = "$enableOnCreateProjectId")] Guid projectId, [FromUri(Name = "$enableOnStatus")] bool enableOnStatus) => this.TfsRequestContext.GetService<IGitAdvSecService>().UpdateEnableOnCreateProjectRegKey(this.TfsRequestContext, projectId, enableOnStatus);

    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("B43DD56F-A1B4-47A5-A857-73FC1B6C700C")]
    [ClientInternalUseOnly(true)]
    public HttpResponseMessage UpdateEnablementStatus([FromBody] List<AdvSecEnablementUpdate> enablementUpdates)
    {
      IGitAdvSecService service = this.TfsRequestContext.GetService<IGitAdvSecService>();
      if (!service.CheckS2SCall(this.TfsRequestContext))
        throw new HttpException(403, "AdvSec calls are S2S only.");
      if (enablementUpdates == null)
        throw new InvalidArgumentValueException(nameof (enablementUpdates), "Missing EnableUpdates");
      service.UpdateEnablementStatus(this.TfsRequestContext, (IEnumerable<GitAdvSecEnablementUpdate>) enablementUpdates.Select<AdvSecEnablementUpdate, GitAdvSecEnablementUpdate>((Func<AdvSecEnablementUpdate, GitAdvSecEnablementUpdate>) (x => new GitAdvSecEnablementUpdate(x.ProjectId, x.RepositoryId, x.NewStatus))).ToList<GitAdvSecEnablementUpdate>());
      return new HttpResponseMessage(HttpStatusCode.OK);
    }
  }
}
