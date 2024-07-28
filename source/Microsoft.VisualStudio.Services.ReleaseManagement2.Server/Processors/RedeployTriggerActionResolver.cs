// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.RedeployTriggerActionResolver
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  public static class RedeployTriggerActionResolver
  {
    public static RedeployTriggerAction ResolveActionAsPerBaseDeployment(
      DeploymentStatus baseDeploymentStatus,
      DeploymentOperationStatus baseDeploymentOperationStatus)
    {
      switch (baseDeploymentStatus)
      {
        case DeploymentStatus.Undefined:
        case DeploymentStatus.NotDeployed:
        case DeploymentStatus.Failed:
          return RedeployTriggerAction.None;
        case DeploymentStatus.InProgress:
          if (baseDeploymentOperationStatus <= DeploymentOperationStatus.PhaseSucceeded)
          {
            if (baseDeploymentOperationStatus <= DeploymentOperationStatus.QueuedForAgent)
            {
              if (baseDeploymentOperationStatus != DeploymentOperationStatus.Pending && baseDeploymentOperationStatus != DeploymentOperationStatus.QueuedForAgent)
                goto label_11;
            }
            else if (baseDeploymentOperationStatus != DeploymentOperationStatus.PhaseInProgress && baseDeploymentOperationStatus != DeploymentOperationStatus.PhaseSucceeded)
              goto label_11;
          }
          else if (baseDeploymentOperationStatus <= DeploymentOperationStatus.ManualInterventionPending)
          {
            if (baseDeploymentOperationStatus != DeploymentOperationStatus.PhasePartiallySucceeded && baseDeploymentOperationStatus != DeploymentOperationStatus.ManualInterventionPending)
              goto label_11;
          }
          else if (baseDeploymentOperationStatus != DeploymentOperationStatus.QueuedForPipeline && baseDeploymentOperationStatus != DeploymentOperationStatus.EvaluatingGates && baseDeploymentOperationStatus != DeploymentOperationStatus.GateFailed)
            goto label_11;
          return RedeployTriggerAction.CreateDeployment;
label_11:
          return RedeployTriggerAction.None;
        case DeploymentStatus.Succeeded:
        case DeploymentStatus.PartiallySucceeded:
          return RedeployTriggerAction.CreateDeployment;
        default:
          return RedeployTriggerAction.None;
      }
    }

    public static RedeployTriggerAction ResolveActionAsPerLatestDeployment(
      DeploymentReason reason,
      DeploymentStatus latestDeploymentStatus,
      DeploymentOperationStatus latestDeploymentOperationStatus)
    {
      if (reason == DeploymentReason.RedeployTrigger)
        return latestDeploymentStatus == DeploymentStatus.InProgress ? RedeployTriggerAction.PushInQueue : RedeployTriggerActionResolver.ResolveActionAsPerLatestDeploymentOperationStatus(latestDeploymentOperationStatus, true);
      switch (latestDeploymentStatus)
      {
        case DeploymentStatus.Undefined:
        case DeploymentStatus.NotDeployed:
        case DeploymentStatus.Failed:
          return RedeployTriggerAction.None;
        case DeploymentStatus.InProgress:
          return RedeployTriggerActionResolver.ResolveActionAsPerLatestDeploymentOperationStatus(latestDeploymentOperationStatus);
        case DeploymentStatus.Succeeded:
        case DeploymentStatus.PartiallySucceeded:
          return RedeployTriggerAction.CreateDeployment;
        default:
          return RedeployTriggerAction.None;
      }
    }

    public static RedeployTriggerAction ResolveActionAsPerLatestDeploymentOperationStatus(
      DeploymentOperationStatus latestDeploymentOperationStatus,
      bool isLastDeploymentWasRetrigger = false)
    {
      if (latestDeploymentOperationStatus <= DeploymentOperationStatus.PhaseSucceeded)
      {
        if (latestDeploymentOperationStatus <= DeploymentOperationStatus.QueuedForAgent)
        {
          if (latestDeploymentOperationStatus != DeploymentOperationStatus.Pending && latestDeploymentOperationStatus != DeploymentOperationStatus.QueuedForAgent)
            goto label_8;
        }
        else if (latestDeploymentOperationStatus != DeploymentOperationStatus.PhaseInProgress && latestDeploymentOperationStatus != DeploymentOperationStatus.PhaseSucceeded)
          goto label_8;
      }
      else if (latestDeploymentOperationStatus <= DeploymentOperationStatus.ManualInterventionPending)
      {
        if (latestDeploymentOperationStatus != DeploymentOperationStatus.PhasePartiallySucceeded && latestDeploymentOperationStatus != DeploymentOperationStatus.ManualInterventionPending)
          goto label_8;
      }
      else if (latestDeploymentOperationStatus != DeploymentOperationStatus.QueuedForPipeline && latestDeploymentOperationStatus != DeploymentOperationStatus.EvaluatingGates && latestDeploymentOperationStatus != DeploymentOperationStatus.GateFailed)
        goto label_8;
      return RedeployTriggerAction.PushInQueue;
label_8:
      return !isLastDeploymentWasRetrigger ? RedeployTriggerAction.None : RedeployTriggerAction.CreateDeployment;
    }
  }
}
