// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.ODataDataQualityTelemetryHandler
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Analytics.OData
{
  public class ODataDataQualityTelemetryHandler
  {
    private static ISet<string> blacklist = (ISet<string>) new HashSet<string>();

    private static string ProductTraceArea => "AnalyticsDataQuality";

    private static string ProductTraceLayer => nameof (ODataDataQualityTelemetryHandler);

    public static void Handle(
      IVssRequestContext requestContext,
      IEnumerable<DataQualityResult> dataQualityResults)
    {
      if (!requestContext.IsFeatureEnabled("Analytics.OData.TraceDataQualityIssues"))
        return;
      foreach (DataQualityResult dataQualityResult in dataQualityResults)
      {
        if (!ODataDataQualityTelemetryHandler.blacklist.Contains(dataQualityResult.Name))
          requestContext.TraceAlways(12012016, TraceLevel.Info, ODataDataQualityTelemetryHandler.ProductTraceArea, ODataDataQualityTelemetryHandler.ProductTraceLayer, JsonConvert.SerializeObject((object) dataQualityResult).ToString());
      }
    }
  }
}
