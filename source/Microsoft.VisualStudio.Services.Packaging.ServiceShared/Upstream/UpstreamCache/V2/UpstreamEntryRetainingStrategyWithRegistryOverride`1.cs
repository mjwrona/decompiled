// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamCache.V2.UpstreamEntryRetainingStrategyWithRegistryOverride`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamCache.V2
{
  public class UpstreamEntryRetainingStrategyWithRegistryOverride<TEntry> : 
    IAsyncHandler<MetadataDocument<TEntry>, List<TEntry>>,
    IHaveInputType<MetadataDocument<TEntry>>,
    IHaveOutputType<List<TEntry>>
    where TEntry : class, IMetadataEntry
  {
    private readonly IRegistryService registryService;
    private readonly string doNotRetainRegistryOverrideLookup = "/Configuration/Packaging/{0}/Upstreams/CacheEntryRetentionPolicy";
    private readonly string defaultRetainRegistryValue = "AllowAll";

    public UpstreamEntryRetainingStrategyWithRegistryOverride(IRegistryService registryService) => this.registryService = registryService;

    public Task<List<TEntry>> Handle(MetadataDocument<TEntry> request)
    {
      if (request?.Properties == null)
        return Task.FromResult<List<TEntry>>(new List<TEntry>());
      List<TEntry> list = request.Entries.Where<TEntry>((Func<TEntry, bool>) (e => e.IsFromUpstream)).ToList<TEntry>();
      if (!list.Any<TEntry>())
        return Task.FromResult<List<TEntry>>(new List<TEntry>());
      return this.registryService.GetValue<string>(new RegistryQuery(string.Format(this.doNotRetainRegistryOverrideLookup, (object) list.First<TEntry>().PackageIdentity.Name.Protocol.CorrectlyCasedName)), this.defaultRetainRegistryValue) != this.defaultRetainRegistryValue ? Task.FromResult<List<TEntry>>(new List<TEntry>()) : Task.FromResult<List<TEntry>>(list);
    }
  }
}
