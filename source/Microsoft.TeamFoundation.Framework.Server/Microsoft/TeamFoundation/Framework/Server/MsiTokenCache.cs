// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MsiTokenCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class MsiTokenCache
  {
    private readonly VssMemoryCacheList<string, MsiToken> m_cache;
    private readonly TimeSpan m_expirationWindowSize;
    private static MsiTokenCache s_instance;
    private const string c_cacheName = "MsiTokenCache";

    public MsiTokenCache(int size = 10, TimeSpan? expirationWindowSize = null)
    {
      this.m_cache = new VssMemoryCacheList<string, MsiToken>((IVssCachePerformanceProvider) new VssCachePerformanceProvider(nameof (MsiTokenCache)), size);
      this.m_expirationWindowSize = expirationWindowSize.HasValue ? expirationWindowSize.Value : TimeSpan.Zero;
    }

    public MsiToken GetToken(string resource)
    {
      MsiToken token;
      this.m_cache.TryGetValue(resource, out token);
      return token;
    }

    public void SetToken(string resource, MsiToken token)
    {
      Capture<TimeSpan> expiryInterval = new Capture<TimeSpan>(token.ExpiresOn.Subtract(this.m_expirationWindowSize) - DateTime.UtcNow);
      Capture<TimeSpan> inactivityInterval = new Capture<TimeSpan>(VssCacheExpiryProvider.NoExpiry);
      this.m_cache.Add(resource, token, true, new VssCacheExpiryProvider<string, MsiToken>(expiryInterval, inactivityInterval));
    }

    public void RemoveToken(string resource) => this.m_cache.Remove(resource);

    public void Clear() => this.m_cache.Clear();

    public static MsiTokenCache SharedCache
    {
      get
      {
        if (MsiTokenCache.s_instance == null)
          MsiTokenCache.s_instance = new MsiTokenCache(expirationWindowSize: new TimeSpan?(TimeSpan.FromMinutes(5.0)));
        return MsiTokenCache.s_instance;
      }
    }
  }
}
