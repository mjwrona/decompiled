// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache.RecentActivityDataProviderService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D4A0500-806F-44D4-BA97-D409A2311716
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.WorkItemRecentHistoryCache
{
  public class RecentActivityDataProviderService : 
    IRecentActivityDataProviderService,
    IVssFrameworkService
  {
    private int maxActivitiesPerUserPerProject;
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1081324, "Data Provider", "WorkItemRecentActivityDataProviderService");
    private RecentActivityCacheService cacheService;

    private long cacheHitCountForGet { get; set; }

    private long cacheMissCountForGet { get; set; }

    private long cacheHitCountForSet { get; set; }

    private long cacheMissCountForSet { get; set; }

    private long sqlWriteCount { get; set; }

    public RecentActivityDataProviderService()
    {
    }

    public RecentActivityDataProviderService(RecentActivityCacheService cacheService) => this.cacheService = cacheService;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      systemRequestContext.CheckProjectCollectionRequestContext();
      if (this.cacheService == null)
        this.cacheService = systemRequestContext.GetService<RecentActivityCacheService>();
      this.maxActivitiesPerUserPerProject = systemRequestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/WorkItemMaxRecentActivitiesPerProjectPerUserInDB", 50);
      this.cacheHitCountForGet = 0L;
      this.cacheHitCountForSet = 0L;
      this.cacheMissCountForGet = 0L;
      this.cacheMissCountForSet = 0L;
      this.sqlWriteCount = 0L;
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("ServiceStartTime", "WorkItemRecentActivityDataProviderService", (double) stopwatch.ElapsedMilliseconds);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public RecencyData GetRecentActivities(
      IVssRequestContext requestContext,
      Guid userId,
      Guid projectId)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      RecencyData recentActivities = (RecencyData) null;
      if (requestContext.IsFeatureEnabled("Search.Server.WorkItem.RecentActivityCacheEnabled"))
      {
        RecentActivityDetails activitiesFromCache = this.cacheService.GetRecentActivitiesFromCache(requestContext, projectId, userId, true);
        if (activitiesFromCache != null)
        {
          recentActivities = new RecencyData();
          ++this.cacheHitCountForGet;
          foreach (int num in new List<int>((IEnumerable<int>) activitiesFromCache.Details.Keys))
            recentActivities.workItemIds.Add(num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          Dictionary<int, int> dictionary1 = new Dictionary<int, int>();
          foreach (KeyValuePair<int, ItemDetails> detail in activitiesFromCache.Details)
          {
            Dictionary<int, int> dictionary2 = dictionary1;
            ItemDetails itemDetails = detail.Value;
            int areaId1 = itemDetails.AreaId;
            int num1;
            ref int local = ref num1;
            if (dictionary2.TryGetValue(areaId1, out local))
            {
              Dictionary<int, int> dictionary3 = dictionary1;
              itemDetails = detail.Value;
              int areaId2 = itemDetails.AreaId;
              int num2 = num1 + 1;
              dictionary3[areaId2] = num2;
            }
            else
            {
              Dictionary<int, int> dictionary4 = dictionary1;
              itemDetails = detail.Value;
              int areaId3 = itemDetails.AreaId;
              dictionary4.Add(areaId3, 1);
            }
          }
          foreach (KeyValuePair<int, int> keyValuePair in dictionary1)
          {
            if (keyValuePair.Value > 1)
              recentActivities.areaIds.Add(keyValuePair.Key.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          }
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("Cache hit count for Get Request", (double) this.cacheHitCountForGet);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("Data Provider", "WorkItemRecentActivityDataProviderService", properties);
        }
      }
      if (recentActivities == null || !requestContext.IsFeatureEnabled("Search.Server.WorkItem.RecentActivityCacheEnabled"))
      {
        ++this.cacheMissCountForGet;
        recentActivities = new RecencyData();
        IReadOnlyCollection<RecentActivity> activitiesFromDatabase = this.GetRecentActivitiesFromDatabase(requestContext, userId, projectId);
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        foreach (RecentActivity recentActivity in (IEnumerable<RecentActivity>) activitiesFromDatabase)
        {
          int result;
          if (requestContext.IsFeatureEnabled("Search.Server.WorkItem.RecentActivityCacheEnabled") && int.TryParse(recentActivity.ArtifactId, out result))
            this.cacheService.UpdateRecentActivitiesInCache(requestContext, recentActivity.IdentityId, recentActivity.ProjectId, recentActivity.ActivityDate, result, recentActivity.AreaId);
          recentActivities.workItemIds.Add(recentActivity.ArtifactId.ToString());
          int num;
          if (dictionary.TryGetValue(recentActivity.AreaId.ToString((IFormatProvider) CultureInfo.InvariantCulture), out num))
            dictionary[recentActivity.AreaId.ToString((IFormatProvider) CultureInfo.InvariantCulture)] = num + 1;
          else
            dictionary.Add(recentActivity.AreaId.ToString((IFormatProvider) CultureInfo.InvariantCulture), 1);
        }
        foreach (KeyValuePair<string, int> keyValuePair in dictionary)
        {
          if (keyValuePair.Value > 1)
            recentActivities.areaIds.Add(keyValuePair.Key);
        }
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("Cache miss count for Get Request", (double) this.cacheMissCountForGet);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("Data Provider", "WorkItemRecentActivityDataProviderService", properties);
      }
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("OverallFetchTime", "WorkItemRecentActivityDataProviderService", (double) stopwatch.ElapsedMilliseconds);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081315, "Data Provider", "WorkItemRecentActivityDataProviderService", recentActivities.ToString());
      return recentActivities;
    }

    public void UpdateRecentActivities(
      IVssRequestContext requestContext,
      Guid userId,
      Guid projectId,
      DateTime activityDate,
      int artifactId,
      int areaId)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.UpdateRecentActivitiesInDatabase(requestContext, userId, projectId, activityDate, artifactId, areaId);
      CustomerIntelligenceData properties1 = new CustomerIntelligenceData();
      properties1.Add("SQL write count", (double) this.sqlWriteCount);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("Data Provider", "WorkItemRecentActivityDataProviderService", properties1);
      if (requestContext.IsFeatureEnabled("Search.Server.WorkItem.RecentActivityCacheEnabled"))
      {
        if (this.cacheService.GetRecentActivitiesFromCache(requestContext, projectId, userId, false) != null)
        {
          ++this.cacheHitCountForSet;
          this.cacheService.UpdateRecentActivitiesInCache(requestContext, userId, projectId, activityDate, artifactId, areaId);
          CustomerIntelligenceData properties2 = new CustomerIntelligenceData();
          properties2.Add("Cache hit count for Set Request", (double) this.cacheHitCountForSet);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("Data Provider", "WorkItemRecentActivityDataProviderService", properties2);
        }
        else
        {
          ++this.cacheMissCountForSet;
          CustomerIntelligenceData properties3 = new CustomerIntelligenceData();
          properties3.Add("Cache miss count for Set Request", (double) this.cacheMissCountForSet);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("Data Provider", "WorkItemRecentActivityDataProviderService", properties3);
        }
      }
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("OverallUpdateTime", "WorkItemRecentActivityDataProviderService", (double) stopwatch.ElapsedMilliseconds);
    }

    public void PopulateUserRecencyDataInCache(IVssRequestContext requestContext, Guid projectId)
    {
      Guid id = requestContext.GetUserIdentity().Id;
      RecentActivityDetails activitiesFromCache1 = this.cacheService.GetRecentActivitiesFromCache(requestContext, projectId, id, false);
      StringBuilder stringBuilder = new StringBuilder("Result from cache: " + (activitiesFromCache1 == null ? "" : activitiesFromCache1.ToString()));
      if (activitiesFromCache1 == null)
      {
        IReadOnlyCollection<RecentActivity> activitiesFromDatabase = this.GetRecentActivitiesFromDatabase(requestContext, id, projectId);
        string str = "{" + (activitiesFromDatabase == null ? "" : string.Join<RecentActivity>(",", (IEnumerable<RecentActivity>) activitiesFromDatabase)) + "}";
        stringBuilder.Append("Result from SQL: " + str);
        foreach (RecentActivity recentActivity in (IEnumerable<RecentActivity>) activitiesFromDatabase)
        {
          int result;
          if (int.TryParse(recentActivity.ArtifactId, out result))
            this.cacheService.UpdateRecentActivitiesInCache(requestContext, id, projectId, recentActivity.ActivityDate, result, recentActivity.AreaId);
        }
      }
      RecentActivityDetails activitiesFromCache2 = this.cacheService.GetRecentActivitiesFromCache(requestContext, projectId, id, false);
      stringBuilder.Append("Modified result from cache: " + (activitiesFromCache2 == null ? "" : activitiesFromCache2.ToString()));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081315, "Data Provider", "WorkItemRecentActivityDataProviderService", stringBuilder.ToString());
    }

    public void CleanupRecentUserActivities(IVssRequestContext requestContext)
    {
      using (WorkItemRecentActivityComponent component = requestContext.CreateComponent<WorkItemRecentActivityComponent>())
        component.CleanupRecentProjectUserActivity(this.maxActivitiesPerUserPerProject);
    }

    private IReadOnlyCollection<RecentActivity> GetRecentActivitiesFromDatabase(
      IVssRequestContext requestContext,
      Guid userId,
      Guid projectId)
    {
      IReadOnlyCollection<RecentActivity> activitiesFromDatabase = (IReadOnlyCollection<RecentActivity>) null;
      try
      {
        using (WorkItemRecentActivityComponent component = requestContext.CreateComponent<WorkItemRecentActivityComponent>())
          activitiesFromDatabase = component.GetProjectUserRecentActivities(projectId, userId, this.maxActivitiesPerUserPerProject);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(RecentActivityDataProviderService.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Unable to fetch data from SQL. Exception:{0} StackTrace {1}", (object) ex.Message, (object) ex.StackTrace)));
      }
      return activitiesFromDatabase;
    }

    private void UpdateRecentActivitiesInDatabase(
      IVssRequestContext requestContext,
      Guid userId,
      Guid projectId,
      DateTime activityDate,
      int artifactId,
      int areaId)
    {
      try
      {
        ++this.sqlWriteCount;
        using (WorkItemRecentActivityComponent component = requestContext.CreateComponent<WorkItemRecentActivityComponent>())
          component.UpdateProjectUserRecentActivities(projectId, userId, activityDate, artifactId, areaId);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(RecentActivityDataProviderService.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Unable to store data in SQL. Exception:{0} StackTrace {1}", (object) ex.Message, (object) ex.StackTrace)));
      }
    }
  }
}
