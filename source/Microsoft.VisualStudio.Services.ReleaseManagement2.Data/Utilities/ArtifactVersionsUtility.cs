// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ArtifactVersionsUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ArtifactVersionsUtility
  {
    public static string GetSupportedDefaultVersionTypesAsString(string artifactType) => string.Join(", ", (IEnumerable<string>) ArtifactVersionsUtility.GetSupportedDefaultVersionTypes(artifactType));

    public static IList<string> GetSupportedDefaultVersionTypes(string artifactType)
    {
      switch (artifactType)
      {
        case "Build":
          return (IList<string>) new List<string>()
          {
            "selectDuringReleaseCreationType",
            "latestType",
            "latestWithBranchAndTagsType",
            "specificVersionType",
            "latestWithBuildDefinitionBranchAndTagsType"
          };
        case "Git":
        case "GitHub":
          return (IList<string>) new List<string>()
          {
            "selectDuringReleaseCreationType",
            "latestFromBranchType",
            "specificVersionType"
          };
        case "TFVC":
        case "PackageManagement":
          return (IList<string>) new List<string>()
          {
            "selectDuringReleaseCreationType",
            "latestType",
            "specificVersionType"
          };
        default:
          return (IList<string>) new List<string>()
          {
            "selectDuringReleaseCreationType",
            "latestType",
            "latestFromBranchType",
            "specificVersionType"
          };
      }
    }

    public static ArtifactMetadata GetArtifactMetadataFromArtifactVersion(
      string alias,
      InputValue artifactVersion)
    {
      if (artifactVersion == null)
        return (ArtifactMetadata) null;
      ArtifactMetadata fromArtifactVersion = new ArtifactMetadata()
      {
        Alias = alias,
        InstanceReference = new BuildVersion()
        {
          Id = artifactVersion.Value,
          Name = artifactVersion.DisplayValue
        }
      };
      if (artifactVersion.Data != null)
      {
        fromArtifactVersion.InstanceReference.DefinitionId = artifactVersion.Data.GetValueOrDefault<string, object>("definitionId")?.ToString();
        fromArtifactVersion.InstanceReference.DefinitionName = artifactVersion.Data.GetValueOrDefault<string, object>("definitionName")?.ToString();
        fromArtifactVersion.InstanceReference.SourceBranch = artifactVersion.Data.GetValueOrDefault<string, object>("branch")?.ToString();
        fromArtifactVersion.InstanceReference.SourceVersion = artifactVersion.Data.GetValueOrDefault<string, object>("sourceVersion")?.ToString();
        fromArtifactVersion.InstanceReference.SourceRepositoryId = artifactVersion.Data.GetValueOrDefault<string, object>("repositoryId")?.ToString();
        fromArtifactVersion.InstanceReference.SourceRepositoryType = artifactVersion.Data.GetValueOrDefault<string, object>("repositoryType")?.ToString();
        string str1 = artifactVersion.Data.GetValueOrDefault<string, object>("pullRequestId")?.ToString();
        string str2 = artifactVersion.Data.GetValueOrDefault<string, object>("pullRequestSourceBranchCommitId")?.ToString();
        string s = artifactVersion.Data.GetValueOrDefault<string, object>("pullRequestMergedAt")?.ToString();
        string str3 = artifactVersion.Data.GetValueOrDefault<string, object>("pullRequestTargetBranch")?.ToString();
        string str4 = artifactVersion.Data.GetValueOrDefault<string, object>("pullRequestSourceBranch")?.ToString();
        string str5 = artifactVersion.Data.GetValueOrDefault<string, object>("pullRequestIterationId")?.ToString();
        if (!string.IsNullOrEmpty(str1))
        {
          fromArtifactVersion.InstanceReference.SourcePullRequestVersion = new SourcePullRequestVersion()
          {
            PullRequestId = str1,
            SourceBranchCommitId = str2,
            TargetBranch = str3,
            SourceBranch = str4,
            IterationId = str5
          };
          DateTime result;
          if (DateTime.TryParse(s, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
            fromArtifactVersion.InstanceReference.SourcePullRequestVersion.PullRequestMergedAt = new DateTime?(result);
        }
      }
      return fromArtifactVersion;
    }

    public static string GetDefaultIdForDefaultVersionType(string artifactType) => ArtifactVersionsUtility.IsGitOrGitHubArtifactType(artifactType) ? "latestFromBranchType" : "latestType";

    public static string GetDefaultNameForDefaultVersionType(string artifactType) => ArtifactVersionsUtility.IsGitOrGitHubArtifactType(artifactType) ? Resources.LatestFromBranchType : Resources.LatestType;

    private static bool IsGitOrGitHubArtifactType(string artifactType) => string.Equals(artifactType, "Git", StringComparison.OrdinalIgnoreCase) || string.Equals(artifactType, "GitHub", StringComparison.OrdinalIgnoreCase);
  }
}
