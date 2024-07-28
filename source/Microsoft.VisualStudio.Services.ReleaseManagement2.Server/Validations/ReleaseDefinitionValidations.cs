// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.ReleaseDefinitionValidations
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "All validations are kept together for readability")]
  public static class ReleaseDefinitionValidations
  {
    public static void Validate(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition,
      IVssRequestContext context,
      Guid projectId)
    {
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      ReleaseDefinitionValidations.ValidateReleaseDefinitionName(definition);
      ReleaseDefinitionValidations.ValidateReleaseDefinitionTriggers(definition);
      if (!definition.IsYamlDefinition())
        ReleaseDefinitionValidations.ValidateDesignerPipeline(definition, context, projectId);
      ReleaseDefinitionValidations.ValidateReleaseNameFormat(definition);
      ReleaseDefinitionValidations.ValidateDeploymentGroupTriggerAtEnvironmentLevel(definition);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Will not be adding an overload for this")]
    public static void ValidateBackCompatibilityChanges(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition serverDefinition = null)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.EnableBackCompatValidations"))
        return;
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      ReleaseDefinitionValidations.ValidateArtifacts(definition.Artifacts, serverDefinition?.LinkedArtifacts);
    }

    private static void ValidateArtifacts(
      IList<Artifact> webApiArtifacts,
      IList<ArtifactSource> serverArtifacts)
    {
      foreach (Artifact artifact1 in (IEnumerable<Artifact>) (webApiArtifacts ?? (IList<Artifact>) new List<Artifact>()))
      {
        Artifact artifact = artifact1;
        string additionalCharacters = (serverArtifacts != null ? serverArtifacts.FirstOrDefault<ArtifactSource>((Func<ArtifactSource, bool>) (x => string.Equals(x.Alias, artifact.Alias, StringComparison.Ordinal))) : (ArtifactSource) null) == null ? "\\(\\)" : string.Empty;
        if (!ArtifactValidations.IsValidAliasName(artifact.Alias, additionalCharacters))
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ArtifactAliasHasInvalidCharacters, (object) artifact.Alias));
      }
    }

    public static void Validate(
      this ReleaseDefinitionEnvironment environment,
      IVssRequestContext context,
      Guid projectId)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> variableGroups = ReleaseDefinitionVariableGroupUtility.GetVariableGroups(context, projectId, environment.VariableGroups);
      environment.Validate((Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition) null, context, variableGroups, (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) null);
    }

    public static void ValidateRetentionPolicy(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        return;
      if (releaseDefinition.RetentionPolicy != null && releaseDefinition.RetentionPolicy.DaysToKeep <= 0)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.DaysToKeepMustBeMoreThanZeroRetentionPolicy));
      if (releaseDefinition.Environments == null)
        return;
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) releaseDefinition.Environments)
      {
        string empty = string.Empty;
        if (environment.RetentionPolicy != null)
        {
          if (environment.RetentionPolicy.DaysToKeep <= 0)
            empty += string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.DaysToKeepMustBeMoreThanZeroRetentionPolicy);
          if (environment.RetentionPolicy.ReleasesToKeep <= 0)
          {
            if (!string.IsNullOrEmpty(empty))
              empty += System.Environment.NewLine;
            empty += string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ReleasesToKeepMustBeMoreThanZeroRetentionPolicy);
          }
        }
        if (!string.IsNullOrEmpty(empty))
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(empty);
      }
    }

    private static void ValidateDesignerPipeline(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition,
      IVssRequestContext context,
      Guid projectId)
    {
      if (definition.Environments == null || definition.Environments.Count == 0)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.DefinitionEnvironmentsCannotBeEmpty);
      IEnumerable<string> strings = definition.Environments.Select<ReleaseDefinitionEnvironment, string>((Func<ReleaseDefinitionEnvironment, string>) (e => e.Name)).GroupBy<string, string>((Func<string, string>) (n => n), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Where<IGrouping<string, string>>((Func<IGrouping<string, string>, bool>) (g => g.Count<string>() > 1)).Select<IGrouping<string, string>, string>((Func<IGrouping<string, string>, string>) (g => g.Key));
      if (strings.Any<string>())
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.DefinitionEnvironmentNamesCannotBeIdentical, (object) string.Join(",", strings)));
      if (!ReleaseDefinitionValidations.AreEnvironmentRanksValid((IList<int>) definition.Environments.Select<ReleaseDefinitionEnvironment, int>((Func<ReleaseDefinitionEnvironment, int>) (e => e.Rank)).ToList<int>()))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.DefinitionEnvironmentRanksNeedToBeCorrect);
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> variableGroupsInRd = ReleaseDefinitionVariableGroupUtility.GetAllVariableGroupsInRD(context, projectId, definition);
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> mergedRDVariables = ReleaseDefinitionVariableGroupUtility.GetVariableGroupVariables(definition.VariableGroups, variableGroupsInRd);
      if (definition.Variables != null)
        mergedRDVariables = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>[2]
        {
          definition.Variables,
          mergedRDVariables
        });
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) definition.Environments)
        environment.Validate(definition, context, variableGroupsInRd, mergedRDVariables);
      definition.ValidateConditions();
    }

    private static void Validate(
      this ReleaseDefinitionEnvironment environment,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition parentDefinition,
      IVssRequestContext context,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> allVariableGroupsInRD,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> mergedRDVariables)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      if (string.IsNullOrWhiteSpace(environment.Name))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.DefinitionEnvironmentNameCannotBeEmpty, (object) environment.Rank));
      if (environment.Name.Length > 256)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ReleaseManagementObjectNameLengthExceeded, (object) "Environment", (object) environment.Name, (object) 256));
      if (environment.Id > 0 && !ReleaseDefinitionValidations.IsOwnerValid(environment.Owner))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.DefinitionEnvironmentOwnerCannotBeEmpty, (object) environment.Name));
      environment.EnvironmentOptions.Validate();
      environment.ExecutionPolicy.Validate(environment.Name);
      if (environment.PreDeployApprovals == null || environment.PostDeployApprovals == null)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ReleaseApprovalsCannotBeEmpty, (object) environment.Name));
      environment.PreDeploymentGates.ValidateGateStep(context, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.PreDeploymentGatesText, environment.Name);
      environment.PostDeploymentGates.ValidateGateStep(context, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.PostDeploymentGatesText, environment.Name);
      ReleaseDefinitionApprovalStepExtensions.ValidateApprovalSteps((ICollection<ReleaseDefinitionApprovalStep>) environment.PreDeployApprovals.Approvals, environment.Name);
      ReleaseDefinitionValidations.ValidateDeployStep(environment.DeployStep, environment.Name);
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> mergedVariables = ReleaseDefinitionValidations.GetMergedVariables(environment, allVariableGroupsInRD, mergedRDVariables);
      ReleaseDefinitionValidations.ValidateDeployPhases(environment, parentDefinition, context, mergedVariables);
      ReleaseDefinitionApprovalStepExtensions.ValidateApprovalSteps((ICollection<ReleaseDefinitionApprovalStep>) environment.PostDeployApprovals.Approvals, environment.Name);
      ReleaseDefinitionApprovals preDeployApprovals = environment.PreDeployApprovals;
      ReleaseDefinitionApproverValidations.ValidateApprovalOptions(preDeployApprovals.ApprovalOptions, preDeployApprovals.Approvals.Count, ApprovalType.PreDeploy, environment.Name);
      ReleaseDefinitionApprovals postDeployApprovals = environment.PostDeployApprovals;
      ReleaseDefinitionApproverValidations.ValidateApprovalOptions(postDeployApprovals.ApprovalOptions, postDeployApprovals.Approvals.Count, ApprovalType.PostDeploy, environment.Name);
      if (environment.Schedules.Count<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule>() > 1)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.CannotHaveMoreThanOneScheduleInEnvironment);
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule schedule in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule>) environment.Schedules)
        ReleaseDefinitionValidations.ValidateReleaseSchedule(schedule);
      if (environment != null && environment.Conditions.Serialize<IList<Condition>>().Length >= 4000)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.MaxPropertyLengthExceeded, (object) "Conditions", (object) 4000.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
    }

    private static void ValidateReleaseDefinitionTriggers(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition)
    {
      bool flag1 = false;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase trigger in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase>) definition.Triggers)
      {
        if (trigger.TriggerType == ReleaseTriggerType.Schedule)
        {
          if (!(trigger is Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduledReleaseTrigger scheduledReleaseTrigger))
            throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidReleaseScheduleTriggerObject);
          ReleaseDefinitionValidations.ValidateReleaseSchedule(scheduledReleaseTrigger.Schedule);
          flag1 = true;
        }
        else if (trigger.TriggerType == ReleaseTriggerType.SourceRepo)
          ReleaseDefinitionValidations.ValidateSourceRepoTrigger(trigger);
        else if (trigger.TriggerType == ReleaseTriggerType.ArtifactSource)
          ReleaseDefinitionValidations.ValidateArtifactSourceTrigger(trigger);
        else if (trigger.TriggerType == ReleaseTriggerType.PullRequest)
          ReleaseDefinitionValidations.ValidatePullRequestTrigger(trigger);
        else if (trigger.TriggerType == ReleaseTriggerType.ContainerImage)
          ReleaseDefinitionValidations.ValidateContainerImageTrigger(trigger);
        ReleaseDefinitionValidations.ValidateTriggerAndArtifactDefaultVersionTypes(trigger, definition);
      }
      if (!flag1)
        return;
      bool flag2 = false;
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) definition.Environments)
      {
        if (environment == null)
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.EnvironmentCannotBeEmpty);
        foreach (Condition condition in (IEnumerable<Condition>) environment.Conditions)
        {
          if (condition == null)
            throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.EnvironmentConditionCannotBeEmpty);
          if (condition.ConditionType == ConditionType.Event && string.Equals(condition.Name, "ReleaseStarted", StringComparison.Ordinal))
          {
            flag2 = true;
            break;
          }
        }
        if (flag2)
          break;
      }
      if (!flag2)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.MustHaveAtLeastOneAutoTriggeredEnvironment, (object) definition.Name, (object) definition.Id.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
    }

    private static void ValidateArtifactSourceTrigger(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase trigger)
    {
      if (!(trigger is Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactSourceTrigger artifactSourceTrigger) || artifactSourceTrigger.ArtifactAlias.IsNullOrEmpty<char>())
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidArtifactSourceTriggerObject);
      if (artifactSourceTrigger.TriggerConditions.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactFilter>())
        return;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactFilter triggerCondition in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactFilter>) artifactSourceTrigger.TriggerConditions)
      {
        if (triggerCondition.SourceBranch != null && triggerCondition.SourceBranch.StartsWith("-", StringComparison.OrdinalIgnoreCase))
        {
          if (triggerCondition.SourceBranch.Substring(1).IsNullOrEmpty<char>())
            throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ExcludeTriggerIsInvalid, (object) artifactSourceTrigger.ArtifactAlias, (object) triggerCondition));
          if (!triggerCondition.Tags.IsNullOrEmpty<string>())
            throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ExcludeTriggerTagsAreInvalid, (object) artifactSourceTrigger.ArtifactAlias, (object) triggerCondition));
        }
      }
    }

    private static void ValidateSourceRepoTrigger(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase trigger)
    {
      if (!(trigger is Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.SourceRepoTrigger sourceRepoTrigger) || sourceRepoTrigger.Alias.IsNullOrEmpty<char>())
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidSourceRepoTriggerObject);
      foreach (string branchFilter in sourceRepoTrigger.BranchFilters)
      {
        if (branchFilter.StartsWith("-", StringComparison.OrdinalIgnoreCase) && branchFilter.Substring(1).IsNullOrEmpty<char>())
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ExcludeTriggerIsInvalid, (object) sourceRepoTrigger.Alias, (object) branchFilter));
      }
    }

    private static void ValidatePullRequestTrigger(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase trigger)
    {
      if (!(trigger is Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestTrigger pullRequestTrigger) || pullRequestTrigger.ArtifactAlias.IsNullOrEmpty<char>() || pullRequestTrigger.PullRequestConfiguration == null)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidPullRequestTriggerObject);
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestFilter> triggerConditions = pullRequestTrigger.TriggerConditions;
      if (triggerConditions.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestFilter>())
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.PullRequestTriggerBranchFilterCannotBeEmpty));
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestFilter pullRequestFilter in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestFilter>) triggerConditions)
      {
        if (string.IsNullOrEmpty(pullRequestFilter.TargetBranch))
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidPullRequestTriggerBranchFilter));
      }
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestConfiguration requestConfiguration = pullRequestTrigger.PullRequestConfiguration;
      if (requestConfiguration.UseArtifactReference)
        return;
      List<string> referenceAttributes = ReleaseDefinitionValidations.GetMissingCodeRepositoryReferenceAttributes(requestConfiguration.CodeRepositoryReference);
      if (!referenceAttributes.IsNullOrEmpty<string>())
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidCodeRepositoryReferenceObject, (object) string.Join(",", (IEnumerable<string>) referenceAttributes)));
    }

    private static void ValidateContainerImageTrigger(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase trigger)
    {
      if (!(trigger is Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ContainerImageTrigger trigger1) || trigger1.Alias.IsNullOrEmpty<char>())
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidContainerImageTriggerObject);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.TagFilter.ValidateTagFilters(trigger1);
    }

    [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Right name is being passed to ArgumentNullException")]
    private static void ValidateTriggerAndArtifactDefaultVersionTypes(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase trigger,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition)
    {
      string b = (string) null;
      switch (trigger.TriggerType)
      {
        case ReleaseTriggerType.ArtifactSource:
          b = trigger is Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactSourceTrigger artifactSourceTrigger ? artifactSourceTrigger.ArtifactAlias : (string) null;
          break;
        case ReleaseTriggerType.SourceRepo:
          b = trigger is Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.SourceRepoTrigger sourceRepoTrigger ? sourceRepoTrigger.Alias : (string) null;
          break;
        case ReleaseTriggerType.ContainerImage:
          b = trigger is Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ContainerImageTrigger containerImageTrigger ? containerImageTrigger.Alias : (string) null;
          break;
        case ReleaseTriggerType.Package:
          b = trigger is Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PackageTrigger packageTrigger ? packageTrigger.Alias : (string) null;
          break;
        case ReleaseTriggerType.PullRequest:
          b = trigger is Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestTrigger pullRequestTrigger ? pullRequestTrigger.ArtifactAlias : (string) null;
          break;
      }
      IList<Artifact> artifacts = definition?.Artifacts;
      if (artifacts == null)
        return;
      foreach (Artifact artifact in (IEnumerable<Artifact>) artifacts)
      {
        if (artifact.DefinitionReference == null)
          throw new ArgumentNullException("DefinitionReference");
        ArtifactSourceReference artifactSourceReference;
        if (artifact.DefinitionReference.TryGetValue("defaultVersionType", out artifactSourceReference) && artifactSourceReference != null && string.Equals(artifactSourceReference.Id, "selectDuringReleaseCreationType", StringComparison.OrdinalIgnoreCase))
        {
          if (trigger.TriggerType == ReleaseTriggerType.Schedule)
            throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.CannotHaveDefaultVersionTypeWithScheduledTrigger, (object) Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.SelectDuringReleaseCreationType, (object) artifact.Alias));
          if (b != null && !string.Equals(artifact.Alias, b, StringComparison.OrdinalIgnoreCase))
            throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.CannotHaveDefaultVersionTypeWithArtifactAutoTrigger, (object) Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.SelectDuringReleaseCreationType, (object) artifact.Alias, (object) b));
        }
      }
    }

    private static List<string> GetMissingCodeRepositoryReferenceAttributes(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.CodeRepositoryReference codeRepositoryReference)
    {
      List<string> referenceAttributes = new List<string>();
      if (codeRepositoryReference == null)
      {
        referenceAttributes.Add(nameof (codeRepositoryReference));
        return referenceAttributes;
      }
      PullRequestSystemType systemType = codeRepositoryReference.SystemType;
      IDictionary<string, ReleaseManagementInputValue> repositoryReference = codeRepositoryReference.RepositoryReference;
      if (repositoryReference == null || repositoryReference.IsNullOrEmpty<KeyValuePair<string, ReleaseManagementInputValue>>())
      {
        referenceAttributes.Add("repositoryReference");
        return referenceAttributes;
      }
      if (systemType.Equals((object) PullRequestSystemType.GitHub))
      {
        if (!repositoryReference.ContainsKey("pullRequestSystemConnectionId"))
          referenceAttributes.Add("pullRequestSystemConnectionId");
        if (!repositoryReference.ContainsKey("pullRequestRepositoryName"))
          referenceAttributes.Add("pullRequestRepositoryName");
      }
      else if (systemType.Equals((object) PullRequestSystemType.TfsGit))
      {
        if (!repositoryReference.ContainsKey("pullRequestProjectId"))
          referenceAttributes.Add("pullRequestProjectId");
        if (!repositoryReference.ContainsKey("pullRequestRepositoryId"))
          referenceAttributes.Add("pullRequestRepositoryId");
      }
      return referenceAttributes;
    }

    private static void ValidateReleaseSchedule(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseSchedule schedule)
    {
      if (schedule == null)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidReleaseScheduleTriggerObject);
      if (schedule.StartHours < 0 || schedule.StartHours > 23 || schedule.StartMinutes < 0 || schedule.StartMinutes > 59)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ScheduledTimeNotInFuture);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "It requires to be in lower case")]
    private static void ValidateConditions(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition)
    {
      definition.Environments.Select<ReleaseDefinitionEnvironment, string>((Func<ReleaseDefinitionEnvironment, string>) (env => env.Name)).ToList<string>();
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) definition.Environments)
      {
        foreach (Condition condition1 in (IEnumerable<Condition>) environment.Conditions)
        {
          Condition condition = condition1;
          if (condition.ConditionType == ConditionType.EnvironmentState)
            condition.Name = (definition.Environments.FirstOrDefault<ReleaseDefinitionEnvironment>((Func<ReleaseDefinitionEnvironment, bool>) (e => string.Equals(e.Name, condition.Name, StringComparison.OrdinalIgnoreCase))) ?? throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.EnvironmentConditionEnvironmentNameIsInvalid, (object) environment.Name, (object) condition.Name, (object) string.Join(",", definition.Environments.Select<ReleaseDefinitionEnvironment, string>((Func<ReleaseDefinitionEnvironment, string>) (e => e.Name)))))).Name;
          else if (condition.ConditionType == ConditionType.Artifact)
          {
            string str = condition.Value;
            if (!string.IsNullOrEmpty(condition.Value))
            {
              Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter expectedArtifactFilter;
              if (!Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter.TryParseArtifactFilter(condition.Value, out expectedArtifactFilter))
                throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ConditionValueIncorrectFormat, (object) condition.Name, (object) str));
              IDictionary<string, JToken> dictionary = (IDictionary<string, JToken>) JObject.Parse(str.ToLower(CultureInfo.InvariantCulture));
              if (!dictionary.ContainsKey("sourcebranch") && !dictionary.ContainsKey("tags") && !dictionary.ContainsKey("tagfilter"))
                throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ConditionValueIncorrectFormat, (object) condition.Name, (object) str));
              string errorMessage;
              if (expectedArtifactFilter.TagFilter != null && !Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.TagFilter.IsValidRegex(expectedArtifactFilter.TagFilter.Pattern, out errorMessage))
                throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidRegexPatternInEnvironmentCondition, (object) expectedArtifactFilter.TagFilter.Pattern, (object) environment.Name, (object) condition.Name, (object) string.Join(",", new string[1]
                {
                  str
                }), (object) errorMessage));
              if (expectedArtifactFilter.SourceBranch.StartsWith("-", StringComparison.OrdinalIgnoreCase))
              {
                if (expectedArtifactFilter.SourceBranch.Substring(1).IsNullOrEmpty<char>())
                  throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ExcludeConditionSourceBranchIsInvalid, (object) environment.Name, (object) condition.Name, (object) string.Join(",", new string[1]
                  {
                    str
                  })));
                if (!expectedArtifactFilter.Tags.IsNullOrEmpty<string>())
                  throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ExcludeConditionTagsAreInvalid, (object) environment.Name, (object) condition.Name, (object) string.Join(",", new string[1]
                  {
                    str
                  })));
              }
            }
          }
        }
      }
    }

    private static void ValidateReleaseDefinitionName(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition)
    {
      if (string.IsNullOrWhiteSpace(definition.Name))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.DefinitionNameCannotBeEmpty);
      if (definition.Name.Length > 256)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ReleaseManagementObjectNameLengthExceeded, (object) "Release Definition", (object) definition.Name, (object) 256));
    }

    private static void ValidateReleaseNameFormat(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition definition)
    {
      string releaseNameFormat = definition.ReleaseNameFormat;
      if (!ReleaseDefinitionValidations.IsValidReleaseNameFormat(releaseNameFormat))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidReleaseNameFormat, (object) releaseNameFormat));
    }

    private static bool IsValidReleaseNameFormat(string releaseNameFormat)
    {
      if (string.IsNullOrEmpty(releaseNameFormat))
        return true;
      if (!ReleaseDefinitionValidations.IsReleaseNameFormatLengthValid(releaseNameFormat))
        return false;
      IList<VariableMatch> environmentVariableMatches = BuildCommonUtil.GetEnvironmentVariableMatches(releaseNameFormat);
      if (environmentVariableMatches.Count < 0)
        return ReleaseDefinitionValidations.HasInvalidCharacters(releaseNameFormat, 0, releaseNameFormat.Length);
      int lastMatchEnd;
      return ReleaseDefinitionValidations.HasInvalidCharactersInBetweenMatchedTokens(releaseNameFormat, (IEnumerable<VariableMatch>) environmentVariableMatches, out lastMatchEnd) && ReleaseDefinitionValidations.HasInvalidCharactersAfterLastMatchedToken(releaseNameFormat, lastMatchEnd);
    }

    private static bool HasInvalidCharactersInBetweenMatchedTokens(
      string releaseNameFormat,
      IEnumerable<VariableMatch> matchedTokens,
      out int lastMatchEnd)
    {
      bool flag = true;
      lastMatchEnd = 0;
      foreach (VariableMatch matchedToken in matchedTokens)
      {
        if (lastMatchEnd < matchedToken.StartIndex)
          flag = ReleaseDefinitionValidations.HasInvalidCharacters(releaseNameFormat, lastMatchEnd, matchedToken.StartIndex - lastMatchEnd);
        if (flag)
          lastMatchEnd = matchedToken.EndIndex + 1;
        else
          break;
      }
      return flag;
    }

    private static bool HasInvalidCharactersAfterLastMatchedToken(
      string releaseNameFormat,
      int lastMatchEnd)
    {
      bool flag = true;
      if (lastMatchEnd < releaseNameFormat.Length - 1)
        flag = ReleaseDefinitionValidations.HasInvalidCharacters(releaseNameFormat, lastMatchEnd, releaseNameFormat.Length - lastMatchEnd);
      return flag;
    }

    private static bool HasInvalidCharacters(string releaseNameFormat, int startIndex, int length) => releaseNameFormat.IndexOfAny(ReleaseNameFormatTokensConstants.ReleaseNameFormatInvalidCharacters.ToArray<char>(), startIndex, length) < 0;

    private static bool IsReleaseNameFormatLengthValid(string releaseNameFormat) => releaseNameFormat.Length <= 256;

    private static bool IsOwnerValid(IdentityRef owner)
    {
      Guid result;
      return owner != null && Guid.TryParse(owner.Id, out result) && result != Guid.Empty;
    }

    private static void ValidateDeployStep(
      ReleaseDefinitionDeployStep deployStep,
      string environmentName)
    {
      if (deployStep == null)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.DefinitionDeployStepCannotBeEmpty, (object) environmentName));
    }

    private static void ValidateDeployPhases(
      ReleaseDefinitionEnvironment environment,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition parentDefinition,
      IVssRequestContext context,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> mergedVariables)
    {
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase> deployPhases = environment.DeployPhases;
      if (deployPhases == null || deployPhases.Count == 0)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.DefinitionDeployPhasesCannotBeEmpty, (object) environment.Name));
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) deployPhases)
      {
        if (deployPhase == null)
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.DefinitionDeployPhasesCannotBeEmpty, (object) environment.Name));
        if (string.IsNullOrWhiteSpace(deployPhase.Name) || deployPhase.Name.Length > 256)
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.InvalidDeployPhaseName, (object) deployPhase.Name, (object) 256));
        deployPhase.ValidatePhaseRefName();
        deployPhase.ValidateWorkflow(environment.Name, mergedVariables, context);
        deployPhase.ValidateDeploymentInput(mergedVariables, parentDefinition?.Artifacts, context);
        deployPhase.ValidatePhaseCondition(context, environment.Name);
      }
      deployPhases.EnsureNoDuplicatePhaseRefNames();
      if (!ReleaseDefinitionValidations.AreDeployPhaseRanksValid((IList<int>) deployPhases.Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, int>) (dp => dp.Rank)).ToList<int>()))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.DefinitionDeployPhaseRanksNeedToBeCorrect);
    }

    private static bool AreEnvironmentRanksValid(IList<int> ranks) => ranks.OrderBy<int, int>((Func<int, int>) (r => r)).SequenceEqual<int>(Enumerable.Range(1, ranks.Count<int>()));

    private static bool AreDeployPhaseRanksValid(IList<int> ranks) => ranks.OrderBy<int, int>((Func<int, int>) (r => r)).SequenceEqual<int>(Enumerable.Range(1, ranks.Count<int>()));

    private static IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> GetMergedVariables(
      ReleaseDefinitionEnvironment environment,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> allVariableGroupsInRD,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> mergedRDVariables)
    {
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> dictionary = mergedRDVariables ?? (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) new Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>();
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variableGroupVariables = ReleaseDefinitionVariableGroupUtility.GetVariableGroupVariables(environment.VariableGroups, allVariableGroupsInRD);
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> mergedVariables = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>[3]
      {
        environment.Variables,
        variableGroupVariables,
        dictionary
      });
      if (environment.ProcessParameters != null)
        mergedVariables = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>[2]
        {
          (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) environment.ProcessParameters.GetProcessParametersAsWebContractVariables(),
          mergedVariables
        });
      return mergedVariables;
    }

    private static void ValidateDeploymentGroupTriggerAtEnvironmentLevel(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition.Environments == null)
        return;
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) releaseDefinition.Environments)
      {
        if (environment.EnvironmentTriggers.Any<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentTrigger>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentTrigger, bool>) (t => t.TriggerType == EnvironmentTriggerType.DeploymentGroupRedeploy)) && !environment.DeployPhases.Any<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, bool>) (phase => phase.PhaseType == DeployPhaseTypes.MachineGroupBasedDeployment)))
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.AutoRedeployTriggerOnlyAvailableWithDGPhase, (object) environment.Name));
      }
    }
  }
}
