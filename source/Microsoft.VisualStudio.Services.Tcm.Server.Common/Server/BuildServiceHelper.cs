// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class BuildServiceHelper : IBuildServiceHelper
  {
    private const int c_BuildsToCompareCount = 1;

    public virtual Microsoft.TeamFoundation.Build.WebApi.Build QueryBuildByUri(
      IVssRequestContext context,
      Guid projectId,
      string uri,
      bool includeBuildDefinitionDetails)
    {
      context.TraceInfo("BusinessLayer", "BuildServiceHelper.QueryBuildByUri started - {0}", (object) uri);
      try
      {
        int buildArtifactId = this.GetBuildArtifactId(uri);
        return this.QueryBuildById(context, projectId, buildArtifactId, includeBuildDefinitionDetails);
      }
      catch (Exception ex)
      {
        context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (Microsoft.TeamFoundation.Build.WebApi.Build) null;
    }

    public virtual Microsoft.TeamFoundation.Build.WebApi.Build QueryBuildById(
      IVssRequestContext context,
      Guid projectId,
      int buildId,
      bool includeBuildDefinitionDetails)
    {
      context.TraceInfo("BusinessLayer", "BuildServiceHelper.QueryBuildById started - {0}", (object) buildId);
      try
      {
        Microsoft.TeamFoundation.Build.WebApi.Build build = this.QueryvNextBuild(context, projectId, buildId) ?? this.QueryXamlBuild(context, projectId, buildId);
        if (build != null & includeBuildDefinitionDetails)
          build.Definition.Uri = this.GetBuildDefinitionUriFromId(context, projectId, build.Definition.Id);
        return build;
      }
      catch (Exception ex)
      {
        context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (Microsoft.TeamFoundation.Build.WebApi.Build) null;
    }

    public virtual BuildConfiguration QueryBuildConfigurationByBuildNumber(
      IVssRequestContext context,
      Guid projectId,
      string buildNumber)
    {
      try
      {
        Microsoft.TeamFoundation.Build.WebApi.Build build = context.GetClient<BuildHttpClient>(context.GetHttpClientOptionsForEventualReadConsistencyLevel("TestManagement.Server.GetBuild.SqlReadReplica")).GetBuildsAsync(projectId, (IEnumerable<int>) null, (IEnumerable<int>) null, buildNumber, new DateTime?(), new DateTime?(), (string) null, new BuildReason?(), new BuildStatus?(), new BuildResult?(), (IEnumerable<string>) null, (IEnumerable<string>) null, new int?(), (string) null, new int?(), new QueryDeletedOption?(), new BuildQueryOrder?(), (string) null, (IEnumerable<int>) null, (string) null, (string) null, (object) null, new CancellationToken()).SyncResult<List<Microsoft.TeamFoundation.Build.WebApi.Build>>().FirstOrDefault<Microsoft.TeamFoundation.Build.WebApi.Build>();
        return this.GetBuildRefFromBuild(context, build);
      }
      catch (Exception ex)
      {
        context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (BuildConfiguration) null;
    }

    public virtual BuildConfiguration QueryBuildConfigurationByBuildUri(
      IVssRequestContext context,
      Guid projectId,
      string uri)
    {
      try
      {
        int buildArtifactId = this.GetBuildArtifactId(uri);
        return this.QueryBuildConfigurationById(context, projectId, buildArtifactId);
      }
      catch (Exception ex)
      {
        context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (BuildConfiguration) null;
    }

    public virtual BuildConfiguration QueryBuildConfigurationById(
      IVssRequestContext context,
      Guid projectId,
      int buildId)
    {
      Microsoft.TeamFoundation.Build.WebApi.Build build = this.QueryvNextBuild(context, projectId, buildId) ?? this.QueryXamlBuild(context, projectId, buildId);
      return this.GetBuildRefFromBuild(context, build);
    }

    public BuildConfiguration QueryLastSuccessfulBuild(
      IVssRequestContext context,
      Guid projectId,
      BuildConfiguration currentBuild,
      DateTime maxFinishTimeForBuild)
    {
      try
      {
        BuildResult buildResult = BuildResult.Succeeded | BuildResult.PartiallySucceeded | BuildResult.Failed;
        return this.GetBuildWithResultFilter(context, projectId, currentBuild, maxFinishTimeForBuild, buildResult);
      }
      catch (Exception ex)
      {
        context.Trace(1015021, TraceLevel.Warning, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (BuildConfiguration) null;
    }

    public BuildConfiguration QueryLastCompleteSuccessfulBuild(
      IVssRequestContext context,
      Guid projectId,
      BuildConfiguration currentBuild,
      DateTime maxFinishTimeForBuild)
    {
      try
      {
        return this.GetBuildWithResultFilter(context, projectId, currentBuild, maxFinishTimeForBuild, BuildResult.Succeeded);
      }
      catch (Exception ex)
      {
        context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (BuildConfiguration) null;
    }

    public IList<BuildConfiguration> QueryBuildsByUris(
      IVssRequestContext context,
      Guid projectId,
      List<string> buildUris)
    {
      try
      {
        List<int> list1 = buildUris.Select<string, int>((Func<string, int>) (b => this.GetBuildArtifactId(b))).ToList<int>();
        List<BuildConfiguration> list2 = this.QueryvNextBuildByIds(context, projectId, list1).Select<Microsoft.TeamFoundation.Build.WebApi.Build, BuildConfiguration>((Func<Microsoft.TeamFoundation.Build.WebApi.Build, BuildConfiguration>) (b => this.GetBuildRefFromBuild(context, b))).ToList<BuildConfiguration>();
        if (list2 != null)
        {
          HashSet<int> intSet = new HashSet<int>(list2.Select<BuildConfiguration, int>((Func<BuildConfiguration, int>) (b => b.BuildId)));
          foreach (int buildId in list1)
          {
            if (!intSet.Contains(buildId))
            {
              Microsoft.TeamFoundation.Build.WebApi.Build build = this.QueryXamlBuild(context, projectId, buildId);
              if (build != null)
                list2.Add(this.GetBuildRefFromBuild(context, build));
            }
          }
        }
        return (IList<BuildConfiguration>) list2;
      }
      catch (Exception ex)
      {
        context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (IList<BuildConfiguration>) null;
    }

    public BuildDefinition GetBuildDefinition(
      IVssRequestContext context,
      Guid projectId,
      int definitionId)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetBuildDefinition), "Build")))
        return this.QueryvNextBuildDefinitionById(context, projectId, definitionId);
    }

    public int GetBuildDefinitionIdFromName(
      IVssRequestContext context,
      Guid projectId,
      string definitionName)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetBuildDefinitionIdFromName), "Build")))
      {
        int definitionIdFromName = 0;
        BuildDefinitionReference definitionReference1 = this.QueryvNextBuildDefinitionByName(context, projectId, definitionName);
        if (definitionReference1 != null)
        {
          definitionIdFromName = definitionReference1.Id;
        }
        else
        {
          DefinitionReference definitionReference2 = this.QueryXamlBuildDefinitionByName(context, projectId, definitionName);
          if (definitionReference2 != null)
            definitionIdFromName = definitionReference2.Id;
        }
        return definitionIdFromName;
      }
    }

    public string GetBuildDefinitionNameFromId(
      IVssRequestContext context,
      Guid projectId,
      int definitionId)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetBuildDefinitionNameFromId), "Build")))
      {
        string definitionNameFromId = (string) null;
        BuildDefinition buildDefinition = this.QueryvNextBuildDefinitionById(context, projectId, definitionId);
        if (buildDefinition == null)
        {
          DefinitionReference definitionReference = this.QueryXamlBuildDefinitionById(context, projectId, definitionId);
          if (definitionReference != null)
            definitionNameFromId = definitionReference.Name;
        }
        else
          definitionNameFromId = buildDefinition.Name;
        return definitionNameFromId;
      }
    }

    public List<BuildDefinitionReference> QueryBuildDefinitionsByIds(
      IVssRequestContext context,
      Guid projectId,
      int[] definitionIds)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (QueryBuildDefinitionsByIds), "Build")))
        return this.QueryBuildDefinitionsByIdsUtil(context, projectId, definitionIds);
    }

    public Uri GetBuildDefinitionUriFromId(
      IVssRequestContext context,
      Guid projectId,
      int definitionId)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetBuildDefinitionUriFromId), "Build")))
      {
        Uri definitionUriFromId = (Uri) null;
        BuildDefinition buildDefinition = this.QueryvNextBuildDefinitionById(context, projectId, definitionId);
        if (buildDefinition == null)
        {
          DefinitionReference definitionReference = this.QueryXamlBuildDefinitionById(context, projectId, definitionId);
          if (definitionReference != null)
            definitionUriFromId = definitionReference.Uri;
        }
        else
          definitionUriFromId = buildDefinition.Uri;
        return definitionUriFromId;
      }
    }

    public int GetBuildArtifactId(string buildUri)
    {
      ArtifactId artifactId = LinkingUtilities.DecodeUri(buildUri);
      int result = 0;
      int.TryParse(artifactId.ToolSpecificId, out result);
      return result;
    }

    public BuildSettings GetBuildSettings(IVssRequestContext context)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetBuildSettings), "Build")))
      {
        try
        {
          return context.GetClient<BuildHttpClient>().GetBuildSettingsAsync().SyncResult<BuildSettings>();
        }
        catch (Exception ex)
        {
          context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
        }
        return (BuildSettings) null;
      }
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference GetBuildRepresentation(
      IVssRequestContext requestContext,
      BuildConfiguration buildRef)
    {
      if (buildRef == null || buildRef.BuildId <= 0)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
      {
        Id = buildRef.BuildId.ToString((IFormatProvider) CultureInfo.InvariantCulture),
        Name = buildRef.BuildNumber,
        Url = UrlBuildHelper.GetResourceUrl(requestContext, ServiceInstanceTypes.TFS, "build", BuildResourceIds.Builds, (object) new
        {
          buildId = buildRef.BuildId
        })
      };
    }

    protected virtual DefinitionReference QueryXamlBuildDefinitionByName(
      IVssRequestContext context,
      Guid projectId,
      string buildDefinitionName)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName("QueryXamlBuildDefinitions", "Build")))
      {
        try
        {
          List<DefinitionReference> source = context.GetClient<XamlBuildHttpClient>().GetDefinitionsAsync(projectId, buildDefinitionName).SyncResult<List<DefinitionReference>>();
          return source != null ? source.FirstOrDefault<DefinitionReference>() : (DefinitionReference) null;
        }
        catch (Exception ex)
        {
          context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
        }
        return (DefinitionReference) null;
      }
    }

    protected virtual List<BuildDefinitionReference> QueryBuildDefinitionsByIdsUtil(
      IVssRequestContext context,
      Guid projectId,
      int[] buildDefinitionIds)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (QueryBuildDefinitionsByIdsUtil), "Build")))
      {
        try
        {
          BuildHttpClient client = context.GetClient<BuildHttpClient>();
          Guid project = projectId;
          IEnumerable<int> ints = (IEnumerable<int>) buildDefinitionIds;
          DefinitionQueryOrder? queryOrder = new DefinitionQueryOrder?();
          int? top = new int?();
          DateTime? minMetricsTimeInUtc = new DateTime?();
          IEnumerable<int> definitionIds = ints;
          DateTime? builtAfter = new DateTime?();
          DateTime? notBuiltAfter = new DateTime?();
          bool? includeLatestBuilds = new bool?();
          Guid? taskIdFilter = new Guid?();
          int? processType = new int?();
          CancellationToken cancellationToken = new CancellationToken();
          return client.GetDefinitionsAsync(project, queryOrder: queryOrder, top: top, minMetricsTimeInUtc: minMetricsTimeInUtc, definitionIds: definitionIds, builtAfter: builtAfter, notBuiltAfter: notBuiltAfter, includeLatestBuilds: includeLatestBuilds, taskIdFilter: taskIdFilter, processType: processType, cancellationToken: cancellationToken).SyncResult<List<BuildDefinitionReference>>();
        }
        catch (Exception ex)
        {
          context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
        }
        return (List<BuildDefinitionReference>) null;
      }
    }

    protected virtual DefinitionReference QueryXamlBuildDefinitionById(
      IVssRequestContext context,
      Guid projectId,
      int buildDefinitionId)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName("QueryXamlBuildDefinition", "Build")))
      {
        try
        {
          return context.GetClient<XamlBuildHttpClient>().GetDefinitionAsync(projectId, buildDefinitionId).SyncResult<DefinitionReference>();
        }
        catch (BuildNotFoundException ex)
        {
          context.Trace(1015022, TraceLevel.Info, "TestManagement", "BusinessLayer", ex.ToString());
        }
        catch (Exception ex)
        {
          context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
        }
        return (DefinitionReference) null;
      }
    }

    protected virtual Microsoft.TeamFoundation.Build.WebApi.Build QueryXamlBuild(
      IVssRequestContext context,
      Guid projectId,
      int buildId)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (QueryXamlBuild), "Build")))
      {
        try
        {
          return context.Elevate().GetClient<XamlBuildHttpClient>(context.GetHttpClientOptionsForEventualReadConsistencyLevel("TestManagement.Server.GetBuild.SqlReadReplica")).GetBuildAsync(projectId, buildId).SyncResult<Microsoft.TeamFoundation.Build.WebApi.Build>();
        }
        catch (BuildNotFoundException ex)
        {
          context.Trace(1015022, TraceLevel.Info, "TestManagement", "BusinessLayer", ex.ToString());
        }
        catch (Exception ex)
        {
          context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
        }
        return (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      }
    }

    protected virtual List<Microsoft.TeamFoundation.Build.WebApi.Build> QueryXamlBuilds(
      IVssRequestContext context,
      Guid projectId,
      int buildDefinitionId,
      DateTime maxFinishTimeForBuild,
      BuildResult buildResult)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (QueryXamlBuilds), "Build")))
      {
        try
        {
          XamlBuildHttpClient client = context.Elevate().GetClient<XamlBuildHttpClient>(context.GetHttpClientOptionsForEventualReadConsistencyLevel("TestManagement.Server.GetBuild.SqlReadReplica"));
          Guid project = projectId;
          int? nullable1 = new int?(1);
          List<int> definitions = new List<int>();
          definitions.Add(buildDefinitionId);
          DateTime? nullable2 = new DateTime?(maxFinishTimeForBuild);
          BuildStatus? nullable3 = new BuildStatus?(BuildStatus.Completed);
          BuildResult? nullable4 = new BuildResult?(buildResult);
          BuildQueryOrder? nullable5 = new BuildQueryOrder?(BuildQueryOrder.FinishTimeDescending);
          QueryDeletedOption? nullable6 = new QueryDeletedOption?(QueryDeletedOption.ExcludeDeleted);
          DateTime? minFinishTime = new DateTime?();
          DateTime? maxFinishTime = nullable2;
          BuildReason? reasonFilter = new BuildReason?();
          BuildStatus? statusFilter = nullable3;
          BuildResult? resultFilter = nullable4;
          int? top = nullable1;
          int? maxBuildsPerDefinition = new int?();
          QueryDeletedOption? deletedFilter = nullable6;
          BuildQueryOrder? queryOrder = nullable5;
          CancellationToken cancellationToken = new CancellationToken();
          return client.GetBuildsAsync(project, (IEnumerable<int>) definitions, minFinishTime: minFinishTime, maxFinishTime: maxFinishTime, reasonFilter: reasonFilter, statusFilter: statusFilter, resultFilter: resultFilter, top: top, maxBuildsPerDefinition: maxBuildsPerDefinition, deletedFilter: deletedFilter, queryOrder: queryOrder, cancellationToken: cancellationToken).SyncResult<List<Microsoft.TeamFoundation.Build.WebApi.Build>>();
        }
        catch (Exception ex)
        {
          context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
        }
        return (List<Microsoft.TeamFoundation.Build.WebApi.Build>) null;
      }
    }

    internal BuildConfiguration GetBuildRefFromBuild(IVssRequestContext requestContext, Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
      if (build == null)
        return (BuildConfiguration) null;
      BuildConfiguration buildRefFromBuild = new BuildConfiguration();
      buildRefFromBuild.BuildId = build.Id;
      buildRefFromBuild.BuildUri = build.Uri.AbsoluteUri;
      buildRefFromBuild.BuildNumber = build.BuildNumber;
      buildRefFromBuild.BuildDefinitionId = build.Definition.Id;
      buildRefFromBuild.RepositoryId = build.Repository != null ? build.Repository.Id : string.Empty;
      buildRefFromBuild.RepositoryType = build.Repository != null ? build.Repository.Type : string.Empty;
      buildRefFromBuild.BranchName = build.SourceBranch;
      buildRefFromBuild.SourceVersion = build.SourceVersion;
      buildRefFromBuild.BuildFlavor = string.Empty;
      buildRefFromBuild.BuildPlatform = string.Empty;
      DateTime? nullable;
      DateTime utcNow1;
      if (!build.StartTime.HasValue)
      {
        utcNow1 = DateTime.UtcNow;
      }
      else
      {
        nullable = build.StartTime;
        utcNow1 = nullable.Value;
      }
      buildRefFromBuild.CreatedDate = utcNow1;
      nullable = build.FinishTime;
      DateTime utcNow2;
      if (!nullable.HasValue)
      {
        utcNow2 = DateTime.UtcNow;
      }
      else
      {
        nullable = build.FinishTime;
        utcNow2 = nullable.Value;
      }
      buildRefFromBuild.CompletedDate = utcNow2;
      buildRefFromBuild.BuildSystem = requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? BuildConstants.TFSOnPremiseBuildSystem : BuildConstants.VSOBuildSystem;
      buildRefFromBuild.BuildDefinitionName = build.Definition.Name;
      return buildRefFromBuild;
    }

    public bool IsPullRequestBuild(
      IVssRequestContext requestContext,
      Guid projectId,
      string buildUri)
    {
      BuildConfiguration buildConfiguration = new BuildServiceHelper().QueryBuildConfigurationByBuildUri(requestContext, projectId, buildUri);
      return buildConfiguration != null && GitHelper.IsPullRequest(buildConfiguration.BranchName);
    }

    private BuildConfiguration GetBuildWithResultFilter(
      IVssRequestContext context,
      Guid projectId,
      BuildConfiguration currentBuild,
      DateTime maxFinishTimeForBuild,
      BuildResult buildResult)
    {
      Microsoft.TeamFoundation.Build.WebApi.Build build = this.QueryvNextBuilds(context, projectId, currentBuild, maxFinishTimeForBuild, buildResult).FirstOrDefault<Microsoft.TeamFoundation.Build.WebApi.Build>();
      if (build == null)
      {
        List<Microsoft.TeamFoundation.Build.WebApi.Build> source = this.QueryXamlBuilds(context, projectId, currentBuild.BuildDefinitionId, maxFinishTimeForBuild, buildResult);
        build = source != null ? source.FirstOrDefault<Microsoft.TeamFoundation.Build.WebApi.Build>() : (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      }
      return this.GetBuildRefFromBuild(context, build);
    }

    private Microsoft.TeamFoundation.Build.WebApi.Build QueryvNextBuild(
      IVssRequestContext context,
      Guid projectId,
      int buildId)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (QueryvNextBuild), "Build")))
      {
        try
        {
          return context.GetClient<BuildHttpClient>(context.GetHttpClientOptionsForEventualReadConsistencyLevel("TestManagement.Server.GetBuild.SqlReadReplica")).GetBuildAsync(projectId, buildId).SyncResult<Microsoft.TeamFoundation.Build.WebApi.Build>();
        }
        catch (BuildNotFoundException ex)
        {
          context.Trace(1015022, TraceLevel.Info, "TestManagement", "BusinessLayer", ex.ToString());
        }
        catch (Exception ex)
        {
          context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
        }
        return (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      }
    }

    private List<Microsoft.TeamFoundation.Build.WebApi.Build> QueryvNextBuildByIds(
      IVssRequestContext context,
      Guid projectId,
      List<int> buildIds)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (QueryvNextBuildByIds), "Build")))
      {
        try
        {
          BuildHttpClient client = context.GetClient<BuildHttpClient>(context.GetHttpClientOptionsForEventualReadConsistencyLevel("TestManagement.Server.GetBuild.SqlReadReplica"));
          Guid project = projectId;
          IEnumerable<int> ints = (IEnumerable<int>) buildIds;
          DateTime? minTime = new DateTime?();
          DateTime? maxTime = new DateTime?();
          BuildReason? reasonFilter = new BuildReason?();
          BuildStatus? statusFilter = new BuildStatus?();
          BuildResult? resultFilter = new BuildResult?();
          int? top = new int?();
          int? maxBuildsPerDefinition = new int?();
          QueryDeletedOption? deletedFilter = new QueryDeletedOption?();
          BuildQueryOrder? queryOrder = new BuildQueryOrder?();
          IEnumerable<int> buildIds1 = ints;
          CancellationToken cancellationToken = new CancellationToken();
          return client.GetBuildsAsync(project, (IEnumerable<int>) null, (IEnumerable<int>) null, (string) null, minTime, maxTime, (string) null, reasonFilter, statusFilter, resultFilter, (IEnumerable<string>) null, (IEnumerable<string>) null, top, (string) null, maxBuildsPerDefinition, deletedFilter, queryOrder, (string) null, buildIds1, (string) null, (string) null, (object) null, cancellationToken).SyncResult<List<Microsoft.TeamFoundation.Build.WebApi.Build>>();
        }
        catch (Exception ex)
        {
          context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
        }
        return (List<Microsoft.TeamFoundation.Build.WebApi.Build>) null;
      }
    }

    private List<Microsoft.TeamFoundation.Build.WebApi.Build> QueryvNextBuilds(
      IVssRequestContext context,
      Guid projectId,
      BuildConfiguration currentBuild,
      DateTime maxFinishTimeForBuild,
      BuildResult buildResult)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (QueryvNextBuilds), "Build")))
      {
        try
        {
          BuildHttpClient client = context.GetClient<BuildHttpClient>(context.GetHttpClientOptionsForEventualReadConsistencyLevel("TestManagement.Server.GetBuild.SqlReadReplica"));
          Guid project = projectId;
          List<int> definitions = new List<int>();
          definitions.Add(currentBuild.BuildDefinitionId);
          DateTime? minTime = new DateTime?();
          DateTime? maxTime = new DateTime?(maxFinishTimeForBuild);
          BuildReason? reasonFilter = new BuildReason?();
          BuildStatus? statusFilter = new BuildStatus?(BuildStatus.Completed);
          BuildResult? resultFilter = new BuildResult?(buildResult);
          int? top = new int?(1);
          int? maxBuildsPerDefinition = new int?();
          QueryDeletedOption? deletedFilter = new QueryDeletedOption?(QueryDeletedOption.ExcludeDeleted);
          BuildQueryOrder? queryOrder = new BuildQueryOrder?(BuildQueryOrder.FinishTimeDescending);
          string branchName = !string.IsNullOrEmpty(currentBuild.BranchName) ? currentBuild.BranchName : (string) null;
          string repositoryId = !string.IsNullOrEmpty(currentBuild.RepositoryId) ? currentBuild.RepositoryId : (string) null;
          string repositoryType = !string.IsNullOrEmpty(currentBuild.RepositoryType) ? currentBuild.RepositoryType : (string) null;
          CancellationToken cancellationToken = new CancellationToken();
          return client.GetBuildsAsync(project, (IEnumerable<int>) definitions, (IEnumerable<int>) null, (string) null, minTime, maxTime, (string) null, reasonFilter, statusFilter, resultFilter, (IEnumerable<string>) null, (IEnumerable<string>) null, top, (string) null, maxBuildsPerDefinition, deletedFilter, queryOrder, branchName, (IEnumerable<int>) null, repositoryId, repositoryType, (object) null, cancellationToken).SyncResult<List<Microsoft.TeamFoundation.Build.WebApi.Build>>();
        }
        catch (Exception ex)
        {
          context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
        }
        return (List<Microsoft.TeamFoundation.Build.WebApi.Build>) null;
      }
    }

    private BuildDefinitionReference QueryvNextBuildDefinitionByName(
      IVssRequestContext context,
      Guid projectId,
      string buildDefinitionName)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (QueryvNextBuildDefinitionByName), "Build")))
      {
        try
        {
          List<BuildDefinitionReference> source = context.GetClient<BuildHttpClient>().GetDefinitionsAsync(projectId, buildDefinitionName, (string) null, (string) null, new DefinitionQueryOrder?(), new int?(), (string) null, new DateTime?(), (IEnumerable<int>) null, (string) null, new DateTime?(), new DateTime?(), new bool?(), new Guid?(), new int?(), (string) null, (object) null, new CancellationToken()).SyncResult<List<BuildDefinitionReference>>();
          return source != null ? source.FirstOrDefault<BuildDefinitionReference>() : (BuildDefinitionReference) null;
        }
        catch (BuildNotFoundException ex)
        {
          context.Trace(1015022, TraceLevel.Info, "TestManagement", "BusinessLayer", ex.ToString());
        }
        catch (Exception ex)
        {
          context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
        }
        return (BuildDefinitionReference) null;
      }
    }

    private BuildDefinition QueryvNextBuildDefinitionById(
      IVssRequestContext context,
      Guid projectId,
      int buildDefinitionId)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (QueryvNextBuildDefinitionById), "Build")))
      {
        try
        {
          return context.GetClient<BuildHttpClient>().GetDefinitionAsync(projectId, buildDefinitionId, new int?(), new DateTime?(), (IEnumerable<string>) null, new bool?(), (object) null, new CancellationToken()).SyncResult<BuildDefinition>();
        }
        catch (DefinitionNotFoundException ex)
        {
          context.Trace(1015022, TraceLevel.Info, "TestManagement", "BusinessLayer", ex.ToString());
        }
        catch (Exception ex)
        {
          context.Trace(1015021, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
        }
        return (BuildDefinition) null;
      }
    }
  }
}
