// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent13
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Necessary to handle AT/DT mismatch")]
  public class ReleaseDefinitionSqlComponent13 : ReleaseDefinitionSqlComponent12
  {
    protected static ReleaseTriggerBase NormalizeDefinitionTrigger(ReleaseTriggerBase releaseTrigger)
    {
      if (releaseTrigger == null)
        return (ReleaseTriggerBase) null;
      if (releaseTrigger.TriggerType == ReleaseTriggerType.ArtifactSource)
        return (ReleaseTriggerBase) releaseTrigger.ToArtifactSourceTrigger();
      if (releaseTrigger.TriggerType == ReleaseTriggerType.SourceRepo)
        return (ReleaseTriggerBase) releaseTrigger.ToSourceRepoTrigger();
      return releaseTrigger.TriggerType == ReleaseTriggerType.ContainerImage ? (ReleaseTriggerBase) releaseTrigger.ToContainerImageTrigger() : releaseTrigger;
    }

    protected override void BindDeployPhases(ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      this.BindForeignKeyReferenceTable("environmentDeployPhasesLink", ReleaseDefinitionSqlComponent.GetParentChildReference<DefinitionEnvironment, DeployPhase>((IEnumerable<DefinitionEnvironment>) releaseDefinition.Environments, (System.Func<DefinitionEnvironment, IList<DeployPhase>>) (e => e.DeployPhases)));
      this.BindDeployPhaseTable(releaseDefinition.Environments.SelectMany<DefinitionEnvironment, DeployPhase>((System.Func<DefinitionEnvironment, IEnumerable<DeployPhase>>) (e => (IEnumerable<DeployPhase>) e.DeployPhases)));
    }

    protected override void AddDeployPhasesBinder(ResultCollection resultCollection)
    {
      if (resultCollection == null)
        throw new ArgumentNullException(nameof (resultCollection));
      resultCollection.AddBinder<DeployPhase>((ObjectBinder<DeployPhase>) this.GetDefinitionEnvironmentDeployPhaseBinder());
    }

    protected override IList<DeployPhase> GetDeployPhases(ResultCollection resultCollection)
    {
      if (resultCollection == null)
        throw new ArgumentNullException(nameof (resultCollection));
      resultCollection.NextResult();
      return (IList<DeployPhase>) resultCollection.GetCurrent<DeployPhase>().Items;
    }

    public override void BindDefinitionEnvironmentTable(IList<DefinitionEnvironment> environments) => this.BindDefinitionEnvironmentTable7("definitionEnvironments", (IEnumerable<DefinitionEnvironment>) environments);

    public virtual void BindDeployPhaseTable(IEnumerable<DeployPhase> environmentDeployPhases) => this.BindDefinitionEnvironmentDeployPhaseTable("definitionEnvironmentDeployPhases", environmentDeployPhases);

    protected virtual DefinitionEnvironmentDeployPhaseBinder GetDefinitionEnvironmentDeployPhaseBinder() => new DefinitionEnvironmentDeployPhaseBinder();

    protected override DefinitionEnvironmentBinder GetDefinitionEnvironmentBinder() => (DefinitionEnvironmentBinder) new DefinitionEnvironmentBinder2((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public override IEnumerable<ReleaseDefinition> ListReleaseDefinitions(
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
      if (projectId.Equals(Guid.Empty))
        this.PrepareStoredProcedure("Release.prc_QueryReleaseDefinitions");
      else
        this.PrepareStoredProcedure("Release.prc_QueryReleaseDefinitions", projectId);
      this.BindString(nameof (nameFilter), nameFilter, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindArtifactsFilter(sourceIdFilter, artifactTypeId);
      this.BindIsDeletedFilter(isDeleted);
      this.BindBoolean(nameof (includeEnvironments), includeEnvironments);
      this.BindBoolean(nameof (includeArtifacts), includeArtifacts);
      this.BindIncludeTriggers(includeTriggers);
      this.BindMaxModifiedTime(maxModifiedTime);
      this.BindSearchTextContainsFolderName(searchTextContainsFolderName);
      int result;
      if (!int.TryParse(continuationToken, out result))
        result = 0;
      this.BindMinReleaseDefinitionId("minReleaseDefinitionId", result);
      this.BindMaxReleaseDefinitionsCount(nameof (maxReleaseDefinitionsCount), maxReleaseDefinitionsCount);
      this.BindFolderPath(path);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        List<DefinitionEnvironment> environments = new List<DefinitionEnvironment>();
        List<ReleaseDefinitionArtifactSourceMap> releaseDefinitionArtifactSourceData = new List<ReleaseDefinitionArtifactSourceMap>();
        List<ReleaseTriggerBase> triggers = new List<ReleaseTriggerBase>();
        resultCollection.AddBinder<ReleaseDefinition>((ObjectBinder<ReleaseDefinition>) this.GetReleaseDefinitionBinder());
        List<ReleaseDefinition> items = resultCollection.GetCurrent<ReleaseDefinition>().Items;
        if (includeEnvironments)
        {
          resultCollection.AddBinder<DefinitionEnvironment>((ObjectBinder<DefinitionEnvironment>) this.GetDefinitionEnvironmentListBinder());
          resultCollection.NextResult();
          environments = resultCollection.GetCurrent<DefinitionEnvironment>().Items;
        }
        if (includeArtifacts)
        {
          resultCollection.AddBinder<ReleaseDefinitionArtifactSourceMap>((ObjectBinder<ReleaseDefinitionArtifactSourceMap>) this.GetReleaseDefinitionArtifactSourceBinder);
          resultCollection.NextResult();
          releaseDefinitionArtifactSourceData = resultCollection.GetCurrent<ReleaseDefinitionArtifactSourceMap>().Items;
        }
        if (includeTriggers)
        {
          this.AddTriggersBinder(resultCollection);
          triggers = this.GetReleaseTriggers(resultCollection).ToList<ReleaseTriggerBase>();
        }
        this.StitchReleaseDefinitions((IList<ReleaseDefinition>) items, (IList<DefinitionEnvironment>) environments, (IList<ReleaseTriggerBase>) triggers, (IList<ReleaseReference>) null, (IList<ReleaseDefinitionArtifactSourceMap>) releaseDefinitionArtifactSourceData, (IList<DefinitionTagData>) null, isDefaultToLatestArtifactVersionEnabled);
        return (IEnumerable<ReleaseDefinition>) items;
      }
    }

    protected void StitchReleaseDefinitions(
      IList<ReleaseDefinition> releaseDefinitions,
      IList<DefinitionEnvironment> environments,
      IList<ReleaseTriggerBase> triggers,
      IList<ReleaseReference> releases,
      IList<ReleaseDefinitionArtifactSourceMap> releaseDefinitionArtifactSourceData,
      IList<DefinitionTagData> tags,
      bool isDefaultToLatestArtifactVersionEnabled)
    {
      if (releaseDefinitions == null)
        throw new ArgumentNullException(nameof (releaseDefinitions));
      foreach (ReleaseDefinition releaseDefinition in (IEnumerable<ReleaseDefinition>) releaseDefinitions)
      {
        ReleaseDefinition definition = releaseDefinition;
        int definitionId = definition.Id;
        foreach (DefinitionEnvironment definitionEnvironment in environments.Where<DefinitionEnvironment>((System.Func<DefinitionEnvironment, bool>) (env => env.ProjectId.Equals(definition.ProjectId) && env.ReleaseDefinitionId == definitionId)))
          definition.Environments.Add(definitionEnvironment);
        definition.LastRelease = releases == null || releases.Count <= 0 ? (ReleaseReference) null : releases.FirstOrDefault<ReleaseReference>((System.Func<ReleaseReference, bool>) (rel => rel.ReleaseDefinitionId == definitionId));
        foreach (ArtifactSource linkedArtifact in this.GetLinkedArtifacts(definition, releaseDefinitionArtifactSourceData))
          definition.LinkedArtifacts.Add(ReleaseDefinitionSqlComponent.NormalizeLinkedArtifact(linkedArtifact, isDefaultToLatestArtifactVersionEnabled));
        foreach (ReleaseTriggerBase releaseTrigger in triggers.Where<ReleaseTriggerBase>((System.Func<ReleaseTriggerBase, bool>) (t => t.ProjectId.Equals(definition.ProjectId) && t.ReleaseDefinitionId == definitionId)))
          definition.Triggers.Add(ReleaseDefinitionSqlComponent13.NormalizeDefinitionTrigger(releaseTrigger));
        if (tags != null)
          definition.Tags.AddRange<string, IList<string>>((IEnumerable<string>) tags.Where<DefinitionTagData>((System.Func<DefinitionTagData, bool>) (t => t.DefinitionId == definitionId)).Select<DefinitionTagData, string>((System.Func<DefinitionTagData, string>) (t => t.Tag)).ToList<string>());
      }
    }

    protected virtual ReleaseDefinitionArtifactSourceBinder GetReleaseDefinitionArtifactSourceBinder => (ReleaseDefinitionArtifactSourceBinder) new ReleaseDefinitionArtifactSourceBinder2((ReleaseManagementSqlResourceComponentBase) this);

    protected virtual IEnumerable<ArtifactSource> GetLinkedArtifacts(
      ReleaseDefinition releaseDefinition,
      IList<ReleaseDefinitionArtifactSourceMap> releaseDefinitionArtifactSourceData)
    {
      IEnumerable<ReleaseDefinitionArtifactSourceMap> artifactSourceMaps = releaseDefinitionArtifactSourceData.Where<ReleaseDefinitionArtifactSourceMap>((System.Func<ReleaseDefinitionArtifactSourceMap, bool>) (rda => rda.ProjectId.Equals(releaseDefinition.ProjectId) && rda.ReleaseDefinitionId == releaseDefinition.Id));
      List<ArtifactSource> linkedArtifacts = new List<ArtifactSource>();
      int num = 1;
      foreach (ReleaseDefinitionArtifactSourceMap artifactSourceMap in artifactSourceMaps)
      {
        ArtifactSource artifactSource = new ArtifactSource()
        {
          SourceId = artifactSourceMap.SourceId,
          ArtifactTypeId = artifactSourceMap.ArtifactTypeId,
          Alias = artifactSourceMap.Alias,
          Id = num++,
          IsPrimary = artifactSourceMap.IsPrimary
        };
        if (artifactSourceMap.SourceData != null)
        {
          foreach (KeyValuePair<string, InputValue> keyValuePair in artifactSourceMap.SourceData)
            artifactSource.SourceData[keyValuePair.Key] = keyValuePair.Value;
        }
        linkedArtifacts.Add(artifactSource);
      }
      return (IEnumerable<ArtifactSource>) linkedArtifacts;
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This can be overriden in derived class.")]
    protected override ReleaseDefinition GetReleaseDefinitionObject(
      bool isDefaultToLatestArtifactVersionEnabled,
      bool includeLastRelease)
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDefinition>((ObjectBinder<ReleaseDefinition>) this.GetReleaseDefinitionBinder());
        resultCollection.AddBinder<DefinitionEnvironment>((ObjectBinder<DefinitionEnvironment>) this.GetDefinitionEnvironmentBinder());
        resultCollection.AddBinder<DefinitionEnvironmentStep>((ObjectBinder<DefinitionEnvironmentStep>) this.GetDefinitionEnvironmentStepBinder());
        this.AddDeployPhasesBinder(resultCollection);
        resultCollection.AddBinder<ArtifactSource>((ObjectBinder<ArtifactSource>) this.GetArtifactSourceBinder());
        resultCollection.AddBinder<ReleaseTriggerBase>((ObjectBinder<ReleaseTriggerBase>) this.GetReleaseTriggerBinder());
        ReleaseDefinition definitionObject = resultCollection.GetCurrent<ReleaseDefinition>().SingleOrDefault<ReleaseDefinition>();
        if (definitionObject == null)
          return (ReleaseDefinition) null;
        resultCollection.NextResult();
        foreach (DefinitionEnvironment definitionEnvironment in resultCollection.GetCurrent<DefinitionEnvironment>().Items)
          definitionObject.Environments.Add(definitionEnvironment);
        resultCollection.NextResult();
        List<DefinitionEnvironmentStep> items1 = resultCollection.GetCurrent<DefinitionEnvironmentStep>().Items;
        foreach (DefinitionEnvironment environment1 in (IEnumerable<DefinitionEnvironment>) definitionObject.Environments)
        {
          DefinitionEnvironment environment = environment1;
          foreach (DefinitionEnvironmentStep definitionEnvironmentStep in items1.Where<DefinitionEnvironmentStep>((System.Func<DefinitionEnvironmentStep, bool>) (s => s.DefinitionEnvironmentId == environment.Id)))
            environment.GetStepsForTests.Add(definitionEnvironmentStep);
        }
        IList<DeployPhase> deployPhases = this.GetDeployPhases(resultCollection);
        foreach (DefinitionEnvironment environment2 in (IEnumerable<DefinitionEnvironment>) definitionObject.Environments)
        {
          DefinitionEnvironment environment = environment2;
          foreach (DeployPhase deployPhase in deployPhases.Where<DeployPhase>((System.Func<DeployPhase, bool>) (p => p.DefinitionEnvironmentId == environment.Id)))
            environment.DeployPhases.Add(deployPhase);
        }
        foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) definitionObject.Environments)
        {
          IEnumerable<DefinitionEnvironmentStep> steps1 = environment.GetSteps(EnvironmentStepType.PreDeploy);
          environment.PreApprovalOptions = ReleaseDefinitionSqlComponent.GetNormalizedApprovalOptions(steps1, environment.PreApprovalOptions);
          IEnumerable<DefinitionEnvironmentStep> steps2 = environment.GetSteps(EnvironmentStepType.PostDeploy);
          environment.PostApprovalOptions = ReleaseDefinitionSqlComponent.GetNormalizedApprovalOptions(steps2, environment.PostApprovalOptions);
        }
        resultCollection.NextResult();
        List<ArtifactSource> items2 = resultCollection.GetCurrent<ArtifactSource>().Items;
        int num = 1;
        foreach (ArtifactSource artifact in items2)
        {
          artifact.Id = num++;
          definitionObject.LinkedArtifacts.Add(ReleaseDefinitionSqlComponent.NormalizeLinkedArtifact(artifact, isDefaultToLatestArtifactVersionEnabled));
        }
        resultCollection.NextResult();
        foreach (ReleaseTriggerBase releaseTrigger in resultCollection.GetCurrent<ReleaseTriggerBase>().Items)
          definitionObject.Triggers.Add(ReleaseDefinitionSqlComponent13.NormalizeDefinitionTrigger(releaseTrigger));
        return definitionObject;
      }
    }

    protected override void BindArtifactsFilter(
      IEnumerable<string> artifactSourceIdFilter,
      string artifactTypeId)
    {
      this.BindString("sourceIdFilter", artifactSourceIdFilter != null ? artifactSourceIdFilter.ElementAtOrDefault<string>(0) : (string) null, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("artifactTypeFilter", artifactTypeId, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Arguments are already validated at service layer.")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "reveiwed")]
    protected override ReleaseDefinition AddOrUpdateReleaseDefinition(
      ReleaseDefinition releaseDefinition,
      bool bindIds,
      string oldToken = null,
      string newToken = null)
    {
      IEnumerable<ArtifactSource> linkedArtifactSources = releaseDefinition.LinkedArtifacts.Select<ArtifactSource, ArtifactSource>((System.Func<ArtifactSource, ArtifactSource>) (e => e));
      IEnumerable<DefinitionEnvironmentStep> steps = releaseDefinition.Environments.SelectMany<DefinitionEnvironment, DefinitionEnvironmentStep>((System.Func<DefinitionEnvironment, IEnumerable<DefinitionEnvironmentStep>>) (e => (IEnumerable<DefinitionEnvironmentStep>) e.GetStepsForTests));
      IEnumerable<ForeignKeyReference> parentChildReference = ReleaseDefinitionSqlComponent.GetParentChildReference<DefinitionEnvironment, DefinitionEnvironmentStep>((IEnumerable<DefinitionEnvironment>) releaseDefinition.Environments, (System.Func<DefinitionEnvironment, IList<DefinitionEnvironmentStep>>) (e => e.GetStepsForTests));
      this.BindToReleaseDefinitionTable(releaseDefinition, bindIds);
      this.BindDefinitionEnvironmentTable(releaseDefinition.Environments);
      this.BindDefinitionEnvironmentSteps(steps);
      this.BindForeignKeyReferenceTable("definitionEnvironment_Step", parentChildReference);
      this.BindReleaseDefinitionArtifactSourceTable(linkedArtifactSources);
      this.BindReleaseTriggerTable(releaseDefinition);
      this.BindDeployPhases(releaseDefinition);
      this.BindFolderPath(releaseDefinition.Path);
      return this.GetReleaseDefinitionObject(false, false);
    }

    protected virtual void BindFolderPath(string path)
    {
    }

    protected virtual void BindDefinitionEnvironmentSteps(
      IEnumerable<DefinitionEnvironmentStep> steps)
    {
      this.BindDefinitionEnvironmentStepTable("definitionEnvironmentSteps", steps);
    }

    protected virtual void BindReleaseDefinitionArtifactSourceTable(
      IEnumerable<ArtifactSource> linkedArtifactSources)
    {
      this.BindReleaseDefinitionArtifactSourceTable2("definitionArtifactSources", linkedArtifactSources);
    }

    protected override void BindReleaseTriggerTable(ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      this.BindReleaseTriggerTable5("triggers", (IEnumerable<ReleaseTriggerBase>) releaseDefinition.Triggers);
    }

    protected virtual void BindReleaseTriggerTable(IEnumerable<ReleaseTriggerBase> triggers) => this.BindReleaseTriggerTable5(nameof (triggers), triggers);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected override ReleaseTriggerBinder GetReleaseTriggerBinder() => (ReleaseTriggerBinder) new ReleaseTriggerBinder2((ReleaseManagementSqlResourceComponentBase) this);

    protected virtual void BindSearchTextContainsFolderName(bool searchTextContainsFolderName)
    {
    }
  }
}
