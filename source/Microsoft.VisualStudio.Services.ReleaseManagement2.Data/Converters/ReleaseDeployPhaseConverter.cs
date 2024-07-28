// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ReleaseDeployPhaseConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ReleaseDeployPhaseConverter
  {
    public static DeployPhaseSnapshot ToServerDeployPhaseSnapshot(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhaseSnapshot)
    {
      if (deployPhaseSnapshot == null)
        throw new ArgumentNullException(nameof (deployPhaseSnapshot));
      DeployPhaseSnapshot deployPhaseSnapshot1 = new DeployPhaseSnapshot();
      deployPhaseSnapshot1.Rank = deployPhaseSnapshot.Rank;
      deployPhaseSnapshot1.Name = deployPhaseSnapshot.Name;
      deployPhaseSnapshot1.RefName = deployPhaseSnapshot.RefName;
      deployPhaseSnapshot1.PhaseType = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes) deployPhaseSnapshot.PhaseType;
      deployPhaseSnapshot1.Workflow = deployPhaseSnapshot.WorkflowTasks == null ? (IList<WorkflowTask>) new List<WorkflowTask>() : (IList<WorkflowTask>) new List<WorkflowTask>((IEnumerable<WorkflowTask>) deployPhaseSnapshot.WorkflowTasks);
      BaseDeploymentInput deploymentInput = deployPhaseSnapshot.GetDeploymentInput();
      deployPhaseSnapshot1.DeploymentInput = deploymentInput == null ? (JObject) null : deploymentInput.ToJObject();
      return deployPhaseSnapshot1;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase ToWebApiDeployPhaseSnapshot(
      this DeployPhaseSnapshot serverDeployPhaseSnapshot)
    {
      if (serverDeployPhaseSnapshot == null)
        throw new ArgumentNullException(nameof (serverDeployPhaseSnapshot));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhaseSnapshot;
      switch (serverDeployPhaseSnapshot.PhaseType)
      {
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.AgentBasedDeployment:
          deployPhaseSnapshot = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase) new AgentBasedDeployPhase()
          {
            DeploymentInput = (AgentDeploymentInput) serverDeployPhaseSnapshot.GetDeploymentInput()
          };
          break;
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.RunOnServer:
          deployPhaseSnapshot = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase) new RunOnServerDeployPhase()
          {
            DeploymentInput = (ServerDeploymentInput) serverDeployPhaseSnapshot.GetDeploymentInput()
          };
          break;
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.MachineGroupBasedDeployment:
          deployPhaseSnapshot = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase) new MachineGroupBasedDeployPhase()
          {
            DeploymentInput = (MachineGroupDeploymentInput) serverDeployPhaseSnapshot.GetDeploymentInput()
          };
          break;
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.DeploymentGates:
          deployPhaseSnapshot = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase) new GatesDeployPhase()
          {
            DeploymentInput = (GatesDeploymentInput) serverDeployPhaseSnapshot.GetDeploymentInput()
          };
          break;
        default:
          return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase) null;
      }
      deployPhaseSnapshot.Name = serverDeployPhaseSnapshot.Name;
      deployPhaseSnapshot.RefName = serverDeployPhaseSnapshot.RefName;
      deployPhaseSnapshot.Rank = serverDeployPhaseSnapshot.Rank;
      deployPhaseSnapshot.WorkflowTasks = serverDeployPhaseSnapshot.Workflow == null ? (IList<WorkflowTask>) new List<WorkflowTask>() : (IList<WorkflowTask>) new List<WorkflowTask>((IEnumerable<WorkflowTask>) serverDeployPhaseSnapshot.Workflow);
      return deployPhaseSnapshot;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase ToWebApiReleaseDeployPhase(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase serverReleaseDeployPhase,
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention> manualInterventions,
      IVssRequestContext requestContext,
      Guid projectId,
      string phaseName,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment serverDeployment)
    {
      if (serverReleaseDeployPhase == null)
        throw new ArgumentNullException(nameof (serverReleaseDeployPhase));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase releaseDeployPhase;
      switch (serverReleaseDeployPhase.PhaseType)
      {
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.RunOnServer:
          releaseDeployPhase = ReleaseDeployPhaseConverter.GetServerDeployPhase(requestContext, manualInterventions, projectId);
          break;
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.DeploymentGates:
          releaseDeployPhase = ReleaseDeployPhaseConverter.GetGatesDeployPhase(serverDeployment, serverReleaseDeployPhase.RunPlanId);
          break;
        default:
          releaseDeployPhase = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase();
          break;
      }
      releaseDeployPhase.Id = serverReleaseDeployPhase.Id;
      releaseDeployPhase.PhaseId = serverReleaseDeployPhase.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      releaseDeployPhase.Name = string.Copy(phaseName);
      releaseDeployPhase.Rank = serverReleaseDeployPhase.Rank;
      releaseDeployPhase.PhaseType = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes) serverReleaseDeployPhase.PhaseType;
      releaseDeployPhase.Status = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseStatus) serverReleaseDeployPhase.Status;
      releaseDeployPhase.ErrorLog = serverReleaseDeployPhase.Logs != null ? string.Copy(serverReleaseDeployPhase.Logs) : (string) null;
      releaseDeployPhase.StartedOn = serverReleaseDeployPhase.DeploymentStartTime;
      return releaseDeployPhase;
    }

    private static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase GetServerDeployPhase(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention> manualInterventions,
      Guid projectId)
    {
      return new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase()
      {
        ManualInterventions = (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention>) manualInterventions.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ManualIntervention, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention>) (mi => mi.ToWebApi(requestContext, projectId))).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ManualIntervention>()
      };
    }

    private static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase GetGatesDeployPhase(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment serverDeployment,
      Guid? phaseRunPlanId)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseGatesPhase webApiGatesPhase = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseGatesPhase();
      if (phaseRunPlanId.HasValue)
      {
        DeploymentGate deploymentGate;
        if (serverDeployment == null)
        {
          deploymentGate = (DeploymentGate) null;
        }
        else
        {
          IList<DeploymentGate> deploymentGates = serverDeployment.DeploymentGates;
          deploymentGate = deploymentGates != null ? deploymentGates.SingleOrDefault<DeploymentGate>((Func<DeploymentGate, bool>) (dg =>
          {
            if (dg.GateType != EnvironmentStepType.Deploy)
              return false;
            Guid? runPlanId = dg.RunPlanId;
            Guid guid = phaseRunPlanId.Value;
            if (!runPlanId.HasValue)
              return false;
            return !runPlanId.HasValue || runPlanId.GetValueOrDefault() == guid;
          })) : (DeploymentGate) null;
        }
        DeploymentGate serverDeploymentGate = deploymentGate;
        if (serverDeploymentGate != null)
          webApiGatesPhase.FillReleaseGatesPhaseWithDeploymentGate(serverDeploymentGate);
      }
      return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase) webApiGatesPhase;
    }
  }
}
