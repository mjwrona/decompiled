// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Controllers.BuildChanges3Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer.Controllers
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "changes", ResourceVersion = 2)]
  [ClientGroupByResource("builds")]
  public class BuildChanges3Controller : BuildApiController
  {
    [HttpGet]
    [ClientLocationId("54572C7B-BBD3-45D4-80DC-28BE08941620")]
    [ClientResponseType(typeof (IPagedList<Microsoft.TeamFoundation.Build.WebApi.Change>), null, null)]
    public virtual HttpResponseMessage GetBuildChanges(
      int buildId,
      string continuationToken = null,
      [FromUri(Name = "$top")] int top = 50,
      bool includeSourceChange = false)
    {
      BuildChangesContinuationToken token;
      if (!BuildChangesContinuationToken.TryParse(continuationToken, out token))
        throw new InvalidContinuationTokenException(Microsoft.TeamFoundation.Build2.WebServer.Resources.InvalidContinuationToken());
      BuildData build = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId);
      if (build == null)
        throw new BuildNotFoundException(Microsoft.TeamFoundation.Build2.WebServer.Resources.BuildNotFound((object) buildId));
      GetChangesResult changes = this.InternalBuildService.GetChanges(this.TfsRequestContext, buildId, includeSourceChange, token.NextChangeId.GetValueOrDefault(), top);
      HttpResponseMessage response = this.Request.CreateResponse<IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Change>>(HttpStatusCode.OK, changes.Changes.Select<Microsoft.TeamFoundation.Build2.Server.Change, Microsoft.TeamFoundation.Build.WebApi.Change>((Func<Microsoft.TeamFoundation.Build2.Server.Change, Microsoft.TeamFoundation.Build.WebApi.Change>) (x => x.ToWebApiChange(build.ToSecuredObject()))));
      this.SetContinuationToken(response, changes);
      return response;
    }

    [HttpGet]
    public virtual List<Microsoft.TeamFoundation.Build.WebApi.Change> GetChangesBetweenBuilds(
      int? fromBuildId = null,
      int? toBuildId = null,
      [FromUri(Name = "$top")] int top = 50)
    {
      int? nullable1 = fromBuildId;
      int num1 = 0;
      if (nullable1.GetValueOrDefault() > num1 & nullable1.HasValue)
      {
        int? nullable2 = toBuildId;
        int num2 = 0;
        if (nullable2.GetValueOrDefault() > num2 & nullable2.HasValue)
          return this.InternalBuildService.GetChangesBetweenBuilds(this.TfsRequestContext, fromBuildId.Value, toBuildId.Value, top).Select<Microsoft.TeamFoundation.Build2.Server.Change, Microsoft.TeamFoundation.Build.WebApi.Change>((Func<Microsoft.TeamFoundation.Build2.Server.Change, Microsoft.TeamFoundation.Build.WebApi.Change>) (x => x.ToWebApiChange((this.InternalBuildService.GetBuildById(this.TfsRequestContext, fromBuildId.Value) ?? throw new BuildNotFoundException(Microsoft.TeamFoundation.Build2.WebServer.Resources.BuildNotFound((object) fromBuildId.Value))).ToSecuredObject()))).ToList<Microsoft.TeamFoundation.Build.WebApi.Change>();
      }
      throw new InvalidBuildQueryException(Microsoft.TeamFoundation.Build2.WebServer.Resources.InvalidInputForGetChangesAndWorkItemsBetweenBuilds());
    }

    internal void SetContinuationToken(
      HttpResponseMessage responseMessage,
      GetChangesResult changesResult)
    {
      if (!changesResult.NextChangeId.HasValue)
        return;
      responseMessage.Headers.Add("x-ms-continuationtoken", new BuildChangesContinuationToken(changesResult).ToString());
    }
  }
}
