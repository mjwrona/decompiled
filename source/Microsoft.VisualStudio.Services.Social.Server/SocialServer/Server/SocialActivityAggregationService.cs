// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.SocialActivityAggregationService
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  public class SocialActivityAggregationService : 
    ISocialActivityAggregationService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual void AddActivity(
      IVssRequestContext requestContext,
      string activityType,
      string data,
      string extendedData)
    {
      this.AddActivityInternal(requestContext, activityType, requestContext.ActivityId, requestContext.RequestTimer.StartTime, requestContext.GetUserId(), data, extendedData, false);
    }

    public virtual void AddActivityImmediately(
      IVssRequestContext requestContext,
      string activityType,
      Guid activityId,
      DateTime activityTimeStamp,
      Guid userId,
      string data,
      string extendedData)
    {
      this.AddActivityInternal(requestContext, activityType, activityId, activityTimeStamp, userId, data, extendedData, true);
    }

    public virtual IList<SocialActivityRecord> GetActivityRecords(
      IVssRequestContext requestContext,
      string activityType,
      DateTime fromDate,
      DateTime tillDate,
      bool fetchExtendedData,
      int skip,
      int take)
    {
      ArgumentUtility.CheckStringLength(activityType, nameof (activityType), 24, 1);
      ArgumentUtility.CheckGreaterThanOrEqualToZero((float) skip, nameof (skip));
      ArgumentUtility.CheckForOutOfRange(take, nameof (take), 1, 20000);
      if (fromDate >= tillDate)
        throw new ArgumentOutOfRangeException("tillDate should be more recent than fromDate.");
      using (SocialActivityAggregationComponent component = requestContext.CreateComponent<SocialActivityAggregationComponent>())
        return (IList<SocialActivityRecord>) component.GetSocialActivityRecords(activityType, fromDate, tillDate, fetchExtendedData, skip, take).ToList<SocialActivityRecord>();
    }

    public virtual IList<SocialActivityAggregatedMetric> GetActivityAggregatedMetrics(
      IVssRequestContext requestContext,
      Guid dataSpaceIdentifier,
      Guid providerId,
      DateTime fromDate,
      DateTime tillDate,
      int skip,
      int take)
    {
      ArgumentUtility.CheckForEmptyGuid(providerId, nameof (providerId));
      ArgumentUtility.CheckGreaterThanOrEqualToZero((float) skip, nameof (skip));
      ArgumentUtility.CheckForOutOfRange(take, nameof (take), 1, 20000);
      if (fromDate >= tillDate)
        throw new ArgumentOutOfRangeException("tillDate should be more recent than fromDate.");
      using (SocialActivityAggregationComponent component = requestContext.CreateComponent<SocialActivityAggregationComponent>())
      {
        List<SocialActivityAggregatedMetric> list = component.GetActivityAggregatedMetricRecords(dataSpaceIdentifier, providerId, fromDate, tillDate, skip, take).ToList<SocialActivityAggregatedMetric>();
        list.ForEach((Action<SocialActivityAggregatedMetric>) (metric => metric.DataSpaceIdentifier = dataSpaceIdentifier));
        return (IList<SocialActivityAggregatedMetric>) list;
      }
    }

    public IList<AggregatedArtifactsRecord> GetAggregatedArtifacts(
      IVssRequestContext requestContext,
      Guid dataSpaceIdentifier,
      Guid providerId)
    {
      ArgumentUtility.CheckForEmptyGuid(providerId, nameof (providerId));
      using (SocialActivityAggregationComponent component = requestContext.CreateComponent<SocialActivityAggregationComponent>())
        return (IList<AggregatedArtifactsRecord>) component.GetAllAggregatedArtifactsRecords(dataSpaceIdentifier, providerId).ToList<AggregatedArtifactsRecord>();
    }

    public IList<SocialActivityAggregatedArtifact> GetSocialActivityAggregatedArtifactBatch(
      IVssRequestContext requestContext,
      Guid dataSpaceIdentifier,
      Guid providerId,
      byte artifactType,
      IList<string> artifactIds)
    {
      ArgumentUtility.CheckForEmptyGuid(providerId, nameof (providerId));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) artifactIds, nameof (artifactIds));
      int registryValue = RegistryHelper.GetRegistryValue<int>(requestContext, "/Configuration/SocialSDK/SocialActivity/SocialActivityAggregatedArtifactMaxArtifactIdCount", 1000);
      if (artifactIds.Count > registryValue)
      {
        requestContext.Trace(11000007, TraceLevel.Error, "SocialActivityAggregation", "Service", string.Format("Number of artifactIds: {0} passed to {1} exceeds the maximum limit: {2}", (object) artifactIds.Count, (object) nameof (GetSocialActivityAggregatedArtifactBatch), (object) registryValue));
        ArgumentUtility.CheckCollectionForMaxLength<string>((ICollection<string>) artifactIds, nameof (artifactIds), registryValue);
      }
      using (SocialActivityAggregationComponent component = requestContext.CreateComponent<SocialActivityAggregationComponent>())
        return (IList<SocialActivityAggregatedArtifact>) component.GetSocialActivityAggregatedArtifactRecordBatch(dataSpaceIdentifier, providerId, artifactType, artifactIds).ToList<SocialActivityAggregatedArtifact>();
    }

    public void DeleteSocialActivityAggregatedArtifactBatch(
      IVssRequestContext requestContext,
      Guid dataSpaceIdentifier,
      Guid providerId,
      byte artifactType,
      IList<string> artifactIds)
    {
      ArgumentUtility.CheckForEmptyGuid(providerId, nameof (providerId));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) artifactIds, nameof (artifactIds));
      int registryValue = RegistryHelper.GetRegistryValue<int>(requestContext, "/Configuration/SocialSDK/SocialActivity/SocialActivityAggregatedArtifactMaxArtifactIdCount", 1000);
      if (artifactIds.Count > registryValue)
      {
        requestContext.Trace(11000007, TraceLevel.Error, "SocialActivityAggregation", "Service", string.Format("Number of artifactIds: {0} passed to {1} exceeds the maximum limit: {2}", (object) artifactIds.Count, (object) "GetSocialActivityAggregatedArtifactBatch", (object) registryValue));
        ArgumentUtility.CheckCollectionForMaxLength<string>((ICollection<string>) artifactIds, nameof (artifactIds), registryValue);
      }
      using (SocialActivityAggregationComponent component = requestContext.CreateComponent<SocialActivityAggregationComponent>())
        component.DeleteSocialActivityAggregatedArtifactRecordBatch(dataSpaceIdentifier, providerId, artifactType, artifactIds);
    }

    internal int DeleteActivityAggregatedMetricRecords(
      IVssRequestContext requestContext,
      Guid providerId,
      DateTime till)
    {
      ArgumentUtility.CheckForEmptyGuid(providerId, nameof (providerId));
      using (SocialActivityAggregationComponent component = requestContext.CreateComponent<SocialActivityAggregationComponent>())
        return component.DeleteActivityAggregatedMetricRecords(providerId, till);
    }

    internal void AddActivityInternal(
      IVssRequestContext requestContext,
      string activityType,
      Guid activityId,
      DateTime activityTimeStamp,
      Guid userId,
      string data,
      string extendedData,
      bool flushImmediately)
    {
      ArgumentUtility.CheckStringLength(activityType, nameof (activityType), 24, 1);
      ArgumentUtility.CheckStringLength(data, nameof (data), 1024);
      ArgumentUtility.CheckStringLength(extendedData, nameof (extendedData), 4000);
      ArgumentUtility.CheckForEmptyGuid(activityId, nameof (activityId));
      SocialActivityRecord socialActivityRecord = new SocialActivityRecord()
      {
        ActivityId = activityId,
        ActivityTimeStamp = activityTimeStamp,
        ActivityType = activityType,
        UserId = userId,
        Data = data,
        ExtendedData = extendedData
      };
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "AddActivity.GetSocialSdkActivityProvider"))
      {
        performanceTimer.AddProperty("ActivityType", (object) socialActivityRecord.ActivityType);
        performanceTimer.AddProperty("ActivityId", (object) socialActivityRecord.ActivityId);
        this.GetSocialSdkActivityProvider(requestContext, socialActivityRecord.ActivityType);
      }
      if (flushImmediately)
      {
        using (SocialActivityAggregationComponent component = requestContext.SqlComponentCreator.CreateComponent<SocialActivityAggregationComponent>(requestContext))
        {
          List<SocialActivityRecord> socialActivities = new List<SocialActivityRecord>();
          socialActivities.Add(socialActivityRecord);
          if (component.AddSocialActivityRecords((IList<SocialActivityRecord>) socialActivities) == socialActivities.Count)
            return;
          requestContext.TraceException(11000001, "SocialActivityAggregation", "SocialActivityCacheService", new Exception("Failed to insert the activity into sql"));
        }
      }
      else
        requestContext.GetService<SocialActivityCacheService>().Add(requestContext, socialActivityRecord);
    }

    internal virtual void AddOrUpdateAggregatedArtifactsRecords(
      IVssRequestContext requestContext,
      Guid dataSpaceId,
      IList<AggregatedArtifactsRecord> aggregatedArtifactsRecords)
    {
      int registryValue = RegistryHelper.GetRegistryValue<int>(requestContext, "/Configuration/SocialSDK/SocialActivity/SocialActivityAggregatedRecordsMaxCount", 20000);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) aggregatedArtifactsRecords, nameof (aggregatedArtifactsRecords));
      ArgumentUtility.CheckCollectionForMaxLength<AggregatedArtifactsRecord>((ICollection<AggregatedArtifactsRecord>) aggregatedArtifactsRecords, nameof (aggregatedArtifactsRecords), registryValue);
      using (SocialActivityAggregationComponent component = requestContext.CreateComponent<SocialActivityAggregationComponent>())
        component.AddOrUpdateAggregatedArtifactsRecord(dataSpaceId, aggregatedArtifactsRecords);
    }

    internal virtual void AddOrUpdateActivityAggregatedMetricRecords(
      IVssRequestContext requestContext,
      IList<SocialActivityAggregatedMetric> aggregatedMetrics)
    {
      int registryValue = RegistryHelper.GetRegistryValue<int>(requestContext, "/Configuration/SocialSDK/SocialActivity/SocialActivityAggregatedRecordsMaxCount", 20000);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) aggregatedMetrics, nameof (aggregatedMetrics));
      ArgumentUtility.CheckCollectionForMaxLength<SocialActivityAggregatedMetric>((ICollection<SocialActivityAggregatedMetric>) aggregatedMetrics, nameof (aggregatedMetrics), registryValue);
      using (SocialActivityAggregationComponent component = requestContext.CreateComponent<SocialActivityAggregationComponent>())
        component.AddOrUpdateActivityAggregatedMetricRecords(aggregatedMetrics);
    }

    internal virtual void AddOrUpdateSocialPluginWatermark(
      IVssRequestContext requestContext,
      Guid jobId,
      Guid providerId,
      DateTime processedTill)
    {
      ArgumentUtility.CheckForEmptyGuid(jobId, nameof (jobId));
      ArgumentUtility.CheckForEmptyGuid(providerId, nameof (providerId));
      using (SocialActivityAggregationComponent component = requestContext.CreateComponent<SocialActivityAggregationComponent>())
        component.AddOrUpdateSocialPluginWatermark(jobId, providerId, processedTill);
    }

    internal virtual void AddOrUpdateSocialActivityAggregatedArtifactRecords(
      IVssRequestContext requestContext,
      Guid dataSpaceId,
      IList<SocialActivityAggregatedArtifact> aggregatedArtifactsRecords)
    {
      int registryValue = RegistryHelper.GetRegistryValue<int>(requestContext, "/Configuration/SocialSDK/SocialActivity/SocialActivityAggregatedRecordsMaxCount", 20000);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) aggregatedArtifactsRecords, nameof (aggregatedArtifactsRecords));
      ArgumentUtility.CheckCollectionForMaxLength<SocialActivityAggregatedArtifact>((ICollection<SocialActivityAggregatedArtifact>) aggregatedArtifactsRecords, nameof (aggregatedArtifactsRecords), registryValue);
      using (SocialActivityAggregationComponent component = requestContext.CreateComponent<SocialActivityAggregationComponent>())
        component.AddOrUpdateSocialActivityAggregatedArtifactRecords(dataSpaceId, aggregatedArtifactsRecords);
    }

    internal virtual DateTime GetSocialPluginProcessedTillTime(
      IVssRequestContext requestContext,
      Guid jobId,
      Guid providerId)
    {
      ArgumentUtility.CheckForEmptyGuid(jobId, nameof (jobId));
      ArgumentUtility.CheckForEmptyGuid(providerId, nameof (providerId));
      using (SocialActivityAggregationComponent component = requestContext.CreateComponent<SocialActivityAggregationComponent>())
        return component.GetSocialPluginProcessedTillTime(jobId, providerId);
    }

    private ISocialSdkSocialActivityProvider GetSocialSdkActivityProvider(
      IVssRequestContext requestContext,
      string activityType)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<ISocialActivityProviderService>().GetSocialSdkSocialActivityProvider(vssRequestContext, activityType);
    }
  }
}
