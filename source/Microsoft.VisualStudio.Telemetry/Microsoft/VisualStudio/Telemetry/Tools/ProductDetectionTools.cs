// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Tools.ProductDetectionTools
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights;

namespace Microsoft.VisualStudio.Telemetry.Tools
{
  internal class ProductDetectionTools
  {
    internal static ProductTarget GetProduct(TelemetrySessionSettings settings) => ProductDetectionTools.GetProduct(settings.AsimovInstrumentationKey, settings.CollectorApiKey);

    private static ProductTarget GetProduct(string asimovIKey, string collectorApiKey) => KeysConstants.VSCodeCollectorApiKey.Equals(collectorApiKey) ? ProductTarget.VSCode : ProductTarget.Other;
  }
}
