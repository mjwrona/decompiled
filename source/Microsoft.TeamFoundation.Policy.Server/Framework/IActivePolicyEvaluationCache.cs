// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.Framework.IActivePolicyEvaluationCache
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Policy.Server.Framework
{
  [DefaultServiceImplementation(typeof (ActivePolicyEvaluationCache))]
  public interface IActivePolicyEvaluationCache : IDisposable
  {
    void BypassCache(IVssRequestContext requestContext);

    void Initialize(
      IVssRequestContext requestContext,
      string targetCacheKey,
      TimeSpan expirationTime,
      int minFileCountToUseCache);

    void Invalidate(IVssRequestContext requestContext, string targetCacheKey, [CallerMemberName] string caller = "");

    void Set(
      IVssRequestContext requestContext,
      int policyId,
      ActivePolicyEvaluationCacheItem value);

    bool TryGet(
      IVssRequestContext requestContext,
      int policyId,
      out ActivePolicyEvaluationCacheItem value);

    bool IsBypassed { get; }

    bool IsInitialized { get; }

    int MinFileCountToUseCache { get; }
  }
}
