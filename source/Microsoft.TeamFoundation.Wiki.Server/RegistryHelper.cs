// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.RegistryHelper
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class RegistryHelper
  {
    public const string MaximumVersionsCountRegistryPath = "/Service/TFS/Wiki/MaximumVersionsCount";
    private const int MaximumVersionsCountDefaultValue = 10;
    public const string MaxPushToBeProcessedPerRun = "/Service/TFS/Wiki/MaxPushToBeProcessedPerRun";
    private const int MaxPushToBeProcessedPerRunDefaultValue = 3;
    public const string MaxPushToBeProcessedInSync = "/Service/TFS/Wiki/MaxPushToBeProcessedInSync";
    private const int MaxPushToBeProcessedInSyncDefaultValue = 3;
    public const string WikiFollowNotificationAggregationDelayInSecs = "/Service/TFS/Wiki/WikiFollowNotificationAggregationDelayInSecs";
    private const int WikiFollowNotificationAggregationDelayInSecsDefaultValue = 1800;
    private const string WikiFollowNotificationCommitsThreshold = "/Service/TFS/Wiki/WikiFollowNotificationCommitsThreshold";
    private const int WikiFollowNotificationCommitsThresholdDefaultValue = 5;
    public const string WikiFollowNotificationMaxFilesToProcess = "/Service/TFS/Wiki/WikiFollowNotificationMaxFilesToProcess";
    private const int WikiFollowNotificationMaxFilesToProcessDefaultValue = 100;
    public const string WikiPageViewUserSessionTimeInSecs = "/Service/TFS/Wiki/WikiPageViewUserSessionTimeInSecs";
    private const int WikiPageViewUserSessionTimeInSecsDefaultValue = 900;
    public const string WikiPageViewAggregationMetricsDeletionDays = "/Service/TFS/Wiki/WikiPageViewAggregationMetricsDeletionDays";
    private const int WikiPageViewAggregationMetricsDeletionDaysDefaultValue = 35;
    public const string WikiPageMetaDeletionDelayInHours = "/Service/TFS/Wiki/WikiPageMetaDeletionDelayInHours";
    private const int WikiPageMetaDeletionDelayInHoursDefaultValue = 4;
    public const string WikiVersionPushWaterMarkCatchUpDelayInHours = "/Service/TFS/Wiki/WikiVersionPushWaterMarkCatchUpDelayInHours";
    private const int WikiVersionPushWaterMarkCatchUpDelayInHoursDefaultValue = 8;
    public const string AccountTokenExpirationDays = "/Service/TFS/Wiki/AccountTokenExpirationDays";
    private const int AccountTokenExpirationDaysDefaultValue = 30;

    public static int GetMaxPushToBeProcessedInSync(this IVssRequestContext requestContext) => RegistryHelper.GetRegistryValue<int>(requestContext, "/Service/TFS/Wiki/MaxPushToBeProcessedInSync", 3);

    public static int GetMaxPushToBeProcessedPerRun(this IVssRequestContext requestContext) => RegistryHelper.GetRegistryValue<int>(requestContext, "/Service/TFS/Wiki/MaxPushToBeProcessedPerRun", 3);

    public static int GetMaximumVersionCount(IVssRequestContext requestContext) => RegistryHelper.GetRegistryValue<int>(requestContext, "/Service/TFS/Wiki/MaximumVersionsCount", 10);

    public static int GetFollowWikiNotificationAggregationDelayInSecs(
      this IVssRequestContext requestContext)
    {
      return RegistryHelper.GetRegistryValue<int>(requestContext, "/Service/TFS/Wiki/WikiFollowNotificationAggregationDelayInSecs", 1800);
    }

    public static int GetFollowNotificationProcessCommitsThreshold(
      this IVssRequestContext requestContext)
    {
      return RegistryHelper.GetRegistryValue<int>(requestContext, "/Service/TFS/Wiki/WikiFollowNotificationCommitsThreshold", 5);
    }

    public static int GetWikiPageViewUserSessionTimeInSecs(this IVssRequestContext requestContext) => RegistryHelper.GetRegistryValue<int>(requestContext, "/Service/TFS/Wiki/WikiPageViewUserSessionTimeInSecs", 900);

    public static int GetWikiPageViewAggregationMetricsDeletionDays(
      this IVssRequestContext requestContext)
    {
      return RegistryHelper.GetRegistryValue<int>(requestContext, "/Service/TFS/Wiki/WikiPageViewAggregationMetricsDeletionDays", 35);
    }

    public static int GetWikiFollowNotificationMaxFilesToProcess(
      this IVssRequestContext requestContext)
    {
      return RegistryHelper.GetRegistryValue<int>(requestContext, "/Service/TFS/Wiki/WikiFollowNotificationMaxFilesToProcess", 100);
    }

    public static int GetWikiPageMetaDeletionDelayInHours(this IVssRequestContext requestContext) => RegistryHelper.GetRegistryValue<int>(requestContext, "/Service/TFS/Wiki/WikiPageMetaDeletionDelayInHours", 4);

    public static int GetWikiWikiPushWaterMarkCatchUpDelayInHours(
      this IVssRequestContext requestContext)
    {
      return RegistryHelper.GetRegistryValue<int>(requestContext, "/Service/TFS/Wiki/WikiVersionPushWaterMarkCatchUpDelayInHours", 8);
    }

    public static int GetAccountTokenExpirationDays(this IVssRequestContext requestContext) => RegistryHelper.GetRegistryValue<int>(requestContext, "/Service/TFS/Wiki/AccountTokenExpirationDays", 30);

    private static T GetRegistryValue<T>(
      IVssRequestContext requestContext,
      string key,
      T defaultValue)
    {
      return requestContext.GetService<IVssRegistryService>().GetValue<T>(requestContext, (RegistryQuery) key, true, defaultValue);
    }
  }
}
