// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Common.BitbucketAccessTokenCacheService
// Assembly: Microsoft.VisualStudio.ExternalProviders.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E34B318-B0E9-49BD-88C0-4A425E8D0753
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.ExternalProviders.Common
{
  internal sealed class BitbucketAccessTokenCacheService : 
    IBitbucketAccessTokenCacheService,
    IVssFrameworkService
  {
    private BitbucketAccessTokenCacheService.CacheImpl impl;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.impl = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<BitbucketAccessTokenCacheService.CacheImpl>();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool TryGetValue(string refreshToken, out BitbucketData.Authorization authorization)
    {
      ArgumentUtility.CheckForNull<string>(refreshToken, nameof (refreshToken));
      return this.impl.MemoryCache.TryGetValue(refreshToken, out authorization);
    }

    public bool AddValue(string refreshToken, BitbucketData.Authorization authorization)
    {
      ArgumentUtility.CheckForNull<string>(refreshToken, nameof (refreshToken));
      ArgumentUtility.CheckForNull<BitbucketData.Authorization>(authorization, nameof (authorization));
      return this.impl.MemoryCache.Add(refreshToken, authorization, true);
    }

    private sealed class CacheImpl : VssMemoryCacheService<string, BitbucketData.Authorization>
    {
      public CacheImpl()
        : base(TimeSpan.FromHours(2.0))
      {
        this.MaxCacheLength.Value = 10000;
        this.ExpiryInterval.Value = TimeSpan.FromHours(2.0) - TimeSpan.FromMinutes(1.0);
      }
    }
  }
}
