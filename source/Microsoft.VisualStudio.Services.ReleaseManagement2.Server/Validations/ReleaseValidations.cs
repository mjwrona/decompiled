// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.ReleaseValidations
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  public static class ReleaseValidations
  {
    private static List<string> preFetchedArtifactSourceDataVariable = new List<string>()
    {
      "sourceVersion",
      "buildUri",
      "requestedForId",
      "requestedFor",
      "repository.provider",
      "repository.name",
      "repository.id",
      "IsXamlBuildArtifactType"
    };

    public static void ValidateRelease(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease,
      IVssRequestContext context)
    {
      if (webApiRelease == null)
        throw new ArgumentNullException(nameof (webApiRelease));
      if (serverRelease == null)
        throw new ArgumentNullException(nameof (serverRelease));
      ReleaseValidations.ReleaseIdShouldBeConsistent(webApiRelease, serverRelease);
      ReleaseValidations.ReleaseShouldBeInEditableState(serverRelease);
      ReleaseValidations.ReleaseNameShouldNotBeEmpty(webApiRelease);
      ReleaseValidations.ReleaseNameLengthShouldNotExceedLimit(webApiRelease);
      ReleaseValidations.EnvironmentsShouldNotBeAddedOrRemoved(webApiRelease, serverRelease);
      ReleaseValidations.CopyPreFetchedArtifactSourceDataVariable(webApiRelease, serverRelease);
      ReleaseValidations.ArtifactSourcesShouldBeConsistent(webApiRelease, serverRelease);
      ReleaseValidations.EnsureEnvironmentNamesAreNotModified(webApiRelease, serverRelease);
      ReleaseValidations.ValidateReleaseIdIsSpecifiedPerEnvironment(webApiRelease);
      ReleaseValidations.ValidateDeployPhaseWorkflowNotModified(webApiRelease, serverRelease);
      ReleaseValidations.ValidateReleaseVariableGroups(webApiRelease, serverRelease);
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) webApiRelease.Environments)
      {
        environment.Validate(webApiRelease, context);
        ReleaseValidations.ValidateEnvironmentVariableGroups(environment, serverRelease.Environments);
      }
      if (serverRelease.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Draft)
        return;
      ReleaseValidations.ArtifactSourcesShouldNotBeModified(webApiRelease, serverRelease);
      ReleaseValidations.InprogressEnvironmentsShouldNotBeModified(webApiRelease, serverRelease);
    }

    public static void CheckForInvalidEnvironmentNames(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      IList<string> environmentNameList)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (environmentNameList.IsNullOrEmpty<string>())
        return;
      List<string> allEnvironmentNames = release.Environments.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, string>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, string>) (env => env.Name)).ToList<string>();
      List<string> list = environmentNameList.Where<string>((Func<string, bool>) (environmentName => !allEnvironmentNames.Contains<string>(environmentName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))).ToList<string>();
      if (!list.IsNullOrEmpty<string>())
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.EnvironmentNamesAreIncorrect, (object) string.Join(",", (IEnumerable<string>) list)));
    }

    private static void ValidateDeployPhaseWorkflowNotModified(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> mergedGroupVariables1 = VariableGroupsMerger.GetMergedGroupVariables(webApiRelease.VariableGroups);
      Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dictionary1 = VariableGroupsMerger.MergeVariableGroups(serverRelease.VariableGroups).ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) (p => new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
      {
        Value = p.Value.Value,
        IsSecret = p.Value.IsSecret
      }));
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) webApiRelease.Environments)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environmentByName = serverRelease.GetEnvironmentByName(environment.Name);
        if (environment.DeployPhasesSnapshot.Count != environmentByName.GetDesignerDeployPhaseSnapshots().Count)
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDeployPhasesSnapshotsCannotBeAddedOrDeleted, (object) environment.Name));
        IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> mergedGroupVariables2 = VariableGroupsMerger.GetMergedGroupVariables(environment.VariableGroups);
        Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dictionary2 = VariableGroupsMerger.MergeVariableGroups(environmentByName.VariableGroups).ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) (p => new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
        {
          Value = p.Value.Value,
          IsSecret = p.Value.IsSecret
        }));
        Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> contractVariables = environment.ProcessParameters.GetProcessParametersAsWebContractVariables();
        Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dataModelVariables = environmentByName.ProcessParameters.GetProcessParametersAsDataModelVariables();
        IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variables1 = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>[5]
        {
          (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>) contractVariables,
          environment.Variables,
          mergedGroupVariables2,
          webApiRelease.Variables,
          mergedGroupVariables1
        });
        IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables2 = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>[5]
        {
          (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) dataModelVariables,
          environmentByName.Variables,
          (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) dictionary2,
          serverRelease.Variables,
          (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) dictionary1
        });
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase1 in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) environment.DeployPhasesSnapshot)
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase = deployPhase1;
          ReleaseEnvironmentValidations.ValidateDeployPhaseSnapshotImmutablePropertyIsNotModified(deployPhase, (IEnumerable<DeployPhaseSnapshot>) environmentByName.GetDesignerDeployPhaseSnapshots(), environment.Name);
          DeployPhaseSnapshot deployPhaseSnapshot = environmentByName.GetDesignerDeployPhaseSnapshots().Single<DeployPhaseSnapshot>((Func<DeployPhaseSnapshot, bool>) (sdps => sdps.Rank == deployPhase.Rank));
          ReleaseEnvironmentValidations.WorkflowTasksShouldNotBeAddedOrRemoved(deployPhase, deployPhaseSnapshot, environment.Name);
          BasePhaseValidator.GetValidator(deployPhase.PhaseType, deployPhase.Name).ValidateImmutableDeploymentInputIsNotModified(deployPhase.GetDeploymentInput(variables1), deployPhaseSnapshot.GetDeploymentInput(variables2));
        }
      }
    }

    private static void InprogressEnvironmentsShouldNotBeModified(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment editableEnvironment in webApiRelease.GetNonEditableEnvironments(serverRelease))
      {
        string name = editableEnvironment.Name;
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environmentByName = serverRelease.GetEnvironmentByName(editableEnvironment.Name);
        DefinitionEnvironmentData definitionEnvironmentData = serverRelease.GetDefinitionEnvironmentData(name);
        ReleaseEnvironmentValidations.EnvironmentShouldNotBeModified(editableEnvironment, environmentByName, definitionEnvironmentData);
      }
    }

    private static void ArtifactSourcesShouldNotBeModified(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      IList<ArtifactSource> linkedArtifacts = serverRelease.LinkedArtifacts;
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> artifacts = webApiRelease.Artifacts;
      if (linkedArtifacts.Count != artifacts.Count)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.ArtifactsCannotBeAddedOrRemoved);
      foreach (ArtifactSource artifactSource in (IEnumerable<ArtifactSource>) linkedArtifacts)
      {
        ArtifactSource source = artifactSource;
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact = artifacts.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact, bool>) (e => string.Equals(e.Alias, source.Alias, StringComparison.OrdinalIgnoreCase)));
        if (artifact == null)
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactSourcesCouldNotBeFound, (object) source.Alias));
        foreach (string key in source.SourceData.Keys)
        {
          if (!string.Equals(key, "artifactSourceDefinitionUrl", StringComparison.OrdinalIgnoreCase) && !string.Equals(key, "artifactSourceVersionUrl", StringComparison.OrdinalIgnoreCase) && (!artifact.DefinitionReference.ContainsKey(key) ? 0 : (string.Equals(key, "branch", StringComparison.OrdinalIgnoreCase) ? (ReleaseValidations.IsSameBranch(source.SourceData[key].Value, artifact.DefinitionReference[key].Id) ? 1 : 0) : (source.SourceData[key].Value == artifact.DefinitionReference[key].Id ? 1 : 0))) == 0)
            throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactSourcesCannotBeModified, (object) source.Alias, (object) key, (object) source.SourceData[key].Value, artifact.DefinitionReference.ContainsKey(key) ? (object) artifact.DefinitionReference[key].Id : (object) (string) null));
        }
      }
    }

    private static void ReleaseShouldBeInEditableState(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      if (serverRelease.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Abandoned)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseCannotBeUpdatedFromCurrentState, (object) serverRelease.Name, (object) serverRelease.Status));
    }

    private static void ReleaseIdShouldBeConsistent(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      if (webApiRelease.Id != serverRelease.Id)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.ReleaseIdDoNotMatch);
    }

    private static void ReleaseNameShouldNotBeEmpty(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease)
    {
      if (string.IsNullOrWhiteSpace(webApiRelease.Name))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.ReleaseNameCannotBeEmpty);
    }

    private static void ReleaseNameLengthShouldNotExceedLimit(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease)
    {
      if (webApiRelease.Name.Length > 256)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseManagementObjectNameLengthExceeded, (object) "Release", (object) webApiRelease.Name, (object) 256));
    }

    private static void EnvironmentsShouldNotBeAddedOrRemoved(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      if (!ReleaseValidations.IsElementsCountConsistent<int>((IList<int>) serverRelease.Environments.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, int>) (env => env.Id)).ToList<int>(), (IList<int>) webApiRelease.Environments.Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, int>) (env => env.Id)).ToList<int>()))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.CannotAddOrRemoveEnvironments);
    }

    [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "required validation")]
    private static void CopyPreFetchedArtifactSourceDataVariable(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      IList<ArtifactSource> linkedArtifacts = serverRelease.LinkedArtifacts;
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> artifacts = webApiRelease.Artifacts;
      if (linkedArtifacts == null || artifacts == null)
        return;
      foreach (ArtifactSource artifactSource in (IEnumerable<ArtifactSource>) linkedArtifacts)
      {
        ArtifactSource source = artifactSource;
        if (source.IsBuildArtifact)
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact = artifacts.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact, bool>) (e => string.Equals(e.Alias, source.Alias, StringComparison.OrdinalIgnoreCase)));
          if (artifact != null)
          {
            if (artifact.DefinitionReference == null)
              throw new ArgumentNullException("DefinitionReference");
            foreach (string key in ReleaseValidations.preFetchedArtifactSourceDataVariable)
            {
              InputValue inputValue;
              if (source.SourceData != null && source.SourceData.TryGetValue(key, out inputValue) && inputValue != null)
                artifact.DefinitionReference[key] = new ArtifactSourceReference()
                {
                  Id = inputValue.Value,
                  Name = inputValue.DisplayValue
                };
            }
            InputValue inputValue1;
            if (!source.IsMultiDefinitionType && source.SourceData != null && source.SourceData.TryGetValue("repository", out inputValue1) && inputValue1 != null)
              artifact.DefinitionReference["repository"] = new ArtifactSourceReference()
              {
                Id = inputValue1.Value,
                Name = inputValue1.DisplayValue
              };
          }
        }
      }
    }

    private static void ArtifactSourcesShouldBeConsistent(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      IList<ArtifactSource> linkedArtifacts = serverRelease.LinkedArtifacts;
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> artifacts = webApiRelease.Artifacts;
      if (!ReleaseValidations.IsElementsCountConsistent<string>((IList<string>) linkedArtifacts.Select<ArtifactSource, string>((Func<ArtifactSource, string>) (e => e.Alias)).ToList<string>(), (IList<string>) artifacts.Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact, string>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact, string>) (e => e.Alias)).ToList<string>()))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.CannotAddOrRemoveArtifacts);
      foreach (ArtifactSource artifactSource in (IEnumerable<ArtifactSource>) linkedArtifacts)
      {
        ArtifactSource source = artifactSource;
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact actualArtifact = artifacts.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact, bool>) (e => string.Equals(e.Alias, source.Alias, StringComparison.OrdinalIgnoreCase)));
        if (actualArtifact == null)
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactSourcesCouldNotBeFound, (object) source.Alias));
        string empty = string.Empty;
        if (!ReleaseValidations.ArtifactsKeysAreConsistent(actualArtifact, source, serverRelease.Status, ref empty))
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(empty);
      }
    }

    private static bool ArtifactsKeysAreConsistent(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact actualArtifact,
      ArtifactSource source,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus serverReleaseStatus,
      ref string errorMessage)
    {
      if (actualArtifact.DefinitionReference == null)
      {
        errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactSourcesCannotBeModified, (object) source.Alias, (object) "DefinitionReference", (object) JsonConvert.SerializeObject((object) source.SourceData), null);
        return false;
      }
      foreach (KeyValuePair<string, InputValue> keyValuePair in source.SourceData)
      {
        if (!string.Equals(keyValuePair.Key, "artifactSourceDefinitionUrl", StringComparison.OrdinalIgnoreCase) && !string.Equals(keyValuePair.Key, "artifactSourceVersionUrl", StringComparison.OrdinalIgnoreCase) && (serverReleaseStatus != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Draft || !string.Equals(keyValuePair.Key, "version", StringComparison.OrdinalIgnoreCase)))
        {
          if (!actualArtifact.DefinitionReference.ContainsKey(keyValuePair.Key))
          {
            errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactSourcesCannotBeModified, (object) source.Alias, (object) keyValuePair.Key, (object) source.SourceData[keyValuePair.Key].Value, null);
            return false;
          }
          if (string.Equals(keyValuePair.Key, "branch", StringComparison.OrdinalIgnoreCase))
          {
            if (!ReleaseValidations.IsSameBranch(actualArtifact.DefinitionReference[keyValuePair.Key].Id, source.SourceData[keyValuePair.Key].Value))
            {
              errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactSourcesCannotBeModified, (object) source.Alias, (object) keyValuePair.Key, (object) source.SourceData[keyValuePair.Key].Value, (object) actualArtifact.DefinitionReference[keyValuePair.Key].Id);
              return false;
            }
            if (!ReleaseValidations.IsSameBranch(actualArtifact.DefinitionReference[keyValuePair.Key].Name, source.SourceData[keyValuePair.Key].DisplayValue))
            {
              errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactSourcesCannotBeModified, (object) source.Alias, (object) keyValuePair.Key, (object) source.SourceData[keyValuePair.Key].DisplayValue, (object) actualArtifact.DefinitionReference[keyValuePair.Key].Name);
              return false;
            }
          }
          else
          {
            if (actualArtifact.DefinitionReference[keyValuePair.Key].Id != source.SourceData[keyValuePair.Key].Value)
            {
              errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactSourcesCannotBeModified, (object) source.Alias, (object) keyValuePair.Key, (object) source.SourceData[keyValuePair.Key].Value, (object) actualArtifact.DefinitionReference[keyValuePair.Key].Id);
              return false;
            }
            if (actualArtifact.DefinitionReference[keyValuePair.Key].Name != source.SourceData[keyValuePair.Key].DisplayValue)
            {
              if (source.IsBuildArtifact && (string.Equals(keyValuePair.Key, "definition", StringComparison.OrdinalIgnoreCase) || string.Equals(keyValuePair.Key, "version", StringComparison.OrdinalIgnoreCase)))
              {
                actualArtifact.DefinitionReference[keyValuePair.Key].Name = source.SourceData[keyValuePair.Key].DisplayValue;
                return true;
              }
              errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactSourcesCannotBeModified, (object) source.Alias, (object) keyValuePair.Key, (object) source.SourceData[keyValuePair.Key].DisplayValue, (object) actualArtifact.DefinitionReference[keyValuePair.Key].Name);
              return false;
            }
          }
        }
      }
      if (serverReleaseStatus == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseStatus.Draft || ReleaseValidations.IsSourceBranchDataConsistent(actualArtifact.DefinitionReference, source))
        return true;
      errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactSourcesCannotBeModified, (object) source.Alias, (object) "branch", (object) source.SourceBranch, actualArtifact.DefinitionReference.ContainsKey("branch") ? (object) actualArtifact.DefinitionReference["branch"].Id : (object) (string) null);
      return false;
    }

    private static bool IsSourceBranchDataConsistent(
      IDictionary<string, ArtifactSourceReference> actualSourceReference,
      ArtifactSource source)
    {
      return string.IsNullOrEmpty(source.SourceBranch) || !actualSourceReference.ContainsKey("branch") || actualSourceReference["branch"].Id == source.SourceBranch;
    }

    private static void EnsureEnvironmentNamesAreNotModified(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      Dictionary<int, string> dictionary = webApiRelease.Environments.ToDictionary<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, int, string>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, int>) (e => e.Id), (Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, string>) (e => e.Name));
      Dictionary<int, string> serverEnvironmentNames = serverRelease.Environments.ToDictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, int, string>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, int>) (e => e.Id), (Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, string>) (e => e.Name));
      Func<KeyValuePair<int, string>, bool> predicate = (Func<KeyValuePair<int, string>, bool>) (env => !serverEnvironmentNames.ContainsKey(env.Key) || !string.Equals(serverEnvironmentNames[env.Key], env.Value));
      if (dictionary.Any<KeyValuePair<int, string>>(predicate))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.ReleaseEnvironmentNamesCannotBeModified);
    }

    private static void ValidateReleaseIdIsSpecifiedPerEnvironment(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease)
    {
      int releaseId = webApiRelease.Id;
      if (webApiRelease.Environments.Any<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, bool>) (env => env.ReleaseId != releaseId)))
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(Resources.ReleaseIdCannotBeModified);
    }

    private static bool IsElementsCountConsistent<T>(IList<T> list1, IList<T> list2) => list1.Count<T>() == list2.Count<T>() && list1.Intersect<T>((IEnumerable<T>) list2).Count<T>() == list1.Count<T>();

    private static void ValidateReleaseVariableGroups(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release webApiRelease, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease) => ReleaseValidations.ValidateVariableGroups(webApiRelease.VariableGroups, serverRelease.VariableGroups, false);

    private static void ValidateEnvironmentVariableGroups(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment webApiReleaseEnvironment,
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment> serverReleaseEnvironments)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = serverReleaseEnvironments.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, bool>) (env => env.Id == webApiReleaseEnvironment.Id)) ?? new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment();
      ReleaseValidations.ValidateVariableGroups(webApiReleaseEnvironment.VariableGroups, releaseEnvironment.VariableGroups, true);
    }

    private static void ValidateVariableGroups(
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup> webApiVariableGroupsList,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> serverVariableGroupsList,
      bool isEnvironmentVariableGroup)
    {
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> serverVariableGroups = serverVariableGroupsList ?? (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) new List<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>();
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup> webApiVariableGroups = webApiVariableGroupsList ?? (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup>();
      List<string> list1 = webApiVariableGroups.Where<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup, bool>) (vg => serverVariableGroups.All<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, bool>) (svg => svg.Id != vg.Id)))).Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup, string>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup, string>) (vg => vg.Name)).ToList<string>();
      if (!list1.IsNullOrEmpty<string>())
      {
        string str = string.Join(", ", (IEnumerable<string>) list1);
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, isEnvironmentVariableGroup ? Resources.ReleaseEnvironmentVariableGroupsCannotBeAdded : Resources.ReleaseVariableGroupsCannotBeAdded, (object) str));
      }
      List<string> list2 = serverVariableGroups.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, bool>) (vg => webApiVariableGroups.All<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup, bool>) (wvg => wvg.Id != vg.Id)))).Select<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, string>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, string>) (vg => vg.Name)).ToList<string>();
      if (!list2.IsNullOrEmpty<string>())
      {
        string str = string.Join(", ", (IEnumerable<string>) list2);
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, isEnvironmentVariableGroup ? Resources.ReleaseEnvironmentVariableGroupsCannotBeRemoved : Resources.ReleaseVariableGroupsCannotBeRemoved, (object) str));
      }
      if (serverVariableGroups.Count != webApiVariableGroups.Count)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, isEnvironmentVariableGroup ? Resources.ReleaseEnvironmentCannotHaveDuplicateVariableGroupIds : Resources.ReleaseCannotHaveDuplicateVariableGroupIds));
      ReleaseValidations.ValidateVariableGroupChanges(webApiVariableGroups, serverVariableGroups, isEnvironmentVariableGroup);
    }

    private static void ValidateVariableGroupChanges(
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup> webApiVariableGroups,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> serverVariableGroups,
      bool isEnvironmentVariableGroup)
    {
      List<string> list = serverVariableGroups.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, bool>) (vg => webApiVariableGroups.First<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup, bool>) (wvg => wvg.Id == vg.Id)).Name != vg.Name)).Select<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, string>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, string>) (vg => vg.Name)).ToList<string>();
      if (!list.IsNullOrEmpty<string>())
      {
        string str = string.Join(", ", (IEnumerable<string>) list);
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, isEnvironmentVariableGroup ? Resources.ReleaseEnvironmentVariableGroupsCannotBeRenamed : Resources.ReleaseVariableGroupsCannotBeRenamed, (object) str));
      }
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup apiVariableGroup in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup>) webApiVariableGroups)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup variableGroup = apiVariableGroup;
        if (!serverVariableGroups.First<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, bool>) (vg => vg.Id == variableGroup.Id)).Type.Equals(variableGroup.Type, StringComparison.OrdinalIgnoreCase))
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, isEnvironmentVariableGroup ? Resources.ReleaseEnvironmentVariableGroupTypeCannotBeModified : Resources.ReleaseVariableGroupTypeCannotBeModified, (object) variableGroup.Name));
        ReleaseValidations.ValidateVariableValueLength(variableGroup);
      }
    }

    private static void ValidateVariableValueLength(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableGroup variableGroup)
    {
      if (variableGroup.Variables == null || !variableGroup.Variables.Any<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue>>())
        return;
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.VariableValue>>) variableGroup.Variables)
      {
        ArgumentUtility.CheckStringLength(variable.Key, "variable.Key", 400, 1);
        if (!variable.Value.IsSecret || variable.Value.Value != null)
          ArgumentUtility.CheckStringLength(variable.Value.Value, "variable.Value.Value", 4096);
      }
    }

    private static bool IsSameBranch(string branch1, string branch2) => string.Equals(ArtifactTypeBase.RefToBranchName(branch1), ArtifactTypeBase.RefToBranchName(branch2), StringComparison.OrdinalIgnoreCase);
  }
}
