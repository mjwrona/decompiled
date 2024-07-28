// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent33
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseSqlComponent33 : ReleaseSqlComponent32
  {
    protected override void BindReleaseEnvironments(
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      this.BindReleaseEnvironmentTable12(nameof (releaseEnvironments), releaseEnvironments);
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Will be overridden in derived classes.")]
    protected virtual DeploymentGateBinder GetDeploymentGateBinder() => new DeploymentGateBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Too heavy to be a property")]
    protected override Release GetReleaseObject(Guid projectId)
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseBinder(projectId));
        resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
        resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
        resultCollection.AddBinder<Deployment>((ObjectBinder<Deployment>) this.GetDeploymentBinder());
        resultCollection.AddBinder<DeploymentGate>((ObjectBinder<DeploymentGate>) this.GetDeploymentGateBinder());
        resultCollection.AddBinder<ManualIntervention>((ObjectBinder<ManualIntervention>) this.GetManualInterventionListBinder());
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
        resultCollection.AddBinder<ReleaseTagData>((ObjectBinder<ReleaseTagData>) this.GetReleaseTagsBinder());
        Release release = resultCollection.GetCurrent<Release>().FirstOrDefault<Release>();
        resultCollection.NextResult();
        List<ReleaseEnvironment> items1 = resultCollection.GetCurrent<ReleaseEnvironment>().Items;
        resultCollection.NextResult();
        List<ReleaseDeployPhase> items2 = resultCollection.GetCurrent<ReleaseDeployPhase>().Items;
        resultCollection.NextResult();
        List<Deployment> items3 = resultCollection.GetCurrent<Deployment>().Items;
        resultCollection.NextResult();
        List<DeploymentGate> items4 = resultCollection.GetCurrent<DeploymentGate>().Items;
        resultCollection.NextResult();
        List<ManualIntervention> items5 = resultCollection.GetCurrent<ManualIntervention>().Items;
        resultCollection.NextResult();
        List<ReleaseEnvironmentStep> items6 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
        resultCollection.NextResult();
        List<PipelineArtifactSource> items7 = resultCollection.GetCurrent<PipelineArtifactSource>().Items;
        resultCollection.NextResult();
        List<ReleaseTagData> items8 = resultCollection.GetCurrent<ReleaseTagData>().Items;
        if (release != null)
          ReleaseSqlComponent15.StitchReleaseObject(release, (IEnumerable<ReleaseEnvironment>) items1, (IEnumerable<ReleaseEnvironmentStep>) items6, (IEnumerable<PipelineArtifactSource>) items7, (IEnumerable<ReleaseDeployPhase>) items2, (IEnumerable<Deployment>) items3, (IEnumerable<ManualIntervention>) items5, (IEnumerable<ReleaseTagData>) items8, (IEnumerable<DeploymentGate>) items4);
        return release;
      }
    }

    public override ReleaseEnvironmentData GetReleaseEnvironmentData(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      bool includeDeployments,
      bool includeApprovals,
      bool includeArtifacts)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseEnvironment", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindBoolean(nameof (includeDeployments), includeDeployments);
      this.BindBoolean(nameof (includeApprovals), includeApprovals);
      this.BindBoolean(nameof (includeArtifacts), includeArtifacts);
      ReleaseEnvironmentData releaseEnvironmentData = new ReleaseEnvironmentData();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ShallowReference>((ObjectBinder<ShallowReference>) this.GetShallowReferenceBinder());
        ShallowReference shallowReference1 = resultCollection.GetCurrent<ShallowReference>().Items.Single<ShallowReference>();
        releaseEnvironmentData.ReleaseId = shallowReference1.Id;
        releaseEnvironmentData.ReleaseName = shallowReference1.Name;
        resultCollection.AddBinder<ShallowReference>((ObjectBinder<ShallowReference>) this.GetShallowReferenceBinder());
        resultCollection.NextResult();
        ShallowReference shallowReference2 = resultCollection.GetCurrent<ShallowReference>().Items.Single<ShallowReference>();
        releaseEnvironmentData.ReleaseDefinitionId = shallowReference2.Id;
        releaseEnvironmentData.ReleaseDefinitionName = shallowReference2.Name;
        resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
        resultCollection.NextResult();
        ReleaseEnvironment releaseEnvironment = resultCollection.GetCurrent<ReleaseEnvironment>().Items.SingleOrDefault<ReleaseEnvironment>();
        if (releaseEnvironment == null)
          return releaseEnvironmentData;
        if (includeDeployments)
        {
          resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
          resultCollection.NextResult();
          List<ReleaseDeployPhase> items1 = resultCollection.GetCurrent<ReleaseDeployPhase>().Items;
          resultCollection.AddBinder<Deployment>((ObjectBinder<Deployment>) this.GetDeploymentBinder());
          resultCollection.NextResult();
          List<Deployment> items2 = resultCollection.GetCurrent<Deployment>().Items;
          resultCollection.AddBinder<DeploymentGate>((ObjectBinder<DeploymentGate>) this.GetDeploymentGateBinder());
          resultCollection.NextResult();
          List<DeploymentGate> items3 = resultCollection.GetCurrent<DeploymentGate>().Items;
          releaseEnvironment.ReleaseDeployPhases.AddRange<ReleaseDeployPhase, IList<ReleaseDeployPhase>>((IEnumerable<ReleaseDeployPhase>) items1);
          releaseEnvironment.DeploymentAttempts.AddRange<Deployment, IList<Deployment>>((IEnumerable<Deployment>) items2);
          foreach (DeploymentGate deploymentGate in items3)
          {
            DeploymentGate gate = deploymentGate;
            items2.SingleOrDefault<Deployment>((System.Func<Deployment, bool>) (d => d.Id == gate.DeploymentId))?.DeploymentGates.Add(gate);
          }
        }
        if (includeApprovals)
        {
          resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
          resultCollection.NextResult();
          List<ReleaseEnvironmentStep> items = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
          releaseEnvironment.GetStepsForTests.AddRange<ReleaseEnvironmentStep, IList<ReleaseEnvironmentStep>>((IEnumerable<ReleaseEnvironmentStep>) items);
        }
        if (includeArtifacts)
        {
          resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
          resultCollection.NextResult();
          foreach (ArtifactSource artifact in ReleaseSqlComponent15.GetArtifacts((IEnumerable<PipelineArtifactSource>) resultCollection.GetCurrent<PipelineArtifactSource>().Items))
            releaseEnvironmentData.LinkedArtifacts.Add(artifact);
        }
        releaseEnvironmentData.Environment = releaseEnvironment;
        return releaseEnvironmentData;
      }
    }

    public override ReleaseLogContainers GetReleaseLogContainers(
      Guid projectId,
      int releaseId,
      bool skipIsDeletedCheck)
    {
      List<ReleaseDeployPhaseRef> source1 = new List<ReleaseDeployPhaseRef>();
      List<DeploymentGateRef> source2 = new List<DeploymentGateRef>();
      this.PrepareStoredProcedure("Release.prc_GetReleaseRunPlanIdRefs", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindBoolean(nameof (skipIsDeletedCheck), skipIsDeletedCheck);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDeployPhaseRef>((ObjectBinder<ReleaseDeployPhaseRef>) this.GetReleaseRunPlanIdRefBinder());
        source1 = resultCollection.GetCurrent<ReleaseDeployPhaseRef>().Items;
        resultCollection.NextResult();
        resultCollection.AddBinder<DeploymentGateRef>((ObjectBinder<DeploymentGateRef>) new DeploymentGateRefBinder());
        source2 = resultCollection.GetCurrent<DeploymentGateRef>().Items;
      }
      return new ReleaseLogContainers()
      {
        DeployPhases = (IList<ReleaseDeployPhaseRef>) source1.Where<ReleaseDeployPhaseRef>((System.Func<ReleaseDeployPhaseRef, bool>) (e => e.PlanId != Guid.Empty)).ToList<ReleaseDeployPhaseRef>(),
        Gates = (IList<DeploymentGateRef>) source2.Where<DeploymentGateRef>((System.Func<DeploymentGateRef, bool>) (g => g.RunPlanId.HasValue && g.RunPlanId.Value != Guid.Empty)).ToList<DeploymentGateRef>()
      };
    }
  }
}
