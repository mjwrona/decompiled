// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ArtifactFilterUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ArtifactFilterUtility
  {
    public static bool IsEmpty(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter expectedArtifactFilter) => expectedArtifactFilter != null && string.IsNullOrEmpty(expectedArtifactFilter.SourceBranch) && expectedArtifactFilter.Tags.IsNullOrEmpty<string>();

    public static bool EvaluateFilters(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter expectedArtifactFilter,
      PipelineArtifactSource artifact)
    {
      bool filters = false;
      if (artifact == null || expectedArtifactFilter == null)
        return false;
      string artifactTypeId = artifact.ArtifactTypeId;
      string sourceBranch = artifact.SourceBranch;
      switch (artifactTypeId)
      {
        case "Git":
        case "GitHub":
          filters = ArtifactFilterUtility.IsBranchMatched(requestContext, expectedArtifactFilter, sourceBranch);
          break;
        case "Build":
          filters = !string.IsNullOrEmpty(expectedArtifactFilter.SourceBranch) ? (!expectedArtifactFilter.Tags.IsNullOrEmpty<string>() ? ArtifactFilterUtility.IsTagMatched(requestContext, expectedArtifactFilter, artifact) && ArtifactFilterUtility.IsBranchMatched(requestContext, expectedArtifactFilter, sourceBranch) : ArtifactFilterUtility.IsBranchMatched(requestContext, expectedArtifactFilter, sourceBranch)) : ArtifactFilterUtility.IsTagMatched(requestContext, expectedArtifactFilter, artifact);
          break;
        case "DockerHub":
        case "AzureContainerRepository":
          filters = ArtifactFilterUtility.IsTagFilterMatched(requestContext, expectedArtifactFilter, artifact);
          break;
      }
      return filters;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public static bool IsBranchMatched(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter expectedArtifactFilter,
      string artifactSourceBranch)
    {
      if (expectedArtifactFilter == null)
        return false;
      string branchFilter = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter.RemoveBranchPrefix(artifactSourceBranch);
      int num = expectedArtifactFilter.IsMatchingBranchFilter(branchFilter, true) ? 1 : 0;
      if (num != 0)
        return num != 0;
      requestContext.Trace(1980006, TraceLevel.Verbose, "ReleaseManagementService", "Pipeline", "ArtifactFilterUtility: Conditions not satisfied because branch {0} not matching with {1}", (object) branchFilter, (object) expectedArtifactFilter.SourceBranch);
      return num != 0;
    }

    public static bool IsTagFilterMatched(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter expectedArtifactFilter,
      PipelineArtifactSource artifact)
    {
      bool flag = true;
      if (expectedArtifactFilter != null && expectedArtifactFilter.TagFilter != null && artifact != null)
        flag = expectedArtifactFilter.IsMatchingTagFilter(requestContext, artifact.Version.Value, artifact.Alias);
      return flag;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public static bool IsTagMatched(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter expectedArtifactFilter,
      PipelineArtifactSource artifact)
    {
      bool flag = true;
      if (artifact == null)
        return true;
      if (expectedArtifactFilter != null && !expectedArtifactFilter.Tags.IsNullOrEmpty<string>())
      {
        if (requestContext == null)
          throw new ArgumentNullException(nameof (requestContext));
        List<string> tags = new List<string>();
        if (artifact.IsBuildArtifact)
        {
          BuildHttpClient buildClient = requestContext.GetClient<BuildHttpClient>();
          Func<Task<Microsoft.TeamFoundation.Build.WebApi.Build>> func = (Func<Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) (() => buildClient.GetBuildAsync(artifact.ProjectData.Value, artifact.VersionId));
          tags = requestContext.ExecuteAsyncAndGetResult<Microsoft.TeamFoundation.Build.WebApi.Build>(func).Tags;
        }
        flag = expectedArtifactFilter.IsMatchingTags((IEnumerable<string>) tags);
        if (!flag)
          requestContext.Trace(1980006, TraceLevel.Verbose, "ReleaseManagementService", "Pipeline", "ArtifactFilterUtility: Conditions not satisfied because tag {0} not matching with {1}", (object) string.Join(System.Environment.NewLine, tags.ToArray()), (object) string.Join(System.Environment.NewLine, expectedArtifactFilter.Tags.ToArray<string>()));
      }
      return flag;
    }

    public static bool EvaluateIncludeExcludeConflict(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter includedArtifactFilter,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactFilter excludedArtifactFilter)
    {
      if (includedArtifactFilter == null || excludedArtifactFilter == null)
        return false;
      if (includedArtifactFilter.EndsWithBranchPattern() || excludedArtifactFilter.EndsWithBranchPattern())
        return includedArtifactFilter.SourceBranch.Length > excludedArtifactFilter.SourceBranch.Length;
      return includedArtifactFilter.Tags != null && includedArtifactFilter.Tags.Count > 0;
    }
  }
}
