// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.QuotaCacheService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.AzComm.SharedContracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi.Contracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi.HttpClients;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class QuotaCacheService : IQuotaCacheServiceV2, IQuotaCacheServiceBase, IVssFrameworkService
  {
    private readonly string CacheInvalidationDurationInSeconds = "/Configuration/BlobStore/QuotaCapping/QuotaCacheInvalidationInSeconds";
    private const int DefaultQuotaCacheInvalidationInSeconds = 600;
    private const int TracePoint = 5701994;
    private const string ProductTraceArea = "QuotaCacheService";
    private const string ProductTraceLayer = "Service";
    protected TraceData ProductTraceData = new TraceData()
    {
      Area = nameof (QuotaCacheService),
      Layer = "Service"
    };

    private DateTimeOffset LastCacheAccessTime { get; set; }

    public TimeSpan CacheValidityExpiration { get; set; }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(systemRequestContext, this.ProductTraceData, 5701994, nameof (ServiceStart)))
      {
        tracer.TraceAlways(string.Format("[{0}]:Starting for Host: {1}.", (object) nameof (QuotaCacheService), (object) systemRequestContext.ServiceHost.InstanceId));
        this.CacheValidityExpiration = TimeSpan.FromSeconds((double) systemRequestContext.GetService<IVssRegistryService>().GetValue<int>(systemRequestContext, (RegistryQuery) this.CacheInvalidationDurationInSeconds, true, 600));
        this.DetermineHostType(systemRequestContext);
        tracer.TraceAlways("[QuotaService]: " + string.Format("Details for Host: {0} ", (object) systemRequestContext.ServiceHost.InstanceId) + string.Format("{0}: {1} ", (object) "CacheValidityExpiration", (object) this.CacheValidityExpiration) + string.Format("{0}: {1}.", (object) "HostDesignation", (object) this.HostDesignation));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.HostDesignation = BlobStoreUtils.HostAccountType.Undefined;
      this.CachedMeterInfo = (MeterUsage2GetResponse) null;
      this.LastCacheAccessTime = new DateTimeOffset();
    }

    public MeterUsage2GetResponse PerformCacheLookupV2(IVssRequestContext requestContext)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, this.ProductTraceData, 5701994, nameof (PerformCacheLookupV2)))
      {
        try
        {
          if (this.CachedMeterInfo == null)
            this.CachedMeterInfo = QuotaCacheService.FetchMeterInfo(requestContext);
          else if (this.IsCacheEntryExpired(requestContext))
            this.CachedMeterInfo = QuotaCacheService.FetchMeterInfo(requestContext);
          this.LastCacheAccessTime = DateTimeOffset.UtcNow;
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          throw;
        }
      }
      return this.CachedMeterInfo;
    }

    public bool IsCacheEntryExpired(IVssRequestContext requestContext) => this.CachedMeterInfo != null && DateTimeOffset.UtcNow.Subtract(this.LastCacheAccessTime) >= this.CacheValidityExpiration;

    public MeterUsage2GetResponse CachedMeterInfo { get; private set; }

    public BlobStoreUtils.HostAccountType HostDesignation { get; private set; }

    private static MeterUsage2GetResponse FetchMeterInfo(IVssRequestContext requestContext)
    {
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      return requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetClient<MeterUsage2HttpClient>().GetMeterUsageAsync(instanceId, AzCommMeterIds.ArtifactsMeterId).SyncResult<MeterUsage2GetResponse>();
    }

    private void DetermineHostType(IVssRequestContext requestContext)
    {
      try
      {
        this.HostDesignation = BlobStoreUtils.IsInternalHost(requestContext) ? BlobStoreUtils.HostAccountType.Internal : BlobStoreUtils.HostAccountType.External;
      }
      catch (Exception ex)
      {
      }
    }
  }
}
