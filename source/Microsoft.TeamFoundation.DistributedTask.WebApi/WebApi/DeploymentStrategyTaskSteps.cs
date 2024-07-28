// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentStrategyTaskSteps
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal static class DeploymentStrategyTaskSteps
  {
    private const string DownloadBuildStep = "downloadBuild";
    private const string GetPackageStep = "getPackage";
    private const string StepTaskPropertyName = "task";
    private const string StepEnvironmentPropertyName = "env";
    private const string StepTargetPropertyName = "target";
    private const string StepTargetContainerName = "container";
    private const string StepTargetCommandName = "commands";

    public static IList<Step> Build(
      IResourceStore resourceStore,
      JEnumerable<JToken> stepsJTokens,
      ValidationResult validationResult,
      bool allowTaskMinorVersion,
      bool resolveDownloadBuildTask)
    {
      List<Step> stepList = new List<Step>();
      foreach (JToken stepsJtoken in stepsJTokens)
      {
        JProperty jproperty = stepsJtoken.Children<JProperty>().FirstOrDefault<JProperty>();
        if (jproperty != null)
        {
          TaskStep step;
          switch (jproperty.Name)
          {
            case "downloadBuild":
              step = DeploymentStrategyTaskSteps.BuildDownloadBuildStep(stepsJtoken, validationResult);
              DeploymentStrategyTaskSteps.ResolveDownloadBuildOrPackageStep(resourceStore, validationResult, resolveDownloadBuildTask, step);
              break;
            case "getPackage":
              step = DeploymentStrategyTaskSteps.BuildGetPackageStep(stepsJtoken, validationResult);
              DeploymentStrategyTaskSteps.ResolveDownloadBuildOrPackageStep(resourceStore, validationResult, resolveDownloadBuildTask, step);
              break;
            default:
              step = DeploymentStrategyTaskSteps.BuildTaskStep(jproperty.Name, stepsJtoken, validationResult, allowTaskMinorVersion);
              break;
          }
          if (step != null && step.Enabled)
            stepList.Add((Step) step);
        }
      }
      return (IList<Step>) stepList;
    }

    private static void ResolveDownloadBuildOrPackageStep(
      IResourceStore resourceStore,
      ValidationResult validationResult,
      bool resolveDownloadBuildTask,
      TaskStep step)
    {
      string errorMessage;
      if (!resolveDownloadBuildTask || resourceStore.Builds.Resolver.ResolveStep(resourceStore, step, out errorMessage))
        return;
      validationResult.Errors.Add(new PipelineValidationError(errorMessage));
    }

    private static TaskStep BuildDownloadBuildStep(
      JToken downloadJToken,
      ValidationResult validationResult)
    {
      TaskStep taskStep = DeploymentStrategyTaskSteps.BuildBaseStep(downloadJToken, new TaskStep()
      {
        Reference = new TaskStepDefinitionReference()
        {
          Name = "downloadBuild"
        }
      }, validationResult);
      foreach (JProperty child in downloadJToken.Children<JProperty>())
      {
        switch (child.Name)
        {
          case "downloadBuild":
            if (string.IsNullOrWhiteSpace(child.Value.ToString()))
            {
              validationResult.Errors.Add(new PipelineValidationError("downloadBuild step must have an alias."));
              continue;
            }
            taskStep.Inputs["alias"] = child.Value.ToString();
            continue;
          case "inputs":
            using (IEnumerator<JProperty> enumerator = child.Value.Children<JProperty>().GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                JProperty current = enumerator.Current;
                if (string.Equals(current.Name, "patterns"))
                  taskStep.Inputs["patterns"] = current.Value.ToString();
              }
              continue;
            }
          default:
            continue;
        }
      }
      return taskStep;
    }

    private static TaskStep BuildGetPackageStep(
      JToken downloadJToken,
      ValidationResult validationResult)
    {
      TaskStep packageStep = DeploymentStrategyTaskSteps.BuildBaseStep(downloadJToken, new TaskStep()
      {
        Reference = new TaskStepDefinitionReference()
        {
          Name = "getPackage"
        }
      }, validationResult);
      foreach (JProperty child in downloadJToken.Children<JProperty>())
      {
        if (child.Name == "getPackage")
        {
          if (string.IsNullOrWhiteSpace(child.Value.ToString()))
            validationResult.Errors.Add(new PipelineValidationError("getPackage step must have an alias."));
          else
            packageStep.Inputs["alias"] = child.Value.ToString();
        }
      }
      return packageStep;
    }

    private static TaskStep BuildBaseStep(
      JToken stepJToken,
      TaskStep step,
      ValidationResult validationResult)
    {
      foreach (JProperty child in stepJToken.Children<JProperty>())
      {
        string name = child.Name;
        if (name != null)
        {
          switch (name.Length)
          {
            case 3:
              if (name == "env")
              {
                using (IEnumerator<JProperty> enumerator = child.Value.Children<JProperty>().GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    JProperty current = enumerator.Current;
                    step.Environment.Add(current.Name, current.Value.ToString());
                  }
                  continue;
                }
              }
              else
                continue;
            case 4:
              if (name == "name")
              {
                step.Name = child.Value.ToString();
                continue;
              }
              continue;
            case 7:
              if (name == "enabled")
              {
                step.Enabled = DeploymentStrategyTaskSteps.ConvertToBoolean(child.Value.ToString(), validationResult);
                continue;
              }
              continue;
            case 9:
              if (name == "condition")
              {
                step.Condition = child.Value.ToString();
                continue;
              }
              continue;
            case 11:
              if (name == "displayName")
              {
                step.DisplayName = child.Value.ToString();
                continue;
              }
              continue;
            case 15:
              if (name == "continueOnError")
              {
                step.ContinueOnError = DeploymentStrategyTaskSteps.ConvertToBoolean(child.Value.ToString(), validationResult);
                continue;
              }
              continue;
            case 16:
              if (name == "timeoutInMinutes")
              {
                step.TimeoutInMinutes = DeploymentStrategyTaskSteps.ConvertToInt32(child.Value.ToString(), validationResult);
                continue;
              }
              continue;
            case 23:
              if (name == "retryCountOnTaskFailure")
              {
                step.RetryCountOnTaskFailure = DeploymentStrategyTaskSteps.ConvertToInt32(child.Value.ToString(), validationResult);
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      return step;
    }

    private static bool ConvertToBoolean(string literal, ValidationResult validationResult)
    {
      bool result;
      if (DeploymentStrategyTaskSteps.TryParseBoolean(literal, out result))
        return result;
      validationResult.Errors.Add(new PipelineValidationError(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Expected a Boolean value. Actual value: {0}.", (object) literal)));
      return false;
    }

    private static int ConvertToInt32(string literal, ValidationResult validationResult)
    {
      int result;
      if (DeploymentStrategyTaskSteps.TryParseInt32(literal, out result))
        return result;
      validationResult.Errors.Add(new PipelineValidationError(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Expected an integer value. Actual value: {0}.", (object) literal)));
      return 0;
    }

    private static bool TryParseBoolean(string value, out bool result)
    {
      if (!string.IsNullOrEmpty(value))
      {
        if (string.Equals(value, "TRUE", StringComparison.OrdinalIgnoreCase))
        {
          result = true;
          return true;
        }
        if (string.Equals(value, "FALSE", StringComparison.OrdinalIgnoreCase))
        {
          result = false;
          return true;
        }
      }
      result = false;
      return false;
    }

    private static bool TryParseInt32(string value, out int result)
    {
      if (!string.IsNullOrEmpty(value) && int.TryParse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        return true;
      result = 0;
      return false;
    }

    private static TaskStep BuildTaskStep(
      string stepName,
      JToken stepJToken,
      ValidationResult validationResult,
      bool allowTaskMinorVersion)
    {
      TaskStep taskStep = stepJToken.ToObject<TaskStep>();
      JProperty jproperty1 = stepJToken.Children<JProperty>().FirstOrDefault<JProperty>((Func<JProperty, bool>) (x => x.Name.ToLower() == "task"));
      if (jproperty1 == null || !jproperty1.HasValues)
      {
        validationResult.Errors.Add(new PipelineValidationError("Step " + stepName + " does not have valid 'task' property"));
        return (TaskStep) null;
      }
      string name;
      string version;
      if (!DeploymentStrategyTaskSteps.TryParseTaskReference(jproperty1.Value.ToString(), allowTaskMinorVersion, out name, out version))
      {
        validationResult.Errors.Add(new PipelineValidationError("Invalid step task reference " + jproperty1.Value.ToString()));
        return (TaskStep) null;
      }
      if (taskStep != null)
      {
        taskStep.Reference = new TaskStepDefinitionReference()
        {
          Name = name,
          Version = version
        };
        JProperty jproperty2 = stepJToken.Children<JProperty>().FirstOrDefault<JProperty>((Func<JProperty, bool>) (x => x.Name.ToLower() == "env"));
        if (jproperty2 != null && jproperty2.HasValues)
        {
          foreach (IEnumerable<JToken> child in jproperty2.Value.Children<JToken>())
          {
            JProperty jproperty3 = child.Value<JProperty>();
            if (jproperty3 != null && !string.IsNullOrWhiteSpace(jproperty3.Name))
              taskStep.Environment[jproperty3.Name] = jproperty3.Value.ToString();
          }
        }
        JProperty jproperty4 = stepJToken.Children<JProperty>().FirstOrDefault<JProperty>((Func<JProperty, bool>) (x => x.Name.ToLower() == "target"));
        if (jproperty4 != null && jproperty4.HasValues)
        {
          foreach (IEnumerable<JToken> child in jproperty4.Value.Children<JToken>())
          {
            JProperty jproperty5 = child.Value<JProperty>();
            if (jproperty5 != null && !string.IsNullOrWhiteSpace(jproperty5.Name))
            {
              switch (jproperty5.Name)
              {
                case "container":
                  taskStep.Target.Target = jproperty5.Value.ToString();
                  continue;
                case "commands":
                  taskStep.Target.Commands = jproperty5.Value.ToString();
                  continue;
                default:
                  throw new NotSupportedException("Unexpected target property '" + jproperty5.Name + "'");
              }
            }
          }
        }
      }
      return taskStep;
    }

    private static bool TryParseTaskReference(
      string value,
      bool allowTaskMinorVersion,
      out string name,
      out string version)
    {
      bool taskReference;
      if (!string.IsNullOrEmpty(value))
      {
        string[] strArray = value.Split('@');
        if (strArray.Length == 2 && !string.IsNullOrEmpty(strArray[0]) && !string.IsNullOrEmpty(strArray[1]) && DeploymentStrategyTaskSteps.ValidateTaskVersion(strArray[1], allowTaskMinorVersion))
        {
          taskReference = true;
          name = strArray[0];
          version = strArray[1];
        }
        else
        {
          taskReference = false;
          name = (string) null;
          version = (string) null;
        }
      }
      else
      {
        taskReference = false;
        name = (string) null;
        version = (string) null;
      }
      return taskReference;
    }

    private static bool ValidateTaskVersion(string taskVersion, bool allowTaskMinorVersion)
    {
      if (int.TryParse(taskVersion, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture, out int _))
        return true;
      if (allowTaskMinorVersion)
      {
        string[] strArray = taskVersion.Split('.');
        if (strArray.Length == 3)
        {
          foreach (string s in strArray)
          {
            if (!int.TryParse(s, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture, out int _))
              return false;
          }
          return true;
        }
      }
      return false;
    }
  }
}
