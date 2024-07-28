// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.AzureDownloadUrlCacheService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Web;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class AzureDownloadUrlCacheService : 
    VssMemoryCacheService<BlobStorageId, Uri>,
    IDownloadUrlCacheService,
    IVssFrameworkService
  {
    private const int MaxElementsDefault = 50000;
    private const int CleanupIntervalMinutesDefault = 15;
    private static readonly Guid UrlInvalidationNotificationGuid = new Guid("6587DACA-B23C-4C66-BE46-AD9A1DC90DFA");
    private static readonly TimeSpan InactivityIntervalDefault = VssCacheExpiryProvider.NoExpiry;
    private readonly ITimeProvider timeProvider;
    private readonly Random random = new Random();

    public AzureDownloadUrlCacheService()
      : this((ITimeProvider) new DefaultTimeProvider(), (IEqualityComparer<BlobStorageId>) EqualityComparer<BlobStorageId>.Default, AzureDownloadUrlCacheService.GetCacheConfiguration())
    {
    }

    protected AzureDownloadUrlCacheService(
      ITimeProvider timeProvider,
      IEqualityComparer<BlobStorageId> comparer,
      MemoryCacheConfiguration<BlobStorageId, Uri> configuration)
      : base(comparer, configuration)
    {
      this.timeProvider = timeProvider;
    }

    public void SetDownloadUrl(IVssRequestContext requestContext, BlobStorageId key, Uri url)
    {
      using (requestContext.GetTracerFacade().Enter((object) this, nameof (SetDownloadUrl)))
      {
        if (!this.IsEnabled(requestContext) || this.MemoryCache.TryGetValueKeepTtl<BlobStorageId, Uri>(key, out Uri _))
          return;
        DateTime? expiryTimeFromUrl = this.ComputeExpiryTimeFromUrl(url);
        if (!expiryTimeFromUrl.HasValue)
          return;
        TimeSpan timeSpanUntilExpiry = expiryTimeFromUrl.Value.ToUniversalTime() - this.timeProvider.Now.ToUniversalTime();
        if (timeSpanUntilExpiry < TimeSpan.Zero)
          return;
        this.AddUrl(key, url, timeSpanUntilExpiry);
      }
    }

    public void InvalidateDownloadUri(IVssRequestContext requestContext, BlobStorageId key)
    {
      using (requestContext.GetTracerFacade().Enter((object) this, nameof (InvalidateDownloadUri)))
      {
        if (!this.IsEnabled(requestContext))
          return;
        this.Remove(requestContext, key);
        requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, AzureDownloadUrlCacheService.UrlInvalidationNotificationGuid, key.ValueString);
      }
    }

    public bool TryGetDownloadUrl(
      IVssRequestContext requestContext,
      BlobStorageId key,
      out Uri url)
    {
      using (requestContext.GetTracerFacade().Enter((object) this, nameof (TryGetDownloadUrl)))
      {
        if (this.IsEnabled(requestContext))
          return this.TryGetValue(requestContext, key, out url);
        url = (Uri) null;
        return false;
      }
    }

    protected virtual void AddUrl(BlobStorageId key, Uri url, TimeSpan timeSpanUntilExpiry) => this.MemoryCache.Add(key, url, false, new VssCacheExpiryProvider<BlobStorageId, Uri>(Capture.Create<TimeSpan>(timeSpanUntilExpiry), Capture.Create<TimeSpan>(AzureDownloadUrlCacheService.InactivityIntervalDefault)));

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      base.ServiceStart(requestContext);
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", AzureDownloadUrlCacheService.UrlInvalidationNotificationGuid, new SqlNotificationCallback(this.OnCachedUrlNoLongerValid), true);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", AzureDownloadUrlCacheService.UrlInvalidationNotificationGuid, new SqlNotificationCallback(this.OnCachedUrlNoLongerValid), true);
      base.ServiceEnd(requestContext);
    }

    private static MemoryCacheConfiguration<BlobStorageId, Uri> GetCacheConfiguration() => MemoryCacheConfiguration<BlobStorageId, Uri>.Default.WithCleanupInterval(TimeSpan.FromMinutes(15.0)).WithMaxElements(50000);

    private bool IsEnabled(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment;

    private void OnCachedUrlNoLongerValid(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      BlobStorageId key = (BlobStorageId) StorageId.Parse(eventData);
      this.Remove(requestContext, key);
    }

    private DateTime? ComputeExpiryTimeFromUrl(Uri url)
    {
      try
      {
        string str = HttpUtility.ParseQueryString(url.Query).Get("se");
        return str.IsNullOrEmpty<char>() ? new DateTime?() : new DateTime?(DateTime.Parse(str) - TimeSpan.FromMinutes((double) (15 + this.random.Next(1, 5))));
      }
      catch (Exception ex)
      {
        return new DateTime?();
      }
    }
  }
}
