// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.Customization
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class Customization
  {
    public static string[] WorkItemTypesBlockedFromCustomization = new string[6]
    {
      "Microsoft.VSTS.WorkItemTypes.SharedParameter",
      "Microsoft.VSTS.WorkItemTypes.SharedStep",
      "Microsoft.VSTS.WorkItemTypes.CodeReviewRequest",
      "Microsoft.VSTS.WorkItemTypes.CodeReviewResponse",
      "Microsoft.VSTS.WorkItemTypes.FeedbackRequest",
      "Microsoft.VSTS.WorkItemTypes.FeedbackResponse"
    };
    public static string[] TcmWorkItemTypesBlockedFromDisable = new string[3]
    {
      "Microsoft.VSTS.WorkItemTypes.TestCase",
      "Microsoft.VSTS.WorkItemTypes.TestPlan",
      "Microsoft.VSTS.WorkItemTypes.TestSuite"
    };
  }
}
