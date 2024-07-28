// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitCommitArtifactId
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitCommitArtifactId
  {
    public static ArtifactId GetArtifactIdForCommit(RepoKey repoKey, Sha1Id objectId) => new ArtifactId("Git", "Commit", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", (object) repoKey.ProjectId, (object) repoKey.RepoId, (object) objectId));

    public static string GetArtifactUriForCommit(RepoKey repoKey, Sha1Id objectId) => LinkingUtilities.EncodeUri(GitCommitArtifactId.GetArtifactIdForCommit(repoKey, objectId));

    public static Sha1Id GetCommitIdFromArtifactUri(string artifactUri)
    {
      ArtifactId artifactId = LinkingUtilities.DecodeUri(artifactUri);
      string str = UriUtility.UrlDecode(artifactId.ToolSpecificId);
      if (GitCommitArtifactId.IsGitCommitArtifactId(artifactId))
      {
        Sha1Id commitId;
        GitCommitArtifactId.DecodeGitCommitArtifactUri(str, out Guid _, out Guid _, out commitId);
        return commitId;
      }
      throw new ArgumentException(Resources.Format("InvalidCommitArtifactUri", (object) str), str);
    }

    public static void Decode(
      ArtifactId artifactId,
      out Guid projectGuid,
      out Guid repositoryId,
      out Sha1Id commitId)
    {
      GitCommitArtifactId.DecodeGitCommitArtifactUri(UriUtility.UrlDecode(artifactId.ToolSpecificId), out projectGuid, out repositoryId, out commitId);
    }

    public static Artifact DecodeArtifactUri(string artifactUri)
    {
      ArtifactId artifactId = LinkingUtilities.DecodeUri(artifactUri);
      string str = UriUtility.UrlDecode(artifactId.ToolSpecificId);
      if (GitCommitArtifactId.IsGitCommitArtifactId(artifactId))
      {
        Sha1Id commitId;
        GitCommitArtifactId.DecodeGitCommitArtifactUri(str, out Guid _, out Guid _, out commitId);
        return new Artifact()
        {
          Uri = artifactUri,
          ArtifactTitle = Resources.Format("GitCommitArtifactFormat", (object) commitId.ToString())
        };
      }
      throw new ArgumentException(Resources.Format("InvalidCommitArtifactUri", (object) str), str);
    }

    public static bool IsGitCommitArtifactId(ArtifactId artifactId) => string.Equals(artifactId.ArtifactType, "Commit", StringComparison.OrdinalIgnoreCase);

    private static void DecodeGitCommitArtifactUri(
      string artifactDetails,
      out Guid teamProjectId,
      out Guid repositoryId,
      out Sha1Id commitId)
    {
      string[] strArray = artifactDetails.Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length != 3)
        throw new ArgumentException(Resources.Format("InvalidCommitArtifactUri", (object) artifactDetails), artifactDetails);
      if (!Guid.TryParse(strArray[0], out teamProjectId))
        throw new ArgumentException(Resources.Format("InvalidCommitArtifactUri", (object) artifactDetails), artifactDetails);
      commitId = Guid.TryParse(strArray[1], out repositoryId) ? new Sha1Id(strArray[2]) : throw new ArgumentException(Resources.Format("InvalidCommitArtifactUri", (object) artifactDetails), artifactDetails);
    }
  }
}
