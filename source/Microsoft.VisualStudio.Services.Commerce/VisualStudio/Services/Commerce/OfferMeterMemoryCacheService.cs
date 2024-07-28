// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferMeterMemoryCacheService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class OfferMeterMemoryCacheService : 
    VssMemoryCacheService<string, OfferMeterCacheContainer>,
    IOfferMeterMemoryCacheService,
    IVssFrameworkService,
    ICommerceMemoryCache<OfferMeterCacheContainer>
  {
    private static readonly TimeSpan defaultCleanupInterval = TimeSpan.FromHours(6.0);
    private static readonly TimeSpan defaultMaxCacheInactivityAge = TimeSpan.FromHours(5.0);

    public OfferMeterMemoryCacheService()
      : base(OfferMeterMemoryCacheService.defaultCleanupInterval)
    {
      this.InactivityInterval.Value = OfferMeterMemoryCacheService.defaultMaxCacheInactivityAge;
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      base.ServiceStart(requestContext);
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext) => base.ServiceEnd(requestContext);

    public bool IsEnabled(IVssRequestContext requestContext, string featureFlag) => requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled(featureFlag);
  }
}
