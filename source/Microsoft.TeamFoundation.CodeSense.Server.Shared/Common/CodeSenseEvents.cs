// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Common.CodeSenseEvents
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

namespace Microsoft.TeamFoundation.CodeSense.Server.Common
{
  public static class CodeSenseEvents
  {
    public const int CatchupIndexingCompleted = 20000;
    public const int ExceededMaximumElements = 20001;
    public const int AnalyzingCodeChange = 20002;
    public const int CatchupJobDisabled = 20003;
    public const int CatchupJobFailedToUpdate = 20004;
    public const int WorkItemTypeCategoryMissing = 20005;
    public const int LongRunningActionExceededThreshold = 20006;
    public const int ErrorUpdatingPerformanceCounters = 20007;
    public const int CannotOpenSqmSession = 20008;
    public const int AggregatingSlicesAfterFailure = 20009;
    public const int FLICatchupIndexingCompleted = 20010;
    public const int FLICatchupJobDisabled = 20011;
    public const int FLICatchupJobFailedToUpdate = 20012;
    private const int BaseEventId = 20000;
  }
}
