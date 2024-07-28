// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.RegistryHelper
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  public static class RegistryHelper
  {
    public const string MaximumHoursToRetainRegistryPath = "/Configuration/SocialSDK/SocialEngagement/SocialAggregateMetricsMaximumHoursToRetain";
    public const int MaximumHoursToRetainDefaultValue = 720;
    public const string SocialActivityCacheFlushIntervalInSec = "/Configuration/SocialSDK/SocialActivity/MemoryCachecFlushIntervalInSec";
    public const int SocialActivityCacheFlushIntervalInSecDefault = 300;
    public const string SocialActivityRetentionTimeInSec = "/Configuration/SocialSDK/SocialActivity/SocialActivityRetentionTimeInSec";
    public const int SocialActivityRetentionTimeInSecDefault = 144000;
    public const string SocialActivityMinimumPartitionSpanInSec = "/Configuration/SocialSDK/SocialActivity/SocialActivityMinimumPartitionSpanInSec";
    public const int SocialActivityMinimumPartitionSpanInSecDefault = 72000;
    public const string SocialActivityAggregationTillTimeCushionInSec = "/Configuration/SocialSDK/SocialActivity/SocialActivityAggregationTillTimeCushionInSec";
    public const int SocialActivityAggregationTillTimeCushionInSecDefault = 1800;
    public const string SocialActivityAggregatedRecordsMaxCount = "/Configuration/SocialSDK/SocialActivity/SocialActivityAggregatedRecordsMaxCount";
    public const int SocialActivityAggregatedRecordsMaxCountDefault = 20000;
    public const string SocialEngagementRecordsMaxArtifactIdCount = "/Configuration/SocialSDK/SocialActivity/SocialEngagementRecordsMaxArtifactIdCount";
    public const int SocialEngagementRecordsMaxArtifactIdCountDefault = 10000;
    public const string SocialActivityAggregatedArtifactMaxArtifactIdCount = "/Configuration/SocialSDK/SocialActivity/SocialActivityAggregatedArtifactMaxArtifactIdCount";
    public const int SocialActivityAggregatedArtifactMaxArtifactIdCountDefault = 1000;

    public static T GetRegistryValue<T>(
      IVssRequestContext requestContext,
      string key,
      T defaultValue)
    {
      return requestContext.GetService<IVssRegistryService>().GetValue<T>(requestContext, (RegistryQuery) key, true, defaultValue);
    }
  }
}
