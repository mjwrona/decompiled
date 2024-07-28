// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.DeploymentOperationStatusConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class DeploymentOperationStatusConverter
  {
    private static readonly IDictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus> ServerToWebApiStatusMap = (IDictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus>) new Dictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus>()
    {
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Undefined,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Undefined
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Queued,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Queued
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Scheduled,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Scheduled
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Pending,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Pending
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Approved,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Approved
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Rejected,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Rejected
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Deferred,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Deferred
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.QueuedForAgent,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.QueuedForAgent
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.PhaseInProgress,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.PhaseInProgress
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.PhaseSucceeded,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.PhaseSucceeded
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.PhasePartiallySucceeded,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.PhasePartiallySucceeded
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.PhaseFailed,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.PhaseFailed
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Canceled,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Canceled
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.PhaseCanceled,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.PhaseCanceled
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.ManualInterventionPending,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.ManualInterventionPending
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.QueuedForPipeline,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.QueuedForPipeline
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Cancelling,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Cancelling
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.EvaluatingGates,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.EvaluatingGates
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.GateFailed,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.GateFailed
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.All,
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.All
      }
    };

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus ToWebApi(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus status)
    {
      return DeploymentOperationStatusConverter.ServerToWebApiStatusMap[status];
    }
  }
}
