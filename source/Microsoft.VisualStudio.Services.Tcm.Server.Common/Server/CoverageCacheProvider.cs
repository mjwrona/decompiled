// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageCacheProvider
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageCacheProvider : ICoverageCacheProvider
  {
    private const string CacheNameSpaceId = "990952E4-242D-48DE-A4C5-568A6DB3080C";

    public string GetRepoProperty(
      IVssRequestContext requestContext,
      string cacheContainerName,
      string propertyName)
    {
      int expiryTimeInHours = new CoverageConfiguration().GetCacheExpiryTimeInHours(requestContext);
      ContainerSettings settings = new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(TimeSpan.FromHours((double) expiryTimeInHours)),
        CiAreaName = nameof (CoverageCacheProvider),
        NoThrowMode = new bool?(true)
      };
      string repoProperty = (string) null;
      IRedisCacheService service = requestContext.GetService<IRedisCacheService>();
      Dictionary<string, string> dictionary;
      if (service.IsEnabled(requestContext) && service.GetVolatileDictionaryContainer<string, Dictionary<string, string>, object>(requestContext, new Guid("990952E4-242D-48DE-A4C5-568A6DB3080C"), settings).TryGet<string, Dictionary<string, string>>(requestContext, cacheContainerName, out dictionary) && dictionary.ContainsKey(propertyName))
        repoProperty = dictionary[propertyName];
      return repoProperty;
    }

    public void SetRepoProperty(
      IVssRequestContext requestContext,
      string cacheContainerName,
      string propertyName,
      string propertyValue)
    {
      int expiryTimeInHours = new CoverageConfiguration().GetCacheExpiryTimeInHours(requestContext);
      ContainerSettings settings = new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(TimeSpan.FromHours((double) expiryTimeInHours)),
        CiAreaName = nameof (CoverageCacheProvider)
      };
      IRedisCacheService service = requestContext.GetService<IRedisCacheService>();
      if (!service.IsEnabled(requestContext))
        return;
      IMutableDictionaryCacheContainer<string, Dictionary<string, string>> dictionaryContainer = service.GetVolatileDictionaryContainer<string, Dictionary<string, string>, object>(requestContext, new Guid("990952E4-242D-48DE-A4C5-568A6DB3080C"), settings);
      Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>()
      {
        {
          cacheContainerName,
          new Dictionary<string, string>()
        }
      };
      dictionary[cacheContainerName].Add(propertyName, propertyValue);
      IVssRequestContext requestContext1 = requestContext;
      Dictionary<string, Dictionary<string, string>> items = dictionary;
      dictionaryContainer.Set(requestContext1, (IDictionary<string, Dictionary<string, string>>) items);
    }
  }
}
