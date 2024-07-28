// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenRevocation.TokenRevocationRuleCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.TokenRevocation
{
  public class TokenRevocationRuleCache : VssCacheBase
  {
    private readonly VssMemoryCacheList<Guid, TokenRevocationRule[]> cache;
    private readonly VssCacheExpiryProvider<Guid, TokenRevocationRule[]> ruleExpiryProvider;
    private Capture<TimeSpan> expiryInterval;
    private Capture<TimeSpan> inactivityInterval;

    public TokenRevocationRuleCache(TimeSpan expirationInterval)
    {
      this.expiryInterval = Capture.Create<TimeSpan>(expirationInterval);
      this.inactivityInterval = Capture.Create<TimeSpan>(expirationInterval);
      this.ruleExpiryProvider = new VssCacheExpiryProvider<Guid, TokenRevocationRule[]>(this.expiryInterval, this.inactivityInterval);
      this.cache = new VssMemoryCacheList<Guid, TokenRevocationRule[]>((IVssCachePerformanceProvider) this, this.ruleExpiryProvider);
    }

    public void Set(IVssRequestContext requestContext, TokenRevocationRule[] rules) => this.cache[requestContext.ServiceHost.InstanceId] = rules;

    public void Clear() => this.cache.Clear();

    public TokenRevocationRule[] GetAll(IVssRequestContext requestContext)
    {
      TokenRevocationRule[] all = (TokenRevocationRule[]) null;
      this.cache.TryGetValue(requestContext.ServiceHost.InstanceId, out all);
      return all;
    }
  }
}
