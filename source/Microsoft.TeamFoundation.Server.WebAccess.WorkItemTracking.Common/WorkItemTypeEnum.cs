// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemTypeEnum
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public enum WorkItemTypeEnum
  {
    Feedback = 1,
    Requirement = 2,
    Task = 3,
    Bug = 4,
    FeedbackRequest = 5,
    FeedbackResponse = 6,
    Portfolio = 7,
    Release = 8,
    Stage = 9,
    SignoffTask = 10, // 0x0000000A
    TestPlan = 11, // 0x0000000B
    TestSuite = 12, // 0x0000000C
  }
}
