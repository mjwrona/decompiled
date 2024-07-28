// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentStrategyTaskStepProcessor
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal static class DeploymentStrategyTaskStepProcessor
  {
    public static void GetReferencedResources(
      PipelineBuildContext context,
      ProviderPhase phase,
      IList<Step> steps,
      ValidationResult result)
    {
      if (steps == null)
        return;
      foreach (Step step in (IEnumerable<Step>) steps)
      {
        if (!step.IsDownloadStepDisabled())
        {
          if (step.IsDownloadBuildTask() && context.ResourceStore?.Builds?.Resolver != null)
          {
            string errorMessage = string.Empty;
            if (!context.ResourceStore.Builds.Resolver.ResolveStep(context.ResourceStore, step as TaskStep, out errorMessage))
            {
              result.Errors.Add(new PipelineValidationError(errorMessage));
              break;
            }
          }
          else if (step.IsGetPackageTask() && context.ResourceStore?.Packages?.Resolver != null)
          {
            string errorMessage = string.Empty;
            if (!context.ResourceStore.Packages.Resolver.ResolveStep(context.ResourceStore, step as TaskStep, out errorMessage))
            {
              result.Errors.Add(new PipelineValidationError(errorMessage));
              break;
            }
          }
          DeploymentStrategyTaskStepProcessor.GetReferencedResourcesForEachTaskStep(context, phase, (TaskStep) step, result);
        }
      }
    }

    private static void GetReferencedResourcesForEachTaskStep(
      PipelineBuildContext context,
      ProviderPhase phase,
      TaskStep taskStep,
      ValidationResult result)
    {
      TaskDefinition taskDefinition = (TaskDefinition) null;
      try
      {
        if (taskStep.Reference.Id != Guid.Empty)
          taskDefinition = context.TaskStore?.ResolveTask(taskStep.Reference.Id, taskStep.Reference.Version);
        else if (!string.IsNullOrEmpty(taskStep.Reference.Name))
          taskDefinition = context.TaskStore?.ResolveTask(taskStep.Reference.Name, taskStep.Reference.Version);
      }
      catch (AmbiguousTaskSpecificationException ex)
      {
        result.Errors.Add(new PipelineValidationError(PipelineStrings.TaskStepReferenceInvalid((object) phase.Name, (object) taskStep.Name, (object) ex.Message)));
        return;
      }
      if (taskDefinition == null || taskDefinition.Disabled)
      {
        string str = taskStep.Reference.Id != Guid.Empty ? taskStep.Reference.Id.ToString() : taskStep.Reference.Name;
        result.Errors.Add(new PipelineValidationError(PipelineStrings.TaskMissing((object) phase.Name, (object) taskStep.Name, (object) str, (object) taskStep.Reference.Version)));
      }
      else if (!phase.Target.IsValid(taskDefinition))
      {
        if (phase.Target is ServerTarget)
          result.Errors.Add(new PipelineValidationError(PipelineStrings.TaskInvalidForServerTarget((object) phase.Name, (object) taskDefinition.Name)));
        else
          result.Errors.Add(new PipelineValidationError(PipelineStrings.TaskInvalidForGivenTarget((object) phase.Name, (object) taskStep.Name, (object) taskDefinition.Name, (object) taskDefinition.Version)));
      }
      else
      {
        taskStep.Reference.Id = taskDefinition.Id;
        taskStep.Reference.Name = taskDefinition.Name;
        taskStep.Reference.Version = (string) taskDefinition.Version;
        PipelineValidationError pipelineValidationError = DeploymentStrategyTaskStepProcessor.ValidateStepCondition(context, phase.Name, taskStep.Name, taskStep.Condition);
        if (pipelineValidationError != null)
          result.Errors.Add(pipelineValidationError);
        DeploymentStrategyTaskStepProcessor.ResolveInputs(context, taskStep, taskDefinition, phase.Name, result);
      }
    }

    private static PipelineValidationError ValidateStepCondition(
      PipelineBuildContext context,
      string phaseName,
      string stepName,
      string stepCondition)
    {
      if (!string.IsNullOrEmpty(stepCondition))
      {
        try
        {
          new ExpressionParser().ValidateSyntax(stepCondition, (ITraceWriter) context.Trace);
        }
        catch (ParseException ex)
        {
          return new PipelineValidationError(PipelineStrings.StepConditionIsNotValid((object) phaseName, (object) stepName, (object) stepCondition, (object) ex.Message));
        }
      }
      return (PipelineValidationError) null;
    }

    private static void ResolveInputs(
      PipelineBuildContext context,
      TaskStep step,
      TaskDefinition taskDefinition,
      string phaseName,
      ValidationResult result)
    {
      foreach (TaskInputDefinition input in (IEnumerable<TaskInputDefinition>) taskDefinition.Inputs)
      {
        string inputAlias = DeploymentStrategyTaskStepProcessor.ResolveAlias(context, step, input);
        string inputValue;
        if (step.Inputs.TryGetValue(input.Name, out inputValue))
        {
          DeploymentStrategyTaskStepProcessor.ValidateInput(context, step, input, phaseName, inputAlias, inputValue, result);
          foreach (PipelineValidationError resolveResource in ResourceResolver.ResolveResources((IPipelineContext) context, phaseName, context.BuildOptions, result.ReferencedResources, result.UnauthorizedResources, step, input, inputAlias, inputValue))
            result.Errors.Add(resolveResource);
        }
      }
    }

    private static string ResolveAlias(
      PipelineBuildContext context,
      TaskStep step,
      TaskInputDefinition input)
    {
      string str1 = input.Name;
      if (context.BuildOptions.ResolveTaskInputAliases && !step.Inputs.ContainsKey(input.Name))
      {
        foreach (string alias in (IEnumerable<string>) input.Aliases)
        {
          string str2;
          if (step.Inputs.TryGetValue(alias, out str2))
          {
            str1 = alias;
            step.Inputs.Remove(alias);
            step.Inputs.Add(input.Name, str2);
            break;
          }
        }
      }
      return str1;
    }

    private static void ValidateInput(
      PipelineBuildContext context,
      TaskStep step,
      TaskInputDefinition input,
      string phaseName,
      string inputAlias,
      string value,
      ValidationResult result)
    {
      if (!context.BuildOptions.ValidateTaskInputs || input.Validation == null)
        return;
      string input1 = context.ExpandVariables(value, false);
      if (VariableUtility.IsVariable(input1))
        return;
      InputValidationContext context1 = new InputValidationContext()
      {
        Evaluate = true,
        EvaluationOptions = context.ExpressionOptions,
        Expression = input.Validation.Expression,
        SecretMasker = context.SecretMasker,
        TraceWriter = (ITraceWriter) context.Trace,
        Value = input1
      };
      InputValidationResult validationResult = context.InputValidator.Validate(context1);
      if (validationResult.IsValid)
        return;
      string str1 = context.SecretMasker.MaskSecrets(input1);
      string str2 = validationResult.Reason ?? input.Validation.Message;
      result.Errors.Add(new PipelineValidationError(PipelineStrings.StepTaskInputInvalid((object) phaseName, (object) step.Name, (object) inputAlias, (object) str1, (object) context1.Expression, (object) str2)));
    }
  }
}
