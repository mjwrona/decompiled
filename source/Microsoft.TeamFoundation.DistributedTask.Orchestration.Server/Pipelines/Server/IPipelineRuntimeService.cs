// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.IPipelineRuntimeService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  [DefaultServiceImplementation(typeof (PipelineRuntimeService))]
  public interface IPipelineRuntimeService : IVssFrameworkService
  {
    Task<IList<StageAttempt>> RetryPipelineAsync(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeIdentifier,
      Guid planId);

    Task<IList<StageAttempt>> RetryStagesAsync(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeIdentifier,
      Guid planId,
      IList<string> stageNames,
      bool forceRetryAllJobs = false,
      bool retryDependencies = true);

    Task<StageAttempt> GetStageAttemptAsync(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeIdentifier,
      Guid planId,
      string name,
      int attempt);

    Task<IList<StageAttempt>> GetAllStageAttemptsAsync(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeId,
      Guid planId,
      string name,
      IList<string> includedPhases = null);

    Task<IList<StageAttempt>> CreateStageAttempts(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeIdentifier,
      TaskOrchestrationPlan plan,
      IList<string> stageNames,
      Guid requestedBy,
      bool forceRetryAllJobs = false,
      bool retryDependencies = true);
  }
}
