// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ArtifactVersionsService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ArtifactVersionsService : ReleaseManagement2ServiceBase
  {
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public ArtifactVersionQueryResult GetArtifactVersions(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int releaseDefinitionId)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      bool isDefaultToLatestArtifactVersionEnabled = requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.DefaultToLatestArtifactVersion");
      Func<ReleaseDefinitionSqlComponent, ReleaseDefinition> action = (Func<ReleaseDefinitionSqlComponent, ReleaseDefinition>) (component => component.GetReleaseDefinition(projectInfo.Id, releaseDefinitionId, isDefaultToLatestArtifactVersionEnabled: isDefaultToLatestArtifactVersionEnabled));
      IList<ArtifactSource> linkedArtifacts = requestContext.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, ReleaseDefinition>(action).LinkedArtifacts;
      ArtifactUtility.ResolveSourceBranchVariablesForArtifactSources(requestContext, projectInfo.Id, linkedArtifacts, releaseDefinitionId);
      return ArtifactVersionsService.GetVersionsForArtifactSources(requestContext, projectInfo, (IEnumerable<ArtifactSource>) linkedArtifacts);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public ArtifactVersionQueryResult GetArtifactVersions(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      IList<ArtifactSource> serverArtifactSources)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (serverArtifactSources == null)
        throw new ArgumentNullException(nameof (serverArtifactSources));
      return ArtifactVersionsService.GetVersionsForArtifactSources(requestContext, projectInfo, (IEnumerable<ArtifactSource>) serverArtifactSources);
    }

    public virtual InputValue GetLatestArtifactVersion(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      ArtifactSource artifactSource)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      Dictionary<string, string> sourceInputs = artifactSource != null ? ArtifactVersionsService.GetSourceInputs(artifactSource) : throw new ArgumentNullException(nameof (artifactSource));
      return requestContext.GetService<ArtifactTypeServiceBase>().GetArtifactType(requestContext, artifactSource.ArtifactTypeId).GetLatestVersion(requestContext, (IDictionary<string, string>) sourceInputs, projectInfo);
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to throw exception, but want to set the error message.")]
    private static ArtifactVersionQueryResult GetVersionsForArtifactSources(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      IEnumerable<ArtifactSource> distinctArtifactSources)
    {
      ArtifactVersionQueryResult forArtifactSources = new ArtifactVersionQueryResult();
      List<ArtifactVersion> artifactVersionList = new List<ArtifactVersion>();
      ArtifactTypeServiceBase service = requestContext.GetService<ArtifactTypeServiceBase>();
      foreach (ArtifactSource distinctArtifactSource in distinctArtifactSources)
      {
        ArtifactTypeBase artifactType = service.GetArtifactType(requestContext, distinctArtifactSource.ArtifactTypeId);
        List<InputValue> inputValueList = new List<InputValue>();
        InputValue defaultVersion = (InputValue) null;
        Exception queryException = (Exception) null;
        try
        {
          Dictionary<string, string> sourceInputs1 = ArtifactVersionsService.GetSourceInputs(distinctArtifactSource);
          Dictionary<string, string> sourceInputs2 = ArtifactVersionsService.RemoveDefaultVersionFilters((IDictionary<string, string>) sourceInputs1);
          inputValueList = artifactType.GetAvailableVersions(requestContext, (IDictionary<string, string>) sourceInputs2, projectInfo).ToList<InputValue>();
          defaultVersion = ArtifactVersionsService.EnsureDefaultVersionIsAddedToAvailableVersions(requestContext, projectInfo, inputValueList, (IDictionary<string, string>) sourceInputs1, artifactType);
        }
        catch (Exception ex)
        {
          queryException = ex;
          requestContext.TraceException(1972007, TraceLevel.Verbose, "ReleaseManagementService", "Service", ex);
        }
        if (defaultVersion != null && !inputValueList.Any<InputValue>((Func<InputValue, bool>) (x => x.Value.Equals(defaultVersion.Value, StringComparison.OrdinalIgnoreCase))))
          inputValueList.Add(defaultVersion);
        ArtifactVersion artifactVersion = ArtifactVersionsService.ToArtifactVersion(distinctArtifactSource, (IList<InputValue>) inputValueList, defaultVersion, queryException);
        artifactVersionList.Add(artifactVersion);
      }
      forArtifactSources.ArtifactVersions = (IList<ArtifactVersion>) artifactVersionList;
      return forArtifactSources;
    }

    private static ArtifactVersion ToArtifactVersion(
      ArtifactSource artifactSource,
      IList<InputValue> versions,
      InputValue version,
      Exception queryException)
    {
      string message = queryException?.Message;
      IList<BuildVersion> buildVersions = ArtifactVersionsService.ToBuildVersions(versions);
      BuildVersion buildVersion = ArtifactVersionsService.ToBuildVersion(version);
      return new ArtifactVersion()
      {
        Versions = buildVersions,
        ErrorMessage = message,
        SourceId = artifactSource.SourceId,
        Alias = artifactSource.Alias,
        DefaultVersion = buildVersion
      };
    }

    private static IList<BuildVersion> ToBuildVersions(IList<InputValue> inputValues) => inputValues == null ? (IList<BuildVersion>) null : (IList<BuildVersion>) inputValues.Select<InputValue, BuildVersion>(ArtifactVersionsService.\u003C\u003EO.\u003C0\u003E__ToBuildVersion ?? (ArtifactVersionsService.\u003C\u003EO.\u003C0\u003E__ToBuildVersion = new Func<InputValue, BuildVersion>(ArtifactVersionsService.ToBuildVersion))).ToList<BuildVersion>();

    private static BuildVersion ToBuildVersion(InputValue inputValue)
    {
      if (inputValue == null)
        return (BuildVersion) null;
      BuildVersion buildVersion = new BuildVersion()
      {
        Id = inputValue.Value,
        Name = inputValue.DisplayValue
      };
      if (inputValue.Data != null)
      {
        bool result;
        if (((!inputValue.Data.ContainsKey("isMultiDefinitionType") ? 0 : (bool.TryParse(inputValue.Data["isMultiDefinitionType"]?.ToString(), out result) ? 1 : 0)) & (result ? 1 : 0)) != 0)
          buildVersion.IsMultiDefinitionType = result;
        if (inputValue.Data.ContainsKey("definitionId"))
          buildVersion.DefinitionId = inputValue.Data["definitionId"]?.ToString();
        if (inputValue.Data.ContainsKey("definitionName"))
          buildVersion.DefinitionName = inputValue.Data["definitionName"]?.ToString();
        if (inputValue.Data.ContainsKey("branch"))
          buildVersion.SourceBranch = inputValue.Data["branch"]?.ToString();
        if (inputValue.Data.ContainsKey("sourceVersion"))
          buildVersion.SourceVersion = inputValue.Data["sourceVersion"]?.ToString();
        if (inputValue.Data.ContainsKey("repositoryId"))
          buildVersion.SourceRepositoryId = inputValue.Data["repositoryId"]?.ToString();
        if (inputValue.Data.ContainsKey("repositoryType"))
          buildVersion.SourceRepositoryType = inputValue.Data["repositoryType"]?.ToString();
        if (inputValue.Data.ContainsKey("commitMessage"))
          buildVersion.CommitMessage = inputValue.Data["commitMessage"]?.ToString();
      }
      return buildVersion;
    }

    private static Dictionary<string, string> GetSourceInputs(ArtifactSource artifactSource)
    {
      Dictionary<string, string> sourceInputs = new Dictionary<string, string>();
      foreach (KeyValuePair<string, InputValue> keyValuePair in artifactSource.SourceData)
        sourceInputs.Add(keyValuePair.Key, keyValuePair.Value.Value);
      return sourceInputs;
    }

    private static Dictionary<string, string> RemoveDefaultVersionFilters(
      IDictionary<string, string> sourceInputs)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (string key in (IEnumerable<string>) sourceInputs.Keys)
      {
        if (!key.Equals("defaultVersionBranch", StringComparison.OrdinalIgnoreCase) && !key.Equals("defaultVersionSpecific", StringComparison.OrdinalIgnoreCase) && !key.Equals("defaultVersionTags", StringComparison.OrdinalIgnoreCase) && !key.Equals("defaultVersionType", StringComparison.OrdinalIgnoreCase))
          dictionary.Add(key, sourceInputs[key]);
      }
      return dictionary;
    }

    private static InputValue EnsureDefaultVersionIsAddedToAvailableVersions(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      List<InputValue> versions,
      IDictionary<string, string> sourceInputs,
      ArtifactTypeBase artifactExtensionType)
    {
      InputValue availableVersions;
      try
      {
        availableVersions = ArtifactVersionsService.GetDefaultVersion(requestContext, projectInfo, versions, sourceInputs, artifactExtensionType);
      }
      catch (InvalidOperationException ex)
      {
        availableVersions = artifactExtensionType.GetLatestVersion(requestContext, sourceInputs, projectInfo);
        if (availableVersions == null)
          throw new InvalidOperationException(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.LatestArtifactVersionUnavailable);
      }
      string defaultVersionType = sourceInputs.GetDefaultVersionType();
      if (!string.IsNullOrEmpty(defaultVersionType) && availableVersions == null && !string.Equals(defaultVersionType, "selectDuringReleaseCreationType", StringComparison.OrdinalIgnoreCase))
      {
        string versionTypesAsString = ArtifactVersionsUtility.GetSupportedDefaultVersionTypesAsString(artifactExtensionType.Name);
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.DefaultVersionTypeNotSupported, (object) defaultVersionType, (object) versionTypesAsString));
      }
      return availableVersions;
    }

    private static InputValue GetDefaultVersion(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      List<InputValue> versions,
      IDictionary<string, string> sourceInputs,
      ArtifactTypeBase artifactExtensionType)
    {
      string defaultVersionType = sourceInputs.GetDefaultVersionType();
      if (string.IsNullOrEmpty(defaultVersionType))
        return (InputValue) null;
      switch (defaultVersionType)
      {
        case "specificVersionType":
          return sourceInputs.GetDefaultVersionSpecificValue((IList<InputValue>) versions);
        case "latestType":
          if (artifactExtensionType.SupportsLatestVersionDataSourceBinding())
          {
            InputValue latestVersion = artifactExtensionType.GetLatestVersion(requestContext, sourceInputs, projectInfo);
            if (latestVersion != null)
              return latestVersion;
          }
          return sourceInputs.GetDefaultVersionLatestValue((IList<InputValue>) versions);
        case "latestWithBuildDefinitionBranchAndTagsType":
          return sourceInputs.GetDefaultVersionLatestWithBuildDefinitionBranchAndTagsValue(requestContext, (IList<InputValue>) versions);
        case "latestFromBranchType":
          return sourceInputs.GetDefaultVersionLatestFromBranchValue((IList<InputValue>) versions);
        case "latestWithBranchAndTagsType":
          return sourceInputs.GetDefaultVersionLatestWithBranchAndTagsValue((IList<InputValue>) versions);
        case "selectDuringReleaseCreationType":
          return (InputValue) null;
        default:
          return (InputValue) null;
      }
    }
  }
}
