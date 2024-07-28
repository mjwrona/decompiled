// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent40
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
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
  public class ReleaseDefinitionSqlComponent40 : ReleaseDefinitionSqlComponent39
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public override ReleaseDefinition GetReleaseDefinition(
      Guid projectId,
      int releaseDefinitionId,
      bool includeDeleted = false,
      bool isDefaultToLatestArtifactVersionEnabled = false,
      bool includeLastRelease = false)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseDefinition", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindBoolean(nameof (includeDeleted), includeDeleted);
      this.BindBoolean(nameof (includeLastRelease), includeLastRelease);
      return this.GetReleaseDefinitionObject(isDefaultToLatestArtifactVersionEnabled, includeLastRelease);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
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
        resultCollection.AddBinder<DefinitionTagData>((ObjectBinder<DefinitionTagData>) this.GetDefinitionTagBinder());
        resultCollection.AddBinder<EnvironmentTrigger>((ObjectBinder<EnvironmentTrigger>) this.GetDefinitionEnvironmentTriggerBinder());
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
        foreach (ReleaseTriggerBase releaseTrigger in resultCollection.GetCurrent<ReleaseTriggerBase>().Items)
          releaseDefinition.Triggers.Add(ReleaseDefinitionSqlComponent13.NormalizeDefinitionTrigger(releaseTrigger));
        resultCollection.NextResult();
        resultCollection.GetCurrent<DefinitionTagData>().Items.ForEach((Action<DefinitionTagData>) (t => releaseDefinition.Tags.Add(t.Tag)));
        resultCollection.NextResult();
        List<EnvironmentTrigger> items3 = resultCollection.GetCurrent<EnvironmentTrigger>().Items;
        foreach (DefinitionEnvironment environment3 in (IEnumerable<DefinitionEnvironment>) releaseDefinition.Environments)
        {
          DefinitionEnvironment environment = environment3;
          foreach (EnvironmentTrigger environmentTrigger in items3.Where<EnvironmentTrigger>((System.Func<EnvironmentTrigger, bool>) (p => p.EnvironmentId == environment.Id)))
            environment.EnvironmentTriggers.Add(environmentTrigger);
        }
        if (includeLastRelease)
        {
          resultCollection.AddBinder<ReleaseReference>((ObjectBinder<ReleaseReference>) this.GetReleaseReferenceBinder());
          resultCollection.NextResult();
          releaseDefinition.LastRelease = resultCollection.GetCurrent<ReleaseReference>().SingleOrDefault<ReleaseReference>();
        }
        return releaseDefinition;
      }
    }
  }
}
