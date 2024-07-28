// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.LegacyCodeReviewArtifactId
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.Git.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class LegacyCodeReviewArtifactId
  {
    public static ArtifactId GetLegacyArtifactIdForCodeReview(
      string teamProjectUri,
      int codeReviewId)
    {
      return new ArtifactId("CodeReview", "CodeReviewId", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) LinkingUtilities.DecodeUri(teamProjectUri).ToolSpecificId, (object) codeReviewId));
    }

    public static void Decode(ArtifactId artifactId, out Guid projectGuid, out int codeReviewId) => LegacyCodeReviewArtifactId.DecodeArtifactUri(UriUtility.UrlDecode(artifactId.ToolSpecificId), out projectGuid, out codeReviewId);

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
        throw new ArgumentException(Resources.Format("InvalidCodeReviewArtifactUri", (object) artifactDetails));
      if (!Guid.TryParse(strArray[0], out teamProjectId))
        throw new ArgumentException(Resources.Format("InvalidCodeReviewArtifactUri", (object) artifactDetails));
      if (!int.TryParse(strArray[1], out codeReviewId) || codeReviewId < 0)
        throw new ArgumentException(Resources.Format("InvalidCodeReviewArtifactUri", (object) artifactDetails));
    }
  }
}
