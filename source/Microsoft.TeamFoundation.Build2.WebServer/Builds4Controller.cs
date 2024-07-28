// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Builds4Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "builds", ResourceVersion = 3)]
  public class Builds4Controller : Builds3r3Controller, IOverrideLoggingMethodNames
  {
    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientDebounce(true, 1500)]
    public override Microsoft.TeamFoundation.Build.WebApi.Build GetBuild(
      int buildId,
      string propertyFilters = null)
    {
      return this.GetBuildInternal(buildId, propertyFilters);
    }

    protected virtual Microsoft.TeamFoundation.Build.WebApi.Build GetBuildInternal(
      int buildId,
      string propertyFilters = null,
      Guid? projectId = null)
    {
      return this.FixResource((projectId.HasValue ? this.BuildService.GetBuildById(this.TfsRequestContext, projectId.Value, buildId, ArtifactPropertyKinds.AsPropertyFilters(propertyFilters), true) : this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId, ArtifactPropertyKinds.AsPropertyFilters(propertyFilters), true)) ?? throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId))).ToWebApiBuild(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
    }

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>), null, null)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetBuilds(
      [ClientParameterAsIEnumerable(typeof (int), ',')] string definitions = null,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string queues = null,
      string buildNumber = "*",
      DateTime? minTime = null,
      DateTime? maxTime = null,
      string requestedFor = null,
      Microsoft.TeamFoundation.Build.WebApi.BuildReason? reasonFilter = null,
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? statusFilter = null,
      Microsoft.TeamFoundation.Build.WebApi.BuildResult? resultFilter = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string tagFilters = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string properties = null,
      [FromUri(Name = "$top")] int top = 1000,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption deletedFilter = Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption.ExcludeDeleted,
      Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder queryOrder = Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending,
      string branchName = null,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string buildIds = null,
      string repositoryId = null,
      string repositoryType = null)
    {
      List<Microsoft.TeamFoundation.Build.WebApi.Build> buildsInternal = this.GetBuildsInternal(definitions, queues, buildNumber, minTime, maxTime, requestedFor, reasonFilter, statusFilter, resultFilter, tagFilters, properties, top, continuationToken, maxBuildsPerDefinition, deletedFilter, queryOrder, branchName, buildIds, repositoryId, repositoryType);
      HttpResponseMessage response = this.Request.CreateResponse<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(HttpStatusCode.OK, buildsInternal.Take<Microsoft.TeamFoundation.Build.WebApi.Build>(top).ToList<Microsoft.TeamFoundation.Build.WebApi.Build>());
      if (buildsInternal.Count > top && !maxBuildsPerDefinition.HasValue)
        this.SetContinuationToken(response, buildsInternal[top], new Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder?(queryOrder));
      return response;
    }

    [ClientIgnore]
    public string GetLoggingMethodName(string methodName, HttpActionContext actionContext) => methodName.Contains("GetBuilds") ? actionContext.ActionDescriptor.ControllerDescriptor.ControllerName + "|" + this.BuildService.GetBuildsMethodName(this.TfsRequestContext, (IDictionary<string, object>) actionContext.ActionArguments) : methodName;

    protected override void VerifyBuildQueryOrder(Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder buildQueryOrder)
    {
    }

    protected override bool IsContinuationSupported(Microsoft.TeamFoundation.Build.WebApi.BuildStatus? buildStatus)
    {
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable1 = buildStatus.HasValue ? buildStatus : new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(Microsoft.TeamFoundation.Build.WebApi.BuildStatus.All);
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable2 = nullable1;
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus1 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed;
      if (!(nullable2.GetValueOrDefault() == buildStatus1 & nullable2.HasValue))
      {
        nullable2 = nullable1;
        Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus2 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.InProgress;
        if (!(nullable2.GetValueOrDefault() == buildStatus2 & nullable2.HasValue))
        {
          nullable2 = nullable1;
          Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus3 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.NotStarted;
          if (!(nullable2.GetValueOrDefault() == buildStatus3 & nullable2.HasValue))
          {
            nullable2 = nullable1;
            Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus4 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.All;
            return nullable2.GetValueOrDefault() == buildStatus4 & nullable2.HasValue;
          }
        }
      }
      return true;
    }

    protected override Microsoft.TeamFoundation.Build.WebApi.BuildStatus HandleBuildMinMaxTimeSetAndGetStatus(
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? statusFilter,
      Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder buildQueryOrder)
    {
      switch (buildQueryOrder)
      {
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeAscending:
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending:
          if (statusFilter.HasValue)
          {
            Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable = statusFilter;
            Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed;
            if (!(nullable.GetValueOrDefault() == buildStatus & nullable.HasValue))
              throw new InvalidBuildQueryException(Resources.InvalidStatusQueryOrderFilterCombination((object) statusFilter, (object) buildQueryOrder));
          }
          statusFilter = new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed);
          break;
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeDescending:
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeAscending:
          if (statusFilter.HasValue)
          {
            Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable = statusFilter;
            Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus1 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.NotStarted;
            if (!(nullable.GetValueOrDefault() == buildStatus1 & nullable.HasValue))
            {
              nullable = statusFilter;
              Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus2 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.All;
              if (!(nullable.GetValueOrDefault() == buildStatus2 & nullable.HasValue))
                throw new InvalidBuildQueryException(Resources.InvalidStatusQueryOrderFilterCombination((object) statusFilter, (object) buildQueryOrder));
            }
          }
          statusFilter = new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(Microsoft.TeamFoundation.Build.WebApi.BuildStatus.All);
          break;
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeDescending:
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeAscending:
          if (statusFilter.HasValue)
          {
            Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable = statusFilter;
            Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.InProgress;
            if (!(nullable.GetValueOrDefault() == buildStatus & nullable.HasValue))
              throw new InvalidBuildQueryException(Resources.InvalidStatusQueryOrderFilterCombination((object) statusFilter, (object) buildQueryOrder));
          }
          statusFilter = new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(Microsoft.TeamFoundation.Build.WebApi.BuildStatus.InProgress);
          break;
      }
      return statusFilter.Value;
    }
  }
}
