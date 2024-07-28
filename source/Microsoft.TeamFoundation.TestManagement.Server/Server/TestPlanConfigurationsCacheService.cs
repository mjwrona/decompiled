// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanConfigurationsCacheService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestPlanConfigurationsCacheService : 
    VssMemoryCacheService<int, Dictionary<int, TestConfiguration>>,
    ITestManagementTestPlanConfigurationsCacheService,
    IVssFrameworkService
  {
    private static MemoryCacheConfiguration<int, Dictionary<int, TestConfiguration>> s_defaultCacheConfiguration = new MemoryCacheConfiguration<int, Dictionary<int, TestConfiguration>>().WithCleanupInterval(TestPointFiltersCacheConstants.TestPointFiltersCacheCleanupInterval).WithExpiryInterval(TestPointFiltersCacheConstants.TestPointFiltersCacheExpirationInterval).WithInactivityInterval(TestPointFiltersCacheConstants.TestPointFiltersCacheExpirationInterval).WithMaxElements(100);

    public TestPlanConfigurationsCacheService()
      : base((IEqualityComparer<int>) EqualityComparer<int>.Default, TestPlanConfigurationsCacheService.s_defaultCacheConfiguration)
    {
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckProjectCollectionRequestContext();
      base.ServiceStart(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", TestPointFiltersCacheConstants.TestPointFiltersConfigurationsChangedEventClass, new SqlNotificationCallback(this.OnPlanConfigurationsChanged), false);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", TestPointFiltersCacheConstants.TestPointFiltersConfigurationsChangedEventClass, new SqlNotificationCallback(this.OnPlanConfigurationsChanged), false);
      base.ServiceEnd(systemRequestContext);
    }

    public bool TryUpdateTestPlanConfigurationsCache(
      IVssRequestContext requestContext,
      int testPlanId,
      Dictionary<int, TestConfiguration> configurations)
    {
      if (configurations == null || !configurations.Any<KeyValuePair<int, TestConfiguration>>() || testPlanId <= 0)
        return false;
      this.Set(requestContext, testPlanId, configurations);
      return true;
    }

    public bool TryGetCachedTestPlanConfigurationsData(
      IVssRequestContext requestContext,
      int testPlanId,
      out Dictionary<int, TestConfiguration> configurations)
    {
      configurations = (Dictionary<int, TestConfiguration>) null;
      Dictionary<int, TestConfiguration> dictionary;
      if (!this.TryGetValue(requestContext, testPlanId, out dictionary))
        return false;
      configurations = dictionary;
      return true;
    }

    private void OnPlanConfigurationsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (!object.Equals((object) TestPointFiltersCacheConstants.TestPointFiltersConfigurationsChangedEventClass, (object) eventClass) || string.IsNullOrWhiteSpace(eventData))
        return;
      CachedConfigurtionsUpdateData configurtionsUpdateData = JsonUtilities.Deserialize<CachedConfigurtionsUpdateData>(eventData);
      Dictionary<int, TestConfiguration> dictionary;
      if (!this.TryPeekValue(requestContext, configurtionsUpdateData.TestPlanId, out dictionary) || configurtionsUpdateData.Configurations == null)
        return;
      foreach (TestConfiguration configuration in (IEnumerable<TestConfiguration>) configurtionsUpdateData.Configurations)
        dictionary[configuration.Id] = configuration;
      this.Set(requestContext, configurtionsUpdateData.TestPlanId, dictionary);
    }
  }
}
