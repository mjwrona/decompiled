// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IEnvironmentTelemetryLogger
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public interface IEnvironmentTelemetryLogger
  {
    void PublishDeploymentPhaseStarted(
      IVssRequestContext requestContext,
      ProviderPhaseRequest phaseRequest);

    void PublishJobCompleted(
      IVssRequestContext requestContext,
      string phaseOrchestrationId,
      JobInstance job);

    void PublishAddEnvironment(
      IVssRequestContext requestContext,
      EnvironmentInstance environment,
      string source);

    void PublishJobCommitterDetails(
      IVssRequestContext requestContext,
      Guid scopeId,
      int runId,
      int definitionId,
      string jobRequestId,
      string planType,
      string stageName,
      string phaseName,
      string planTemplateType,
      int targetEnvironmentId);
  }
}
