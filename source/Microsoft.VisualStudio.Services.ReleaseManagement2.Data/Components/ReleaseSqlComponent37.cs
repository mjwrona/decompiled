// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent37
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent37 : ReleaseSqlComponent36
  {
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Cannot avoid this as it is required to construct the basic Release object")]
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
      this.ReleaseDefinitionSummaryBindDefinitionEnvironmentIdsFilter(definitionEnvironmentIdsFilterList);
      ReleaseDefinitionSummary releaseDefinition = new ReleaseDefinitionSummary();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        List<ReleaseDeployPhase> releaseDeployPhaseList = new List<ReleaseDeployPhase>();
        List<Deployment> deploymentList = new List<Deployment>();
        List<PipelineArtifactSource> source1 = new List<PipelineArtifactSource>();
        List<ManualIntervention> manualInterventionList = new List<ManualIntervention>();
        resultCollection.AddBinder<ReleaseDefinitionShallowReference>((ObjectBinder<ReleaseDefinitionShallowReference>) this.GetReleaseDefinitionShallowReferenceBinder());
        ReleaseDefinitionShallowReference shallowReference = resultCollection.GetCurrent<ReleaseDefinitionShallowReference>().Items.Single<ReleaseDefinitionShallowReference>();
        releaseDefinition.ReleaseDefinition = shallowReference;
        resultCollection.AddBinder<DefinitionEnvironment>((ObjectBinder<DefinitionEnvironment>) this.GetDefinitionEnvironmentListBinder());
        resultCollection.NextResult();
        List<DefinitionEnvironment> items1 = resultCollection.GetCurrent<DefinitionEnvironment>().Items;
        resultCollection.AddBinder<ReleaseToEnvironmentMap>((ObjectBinder<ReleaseToEnvironmentMap>) this.GetReleaseToEnvironmentMapBinder());
        resultCollection.NextResult();
        List<ReleaseToEnvironmentMap> items2 = resultCollection.GetCurrent<ReleaseToEnvironmentMap>().Items;
        foreach (ReleaseToEnvironmentMap toEnvironmentMap in items2)
          releaseDefinition.ReleaseToEnvironmentMap.Add(toEnvironmentMap);
        foreach (int num in items1.Select<DefinitionEnvironment, int>((System.Func<DefinitionEnvironment, int>) (e => e.Id)).Except<int>(items2.Select<ReleaseToEnvironmentMap, int>((System.Func<ReleaseToEnvironmentMap, int>) (map => map.DefinitionEnvironmentId)).Distinct<int>()))
          releaseDefinition.ReleaseToEnvironmentMap.Add(new ReleaseToEnvironmentMap()
          {
            DefinitionEnvironmentId = num
          });
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseBinder(projectId));
        resultCollection.NextResult();
        List<Release> items3 = resultCollection.GetCurrent<Release>().Items;
        resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
        resultCollection.NextResult();
        List<ReleaseEnvironment> items4 = resultCollection.GetCurrent<ReleaseEnvironment>().Items;
        resultCollection.AddBinder<DefinitionEnvironment>((ObjectBinder<DefinitionEnvironment>) this.GetDefinitionEnvironmentScheduleBinder());
        resultCollection.NextResult();
        List<DefinitionEnvironment> items5 = resultCollection.GetCurrent<DefinitionEnvironment>().Items;
        resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
        resultCollection.NextResult();
        List<ReleaseDeployPhase> items6 = resultCollection.GetCurrent<ReleaseDeployPhase>().Items;
        resultCollection.AddBinder<Deployment>((ObjectBinder<Deployment>) this.GetDeploymentBinder());
        resultCollection.NextResult();
        List<Deployment> items7 = resultCollection.GetCurrent<Deployment>().Items;
        resultCollection.AddBinder<ManualIntervention>((ObjectBinder<ManualIntervention>) this.GetManualInterventionListBinder());
        resultCollection.NextResult();
        List<ManualIntervention> items8 = resultCollection.GetCurrent<ManualIntervention>().Items;
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        resultCollection.NextResult();
        List<ReleaseEnvironmentStep> items9 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
        if (includeArtifact)
        {
          resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
          resultCollection.NextResult();
          source1 = resultCollection.GetCurrent<PipelineArtifactSource>().Items;
        }
        foreach (ReleaseEnvironment releaseEnvironment in items4)
        {
          ReleaseEnvironment env = releaseEnvironment;
          DefinitionEnvironment definitionEnvironment = items5.FirstOrDefault<DefinitionEnvironment>((System.Func<DefinitionEnvironment, bool>) (d => d.ProjectId == env.ProjectId && d.Id == env.DefinitionEnvironmentId));
          if (definitionEnvironment != null)
            env.Schedules = definitionEnvironment.Schedules;
        }
        foreach (Release release in items3)
        {
          int releaseId = release.Id;
          ReleaseSqlComponent15.StitchReleaseObject(release, items4.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (env => env.ReleaseId == releaseId)), items9.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (step => step.ReleaseId == releaseId)), source1.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (source => source.ReleaseId == releaseId)), (IEnumerable<ReleaseDeployPhase>) items6, (IEnumerable<Deployment>) items7, (IEnumerable<ManualIntervention>) items8, (IEnumerable<ReleaseTagData>) null);
          releaseDefinition.Releases.Add(release);
        }
        return releaseDefinition;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Override of base class method.")]
    protected virtual ReleaseDefinitionShallowReferenceBinder GetReleaseDefinitionShallowReferenceBinder() => new ReleaseDefinitionShallowReferenceBinder((ReleaseManagementSqlResourceComponentBase) this);
  }
}
