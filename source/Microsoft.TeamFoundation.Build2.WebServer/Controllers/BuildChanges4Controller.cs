// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Controllers.BuildChanges4Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer.Controllers
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "changes", ResourceVersion = 2)]
  [ClientGroupByResource("builds")]
  public class BuildChanges4Controller : BuildChanges3Controller
  {
    [HttpGet]
    [ClientLocationId("54572C7B-BBD3-45D4-80DC-28BE08941620")]
    [ClientResponseType(typeof (IPagedList<Microsoft.TeamFoundation.Build.WebApi.Change>), null, null)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetBuildChanges(
      int buildId,
      string continuationToken = null,
      [FromUri(Name = "$top")] int top = 50,
      bool includeSourceChange = false)
    {
      return this.GetBuildChangesInternal(buildId, continuationToken, top, includeSourceChange);
    }

    protected virtual HttpResponseMessage GetBuildChangesInternal(
      int buildId,
      string continuationToken = null,
      int top = 50,
      bool includeSourceChange = false,
      Guid? projectId = null)
    {
      BuildChangesContinuationToken token;
      if (!BuildChangesContinuationToken.TryParse(continuationToken, out token))
        throw new InvalidContinuationTokenException(Microsoft.TeamFoundation.Build2.WebServer.Resources.InvalidContinuationToken());
      BuildData build = projectId.HasValue ? this.BuildService.GetBuildById(this.TfsRequestContext, projectId.Value, buildId) : this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId);
      if (build == null)
        throw new BuildNotFoundException(Microsoft.TeamFoundation.Build2.WebServer.Resources.BuildNotFound((object) buildId));
      GetChangesResult changesResult = projectId.HasValue ? this.BuildService.GetChanges(this.TfsRequestContext, projectId.Value, buildId, includeSourceChange, token.NextChangeId.GetValueOrDefault(), top) : this.InternalBuildService.GetChanges(this.TfsRequestContext, buildId, includeSourceChange, token.NextChangeId.GetValueOrDefault(), top);
      HttpResponseMessage response = this.Request.CreateResponse<IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Change>>(HttpStatusCode.OK, changesResult.Changes.Select<Microsoft.TeamFoundation.Build2.Server.Change, Microsoft.TeamFoundation.Build.WebApi.Change>((Func<Microsoft.TeamFoundation.Build2.Server.Change, Microsoft.TeamFoundation.Build.WebApi.Change>) (x => x.ToWebApiChange(build.ToSecuredObject()))));
      this.SetContinuationToken(response, changesResult);
      return response;
    }

    [HttpGet]
    [PublicProjectRequestRestrictions(true, true, null)]
    public override List<Microsoft.TeamFoundation.Build.WebApi.Change> GetChangesBetweenBuilds(
      int? fromBuildId = null,
      int? toBuildId = null,
      [FromUri(Name = "$top")] int top = 50)
    {
      return this.GetChangesBetweenBuildsInternal(fromBuildId, toBuildId, top);
    }

    internal virtual List<Microsoft.TeamFoundation.Build.WebApi.Change> GetChangesBetweenBuildsInternal(
      int? fromBuildId = null,
      int? toBuildId = null,
      int top = 50,
      Guid? projectId = null)
    {
      int? nullable1 = fromBuildId;
      int num1 = 0;
      if (nullable1.GetValueOrDefault() > num1 & nullable1.HasValue)
      {
        int? nullable2 = toBuildId;
        int num2 = 0;
        if (nullable2.GetValueOrDefault() > num2 & nullable2.HasValue)
          return (projectId.HasValue ? this.BuildService.GetChangesBetweenBuilds(this.TfsRequestContext, projectId.Value, fromBuildId.Value, toBuildId.Value, top) : this.InternalBuildService.GetChangesBetweenBuilds(this.TfsRequestContext, fromBuildId.Value, toBuildId.Value, top)).Select<Microsoft.TeamFoundation.Build2.Server.Change, Microsoft.TeamFoundation.Build.WebApi.Change>((Func<Microsoft.TeamFoundation.Build2.Server.Change, Microsoft.TeamFoundation.Build.WebApi.Change>) (x => x.ToWebApiChange(((projectId.HasValue ? this.BuildService.GetBuildById(this.TfsRequestContext, projectId.Value, fromBuildId.Value) : this.InternalBuildService.GetBuildById(this.TfsRequestContext, fromBuildId.Value)) ?? throw new BuildNotFoundException(Microsoft.TeamFoundation.Build2.WebServer.Resources.BuildNotFound((object) fromBuildId.Value))).ToSecuredObject()))).ToList<Microsoft.TeamFoundation.Build.WebApi.Change>();
      }
      throw new InvalidBuildQueryException(Microsoft.TeamFoundation.Build2.WebServer.Resources.InvalidInputForGetChangesAndWorkItemsBetweenBuilds());
    }
  }
}
