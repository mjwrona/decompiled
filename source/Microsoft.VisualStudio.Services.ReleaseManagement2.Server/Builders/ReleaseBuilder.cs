// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Builders.ReleaseBuilder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Builders
{
  public class ReleaseBuilder : ReleaseBuilderBase, IReleaseBuilder
  {
    public ReleaseBuilder()
    {
    }

    public ReleaseBuilder(
      Action<Release, ReleaseDefinition, IVssRequestContext, string> releaseNameFillerParameter)
      : base(releaseNameFillerParameter)
    {
    }

    public static IList<ReleaseEnvironment> ClearConditionsForSelectedEnvironments(
      IVssRequestContext requestContext,
      Release release,
      IList<string> manualEnvironments)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (manualEnvironments == null)
        return (IList<ReleaseEnvironment>) new List<ReleaseEnvironment>();
      if (!manualEnvironments.Any<string>())
        return release.Environments;
      ReleaseBuilderBase.Trace(requestContext, 1900001, "ReleaseBuilder: ClearConditionsForSelectedEnvironments: ReleaseId: {0}, EnvironmentList: {1}", TraceLevel.Info, (object) release.Id, (object) string.Join(",", (IEnumerable<string>) manualEnvironments));
      ReleaseValidations.CheckForInvalidEnvironmentNames(release, manualEnvironments);
      foreach (string manualEnvironment in (IEnumerable<string>) manualEnvironments)
      {
        ReleaseEnvironment environmentByName = release.GetEnvironmentByName(manualEnvironment);
        if (environmentByName.Conditions != null)
          environmentByName.Conditions.Clear();
      }
      return release.Environments;
    }

    public Release Build(
      ReleaseDefinition definition,
      CreateReleaseParameters createReleaseParameters,
      IVssRequestContext requestContext,
      ReleaseProjectInfo projectInfo)
    {
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      if (createReleaseParameters == null)
        throw new ArgumentNullException(nameof (createReleaseParameters));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      Release release = new Release()
      {
        ReleaseDefinitionId = definition.Id,
        ReleaseDefinitionRevision = definition.Revision,
        Status = createReleaseParameters.IsDraft ? ReleaseStatus.Draft : ReleaseStatus.Active,
        Description = createReleaseParameters.Description,
        CreatedBy = createReleaseParameters.CreatedBy,
        CreatedFor = createReleaseParameters.CreatedFor,
        ModifiedBy = createReleaseParameters.CreatedBy,
        DefinitionSnapshot = definition.ToReleaseDefinitionEnvironmentSnapshot(),
        Reason = createReleaseParameters.Reason,
        ReleaseNameFormat = ReleaseBuilder.SetReleaseFormatMask(definition)
      };
      release.TriggeringArtifactAlias = createReleaseParameters.TriggeringArtifactAlias;
      VariablesUtility.FillVariables(definition.Variables, release.Variables);
      ReleaseBuilder.OverrideDeploymentTimeVariables(release.Variables, createReleaseParameters.Variables);
      Dictionary<int, Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> variableGroups = ReleaseBuilder.GetVariableGroups(requestContext, projectInfo, definition);
      foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) definition.Environments)
      {
        ReleaseEnvironment serverReleaseEnvironment = new ReleaseEnvironment()
        {
          Name = environment.Name,
          DefinitionEnvironmentId = environment.Id,
          Rank = environment.Rank,
          OwnerId = environment.OwnerId,
          PreApprovalOptions = environment.PreApprovalOptions,
          PostApprovalOptions = environment.PostApprovalOptions,
          EnvironmentOptions = environment.EnvironmentOptions,
          ProcessParameters = environment.ProcessParameters,
          PreDeploymentGates = environment.PreDeploymentGates,
          PostDeploymentGates = environment.PostDeploymentGates
        };
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase deployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase>) environment.DeployPhases)
          serverReleaseEnvironment.AddDesignerDeployPhaseSnapshot(ReleaseBuilder.ConvertToReleaseDeployPhase(deployPhase));
        VariablesUtility.FillVariables(environment.Variables, serverReleaseEnvironment.Variables);
        ReleaseBuilder.OverrideDeploymentTimeVariables(serverReleaseEnvironment, createReleaseParameters);
        ReleaseBuilder.PopulateVariableGroups(serverReleaseEnvironment.VariableGroups, environment.VariableGroups, variableGroups);
        release.Environments.Add(serverReleaseEnvironment);
      }
      ReleaseBuilder.PopulateVariableGroups(release.VariableGroups, definition.VariableGroups, variableGroups);
      ReleaseBuilderBase.PopulateArtifactData(release, definition, createReleaseParameters.ArtifactData, requestContext, createReleaseParameters.TriggeringArtifactAlias);
      this.ReleaseNameFiller(release, definition, requestContext, projectInfo.Name);
      ReleaseBuilder.PopulateReleaseEnvironmentConditions(requestContext, definition, release, createReleaseParameters.ManualEnvironments);
      ReleaseManagementArtifactPropertyKinds.CopyProperties(release.Properties, createReleaseParameters.Properties);
      return release;
    }

    private static void OverrideDeploymentTimeVariables(
      ReleaseEnvironment serverReleaseEnvironment,
      CreateReleaseParameters createReleaseParameters)
    {
      ReleaseStartEnvironmentMetadata environmentMetadata = createReleaseParameters.EnvironmentsMetadata.SingleOrDefault<ReleaseStartEnvironmentMetadata>((Func<ReleaseStartEnvironmentMetadata, bool>) (e => e.DefinitionEnvironmentId == serverReleaseEnvironment.DefinitionEnvironmentId));
      if (environmentMetadata == null)
        return;
      ReleaseBuilder.OverrideDeploymentTimeVariables(serverReleaseEnvironment.Variables, environmentMetadata.Variables);
    }

    private static void OverrideDeploymentTimeVariables(
      IDictionary<string, ConfigurationVariableValue> serverVariables,
      IDictionary<string, ConfigurationVariableValue> deploymentTimeVariables)
    {
      deploymentTimeVariables.ForEach<KeyValuePair<string, ConfigurationVariableValue>>((Action<KeyValuePair<string, ConfigurationVariableValue>>) (v =>
      {
        if (!serverVariables.ContainsKey(v.Key) || serverVariables[v.Key].IsSecret && string.IsNullOrEmpty(v.Value.Value))
          return;
        serverVariables[v.Key].Value = v.Value.Value;
      }));
    }

    private static Dictionary<int, Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> GetVariableGroups(
      IVssRequestContext requestContext,
      ReleaseProjectInfo projectInfo,
      ReleaseDefinition definition)
    {
      List<int> source = new List<int>();
      if (definition.VariableGroups != null)
        source.AddRange((IEnumerable<int>) definition.VariableGroups);
      if (definition.Environments != null)
      {
        foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) definition.Environments)
        {
          if (environment?.VariableGroups != null && environment.VariableGroups.Count > 0)
            source.AddRange((IEnumerable<int>) environment.VariableGroups);
        }
      }
      List<int> list = source.Distinct<int>().ToList<int>();
      Dictionary<int, Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> variableGroups = new Dictionary<int, Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>();
      if (list.Any<int>())
      {
        variableGroups = (requestContext.GetService<IVariableGroupService>().GetVariableGroups(requestContext.Elevate(), projectInfo.Id, (IList<int>) list) ?? (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) new List<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>()).ToDictionary<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, int, Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, int>) (vg => vg.Id), (Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) (vg => vg));
        List<int> intList = new List<int>();
        foreach (int key in list)
        {
          if (variableGroups.ContainsKey(key) && variableGroups[key] != null)
            ReleaseBuilder.ClearVariableGroupFieldsWithinRelease(variableGroups[key]);
          else
            intList.Add(key);
        }
        if (!intList.IsNullOrEmpty<int>())
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDefinitionLinkedVariableGroupsDoesNotExist, (object) string.Join<int>(", ", (IEnumerable<int>) intList)));
      }
      return variableGroups;
    }

    private static void PopulateVariableGroups(
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> variableGroups,
      IList<int> variableGroupIds,
      Dictionary<int, Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> variableGroupMap)
    {
      if (variableGroupIds == null || !variableGroupIds.Any<int>())
        return;
      foreach (int variableGroupId in (IEnumerable<int>) variableGroupIds)
      {
        Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup variableGroup;
        if (variableGroupMap.TryGetValue(variableGroupId, out variableGroup))
          variableGroups.Add(variableGroup);
      }
    }

    private static DeployPhaseSnapshot ConvertToReleaseDeployPhase(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase deployPhase) => new DeployPhaseSnapshot()
    {
      Rank = deployPhase.Rank,
      Name = deployPhase.Name,
      RefName = deployPhase.RefName,
      PhaseType = deployPhase.PhaseType,
      Workflow = (IList<WorkflowTask>) deployPhase.Workflow.ToWorkflowTaskList(),
      DeploymentInput = deployPhase.DeploymentInput
    };

    private static void PopulateReleaseEnvironmentConditions(
      IVssRequestContext requestContext,
      ReleaseDefinition definition,
      Release release,
      IList<string> manualEnvironments)
    {
      if (definition.Environments.All<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (env => env.Conditions == null)))
      {
        release.PopulateEnvironmentConditions((IList<ArtifactSourceTrigger>) definition.ArtifactSourceTriggers);
      }
      else
      {
        foreach (ReleaseEnvironment environment1 in (IEnumerable<ReleaseEnvironment>) release.Environments)
        {
          DefinitionEnvironment environment2 = definition.GetEnvironment(environment1.DefinitionEnvironmentId);
          environment1.Conditions = environment2.Conditions.ToReleaseConditions();
        }
      }
      ReleaseBuilder.ClearConditionsForSelectedEnvironments(requestContext, release, manualEnvironments);
    }

    private static void ClearVariableGroupFieldsWithinRelease(Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup variableGroup)
    {
      variableGroup.CreatedBy = (IdentityRef) null;
      variableGroup.ModifiedBy = (IdentityRef) null;
      variableGroup.CreatedOn = new DateTime();
      variableGroup.ModifiedOn = new DateTime();
    }

    private static string SetReleaseFormatMask(ReleaseDefinition definition) => !string.IsNullOrWhiteSpace(definition.ReleaseNameFormat) ? definition.ReleaseNameFormat : "Release-$(Rev:r)";
  }
}
