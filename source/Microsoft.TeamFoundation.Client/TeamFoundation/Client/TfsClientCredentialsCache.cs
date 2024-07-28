// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsClientCredentialsCache
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Client
{
  internal static class TfsClientCredentialsCache
  {
    private static Dictionary<string, VssCredentials> s_cachedProvider = new Dictionary<string, VssCredentials>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static object s_cacheProviderLock = new object();

    public static VssCredentials GetCredentials(Uri uri) => TfsClientCredentialsCache.GetCredentials((string) null, uri);

    public static VssCredentials GetCredentials(string featureRegistryKeyword, Uri uri)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      TeamFoundationTrace.Info("TfsCredentialCache.GetCredentials(): name - {0}", (object) uri);
      string uniqueAccountKey = TFUtil.GetUniqueAccountKey(uri);
      VssCredentials credentials = (VssCredentials) null;
      lock (TfsClientCredentialsCache.s_cacheProviderLock)
      {
        if (!TfsClientCredentialsCache.s_cachedProvider.TryGetValue(uniqueAccountKey, out credentials))
        {
          credentials = VssClientCredentials.LoadCachedCredentials(featureRegistryKeyword, uri, false);
          TfsClientCredentialsCache.s_cachedProvider.Add(uniqueAccountKey, credentials);
        }
      }
      return credentials;
    }

    public static bool SetCredentials(Uri serverUrl, VssCredentials credentials)
    {
      ArgumentUtility.CheckForNull<Uri>(serverUrl, nameof (serverUrl));
      ArgumentUtility.CheckForNull<VssCredentials>(credentials, nameof (credentials));
      TeamFoundationTrace.Info("TfsCredentialCache.SetCredentials(): name - {0}", (object) serverUrl);
      bool flag = false;
      string uniqueAccountKey = TFUtil.GetUniqueAccountKey(serverUrl);
      lock (TfsClientCredentialsCache.s_cacheProviderLock)
      {
        VssCredentials vssCredentials;
        if (TfsClientCredentialsCache.s_cachedProvider.TryGetValue(uniqueAccountKey, out vssCredentials))
        {
          if (credentials == vssCredentials)
            goto label_7;
        }
        TfsClientCredentialsCache.s_cachedProvider[uniqueAccountKey] = credentials;
        flag = vssCredentials != null;
      }
label_7:
      return flag;
    }

    public static bool RemoveCredentials(Uri serverUrl, VssCredentials credentials)
    {
      ArgumentUtility.CheckForNull<Uri>(serverUrl, nameof (serverUrl));
      ArgumentUtility.CheckForNull<VssCredentials>(credentials, nameof (credentials));
      TeamFoundationTrace.Info("TfsCredentialCache.RemoveCredentials(): name - {0}", (object) serverUrl);
      bool flag = false;
      string uniqueAccountKey = TFUtil.GetUniqueAccountKey(serverUrl);
      lock (TfsClientCredentialsCache.s_cacheProviderLock)
      {
        VssCredentials vssCredentials;
        if (TfsClientCredentialsCache.s_cachedProvider.TryGetValue(uniqueAccountKey, out vssCredentials))
        {
          if (credentials == vssCredentials)
          {
            TfsClientCredentialsCache.s_cachedProvider.Remove(uniqueAccountKey);
            flag = true;
          }
        }
      }
      return flag;
    }
  }
}
