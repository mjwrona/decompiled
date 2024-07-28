// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.ODataEndOfRequestTelemetryHandler
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class ODataEndOfRequestTelemetryHandler
  {
    private static string ProductTraceArea => "AnalyticsModel";

    private static string ProductTraceLayer => "ODataRequestCompletionTraceListener";

    public static void ProcessEvent(
      IVssRequestContext requestContext,
      ODataQueryFinishedEventArgs args)
    {
      ODataEndOfRequestTelemetryHandler.LogToCustomerIntelligence(requestContext, args);
      ODataEndOfRequestTelemetryHandler.LogToProductTrace(requestContext, args);
    }

    private static void LogToCustomerIntelligence(
      IVssRequestContext requestContext,
      ODataQueryFinishedEventArgs args)
    {
      Guid result1 = Guid.Empty;
      NameValueCollection queryString = args.RequestUri.ParseQueryString();
      if (queryString == null || queryString["ViewId"] == null || !Guid.TryParse(queryString["ViewId"], out result1))
        return;
      bool result2 = false;
      bool.TryParse(queryString["IsVerification"], out result2);
      bool result3 = false;
      bool.TryParse(queryString["IsDefaultView"], out result3);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("ViewId", (object) result1);
      properties.Add("IsVerification", result2);
      properties.Add("E2EID", (object) requestContext.E2EId);
      properties.Add("RelatedActivityID", (object) requestContext.UniqueIdentifier);
      properties.Add("Count", (double) args.ResponseCount);
      properties.Add("IsPowerBIPreview", args.RequestUri.PathAndQuery.Contains("top=1000"));
      properties.Add("IsDefaultView", result3);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "AnalyticsViews", "RanQuery", properties);
    }

    private static void LogToProductTrace(
      IVssRequestContext requestContext,
      ODataQueryFinishedEventArgs args)
    {
      if (args == null || !(args.RequestUri != (Uri) null))
        return;
      requestContext.TraceLongTextAlways(12013030, TraceLevel.Info, ODataEndOfRequestTelemetryHandler.ProductTraceArea, ODataEndOfRequestTelemetryHandler.ProductTraceLayer, JsonConvert.SerializeObject((object) args).ToString());
    }
  }
}
