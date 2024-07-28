// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ContentSecurityPolicy.CspReportLoggingService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.ContentSecurityPolicy
{
  public class CspReportLoggingService : IVssFrameworkService
  {
    private const string c_area = "ContentSecurityPolicy";
    private const string c_layer = "LoggingService";
    private const string c_ciArea = "WebPlatform";
    private const string c_ciFeature = "ContentSecurityPolicy";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void LogCspReport(IVssRequestContext requestContext, string cspReportAsString)
    {
      requestContext.Trace(15140330, TraceLevel.Info, "ContentSecurityPolicy", "LoggingService", cspReportAsString);
      if (!requestContext.IsFeatureEnabled(ContentSecurityPolicyFeatureFlags.ContentSecurityPolicyCspReportEndpointLoggingFeatureFlag) || string.IsNullOrWhiteSpace(cspReportAsString) || !cspReportAsString.Contains("csp-report") || !cspReportAsString.Contains("document-uri"))
        return;
      if (!cspReportAsString.Contains("violated-directive"))
        return;
      try
      {
        CspPost cspPost = JsonConvert.DeserializeObject<CspPost>(cspReportAsString);
        if (cspPost == null || cspPost.CspReport == null)
          return;
        CspReport cspReport = cspPost.CspReport;
        CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add("cspReport", (object) cspReport);
        intelligenceData.Add("DocumentUri", cspReport.DocumentUri);
        intelligenceData.Add("Referrer", cspReport.Referrer);
        intelligenceData.Add("EffectiveDirective", cspReport.EffectiveDirective);
        intelligenceData.Add("ViolatedDirective", cspReport.ViolatedDirective);
        intelligenceData.Add("OriginalPolicy", cspReport.OriginalPolicy);
        intelligenceData.Add("BlockedUri", cspReport.BlockedUri);
        intelligenceData.Add("SourceFile", cspReport.SourceFile);
        intelligenceData.Add("LineNumber", (double) cspReport.LineNumber);
        intelligenceData.Add("ColumnNumber", (double) cspReport.ColumnNumber);
        intelligenceData.Add("StatusCode", cspReport.StatusCode);
        IVssRequestContext requestContext1 = requestContext;
        CustomerIntelligenceData properties = intelligenceData;
        service.Publish(requestContext1, "WebPlatform", "ContentSecurityPolicy", properties);
      }
      catch (Exception ex)
      {
      }
    }
  }
}
