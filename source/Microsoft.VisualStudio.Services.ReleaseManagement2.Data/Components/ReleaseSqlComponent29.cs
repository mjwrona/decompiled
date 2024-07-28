// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent29
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
  public class ReleaseSqlComponent29 : ReleaseSqlComponent28
  {
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual DefinitionEnvironmentScheduleBinder GetDefinitionEnvironmentScheduleBinder() => new DefinitionEnvironmentScheduleBinder((ReleaseManagementSqlResourceComponentBase) this);

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

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "by design")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Cannot avoid this as it is required to construct the basic Release object")]
    protected override IEnumerable<Release> GetReleaseObjects(
      Guid projectId,
      bool includeEnvironments,
      bool includeManualInterventions,
      bool includeApprovals,
      bool includeArtifacts,
      bool includeTags)
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        List<ReleaseEnvironment> source1 = new List<ReleaseEnvironment>();
        List<ReleaseDeployPhase> source2 = new List<ReleaseDeployPhase>();
        List<ReleaseEnvironmentStep> source3 = new List<ReleaseEnvironmentStep>();
        List<PipelineArtifactSource> source4 = new List<PipelineArtifactSource>();
        List<Deployment> source5 = new List<Deployment>();
        List<ManualIntervention> source6 = new List<ManualIntervention>();
        List<ReleaseTagData> source7 = new List<ReleaseTagData>();
        List<DefinitionEnvironment> source8 = new List<DefinitionEnvironment>();
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseBinder(projectId));
        List<Release> items = resultCollection.GetCurrent<Release>().Items;
        if (items.Count<Release>() == 0)
          return (IEnumerable<Release>) items;
        if (includeEnvironments)
        {
          resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
          resultCollection.NextResult();
          source1 = resultCollection.GetCurrent<ReleaseEnvironment>().Items;
          resultCollection.AddBinder<DefinitionEnvironment>((ObjectBinder<DefinitionEnvironment>) this.GetDefinitionEnvironmentScheduleBinder());
          resultCollection.NextResult();
          source8 = resultCollection.GetCurrent<DefinitionEnvironment>().Items;
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
        if (includeTags)
        {
          resultCollection.AddBinder<ReleaseTagData>((ObjectBinder<ReleaseTagData>) this.GetReleaseTagsBinder());
          resultCollection.NextResult();
          source7 = resultCollection.GetCurrent<ReleaseTagData>().Items;
        }
        foreach (ReleaseEnvironment releaseEnvironment in source1)
        {
          ReleaseEnvironment env = releaseEnvironment;
          DefinitionEnvironment definitionEnvironment = source8.FirstOrDefault<DefinitionEnvironment>((System.Func<DefinitionEnvironment, bool>) (d => d.ProjectId == env.ProjectId && d.Id == env.DefinitionEnvironmentId));
          if (definitionEnvironment != null)
            env.Schedules = definitionEnvironment.Schedules;
        }
        foreach (Release release1 in items)
        {
          Release release = release1;
          List<ReleaseEnvironment> list = source1.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (env => env.ProjectId.Equals(release.ProjectId) && env.ReleaseId == release.Id)).ToList<ReleaseEnvironment>();
          List<Deployment> deploymentList = new List<Deployment>();
          List<ReleaseEnvironmentStep> steps = new List<ReleaseEnvironmentStep>();
          List<ReleaseDeployPhase> releaseDeployPhaseList = new List<ReleaseDeployPhase>();
          List<ManualIntervention> manualInterventionList = new List<ManualIntervention>();
          IEnumerable<ReleaseTagData> releaseTags = source7.Where<ReleaseTagData>((System.Func<ReleaseTagData, bool>) (r => r.ReleaseId == release.Id));
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
          ReleaseSqlComponent15.StitchReleaseObject(release, (IEnumerable<ReleaseEnvironment>) list, (IEnumerable<ReleaseEnvironmentStep>) steps, source4.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (source => source.ReleaseId == release.Id)), (IEnumerable<ReleaseDeployPhase>) releaseDeployPhaseList, (IEnumerable<Deployment>) deploymentList, (IEnumerable<ManualIntervention>) manualInterventionList, releaseTags);
        }
        return (IEnumerable<Release>) items;
      }
    }
  }
}
