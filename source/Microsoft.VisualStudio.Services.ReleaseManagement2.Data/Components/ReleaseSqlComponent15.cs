// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent15
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent15 : ReleaseSqlComponent14
  {
    protected static PipelineArtifactSource ConvertToReleaseArtifactSource(ArtifactSource artifact)
    {
      if (artifact == null)
        throw new ArgumentNullException(nameof (artifact));
      if (!artifact.SourceData.ContainsKey("version"))
        return (PipelineArtifactSource) null;
      PipelineArtifactSource pipelineArtifactSource = new PipelineArtifactSource();
      pipelineArtifactSource.SourceId = artifact.SourceId;
      pipelineArtifactSource.Alias = artifact.Alias;
      pipelineArtifactSource.ArtifactVersion = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) artifact.SourceData["version"]);
      pipelineArtifactSource.SourceBranch = artifact.SourceBranch;
      pipelineArtifactSource.IsPrimary = artifact.IsPrimary;
      pipelineArtifactSource.ArtifactTypeId = artifact.ArtifactTypeId;
      PipelineArtifactSource releaseArtifactSource = pipelineArtifactSource;
      ArtifactSourceDataUtility.CompressSourceData((IDictionary<string, InputValue>) artifact.SourceData, (IDictionary<string, InputValue>) releaseArtifactSource.SourceData);
      return releaseArtifactSource;
    }

    protected static IEnumerable<ArtifactSource> GetArtifacts(
      IEnumerable<PipelineArtifactSource> releaseArtifactSources)
    {
      if (releaseArtifactSources == null)
        return (IEnumerable<ArtifactSource>) null;
      List<ArtifactSource> artifacts = new List<ArtifactSource>();
      foreach (PipelineArtifactSource releaseArtifactSource in releaseArtifactSources)
      {
        ReleaseSqlComponent.SetArtifactVersionDetails((ArtifactSource) releaseArtifactSource, releaseArtifactSource.Version);
        artifacts.Add((ArtifactSource) releaseArtifactSource);
      }
      return (IEnumerable<ArtifactSource>) artifacts;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "by design")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Cannot avoid this as it is required to construct the basic Release object")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "params added later")]
    protected static void StitchReleaseObject(
      Release release,
      IEnumerable<ReleaseEnvironment> environments,
      IEnumerable<ReleaseEnvironmentStep> steps,
      IEnumerable<PipelineArtifactSource> releaseArtifactSources,
      IEnumerable<ReleaseDeployPhase> deployPhases,
      IEnumerable<Deployment> deployments,
      IEnumerable<ManualIntervention> manualInterventions,
      IEnumerable<ReleaseTagData> releaseTags,
      IEnumerable<DeploymentGate> deploymentGates = null,
      IEnumerable<DeploymentIssue> deploymentIssues = null)
    {
      if (deployPhases == null)
        deployPhases = (IEnumerable<ReleaseDeployPhase>) new List<ReleaseDeployPhase>();
      if (releaseArtifactSources == null)
        releaseArtifactSources = (IEnumerable<PipelineArtifactSource>) new List<PipelineArtifactSource>();
      if (deployments == null)
        deployments = (IEnumerable<Deployment>) new List<Deployment>();
      if (manualInterventions == null)
        manualInterventions = (IEnumerable<ManualIntervention>) new List<ManualIntervention>();
      if (releaseTags == null)
        releaseTags = (IEnumerable<ReleaseTagData>) new List<ReleaseTagData>();
      if (deploymentGates == null)
        deploymentGates = (IEnumerable<DeploymentGate>) new List<DeploymentGate>();
      if (deploymentIssues == null)
        deploymentIssues = (IEnumerable<DeploymentIssue>) new List<DeploymentIssue>();
      foreach (ReleaseEnvironment releaseEnvironment in environments.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (env => env.ProjectId.Equals(release.ProjectId) && env.ReleaseId == release.Id)))
      {
        ReleaseEnvironment environment = releaseEnvironment;
        foreach (ReleaseDeployPhase releaseDeployPhase in deployPhases.Where<ReleaseDeployPhase>((System.Func<ReleaseDeployPhase, bool>) (d => d.ProjectId.Equals(environment.ProjectId) && d.ReleaseEnvironmentId == environment.Id)))
          environment.ReleaseDeployPhases.Add(releaseDeployPhase);
        foreach (Deployment deployment1 in deployments.Where<Deployment>((System.Func<Deployment, bool>) (d => d.ProjectId.Equals(environment.ProjectId) && d.ReleaseEnvironmentId == environment.Id)))
        {
          Deployment deployment = deployment1;
          deployment.ReleaseName = release.Name;
          deployment.ReleaseDefinitionName = release.ReleaseDefinitionName;
          deployment.ReleaseDefinitionPath = release.ReleaseDefinitionPath;
          deployment.ReleaseEnvironmentName = environment.Name;
          deployment.Conditions = environment.Conditions;
          deployment.Steps.AddRange<ReleaseEnvironmentStep, IList<ReleaseEnvironmentStep>>(environment.GetStepsForTests.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (step => step.TrialNumber == deployment.Attempt)));
          DateTime? scheduledDeploymentTime = environment.ScheduledDeploymentTime;
          if (scheduledDeploymentTime.HasValue)
          {
            Deployment deployment2 = deployment;
            scheduledDeploymentTime = environment.ScheduledDeploymentTime;
            DateTime dateTime = scheduledDeploymentTime.Value;
            deployment2.ScheduledDeploymentTime = dateTime;
          }
          foreach (DeploymentGate deploymentGate in deploymentGates.Where<DeploymentGate>((System.Func<DeploymentGate, bool>) (g => g.ReleaseEnvironmentId == environment.Id && g.DeploymentId == deployment.Id)))
            deployment.DeploymentGates.Add(deploymentGate);
          deployment.DeploymentIssues.AddRange<Issue, IList<Issue>>(deploymentIssues.Where<DeploymentIssue>((System.Func<DeploymentIssue, bool>) (issue => issue.ReleaseId == release.Id && issue.DeploymentId == deployment.Id)).Select<DeploymentIssue, Issue>((System.Func<DeploymentIssue, Issue>) (e => e.Issue)));
          environment.DeploymentAttempts.Add(deployment);
        }
        release.Environments.Add(environment);
      }
      foreach (ReleaseEnvironment environment in (IEnumerable<ReleaseEnvironment>) release.Environments)
      {
        ReleaseEnvironment env = environment;
        int environmentId = env.Id;
        foreach (ReleaseEnvironmentStep releaseEnvironmentStep in steps.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (step => step.ProjectId.Equals(env.ProjectId) && step.ReleaseEnvironmentId == environmentId)))
          env.GetStepsForTests.Add(releaseEnvironmentStep);
        if (release.DefinitionSnapshot != null)
        {
          DefinitionEnvironmentData definitionEnvironmentData = release.GetDefinitionEnvironmentData(env.Name);
          if (definitionEnvironmentData != null)
          {
            IEnumerable<DefinitionEnvironmentStepData> environmentSteps1 = definitionEnvironmentData.GetDefinitionEnvironmentSteps(EnvironmentStepType.PreDeploy);
            env.PreApprovalOptions = ReleaseSqlComponent.PopulateApprovalOptionsIfNotPresent(environmentSteps1, env.PreApprovalOptions);
            IEnumerable<DefinitionEnvironmentStepData> environmentSteps2 = definitionEnvironmentData.GetDefinitionEnvironmentSteps(EnvironmentStepType.PostDeploy);
            env.PostApprovalOptions = ReleaseSqlComponent.PopulateApprovalOptionsIfNotPresent(environmentSteps2, env.PostApprovalOptions);
          }
        }
        foreach (ManualIntervention manualIntervention in manualInterventions.Where<ManualIntervention>((System.Func<ManualIntervention, bool>) (mi => mi.ProjectId.Equals(env.ProjectId) && mi.ReleaseEnvironmentId == environmentId)))
        {
          ReleaseSqlComponent15.PopulateManualInterventionData(manualIntervention, env, release);
          env.ManualInterventions.Add(manualIntervention);
        }
      }
      foreach (ArtifactSource artifact in ReleaseSqlComponent15.GetArtifacts(releaseArtifactSources.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (r => r.ProjectId.Equals(release.ProjectId)))))
        release.LinkedArtifacts.Add(artifact);
      foreach (ReleaseEnvironment environment in release.Environments.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (env => env.Status == ReleaseEnvironmentStatus.Undefined)))
        environment.Status = ReleaseEnvironmentConverter.GetEnvironmentStatus(release, environment).FromWebApi();
      foreach (ReleaseTagData releaseTag in releaseTags)
      {
        if (release.Id == releaseTag.ReleaseId)
          release.Tags.Add(releaseTag.Tag);
      }
    }

    protected override ReleaseArtifactSourceBinder GetReleaseArtifactSourceBinder() => (ReleaseArtifactSourceBinder) new ReleaseArtifactSourceBinder5((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected override ReleaseEnvironmentBinder GetReleaseEnvironmentBinder() => (ReleaseEnvironmentBinder) new ReleaseEnvironmentBinder2((ReleaseManagementSqlResourceComponentBase) this);

    protected override void BindReleaseEnvironments(
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      this.BindReleaseEnvironmentTable7(nameof (releaseEnvironments), releaseEnvironments);
    }

    protected override void BindReleaseArtifactSourceTable(
      IEnumerable<PipelineArtifactSource> releaseArtifactSources)
    {
      this.BindReleaseArtifactSourceTable6(nameof (releaseArtifactSources), releaseArtifactSources);
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
      this.BindInt(nameof (attempt), attempt);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
        return resultCollection.GetCurrent<ReleaseDeployPhase>().Items.FirstOrDefault<ReleaseDeployPhase>();
      }
    }

    public override ReleaseDeployPhase UpdateReleaseDeployPhase(
      Guid projectId,
      ReleaseDeployPhase releaseDeployPhase,
      DeploymentOperationStatus operationStatus,
      Guid changedBy)
    {
      if (releaseDeployPhase == null)
        throw new ArgumentNullException(nameof (releaseDeployPhase));
      this.PrepareStoredProcedure("Release.prc_UpdateReleaseDeployPhase", projectId);
      this.BindInt("releaseId", releaseDeployPhase.ReleaseId);
      this.BindInt("releaseEnvironmentId", releaseDeployPhase.ReleaseEnvironmentId);
      this.BindInt("releaseDeployPhaseId", releaseDeployPhase.Id);
      this.BindByte("status", (byte) releaseDeployPhase.Status);
      this.BindNullableGuid("runPlanId", releaseDeployPhase.RunPlanId);
      this.BindString("logs", releaseDeployPhase.Logs, 4000, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
        return resultCollection.GetCurrent<ReleaseDeployPhase>().Items.FirstOrDefault<ReleaseDeployPhase>();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
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
      this.PrepareStoredProcedure("Release.prc_QueryReleases", projectId);
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
      this.BindString(nameof (artifactTypeId), artifactTypeId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("SourceId", sourceId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString(nameof (artifactVersionId), artifactVersionId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindIsDeleted(nameof (isDeleted), isDeleted);
      this.BindFolderPath(path);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        List<ReleaseEnvironment> source1 = new List<ReleaseEnvironment>();
        List<ReleaseDeployPhase> deployPhases = new List<ReleaseDeployPhase>();
        List<ReleaseEnvironmentStep> source2 = new List<ReleaseEnvironmentStep>();
        List<PipelineArtifactSource> source3 = new List<PipelineArtifactSource>();
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseBinder(projectId));
        List<Release> items = resultCollection.GetCurrent<Release>().Items;
        if (includeEnvironments)
        {
          resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
          resultCollection.NextResult();
          source1 = resultCollection.GetCurrent<ReleaseEnvironment>().Items;
          resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
          resultCollection.NextResult();
          deployPhases = resultCollection.GetCurrent<ReleaseDeployPhase>().Items;
        }
        if (includeApprovals)
        {
          resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
          resultCollection.NextResult();
          source2 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
        }
        if (includeArtifacts)
        {
          resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
          resultCollection.NextResult();
          source3 = resultCollection.GetCurrent<PipelineArtifactSource>().Items;
        }
        foreach (Release release1 in items)
        {
          Release release = release1;
          ReleaseSqlComponent15.StitchReleaseObject(release, source1.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (env => env.ReleaseId == release.Id)), source2.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (step => step.ReleaseId == release.Id)), source3.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (source => source.ReleaseId == release.Id)), (IEnumerable<ReleaseDeployPhase>) deployPhases, (IEnumerable<Deployment>) null, (IEnumerable<ManualIntervention>) null, (IEnumerable<ReleaseTagData>) null);
        }
        return (IEnumerable<Release>) items;
      }
    }

    public override ReleaseDefinitionSummary GetReleaseSummaryForReleaseDefinition(
      Guid projectId,
      int releaseDefinitionId,
      int releaseCount,
      bool includeArtifact,
      IList<int> definitionEnvironmentIdsFilterList)
    {
      releaseCount = releaseCount == 0 ? 10 : releaseCount;
      this.PrepareStoredProcedure("Release.prc_ReleaseDefinitionSummary_ListReleases", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt(nameof (releaseCount), releaseCount);
      this.BindBoolean(nameof (includeArtifact), includeArtifact);
      ReleaseDefinitionSummary releaseDefinition = new ReleaseDefinitionSummary();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        List<PipelineArtifactSource> source1 = new List<PipelineArtifactSource>();
        resultCollection.AddBinder<ShallowReference>((ObjectBinder<ShallowReference>) this.GetShallowReferenceBinder());
        ShallowReference shallowReference1 = resultCollection.GetCurrent<ShallowReference>().Items.Single<ShallowReference>();
        ReleaseDefinitionSummary definitionSummary = releaseDefinition;
        ReleaseDefinitionShallowReference shallowReference2 = new ReleaseDefinitionShallowReference();
        shallowReference2.Id = shallowReference1.Id;
        shallowReference2.Name = shallowReference1.Name;
        definitionSummary.ReleaseDefinition = shallowReference2;
        resultCollection.AddBinder<DefinitionEnvironment>((ObjectBinder<DefinitionEnvironment>) this.GetDefinitionEnvironmentListBinder());
        resultCollection.NextResult();
        List<DefinitionEnvironment> items1 = resultCollection.GetCurrent<DefinitionEnvironment>().Items;
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseListBinder());
        resultCollection.NextResult();
        List<Release> items2 = resultCollection.GetCurrent<Release>().Items;
        resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentListBinder());
        resultCollection.NextResult();
        List<ReleaseEnvironment> items3 = resultCollection.GetCurrent<ReleaseEnvironment>().Items;
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        resultCollection.NextResult();
        List<ReleaseEnvironmentStep> items4 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
        resultCollection.AddBinder<ReleaseToEnvironmentMap>((ObjectBinder<ReleaseToEnvironmentMap>) this.GetReleaseToEnvironmentMapBinder());
        resultCollection.NextResult();
        List<ReleaseToEnvironmentMap> items5 = resultCollection.GetCurrent<ReleaseToEnvironmentMap>().Items;
        foreach (ReleaseToEnvironmentMap toEnvironmentMap in items5)
          releaseDefinition.ReleaseToEnvironmentMap.Add(toEnvironmentMap);
        foreach (int num in items1.Select<DefinitionEnvironment, int>((System.Func<DefinitionEnvironment, int>) (e => e.Id)).Except<int>(items5.Select<ReleaseToEnvironmentMap, int>((System.Func<ReleaseToEnvironmentMap, int>) (map => map.DefinitionEnvironmentId)).Distinct<int>()))
          releaseDefinition.ReleaseToEnvironmentMap.Add(new ReleaseToEnvironmentMap()
          {
            DefinitionEnvironmentId = num
          });
        if (includeArtifact)
        {
          resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
          resultCollection.NextResult();
          source1 = resultCollection.GetCurrent<PipelineArtifactSource>().Items;
        }
        foreach (Release release in items2)
        {
          int releaseId = release.Id;
          ReleaseSqlComponent15.StitchReleaseObject(release, items3.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (env => env.ReleaseId == releaseId)), items4.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (step => step.ReleaseId == releaseId)), source1.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (source => source.ReleaseId == releaseId)), (IEnumerable<ReleaseDeployPhase>) null, (IEnumerable<Deployment>) null, (IEnumerable<ManualIntervention>) null, (IEnumerable<ReleaseTagData>) null);
          releaseDefinition.Releases.Add(release);
        }
        return releaseDefinition;
      }
    }

    public override IList<ReleaseDeployPhase> GetDeployPhasesForEnvironment(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int releaseDeployPhaseId,
      int attempt)
    {
      this.PrepareStoredProcedure("Release.prc_RunPlanId_Get", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt("environmentId", releaseEnvironmentId);
      this.BindInt("trialNumber", attempt);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new RunPlanIdBinder());
        List<Guid> items = resultCollection.GetCurrent<Guid>().Items;
        IList<ReleaseDeployPhase> phasesForEnvironment = (IList<ReleaseDeployPhase>) new List<ReleaseDeployPhase>();
        foreach (Guid guid in items)
          phasesForEnvironment.Add(new ReleaseDeployPhase()
          {
            ReleaseId = releaseId,
            ReleaseEnvironmentId = releaseEnvironmentId,
            Attempt = attempt,
            Id = releaseDeployPhaseId,
            PhaseType = DeployPhaseTypes.AgentBasedDeployment,
            Rank = 1,
            RunPlanId = new Guid?(guid)
          });
        return phasesForEnvironment;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Too heavy to be a property")]
    protected override Release GetReleaseObject(Guid projectId)
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseBinder(projectId));
        resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
        resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
        Release release = resultCollection.GetCurrent<Release>().FirstOrDefault<Release>();
        resultCollection.NextResult();
        List<ReleaseEnvironment> items1 = resultCollection.GetCurrent<ReleaseEnvironment>().Items;
        resultCollection.NextResult();
        List<ReleaseDeployPhase> items2 = resultCollection.GetCurrent<ReleaseDeployPhase>().Items;
        resultCollection.NextResult();
        List<ReleaseEnvironmentStep> items3 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
        resultCollection.NextResult();
        List<PipelineArtifactSource> items4 = resultCollection.GetCurrent<PipelineArtifactSource>().Items;
        if (release != null)
          ReleaseSqlComponent15.StitchReleaseObject(release, (IEnumerable<ReleaseEnvironment>) items1, (IEnumerable<ReleaseEnvironmentStep>) items3, (IEnumerable<PipelineArtifactSource>) items4, (IEnumerable<ReleaseDeployPhase>) items2, (IEnumerable<Deployment>) null, (IEnumerable<ManualIntervention>) null, (IEnumerable<ReleaseTagData>) null);
        return release;
      }
    }

    protected override IEnumerable<PipelineArtifactSource> GetReleaseArtifactSources(
      IEnumerable<ArtifactSource> linkedArtifacts)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return linkedArtifacts == null ? (IEnumerable<PipelineArtifactSource>) new List<PipelineArtifactSource>() : (IEnumerable<PipelineArtifactSource>) linkedArtifacts.Select<ArtifactSource, PipelineArtifactSource>(ReleaseSqlComponent15.\u003C\u003EO.\u003C0\u003E__ConvertToReleaseArtifactSource ?? (ReleaseSqlComponent15.\u003C\u003EO.\u003C0\u003E__ConvertToReleaseArtifactSource = new System.Func<ArtifactSource, PipelineArtifactSource>(ReleaseSqlComponent15.ConvertToReleaseArtifactSource))).Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (releaseArtifactSource => releaseArtifactSource != null)).ToList<PipelineArtifactSource>();
    }

    private static void PopulateManualInterventionData(
      ManualIntervention manualIntervention,
      ReleaseEnvironment environment,
      Release release)
    {
      manualIntervention.ReleaseName = release.Name;
      manualIntervention.ReleaseEnvironmentName = environment.Name;
      manualIntervention.ReleaseDefinitionId = release.ReleaseDefinitionId;
      manualIntervention.ReleaseDefinitionName = release.ReleaseDefinitionName;
    }

    protected override void BindDefinitionEnvironmentRetentionPolicy(
      string parameterName,
      IEnumerable<DefinitionEnvironment> definitionEnvironments)
    {
      this.BindEnvironmentRetentionPolicyTable(parameterName, definitionEnvironments);
    }

    protected override void BindThresholdDate(string parameterName, DateTime thresholdDate)
    {
    }
  }
}
