// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Build.BuildArtifact
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi;
using Microsoft.VisualStudio.ExternalProviders.Common;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.FileContainer.Client;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Git;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.GitHub;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Build
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Required use of types")]
  public class BuildArtifact : FirstPartyArtifactTypeBase
  {
    private const string IsMultiDefinitionTypeInputId = "IsMultiDefinitionType";
    private const string ArtifactProjectInputId = "project";
    private const string ArtifactDefinitionInputId = "definition";
    private const string ArtifactMultipleDefinitionsInputId = "definitions";
    private const string ArtifactRepositoryInputId = "repository";
    private const string DefaultVersionTypeInputId = "defaultVersionType";
    private const string DefaultVersionBranchInputId = "defaultVersionBranch";
    private const string DefaultVersionTagsInputId = "defaultVersionTags";
    private const string DefaultVersionSpecificInputId = "defaultVersionSpecific";
    private const string ConnectionId = "connection";
    private const string VisibleRule = "visibleRule";
    private const string DisableRule = "disableRule";
    private const string IsSearchable = "isSearchable";
    private const string ArtifactId = "artifacts";
    private const int DefaultMaximumDefintionsToFetch = 20000;
    private const string HasCustomStorageArtifactsInputId = "hasCustomStorageArtifacts";
    private const string RootIdKey = "RootId";
    private const string ManifestName = "manifest";
    private const string PipelineArtifactType = "PipelineArtifact";
    protected const int ProjectIdLengthMin = 1;
    protected const int ProjectIdLengthMax = 256;
    protected const int BuildDefinitionNameLengthMin = 1;
    protected const int BuildDefinitionNameLengthMax = 260;
    protected const int RepositoryNameLengthMin = 1;
    protected const int RepositoryNameLengthMax = 260;
    public const string WorkItemsWebUrlFieldKey = "Release.WorkItemWebUrl";
    public const string WorkItemsTitleFieldKey = "System.Title";
    public const string WorkItemsStateFieldKey = "System.State";
    public const string WorkItemsTypeKey = "System.WorkItemType";
    public const string WorkItemsTeamProject = "System.TeamProject";
    private BuildArtifact.BuildDefinitionsRetrieverPaged buildDefinitionsRetrieverPaged;
    private readonly Func<IVssRequestContext, Guid, int, IList<InputValue>> branchesRetriever;
    private readonly Func<IVssRequestContext, Guid, IList<InputValue>> tagsRetriever;
    private readonly Func<IVssRequestContext, Guid, int[], BuildStatus, BuildResult, string, IEnumerable<string>, int?, IList<Microsoft.TeamFoundation.Build.WebApi.Build>> buildsRetriever;
    private readonly Func<IVssRequestContext, Guid, int, Microsoft.TeamFoundation.Build.WebApi.Build> buildRetriever;
    private readonly Func<IVssRequestContext, Guid, int[], string, Microsoft.TeamFoundation.Build.WebApi.Build> buildRetrieverForBuildNumber;
    private readonly Func<IVssRequestContext, Guid, string, GitRepository> repositoryRetriever;
    private readonly Func<IVssRequestContext, Guid, IList<GitRepository>> repositoriesRetriever;
    private readonly Func<IVssRequestContext, Guid, int, IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>> artifactsRetriever;
    private readonly Func<IVssRequestContext, Guid, Uri, string, IList<FileContainerItem>> artifactItemsRetriever;
    private readonly Func<IVssRequestContext, Guid, Uri, string, string> artifactContentRetriever;
    private readonly Func<IVssRequestContext, Guid, int, int, int, IList<Microsoft.TeamFoundation.Build.WebApi.Change>> changesRetriever;
    private readonly Func<IVssRequestContext, Guid, int, int, int, IList<ResourceRef>> witRetriever;
    [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "Easier testability")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed.")]
    [StaticSafe]
    public static Func<IVssRequestContext, Guid, int, bool> IsXamlBuildArtifact = (Func<IVssRequestContext, Guid, int, bool>) ((context, projectId, buildDefinitionId) =>
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      try
      {
        return context.Elevate().GetClient<XamlBuildHttpClient>().GetDefinitionAsync(projectId, buildDefinitionId).GetResult<DefinitionReference>(context.CancellationToken).Type == DefinitionType.Xaml;
      }
      catch (ReleaseManagementExternalServiceException ex)
      {
        if (ex.InnerException != null && ex.InnerException.GetType() == typeof (DefinitionNotFoundException))
          return false;
        throw;
      }
    });
    private static readonly Regex IssueRegex = new Regex("(?<reponame>[^\\#^\\s^\\,]*)(?:\\#|GH-)(?<issue>[0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1.0));
    private static readonly Regex JiraIssueRegex = new Regex("(?:(?<=[\\s\\p{P}])|^)(?<issueKey>\\p{Lu}[\\p{Lu}\\d_]+-\\d+)(?:(?=[\\s\\p{P}])|$)", RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1.0));
    private const string PublishTelemetryAreaName = "ReleaseMangementWorkItems";
    private const string PublishGitHubIssuesFeature = "GitHubIssues";
    private const string PublishGitHubWorkItemsFeature = "GitHubWorkItems";
    private const string PublishDeploymentStatusToWorkItems = "PublishDeploymentStatusToWorkItems";
    private const string GetBuildsQueryTopRegistryKey = "/Service/ReleaseManagement/Settings/getBuilds/top";
    private const string GetWorkItemsToLinkRegistryKey = "/Service/ReleaseManagement/Settings/MaxWorkItemsToLink";
    private const string PublishDeploymentStatusToWorkItemsContributionId = "ms.vss-work-web.work-item-write-external-deployments-data-provider";
    private const string ArtifactProviderKey = "github.com";
    private const int MaxWorkItemsToLinkForBoardsIntegration = 1000;
    private const string FolderPathSeparator = "\\";
    private bool isCustomArtifactTypeSupported;

    public override string Name => "Build";

    public override string DisplayName => Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BuildArtifactDisplayName;

    public override string EndpointTypeId => "Build";

    public override Guid ArtifactDownloadTaskId { get; }

    public override IDictionary<string, Guid> ArtifactDownloadTaskIds { get; }

    public override IDictionary<Guid, IDictionary<string, string>> TaskInputMappings { get; }

    public override IDictionary<string, string> TaskInputDefaultValues { get; }

    public override string Type { get; }

    public override IDictionary<string, string> TaskInputMapping { get; }

    public override string UniqueSourceIdentifier => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{{{{0}}}}}:{{{{{1}}}}}", (object) "project", (object) "definition");

    public override bool IsCommitsTraceabilitySupported => true;

    public override bool IsWorkitemsTraceabilitySupported => true;

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "By design")]
    public BuildArtifact(bool isCustomArtifactTypeSupported = false)
      : this(BuildArtifact.\u003C\u003EO.\u003C0\u003E__GetDefinitionReferencesPaged ?? (BuildArtifact.\u003C\u003EO.\u003C0\u003E__GetDefinitionReferencesPaged = new BuildArtifact.BuildDefinitionsRetrieverPaged(BuildArtifact.GetDefinitionReferencesPaged)), BuildArtifact.\u003C\u003EO.\u003C1\u003E__GetBranchesInputValues ?? (BuildArtifact.\u003C\u003EO.\u003C1\u003E__GetBranchesInputValues = new Func<IVssRequestContext, Guid, int, IList<InputValue>>(BuildArtifact.GetBranchesInputValues)), BuildArtifact.\u003C\u003EO.\u003C2\u003E__GetTags ?? (BuildArtifact.\u003C\u003EO.\u003C2\u003E__GetTags = new Func<IVssRequestContext, Guid, IList<InputValue>>(BuildArtifact.GetTags)), BuildArtifact.\u003C\u003EO.\u003C3\u003E__GetBuilds ?? (BuildArtifact.\u003C\u003EO.\u003C3\u003E__GetBuilds = new Func<IVssRequestContext, Guid, int[], BuildStatus, BuildResult, string, IEnumerable<string>, int?, IList<Microsoft.TeamFoundation.Build.WebApi.Build>>(BuildArtifact.GetBuilds)), BuildArtifact.\u003C\u003EO.\u003C4\u003E__GetBuild ?? (BuildArtifact.\u003C\u003EO.\u003C4\u003E__GetBuild = new Func<IVssRequestContext, Guid, int, Microsoft.TeamFoundation.Build.WebApi.Build>(BuildArtifact.GetBuild)), BuildArtifact.\u003C\u003EO.\u003C5\u003E__GetBuildForBuildNumber ?? (BuildArtifact.\u003C\u003EO.\u003C5\u003E__GetBuildForBuildNumber = new Func<IVssRequestContext, Guid, int[], string, Microsoft.TeamFoundation.Build.WebApi.Build>(BuildArtifact.GetBuildForBuildNumber)), BuildArtifact.\u003C\u003EO.\u003C6\u003E__GetBuildArtifacts ?? (BuildArtifact.\u003C\u003EO.\u003C6\u003E__GetBuildArtifacts = new Func<IVssRequestContext, Guid, int, IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>>(BuildArtifact.GetBuildArtifacts)), BuildArtifact.\u003C\u003EO.\u003C7\u003E__GetArtifactItems ?? (BuildArtifact.\u003C\u003EO.\u003C7\u003E__GetArtifactItems = new Func<IVssRequestContext, Guid, Uri, string, IList<FileContainerItem>>(BuildArtifact.GetArtifactItems)), BuildArtifact.\u003C\u003EO.\u003C8\u003E__GetArtifactItemContent ?? (BuildArtifact.\u003C\u003EO.\u003C8\u003E__GetArtifactItemContent = new Func<IVssRequestContext, Guid, Uri, string, string>(BuildArtifact.GetArtifactItemContent)), BuildArtifact.\u003C\u003EO.\u003C9\u003E__GetRepository ?? (BuildArtifact.\u003C\u003EO.\u003C9\u003E__GetRepository = new Func<IVssRequestContext, Guid, string, GitRepository>(BuildArtifact.GetRepository)), BuildArtifact.\u003C\u003EO.\u003C10\u003E__GetRepositories ?? (BuildArtifact.\u003C\u003EO.\u003C10\u003E__GetRepositories = new Func<IVssRequestContext, Guid, IList<GitRepository>>(BuildArtifact.GetRepositories)), BuildArtifact.\u003C\u003EO.\u003C11\u003E__GetChanges ?? (BuildArtifact.\u003C\u003EO.\u003C11\u003E__GetChanges = new Func<IVssRequestContext, Guid, int, int, int, IList<Microsoft.TeamFoundation.Build.WebApi.Change>>(BuildArtifact.GetChanges)), BuildArtifact.\u003C\u003EO.\u003C12\u003E__GetWorkItems ?? (BuildArtifact.\u003C\u003EO.\u003C12\u003E__GetWorkItems = new Func<IVssRequestContext, Guid, int, int, int, IList<ResourceRef>>(BuildArtifact.GetWorkItems)), isCustomArtifactTypeSupported)
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    public BuildArtifact()
      : this(BuildArtifact.\u003C\u003EO.\u003C0\u003E__GetDefinitionReferencesPaged ?? (BuildArtifact.\u003C\u003EO.\u003C0\u003E__GetDefinitionReferencesPaged = new BuildArtifact.BuildDefinitionsRetrieverPaged(BuildArtifact.GetDefinitionReferencesPaged)), BuildArtifact.\u003C\u003EO.\u003C1\u003E__GetBranchesInputValues ?? (BuildArtifact.\u003C\u003EO.\u003C1\u003E__GetBranchesInputValues = new Func<IVssRequestContext, Guid, int, IList<InputValue>>(BuildArtifact.GetBranchesInputValues)), BuildArtifact.\u003C\u003EO.\u003C2\u003E__GetTags ?? (BuildArtifact.\u003C\u003EO.\u003C2\u003E__GetTags = new Func<IVssRequestContext, Guid, IList<InputValue>>(BuildArtifact.GetTags)), BuildArtifact.\u003C\u003EO.\u003C3\u003E__GetBuilds ?? (BuildArtifact.\u003C\u003EO.\u003C3\u003E__GetBuilds = new Func<IVssRequestContext, Guid, int[], BuildStatus, BuildResult, string, IEnumerable<string>, int?, IList<Microsoft.TeamFoundation.Build.WebApi.Build>>(BuildArtifact.GetBuilds)), BuildArtifact.\u003C\u003EO.\u003C4\u003E__GetBuild ?? (BuildArtifact.\u003C\u003EO.\u003C4\u003E__GetBuild = new Func<IVssRequestContext, Guid, int, Microsoft.TeamFoundation.Build.WebApi.Build>(BuildArtifact.GetBuild)), BuildArtifact.\u003C\u003EO.\u003C5\u003E__GetBuildForBuildNumber ?? (BuildArtifact.\u003C\u003EO.\u003C5\u003E__GetBuildForBuildNumber = new Func<IVssRequestContext, Guid, int[], string, Microsoft.TeamFoundation.Build.WebApi.Build>(BuildArtifact.GetBuildForBuildNumber)), BuildArtifact.\u003C\u003EO.\u003C6\u003E__GetBuildArtifacts ?? (BuildArtifact.\u003C\u003EO.\u003C6\u003E__GetBuildArtifacts = new Func<IVssRequestContext, Guid, int, IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>>(BuildArtifact.GetBuildArtifacts)), BuildArtifact.\u003C\u003EO.\u003C7\u003E__GetArtifactItems ?? (BuildArtifact.\u003C\u003EO.\u003C7\u003E__GetArtifactItems = new Func<IVssRequestContext, Guid, Uri, string, IList<FileContainerItem>>(BuildArtifact.GetArtifactItems)), BuildArtifact.\u003C\u003EO.\u003C8\u003E__GetArtifactItemContent ?? (BuildArtifact.\u003C\u003EO.\u003C8\u003E__GetArtifactItemContent = new Func<IVssRequestContext, Guid, Uri, string, string>(BuildArtifact.GetArtifactItemContent)), BuildArtifact.\u003C\u003EO.\u003C9\u003E__GetRepository ?? (BuildArtifact.\u003C\u003EO.\u003C9\u003E__GetRepository = new Func<IVssRequestContext, Guid, string, GitRepository>(BuildArtifact.GetRepository)), BuildArtifact.\u003C\u003EO.\u003C10\u003E__GetRepositories ?? (BuildArtifact.\u003C\u003EO.\u003C10\u003E__GetRepositories = new Func<IVssRequestContext, Guid, IList<GitRepository>>(BuildArtifact.GetRepositories)), BuildArtifact.\u003C\u003EO.\u003C11\u003E__GetChanges ?? (BuildArtifact.\u003C\u003EO.\u003C11\u003E__GetChanges = new Func<IVssRequestContext, Guid, int, int, int, IList<Microsoft.TeamFoundation.Build.WebApi.Change>>(BuildArtifact.GetChanges)), BuildArtifact.\u003C\u003EO.\u003C12\u003E__GetWorkItems ?? (BuildArtifact.\u003C\u003EO.\u003C12\u003E__GetWorkItems = new Func<IVssRequestContext, Guid, int, int, int, IList<ResourceRef>>(BuildArtifact.GetWorkItems)), false)
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    public static IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact> GetBuildArtifacts(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId)
    {
      BuildHttpClient buildHttpClient = requestContext != null ? BuildArtifact.GetBuildHttpClient(requestContext) : throw new ArgumentNullException(nameof (requestContext));
      try
      {
        return (IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>) buildHttpClient.GetArtifactsAsync(projectId, buildId).GetResult<List<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>>(requestContext.CancellationToken);
      }
      catch (ReleaseManagementExternalServiceException ex)
      {
        if (ex.InnerException is BuildNotFoundException)
        {
          XamlBuildHttpClient xamlBuildHttpClient = BuildArtifact.GetXamlBuildHttpClient(requestContext);
          return xamlBuildHttpClient != null ? (IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>) xamlBuildHttpClient.GetArtifactsAsync(projectId, buildId).GetResult<List<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>>(requestContext.CancellationToken) : (IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>) null;
        }
        throw;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions must be caught inorder to prevent them from being thrown")]
    public static GitRepository GetRepository(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId)
    {
      GitRepository repository = (GitRepository) null;
      try
      {
        repository = new GitHelper((IGitData) new GitData()).GetRepository(requestContext, projectId, repositoryId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1971046, "ReleaseManagementService", "Service", ex);
      }
      return repository;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public static GitHubHttpClient GetGitHubHttpClient(IVssRequestContext requestContext)
    {
      IGitHubAppAccessTokenProvider accessTokenProvider = GitHubHttpClientFactory.GetAppAccessTokenProvider(requestContext, GitHubAppType.Pipelines);
      if (accessTokenProvider == null)
        requestContext.Trace(ExternalProvidersTracePoints.LoadAppExtensionFailed, TraceLevel.Warning, "ReleaseManagementService", "Service", "Unable to load the {0} extension.", (object) "IGitHubAppAccessTokenProvider");
      return GitHubHttpClientFactory.Create(requestContext, accessTokenProvider);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for complexity of type")]
    protected BuildArtifact(
      BuildArtifact.BuildDefinitionsRetrieverPaged buildDefinitionsRetrieverPaged,
      Func<IVssRequestContext, Guid, int, IList<InputValue>> branchesRetriever,
      Func<IVssRequestContext, Guid, IList<InputValue>> tagsRetriever,
      Func<IVssRequestContext, Guid, int[], BuildStatus, BuildResult, string, IEnumerable<string>, int?, IList<Microsoft.TeamFoundation.Build.WebApi.Build>> buildsRetriever,
      Func<IVssRequestContext, Guid, int, Microsoft.TeamFoundation.Build.WebApi.Build> buildRetriever,
      Func<IVssRequestContext, Guid, int[], string, Microsoft.TeamFoundation.Build.WebApi.Build> buildRetrieverForBuildNumber,
      Func<IVssRequestContext, Guid, int, IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>> artifactsRetriever,
      Func<IVssRequestContext, Guid, Uri, string, IList<FileContainerItem>> artifactItemsRetriever,
      Func<IVssRequestContext, Guid, Uri, string, string> artifactContentRetriever,
      Func<IVssRequestContext, Guid, string, GitRepository> repositoryRetriever,
      Func<IVssRequestContext, Guid, IList<GitRepository>> repositoriesRetriever,
      Func<IVssRequestContext, Guid, int, int, int, IList<Microsoft.TeamFoundation.Build.WebApi.Change>> changesRetriever,
      Func<IVssRequestContext, Guid, int, int, int, IList<ResourceRef>> witRetriever,
      bool isCustomArtifactTypeSupported)
    {
      this.buildDefinitionsRetrieverPaged = buildDefinitionsRetrieverPaged;
      this.branchesRetriever = branchesRetriever;
      this.tagsRetriever = tagsRetriever;
      this.buildsRetriever = buildsRetriever;
      this.buildRetriever = buildRetriever;
      this.buildRetrieverForBuildNumber = buildRetrieverForBuildNumber;
      this.repositoryRetriever = repositoryRetriever;
      this.repositoriesRetriever = repositoriesRetriever;
      this.artifactsRetriever = artifactsRetriever;
      this.artifactItemsRetriever = artifactItemsRetriever;
      this.artifactContentRetriever = artifactContentRetriever;
      this.changesRetriever = changesRetriever;
      this.witRetriever = witRetriever;
      this.isCustomArtifactTypeSupported = isCustomArtifactTypeSupported;
    }

    public static InputValues GetDefaultVersionTypes(
      IVssRequestContext context,
      Guid projectId,
      int definitionId)
    {
      int num = BuildArtifact.IsBranchSupportedForBuildDefinitionRepository(context, projectId, definitionId) ? 1 : 0;
      List<InputValue> inputValueList = new List<InputValue>();
      InputValue inputValue = new InputValue()
      {
        Value = "latestType",
        DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.LatestType
      };
      inputValueList.Add(new InputValue()
      {
        Value = "selectDuringReleaseCreationType",
        DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.SelectDuringReleaseCreationType
      });
      if (num != 0)
      {
        if (!BuildArtifact.IsXamlBuildArtifact(context, projectId, definitionId))
          inputValueList.Add(new InputValue()
          {
            Value = "latestWithBuildDefinitionBranchAndTagsType",
            DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.LatestWithBuildDefinitionBranchAndTagsType
          });
        inputValueList.Add(new InputValue()
        {
          Value = "latestWithBranchAndTagsType",
          DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.LatestWithBranchAndTagsType
        });
      }
      inputValueList.Add(new InputValue()
      {
        Value = "specificVersionType",
        DisplayValue = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.SpecificVersionType
      });
      inputValueList.Add(inputValue);
      return new InputValues()
      {
        InputId = "defaultVersionType",
        DefaultValue = inputValue.DisplayValue,
        PossibleValues = (IList<InputValue>) inputValueList,
        IsLimitedToPossibleValues = true,
        IsDisabled = false,
        IsReadOnly = false,
        Error = (InputValuesError) null
      };
    }

    public static Microsoft.TeamFoundation.Build.WebApi.Build GetBuild(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectId == Guid.Empty)
        throw new ArgumentException("Invalid projectId");
      Microsoft.TeamFoundation.Build.WebApi.Build buildInternal;
      try
      {
        buildInternal = BuildArtifact.GetBuildInternal(requestContext, projectId, buildId);
      }
      catch (ReleaseManagementExternalServiceException ex)
      {
        requestContext.TraceCatch(1979015, "ReleaseManagementService", "Service", (Exception) ex);
        throw;
      }
      if (!buildInternal.IsValidBuild())
      {
        string traceMessageFormat = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Build with id {0} is not valid, project {1}", (object) buildId, (object) projectId);
        if (buildInternal != null)
          traceMessageFormat = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}. Build status: {1}, Result: {2}, Deleted state: {3}", (object) traceMessageFormat, (object) buildInternal.Status, (object) buildInternal.Result, (object) buildInternal.Deleted);
        BuildArtifact.Trace(requestContext, 1976373, TraceLevel.Warning, traceMessageFormat);
        throw new ReleaseManagementExternalServiceException(BuildArtifact.GetBuildNotFoundMessage<int>(requestContext, buildId));
      }
      return buildInternal;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.Build GetBuildForBuildNumber(
      IVssRequestContext requestContext,
      Guid projectId,
      int[] buildDefinitionIds,
      string buildNumber)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectId == Guid.Empty)
        throw new ArgumentException("Invalid projectId");
      Microsoft.TeamFoundation.Build.WebApi.Build buildInternal;
      try
      {
        buildInternal = BuildArtifact.GetBuildInternal(requestContext, projectId, buildDefinitionIds, buildNumber);
      }
      catch (ReleaseManagementExternalServiceException ex)
      {
        requestContext.TraceCatch(1979015, "ReleaseManagementService", "Service", (Exception) ex);
        throw;
      }
      if (!buildInternal.IsValidBuild())
      {
        string traceMessageFormat = string.Format((IFormatProvider) CultureInfo.InvariantCulture, string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InvalidBuildNumber, (object) buildNumber, (object) projectId));
        if (buildInternal != null)
          traceMessageFormat = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}. Build status: {1}, Result: {2}, Deleted state: {3}", (object) traceMessageFormat, (object) buildInternal.Status, (object) buildInternal.Result, (object) buildInternal.Deleted);
        BuildArtifact.Trace(requestContext, 1976373, TraceLevel.Warning, traceMessageFormat);
        throw new ReleaseManagementExternalServiceException(BuildArtifact.GetBuildNotFoundMessage<string>(requestContext, buildNumber));
      }
      return buildInternal;
    }

    public static string GetDefaultBranchForBuildDefinition(
      IVssRequestContext context,
      Guid projectId,
      int buildDefinitionId)
    {
      BuildHttpClient buildClient = context != null ? context.Elevate().GetClient<BuildHttpClient>() : throw new ArgumentNullException(nameof (context));
      string forBuildDefinition = string.Empty;
      if (!BuildArtifact.IsXamlBuildArtifact(context, projectId, buildDefinitionId))
      {
        Func<Task<BuildDefinition>> func = (Func<Task<BuildDefinition>>) (() => buildClient.GetDefinitionAsync(projectId, buildDefinitionId, new int?(), new DateTime?(), (IEnumerable<string>) null, new bool?(), (object) null, new CancellationToken()));
        BuildDefinition buildDefinition = context.ExecuteAsyncAndSyncResult<BuildDefinition>(func);
        forBuildDefinition = buildDefinition == null || buildDefinition.Repository == null || buildDefinition.Repository.DefaultBranch == null ? string.Empty : buildDefinition.Repository.DefaultBranch;
      }
      return forBuildDefinition;
    }

    public static bool ShouldFetchArtifacts(IDictionary<string, string> currentInputValues)
    {
      if (currentInputValues == null)
        throw new ArgumentNullException(nameof (currentInputValues));
      string empty = string.Empty;
      currentInputValues.TryGetValue("defaultVersionType", out empty);
      return !string.IsNullOrEmpty(empty) && (!string.Equals(empty, "specificVersionType", StringComparison.OrdinalIgnoreCase) || currentInputValues.ContainsKey("defaultVersionSpecific") && !string.IsNullOrEmpty(currentInputValues["defaultVersionSpecific"]));
    }

    public override IList<InputDescriptor> InputDescriptors
    {
      get
      {
        List<InputDescriptor> inputDescriptors = new List<InputDescriptor>();
        inputDescriptors.AddRange((IEnumerable<InputDescriptor>) new List<InputDescriptor>()
        {
          new InputDescriptor()
          {
            Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.IsMultiDefinitionTypeDisplayName,
            Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.IsMultiDefinitionTypeDescription,
            InputMode = InputMode.None,
            Id = "IsMultiDefinitionType",
            IsConfidential = false,
            Validation = new InputValidation()
            {
              IsRequired = false,
              DataType = InputDataType.Boolean
            },
            HasDynamicValueInformation = true
          },
          new InputDescriptor()
          {
            Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForProject,
            Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DescriptionForProject,
            InputMode = InputMode.Combo,
            Id = "project",
            IsConfidential = false,
            Validation = new InputValidation()
            {
              IsRequired = true,
              DataType = InputDataType.Guid,
              MinLength = new int?(1),
              MaxLength = new int?(256)
            },
            DependencyInputIds = (IList<string>) new List<string>()
            {
              "IsMultiDefinitionType"
            },
            HasDynamicValueInformation = true
          },
          new InputDescriptor()
          {
            Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForRepositoryName,
            Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DescriptionForRepositoryName,
            InputMode = InputMode.Combo,
            Id = "repository",
            IsConfidential = false,
            Validation = new InputValidation()
            {
              IsRequired = true,
              DataType = InputDataType.String,
              MinLength = new int?(1),
              MaxLength = new int?(260)
            },
            DependencyInputIds = (IList<string>) new List<string>()
            {
              "project"
            },
            Properties = (IDictionary<string, object>) new Dictionary<string, object>()
            {
              {
                "visibleRule",
                (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} == {1}", (object) "IsMultiDefinitionType", (object) true.ToString())
              },
              {
                "disableRule",
                (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} != {1}", (object) "IsMultiDefinitionType", (object) true.ToString())
              }
            },
            HasDynamicValueInformation = true
          },
          new InputDescriptor()
          {
            Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForMulitpleBuildDefinitions,
            Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DescriptionForMulitpleBuildDefinitions,
            Type = "picklist",
            Id = "definitions",
            IsConfidential = false,
            Validation = new InputValidation()
            {
              IsRequired = true,
              DataType = InputDataType.String,
              MinLength = new int?(1),
              MaxLength = new int?(1300)
            },
            DependencyInputIds = (IList<string>) new List<string>()
            {
              "project",
              "repository"
            },
            Properties = (IDictionary<string, object>) new Dictionary<string, object>()
            {
              {
                "isSearchable",
                (object) true
              },
              {
                "visibleRule",
                (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} == {1}", (object) "IsMultiDefinitionType", (object) true.ToString())
              }
            },
            HasDynamicValueInformation = true
          },
          new InputDescriptor()
          {
            Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForBuildDefinition,
            Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DescriptionForBuildDefinition,
            InputMode = InputMode.Combo,
            Id = "definition",
            IsConfidential = false,
            Validation = new InputValidation()
            {
              IsRequired = true,
              DataType = InputDataType.String,
              MinLength = new int?(1),
              MaxLength = new int?(260)
            },
            DependencyInputIds = (IList<string>) new List<string>()
            {
              "project"
            },
            HasDynamicValueInformation = true,
            Properties = (IDictionary<string, object>) new Dictionary<string, object>()
            {
              {
                "isSearchable",
                (object) true
              },
              {
                "visibleRule",
                (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} != {1}", (object) "IsMultiDefinitionType", (object) true.ToString())
              }
            }
          }
        });
        inputDescriptors.AddRange((IEnumerable<InputDescriptor>) this.GetDefaultVersionInputDescriptor());
        return (IList<InputDescriptor>) inputDescriptors;
      }
    }

    public override bool ResolveStep(
      IVssRequestContext requestContext,
      IPipelineContext pipelineContext,
      Guid projectId,
      JobStep step,
      out IList<TaskStep> resolvedSteps)
    {
      throw new NotImplementedException();
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By Design")]
    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "By Design")]
    public override InputValues GetInputValues(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      string inputId,
      IDictionary<string, string> currentInputValues)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (currentInputValues == null)
        throw new ArgumentNullException(nameof (currentInputValues));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (inputId == null)
        throw new ArgumentNullException(nameof (inputId));
      InputValues inputValues1 = new InputValues();
      string str1 = (string) null;
      if (inputId != null)
      {
        switch (inputId.Length)
        {
          case 7:
            if (inputId == "project")
              return BuildArtifact.GetProjects(context, projectInfo, inputId, currentInputValues);
            goto label_82;
          case 9:
            if (inputId == "artifacts")
            {
              int[] buildDefinitionIds;
              int num = BuildArtifact.HasBuildDefinitions(currentInputValues, out buildDefinitionIds) ? 1 : 0;
              string artifactSourceVersionId = string.Empty;
              bool flag = FirstPartyArtifactTypeBase.HasArtifactSourceVersion(currentInputValues, out artifactSourceVersionId);
              if (!flag)
                flag = currentInputValues.TryGetValue("defaultVersionSpecific", out artifactSourceVersionId);
              if (num == 0 && !flag)
              {
                str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BuildDefinitionIdNotPresent;
                goto label_82;
              }
              else
              {
                Guid projectId;
                if (!FirstPartyArtifactTypeBase.HasProjectInput(currentInputValues, out projectId))
                {
                  Guid id = projectInfo.Id;
                }
                return this.GetArtifactInputValues(context, projectId, currentInputValues, buildDefinitionIds, artifactSourceVersionId, inputId);
              }
            }
            else
              goto label_82;
          case 10:
            switch (inputId[0])
            {
              case 'c':
                if (inputId == "connection" && BuildArtifact.ShouldFetchArtifacts(currentInputValues))
                  return this.GetConnectionInputValues(context, currentInputValues, inputId);
                goto label_82;
              case 'd':
                if (inputId == "definition")
                  break;
                goto label_82;
              case 'r':
                if (inputId == "repository")
                  return this.GetRepositories(context, projectInfo, inputId, currentInputValues);
                goto label_82;
              default:
                goto label_82;
            }
            break;
          case 11:
            if (inputId == "definitions")
              break;
            goto label_82;
          case 13:
            if (inputId == "artifactItems")
            {
              int[] buildDefinitionIds;
              int num = BuildArtifact.HasBuildDefinitions(currentInputValues, out buildDefinitionIds) ? 1 : 0;
              string artifactSourceVersionId = string.Empty;
              bool flag = FirstPartyArtifactTypeBase.HasArtifactSourceVersion(currentInputValues, out artifactSourceVersionId);
              if (!flag)
                flag = currentInputValues.TryGetValue("defaultVersionSpecific", out artifactSourceVersionId);
              if (num == 0 && !flag)
              {
                str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BuildDefinitionIdNotPresent;
                goto label_82;
              }
              else
              {
                string empty = string.Empty;
                currentInputValues.TryGetValue("itemPath", out empty);
                Guid projectId;
                if (!FirstPartyArtifactTypeBase.HasProjectInput(currentInputValues, out projectId))
                {
                  str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProjectIdNotPresent;
                  goto label_82;
                }
                else
                {
                  string versionBranchFilter = currentInputValues.GetDefaultVersionBranchFilter();
                  List<string> list = currentInputValues.GetDefaultVersionTagsFilter().ToList<string>();
                  return this.GetBuildArtifactItems(context, projectId, buildDefinitionIds, artifactSourceVersionId, empty, versionBranchFilter, (IEnumerable<string>) list, inputId);
                }
              }
            }
            else
              goto label_82;
          case 18:
            switch (inputId[15])
            {
              case 'a':
                if (inputId == "defaultVersionTags")
                {
                  Guid projectId;
                  if (FirstPartyArtifactTypeBase.HasProjectInput(currentInputValues, out projectId))
                    return this.GetTagsInputValues(context, projectId, currentInputValues, inputId);
                  str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProjectIdNotPresent;
                  goto label_82;
                }
                else
                  goto label_82;
              case 'y':
                if (inputId == "defaultVersionType")
                {
                  int[] buildDefinitionIds;
                  if (!BuildArtifact.HasBuildDefinitions(currentInputValues, out buildDefinitionIds))
                  {
                    str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BuildDefinitionIdNotPresent;
                    goto label_82;
                  }
                  else
                  {
                    Guid projectId;
                    if (FirstPartyArtifactTypeBase.HasProjectInput(currentInputValues, out projectId))
                      return BuildArtifact.GetDefaultVersionTypes(context, projectId, buildDefinitionIds[0]);
                    str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProjectIdNotPresent;
                    goto label_82;
                  }
                }
                else
                  goto label_82;
              default:
                goto label_82;
            }
          case 19:
            if (inputId == "artifactItemContent")
            {
              string artifactItemPath = string.Empty;
              string empty = string.Empty;
              if (!FirstPartyArtifactTypeBase.HasArtifactItemPath(currentInputValues, out artifactItemPath))
              {
                str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ArtifactItemPathNotPresent;
                goto label_82;
              }
              else
              {
                int[] buildDefinitionIds;
                int num = BuildArtifact.HasBuildDefinitions(currentInputValues, out buildDefinitionIds) ? 1 : 0;
                string artifactSourceVersionId = string.Empty;
                bool flag = FirstPartyArtifactTypeBase.HasArtifactSourceVersion(currentInputValues, out artifactSourceVersionId);
                if (!flag)
                  flag = currentInputValues.TryGetValue("defaultVersionSpecific", out artifactSourceVersionId);
                if (num == 0 && !flag)
                {
                  str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BuildDefinitionIdNotPresent;
                  goto label_82;
                }
                else
                {
                  Guid projectId;
                  if (!FirstPartyArtifactTypeBase.HasProjectInput(currentInputValues, out projectId))
                  {
                    str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProjectIdNotPresent;
                    goto label_82;
                  }
                  else
                  {
                    string versionBranchFilter = currentInputValues.GetDefaultVersionBranchFilter();
                    List<string> list = currentInputValues.GetDefaultVersionTagsFilter().ToList<string>();
                    return this.GetBuildArtifactContent(context, projectId, buildDefinitionIds, artifactSourceVersionId, artifactItemPath, versionBranchFilter, (IEnumerable<string>) list, inputId);
                  }
                }
              }
            }
            else
              goto label_82;
          case 20:
            if (inputId == "defaultVersionBranch")
            {
              int[] buildDefinitionIds;
              if (!BuildArtifact.HasBuildDefinitions(currentInputValues, out buildDefinitionIds))
              {
                str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BuildDefinitionIdNotPresent;
                goto label_82;
              }
              else
              {
                Guid projectId;
                if (FirstPartyArtifactTypeBase.HasProjectInput(currentInputValues, out projectId))
                  return this.GetBranchInputValues(context, projectId, currentInputValues, buildDefinitionIds[0], inputId);
                str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProjectIdNotPresent;
                goto label_82;
              }
            }
            else
              goto label_82;
          case 21:
            if (inputId == "IsMultiDefinitionType")
              return BuildArtifact.GetIsMultiDefinitionTypeInputValues(context, inputId);
            goto label_82;
          case 22:
            if (inputId == "defaultVersionSpecific")
            {
              if (FirstPartyArtifactTypeBase.HasProjectInput(currentInputValues, out Guid _))
                return this.GetDefaultVersionBuildInputValues(context, currentInputValues, inputId);
              str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProjectIdNotPresent;
              goto label_82;
            }
            else
              goto label_82;
          case 25:
            if (inputId == "hasCustomStorageArtifacts")
              return this.GetHasCustomStorageArtifactsInputValues(context, currentInputValues, inputId);
            goto label_82;
          default:
            goto label_82;
        }
        Guid projectId1;
        if (!FirstPartyArtifactTypeBase.HasProjectInput(currentInputValues, out projectId1))
        {
          str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProjectIdNotPresent;
        }
        else
        {
          string repositoryId = (string) null;
          if ("definitions" == inputId && !FirstPartyArtifactTypeBase.HasRepositoryInput(currentInputValues, out repositoryId))
          {
            str1 = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.RepositoryIdNotPresent;
          }
          else
          {
            string continuationToken = (string) null;
            int[] buildDefinitionIds;
            if (FirstPartyArtifactTypeBase.IsArtifactEditingMode(currentInputValues) && BuildArtifact.HasBuildDefinitions(currentInputValues, out buildDefinitionIds))
              return this.GetBuildDefinitionsPaged(context, projectId1, repositoryId, inputId, string.Empty, buildDefinitionIds, ref continuationToken);
            string empty = string.Empty;
            InputValues inputValues2 = new InputValues();
            if (currentInputValues.ContainsKey("continuationToken"))
              continuationToken = currentInputValues["continuationToken"];
            InputValues inputValues3 = !currentInputValues.TryGetValue("name", out empty) || string.IsNullOrEmpty(empty) ? this.GetBuildDefinitionsPaged(context, projectId1, repositoryId, inputId, string.Empty, (int[]) null, ref continuationToken) : this.GetBuildDefinitionsPaged(context, projectId1, repositoryId, inputId, empty, (int[]) null, ref continuationToken);
            if (continuationToken != null)
            {
              currentInputValues["continuationToken"] = continuationToken;
              currentInputValues["callbackRequired"] = true.ToString();
            }
            else
              currentInputValues["callbackRequired"] = false.ToString();
            return inputValues3;
          }
        }
      }
