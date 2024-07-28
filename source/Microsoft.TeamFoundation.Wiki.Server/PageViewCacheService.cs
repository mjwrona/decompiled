// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.PageViewCacheService
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class PageViewCacheService : IVssFrameworkService
  {
    private ConcurrentDictionary<PageViewKey, PageViewValue> m_cache = new ConcurrentDictionary<PageViewKey, PageViewValue>();
    private const int c_FlushIntervalInSecondsDefaultValue = 30;
    private const string c_WikiSettingsParentPath = "/Service/Wiki/Settings/";
    private const string c_FlushIntervalInSecondsKey = "/Service/Wiki/Settings/PageViewsFlushToDbIntervalInSeconds";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckProjectCollectionRequestContext();
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnFlushToDbIntervalRegistryUpdated), true, "/Service/Wiki/Settings/PageViewsFlushToDbIntervalInSeconds");
      TeamFoundationTaskService service = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
      this.AddFlushToDbTask(systemRequestContext, service);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckProjectCollectionRequestContext();
      systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().RemoveTask(systemRequestContext, new TeamFoundationTaskCallback(this.FlushToDb));
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnFlushToDbIntervalRegistryUpdated));
    }

    public virtual PageViewValue AddOrUpdate(
      IVssRequestContext requestContext,
      PageViewKey currentKey,
      PageViewKey prevKey = null)
    {
      PageViewKey key1 = currentKey;
      PageViewKey key2 = prevKey ?? key1;
      bool flag = key1 == key2;
      PageViewValue pageViewValue1;
      PageViewValue pageViewValue2;
      if (this.m_cache.TryGetValue(key2, out pageViewValue1))
      {
        if (flag)
        {
          pageViewValue1.IncrementViewCount();
          pageViewValue2 = pageViewValue1;
        }
        else
        {
          pageViewValue2 = new PageViewValue(key1, 0, pageViewValue1.ViewCountBase + pageViewValue1.ViewCountDelta + 1, DateTime.UtcNow);
          this.m_cache.TryAdd(key1, pageViewValue2);
        }
      }
      else
      {
        PageView pageView = (PageView) null;
        using (WikiComponent component = requestContext.SqlComponentCreator.CreateComponent<WikiComponent>(requestContext))
          pageView = component.GetPageView(new PageView(component.GetDataspaceId(key2.ProjectId), key2.WikiId, key2.Version, key2.Path));
        int viewCountBase = 0;
        int viewCountDelta = 1;
        if (pageView != null)
        {
          viewCountBase = flag ? pageView.ViewCount : 0;
          viewCountDelta = flag ? 1 : pageView.ViewCount + 1;
        }
        PageViewValue newValue = new PageViewValue(key1, viewCountBase, viewCountDelta, DateTime.UtcNow);
        pageViewValue2 = this.m_cache.AddOrUpdate(key1, newValue, (Func<PageViewKey, PageViewValue, PageViewValue>) ((key, oldValue) => new PageViewValue(key, oldValue.ViewCountBase > newValue.ViewCountBase ? oldValue.ViewCountBase : newValue.ViewCountBase, oldValue.ViewCountDelta + newValue.ViewCountDelta, newValue.LastViewedAt)));
      }
      return pageViewValue2;
    }

    private void AddFlushToDbTask(
      IVssRequestContext requestContext,
      TeamFoundationTaskService taskService)
    {
      TeamFoundationTask task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.FlushToDb), (object) this, (int) TimeSpan.FromSeconds((double) PageViewCacheService.GetFlushIntervalInSeconds(requestContext)).TotalMilliseconds);
      taskService.AddTask(requestContext, task);
    }

    private void FlushToDb(IVssRequestContext requestContext, object taskArgs)
    {
      if (this.m_cache.IsEmpty)
        return;
      ConcurrentDictionary<PageViewKey, PageViewValue> cache = this.m_cache;
      this.m_cache = new ConcurrentDictionary<PageViewKey, PageViewValue>();
      using (WikiComponent component = requestContext.SqlComponentCreator.CreateComponent<WikiComponent>(requestContext))
      {
        List<PageView> pageViewsList = new List<PageView>();
        foreach (KeyValuePair<PageViewKey, PageViewValue> keyValuePair in cache)
        {
          PageViewKey key = keyValuePair.Key;
          PageViewValue pageViewValue = keyValuePair.Value;
          pageViewsList.Add(new PageView(component.GetDataspaceId(key.ProjectId), key.WikiId, key.Version, key.Path, pageViewValue.ViewCountDelta, pageViewValue.LastViewedAt));
        }
        component.UpdatePageViewBatch(pageViewsList);
        cache.Clear();
      }
    }

    private static int GetFlushIntervalInSeconds(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Wiki/Settings/PageViewsFlushToDbIntervalInSeconds", true, 30);

    private static TeamFoundationTaskService GetTaskService(IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();

    private void OnFlushToDbIntervalRegistryUpdated(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      TeamFoundationTaskService taskService = PageViewCacheService.GetTaskService(requestContext);
      taskService.RemoveTask(requestContext, new TeamFoundationTaskCallback(this.FlushToDb));
      this.AddFlushToDbTask(requestContext, taskService);
    }
  }
}
