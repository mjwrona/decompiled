// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.LocationHelper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Routing
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
