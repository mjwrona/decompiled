// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.DeployTimeVariableValidations
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  public static class DeployTimeVariableValidations
  {
    public static void ValidateDeployTimeVariables(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      CreateReleaseParameters createReleaseParameters)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      if (createReleaseParameters == null)
        throw new ArgumentNullException(nameof (createReleaseParameters));
      DeployTimeVariableValidations.ValidateReleaseVariables(releaseDefinition, createReleaseParameters);
      DeployTimeVariableValidations.ValidateEnvironmentVariables(releaseDefinition, createReleaseParameters);
      DeployTimeVariableValidations.CheckPermissions(requestContext, projectId, releaseDefinition, createReleaseParameters.EnvironmentsMetadata);
    }

    public static void ValidateDeployTimeEndpointVariable(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApi = ReleaseDefinitionConverter.ToWebApi(requestContext, projectId, releaseDefinition);
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release contract = release.ConvertModelToContract(requestContext, projectId);
      IList<TaskDefinition> taskDefinitions = requestContext.GetService<IDistributedTaskPoolService>().GetTaskDefinitions(requestContext.Elevate());
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> byVariableUpdate = ServiceEndpointVariablesHelper.GetReleaseEnvironmentsImpactedByVariableUpdate(requestContext, webApi, contract, taskDefinitions);
      if (!byVariableUpdate.Any<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>())
        return;
      ServiceEndpointPermissionValidator.EnsureServiceEndpointSecurityPermission(requestContext, projectId, byVariableUpdate, contract.Variables, (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release) null, taskDefinitions);
    }

    public static void ValidateReleaseEnvironmentDeployTimeVariables(
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> serverVariables,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> deployTimeVariables,
      string environmentName)
    {
      DeployTimeVariableValidations.EnsureThatNoNewVariablesAreAdded(serverVariables, deployTimeVariables, environmentName, false);
      DeployTimeVariableValidations.EnsureThatAllVariablesAreOverridable(serverVariables, deployTimeVariables, environmentName);
      DeployTimeVariableValidations.EnsureThatSecrecyOfTheVariablesIsNotModified(serverVariables, deployTimeVariables, environmentName, false);
    }

    private static void ValidateReleaseVariables(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      CreateReleaseParameters parameters)
    {
      DeployTimeVariableValidations.EnsureThatNoNewVariablesAreAdded(releaseDefinition.Variables, parameters.Variables, Resources.DeployTimeVariablesReleaseScope);
      DeployTimeVariableValidations.EnsureThatAllVariablesAreOverridable(releaseDefinition.Variables, parameters.Variables, Resources.DeployTimeVariablesReleaseScope);
      DeployTimeVariableValidations.EnsureThatSecrecyOfTheVariablesIsNotModified(releaseDefinition.Variables, parameters.Variables, Resources.DeployTimeVariablesReleaseScope);
    }

    private static void EnsureThatNoNewVariablesAreAdded(
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> serverVariables,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> deployTimeVariables,
      string scope,
      bool isReleaseCreationValidation = true)
    {
      if (deployTimeVariables.IsNullOrEmpty<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>())
        return;
      List<string> list = deployTimeVariables.Where<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, bool>) (rv => !serverVariables.ContainsKey(rv.Key))).Select<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (rv => rv.Key)).ToList<string>();
      if (list.Any<string>())
      {
        string empty = string.Empty;
        throw new InvalidRequestException(!isReleaseCreationValidation ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UpdateReleaseEnvironmentNoNewVariableAllowed, (object) string.Join(", ", (IEnumerable<string>) list), (object) scope) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeploymentTimeCannotAddNewVariables, (object) string.Join(", ", (IEnumerable<string>) list), (object) scope));
      }
    }

    private static void EnsureThatAllVariablesAreOverridable(
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> serverVariables,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> deployTimeVariables,
      string scope)
    {
      if (deployTimeVariables.IsNullOrEmpty<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>())
        return;
      List<string> list = deployTimeVariables.Where<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, bool>) (dv => !serverVariables[dv.Key].AllowOverride)).Select<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (dv => dv.Key)).ToList<string>();
      if (list.Any<string>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeploymentTimeVariablesCannotBeOverridden, (object) string.Join(", ", (IEnumerable<string>) list), (object) scope));
    }

    private static void EnsureThatSecrecyOfTheVariablesIsNotModified(
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> serverVariables,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> deployTimeVariables,
      string scope,
      bool isReleaseCreationValidation = true)
    {
      if (deployTimeVariables.IsNullOrEmpty<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>())
        return;
      List<string> list = deployTimeVariables.Where<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, bool>) (dv => serverVariables[dv.Key].IsSecret != dv.Value.IsSecret)).Select<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (dv => dv.Key)).ToList<string>();
      if (list.Any<string>())
      {
        string empty = string.Empty;
        throw new InvalidRequestException(!isReleaseCreationValidation ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseEnvironmentUpdateVariablesSecrecyCannotBeChanged, (object) string.Join(", ", (IEnumerable<string>) list), (object) scope) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeployTimeVariablesSecrecyCannotBeChanged, (object) string.Join(", ", (IEnumerable<string>) list), (object) scope));
      }
    }

    private static void ValidateEnvironmentVariables(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      CreateReleaseParameters parameters)
    {
      if (parameters.EnvironmentsMetadata.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseStartEnvironmentMetadata>())
        return;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseStartEnvironmentMetadata environmentMetadata in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseStartEnvironmentMetadata>) parameters.EnvironmentsMetadata)
        DeployTimeVariableValidations.ValidateEnvironmentVariables(releaseDefinition, environmentMetadata);
    }

    private static void ValidateEnvironmentVariables(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseStartEnvironmentMetadata environmentMetadata)
    {
      if (environmentMetadata == null || environmentMetadata.Variables.IsNullOrEmpty<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>())
        return;
      DefinitionEnvironment definitionEnvironment = releaseDefinition.Environments.SingleOrDefault<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (e => e.Id == environmentMetadata.DefinitionEnvironmentId));
      if (definitionEnvironment == null)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeploymentTimeVariablesInvalidEnvironment, (object) environmentMetadata.DefinitionEnvironmentId, (object) string.Join(", ", environmentMetadata.Variables.Select<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (v => v.Key)))));
      DeployTimeVariableValidations.EnsureThatNoNewVariablesAreAdded(definitionEnvironment.Variables, environmentMetadata.Variables, definitionEnvironment.Name);
      DeployTimeVariableValidations.EnsureThatAllVariablesAreOverridable(definitionEnvironment.Variables, environmentMetadata.Variables, definitionEnvironment.Name);
      DeployTimeVariableValidations.EnsureThatSecrecyOfTheVariablesIsNotModified(definitionEnvironment.Variables, environmentMetadata.Variables, definitionEnvironment.Name);
    }

    private static void CheckPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseStartEnvironmentMetadata> environmentsMetadata)
    {
      if (environmentsMetadata.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseStartEnvironmentMetadata>())
        return;
      environmentsMetadata.ForEach<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseStartEnvironmentMetadata>((Action<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseStartEnvironmentMetadata>) (e => DeployTimeVariableValidations.CheckPermissions(requestContext, projectId, releaseDefinition, e)));
    }

    private static void CheckPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseStartEnvironmentMetadata environmentMetadata)
    {
      if (!environmentMetadata.Variables.IsNullOrEmpty<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>() && !requestContext.HasPermission(projectId, releaseDefinition.Path, releaseDefinition.Id, environmentMetadata.DefinitionEnvironmentId, ReleaseManagementSecurityPermissions.EditReleaseEnvironment))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeployTimeVariablesNoEditEnvironmentPermissions, (object) releaseDefinition.Environments.Single<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (e => e.Id == environmentMetadata.DefinitionEnvironmentId)).Name, (object) string.Join(", ", environmentMetadata.Variables.Select<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (v => v.Key)))));
    }
  }
}
