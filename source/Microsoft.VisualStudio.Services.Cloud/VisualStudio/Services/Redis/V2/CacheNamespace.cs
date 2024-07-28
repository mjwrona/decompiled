// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V2.CacheNamespace
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.VisualStudio.Services.Redis.V2
{
  internal class CacheNamespace
  {
    private const string c_containerPrefix = "/Containers/";
    private const int c_hashCacheSizeThreshold = 65535;
    private readonly ConcurrentDictionary<Type, string> m_hashCache = new ConcurrentDictionary<Type, string>();
    private readonly HashAlgorithm m_hashAlgorithm = (HashAlgorithm) SHA1.Create();

    public string MakeCacheId<T>(IVssRequestContext requestContext, Guid namespaceId)
    {
      string format = "{0}{1}/{2}/{3}/{4}";
      if (RedisConfiguration.IsClusterEnabled(requestContext))
        format = "{0}{1}/{{{2}/{3}}}/{4}";
      return string.Format(format, (object) "/Containers/", (object) requestContext.ServiceInstanceType().ToString("N"), (object) namespaceId.ToString("N"), (object) requestContext.ServiceHost.InstanceId.ToString("N"), (object) this.GetSentinel(typeof (T)));
    }

    public static bool TryParseCacheId(string cacheId, out Guid namespaceId)
    {
      if (cacheId.StartsWith("/Containers/"))
      {
        int num1 = cacheId.IndexOf('/', "/Containers/".Length);
        if (num1 != -1)
        {
          int num2 = cacheId.IndexOf('/', num1 + 1);
          if (num2 != -1)
          {
            if (Guid.TryParseExact(cacheId.Substring(num1 + 1, num2 - num1 - 1).TrimStart('{'), "N", out namespaceId))
              return true;
          }
        }
      }
      namespaceId = new Guid();
      return false;
    }

    private string GetSentinel(Type securityToken)
    {
      string sentinel1;
      if (this.m_hashCache.TryGetValue(securityToken, out sentinel1))
        return sentinel1;
      string sentinel2 = this.CreateSentinel(securityToken);
      if (this.m_hashCache.TryAdd(securityToken, sentinel2) && this.m_hashCache.Count > (int) ushort.MaxValue)
      {
        this.m_hashCache.Clear();
        this.m_hashCache.TryAdd(securityToken, sentinel2);
      }
      return sentinel2;
    }

    private string CreateSentinel(Type type)
    {
      if (type == typeof (object))
        return string.Empty;
      if (type.IsVisible)
        throw new ArgumentException(string.Format("'{0}' is a public type and cannot be used as a cache security token (use System.Object for sharing cache)", (object) type.FullName));
      byte[] bytes = Encoding.UTF8.GetBytes(type.AssemblyQualifiedName);
      byte[] hash;
      lock (this.m_hashAlgorithm)
        hash = this.m_hashAlgorithm.ComputeHash(bytes);
      return HexConverter.ToString(hash);
    }
  }
}
