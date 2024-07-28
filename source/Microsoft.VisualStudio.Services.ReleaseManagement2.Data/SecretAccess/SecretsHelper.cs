// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.SecretAccess.SecretsHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.SecretAccess
{
  public class SecretsHelper
  {
    private readonly ISecretsAccessor accessor;

    public SecretsHelper()
      : this((ISecretsAccessor) new SecretsAccessor())
    {
    }

    protected SecretsHelper(ISecretsAccessor accessor) => this.accessor = accessor;

    public virtual void ReadSecrets(
      IVssRequestContext context,
      Guid projectIdentifier,
      ReleaseDefinition definition)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      if (!definition.HasSecrets())
        return;
      using (ReleaseManagementTimer.Create(context, "Service", "SecretsHelper.ReadSecrets", 1961201))
      {
        Guid drawerId = this.accessor.UnlockDrawer(context, definition.GetDrawerName(projectIdentifier));
        if (drawerId == Guid.Empty)
          return;
        Dictionary<string, ConfigurationVariableValue> dictionary = new Dictionary<string, ConfigurationVariableValue>();
        SecretsHelper.AddConfigVariablesToMap(definition.Variables, definition.SecretVariableLookupPrefix, dictionary);
        foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) definition.Environments)
          SecretsHelper.AddConfigVariablesToMap(environment.Variables, environment.GetSecretVariableLookupPrefix(definition.Revision), dictionary);
        this.ReadSecretVariables(context, drawerId, (IDictionary<string, ConfigurationVariableValue>) dictionary, (IDictionary<string, VariableValue>) null);
      }
    }

    public virtual IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Issue> ReadSecrets(
      IVssRequestContext context,
      Guid projectIdentifier,
      Release release,
      ReleaseEnvironment environment,
      bool includeExternalVariables)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Issue> issues = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Issue>();
      if (!release.HasSecrets())
        return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Issue>) issues;
      Guid drawerId = this.accessor.UnlockDrawer(context, release.GetDrawerName(projectIdentifier));
      if (drawerId == Guid.Empty)
        return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Issue>) issues;
      Dictionary<string, ConfigurationVariableValue> dictionary = new Dictionary<string, ConfigurationVariableValue>();
      Dictionary<string, VariableValue> variablesMap = new Dictionary<string, VariableValue>();
      SecretsHelper.AddConfigVariablesToMap(release.Variables, release.SecretVariableLookupPrefix, dictionary);
      release.VariableGroups.ToList<VariableGroup>().ForEach((Action<VariableGroup>) (group =>
      {
        if (group.Type.Equals("Vsts", StringComparison.OrdinalIgnoreCase))
        {
          SecretsHelper.AddVariablesToMap(group.Variables, release.VariableGroupSecretsKeyPrefix(group.Id), variablesMap);
        }
        else
        {
          if (!includeExternalVariables)
            return;
          SecretsHelper.ReadExternalVariablesAndIgnoreExceptions(group, context, projectIdentifier, issues);
        }
      }));
      SecretsHelper.AddConfigVariablesToMap(environment.Variables, environment.SecretVariableLookupPrefix, dictionary);
      foreach (VariableGroup variableGroup in (IEnumerable<VariableGroup>) environment.VariableGroups)
      {
        if (variableGroup.Type.Equals("Vsts", StringComparison.OrdinalIgnoreCase))
          SecretsHelper.AddVariablesToMap(variableGroup.Variables, environment.VariableGroupSecretsKeyPrefix(variableGroup.Id), variablesMap);
        else if (includeExternalVariables)
          SecretsHelper.ReadExternalVariablesAndIgnoreExceptions(variableGroup, context, projectIdentifier, issues);
      }
      this.ReadSecretVariables(context, drawerId, (IDictionary<string, ConfigurationVariableValue>) dictionary, (IDictionary<string, VariableValue>) variablesMap);
      return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Issue>) issues;
    }

    public virtual void ReadSecrets(
      IVssRequestContext context,
      Guid projectIdentifier,
      ReleaseEnvironmentSnapshotDelta deploymentDelta)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (deploymentDelta == null || deploymentDelta.Variables.IsNullOrEmpty<KeyValuePair<string, ConfigurationVariableValue>>() || !VariablesUtility.HasSecrets(deploymentDelta.Variables))
        return;
      Guid drawerId = this.accessor.UnlockDrawer(context, ReleaseDataModelUtility.GetDrawerName(projectIdentifier, deploymentDelta.ReleaseId));
      if (drawerId == Guid.Empty)
        return;
      Dictionary<string, ConfigurationVariableValue> dictionary = new Dictionary<string, ConfigurationVariableValue>();
      SecretsHelper.AddConfigVariablesToMap(deploymentDelta.Variables, ReleaseEnvironmentSnapshotDelta.GetSecretVariableLookupPrefix(deploymentDelta.ReleaseEnvironmentId, deploymentDelta.DeploymentId), dictionary);
      this.ReadSecretVariables(context, drawerId, (IDictionary<string, ConfigurationVariableValue>) dictionary, (IDictionary<string, VariableValue>) null);
    }

    public virtual void StoreSecrets(
      IVssRequestContext context,
      Guid projectIdentifier,
      ReleaseDefinition definition)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (definition == null)
        throw new ArgumentNullException(nameof (definition));
      if (!definition.HasSecretsWithValues())
        return;
      using (ReleaseManagementTimer.Create(context, "Service", "SecretsHelper.StoreSecrets.ReleaseDefinition", 1961202))
      {
        string drawerName = definition.GetDrawerName(projectIdentifier);
        Guid drawerId = this.accessor.GetOrCreateDrawer(context, drawerName);
        this.StoreSecretVariables(context, drawerId, definition.SecretVariableLookupPrefix, definition.Variables);
        definition.Environments.ToList<DefinitionEnvironment>().ForEach((Action<DefinitionEnvironment>) (env => this.StoreSecretVariables(context, drawerId, env.GetSecretVariableLookupPrefix(definition.Revision), env.Variables)));
      }
    }

    public virtual void StoreSecrets(
      IVssRequestContext context,
      Guid projectIdentifier,
      Release release)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (!release.HasSecretsWithValues())
        return;
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "SecretsHelper.StoreSecrets.Release", 1960004))
      {
        string drawerName = release.GetDrawerName(projectIdentifier);
        Guid drawerId = this.accessor.GetOrCreateDrawer(context, drawerName);
        this.StoreSecretVariables(context, drawerId, release.SecretVariableLookupPrefix, release.Variables);
        release.VariableGroups.ToList<VariableGroup>().ForEach((Action<VariableGroup>) (group =>
        {
          if (!group.Type.Equals("Vsts", StringComparison.OrdinalIgnoreCase))
            return;
          this.StoreSecretVariables(context, drawerId, release.VariableGroupSecretsKeyPrefix(group.Id), group.Variables);
        }));
        release.Environments.ToList<ReleaseEnvironment>().ForEach((Action<ReleaseEnvironment>) (env =>
        {
          this.StoreSecretVariables(context, drawerId, env.SecretVariableLookupPrefix, env.Variables);
          env.VariableGroups.ToList<VariableGroup>().ForEach((Action<VariableGroup>) (group =>
          {
            if (!group.Type.Equals("Vsts", StringComparison.OrdinalIgnoreCase))
              return;
            this.StoreSecretVariables(context, drawerId, env.VariableGroupSecretsKeyPrefix(group.Id), group.Variables);
          }));
        }));
      }
    }

    public virtual void StoreSecrets(
      IVssRequestContext context,
      Guid projectIdentifier,
      ReleaseEnvironmentSnapshotDelta delta)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (delta == null || delta.Variables.IsNullOrEmpty<KeyValuePair<string, ConfigurationVariableValue>>() || !VariablesUtility.HasSecrets(delta.Variables))
        return;
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "SecretsHelper.StoreSecrets.ReleaseEnvironmentDeployment", 1960097))
      {
        string drawerName = ReleaseDataModelUtility.GetDrawerName(projectIdentifier, delta.ReleaseId);
        Guid drawer = this.accessor.GetOrCreateDrawer(context, drawerName);
        this.StoreSecretVariables(context, drawer, ReleaseEnvironmentSnapshotDelta.GetSecretVariableLookupPrefix(delta.ReleaseEnvironmentId, delta.DeploymentId), delta.Variables);
      }
    }

    public void CopySecrets(
      IVssRequestContext context,
      Guid projectId,
      ReleaseDefinition sourceReleaseDefinition,
      ReleaseDefinition targetReleaseDefinition)
    {
      if (targetReleaseDefinition == null)
        throw new ArgumentNullException(nameof (targetReleaseDefinition));
      this.ReadSecrets(context, projectId, sourceReleaseDefinition);
      targetReleaseDefinition.FillSecrets(sourceReleaseDefinition);
    }

    public void DeleteSecrets(
      IVssRequestContext context,
      Guid projectId,
      ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      Guid drawerId = this.accessor.UnlockDrawer(context, releaseDefinition.GetDrawerName(projectId));
      if (drawerId == Guid.Empty)
        return;
      this.accessor.DeleteDrawer(context, drawerId);
    }

    public void DeleteSecrets(IVssRequestContext context, Guid projectId, Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      Guid drawerId = this.accessor.UnlockDrawer(context, release.GetDrawerName(projectId));
      if (drawerId == Guid.Empty)
        return;
      this.accessor.DeleteDrawer(context, drawerId);
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design.")]
    private static void ReadExternalVariablesAndIgnoreExceptions(
      VariableGroup group,
      IVssRequestContext context,
      Guid projectIdentifier,
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Issue> issues)
    {
      try
      {
        group.ReadExternalVariables(context, projectIdentifier);
      }
      catch (Exception ex)
      {
        issues.Add(new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Issue()
        {
          IssueType = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.IssueType.Warning,
          Message = ex.Message
        });
      }
    }

    private static void AddVariablesToMap(
      IDictionary<string, VariableValue> variables,
      string keyPrefix,
      Dictionary<string, VariableValue> variablesMap)
    {
      if (variables == null)
        return;
      foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) variables)
      {
        if (variable.Value != null && variable.Value.IsSecret)
        {
          string key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) keyPrefix, (object) variable.Key);
          variablesMap.Add(key, variable.Value);
        }
      }
    }

    private static void AddConfigVariablesToMap(
      IDictionary<string, ConfigurationVariableValue> variables,
      string keyPrefix,
      Dictionary<string, ConfigurationVariableValue> variablesMap)
    {
      if (variables == null)
        return;
      foreach (KeyValuePair<string, ConfigurationVariableValue> variable in (IEnumerable<KeyValuePair<string, ConfigurationVariableValue>>) variables)
      {
        if (variable.Value != null && variable.Value.IsSecret && variable.Value.Value == null)
        {
          string key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) keyPrefix, (object) variable.Key);
          variablesMap.Add(key, variable.Value);
        }
      }
    }

    private static IEnumerable<KeyValuePair<string, ConfigurationVariableValue>> GetSecretVariablesToBeStored(
      IDictionary<string, ConfigurationVariableValue> secrets)
    {
      return secrets.Where<KeyValuePair<string, ConfigurationVariableValue>>((Func<KeyValuePair<string, ConfigurationVariableValue>, bool>) (s => s.Value != null && s.Value.IsSecret && !string.IsNullOrEmpty(s.Value.Value)));
    }

    private static IDictionary<string, VariableValue> GetSecretVariablesToBeStored(
      IDictionary<string, VariableValue> secrets)
    {
      Dictionary<string, VariableValue> variablesToBeStored = new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, VariableValue> secret in (IEnumerable<KeyValuePair<string, VariableValue>>) secrets)
      {
        if (secret.Value != null && secret.Value.IsSecret && !string.IsNullOrEmpty(secret.Value.Value))
          variablesToBeStored.Add(secret.Key, secret.Value);
      }
      return (IDictionary<string, VariableValue>) variablesToBeStored;
    }

    private void StoreSecretVariables(
      IVssRequestContext context,
      Guid drawerId,
      string prefix,
      IDictionary<string, ConfigurationVariableValue> variables)
    {
      IEnumerable<KeyValuePair<string, ConfigurationVariableValue>> variablesToBeStored = SecretsHelper.GetSecretVariablesToBeStored(variables);
      if (variables.Count == 0)
        return;
      variablesToBeStored.ToList<KeyValuePair<string, ConfigurationVariableValue>>().ForEach((Action<KeyValuePair<string, ConfigurationVariableValue>>) (kvp => this.accessor.StoreValue(context, drawerId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) prefix, (object) kvp.Key), kvp.Value.Value)));
    }

    private void StoreSecretVariables(
      IVssRequestContext context,
      Guid drawerId,
      string keyPrefix,
      IDictionary<string, VariableValue> variables)
    {
      IDictionary<string, VariableValue> variablesToBeStored = SecretsHelper.GetSecretVariablesToBeStored(variables);
      this.accessor.StoreValues(context, drawerId, keyPrefix, variablesToBeStored);
    }

    private void ReadSecretVariables(
      IVssRequestContext context,
      Guid drawerId,
      IDictionary<string, ConfigurationVariableValue> configVariables,
      IDictionary<string, VariableValue> variables)
    {
      int count = configVariables != null ? configVariables.Count : 0;
      if (variables != null)
        count += variables.Count;
      HashSet<string> stringSet = new HashSet<string>(count);
      if (configVariables != null && configVariables.Count > 0)
        stringSet.AddRange<string, HashSet<string>>((IEnumerable<string>) configVariables.Keys);
      if (variables != null && variables.Count > 0)
        stringSet.AddRange<string, HashSet<string>>((IEnumerable<string>) variables.Keys);
      this.accessor.ReadValues(context, drawerId, stringSet, (Action<string, string>) ((lookupKey, secret) =>
      {
        ConfigurationVariableValue configurationVariableValue;
        if (configVariables != null && configVariables.TryGetValue(lookupKey, out configurationVariableValue))
        {
          configurationVariableValue.Value = secret ?? string.Empty;
        }
        else
        {
          VariableValue variableValue;
          if (variables == null || !variables.TryGetValue(lookupKey, out variableValue))
            return;
          variableValue.Value = secret;
        }
      }));
    }
  }
}
