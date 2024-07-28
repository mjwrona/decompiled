// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.HttpBearerChallengeCache
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault
{
  public sealed class HttpBearerChallengeCache
  {
    private static HttpBearerChallengeCache _instance = new HttpBearerChallengeCache();
    private Dictionary<string, HttpBearerChallenge> _cache;
    private object _cacheLock;

    public static HttpBearerChallengeCache GetInstance() => HttpBearerChallengeCache._instance;

    private HttpBearerChallengeCache()
    {
      this._cache = new Dictionary<string, HttpBearerChallenge>();
      this._cacheLock = new object();
    }

    public HttpBearerChallenge GetChallengeForURL(Uri url)
    {
      if (url == (Uri) null)
        throw new ArgumentNullException(nameof (url));
      HttpBearerChallenge challengeForUrl = (HttpBearerChallenge) null;
      lock (this._cacheLock)
        this._cache.TryGetValue(url.FullAuthority(), out challengeForUrl);
      return challengeForUrl;
    }

    public void RemoveChallengeForURL(Uri url)
    {
      if (url == (Uri) null)
        throw new ArgumentNullException(nameof (url));
      lock (this._cacheLock)
        this._cache.Remove(url.FullAuthority());
    }

    public void SetChallengeForURL(Uri url, HttpBearerChallenge value)
    {
      if (url == (Uri) null)
        throw new ArgumentNullException(nameof (url));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (string.Compare(url.FullAuthority(), value.SourceAuthority, StringComparison.OrdinalIgnoreCase) != 0)
        throw new ArgumentException("Source URL and Challenge URL do not match");
      lock (this._cacheLock)
        this._cache[url.FullAuthority()] = value;
    }

    public void Clear()
    {
      lock (this._cacheLock)
        this._cache.Clear();
    }
  }
}
