// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Builds2Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Build2.Xaml;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "builds", ResourceVersion = 2)]
  public class Builds2Controller : BuildCompatApiController
  {
    private const int DefaultTop = 1000;
    private const int MaxBuilds = 5000;

    [HttpGet]
    public Microsoft.TeamFoundation.Build.WebApi.Build GetBuild(
      int buildId,
      string propertyFilters = null,
      Microsoft.TeamFoundation.Build.WebApi.DefinitionType? type = null)
    {
      Microsoft.TeamFoundation.Build.WebApi.Build build = (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      if (((int) type ?? 2) == 2)
        build = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId, ArtifactPropertyKinds.AsPropertyFilters(propertyFilters), true).ToWebApiBuild(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
      if (build == null && ((int) type ?? 1) == 1)
        build = this.TfsRequestContext.GetService<IXamlBuildProvider>().GetBuild(this.TfsRequestContext, this.ProjectInfo, buildId);
      return build != null ? this.FixResource(build) : throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
    }

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>), null, null)]
    public HttpResponseMessage GetBuilds(
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
      Microsoft.TeamFoundation.Build.WebApi.DefinitionType? type = null,
      [FromUri(Name = "$top")] int top = 1000,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption deletedFilter = Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption.ExcludeDeleted,
      Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder queryOrder = Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending,
      string branchName = null)
    {
      reasonFilter = this.FixBuildReasonFilter(reasonFilter);
      this.VerifyBuildQueryOrder(queryOrder);
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable1;
      if (minFinishTime.HasValue || maxFinishTime.HasValue)
      {
        if (statusFilter.HasValue)
        {
          nullable1 = statusFilter;
          Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed;
          if (!(nullable1.GetValueOrDefault() == buildStatus & nullable1.HasValue))
            throw new InvalidBuildQueryException(Resources.InvalidStatusFilterFinishTimeCombination((object) statusFilter));
        }
        statusFilter = new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed);
      }
      Microsoft.TeamFoundation.Build.WebApi.BuildResult? nullable2;
      if (resultFilter.HasValue)
      {
        nullable2 = resultFilter;
        Microsoft.TeamFoundation.Build.WebApi.BuildResult buildResult = Microsoft.TeamFoundation.Build.WebApi.BuildResult.None;
        if (!(nullable2.GetValueOrDefault() == buildResult & nullable2.HasValue) && statusFilter.HasValue)
        {
          nullable1 = statusFilter;
          Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus1 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed;
          if (!(nullable1.GetValueOrDefault() == buildStatus1 & nullable1.HasValue))
          {
            nullable1 = statusFilter;
            Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus2 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.None;
            if (!(nullable1.GetValueOrDefault() == buildStatus2 & nullable1.HasValue))
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
      if (top != 0)
      {
        int? nullable3 = maxBuildsPerDefinition;
        int num1 = 0;
        if (!(nullable3.GetValueOrDefault() == num1 & nullable3.HasValue))
        {
          nullable3 = maxBuildsPerDefinition;
          ArgumentUtility.CheckBoundsInclusive(nullable3 ?? top, 1, 5000, nameof (maxBuildsPerDefinition));
          ArgumentUtility.CheckBoundsInclusive(top, 1, 5000, "$top");
          IList<int> int32List = RestHelpers.ToInt32List(queues);
          IList<int> intList1 = RestHelpers.ToInt32List(definitions) ?? (IList<int>) new List<int>();
          BuildsContinuationToken token;
          if (this.IsContinuationSupported(statusFilter) && !string.IsNullOrEmpty(continuationToken) && BuildsContinuationToken.TryParse(continuationToken, out token))
          {
            switch (queryOrder)
            {
              case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeAscending:
                minFinishTime = token.Time;
                break;
              case Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending:
                maxFinishTime = token.Time;
                break;
            }
          }
          WebApiBatchConverter batchConverter = new WebApiBatchConverter(this.TfsRequestContext, false);
          List<Microsoft.TeamFoundation.Build.WebApi.Build> source = new List<Microsoft.TeamFoundation.Build.WebApi.Build>();
          if (!maxBuildsPerDefinition.HasValue)
          {
            if (((int) type ?? 2) == 2)
            {
              List<Microsoft.TeamFoundation.Build.WebApi.Build> buildList = source;
              IBuildService buildService = this.BuildService;
              IVssRequestContext tfsRequestContext = this.TfsRequestContext;
              Guid projectId = this.ProjectId;
              int count = top + 1;
              IList<int> definitionIds = intList1;
              IList<int> queueIds = int32List;
              string buildNumber1 = buildNumber;
              DateTime? minFinishTime1 = minFinishTime;
              DateTime? maxFinishTime1 = maxFinishTime;
              string requestedFor1 = requestedFor;
              Microsoft.TeamFoundation.Build.WebApi.BuildReason? nullable4 = reasonFilter;
              Microsoft.TeamFoundation.Build2.Server.BuildReason? reasonFilter1 = nullable4.HasValue ? new Microsoft.TeamFoundation.Build2.Server.BuildReason?((Microsoft.TeamFoundation.Build2.Server.BuildReason) nullable4.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.BuildReason?();
              nullable1 = statusFilter;
              Microsoft.TeamFoundation.Build2.Server.BuildStatus? statusFilter1 = nullable1.HasValue ? new Microsoft.TeamFoundation.Build2.Server.BuildStatus?((Microsoft.TeamFoundation.Build2.Server.BuildStatus) nullable1.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.BuildStatus?();
              nullable2 = resultFilter;
              Microsoft.TeamFoundation.Build2.Server.BuildResult? resultFilter1 = nullable2.HasValue ? new Microsoft.TeamFoundation.Build2.Server.BuildResult?((Microsoft.TeamFoundation.Build2.Server.BuildResult) nullable2.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.BuildResult?();
              IEnumerable<string> propertyFilters = ArtifactPropertyKinds.AsPropertyFilters(properties);
              IList<string> stringList = RestHelpers.ToStringList(tagFilters);
              int serverBuildQueryOrder = (int) queryOrder.ToServerBuildQueryOrder();
              int num2 = (int) deletedFilter;
              string branchName1 = branchName;
              nullable3 = new int?();
              int? maxBuildsPerDefinition1 = nullable3;
              IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> collection = buildService.GetBuildsLegacy(tfsRequestContext, projectId, count, (IEnumerable<int>) definitionIds, (IEnumerable<int>) queueIds, buildNumber1, minFinishTime1, maxFinishTime1, requestedFor1, reasonFilter1, statusFilter1, resultFilter1, propertyFilters, (IEnumerable<string>) stringList, (Microsoft.TeamFoundation.Build2.Server.BuildQueryOrder) serverBuildQueryOrder, (Microsoft.TeamFoundation.Build2.Server.QueryDeletedOption) num2, branchName: branchName1, maxBuildsPerDefinition: maxBuildsPerDefinition1).Select<BuildData, Microsoft.TeamFoundation.Build.WebApi.Build>((Func<BuildData, Microsoft.TeamFoundation.Build.WebApi.Build>) (x => batchConverter.ConvertBuild(x, this.GetApiResourceVersion().ApiVersion, true)));
              buildList.AddRange(collection);
            }
            bool flag = !string.IsNullOrEmpty(branchName);
            if (!flag && intList1.Count > 0)
            {
              HashSet<int> second = new HashSet<int>(source.Select<Microsoft.TeamFoundation.Build.WebApi.Build, int>((Func<Microsoft.TeamFoundation.Build.WebApi.Build, int>) (b => b.Definition.Id)));
              intList1 = (IList<int>) intList1.Except<int>((IEnumerable<int>) second).ToList<int>();
              flag = intList1.Count == 0;
            }
            if (!flag && ((int) type ?? 1) == 1)
            {
              IXamlBuildProvider service = this.TfsRequestContext.GetService<IXamlBuildProvider>();
              List<Microsoft.TeamFoundation.Build.WebApi.Build> buildList = source;
              IXamlBuildProvider xamlBuildProvider = service;
              IVssRequestContext tfsRequestContext = this.TfsRequestContext;
              ProjectInfo projectInfo = this.ProjectInfo;
              int count = top + 1;
              IList<int> definitionIds = intList1;
              IList<int> queueIds = int32List;
              string buildNumber2 = buildNumber;
              DateTime? minFinishTime2 = minFinishTime;
              DateTime? maxFinishTime2 = maxFinishTime;
              string requestedFor2 = requestedFor;
              int reasonFilter2 = (int) reasonFilter ?? 2031;
              Microsoft.TeamFoundation.Build.WebApi.BuildStatus? statusFilter2 = statusFilter;
              Microsoft.TeamFoundation.Build.WebApi.BuildResult? resultFilter2 = resultFilter;
              int queryOrder1 = (int) queryOrder;
              int num3 = (int) deletedFilter;
              nullable3 = new int?();
              int? maxBuildsPerDefinition2 = nullable3;
              IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> builds = xamlBuildProvider.GetBuilds(tfsRequestContext, projectInfo, count, definitionIds, queueIds, buildNumber2, minFinishTime2, maxFinishTime2, requestedFor2, (Microsoft.TeamFoundation.Build.WebApi.BuildReason) reasonFilter2, statusFilter2, resultFilter2, (Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder) queryOrder1, (Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption) num3, maxBuildsPerDefinition2);
              buildList.AddRange(builds);
            }
          }
          else
          {
            maxBuildsPerDefinition = new int?(Math.Min(maxBuildsPerDefinition.Value, top));
            List<Microsoft.TeamFoundation.Build2.Server.BuildDefinition> definitionsByIds = this.DefinitionService.GetDefinitionsByIds(this.TfsRequestContext, this.ProjectId, (IEnumerable<int>) intList1.ToList<int>());
            List<int> intList2 = new List<int>();
            List<int> list = intList1.ToList<int>();
            foreach (Microsoft.TeamFoundation.Build2.Server.BuildDefinition buildDefinition in definitionsByIds)
            {
              intList2.Add(buildDefinition.Id);
              list.Remove(buildDefinition.Id);
            }
            if (intList2.Count > 0)
            {
              List<Microsoft.TeamFoundation.Build.WebApi.Build> buildList = source;
              IBuildService buildService = this.BuildService;
              IVssRequestContext tfsRequestContext = this.TfsRequestContext;
              Guid projectId = this.ProjectId;
              int count = top;
              List<int> definitionIds = intList2;
              IList<int> queueIds = int32List;
              string buildNumber3 = buildNumber;
              DateTime? minFinishTime3 = minFinishTime;
              DateTime? maxFinishTime3 = maxFinishTime;
              string requestedFor3 = requestedFor;
              Microsoft.TeamFoundation.Build.WebApi.BuildReason? nullable5 = reasonFilter;
              Microsoft.TeamFoundation.Build2.Server.BuildReason? reasonFilter3 = nullable5.HasValue ? new Microsoft.TeamFoundation.Build2.Server.BuildReason?((Microsoft.TeamFoundation.Build2.Server.BuildReason) nullable5.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.BuildReason?();
              Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable6 = statusFilter;
              Microsoft.TeamFoundation.Build2.Server.BuildStatus? statusFilter3 = nullable6.HasValue ? new Microsoft.TeamFoundation.Build2.Server.BuildStatus?((Microsoft.TeamFoundation.Build2.Server.BuildStatus) nullable6.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.BuildStatus?();
              Microsoft.TeamFoundation.Build.WebApi.BuildResult? nullable7 = resultFilter;
              Microsoft.TeamFoundation.Build2.Server.BuildResult? resultFilter3 = nullable7.HasValue ? new Microsoft.TeamFoundation.Build2.Server.BuildResult?((Microsoft.TeamFoundation.Build2.Server.BuildResult) nullable7.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.BuildResult?();
              IEnumerable<string> propertyFilters = ArtifactPropertyKinds.AsPropertyFilters(properties);
              IList<string> stringList = RestHelpers.ToStringList(tagFilters);
              int serverBuildQueryOrder = (int) queryOrder.ToServerBuildQueryOrder();
              int num4 = (int) deletedFilter;
              string branchName2 = branchName;
              int? maxBuildsPerDefinition3 = new int?(maxBuildsPerDefinition.Value);
              IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> collection = buildService.GetBuildsLegacy(tfsRequestContext, projectId, count, (IEnumerable<int>) definitionIds, (IEnumerable<int>) queueIds, buildNumber3, minFinishTime3, maxFinishTime3, requestedFor3, reasonFilter3, statusFilter3, resultFilter3, propertyFilters, (IEnumerable<string>) stringList, (Microsoft.TeamFoundation.Build2.Server.BuildQueryOrder) serverBuildQueryOrder, (Microsoft.TeamFoundation.Build2.Server.QueryDeletedOption) num4, branchName: branchName2, maxBuildsPerDefinition: maxBuildsPerDefinition3).Select<BuildData, Microsoft.TeamFoundation.Build.WebApi.Build>((Func<BuildData, Microsoft.TeamFoundation.Build.WebApi.Build>) (x => batchConverter.ConvertBuild(x, this.GetApiResourceVersion().ApiVersion, true)));
              buildList.AddRange(collection);
            }
            if (string.IsNullOrEmpty(branchName) && list.Count > 0)
            {
              IXamlBuildProvider service = this.TfsRequestContext.GetService<IXamlBuildProvider>();
              source.AddRange(service.GetBuilds(this.TfsRequestContext, this.ProjectInfo, top, (IList<int>) list, int32List, buildNumber, minFinishTime, maxFinishTime, requestedFor, (Microsoft.TeamFoundation.Build.WebApi.BuildReason) ((int) reasonFilter ?? 2031), statusFilter, resultFilter, queryOrder, deletedFilter, new int?(maxBuildsPerDefinition.Value)));
            }
          }
          if (queryOrder == Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeAscending)
            source.Sort((Comparison<Microsoft.TeamFoundation.Build.WebApi.Build>) ((b1, b2) =>
            {
              DateTime? finishTime = b1.FinishTime;
              DateTime t1 = finishTime ?? DateTime.MaxValue;
              finishTime = b2.FinishTime;
              DateTime t2 = finishTime ?? DateTime.MaxValue;
              return DateTime.Compare(t1, t2);
            }));
          else
            source.Sort((Comparison<Microsoft.TeamFoundation.Build.WebApi.Build>) ((b1, b2) =>
            {
              DateTime? finishTime = b2.FinishTime;
              DateTime t1 = finishTime ?? DateTime.MaxValue;
              finishTime = b1.FinishTime;
              DateTime t2 = finishTime ?? DateTime.MaxValue;
              return DateTime.Compare(t1, t2);
            }));
          HttpResponseMessage response = this.Request.CreateResponse<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(HttpStatusCode.OK, source.Take<Microsoft.TeamFoundation.Build.WebApi.Build>(top).Select<Microsoft.TeamFoundation.Build.WebApi.Build, Microsoft.TeamFoundation.Build.WebApi.Build>((Func<Microsoft.TeamFoundation.Build.WebApi.Build, Microsoft.TeamFoundation.Build.WebApi.Build>) (b => this.FixResource(b))).ToList<Microsoft.TeamFoundation.Build.WebApi.Build>());
          if (source.Count > top && this.IsContinuationSupported(statusFilter) && !maxBuildsPerDefinition.HasValue)
            this.SetContinuationToken(response, source[top]);
          return response;
        }
      }
      return this.Request.CreateResponse<IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build>>(HttpStatusCode.OK, Enumerable.Empty<Microsoft.TeamFoundation.Build.WebApi.Build>());
    }

    [HttpPost]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.Build.WebApi.Build), null, null)]
    public HttpResponseMessage QueueBuild(
      [FromBody] Microsoft.TeamFoundation.Build.WebApi.Build build,
      [FromUri] bool ignoreWarnings = false,
      [FromUri] string checkInTicket = null,
      [FromUri] Microsoft.TeamFoundation.Build.WebApi.DefinitionType? type = null)
    {
      this.CheckRequestContent((object) build);
      this.ValidateBuild(build);
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition buildDefinition = (Microsoft.TeamFoundation.Build2.Server.BuildDefinition) null;
      XamlBuildDefinition xamlBuildDefinition = (XamlBuildDefinition) null;
      Microsoft.TeamFoundation.Build.WebApi.Build build1 = (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      Microsoft.TeamFoundation.Build.WebApi.DefinitionType? nullable = type;
      if (((int) nullable ?? 2) == 2)
        buildDefinition = this.DefinitionService.GetDefinition(this.TfsRequestContext, this.ProjectId, build.Definition.Id);
      if (buildDefinition != null)
      {
        if (build.Controller != null)
          throw new InvalidBuildException(Resources.BuildControllerMustNotBeSpecifiedForBuild());
        if (build.Queue != null)
          build.Queue.ResolveToProject(this.TfsRequestContext, this.ProjectId);
        BuildRequestValidationFlags validationFlags = ignoreWarnings ? BuildRequestValidationFlags.None : BuildRequestValidationFlags.WarningsAsErrors;
        BuildData srvBuild = build.ToBuildServerBuildData(this.TfsRequestContext);
        HashSet<string> internalRuntimeVariables = new HashSet<string>();
        srvBuild.Parameters = BuildRequestHelper.SanitizeDiagnosticsParameters(srvBuild.Parameters, out internalRuntimeVariables);
        IEnumerable<IBuildRequestValidator> validators = BuildRequestValidatorProvider.GetValidators(new BuildRequestValidationOptions()
        {
          RequireOnlineAgent = true,
          WarnIfNoMatchingAgent = true,
          ValidateSourceVersionFormat = true,
          InternalRuntimeVariables = internalRuntimeVariables
        });
        build1 = this.BuildService.QueueBuild(this.TfsRequestContext, srvBuild, validators, validationFlags, checkInTicket, callingMethod: nameof (QueueBuild), callingFile: "D:\\a\\_work\\1\\s\\Tfs\\Service\\Build2\\Web\\Server\\Controllers\\2.0\\Builds2Controller.cs").ToWebApiBuild(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
        if (build1 != null)
          this.TfsRequestContext.TraceInfo(12030196, "TriggeredBuilds", "Builds2Controller: Queued manual build {0} requested for {1} with definition {2} using branch {3} version {4}", (object) build1.Id, (object) build1.RequestedFor?.DisplayName, (object) build1.Definition?.Id, (object) build1.SourceBranch, (object) build1.SourceVersion);
        else
          this.TfsRequestContext.TraceInfo(12030196, "TriggeredBuilds", "Builds2Controller: Manual call to queue a build requested for {0} with definition {1} using branch {2} did not actually queue a build", (object) build.RequestedFor?.DisplayName, (object) build.Definition?.Id, (object) srvBuild.SourceBranch);
        this.TraceQueueBuildValidationResults(srvBuild);
        if (srvBuild.ValidationResults != null && srvBuild.ValidationResults.Any<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult>((Func<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, bool>) (vr => vr.Result == Microsoft.TeamFoundation.Build2.Server.ValidationResult.Error)))
          return this.Request.CreateResponse<BuildRequestValidationFailedException>(HttpStatusCode.BadRequest, new BuildRequestValidationFailedException(Resources.BuildRequestValidationFailed(), srvBuild.ValidationResults.Select<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>((Func<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>) (vr => BuildRequestValidationResultExtensions.ToWebApiBuildRequestValidationResult(vr, srvBuild.ToSecuredObject()))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>()));
        if (!ignoreWarnings && srvBuild.ValidationResults.Any<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult>((Func<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, bool>) (vr => vr.Result == Microsoft.TeamFoundation.Build2.Server.ValidationResult.Warning && !vr.Ignorable)))
          return this.Request.CreateResponse<BuildRequestValidationFailedException>(HttpStatusCode.Conflict, new BuildRequestValidationFailedException(Resources.BuildRequestValidationFailed(), srvBuild.ValidationResults.Select<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>((Func<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>) (vr => BuildRequestValidationResultExtensions.ToWebApiBuildRequestValidationResult(vr, srvBuild.ToSecuredObject()))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>()));
      }
      else
      {
        nullable = type;
        if (((int) nullable ?? 1) == 1)
        {
          build.Definition.Type = Microsoft.TeamFoundation.Build.WebApi.DefinitionType.Xaml;
          xamlBuildDefinition = this.TfsRequestContext.GetService<IXamlDefinitionProvider>().GetDefinition(this.TfsRequestContext, this.ProjectInfo, build.Definition.Id);
          if (xamlBuildDefinition != null)
          {
            if (build.Queue != null)
              throw new InvalidBuildException(Resources.BuildQueueMustNotBeSpecifiedForXamlBuild());
            build1 = this.TfsRequestContext.GetService<IXamlBuildProvider>().QueueBuild(this.TfsRequestContext, this.ProjectInfo, build, checkInTicket);
          }
        }
      }
      if (buildDefinition == null && xamlBuildDefinition == null)
        throw new DefinitionNotFoundException(Resources.DefinitionNotFound((object) build.Definition.Id));
      return this.Request.CreateResponse<Microsoft.TeamFoundation.Build.WebApi.Build>(HttpStatusCode.OK, this.FixResource(build1));
    }

    [HttpPatch]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.Build.WebApi.Build), null, null)]
    public HttpResponseMessage UpdateBuild(int buildId, Microsoft.TeamFoundation.Build.WebApi.Build build, Microsoft.TeamFoundation.Build.WebApi.DefinitionType? type = null)
    {
      this.CheckRequestContent((object) build);
      this.ValidateBuildId(build, buildId);
      this.ValidateProject(build);
      Microsoft.TeamFoundation.Build.WebApi.Build build1;
      if (!type.HasValue)
      {
        try
        {
          IBuildService buildService = this.BuildService;
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
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
          bool? retainedByRelease = new bool?();
          build1 = buildService.UpdateBuild(tfsRequestContext, id, buildId1, buildNumber, startTime, finishTime, status2, sourceBranch, sourceVersion, result2, keepForever, retainedByRelease).ToWebApiBuild(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
        }
        catch (BuildNotFoundException ex)
        {
          build1 = this.TfsRequestContext.GetService<IXamlBuildProvider>().UpdateBuild(this.TfsRequestContext, this.ProjectInfo, build);
          if (build1 == null)
            return this.Request.CreateResponse(HttpStatusCode.NoContent);
        }
      }
      else
      {
        Microsoft.TeamFoundation.Build.WebApi.DefinitionType? nullable = type;
        Microsoft.TeamFoundation.Build.WebApi.DefinitionType definitionType = Microsoft.TeamFoundation.Build.WebApi.DefinitionType.Xaml;
        if (nullable.GetValueOrDefault() == definitionType & nullable.HasValue)
        {
          build1 = this.TfsRequestContext.GetService<IXamlBuildProvider>().UpdateBuild(this.TfsRequestContext, this.ProjectInfo, build);
          if (build1 == null)
            return this.Request.CreateResponse(HttpStatusCode.NoContent);
        }
        else
        {
          IBuildService buildService = this.BuildService;
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          Guid id = build.Project.Id;
          int buildId2 = buildId;
          string buildNumber = build.BuildNumber;
          DateTime? startTime = build.StartTime;
          DateTime? finishTime = build.FinishTime;
          Microsoft.TeamFoundation.Build.WebApi.BuildStatus? status3 = build.Status;
          Microsoft.TeamFoundation.Build2.Server.BuildStatus? status4 = status3.HasValue ? new Microsoft.TeamFoundation.Build2.Server.BuildStatus?((Microsoft.TeamFoundation.Build2.Server.BuildStatus) status3.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.BuildStatus?();
          string sourceBranch = build.SourceBranch;
          string sourceVersion = build.SourceVersion;
          Microsoft.TeamFoundation.Build.WebApi.BuildResult? result3 = build.Result;
          Microsoft.TeamFoundation.Build2.Server.BuildResult? result4 = result3.HasValue ? new Microsoft.TeamFoundation.Build2.Server.BuildResult?((Microsoft.TeamFoundation.Build2.Server.BuildResult) result3.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.BuildResult?();
          bool? keepForever = build.KeepForever;
          bool? retainedByRelease = new bool?();
          build1 = buildService.UpdateBuild(tfsRequestContext, id, buildId2, buildNumber, startTime, finishTime, status4, sourceBranch, sourceVersion, result4, keepForever, retainedByRelease).ToWebApiBuild(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
        }
      }
      if (build1 == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      build1.AddLinks(this.TfsRequestContext);
      return this.Request.CreateResponse<Microsoft.TeamFoundation.Build.WebApi.Build>(HttpStatusCode.OK, this.FixResource(build1));
    }

    [HttpDelete]
    public void DeleteBuild(int buildId, Microsoft.TeamFoundation.Build.WebApi.DefinitionType? type = null) => this.ExecuteCompatMethod(type, (Action) (() => this.BuildService.DeleteBuild(this.TfsRequestContext, this.ProjectId, buildId)), (Action) (() => this.TfsRequestContext.GetService<IXamlBuildProvider>().DeleteBuild(this.TfsRequestContext, this.ProjectInfo, buildId)));

    private void SetContinuationToken(HttpResponseMessage responseMessage, Microsoft.TeamFoundation.Build.WebApi.Build nextBuild)
    {
      if (nextBuild == null)
        return;
      string str = new BuildsContinuationToken(nextBuild.QueueTime, nextBuild.StartTime, nextBuild.FinishTime).ToString();
      if (string.IsNullOrEmpty(str))
        return;
      responseMessage.Headers.Add("x-ms-continuationtoken", str);
    }

    private bool IsContinuationSupported(Microsoft.TeamFoundation.Build.WebApi.BuildStatus? buildStatus)
    {
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable = buildStatus;
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus1 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed;
      return nullable.GetValueOrDefault() == buildStatus1 & nullable.HasValue;
    }

    private void ValidateBuild(Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build.WebApi.Build>(build, nameof (build));
      ArgumentUtility.CheckForNull<DefinitionReference>(build.Definition, "build.Definition");
      this.ValidateProject(build);
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

    private Microsoft.TeamFoundation.Build.WebApi.Build FixResource(Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
      build.Reason = this.FixBuildReason(build.Reason);
      return build;
    }
  }
}
