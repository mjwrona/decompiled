// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheUtils
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal static class ImsCacheUtils
  {
    internal static void ValidateTypes(IEnumerable<Type> types)
    {
      using (IEnumerator<Type> enumerator = types.Where<Type>((Func<Type, bool>) (type => !typeof (ImsCacheObject).IsAssignableFrom(type))).GetEnumerator())
      {
        if (enumerator.MoveNext())
          throw new ArgumentException("Type must be assignable to ImsCacheObject: " + enumerator.Current?.ToString());
      }
    }

    internal static IList<T> ToNullFilteredList<T>(this IEnumerable<T> source) => (IList<T>) source.Where<T>((Func<T, bool>) (x => (object) x != null)).ToList<T>();

    internal static TElement TryGetOrDefault<TKey, TElement>(
      this IDictionary<TKey, TElement> map,
      TKey key)
    {
      TElement element;
      return !map.TryGetValue(key, out element) ? default (TElement) : element;
    }

    internal static IdentityType ExtractIdentityType(IReadOnlyVssIdentity identity)
    {
      IdentityType identityType1 = IdentityType.None;
      if (identity == null)
        return identityType1;
      if (identity.Descriptor.IsAadServicePrincipalType())
        return IdentityType.ServicePrincipal;
      if (identity.IsExternalUser)
        return IdentityType.User | IdentityType.External;
      if (AadIdentityHelper.IsAadGroup(identity.Descriptor))
        return IdentityType.Group | IdentityType.External;
      IdentityType identityType2 = identityType1 | IdentityType.Internal;
      return !identity.IsContainer ? identityType2 | IdentityType.User : identityType2 | IdentityType.Group;
    }

    internal static IdentityId ExtractIdentityId(IReadOnlyVssIdentity identity) => identity == null ? (IdentityId) null : new IdentityId(identity.Id, ImsCacheUtils.ExtractIdentityType(identity), identity.Descriptor);

    internal static bool CacheBypassRequested(IVssRequestContext context) => context.IsSystemContext && context.Items != null && context.Items.GetCastedValueOrDefault<string, bool>("ImsCacheConstants.Token.BypassCache");

    internal static void Trace(
      IVssRequestContext requestContext,
      int tracePoint,
      TraceLevel traceLevel,
      string area,
      string layer,
      int maxSameRequestTraces,
      Func<string> traceMessage)
    {
      if (!requestContext.IsTracing(tracePoint, traceLevel, area, layer))
        return;
      Dictionary<int, int> castedValueOrDefault = requestContext.Items.GetCastedValueOrDefault<string, Dictionary<int, int>>("Microsoft.VisualStudio.Services.Identity.Cache.TracePointHits");
      int num;
      if (castedValueOrDefault != null && castedValueOrDefault.TryGetValue(tracePoint, out num) && num >= maxSameRequestTraces)
        return;
      requestContext.Trace(tracePoint, traceLevel, area, layer, traceMessage());
      if (castedValueOrDefault == null)
        requestContext.Items["Microsoft.VisualStudio.Services.Identity.Cache.TracePointHits"] = (object) new Dictionary<int, int>()
        {
          {
            tracePoint,
            1
          }
        };
      else if (!castedValueOrDefault.ContainsKey(tracePoint))
        castedValueOrDefault[tracePoint] = 1;
      else
        castedValueOrDefault[tracePoint]++;
    }
  }
}
