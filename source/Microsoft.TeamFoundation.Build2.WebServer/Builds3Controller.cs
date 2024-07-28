// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Builds3Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "builds", ResourceVersion = 2)]
  public class Builds3Controller : BuildApiController
  {
    protected const int DefaultTop = 1000;
    protected const int MaxBuilds = 5000;
    private static readonly Version s_retryMinVersion = new Version(5, 0);

    [HttpGet]
    public virtual Microsoft.TeamFoundation.Build.WebApi.Build GetBuild(
      int buildId,
      string propertyFilters = null)
    {
      return this.FixResource(this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId, ArtifactPropertyKinds.AsPropertyFilters(propertyFilters), true) ?? throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId))).ToWebApiBuild(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
    }

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>), null, null)]
    public virtual HttpResponseMessage GetBuilds(
      [ClientParameterAsIEnumerable(typeof (int), ',')] string definitions = null,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string queues = null,
      string buildNumber = "*",
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
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
      List<Microsoft.TeamFoundation.Build.WebApi.Build> buildsInternal = this.GetBuildsInternal(definitions, queues, buildNumber, minFinishTime, maxFinishTime, requestedFor, reasonFilter, statusFilter, resultFilter, tagFilters, properties, top, continuationToken, maxBuildsPerDefinition, deletedFilter, queryOrder, branchName, buildIds, repositoryId, repositoryType);
      HttpResponseMessage response = this.Request.CreateResponse<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(HttpStatusCode.OK, buildsInternal.Take<Microsoft.TeamFoundation.Build.WebApi.Build>(top).ToList<Microsoft.TeamFoundation.Build.WebApi.Build>());
      if (buildsInternal.Count > top && this.IsContinuationSupported(statusFilter) && !maxBuildsPerDefinition.HasValue)
        this.SetContinuationToken(response, buildsInternal[top]);
      return response;
    }

    [HttpPost]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.Build.WebApi.Build), null, null)]
    public virtual HttpResponseMessage QueueBuild(
      [FromBody] Microsoft.TeamFoundation.Build.WebApi.Build build,
      [FromUri] bool ignoreWarnings = false,
      [FromUri] string checkInTicket = null,
      [FromUri] int? sourceBuildId = null,
      [FromUri] int? definitionId = null)
    {
      this.CheckRequestContent((object) build);
      this.ValidateBuild(build);
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition = this.DefinitionService.GetDefinition(this.TfsRequestContext, this.ProjectId, build.Definition.Id);
      if (definition == null)
        throw new DefinitionNotFoundException(Resources.DefinitionNotFound((object) build.Definition.Id));
      if (definition.Type == Microsoft.TeamFoundation.Build2.Server.DefinitionType.Xaml)
        throw new NotSupportedOnXamlBuildException(Resources.NotSupportedOnXamlBuildDefinition());
      BuildData serverBuild = (BuildData) null;
      if (definition.Type != Microsoft.TeamFoundation.Build2.Server.DefinitionType.Build)
        throw new DefinitionTypeNotSupportedException(Resources.DefinitionTypeNotSupported((object) definition.Type));
      BuildRequestValidationFlags validationFlags = ignoreWarnings ? BuildRequestValidationFlags.None : BuildRequestValidationFlags.WarningsAsErrors;
      serverBuild = build.ToBuildServerBuildData(this.TfsRequestContext);
      HashSet<string> internalRuntimeVariables = new HashSet<string>();
      serverBuild.Parameters = BuildRequestHelper.SanitizeDiagnosticsParameters(serverBuild.Parameters, out internalRuntimeVariables);
      IEnumerable<IBuildRequestValidator> validators = BuildRequestValidatorProvider.GetValidators(new BuildRequestValidationOptions()
      {
        RequireOnlineAgent = true,
        WarnIfNoMatchingAgent = true,
        ValidateSourceVersionFormat = true,
        InternalRuntimeVariables = internalRuntimeVariables
      });
      Microsoft.TeamFoundation.Build.WebApi.Build webApiBuild = this.BuildService.QueueBuild(this.TfsRequestContext, serverBuild, validators, validationFlags, checkInTicket, callingMethod: nameof (QueueBuild), callingFile: "D:\\a\\_work\\1\\s\\Tfs\\Service\\Build2\\Web\\Server\\Controllers\\3.0\\Builds3Controller.cs").ToWebApiBuild(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
      if (webApiBuild != null)
        this.TfsRequestContext.TraceInfo(12030196, "TriggeredBuilds", "Builds3Controller: Queued manual build {0} requested for {1} with definition {2} using branch {3} version {4}", (object) webApiBuild.Id, (object) webApiBuild.RequestedFor?.DisplayName, (object) webApiBuild.Definition?.Id, (object) webApiBuild.SourceBranch, (object) webApiBuild.SourceVersion);
      else
        this.TfsRequestContext.TraceInfo(12030196, "TriggeredBuilds", "Builds3Controller: Manual call to queue a build requested for {0} with definition {1} using branch {2} did not actually queue a build", (object) build.RequestedFor?.DisplayName, (object) build.Definition?.Id, (object) serverBuild.SourceBranch);
      this.TraceQueueBuildValidationResults(serverBuild);
      if (serverBuild.ValidationResults.Any<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult>((Func<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, bool>) (vr => vr.Result == Microsoft.TeamFoundation.Build2.Server.ValidationResult.Error)))
        return this.Request.CreateResponse<BuildRequestValidationFailedException>(HttpStatusCode.BadRequest, new BuildRequestValidationFailedException(Resources.BuildRequestValidationFailed(), serverBuild.ValidationResults.Select<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>((Func<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>) (vr => BuildRequestValidationResultExtensions.ToWebApiBuildRequestValidationResult(vr, serverBuild.ToSecuredObject()))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>()));
      return !ignoreWarnings && serverBuild.ValidationResults.Any<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult>((Func<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, bool>) (vr => vr.Result == Microsoft.TeamFoundation.Build2.Server.ValidationResult.Warning && !vr.Ignorable)) ? this.Request.CreateResponse<BuildRequestValidationFailedException>(HttpStatusCode.Conflict, new BuildRequestValidationFailedException(Resources.BuildRequestValidationFailed(), serverBuild.ValidationResults.Select<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>((Func<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>) (vr => BuildRequestValidationResultExtensions.ToWebApiBuildRequestValidationResult(vr, serverBuild.ToSecuredObject()))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>())) : this.Request.CreateResponse<Microsoft.TeamFoundation.Build.WebApi.Build>(HttpStatusCode.OK, this.FixResource(webApiBuild));
    }

    [HttpPatch]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.Build.WebApi.Build), null, null)]
    [ClientMethodAccessModifier(ClientMethodAccessModifierAttribute.AccessModifier.PrivateProtected)]
    public virtual async Task<HttpResponseMessage> UpdateBuild(
      int buildId,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      [FromUri] bool retry = false)
    {
      Builds3Controller builds3Controller = this;
      BuildData srvBuildData;
      if (retry)
      {
        builds3Controller.ValidateRetrySupported();
        if (build?.Definition != null)
          throw new RequestContentException(Resources.BodyMustBeEmptyForRetry());
        srvBuildData = await builds3Controller.BuildService.RetryBuildAsync(builds3Controller.TfsRequestContext, builds3Controller.ProjectId, buildId);
      }
      else
      {
        builds3Controller.CheckRequestContent((object) build);
        builds3Controller.ValidateBuildId(build, buildId);
        builds3Controller.ValidateProject(build);
        IBuildService buildService = builds3Controller.BuildService;
        IVssRequestContext tfsRequestContext = builds3Controller.TfsRequestContext;
        Guid id = build.Project.Id;
        int buildId1 = buildId;
        string buildNumber = build.BuildNumber;
        DateTime? startTime = build.StartTime;
        DateTime? finishTime = build.FinishTime;
        Microsoft.TeamFoundation.Build.WebApi.BuildStatus? status1 = build.Status;
        Microsoft.TeamFoundation.Build2.Server.BuildStatus? status2 = status1.HasValue ? new Microsoft.TeamFoundation.Build2.Server.BuildStatus?((Microsoft.TeamFoundation.Build2.Server.BuildStatus) status1.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.BuildStatus?();
        string sourceBranch = build.SourceBranch;
        string sourceVersion = build.SourceVersion;
        Microsoft.TeamFoundation.Build.WebApi.BuildResult? result1 = build.Result;
        Microsoft.TeamFoundation.Build2.Server.BuildResult? result2 = result1.HasValue ? new Microsoft.TeamFoundation.Build2.Server.BuildResult?((Microsoft.TeamFoundation.Build2.Server.BuildResult) result1.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.BuildResult?();
        bool? keepForever = build.KeepForever;
        bool? retainedByRelease = build.RetainedByRelease;
        srvBuildData = await buildService.UpdateBuildAsync(tfsRequestContext, id, buildId1, buildNumber, startTime, finishTime, status2, sourceBranch, sourceVersion, result2, keepForever, retainedByRelease);
      }
      if (srvBuildData == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      Microsoft.TeamFoundation.Build.WebApi.Build webApiBuild = srvBuildData.ToWebApiBuild(builds3Controller.TfsRequestContext, builds3Controller.GetApiResourceVersion().ApiVersion);
      return builds3Controller.Request.CreateResponse<Microsoft.TeamFoundation.Build.WebApi.Build>(HttpStatusCode.OK, builds3Controller.FixResource(webApiBuild));
    }

    [HttpPatch]
    [ClientResponseType(typeof (List<Microsoft.TeamFoundation.Build.WebApi.Build>), null, null)]
    public virtual async Task<HttpResponseMessage> UpdateBuilds([FromBody] IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> builds)
    {
      Builds3Controller builds3Controller = this;
      builds3Controller.CheckRequestContent((object) builds);
      foreach (Microsoft.TeamFoundation.Build.WebApi.Build build in builds)
      {
        builds3Controller.CheckRequestContent((object) build);
        builds3Controller.ValidateProject(build);
      }
      IdentityMap identityMap = new IdentityMap(builds3Controller.TfsRequestContext.GetService<IdentityService>());
      // ISSUE: reference to a compiler-generated method
      List<BuildData> buildDataList = await builds3Controller.BuildService.UpdateBuildsAsync(builds3Controller.TfsRequestContext, builds.Select<Microsoft.TeamFoundation.Build.WebApi.Build, BuildData>(new Func<Microsoft.TeamFoundation.Build.WebApi.Build, BuildData>(builds3Controller.\u003CUpdateBuilds\u003Eb__4_0)).ToList<BuildData>());
      List<Microsoft.TeamFoundation.Build.WebApi.Build> buildList = new List<Microsoft.TeamFoundation.Build.WebApi.Build>();
      if (buildDataList != null)
      {
        foreach (BuildData build in buildDataList)
          buildList.Add(builds3Controller.FixResource(build).ToWebApiBuild(builds3Controller.TfsRequestContext, builds3Controller.GetApiResourceVersion().ApiVersion, identityMap));
      }
      HttpResponseMessage response = builds3Controller.Request.CreateResponse<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(HttpStatusCode.OK, buildList);
      identityMap = (IdentityMap) null;
      return response;
    }

    [HttpDelete]
    public void DeleteBuild(int buildId) => this.BuildService.DeleteBuild(this.TfsRequestContext, this.ProjectId, buildId);

    protected List<Microsoft.TeamFoundation.Build.WebApi.Build> GetBuildsInternal(
      string definitions = null,
      string queues = null,
      string buildNumber = "*",
      DateTime? minTime = null,
      DateTime? maxTime = null,
      string requestedFor = null,
      Microsoft.TeamFoundation.Build.WebApi.BuildReason? reasonFilter = null,
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? statusFilter = null,
      Microsoft.TeamFoundation.Build.WebApi.BuildResult? resultFilter = null,
      string tagFilters = null,
      string properties = null,
      int top = 1000,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption deletedFilter = Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption.ExcludeDeleted,
      Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder queryOrder = Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending,
      string branchName = null,
      string buildIds = null,
      string repositoryId = null,
      string repositoryType = null)
    {
      if (top != 0)
      {
        int? nullable1 = maxBuildsPerDefinition;
        int num1 = 0;
        if (!(nullable1.GetValueOrDefault() == num1 & nullable1.HasValue))
        {
          WebApiBatchConverter batchConverter = new WebApiBatchConverter(this.TfsRequestContext, false);
          reasonFilter = this.FixBuildReasonFilter(reasonFilter);
          this.VerifyBuildQueryOrder(queryOrder);
          IList<int> intList1 = RestHelpers.ToInt32List(buildIds) ?? (IList<int>) new List<int>();
          if (intList1 != null && intList1.Any<int>())
          {
            if (definitions != null || queues != null || buildNumber != "*" || minTime.HasValue || maxTime.HasValue || requestedFor != null || reasonFilter.HasValue || statusFilter.HasValue || resultFilter.HasValue || tagFilters != null || properties != null || continuationToken != null || maxBuildsPerDefinition.HasValue || branchName != null || deletedFilter == Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption.OnlyDeleted || repositoryId != null || repositoryType != null)
              throw new InvalidBuildQueryException(Resources.InvalidBuildIdsQuery());
            bool includeDeleted = false;
            if (deletedFilter == Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption.IncludeDeleted)
              includeDeleted = true;
            return this.InternalBuildService.GetBuildsByIds(this.TfsRequestContext, (IEnumerable<int>) intList1, includeDeleted: includeDeleted).Take<BuildData>(top).Select<BuildData, Microsoft.TeamFoundation.Build.WebApi.Build>((Func<BuildData, Microsoft.TeamFoundation.Build.WebApi.Build>) (b => batchConverter.ConvertBuild(this.FixResource(b), this.GetApiResourceVersion().ApiVersion, true))).ToList<Microsoft.TeamFoundation.Build.WebApi.Build>();
          }
          if (minTime.HasValue || maxTime.HasValue)
            statusFilter = new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(this.HandleBuildMinMaxTimeSetAndGetStatus(statusFilter, queryOrder));
          Microsoft.TeamFoundation.Build.WebApi.BuildResult? nullable2;
          Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable3;
          if (resultFilter.HasValue)
          {
            nullable2 = resultFilter;
            Microsoft.TeamFoundation.Build.WebApi.BuildResult buildResult = Microsoft.TeamFoundation.Build.WebApi.BuildResult.None;
            if (!(nullable2.GetValueOrDefault() == buildResult & nullable2.HasValue) && statusFilter.HasValue)
            {
              nullable3 = statusFilter;
              Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus1 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed;
              if (!(nullable3.GetValueOrDefault() == buildStatus1 & nullable3.HasValue))
              {
                nullable3 = statusFilter;
                Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus2 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.None;
                if (!(nullable3.GetValueOrDefault() == buildStatus2 & nullable3.HasValue))
                  throw new InvalidBuildQueryException(Resources.InvalidStatusResultFilterCombination((object) statusFilter, (object) resultFilter));
              }
            }
          }
          if (resultFilter.HasValue)
          {
            nullable2 = resultFilter;
            Microsoft.TeamFoundation.Build.WebApi.BuildResult buildResult = Microsoft.TeamFoundation.Build.WebApi.BuildResult.None;
            if (!(nullable2.GetValueOrDefault() == buildResult & nullable2.HasValue))
              statusFilter = new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed);
          }
          nullable1 = maxBuildsPerDefinition;
          ArgumentUtility.CheckBoundsInclusive(nullable1 ?? top, 1, 5000, nameof (maxBuildsPerDefinition));
          ArgumentUtility.CheckBoundsInclusive(top, 1, 5000, "$top");
          IList<int> int32List = RestHelpers.ToInt32List(queues);
          IList<int> intList2 = RestHelpers.ToInt32List(definitions) ?? (IList<int>) new List<int>();
          DateTime? minTime1;
          DateTime? maxTime1;
          this.ParseBuildContinuationToken(statusFilter, queryOrder, continuationToken, out minTime1, out maxTime1);
          if (minTime1.HasValue || maxTime1.HasValue)
          {
            minTime = minTime1 ?? minTime;
            maxTime = maxTime1 ?? maxTime;
          }
          IBuildService buildService = this.BuildService;
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          Guid projectId = this.ProjectId;
          int count = top + 1;
          IList<int> definitionIds = intList2;
          IList<int> queueIds = int32List;
          string buildNumber1 = buildNumber;
          DateTime? minFinishTime = minTime;
          DateTime? maxFinishTime = maxTime;
          string requestedFor1 = requestedFor;
          Microsoft.TeamFoundation.Build.WebApi.BuildReason? nullable4 = reasonFilter;
          Microsoft.TeamFoundation.Build2.Server.BuildReason? reasonFilter1 = nullable4.HasValue ? new Microsoft.TeamFoundation.Build2.Server.BuildReason?((Microsoft.TeamFoundation.Build2.Server.BuildReason) nullable4.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.BuildReason?();
          nullable3 = statusFilter;
          Microsoft.TeamFoundation.Build2.Server.BuildStatus? statusFilter1 = nullable3.HasValue ? new Microsoft.TeamFoundation.Build2.Server.BuildStatus?((Microsoft.TeamFoundation.Build2.Server.BuildStatus) nullable3.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.BuildStatus?();
          nullable2 = resultFilter;
          Microsoft.TeamFoundation.Build2.Server.BuildResult? resultFilter1 = nullable2.HasValue ? new Microsoft.TeamFoundation.Build2.Server.BuildResult?((Microsoft.TeamFoundation.Build2.Server.BuildResult) nullable2.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.BuildResult?();
          IEnumerable<string> propertyFilters = ArtifactPropertyKinds.AsPropertyFilters(properties);
          IList<string> stringList = RestHelpers.ToStringList(tagFilters);
          int serverBuildQueryOrder = (int) queryOrder.ToServerBuildQueryOrder();
          int num2 = (int) deletedFilter;
          string repositoryId1 = repositoryId;
          string repositoryType1 = repositoryType;
          string branchName1 = branchName;
          int? maxBuildsPerDefinition1 = maxBuildsPerDefinition;
          return buildService.GetBuildsLegacy(tfsRequestContext, projectId, count, (IEnumerable<int>) definitionIds, (IEnumerable<int>) queueIds, buildNumber1, minFinishTime, maxFinishTime, requestedFor1, reasonFilter1, statusFilter1, resultFilter1, propertyFilters, (IEnumerable<string>) stringList, (Microsoft.TeamFoundation.Build2.Server.BuildQueryOrder) serverBuildQueryOrder, (Microsoft.TeamFoundation.Build2.Server.QueryDeletedOption) num2, repositoryId1, repositoryType1, branchName1, maxBuildsPerDefinition1).ToList<BuildData>().Select<BuildData, Microsoft.TeamFoundation.Build.WebApi.Build>((Func<BuildData, Microsoft.TeamFoundation.Build.WebApi.Build>) (b => batchConverter.ConvertBuild(this.FixResource(b), this.GetApiResourceVersion().ApiVersion, true))).ToList<Microsoft.TeamFoundation.Build.WebApi.Build>();
        }
      }
      return new List<Microsoft.TeamFoundation.Build.WebApi.Build>();
    }

    protected void ParseBuildContinuationToken(
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? statusFilter,
      Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder queryOrder,
      string continuationToken,
      out DateTime? minTime,
      out DateTime? maxTime)
    {
      minTime = new DateTime?();
      maxTime = new DateTime?();
      BuildsContinuationToken token;
      if (string.IsNullOrEmpty(continuationToken) || !this.IsContinuationSupported(statusFilter) || !BuildsContinuationToken.TryParse(continuationToken, out token))
        return;
      switch (queryOrder)
      {
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeAscending:
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeAscending:
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeAscending:
          minTime = token.Time;
          break;
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending:
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.QueueTimeDescending:
        case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.StartTimeDescending:
          maxTime = token.Time;
          break;
      }
    }

    protected virtual Microsoft.TeamFoundation.Build.WebApi.Build FixResource(Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
      if (build != null)
        build.Reason = this.FixBuildReason(build.Reason);
      return build;
    }

    protected BuildData FixResource(BuildData build)
    {
      if (build != null)
        build.Reason = this.FixBuildReason(build.Reason);
      return build;
    }

    protected void SetContinuationToken(
      HttpResponseMessage responseMessage,
      Microsoft.TeamFoundation.Build.WebApi.Build nextBuild,
      Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder? buildQueryOrder = null)
    {
      if (nextBuild == null)
        return;
      string str = new BuildsContinuationToken(nextBuild.QueueTime, nextBuild.StartTime, nextBuild.FinishTime, buildQueryOrder).ToString();
      if (string.IsNullOrEmpty(str))
        return;
      responseMessage.Headers.Add("x-ms-continuationtoken", str);
    }

    protected virtual bool IsContinuationSupported(Microsoft.TeamFoundation.Build.WebApi.BuildStatus? buildStatus)
    {
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable = buildStatus;
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus1 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed;
      return nullable.GetValueOrDefault() == buildStatus1 & nullable.HasValue;
    }

    protected virtual void ValidateBuild(Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build.WebApi.Build>(build, nameof (build));
      ArgumentUtility.CheckForNull<DefinitionReference>(build.Definition, "build.Definition");
      if (build.Definition.Type == Microsoft.TeamFoundation.Build.WebApi.DefinitionType.Xaml)
        throw new NotSupportedOnXamlBuildException(Resources.NotSupportedOnXamlBuildDefinition());
      if (build.Controller != null)
        throw new InvalidBuildException(Resources.BuildControllerMustNotBeSpecifiedForBuild());
      this.ValidateProject(build);
    }

    private void ValidateRetrySupported()
    {
      ApiResourceVersion apiResourceVersion = this.GetApiResourceVersion();
      if (this.IsResourceVersionOlderThan(apiResourceVersion, 4, Builds3Controller.s_retryMinVersion))
        throw new ArgumentException(Resources.RetryNotSupported((object) apiResourceVersion, (object) Builds3Controller.s_retryMinVersion)).Expected(this.TraceArea);
    }

    private void ValidateBuildId(Microsoft.TeamFoundation.Build.WebApi.Build build, int buildId)
    {
      if (build.Id == 0)
        build.Id = buildId;
      else if (build.Id != buildId)
        throw new RouteIdConflictException(Resources.WrongIdSpecifiedForBuild((object) buildId, (object) build.Id));
    }

    private void ValidateProject(Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
      if (this.ProjectId == Guid.Empty && (build.Project == null || build.Project.Id == Guid.Empty))
        throw new ProjectDoesNotExistException(Resources.ProjectRequired());
      if (build.Project == null)
        build.Project = new TeamProjectReference()
        {
          Id = this.ProjectId
        };
      else if (build.Project.Id != this.ProjectId && this.ProjectId != Guid.Empty)
        throw new RouteIdConflictException(Resources.WrongProjectSpecifiedForBuild((object) this.ProjectId, (object) build.Project.Id));
    }
  }
}
