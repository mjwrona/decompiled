// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent21
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
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
  public class ReleaseDefinitionSqlComponent21 : ReleaseDefinitionSqlComponent20
  {
    protected static IEnumerable<ReleaseDefinition> SortDefinitionsOnQueryOrder(
      IList<ReleaseDefinition> definitions,
      ReleaseDefinitionQueryOrder queryOrder)
    {
      IEnumerable<ReleaseDefinition> releaseDefinitions = (IEnumerable<ReleaseDefinition>) null;
      switch (queryOrder)
      {
        case ReleaseDefinitionQueryOrder.IdAscending:
          releaseDefinitions = (IEnumerable<ReleaseDefinition>) definitions.OrderBy<ReleaseDefinition, int>((System.Func<ReleaseDefinition, int>) (rd => rd.Id));
          break;
        case ReleaseDefinitionQueryOrder.IdDescending:
          releaseDefinitions = (IEnumerable<ReleaseDefinition>) definitions.OrderByDescending<ReleaseDefinition, int>((System.Func<ReleaseDefinition, int>) (rd => rd.Id));
          break;
        case ReleaseDefinitionQueryOrder.NameAscending:
          releaseDefinitions = (IEnumerable<ReleaseDefinition>) definitions.OrderBy<ReleaseDefinition, string>((System.Func<ReleaseDefinition, string>) (rd => rd.Name));
          break;
        case ReleaseDefinitionQueryOrder.NameDescending:
          releaseDefinitions = (IEnumerable<ReleaseDefinition>) definitions.OrderByDescending<ReleaseDefinition, string>((System.Func<ReleaseDefinition, string>) (rd => rd.Name));
          break;
      }
      return releaseDefinitions;
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This can be overriden in derived class.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "By Design")]
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
        resultCollection.AddBinder<DefinitionTagData>((ObjectBinder<DefinitionTagData>) this.GetDefinitionTagBinder());
        ReleaseDefinition releaseDefinition = resultCollection.GetCurrent<ReleaseDefinition>().SingleOrDefault<ReleaseDefinition>();
        if (releaseDefinition == null)
          return (ReleaseDefinition) null;
        resultCollection.NextResult();
        foreach (DefinitionEnvironment definitionEnvironment in resultCollection.GetCurrent<DefinitionEnvironment>().Items)
          releaseDefinition.Environments.Add(definitionEnvironment);
        resultCollection.NextResult();
        List<DefinitionEnvironmentStep> items1 = resultCollection.GetCurrent<DefinitionEnvironmentStep>().Items;
        foreach (DefinitionEnvironment environment1 in (IEnumerable<DefinitionEnvironment>) releaseDefinition.Environments)
        {
          DefinitionEnvironment environment = environment1;
          foreach (DefinitionEnvironmentStep definitionEnvironmentStep in items1.Where<DefinitionEnvironmentStep>((System.Func<DefinitionEnvironmentStep, bool>) (s => s.DefinitionEnvironmentId == environment.Id)))
            environment.GetStepsForTests.Add(definitionEnvironmentStep);
        }
        IList<DeployPhase> deployPhases = this.GetDeployPhases(resultCollection);
        foreach (DefinitionEnvironment environment2 in (IEnumerable<DefinitionEnvironment>) releaseDefinition.Environments)
        {
          DefinitionEnvironment environment = environment2;
          foreach (DeployPhase deployPhase in deployPhases.Where<DeployPhase>((System.Func<DeployPhase, bool>) (p => p.DefinitionEnvironmentId == environment.Id)))
            environment.DeployPhases.Add(deployPhase);
        }
        foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) releaseDefinition.Environments)
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
          releaseDefinition.LinkedArtifacts.Add(ReleaseDefinitionSqlComponent.NormalizeLinkedArtifact(artifact, isDefaultToLatestArtifactVersionEnabled));
        }
        resultCollection.NextResult();
        foreach (ReleaseTriggerBase definitionTrigger in resultCollection.GetCurrent<ReleaseTriggerBase>().Items.GetConsolidatedReleaseDefinitionTriggers())
          releaseDefinition.Triggers.Add(ReleaseDefinitionSqlComponent13.NormalizeDefinitionTrigger(definitionTrigger));
        resultCollection.NextResult();
        resultCollection.GetCurrent<DefinitionTagData>().Items.ForEach((Action<DefinitionTagData>) (t => releaseDefinition.Tags.Add(t.Tag)));
        return releaseDefinition;
      }
    }

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
      this.BindByte("definitionType", (byte) 1);
      this.BindString(nameof (nameFilter), nameFilter, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindArtifactsFilter(sourceIdFilter, artifactTypeId);
      this.BindIsDeletedFilter(isDeleted);
      this.BindBoolean(nameof (includeEnvironments), includeEnvironments);
      this.BindBoolean(nameof (includeArtifacts), includeArtifacts);
      this.BindBoolean("includeLastRelease", includeLatestRelease);
      this.BindBoolean(nameof (includeTags), includeTags);
      this.BindIncludeTriggers(includeTriggers);
      this.BindMaxModifiedTime(maxModifiedTime);
      this.BindByte(nameof (queryOrder), (byte) queryOrder);
      this.BindContinuationToken(continuationToken);
      this.BindMaxReleaseDefinitionsCount(nameof (maxReleaseDefinitionsCount), maxReleaseDefinitionsCount);
      this.BindFolderPath(path);
      this.BindIsExactNameMatch(isExactNameMatch);
      this.BindStringTable(nameof (tagFilter), tagFilter);
      this.BindSearchTextContainsFolderName(searchTextContainsFolderName);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        List<DefinitionEnvironment> environments = new List<DefinitionEnvironment>();
        List<ReleaseDefinitionArtifactSourceMap> releaseDefinitionArtifactSourceData = new List<ReleaseDefinitionArtifactSourceMap>();
        List<ReleaseReference> releases = new List<ReleaseReference>();
        List<ReleaseTriggerBase> triggers = new List<ReleaseTriggerBase>();
        List<DefinitionTagData> tags = new List<DefinitionTagData>();
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
        if (includeLatestRelease)
        {
          resultCollection.AddBinder<ReleaseReference>((ObjectBinder<ReleaseReference>) this.GetReleaseReferenceBinder());
          resultCollection.NextResult();
          releases = resultCollection.GetCurrent<ReleaseReference>().Items;
        }
        if (includeTags)
        {
          resultCollection.AddBinder<DefinitionTagData>((ObjectBinder<DefinitionTagData>) this.GetDefinitionTagBinder());
          resultCollection.NextResult();
          tags = resultCollection.GetCurrent<DefinitionTagData>().Items;
        }
        this.StitchReleaseDefinitions((IList<ReleaseDefinition>) items, (IList<DefinitionEnvironment>) environments, (IList<ReleaseTriggerBase>) triggers, (IList<ReleaseReference>) releases, (IList<ReleaseDefinitionArtifactSourceMap>) releaseDefinitionArtifactSourceData, (IList<DefinitionTagData>) tags, isDefaultToLatestArtifactVersionEnabled);
        return ReleaseDefinitionSqlComponent21.SortDefinitionsOnQueryOrder((IList<ReleaseDefinition>) items, queryOrder);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "We may need to override this later")]
    protected virtual DefinitionTagBinder GetDefinitionTagBinder() => new DefinitionTagBinder((ReleaseManagementSqlResourceComponentBase) this);
  }
}
