// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent36
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent36 : ReleaseSqlComponent35
  {
    public override ReleaseLogContainers GetReleaseLogContainers(
      Guid projectId,
      int releaseId,
      bool skipIsDeletedCheck)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseRunPlanIdRefs", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindBoolean(nameof (skipIsDeletedCheck), skipIsDeletedCheck);
      List<ReleaseDeployPhaseRef> items1;
      List<DeploymentGateRef> items2;
      List<ReleaseEnvironmentStep> items3;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDeployPhaseRef>((ObjectBinder<ReleaseDeployPhaseRef>) this.GetReleaseRunPlanIdRefBinder());
        items1 = resultCollection.GetCurrent<ReleaseDeployPhaseRef>().Items;
        resultCollection.NextResult();
        resultCollection.AddBinder<DeploymentGateRef>((ObjectBinder<DeploymentGateRef>) new DeploymentGateRefBinder());
        items2 = resultCollection.GetCurrent<DeploymentGateRef>().Items;
        resultCollection.NextResult();
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        items3 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
      }
      ReleaseLogContainers releaseLogContainers = new ReleaseLogContainers();
      releaseLogContainers.DeployPhases = (IList<ReleaseDeployPhaseRef>) items1.Where<ReleaseDeployPhaseRef>((System.Func<ReleaseDeployPhaseRef, bool>) (e => e.PlanId != Guid.Empty)).ToList<ReleaseDeployPhaseRef>();
      releaseLogContainers.Gates = (IList<DeploymentGateRef>) items2.Where<DeploymentGateRef>((System.Func<DeploymentGateRef, bool>) (g => g.RunPlanId.HasValue && g.RunPlanId.Value != Guid.Empty)).ToList<DeploymentGateRef>();
      releaseLogContainers.DeploySteps.AddRange<ReleaseEnvironmentStep, IList<ReleaseEnvironmentStep>>((IEnumerable<ReleaseEnvironmentStep>) items3);
      return releaseLogContainers;
    }

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
        resultCollection.AddBinder<DeploymentIssue>((ObjectBinder<DeploymentIssue>) this.GetDeploymentIssueBinder());
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
        List<DeploymentIssue> items5 = resultCollection.GetCurrent<DeploymentIssue>().Items;
        resultCollection.NextResult();
        List<ManualIntervention> items6 = resultCollection.GetCurrent<ManualIntervention>().Items;
        resultCollection.NextResult();
        List<ReleaseEnvironmentStep> items7 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
        resultCollection.NextResult();
        List<PipelineArtifactSource> items8 = resultCollection.GetCurrent<PipelineArtifactSource>().Items;
        resultCollection.NextResult();
        List<ReleaseTagData> items9 = resultCollection.GetCurrent<ReleaseTagData>().Items;
        if (release != null)
          ReleaseSqlComponent15.StitchReleaseObject(release, (IEnumerable<ReleaseEnvironment>) items1, (IEnumerable<ReleaseEnvironmentStep>) items7, (IEnumerable<PipelineArtifactSource>) items8, (IEnumerable<ReleaseDeployPhase>) items2, (IEnumerable<Deployment>) items3, (IEnumerable<ManualIntervention>) items6, (IEnumerable<ReleaseTagData>) items9, (IEnumerable<DeploymentGate>) items4, (IEnumerable<DeploymentIssue>) items5);
        return release;
      }
    }

    public override IEnumerable<AutoTriggerIssue> GetAutoTriggerIssues(
      Guid projectId,
      string artifactType,
      string sourceId,
      string artifactVersionId)
    {
      if (projectId == Guid.Empty)
        this.PrepareStoredProcedure("Release.prc_GetAutoTriggerIssues");
      else
        this.PrepareStoredProcedure("Release.prc_GetAutoTriggerIssues", projectId);
      this.BindString(nameof (artifactType), artifactType, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString(nameof (sourceId), sourceId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString(nameof (artifactVersionId), artifactVersionId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      return this.GetAutoTriggerIssuesObject();
    }

    public override Release UpdateApprovalStepsStatus(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      EnvironmentStepType stepType,
      ReleaseEnvironmentStepStatus statusFrom,
      ReleaseEnvironmentStepStatus statusTo,
      string comment)
    {
      this.PrepareStoredProcedure("Release.prc_ReleaseEnvironmentStep_BulkUpdate", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindByte(nameof (stepType), (byte) stepType);
      this.BindByte(nameof (statusFrom), (byte) statusFrom);
      this.BindByte(nameof (statusTo), (byte) statusTo);
      this.BindString(nameof (comment), comment, 4000, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      return this.GetReleaseObject(projectId);
    }
  }
}
