// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ArtifactSourceDataUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Required")]
  public static class ArtifactSourceDataUtility
  {
    [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "Testing")]
    public static Dictionary<string, string> DecompressKeyMap = new Dictionary<string, string>()
    {
      {
        "projectId",
        "project"
      },
      {
        "servicesId",
        "connection"
      },
      {
        "sourceId",
        "definition"
      },
      {
        "prSysType",
        WellKnownPullRequestVariables.PullRequestSystemType
      },
      {
        "prId",
        WellKnownPullRequestVariables.PullRequestId
      },
      {
        "prIterationId",
        WellKnownPullRequestVariables.PullRequestIterationId
      },
      {
        "prBranchCommId",
        WellKnownPullRequestVariables.PullRequestSourceBranchCommitId
      },
      {
        "prMergeCommId",
        WellKnownPullRequestVariables.PullRequestMergeCommitId
      },
      {
        "prMergedAt",
        WellKnownPullRequestVariables.PullRequestMergedAt
      },
      {
        "prPolicyName",
        WellKnownPullRequestVariables.PullRequestStatusPolicyName
      },
      {
        "prSBranch",
        WellKnownPullRequestVariables.PullRequestSourceBranch
      },
      {
        "prTBranch",
        WellKnownPullRequestVariables.PullRequestTargetBranch
      },
      {
        "prRepoId",
        "pullRequestRepositoryId"
      },
      {
        "prProjectId",
        "pullRequestProjectId"
      },
      {
        "prRepoName",
        "pullRequestRepositoryName"
      },
      {
        "prSysConnId",
        "pullRequestSystemConnectionId"
      },
      {
        "sourceUrl",
        WellKnownArtifactInputs.ArtifactSourceDefinitionUrl
      },
      {
        "isMultiDef",
        "IsMultiDefinitionType"
      },
      {
        "isXaml",
        "IsXamlBuildArtifactType"
      },
      {
        "isTrigger",
        "IsTriggeringArtifact"
      },
      {
        "repo.name",
        "repository.name"
      },
      {
        "repo.provider",
        "repository.provider"
      },
      {
        "repo.id",
        "repository.id"
      }
    };
    [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "Testing")]
    public static Dictionary<string, string> CompressKeyMap = new Dictionary<string, string>()
    {
      {
        WellKnownPullRequestVariables.PullRequestSystemType,
        "prSysType"
      },
      {
        WellKnownPullRequestVariables.PullRequestId,
        "prId"
      },
      {
        WellKnownPullRequestVariables.PullRequestIterationId,
        "prIterationId"
      },
      {
        WellKnownPullRequestVariables.PullRequestSourceBranchCommitId,
        "prBranchCommId"
      },
      {
        WellKnownPullRequestVariables.PullRequestMergeCommitId,
        "prMergeCommId"
      },
      {
        WellKnownPullRequestVariables.PullRequestMergedAt,
        "prMergedAt"
      },
      {
        WellKnownPullRequestVariables.PullRequestStatusPolicyName,
        "prPolicyName"
      },
      {
        WellKnownPullRequestVariables.PullRequestSourceBranch,
        "prSBranch"
      },
      {
        WellKnownPullRequestVariables.PullRequestTargetBranch,
        "prTBranch"
      },
      {
        "pullRequestRepositoryId",
        "prRepoId"
      },
      {
        "pullRequestProjectId",
        "prProjectId"
      },
      {
        "pullRequestRepositoryName",
        "prRepoName"
      },
      {
        "pullRequestSystemConnectionId",
        "prSysConnId"
      },
      {
        WellKnownArtifactInputs.ArtifactSourceDefinitionUrl,
        "sourceUrl"
      },
      {
        "IsMultiDefinitionType",
        "isMultiDef"
      },
      {
        "IsXamlBuildArtifactType",
        "isXaml"
      },
      {
        "IsTriggeringArtifact",
        "isTrigger"
      },
      {
        "repository.name",
        "repo.name"
      },
      {
        "repository.provider",
        "repo.provider"
      },
      {
        "repository.id",
        "repo.id"
      }
    };

    public static string GetCompressedKeyName(string originalKey) => !ArtifactSourceDataUtility.CompressKeyMap.ContainsKey(originalKey) ? originalKey : ArtifactSourceDataUtility.CompressKeyMap[originalKey];

    public static string GetFullKeyName(string compressedKey) => !ArtifactSourceDataUtility.DecompressKeyMap.ContainsKey(compressedKey) ? compressedKey : ArtifactSourceDataUtility.DecompressKeyMap[compressedKey];

    public static void CompressSourceData(
      string sourceData,
      IDictionary<string, InputValue> targetSourceData)
    {
      if (string.IsNullOrWhiteSpace(sourceData))
        return;
      if (targetSourceData == null)
        throw new ArgumentNullException(nameof (targetSourceData));
      ArtifactSourceDataUtility.CompressSourceData((IDictionary<string, InputValue>) ServerModelUtility.FromString<Dictionary<string, InputValue>>(sourceData), targetSourceData);
    }

    public static void CompressSourceData(
      IDictionary<string, InputValue> originalSourceData,
      IDictionary<string, InputValue> targetSourceData)
    {
      if (targetSourceData == null)
        throw new ArgumentNullException(nameof (targetSourceData));
      if (originalSourceData == null)
        return;
      foreach (KeyValuePair<string, InputValue> keyValuePair in (IEnumerable<KeyValuePair<string, InputValue>>) originalSourceData)
        targetSourceData[ArtifactSourceDataUtility.GetCompressedKeyName(keyValuePair.Key)] = keyValuePair.Value;
    }

    public static void DecompressSourceData(
      string sourceData,
      IDictionary<string, InputValue> targetSourceData)
    {
      if (string.IsNullOrWhiteSpace(sourceData))
        return;
      if (targetSourceData == null)
        throw new ArgumentNullException(nameof (targetSourceData));
      Dictionary<string, InputValue> dictionary = ServerModelUtility.FromString<Dictionary<string, InputValue>>(sourceData);
      if (dictionary == null)
        return;
      foreach (KeyValuePair<string, InputValue> keyValuePair in dictionary)
        targetSourceData[ArtifactSourceDataUtility.GetFullKeyName(keyValuePair.Key)] = keyValuePair.Value;
    }
  }
}
