// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.UxServices.UxServicesCacheService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.UxServices
{
  public class UxServicesCacheService : 
    VssCacheService,
    IUxServicesCacheService,
    IVssFrameworkService
  {
    private VssMemoryCacheList<string, object> m_cache;
    private TeamFoundationTask m_cleanupTask;
    private UxServicesCacheConfiguration m_currentUxServicesConfiguration;
    private VssRefreshCache<UxServicesCacheConfiguration> uxServicesConfigurationCache;
    private bool m_updateLastRefreshDate;
    private IVssDateTimeProvider m_dateTimeProvider;
    private static readonly TimeSpan DefaultRefreshInterval = TimeSpan.FromMinutes(10.0);
    private const string UxServicesCacheRootRegKey = "/WebAccess/UxServices/Cache";
    private const string CacheRefreshIntervalRegKey = "/WebAccess/UxServices/Cache/RefreshInterval";
    private const string CacheLastRefreshedRegKey = "/WebAccess/UxServices/Cache/LastRefreshedDate";

    public UxServicesCacheService() => this.m_dateTimeProvider = VssDateTimeProvider.DefaultProvider;

    internal UxServicesCacheService(IVssDateTimeProvider dateTimeProvider) => this.m_dateTimeProvider = dateTimeProvider;

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      base.ServiceStart(requestContext);
      this.uxServicesConfigurationCache = new VssRefreshCache<UxServicesCacheConfiguration>(UxServicesCacheService.DefaultRefreshInterval, new Func<IVssRequestContext, UxServicesCacheConfiguration>(this.FetchUxServicesCacheConfiguration));
      this.m_cache = new VssMemoryCacheList<string, object>((IVssCachePerformanceProvider) this);
      this.m_cleanupTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.DoCleanup), (object) null, (int) UxServicesCacheService.DefaultRefreshInterval.TotalMilliseconds);
      requestContext.GetService<ITeamFoundationTaskService>().AddTask(requestContext, this.m_cleanupTask);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.GetService<ITeamFoundationTaskService>().RemoveTask(requestContext, this.m_cleanupTask);
      this.m_cache.Clear();
      this.uxServicesConfigurationCache = (VssRefreshCache<UxServicesCacheConfiguration>) null;
      base.ServiceEnd(requestContext);
    }

    public bool TryGetValue(IVssRequestContext requestContext, string key, out object value) => this.m_cache.TryGetValue(key, out value);

    public void Set(IVssRequestContext requestContext, string key, object value) => this.m_cache.Add(key, value, true);

    public bool Remove(IVssRequestContext requestContext, string key) => this.m_cache.Remove(key);

    public void RefreshUxServicesCacheConfiguration(IVssRequestContext context)
    {
      if (this.m_updateLastRefreshDate)
      {
        context.GetService<IVssRegistryService>().SetValue<DateTime>(context, "/WebAccess/UxServices/Cache/LastRefreshedDate", this.m_dateTimeProvider.UtcNow);
        this.m_updateLastRefreshDate = false;
      }
      this.m_currentUxServicesConfiguration = this.uxServicesConfigurationCache.Get(context);
    }

    internal UxServicesCacheConfiguration FetchUxServicesCacheConfiguration(
      IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      TimeSpan timeSpan = TimeSpan.FromHours((double) service.GetValue<int>(requestContext, (RegistryQuery) "/WebAccess/UxServices/Cache/RefreshInterval", 24));
      DateTime dateTime = service.GetValue<DateTime>(requestContext, (RegistryQuery) "/WebAccess/UxServices/Cache/LastRefreshedDate", this.m_dateTimeProvider.UtcNow);
      return new UxServicesCacheConfiguration()
      {
        CacheRefreshInterval = timeSpan,
        CacheLastRefreshed = (DateTimeOffset) dateTime
      };
    }

    internal void DoCleanup(IVssRequestContext requestContext, object state)
    {
      UxServicesCacheConfiguration servicesConfiguration = this.m_currentUxServicesConfiguration;
      if (servicesConfiguration == null || servicesConfiguration.CacheLastRefreshed.Add(servicesConfiguration.CacheRefreshInterval) > (DateTimeOffset) this.m_dateTimeProvider.UtcNow)
        return;
      this.m_cache.Clear();
      this.m_updateLastRefreshDate = true;
    }
  }
}
