// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent39
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent39 : ReleaseSqlComponent38
  {
    private const string UnknownEnvironmentName = "UnknownEnvironment";
    private const int UnknownEnvironmentRank = 1;

    public override ReleaseDeployPhase GetReleaseDeployPhase(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int releaseDeployPhaseId,
      Guid? planId)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseDeployPhase", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (releaseDeployPhaseId), releaseDeployPhaseId);
      this.BindNullableGuid("runPlanId", planId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
        return resultCollection.GetCurrent<ReleaseDeployPhase>().Items.FirstOrDefault<ReleaseDeployPhase>();
      }
    }

    public override ReleaseLogContainers GetReleaseLogContainers(
      Guid projectId,
      int releaseId,
      bool skipIsDeletedCheck)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseRunPlanIdRefs", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindBoolean(nameof (skipIsDeletedCheck), skipIsDeletedCheck);
      return this.FetchReleaseLogContainers();
    }

    protected ReleaseLogContainers FetchReleaseLogContainers()
    {
      IDictionary<int, ReleaseEnvironment> dictionary;
      List<ReleaseDeployPhase> items1;
      List<DeploymentGateRef> items2;
      List<ReleaseEnvironmentStep> items3;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
        dictionary = (IDictionary<int, ReleaseEnvironment>) resultCollection.GetCurrent<ReleaseEnvironment>().Items.ToDictionary<ReleaseEnvironment, int, ReleaseEnvironment>((System.Func<ReleaseEnvironment, int>) (e => e.Id), (System.Func<ReleaseEnvironment, ReleaseEnvironment>) (e => e));
        resultCollection.NextResult();
        resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
        items1 = resultCollection.GetCurrent<ReleaseDeployPhase>().Items;
        resultCollection.NextResult();
        resultCollection.AddBinder<DeploymentGateRef>((ObjectBinder<DeploymentGateRef>) new DeploymentGateRefBinder());
        items2 = resultCollection.GetCurrent<DeploymentGateRef>().Items;
        resultCollection.NextResult();
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        items3 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
      }
      ReleaseLogContainers releaseLogContainers = new ReleaseLogContainers();
      ReleaseSqlComponent39.PopulateDeployPhaseRefs(items1, dictionary, releaseLogContainers);
      ReleaseSqlComponent39.PopulateSteps(items3, dictionary, releaseLogContainers);
      ReleaseSqlComponent39.PopulateGates(items2, dictionary, releaseLogContainers);
      return releaseLogContainers;
    }

    private static void PopulateGates(
      List<DeploymentGateRef> gates,
      IDictionary<int, ReleaseEnvironment> environments,
      ReleaseLogContainers releaseLogContainers)
    {
      foreach (DeploymentGateRef deploymentGateRef in gates.Where<DeploymentGateRef>((System.Func<DeploymentGateRef, bool>) (g => g.RunPlanId.HasValue)))
      {
        ReleaseEnvironment releaseEnvironment;
        string str;
        int num;
        if (environments.TryGetValue(deploymentGateRef.ReleaseEnvironmentId, out releaseEnvironment))
        {
          str = releaseEnvironment.Name;
          num = releaseEnvironment.Rank;
        }
        else
        {
          str = "UnknownEnvironment";
          num = 1;
        }
        deploymentGateRef.EnvironmentName = str;
        deploymentGateRef.EnvironmentRank = num;
        releaseLogContainers.Gates.Add(deploymentGateRef);
      }
    }

    private static void PopulateSteps(
      List<ReleaseEnvironmentStep> deploySteps,
      IDictionary<int, ReleaseEnvironment> environments,
      ReleaseLogContainers releaseLogContainers)
    {
      foreach (ReleaseEnvironmentStep deployStep in deploySteps)
      {
        ReleaseEnvironment releaseEnvironment;
        string str = !environments.TryGetValue(deployStep.ReleaseEnvironmentId, out releaseEnvironment) ? "UnknownEnvironment" : releaseEnvironment.Name;
        deployStep.ReleaseEnvironmentName = str;
        releaseLogContainers.DeploySteps.Add(deployStep);
      }
    }

    private static void PopulateDeployPhaseRefs(
      List<ReleaseDeployPhase> deployPhases,
      IDictionary<int, ReleaseEnvironment> environments,
      ReleaseLogContainers releaseLogContainers)
    {
      foreach (ReleaseDeployPhase releaseDeployPhase in deployPhases.Where<ReleaseDeployPhase>((System.Func<ReleaseDeployPhase, bool>) (p => p.RunPlanId.HasValue)))
      {
        ReleaseEnvironment releaseEnvironment;
        string str;
        int num;
        IDeploymentSnapshot deploymentSnapshot1;
        if (environments.TryGetValue(releaseDeployPhase.ReleaseEnvironmentId, out releaseEnvironment))
        {
          str = releaseEnvironment.Name;
          num = releaseEnvironment.Rank;
          deploymentSnapshot1 = releaseEnvironment.DeploymentSnapshot;
        }
        else
        {
          str = "UnknownEnvironment";
          num = 1;
          deploymentSnapshot1 = (IDeploymentSnapshot) null;
        }
        ReleaseDeployPhaseRef releaseDeployPhaseRef = new ReleaseDeployPhaseRef()
        {
          PlanId = releaseDeployPhase.RunPlanId.Value,
          Attempt = releaseDeployPhase.Attempt,
          EnvironmentName = str,
          EnvironmentRank = num,
          PhaseType = releaseDeployPhase.PhaseType,
          PhaseRank = releaseDeployPhase.Rank,
          PhaseId = releaseDeployPhase.Id,
          PhaseName = ServerModelUtility.GetDefaultPhaseName(releaseDeployPhase.PhaseType)
        };
        if (deploymentSnapshot1 is DesignerDeploymentSnapshot deploymentSnapshot2)
          releaseDeployPhaseRef.PhaseName = deploymentSnapshot2.GetPhaseName(releaseDeployPhase.Rank, releaseDeployPhase.PhaseType);
        releaseLogContainers.DeployPhases.Add(releaseDeployPhaseRef);
      }
    }
  }
}
