// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRefArtifactId
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class GitRefArtifactId
  {
    internal const string RefArtifactUriFormat = "{0}/{1}/{2}";

    internal static ArtifactId GetArtifactIdForRef(RepoKey repoKey, string refName)
    {
      ArgumentUtility.CheckForNull<RepoKey>(repoKey, nameof (repoKey));
      ArgumentUtility.CheckStringForNullOrEmpty(refName, nameof (refName));
      return new ArtifactId("Git", "Ref", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", (object) repoKey.ProjectId, (object) repoKey.RepoId, (object) refName));
    }

    internal static bool IsGitRefArtifactId(ArtifactId artifactId) => string.Equals(artifactId.ArtifactType, "Ref", StringComparison.OrdinalIgnoreCase);

    internal static void DecodeGitRefToolSpecificId(
      string toolSpecificId,
      out Guid teamProjectId,
      out Guid repositoryId,
      out string refName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(toolSpecificId, "decodedToolSpecificId");
      string[] source = toolSpecificId.Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (source.Length < 3)
        throw new ArgumentException(Resources.Format("InvalidGitRefArtifactUri", (object) toolSpecificId), toolSpecificId);
      if (!Guid.TryParse(source[0], out teamProjectId))
        throw new ArgumentException(Resources.Format("InvalidGitRefArtifactUri", (object) toolSpecificId), toolSpecificId);
      refName = Guid.TryParse(source[1], out repositoryId) ? string.Join("/", ((IEnumerable<string>) source).Skip<string>(2)) : throw new ArgumentException(Resources.Format("InvalidGitRefArtifactUri", (object) toolSpecificId), toolSpecificId);
    }

    internal static Artifact DecodeArtifactUri(
      IVssRequestContext requestContext,
      IDictionary<Guid, ITfsGitRepository> repositoriesCache,
      string artifactUri,
      ArtifactId artifactId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDictionary<Guid, ITfsGitRepository>>(repositoriesCache, nameof (repositoriesCache));
      ArgumentUtility.CheckStringForNullOrEmpty(artifactUri, nameof (artifactUri));
      ArgumentUtility.CheckForNull<ArtifactId>(artifactId, nameof (artifactId));
      string str1 = UriUtility.UrlDecode(artifactId.ToolSpecificId);
      if (GitRefArtifactId.IsGitRefArtifactId(artifactId))
      {
        ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
        Guid teamProjectId;
        Guid repositoryId;
        string refName;
        GitRefArtifactId.DecodeGitRefToolSpecificId(str1, out teamProjectId, out repositoryId, out refName);
        string str2 = GitRefArtifactId.DecodeRefName(refName);
        ITfsGitRepository tfsGitRepository = (ITfsGitRepository) null;
        if (!repositoriesCache.TryGetValue(repositoryId, out tfsGitRepository))
        {
          tfsGitRepository = service.FindRepositoryByName(requestContext, teamProjectId.ToString(), repositoryId.ToString());
          repositoriesCache[repositoryId] = tfsGitRepository;
        }
        string name = tfsGitRepository.Name;
        return new Artifact()
        {
          Uri = artifactUri,
          ArtifactTitle = Resources.Format("GitRefArtifactFormat", (object) str2, (object) name)
        };
      }
      throw new ArgumentException(Resources.Format("InvalidGitRefArtifactUri", (object) str1), str1);
    }

    private static string DecodeRefName(string refName) => refName.Substring(2);

    internal static string GetArtifactUriForRef(RepoKey repoKey, string refName) => LinkingUtilities.EncodeUri(GitRefArtifactId.GetArtifactIdForRef(repoKey, refName));
  }
}
