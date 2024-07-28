// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent22
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
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
  public class ReleaseSqlComponent22 : ReleaseSqlComponent21
  {
    protected override void BindIncludeVariables(bool includeVariables) => this.BindBoolean(nameof (includeVariables), includeVariables);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Override of base class method.")]
    protected override ReleaseRunPlanIdRefBinder GetReleaseRunPlanIdRefBinder() => (ReleaseRunPlanIdRefBinder) new ReleaseRunPlanIdRefBinder3();

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This class has many dependencies on other classes.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "by design")]
    public override IEnumerable<Release> ListReleases(
      Guid projectId,
      string namePattern,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      ReleaseStatus statusFilter,
      ReleaseEnvironmentStatus environmentStatusFilter,
      IEnumerable<Guid> createdByIds,
      DateTime? minCreatedTime,
      DateTime? maxCreatedTime,
      DateTime? maxModifiedTime,
      int maxReleases,
      ReleaseQueryOrder releaseQueryOrder,
      int releaseContinuationToken,
      bool includeEnvironments,
      bool includeArtifacts,
      bool includeApprovals,
      bool includeManualInterventions,
      string artifactTypeId,
      string sourceId,
      string artifactVersionId,
      string sourceBranchFilter,
      bool isDeleted,
      bool includeDeletedReleaseDefinitions,
      bool includeVariables = false,
      bool includeTags = false,
      IEnumerable<string> tagFilter = null,
      IEnumerable<int> releaseIds = null,
      string path = null)
    {
      if (projectId.Equals(Guid.Empty))
        this.PrepareStoredProcedure("Release.prc_QueryReleases");
      else
        this.PrepareStoredProcedure("Release.prc_QueryReleases", projectId);
      this.BindByte("definitionType", (byte) 1);
      this.BindString(nameof (namePattern), namePattern, 256, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt(nameof (definitionEnvironmentId), definitionEnvironmentId);
      this.BindByte(nameof (statusFilter), (byte) statusFilter);
      this.BindEnvironmentStatusFilter(environmentStatusFilter);
      this.BindGuidTable("createdByIdTable", createdByIds);
      this.BindNullableDateTime(nameof (minCreatedTime), minCreatedTime);
      this.BindNullableDateTime(nameof (maxCreatedTime), maxCreatedTime);
      this.BindMaxModifiedTime(nameof (maxModifiedTime), maxModifiedTime);
      this.BindInt(nameof (maxReleases), maxReleases);
      this.BindByte("queryOrder", (byte) releaseQueryOrder);
      this.BindInt(nameof (releaseContinuationToken), releaseContinuationToken);
      this.BindBoolean(nameof (includeEnvironments), includeEnvironments);
      this.BindBoolean(nameof (includeArtifacts), includeArtifacts);
      this.BindBoolean(nameof (includeApprovals), includeApprovals);
      this.BindBoolean(nameof (includeManualInterventions), includeManualInterventions);
      this.BindIncludeVariables(includeVariables);
      this.BindBoolean(nameof (includeDeletedReleaseDefinitions), includeDeletedReleaseDefinitions);
      this.BindString(nameof (artifactTypeId), artifactTypeId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("SourceId", sourceId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString(nameof (artifactVersionId), artifactVersionId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString(nameof (sourceBranchFilter), sourceBranchFilter, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindIsDeleted(nameof (isDeleted), isDeleted);
      this.BindFolderPath(path);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        List<ReleaseEnvironment> source1 = new List<ReleaseEnvironment>();
        List<ReleaseDeployPhase> source2 = new List<ReleaseDeployPhase>();
        List<ReleaseEnvironmentStep> source3 = new List<ReleaseEnvironmentStep>();
        List<PipelineArtifactSource> source4 = new List<PipelineArtifactSource>();
        List<Deployment> source5 = new List<Deployment>();
        List<ManualIntervention> source6 = new List<ManualIntervention>();
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseBinder(projectId));
        List<Release> items = resultCollection.GetCurrent<Release>().Items;
        if (includeEnvironments)
        {
          resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
          resultCollection.NextResult();
          source1 = resultCollection.GetCurrent<ReleaseEnvironment>().Items;
          resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
          resultCollection.NextResult();
          source2 = resultCollection.GetCurrent<ReleaseDeployPhase>().Items;
          resultCollection.AddBinder<Deployment>((ObjectBinder<Deployment>) this.GetDeploymentBinder());
          resultCollection.NextResult();
          source5 = resultCollection.GetCurrent<Deployment>().Items;
        }
        if (includeManualInterventions)
        {
          resultCollection.AddBinder<ManualIntervention>((ObjectBinder<ManualIntervention>) this.GetManualInterventionListBinder());
          resultCollection.NextResult();
          source6 = resultCollection.GetCurrent<ManualIntervention>().Items;
        }
        if (includeApprovals)
        {
          resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
          resultCollection.NextResult();
          source3 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
        }
        if (includeArtifacts)
        {
          resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
          resultCollection.NextResult();
          source4 = resultCollection.GetCurrent<PipelineArtifactSource>().Items;
        }
        foreach (Release release1 in items)
        {
          Release release = release1;
          List<ReleaseEnvironment> list = source1.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (env => env.ProjectId.Equals(release.ProjectId) && env.ReleaseId == release.Id)).ToList<ReleaseEnvironment>();
          List<Deployment> deploymentList = new List<Deployment>();
          List<ReleaseEnvironmentStep> steps = new List<ReleaseEnvironmentStep>();
          List<ReleaseDeployPhase> releaseDeployPhaseList = new List<ReleaseDeployPhase>();
          List<ManualIntervention> manualInterventionList = new List<ManualIntervention>();
          foreach (ReleaseEnvironment releaseEnvironment in list)
          {
            ReleaseEnvironment environment = releaseEnvironment;
            Deployment lastDeployment = source5.Where<Deployment>((System.Func<Deployment, bool>) (d => d.ProjectId.Equals(release.ProjectId) && d.ReleaseId == release.Id && d.ReleaseEnvironmentId == environment.Id)).OrderBy<Deployment, int>((System.Func<Deployment, int>) (d => d.Attempt)).LastOrDefault<Deployment>();
            if (lastDeployment != null)
            {
              deploymentList.Add(lastDeployment);
              if (source3 != null)
                steps.AddRange(source3.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (s => s.ProjectId.Equals(release.ProjectId) && s.ReleaseId == release.Id && s.ReleaseEnvironmentId == environment.Id && s.TrialNumber == lastDeployment.Attempt)));
              if (source2 != null)
                releaseDeployPhaseList.AddRange(source2.Where<ReleaseDeployPhase>((System.Func<ReleaseDeployPhase, bool>) (e => e.ProjectId.Equals(release.ProjectId) && e.ReleaseId == release.Id && e.ReleaseEnvironmentId == environment.Id && e.Attempt == lastDeployment.Attempt)));
              if (source6 != null)
              {
                IEnumerable<int> allReleaseDeployPhaseIds = releaseDeployPhaseList.Where<ReleaseDeployPhase>((System.Func<ReleaseDeployPhase, bool>) (d => d.ReleaseEnvironmentId == environment.Id && d.Attempt == lastDeployment.Attempt)).Select<ReleaseDeployPhase, int>((System.Func<ReleaseDeployPhase, int>) (r => r.Id));
                manualInterventionList.AddRange(source6.Where<ManualIntervention>((System.Func<ManualIntervention, bool>) (m => m.ProjectId.Equals(release.ProjectId) && allReleaseDeployPhaseIds.Contains<int>(m.ReleaseDeployPhaseId))));
              }
            }
          }
          ReleaseSqlComponent15.StitchReleaseObject(release, (IEnumerable<ReleaseEnvironment>) list, (IEnumerable<ReleaseEnvironmentStep>) steps, source4.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (source => source.ReleaseId == release.Id)), (IEnumerable<ReleaseDeployPhase>) releaseDeployPhaseList, (IEnumerable<Deployment>) deploymentList, (IEnumerable<ManualIntervention>) manualInterventionList, (IEnumerable<ReleaseTagData>) null);
        }
        return (IEnumerable<Release>) items;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override Release GetRelease(Guid projectId, int releaseId, bool isDeleted = false)
    {
      this.PrepareStoredProcedure("Release.prc_Release_Get", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindBoolean(nameof (isDeleted), isDeleted);
      return this.GetReleaseObject(projectId);
    }
  }
}
