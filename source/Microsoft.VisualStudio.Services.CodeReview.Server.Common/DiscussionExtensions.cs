// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.DiscussionExtensions
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
{
  public static class DiscussionExtensions
  {
    public static void ExtractMetadata(string artifactUri, out Guid projectId, out int reviewId)
    {
      if (!string.IsNullOrEmpty(artifactUri))
      {
        ArtifactId artifactId = LinkingUtilities.DecodeUri(artifactUri);
        if (artifactId.Tool == "CodeReview" && artifactId.ArtifactType == "ReviewId")
        {
          string[] strArray = artifactId.ToolSpecificId.Split(LinkingUtilities.URISEPARATOR.ToCharArray());
          if (strArray.Length == 2)
          {
            projectId = new Guid(strArray[0]);
            reviewId = Convert.ToInt32(strArray[1]);
            return;
          }
        }
      }
      projectId = Guid.Empty;
      reviewId = int.MinValue;
    }
  }
}
