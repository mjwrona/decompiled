// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens.AadOnBehalfOfCache
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens
{
  public class AadOnBehalfOfCache : VssCacheBase
  {
    private const string CacheValue = "OBO";
    private const int MaximumCacheSize = 100000;
    private VssCacheExpiryProvider<IdentityDescriptor, string> expiryProvider;
    private readonly VssMemoryCacheList<IdentityDescriptor, string> cache;

    public AadOnBehalfOfCache()
    {
      this.expiryProvider = new VssCacheExpiryProvider<IdentityDescriptor, string>(Capture.Create<TimeSpan>(TimeSpan.FromMinutes(60.0)), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry));
      this.cache = new VssMemoryCacheList<IdentityDescriptor, string>((IVssCachePerformanceProvider) this, 100000, this.expiryProvider);
    }

    public bool HasValue(IdentityDescriptor cacheKey) => this.cache.TryGetValue(cacheKey, out string _);

    public void Set(IdentityDescriptor identityDescriptor) => this.cache.Add(identityDescriptor, "OBO", true);

    public void Sweep() => this.cache.Sweep();
  }
}