label_82:
      string str2 = str1 ?? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InputTypeNotSupported, (object) inputId);
      inputValues1.Error = new InputValuesError()
      {
        Message = str2
      };
      return inputValues1;
    }

    public override IList<InputValue> GetAvailableVersions(
      IVssRequestContext requestContext,
      IDictionary<string, string> sourceInputs,
      ProjectInfo projectInfo)
    {
      return this.GetAvailableVersions(requestContext, sourceInputs, new int?(100));
    }

    public override InputValue GetLatestVersion(
      IVssRequestContext requestContext,
      IDictionary<string, string> sourceInputs,
      ProjectInfo projectInfo)
    {
      return this.GetAvailableVersions(requestContext, sourceInputs, new int?(1)).FirstOrDefault<InputValue>();
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should catch all exceptions because we can try without authentication too")]
    public override bool? UsesExternalAndPublicSourceRepo(
      IVssRequestContext requestContext,
      Dictionary<string, InputValue> sourceData,
      Guid projectId,
      object artifactContext)
    {
      if (requestContext == null || sourceData == null)
        return new bool?();
      bool? nullable = new bool?();
      InputValue inputValue;
      int result;
      if (sourceData.TryGetValue("definition", out inputValue) && !string.IsNullOrEmpty(inputValue?.Value) && int.TryParse(inputValue.Value, out result))
      {
        if (!(artifactContext is BuildDefinition buildDefinition1))
          buildDefinition1 = BuildArtifact.GetBuildDefinition(requestContext, projectId, result, true);
        BuildDefinition buildDefinition2 = buildDefinition1;
        BuildRepository repository = buildDefinition2?.Repository;
        if (repository != null && string.Equals(repository.Type, "GitHub", StringComparison.OrdinalIgnoreCase))
        {
          GitHubAuthentication authentication = (GitHubAuthentication) null;
          try
          {
            ServiceEndpoint serviceEndpoint = BuildArtifact.GetServiceEndpoint(requestContext, projectId, buildDefinition2);
            if (serviceEndpoint != null)
              authentication = serviceEndpoint.GetGitHubAuthentication(requestContext, projectId);
          }
          catch (Exception ex)
          {
            BuildArtifact.TraceError(requestContext, 1971017, ex.Message);
          }
          nullable = new bool?(ArtifactTypeUtility.IsGitHubRepoPublic(requestContext, authentication, repository.Id));
        }
      }
      return nullable;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "By Design.")]
    public override IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> GetChanges(
      IVssRequestContext requestContext,
      Dictionary<string, InputValue> sourceData,
      InputValue startVersion,
      InputValue endVersion,
      ProjectInfo projectInfo,
      int top,
      object artifactContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      int buildId1 = BuildArtifact.TryParseBuildId(startVersion, false);
      int buildId2 = BuildArtifact.TryParseBuildId(endVersion, false);
      if (buildId1 == buildId2)
        return (IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>) new List<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>();
      if (buildId2 == 0)
        return (IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>) new List<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>();
      InputValue inputValue;
      int result;
      if (!(artifactContext is BuildDefinition buildDefinition) && sourceData != null && sourceData.TryGetValue("definition", out inputValue) && int.TryParse(inputValue.Value, out result))
        buildDefinition = BuildArtifact.GetBuildDefinition(requestContext, projectInfo.Id, result, true);
      IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> changeList = (IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>) new List<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>();
      if (string.Equals(BuildArtifact.GetBuildDefinitionRepositoryType(buildDefinition), "GitHub", StringComparison.OrdinalIgnoreCase))
        return BuildArtifact.GetChangesForGitHubSource(requestContext, projectInfo, buildDefinition, buildId1, buildId2, top);
      try
      {
        return BuildChangeConverter.ToReleaseChanges(this.changesRetriever(requestContext, projectInfo.Id, buildId1, buildId2, top));
      }
      catch (AggregateException ex)
      {
        requestContext.TraceCatch(1979015, "ReleaseManagementService", "Service", (Exception) ex);
        throw new ReleaseManagementExternalServiceException(ExceptionsUtilities.GetAllInnerExceptionsMessages(ex));
      }
      catch (ReleaseManagementExternalServiceException ex)
      {
        requestContext.TraceCatch(1979015, "ReleaseManagementService", "Service", (Exception) ex);
        throw;
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "2#", Justification = "By design")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "3#", Justification = "By design")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Currently only using in telemetry should not fail")]
    public override IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> GetChangesBetweenArtifactSource(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineArtifactSource currentReleaseArtifactSource,
      PipelineArtifactSource lastReleaseArtifactSource,
      int top)
    {
      if (currentReleaseArtifactSource == null)
        throw new ArgumentNullException(nameof (currentReleaseArtifactSource));
      try
      {
        ProjectInfo projectInfo = new ProjectInfo();
        InputValue inputValue;
        if (currentReleaseArtifactSource.SourceData.TryGetValue("project", out inputValue) && inputValue != null)
        {
          projectInfo.Id = new Guid(inputValue.Value);
          projectInfo.Name = inputValue.DisplayValue;
        }
        else
          projectInfo.Id = projectId;
        Microsoft.TeamFoundation.Build.WebApi.Build build = BuildArtifact.GetBuild(requestContext, projectInfo.Id, currentReleaseArtifactSource.VersionId);
        if (string.Equals(build?.Repository?.Type, "GitHub", StringComparison.OrdinalIgnoreCase))
          return this.GetChanges(requestContext, currentReleaseArtifactSource.SourceData, lastReleaseArtifactSource?.Version, currentReleaseArtifactSource.Version, projectInfo, top, (object) null);
        if (lastReleaseArtifactSource == null || lastReleaseArtifactSource.VersionId == 0)
          return BuildArtifact.GetChangesForSingleBuildArtifact(requestContext, build, projectInfo.Id, top);
        return currentReleaseArtifactSource.VersionId > lastReleaseArtifactSource.VersionId ? BuildChangeConverter.ToReleaseChanges(BuildArtifact.GetChanges(requestContext, projectInfo.Id, lastReleaseArtifactSource.VersionId, currentReleaseArtifactSource.VersionId, top)) : (IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>) new List<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>();
      }
      catch
      {
        return (IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>) new List<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>();
      }
    }

    public override IList<WorkItemRef> GetWorkItems(
      IVssRequestContext requestContext,
      Dictionary<string, InputValue> sourceData,
      InputValue startVersion,
      InputValue endVersion,
      ProjectInfo projectInfo,
      int top,
      object artifactContext,
      GetConfig getConfig)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      int buildId1 = BuildArtifact.TryParseBuildId(startVersion, false);
      int buildId2 = BuildArtifact.TryParseBuildId(endVersion, false);
      if (buildId1 == buildId2)
        return (IList<WorkItemRef>) new List<WorkItemRef>();
      if (buildId2 == 0)
        return (IList<WorkItemRef>) new List<WorkItemRef>();
      InputValue inputValue;
      int result;
      if (!(artifactContext is BuildDefinition buildDefinition) && sourceData != null && sourceData.TryGetValue("definition", out inputValue) && int.TryParse(inputValue.Value, out result))
        buildDefinition = BuildArtifact.GetBuildDefinition(requestContext, projectInfo.Id, result, true);
      if (string.Equals(BuildArtifact.GetBuildDefinitionRepositoryType(buildDefinition), "GitHub", StringComparison.OrdinalIgnoreCase))
      {
        IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> changesForGitHubSource = BuildArtifact.GetChangesForGitHubSource(requestContext, projectInfo, buildDefinition, buildId1, buildId2, 250);
        IList<WorkItemRef> workItemRefList = (IList<WorkItemRef>) new List<WorkItemRef>();
        IList<WorkItemRef> relatedWorkItems = BuildArtifact.GetDirectlyRelatedWorkItems(requestContext, projectInfo, buildDefinition, changesForGitHubSource);
        workItemRefList.AddRange<WorkItemRef, IList<WorkItemRef>>((IEnumerable<WorkItemRef>) relatedWorkItems);
        CustomerIntelligenceData telemetryData = new CustomerIntelligenceData();
        telemetryData.Add("AzDevWorkItems", workItemRefList != null ? (double) workItemRefList.Count : 0.0);
        if (workItemRefList.Count < top)
        {
          IList<WorkItemRef> issuesForGitHubSource = BuildArtifact.GetIssuesForGitHubSource(requestContext, projectInfo, buildDefinition, changesForGitHubSource, top - workItemRefList.Count);
          workItemRefList = workItemRefList.AddRange<WorkItemRef, IList<WorkItemRef>>((IEnumerable<WorkItemRef>) issuesForGitHubSource);
          telemetryData.Add("GitHubIssues", issuesForGitHubSource != null ? (double) issuesForGitHubSource.Count : 0.0);
        }
        if (workItemRefList.Count < top)
        {
          IList<WorkItemRef> jiraWorkItems = BuildArtifact.GetJiraWorkItems(requestContext, projectInfo, changesForGitHubSource, top - workItemRefList.Count, getConfig);
          workItemRefList.AddRange<WorkItemRef, IList<WorkItemRef>>((IEnumerable<WorkItemRef>) jiraWorkItems);
          telemetryData.Add("JiraIssues", jiraWorkItems != null ? (double) jiraWorkItems.Count : 0.0);
        }
        BuildArtifact.PublishGitHubWorkItemsTelemetry(requestContext, telemetryData);
        return (IList<WorkItemRef>) workItemRefList.Take<WorkItemRef>(top).ToList<WorkItemRef>();
      }
      try
      {
        IList<ResourceRef> buildWorkItems = this.witRetriever(requestContext, projectInfo.Id, buildId1, buildId2, top);
        IList<WorkItemRef> releaseWorkItems = ReleaseWorkItemConverter.ToReleaseWorkItems(buildWorkItems);
        if (string.Equals(BuildArtifact.GetBuildDefinitionRepositoryType(buildDefinition), "TfsGit", StringComparison.OrdinalIgnoreCase) && releaseWorkItems.Count < top)
        {
          IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> releaseChanges = BuildChangeConverter.ToReleaseChanges(this.changesRetriever(requestContext, projectInfo.Id, buildId1, buildId2, top));
          IList<WorkItemRef> jiraWorkItems = BuildArtifact.GetJiraWorkItems(requestContext, projectInfo, releaseChanges, top - buildWorkItems.Count, getConfig);
          releaseWorkItems.AddRange<WorkItemRef, IList<WorkItemRef>>((IEnumerable<WorkItemRef>) jiraWorkItems);
        }
        return (IList<WorkItemRef>) releaseWorkItems.Take<WorkItemRef>(top).ToList<WorkItemRef>();
      }
      catch (AggregateException ex)
      {
        throw new ReleaseManagementExternalServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionsUtilities.GetAllInnerExceptionsMessages(ex)));
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static IList<WorkItemRef> GetJiraWorkItems(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> changes,
      int top,
      GetConfig getConfig)
    {
      IList<WorkItemRef> jiraWorkItems = (IList<WorkItemRef>) new List<WorkItemRef>();
      if (getConfig != null && getConfig.IntegrateJiraWorkItems && !getConfig.JiraEndpointId.IsNullOrEmpty<char>())
      {
        string jiraEndpointId = getConfig.JiraEndpointId;
        Guid result;
        if (string.IsNullOrEmpty(jiraEndpointId) || !Guid.TryParse(jiraEndpointId, out result))
          throw new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InvalidServiceEndpointId, (object) jiraEndpointId));
        ServiceEndpoint serviceEndpoint = ServiceEndpointHelper.GetServiceEndpoint(requestContext, projectInfo.Id, result);
        JiraAuthentication jiraAuthentication = serviceEndpoint.GetJiraAuthentication(requestContext);
        IEnumerable<string> jiraIssuesKeys = (IEnumerable<string>) BuildArtifact.GetJiraIssuesKeys(changes.Select<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change, string>((Func<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change, string>) (c => c.Message)), top);
        if (jiraAuthentication != null && jiraIssuesKeys.Any<string>())
        {
          JiraResult<JiraData.V2.IssuesResponseData> jiraResult = BuildArtifact.GetJiraHttpClient(requestContext).SearchIssues(jiraAuthentication, jiraIssuesKeys);
          if (!jiraResult.IsSuccessful)
            requestContext.Trace(1973109, TraceLevel.Error, "ReleaseManagementService", "Pipeline", "GetJiraWorkItems(Build artifact): Failed to get work items with error: {0}", (object) jiraResult.ErrorMessage);
          if (jiraResult.Result != null && jiraResult.Result.Issues != null)
            jiraWorkItems = ReleaseWorkItemConverter.JiraIssuesToReleaseWorkItems(jiraResult.Result.Issues, serviceEndpoint.Url);
        }
      }
      return jiraWorkItems;
    }

    private static List<string> GetJiraIssuesKeys(
      IEnumerable<string> commitsMessages,
      int top,
      bool ignoreTop = false)
    {
      List<string> jiraIssuesKeys = new List<string>();
      int num = 0;
      foreach (string commitsMessage in commitsMessages)
      {
        MatchCollection matchCollection = BuildArtifact.JiraIssueRegex.Matches(commitsMessage);
        if (matchCollection != null)
        {
          foreach (Match match in matchCollection)
          {
            string str = match.Groups["issueKey"].Value;
            jiraIssuesKeys.Add(str);
            ++num;
            if (!ignoreTop && num >= top)
              return jiraIssuesKeys;
          }
        }
      }
      return jiraIssuesKeys;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By Design")]
    public override void LinkDeploymentToWorkItems(
      IVssRequestContext requestContext,
      Dictionary<string, InputValue> sourceData,
      InputValue startVersion,
      InputValue endVersion,
      ProjectInfo artifactSourceProjectInfo,
      ProjectInfo releaseProjectInfo,
      int top,
      object artifactContext,
      LinkConfig linkConfig,
      Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.DeploymentData deploymentData)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (artifactSourceProjectInfo == null)
        throw new ArgumentNullException(nameof (artifactSourceProjectInfo));
      if (releaseProjectInfo == null)
        throw new ArgumentNullException(nameof (releaseProjectInfo));
      if (linkConfig == null)
        throw new ArgumentNullException(nameof (linkConfig));
      if (deploymentData == null)
        throw new ArgumentNullException(nameof (deploymentData));
      int buildId1 = BuildArtifact.TryParseBuildId(startVersion, false);
      int buildId2 = BuildArtifact.TryParseBuildId(endVersion, false);
      if (buildId1 == buildId2 || buildId2 == 0)
        return;
      InputValue inputValue;
      int result;
      if (!(artifactContext is BuildDefinition buildDefinition) && sourceData != null && sourceData.TryGetValue("definition", out inputValue) && int.TryParse(inputValue.Value, out result))
        buildDefinition = BuildArtifact.GetBuildDefinition(requestContext, artifactSourceProjectInfo.Id, result, true);
      bool flag = linkConfig.AutoLinkWorkItems || linkConfig.AutoLinkBoardsWorkItems;
      string definitionRepositoryType = BuildArtifact.GetBuildDefinitionRepositoryType(buildDefinition);
      if (string.Equals(definitionRepositoryType, "GitHub", StringComparison.OrdinalIgnoreCase))
      {
        IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> changesForGitHubSource = BuildArtifact.GetChangesForGitHubSource(requestContext, artifactSourceProjectInfo, buildDefinition, buildId1, buildId2, 250);
        if (flag)
        {
          IList<WorkItemRef> relatedWorkItems = BuildArtifact.GetDirectlyRelatedWorkItems(requestContext, artifactSourceProjectInfo, buildDefinition, changesForGitHubSource);
          if (linkConfig.AutoLinkWorkItems)
            this.LinkDeploymentDetailsToAzureBoardWorkItems(requestContext, relatedWorkItems, deploymentData.Run.Id, deploymentData.Run.EnvironmentId, deploymentData.Identity, releaseProjectInfo);
          if (linkConfig.AutoLinkBoardsWorkItems)
            BuildArtifact.PublishDeploymentDetailsToAzureBoardWorkItems(requestContext, 250, relatedWorkItems, deploymentData);
        }
        if (!linkConfig.AutoLinkJiraWorkItems)
          return;
        IEnumerable<string> jiraIssuesKeys = (IEnumerable<string>) BuildArtifact.GetJiraIssuesKeys(changesForGitHubSource.Select<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change, string>((Func<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change, string>) (c => c.Message)), top, true);
        if (!jiraIssuesKeys.Any<string>())
          return;
        BuildArtifact.LinkDeploymentDetailsToJiraIssues(requestContext, artifactSourceProjectInfo, jiraIssuesKeys.ToArray<string>(), linkConfig.JiraEndpointId, deploymentData);
      }
      else
      {
        try
        {
          if (flag)
          {
            int maxWorkItemsToLink = linkConfig.AutoLinkBoardsWorkItems ? BuildArtifact.GetMaxWorkItemsToLinkCount(requestContext) : top;
            IList<ResourceRef> resourceRefList = this.witRetriever(requestContext, artifactSourceProjectInfo.Id, buildId1, buildId2, maxWorkItemsToLink);
            if (linkConfig.AutoLinkWorkItems)
              this.LinkDeploymentDetailsToAzureBoardWorkItems(requestContext, ReleaseWorkItemConverter.ToReleaseWorkItems((IList<ResourceRef>) resourceRefList.Take<ResourceRef>(top).ToList<ResourceRef>()), deploymentData.Run.Id, deploymentData.Run.EnvironmentId, deploymentData.Identity, releaseProjectInfo);
            if (linkConfig.AutoLinkBoardsWorkItems)
              BuildArtifact.PublishDeploymentDetailsToAzureBoardWorkItems(requestContext, maxWorkItemsToLink, ReleaseWorkItemConverter.ToReleaseWorkItems(resourceRefList), deploymentData);
          }
          if (!string.Equals(definitionRepositoryType, "TfsGit", StringComparison.OrdinalIgnoreCase) || !linkConfig.AutoLinkJiraWorkItems)
            return;
          IEnumerable<string> jiraIssuesKeys = (IEnumerable<string>) BuildArtifact.GetJiraIssuesKeys(BuildChangeConverter.ToReleaseChanges(this.changesRetriever(requestContext, artifactSourceProjectInfo.Id, buildId1, buildId2, top)).Select<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change, string>((Func<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change, string>) (c => c.Message)), top, true);
          if (!jiraIssuesKeys.Any<string>())
            return;
          BuildArtifact.LinkDeploymentDetailsToJiraIssues(requestContext, artifactSourceProjectInfo, jiraIssuesKeys.ToArray<string>(), linkConfig.JiraEndpointId, deploymentData);
        }
        catch (AggregateException ex)
        {
          throw new ReleaseManagementExternalServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionsUtilities.GetAllInnerExceptionsMessages(ex)));
        }
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design")]
    [SuppressMessage("Microsoft.Globalization", "CA1305:Specify IFormatProvider", Justification = "By design")]
    private static void PublishDeploymentDetailsToAzureBoardWorkItems(
      IVssRequestContext requestContext,
      int maxWorkItemsToLink,
      IList<WorkItemRef> workItems,
      Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.DeploymentData deploymentData)
    {
      CustomerIntelligenceData telemetryData = new CustomerIntelligenceData();
      telemetryData.Add("MaxWorkItems", (double) maxWorkItemsToLink);
      telemetryData.Add("WorkItemsCount", workItems != null ? (double) workItems.Count : 0.0);
      telemetryData.Add("DeploymentData", (object) BuildArtifact.SanitizeTelemetryData(deploymentData));
      if (workItems.Count > 0)
      {
        object deploymentRequestData = BuildArtifact.GetAzureBoardsDeploymentRequestData((IReadOnlyCollection<int>) workItems.Select<WorkItemRef, int>((Func<WorkItemRef, int>) (w => int.Parse(w.Id))).ToList<int>(), deploymentData);
        DataProviderContext dataProviderContext = new DataProviderContext()
        {
          Properties = new Dictionary<string, object>()
          {
            ["externalDeployment"] = deploymentRequestData
          }
        };
        try
        {
          JObject fromDataProvider = BuildArtifact.GetArtifactValueFromDataProvider(requestContext, dataProviderContext);
          if (fromDataProvider != null)
          {
            JToken jtoken;
            if (fromDataProvider.TryGetValue("errorMessage", out jtoken))
            {
              requestContext.Trace(1976428, TraceLevel.Error, "ReleaseManagementService", "Pipeline", Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.PublishDeploymentDetailsToAzureBoardWorkItemsError, (object) jtoken.Value<string>());
              telemetryData.Add("ErrorMessage", jtoken.Value<string>());
            }
          }
        }
        catch (Exception ex)
        {
          requestContext.Trace(1976428, TraceLevel.Error, "ReleaseManagementService", "Pipeline", Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.PublishDeploymentDetailsToAzureBoardWorkItemsError, (object) ex.Message);
          telemetryData.Add("ErrorMessage", ex.Message);
        }
      }
      BuildArtifact.PublishDeploymentStatusToWorkItemsTelemetry(requestContext, telemetryData);
    }

    private static JObject GetArtifactValueFromDataProvider(
      IVssRequestContext requestContext,
      DataProviderContext dataProviderContext)
    {
      DataProviderQuery query = new DataProviderQuery()
      {
        ContributionIds = new List<string>((IEnumerable<string>) new string[1]
        {
          "ms.vss-work-web.work-item-write-external-deployments-data-provider"
        }),
        Context = dataProviderContext
      };
      DataProviderResult dataProviderResult = !requestContext.ExecutionEnvironment.IsHostedDeployment ? requestContext.GetService<IExtensionDataProviderService>().GetDataProviderData(requestContext, query) : requestContext.GetClient<ContributionsHttpClient>(Microsoft.VisualStudio.Services.WebApi.ServiceInstanceTypes.TFS).QueryDataProvidersAsync(query).SyncResult<DataProviderResult>();
      DataProviderExceptionDetails exceptionDetails;
      Dictionary<string, object> source = dataProviderResult.Exceptions == null || !dataProviderResult.Exceptions.Any<KeyValuePair<string, DataProviderExceptionDetails>>() || !dataProviderResult.Exceptions.TryGetValue("ms.vss-work-web.work-item-write-external-deployments-data-provider", out exceptionDetails) ? dataProviderResult.Data : throw new DataProviderException(exceptionDetails.Message);
      return (source != null ? source.First<KeyValuePair<string, object>>().Value : (object) null) as JObject;
    }

    private static Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.DeploymentData SanitizeTelemetryData(
      Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.DeploymentData deploymentData)
    {
      return new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.DeploymentData()
      {
        Id = deploymentData.Id,
        Pipeline = deploymentData.Pipeline,
        Run = deploymentData.Run,
        Status = deploymentData.Status,
        Environment = deploymentData.Environment,
        CompletionTime = deploymentData.CompletionTime,
        AttemptNumber = deploymentData.AttemptNumber
      };
    }

    private static object GetAzureBoardsDeploymentRequestData(
      IReadOnlyCollection<int> workItemIds,
      Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.DeploymentData deployment)
    {
      ExternalEnvironment externalEnvironment = new ExternalEnvironment()
      {
        Id = deployment.Environment.Id,
        DisplayName = deployment.Environment.Name,
        Type = deployment.Environment.Type
      };
      ExternalPipeline externalPipeline = new ExternalPipeline()
      {
        Id = deployment.Pipeline.Id,
        DisplayName = deployment.Pipeline.Name,
        Url = new Uri(deployment.Pipeline.Url)
      };
      DateTime dateTime = deployment.CompletionTime == new DateTime() ? DateTime.UtcNow : deployment.CompletionTime;
      return (object) new ExternalDeployment()
      {
        CreatedBy = deployment.Identity.Id,
        Group = string.Empty,
        RunId = deployment.Run.Id,
        SequenceNumber = deployment.AttemptNumber,
        Status = BuildArtifact.GetAzureBoardsDeploymentStatus(deployment.Status),
        StatusDate = dateTime,
        DisplayName = deployment.Run.Name,
        Url = new Uri(deployment.Run.Url),
        Description = deployment.Run.Description,
        Pipeline = externalPipeline,
        Environment = externalEnvironment,
        RelatedWorkItemIds = (IEnumerable<int>) workItemIds
      };
    }

    private static string GetAzureBoardsDeploymentStatus(DeploymentState status)
    {
      string deploymentStatus;
      switch (status)
      {
        case DeploymentState.Succeeded:
          deploymentStatus = WitConstants.AzureBoardsDeploymentStatus.Successful;
          break;
        case DeploymentState.PartiallySucceeded:
          deploymentStatus = WitConstants.AzureBoardsDeploymentStatus.Successful;
          break;
        case DeploymentState.Failed:
          deploymentStatus = WitConstants.AzureBoardsDeploymentStatus.Failed;
          break;
        default:
          deploymentStatus = WitConstants.AzureBoardsDeploymentStatus.Unknown;
          break;
      }
      return deploymentStatus;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static void LinkDeploymentDetailsToAzureBoardUnlinkedWorkItems(
      IVssRequestContext requestContext,
      IList<WorkItemRef> workItems,
      int releaseId,
      int releaseEnvironmentId,
      ProjectInfo releaseProjectInfo)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (workItems == null)
        throw new ArgumentNullException(nameof (workItems));
      WorkItemRelation workItemRelation1 = new WorkItemRelation();
      workItemRelation1.Rel = "ArtifactLink";
      workItemRelation1.Url = BuildArtifact.BuildWorkItemLinkUri(releaseProjectInfo.Id, releaseId, releaseEnvironmentId);
      workItemRelation1.Attributes = (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "name",
          (object) "Integrated in release environment"
        }
      };
      WorkItemRelation workItemRelation2 = workItemRelation1;
      JsonPatchDocument document = new JsonPatchDocument();
      document.Add(new JsonPatchOperation()
      {
        Operation = Operation.Add,
        Path = "/relations/-",
        Value = (object) workItemRelation2
      });
      WorkItemTrackingHttpClient trackingHttpClient = BuildArtifact.GetWorkItemTrackingHttpClient(requestContext);
      using (ReleaseManagementTimer.Create(requestContext, "Service", "BuildArtifact.LinkWorkItemsToEnvironment", 1971065))
      {
        foreach (WorkItemRef workItem in (IEnumerable<WorkItemRef>) workItems)
        {
          int result;
          if (int.TryParse(workItem.Id, out result))
          {
            try
            {
              trackingHttpClient.UpdateWorkItemAsync(document, result, new bool?(false), new bool?(false)).Wait();
            }
            catch (AggregateException ex)
            {
              requestContext.Trace(1976423, TraceLevel.Error, "ReleaseManagementService", "Pipeline", "AutoLinkWorkItems(Build artifact): Failed to update work item with Id: {0}.Exception: {1}", (object) result, (object) ex);
            }
          }
        }
      }
    }

    public override IEnumerable<string> GetLinkedWorkItemIds(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      string linkUri = BuildArtifact.BuildWorkItemLinkUri(projectId, releaseId, environmentId);
      ArtifactUriQuery artifactUriQuery = new ArtifactUriQuery()
      {
        ArtifactUris = (IEnumerable<string>) new List<string>()
        {
          linkUri
        }
      };
      return BuildArtifact.GetWorkItemTrackingHttpClient(requestContext).QueryWorkItemsForArtifactUrisAsync(artifactUriQuery).GetResult<ArtifactUriQueryResult>(requestContext.CancellationToken).ArtifactUrisQueryResult.Where<KeyValuePair<string, IEnumerable<WorkItemReference>>>((Func<KeyValuePair<string, IEnumerable<WorkItemReference>>, bool>) (pair => linkUri.Equals(pair.Key, StringComparison.OrdinalIgnoreCase))).SelectMany<KeyValuePair<string, IEnumerable<WorkItemReference>>, WorkItemReference>((Func<KeyValuePair<string, IEnumerable<WorkItemReference>>, IEnumerable<WorkItemReference>>) (pair => pair.Value)).Select<WorkItemReference, string>((Func<WorkItemReference, string>) (workItemRef => workItemRef.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static void LinkDeploymentDetailsToJiraIssues(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      string[] issueKeys,
      string endpointId,
      Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.DeploymentData deploymentData)
    {
      Guid result;
      if (string.IsNullOrEmpty(endpointId) || !Guid.TryParse(endpointId, out result))
        throw new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InvalidServiceEndpointId, (object) endpointId));
      JiraAuthentication jiraAuthentication = ServiceEndpointHelper.GetServiceEndpoint(requestContext, projectInfo.Id, result).GetJiraAuthentication(requestContext);
      JiraData.V0_1.DeploymentRequestData deploymentRequestData = BuildArtifact.GetJiraDeploymentRequestData(requestContext, projectInfo, issueKeys, deploymentData);
      JiraResult<JiraData.V0_1.DeploymentResponseData> issues = BuildArtifact.GetJiraHttpClient(requestContext).LinkDeploymentToIssues(jiraAuthentication, deploymentRequestData);
      if (issues.Result.RejectedDeployments.Length != 1)
        return;
      requestContext.Trace(1976423, TraceLevel.Error, "ReleaseManagementService", "Pipeline", "AutoLinkJiraWorkItems(Build artifact): Failed to update work item with error: {0} and following unknown issue keys: {1}", (object) string.Join(",", ((IEnumerable<JiraData.V0_1.Error>) issues.Result.RejectedDeployments[0].Errors).Select<JiraData.V0_1.Error, string>((Func<JiraData.V0_1.Error, string>) (error => error.Message))), (object) string.Join(",", issues.Result.UnknownIssueKeys));
    }

    private static JiraData.V0_1.DeploymentRequestData GetJiraDeploymentRequestData(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      string[] issueKeys,
      Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.DeploymentData deployment)
    {
      JiraData.V0_1.Environment environment = new JiraData.V0_1.Environment()
      {
        Id = deployment.Environment.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture),
        DisplayName = deployment.Environment.Name,
        Type = deployment.Environment.Type
      };
      JiraData.V0_1.Pipeline pipeline = new JiraData.V0_1.Pipeline()
      {
        Id = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) projectInfo.Id.ToString(), (object) deployment.Pipeline.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
        DisplayName = deployment.Pipeline.Name,
        Url = deployment.Pipeline.Url
      };
      JiraData.V0_1.DeploymentData deploymentData = new JiraData.V0_1.DeploymentData()
      {
        DeploymentSequenceNumber = deployment.Id,
        UpdateSequenceNumber = deployment.AttemptNumber,
        IssueKeys = issueKeys,
        DisplayName = deployment.Run.Name,
        Url = deployment.Run.Url,
        Description = deployment.Run.Description,
        LastUpdated = deployment.CompletionTime,
        Label = string.Empty,
        State = BuildArtifact.GetDeploymentStatus(deployment.Status),
        Pipeline = pipeline,
        Environment = environment,
        SchemaVersion = "1.0"
      };
      return new JiraData.V0_1.DeploymentRequestData()
      {
        Properties = new JiraData.V0_1.PropertyData()
        {
          AccountId = requestContext.ServiceHost.InstanceId.ToString(),
          ProjectId = projectInfo.Id.ToString(),
          PipelineId = deployment.Pipeline.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        Deployments = new JiraData.V0_1.DeploymentData[1]
        {
          deploymentData
        }
      };
    }

    private static string GetDeploymentStatus(DeploymentState status)
    {
      string deploymentStatus;
      switch (status)
      {
        case DeploymentState.Succeeded:
          deploymentStatus = JiraConstants.JiraDeploymentStatus.Successful;
          break;
        case DeploymentState.PartiallySucceeded:
          deploymentStatus = JiraConstants.JiraDeploymentStatus.Successful;
          break;
        default:
          deploymentStatus = JiraConstants.JiraDeploymentStatus.Unknown;
          break;
      }
      return deploymentStatus;
    }

    private static JiraHttpClient GetJiraHttpClient(IVssRequestContext requestContext) => JiraHttpClientFactory.Create(requestContext);

    protected static string GetDefaultVersionInputVisibilityRules(string inputId)
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      switch (inputId)
      {
        case "defaultVersionBranch":
          keyValuePairList.Add(new KeyValuePair<string, string>("defaultVersionType", Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.LatestWithBranchAndTagsType));
          break;
        case "defaultVersionTags":
          keyValuePairList.Add(new KeyValuePair<string, string>("defaultVersionType", Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.LatestWithBranchAndTagsType));
          keyValuePairList.Add(new KeyValuePair<string, string>("defaultVersionType", Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.LatestWithBuildDefinitionBranchAndTagsType));
          break;
        case "defaultVersionSpecific":
          keyValuePairList.Add(new KeyValuePair<string, string>("defaultVersionType", Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.SpecificVersionType));
          break;
      }
      return JsonConvert.SerializeObject((object) keyValuePairList);
    }

    protected static InputValues GetIsMultiDefinitionTypeInputValues(
      IVssRequestContext requestContext,
      string inputId)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (inputId == null)
        throw new ArgumentNullException(nameof (inputId));
      InputValues definitionTypeInputValues = new InputValues();
      definitionTypeInputValues.InputId = inputId;
      definitionTypeInputValues.DefaultValue = false.ToString();
      InputValues inputValues = definitionTypeInputValues;
      InputValue[] inputValueArray = new InputValue[2];
      InputValue inputValue1 = new InputValue();
      bool flag = false;
      inputValue1.Value = flag.ToString();
      flag = false;
      inputValue1.DisplayValue = flag.ToString();
      inputValueArray[0] = inputValue1;
      InputValue inputValue2 = new InputValue();
      flag = true;
      inputValue2.Value = flag.ToString();
      flag = true;
      inputValue2.DisplayValue = flag.ToString();
      inputValueArray[1] = inputValue2;
      inputValues.PossibleValues = (IList<InputValue>) inputValueArray;
      definitionTypeInputValues.IsLimitedToPossibleValues = false;
      definitionTypeInputValues.IsDisabled = true;
      definitionTypeInputValues.IsReadOnly = true;
      definitionTypeInputValues.Error = (InputValuesError) null;
      return definitionTypeInputValues;
    }

    protected static InputValues GetProjects(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      string inputId,
      IDictionary<string, string> currentInputValues)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (inputId == null)
        throw new ArgumentNullException(nameof (inputId));
      IProjectService service = requestContext.GetService<IProjectService>();
      IList<InputValue> inputValueList = (IList<InputValue>) new List<InputValue>();
      string str = projectInfo.Name;
      Guid projectId;
      string projectName;
      if (FirstPartyArtifactTypeBase.IsArtifactEditingMode(currentInputValues) && FirstPartyArtifactTypeBase.HasProjectInput(currentInputValues, out projectId) && service.TryGetProjectName(requestContext, projectId, out projectName))
      {
        str = projectName;
        inputValueList.Add(new InputValue()
        {
          Value = projectId.ToString(),
          DisplayValue = projectName
        });
      }
      else
        inputValueList = (IList<InputValue>) service.GetProjects(requestContext, ProjectState.WellFormed).OrderBy<ProjectInfo, string>((Func<ProjectInfo, string>) (x => x.Name)).Select<ProjectInfo, InputValue>((Func<ProjectInfo, InputValue>) (project => new InputValue()
        {
          Value = project.Id.ToString(),
          DisplayValue = project.Name
        })).ToList<InputValue>();
      return new InputValues()
      {
        InputId = inputId,
        DefaultValue = str,
        PossibleValues = inputValueList,
        IsLimitedToPossibleValues = true,
        IsDisabled = false,
        IsReadOnly = false,
        Error = (InputValuesError) null
      };
    }

    protected static InputValue GetProjectDetails(Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition artifact)
    {
      if (artifact == null)
        throw new ArgumentNullException(nameof (artifact));
      string key = "project";
      InputValue projectDetails;
      if (!artifact.SourceData.TryGetValue(key, out projectDetails) || projectDetails == null)
        throw new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProjectDetailsNotAvailable, (object) artifact.SourceDataKeys, (object) key));
      return projectDetails;
    }

    protected static InputValue GetBuildDefinitionDetails(Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition artifact)
    {
      string str = artifact != null ? artifact.SourceDataKeys : throw new ArgumentNullException(nameof (artifact));
      string key = "definition";
      string errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BuildDefinitionDetailsNotAvailable, (object) str, (object) key);
      return FirstPartyArtifactTypeBase.GetDetailsFromSourceInputs((IDictionary<string, InputValue>) artifact.SourceData, key, true, errorMessage);
    }

    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", Justification = "string")]
    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "Need to return multiple values from this function")]
    protected static IList<DefinitionReference> GetDefinitionReferencesPaged(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string searchQuery,
      int[] definitionIds,
      ref string continuationToken)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectId == Guid.Empty)
        throw new ArgumentNullException(nameof (projectId));
      BuildHttpClient buildHttpClient = BuildArtifact.GetBuildHttpClient(requestContext);
      IList<DefinitionReference> definitionReferenceList1 = ((IEnumerable<int>) definitionIds).IsNullOrEmpty<int>() ? (!string.IsNullOrEmpty(searchQuery) ? BuildArtifact.SearchBuildDefinitionsPaged(requestContext, buildHttpClient, projectId, repositoryId, searchQuery, ref continuationToken) : BuildArtifact.GetBuildDefinitionsPaged(requestContext, buildHttpClient, projectId, repositoryId, (int[]) null, ref continuationToken)) : BuildArtifact.GetBuildDefinitionsPaged(requestContext, buildHttpClient, projectId, (string) null, definitionIds, ref continuationToken);
      bool flag = !string.IsNullOrEmpty(searchQuery) && definitionReferenceList1.Count<DefinitionReference>() == 1 || !((IEnumerable<int>) definitionIds).IsNullOrEmpty<int>() && definitionReferenceList1.Count<DefinitionReference>() == ((IEnumerable<int>) definitionIds).Count<int>();
      IList<DefinitionReference> definitionReferencesPaged = definitionReferenceList1;
      if (continuationToken == null && !flag)
      {
        XamlBuildHttpClient xamlBuildHttpClient = BuildArtifact.GetXamlBuildHttpClient(requestContext);
        IList<DefinitionReference> definitionReferenceList2 = !string.IsNullOrEmpty(searchQuery) ? BuildArtifact.GetXamlBuildDefinitions(requestContext, xamlBuildHttpClient, projectId, searchQuery) : BuildArtifact.GetXamlBuildDefinitions(requestContext, xamlBuildHttpClient, projectId);
        definitionReferencesPaged = (IList<DefinitionReference>) definitionReferenceList1.Concat<DefinitionReference>((IEnumerable<DefinitionReference>) (definitionReferenceList2 ?? (IList<DefinitionReference>) new List<DefinitionReference>())).OrderBy<DefinitionReference, string>((Func<DefinitionReference, string>) (definition => definition.Name)).ToList<DefinitionReference>();
      }
      return definitionReferencesPaged;
    }

    protected static IList<ResourceRef> GetWorkItems(
      IVssRequestContext requestContext,
      Guid projectId,
      int startBuildId,
      int endBuildId,
      int top)
    {
      BuildHttpClient buildHttpClient = requestContext != null ? BuildArtifact.GetBuildHttpClient(requestContext) : throw new ArgumentNullException(nameof (requestContext));
      if (startBuildId != 0)
        return (IList<ResourceRef>) buildHttpClient.GetWorkItemsBetweenBuildsAsync(projectId, startBuildId, endBuildId, new int?(top)).GetResult<List<ResourceRef>>(requestContext.CancellationToken);
      try
      {
        return (IList<ResourceRef>) buildHttpClient.GetBuildWorkItemsRefsAsync(projectId, endBuildId, new int?(top)).GetResult<List<ResourceRef>>(requestContext.CancellationToken);
      }
      catch (ReleaseManagementExternalServiceException ex)
      {
        if (ex.InnerException != null && ex.InnerException is BuildNotFoundException)
          return (IList<ResourceRef>) BuildArtifact.GetXamlBuildHttpClient(requestContext).GetBuildWorkItemsRefsAsync(projectId, endBuildId, new int?(top)).GetResult<List<ResourceRef>>(requestContext.CancellationToken);
        throw;
      }
    }

    protected static IList<Microsoft.TeamFoundation.Build.WebApi.Change> GetChanges(
      IVssRequestContext requestContext,
      Guid projectId,
      int startBuildId,
      int endBuildId,
      int top)
    {
      BuildHttpClient buildHttpClient = requestContext != null ? BuildArtifact.GetBuildHttpClient(requestContext) : throw new ArgumentNullException(nameof (requestContext));
      if (startBuildId != 0)
        return (IList<Microsoft.TeamFoundation.Build.WebApi.Change>) buildHttpClient.GetChangesBetweenBuildsAsync(projectId, new int?(startBuildId), new int?(endBuildId), new int?(top)).GetResult<List<Microsoft.TeamFoundation.Build.WebApi.Change>>(requestContext.CancellationToken);
      try
      {
        return (IList<Microsoft.TeamFoundation.Build.WebApi.Change>) buildHttpClient.GetBuildChangesAsync(projectId, endBuildId, top: new int?(top)).GetResult<List<Microsoft.TeamFoundation.Build.WebApi.Change>>(requestContext.CancellationToken);
      }
      catch (ReleaseManagementExternalServiceException ex)
      {
        if (ex.InnerException != null && ex.InnerException is BuildNotFoundException)
          return (IList<Microsoft.TeamFoundation.Build.WebApi.Change>) BuildArtifact.GetXamlBuildHttpClient(requestContext).GetBuildChangesAsync(projectId, endBuildId, top: new int?(top)).GetResult<List<Microsoft.TeamFoundation.Build.WebApi.Change>>(requestContext.CancellationToken);
        throw;
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "By Design.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By Design")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw exception, but want to set the error message.")]
    protected static IList<InputValue> GetBranchesInputValues(
      IVssRequestContext context,
      Guid projectId,
      int buildDefinitionId)
    {
      List<InputValue> branchesInputValues = new List<InputValue>();
      BuildDefinition buildDefinition = BuildArtifact.GetBuildDefinition(context, projectId, buildDefinitionId, false);
      if (BuildArtifact.IsBranchSupportedForBuildDefinitionRepository(buildDefinition))
      {
        string definitionRepositoryType = BuildArtifact.GetBuildDefinitionRepositoryType(buildDefinition);
        using (ReleaseManagementTimer.Create(context, "Service", "ArtifactTypeBuildBase.GetBranches", 1971056))
        {
          string errorMessage = (string) null;
          try
          {
            switch (definitionRepositoryType)
            {
              case "TfsGit":
                GitHttpClient gitHttpClient = context.GetClient<GitHttpClient>();
                Guid repositoryId;
                if (buildDefinition.Repository.Id == null || !Guid.TryParse(buildDefinition.Repository.Id, out repositoryId))
                  throw new InvalidRepositoryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InvalidRepository, (object) buildDefinition.Repository.Id, (object) buildDefinition.Name));
                Func<Task<List<GitRef>>> func = (Func<Task<List<GitRef>>>) (() => gitHttpClient.GetRefsAsync(projectId, repositoryId, "heads", new bool?(), new bool?(), new bool?(), new bool?(), new bool?(), (string) null, new int?(), (string) null, (object) null, new CancellationToken()));
                List<GitRef> gitRefList = context.ExecuteAsyncAndSyncResult<List<GitRef>>(func);
                if (gitRefList == null || gitRefList.Count == 0)
                {
                  errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.NoBranchesFound, (object) buildDefinition.Repository.Name);
                  break;
                }
                using (List<GitRef>.Enumerator enumerator = gitRefList.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    string str = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter.RemoveBranchPrefix(enumerator.Current.Name);
                    branchesInputValues.Add(new InputValue()
                    {
                      Value = str,
                      DisplayValue = str
                    });
                  }
                  break;
                }
              case "GitHub":
                Guid serviceEndpointGuid;
                BuildArtifact.ValidateAndGetServiceEndpointIdFromBuildDefinition(context, buildDefinition, out serviceEndpointGuid);
                GitHubEndpointHelper hubEndpointHelper = new GitHubEndpointHelper();
                branchesInputValues = !context.IsFeatureEnabled("AzureDevOps.ReleaseManagement.EnableGitHubDataSourcesForBuildArtifact") ? BuildArtifact.GetRepoBranchesFromGitHubHttpClient(context, projectId, buildDefinition, out errorMessage) : hubEndpointHelper.GetRepoBranches(context, projectId, serviceEndpointGuid, buildDefinition.Repository.Name, out errorMessage);
                break;
              case "GitHubEnterprise":
                string name1 = buildDefinition.Repository.Name;
                ServiceEndpoint serviceEndpoint1 = BuildArtifact.GetServiceEndpoint(context, projectId, buildDefinition);
                GitHubData.V3.Branch[] branchArray1 = (GitHubData.V3.Branch[]) null;
                string absoluteUri = serviceEndpoint1?.Url?.AbsoluteUri;
                GitHubAuthentication authentication1;
                if (!string.IsNullOrEmpty(name1) && !string.IsNullOrEmpty(absoluteUri) && BuildArtifact.TryGetGitHubAuthentication(context, serviceEndpoint1, projectId, out authentication1))
                {
                  GitHubResult<GitHubData.V3.Branch[]> repoBranches = BuildArtifact.GetGitHubHttpClient(context).GetRepoBranches(authentication1, absoluteUri, name1);
                  errorMessage = repoBranches.ErrorMessage;
                  branchArray1 = repoBranches.Result;
                }
                if (branchArray1 != null && branchArray1.Length != 0)
                {
                  foreach (GitHubData.V3.Branch branch in branchArray1)
                    branchesInputValues.Add(new InputValue()
                    {
                      Value = branch.Name,
                      DisplayValue = branch.Name
                    });
                  break;
                }
                if (string.IsNullOrEmpty(errorMessage))
                {
                  errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.NoBranchesFound, (object) name1);
                  break;
                }
                break;
              case "Bitbucket":
                ServiceEndpoint serviceEndpoint2 = BuildArtifact.GetServiceEndpoint(context, projectId, buildDefinition);
                if (serviceEndpoint2 != null)
                {
                  BitbucketData.Authentication authentication2;
                  if (BuildArtifact.TryGetBitbucketAuthentication(serviceEndpoint2, out authentication2))
                  {
                    BitbucketData.V2.Branch[] branchArray2 = (BitbucketData.V2.Branch[]) null;
                    string name2 = buildDefinition.Repository.Name;
                    if (!string.IsNullOrEmpty(name2))
                    {
                      BitbucketResult<BitbucketData.V2.Branch[]> repoBranches = BitbucketHttpClientFactory.Create(context).GetRepoBranches(authentication2, name2);
                      errorMessage = repoBranches.ErrorMessage;
                      branchArray2 = repoBranches.Result;
                    }
                    if (branchArray2 != null && branchArray2.Length != 0)
                    {
                      foreach (BitbucketData.V2.Branch branch in branchArray2)
                        branchesInputValues.Add(new InputValue()
                        {
                          Value = branch.Name,
                          DisplayValue = branch.Name
                        });
                      break;
                    }
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                      errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.NoBranchesFound, (object) name2);
                      break;
                    }
                    break;
                  }
                  break;
                }
                break;
            }
          }
          catch (AggregateException ex)
          {
            errorMessage = ExceptionsUtilities.GetAllInnerExceptionsMessages(ex);
          }
          catch (Exception ex)
          {
            errorMessage = ex.Message;
          }
          if (!string.IsNullOrEmpty(errorMessage))
          {
            context.Trace(1900000, TraceLevel.Error, "ReleaseManagementService", "Service", errorMessage);
            throw new InvalidRepositoryException(errorMessage);
          }
        }
      }
      return (IList<InputValue>) branchesInputValues;
    }

    private static List<InputValue> GetRepoBranchesFromGitHubHttpClient(
      IVssRequestContext context,
      Guid projectId,
      BuildDefinition buildDefinition,
      out string errorMessage)
    {
      List<InputValue> gitHubHttpClient = new List<InputValue>();
      errorMessage = string.Empty;
      string name = buildDefinition.Repository.Name;
      GitHubData.V3.Branch[] branchArray = (GitHubData.V3.Branch[]) null;
      GitHubAuthentication authentication = (GitHubAuthentication) null;
      ServiceEndpoint serviceEndpoint = BuildArtifact.GetServiceEndpoint(context, projectId, buildDefinition);
      if (serviceEndpoint != null)
        authentication = serviceEndpoint.GetGitHubAuthentication(context, projectId);
      if (!string.IsNullOrEmpty(name))
      {
        GitHubResult<GitHubData.V3.Branch[]> repoBranches = BuildArtifact.GetGitHubHttpClient(context).GetRepoBranches(authentication, (string) null, name);
        errorMessage = repoBranches.ErrorMessage;
        branchArray = repoBranches.Result;
      }
      if (branchArray != null && branchArray.Length != 0)
      {
        foreach (GitHubData.V3.Branch branch in branchArray)
          gitHubHttpClient.Add(new InputValue()
          {
            Value = branch.Name,
            DisplayValue = branch.Name
          });
      }
      else if (string.IsNullOrEmpty(errorMessage))
        errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.NoBranchesFound, (object) name);
      return gitHubHttpClient;
    }

    protected static IList<InputValue> GetTags(IVssRequestContext context, Guid projectId)
    {
      BuildHttpClient buildHttpClient = context != null ? BuildArtifact.GetBuildHttpClient(context) : throw new ArgumentNullException(nameof (context));
      List<string> source = (List<string>) null;
      using (ReleaseManagementTimer.Create(context, "Service", "ArtifactTypeBuildBase.GetTags", 1971057))
        source = buildHttpClient.GetTagsAsync(projectId, (object) context).GetResult<List<string>>(context.CancellationToken).Distinct<string>().ToList<string>();
      return (IList<InputValue>) source.Select<string, InputValue>((Func<string, InputValue>) (tag => new InputValue()
      {
        Value = tag,
        DisplayValue = tag
      })).ToList<InputValue>();
    }

    protected static IList<Microsoft.TeamFoundation.Build.WebApi.Build> GetBuilds(
      IVssRequestContext teamFoundationRequestContext,
      Guid projectId,
      int[] buildDefinitionIds,
      BuildStatus status,
      BuildResult result,
      string branchName,
      IEnumerable<string> tagFilters,
      int? top)
    {
      BuildHttpClient buildHttpClient = BuildArtifact.GetBuildHttpClient(teamFoundationRequestContext);
      IList<Microsoft.TeamFoundation.Build.WebApi.Build> builds = BuildArtifact.GetBuilds(teamFoundationRequestContext, buildHttpClient, projectId, buildDefinitionIds, status, result, branchName, tagFilters, top);
      if (builds.Count == 0)
      {
        XamlBuildHttpClient xamlBuildHttpClient = BuildArtifact.GetXamlBuildHttpClient(teamFoundationRequestContext);
        builds = BuildArtifact.GetXamlBuilds(teamFoundationRequestContext, xamlBuildHttpClient, projectId, buildDefinitionIds, status, result, top);
      }
      return builds;
    }

    protected static IList<FileContainerItem> GetArtifactItems(
      IVssRequestContext requestContext,
      Guid projectId,
      Uri buildUri,
      string itemPath)
    {
      FileContainerHttpClient containerHttpClient1 = requestContext != null ? requestContext.GetClient<FileContainerHttpClient>(Microsoft.VisualStudio.Services.WebApi.ServiceInstanceTypes.TFS) : throw new ArgumentNullException(nameof (requestContext));
      FileContainerHttpClient containerHttpClient2 = containerHttpClient1;
      List<Uri> artifactUris = new List<Uri>();
      artifactUris.Add(buildUri);
      Guid scopeIdentifier = projectId;
      CancellationToken cancellationToken = new CancellationToken();
      List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> result = containerHttpClient2.QueryContainersAsync(artifactUris, scopeIdentifier, cancellationToken: cancellationToken).GetResult<List<Microsoft.VisualStudio.Services.FileContainer.FileContainer>>(requestContext.CancellationToken);
      if (result.Count == 0)
        return (IList<FileContainerItem>) new List<FileContainerItem>();
      List<FileContainerItem> artifactItems = new List<FileContainerItem>();
      foreach (Microsoft.VisualStudio.Services.FileContainer.FileContainer fileContainer in result)
        artifactItems.AddRange((IEnumerable<FileContainerItem>) containerHttpClient1.QueryContainerItemsAsync(fileContainer.Id, projectId, true, itemPath).GetResult<List<FileContainerItem>>(requestContext.CancellationToken));
      return (IList<FileContainerItem>) artifactItems;
    }

    protected static string GetArtifactItemContent(
      IVssRequestContext requestContext,
      Guid projectId,
      Uri buildUri,
      string itemPath)
    {
      FileContainerHttpClient containerHttpClient1 = requestContext != null ? requestContext.GetClient<FileContainerHttpClient>(Microsoft.VisualStudio.Services.WebApi.ServiceInstanceTypes.TFS) : throw new ArgumentNullException(nameof (requestContext));
      FileContainerHttpClient containerHttpClient2 = containerHttpClient1;
      List<Uri> artifactUris = new List<Uri>();
      artifactUris.Add(buildUri);
      Guid scopeIdentifier = projectId;
      CancellationToken cancellationToken1 = new CancellationToken();
      List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> result = containerHttpClient2.QueryContainersAsync(artifactUris, scopeIdentifier, cancellationToken: cancellationToken1).GetResult<List<Microsoft.VisualStudio.Services.FileContainer.FileContainer>>(requestContext.CancellationToken);
      long containerId = 0;
      long num = 0;
      foreach (Microsoft.VisualStudio.Services.FileContainer.FileContainer fileContainer in result)
      {
        try
        {
          foreach (FileContainerItem fileContainerItem in containerHttpClient1.QueryContainerItemsAsync(fileContainer.Id, projectId, true, itemPath).GetResult<List<FileContainerItem>>(requestContext.CancellationToken))
          {
            if (string.CompareOrdinal(fileContainerItem.Path, itemPath) == 0)
            {
              containerId = fileContainer.Id;
              num = fileContainerItem.FileLength;
              break;
            }
          }
          if (containerId > 0L)
            break;
        }
        catch (ReleaseManagementExternalServiceException ex)
        {
        }
      }
      string empty = string.Empty;
      if (containerId <= 0L)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.FileNotFoundInArtifact, (object) itemPath));
      if (num <= 0L || num >= 4194304L)
        throw new InvalidOperationException(Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.FileSizeIsTooLargeThan4);
      CancellationToken cancellationToken2 = new CancellationToken();
      using (StreamReader streamReader = new StreamReader(containerHttpClient1.DownloadFileAsync(containerId, itemPath, cancellationToken2, projectId).GetResult<Stream>(requestContext.CancellationToken)))
        return streamReader.ReadToEnd();
    }

    protected static IList<GitRepository> GetRepositories(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return new GitHelper((IGitData) new GitData()).GetRepositories(requestContext, projectId);
    }

    protected InputValues GetRepositories(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      string inputId,
      IDictionary<string, string> currentInputValues)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (inputId == null)
        throw new ArgumentNullException(nameof (inputId));
      List<InputValue> source = new List<InputValue>();
      string enumerable = (string) null;
      Guid projectId;
      string repositoryId;
      if (currentInputValues.IsMultiDefinitionType() && FirstPartyArtifactTypeBase.IsArtifactEditingMode(currentInputValues) && FirstPartyArtifactTypeBase.HasProjectInput(currentInputValues, out projectId) && FirstPartyArtifactTypeBase.HasRepositoryInput(currentInputValues, out repositoryId))
      {
        string str = repositoryId;
        GitRepository gitRepository = this.repositoryRetriever(requestContext, projectId, repositoryId);
        if (gitRepository != null)
        {
          enumerable = gitRepository.Name;
          str = gitRepository.Name;
        }
        InputValue inputValue = new InputValue()
        {
          Value = repositoryId,
          DisplayValue = str
        };
        source.Add(inputValue);
      }
      else if (this.repositoriesRetriever != null)
      {
        foreach (GitRepository gitRepository in (IEnumerable<GitRepository>) this.repositoriesRetriever(requestContext, projectInfo.Id))
        {
          InputValue inputValue = new InputValue()
          {
            Value = gitRepository.Id.ToString(),
            DisplayValue = gitRepository.Name
          };
          source.Add(inputValue);
          if (enumerable.IsNullOrEmpty<char>())
            enumerable = gitRepository.Name;
        }
      }
      return new InputValues()
      {
        InputId = inputId,
        DefaultValue = enumerable,
        PossibleValues = (IList<InputValue>) source.OrderBy<InputValue, string>((Func<InputValue, string>) (x => x.DisplayValue)).ToList<InputValue>(),
        IsLimitedToPossibleValues = true,
        IsDisabled = false,
        IsReadOnly = false,
        Error = (InputValuesError) null
      };
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By design.")]
    protected override IDictionary<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue> GetArtifactConfigurationVariables(
      IVssRequestContext context,
      ArtifactSource artifactSource)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      Dictionary<string, InputValue> dictionary = artifactSource != null ? artifactSource.SourceData : throw new ArgumentNullException(nameof (artifactSource));
      Dictionary<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue> variables = new Dictionary<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue>();
      InputValue inputValue1 = (InputValue) null;
      InputValue versionData = (InputValue) null;
      InputValue inputValue2 = (InputValue) null;
      if (dictionary.TryGetValue("definition", out inputValue1))
      {
        variables.Add("definitionName", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue1.DisplayValue
        });
        variables.Add("definitionId", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue1.Value
        });
      }
      if (dictionary.TryGetValue("version", out versionData))
        variables.Add("buildNumber", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = versionData.DisplayValue
        });
      InputValue inputValue3;
      if (dictionary.TryGetValue(WellKnownPullRequestVariables.PullRequestId, out inputValue3))
        variables.Add("pullrequest.id", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue3.Value
        });
      InputValue inputValue4;
      if (dictionary.TryGetValue(WellKnownPullRequestVariables.PullRequestTargetBranch, out inputValue4))
      {
        variables.Add("pullrequest.targetBranch", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue4.Value
        });
        variables.Add("pullrequest.targetBranchName", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = ArtifactTypeBase.RefToBranchName(inputValue4.Value)
        });
      }
      InputValue inputValue5;
      if (dictionary.TryGetValue(WellKnownPullRequestVariables.PullRequestSourceBranch, out inputValue5))
      {
        variables.Add("pullrequest.sourceBranch", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue5.Value
        });
        variables.Add("pullrequest.sourceBranchName", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = ArtifactTypeBase.RefToBranchName(inputValue5.Value)
        });
      }
      if (dictionary.TryGetValue("project", out inputValue2))
      {
        variables.Add("projectId", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue2.Value
        });
        variables.Add("projectName", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue2.DisplayValue
        });
      }
      if (inputValue2 != null && versionData != null)
      {
        Guid result = Guid.TryParse(inputValue2.Value, out result) ? new Guid(inputValue2.Value) : throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InvalidProjectId, (object) versionData.Value));
        int buildId = BuildArtifact.TryParseBuildId(versionData, true);
        variables.Add("buildId", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        });
        if (!BuildArtifact.TryUpdateArtifactVariables(context, artifactSource, (IDictionary<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue>) variables))
        {
          Microsoft.TeamFoundation.Build.WebApi.Build build = this.buildRetriever(context, result, buildId);
          if (build != null)
          {
            if (build.Definition != null && !string.IsNullOrEmpty(build.Definition.Name))
              variables["definitionName"] = new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
              {
                Value = build.Definition.Name
              };
            if (!string.IsNullOrEmpty(build.BuildNumber))
              variables["buildNumber"] = new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
              {
                Value = build.BuildNumber
              };
            variables["sourceBranch"] = new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
            {
              Value = build.SourceBranch
            };
            variables["sourceBranchName"] = new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
            {
              Value = ArtifactTypeBase.GetBranchName(build.SourceBranch)
            };
            variables["sourceVersion"] = new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
            {
              Value = build.SourceVersion
            };
            variables["buildUri"] = new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
            {
              Value = build.Uri?.ToString()
            };
            variables["requestedForId"] = new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
            {
              Value = build.RequestedFor?.Id
            };
            if (build.Repository != null && !string.IsNullOrEmpty(build.Repository.Type))
            {
              variables.Add("repository.provider", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
              {
                Value = build.Repository.Type
              });
              if (build.Repository.Type.Equals("TfsGit", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(build.Repository.Id))
              {
                GitRepository gitRepository = this.repositoryRetriever(context, result, build.Repository.Id);
                if (gitRepository != null)
                {
                  variables.Add("repository.name", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
                  {
                    Value = gitRepository.Name
                  });
                  variables.Add("repository.id", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
                  {
                    Value = gitRepository.Id.ToString()
                  });
                }
              }
              else if (build.Repository.Type.Equals("TfsVersionControl", StringComparison.OrdinalIgnoreCase))
              {
                variables.Add("repository.name", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
                {
                  Value = inputValue2.DisplayValue
                });
                variables.Add("repository.id", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
                {
                  Value = inputValue2.Value
                });
              }
              else if (build.Repository.Type.Equals("GitHub", StringComparison.OrdinalIgnoreCase))
              {
                variables.Add("repository.name", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
                {
                  Value = build.Repository.Id
                });
                variables.Add("repository.id", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
                {
                  Value = build.Repository.Id
                });
              }
            }
            variables["requestedFor"] = new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
            {
              Value = build.RequestedFor?.DisplayName
            };
          }
          else
            BuildArtifact.TraceError(context, 1973115, "Build can not be found for BuildId {0} in Project {1}", (object) buildId, (object) inputValue2.DisplayValue);
        }
        IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact> buildArtifacts = BuildArtifact.GetBuildArtifacts(context, result, buildId);
        if (buildArtifacts == null)
        {
          context.Trace(1976400, TraceLevel.Info, "ReleaseManagementService", "ArtifactExtensions", Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.NoArtifactVersionsAvailable);
        }
        else
        {
          foreach (Microsoft.TeamFoundation.Build.WebApi.BuildArtifact buildArtifact in (IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>) buildArtifacts)
          {
            foreach (KeyValuePair<string, string> keyValuePair in buildArtifact.Resource?.Properties ?? new Dictionary<string, string>())
            {
              if (string.Equals(keyValuePair.Key, "definition", StringComparison.OrdinalIgnoreCase))
              {
                variables.Add(buildArtifact.Name + ".defintionId", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
                {
                  Value = keyValuePair.Value
                });
                variables.Add(buildArtifact.Name + ".definitionId", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
                {
                  Value = keyValuePair.Value
                });
              }
              if (string.Equals(keyValuePair.Key, "version", StringComparison.OrdinalIgnoreCase))
                variables.Add(buildArtifact.Name + ".buildId", new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
                {
                  Value = keyValuePair.Value
                });
            }
          }
        }
      }
      return (IDictionary<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue>) variables;
    }

    protected InputValues GetBranchInputValues(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, string> inputValues,
      int definitionId,
      string inputId)
    {
      if (inputValues == null)
        throw new ArgumentNullException(nameof (inputValues));
      if (inputId == null)
        throw new ArgumentNullException(nameof (inputId));
      string str = (string) null;
      List<InputValue> inputValueList = new List<InputValue>();
      try
      {
        if (!inputValues.HasLatestFromBranchDefaultVersionType())
        {
          if (!inputValues.HasLatestWithBranchAndTagsDefaultVersionType())
            goto label_9;
        }
        inputValueList = this.branchesRetriever(requestContext, projectId, definitionId).ToList<InputValue>();
      }
      catch (InvalidRepositoryException ex)
      {
        str = ex.Message;
      }
label_9:
      return new InputValues()
      {
        InputId = inputId,
        DefaultValue = string.Empty,
        PossibleValues = (IList<InputValue>) inputValueList,
        IsLimitedToPossibleValues = false,
        IsReadOnly = false,
        Error = new InputValuesError() { Message = str }
      };
    }

    protected InputValues GetTagsInputValues(
      IVssRequestContext requestContext,
      Guid projectId,
      IDictionary<string, string> inputValues,
      string inputId)
    {
      if (inputValues == null)
        throw new ArgumentNullException(nameof (inputValues));
      if (inputId == null)
        throw new ArgumentNullException(nameof (inputId));
      string str = (string) null;
      List<InputValue> inputValueList = new List<InputValue>();
      if (inputValues.HasLatestWithBranchAndTagsDefaultVersionType() || inputValues.HasLatestWithBuildDefinitionBranchAndTagsDefaultVersionType())
        inputValueList = this.tagsRetriever(requestContext, projectId).ToList<InputValue>();
      return new InputValues()
      {
        InputId = inputId,
        DefaultValue = string.Empty,
        PossibleValues = (IList<InputValue>) inputValueList,
        IsLimitedToPossibleValues = false,
        IsReadOnly = false,
        Error = new InputValuesError() { Message = str }
      };
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw exception, but want to set the error message.")]
    protected InputValues GetDefaultVersionBuildInputValues(
      IVssRequestContext requestContext,
      IDictionary<string, string> inputValues,
      string inputId)
    {
      if (inputValues == null)
        throw new ArgumentNullException(nameof (inputValues));
      if (inputId == null)
        throw new ArgumentNullException(nameof (inputId));
      string str = (string) null;
      List<InputValue> inputValueList = new List<InputValue>();
      if (inputValues.HasSpecificVersionDefaultVersionType())
      {
        if ((FirstPartyArtifactTypeBase.IsArtifactEditingMode(inputValues) ? 1 : (!inputValues.HasArtifactSpecificVersion() ? 1 : 0)) != 0)
          inputValues.Remove("defaultVersionType");
        try
        {
          inputValueList = this.GetAvailableVersions(requestContext, inputValues, new int?()).ToList<InputValue>();
        }
        catch (Exception ex)
        {
          str = ex.Message;
          requestContext.TraceException(1971018, "ReleaseManagementService", "Service", ex);
        }
      }
      return new InputValues()
      {
        InputId = inputId,
        DefaultValue = string.Empty,
        PossibleValues = (IList<InputValue>) inputValueList,
        IsLimitedToPossibleValues = true,
        IsReadOnly = false,
        Error = new InputValuesError() { Message = str }
      };
    }

    private static bool TrySetVariableFromSourceInput(
      IDictionary<string, InputValue> sourceInputs,
      IDictionary<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue> variables,
      string variableName)
    {
      bool flag = false;
      InputValue inputValue;
      if (sourceInputs.TryGetValue(variableName, out inputValue))
      {
        variables[variableName] = new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = inputValue.Value
        };
        flag = true;
      }
      return flag;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ReleaseArtifactSource", Justification = "This is a model name")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.TeamFoundation.Framework.Server.VssRequestContextExtensions.Trace(Microsoft.TeamFoundation.Framework.Server.IVssRequestContext,System.Int32,System.Diagnostics.TraceLevel,System.String,System.String,System.String)", Justification = "This message is not shown to users")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "we want to handle all exceptions here")]
    private static bool TryUpdateArtifactVariables(
      IVssRequestContext context,
      ArtifactSource artifactSource,
      IDictionary<string, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue> variables)
    {
      if (!(artifactSource is PipelineArtifactSource pipelineArtifactSource))
      {
        context.Trace(1976485, TraceLevel.Error, "ReleaseManagementService", "Service", "Cannot get ReleaseArtifactSource. Reading build object again.");
        return false;
      }
      try
      {
        if (!string.IsNullOrEmpty(pipelineArtifactSource.DefinitionsData?.DisplayValue))
          variables["definitionName"] = new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
          {
            Value = pipelineArtifactSource.DefinitionsData.DisplayValue
          };
        if (!string.IsNullOrEmpty(pipelineArtifactSource.ArtifactVersion))
        {
          InputValue inputValue = JsonConvert.DeserializeObject<InputValue>(pipelineArtifactSource.ArtifactVersion);
          variables["buildNumber"] = new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
          {
            Value = inputValue.DisplayValue
          };
        }
        if (string.IsNullOrEmpty(pipelineArtifactSource.SourceBranch))
          return false;
        variables["sourceBranch"] = new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = pipelineArtifactSource.SourceBranch
        };
        variables["sourceBranchName"] = new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
        {
          Value = ArtifactTypeBase.GetBranchName(pipelineArtifactSource.SourceBranch)
        };
        string[] strArray = new string[4]
        {
          "sourceVersion",
          "buildUri",
          "requestedForId",
          "requestedFor"
        };
        foreach (string variableName in strArray)
        {
          if (!BuildArtifact.TrySetVariableFromSourceInput((IDictionary<string, InputValue>) pipelineArtifactSource.SourceData, variables, variableName))
            return false;
        }
        BuildArtifact.TrySetVariableFromSourceInput((IDictionary<string, InputValue>) pipelineArtifactSource.SourceData, variables, "repository.provider");
        int num = BuildArtifact.TrySetVariableFromSourceInput((IDictionary<string, InputValue>) pipelineArtifactSource.SourceData, variables, "repository.name") ? 1 : 0;
        bool flag = BuildArtifact.TrySetVariableFromSourceInput((IDictionary<string, InputValue>) pipelineArtifactSource.SourceData, variables, "repository.id");
        if (num == 0)
        {
          if (!flag)
          {
            InputValue inputValue;
            if (pipelineArtifactSource.SourceData.TryGetValue("repository", out inputValue))
            {
              variables["repository.name"] = new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
              {
                Value = inputValue.DisplayValue
              };
              variables["repository.id"] = new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue()
              {
                Value = inputValue.Value
              };
            }
          }
        }
      }
      catch (Exception ex)
      {
        context.TraceException(1976489, TraceLevel.Info, "ReleaseManagementService", "Service", ex);
        return false;
      }
      return true;
    }

    private static IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> GetChangesForGitHubSource(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      BuildDefinition buildDefinition,
      int startBuildId,
      int endBuildId,
      int top)
    {
      return !requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.EnableGitHubDataSourcesForBuildArtifact") ? BuildArtifact.GetChangesFromGitHubHttpClient(requestContext, projectInfo, buildDefinition, startBuildId, endBuildId, top) : BuildArtifact.GetCommitsFromGitHubDataSource(requestContext, projectInfo, buildDefinition, startBuildId, endBuildId, top);
    }

    private static IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> GetChangesFromGitHubHttpClient(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      BuildDefinition buildDefinition,
      int startBuildId,
      int endBuildId,
      int top)
    {
      IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> gitHubHttpClient1 = (IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>) new List<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>();
      Microsoft.TeamFoundation.Build.WebApi.Build build1 = BuildArtifact.GetBuild(requestContext, projectInfo.Id, endBuildId);
      ServiceEndpoint serviceEndpoint = BuildArtifact.GetServiceEndpoint(requestContext, projectInfo.Id, buildDefinition);
      GitHubAuthentication authentication;
      if (!BuildArtifact.TryGetGitHubAuthentication(requestContext, serviceEndpoint, projectInfo.Id, out authentication))
        throw new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ProblemConnectingService);
      string id = buildDefinition?.Repository.Id;
      GitHubHttpClient gitHubHttpClient2 = BuildArtifact.GetGitHubHttpClient(requestContext);
      IEnumerable<GitHubData.V3.CommitListItem> source = (IEnumerable<GitHubData.V3.CommitListItem>) null;
      if (startBuildId == 0)
      {
        BuildArtifact.ValidateBuildAndBuildDefinitionRepositories((Microsoft.TeamFoundation.Build.WebApi.Build) null, build1, buildDefinition);
        UriBuilder uriBuilder = new GitHubApiRoot().CommitsUri(id, build1.SourceVersion);
        GitHubResult<GitHubData.V3.CommitListItem[]> commits = gitHubHttpClient2.GetCommits(authentication, uriBuilder.AbsoluteUri());
        if (commits.IsSuccessful && string.IsNullOrEmpty(commits.ErrorMessage))
        {
          source = (IEnumerable<GitHubData.V3.CommitListItem>) commits.Result;
        }
        else
        {
          string traceMessageFormat = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.GitHubCommitsError, (object) commits.StatusCode, (object) commits.ErrorMessage);
          BuildArtifact.TraceError(requestContext, 1980012, traceMessageFormat);
        }
      }
      else
      {
        Microsoft.TeamFoundation.Build.WebApi.Build build2 = BuildArtifact.GetBuild(requestContext, projectInfo.Id, startBuildId);
        BuildArtifact.ValidateBuildAndBuildDefinitionRepositories(build2, build1, buildDefinition);
        GitHubResult<GitHubData.V3.CommitsDiff> commitsDiff = gitHubHttpClient2.GetCommitsDiff(authentication, id, build2.SourceVersion, build1.SourceVersion);
        if (commitsDiff.IsSuccessful && string.IsNullOrEmpty(commitsDiff.ErrorMessage))
        {
          source = (IEnumerable<GitHubData.V3.CommitListItem>) commitsDiff.Result?.Commits;
        }
        else
        {
          string traceMessageFormat = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.GitHubCommitsError, (object) commitsDiff.StatusCode, (object) commitsDiff.ErrorMessage);
          BuildArtifact.TraceError(requestContext, 1980012, traceMessageFormat);
        }
      }
      if (source != null)
        gitHubHttpClient1 = ArtifactTypeUtility.ConvertGitHubCommitsToChanges(top > 0 ? source.Take<GitHubData.V3.CommitListItem>(top) : source);
      return gitHubHttpClient1;
    }

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1113:CommaMustBeOnSameLineAsPreviousParameter", Justification = "Reviewed. False alarm by StyleCop in 2417.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Reviewed. False alarm by StyleCop in 2435")]
    private static IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> GetCommitsFromGitHubDataSource(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      BuildDefinition buildDefinition,
      int startBuildId,
      int endBuildId,
      int top)
    {
      IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> gitHubDataSource = (IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>) null;
      Guid serviceEndpointGuid;
      BuildArtifact.ValidateAndGetServiceEndpointIdFromBuildDefinition(requestContext, buildDefinition, out serviceEndpointGuid);
      Microsoft.TeamFoundation.Build.WebApi.Build build1 = startBuildId != 0 ? BuildArtifact.GetBuild(requestContext, projectInfo.Id, startBuildId) : (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      Microsoft.TeamFoundation.Build.WebApi.Build build2 = BuildArtifact.GetBuild(requestContext, projectInfo.Id, endBuildId);
      string id1 = buildDefinition?.Repository.Id;
      GitHubEndpointHelper hubEndpointHelper = new GitHubEndpointHelper();
      BuildArtifact.ValidateBuildAndBuildDefinitionRepositories(build1, build2, buildDefinition);
      IVssRequestContext requestContext1 = requestContext;
      Guid id2 = projectInfo.Id;
      Guid serviceEndpointId = serviceEndpointGuid;
      string repositoryName = id1;
      string sourceVersion1 = build1?.SourceVersion;
      string sourceVersion2 = build2?.SourceVersion;
      GitHubData.V3.CommitListItem[] commitListItemArray;
      ref GitHubData.V3.CommitListItem[] local1 = ref commitListItemArray;
      string str;
      ref string local2 = ref str;
      hubEndpointHelper.GetCommitsDiff(requestContext1, id2, serviceEndpointId, repositoryName, sourceVersion1, sourceVersion2, out local1, out local2);
      IEnumerable<GitHubData.V3.CommitListItem> source = (IEnumerable<GitHubData.V3.CommitListItem>) commitListItemArray;
      if (str.IsNullOrEmpty<char>() && source != null)
        gitHubDataSource = ArtifactTypeUtility.ConvertGitHubCommitsToChanges(top > 0 ? source.Take<GitHubData.V3.CommitListItem>(top) : source);
      else
        BuildArtifact.TraceError(requestContext, 1976490, str);
      return gitHubDataSource;
    }

    private static IList<WorkItemRef> GetDirectlyRelatedWorkItems(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      BuildDefinition buildDefinition,
      IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> changes)
    {
      List<string> stringList = new List<string>();
      stringList.AddRange(BuildArtifact.GetArtifactUrisForChanges(changes, buildDefinition.Repository));
      if (stringList.Count > 0)
      {
        ArtifactUriQuery artifactUriQuery = new ArtifactUriQuery()
        {
          ArtifactUris = (IEnumerable<string>) stringList
        };
        WorkItemTrackingHttpClient trackingHttpClient = BuildArtifact.GetWorkItemTrackingHttpClient(requestContext);
        IEnumerable<int> ints = trackingHttpClient.QueryWorkItemsForArtifactUrisAsync(artifactUriQuery, projectInfo.Id).GetResult<ArtifactUriQueryResult>(requestContext.CancellationToken).ArtifactUrisQueryResult.Where<KeyValuePair<string, IEnumerable<WorkItemReference>>>((Func<KeyValuePair<string, IEnumerable<WorkItemReference>>, bool>) (pair => pair.Value != null && pair.Value.Count<WorkItemReference>() != 0)).SelectMany<KeyValuePair<string, IEnumerable<WorkItemReference>>, WorkItemReference>((Func<KeyValuePair<string, IEnumerable<WorkItemReference>>, IEnumerable<WorkItemReference>>) (pair => pair.Value)).Select<WorkItemReference, int>((Func<WorkItemReference, int>) (workItemRef => workItemRef.Id));
        List<string> fields = new List<string>()
        {
          "System.State",
          "System.Title",
          "System.WorkItemType",
          "System.TeamProject"
        };
        if (ints != null && ints.Count<int>() > 0)
        {
          IEnumerable<WorkItem> result = (IEnumerable<WorkItem>) trackingHttpClient.GetWorkItemsAsync(projectInfo.Id, ints, (IEnumerable<string>) fields).GetResult<List<WorkItem>>(requestContext.CancellationToken);
          foreach (WorkItem workItem in result)
          {
            string projectName = workItem.Fields["System.TeamProject"] != null ? workItem.Fields["System.TeamProject"].ToString() : projectInfo.Name;
            string itemWebAccessUri = WebAccessUrlBuilder.GetWorkItemWebAccessUri(WebAccessUrlBuilder.GetCollectionUrl(requestContext), projectName, workItem.Id);
            workItem.Fields.Add("Release.WorkItemWebUrl", (object) itemWebAccessUri);
          }
          return (IList<WorkItemRef>) result.Where<WorkItem>((Func<WorkItem, bool>) (workItem => workItem != null)).Select<WorkItem, WorkItemRef>((Func<WorkItem, WorkItemRef>) (workItem => ReleaseWorkItemConverter.AzureDevWorkItemsToReleaseWorkItem(workItem))).ToList<WorkItemRef>();
        }
      }
      return (IList<WorkItemRef>) new List<WorkItemRef>();
    }

    private static IEnumerable<string> GetArtifactUrisForChanges(
      IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> changes,
      BuildRepository repository)
    {
      ArgumentUtility.CheckForNull<IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>>(changes, nameof (changes));
      ArgumentUtility.CheckForNull<BuildRepository>(repository, nameof (repository));
      ArgumentUtility.CheckStringForNullOrEmpty(repository.Name, "repository.Name");
      return changes.Select<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change, string>((Func<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change, string>) (change => BuildArtifact.GetUserFriendlyArtifactUrl("GitHub", "Commit", "github.com", repository.Name, change.Id)));
    }

    private static string GetUserFriendlyArtifactUrl(
      string toolName,
      string artifactType,
      string providerKey,
      string fullRepoName,
      string identifier)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(toolName, nameof (toolName));
      ArgumentUtility.CheckStringForNullOrEmpty(artifactType, nameof (artifactType));
      ArgumentUtility.CheckStringForNullOrEmpty(fullRepoName, nameof (fullRepoName));
      ArgumentUtility.CheckStringForNullOrEmpty(identifier, nameof (identifier));
      ArgumentUtility.CheckStringForNullOrEmpty(providerKey, nameof (providerKey));
      string specificId = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", (object) Uri.EscapeDataString(providerKey), (object) Uri.EscapeDataString(fullRepoName), (object) Uri.EscapeDataString(identifier));
      return LinkingUtilities.EncodeUri(new Microsoft.VisualStudio.Services.Common.ArtifactId(toolName, artifactType, specificId));
    }

    private static IList<WorkItemRef> GetIssuesForGitHubSource(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      BuildDefinition buildDefinition,
      IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> commits,
      int top)
    {
      IList<WorkItemRef> issuesForGitHubSource = (IList<WorkItemRef>) new List<WorkItemRef>();
      ServiceEndpoint serviceEndpoint = BuildArtifact.GetServiceEndpoint(requestContext, projectInfo.Id, buildDefinition);
      GitHubAuthentication authentication;
      if (!BuildArtifact.TryGetGitHubAuthentication(requestContext, serviceEndpoint, projectInfo.Id, out authentication))
        return issuesForGitHubSource;
      if (buildDefinition != null)
      {
        string id = buildDefinition.Repository.Id;
      }
      GitHubHttpClient gitHubHttpClient = BuildArtifact.GetGitHubHttpClient(requestContext);
      IEnumerable<string> commitsMessages = commits.Select<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change, string>((Func<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change, string>) (c => c.Message));
      IDictionary<string, IList<string>> relatedGitHubIssuesIds = BuildArtifact.GetRelatedGitHubIssuesIds(requestContext, buildDefinition, commitsMessages, top);
      GitHubData.V4.Issue[] issueArray = new GitHubData.V4.Issue[0];
      if (relatedGitHubIssuesIds != null && relatedGitHubIssuesIds.Keys != null)
      {
        BuildArtifact.PublishGitHubIssuesTelemetry(requestContext, relatedGitHubIssuesIds.Keys.Count);
        foreach (KeyValuePair<string, IList<string>> keyValuePair in (IEnumerable<KeyValuePair<string, IList<string>>>) relatedGitHubIssuesIds)
        {
          string key = keyValuePair.Key;
          GitHubResult<GitHubData.V4.Issue[]> gitHubIssues = BuildArtifact.GetGitHubIssues(gitHubHttpClient, authentication, key, (IEnumerable<string>) keyValuePair.Value);
          if (gitHubIssues != null && gitHubIssues.Result != null)
            issueArray = ((IEnumerable<GitHubData.V4.Issue>) issueArray).Concat<GitHubData.V4.Issue>((IEnumerable<GitHubData.V4.Issue>) gitHubIssues.Result).ToArray<GitHubData.V4.Issue>();
        }
      }
      return (IList<WorkItemRef>) ((IEnumerable<GitHubData.V4.Issue>) issueArray).Where<GitHubData.V4.Issue>((Func<GitHubData.V4.Issue, bool>) (issue => issue != null)).Select<GitHubData.V4.Issue, WorkItemRef>((Func<GitHubData.V4.Issue, WorkItemRef>) (issue => ReleaseWorkItemConverter.GitHubIssueToReleaseWorkItem(issue))).ToList<WorkItemRef>();
    }

    private static GitHubResult<GitHubData.V4.Issue[]> GetGitHubIssues(
      GitHubHttpClient gitHubHttpClient,
      GitHubAuthentication gitHubAuthentication,
      string repositoryName,
      IEnumerable<string> issuesIds)
    {
      return gitHubHttpClient.GetIssues(gitHubAuthentication, repositoryName, issuesIds);
    }

    private static IDictionary<string, IList<string>> GetRelatedGitHubIssuesIds(
      IVssRequestContext requestContext,
      BuildDefinition buildDefinition,
      IEnumerable<string> commitsMessages,
      int top)
    {
      IDictionary<string, IList<string>> relatedGitHubIssuesIds = (IDictionary<string, IList<string>>) new Dictionary<string, IList<string>>();
      int num = 0;
      foreach (string commitsMessage in commitsMessages)
      {
        MatchCollection matchCollection = BuildArtifact.IssueRegex.Matches(commitsMessage);
        if (matchCollection != null)
        {
          foreach (Match match in matchCollection)
          {
            int result;
            if (!int.TryParse(match.Groups["issue"].Value, out result))
            {
              BuildArtifact.TraceError(requestContext, 1980012, "GetRelatedGitHubIssuesIds could not get parse issue");
            }
            else
            {
              string name = match.Groups["reponame"].Value;
              if (string.IsNullOrEmpty(name))
              {
                name = buildDefinition?.Repository.Name;
              }
              else
              {
                string[] strArray = name.Split('/');
                if (strArray.Length != 2 || string.IsNullOrWhiteSpace(strArray[0]) || string.IsNullOrWhiteSpace(strArray[0]))
                  name = buildDefinition?.Repository.Name;
              }
              if (relatedGitHubIssuesIds.ContainsKey(name))
              {
                if (!relatedGitHubIssuesIds[name].Contains<string>(result.ToString((IFormatProvider) CultureInfo.InvariantCulture)))
                {
                  IList<string> stringList = relatedGitHubIssuesIds[name];
                  stringList.Add(result.ToString((IFormatProvider) CultureInfo.InvariantCulture));
                  relatedGitHubIssuesIds[name] = stringList;
                }
                else
                  continue;
              }
              else
                relatedGitHubIssuesIds.Add(name, (IList<string>) new List<string>()
                {
                  result.ToString((IFormatProvider) CultureInfo.InvariantCulture)
                });
              ++num;
              if (num >= top)
                return relatedGitHubIssuesIds;
            }
          }
        }
      }
      return relatedGitHubIssuesIds;
    }

    private static void PublishGitHubIssuesTelemetry(
      IVssRequestContext requestContext,
      int repositoryCount)
    {
      if (repositoryCount <= 1)
        return;
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("RepositoryCount", (double) repositoryCount);
      IVssRequestContext requestContext1 = requestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, "ReleaseMangementWorkItems", "GitHubIssues", properties);
    }

    private static void PublishGitHubWorkItemsTelemetry(
      IVssRequestContext requestContext,
      CustomerIntelligenceData telemetryData)
    {
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ReleaseMangementWorkItems", "GitHubWorkItems", telemetryData);
    }

    private static void PublishDeploymentStatusToWorkItemsTelemetry(
      IVssRequestContext requestContext,
      CustomerIntelligenceData telemetryData)
    {
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ReleaseMangementWorkItems", "PublishDeploymentStatusToWorkItems", telemetryData);
    }

    private static void ValidateBuildAndBuildDefinitionRepositories(
      Microsoft.TeamFoundation.Build.WebApi.Build startBuild,
      Microsoft.TeamFoundation.Build.WebApi.Build endBuild,
      BuildDefinition buildDefinition)
    {
      if (startBuild != null && (string.IsNullOrEmpty(startBuild.Repository?.Id) || string.IsNullOrEmpty(startBuild.Repository?.Type)))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.RepositoryInformationMissing, (object) Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BuildAssociatedWithPreviousRelease));
      if (string.IsNullOrEmpty(endBuild?.Repository?.Id) || string.IsNullOrEmpty(endBuild?.Repository?.Type))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.RepositoryInformationMissing, (object) Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BuildAssociatedWithCurrentRelease));
      if (string.IsNullOrEmpty(buildDefinition?.Repository?.Id) || string.IsNullOrEmpty(buildDefinition?.Repository?.Type))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.RepositoryInformationMissing, (object) nameof (buildDefinition)));
      if (startBuild != null)
      {
        if (!string.Equals(startBuild.Repository.Id, endBuild.Repository.Id, StringComparison.OrdinalIgnoreCase))
          throw new InvalidOperationException(Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.CannotGetCommitsAsBuildSourcesAreDifferent);
        if (!string.Equals(startBuild.Repository.Id, buildDefinition.Repository.Id, StringComparison.OrdinalIgnoreCase))
          throw new InvalidOperationException(Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.CannotGetCommitsAsBuildDefinitionChanged);
      }
      if (!string.Equals(endBuild.Repository.Id, buildDefinition.Repository.Id, StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.CannotGetCommitsAsBuildDefinitionChanged);
    }

    private static bool TryGetGitHubAuthentication(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint,
      Guid projectId,
      out GitHubAuthentication authentication)
    {
      authentication = serviceEndpoint != null ? serviceEndpoint.GetGitHubAuthentication(requestContext, projectId) : (GitHubAuthentication) null;
      return authentication != null;
    }

    private static string BuildWorkItemLinkUri(Guid projectId, int releaseId, int environmentId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.WorkItemLinkUriFormat, (object) projectId, (object) releaseId, (object) environmentId);

    private static bool TryGetBitbucketAuthentication(
      ServiceEndpoint serviceEndpoint,
      out BitbucketData.Authentication authentication)
    {
      authentication = (BitbucketData.Authentication) null;
      if (serviceEndpoint?.Authorization?.Parameters == null)
        return false;
      switch (serviceEndpoint.Authorization.Scheme)
      {
        case "UsernamePassword":
          string username;
          string password;
          if (serviceEndpoint.Authorization.Parameters.TryGetValue("Username", out username) && serviceEndpoint.Authorization.Parameters.TryGetValue("Password", out password))
          {
            authentication = new BitbucketData.Authentication(username, password);
            return true;
          }
          break;
        case "OAuth":
        case "OAuth2":
          string accessToken;
          if (serviceEndpoint.Authorization.Parameters.TryGetValue("AccessToken", out accessToken))
          {
            authentication = new BitbucketData.Authentication(accessToken);
            return true;
          }
          break;
      }
      return false;
    }

    private static ServiceEndpoint GetServiceEndpoint(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildDefinition buildDefinition)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      BuildRepository buildRepository = buildDefinition != null ? buildDefinition.Repository : throw new ArgumentNullException(nameof (buildDefinition));
      string input = (string) null;
      if (buildRepository?.Properties == null)
        throw new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.RepositoryDetailsMissing);
      if (buildRepository.Properties.TryGetValue("connectedServiceId", out input) && !string.IsNullOrEmpty(input))
      {
        Guid result;
        if (Guid.TryParse(input, out result))
        {
          try
          {
            return ServiceEndpointHelper.GetServiceEndpoint(requestContext, projectId, result);
          }
          catch (Exception ex)
          {
            requestContext.TraceCatch(1971017, "ReleaseManagementService", "Service", ex);
            throw new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.InvalidRequestException(ex.Message);
          }
        }
      }
      throw new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InvalidServiceEndpointId, (object) input));
    }

    private static void ValidateAndGetServiceEndpointIdFromBuildDefinition(
      IVssRequestContext requestContext,
      BuildDefinition buildDefinition,
      out Guid serviceEndpointGuid)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      BuildRepository buildRepository = buildDefinition != null ? buildDefinition.Repository : throw new ArgumentNullException(nameof (buildDefinition));
      string input = (string) null;
      if (buildRepository?.Properties == null)
        throw new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.RepositoryDetailsMissing);
      if (!buildRepository.Properties.TryGetValue("connectedServiceId", out input) || string.IsNullOrEmpty(input) || !Guid.TryParse(input, out serviceEndpointGuid))
        throw new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InvalidServiceEndpointId, (object) input));
    }

    private static Microsoft.TeamFoundation.Build.WebApi.Build GetBuildInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId)
    {
      BuildHttpClient buildHttpClient = BuildArtifact.GetBuildHttpClient(requestContext);
      try
      {
        BuildArtifact.Trace(requestContext, 1976371, TraceLevel.Info, "Fetching build {0} from project {1}", (object) buildId, (object) projectId);
        Func<Task<Microsoft.TeamFoundation.Build.WebApi.Build>> func = (Func<Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) (() => buildHttpClient.GetBuildAsync(projectId, buildId));
        return requestContext.ExecuteAsyncAndGetResult<Microsoft.TeamFoundation.Build.WebApi.Build>(func);
      }
      catch (ReleaseManagementExternalServiceException ex1)
      {
        if (ex1.InnerException != null && ex1.InnerException is BuildNotFoundException)
        {
          XamlBuildHttpClient xamlBuildHttpClient = BuildArtifact.GetXamlBuildHttpClient(requestContext);
          try
          {
            BuildArtifact.Trace(requestContext, 1976372, TraceLevel.Info, "Fetching xaml build {0} from project {1}", (object) buildId, (object) projectId);
            return xamlBuildHttpClient.GetBuildAsync(projectId, buildId).GetResult<Microsoft.TeamFoundation.Build.WebApi.Build>(requestContext.CancellationToken);
          }
          catch (ReleaseManagementExternalServiceException ex2)
          {
            if (ex2.InnerException != null)
            {
              if (ex2.InnerException is BuildNotFoundException)
                throw new ReleaseManagementExternalServiceException(BuildArtifact.GetBuildNotFoundMessage<int>(requestContext, buildId), (Exception) ex2);
            }
          }
          throw;
        }
        else
          throw;
      }
    }

    private static Microsoft.TeamFoundation.Build.WebApi.Build GetBuildInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int[] buildDefinitionIds,
      string buildNumber)
    {
      BuildHttpClient buildHttpClient = BuildArtifact.GetBuildHttpClient(requestContext);
      BuildArtifact.Trace(requestContext, 1976375, TraceLevel.Info, "Fetching build {0} from project {2}", (object) buildNumber, (object) buildDefinitionIds, (object) projectId);
      Func<Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>>> func = (Func<Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>>>) (() => buildHttpClient.GetBuildsAsync(projectId, (IEnumerable<int>) buildDefinitionIds, (IEnumerable<int>) null, buildNumber, new DateTime?(), new DateTime?(), (string) null, new BuildReason?(), new BuildStatus?(), new BuildResult?(), (IEnumerable<string>) null, (IEnumerable<string>) null, new int?(), (string) null, new int?(), new QueryDeletedOption?(), new BuildQueryOrder?(), (string) null, (IEnumerable<int>) null, (string) null, (string) null, (object) null, new CancellationToken()));
      List<Microsoft.TeamFoundation.Build.WebApi.Build> result = requestContext.ExecuteAsyncAndGetResult<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(func);
      if (result == null || !result.Any<Microsoft.TeamFoundation.Build.WebApi.Build>())
        throw new BuildNotFoundException(BuildArtifact.GetBuildNotFoundMessage<string>(requestContext, buildNumber));
      return result.Count <= 1 ? result[0] : throw new ReleaseManagementExternalServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.MultipleBuildsFoundErrorMessage, (object) buildNumber));
    }

    private static string GetBuildNotFoundMessage<T>(
      IVssRequestContext requestContext,
      T buildIdentifier)
    {
      string buildNotFoundMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.BuildDoesNotExistErrorMessage, (object) buildIdentifier);
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        buildNotFoundMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) buildNotFoundMessage, (object) Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.EnsureViewBuidPermissionForProjectCollectionServiceAccounts);
      return buildNotFoundMessage;
    }

    private static int TryParseBuildId(InputValue versionData, bool shouldThrow)
    {
      if (versionData == null)
        return 0;
      int result;
      if (!string.IsNullOrEmpty(versionData.Value) && int.TryParse(versionData.Value, out result))
        return result;
      if (shouldThrow)
        throw new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.InvalidBuildId, (object) versionData.Value));
      return 0;
    }

    private static string GetBuildDefinitionRepositoryType(BuildDefinition buildDefinition) => buildDefinition == null || buildDefinition.Repository == null || string.IsNullOrEmpty(buildDefinition.Repository.Type) ? (string) null : buildDefinition.Repository.Type;

    private static bool IsBranchSupportedForBuildDefinitionRepository(
      IVssRequestContext context,
      Guid projectId,
      int buildDefinitionId)
    {
      return BuildArtifact.IsBranchSupportedForBuildDefinitionRepository(BuildArtifact.GetBuildDefinition(context, projectId, buildDefinitionId, false));
    }

    private static bool IsBranchSupportedForBuildDefinitionRepository(
      BuildDefinition buildDefinition)
    {
      if (buildDefinition == null || buildDefinition.Repository == null || string.IsNullOrEmpty(buildDefinition.Repository.Type))
        return false;
      return buildDefinition.Repository.Type.Equals("TfsGit", StringComparison.OrdinalIgnoreCase) || buildDefinition.Repository.Type.Equals("GitHub", StringComparison.OrdinalIgnoreCase) || buildDefinition.Repository.Type.Equals("Bitbucket", StringComparison.OrdinalIgnoreCase) || buildDefinition.Repository.Type.Equals("GitHubEnterprise", StringComparison.OrdinalIgnoreCase) || buildDefinition.Repository.Type.Equals("Git", StringComparison.OrdinalIgnoreCase);
    }

    private static bool LogGetBuildDefinitionFailure(
      IVssRequestContext context,
      ReleaseManagementExternalServiceException exception,
      bool throwExceptionIfNotFound)
    {
      bool definitionFailure = true;
      if (exception.InnerException != null)
      {
        if (exception.InnerException is DefinitionNotFoundException)
        {
          context.TraceException(1971013, "ReleaseManagementService", "Service", exception.InnerException);
          definitionFailure = throwExceptionIfNotFound;
        }
        if (exception.InnerException is ProjectDoesNotExistException)
        {
          context.TraceException(1971013, "ReleaseManagementService", "Service", exception.InnerException);
          throw new ReleaseManagementExternalServiceException(exception.InnerException.Message, exception.InnerException);
        }
        if (exception.InnerException is VssServiceResponseException && exception.InnerException.InnerException != null && exception.InnerException.InnerException is UnauthorizedAccessException)
        {
          context.TraceException(1971013, "ReleaseManagementService", "Service", exception.InnerException.InnerException);
          throw new ReleaseManagementExternalServiceException(exception.InnerException.InnerException.Message, exception.InnerException.InnerException);
        }
      }
      return definitionFailure;
    }

    private static IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> GetChangesForSingleBuildArtifact(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build.WebApi.Build buildArtifact,
      Guid projectId,
      int top)
    {
      if (string.Equals(buildArtifact.Repository.Type, "TfsGit", StringComparison.OrdinalIgnoreCase))
        return BuildChangeConverter.ToReleaseChanges(BuildArtifact.GetChanges(requestContext, projectId, 0, buildArtifact.Id, top));
      return string.Equals(buildArtifact.Repository.Type, "TfsVersionControl", StringComparison.OrdinalIgnoreCase) ? TfvcChangesetRefConverter.ToReleaseChanges((IList<TfvcChangesetRef>) BuildArtifact.GetChangesetsFromTfvcRepository(requestContext, buildArtifact.SourceVersion, buildArtifact.SourceBranch, projectId, top)) : (IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>) new List<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>();
    }

    private static List<TfvcChangesetRef> GetChangesetsFromTfvcRepository(
      IVssRequestContext requestContext,
      string sourceVersion,
      string sourceBranch,
      Guid projectId,
      int top)
    {
      return requestContext.GetClient<TfvcHttpClient>().GetChangesetsAsync(projectId, top: new int?(top), searchCriteria: new TfvcChangesetSearchCriteria()
      {
        ItemPath = sourceBranch,
        ToId = Convert.ToInt32(sourceVersion, (IFormatProvider) CultureInfo.InvariantCulture)
      }).GetResult<List<TfvcChangesetRef>>(requestContext.CancellationToken);
    }

    private static IList<Microsoft.TeamFoundation.Build.WebApi.Build> GetBuilds(
      IVssRequestContext requestContext,
      BuildHttpClient buildHttpClient,
      Guid projectId,
      int[] buildDefinitionIds,
      BuildStatus status,
      BuildResult result,
      string branchName,
      IEnumerable<string> tagFilters,
      int? top)
    {
      bool flag = false;
      if (!string.IsNullOrEmpty(branchName))
        flag = BuildArtifact.IsBranchSupportedForBuildDefinitionRepository(requestContext, projectId, buildDefinitionIds[0]);
      int? nullable1 = new int?();
      int? nullable2;
      if (top.HasValue)
      {
        nullable1 = top;
        int length = buildDefinitionIds.Length;
        nullable2 = top;
        top = nullable2.HasValue ? new int?(length * nullable2.GetValueOrDefault()) : new int?();
      }
      using (ReleaseManagementTimer.Create(requestContext, "Service", "ArtifactTypeBuildBase.GetBuilds", 1971018))
      {
        BuildHttpClient buildHttpClient1 = buildHttpClient;
        Guid project = projectId;
        int[] definitions = buildDefinitionIds;
        BuildStatus? nullable3 = new BuildStatus?(status);
        BuildResult? nullable4 = new BuildResult?(result);
        IEnumerable<string> strings = tagFilters;
        nullable2 = top;
        int? nullable5 = nullable1;
        string str = flag ? Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter.AddBranchPrefixIfRequired(branchName) : branchName;
        object obj = (object) requestContext;
        DateTime? minTime = new DateTime?();
        DateTime? maxTime = new DateTime?();
        BuildReason? reasonFilter = new BuildReason?();
        BuildStatus? statusFilter = nullable3;
        BuildResult? resultFilter = nullable4;
        IEnumerable<string> tagFilters1 = strings;
        int? top1 = nullable2;
        int? maxBuildsPerDefinition = nullable5;
        QueryDeletedOption? deletedFilter = new QueryDeletedOption?();
        BuildQueryOrder? queryOrder = new BuildQueryOrder?();
        string branchName1 = str;
        object userState = obj;
        CancellationToken cancellationToken = new CancellationToken();
        return (IList<Microsoft.TeamFoundation.Build.WebApi.Build>) buildHttpClient1.GetBuildsAsync(project, (IEnumerable<int>) definitions, (IEnumerable<int>) null, (string) null, minTime, maxTime, (string) null, reasonFilter, statusFilter, resultFilter, tagFilters1, (IEnumerable<string>) null, top1, (string) null, maxBuildsPerDefinition, deletedFilter, queryOrder, branchName1, (IEnumerable<int>) null, (string) null, (string) null, userState, cancellationToken).GetResult<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(requestContext.CancellationToken);
      }
    }

    private static IList<Microsoft.TeamFoundation.Build.WebApi.Build> GetXamlBuilds(
      IVssRequestContext requestContext,
      XamlBuildHttpClient xamlBuildHttpClient,
      Guid projectId,
      int[] buildDefinitionIds,
      BuildStatus status,
      BuildResult result,
      int? top)
    {
      using (ReleaseManagementTimer.Create(requestContext, "Service", "ArtifactTypeBuildBase.GetXamlBuilds", 1971045))
      {
        XamlBuildHttpClient xamlBuildHttpClient1 = xamlBuildHttpClient;
        Guid project = projectId;
        int[] definitions = buildDefinitionIds;
        BuildStatus? nullable1 = new BuildStatus?(status);
        BuildResult? nullable2 = new BuildResult?(result);
        int? nullable3 = top;
        object obj = (object) requestContext;
        DateTime? minFinishTime = new DateTime?();
        DateTime? maxFinishTime = new DateTime?();
        BuildReason? reasonFilter = new BuildReason?();
        BuildStatus? statusFilter = nullable1;
        BuildResult? resultFilter = nullable2;
        int? top1 = nullable3;
        int? maxBuildsPerDefinition = new int?();
        QueryDeletedOption? deletedFilter = new QueryDeletedOption?();
        BuildQueryOrder? queryOrder = new BuildQueryOrder?();
        object userState = obj;
        CancellationToken cancellationToken = new CancellationToken();
        return (IList<Microsoft.TeamFoundation.Build.WebApi.Build>) xamlBuildHttpClient1.GetBuildsAsync(project, (IEnumerable<int>) definitions, minFinishTime: minFinishTime, maxFinishTime: maxFinishTime, reasonFilter: reasonFilter, statusFilter: statusFilter, resultFilter: resultFilter, top: top1, maxBuildsPerDefinition: maxBuildsPerDefinition, deletedFilter: deletedFilter, queryOrder: queryOrder, userState: userState, cancellationToken: cancellationToken).GetResult<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(requestContext.CancellationToken);
      }
    }

    private static WorkItemTrackingHttpClient GetWorkItemTrackingHttpClient(
      IVssRequestContext teamFoundationRequestContext)
    {
      using (ReleaseManagementTimer.Create(teamFoundationRequestContext, "Service", "BuildArtifact.GetWorkItemTrackingHttpClient", 1971064))
        return teamFoundationRequestContext.GetClient<WorkItemTrackingHttpClient>();
    }

    private static BuildHttpClient GetBuildHttpClient(
      IVssRequestContext teamFoundationRequestContext)
    {
      using (ReleaseManagementTimer.Create(teamFoundationRequestContext, "Service", "BuildArtifact.GetBuildHttpClient", 1971012))
        return teamFoundationRequestContext.GetClient<BuildHttpClient>();
    }

    private static XamlBuildHttpClient GetXamlBuildHttpClient(
      IVssRequestContext teamFoundationRequestContext)
    {
      using (ReleaseManagementTimer.Create(teamFoundationRequestContext, "Service", "BuildArtifact.GetXamlBuildHttpClient", 1971043))
        return teamFoundationRequestContext != null ? teamFoundationRequestContext.GetClient<XamlBuildHttpClient>() : (XamlBuildHttpClient) null;
    }

    private static BuildDefinition GetBuildDefinition(
      IVssRequestContext context,
      Guid projectId,
      int buildDefinitionId,
      bool throwExceptionIfNotFound)
    {
      BuildDefinition buildDefinition = (BuildDefinition) null;
      try
      {
        BuildHttpClient buildHttpClient = BuildArtifact.GetBuildHttpClient(context);
        using (ReleaseManagementTimer.Create(context, "Service", "BuildArtifact.GetBuildDefinition", 1971055))
          buildDefinition = buildHttpClient.GetDefinitionAsync(projectId, buildDefinitionId, new int?(), new DateTime?(), (IEnumerable<string>) null, new bool?(), (object) null, new CancellationToken()).GetResult<BuildDefinition>(context.CancellationToken);
      }
      catch (ReleaseManagementExternalServiceException ex)
      {
        if (BuildArtifact.LogGetBuildDefinitionFailure(context, ex, throwExceptionIfNotFound))
          throw;
      }
      return buildDefinition;
    }

    private static IList<DefinitionReference> SearchBuildDefinitionsPaged(
      IVssRequestContext requestContext,
      BuildHttpClient buildHttpClient,
      Guid projectId,
      string repositoryId,
      string searchText,
      ref string continuationToken)
    {
      using (ReleaseManagementTimer.Create(requestContext, "Service", "BuildArtifact.SearchBuildDefinitionsPaged", 1971013))
      {
        int result1;
        if (int.TryParse(searchText, out result1))
          return (IList<DefinitionReference>) new List<DefinitionReference>()
          {
            (DefinitionReference) buildHttpClient.GetDefinitionAsync(projectId, result1, new int?(), new DateTime?(), (IEnumerable<string>) null, new bool?(), (object) null, new CancellationToken()).GetResult<BuildDefinition>(requestContext.CancellationToken)
          };
        string str1 = searchText;
        string str2 = string.Empty;
        if (!searchText.IsNullOrEmpty<char>())
        {
          if (searchText.Contains("\\"))
          {
            string[] source = searchText.Split("\\".ToCharArray());
            str2 = string.Join("\\", ((IEnumerable<string>) source).Take<string>(source.Length - 1));
            str1 = "*" + source[source.Length - 1] + "*";
            if (!str2.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
              str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) "\\", (object) str2);
          }
          else
            str1 = "*" + searchText + "*";
        }
        int definitionsCount = BuildArtifact.GetMaxBuildDefinitionsCount(requestContext);
        BuildHttpClient buildHttpClient1 = buildHttpClient;
        Guid project = projectId;
        string str3 = repositoryId;
        DefinitionQueryOrder? nullable = new DefinitionQueryOrder?(DefinitionQueryOrder.LastModifiedDescending);
        string name = str1;
        string repositoryId1 = str3;
        DefinitionQueryOrder? queryOrder = nullable;
        string str4 = str2;
        int? top = new int?(definitionsCount);
        object obj = (object) requestContext;
        DateTime? minMetricsTimeInUtc = new DateTime?();
        string path = str4;
        DateTime? builtAfter = new DateTime?();
        DateTime? notBuiltAfter = new DateTime?();
        bool? includeLatestBuilds = new bool?();
        Guid? taskIdFilter = new Guid?();
        int? processType = new int?();
        object userState = obj;
        CancellationToken cancellationToken = new CancellationToken();
        IPagedList<BuildDefinitionReference> result2 = buildHttpClient1.GetDefinitionsAsync2(project, name, repositoryId1, queryOrder: queryOrder, top: top, minMetricsTimeInUtc: minMetricsTimeInUtc, path: path, builtAfter: builtAfter, notBuiltAfter: notBuiltAfter, includeLatestBuilds: includeLatestBuilds, taskIdFilter: taskIdFilter, processType: processType, userState: userState, cancellationToken: cancellationToken).GetResult<IPagedList<BuildDefinitionReference>>(requestContext.CancellationToken);
        continuationToken = result2.ContinuationToken;
        return (IList<DefinitionReference>) ((IEnumerable<DefinitionReference>) result2).ToList<DefinitionReference>();
      }
    }

    private static IList<DefinitionReference> GetBuildDefinitionsPaged(
      IVssRequestContext requestContext,
      BuildHttpClient buildHttpClient,
      Guid projectId,
      string repositoryId,
      int[] definitionIds,
      ref string continuationToken)
    {
      using (ReleaseManagementTimer.Create(requestContext, "Service", "BuildArtifact.GetBuildDefinitionsPaged", 1971013))
      {
        string str1 = (string) null;
        if (!repositoryId.IsNullOrEmpty<char>())
          str1 = "TfsGit";
        int definitionsCount = BuildArtifact.GetMaxBuildDefinitionsCount(requestContext);
        BuildHttpClient buildHttpClient1 = buildHttpClient;
        Guid project = projectId;
        string repositoryId1 = repositoryId;
        string repositoryType = str1;
        IEnumerable<int> ints = (IEnumerable<int>) definitionIds;
        DefinitionQueryOrder? queryOrder = new DefinitionQueryOrder?(DefinitionQueryOrder.LastModifiedDescending);
        string str2 = continuationToken;
        int? top = new int?(definitionsCount);
        string continuationToken1 = str2;
        object obj = (object) requestContext;
        DateTime? minMetricsTimeInUtc = new DateTime?();
        IEnumerable<int> definitionIds1 = ints;
        DateTime? builtAfter = new DateTime?();
        DateTime? notBuiltAfter = new DateTime?();
        bool? includeLatestBuilds = new bool?();
        Guid? taskIdFilter = new Guid?();
        int? processType = new int?();
        object userState = obj;
        CancellationToken cancellationToken = new CancellationToken();
        IPagedList<BuildDefinitionReference> result = buildHttpClient1.GetDefinitionsAsync2(project, repositoryId: repositoryId1, repositoryType: repositoryType, queryOrder: queryOrder, top: top, continuationToken: continuationToken1, minMetricsTimeInUtc: minMetricsTimeInUtc, definitionIds: definitionIds1, builtAfter: builtAfter, notBuiltAfter: notBuiltAfter, includeLatestBuilds: includeLatestBuilds, taskIdFilter: taskIdFilter, processType: processType, userState: userState, cancellationToken: cancellationToken).GetResult<IPagedList<BuildDefinitionReference>>(requestContext.CancellationToken);
        continuationToken = result.ContinuationToken;
        return (IList<DefinitionReference>) ((IEnumerable<DefinitionReference>) result).ToList<DefinitionReference>();
      }
    }

    private static IList<DefinitionReference> GetXamlBuildDefinitions(
      IVssRequestContext requestContext,
      XamlBuildHttpClient xamlBuildHttpClient,
      Guid projectId)
    {
      using (ReleaseManagementTimer.Create(requestContext, "Service", "BuildArtifact.GetXamlBuildDefinitions", 1971044))
      {
        XamlBuildHttpClient xamlBuildHttpClient1 = xamlBuildHttpClient;
        Guid project = projectId;
        object obj = (object) requestContext;
        DefinitionQueryOrder? queryOrder = new DefinitionQueryOrder?();
        int? top = new int?();
        object userState = obj;
        CancellationToken cancellationToken = new CancellationToken();
        return (IList<DefinitionReference>) xamlBuildHttpClient1.GetDefinitionsAsync(project, queryOrder: queryOrder, top: top, userState: userState, cancellationToken: cancellationToken).GetResult<List<DefinitionReference>>(requestContext.CancellationToken);
      }
    }

    private static IList<DefinitionReference> GetXamlBuildDefinitions(
      IVssRequestContext requestContext,
      XamlBuildHttpClient xamlBuildHttpClient,
      Guid projectId,
      string searchText)
    {
      using (ReleaseManagementTimer.Create(requestContext, "Service", "BuildArtifact.SearchXamlBuildDefinitions", 1971044))
      {
        if (!searchText.IsNullOrEmpty<char>())
          searchText = "*" + searchText + "*";
        XamlBuildHttpClient xamlBuildHttpClient1 = xamlBuildHttpClient;
        Guid project = projectId;
        object obj = (object) requestContext;
        string name = searchText;
        DefinitionQueryOrder? queryOrder = new DefinitionQueryOrder?();
        int? top = new int?();
        object userState = obj;
        CancellationToken cancellationToken = new CancellationToken();
        return (IList<DefinitionReference>) xamlBuildHttpClient1.GetDefinitionsAsync(project, name, queryOrder: queryOrder, top: top, userState: userState, cancellationToken: cancellationToken).GetResult<List<DefinitionReference>>(requestContext.CancellationToken);
      }
    }

    private static int GetMaxBuildDefinitionsCount(IVssRequestContext requestContext)
    {
      int num = 1000;
      int result;
      return int.TryParse(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/ReleaseManagement/Settings/getBuilds/top", num.ToString((IFormatProvider) CultureInfo.InvariantCulture)), out result) ? result : num;
    }

    private static int GetMaxWorkItemsToLinkCount(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/ReleaseManagement/Settings/MaxWorkItemsToLink", 1000);

    private static void ValidateBuild(int[] buildDefinitionIds, IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> builds)
    {
      if (!builds.Any<Microsoft.TeamFoundation.Build.WebApi.Build>())
        throw new ReleaseManagementObjectNotFoundException(buildDefinitionIds.Length != 1 ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.NoBuildForMultipleBuildDefinitions, (object) string.Join<int>(",", (IEnumerable<int>) buildDefinitionIds)) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.NoBuildForBuildDefinition, (object) ((IEnumerable<int>) buildDefinitionIds).First<int>()));
    }

    private static void TraceError(
      IVssRequestContext requestContext,
      int tracepoint,
      string traceMessageFormat,
      params object[] messageParams)
    {
      BuildArtifact.Trace(requestContext, tracepoint, TraceLevel.Error, "ReleaseManagementService", (object) "Service", (object) traceMessageFormat, (object) messageParams);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static void Trace(
      IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string traceMessageFormat,
      params object[] messageParams)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, level, "ReleaseManagementService", "Service", traceMessageFormat, messageParams);
    }

    private static Guid GetProjectId(IDictionary<string, string> dictionary) => Guid.Parse(ArtifactTypeBase.GetSourceInput(dictionary, "project"));

    private static int[] GetBuildDefinitionIds(
      IDictionary<string, string> sourceInputValues,
      bool throwOnException)
    {
      if (sourceInputValues == null)
        throw new ArgumentNullException(nameof (sourceInputValues));
      List<int> intList = new List<int>();
      if (sourceInputValues.IsMultiDefinitionType())
      {
        string str;
        if (sourceInputValues.TryGetValue("definitions", out str) && !string.IsNullOrEmpty(str))
        {
          foreach (string s in str.Split(",".ToCharArray()))
          {
            int result;
            if (int.TryParse(s, out result))
              intList.Add(result);
            else if (throwOnException)
              throw new ArgumentException("definitions");
          }
        }
        else if (throwOnException)
          throw new ArgumentException("definitions");
      }
      else
      {
        string s;
        if (sourceInputValues.TryGetValue("definition", out s) && !string.IsNullOrEmpty(s))
        {
          int result;
          if (int.TryParse(s, out result))
            intList.Add(result);
          else if (throwOnException)
            throw new ArgumentException("definition");
        }
        else if (throwOnException)
          throw new ArgumentException("definition");
      }
      return intList.ToArray();
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "Required to definition Id.")]
    private static bool HasBuildDefinitions(
      IDictionary<string, string> sourceInputValues,
      out int[] buildDefinitionIds)
    {
      if (sourceInputValues == null)
        throw new ArgumentNullException(nameof (sourceInputValues));
      buildDefinitionIds = (int[]) null;
      if (!sourceInputValues.ContainsKey("definition") && !sourceInputValues.ContainsKey("definitions"))
        return false;
      buildDefinitionIds = BuildArtifact.GetBuildDefinitionIds(sourceInputValues, false);
      return buildDefinitionIds.Length != 0;
    }

    private static IList<FileContainerItem> GetPipelineArtifactItems(
      IVssRequestContext context,
      string prefixItemPath,
      Guid projectId,
      int buildId,
      Microsoft.TeamFoundation.Build.WebApi.BuildArtifact artifact)
    {
      BuildHttpClient client = context.GetClient<BuildHttpClient>();
      Manifest manifest = Microsoft.VisualStudio.Services.Content.Common.JsonSerializer.Deserialize<Manifest>(context.RunSynchronously<Stream>((Func<Task<Stream>>) (() => client.GetFileAsync(projectId.ToString(), buildId, artifact.Name, artifact.Resource.Data, "manifest"))));
      Dictionary<string, bool> dictionary1 = new Dictionary<string, bool>();
      string[] strArray1 = prefixItemPath.Split(Path.AltDirectorySeparatorChar);
      foreach (ManifestItem manifestItem in (IEnumerable<ManifestItem>) manifest.Items)
      {
        string str1 = artifact.Name + manifestItem.Path;
        char directorySeparatorChar;
        string str2;
        if (!prefixItemPath.EndsWith(char.ToString(Path.AltDirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
        {
          string str3 = prefixItemPath;
          directorySeparatorChar = Path.AltDirectorySeparatorChar;
          string str4 = directorySeparatorChar.ToString();
          str2 = str3 + str4;
        }
        else
          str2 = prefixItemPath;
        string str5 = str2;
        if (str1.StartsWith(str5, StringComparison.CurrentCultureIgnoreCase))
        {
          string[] strArray2 = str1.Split(Path.AltDirectorySeparatorChar);
          bool flag = true;
          if (strArray1.Length < strArray2.Length)
          {
            Dictionary<string, bool> dictionary2 = dictionary1;
            string str6 = prefixItemPath;
            directorySeparatorChar = Path.AltDirectorySeparatorChar;
            string str7 = directorySeparatorChar.ToString();
            string str8 = strArray2[strArray1.Length];
            string key1 = str6 + str7 + str8;
            if (!dictionary2.ContainsKey(key1) && manifestItem.Type != ManifestItemType.EmptyDirectory)
            {
              if (strArray2.Length == strArray1.Length + 1)
                flag = false;
              Dictionary<string, bool> dictionary3 = dictionary1;
              string str9 = prefixItemPath;
              directorySeparatorChar = Path.AltDirectorySeparatorChar;
              string str10 = directorySeparatorChar.ToString();
              string str11 = strArray2[strArray1.Length];
              string key2 = str9 + str10 + str11;
              int num = flag ? 1 : 0;
              dictionary3.Add(key2, num != 0);
            }
          }
        }
      }
      List<FileContainerItem> pipelineArtifactItems = new List<FileContainerItem>();
      foreach (KeyValuePair<string, bool> keyValuePair in dictionary1)
      {
        ContainerItemType containerItemType = keyValuePair.Value ? ContainerItemType.Folder : ContainerItemType.File;
        pipelineArtifactItems.Add(new FileContainerItem()
        {
          ContainerId = 0L,
          Path = keyValuePair.Key,
          ItemType = containerItemType,
          Status = ContainerItemStatus.Created
        });
      }
      return (IList<FileContainerItem>) pipelineArtifactItems;
    }

    private IList<InputDescriptor> GetDefaultVersionInputDescriptor()
    {
      List<InputDescriptor> versionInputDescriptor = new List<InputDescriptor>()
      {
        new InputDescriptor()
        {
          Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForDefaultVersion,
          Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DescriptionForDefaultVersion,
          InputMode = InputMode.Combo,
          Id = "defaultVersionType",
          IsConfidential = false,
          Validation = new InputValidation()
          {
            IsRequired = true,
            DataType = InputDataType.String
          },
          DependencyInputIds = (IList<string>) new List<string>()
          {
            "project",
            "definition"
          },
          Properties = (IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "visibleRule",
              (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} != {1}", (object) "IsMultiDefinitionType", (object) true.ToString())
            }
          },
          HasDynamicValueInformation = true
        },
        new InputDescriptor()
        {
          Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForDefaultVersionBranch,
          Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DescriptionForDefaultVersionBranch,
          InputMode = InputMode.Combo,
          Id = "defaultVersionBranch",
          IsConfidential = false,
          Validation = new InputValidation()
          {
            IsRequired = false,
            DataType = InputDataType.String
          },
          DependencyInputIds = (IList<string>) new List<string>()
          {
            "project",
            "definition",
            "defaultVersionType"
          },
          Properties = (IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "visibleRule",
              (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} == {1} && {2} != {3}", (object) "defaultVersionType", (object) "latestWithBranchAndTagsType", (object) "IsMultiDefinitionType", (object) true.ToString())
            }
          },
          HasDynamicValueInformation = true,
          GroupName = BuildArtifact.GetDefaultVersionInputVisibilityRules("defaultVersionBranch")
        },
        new InputDescriptor()
        {
          Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForDefaultVersionTags,
          Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DescriptionForDefaultVersionTags,
          InputMode = InputMode.Combo,
          Id = "defaultVersionTags",
          IsConfidential = false,
          Validation = new InputValidation()
          {
            IsRequired = false,
            DataType = InputDataType.String
          },
          DependencyInputIds = (IList<string>) new List<string>()
          {
            "project",
            "definition",
            "defaultVersionType"
          },
          Properties = (IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "visibleRule",
              (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} == {1} || {0} == {2}", (object) "defaultVersionType", (object) "latestWithBranchAndTagsType", (object) "latestWithBuildDefinitionBranchAndTagsType")
            }
          },
          HasDynamicValueInformation = true,
          GroupName = BuildArtifact.GetDefaultVersionInputVisibilityRules("defaultVersionTags")
        },
        new InputDescriptor()
        {
          Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForDefaultVersionSpecific,
          Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DescriptionForDefaultVersionSpecific,
          InputMode = InputMode.Combo,
          Id = "defaultVersionSpecific",
          IsConfidential = false,
          Validation = new InputValidation()
          {
            IsRequired = true,
            DataType = InputDataType.String
          },
          DependencyInputIds = (IList<string>) new List<string>()
          {
            "project",
            "definition",
            "defaultVersionType"
          },
          Properties = (IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "visibleRule",
              (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} == {1} && {2} != {3}", (object) "defaultVersionType", (object) "specificVersionType", (object) "IsMultiDefinitionType", (object) true.ToString())
            }
          },
          HasDynamicValueInformation = true,
          GroupName = BuildArtifact.GetDefaultVersionInputVisibilityRules("defaultVersionSpecific")
        },
        new InputDescriptor()
        {
          Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DisplayNameForArtifacts,
          Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.DescriptionForArtifacts,
          InputMode = InputMode.None,
          Id = "artifacts",
          IsConfidential = false,
          Validation = new InputValidation()
          {
            IsRequired = false,
            DataType = InputDataType.String,
            MinLength = new int?(0),
            MaxLength = new int?(260)
          },
          DependencyInputIds = (IList<string>) new List<string>()
          {
            "IsMultiDefinitionType",
            "project",
            "definition",
            "definitions",
            "defaultVersionType",
            "defaultVersionBranch",
            "defaultVersionTags",
            "defaultVersionSpecific"
          },
          Properties = (IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "visibleRule",
              (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} != {1}", (object) "IsMultiDefinitionType", (object) true.ToString())
            }
          },
          HasDynamicValueInformation = true
        }
      };
      if (this.isCustomArtifactTypeSupported)
      {
        versionInputDescriptor.Add(new InputDescriptor()
        {
          Name = "hasCustomStorageArtifacts",
          Description = "hasCustomStorageArtifacts",
          InputMode = InputMode.CheckBox,
          Id = "hasCustomStorageArtifacts",
          IsConfidential = false,
          Validation = new InputValidation()
          {
            IsRequired = false,
            DataType = InputDataType.Boolean
          },
          DependencyInputIds = (IList<string>) new List<string>()
          {
            "IsMultiDefinitionType",
            "project",
            "definition",
            "definitions",
            "defaultVersionType",
            "defaultVersionBranch",
            "defaultVersionTags",
            "defaultVersionSpecific"
          },
          Properties = (IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "visibleRule",
              (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} == {1}", (object) "IsMultiDefinitionType", (object) "randomvalue")
            }
          },
          HasDynamicValueInformation = true
        });
        versionInputDescriptor.Add(new InputDescriptor()
        {
          Name = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ServiceName,
          Description = Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.ServiceConnectionsUsedForCustomArtifacts,
          InputMode = InputMode.Combo,
          Id = "connection",
          IsConfidential = false,
          Validation = new InputValidation()
          {
            IsRequired = false,
            DataType = InputDataType.Guid
          },
          DependencyInputIds = (IList<string>) new List<string>()
          {
            "IsMultiDefinitionType",
            "project",
            "definition",
            "definitions",
            "defaultVersionType",
            "defaultVersionBranch",
            "defaultVersionTags",
            "defaultVersionSpecific",
            "hasCustomStorageArtifacts"
          },
          Properties = (IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "visibleRule",
              (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} == {1}", (object) "hasCustomStorageArtifacts", (object) true.ToString())
            }
          },
          HasDynamicValueInformation = true
        });
      }
      return (IList<InputDescriptor>) versionInputDescriptor;
    }

    private InputValues GetHasCustomStorageArtifactsInputValues(
      IVssRequestContext context,
      IDictionary<string, string> currentInputValues,
      string inputId)
    {
      IList<Guid> enumerable = currentInputValues != null ? this.GetCustomStorageEndpointIds(context, currentInputValues, out Guid _) : throw new ArgumentNullException(nameof (currentInputValues));
      InputValues artifactsInputValues = new InputValues();
      artifactsInputValues.DefaultValue = (!enumerable.IsNullOrEmpty<Guid>()).ToString();
      artifactsInputValues.InputId = inputId;
      artifactsInputValues.IsLimitedToPossibleValues = true;
      InputValues inputValues = artifactsInputValues;
      InputValue[] inputValueArray = new InputValue[2];
      InputValue inputValue1 = new InputValue();
      bool flag1 = false;
      inputValue1.Value = flag1.ToString();
      flag1 = false;
      inputValue1.DisplayValue = flag1.ToString();
      inputValueArray[0] = inputValue1;
      InputValue inputValue2 = new InputValue();
      bool flag2 = true;
      inputValue2.Value = flag2.ToString();
      flag2 = true;
      inputValue2.DisplayValue = flag2.ToString();
      inputValueArray[1] = inputValue2;
      inputValues.PossibleValues = (IList<InputValue>) inputValueArray;
      artifactsInputValues.IsReadOnly = true;
      artifactsInputValues.IsDisabled = false;
      return artifactsInputValues;
    }

    private InputValues GetConnectionInputValues(
      IVssRequestContext context,
      IDictionary<string, string> currentInputValues,
      string inputId)
    {
      Guid projectId;
      IList<Guid> guidList = currentInputValues != null ? this.GetCustomStorageEndpointIds(context, currentInputValues, out projectId) : throw new ArgumentNullException(nameof (currentInputValues));
      string str1 = (string) null;
      Guid result;
      if (currentInputValues.ContainsKey("connection") && Guid.TryParse(currentInputValues["connection"], out result))
      {
        str1 = result.ToString();
        if (!guidList.Contains(result))
          guidList.Add(result);
      }
      List<InputValue> possibleValues = new List<InputValue>();
      if (!guidList.IsNullOrEmpty<Guid>())
      {
        IList<ServiceEndpoint> endpoints = ServiceEndpointHelper.GetServiceEndpointsByIds(context, projectId, guidList, true);
        guidList.ForEach<Guid>((Action<Guid>) (endpointId =>
        {
          string str2 = endpointId.ToString();
          string name = endpoints.SingleOrDefault<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (e => e.Id.Equals(endpointId)))?.Name;
          possibleValues.Add(new InputValue()
          {
            Value = str2,
            DisplayValue = name ?? str2
          });
        }));
      }
      return new InputValues()
      {
        DefaultValue = str1,
        InputId = inputId,
        IsLimitedToPossibleValues = true,
        PossibleValues = (IList<InputValue>) possibleValues,
        IsReadOnly = true,
        IsDisabled = false
      };
    }

    private IList<Guid> GetCustomStorageEndpointIds(
      IVssRequestContext context,
      IDictionary<string, string> currentInputValues,
      out Guid projectId)
    {
      IList<Guid> endpointIds = (IList<Guid>) new List<Guid>();
      int[] buildDefinitionIds;
      if (FirstPartyArtifactTypeBase.HasProjectInput(currentInputValues, out projectId) && BuildArtifact.HasBuildDefinitions(currentInputValues, out buildDefinitionIds))
      {
        string versionBranchFilter = currentInputValues.GetDefaultVersionBranchFilter();
        List<string> list = currentInputValues.GetDefaultVersionTagsFilter().ToList<string>();
        string artifactSourceVersionId = string.Empty;
        if (!FirstPartyArtifactTypeBase.HasArtifactSourceVersion(currentInputValues, out artifactSourceVersionId))
          currentInputValues.TryGetValue("defaultVersionSpecific", out artifactSourceVersionId);
        Microsoft.TeamFoundation.Build.WebApi.Build latestBuild = this.GetLatestBuild(context, projectId, buildDefinitionIds, artifactSourceVersionId, versionBranchFilter, (IEnumerable<string>) list);
        if (latestBuild != null)
        {
          IList<InputValue> artifacts = this.GetArtifacts(context, projectId, buildDefinitionIds, latestBuild.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture), versionBranchFilter, (IEnumerable<string>) list);
          List<string> wellKnownArtifactTypes = new List<string>()
          {
            "PipelineArtifact",
            "FilePath",
            "Container"
          };
          Func<InputValue, bool> predicate = (Func<InputValue, bool>) (artifact => artifact.Data != null && artifact.Data.ContainsKey("type") && !((IEnumerable<object>) wellKnownArtifactTypes).Contains<object>(artifact.Data["type"]));
          artifacts.Where<InputValue>(predicate).ToList<InputValue>().ForEach<InputValue>((Action<InputValue>) (artifact =>
          {
            if (artifact.Data == null || !artifact.Data.ContainsKey("connection"))
              return;
            Guid guid = new Guid(artifact.Data["connection"].ToString());
            if (endpointIds.Contains(guid))
              return;
            endpointIds.Add(guid);
          }));
        }
      }
      return endpointIds;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw exception, but want to set the error message.")]
    private InputValues GetBuildArtifacts(
      IVssRequestContext requestContext,
      Guid projectId,
      int[] buildDefinitionIds,
      string buildId,
      string branchName,
      IEnumerable<string> tagFilters,
      string inputId)
    {
      InputValuesError inputValuesError = new InputValuesError();
      IList<InputValue> inputValueList = (IList<InputValue>) new List<InputValue>();
      try
      {
        inputValueList = this.GetArtifacts(requestContext, projectId, buildDefinitionIds, buildId, branchName, tagFilters);
      }
      catch (AggregateException ex)
      {
        inputValuesError = new InputValuesError()
        {
          Message = ExceptionsUtilities.GetAllInnerExceptionsMessages(ex)
        };
      }
      catch (Exception ex)
      {
        inputValuesError = new InputValuesError()
        {
          Message = ex.Message
        };
      }
      return new InputValues()
      {
        InputId = inputId,
        PossibleValues = inputValueList,
        DefaultValue = string.Empty,
        IsLimitedToPossibleValues = true,
        IsReadOnly = true,
        Error = inputValuesError
      };
    }

    private IList<InputValue> GetArtifacts(
      IVssRequestContext requestContext,
      Guid projectId,
      int[] buildDefinitionIds,
      string buildId,
      string branchName,
      IEnumerable<string> tagFilters)
    {
      int num;
      if (!string.IsNullOrEmpty(buildId))
      {
        num = Convert.ToInt32(buildId, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      else
      {
        IList<Microsoft.TeamFoundation.Build.WebApi.Build> source = this.buildsRetriever(requestContext, projectId, buildDefinitionIds, BuildStatus.Completed, BuildResult.Succeeded | BuildResult.PartiallySucceeded, branchName, tagFilters, new int?(1));
        if (source.Count == 0)
          return (IList<InputValue>) new List<InputValue>();
        num = source.First<Microsoft.TeamFoundation.Build.WebApi.Build>().Id;
      }
      IList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact> buildArtifactList = this.artifactsRetriever(requestContext, projectId, num);
      List<InputValue> artifacts = new List<InputValue>();
      foreach (Microsoft.TeamFoundation.Build.WebApi.BuildArtifact buildArtifact in (IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>) buildArtifactList)
      {
        string name = buildArtifact.Name;
        if (string.Equals(buildArtifact.Resource.Type, "Container", StringComparison.OrdinalIgnoreCase))
          name = buildArtifact.Resource.Data.Split('/')[2];
        InputValue inputValue = new InputValue()
        {
          DisplayValue = name,
          Value = buildArtifact.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture),
          Data = (IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "type",
              (object) buildArtifact.Resource.Type
            },
            {
              "downloadUrl",
              (object) buildArtifact.Resource.DownloadUrl
            }
          }
        };
        foreach (KeyValuePair<string, string> keyValuePair in buildArtifact.Resource.Properties ?? new Dictionary<string, string>())
          inputValue.Data[keyValuePair.Key] = (object) keyValuePair.Value;
        inputValue.Data["artifactname"] = (object) buildArtifact.Name;
        artifacts.Add(inputValue);
      }
      return (IList<InputValue>) artifacts;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw exception, but want to set the error message.")]
    private InputValues GetBuildArtifactItems(
      IVssRequestContext context,
      Guid projectId,
      int[] buildDefinitionIds,
      string buildId,
      string itemPath,
      string branchName,
      IEnumerable<string> tagFilters,
      string inputId)
    {
      InputValuesError inputValuesError = new InputValuesError();
      IList<InputValue> inputValueList = (IList<InputValue>) new List<InputValue>();
      try
      {
        Microsoft.TeamFoundation.Build.WebApi.Build latestBuild = this.GetLatestBuild(context, projectId, buildDefinitionIds, buildId, branchName, tagFilters);
        inputValueList = this.GetBuildArtifactItems(context, projectId, latestBuild, itemPath);
      }
      catch (AggregateException ex)
      {
        inputValuesError = new InputValuesError()
        {
          Message = ExceptionsUtilities.GetAllInnerExceptionsMessages(ex)
        };
      }
      catch (Exception ex)
      {
        inputValuesError = new InputValuesError()
        {
          Message = ex.Message
        };
      }
      return new InputValues()
      {
        InputId = inputId,
        PossibleValues = inputValueList,
        DefaultValue = string.Empty,
        IsLimitedToPossibleValues = true,
        IsReadOnly = true,
        Error = inputValuesError
      };
    }

    private IList<InputValue> GetBuildArtifactItems(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      string itemPath)
    {
      if (build == null)
        return (IList<InputValue>) new List<InputValue>();
      BuildHttpClient client = requestContext.GetClient<BuildHttpClient>();
      string artifactName = itemPath;
      if (!artifactName.IsNullOrEmpty<char>() && artifactName.IndexOf(Path.AltDirectorySeparatorChar) > 0)
        artifactName = artifactName.Substring(0, artifactName.IndexOf(Path.AltDirectorySeparatorChar));
      Microsoft.TeamFoundation.Build.WebApi.BuildArtifact artifact = requestContext.RunSynchronously<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>((Func<Task<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>>) (() => client.GetArtifactAsync(projectId, build.Id, artifactName)));
      return (IList<InputValue>) (artifact == null || !artifact.Resource.Type.Equals("PipelineArtifact") ? (IEnumerable<FileContainerItem>) this.artifactItemsRetriever(requestContext, projectId, build.Uri, itemPath) : (IEnumerable<FileContainerItem>) BuildArtifact.GetPipelineArtifactItems(requestContext, itemPath, projectId, build.Id, artifact)).Select<FileContainerItem, InputValue>((Func<FileContainerItem, InputValue>) (item => new InputValue()
      {
        Value = item.Path,
        DisplayValue = ((IEnumerable<string>) item.Path.Split(new char[2]
        {
          '/',
          '\\'
        }, StringSplitOptions.RemoveEmptyEntries)).Last<string>(),
        Data = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "itemType",
            (object) item.ItemType.ToString()
          }
        }
      })).ToList<InputValue>();
    }

    private Microsoft.TeamFoundation.Build.WebApi.Build GetLatestBuild(
      IVssRequestContext requestContext,
      Guid projectId,
      int[] buildDefinitionIds,
      string buildId,
      string branchName,
      IEnumerable<string> tagFilters)
    {
      Microsoft.TeamFoundation.Build.WebApi.Build latestBuild;
      if (!string.IsNullOrEmpty(buildId))
      {
        int int32 = Convert.ToInt32(buildId, (IFormatProvider) CultureInfo.InvariantCulture);
        latestBuild = this.buildRetriever(requestContext, projectId, int32);
      }
      else
      {
        IList<Microsoft.TeamFoundation.Build.WebApi.Build> source = this.buildsRetriever(requestContext, projectId, buildDefinitionIds, BuildStatus.Completed, BuildResult.Succeeded | BuildResult.PartiallySucceeded, branchName, tagFilters, new int?(1));
        if (source.Count == 0)
          return (Microsoft.TeamFoundation.Build.WebApi.Build) null;
        latestBuild = source.First<Microsoft.TeamFoundation.Build.WebApi.Build>();
      }
      return latestBuild;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw exception, but want to set the error message.")]
    private InputValues GetBuildArtifactContent(
      IVssRequestContext requestContext,
      Guid projectId,
      int[] buildDefinitionIds,
      string buildId,
      string itemPath,
      string branchName,
      IEnumerable<string> tagFilters,
      string inputId)
    {
      InputValuesError inputValuesError = new InputValuesError();
      List<InputValue> inputValueList = new List<InputValue>();
      try
      {
        Microsoft.TeamFoundation.Build.WebApi.Build build = (Microsoft.TeamFoundation.Build.WebApi.Build) null;
        if (!string.IsNullOrEmpty(buildId))
        {
          int int32 = Convert.ToInt32(buildId, (IFormatProvider) CultureInfo.InvariantCulture);
          build = this.buildRetriever(requestContext, projectId, int32);
        }
        else
        {
          IList<Microsoft.TeamFoundation.Build.WebApi.Build> source = this.buildsRetriever(requestContext, projectId, buildDefinitionIds, BuildStatus.Completed, BuildResult.Succeeded | BuildResult.PartiallySucceeded, branchName, tagFilters, new int?(1));
          if (source.Count > 0)
            build = source.First<Microsoft.TeamFoundation.Build.WebApi.Build>();
        }
        if (build != null)
        {
          string str = this.artifactContentRetriever(requestContext, projectId, build.Uri, itemPath);
          InputValue inputValue = new InputValue()
          {
            Value = itemPath,
            DisplayValue = ((IEnumerable<string>) itemPath.Split(new char[1]
            {
              '/'
            }, StringSplitOptions.RemoveEmptyEntries)).Last<string>(),
            Data = (IDictionary<string, object>) new Dictionary<string, object>()
            {
              {
                "artifactItemContent",
                (object) str
              }
            }
          };
          inputValueList.Add(inputValue);
        }
      }
      catch (AggregateException ex)
      {
        inputValuesError = new InputValuesError()
        {
          Message = ExceptionsUtilities.GetAllInnerExceptionsMessages(ex)
        };
      }
      catch (Exception ex)
      {
        inputValuesError = new InputValuesError()
        {
          Message = ex.Message
        };
      }
      return new InputValues()
      {
        InputId = inputId,
        PossibleValues = (IList<InputValue>) inputValueList,
        DefaultValue = string.Empty,
        IsLimitedToPossibleValues = true,
        IsReadOnly = true,
        Error = inputValuesError
      };
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By design.")]
    private IList<InputValue> GetAvailableVersions(
      IVssRequestContext requestContext,
      IDictionary<string, string> sourceInputs,
      int? top)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      bool flag = sourceInputs != null ? sourceInputs.IsMultiDefinitionType() : throw new ArgumentNullException(nameof (sourceInputs));
      int[] buildDefinitionIds = BuildArtifact.GetBuildDefinitionIds(sourceInputs, true);
      Guid projectId = BuildArtifact.GetProjectId(sourceInputs);
      string empty = string.Empty;
      List<Microsoft.TeamFoundation.Build.WebApi.Build> buildList = new List<Microsoft.TeamFoundation.Build.WebApi.Build>();
      string defaultVersionType = sourceInputs.GetDefaultVersionType();
      string str = (string) null;
      List<string> stringList = (List<string>) null;
      if (!flag && defaultVersionType == "latestWithBuildDefinitionBranchAndTagsType")
      {
        str = Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ArtifactFilter.RemoveBranchPrefix(BuildArtifact.GetDefaultBranchForBuildDefinition(requestContext, projectId, buildDefinitionIds[0]));
        stringList = sourceInputs.GetDefaultVersionTagsFilter().ToList<string>();
      }
      else if (!flag && string.Equals(defaultVersionType, "latestWithBranchAndTagsType", StringComparison.OrdinalIgnoreCase))
      {
        str = sourceInputs.GetDefaultVersionBranchFilter();
        stringList = sourceInputs.GetDefaultVersionTagsFilter().ToList<string>();
      }
      sourceInputs.TryGetValue("defaultVersionSpecific", out empty);
      if (!string.Equals(defaultVersionType, "specificVersionType", StringComparison.OrdinalIgnoreCase))
      {
        buildList = this.buildsRetriever(requestContext, projectId, buildDefinitionIds, BuildStatus.Completed, BuildResult.Succeeded | BuildResult.PartiallySucceeded, str, (IEnumerable<string>) stringList, top).ToList<Microsoft.TeamFoundation.Build.WebApi.Build>();
      }
      else
      {
        if (empty == null)
          throw new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Resources.NoSpecificVersionValueAvailableForSpecificVersionType));
        int result;
        Microsoft.TeamFoundation.Build.WebApi.Build build = int.TryParse(empty, out result) ? this.buildRetriever(requestContext, projectId, result) : this.buildRetrieverForBuildNumber(requestContext, projectId, buildDefinitionIds, empty);
        if (build != null)
          buildList.Add(build);
      }
      BuildArtifact.ValidateBuild(buildDefinitionIds, (IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build>) buildList);
      List<InputValue> availableVersions = new List<InputValue>();
      foreach (Microsoft.TeamFoundation.Build.WebApi.Build build in buildList)
      {
        InputValue inputValue = new InputValue()
        {
          Value = build.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture),
          DisplayValue = build.BuildNumber
        };
        if (build.Definition != null)
        {
          inputValue.Data = (IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "definitionId",
              (object) build.Definition.Id
            },
            {
              "definitionName",
              (object) build.Definition.Name
            }
          };
          if (flag)
            inputValue.Data["isMultiDefinitionType"] = (object) flag;
        }
        if (!string.IsNullOrEmpty(build.SourceBranch))
        {
          inputValue.Data = inputValue.Data ?? (IDictionary<string, object>) new Dictionary<string, object>();
          inputValue.Data.Add("branch", (object) build.SourceBranch);
        }
        if (!string.IsNullOrEmpty(build.SourceVersion))
        {
          inputValue.Data = inputValue.Data ?? (IDictionary<string, object>) new Dictionary<string, object>();
          inputValue.Data.Add("sourceVersion", (object) build.SourceVersion);
        }
        if (build.Repository != null && !string.IsNullOrEmpty(build.Repository.Id))
        {
          inputValue.Data = inputValue.Data ?? (IDictionary<string, object>) new Dictionary<string, object>();
          inputValue.Data.Add("repositoryId", (object) build.Repository.Id);
        }
        if (build.Repository != null && !string.IsNullOrEmpty(build.Repository.Type))
        {
          inputValue.Data = inputValue.Data ?? (IDictionary<string, object>) new Dictionary<string, object>();
          inputValue.Data.Add("repositoryType", (object) build.Repository.Type);
        }
        if (build.Tags.Any<string>())
        {
          if (inputValue.Data == null)
            inputValue.Data = (IDictionary<string, object>) new Dictionary<string, object>();
          inputValue.Data["tags"] = (object) string.Join(",", (IEnumerable<string>) build.Tags);
        }
        availableVersions.Add(inputValue);
      }
      return (IList<InputValue>) availableVersions;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw exception, but want to set the error message.")]
    private InputValues GetBuildDefinitionsPaged(
      IVssRequestContext context,
      Guid projectId,
      string repositoryId,
      string inputId,
      string searchText,
      int[] definitionIds,
      ref string continuationToken)
    {
      IList<InputValue> inputValueList = (IList<InputValue>) new List<InputValue>();
      InputValuesError inputValuesError = (InputValuesError) null;
      try
      {
        inputValueList = this.GetBuildDefinitionsPaged(context, projectId, repositoryId, searchText, definitionIds, ref continuationToken);
      }
      catch (AggregateException ex)
      {
        inputValuesError = new InputValuesError()
        {
          Message = ExceptionsUtilities.GetAllInnerExceptionsMessages(ex)
        };
      }
      catch (Exception ex)
      {
        inputValuesError = new InputValuesError()
        {
          Message = ex.Message
        };
      }
      return new InputValues()
      {
        InputId = inputId,
        DefaultValue = string.Empty,
        PossibleValues = inputValueList,
        IsLimitedToPossibleValues = true,
        IsDisabled = false,
        IsReadOnly = false,
        Error = inputValuesError
      };
    }

    private IList<InputValue> GetBuildDefinitionsPaged(
      IVssRequestContext context,
      Guid projectId,
      string repositoryId,
      string searchText,
      int[] definitionIds,
      ref string continuationToken)
    {
      IList<DefinitionReference> definitionReferenceList = this.buildDefinitionsRetrieverPaged(context, projectId, repositoryId, searchText, definitionIds, ref continuationToken);
      List<InputValue> definitionsPaged = new List<InputValue>();
      foreach (DefinitionReference definitionReference1 in (IEnumerable<DefinitionReference>) definitionReferenceList)
      {
        if (definitionReference1 is BuildDefinitionReference definitionReference2)
        {
          DefinitionQuality? definitionQuality1 = definitionReference2.DefinitionQuality;
          if (definitionQuality1.HasValue)
          {
            definitionQuality1 = definitionReference2.DefinitionQuality;
            DefinitionQuality definitionQuality2 = DefinitionQuality.Draft;
            if (definitionQuality1.GetValueOrDefault() == definitionQuality2 & definitionQuality1.HasValue)
              continue;
          }
        }
        InputValue inputValue = new InputValue()
        {
          Value = definitionReference1.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture),
          DisplayValue = definitionReference1.Name
        };
        if (!string.IsNullOrWhiteSpace(definitionReference1.Path))
          inputValue.Data = (IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              "folderPath",
              (object) definitionReference1.Path
            }
          };
        definitionsPaged.Add(inputValue);
      }
      return (IList<InputValue>) definitionsPaged;
    }

    private InputValues GetArtifactInputValues(
      IVssRequestContext context,
      Guid projectId,
      IDictionary<string, string> currentInputValues,
      int[] buildDefinitionIds,
      string buildId,
      string inputId)
    {
      string versionBranchFilter = currentInputValues.GetDefaultVersionBranchFilter();
      List<string> list = currentInputValues.GetDefaultVersionTagsFilter().ToList<string>();
      return this.GetBuildArtifacts(context, projectId, buildDefinitionIds, buildId, versionBranchFilter, (IEnumerable<string>) list, inputId);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design")]
    private void LinkDeploymentDetailsToAzureBoardWorkItems(
      IVssRequestContext requestContext,
      IList<WorkItemRef> workItems,
      int releaseId,
      int releaseEnvironmentId,
      Microsoft.VisualStudio.Services.Identity.Identity linkAs,
      ProjectInfo currentProjectInfo)
    {
      IList<WorkItemRef> azureBoardWorkItems = this.GetUnlinkedAzureBoardWorkItems(requestContext, workItems, releaseId, releaseEnvironmentId, currentProjectInfo);
      if (azureBoardWorkItems.Count <= 0)
        return;
      requestContext.Trace(1976423, TraceLevel.Verbose, "ReleaseManagementService", "Pipeline", "AutoLinkWorkItems: Linking {0} workitems via {1} artifact.", (object) azureBoardWorkItems.Count<WorkItemRef>(), (object) "Build");
      using (IVssRequestContext userContext = requestContext.CreateUserContext(linkAs.Descriptor))
        BuildArtifact.LinkDeploymentDetailsToAzureBoardUnlinkedWorkItems(userContext, azureBoardWorkItems, releaseId, releaseEnvironmentId, currentProjectInfo);
    }

    private IList<WorkItemRef> GetUnlinkedAzureBoardWorkItems(
      IVssRequestContext requestContext,
      IList<WorkItemRef> workItems,
      int releaseId,
      int releaseEnvironmentId,
      ProjectInfo currentProjectInfo)
    {
      IEnumerable<string> hashSet = (IEnumerable<string>) this.GetLinkedWorkItemIds(requestContext, currentProjectInfo.Id, releaseId, releaseEnvironmentId).ToHashSet<string>();
      List<WorkItemRef> azureBoardWorkItems = new List<WorkItemRef>();
      if (hashSet.Count<string>() > 0)
      {
        foreach (WorkItemRef workItem in (IEnumerable<WorkItemRef>) workItems)
        {
          if (!hashSet.Contains<string>(workItem.Id, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
            azureBoardWorkItems.Add(workItem);
        }
      }
      else
        azureBoardWorkItems = workItems.ToList<WorkItemRef>();
      return (IList<WorkItemRef>) azureBoardWorkItems;
    }

    public override Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition ToAgentArtifact(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition serverArtifact)
    {
      InputValue inputValue = serverArtifact != null ? BuildArtifact.GetProjectDetails(serverArtifact) : throw new ArgumentNullException(nameof (serverArtifact));
      InputValue definitionDetails = BuildArtifact.GetBuildDefinitionDetails(serverArtifact);
      string str = JsonConvert.SerializeObject((object) new Dictionary<string, string>()
      {
        {
          "RelativePath",
          serverArtifact.Path
        },
        {
          "Project",
          inputValue.Value
        },
        {
          "DefinitionId",
          definitionDetails.Value
        },
        {
          "DefinitionName",
          definitionDetails.DisplayValue
        }
      });
      return new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition()
      {
        Name = serverArtifact.Name,
        Alias = serverArtifact.Alias,
        Version = serverArtifact.ArtifactVersion,
        ArtifactType = AgentArtifactType.Build,
        Details = str
      };
    }

    public override string GetDefaultSourceAlias(ArtifactSource artifact)
    {
      InputValue inputValue = artifact != null ? artifact.DefinitionsData : throw new ArgumentNullException(nameof (artifact));
      if (inputValue == null)
        throw new ArgumentException("SourceIdData");
      return inputValue.DisplayValue ?? string.Empty;
    }

    public override Uri GetArtifactSourceVersionUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId)
    {
      if (serverArtifact == null)
        throw new ArgumentNullException(nameof (serverArtifact));
      Uri sourceVersionUrl = (Uri) null;
      if (serverArtifact.SourceData.ContainsKey("version") && serverArtifact.SourceData.ContainsKey("project"))
      {
        string str1 = ArtifactTypeBase.GetUriFromRequestContext(requestContext).ToString();
        string str2 = ArtifactTypeBase.GetCollectionId(requestContext).ToString("D");
        string str3 = serverArtifact.SourceData["version"].Value;
        sourceVersionUrl = new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_permalink/_build/index?collectionId={1}&projectId={2}&buildId={3}", (object) str1, (object) str2, (object) serverArtifact.SourceData["project"].Value, (object) str3), UriKind.Absolute);
      }
      return sourceVersionUrl;
    }

    public override Uri GetArtifactSourceDefinitionUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId)
    {
      if (serverArtifact == null)
        throw new ArgumentNullException(nameof (serverArtifact));
      Uri sourceDefinitionUrl = (Uri) null;
      if (serverArtifact.SourceData.ContainsKey("definition") && serverArtifact.SourceData.ContainsKey("project"))
      {
        string str1 = ArtifactTypeBase.GetUriFromRequestContext(requestContext).ToString();
        string str2 = ArtifactTypeBase.GetCollectionId(requestContext).ToString("D");
        string str3 = serverArtifact.SourceData["definition"].Value;
        sourceDefinitionUrl = new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_permalink/_build/index?collectionId={1}&projectId={2}&definitionId={3}", (object) str1, (object) str2, (object) serverArtifact.SourceData["project"].Value, (object) str3), UriKind.Absolute);
      }
      return sourceDefinitionUrl;
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "To be used for testing")]
    public delegate IList<DefinitionReference> BuildDefinitionsRetrieverPaged(
      IVssRequestContext context,
      Guid projectId,
      string repositoryId,
      string searchText,
      int[] definitionIds,
      ref string continuationToken);
  }
}
