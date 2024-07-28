// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.SocialActivityCacheService
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  internal class SocialActivityCacheService : IVssFrameworkService
  {
    private readonly ConcurrentQueue<SocialActivityRecord> m_cache = new ConcurrentQueue<SocialActivityRecord>();

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionOrOrganizationRequestContext();
      TeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
      int interval = RegistryHelper.GetRegistryValue<int>(requestContext, "/Configuration/SocialSDK/SocialActivity/MemoryCachecFlushIntervalInSec", 300) * 1000;
      IVssRequestContext requestContext1 = requestContext;
      TeamFoundationTask task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.Flush), (object) this, interval);
      service.AddTask(requestContext1, task);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionOrOrganizationRequestContext();
      requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().RemoveTask(requestContext, new TeamFoundationTaskCallback(this.Flush));
    }

    internal void Add(IVssRequestContext requestContext, SocialActivityRecord socialActivityRecord)
    {
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "SocialActivityCacheService.Add"))
      {
        performanceTimer.AddProperty("ActivityType", (object) socialActivityRecord.ActivityType);
        this.m_cache.Enqueue(socialActivityRecord);
      }
    }

    private void Flush(IVssRequestContext requestContext, object taskArgs)
    {
      if (this.m_cache.IsEmpty)
        return;
      int num = 10000;
      List<SocialActivityRecord> activityRecords = new List<SocialActivityRecord>();
      SocialActivityRecord result;
      for (int index = 0; index < num && this.m_cache.TryDequeue(out result); ++index)
        activityRecords.Add(result);
      SocialActivityCacheService.FlushToDb(requestContext, activityRecords);
      this.FlushToPlugins(requestContext, activityRecords);
    }

    private static void FlushToDb(
      IVssRequestContext requestContext,
      List<SocialActivityRecord> activityRecords)
    {
      int count = activityRecords.Count;
      using (SocialActivityAggregationComponent component = requestContext.CreateComponent<SocialActivityAggregationComponent>())
      {
        int num = component.AddSocialActivityRecords((IList<SocialActivityRecord>) activityRecords);
        if (num != activityRecords.Count)
          requestContext.TraceException(11000001, "SocialActivityAggregation", nameof (SocialActivityCacheService), new Exception(string.Format("Out of {0} total records, sql created {1} records.", (object) count, (object) num)));
      }
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("FlushedActivityRecordsCount", (double) count);
      IVssRequestContext requestContext1 = requestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, "SocialActivityAggregation", nameof (SocialActivityCacheService), properties);
    }

    private void FlushToPlugins(
      IVssRequestContext requestContext,
      List<SocialActivityRecord> activityRecords)
    {
      int num = 0;
      using (TimedCiEvent timedCiEvent = new TimedCiEvent(requestContext, "SocialActivityAggregation", nameof (FlushToPlugins)))
      {
        using (IDisposableReadOnlyList<IInMemorySocialAggregation> extensions = requestContext.GetExtensions<IInMemorySocialAggregation>())
        {
          foreach (IInMemorySocialAggregation socialAggregation in (IEnumerable<IInMemorySocialAggregation>) extensions)
          {
            try
            {
              socialAggregation.AddActivities(requestContext, (IEnumerable<SocialActivityRecord>) activityRecords);
            }
            catch (Exception ex)
            {
              ++num;
              requestContext.TraceException(11000001, TraceLevel.Error, "SocialActivityAggregation", nameof (SocialActivityCacheService), ex);
            }
          }
          timedCiEvent.Properties.Add("numInMemoryExtensions", (object) extensions.Count);
          timedCiEvent.Properties.Add("numActivityRecords", (object) activityRecords.Count);
          timedCiEvent.Properties.Add("numfailedExtensions", (object) num);
        }
      }
    }
  }
}
