// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Xaml.IXamlBuildProvider
// Assembly: Microsoft.TeamFoundation.Build2.Xaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 48A241AC-D20F-49E0-A581-C219E1ED7760
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Xaml.dll

using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.TeamFoundation.Build2.Xaml
{
  [DefaultServiceImplementation(typeof (XamlBuildProvider))]
  public interface IXamlBuildProvider : IVssFrameworkService
  {
    Microsoft.TeamFoundation.Build.WebApi.Build GetBuild(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      bool includeDeleted = false);

    IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> GetBuilds(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int count,
      IList<int> definitionIds = null,
      IList<int> queueIds = null,
      string buildNumber = "*",
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
      string requestedFor = null,
      Microsoft.TeamFoundation.Build.WebApi.BuildReason reasonFilter = Microsoft.TeamFoundation.Build.WebApi.BuildReason.All,
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder queryOrder = Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending,
      Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption queryDeletedOption = Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption.ExcludeDeleted,
      int? maxBuildsPerDefinition = null);

    Microsoft.TeamFoundation.Build.WebApi.Build QueueBuild(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      string checkInTicket);

    Microsoft.TeamFoundation.Build.WebApi.Build UpdateBuild(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Microsoft.TeamFoundation.Build.WebApi.Build build);

    void DeleteBuild(IVssRequestContext requestContext, ProjectInfo projectInfo, int buildId);

    void DeleteBuildsForDefinition(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      XamlBuildDefinition definition);

    IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> GetCompletedBuilds(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      IList<int> definitionIds,
      IList<int> queueIds,
      string buildNumber,
      DateTime? minFinishTime,
      DateTime? maxFinishTime,
      string requestedFor,
      Microsoft.TeamFoundation.Build.Server.BuildReason xamlBuildReason,
      Microsoft.TeamFoundation.Build.Server.BuildStatus xamlBuildStatus,
      Microsoft.TeamFoundation.Build.Server.BuildQueryOrder xamlQueryOrder,
      Microsoft.TeamFoundation.Build.Server.QueryDeletedOption queryDeletedOption,
      int? maxBuildsPerDefinition,
      int count);

    IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> GetRequestedBuilds(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int count,
      IList<int> definitionIds,
      IList<int> queueIds,
      DateTime? maxFinishTime,
      string requestedFor,
      QueueStatus xamlQueueStatus,
      Microsoft.TeamFoundation.Build.Server.BuildReason xamlBuildReason);

    IEnumerable<BuildArtifact> GetArtifacts(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      ApiResourceVersion resourceVersion,
      string artifactName = null);

    IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Change> GetBuildChanges(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      int top,
      int maxMessageLength);

    IEnumerable<Issue> GetBuildIssues(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      int top);

    IEnumerable<ResourceRef> GetBuildWorkItemRefs(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      int top);

    IEnumerable<Deployment> GetBuildDeployments(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId);

    IEnumerable<BuildLog> GetBuildLogsMetadata(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId);

    PushStreamContent GetBuildLogsZip(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId);

    StreamContent GetBuildLog(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      int logId);
  }
}
