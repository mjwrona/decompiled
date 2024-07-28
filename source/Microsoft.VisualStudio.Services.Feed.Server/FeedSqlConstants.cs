// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedSqlConstants
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal static class FeedSqlConstants
  {
    public const int DeleteHoldLifeTimeInMinutes = 10;
    public const string FeedChangedNotification = "B054E17F-E082-4F67-9186-D3DAE24CB6D4";
    public const string FeedViewChangedNotification = "88A497B2-C285-4685-AA39-B2CF5A0962CA";
    public const int FeedViewType_Release = 1;
    public const int FeedViewType_Implicit = 2;
    public const int Feed_InternalState_PendingUpgrade = 1;
    public const int Feed_InternalState_V2 = 2;
    public const int Feed_InternalState_UpgradeInProgress = 3;
    public const int FeedCapabilities_None = 0;
    public const int FeedCapabilities_UpstreamV2 = 1;
    public const int FeedCapabilities_UnderMaintenance = -2147483648;
    public const int PackageVersionIndex_InternalState_ImplicitLocalView = 2;
    public const int PackageVersionIndex_InternalState_RerunImplicitLocalView = 3;
    public const string EmptyGuid = "00000000-0000-0000-0000-000000000000";
    public const string FeedDataspaceCategory = "Feed";
    public const string FeedCollectionDataspaceIdentifier = "00000000-0000-0000-0000-000000000000";
  }
}
