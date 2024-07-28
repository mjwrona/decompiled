// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Tracepoints
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class Tracepoints
  {
    public const int MqSecuritySubscriberEnterProcessMqSecurityEvent = 800001;
    public const int MqSecuritySubscriberLeaveProcessMqSecurityEvent = 800002;
    public const int MqSecuritySubscriberHasUseBuildResources = 800003;
    public const int MqSecuritySubscriberWrongQueueName = 800004;
    public const int AdministrationComponentEnterVerifyBuildServiceHostOwnership = 800005;
    public const int AdministrationComponentLeaveVerifyBuildServiceHostOwnership = 800006;
    public const int BuildPushNotificationSubscriberEnterProcessNotification = 800007;
    public const int BuildPushNotificationSubscriberLeaveProcessNotification = 800008;
    public const int BuildPushNotificationSubscriberRetrievedProject = 800009;
    public const int BuildPushNotificationSubscriberGetAffectedDefinitions = 800010;
    public const int BuildPushNotificationSubscriberRetrievedIdentity = 800011;
    public const int BuildPushNotificationSubscriberRetrievedGitRepository = 800012;
    public const int BuildPushNotificationSubscriberRetrievedProcessTemplates = 800013;
    public const int BuildPushNotificationSubscriberEnsureBuildGroups = 800014;
    public const int BuildPushNotificationSubscriberRetrievedNewGitCommit = 800015;
    public const int BuildPushNotificationSubscriberRetrievedOldGitCommit = 800016;
    public const int BuildPushNotificationSubscriberRetrievedManifest = 800017;
    public const int BuildPushNotificationSubscriberDiffTrees = 800018;
    public const int BuildPushNotificationSubscriberUpdatedCachedProcessParameters = 800019;
    public const int BuildPushNotificationSubscriberDeletedProcessTemplates = 800020;
    public const int BuildPushNotificationSubscriberAddedProcessTemplates = 800021;
    public const int BuildPushNotificationSubscriberUpdatedProcessTemplates = 800022;
    public const int BuildPushNotificationSubscriberEnterUpdateProcessTemplates = 800023;
    public const int BuildPushNotificationSubscriberLeaveUpdateProcessTemplates = 800024;
    public const int BuildPushNotificationSubscriberEnterUpdateBuildControllers = 800025;
    public const int BuildPushNotificationSubscriberLeaveUpdateBuildControllers = 800026;
    public const int BuildPushNotificationSubscriberQueryBuildControllers = 800027;
    public const int BuildPushNotificationSubscriberUpdatingBuildController = 800028;
    public const int CheckinNotificationEnterProcessChangeset = 800029;
    public const int CheckinNotificationLeaveProcessChangeset = 800030;
    public const int CheckinNotificationProcessedChangeset = 800031;
    public const int CheckinNotificationPublishBuildsQueued = 800032;
    public const int CheckinNotificationScheduledRollingJob = 800033;
    public const int CheckinNotificationStartedBuilds = 800034;
    public const int BuildPushNotificationSubscriberDetailedTiming = 800035;
    public const int EatingGitRepositoryNotFoundException = 800036;
    public const int EatingProjectDoesNotExistWithNameException = 800037;
    public const int EatingProjectDoesNotExistException = 800038;
    public const int GetBuildsByIdsRepeatCall = 800039;
  }
}
