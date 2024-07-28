// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.TestArtifactDeletionConstants
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement
{
  public static class TestArtifactDeletionConstants
  {
    public const int WaitDaysForCleanupOfTestRunsOnHosted = 3;
    public const int WaitDaysForCleanupOfTestRunsOnOnPrem = 0;
    public const int RunsDeletionBatchSize = 20;
    public const int ResultsDeletionBatchSize = 500;
    public const int RunsDeletionBatchSizeForRetention = 1000;
    public const int MaxNumOfRunsToDelete = 100000;
    public const int MaxNumOfRunsToCleanup = 10000;
    public const int MaxNumOfCleanupRetry = 5;
    public const int WaitDaysForCleanupOfTestPlans = 15;
    public const int WaitDaysForCleanupOfTestPoints = 15;
    public const int WaitDaysForCleanupOfTestSuites = 15;
    public const int PlanArtifactsDeletionBatchSize = 10000;
    public const int PointArtifactsDeletionBatchSize = 1000;
    public const int SuiteArtifactsDeletionBatchSize = 10000;
    public const int SoftDeleteTestPointsForSuiteBatchSize = 10000;
    public const int MaxNumOfRunDimensionToDelete = 100000;
    public const int ResultsRetainedBeyondDays = 30;
  }
}
