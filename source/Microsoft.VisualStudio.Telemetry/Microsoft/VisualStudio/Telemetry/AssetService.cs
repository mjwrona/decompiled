// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.AssetService
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  public sealed class AssetService
  {
    private object locker = new object();
    private ConcurrentDictionary<string, IAssetProvider> registeredProviders = new ConcurrentDictionary<string, IAssetProvider>();
    private ConcurrentDictionary<AssetService.CacheKey, TelemetryEventCorrelation> registeredCorrelations = new ConcurrentDictionary<AssetService.CacheKey, TelemetryEventCorrelation>();
    private static readonly Lazy<AssetService> lazyAssetService = new Lazy<AssetService>((Func<AssetService>) (() => new AssetService((IAssetServiceThreadScheduler) new AssetService.BackgroundThreadScheduler())));

    private IAssetServiceThreadScheduler ThreadScheduler { get; }

    internal AssetService(IAssetServiceThreadScheduler scheduler) => this.ThreadScheduler = scheduler;

    public static AssetService Instance => AssetService.lazyAssetService.Value;

    public void RegisterCorrelation(
      string assetTypeName,
      Guid assetId,
      TelemetryEventCorrelation correlation)
    {
      assetTypeName.RequiresArgumentNotNullAndNotWhiteSpace(assetTypeName);
      assetId.RequiresArgumentNotEmpty(nameof (assetId));
      this.RegisterCorrelation(assetTypeName, assetId.ToString("D"), correlation);
    }

    public void RegisterCorrelation(
      string assetTypeName,
      string assetId,
      TelemetryEventCorrelation correlation)
    {
      assetTypeName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (assetTypeName));
      assetId.RequiresArgumentNotNullAndNotWhiteSpace(nameof (assetId));
      this.registeredCorrelations[new AssetService.CacheKey(assetTypeName, assetId)] = correlation;
    }

    public void UnregisterCorrelation(string assetTypeName, Guid assetId)
    {
      assetTypeName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (assetTypeName));
      assetId.RequiresArgumentNotEmpty(nameof (assetId));
      this.UnregisterCorrelation(assetTypeName, assetId.ToString("D"));
    }

    public void UnregisterCorrelation(string assetTypeName, string assetId)
    {
      assetTypeName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (assetTypeName));
      assetId.RequiresArgumentNotNullAndNotWhiteSpace(nameof (assetId));
      this.registeredCorrelations.TryRemove(new AssetService.CacheKey(assetTypeName, assetId), out TelemetryEventCorrelation _);
    }

    public void RegisterProvider(string assetTypeName, IAssetProvider assetProvider)
    {
      assetTypeName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (assetTypeName));
      assetProvider.RequiresArgumentNotNull<IAssetProvider>(nameof (assetProvider));
      this.registeredProviders[assetTypeName] = assetProvider;
    }

    public void UnregisterProvider(string assetTypeName)
    {
      assetTypeName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (assetTypeName));
      this.registeredProviders.TryRemove(assetTypeName, out IAssetProvider _);
    }

    public TelemetryEventCorrelation GetCorrelation(string assetTypeName, Guid assetId)
    {
      assetTypeName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (assetTypeName));
      assetId.RequiresArgumentNotEmpty(nameof (assetId));
      return this.GetCorrelation(assetTypeName, assetId.ToString("D"));
    }

    public TelemetryEventCorrelation GetCorrelation(string assetTypeName, string assetId)
    {
      assetTypeName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (assetTypeName));
      assetId.RequiresArgumentNotNullAndNotWhiteSpace(nameof (assetId));
      IAssetProvider provider = (IAssetProvider) null;
      AssetService.CacheKey key = new AssetService.CacheKey(assetTypeName, assetId);
      TelemetryEventCorrelation correlation = TelemetryEventCorrelation.Empty;
      lock (this.locker)
      {
        if (!this.registeredCorrelations.TryGetValue(key, out correlation))
        {
          correlation = TelemetryEventCorrelation.Empty;
          if (this.registeredProviders.TryGetValue(assetTypeName, out provider))
          {
            correlation = new TelemetryEventCorrelation(Guid.NewGuid(), DataModelEventType.Asset);
            this.RegisterCorrelation(assetTypeName, assetId, correlation);
          }
        }
      }
      if (provider != null)
        this.ThreadScheduler.Schedule((Action) (() =>
        {
          bool flag = false;
          try
          {
            flag = provider.PostAsset(assetId, correlation);
          }
          finally
          {
            if (!flag)
              this.UnregisterCorrelation(assetTypeName, assetId);
          }
        }));
      return correlation;
    }

    private class BackgroundThreadScheduler : IAssetServiceThreadScheduler
    {
      public void Schedule(Action action) => ThreadPool.QueueUserWorkItem((WaitCallback) (state => action()));
    }

    private struct CacheKey
    {
      private string assetTypeName;
      private string assetId;

      public CacheKey(string assetTypeName, string assetId)
      {
        this.assetTypeName = assetTypeName;
        this.assetId = assetId;
      }
    }
  }
}
