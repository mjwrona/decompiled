// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Routes.IPipelinesRouteService
// Assembly: Microsoft.TeamFoundation.Build2.Routes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3759BAC-2581-46BE-B787-E219FAA96ED4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Routes.dll

using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Pipelines.Routes
{
  [DefaultServiceImplementation(typeof (PipelinesRouteService))]
  public interface IPipelinesRouteService : IVssFrameworkService
  {
    string GetArtifactRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId,
      string artifactName,
      GetArtifactExpandOptions? expandOptions = null);

    string GetLogCollectionRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId,
      GetLogExpandOptions? expandOptions = null);

    string GetLogRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId,
      int logId,
      GetLogExpandOptions? expandOptions = null);

    string GetPipelineRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int? pipelineRevision);

    string GetPipelineWebUrl(IVssRequestContext requestContext, Guid projectId, int pipelineId);

    string GetRunsRestUrl(IVssRequestContext requestContext, Guid projectId, int pipelineId);

    string GetRunRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId);

    string GetRunWebUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId);

    string GetSignedLogContentRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId,
      int logId);

    string GetSignedLogsContentRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId);

    string GetSignalRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId);

    string GetSignedSignalRWebsocketRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId);
  }
}
