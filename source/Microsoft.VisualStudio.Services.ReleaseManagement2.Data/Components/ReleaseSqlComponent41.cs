// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent41
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
  public class ReleaseSqlComponent41 : ReleaseSqlComponent40
  {
    protected const DeploymentOperationStatus DesiredOperationStatus = DeploymentOperationStatus.Queued | DeploymentOperationStatus.Scheduled | DeploymentOperationStatus.Pending | DeploymentOperationStatus.Approved | DeploymentOperationStatus.Deferred | DeploymentOperationStatus.QueuedForAgent | DeploymentOperationStatus.QueuedForPipeline | DeploymentOperationStatus.EvaluatingGates;

    public override IEnumerable<Release> QueryActiveReleases(
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
      this.PrepareStoredProcedure("Release.prc_QueryActiveReleases", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt("notDeployedOperationStatus", 82031);
      this.BindInt(nameof (maxReleasesCount), maxReleasesCount);
      this.BindInt(nameof (continuationToken), continuationToken);
      this.BindBoolean(nameof (includeEnvironments), includeEnvironments);
      this.BindBoolean(nameof (includeArtifacts), includeArtifacts);
      this.BindBoolean(nameof (includeApprovals), includeApprovals);
      this.BindBoolean(nameof (includeManualInterventions), includeManualInterventions);
      this.BindIncludeVariables(includeVariables);
      this.BindBoolean(nameof (includeTags), includeTags);
      return this.GetReleaseObjects(projectId, includeEnvironments, includeManualInterventions, includeApprovals, includeArtifacts, includeTags);
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Will be overridden in derived classes.")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "It is a virtual function")]
    protected virtual DefinitionEnvironmentSchedulesBinder GetEnvironmentSchedulesBinder() => new DefinitionEnvironmentSchedulesBinder();

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Too heavy to be a property")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Coupling is there to build the complex Release object efficiently.")]
    protected override Release GetReleaseObject(Guid projectId)
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseBinder(projectId));
        resultCollection.AddBinder<DefinitionEnvironment>((ObjectBinder<DefinitionEnvironment>) this.GetEnvironmentSchedulesBinder());
        resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
        resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
        resultCollection.AddBinder<Deployment>((ObjectBinder<Deployment>) this.GetDeploymentBinder());
        resultCollection.AddBinder<DeploymentGate>((ObjectBinder<DeploymentGate>) this.GetDeploymentGateBinder());
        resultCollection.AddBinder<DeploymentIssue>((ObjectBinder<DeploymentIssue>) this.GetDeploymentIssueBinder());
        resultCollection.AddBinder<ManualIntervention>((ObjectBinder<ManualIntervention>) this.GetManualInterventionListBinder());
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
        resultCollection.AddBinder<ReleaseTagData>((ObjectBinder<ReleaseTagData>) this.GetReleaseTagsBinder());
        Release release = resultCollection.GetCurrent<Release>().FirstOrDefault<Release>();
        resultCollection.NextResult();
        List<DefinitionEnvironment> items1 = resultCollection.GetCurrent<DefinitionEnvironment>().Items;
        resultCollection.NextResult();
        List<ReleaseEnvironment> items2 = resultCollection.GetCurrent<ReleaseEnvironment>().Items;
        resultCollection.NextResult();
        List<ReleaseDeployPhase> items3 = resultCollection.GetCurrent<ReleaseDeployPhase>().Items;
        resultCollection.NextResult();
        List<Deployment> items4 = resultCollection.GetCurrent<Deployment>().Items;
        resultCollection.NextResult();
        List<DeploymentGate> items5 = resultCollection.GetCurrent<DeploymentGate>().Items;
        resultCollection.NextResult();
        List<DeploymentIssue> items6 = resultCollection.GetCurrent<DeploymentIssue>().Items;
        resultCollection.NextResult();
        List<ManualIntervention> items7 = resultCollection.GetCurrent<ManualIntervention>().Items;
        resultCollection.NextResult();
        List<ReleaseEnvironmentStep> items8 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
        resultCollection.NextResult();
        List<PipelineArtifactSource> items9 = resultCollection.GetCurrent<PipelineArtifactSource>().Items;
        resultCollection.NextResult();
        List<ReleaseTagData> items10 = resultCollection.GetCurrent<ReleaseTagData>().Items;
        Dictionary<int, IList<ReleaseSchedule>> dictionary = items1.ToDictionary<DefinitionEnvironment, int, IList<ReleaseSchedule>>((System.Func<DefinitionEnvironment, int>) (d => d.Id), (System.Func<DefinitionEnvironment, IList<ReleaseSchedule>>) (d => d.Schedules));
        foreach (ReleaseEnvironment releaseEnvironment in items2)
        {
          if (dictionary.ContainsKey(releaseEnvironment.DefinitionEnvironmentId))
            releaseEnvironment.Schedules = dictionary[releaseEnvironment.DefinitionEnvironmentId];
        }
        if (release != null)
          ReleaseSqlComponent15.StitchReleaseObject(release, (IEnumerable<ReleaseEnvironment>) items2, (IEnumerable<ReleaseEnvironmentStep>) items8, (IEnumerable<PipelineArtifactSource>) items9, (IEnumerable<ReleaseDeployPhase>) items3, (IEnumerable<Deployment>) items4, (IEnumerable<ManualIntervention>) items7, (IEnumerable<ReleaseTagData>) items10, (IEnumerable<DeploymentGate>) items5, (IEnumerable<DeploymentIssue>) items6);
        return release;
      }
    }
  }
}
