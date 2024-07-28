// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Machine.ServiceProxies.ClientCertificateChoiceCache
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.TeamFoundation.Build.Machine.ServiceProxies
{
  internal class ClientCertificateChoiceCache
  {
    private static Dictionary<Uri, X509Certificate2> m_cache = new Dictionary<Uri, X509Certificate2>();

    private ClientCertificateChoiceCache()
    {
    }

    public static bool TryGet(Uri uri, out X509Certificate2 cachedCertificate)
    {
      if (ClientCertificateChoiceCache.m_cache.TryGetValue(uri, out cachedCertificate))
        return true;
      lock (ClientCertificateChoiceCache.m_cache)
        return ClientCertificateChoiceCache.m_cache.TryGetValue(uri, out cachedCertificate);
    }

    public static void Invalidate(Uri uri)
    {
      lock (ClientCertificateChoiceCache.m_cache)
      {
        if (!ClientCertificateChoiceCache.m_cache.ContainsKey(uri))
          return;
        ClientCertificateChoiceCache.m_cache.Remove(uri);
      }
    }

    public static void Set(Uri uri, X509Certificate2 certificate)
    {
      lock (ClientCertificateChoiceCache.m_cache)
      {
        if (ClientCertificateChoiceCache.m_cache.ContainsKey(uri))
          ClientCertificateChoiceCache.m_cache[uri] = certificate;
        else
          ClientCertificateChoiceCache.m_cache.Add(uri, certificate);
      }
    }
  }
}
