// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionValidator
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildDefinitionValidator
  {
    public readonly char[] IllegalNameChars = ((IEnumerable<char>) FileSpec.IllegalNtfsChars).Union<char>((IEnumerable<char>) new char[2]
    {
      '$',
      '@'
    }).ToArray<char>();

    public void Validate(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      bool isUpdate)
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      ArgumentUtility.CheckForEmptyGuid(definition.ProjectId, "definition.Project.Id");
      ArgumentUtility.CheckForNull<BuildRepository>(definition.Repository, "definition.Repository");
      ArgumentUtility.CheckForNull<BuildProcess>(definition.Process, "definition.Process");
      if (definition.Process.Type == 3 && !requestContext.IsFeatureEnabled("Build2.DockerProcess"))
        throw new InvalidDefinitionException(BuildServerResources.ProcessTypeNotSupported((object) definition.Process.Type));
      if (isUpdate)
        ArgumentUtility.CheckForNull<int>(definition.Revision, "definition.Revision");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(definition.Name, "definition.Name");
      ArgumentUtility.CheckStringForInvalidCharacters(definition.Name, "definition.Name", this.IllegalNameChars);
      ArgumentUtility.CheckForNonnegativeInt(definition.JobTimeoutInMinutes, "definition.JobTimeoutInMinutes");
      ArgumentUtility.CheckForNonnegativeInt(definition.JobCancelTimeoutInMinutes, "definition.JobCancelTimeoutInMinutes");
      if (!string.IsNullOrEmpty(definition.BuildNumberFormat))
      {
        definition.BuildNumberFormat = definition.BuildNumberFormat.TrimEnd();
        if (!ArgumentValidation.IsValidBuildNumberFormat(definition.BuildNumberFormat))
          throw new BuildNumberFormatException(BuildServerResources.BuildNumberFormatInvalidCharacters((object) definition.BuildNumberFormat));
      }
      if (!string.IsNullOrEmpty(definition.Path))
      {
        string path = definition.Path;
        FolderValidator.CheckValidItemPath(ref path, false, true);
        if (path.Length > GitConstants.MaxGitRefNameLength)
          throw new InvalidFolderException(BuildServerResources.InvalidFolder((object) path));
        definition.Path = path;
      }
      definition.Name = definition.Name.Trim();
      this.ValidateDraftDefinition(requestContext, definition);
      this.ValidateBuildSteps(requestContext, definition.AllSteps().ToList<BuildDefinitionStep>());
      this.ValidateRepository(requestContext, definition.ProjectId, definition.Repository);
      this.ValidateBuildOptions(requestContext, definition.Options, (IDictionary<string, BuildDefinitionVariable>) definition.Variables);
      this.ValidateBuildJobScope(requestContext, definition);
      definition.ValidateVariables();
      definition.ValidateVariableGroups(requestContext);
      this.ValidateBuildDemands(definition.Demands);
      this.ValidateTriggers(requestContext, definition.Process.Type, definition.Repository, definition.Triggers);
    }

    public void ValidateDraftDefinition(
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      DefinitionQuality? definitionQuality1 = definition.DefinitionQuality;
      DefinitionQuality definitionQuality2 = DefinitionQuality.Draft;
      if (definitionQuality1.GetValueOrDefault() == definitionQuality2 & definitionQuality1.HasValue)
      {
        if (definition.Triggers.Any<BuildTrigger>())
          throw new InvalidDefinitionException(BuildServerResources.InvalidDraftDefinition());
      }
      else if (definition.ParentDefinition != null)
        throw new InvalidDefinitionException(BuildServerResources.InvalidDefinitionProperty((object) "DraftOf"));
    }

    public void ValidateBuildSteps(
      IVssRequestContext requestContext,
      List<BuildDefinitionStep> steps)
    {
      ArgumentUtility.CheckForNull<List<BuildDefinitionStep>>(steps, "definition.Steps", "Build2");
      for (int index = 0; index < steps.Count; ++index)
      {
        BuildDefinitionStep step = steps[index];
        ArgumentUtility.CheckForNull<BuildDefinitionStep>(step, string.Format("definition.Steps[{0}]", (object) index), "Build2");
        ArgumentUtility.CheckForNull<TaskDefinitionReference>(step.TaskDefinition, string.Format("definition.Steps[{0}]", (object) index), "Build2");
        if (!string.IsNullOrEmpty(step.DisplayName))
          ArgumentUtility.CheckStringForInvalidCharacters(step.DisplayName, "definition.Steps.Step.DisplayName", "Build2");
        ArgumentUtility.CheckForNonnegativeInt(step.TimeoutInMinutes, "TimeoutInMinutes", "Build2");
        foreach (KeyValuePair<string, string> input in (IEnumerable<KeyValuePair<string, string>>) step.Inputs)
        {
          if (!string.IsNullOrEmpty(input.Value))
            ArgumentUtility.CheckStringForInvalidCharacters(input.Value, "definition.Steps." + input.Key, true, "Build2");
        }
        string reason;
        if (!string.IsNullOrEmpty(step.Condition) && !requestContext.GetService<IInputValidationService>().ValidateInput(requestContext, "expression", step.Condition, out reason))
          throw new ArgumentException(reason, "condition").Expected("Build2");
      }
    }

    public void ValidateRepository(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository)
    {
      using (requestContext.TraceScope("Service", nameof (ValidateRepository)))
      {
        if (repository == null)
          return;
        ArgumentUtility.CheckStringForNullOrEmpty(repository.Type, "repository.Type");
        requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, repository.Type, false)?.ValidateRepository(requestContext, projectId, repository);
      }
    }

    public void ValidateBuildJobScope(IVssRequestContext requestContext, BuildDefinition definition)
    {
      ProjectPipelineGeneralSettingsHelper generalSettingsHelper = new ProjectPipelineGeneralSettingsHelper(requestContext, definition.ProjectId, true);
      if (definition.JobAuthorizationScope != BuildAuthorizationScope.Project && !generalSettingsHelper.EnforceJobAuthScope)
        return;
      IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, definition.Repository.Type, false);
      Guid guid = Guid.Empty;
      if (sourceProvider != null && definition.Repository != null)
        guid = sourceProvider.GetRepositoryProjectId(requestContext, definition.Repository);
      if (guid != Guid.Empty && definition.ProjectId != guid && !requestContext.IsFeatureEnabled("Build2.AllowOutOfScopeRepository"))
        throw new InvalidDefinitionException(BuildServerResources.InvalidAuthScopeErrorMessage());
    }

    public void ValidateBuildOptions(
      IVssRequestContext requestContext,
      List<BuildOption> options,
      IDictionary<string, BuildDefinitionVariable> variables)
    {
      ArgumentUtility.CheckForNull<List<BuildOption>>(options, "definition.Options");
      if (options.Count <= 0)
        return;
      HashSet<Guid> guidSet = new HashSet<Guid>();
      for (int index = 0; index < options.Count; ++index)
      {
        BuildOption option = options[index];
        ArgumentUtility.CheckForNull<BuildOptionDefinition>(option.Definition, string.Format("definition.Options[{0}].Definition", (object) index));
        if (!guidSet.Add(option.Definition.Id))
          throw new InvalidDefinitionException(BuildServerResources.DuplicateBuildOption((object) option.Definition.Id));
      }
      using (IDisposableReadOnlyList<IBuildOption> extensions = requestContext.GetExtensions<IBuildOption>())
      {
        foreach (IBuildOption buildOption in (IEnumerable<IBuildOption>) extensions)
        {
          IBuildOption buildOptionExtension = buildOption;
          BuildOption option = options.FirstOrDefault<BuildOption>((Func<BuildOption, bool>) (x => x.Enabled && x.Definition.Id == buildOptionExtension.GetId()));
          if (option != null)
          {
            ArgumentUtility.CheckForNull<IDictionary<string, string>>(option.Inputs, "definition.Options.Inputs");
            foreach (KeyValuePair<string, string> input in (IEnumerable<KeyValuePair<string, string>>) option.Inputs)
              ArgumentUtility.CheckStringForInvalidCharacters(input.Value, "definition.Options." + input.Key);
            string errorMessage;
            if (!buildOptionExtension.Validate(option, variables, out errorMessage))
              throw new InvalidDefinitionException(errorMessage);
          }
        }
      }
    }

    public void ValidateBuildDemands(List<Demand> demands)
    {
      ArgumentUtility.CheckForNull<List<Demand>>(demands, "definition.Demands");
      foreach (Demand demand in demands)
      {
        ArgumentUtility.CheckForNull<Demand>(demand, "definition.Demands.Demand");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(demand.Name, "definition.Demands.Demand.Name");
        ArgumentUtility.CheckStringForInvalidCharacters(demand.Name, "definition.Demands.Demand.Name");
        ArgumentUtility.CheckStringForInvalidCharacters(demand.Name, "definition.Demands.Demand.Name", new char[1]
        {
          ' '
        });
        if (demand.Value != null)
        {
          ArgumentUtility.CheckStringForInvalidCharacters(demand.Value, "definition.Demands.Demand.Value");
          ArgumentUtility.CheckStringForNullOrWhiteSpace(demand.Value, "definition.Demands.Demand.Value");
        }
      }
    }

    public void ValidateTriggers(
      IVssRequestContext requestContext,
      int processType,
      BuildRepository repository,
      List<BuildTrigger> triggers)
    {
      ArgumentUtility.CheckForNull<BuildRepository>(repository, nameof (repository), "Build2");
      if (triggers.Count == 0)
        return;
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, repository.Type, false);
      foreach (BuildTrigger trigger1 in triggers)
      {
        if (sourceProvider != null && !sourceProvider.SupportsTriggerType(requestContext, trigger1.TriggerType))
          throw new InvalidDefinitionException(BuildServerResources.TriggerNotSupportedForRepository((object) trigger1.TriggerType, (object) repository.Type));
        ContinuousIntegrationTrigger integrationTrigger = trigger1 as ContinuousIntegrationTrigger;
        GatedCheckInTrigger gatedCheckInTrigger = trigger1 as GatedCheckInTrigger;
        ScheduleTrigger trigger2 = trigger1 as ScheduleTrigger;
        if (integrationTrigger != null)
        {
          if (++num1 > 1)
            throw new InvalidDefinitionException(BuildServerResources.MultipleTriggersNotAllowed((object) trigger1.TriggerType));
          switch (integrationTrigger.SettingsSourceType)
          {
            case 1:
              if (integrationTrigger.MaxConcurrentBuildsPerBranch < 1)
              {
                integrationTrigger.MaxConcurrentBuildsPerBranch = 1;
                break;
              }
              break;
            case 2:
              if (processType != 2)
                throw new InvalidDefinitionException(BuildServerResources.InvalidTriggerSettingsSourceType((object) integrationTrigger.SettingsSourceType, (object) processType));
              integrationTrigger.BatchChanges = false;
              integrationTrigger.BranchFilters.Clear();
              integrationTrigger.MaxConcurrentBuildsPerBranch = 0;
              integrationTrigger.PathFilters.Clear();
              integrationTrigger.PollingInterval = new int?();
              integrationTrigger.PollingJobId = Guid.Empty;
              break;
            default:
              throw new InvalidDefinitionException(BuildServerResources.InvalidSettingsSourceType((object) integrationTrigger.SettingsSourceType));
          }
        }
        else if (gatedCheckInTrigger != null)
        {
          if (++num2 > 1)
            throw new InvalidDefinitionException(BuildServerResources.MultipleTriggersNotAllowed((object) trigger1.TriggerType));
        }
        else if (trigger2 != null)
        {
          if (++num3 > 1)
            throw new InvalidDefinitionException(BuildServerResources.MultipleTriggersNotAllowed((object) trigger1.TriggerType));
          this.ValidateScheduledTrigger(trigger2);
        }
        sourceProvider?.ValidateTrigger(requestContext, trigger1, repository);
      }
    }

    public void ValidateScheduledTrigger(ScheduleTrigger trigger)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) trigger.Schedules, "definition.Triggers.ScheduledTrigger.Schedules");
      foreach (Schedule schedule in trigger.Schedules)
      {
        if (schedule.DaysToBuild == ScheduleDays.None)
          throw new InvalidDefinitionException(BuildServerResources.DaysToBuildInvalid());
        FilterValidation.ValidateFilters(schedule.BranchFilters, "definition.Triggers.ScheduledTrigger.Schedule.BranchFilters.Filter");
      }
    }
  }
}
