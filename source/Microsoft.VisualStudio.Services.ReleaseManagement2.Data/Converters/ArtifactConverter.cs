// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ArtifactConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ArtifactConverter
  {
    public static ArtifactSource FromWebApi(
      this Artifact webApiArtifact,
      bool isDefaultToLatestArtifactVersionEnabled)
    {
      return webApiArtifact != null ? webApiArtifact.ToServerSource(isDefaultToLatestArtifactVersionEnabled) : throw new ArgumentNullException(nameof (webApiArtifact));
    }

    public static ArtifactSource ToServerSource(
      this Artifact webApiSource,
      bool isDefaultToLatestArtifactVersionEnabled)
    {
      bool isMultiDefinitionType;
      string key1 = webApiSource != null ? webApiSource.GetDefinitionIdentifier(out isMultiDefinitionType) : throw new ArgumentNullException(nameof (webApiSource));
      if (webApiSource.DefinitionReference == null || !webApiSource.DefinitionReference.ContainsKey(key1))
        throw new ArgumentException(Resources.DefinitionReferenceNotFound);
      ArtifactSource serverSource = new ArtifactSource()
      {
        ArtifactTypeId = webApiSource.Type,
        Alias = webApiSource.Alias,
        SourceId = webApiSource.SourceId,
        IsPrimary = webApiSource.IsPrimary
      };
      serverSource.SourceBranch = ArtifactConverter.GetArtifactSourceBranch(webApiSource);
      string key2 = "defaultVersionType";
      int num = !webApiSource.DefinitionReference.ContainsKey(key2) || webApiSource.DefinitionReference[key2] == null ? 0 : (!webApiSource.DefinitionReference[key2].Id.IsNullOrEmpty<char>() ? 1 : 0);
      string str1 = num != 0 ? webApiSource.DefinitionReference[key2].Id : string.Empty;
      if (num == 0)
      {
        if (isMultiDefinitionType && "Build".Equals(webApiSource.Type, StringComparison.OrdinalIgnoreCase))
        {
          webApiSource.DefinitionReference[key2] = new ArtifactSourceReference()
          {
            Id = "latestType",
            Name = Resources.LatestType
          };
        }
        else
        {
          string str2 = isDefaultToLatestArtifactVersionEnabled ? ArtifactVersionsUtility.GetDefaultIdForDefaultVersionType(webApiSource.Type) : "selectDuringReleaseCreationType";
          string str3 = isDefaultToLatestArtifactVersionEnabled ? ArtifactVersionsUtility.GetDefaultNameForDefaultVersionType(webApiSource.Type) : Resources.SelectDuringReleaseCreationType;
          webApiSource.DefinitionReference[key2] = new ArtifactSourceReference()
          {
            Id = str2,
            Name = str3
          };
        }
      }
      else
      {
        string errorMessage;
        if (!ArtifactConverter.IsValidDefaultVersionTypeForArtifact(str1, webApiSource.Type, out errorMessage))
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(errorMessage);
        string defaultVersionBranchId = ArtifactConverter.GetArtifactDefaultVersionBranchId(webApiSource);
        if (webApiSource.Type == "Build" && string.Equals(str1, "latestWithBuildDefinitionBranchAndTagsType", StringComparison.OrdinalIgnoreCase) && !defaultVersionBranchId.IsNullOrEmpty<char>())
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.BranchNotAllowedForLatestWithBuildDefinitionBranchAndTagsType));
      }
      foreach (KeyValuePair<string, ArtifactSourceReference> keyValuePair in (IEnumerable<KeyValuePair<string, ArtifactSourceReference>>) webApiSource.DefinitionReference)
      {
        ArtifactSourceReference artifactSourceReference = keyValuePair.Value;
        if (artifactSourceReference != null)
        {
          InputValue inputValue = new InputValue()
          {
            Value = artifactSourceReference.Id,
            DisplayValue = artifactSourceReference.Name
          };
          serverSource.SourceData.Add(keyValuePair.Key, inputValue);
        }
      }
      return serverSource;
    }

    public static string GetDefinitionIdentifier(
      this Artifact artifact,
      out bool isMultiDefinitionType)
    {
      if (artifact == null)
        throw new ArgumentNullException(nameof (artifact));
      isMultiDefinitionType = false;
      ArtifactSourceReference artifactSourceReference1;
      if (artifact.DefinitionReference != null && artifact.DefinitionReference.TryGetValue("IsMultiDefinitionType", out artifactSourceReference1))
        bool.TryParse(artifactSourceReference1?.Id, out isMultiDefinitionType);
      ArtifactSourceReference artifactSourceReference2;
      return isMultiDefinitionType && artifact.DefinitionReference != null && artifact.DefinitionReference.TryGetValue("definitions", out artifactSourceReference2) && !artifactSourceReference2.Id.IsNullOrEmpty<char>() ? "definitions" : "definition";
    }

    private static string GetArtifactSourceBranch(Artifact webApiArtifact) => webApiArtifact.DefinitionReference != null && webApiArtifact.DefinitionReference.ContainsKey("branch") ? webApiArtifact.DefinitionReference["branch"].Id : string.Empty;

    private static string GetArtifactDefaultVersionBranchId(Artifact webApiArtifact) => webApiArtifact.DefinitionReference != null && webApiArtifact.DefinitionReference.ContainsKey("defaultVersionBranch") ? webApiArtifact.DefinitionReference["defaultVersionBranch"]?.Id ?? string.Empty : string.Empty;

    public static Artifact ToWebApi(
      this ArtifactSource serverArtifact,
      bool isDefaultToLatestArtifactVersionEnabled)
    {
      if (serverArtifact == null)
        throw new ArgumentNullException(nameof (serverArtifact));
      Artifact webApi = new Artifact()
      {
        Type = serverArtifact.ArtifactTypeId,
        Alias = serverArtifact.Alias,
        DefinitionReference = serverArtifact.ToActualSourceReference(isDefaultToLatestArtifactVersionEnabled),
        SourceId = serverArtifact.SourceId,
        IsPrimary = serverArtifact.IsPrimary,
        IsRetained = serverArtifact.IsRetained
      };
      if (!string.IsNullOrWhiteSpace(serverArtifact.SourceBranch))
        webApi.DefinitionReference["branch"] = new ArtifactSourceReference()
        {
          Id = serverArtifact.SourceBranch,
          Name = serverArtifact.SourceBranch
        };
      return webApi;
    }

    public static ArtifactSourceIdsQueryResult ToWebApi(IEnumerable<ArtifactSource> artifactSources)
    {
      IEnumerable<IGrouping<string, ArtifactSource>> groupings = artifactSources.GroupBy<ArtifactSource, string>((Func<ArtifactSource, string>) (s => s.ArtifactTypeId));
      List<ArtifactSourceId> artifactSourceIdList = new List<ArtifactSourceId>();
      foreach (IGrouping<string, ArtifactSource> source in groupings)
      {
        List<InputValue> list = source.Select<ArtifactSource, InputValue>((Func<ArtifactSource, InputValue>) (artifactSource => artifactSource.DefinitionsData)).ToList<InputValue>();
        artifactSourceIdList.Add(ArtifactConverter.ToArtifactSourceId(source.Key, (IEnumerable<InputValue>) list));
      }
      return new ArtifactSourceIdsQueryResult()
      {
        ArtifactSourceIds = (IList<ArtifactSourceId>) artifactSourceIdList
      };
    }

    private static IDictionary<string, ArtifactSourceReference> ToActualSourceReference(
      this ArtifactSource serverArtifact,
      bool isDefaultToLatestArtifactVersionEnabled)
    {
      Dictionary<string, ArtifactSourceReference> dictionary = serverArtifact.SourceData.ToDictionary<KeyValuePair<string, InputValue>, string, ArtifactSourceReference>((Func<KeyValuePair<string, InputValue>, string>) (data => data.Key), (Func<KeyValuePair<string, InputValue>, ArtifactSourceReference>) (data => ArtifactConverter.ToArtifactSourceReference(data.Value)));
      string key = "createReleaseOnTaggingExistingBuild";
      if (dictionary.ContainsKey(key))
        dictionary.Remove(key);
      if (!isDefaultToLatestArtifactVersionEnabled && !dictionary.ContainsKey("defaultVersionType"))
      {
        if (serverArtifact.IsMultiDefinitionType && serverArtifact.ArtifactTypeId.Equals("Build", StringComparison.OrdinalIgnoreCase))
          dictionary["defaultVersionType"] = new ArtifactSourceReference()
          {
            Id = "latestType",
            Name = Resources.LatestType
          };
        else
          dictionary["defaultVersionType"] = new ArtifactSourceReference()
          {
            Id = "selectDuringReleaseCreationType",
            Name = Resources.SelectDuringReleaseCreationType
          };
      }
      return (IDictionary<string, ArtifactSourceReference>) dictionary;
    }

    private static ArtifactSourceReference ToArtifactSourceReference(InputValue data) => new ArtifactSourceReference()
    {
      Id = data.Value,
      Name = data.DisplayValue
    };

    private static ArtifactSourceId ToArtifactSourceId(
      string artifactTypeId,
      IEnumerable<InputValue> inputValues)
    {
      return new ArtifactSourceId()
      {
        ArtifactTypeId = artifactTypeId,
        SourceIdInputs = (IList<SourceIdInput>) ArtifactConverter.ToSourceIdInputs(inputValues)
      };
    }

    private static List<SourceIdInput> ToSourceIdInputs(IEnumerable<InputValue> inputValues) => inputValues.Select<InputValue, SourceIdInput>(ArtifactConverter.\u003C\u003EO.\u003C0\u003E__ToSourceId ?? (ArtifactConverter.\u003C\u003EO.\u003C0\u003E__ToSourceId = new Func<InputValue, SourceIdInput>(ArtifactConverter.ToSourceId))).ToList<SourceIdInput>();

    private static SourceIdInput ToSourceId(InputValue inputValue) => new SourceIdInput()
    {
      Id = inputValue.Value,
      Name = inputValue.DisplayValue
    };

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Required to get the errorMessage along with validity check")]
    public static bool IsValidDefaultVersionTypeForArtifact(
      string defaultVersionTypeId,
      string artifactType,
      out string errorMessage)
    {
      errorMessage = string.Empty;
      IList<string> defaultVersionTypes = ArtifactVersionsUtility.GetSupportedDefaultVersionTypes(artifactType);
      bool flag = false;
      Func<string, bool> predicate = (Func<string, bool>) (supportedDefaultVersionType => string.Equals(defaultVersionTypeId, supportedDefaultVersionType, StringComparison.OrdinalIgnoreCase));
      if (defaultVersionTypes.Any<string>(predicate))
        flag = true;
      else
        errorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DefaultVersionTypeNotAllowedForArtifactType, (object) defaultVersionTypeId, (object) artifactType, (object) ArtifactVersionsUtility.GetSupportedDefaultVersionTypesAsString(artifactType));
      return flag;
    }
  }
}
