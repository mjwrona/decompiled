// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent34
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseDefinitionSqlComponent34 : ReleaseDefinitionSqlComponent33
  {
    private static readonly SqlMetaData[] TypInt32Table = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.Int)
    };

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "reviewed")]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Arguments are already validated at service layer.")]
    protected override ReleaseDefinition AddOrUpdateReleaseDefinition(
      ReleaseDefinition releaseDefinition,
      bool bindIds,
      string oldToken = null,
      string newToken = null)
    {
      this.BindEnvironmentTriggerAndDeployTriggerTable(releaseDefinition);
      return base.AddOrUpdateReleaseDefinition(releaseDefinition, bindIds, oldToken, newToken);
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
        IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase> deployPhases = this.GetDeployPhases(resultCollection);
        foreach (DefinitionEnvironment environment2 in (IEnumerable<DefinitionEnvironment>) releaseDefinition.Environments)
        {
          DefinitionEnvironment environment = environment2;
          foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase deployPhase in deployPhases.Where<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase>((System.Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase, bool>) (p => p.DefinitionEnvironmentId == environment.Id)))
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
        return releaseDefinition;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual EnvironmentTriggerBinder GetDefinitionEnvironmentTriggerBinder() => new EnvironmentTriggerBinder();

    protected virtual void BindEnvironmentTriggerAndDeployTriggerTable(
      ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      IList<EnvironmentTrigger> environmentTriggerList = (IList<EnvironmentTrigger>) new List<EnvironmentTrigger>();
      IList<EnvironmentDeploymentGroupPhaseMapping> environmentDeploymentGroupPhaseMappings = (IList<EnvironmentDeploymentGroupPhaseMapping>) new List<EnvironmentDeploymentGroupPhaseMapping>();
      Dictionary<int, string> dictionary = new Dictionary<int, string>();
      foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) releaseDefinition.Environments)
      {
        foreach (EnvironmentTrigger environmentTrigger in (IEnumerable<EnvironmentTrigger>) environment.EnvironmentTriggers)
        {
          if (environmentTrigger.TriggerType == (byte) 1)
          {
            foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase deployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase>) environment.DeployPhases)
            {
              if (deployPhase.PhaseType == DeployPhaseTypes.MachineGroupBasedDeployment)
              {
                MachineGroupDeploymentInput deploymentInput = (MachineGroupDeploymentInput) deployPhase.GetDeploymentInput();
                environmentDeploymentGroupPhaseMappings.Add(new EnvironmentDeploymentGroupPhaseMapping(releaseDefinition.Id, deployPhase.DefinitionEnvironmentId, deploymentInput.QueueId, string.Join(",", (IEnumerable<string>) deploymentInput.Tags)));
              }
            }
          }
        }
        dictionary[environment.Id] = environment.Name;
        environmentTriggerList = (IList<EnvironmentTrigger>) environmentTriggerList.Concat<EnvironmentTrigger>((IEnumerable<EnvironmentTrigger>) environment.EnvironmentTriggers).ToList<EnvironmentTrigger>();
      }
      this.BindEnvironmentTriggerTable1("environmentTriggers", environmentTriggerList, (IDictionary<int, string>) dictionary);
      this.BindEnvironmentDeploymentGroupPhaseMappingTable1("environmentDeploymentGroupPhaseMappings", environmentDeploymentGroupPhaseMappings, (IDictionary<int, string>) dictionary);
    }

    public override IList<RedeployTriggerEnvironmentDGPhaseData> GetRedeployTriggerEnvironmentDGPhaseData(
      Guid projectId,
      IEnumerable<int> deploymentGroupIds)
    {
      if (deploymentGroupIds == null || !deploymentGroupIds.Any<int>())
        return (IList<RedeployTriggerEnvironmentDGPhaseData>) new List<RedeployTriggerEnvironmentDGPhaseData>();
      this.PrepareStoredProcedure("Release.prc_GetRedeployTriggerEnvironmentDGPhaseData", projectId);
      this.BindInt32Table(nameof (deploymentGroupIds), deploymentGroupIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<RedeployTriggerEnvironmentDGPhaseData>((ObjectBinder<RedeployTriggerEnvironmentDGPhaseData>) new RedeployTriggerEnvironmentDGPhaseDataBinder());
        return (IList<RedeployTriggerEnvironmentDGPhaseData>) resultCollection.GetCurrent<RedeployTriggerEnvironmentDGPhaseData>().Items;
      }
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(int eventId)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(ReleaseDefinitionSqlComponent34.TypInt32Table);
      sqlDataRecord.SetInt32(0, eventId);
      return sqlDataRecord;
    }

    protected override void BindReleaseTriggerTable(ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      this.BindReleaseTriggerTable7("triggers", (IEnumerable<ReleaseTriggerBase>) releaseDefinition.Triggers);
    }

    protected override void BindReleaseTriggerTable(IEnumerable<ReleaseTriggerBase> triggers) => this.BindReleaseTriggerTable7(nameof (triggers), triggers);
  }
}
