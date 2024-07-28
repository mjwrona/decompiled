// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.FeatureAvailabilityFlags
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public static class FeatureAvailabilityFlags
  {
    public const string CodeReviewServiceHooks = "CodeReview.ServiceHooks";
    public const string CodeReviewProjectReadCheckForNotification = "CodeReview.ProjectReadCheck";
    public const string CodeReviewProjectReadCheckForReview = "CodeReview.ProjectReadCheckForReview";
    public const string GitUtilRequestContextCaching = "Git.Util.RequestContextCaching";
    public const string MonikerPerfFixDisabled = "CodeReview.Moniker.PerfFixDisabled";
    public const string CodeReviewThreadContextCheckCommentLineOffsetPosition = "CodeReview.Thread.ContextCheckCommentLineOffsetPosition";
  }
}
