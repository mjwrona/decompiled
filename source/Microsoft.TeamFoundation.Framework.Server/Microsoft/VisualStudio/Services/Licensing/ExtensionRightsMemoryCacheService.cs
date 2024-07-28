// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ExtensionRightsMemoryCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class ExtensionRightsMemoryCacheService : 
    VssMemoryCacheService<ExtensionRightsCacheKey, ExtensionRightsResult>,
    IExtensionRightsMemoryCacheService,
    IVssFrameworkService,
    IExtensionRightsCache
  {
    private IVssMemoryCacheGrouping<ExtensionRightsCacheKey, ExtensionRightsResult, string> cacheExtensionIdGrouping;
    private const int defaultMaxSize = 15360;
    private static readonly TimeSpan defaultTimeToLiveInterval = TimeSpan.FromDays(1.0);
    private static readonly TimeSpan s_defaultCleanupInterval = TimeSpan.FromMinutes(10.0);
    private static readonly TimeSpan s_defaultMaxCacheInactivityAge = TimeSpan.FromDays(1.0);
    private const string area = "Licensing";
    private const string layer = "ExtensionRightsMemoryCacheService";

    private ExtensionRightsMemoryCacheService()
      : base(ExtensionRightsMemoryCacheService.s_defaultCleanupInterval)
    {
      this.ExpiryInterval.Value = ExtensionRightsMemoryCacheService.defaultTimeToLiveInterval;
      this.InactivityInterval.Value = ExtensionRightsMemoryCacheService.s_defaultMaxCacheInactivityAge;
      this.MaxCacheSize.Value = 15360L;
    }

    public void InvalidateAllUsersInHost(
      IVssRequestContext requestContext,
      Guid targetHostId,
      string extensionId)
    {
      requestContext.CheckOrganizationRequestContext();
      IEnumerable<ExtensionRightsCacheKey> keys;
      if (this.cacheExtensionIdGrouping.TryGetKeys(this.GetExtensionIdReverseMapKey(targetHostId), out keys))
        keys.ForEach<ExtensionRightsCacheKey>((Action<ExtensionRightsCacheKey>) (key =>
        {
          this.Remove(requestContext, key);
          requestContext.Trace(1039035, TraceLevel.Info, "Licensing", nameof (ExtensionRightsMemoryCacheService), string.Format("Invalidated {0}.", (object) key));
        }));
      requestContext.Trace(1039034, TraceLevel.Info, "Licensing", nameof (ExtensionRightsMemoryCacheService), string.Format("Invalidate L1 cache called for account {0}, Extension {1}.", (object) targetHostId, (object) extensionId));
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      base.ServiceStart(requestContext);
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      this.cacheExtensionIdGrouping = VssMemoryCacheGroupingFactory.Create<ExtensionRightsCacheKey, ExtensionRightsResult, string>(requestContext, this.MemoryCache, (Func<ExtensionRightsCacheKey, ExtensionRightsResult, IEnumerable<string>>) ((key, val) => val?.EntitledExtensions != null ? (IEnumerable<string>) new string[1]
      {
        this.GetExtensionIdReverseMapKey(val.HostId)
      } : (IEnumerable<string>) new string[1]
      {
        string.Empty
      }), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext) => base.ServiceEnd(requestContext);

    private string GetExtensionIdReverseMapKey(Guid hostId) => string.Format("Ext.R.V3/{0}", (object) hostId);
  }
}
