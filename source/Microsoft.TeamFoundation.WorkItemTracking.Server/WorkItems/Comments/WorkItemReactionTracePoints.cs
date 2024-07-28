// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemReactionTracePoints
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments
{
  public static class WorkItemReactionTracePoints
  {
    public const string ReactionServiceArea = "ReactionService";
    public const string ReactionServiceLayer = "Service";
    private const int ReactionServiceStart = 100160000;
    private const int ReactionBase = 100160000;
    private const int ReactionErrorBase = 100160000;
    private const int ReactionWarningBase = 100160300;
    private const int ReactionInfoBase = 100160600;
    public const int CreateSocialEngagementRecordFailure = 100160001;
    public const int DeleteSocialEngagementRecordFailure = 100160002;
    public const int GetSocialEngagementRecordFailure = 100160003;
    public const int InvalidArtifactIdReturnedSocialSDK = 100160004;
    public const int CreateReactionPermissionFail = 100160005;
    public const int DeleteReactionPermissionFail = 100160006;
    public const int WorkItemIdsListEmptyWarn = 100160301;
    public const int CreateReactionInputParameterInfo = 100160601;
    public const int DeleteReactionInputParameterInfo = 100160602;
    public const int GetReactionInputParameterInfo = 100160603;
    public const int GetSortedReactionsCountInParamInfo = 100160604;
    public const int GetEngagedUsersInputParamterInfo = 100160605;
  }
}
