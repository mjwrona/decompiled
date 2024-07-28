// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestReportingConstants
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public static class TestReportingConstants
  {
    public const string CIWorkflow = "Continuous Integration";
    public const string CDWorkflow = "Continuous Delivery";
    public const string ManualWorkflow = "Manual Workflow";
    public const string TestOutcomeCategoryField = "TestOutcome";
    public const string OutcomeConfidenceCategoryName = "OutcomeConfidence";
    public const float OutcomeConfidenceFilterValue = 0.0f;
    public const int SummaryAndInsightsPublishBatchSize = 10;
  }
}
