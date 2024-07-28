// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent30
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseSqlComponent30 : ReleaseSqlComponent29
  {
    public override IEnumerable<Release> RejectMultipleReleaseEnvironments(
      Guid projectId,
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      if (releaseEnvironmentSteps.Count<ReleaseEnvironmentStep>() > 1)
        this.PrepareForAuditingAction(ReleaseAuditConstants.DeploymentsCompleted, projectId: projectId, excludeSqlParameters: true);
      else
        this.PrepareForAuditingAction(ReleaseAuditConstants.DeploymentCompleted, projectId: projectId, excludeSqlParameters: true);
      this.PrepareStoredProcedure("Release.prc_RejectMultipleReleaseEnvironment", projectId);
      this.BindToReleaseEnvironmentStepsTable(nameof (releaseEnvironmentSteps), (IList<ReleaseEnvironmentStep>) releaseEnvironmentSteps.ToList<ReleaseEnvironmentStep>());
      return this.GetReleaseObjects(projectId, true, true, true, true, true);
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
          releaseEnvironment.ReleaseDeployPhases.AddRange<ReleaseDeployPhase, IList<ReleaseDeployPhase>>((IEnumerable<ReleaseDeployPhase>) items1);
          releaseEnvironment.DeploymentAttempts.AddRange<Deployment, IList<Deployment>>((IEnumerable<Deployment>) items2);
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

    public override ReleaseDeployPhase AddReleaseDeployPhase(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int rank,
      int attempt,
      DeployPhaseTypes phaseType,
      Guid changedBy)
    {
      this.PrepareStoredProcedure("Release.prc_AddReleaseDeployPhase", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt("deployPhaseRank", rank);
      this.BindByte("deployPhaseType", (byte) phaseType);
      this.BindInt(nameof (attempt), attempt);
      this.BindGuid("changedby", changedBy);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
        return resultCollection.GetCurrent<ReleaseDeployPhase>().Items.FirstOrDefault<ReleaseDeployPhase>();
      }
    }

    protected override void BindToReleaseEnvironmentStepsTable(
      string parameterName,
      IList<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      this.BindReleaseEnvironmentStepTable7(parameterName, (IEnumerable<ReleaseEnvironmentStep>) releaseEnvironmentSteps);
    }
  }
}
