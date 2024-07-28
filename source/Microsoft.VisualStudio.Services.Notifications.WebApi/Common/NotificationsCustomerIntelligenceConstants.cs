// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Common.NotificationsCustomerIntelligenceConstants
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Notifications.Common
{
  [GenerateAllConstants(null)]
  public static class NotificationsCustomerIntelligenceConstants
  {
    public const string Area = "Microsoft.VisualStudio.Services.Notifications";

    [GenerateAllConstants(null)]
    public static class FollowsCustomerIntelligenceConstants
    {
      public const string Feature = "Follows";
      public const string FollowAction = "Follow";
      public const string UnfollowAction = "Unfollow";
    }

    [GenerateAllConstants(null)]
    public static class NotifSubsServiceCIConstanst
    {
      public const string Feature = "NotificationSubscriptionService";
      public const string EvaluateStarted = "EvaluateSubscriptionStarted";
      public const string PrepareForCreateCompleted = "PrepareForCreateCompleted";
      public const string PrepareForSavingCompleted = "PrepareSubscriptionForSavingCompleted";
      public const string EvaluationJobDataSerialized = "EvaluationJobDataSerialized";
      public const string EvaluationJobQueued = "EvaluationJobQueued";
      public const string EventsRead = "EventsRead";
      public const string NotificationsCreated = "NotificationsCreated";
    }
  }
}
