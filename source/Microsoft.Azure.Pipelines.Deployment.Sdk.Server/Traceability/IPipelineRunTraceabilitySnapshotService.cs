// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.IPipelineRunTraceabilitySnapshotService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2CF55160-AB9F-45A3-BD33-54D24F269988
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Sdk.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability
{
  [DefaultServiceImplementation("Microsoft.Azure.Pipelines.Deployment.Services.PipelineRunTraceabilitySnapshotService, Microsoft.Azure.Pipelines.Deployment.Server")]
  [InheritedExport]
  public interface IPipelineRunTraceabilitySnapshotService : IVssFrameworkService
  {
    void SaveBaseRunTraceabilityDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineRunTraceabilitySnapshotObject snapshotObject);

    PipelineRunTraceabilitySnapshotObject GetRunTraceabilitySnapshot(
      IVssRequestContext requestContext,
      Guid projectId,
      int currentRunId);
  }
}
