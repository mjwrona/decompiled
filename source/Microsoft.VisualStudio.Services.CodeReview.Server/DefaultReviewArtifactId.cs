// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.DefaultReviewArtifactId
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public class DefaultReviewArtifactId
  {
    public static bool IsValid(string artifactId) => DefaultReviewArtifactId.IsValid(artifactId, out ArtifactId _);

    public static bool IsValid(string artifactId, out ArtifactId parsedReviewArtifactId)
    {
      if (!string.IsNullOrEmpty(artifactId) && LinkingUtilities.IsUriWellFormed(artifactId))
      {
        ArtifactId artifactId1 = LinkingUtilities.DecodeUri(artifactId);
        if (DefaultReviewArtifactId.IsValid(artifactId1))
        {
          parsedReviewArtifactId = artifactId1;
          return true;
        }
      }
      parsedReviewArtifactId = (ArtifactId) null;
      return false;
    }

    public static bool IsValid(ArtifactId artifactId) => artifactId != null && artifactId.Tool == "CodeReview" && artifactId.ArtifactType != "PullRequestId";
  }
}
