// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.DedicatedGatewayRequestOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos
{
  public class DedicatedGatewayRequestOptions
  {
    public TimeSpan? MaxIntegratedCacheStaleness { get; set; }

    internal static void PopulateMaxIntegratedCacheStalenessOption(
      DedicatedGatewayRequestOptions dedicatedGatewayRequestOptions,
      RequestMessage request)
    {
      if (dedicatedGatewayRequestOptions == null || !dedicatedGatewayRequestOptions.MaxIntegratedCacheStaleness.HasValue)
        return;
      double totalMilliseconds = dedicatedGatewayRequestOptions.MaxIntegratedCacheStaleness.Value.TotalMilliseconds;
      if (totalMilliseconds < 0.0)
        throw new ArgumentOutOfRangeException("MaxIntegratedCacheStaleness", "MaxIntegratedCacheStaleness cannot be negative.");
      request.Headers.Set("x-ms-dedicatedgateway-max-age", totalMilliseconds.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }
  }
}
