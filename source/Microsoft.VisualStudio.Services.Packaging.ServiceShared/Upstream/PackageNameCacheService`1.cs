// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.PackageNameCacheService`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public class PackageNameCacheService<TName> : IPackageNameCacheService<TName> where TName : IPackageName
  {
    private readonly ICache<string, object> requestContextItemsAsCache;
    private readonly IUpstreamPackageNamesClient upstreamClient;
    private readonly UpstreamSource upstreamSource;

    public PackageNameCacheService(
      ICache<string, object> requestContextItemsAsCache,
      IUpstreamPackageNamesClient upstreamClient,
      UpstreamSource upstreamSource)
    {
      this.requestContextItemsAsCache = requestContextItemsAsCache;
      this.upstreamClient = upstreamClient;
      this.upstreamSource = upstreamSource;
    }

    public async Task<bool> Has(TName packageName) => (await this.GetAsDictionary()).ContainsKey(packageName.NormalizedName);

    private async Task<Dictionary<string, object>> GetAsDictionary()
    {
      string key = "Packaging." + this.upstreamSource.Protocol + ".PackageNameCache." + this.upstreamSource.Name;
      object val1;
      Dictionary<string, object> val2;
      if (this.requestContextItemsAsCache.TryGet(key, out val1))
      {
        val2 = (Dictionary<string, object>) val1;
      }
      else
      {
        val2 = (await this.upstreamClient.GetPackageNames()).ToDictionary<RawPackageNameEntry, string, object>((Func<RawPackageNameEntry, string>) (e => e.Name), (Func<RawPackageNameEntry, object>) (e => (object) null));
        this.requestContextItemsAsCache.Set(key, (object) val2);
      }
      Dictionary<string, object> asDictionary = val2;
      key = (string) null;
      return asDictionary;
    }

    public UpstreamSource GetSource() => this.upstreamSource;
  }
}
