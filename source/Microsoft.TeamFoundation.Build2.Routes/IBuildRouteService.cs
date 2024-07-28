// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Routes.IBuildRouteService
// Assembly: Microsoft.TeamFoundation.Build2.Routes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3759BAC-2581-46BE-B787-E219FAA96ED4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Routes.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Routes
{
  [DefaultServiceImplementation(typeof (BuildRouteService))]
  public interface IBuildRouteService : IVssFrameworkService
  {
    string GetBuildWebUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      bool useLegacyUrl = false);

    string GetBuildRestUrl(IVssRequestContext requestContext, Guid projectId, int buildId);

    string GetBuildAttachmentRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      Guid timelineId,
      Guid timelineRecordId,
      string type,
      string name);

    string GetBuildLogsRestUrl(IVssRequestContext requestContext, Guid projectId, int buildId);

    string GetBuildLogRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      int logId);

    string GetBuildSourcesRestUrl(IVssRequestContext requestContext, Guid projectId, int buildId);

    string GetDefinitionWebUrl(IVssRequestContext requestContext, Guid projectId, int definitionId);

    string GetDefinitionDesignerUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId);

    string GetNewDefinitionDesignerUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      string selectedRepositoryType,
      string connectionId = null,
      IEnumerable<DefinitionTriggerType> triggers = null,
      bool useNewDesigner = false,
      string requestSource = null,
      string repositoryId = null,
      string telemetrySession = null);

    string GetEditPipelineDesignerUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      string pipelineId,
      string nonce = null);

    string GetDefinitionRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int? definitionRevision = null);

    string GetArtifactWebUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      string type,
      string data);

    string GetArtifactRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      string artifactName,
      ApiResourceVersion resourceVersion);

    string GetQueueRestUrl(IVssRequestContext requestContext, int queueId);

    string GetTimelineRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      Guid? timelineId = null);

    string GetBranchBadgeRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      string repoType,
      string branchName,
      string repoId);

    string GetTfvcItemRestUrl(IVssRequestContext requestContext, string path);

    string GetFileContainerItemRestUrl(
      IVssRequestContext requestContext,
      string container,
      string itemPath);

    string GetStatusBadgeUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      string branchName = null);
  }
}
