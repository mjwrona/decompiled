// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle.TokenSigningKeyNamespaceCache
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle
{
  public class TokenSigningKeyNamespaceCache : VssCacheBase
  {
    private readonly VssMemoryCacheList<string, TokenSigningKeyNamespace> cache;
    private readonly VssCacheExpiryProvider<string, TokenSigningKeyNamespace> namespaceExpiryProvider;
    private Capture<TimeSpan> expiryInterval;
    private Capture<TimeSpan> inactivityInterval;

    public TokenSigningKeyNamespaceCache(TimeSpan expirationInterval)
    {
      this.expiryInterval = Capture.Create<TimeSpan>(expirationInterval);
      this.inactivityInterval = Capture.Create<TimeSpan>(expirationInterval);
      this.namespaceExpiryProvider = new VssCacheExpiryProvider<string, TokenSigningKeyNamespace>(this.expiryInterval, this.inactivityInterval);
      this.cache = new VssMemoryCacheList<string, TokenSigningKeyNamespace>((IVssCachePerformanceProvider) this, this.namespaceExpiryProvider);
    }

    public bool TryGetValue(string namespaceName, out TokenSigningKeyNamespace namespaceObject) => this.cache.TryGetValue(namespaceName, out namespaceObject);

    public void Set(string namespaceName, TokenSigningKeyNamespace namespaceObject) => this.cache[namespaceName] = namespaceObject;

    public void Clear() => this.cache.Clear();
  }
}
