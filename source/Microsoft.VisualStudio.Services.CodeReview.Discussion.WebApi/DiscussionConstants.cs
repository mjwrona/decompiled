// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.DiscussionConstants
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 443E2621-CB19-4319-96B1-AE621A0F5B5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.dll

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi
{
  public class DiscussionConstants
  {
    public const int MinThreadId = 1;
    public const int MinCommentId = 1;
    public const string MaxDiscussionsForArtifactQueryKey = "/CodeReview/MaxDiscussionsForArtifactQuery";
    public const int MaxDiscussionsForArtifactQuery = 10000;
    public const string MaxDiscussionsPerArtifactKey = "/CodeReview/MaxDiscussionsPerArtifact";
    public const int MaxDiscussionsPerArtifact = 10000;
    public const int MinDiscussionsForArtifactQueryRegistryCheck = 50;
    public const string LeftBufferPosition = "LeftBuffer";
    public const string RightBufferPosition = "RightBuffer";
  }
}
