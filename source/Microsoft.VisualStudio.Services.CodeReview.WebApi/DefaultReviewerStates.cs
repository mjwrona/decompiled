// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.DefaultReviewerStates
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  public enum DefaultReviewerStates : short
  {
    Declined = -15, // 0xFFF1
    Rejected = -10, // 0xFFF6
    CodeNotReadyYet = -5, // 0xFFFB
    NoResponse = 0,
    ApprovedWithComments = 5,
    Approved = 10, // 0x000A
  }
}
