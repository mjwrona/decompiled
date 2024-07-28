// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.PipelineRunTraceabilitySnapshotService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.DataAccess;
using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability;
using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  public sealed class PipelineRunTraceabilitySnapshotService : 
    IPipelineRunTraceabilitySnapshotService,
    IVssFrameworkService
  {
    public void SaveBaseRunTraceabilityDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineRunTraceabilitySnapshotObject snapshotObject)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<PipelineRunTraceabilitySnapshotObject>(snapshotObject, nameof (snapshotObject));
      using (PipelineRunTraceabilitySnapshotComponent component = requestContext.CreateComponent<PipelineRunTraceabilitySnapshotComponent>())
        component.SaveBaseRunDetails(requestContext, projectId, snapshotObject);
    }

    public PipelineRunTraceabilitySnapshotObject GetRunTraceabilitySnapshot(
      IVssRequestContext requestContext,
      Guid projectId,
      int currentRunId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNonPositiveInt(currentRunId, nameof (currentRunId));
      using (PipelineRunTraceabilitySnapshotComponent component = requestContext.CreateComponent<PipelineRunTraceabilitySnapshotComponent>())
        return component.GetRunTraceabilitySnapshot(requestContext, projectId, currentRunId);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
