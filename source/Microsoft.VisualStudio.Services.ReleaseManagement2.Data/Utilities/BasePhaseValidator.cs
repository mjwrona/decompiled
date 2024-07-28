// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.BasePhaseValidator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public abstract class BasePhaseValidator
  {
    public static BasePhaseValidator GetValidator(
      DeployPhaseTypes phaseType,
      string deployPhaseName)
    {
      switch (phaseType)
      {
        case DeployPhaseTypes.AgentBasedDeployment:
          return (BasePhaseValidator) new AgentBasedDeployPhaseValidator(deployPhaseName);
        case DeployPhaseTypes.RunOnServer:
          return (BasePhaseValidator) new RunOnServerDeployPhaseValidator(deployPhaseName);
        case DeployPhaseTypes.MachineGroupBasedDeployment:
          return (BasePhaseValidator) new MachineGroupBasedDeployPhaseValidator(deployPhaseName);
        case DeployPhaseTypes.DeploymentGates:
          return (BasePhaseValidator) new DeploymentGatesPhaseValidator(deployPhaseName);
        default:
          throw new NotSupportedException();
      }
    }

    protected string DeployPhaseName { get; }

    protected BasePhaseValidator(string deployPhaseName) => this.DeployPhaseName = deployPhaseName;

    public void ValidateDeploymentInput(
      BaseDeploymentInput deploymentInput,
      IDictionary<string, ConfigurationVariableValue> variables,
      IList<Artifact> linkedArtifacts,
      IVssRequestContext context)
    {
      if (deploymentInput == null)
        throw new ArgumentNullException(nameof (deploymentInput));
      IDictionary<string, string> invalidInputs = this.GetInvalidInputs(deploymentInput, variables, context);
      int timeoutInMinutes;
      if (deploymentInput.TimeoutInMinutes < 0)
      {
        IDictionary<string, string> dictionary = invalidInputs;
        timeoutInMinutes = deploymentInput.TimeoutInMinutes;
        string str = timeoutInMinutes.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        dictionary.Add("TimeoutInMinutes", str);
      }
      if (deploymentInput.JobCancelTimeoutInMinutes < 1 || deploymentInput.JobCancelTimeoutInMinutes > 60)
      {
        IDictionary<string, string> dictionary = invalidInputs;
        timeoutInMinutes = deploymentInput.JobCancelTimeoutInMinutes;
        string str = timeoutInMinutes.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        dictionary.Add("JobCancelTimeoutInMinutes", str);
      }
      BasePhaseValidator.CheckForInvalidInputs(this.DeployPhaseName, invalidInputs);
      this.ValidateArtifactsDownloadInput(deploymentInput, linkedArtifacts);
    }

    public void ValidateDeploymentInputIsNotModified(
      BaseDeploymentInput webApiDeploymentInput,
      BaseDeploymentInput serverDeploymentInput)
    {
      if (webApiDeploymentInput == null)
        throw new ArgumentNullException(nameof (webApiDeploymentInput));
      IList<string> stringList = serverDeploymentInput != null ? this.GetModifiedProperties(webApiDeploymentInput, serverDeploymentInput) : throw new ArgumentNullException(nameof (serverDeploymentInput));
      if (webApiDeploymentInput.TimeoutInMinutes != serverDeploymentInput.TimeoutInMinutes)
        stringList.Add(BasePhaseValidator.GetPropertyName<int>((Expression<Func<int>>) (() => serverDeploymentInput.TimeoutInMinutes)));
      IDictionary<string, string> overrideInputs1 = webApiDeploymentInput.OverrideInputs;
      IDictionary<string, string> overrideInputs2 = serverDeploymentInput.OverrideInputs;
      string propertyName = BasePhaseValidator.GetPropertyName<IDictionary<string, string>>((Expression<Func<IDictionary<string, string>>>) (() => serverDeploymentInput.OverrideInputs));
      if (overrideInputs1 == null && overrideInputs2 != null || overrideInputs1 != null && overrideInputs2 == null)
        stringList.Add(propertyName);
      if (overrideInputs1 != null && overrideInputs2 != null)
      {
        if (overrideInputs1.Count != overrideInputs2.Count)
          stringList.Add(propertyName);
        foreach (string key in (IEnumerable<string>) overrideInputs1.Keys)
        {
          if (!overrideInputs2.ContainsKey(key) || !string.Equals(overrideInputs1[key], overrideInputs2[key], StringComparison.OrdinalIgnoreCase))
          {
            stringList.Add(propertyName);
            break;
          }
        }
      }
      if (!stringList.IsNullOrEmpty<string>())
      {
        stringList.ToList<string>().Sort();
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDeployPhasesSnapshotDeploymentInputCannotBeModified, (object) this.DeployPhaseName, (object) string.Join(", ", (IEnumerable<string>) stringList)));
      }
    }

    public void ValidateImmutableDeploymentInputIsNotModified(
      BaseDeploymentInput webApiDeploymentInput,
      BaseDeploymentInput serverDeploymentInput)
    {
      IList<string> immutableProperties = this.GetModifiedImmutableProperties(webApiDeploymentInput, serverDeploymentInput);
      if (!immutableProperties.IsNullOrEmpty<string>())
      {
        immutableProperties.ToList<string>().Sort();
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDeployPhasesSnapshotDeploymentInputCannotBeModified, (object) this.DeployPhaseName, (object) string.Join(", ", (IEnumerable<string>) immutableProperties)));
      }
    }

    public abstract void ValidateDeploymentInputOverrideInputs(
      BaseDeploymentInput deploymentInput,
      IDictionary<string, ConfigurationVariableValue> variables,
      string phaseName);

    public abstract void ValidateArtifactsDownloadInput(
      BaseDeploymentInput deploymentInput,
      IList<Artifact> linkedArtifacts);

    protected static void CheckForInvalidInputs(
      string deployPhaseName,
      IDictionary<string, string> invalidInputs)
    {
      if (invalidInputs != null && invalidInputs.Count != 0)
      {
        string str = string.Join(",", (IEnumerable<string>) invalidInputs.Keys);
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidValuesInDeploymentInput, (object) string.Join(",", (IEnumerable<string>) invalidInputs.Values), (object) str, (object) deployPhaseName));
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Required in one method")]
    protected static void ValidateOverrideInputs(
      IList<PropertyInfo> inputProperties,
      IDictionary<string, string> overrideInputs,
      IDictionary<string, ConfigurationVariableValue> variables,
      string phaseName)
    {
      if (inputProperties == null || overrideInputs == null || overrideInputs.Count == 0)
        return;
      if (variables == null)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProcessParametersNotDefinitedForOverrideInputs, (object) phaseName));
      Dictionary<string, string> dictionary = variables.Where<KeyValuePair<string, ConfigurationVariableValue>>((Func<KeyValuePair<string, ConfigurationVariableValue>, bool>) (v => v.Value != null && !v.Value.IsSecret)).ToDictionary<KeyValuePair<string, ConfigurationVariableValue>, string, string>((Func<KeyValuePair<string, ConfigurationVariableValue>, string>) (v => v.Key), (Func<KeyValuePair<string, ConfigurationVariableValue>, string>) (v => v.Value.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      overrideInputs = (IDictionary<string, string>) overrideInputs.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (v => v.Key), (Func<KeyValuePair<string, string>, string>) (v => v.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, string> overrideInput in (IEnumerable<KeyValuePair<string, string>>) overrideInputs)
      {
        string overrideInputKey = overrideInput.Key;
        string str1 = overrideInput.Value;
        if (overrideInputKey == null || str1 == null)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidOverrideInputsKeyValuePair, (object) phaseName));
        PropertyInfo propertyInfo = inputProperties.FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>) (i => string.Equals(i.Name, overrideInputKey, StringComparison.OrdinalIgnoreCase)));
        if (propertyInfo == (PropertyInfo) null)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidOverrideInput, (object) overrideInputKey, (object) phaseName));
        int num = BuildCommonUtil.IsEnvironmentVariable(str1) ? 1 : 0;
        string str2 = BuildCommonUtil.ExpandEnvironmentVariables(str1, (IDictionary<string, string>) dictionary, (Func<string, string, string>) ((m, e) => e));
        if (num != 0 && string.Equals(str2, str1))
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProcessParametersNotDefiniedForOverrideInput, (object) phaseName, (object) overrideInputKey));
        if (typeof (int).IsAssignableFrom(propertyInfo.PropertyType))
        {
          if (!int.TryParse(str2, out int _))
            BasePhaseValidator.ThrowError(overrideInputKey, str2, phaseName);
        }
        else if (typeof (bool).IsAssignableFrom(propertyInfo.PropertyType))
        {
          if (!bool.TryParse(str2, out bool _))
            BasePhaseValidator.ThrowError(overrideInputKey, str2, phaseName);
        }
        else if (typeof (IList<string>).IsAssignableFrom(propertyInfo.PropertyType))
        {
          if (!JsonUtilities.TryDeserialize<IList<string>>(str2, out IList<string> _))
            BasePhaseValidator.ThrowError(overrideInputKey, str2, phaseName);
        }
        else if (!typeof (string).IsAssignableFrom(propertyInfo.PropertyType))
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.PropertyNotAllowedToOverride, (object) overrideInputKey, (object) phaseName));
      }
    }

    protected abstract IDictionary<string, string> GetInvalidInputs(
      BaseDeploymentInput deploymentInput,
      IDictionary<string, ConfigurationVariableValue> variables,
      IVssRequestContext context);

    protected abstract IList<string> GetModifiedProperties(
      BaseDeploymentInput webApiDeploymentInput,
      BaseDeploymentInput serverDeploymentInput);

    protected abstract IList<string> GetModifiedImmutableProperties(
      BaseDeploymentInput webApiDeploymentInput,
      BaseDeploymentInput serverDeploymentInput);

    internal static string GetPropertyName<T>(Expression<Func<T>> propertyExpression) => !(propertyExpression.Body is MemberExpression) ? string.Empty : ((MemberExpression) propertyExpression.Body).Member.Name;

    private static void ThrowError(string propertyName, string value, string phaseName) => throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.OverrideInputPropertyTypeNotMatching, (object) value, (object) propertyName, (object) phaseName));
  }
}
