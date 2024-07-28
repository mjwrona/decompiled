// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.DeployPhaseConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class DeployPhaseConverter
  {
    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase ToServerDeployPhase(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase,
      ReleaseDefinitionEnvironment webApiEnvironment)
    {
      if (webApiEnvironment == null)
        throw new ArgumentNullException(nameof (webApiEnvironment));
      if (deployPhase == null)
        throw new ArgumentNullException(nameof (deployPhase));
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase()
      {
        DefinitionEnvironmentId = webApiEnvironment.Id,
        Rank = deployPhase.Rank,
        Name = deployPhase.Name,
        RefName = deployPhase.RefName,
        PhaseType = (DeployPhaseTypes) deployPhase.PhaseType,
        Workflow = deployPhase.WorkflowTasks.ToJsonString(),
        DeploymentInput = deployPhase.GetDeploymentInput()?.ToJObject()
      };
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase ToWebApiDeployPhase(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase serverDeployPhase)
    {
      if (serverDeployPhase == null)
        throw new ArgumentNullException(nameof (serverDeployPhase));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase webApiDeployPhase;
      switch (serverDeployPhase.PhaseType)
      {
        case DeployPhaseTypes.AgentBasedDeployment:
          webApiDeployPhase = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase) new AgentBasedDeployPhase()
          {
            DeploymentInput = (AgentDeploymentInput) serverDeployPhase.GetDeploymentInput()
          };
          break;
        case DeployPhaseTypes.RunOnServer:
          webApiDeployPhase = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase) new RunOnServerDeployPhase()
          {
            DeploymentInput = (ServerDeploymentInput) serverDeployPhase.GetDeploymentInput()
          };
          break;
        case DeployPhaseTypes.MachineGroupBasedDeployment:
          webApiDeployPhase = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase) new MachineGroupBasedDeployPhase()
          {
            DeploymentInput = (MachineGroupDeploymentInput) serverDeployPhase.GetDeploymentInput()
          };
          break;
        case DeployPhaseTypes.DeploymentGates:
          webApiDeployPhase = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase) new GatesDeployPhase()
          {
            DeploymentInput = (GatesDeploymentInput) serverDeployPhase.GetDeploymentInput()
          };
          break;
        default:
          return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase) null;
      }
      webApiDeployPhase.Name = serverDeployPhase.Name;
      webApiDeployPhase.RefName = serverDeployPhase.RefName;
      webApiDeployPhase.Rank = serverDeployPhase.Rank;
      webApiDeployPhase.WorkflowTasks = (IList<WorkflowTask>) serverDeployPhase.Workflow.ToWorkflowTaskList();
      return webApiDeployPhase;
    }
  }
}
