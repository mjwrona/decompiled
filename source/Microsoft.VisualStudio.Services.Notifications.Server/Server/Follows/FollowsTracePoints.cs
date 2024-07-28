// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.Follows.FollowsTracePoints
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

namespace Microsoft.VisualStudio.Services.Notifications.Server.Follows
{
  public static class FollowsTracePoints
  {
    public const string Area = "Follows";

    public static class FollowsMatcher
    {
      public const string Layer = "FollowsMatcher";
      public const int LoadFollowsSubscriptionsEnter = 15090000;
      public const int LoadFollowsSubscriptionsError = 15090001;
      public const int LoadFollowsSubscriptionsLeave = 15090002;
      public const int ProcessError = 15090003;
      public const int PreProcessError = 15090004;
    }

    public static class FollowsService
    {
      public const string Layer = "FollowsService";
      public const int FollowArtifactEnter = 15091000;
      public const int FollowArtifactLeave = 15091001;
      public const int FollowArtifactError = 15091002;
      public const int GetArtifactSubscriptionsForUserEnter = 15091003;
      public const int GetArtifactSubscriptionsForUserLeave = 15091004;
      public const int GetArtifactSubscriptionsForUserError = 15091005;
      public const int UnfollowArtifactEnter = 15091006;
      public const int UnfollowArtifactLeave = 15091007;
      public const int unfollowArtifactError = 15091008;
      public const int UnfollowArtifactsEnter = 15091009;
      public const int UnfollowArtifactsLeave = 15091010;
      public const int UnfollowArtifactsError = 15091011;
      public const int GetArtifactSubscriptionEnter = 15091012;
      public const int GetArtifactSubscriptionLeave = 15091013;
      public const int GetArtifactSubscriptionError = 15091014;
      public const int GetArtifactSubscriptionByIdEnter = 15091015;
      public const int GetArtifactSubscriptionByIdLeave = 15091016;
      public const int GetArtifactSubscriptionByIdError = 15091017;
      public const int GetArtifactSubscriptionsEnter = 15091018;
      public const int GetArtifactSubscriptionsLeave = 15091019;
      public const int GetArtifactSubscriptionsError = 15091020;
      public const int TryParseArtifactError = 15091021;
      public const int PublishCustomerIntelligenceDataError = 15092000;
    }

    public static class FollowsEmailProvider
    {
      public const string Layer = "FollowsController";
      public const int TransformNotificationError = 15093000;
    }
  }
}
