// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class DeploymentSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[18]
    {
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent>(1),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent2>(2),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent3>(3),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent4>(4),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent5>(5),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent6>(6),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent7>(7),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent8>(8),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent9>(9),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent10>(10),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent11>(11),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent12>(12),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent13>(13),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent14>(14),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent15>(15),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent16>(16),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent17>(17),
      (IComponentCreator) new ComponentCreator<DeploymentSqlComponent18>(18)
    }, "ReleaseManagementDeployments", "ReleaseManagement");

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected void StitchDeploymentObject(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment,
      IEnumerable<ReleaseEnvironmentStep> steps,
      IEnumerable<PipelineArtifactSource> releaseArtifactSources)
    {
      if (releaseArtifactSources == null)
        releaseArtifactSources = (IEnumerable<PipelineArtifactSource>) new List<PipelineArtifactSource>();
      foreach (ArtifactSource artifact in this.GetArtifacts(releaseArtifactSources))
        deployment.LinkedArtifacts.Add(artifact);
      foreach (ReleaseEnvironmentStep releaseEnvironmentStep in steps.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (step => step.ReleaseEnvironmentId == deployment.ReleaseEnvironmentId)))
      {
        releaseEnvironmentStep.ReleaseDefinitionName = deployment.ReleaseDefinitionName;
        releaseEnvironmentStep.ReleaseDefinitionPath = deployment.ReleaseDefinitionPath;
        releaseEnvironmentStep.ReleaseName = deployment.ReleaseName;
        releaseEnvironmentStep.ReleaseEnvironmentName = deployment.ReleaseEnvironmentName;
        deployment.Steps.Add(releaseEnvironmentStep);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual DeploymentApiBinder GetDeploymentApiBinder() => new DeploymentApiBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseArtifactSourceBinder GetReleaseArtifactSourceBinder() => (ReleaseArtifactSourceBinder) new ReleaseArtifactSourceBinder5((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseEnvironmentStepBinder GetReleaseEnvironmentStepBinder() => new ReleaseEnvironmentStepBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Will be overridden in derived classes.")]
    protected virtual DeploymentGateBinder GetDeploymentGateBinder() => new DeploymentGateBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Will be overridden in derived classes.")]
    protected virtual ReleaseEnvironmentSnapshotDeltaBinder GetReleaseEnvironmentSnapshotDeltaBinder() => new ReleaseEnvironmentSnapshotDeltaBinder();

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By design.")]
    protected IEnumerable<ArtifactSource> GetArtifacts(
      IEnumerable<PipelineArtifactSource> releaseArtifactSources)
    {
      if (releaseArtifactSources == null)
        return (IEnumerable<ArtifactSource>) null;
      List<ArtifactSource> artifacts = new List<ArtifactSource>();
      foreach (PipelineArtifactSource releaseArtifactSource in releaseArtifactSources)
      {
        if (releaseArtifactSource != null)
          releaseArtifactSource.SourceData["version"] = releaseArtifactSource.Version;
        artifacts.Add((ArtifactSource) releaseArtifactSource);
      }
      return (IEnumerable<ArtifactSource>) artifacts;
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> QueryDeploymentsFailingSince(
      Guid projectId,
      IList<DefinitionEnvironmentReference> environments,
      string artifactTypeId,
      string artifactSourceId,
      string sourceBranch,
      DeploymentExpands deploymentExpands = DeploymentExpands.All)
    {
      return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>();
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> ListDeployments(
      Guid projectId,
      int releaseDefinitionId,
      int releaseDefinitionEnvironmentId,
      int releaseId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus deploymentStatus,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus operationStatus,
      IList<Guid> createdByIds,
      bool latestAttemptsOnly,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder queryOrder,
      int continuationToken,
      bool isDeleted,
      DateTime? minModifiedTime,
      DateTime? maxModifiedTime,
      IList<Guid> createdForIds,
      int maxDeployments,
      string branchName,
      DateTime? minStartedTime,
      DateTime? maxStartedTime,
      string artifactTypeId = "",
      string sourceId = "",
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason releaseReason = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.None)
    {
      List<DefinitionEnvironmentReference> environmentReferenceList = new List<DefinitionEnvironmentReference>();
      environmentReferenceList.Add(new DefinitionEnvironmentReference()
      {
        ReleaseDefinitionId = releaseDefinitionId,
        DefinitionEnvironmentId = releaseDefinitionEnvironmentId
      });
      Guid projectId1 = projectId;
      List<DefinitionEnvironmentReference> environments = environmentReferenceList;
      int num1 = (int) deploymentStatus;
      int operationStatus1 = (int) operationStatus;
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      IList<Guid> createdByIds1 = createdByIds;
      IList<Guid> guidList = createdForIds;
      int num2 = latestAttemptsOnly ? 1 : 0;
      int queryOrder1 = (int) queryOrder;
      int continuationToken1 = continuationToken;
      int maxDeployments1 = maxDeployments;
      int num3 = isDeleted ? 1 : 0;
      DateTime? minModifiedTime1 = minModifiedTime;
      DateTime? maxModifiedTime1 = maxModifiedTime;
      IList<Guid> createdForIds1 = guidList;
      DateTime? minStartedTime1 = minStartedTime;
      DateTime? maxStartedTime1 = maxStartedTime;
      return this.ListDeployments(projectId1, (IList<DefinitionEnvironmentReference>) environments, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus) num1, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus) operationStatus1, empty1, empty2, (IList<string>) null, createdByIds1, num2 != 0, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder) queryOrder1, continuationToken1, maxDeployments1, num3 != 0, false, minModifiedTime1, maxModifiedTime1, createdForIds1, minStartedTime1, maxStartedTime1);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "required")]
    public virtual DeploymentsAndDefinitions GetDeploymentsAndDefinitionsAcrossProjects(
      Guid identityId,
      HashSet<KeyValuePair<int, int>> dataspaceDefinitionIdKeyValuePairs,
      int maxItems = 30)
    {
      return new DeploymentsAndDefinitions();
    }

    public virtual DeploymentsAndDefinitions GetMyDeploymentsAndDefinitionsAcrossProjects() => new DeploymentsAndDefinitions();

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> GetDeploymentsForMultipleEnvironments(
      Guid projectId,
      IList<DefinitionEnvironmentReference> environments,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus deploymentStatus,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus operationStatus,
      string artifactSourceId,
      string artifactTypeId,
      string sourceBranch,
      IList<string> artifactVersions,
      int deploymentsPerEnvironment,
      bool isDeleted,
      DeploymentExpands deploymentExpands = DeploymentExpands.All)
    {
      return this.ListDeployments(projectId, environments, deploymentStatus, operationStatus, artifactSourceId, artifactTypeId, artifactVersions, (IList<Guid>) null, true, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder.IdDescending, 0, 1, isDeleted, true, new DateTime?(), new DateTime?(), (IList<Guid>) null, new DateTime?(), new DateTime?());
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "reviewed.")]
    protected IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> GetDeployments(
      bool includeApprovals,
      bool includeArtifacts)
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return this.GetDeployments(resultCollection, includeApprovals, includeArtifacts);
    }

    protected IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> GetDeployments(
      ResultCollection resultCollection,
      bool includeApprovals,
      bool includeArtifacts)
    {
      if (resultCollection == null)
        throw new ArgumentNullException(nameof (resultCollection));
      List<ReleaseEnvironmentStep> source1 = new List<ReleaseEnvironmentStep>();
      List<PipelineArtifactSource> source2 = new List<PipelineArtifactSource>();
      resultCollection.AddBinder<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>((ObjectBinder<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>) this.GetDeploymentApiBinder());
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> items = resultCollection.GetCurrent<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>().Items;
      if (includeApprovals)
      {
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        resultCollection.NextResult();
        source1 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
      }
      if (includeArtifacts)
      {
        resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
        resultCollection.NextResult();
        source2 = resultCollection.GetCurrent<PipelineArtifactSource>().Items;
      }
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment1 in items)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment = deployment1;
        IEnumerable<ReleaseEnvironmentStep> steps = source1.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (step => step.ProjectId.Equals(deployment.ProjectId) && step.ReleaseId == deployment.ReleaseId && step.ReleaseEnvironmentId == deployment.ReleaseEnvironmentId && step.TrialNumber == deployment.Attempt));
        IEnumerable<PipelineArtifactSource> releaseArtifactSources = source2.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (source => source.ProjectId.Equals(deployment.ProjectId) && source.ReleaseId == deployment.ReleaseId));
        this.StitchDeploymentObject(deployment, steps, releaseArtifactSources);
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>) items;
    }

    private IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> ListDeployments(
      Guid projectId,
      IList<DefinitionEnvironmentReference> environments,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus deploymentStatus,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus operationStatus,
      string artifactSourceId,
      string artifactTypeId,
      IList<string> artifactVersions,
      IList<Guid> createdByIds,
      bool latestAttemptsOnly,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder queryOrder,
      int continuationToken,
      int maxDeployments,
      bool isDeleted,
      bool groupByEnvironment,
      DateTime? minModifiedTime,
      DateTime? maxModifiedTime,
      IList<Guid> createdForIds,
      DateTime? minStartedTime,
      DateTime? maxStartedTime)
    {
      this.PrepareStoredProcedure("Release.prc_QueryDeployments", projectId);
      this.BindDeploymentsEnvironmentsFilterTable("releaseDefinitionAndEnvironmentIds", (IEnumerable<DefinitionEnvironmentReference>) environments);
      this.BindNullableInt(nameof (deploymentStatus), new int?((int) deploymentStatus));
      this.BindNullableInt(nameof (operationStatus), new int?((int) operationStatus));
      this.BindString(nameof (artifactSourceId), artifactSourceId, 256, true, SqlDbType.NVarChar);
      this.BindString(nameof (artifactTypeId), artifactTypeId, 256, true, SqlDbType.NVarChar);
      this.BindStringTable("artifactVersionsFilter", (IEnumerable<string>) artifactVersions, true, 256);
      this.BindGuidTable("createdByIdsTable", (IEnumerable<Guid>) createdByIds);
      this.BindNullableBoolean(nameof (latestAttemptsOnly), new bool?(latestAttemptsOnly));
      this.BindNullableByte(nameof (queryOrder), (byte) queryOrder, (byte) 0);
      this.BindNullableInt("deploymentContinuationToken", new int?(continuationToken));
      this.BindMaxDeployments(maxDeployments);
      this.BindNullableBoolean(nameof (isDeleted), new bool?(isDeleted));
      this.BindGroupByEnvironment(groupByEnvironment);
      this.BindModifiedTime(minModifiedTime, maxModifiedTime);
      this.BindCreatedForIds(createdForIds);
      return this.GetDeployments();
    }

    public virtual DeploymentGate AddDeploymentGate(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int deploymentId,
      int stepId,
      EnvironmentStepType gateType)
    {
      return new DeploymentGate();
    }

    public virtual DeploymentGate UpdateDeploymentGate(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int stepId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.GateStatus status,
      Guid? runPlanId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus operationStatus,
      Guid changedBy)
    {
      return new DeploymentGate();
    }

    public virtual DeploymentGate GetDeploymentGate(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int stepId)
    {
      return new DeploymentGate();
    }

    public virtual DeploymentGate HandleGreenlightingStabilizationCompletion(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int stepId,
      Guid runPlanId)
    {
      return new DeploymentGate();
    }

    public virtual DeploymentGate UpdateGreenlightingSucceedingSince(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int stepId,
      Guid runPlanId,
      DateTime? succeedingSince)
    {
      return new DeploymentGate();
    }

    public virtual ReleaseEnvironmentSnapshotDelta AddReleaseEnvironmentSnapshotDelta(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int deploymentId,
      IEnumerable<DeploymentGroupPhaseDelta> deploymentGroupPhaseDelta,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables)
    {
      throw new NotImplementedException();
    }

    public virtual ReleaseEnvironmentSnapshotDelta GetReleaseEnvironmentSnapshotDelta(
      Guid projectId,
      int releaseId,
      int deploymentId)
    {
      throw new NotImplementedException();
    }

    public virtual IList<DeploymentIssue> GetDeploymentIssues(
      Guid projectId,
      int releaseId,
      int deploymentId)
    {
      throw new NotImplementedException();
    }

    public virtual IList<DeploymentIssue> AddDeploymentIssues(
      Guid projectId,
      int releaseId,
      int deploymentId,
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Issue> issues)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> GetLatestDeploymentsByReleaseDefinitions(
      Guid projectId,
      int releaseDefinitionsCount,
      IEnumerable<int> releaseDefinitionIdsToExclude)
    {
      return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>();
    }

    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> GetLastDeploymentForReleaseDefinitions(
      Guid projectId,
      IEnumerable<int> releaseDefinitionIds)
    {
      return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>();
    }

    public virtual DeploymentGate UpdateIgnoredGates(
      Guid projectId,
      int releaseId,
      int stepId,
      IEnumerable<string> gatesToIgnore,
      string beforeGatesIgnored,
      string afterGatesIgnored,
      string comment,
      Guid changedBy,
      bool markProcessed)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> GetDeploymentsCreatedByAnIdentity(
      Guid projectId,
      Guid createdById,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus deploymentStatus,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus operationStatus,
      int maxDeployments,
      DateTime minQueuedTime,
      DateTime maxQueuedTime)
    {
      return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>();
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for complexity of type")]
    public virtual IEnumerable<DeploymentAttemptData> GetDeploymentsByReasonForMultipleEnvironments(
      Guid projectId,
      HashSet<KeyValuePair<int, int>> definitionEnvironmentReleaseDefinitionIdKeyValuePairs,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentReason deploymentReasonFilter,
      int deploymentsPerEnvironment,
      bool isDeleted)
    {
      throw new NotImplementedException();
    }

    protected virtual void BindCreatedForIds(IList<Guid> createdForIds)
    {
    }

    protected virtual void BindMaxDeployments(int maxDeployments) => this.BindNullableInt("maxDeploymentsPerEnv", new int?(maxDeployments));

    protected virtual void BindGroupByEnvironment(bool groupByEnvironment)
    {
    }

    protected virtual void BindModifiedTime(DateTime? minModifiedTime, DateTime? maxModifiedTime)
    {
    }

    protected IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> GetDeployments() => this.GetDeployments(true, true);
  }
}
