// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.CodeReviewSdkArtifactId
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  public class CodeReviewSdkArtifactId
  {
    public static string GetArtifactUri(CommentThread reviewCommentThread)
    {
      if (reviewCommentThread.ProjectId == Guid.Empty)
        throw new ArgumentException(CodeReviewWebAPIResources.ProjectIdMustBeSpecified((object) reviewCommentThread.DiscussionId), nameof (reviewCommentThread));
      return CodeReviewSdkArtifactId.GetArtifactUri(reviewCommentThread.ProjectId, reviewCommentThread.ReviewId);
    }

    public static string GetArtifactUri(string teamProjectUri, int reviewId) => CodeReviewSdkArtifactId.GetArtifactUri(new Guid(LinkingUtilities.DecodeUri(teamProjectUri).ToolSpecificId), reviewId);

    public static string GetArtifactUri(Guid projectId, int reviewId) => LinkingUtilities.EncodeUri(CodeReviewSdkArtifactId.GetArtifactId(projectId, reviewId));

    public static ArtifactId GetArtifactId(string teamProjectUri, int reviewId) => CodeReviewSdkArtifactId.GetArtifactId(new Guid(LinkingUtilities.DecodeUri(teamProjectUri).ToolSpecificId), reviewId);

    public static ArtifactId GetArtifactId(Guid projectId, int reviewId) => new ArtifactId("CodeReview", "ReviewId", string.Format("{0}{1}{2}", (object) projectId, (object) LinkingUtilities.URISEPARATOR, (object) reviewId));

    public static void Decode(ArtifactId artifactId, out Guid projectGuid, out int codeReviewId) => CodeReviewSdkArtifactId.DecodeArtifactUri(UriUtility.UrlDecode(artifactId.ToolSpecificId), out projectGuid, out codeReviewId);

    private static void DecodeArtifactUri(
      string artifactDetails,
      out Guid teamProjectId,
      out int codeReviewId)
    {
      string[] strArray = artifactDetails.Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length != 2)
        throw new ArgumentException(CodeReviewWebAPIResources.InvalidCodeReviewArtifactUri((object) artifactDetails));
      if (!Guid.TryParse(strArray[0], out teamProjectId))
        throw new ArgumentException(CodeReviewWebAPIResources.InvalidCodeReviewArtifactUri((object) artifactDetails));
      if (!int.TryParse(strArray[1], out codeReviewId) || codeReviewId < 0)
        throw new ArgumentException(CodeReviewWebAPIResources.InvalidCodeReviewArtifactUri((object) artifactDetails));
    }
  }
}
