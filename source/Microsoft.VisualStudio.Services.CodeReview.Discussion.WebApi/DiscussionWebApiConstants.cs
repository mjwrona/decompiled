// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.DiscussionWebApiConstants
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 443E2621-CB19-4319-96B1-AE621A0F5B5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi
{
  [GenerateAllConstants(null)]
  public class DiscussionWebApiConstants
  {
    public const string AreaId = "6823169A-2419-4015-B2FD-6FD6F026CA00";
    public const string AreaName = "discussion";
    public const string MultipleThreadsLocationIdString = "A50DDBE2-1A1D-4C55-857F-73C6A3A31722";
    public static readonly Guid MultipleThreadsLocationId = new Guid("A50DDBE2-1A1D-4C55-857F-73C6A3A31722");
    public const string ThreadsLocationIdString = "010054F6-D9ED-4ED2-855F-7F86BFF10C02";
    public static readonly Guid ThreadsLocationId = new Guid("010054F6-D9ED-4ED2-855F-7F86BFF10C02");
    public static readonly Guid ThreadsBatchLocationId = new Guid("255A0B5E-3C2F-43C2-A688-36C878210BA2");
    public const string MultipleCommentsLocationIdString = "20933FC0-B6A7-4A57-8111-A7458DA5441B";
    public static readonly Guid MultipleCommentsLocationId = new Guid("20933FC0-B6A7-4A57-8111-A7458DA5441B");
    public const string CommentsLocationIdString = "495211BD-B463-4578-86FE-924EA4953693";
    public static readonly Guid CommentsLocationId = new Guid("495211BD-B463-4578-86FE-924EA4953693");
    public const string CodeReviewArtifactType = "ReviewId";
    public const string CodeReviewArtifactTool = "CodeReview";
    public const int MaxCommentContentLength = 150000;
  }
}
