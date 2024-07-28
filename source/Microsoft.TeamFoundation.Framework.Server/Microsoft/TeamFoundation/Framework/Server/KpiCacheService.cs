// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.KpiCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class KpiCacheService : VssCacheService
  {
    private VssMemoryCacheList<string, KpiDefinition> m_cache;
    private INotificationRegistration m_kpiRegistration;

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      this.m_cache = new VssMemoryCacheList<string, KpiDefinition>((IVssCachePerformanceProvider) this, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase, 1000);
      this.m_kpiRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.KpiChanged, new SqlNotificationCallback(this.OnKpiDefinitionChanged), false, false);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_kpiRegistration.Unregister(systemRequestContext);
      this.m_cache.Clear();
      base.ServiceEnd(systemRequestContext);
    }

    public bool TryGetValue(
      IVssRequestContext requestContext,
      string area,
      string name,
      string scope,
      out KpiDefinition definition)
    {
      return this.m_cache.TryGetValue(this.GenerateKey(area, name, scope), out definition);
    }

    public void Set(
      IVssRequestContext requestContext,
      string area,
      string name,
      string scope,
      KpiDefinition definition)
    {
      this.m_cache.Add(this.GenerateKey(area, name, scope), definition, true);
    }

    public void Remove(IVssRequestContext requestContext, string area, string name, string scope) => this.m_cache.Remove(this.GenerateKey(area, name, scope));

    private string GenerateKey(string area, string name, string scope) => string.Format("{0}-{1}-{2}", (object) area, (object) name, (object) (scope ?? "default"));

    private void OnKpiDefinitionChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (string.IsNullOrEmpty(eventData))
        this.m_cache.Clear();
      else
        this.m_cache.Remove(eventData);
    }
  }
}
