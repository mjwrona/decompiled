// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.ProxyUtilities
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Server
{
  public class ProxyUtilities
  {
    public static string GetServerUrl(string absoluteUrl)
    {
      string serverUrl = !string.IsNullOrEmpty(absoluteUrl) ? absoluteUrl.Trim() : throw new ArgumentNullException(nameof (absoluteUrl));
      string lowerInvariant = serverUrl.ToLowerInvariant();
      int length = lowerInvariant.IndexOf("/Services/", StringComparison.OrdinalIgnoreCase);
      if (length != -1)
        serverUrl = lowerInvariant.Substring(0, length);
      if (serverUrl.EndsWith("/", StringComparison.Ordinal))
        serverUrl = serverUrl.Remove(serverUrl.Length - 1);
      return serverUrl;
    }
  }
}
