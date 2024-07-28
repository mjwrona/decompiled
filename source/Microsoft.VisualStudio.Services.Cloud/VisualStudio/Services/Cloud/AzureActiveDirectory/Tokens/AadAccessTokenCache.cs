// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens.AadAccessTokenCache
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens
{
  public class AadAccessTokenCache : VssCacheBase
  {
    private readonly AadAccessTokenCache.Range[] ranges = new AadAccessTokenCache.Range[6]
    {
      new AadAccessTokenCache.Range()
      {
        Start = 0,
        End = 70,
        ExpiryProvider = new VssCacheExpiryProvider<string, JwtSecurityToken>(Capture.Create<TimeSpan>(TimeSpan.FromMinutes(70.0)), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry))
      },
      new AadAccessTokenCache.Range()
      {
        Start = 71,
        End = 140,
        ExpiryProvider = new VssCacheExpiryProvider<string, JwtSecurityToken>(Capture.Create<TimeSpan>(TimeSpan.FromMinutes(140.0)), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry))
      },
      new AadAccessTokenCache.Range()
      {
        Start = 141,
        End = 210,
        ExpiryProvider = new VssCacheExpiryProvider<string, JwtSecurityToken>(Capture.Create<TimeSpan>(TimeSpan.FromMinutes(210.0)), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry))
      },
      new AadAccessTokenCache.Range()
      {
        Start = 211,
        End = 280,
        ExpiryProvider = new VssCacheExpiryProvider<string, JwtSecurityToken>(Capture.Create<TimeSpan>(TimeSpan.FromMinutes(280.0)), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry))
      },
      new AadAccessTokenCache.Range()
      {
        Start = 281,
        End = 350,
        ExpiryProvider = new VssCacheExpiryProvider<string, JwtSecurityToken>(Capture.Create<TimeSpan>(TimeSpan.FromMinutes(350.0)), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry))
      },
      new AadAccessTokenCache.Range()
      {
        Start = 351,
        End = int.MaxValue,
        ExpiryProvider = new VssCacheExpiryProvider<string, JwtSecurityToken>(Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry), Capture.Create<TimeSpan>(TimeSpan.FromHours(8.0)))
      }
    };
    private readonly VssMemoryCacheList<string, JwtSecurityToken> cache;
    private static AadAccessTokenCache.RangeComparer rangeComparer = new AadAccessTokenCache.RangeComparer();

    public AadAccessTokenCache() => this.cache = new VssMemoryCacheList<string, JwtSecurityToken>((IVssCachePerformanceProvider) this);

    public bool TryGetValue(string cacheKey, out JwtSecurityToken accessToken) => this.cache.TryGetValue(cacheKey, out accessToken);

    public void Set(string cacheKey, JwtSecurityToken accessToken)
    {
      VssCacheExpiryProvider<string, JwtSecurityToken> expiryProvider = this.LookupExpiryProvider((int) (accessToken.ValidTo.ToUniversalTime() - DateTime.UtcNow).TotalMinutes);
      if (expiryProvider == null)
        return;
      this.cache.Add(cacheKey, accessToken, true, expiryProvider);
    }

    public void Sweep() => this.cache.Sweep();

    internal VssCacheExpiryProvider<string, JwtSecurityToken> LookupExpiryProvider(
      int valueInMinutes)
    {
      int index = Array.BinarySearch((Array) this.ranges, (object) valueInMinutes, (IComparer) AadAccessTokenCache.rangeComparer);
      return index < 0 ? (VssCacheExpiryProvider<string, JwtSecurityToken>) null : this.ranges[index].ExpiryProvider;
    }

    internal struct Range
    {
      public int Start;
      public int End;
      public VssCacheExpiryProvider<string, JwtSecurityToken> ExpiryProvider;
    }

    internal class RangeComparer : IComparer
    {
      public int Compare(object range, object value)
      {
        if (!(range is AadAccessTokenCache.Range) || !(value is int num))
          throw new ArgumentException("Parameters are not in correct type");
        return this.Compare((AadAccessTokenCache.Range) range, num);
      }

      private int Compare(AadAccessTokenCache.Range range, int value)
      {
        if (range.Start > value)
          return 1;
        return range.End < value ? -1 : 0;
      }
    }
  }
}
