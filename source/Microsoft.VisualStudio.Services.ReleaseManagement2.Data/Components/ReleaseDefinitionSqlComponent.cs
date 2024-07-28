// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SupportedSqlAccessIntent(SqlAccessIntent.ReadWrite | SqlAccessIntent.ReadOnly, null)]
  public class ReleaseDefinitionSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[40]
    {
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent>(1),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent2>(2),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent4>(4),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent5>(5),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent6>(6),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent7>(7),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent8>(8),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent9>(9),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent10>(10),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent11>(11),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent12>(12),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent13>(13),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent14>(14),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent15>(15),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent16>(16),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent17>(17),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent18>(18),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent19>(19),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent20>(20),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent21>(21),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent22>(22),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent23>(23),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent24>(24),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent25>(25),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent26>(26),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent27>(27),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent28>(28),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent29>(29),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent30>(30),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent31>(31),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent32>(32),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent33>(33),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent34>(34),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent35>(35),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent36>(36),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent37>(37),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent38>(38),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent39>(39),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent40>(40),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionSqlComponent41>(41)
    }, "ReleaseManagementReleaseDefinition", "ReleaseManagement");

    protected static ApprovalOptions GetNormalizedApprovalOptions(
      IEnumerable<DefinitionEnvironmentStep> approvalSteps,
      ApprovalOptions existingApprovalOptions)
    {
      if (existingApprovalOptions != null)
        return existingApprovalOptions;
      DefinitionEnvironmentStep definitionEnvironmentStep = approvalSteps.FirstOrDefault<DefinitionEnvironmentStep>();
      return definitionEnvironmentStep != null && !definitionEnvironmentStep.IsAutomated ? new ApprovalOptions() : existingApprovalOptions;
    }

    protected static ArtifactSource NormalizeLinkedArtifact(
      ArtifactSource artifact,
      bool isDefaultToLatestArtifactVersionEnabled)
    {
      if (artifact == null)
        return (ArtifactSource) null;
      InputValue inputValue;
      if (isDefaultToLatestArtifactVersionEnabled && artifact.SourceData != null && (!artifact.SourceData.TryGetValue("defaultVersionType", out inputValue) || string.IsNullOrEmpty(inputValue?.Value)))
        artifact.SourceData["defaultVersionType"] = new InputValue()
        {
          Value = ArtifactVersionsUtility.GetDefaultIdForDefaultVersionType(artifact.ArtifactTypeId),
          DisplayValue = ArtifactVersionsUtility.GetDefaultNameForDefaultVersionType(artifact.ArtifactTypeId)
        };
      if (!string.IsNullOrWhiteSpace(artifact.Alias))
        return artifact;
      string str = (string) null;
      InputValue definitionsData = artifact.DefinitionsData;
      if (definitionsData != null)
        str = definitionsData.DisplayValue;
      artifact.Alias = string.IsNullOrWhiteSpace(str) ? string.Empty : str;
      return artifact;
    }

    public virtual bool SupportsDataspaceMapper() => false;

    public ReleaseDefinition AddReleaseDefinition(
      Guid projectId,
      ReleaseDefinition releaseDefinition)
    {
      this.PrepareForAuditingAction(ReleaseAuditConstants.ReleasePipelineCreated, projectId: projectId, excludeSqlParameters: true);
      this.PrepareStoredProcedure("Release.prc_ReleaseDefinition_Add", projectId);
      return this.AddOrUpdateReleaseDefinition(releaseDefinition, false);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "reviewed")]
    public virtual ReleaseDefinition UpdateReleaseDefinition(
      Guid projectId,
      ReleaseDefinition releaseDefinition,
      string oldToken = null,
      string newToken = null)
    {
      this.PrepareStoredProcedure("Release.prc_ReleaseDefinition_Update", projectId);
      return this.AddOrUpdateReleaseDefinition(releaseDefinition, true);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public virtual ReleaseDefinition GetReleaseDefinition(
      Guid projectId,
      int releaseDefinitionId,
      bool includeDeleted = false,
      bool isDefaultToLatestArtifactVersionEnabled = false,
      bool includeLastRelease = false)
    {
      this.PrepareStoredProcedure("Release.prc_ReleaseDefinition_Get", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      return this.GetReleaseDefinitionObject(isDefaultToLatestArtifactVersionEnabled, includeLastRelease);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public virtual IEnumerable<ReleaseDefinition> ListReleaseDefinitions(
      Guid projectId,
      string nameFilter,
      IEnumerable<string> sourceIdFilter = null,
      string artifactTypeId = null,
      bool isDeleted = false,
      bool includeEnvironments = false,
      bool includeArtifacts = false,
      bool includeTriggers = false,
      bool includeLatestRelease = true,
      DateTime? maxModifiedTime = null,
      ReleaseDefinitionQueryOrder queryOrder = ReleaseDefinitionQueryOrder.IdAscending,
      string continuationToken = null,
      int maxReleaseDefinitionsCount = 0,
      string path = null,
      bool isExactNameMatch = false,
      bool includeTags = false,
      IEnumerable<string> tagFilter = null,
      bool includeVariables = false,
      IEnumerable<int> definitionIdFilter = null,
      bool isDefaultToLatestArtifactVersionEnabled = false,
      bool searchTextContainsFolderName = false)
    {
      return (IEnumerable<ReleaseDefinition>) new List<ReleaseDefinition>();
    }

    public virtual IEnumerable<ReleaseDefinition> ListAllReleaseDefinitions(Guid projectId) => this.ListReleaseDefinitions(projectId, (string) null);

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public virtual void SoftDeleteReleaseDefinition(
      Guid projectId,
      Guid requestorId,
      int releaseDefinitionId = 0,
      string comment = null,
      bool forceDelete = false)
    {
    }

    public virtual void HardDeleteReleaseDefinition(Guid projectId, int releaseDefinitionId) => this.DeleteReleaseDefinition(projectId, releaseDefinitionId, Guid.Empty);

    public virtual void UndeleteReleaseDefinition(
      Guid projectId,
      Guid requestorId,
      int releaseDefinitionId,
      string comment)
    {
    }

    public virtual void DeleteReleaseDefinition(
      Guid projectId,
      int releaseDefinitionId,
      Guid requestorId)
    {
    }

    public virtual void UpdateReleaseDefinitionTriggers(
      Guid projectId,
      int releaseDefinitionId,
      ReleaseDefinition definition)
    {
    }

    public virtual void UpdateDefinitionEnvironmentSchedules(
      Guid projectId,
      int releaseDefinitionId,
      DefinitionEnvironment definitionEnvironment)
    {
    }

    public virtual IDictionary<int, string> GetFolderPaths(
      Guid projectId,
      IEnumerable<int> definitionIds)
    {
      return (IDictionary<int, string>) new Dictionary<int, string>();
    }

    public virtual IEnumerable<ShallowReference> QueryAllDefinitionsForDataProvider(Guid projectId) => this.ListReleaseDefinitions(projectId, string.Empty, artifactTypeId: string.Empty, includeEnvironments: true, includeLatestRelease: false, continuationToken: "0").Select<ReleaseDefinition, ShallowReference>((Func<ReleaseDefinition, ShallowReference>) (e => new ShallowReference()
    {
      Id = e.Id,
      Name = e.Name
    }));

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public virtual IEnumerable<int> ListHardDeleteReleaseDefinitionCandidates(
      Guid projectId,
      DateTime? maxModifiedTime = null,
      int maxReleaseDefinitionsCount = 0,
      int continuationToken = 0)
    {
      return this.ListReleaseDefinitions(projectId, string.Empty, isDeleted: true, maxModifiedTime: maxModifiedTime, continuationToken: continuationToken.ToString((IFormatProvider) CultureInfo.InvariantCulture), maxReleaseDefinitionsCount: maxReleaseDefinitionsCount).ToList<ReleaseDefinition>().Select<ReleaseDefinition, int>((Func<ReleaseDefinition, int>) (x => x.Id));
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for complexity of type")]
    protected static IEnumerable<ForeignKeyReference> GetParentChildReference<TParent, TChild>(
      IEnumerable<TParent> parents,
      Func<TParent, IList<TChild>> getChildren)
      where TParent : ForeignKeyModelBase
      where TChild : ForeignKeyModelBase
    {
      return (IEnumerable<ForeignKeyReference>) parents.SelectMany<TParent, TChild, ForeignKeyReference>((Func<TParent, IEnumerable<TChild>>) (parent => (IEnumerable<TChild>) getChildren(parent)), (Func<TParent, TChild, ForeignKeyReference>) ((parent, child) => new ForeignKeyReference()
      {
        ChildId = child.GuidId,
        ParentId = parent.GuidId
      })).ToList<ForeignKeyReference>();
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Override of base class method.")]
    protected virtual ReleaseDefinitionBinder GetReleaseDefinitionBinder() => new ReleaseDefinitionBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual DefinitionEnvironmentBinder GetDefinitionEnvironmentBinder() => (DefinitionEnvironmentBinder) new DefinitionEnvironmentBinder1((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseReferenceBinder GetReleaseReferenceBinder() => new ReleaseReferenceBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ArtifactSourceBinder GetArtifactSourceBinder() => new ArtifactSourceBinder();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual DefinitionEnvironmentStepBinder GetDefinitionEnvironmentStepBinder() => new DefinitionEnvironmentStepBinder();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseTriggerBinder GetReleaseTriggerBinder() => new ReleaseTriggerBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseDefinitionArtifactBinder GetReleaseDefinitionArtifactBinder() => new ReleaseDefinitionArtifactBinder();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual DefinitionEnvironmentListBinder GetDefinitionEnvironmentListBinder() => new DefinitionEnvironmentListBinder((ReleaseManagementSqlResourceComponentBase) this);

    protected virtual void BindDeployPhases(ReleaseDefinition releaseDefinition)
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "reviewed")]
    protected virtual ShallowReferenceBinder GetShallowReferenceBinder() => new ShallowReferenceBinder();

    protected virtual void BindSecurityTokens(string oldToken, string newToken)
    {
    }

    protected virtual void BindReleaseTriggerTable(ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      this.BindReleaseTriggerTable3("triggers", (IEnumerable<ReleaseTriggerBase>) releaseDefinition.Triggers, (IEnumerable<ArtifactSource>) releaseDefinition.LinkedArtifacts);
    }

    [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Arguments are already validated at service layer.")]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "reviewed")]
    protected virtual ReleaseDefinition AddOrUpdateReleaseDefinition(
      ReleaseDefinition releaseDefinition,
      bool bindIds,
      string oldToken = null,
      string newToken = null)
    {
      throw new NotImplementedException();
    }

    protected virtual void BindToReleaseDefinitionTable(
      ReleaseDefinition releaseDefinition,
      bool bindIds)
    {
      this.BindReleaseDefinitionTable(nameof (releaseDefinition), releaseDefinition);
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This can be overriden in derived class.")]
    protected virtual ReleaseDefinition GetReleaseDefinitionObject(
      bool isDefaultToLatestArtifactVersionEnabled,
      bool includeLastRelease)
    {
      throw new NotImplementedException();
    }

    protected virtual IList<DeployPhase> GetDeployPhases(ResultCollection resultCollection) => (IList<DeployPhase>) new List<DeployPhase>();

    protected virtual void AddDeployPhasesBinder(ResultCollection resultCollection)
    {
    }

    public virtual IList<RedeployTriggerEnvironmentDGPhaseData> GetRedeployTriggerEnvironmentDGPhaseData(
      Guid projectId,
      IEnumerable<int> deploymentGroupIds)
    {
      throw new NotImplementedException();
    }

    public virtual IList<ReleaseTriggerBase> GetReleaseTriggersWithMatchingRepositoryData(
      ReleaseTriggerType triggerType,
      string repositoryName,
      string connectionId)
    {
      return (IList<ReleaseTriggerBase>) new List<ReleaseTriggerBase>();
    }
  }
}
