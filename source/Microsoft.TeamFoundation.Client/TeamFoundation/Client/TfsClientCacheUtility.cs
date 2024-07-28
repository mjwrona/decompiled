// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsClientCacheUtility
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.TeamFoundation.Client
{
  public static class TfsClientCacheUtility
  {
    public static string GetCacheDirectory(Uri uri, Guid instanceId, Guid userId = default (Guid)) => TfsClientCacheUtility.GetDirectory(TfsConnection.ClientCacheDirectory, uri, instanceId, userId);

    public static string GetVolatileCacheDirectory(Uri uri, Guid instanceId) => TfsClientCacheUtility.GetDirectory(TfsConnection.ClientVolatileCacheDirectory, uri, instanceId);

    internal static void DeleteVolatileCacheDirectory(Uri uri, Guid instanceId)
    {
      string volatileCacheDirectory = TfsClientCacheUtility.GetVolatileCacheDirectory(uri, instanceId);
      if (!Directory.Exists(volatileCacheDirectory))
        return;
      try
      {
        FileSpec.DeleteDirectory(volatileCacheDirectory, true);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Deleting directory {0} failed.", (object) volatileCacheDirectory), nameof (DeleteVolatileCacheDirectory), ex);
        throw new TeamFoundationServerException(ClientResources.DeleteVolatileCacheDirectoryError((object) volatileCacheDirectory, (object) ex.Message), ex);
      }
    }

    private static string GetDirectory(
      string cacheDirectory,
      Uri uri,
      Guid instanceId,
      Guid userId = default (Guid))
    {
      string str = uri == (Uri) null ? "_Unknown" : "_" + uri.Scheme;
      return userId == Guid.Empty ? Path.Combine(cacheDirectory, instanceId.ToString() + str) : Path.Combine(cacheDirectory, userId.ToString(), instanceId.ToString() + str);
    }
  }
}
