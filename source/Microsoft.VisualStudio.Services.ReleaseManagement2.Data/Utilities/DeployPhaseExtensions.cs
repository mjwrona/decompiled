// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.DeployPhaseExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class DeployPhaseExtensions
  {
    public static BaseDeploymentInput GetDeploymentInput(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variables)
    {
      if (deployPhase == null)
        return (BaseDeploymentInput) null;
      Dictionary<string, string> dictionary = variables == null ? (Dictionary<string, string>) null : variables.Where<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>, bool>) (v => v.Value != null && !v.Value.IsSecret)).ToDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>, string, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>, string>) (v => v.Key), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>, string>) (v => v.Value.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      return DeployPhaseExtensions.GetDeploymentInput(deployPhase.GetDeploymentInput(), deployPhase.PhaseType, (IDictionary<string, string>) dictionary);
    }

    public static BaseDeploymentInput GetDeploymentInput(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables)
    {
      if (deployPhase == null)
        return (BaseDeploymentInput) null;
      Dictionary<string, string> dictionary = variables == null ? (Dictionary<string, string>) null : variables.Where<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, bool>) (v => v.Value != null && !v.Value.IsSecret)).ToDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (v => v.Key), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (v => v.Value.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      return DeployPhaseExtensions.GetDeploymentInput(deployPhase.GetDeploymentInput(), deployPhase.PhaseType, (IDictionary<string, string>) dictionary);
    }

    public static BaseDeploymentInput GetDeploymentInput(
      this DeployPhaseSnapshot deployPhaseSnapshot,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables)
    {
      if (deployPhaseSnapshot == null)
        return (BaseDeploymentInput) null;
      Dictionary<string, string> dictionary = variables == null ? (Dictionary<string, string>) null : variables.Where<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, bool>) (v => v.Value != null && !v.Value.IsSecret)).ToDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (v => v.Key), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (v => v.Value.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes phaseType;
      switch (deployPhaseSnapshot.PhaseType)
      {
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.AgentBasedDeployment:
          phaseType = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.AgentBasedDeployment;
          break;
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.RunOnServer:
          phaseType = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.RunOnServer;
          break;
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.MachineGroupBasedDeployment:
          phaseType = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.MachineGroupBasedDeployment;
          break;
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.DeploymentGates:
          phaseType = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.DeploymentGates;
          break;
        default:
          throw new NotSupportedException();
      }
      return DeployPhaseExtensions.GetDeploymentInput(deployPhaseSnapshot.GetDeploymentInput(), phaseType, (IDictionary<string, string>) dictionary);
    }

    public static IEnumerable<ReleaseTask> GetAllTasks(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase> releaseDeployPhases)
    {
      return releaseDeployPhases.SelectMany<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase, DeploymentJob>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase, IEnumerable<DeploymentJob>>) (rdp => (IEnumerable<DeploymentJob>) rdp.DeploymentJobs)).GetAllTasks();
    }

    public static IEnumerable<ReleaseTask> GetAllJobs(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase> releaseDeployPhases)
    {
      return releaseDeployPhases.SelectMany<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase, DeploymentJob>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDeployPhase, IEnumerable<DeploymentJob>>) (rdp => (IEnumerable<DeploymentJob>) rdp.DeploymentJobs)).GetAllJobs();
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase> FilterForAttempt(
      this IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase> releaseDeployPhases,
      int attemptNumber)
    {
      return releaseDeployPhases.Where<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase, bool>) (rdp => rdp.Attempt == attemptNumber));
    }

    public static void EnsureNoDuplicatePhaseRefNames(this IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase> deployPhases)
    {
      if (deployPhases == null)
        return;
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) deployPhases)
      {
        if (deployPhase != null && !string.IsNullOrEmpty(deployPhase.RefName))
        {
          if (stringSet.Contains(deployPhase.RefName))
            throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DuplicatePhaseRefNameUsedInStage, (object) deployPhase.RefName));
          stringSet.Add(deployPhase.RefName);
        }
      }
    }

    public static void ValidateWorkflow(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase,
      string environmentName,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variables,
      IVssRequestContext requestContext)
    {
      if (deployPhase == null)
        throw new ArgumentNullException(nameof (deployPhase));
      if (deployPhase.WorkflowTasks == null || deployPhase.WorkflowTasks.Any<WorkflowTask>((Func<WorkflowTask, bool>) (t => t.TaskId == Guid.Empty || string.IsNullOrEmpty(t.Version))))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeployPhaseWorkflowCannotBeEmpty, (object) deployPhase.Name, (object) environmentName));
      IInputValidationService service = requestContext.GetService<IInputValidationService>();
      foreach (WorkflowTask workflowTask in (IEnumerable<WorkflowTask>) deployPhase.WorkflowTasks)
      {
        if (!string.IsNullOrEmpty(workflowTask.Condition) && !service.ValidateInput(requestContext, "expression", workflowTask.Condition, out string _))
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidTaskCondition, (object) workflowTask.Name, (object) deployPhase.Name, (object) environmentName));
        if (workflowTask.TimeoutInMinutes < 0)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidTaskTimeout, (object) workflowTask.Name, (object) deployPhase.Name, (object) environmentName));
        if (workflowTask.OverrideInputs != null && workflowTask.OverrideInputs.Count > 0)
          DeployPhaseExtensions.ValidateTaskOverrideInputs(workflowTask, deployPhase.Name, environmentName, variables);
      }
      if (deployPhase.PhaseType != Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.DeploymentGates)
        return;
      DeploymentGatesHelper.ValidateGateTasks((IEnumerable<WorkflowTask>) deployPhase.WorkflowTasks, deployPhase.Name, environmentName);
    }

    public static void ValidateDeploymentInput(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variables,
      IList<Artifact> linkedArtifacts,
      IVssRequestContext context)
    {
      if (deployPhase == null)
        throw new ArgumentNullException(nameof (deployPhase));
      BasePhaseValidator validator = BasePhaseValidator.GetValidator(deployPhase.PhaseType, deployPhase.Name);
      validator.ValidateDeploymentInputOverrideInputs(deployPhase.GetDeploymentInput(), variables, deployPhase.Name);
      validator.ValidateDeploymentInput(deployPhase.GetDeploymentInput(variables), variables, linkedArtifacts, context);
    }

    public static void ValidateDeploymentInputIsNotModified(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase,
      DeployPhaseSnapshot serverDeployPhase)
    {
      if (deployPhase == null)
        throw new ArgumentNullException(nameof (deployPhase));
      if (serverDeployPhase == null)
        throw new ArgumentNullException(nameof (serverDeployPhase));
      BasePhaseValidator.GetValidator(deployPhase.PhaseType, deployPhase.Name).ValidateDeploymentInputIsNotModified(deployPhase.GetDeploymentInput(), serverDeployPhase.GetDeploymentInput());
    }

    public static void ValidatePhaseCondition(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase,
      IVssRequestContext context,
      string environmentName)
    {
      string expression = deployPhase != null ? deployPhase.GetDeploymentInput().Condition : throw new ArgumentNullException(nameof (deployPhase));
      if (string.IsNullOrEmpty(expression))
        return;
      try
      {
        new ExpressionParser().ValidateSyntax(expression, (ITraceWriter) new PhaseConditionTracer(context));
      }
      catch (ParseException ex)
      {
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.PhaseConditionInvalid, (object) deployPhase.Name, (object) environmentName, (object) ex.Message));
      }
    }

    public static void ValidatePhasesRefName(this IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase> deployPhaseSnapshots)
    {
      if (deployPhaseSnapshots != null)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhaseSnapshot in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) deployPhaseSnapshots)
          deployPhaseSnapshot.ValidatePhaseRefName();
      }
      deployPhaseSnapshots.EnsureNoDuplicatePhaseRefNames();
    }

    public static void ValidatePhaseRefName(this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase)
    {
      if (deployPhase == null || string.IsNullOrEmpty(deployPhase.RefName))
        return;
      string refName = deployPhase.RefName;
      if (refName.Length > 256 || !NameValidation.IsValid(refName))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidDeployPhaseRefName, (object) refName, (object) 256));
    }

    public static void NormalizeArtifactsDownloadInput(
      this BaseDeploymentInput deploymentInput,
      DeployPhaseSnapshot phaseData)
    {
      if (deploymentInput == null)
        return;
      if (phaseData == null)
        throw new ArgumentNullException(nameof (phaseData));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes deployPhaseTypes;
      switch (phaseData.PhaseType)
      {
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.AgentBasedDeployment:
          deployPhaseTypes = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.AgentBasedDeployment;
          break;
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.RunOnServer:
          deployPhaseTypes = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.RunOnServer;
          break;
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.MachineGroupBasedDeployment:
          deployPhaseTypes = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.MachineGroupBasedDeployment;
          break;
        case Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.DeploymentGates:
          deployPhaseTypes = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.DeploymentGates;
          break;
        default:
          throw new NotSupportedException();
      }
      if (deployPhaseTypes != Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.AgentBasedDeployment && deployPhaseTypes != Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.MachineGroupBasedDeployment)
        return;
      DeploymentInput deploymentInput1 = (DeploymentInput) deploymentInput;
      if (deploymentInput1.SkipArtifactsDownload || deploymentInput1.ArtifactsDownloadInput == null || deploymentInput1.ArtifactsDownloadInput.DownloadInputs.IsNullOrEmpty<ArtifactDownloadInputBase>() || !deploymentInput1.ArtifactsDownloadInput.DownloadInputs.All<ArtifactDownloadInputBase>((Func<ArtifactDownloadInputBase, bool>) (input => input.ArtifactDownloadMode.Equals("Skip", StringComparison.OrdinalIgnoreCase))))
        return;
      deploymentInput1.SkipArtifactsDownload = true;
      deploymentInput1.ArtifactsDownloadInput.DownloadInputs.Clear();
    }

    public static void FillReleaseGatesPhaseWithDeploymentGate(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseGatesPhase webApiGatesPhase,
      DeploymentGate serverDeploymentGate)
    {
      if (webApiGatesPhase == null || serverDeploymentGate == null)
        return;
      webApiGatesPhase.StabilizationCompletedOn = serverDeploymentGate.StabilizationCompletedOn;
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseGatesPhase releaseGatesPhase = webApiGatesPhase;
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate> ignoredGates = serverDeploymentGate.IgnoredGates;
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.IgnoredGate> webApi = ignoredGates != null ? ignoredGates.ToWebApi() : (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.IgnoredGate>) null;
      releaseGatesPhase.IgnoredGates = webApi;
      webApiGatesPhase.SucceedingSince = serverDeploymentGate.SucceedingSince;
    }

    public static void FillReleaseGatesPhaseWithDeploymentGate(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseGatesPhase serverGatesPhase,
      DeploymentGate serverDeploymentGate)
    {
      if (serverGatesPhase == null || serverDeploymentGate == null)
        return;
      serverGatesPhase.StabilizationCompletedOn = serverDeploymentGate.StabilizationCompletedOn;
      serverGatesPhase.SucceedingSince = serverDeploymentGate.SucceedingSince;
      if (serverDeploymentGate.IgnoredGates == null || !serverDeploymentGate.IgnoredGates.Any<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate>())
        return;
      serverGatesPhase.IgnoredGates = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate>();
      serverDeploymentGate.IgnoredGates.ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate>().ForEach((Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate>) (ignoredGate => serverGatesPhase.IgnoredGates.Add(ignoredGate.DeepClone())));
    }

    public static bool IsGatesPhase(this DeployPhaseSnapshot deployPhase) => deployPhase != null && deployPhase.PhaseType == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.DeploymentGates;

    private static BaseDeploymentInput GetDeploymentInput(
      BaseDeploymentInput deploymentInput,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes phaseType,
      IDictionary<string, string> variables)
    {
      if (deploymentInput == null || deploymentInput.OverrideInputs == null || deploymentInput.OverrideInputs.Count == 0 || variables == null || variables.Count == 0)
        return deploymentInput;
      switch (phaseType)
      {
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.AgentBasedDeployment:
          return DeployPhaseExtensions.GetAgentDeploymentInput(deploymentInput, variables);
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.RunOnServer:
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.DeploymentGates:
          return deploymentInput;
        case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeployPhaseTypes.MachineGroupBasedDeployment:
          return DeployPhaseExtensions.GetMachineGroupDeploymentInput(deploymentInput, variables);
        default:
          throw new NotSupportedException();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Will log details")]
    private static BaseDeploymentInput GetAgentDeploymentInput(
      BaseDeploymentInput deploymentInput,
      IDictionary<string, string> variables)
    {
      Type type = typeof (AgentDeploymentInput);
      BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public;
      IDictionary<string, object> deploymentInputProperties = DeployPhaseExtensions.GetValidDeploymentInputProperties((IList<PropertyInfo>) type.GetProperties(bindingAttr), deploymentInput.OverrideInputs, variables);
      AgentDeploymentInput target = (AgentDeploymentInput) deploymentInput;
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) deploymentInputProperties)
      {
        try
        {
          type.InvokeMember(keyValuePair.Key, bindingAttr | BindingFlags.NonPublic | BindingFlags.SetProperty, (Binder) null, (object) target, new object[1]
          {
            keyValuePair.Value
          }, CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
        }
      }
      return (BaseDeploymentInput) target;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Will log details")]
    private static BaseDeploymentInput GetMachineGroupDeploymentInput(
      BaseDeploymentInput deploymentInput,
      IDictionary<string, string> variables)
    {
      Type type = typeof (MachineGroupDeploymentInput);
      BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public;
      IDictionary<string, object> deploymentInputProperties = DeployPhaseExtensions.GetValidDeploymentInputProperties((IList<PropertyInfo>) type.GetProperties(bindingAttr), deploymentInput.OverrideInputs, variables);
      MachineGroupDeploymentInput target = (MachineGroupDeploymentInput) deploymentInput;
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) deploymentInputProperties)
      {
        try
        {
          type.InvokeMember(keyValuePair.Key, bindingAttr | BindingFlags.NonPublic | BindingFlags.SetProperty, (Binder) null, (object) target, new object[1]
          {
            keyValuePair.Value
          }, CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
        }
      }
      return (BaseDeploymentInput) target;
    }

    private static IDictionary<string, object> GetValidDeploymentInputProperties(
      IList<PropertyInfo> inputProperties,
      IDictionary<string, string> overrideInputs,
      IDictionary<string, string> variables)
    {
      Dictionary<string, object> deploymentInputProperties = new Dictionary<string, object>();
      overrideInputs = (IDictionary<string, string>) overrideInputs.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (v => v.Key), (Func<KeyValuePair<string, string>, string>) (v => v.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (PropertyInfo inputProperty in (IEnumerable<PropertyInfo>) inputProperties)
      {
        string str1;
        if (overrideInputs.TryGetValue(inputProperty.Name, out str1) && str1 != null)
        {
          int num = BuildCommonUtil.IsEnvironmentVariable(str1) ? 1 : 0;
          string str2 = BuildCommonUtil.ExpandEnvironmentVariables(str1, variables, (Func<string, string, string>) ((m, e) => e));
          if (num == 0 || !string.Equals(str2, str1))
          {
            if (typeof (int).IsAssignableFrom(inputProperty.PropertyType))
            {
              int result;
              if (int.TryParse(str2, out result))
                deploymentInputProperties[inputProperty.Name] = (object) result;
            }
            else if (typeof (bool).IsAssignableFrom(inputProperty.PropertyType))
            {
              bool result;
              if (bool.TryParse(str2, out result))
                deploymentInputProperties[inputProperty.Name] = (object) result;
            }
            else if (typeof (IList<string>).IsAssignableFrom(inputProperty.PropertyType))
            {
              IList<string> stringList;
              if (JsonUtilities.TryDeserialize<IList<string>>(str2, out stringList))
                deploymentInputProperties[inputProperty.Name] = (object) stringList;
            }
            else if (typeof (string).IsAssignableFrom(inputProperty.PropertyType))
              deploymentInputProperties[inputProperty.Name] = (object) str2;
          }
        }
      }
      return (IDictionary<string, object>) deploymentInputProperties;
    }

    private static void ValidateTaskOverrideInputs(
      WorkflowTask task,
      string phaseName,
      string environmentName,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variables)
    {
      string b = "TimeoutInMinutes";
      string key = task.OverrideInputs.First<KeyValuePair<string, string>>().Key;
      string str1 = task.OverrideInputs.First<KeyValuePair<string, string>>().Value;
      if (task.OverrideInputs.Count > 1 || task.OverrideInputs.Count == 1 && !string.Equals(key, b, StringComparison.OrdinalIgnoreCase))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidTaskOverrideInputs, (object) task.Name, (object) phaseName, (object) environmentName, (object) b));
      if (str1 == null)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidTaskOverrideInputValue, (object) task.Name, (object) phaseName, (object) environmentName, (object) b));
      Dictionary<string, string> dictionary = variables.Where<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>, bool>) (v => v.Value != null && !v.Value.IsSecret)).ToDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>, string, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>, string>) (v => v.Key), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>, string>) (v => v.Value.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      int num = BuildCommonUtil.IsEnvironmentVariable(str1) ? 1 : 0;
      string str2 = BuildCommonUtil.ExpandEnvironmentVariables(str1, (IDictionary<string, string>) dictionary, (Func<string, string, string>) ((m, e) => e));
      if (num != 0 && string.Equals(str2, str1))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidTaskOverrideInputEnvironmentVariable, (object) str1, (object) task.Name, (object) phaseName, (object) environmentName));
      int result;
      if (!int.TryParse(str2, out result) || result < 0)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidTaskOverrideInputValue, (object) task.Name, (object) phaseName, (object) environmentName, (object) b));
    }
  }
}
