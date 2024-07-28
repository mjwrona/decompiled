// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.InputValuesDictionaryExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public static class InputValuesDictionaryExtension
  {
    public static string GetDefaultVersionType(this IDictionary<string, string> inputValues)
    {
      if (inputValues == null)
        throw new ArgumentNullException(nameof (inputValues));
      string str;
      inputValues.TryGetValue("defaultVersionType", out str);
      return str ?? string.Empty;
    }

    public static string GetDefaultVersionBranchFilter(this IDictionary<string, string> inputValues)
    {
      if (inputValues == null)
        throw new ArgumentNullException(nameof (inputValues));
      string empty = string.Empty;
      if (inputValues.GetDefaultVersionType() == "latestWithBranchAndTagsType")
        inputValues.TryGetValue("defaultVersionBranch", out empty);
      return empty ?? string.Empty;
    }

    public static IList<string> GetDefaultVersionTagsFilter(
      this IDictionary<string, string> inputValues)
    {
      if (inputValues == null)
        throw new ArgumentNullException(nameof (inputValues));
      string empty = string.Empty;
      List<string> versionTagsFilter = new List<string>();
      string defaultVersionType = inputValues.GetDefaultVersionType();
      if ((defaultVersionType == "latestWithBranchAndTagsType" || defaultVersionType == "latestWithBuildDefinitionBranchAndTagsType") && inputValues.TryGetValue("defaultVersionTags", out empty) && !string.IsNullOrEmpty(empty))
      {
        string str1 = empty;
        char[] chArray = new char[2]{ ',', ';' };
        foreach (string str2 in str1.Split(chArray))
        {
          if (!string.IsNullOrEmpty(str2.Trim()))
            versionTagsFilter.Add(str2.Trim());
        }
      }
      return (IList<string>) versionTagsFilter;
    }

    public static string GetDefaultVersionMergedTags(this IDictionary<string, string> inputValues) => string.Join(",", (IEnumerable<string>) inputValues.GetDefaultVersionTagsFilter());

    public static InputValue GetDefaultVersionLatestValue(
      this IDictionary<string, string> inputValues,
      IList<InputValue> versions)
    {
      if (inputValues == null)
        throw new ArgumentNullException(nameof (inputValues));
      return (versions != null ? versions.FirstOrDefault<InputValue>() : throw new ArgumentNullException(nameof (inputValues))) ?? throw new InvalidOperationException(TaskResources.NoArtifactVersionsAvailable());
    }

    public static InputValue GetDefaultVersionLatestFromBranchValue(
      this IDictionary<string, string> inputValues,
      IList<InputValue> versions)
    {
      if (inputValues == null)
        throw new ArgumentNullException(nameof (inputValues));
      if (versions == null)
        throw new ArgumentNullException(nameof (inputValues));
      InputValue inputValue = (InputValue) null;
      string branchName = string.Empty;
      if (inputValues.TryGetValue("defaultVersionBranch", out branchName) || inputValues.TryGetValue("branch", out branchName) || inputValues.TryGetValue("branches", out branchName))
        inputValue = versions.FirstOrDefault<InputValue>((Func<InputValue, bool>) (v => InputValuesDictionaryExtension.IsMatchingSourceBranch(v, branchName)));
      return inputValue != null ? inputValue : throw new InvalidOperationException(TaskResources.NoArtifactVersionsAvailable());
    }

    public static InputValue GetDefaultVersionLatestWithBranchAndTagsValue(
      this IDictionary<string, string> inputValues,
      IList<InputValue> versions)
    {
      if (inputValues == null)
        throw new ArgumentNullException(nameof (inputValues));
      if (versions == null)
        throw new ArgumentNullException(nameof (inputValues));
      string versionBranchFilter = inputValues.GetDefaultVersionBranchFilter();
      return inputValues.GetVersionWithBranchAndTagsValue(versions, versionBranchFilter);
    }

    public static InputValue GetDefaultVersionSpecificValue(
      this IDictionary<string, string> inputValues,
      IList<InputValue> versions)
    {
      if (inputValues == null)
        throw new ArgumentNullException(nameof (inputValues));
      if (versions == null)
        throw new ArgumentNullException(nameof (inputValues));
      string key = "defaultVersionSpecific";
      string specificVersion = inputValues.ContainsKey(key) && !string.IsNullOrEmpty(inputValues[key]) ? inputValues[key] : throw new InvalidOperationException(TaskResources.NoSpecificVersionValueAvailableForSpecificVersionType());
      return versions.FirstOrDefault<InputValue>((Func<InputValue, bool>) (v => string.Compare(v.Value, specificVersion, StringComparison.OrdinalIgnoreCase) == 0)) ?? throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TaskResources.SpecificArtifactVersionNotAvailable((object) specificVersion)));
    }

    public static bool HasLatestFromBranchDefaultVersionType(
      this IDictionary<string, string> inputValues)
    {
      return inputValues != null ? inputValues.HasDefaultVersionTypeMatchingId("latestFromBranchType") : throw new ArgumentNullException(nameof (inputValues));
    }

    public static bool HasLatestWithBranchAndTagsDefaultVersionType(
      this IDictionary<string, string> inputValues)
    {
      return inputValues != null ? inputValues.HasDefaultVersionTypeMatchingId("latestWithBranchAndTagsType") : throw new ArgumentNullException(nameof (inputValues));
    }

    public static bool HasLatestWithBuildDefinitionBranchAndTagsDefaultVersionType(
      this IDictionary<string, string> inputValues)
    {
      return inputValues != null ? inputValues.HasDefaultVersionTypeMatchingId("latestWithBuildDefinitionBranchAndTagsType") : throw new ArgumentNullException(nameof (inputValues));
    }

    public static bool HasSpecificVersionDefaultVersionType(
      this IDictionary<string, string> inputValues)
    {
      return inputValues != null ? inputValues.HasDefaultVersionTypeMatchingId("specificVersionType") : throw new ArgumentNullException(nameof (inputValues));
    }

    public static bool HasArtifactSpecificVersion(this IDictionary<string, string> inputValues)
    {
      if (inputValues == null)
        throw new ArgumentNullException(nameof (inputValues));
      string str;
      return inputValues.TryGetValue("defaultVersionSpecific", out str) && !string.IsNullOrEmpty(str);
    }

    public static InputValue GetVersionWithBranchAndTagsValue(
      this IDictionary<string, string> inputValues,
      IList<InputValue> versions,
      string branchFilter)
    {
      bool flag1 = !string.IsNullOrEmpty(branchFilter);
      List<string> list = inputValues.GetDefaultVersionTagsFilter().ToList<string>();
      bool flag2 = list != null && list.Any<string>();
      if (!flag1 && !flag2)
        return inputValues.GetDefaultVersionLatestValue(versions);
      if (!flag1)
        return InputValuesDictionaryExtension.GetDefaultVersionLatestWithTagsFilter((IList<string>) list, versions);
      if (!branchFilter.StartsWith(ArtifactFilter.BranchPrefix, StringComparison.Ordinal))
        branchFilter = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}{1}", (object) ArtifactFilter.BranchPrefix, (object) branchFilter);
      return !flag2 ? InputValuesDictionaryExtension.GetDefaultVersionLatestWithBranchFilter(branchFilter, versions) : InputValuesDictionaryExtension.GetDefaultVersionLatestWithBranchAndTagsFilters(branchFilter, (IList<string>) list, versions);
    }

    public static bool HasSelectDuringReleaseCreationDefaultVersionType(
      this IDictionary<string, string> inputValues)
    {
      return inputValues != null ? inputValues.HasDefaultVersionTypeMatchingId("selectDuringReleaseCreationType") : throw new ArgumentNullException(nameof (inputValues));
    }

    private static bool IsMatchingSourceBranch(InputValue version, string branchName)
    {
      string empty = string.Empty;
      if (version?.Data != null)
        empty = (string) version.Data["branch"];
      return string.Equals(empty, branchName, StringComparison.Ordinal);
    }

    private static bool HasDefaultVersionTypeMatchingId(
      this IDictionary<string, string> inputValues,
      string id)
    {
      return inputValues.GetDefaultVersionType().Equals(id, StringComparison.OrdinalIgnoreCase);
    }

    private static InputValue GetDefaultVersionLatestWithBranchFilter(
      string branchFilter,
      IList<InputValue> versions)
    {
      return versions.FirstOrDefault<InputValue>((Func<InputValue, bool>) (v => InputValuesDictionaryExtension.IsVersionMatchingBranchFilter(v, branchFilter))) ?? throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TaskResources.NoArtifactVersionsForBranchAvailable((object) branchFilter)));
    }

    private static InputValue GetDefaultVersionLatestWithTagsFilter(
      IList<string> tagsFilter,
      IList<InputValue> versions)
    {
      return versions.FirstOrDefault<InputValue>((Func<InputValue, bool>) (v => InputValuesDictionaryExtension.IsVersionMatchingTagsFilter(v, tagsFilter))) ?? throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TaskResources.NoArtifactVersionsForTagsAvailable((object) string.Join(",", (IEnumerable<string>) tagsFilter))));
    }

    private static InputValue GetDefaultVersionLatestWithBranchAndTagsFilters(
      string branchFilter,
      IList<string> tagsFilter,
      IList<InputValue> versions)
    {
      return versions.FirstOrDefault<InputValue>((Func<InputValue, bool>) (v => InputValuesDictionaryExtension.IsVersionMatchingBranchFilter(v, branchFilter) && InputValuesDictionaryExtension.IsVersionMatchingTagsFilter(v, tagsFilter))) ?? throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TaskResources.NoArtifactVersionsForBranchAndTagsAvailable((object) branchFilter, (object) string.Join(",", (IEnumerable<string>) tagsFilter))));
    }

    private static bool IsVersionMatchingBranchFilter(InputValue version, string branchFilter)
    {
      if (version.Data == null || !version.Data.ContainsKey("branch"))
        return false;
      return new ArtifactFilter()
      {
        SourceBranch = branchFilter
      }.IsMatchingBranchFilter(version.Data["branch"].ToString(), false);
    }

    private static bool IsVersionMatchingTagsFilter(InputValue version, IList<string> tagsFilter)
    {
      if (version.Data == null || !version.Data.ContainsKey("tags"))
        return false;
      return new ArtifactFilter() { Tags = tagsFilter }.IsMatchingTags(((IEnumerable<string>) version.Data["tags"].ToString().Split(',')).Select<string, string>((Func<string, string>) (t => t.Trim())));
    }
  }
}
