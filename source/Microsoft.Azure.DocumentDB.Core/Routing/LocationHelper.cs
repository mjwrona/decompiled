// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.LocationHelper
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Routing
{
  internal static class LocationHelper
  {
    internal static Uri GetLocationEndpoint(Uri serviceEndpoint, string location)
    {
      UriBuilder uriBuilder = new UriBuilder(serviceEndpoint);
      string[] strArray = uriBuilder.Host.Split(new char[1]
      {
        '.'
      }, 2);
      if (strArray.Length != 0)
      {
        strArray[0] = strArray[0] + "-" + location.DataCenterToUriPostfix();
        uriBuilder.Host = string.Join(".", strArray);
      }
      return uriBuilder.Uri;
    }

    private static string DataCenterToUriPostfix(this string datacenter) => datacenter.Replace(" ", string.Empty);
  }
}
