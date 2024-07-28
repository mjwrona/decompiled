// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestArtifactHelper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class PullRequestArtifactHelper
  {
    public static int GetPullRequestId(
      IVssRequestContext requestContext,
      string artifactUri,
      out Guid projectGuid)
    {
      ArtifactId artifactId = LinkingUtilities.DecodeUri(artifactUri);
      projectGuid = Guid.Empty;
      if (!string.Equals(artifactId.Tool, "CodeReview", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(Resources.Format("InvalidCodeReviewArtifactUri", (object) artifactUri));
      if (string.Equals(artifactId.ArtifactType, "CodeReviewId", StringComparison.OrdinalIgnoreCase))
      {
        int codeReviewId;
        LegacyCodeReviewArtifactId.Decode(artifactId, out projectGuid, out codeReviewId);
        return codeReviewId;
      }
      if (string.Equals(artifactId.ArtifactType, "ReviewId", StringComparison.OrdinalIgnoreCase))
      {
        Guid projectGuid1;
        int codeReviewId;
        CodeReviewSdkArtifactId.Decode(artifactId, out projectGuid1, out codeReviewId);
        int pullRequestId;
        PullRequestArtifactId.Decode(LinkingUtilities.DecodeUri(requestContext.GetService<ICodeReviewService>().GetReview(requestContext, projectGuid1, codeReviewId, CodeReviewExtendedProperties.None, ReviewScope.ReviewLevelOnly).SourceArtifactId), out projectGuid, out Guid _, out pullRequestId);
        return pullRequestId;
      }
      throw new ArgumentException(Resources.Format("InvalidCodeReviewArtifactUri", (object) artifactUri));
    }
  }
}
