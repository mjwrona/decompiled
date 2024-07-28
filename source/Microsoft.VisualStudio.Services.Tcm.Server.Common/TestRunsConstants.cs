// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.TestRunsConstants
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement
{
  public static class TestRunsConstants
  {
    public const int TestRunsBatchSize = 1000;
    public const int TestRunsPageSize = 100;
    public const int MaxDaysForQueryTestRuns = 7;
    public const int MaxIdsForQueryTestRuns = 10;
    public const int DefaultTestRunCommentMaxSize = 1000;
    public const int DefaultTestRunBatchSizeForInsightsCalculation = 10;
    public const int DefaultTestRunBatchSizeForSimilarTestResultJob = 10;
    public const int DefaultMaxRunsToBeProcessedForSimilarTestResultJob = 1000;
    public const int DefaultCalculateTestInsightsJobDelayInSec = 1;
    public const int TestRunNoOfTagLimit = 5;
    public const int TestRunNoOfTagLimitForBuild = 20;
    public const int TestRunTagSizeLimit = 50;
    public const int DefaultTestRunErrorMessageMaxSize = 512;
    public const string TestRunAllowedSpecialCharsInTagName = "=-_";
    public const int TestRunCustomFieldValueMaxSizeInBytes = 2000;
    public const string TestRunCustomCustomFieldValueMaxSize = "TestRunCustomFieldValueMaxSizeInBytes";
    public const int TestRunCustomCustomFieldValueMaxSizeExpiryInHrs = 12;
    public const string TestRunCacheNameSpaceId = "40b9310a-8b38-4129-9067-357cf33e1fd2";
  }
}
