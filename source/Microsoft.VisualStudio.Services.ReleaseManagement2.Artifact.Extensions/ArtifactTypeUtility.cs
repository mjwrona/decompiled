// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.ArtifactTypeUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Common;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions
{
  public static class ArtifactTypeUtility
  {
    public static bool IsCustomArtifact(IVssRequestContext requestContext, string artifactType)
    {
      List<string> source = new List<string>();
      source.AddRange((IEnumerable<string>) new List<string>()
      {
        "Build",
        "Git",
        "GitHub",
        "TFVC"
      });
      if (!requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.BuildArtifactsTasks"))
        source.Add("Jenkins");
      return !string.IsNullOrEmpty(artifactType) && !source.Any<string>((Func<string, bool>) (x => string.Equals(x, artifactType, StringComparison.OrdinalIgnoreCase)));
    }

    public static string GetCommitDisplayValue(
      string commitId,
      string commitMessage,
      int numberOfCharactersToDisplayFromCommitId,
      bool appendCommitMessage)
    {
      if (commitId == null)
        return string.Empty;
      string format = "{0} ({1})";
      if (!appendCommitMessage || string.IsNullOrEmpty(commitMessage))
        format = "{0}";
      if (commitMessage != null)
        commitMessage = ArtifactTypeUtility.RemoveNewLineCharacters(commitMessage);
      string str = commitId.Length <= numberOfCharactersToDisplayFromCommitId ? commitId : commitId.Substring(0, numberOfCharactersToDisplayFromCommitId);
      string commitDisplayValue = string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) str, (object) commitMessage);
      int num = 256;
      if (commitDisplayValue.Length > num)
        commitDisplayValue = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) commitDisplayValue.Substring(0, num - 4), (object) "...)");
      return commitDisplayValue;
    }

    public static string RemoveNewLineCharacters(string input) => input == null ? (string) null : new Regex("\r\n|\r|\n").Replace(input, " ");

    public static IList<Change> ConvertGitHubCommitsToChanges(
      IEnumerable<GitHubData.V3.CommitListItem> commits)
    {
      List<Change> changes = new List<Change>();
      if (commits != null)
      {
        foreach (GitHubData.V3.CommitListItem commit in commits)
        {
          IdentityRef identityRef = new IdentityRef()
          {
            Id = (string) null,
            DisplayName = commit.Commit?.Author?.Name,
            Url = commit.Author?.Html_url,
            UniqueName = commit.Author?.Login
          };
          if (!string.IsNullOrEmpty(commit.Author?.Avatar_url))
          {
            identityRef.Links = new ReferenceLinks();
            identityRef.Links.AddLink("avatar", commit.Author.Avatar_url);
          }
          changes.Add(new Change()
          {
            Id = commit.Sha,
            Message = commit.Commit?.Message,
            ChangeType = "GitHub",
            Author = identityRef,
            Timestamp = new DateTime?(Convert.ToDateTime(commit.Commit?.Author?.Date, (IFormatProvider) CultureInfo.InvariantCulture)),
            Location = new Uri(commit.Html_url),
            DisplayUri = new Uri(commit.Html_url),
            PushedBy = (IdentityRef) null
          });
        }
      }
      return (IList<Change>) changes;
    }

    public static bool IsGitHubRepoPublic(
      IVssRequestContext requestContext,
      GitHubAuthentication authentication,
      string repository)
    {
      if (string.IsNullOrWhiteSpace(repository))
        return false;
      authentication = authentication ?? new GitHubAuthentication(GitHubAuthScheme.None, string.Empty);
      bool flag = false;
      GitHubResult<GitHubData.V3.Repository> repo = GitHubHttpClientFactory.Create(requestContext).GetRepo((string) null, authentication, repository);
      if (repo.IsSuccessful)
      {
        GitHubData.V3.Repository result = repo.Result;
        if (result != null && !result.Private)
          flag = true;
      }
      return flag;
    }
  }
}
