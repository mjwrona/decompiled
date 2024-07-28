// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanAssignedTestersCacheService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestPlanAssignedTestersCacheService : 
    VssMemoryCacheService<int, Dictionary<Guid, CachedIdentityData>>,
    ITestManagementTestPlanAssignedTestersCacheService,
    IVssFrameworkService
  {
    private static MemoryCacheConfiguration<int, Dictionary<Guid, CachedIdentityData>> s_defaultCacheConfiguration = new MemoryCacheConfiguration<int, Dictionary<Guid, CachedIdentityData>>().WithCleanupInterval(TestPointFiltersCacheConstants.TestPointFiltersCacheCleanupInterval).WithExpiryInterval(TestPointFiltersCacheConstants.TestPointFiltersCacheExpirationInterval).WithInactivityInterval(TestPointFiltersCacheConstants.TestPointFiltersCacheExpirationInterval).WithMaxElements(100);

    public TestPlanAssignedTestersCacheService()
      : base((IEqualityComparer<int>) EqualityComparer<int>.Default, TestPlanAssignedTestersCacheService.s_defaultCacheConfiguration)
    {
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckProjectCollectionRequestContext();
      base.ServiceStart(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", TestPointFiltersCacheConstants.TestPointFiltersTestersChangedEventClass, new SqlNotificationCallback(this.OnPlanTestersChanged), false);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", TestPointFiltersCacheConstants.TestPointFiltersTestersChangedEventClass, new SqlNotificationCallback(this.OnPlanTestersChanged), false);
      base.ServiceEnd(systemRequestContext);
    }

    public bool TryUpdateTestPlanAssignedTestersCache(
      IVssRequestContext requestContext,
      int testPlanId,
      Dictionary<Guid, CachedIdentityData> assignedTesters)
    {
      if (assignedTesters == null || testPlanId <= 0)
        return false;
      this.Set(requestContext, testPlanId, assignedTesters);
      return true;
    }

    public bool TryGetCachedTestPlanAssignedTestersData(
      IVssRequestContext requestContext,
      int testPlanId,
      out Dictionary<Guid, CachedIdentityData> assignedTesters)
    {
      assignedTesters = (Dictionary<Guid, CachedIdentityData>) null;
      Dictionary<Guid, CachedIdentityData> dictionary;
      if (!this.TryGetValue(requestContext, testPlanId, out dictionary))
        return false;
      assignedTesters = dictionary;
      return true;
    }

    private void OnPlanTestersChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (!object.Equals((object) TestPointFiltersCacheConstants.TestPointFiltersTestersChangedEventClass, (object) eventClass) || string.IsNullOrWhiteSpace(eventData))
        return;
      CachedTestersUpdateData testersUpdateData = JsonUtilities.Deserialize<CachedTestersUpdateData>(eventData);
      Dictionary<Guid, CachedIdentityData> dictionary;
      if (!this.TryPeekValue(requestContext, testersUpdateData.TestPlanId, out dictionary) || testersUpdateData.Testers == null)
        return;
      foreach (CachedIdentityData tester in (IEnumerable<CachedIdentityData>) testersUpdateData.Testers)
        dictionary[tester.Id] = tester;
      this.Set(requestContext, testersUpdateData.TestPlanId, dictionary);
    }
  }
}
