// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.Migration.IExternalRunsService
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Server.Migration
{
  [DefaultServiceImplementation(typeof (ExternalRunsService))]
  public interface IExternalRunsService : IVssFrameworkService
  {
    Run GetRun(IVssRequestContext requestContext, Guid projectId, int pipelineId, int runId);

    IReadOnlyList<Run> GetRuns(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int count);

    Run RunPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int? pipelineVersion,
      RunPipelineParameters parameters);

    IReadOnlyList<Artifact> GetArtifacts(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId,
      string artifactName);
  }
}
