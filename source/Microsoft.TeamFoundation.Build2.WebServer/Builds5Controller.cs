// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Builds5Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "builds", ResourceVersion = 4)]
  public class Builds5Controller : Builds4r4Controller
  {
    [HttpGet]
    [PublicProjectRequestRestrictions]
    public override Microsoft.TeamFoundation.Build.WebApi.Build GetBuild(
      int buildId,
      string propertyFilters = null)
    {
      return this.GetBuildInternal(buildId, propertyFilters, new Guid?(this.ProjectId));
    }

    [HttpPost]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.Build.WebApi.Build), null, null)]
    public override HttpResponseMessage QueueBuild(
      [FromBody] Microsoft.TeamFoundation.Build.WebApi.Build build,
      [FromUri] bool ignoreWarnings = false,
      [FromUri] string checkInTicket = null,
      [FromUri] int? sourceBuildId = null,
      [FromUri] int? definitionId = null)
    {
      if (!sourceBuildId.HasValue)
        return this.QueueBuildInternal(build, ignoreWarnings, checkInTicket);
      if (build?.Definition != null)
        throw new InvalidBuildException(Resources.BodyMustBeEmptyForRebuild());
      Microsoft.TeamFoundation.Build.WebApi.Build build1 = this.GetBuild(sourceBuildId.Value, (string) null);
      if (build1.Reason != BuildReason.PullRequest)
        throw new BuildOptionNotSupportedException(Resources.RebuildMustBePRBuild());
      build1.BuildNumber = (string) null;
      build1.BuildNumberRevision = new int?();
      build1.RequestedBy = (IdentityRef) null;
      build1.TriggeredByBuild = (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      build1.QueueTime = new DateTime?();
      build1.StartTime = new DateTime?();
      build1.FinishTime = new DateTime?();
      return this.QueueBuildInternal(build1, ignoreWarnings, checkInTicket, sourceBuildId);
    }

    [HttpGet]
    [ClientIgnoreRouteScopes(ClientRouteScopes.Collection)]
    [ClientResponseType(typeof (IPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>), null, null)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetBuilds(
      [ClientParameterAsIEnumerable(typeof (int), ',')] string definitions = null,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string queues = null,
      string buildNumber = "*",
      DateTime? minTime = null,
      DateTime? maxTime = null,
      string requestedFor = null,
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string tagFilters = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string properties = null,
      [FromUri(Name = "$top")] int top = 1000,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      QueryDeletedOption deletedFilter = QueryDeletedOption.ExcludeDeleted,
      BuildQueryOrder queryOrder = BuildQueryOrder.FinishTimeDescending,
      string branchName = null,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string buildIds = null,
      string repositoryId = null,
      string repositoryType = null)
    {
      return base.GetBuilds(definitions, queues, buildNumber, minTime, maxTime, requestedFor, reasonFilter, statusFilter, resultFilter, tagFilters, properties, top, continuationToken, maxBuildsPerDefinition, deletedFilter, queryOrder, branchName, buildIds, repositoryId, repositoryType);
    }
  }
}
