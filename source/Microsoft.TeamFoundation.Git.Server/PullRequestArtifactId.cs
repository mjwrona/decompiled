// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestArtifactId
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.Git.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PullRequestArtifactId
  {
    public static ArtifactId GetArtifactIdForPullRequest(
      string teamProjectUri,
      Guid repositoryId,
      int pullRequestId)
    {
      return new ArtifactId("Git", "PullRequestId", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", (object) LinkingUtilities.DecodeUri(teamProjectUri).ToolSpecificId, (object) repositoryId, (object) pullRequestId));
    }

    public static void Decode(
      ArtifactId artifactId,
      out Guid projectGuid,
      out Guid repositoryId,
      out int pullRequestId)
    {
      PullRequestArtifactId.DecodeArtifactUri(UriUtility.UrlDecode(artifactId.ToolSpecificId), out projectGuid, out repositoryId, out pullRequestId);
    }

    public static bool TryDecode(
      ArtifactId artifactId,
      out Guid projectGuid,
      out Guid repositoryId,
      out int pullRequestId)
    {
      try
      {
        PullRequestArtifactId.DecodeArtifactUri(UriUtility.UrlDecode(artifactId.ToolSpecificId), out projectGuid, out repositoryId, out pullRequestId);
        return true;
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case ArgumentException _:
          case NullReferenceException _:
            projectGuid = new Guid();
            repositoryId = new Guid();
            pullRequestId = 0;
            return false;
          default:
            throw;
        }
      }
    }

    public static bool TryDecodePullRequestArtifactId(
      string artifactId,
      out RepoKey repoKey,
      out int pullRequestId)
    {
      ArtifactId artifactId1 = LinkingUtilities.DecodeUri(artifactId);
      if (string.Equals(artifactId1.Tool, "Git", StringComparison.OrdinalIgnoreCase) && string.Equals(artifactId1.ArtifactType, "PullRequestId", StringComparison.OrdinalIgnoreCase))
      {
        Guid projectGuid;
        Guid repositoryId;
        PullRequestArtifactId.Decode(artifactId1, out projectGuid, out repositoryId, out pullRequestId);
        if (projectGuid != Guid.Empty && repositoryId != Guid.Empty && pullRequestId > 0)
        {
          repoKey = new RepoKey(projectGuid, repositoryId);
          return true;
        }
      }
      repoKey = (RepoKey) null;
      pullRequestId = 0;
      return false;
    }

    private static void DecodeArtifactUri(
      string artifactDetails,
      out Guid teamProjectId,
      out Guid repositoryId,
      out int pullRequestId)
    {
      string[] strArray = artifactDetails.Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length != 3)
        throw new ArgumentException(Resources.Format("InvalidPullRequestArtifactUri", (object) artifactDetails), artifactDetails);
      if (!Guid.TryParse(strArray[0], out teamProjectId))
        throw new ArgumentException(Resources.Format("InvalidPullRequestArtifactUri", (object) artifactDetails), artifactDetails);
      if (!Guid.TryParse(strArray[1], out repositoryId))
        throw new ArgumentException(Resources.Format("InvalidPullRequestArtifactUri", (object) artifactDetails), artifactDetails);
      if (!int.TryParse(strArray[2], out pullRequestId) || pullRequestId < 0)
        throw new ArgumentException(Resources.Format("InvalidPullRequestArtifactUri", (object) artifactDetails), artifactDetails);
    }

    internal static Artifact GetArtifactForPullRequestArtifactUri(
      IVssRequestContext requestContext,
      IDictionary<Guid, ITfsGitRepository> repositoriesCache,
      string artifactUri,
      ArtifactId artifactId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDictionary<Guid, ITfsGitRepository>>(repositoriesCache, nameof (repositoriesCache));
      ArgumentUtility.CheckStringForNullOrEmpty(artifactUri, nameof (artifactUri));
      ArgumentUtility.CheckForNull<ArtifactId>(artifactId, nameof (artifactId));
      string str = UriUtility.UrlDecode(artifactId.ToolSpecificId);
      if (PullRequestArtifactId.IsValid(artifactId))
      {
        ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
        Guid teamProjectId;
        Guid repositoryId;
        int pullRequestId;
        PullRequestArtifactId.DecodeArtifactUri(str, out teamProjectId, out repositoryId, out pullRequestId);
        ITfsGitRepository repository = (ITfsGitRepository) null;
        if (!repositoriesCache.TryGetValue(repositoryId, out repository))
        {
          repository = service.FindRepositoryByName(requestContext, teamProjectId.ToString(), repositoryId.ToString());
          repositoriesCache[repositoryId] = repository;
        }
        string name = repository.Name;
        return new Artifact()
        {
          Uri = artifactUri,
          ArtifactTitle = Resources.Format("PullRequestArtifactFormat", (object) name, (object) pullRequestId, (object) (requestContext.GetService<ITeamFoundationGitPullRequestService>().GetPullRequestDetails(requestContext, repository, pullRequestId) ?? throw new ArgumentException(Resources.Format("PullRequestCouldNotBeFound", (object) pullRequestId, (object) name))).Title)
        };
      }
      throw new ArgumentException(Resources.Format("InvalidPullRequestArtifactUri", (object) str), str);
    }

    internal static string GetPullRequestArtifactUriPrefix(string teamProjectUri, Guid repositoryId) => LinkingUtilities.EncodeUri(new ArtifactId("Git", "PullRequestId", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) LinkingUtilities.DecodeUri(teamProjectUri).ToolSpecificId, (object) repositoryId)));

    public static bool IsValid(string artifactId, out ArtifactId parsedPullRequestArtifactId)
    {
      if (LinkingUtilities.IsUriWellFormed(artifactId))
      {
        ArtifactId artifactId1 = LinkingUtilities.DecodeUri(artifactId);
        if (artifactId1.Tool == "Git" && artifactId1.ArtifactType == "PullRequestId")
        {
          parsedPullRequestArtifactId = artifactId1;
          return true;
        }
      }
      parsedPullRequestArtifactId = (ArtifactId) null;
      return false;
    }

    public static bool IsValid(string artifactId) => PullRequestArtifactId.IsValid(artifactId, out ArtifactId _);

    public static bool IsValid(ArtifactId artifactId) => artifactId.Tool == "Git" && artifactId.ArtifactType == "PullRequestId";
  }
}
