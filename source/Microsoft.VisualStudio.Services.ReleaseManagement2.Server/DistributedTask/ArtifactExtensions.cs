// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.ArtifactExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.ExternalProviders.Common;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public static class ArtifactExtensions
  {
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to continue on error")]
    public static string GetParallelismTag(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<ArtifactSource> linkedArtifacts)
    {
      if (linkedArtifacts == null)
        throw new ArgumentNullException(nameof (linkedArtifacts));
      IDictionary<string, bool> visibilityCache = (IDictionary<string, bool>) new Dictionary<string, bool>();
      if (!ArtifactExtensions.IsProjectPublic(requestContext, projectId.ToString(), visibilityCache))
        return "Private";
      if (linkedArtifacts.IsNullOrEmpty<ArtifactSource>())
        return "Public";
      string parallelismTag = "Private";
      foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) linkedArtifacts)
      {
        switch (linkedArtifact.ArtifactTypeId)
        {
          case "Build":
          case "Git":
          case "TFVC":
            string projectId1 = linkedArtifact.ProjectData?.Value;
            if (projectId1 != null)
            {
              parallelismTag = ArtifactExtensions.IsProjectPublic(requestContext, projectId1, visibilityCache) ? "Public" : "Private";
              break;
            }
            break;
          case "GitHub":
            if (linkedArtifact.SourceData != null)
            {
              string str = linkedArtifact.SourceData["definition"]?.Value;
              try
              {
                if (!str.IsNullOrEmpty<char>())
                {
                  string gitHubRepoUrl = ArtifactExtensions.GetGitHubRepoUrl(str);
                  parallelismTag = ArtifactExtensions.IsGitHubRepoPublic(requestContext, gitHubRepoUrl, visibilityCache) ? "Public" : "Private";
                  break;
                }
                break;
              }
              catch (Exception ex)
              {
                string message = StringUtil.Format("Failed to identify ParallelismTagTypes for projectId {0}. Failed when checking visibility for GitHub repo: {1} with error message {2}", (object) projectId.ToString(), (object) str, (object) TeamFoundationExceptionFormatter.FormatException(ex, false));
                requestContext.Trace(1972013, TraceLevel.Error, "ReleaseManagementService", nameof (ArtifactExtensions), message);
                parallelismTag = "Private";
                break;
              }
            }
            else
              break;
          default:
            parallelismTag = "Private";
            break;
        }
        if (parallelismTag != "Public")
          break;
      }
      return parallelismTag;
    }

    private static string GetGitHubRepoUrl(string repoName)
    {
      if (repoName.IsNullOrEmpty<char>())
        return repoName;
      return StringUtil.Format("https://api.github.com/repos/{0}", (object) repoName);
    }

    private static bool IsProjectPublic(
      IVssRequestContext requestContext,
      string projectId,
      IDictionary<string, bool> visibilityCache)
    {
      IProjectService service = requestContext.GetService<IProjectService>();
      bool flag;
      Guid result;
      if (!visibilityCache.TryGetValue(projectId, out flag) && Guid.TryParse(projectId, out result))
      {
        flag = service.GetProject(requestContext, result).Visibility == ProjectVisibility.Public;
        visibilityCache.Add(projectId, flag);
      }
      return flag;
    }

    private static bool IsGitHubRepoPublic(
      IVssRequestContext requestContext,
      string gitHubRepoUrl,
      IDictionary<string, bool> visibilityCache)
    {
      bool flag;
      if (!visibilityCache.TryGetValue(gitHubRepoUrl, out flag))
      {
        GitHubHttpClient gitHubHttpClient = GitHubHttpClientFactory.Create(requestContext);
        GitHubAuthentication authentication = new GitHubAuthentication(GitHubAuthScheme.None, string.Empty);
        authentication.AcceptUntrustedCertificates = true;
        string repoUrl = gitHubRepoUrl;
        GitHubResult<GitHubData.V3.Repository> repo = gitHubHttpClient.GetRepo(authentication, repoUrl);
        if (repo.IsSuccessful)
        {
          GitHubData.V3.Repository result = repo.Result;
          flag = result != null && !result.Private;
          visibilityCache.Add(gitHubRepoUrl, flag);
        }
      }
      return flag;
    }
  }
}
