// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildApiController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ApiTelemetry(true, false)]
  [CheckWellFormedProject]
  public abstract class BuildApiController : TfsProjectApiController
  {
    private ApiResourceVersion m_resourceVersion;
    private static readonly Version Tfs2017Update2Version = new Version(3, 2);
    private static readonly Version ApiVersion41 = new Version(4, 1);
    private static readonly Version ApiVersion52 = new Version(5, 2);
    public const string ContinuationTokenHeaderName = "x-ms-continuationtoken";
    public const string JsonPatchMediaType = "application/json-patch+json";

    public override string TraceArea => "Build";

    public override string ActivityLogArea => "Build";

    protected void CheckRequestContent(object deserializedContent)
    {
      if (deserializedContent == null)
        throw new RequestContentException(this.Request.Content.Headers.ContentLength.GetValueOrDefault() == 0L ? Resources.MissingRequestContent() : Resources.InvalidRequestContent());
    }

    protected virtual Microsoft.TeamFoundation.Build.WebApi.BuildReason FixBuildReason(
      Microsoft.TeamFoundation.Build.WebApi.BuildReason buildReason)
    {
      ApiResourceVersion apiResourceVersion = this.GetApiResourceVersion();
      if ((buildReason & Microsoft.TeamFoundation.Build.WebApi.BuildReason.PullRequest) == Microsoft.TeamFoundation.Build.WebApi.BuildReason.PullRequest && this.IsResourceVersionOlderThan(apiResourceVersion, 3, BuildApiController.Tfs2017Update2Version))
        buildReason = (buildReason | Microsoft.TeamFoundation.Build.WebApi.BuildReason.ValidateShelveset) & ~Microsoft.TeamFoundation.Build.WebApi.BuildReason.PullRequest;
      if ((buildReason & Microsoft.TeamFoundation.Build.WebApi.BuildReason.BuildCompletion) == Microsoft.TeamFoundation.Build.WebApi.BuildReason.BuildCompletion && this.IsResourceVersionOlderThan(apiResourceVersion, 4, BuildApiController.ApiVersion41))
        buildReason = (buildReason | Microsoft.TeamFoundation.Build.WebApi.BuildReason.Manual) & ~Microsoft.TeamFoundation.Build.WebApi.BuildReason.BuildCompletion;
      if ((buildReason & Microsoft.TeamFoundation.Build.WebApi.BuildReason.ResourceTrigger) == Microsoft.TeamFoundation.Build.WebApi.BuildReason.ResourceTrigger && this.IsResourceVersionOlderThan(apiResourceVersion, 5, BuildApiController.ApiVersion52))
        buildReason = (buildReason | Microsoft.TeamFoundation.Build.WebApi.BuildReason.Manual) & ~Microsoft.TeamFoundation.Build.WebApi.BuildReason.ResourceTrigger;
      return buildReason;
    }

    protected virtual Microsoft.TeamFoundation.Build2.Server.BuildReason FixBuildReason(
      Microsoft.TeamFoundation.Build2.Server.BuildReason buildReason)
    {
      ApiResourceVersion apiResourceVersion = this.GetApiResourceVersion();
      if ((buildReason & Microsoft.TeamFoundation.Build2.Server.BuildReason.PullRequest) == Microsoft.TeamFoundation.Build2.Server.BuildReason.PullRequest && this.IsResourceVersionOlderThan(apiResourceVersion, 3, BuildApiController.Tfs2017Update2Version))
        buildReason = (buildReason | Microsoft.TeamFoundation.Build2.Server.BuildReason.ValidateShelveset) & ~Microsoft.TeamFoundation.Build2.Server.BuildReason.PullRequest;
      if ((buildReason & Microsoft.TeamFoundation.Build2.Server.BuildReason.BuildCompletion) == Microsoft.TeamFoundation.Build2.Server.BuildReason.BuildCompletion && this.IsResourceVersionOlderThan(apiResourceVersion, 4, BuildApiController.ApiVersion41))
        buildReason = (buildReason | Microsoft.TeamFoundation.Build2.Server.BuildReason.Manual) & ~Microsoft.TeamFoundation.Build2.Server.BuildReason.BuildCompletion;
      if ((buildReason & Microsoft.TeamFoundation.Build2.Server.BuildReason.ResourceTrigger) == Microsoft.TeamFoundation.Build2.Server.BuildReason.ResourceTrigger && this.IsResourceVersionOlderThan(apiResourceVersion, 5, BuildApiController.ApiVersion52))
        buildReason = (buildReason | Microsoft.TeamFoundation.Build2.Server.BuildReason.Manual) & ~Microsoft.TeamFoundation.Build2.Server.BuildReason.ResourceTrigger;
      return buildReason;
    }

    protected virtual Microsoft.TeamFoundation.Build.WebApi.BuildStatus HandleBuildMinMaxTimeSetAndGetStatus(
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? statusFilter,
      Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder buildQueryOrder)
    {
      if (statusFilter.HasValue)
      {
        Microsoft.TeamFoundation.Build.WebApi.BuildStatus? nullable = statusFilter;
        Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed;
        if (!(nullable.GetValueOrDefault() == buildStatus & nullable.HasValue))
          throw new InvalidBuildQueryException(Resources.InvalidStatusFilterFinishTimeCombination((object) statusFilter));
      }
      statusFilter = new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed);
      return statusFilter.Value;
    }

    protected virtual void VerifyBuildQueryOrder(Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder buildQueryOrder)
    {
      if (!buildQueryOrder.HasFlag((System.Enum) Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeAscending) && !buildQueryOrder.HasFlag((System.Enum) Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending))
        throw new InvalidBuildException(Resources.BuildQueryOrderOnlyFinishTimeIsSupported((object) System.Enum.GetName(typeof (Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder), (object) buildQueryOrder)));
    }

    protected Microsoft.TeamFoundation.Build.WebApi.BuildReason? FixBuildReasonFilter(
      Microsoft.TeamFoundation.Build.WebApi.BuildReason? buildReason)
    {
      if (buildReason.HasValue && this.IsResourceVersionOlderThan(this.GetApiResourceVersion(), 3, BuildApiController.Tfs2017Update2Version))
      {
        if ((buildReason.Value & Microsoft.TeamFoundation.Build.WebApi.BuildReason.ValidateShelveset) != Microsoft.TeamFoundation.Build.WebApi.BuildReason.ValidateShelveset)
        {
          Microsoft.TeamFoundation.Build.WebApi.BuildReason? nullable = buildReason;
          Microsoft.TeamFoundation.Build.WebApi.BuildReason buildReason1 = Microsoft.TeamFoundation.Build.WebApi.BuildReason.Manual | Microsoft.TeamFoundation.Build.WebApi.BuildReason.IndividualCI | Microsoft.TeamFoundation.Build.WebApi.BuildReason.BatchedCI | Microsoft.TeamFoundation.Build.WebApi.BuildReason.Schedule | Microsoft.TeamFoundation.Build.WebApi.BuildReason.UserCreated | Microsoft.TeamFoundation.Build.WebApi.BuildReason.CheckInShelveset | Microsoft.TeamFoundation.Build.WebApi.BuildReason.BuildCompletion | Microsoft.TeamFoundation.Build.WebApi.BuildReason.ResourceTrigger;
          if (!(nullable.GetValueOrDefault() == buildReason1 & nullable.HasValue))
            goto label_4;
        }
        Microsoft.TeamFoundation.Build.WebApi.BuildReason? nullable1 = buildReason;
        buildReason = nullable1.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.BuildReason?(nullable1.GetValueOrDefault() | Microsoft.TeamFoundation.Build.WebApi.BuildReason.PullRequest) : new Microsoft.TeamFoundation.Build.WebApi.BuildReason?();
      }
label_4:
      return buildReason;
    }

    protected void FixOutgoingDefinition(Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition)
    {
      if (definition == null)
        return;
      ApiResourceVersion apiResourceVersion = this.GetApiResourceVersion();
      if (definition.Repository != null)
        this.TfsRequestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(this.TfsRequestContext, definition.Repository.Type, false)?.FixOutgoingDefinition(this.TfsRequestContext, definition, apiResourceVersion);
      if (this.IsResourceVersionOlderThan(apiResourceVersion, 3, BuildApiController.Tfs2017Update2Version))
        definition.Triggers.RemoveAll((Predicate<Microsoft.TeamFoundation.Build2.Server.BuildTrigger>) (trigger => trigger.TriggerType == Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.PullRequest));
      if (!this.IsResourceVersionOlderThan(apiResourceVersion, 4, BuildApiController.Tfs2017Update2Version))
        return;
      foreach (Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep allStep in definition.AllSteps())
        allStep.Condition = (string) null;
    }

    protected virtual void FixIncomingDefinition(Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition, bool isUpdate)
    {
      if (definition == null)
        return;
      ApiResourceVersion apiResourceVersion = this.GetApiResourceVersion();
      definition.Triggers.RemoveAll((Predicate<Microsoft.TeamFoundation.Build2.Server.BuildTrigger>) (trigger => trigger == null));
      if (definition.Process is Microsoft.TeamFoundation.Build2.Server.DesignerProcess process)
        process.Phases.RemoveAll((Predicate<Microsoft.TeamFoundation.Build2.Server.Phase>) (phase => phase == null));
      if (definition.Repository?.Type != null)
        this.TfsRequestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(this.TfsRequestContext, definition.Repository.Type, false)?.FixIncomingDefinition(this.TfsRequestContext, definition, apiResourceVersion, isUpdate);
      if (this.IsResourceVersionOlderThan(apiResourceVersion, 3, BuildApiController.Tfs2017Update2Version))
        definition.Triggers.RemoveAll((Predicate<Microsoft.TeamFoundation.Build2.Server.BuildTrigger>) (trigger => trigger.TriggerType == Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.PullRequest));
      if (this.IsResourceVersionOlderThan(apiResourceVersion, 4, BuildApiController.Tfs2017Update2Version))
      {
        foreach (Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep allStep in definition.AllSteps())
          allStep.Condition = (string) null;
      }
      else
      {
        foreach (Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep allStep in definition.AllSteps())
          allStep.AlwaysRun = TaskConditions.IsLegacyAlwaysRun(allStep.Condition);
      }
      definition.Options.RemoveAll((Predicate<Microsoft.TeamFoundation.Build2.Server.BuildOption>) (o => o == null || o.Definition == null));
      if (definition.Options.Count <= 0)
        return;
      using (IDisposableReadOnlyList<IBuildOption> extensions = this.TfsRequestContext.GetExtensions<IBuildOption>())
      {
        List<IBuildOption> list = extensions.OrderBy<IBuildOption, int>((Func<IBuildOption, int>) (option => option.GetOrdinal())).ToList<IBuildOption>();
        definition.ModernizeOptions(this.TfsRequestContext, (IList<IBuildOption>) list);
      }
    }

    protected void TraceQueueBuildValidationResults(BuildData serverBuild)
    {
      IEnumerable<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult> source = serverBuild.ValidationResults.Where<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult>((Func<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, bool>) (vr => vr.Result != 0));
      if (!source.Any<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult>())
        return;
      string str = string.Join(Environment.NewLine, source.Select<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, string>((Func<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, string>) (vr => vr.Result.ToString() + ": " + vr.Message)));
      this.TfsRequestContext.Trace(12030105, TraceLevel.Warning, "Build2", this.GetType().Name, "Validation warning and error messages when queuing build: {0}", (object) (Environment.NewLine + str));
    }

    protected IBuildService BuildService => this.TfsRequestContext.GetService<IBuildService>();

    internal IBuildServiceInternal InternalBuildService => this.TfsRequestContext.GetService<IBuildServiceInternal>();

    protected IBuildDefinitionService DefinitionService => this.TfsRequestContext.GetService<IBuildDefinitionService>();

    protected ITagService TagService => this.TfsRequestContext.GetService<ITagService>();

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.AccessDeniedException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<ActionDeniedBySubscriberException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<AgentsNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArtifactNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ArtifactTypeNotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<UnauthorizedRequestException>(HttpStatusCode.Unauthorized);
      exceptionMap.AddStatusCode<BranchNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<BuildNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<BuildNumberFormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BuildOptionNotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BuildOrchestrationExistsException>(HttpStatusCode.BadGateway);
      exceptionMap.AddStatusCode<BuildRepositoryTypeNotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BuildRequestValidationFailedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BuildRequestValidationWarningException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<CannotDeleteDefinitionWithRetainedBuildsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<CannotDeleteRetainedBuildException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CouldNotRetrieveSourceVersionDisplayUrlException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<DefinitionDisabledException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<DefinitionExistsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<DefinitionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<DefinitionNotMatchedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<DefinitionTemplateExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<DuplicateBuildSpecException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<EndpointAccessDeniedException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<ExternalSourceProviderException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FolderNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<FolderExistsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<IndexOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidArtifactDataException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidBuildException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidBuildQueryException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidDefinitionException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidDefinitionQueryException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidEndpointUrlException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidFolderException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidLogLocationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidPathException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidProjectException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidSourceLabelFormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<MetaTaskDefinitionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<MetricAggregationTypeNotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<MissingEndpointInformationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<MissingRepositoryException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<MissingRequiredParameterException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<MissingRequiredHeaderException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<NotSupportedOnXamlBuildException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<PipelineValidationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ProjectConflictException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ProjectDoesNotExistException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ProjectDoesNotExistWithNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<PropertiesCollectionPatchException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<PublisherNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<QueueExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<QueueNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ReportFormatTypeNotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ReportStreamNotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<RepositoryInformationInvalidException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<RequestContentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<RetentionLeaseNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<RouteIdConflictException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskAgentPoolExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskAgentQueueNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Build.WebApi.TimelineNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<UriFormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<VariableNameIsReservedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<VariableGroupsAccessDeniedException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<InvalidGitVersionSpec>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidTimelineRecordStateChange>(HttpStatusCode.BadRequest);
      foreach (IBuildSourceProvider sourceProvider in this.TfsRequestContext.GetService<IBuildSourceProviderService>().GetSourceProviders(this.TfsRequestContext))
        sourceProvider.InitializeExceptionMap(exceptionMap);
    }

    protected ApiResourceVersion GetApiResourceVersion()
    {
      if (this.m_resourceVersion == null)
        this.m_resourceVersion = this.Request.GetApiResourceVersion(VssRestApiVersionsRegistry.GetLatestReleasedVersion());
      return this.m_resourceVersion;
    }

    protected bool IsResourceVersionOlderThan(
      ApiResourceVersion currentApiResourceVersion,
      int resourceVersionToCompare,
      Version versionToCompare)
    {
      return currentApiResourceVersion.ApiVersion.Major <= versionToCompare.Major && (currentApiResourceVersion.ApiVersion.Major != versionToCompare.Major || currentApiResourceVersion.ApiVersion.Minor <= versionToCompare.Minor && (currentApiResourceVersion.ApiVersion.Minor != versionToCompare.Minor || currentApiResourceVersion.IsPreview)) && currentApiResourceVersion.ResourceVersion < resourceVersionToCompare;
    }
  }
}
