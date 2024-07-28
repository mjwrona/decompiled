// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
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
  [SupportedSqlAccessIntent(SqlAccessIntent.ReadWrite | SqlAccessIntent.ReadOnly, null)]
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "To be reviewed.")]
  public class ReleaseSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[50]
    {
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent>(1),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent4>(4),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent5>(5),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent6>(6),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent7>(7),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent8>(8),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent9>(9),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent10>(10),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent11>(11),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent12>(12),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent13>(13),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent14>(14),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent15>(15),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent16>(16),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent17>(17),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent18>(18),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent19>(19),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent20>(20),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent21>(21),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent22>(22),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent23>(23),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent24>(24),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent25>(25),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent26>(26),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent27>(27),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent28>(28),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent29>(29),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent30>(30),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent31>(31),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent32>(32),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent33>(33),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent34>(34),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent35>(35),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent36>(36),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent37>(37),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent38>(38),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent39>(39),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent40>(40),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent41>(41),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent42>(42),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent43>(43),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent44>(44),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent45>(45),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent46>(46),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent47>(47),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent48>(48),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent49>(49),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent50>(50),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent51>(51),
      (IComponentCreator) new ComponentCreator<ReleaseSqlComponent52>(52)
    }, "ReleaseManagementRelease", "ReleaseManagement");

    protected static void SetArtifactVersionDetails(
      ArtifactSource artifactSource,
      InputValue version)
    {
      if (artifactSource == null || version == null)
        return;
      if (artifactSource.SourceData.ContainsKey(nameof (version)))
      {
        artifactSource.SourceData[nameof (version)].DisplayValue = version.DisplayValue;
        artifactSource.SourceData[nameof (version)].Value = version.Value;
      }
      else
        artifactSource.SourceData[nameof (version)] = version;
    }

    protected static ApprovalOptions PopulateApprovalOptionsIfNotPresent(
      IEnumerable<DefinitionEnvironmentStepData> approvalSteps,
      ApprovalOptions existingApprovalOptions)
    {
      if (existingApprovalOptions != null || approvalSteps == null)
        return existingApprovalOptions;
      DefinitionEnvironmentStepData environmentStepData = approvalSteps.FirstOrDefault<DefinitionEnvironmentStepData>();
      return environmentStepData != null && !environmentStepData.IsAutomated ? new ApprovalOptions() : (ApprovalOptions) null;
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "deployPhases", Justification = "By design")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "environments", Justification = "By design")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "release", Justification = "By design")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "releaseArtifactDefinitions", Justification = "By design")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "releaseArtifactSources", Justification = "By design")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "steps", Justification = "By design")]
    protected static void StitchReleaseObject(
      Release release,
      IEnumerable<ReleaseEnvironment> environments,
      IEnumerable<ReleaseEnvironmentStep> steps,
      IEnumerable<PipelineArtifactSource> releaseArtifactSources,
      IEnumerable<ReleaseArtifactDefinition> releaseArtifactDefinitions,
      IEnumerable<ReleaseDeployPhase> deployPhases)
    {
      throw new NotImplementedException();
    }

    protected virtual IEnumerable<PipelineArtifactSource> GetReleaseArtifactSources(
      IEnumerable<ArtifactSource> linkedArtifacts)
    {
      if (linkedArtifacts == null)
        return (IEnumerable<PipelineArtifactSource>) new List<PipelineArtifactSource>();
      List<PipelineArtifactSource> releaseArtifactSources = new List<PipelineArtifactSource>();
      foreach (ArtifactSource linkedArtifact in linkedArtifacts)
      {
        if (linkedArtifact.SourceData.ContainsKey("version"))
        {
          List<PipelineArtifactSource> pipelineArtifactSourceList = releaseArtifactSources;
          PipelineArtifactSource pipelineArtifactSource = new PipelineArtifactSource();
          pipelineArtifactSource.ArtifactSourceId = linkedArtifact.Id;
          pipelineArtifactSource.Alias = linkedArtifact.Alias;
          pipelineArtifactSource.ArtifactVersion = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) linkedArtifact.SourceData["version"]);
          pipelineArtifactSource.SourceBranch = linkedArtifact.SourceBranch;
          pipelineArtifactSource.IsPrimary = linkedArtifact.IsPrimary;
          pipelineArtifactSourceList.Add(pipelineArtifactSource);
        }
      }
      return (IEnumerable<PipelineArtifactSource>) releaseArtifactSources;
    }

    protected virtual void BindToReleaseEnvironmentStepsTable(
      string parameterName,
      IList<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      throw new NotImplementedException();
    }

    public virtual Release UpdateApprovalStepsStatus(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      EnvironmentStepType stepType,
      ReleaseEnvironmentStepStatus statusFrom,
      ReleaseEnvironmentStepStatus statusTo,
      string comment)
    {
      throw new NotImplementedException();
    }

    public virtual ReleaseEnvironmentData GetReleaseEnvironmentData(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      bool includeDeployments,
      bool includeApprovals,
      bool includeArtifacts)
    {
      Release release = this.GetRelease(projectId, releaseId);
      ReleaseEnvironment environment = release.GetEnvironment(releaseEnvironmentId);
      return new ReleaseEnvironmentData()
      {
        Environment = environment,
        LinkedArtifacts = release.LinkedArtifacts,
        ReleaseDefinitionId = release.ReleaseDefinitionId,
        ReleaseDefinitionName = release.ReleaseDefinitionName,
        ReleaseId = release.Id,
        ReleaseName = release.Name
      };
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Too heavy to be a property")]
    protected virtual Release GetReleaseObject(Guid projectId)
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseBinder(projectId));
        resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
        resultCollection.AddBinder<ReleaseArtifactDefinition>((ObjectBinder<ReleaseArtifactDefinition>) this.GetReleaseArtifactDefinitionBinder());
        Release release = resultCollection.GetCurrent<Release>().FirstOrDefault<Release>();
        resultCollection.NextResult();
        List<ReleaseEnvironment> items1 = resultCollection.GetCurrent<ReleaseEnvironment>().Items;
        resultCollection.NextResult();
        List<ReleaseEnvironmentStep> items2 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
        resultCollection.NextResult();
        List<PipelineArtifactSource> items3 = resultCollection.GetCurrent<PipelineArtifactSource>().Items;
        resultCollection.NextResult();
        List<ReleaseArtifactDefinition> items4 = resultCollection.GetCurrent<ReleaseArtifactDefinition>().Items;
        if (release != null)
        {
          ReleaseSqlComponent.StitchReleaseObject(release, (IEnumerable<ReleaseEnvironment>) items1, (IEnumerable<ReleaseEnvironmentStep>) items2, (IEnumerable<PipelineArtifactSource>) items3, (IEnumerable<ReleaseArtifactDefinition>) items4, (IEnumerable<ReleaseDeployPhase>) null);
          if (items2.Any<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (step => step.StepType == EnvironmentStepType.Deploy)))
          {
            IList<RunPlanData> planDataForRelease = this.GetRunPlanDataForRelease(projectId, release.Id);
            foreach (ReleaseEnvironmentStep releaseEnvironmentStep in items2.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (step => step.StepType == EnvironmentStepType.Deploy)))
            {
              ReleaseEnvironmentStep deployStep = releaseEnvironmentStep;
              ReleaseEnvironment environment = release.GetEnvironment(deployStep.ReleaseEnvironmentId);
              RunPlanData runPlanData = planDataForRelease.FirstOrDefault<RunPlanData>((System.Func<RunPlanData, bool>) (data => data.ReleaseStepId == deployStep.Id));
              ReleaseDeployPhase releaseDeployPhase = new ReleaseDeployPhase()
              {
                Attempt = deployStep.TrialNumber,
                Logs = deployStep.Logs,
                Rank = 1,
                ReleaseEnvironmentId = deployStep.ReleaseEnvironmentId,
                ReleaseId = deployStep.ReleaseId
              };
              if (runPlanData != null)
                releaseDeployPhase.RunPlanId = new Guid?(runPlanData.RunPlanId);
              environment.ReleaseDeployPhases.Add(releaseDeployPhase);
            }
          }
        }
        return release;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Override of base class method.")]
    protected virtual ReleaseBinder GetReleaseBinder(Guid projectId) => new ReleaseBinder(this.RequestContext, (ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseEnvironmentBinder GetReleaseEnvironmentBinder() => new ReleaseEnvironmentBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseEnvironmentStepBinder GetReleaseEnvironmentStepBinder() => new ReleaseEnvironmentStepBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseArtifactSourceBinder GetReleaseArtifactSourceBinder() => new ReleaseArtifactSourceBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseArtifactDefinitionBinder GetReleaseArtifactDefinitionBinder() => new ReleaseArtifactDefinitionBinder();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ShallowReferenceBinder GetShallowReferenceBinder() => new ShallowReferenceBinder();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseListBinder GetReleaseListBinder() => new ReleaseListBinder();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseEnvironmentListBinder GetReleaseEnvironmentListBinder() => new ReleaseEnvironmentListBinder();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseToEnvironmentMapBinder GetReleaseToEnvironmentMapBinder() => new ReleaseToEnvironmentMapBinder();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual DefinitionEnvironmentListBinder GetDefinitionEnvironmentListBinder() => new DefinitionEnvironmentListBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseIdListBinder GetReleaseIdListBinder() => new ReleaseIdListBinder();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseDeployPhaseBinder GetReleaseDeployPhaseBinder() => new ReleaseDeployPhaseBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual DeploymentBinder GetDeploymentBinder() => new DeploymentBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ManualInterventionListBinder GetManualInterventionListBinder() => new ManualInterventionListBinder((ReleaseManagementSqlResourceComponentBase) this);

    protected virtual IEnumerable<Release> GetReleaseObjects(
      Guid projectId,
      bool includeEnvironments,
      bool includeManualInterventions,
      bool includeApprovals,
      bool includeArtifacts,
      bool includeTags)
    {
      return (IEnumerable<Release>) new List<Release>();
    }

    public virtual IEnumerable<AutoTriggerIssue> AddAutoTriggerIssues(
      IEnumerable<AutoTriggerIssue> autoTriggerIssuesList)
    {
      return (IEnumerable<AutoTriggerIssue>) new List<AutoTriggerIssue>();
    }

    public virtual IEnumerable<AutoTriggerIssue> GetAutoTriggerIssues(
      Guid projectId,
      string artifactType,
      string sourceId,
      string artifactVersionId)
    {
      return (IEnumerable<AutoTriggerIssue>) new List<AutoTriggerIssue>();
    }

    public virtual IEnumerable<Release> ResetScheduledReleaseEnvironments(
      Guid projectId,
      int releaseDefinitionId,
      IEnumerable<int> definitionEnvironmentIds,
      Guid changedBy,
      ReleaseEnvironmentStatusChangeDetails changeDetails,
      string comment)
    {
      return (IEnumerable<Release>) new List<Release>();
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for test")]
    public virtual Release UpdateReleaseEnvironmentStatus(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      ReleaseEnvironmentStatus statusFrom,
      ReleaseEnvironmentStatus statusTo,
      Guid changedBy,
      ReleaseEnvironmentStatusChangeDetails changeDetails,
      string comment,
      int attempt = 0)
    {
      return this.GetRelease(projectId, releaseId);
    }

    public virtual Release UpdateReleaseEnvironmentConditions(
      Guid projectId,
      int releaseId,
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      return this.GetRelease(projectId, releaseId);
    }

    public virtual Release UpdateReleaseEnvironment(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      DateTime? environmentScheduledDateTime,
      Guid? stageSchedulingJobId,
      Guid changedBy)
    {
      return this.GetRelease(projectId, releaseId);
    }

    public virtual void DeleteRelease(
      Guid projectId,
      int releaseId,
      Guid modifiedBy,
      string comment)
    {
      this.PrepareStoredProcedure("Release.prc_Release_DeleteDraft", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.ExecuteScalar();
    }

    public virtual void UndeleteRelease(
      Guid projectId,
      int releaseId,
      Guid modifiedBy,
      string comment)
    {
    }

    public virtual Release UpdateDraftRelease(Guid projectId, Release release, string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      return this.GetRelease(projectId, release.Id);
    }

    public virtual Release AddRelease(Guid projectId, Release release, string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      this.PrepareStoredProcedure("Release.prc_Release_Add", projectId);
      IEnumerable<PipelineArtifactSource> releaseArtifactSources = this.GetReleaseArtifactSources((IEnumerable<ArtifactSource>) release.LinkedArtifacts);
      this.BindReleaseTable(nameof (release), release);
      this.BindReleaseEnvironmentTable("releaseEnvironments", (IEnumerable<ReleaseEnvironment>) release.Environments);
      this.BindReleaseArtifactSourceTable("releaseArtifactSources", releaseArtifactSources);
      return this.GetReleaseObject(projectId);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public virtual Release GetRelease(Guid projectId, int releaseId, bool isDeleted = false)
    {
      this.PrepareStoredProcedure("Release.prc_Release_Get", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      return this.GetReleaseObject(projectId);
    }

    public virtual Release UpdateReleaseStatus(Guid projectId, Release release, Guid modifiedBy) => (Release) null;

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public virtual Release PatchRelease(
      Guid projectId,
      int releaseId,
      Guid modifiedBy,
      IList<ReleaseEnvironment> releaseEnvironments,
      string comment,
      ReleaseStatus? status = null,
      bool? keepForever = null)
    {
      return (Release) null;
    }

    public virtual Release CancelDeploymentOnEnvironment(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      Guid changedBy,
      string comment,
      bool addCommentAsDeploymentIssue,
      bool evaluateForCanceling)
    {
      return this.CancelDeploymentOnEnvironment(projectId, releaseId, releaseEnvironmentId, changedBy, comment, evaluateForCanceling);
    }

    public virtual Release CancelDeploymentOnEnvironment(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      Guid changedBy,
      string comment,
      bool evaluateForCanceling)
    {
      return this.CancelDeploymentOnEnvironment(projectId, releaseId, releaseEnvironmentId, changedBy, comment);
    }

    public virtual Release CancelDeploymentOnEnvironment(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      Guid changedBy,
      string comment)
    {
      this.PrepareStoredProcedure("Release.prc_CancelDeployment", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindGuid(nameof (changedBy), changedBy);
      this.BindInt("attempt", 0);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      return this.GetReleaseObject(projectId);
    }

    public virtual Release RejectReleaseEnvironment(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int attempt,
      Guid changedBy,
      string comment)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<int> SoftDeleteReleases(
      Guid projectId,
      int definitionId,
      DateTime maxReleaseModifiedTimeForSoftDelete,
      IEnumerable<DefinitionEnvironment> definitionEnvironments,
      Guid modifiedBy,
      int defaultNumberOfDaysToRetainRelease)
    {
      return (IEnumerable<int>) new List<int>();
    }

    public virtual void HardDeleteReleases(Guid projectId, IEnumerable<int> releaseIds)
    {
    }

    public virtual IEnumerable<PipelineArtifactSource> GetReleaseArtifactSources(
      Guid projectId,
      IEnumerable<int> releaseIds,
      string artifactTypeId)
    {
      return (IEnumerable<PipelineArtifactSource>) new List<PipelineArtifactSource>();
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public virtual IEnumerable<Release> ListReleases(
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
      this.PrepareStoredProcedure("Release.prc_Release_List", projectId);
      this.BindString(nameof (namePattern), namePattern, 256, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindByte(nameof (statusFilter), (byte) statusFilter);
      this.BindNullableDateTime(nameof (minCreatedTime), minCreatedTime);
      this.BindNullableDateTime(nameof (maxCreatedTime), maxCreatedTime);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseBinder(projectId));
        resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
        resultCollection.AddBinder<ReleaseArtifactDefinition>((ObjectBinder<ReleaseArtifactDefinition>) this.GetReleaseArtifactDefinitionBinder());
        List<Release> items1 = resultCollection.GetCurrent<Release>().Items;
        resultCollection.NextResult();
        List<ReleaseEnvironment> items2 = resultCollection.GetCurrent<ReleaseEnvironment>().Items;
        resultCollection.NextResult();
        List<ReleaseEnvironmentStep> items3 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
        resultCollection.NextResult();
        List<PipelineArtifactSource> items4 = resultCollection.GetCurrent<PipelineArtifactSource>().Items;
        resultCollection.NextResult();
        List<ReleaseArtifactDefinition> items5 = resultCollection.GetCurrent<ReleaseArtifactDefinition>().Items;
        foreach (Release release in items1)
        {
          int releaseId = release.Id;
          ReleaseSqlComponent.StitchReleaseObject(release, items2.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (env => env.ReleaseId == releaseId)), items3.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (step => step.ReleaseId == releaseId)), items4.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (source => source.ReleaseId == releaseId)), (IEnumerable<ReleaseArtifactDefinition>) items5, (IEnumerable<ReleaseDeployPhase>) null);
        }
        return (IEnumerable<Release>) items1;
      }
    }

    public virtual ReleaseDefinitionSummary GetReleaseSummaryForReleaseDefinition(
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
        List<PipelineArtifactSource> source1 = (List<PipelineArtifactSource>) null;
        List<ReleaseArtifactDefinition> releaseArtifactDefinitions = (List<ReleaseArtifactDefinition>) null;
        resultCollection.AddBinder<ShallowReference>((ObjectBinder<ShallowReference>) this.GetShallowReferenceBinder());
        resultCollection.AddBinder<DefinitionEnvironment>((ObjectBinder<DefinitionEnvironment>) this.GetDefinitionEnvironmentListBinder());
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseListBinder());
        resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentListBinder());
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        resultCollection.AddBinder<ReleaseToEnvironmentMap>((ObjectBinder<ReleaseToEnvironmentMap>) this.GetReleaseToEnvironmentMapBinder());
        if (includeArtifact)
        {
          resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
          resultCollection.AddBinder<ReleaseArtifactDefinition>((ObjectBinder<ReleaseArtifactDefinition>) this.GetReleaseArtifactDefinitionBinder());
        }
        ShallowReference shallowReference1 = resultCollection.GetCurrent<ShallowReference>().Items.Single<ShallowReference>();
        ReleaseDefinitionSummary definitionSummary = releaseDefinition;
        ReleaseDefinitionShallowReference shallowReference2 = new ReleaseDefinitionShallowReference();
        shallowReference2.Id = shallowReference1.Id;
        shallowReference2.Name = shallowReference1.Name;
        definitionSummary.ReleaseDefinition = shallowReference2;
        resultCollection.NextResult();
        List<DefinitionEnvironment> items1 = resultCollection.GetCurrent<DefinitionEnvironment>().Items;
        resultCollection.NextResult();
        List<Release> items2 = resultCollection.GetCurrent<Release>().Items;
        resultCollection.NextResult();
        List<ReleaseEnvironment> items3 = resultCollection.GetCurrent<ReleaseEnvironment>().Items;
        resultCollection.NextResult();
        List<ReleaseEnvironmentStep> items4 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
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
          resultCollection.NextResult();
          source1 = resultCollection.GetCurrent<PipelineArtifactSource>().Items;
          resultCollection.NextResult();
          releaseArtifactDefinitions = resultCollection.GetCurrent<ReleaseArtifactDefinition>().Items;
        }
        foreach (Release release in items2)
        {
          int releaseId = release.Id;
          ReleaseSqlComponent.StitchReleaseObject(release, items3.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (env => env.ReleaseId == releaseId)), items4.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (step => step.ReleaseId == releaseId)), source1 == null ? (IEnumerable<PipelineArtifactSource>) null : source1.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (source => source.ReleaseId == releaseId)), (IEnumerable<ReleaseArtifactDefinition>) releaseArtifactDefinitions, (IEnumerable<ReleaseDeployPhase>) null);
          releaseDefinition.Releases.Add(release);
        }
        return releaseDefinition;
      }
    }

    public virtual KeyValuePair<int, string> GetReleaseDefinitionFolderPathAndId(
      Guid projectId,
      int releaseId)
    {
      return new KeyValuePair<int, string>(this.GetRelease(projectId, releaseId).ReleaseDefinitionId, (string) null);
    }

    public virtual Release UpdateReleaseAndStepsStatus(
      Guid projectId,
      Release release,
      ReleaseStatus releaseStatus,
      ReleaseEnvironmentStepStatus statusFrom,
      ReleaseEnvironmentStepStatus statusTo,
      Guid modifiedBy)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      this.PrepareStoredProcedure("Release.prc_ReleaseEnvironmentStep_BulkUpdate", projectId);
      this.BindInt("releaseId", release.Id);
      this.BindByte(nameof (statusFrom), (byte) statusFrom);
      this.BindByte(nameof (statusTo), (byte) statusTo);
      this.ExecuteNonQuery();
      release.Status = releaseStatus;
      return this.UpdateReleaseStatus(projectId, release, modifiedBy);
    }

    public virtual void UpdateReleaseArtifactSources(
      Guid projectId,
      int releaseId,
      IEnumerable<PipelineArtifactSource> artifactSources)
    {
    }

    public virtual Release StartDraftRelease(
      Guid projectId,
      Release release,
      Guid modifiedBy,
      string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      return this.UpdateReleaseStatus(projectId, release, modifiedBy);
    }

    public virtual Release UpdateEnvironmentAndStepsStatus(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      ReleaseEnvironmentStatus environmentStatus,
      ReleaseEnvironmentStepStatus statusFrom,
      ReleaseEnvironmentStepStatus statusTo)
    {
      this.PrepareStoredProcedure("Release.prc_ReleaseEnvironmentStep_BulkUpdate", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindByte(nameof (statusFrom), (byte) statusFrom);
      this.BindByte(nameof (statusTo), (byte) statusTo);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
      return this.GetRelease(projectId, releaseId);
    }

    public virtual ReleaseDeployPhase AddReleaseDeployPhase(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int rank,
      int attempt,
      DeployPhaseTypes phaseType,
      Guid changedBy)
    {
      return new ReleaseDeployPhase()
      {
        Status = DeployPhaseStatus.NotStarted,
        ReleaseId = releaseId,
        ReleaseEnvironmentId = releaseEnvironmentId,
        Rank = rank,
        Attempt = attempt,
        PhaseType = phaseType
      };
    }

    public virtual ReleaseDeployPhase GetReleaseDeployPhase(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int releaseDeployPhaseId,
      Guid? planId)
    {
      throw new NotImplementedException();
    }

    public virtual ReleaseDeployPhase UpdateReleaseDeployPhase(
      Guid projectId,
      ReleaseDeployPhase releaseDeployPhase,
      DeploymentOperationStatus operationStatus,
      Guid changedBy)
    {
      throw new NotImplementedException();
    }

    public virtual IList<ReleaseDeployPhase> GetDeployPhasesForEnvironment(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int releaseDeployPhaseId,
      int attempt)
    {
      Guid runPlanId = this.GetRelease(projectId, releaseId).GetRunPlanId(releaseEnvironmentId, attempt);
      return (IList<ReleaseDeployPhase>) new List<ReleaseDeployPhase>()
      {
        new ReleaseDeployPhase()
        {
          PhaseType = DeployPhaseTypes.AgentBasedDeployment,
          RunPlanId = new Guid?(runPlanId)
        }
      };
    }

    public virtual IEnumerable<string> GetSourceBranches(Guid projectId, int releaseDefinitionId) => (IEnumerable<string>) new List<string>();

    public virtual Deployment CreateAndQueueDeployment(
      Guid projectId,
      int releaseDefinitionId,
      int releaseId,
      int definitionEnvironmentId,
      int releaseEnvironmentId,
      Guid requestedBy,
      Guid requestedFor,
      DeploymentReason reason,
      string comment)
    {
      this.UpdateReleaseEnvironmentStatus(projectId, releaseId, releaseEnvironmentId, ReleaseEnvironmentStatus.Undefined, ReleaseEnvironmentStatus.Queued, requestedBy, (ReleaseEnvironmentStatusChangeDetails) null, comment);
      return new Deployment()
      {
        Reason = DeploymentReason.None,
        ReleaseId = releaseId,
        ReleaseEnvironmentId = releaseEnvironmentId,
        RequestedBy = requestedBy,
        RequestedFor = requestedFor
      };
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public virtual Release UpdateEnvironmentAndDeploymentStatus(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int attempt,
      Guid changedBy,
      ReleaseEnvironmentStatus environmentStatus,
      DeploymentStatus deploymentStatus,
      ReleaseEnvironmentStatusChangeDetails changeDetails,
      DeploymentOperationStatus operationStatus = DeploymentOperationStatus.Undefined)
    {
      return this.UpdateReleaseEnvironmentStatus(projectId, releaseId, releaseEnvironmentId, ReleaseEnvironmentStatus.Undefined, environmentStatus, changedBy, changeDetails, (string) null, attempt);
    }

    public virtual Deployment UpdateDeploymentOperationStatus(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int attempt,
      DeploymentOperationStatus operationStatus,
      Guid changedBy)
    {
      return (Deployment) null;
    }

    public virtual ReleaseLogContainers GetReleaseLogContainers(
      Guid projectId,
      int releaseId,
      bool skipIsDeletedCheck)
    {
      this.PrepareStoredProcedure("Release.prc_ReleaseRunPlanIdRef_Get", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      List<ReleaseDeployPhaseRef> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDeployPhaseRef>((ObjectBinder<ReleaseDeployPhaseRef>) new ReleaseRunPlanIdRefBinder());
        items = resultCollection.GetCurrent<ReleaseDeployPhaseRef>().Items;
      }
      return new ReleaseLogContainers()
      {
        DeployPhases = (IList<ReleaseDeployPhaseRef>) items.Where<ReleaseDeployPhaseRef>((System.Func<ReleaseDeployPhaseRef, bool>) (e => e.PlanId != Guid.Empty)).ToList<ReleaseDeployPhaseRef>()
      };
    }

    public virtual ReleaseLogContainers GetReleaseLogContainers(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      bool skipIsDeletedCheck)
    {
      return this.GetReleaseLogContainers(projectId, releaseId, skipIsDeletedCheck);
    }

    private IList<RunPlanData> GetRunPlanDataForRelease(Guid projectId, int releaseId)
    {
      this.PrepareStoredProcedure("Release.prc_RunPlanId_GetFromReleaseId", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      List<RunPlanData> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<RunPlanData>((ObjectBinder<RunPlanData>) new RunPlanDataBinder());
        items = resultCollection.GetCurrent<RunPlanData>().Items;
      }
      return (IList<RunPlanData>) items ?? (IList<RunPlanData>) new List<RunPlanData>();
    }

    public virtual IEnumerable<int> GetReleasesForDefinition(
      Guid projectId,
      int definitionId,
      int top,
      int continuationToken)
    {
      this.PrepareStoredProcedure("Release.prc_QueryReleasesByReleaseDefinition", projectId);
      this.BindInt(nameof (definitionId), definitionId);
      this.BindInt(nameof (continuationToken), continuationToken);
      this.BindInt(nameof (top), top);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) this.GetReleaseIdListBinder());
        return (IEnumerable<int>) resultCollection.GetCurrent<int>().Items;
      }
    }

    public virtual IEnumerable<Release> RejectMultipleReleaseEnvironments(
      Guid projectId,
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      if (releaseEnvironmentSteps == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentSteps));
      List<Release> source = new List<Release>();
      foreach (ReleaseEnvironmentStep releaseEnvironmentStep in releaseEnvironmentSteps)
        source.Add(this.RejectReleaseEnvironment(projectId, releaseEnvironmentStep.ReleaseId, releaseEnvironmentStep.ReleaseEnvironmentId, releaseEnvironmentStep.TrialNumber, releaseEnvironmentStep.ActualApproverId, releaseEnvironmentStep.ApproverComment));
      return source.GroupBy<Release, int>((System.Func<Release, int>) (r => r.Id)).Select<IGrouping<int, Release>, Release>((System.Func<IGrouping<int, Release>, Release>) (g => g.First<Release>()));
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "it could be overloaded in future")]
    public virtual DeploymentIssueBinder GetDeploymentIssueBinder() => new DeploymentIssueBinder();

    public virtual IEnumerable<Release> ListHardDeleteReleaseCandidates(
      Guid projectId,
      DateTime? maxModifiedTime,
      int maxReleases,
      int continuationToken)
    {
      return this.ListReleases(projectId, string.Empty, 0, 0, ReleaseStatus.Undefined, ReleaseEnvironmentStatus.Undefined, (IEnumerable<Guid>) new List<Guid>(), new DateTime?(), new DateTime?(), maxModifiedTime, maxReleases, ReleaseQueryOrder.IdAscending, continuationToken, false, false, false, false, string.Empty, string.Empty, string.Empty, string.Empty, true, false);
    }

    public virtual IEnumerable<Release> QueryActiveReleases(
      Guid projectId,
      int releaseDefinitionId,
      int maxReleasesCount,
      int continuationToken,
      bool includeEnvironments,
      bool includeArtifacts,
      bool includeApprovals,
      bool includeManualInterventions,
      bool includeVariables,
      bool includeTags)
    {
      return (IEnumerable<Release>) new List<Release>();
    }
  }
}
