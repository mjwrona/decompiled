// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestSuiteCacheService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestSuiteCacheService : 
    VssMemoryCacheService<int, CachedWorkItemData>,
    ITestManagementWorkItemCacheService
  {
    private static MemoryCacheConfiguration<int, CachedWorkItemData> s_defaultCacheConfiguration = new MemoryCacheConfiguration<int, CachedWorkItemData>().WithCleanupInterval(TestSuiteCacheConstants.TestSuiteCacheCleanupInterval).WithExpiryInterval(TestSuiteCacheConstants.TestSuiteCacheExpirationInterval).WithInactivityInterval(TestSuiteCacheConstants.TestSuiteCacheExpirationInterval).WithMaxElements(1000);

    public TestSuiteCacheService()
      : base((IEqualityComparer<int>) EqualityComparer<int>.Default, TestSuiteCacheService.s_defaultCacheConfiguration)
    {
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckProjectCollectionRequestContext();
      base.ServiceStart(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", TestSuiteCacheConstants.TestSuiteChangedEventClass, new SqlNotificationCallback(this.OnSuiteChanged), false);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", TestSuiteCacheConstants.TestSuiteChangedEventClass, new SqlNotificationCallback(this.OnSuiteChanged), false);
      base.ServiceEnd(systemRequestContext);
    }

    public bool TryUpdateWorkItemCache(
      IVssRequestContext requestContext,
      List<CachedWorkItemData> suites)
    {
      if (suites == null)
        return false;
      foreach (CachedWorkItemData suite in suites)
      {
        if (suite.Id > 0)
          this.Set(requestContext, suite.Id, suite);
      }
      return true;
    }

    public bool TryGetCachedWorkItemData(
      IVssRequestContext requestContext,
      int workItemId,
      out CachedWorkItemData cachedWorkItem)
    {
      cachedWorkItem = (CachedWorkItemData) null;
      CachedWorkItemData cachedWorkItemData;
      if (!this.TryGetValue(requestContext, workItemId, out cachedWorkItemData))
        return false;
      cachedWorkItem = cachedWorkItemData;
      return true;
    }

    private void OnSuiteChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (!object.Equals((object) TestSuiteCacheConstants.TestSuiteChangedEventClass, (object) eventClass) || string.IsNullOrWhiteSpace(eventData))
        return;
      CachedWorkItemData cachedWorkItemData1 = JsonUtilities.Deserialize<CachedWorkItemData>(eventData);
      CachedWorkItemData cachedWorkItemData2;
      if (!this.TryPeekValue(requestContext, cachedWorkItemData1.Id, out cachedWorkItemData2))
        return;
      if (!string.IsNullOrEmpty(cachedWorkItemData1.AreaUri))
        cachedWorkItemData2.AreaUri = cachedWorkItemData1.AreaUri;
      if (!string.IsNullOrEmpty(cachedWorkItemData1.Title))
        cachedWorkItemData2.Title = cachedWorkItemData1.Title;
      this.Set(requestContext, cachedWorkItemData1.Id, cachedWorkItemData2);
    }
  }
}
