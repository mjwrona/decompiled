// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.SequenceContextRedisCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class SequenceContextRedisCache
  {
    private static readonly Guid sequenceContextRedisNs = new Guid("38d9a52a-ffff-1111-9999-b47b630684b3");
    private const string sequenceContextRedisCacheKey = "SequenceContext";
    private const string redisCiAreaName = "SequenceContext";
    private const string c_area = "SequenceContextRedisCache";
    private const string c_sequenceIdsLayer = "SequenceIDs";

    public void Invalidate(IVssRequestContext targetContext) => targetContext.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<string, SequenceContext, SequenceContextRedisCache.SequenceContextCacheSecurityToken>(targetContext, SequenceContextRedisCache.sequenceContextRedisNs, new ContainerSettings()
    {
      CiAreaName = "SequenceContext"
    }).Invalidate(targetContext, (IEnumerable<string>) new string[1]
    {
      "SequenceContext"
    });

    public SequenceContext GetOrSet(
      IVssRequestContext targetContext,
      Func<SequenceContext> onCacheMiss)
    {
      IMutableDictionaryCacheContainer<string, SequenceContext> dictionaryContainer = targetContext.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<string, SequenceContext, SequenceContextRedisCache.SequenceContextCacheSecurityToken>(targetContext, SequenceContextRedisCache.sequenceContextRedisNs, new ContainerSettings()
      {
        CiAreaName = "SequenceContext"
      });
      IEnumerable<SequenceContext> source = dictionaryContainer.Get(targetContext, (IEnumerable<string>) new string[1]
      {
        "SequenceContext"
      }, (Func<IEnumerable<string>, IDictionary<string, SequenceContext>>) (_ =>
      {
        SequenceContext sequenceContext = onCacheMiss();
        return (IDictionary<string, SequenceContext>) new Dictionary<string, SequenceContext>()
        {
          ["SequenceContext"] = sequenceContext
        };
      }));
      try
      {
        return source.Single<SequenceContext>();
      }
      catch (InvalidOperationException ex)
      {
        targetContext.TraceException(1765003, TraceLevel.Warning, nameof (SequenceContextRedisCache), "SequenceIDs", (Exception) ex, "Cached multiple sequence context values under same key, which was not expected. Falling back to init and invalidating. Cached values: " + string.Join(";", source.Select<SequenceContext, string>((Func<SequenceContext, string>) (sc => sc.ToString()))));
        dictionaryContainer.Invalidate(targetContext, (IEnumerable<string>) new string[1]
        {
          "SequenceContext"
        });
        return SequenceContext.InitSequenceContext;
      }
    }

    internal class SequenceContextCacheSecurityToken
    {
    }
  }
}
