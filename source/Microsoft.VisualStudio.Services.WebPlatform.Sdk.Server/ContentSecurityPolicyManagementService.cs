// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ContentSecurityPolicyManagementService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Web;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public class ContentSecurityPolicyManagementService : IVssFrameworkService
  {
    private const string c_cspResponseHeaderReportOnly = "Content-Security-Policy-Report-Only";
    private const string c_cspResponseHeader = "Content-Security-Policy";
    private const string c_area = "ContentSecurityPolicy";
    private const string c_layer = "Service";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(15140100, "ContentSecurityPolicy", "Service", nameof (ServiceStart));
      systemRequestContext.TraceLeave(15140101, "ContentSecurityPolicy", "Service", nameof (ServiceStart));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(15140102, "ContentSecurityPolicy", "Service", nameof (ServiceEnd));
      systemRequestContext.TraceLeave(15140103, "ContentSecurityPolicy", "Service", nameof (ServiceEnd));
    }

    public void AddContentSecurityPolicyHeader(
      IVssRequestContext requestContext,
      HttpContextBase context)
    {
      requestContext.TraceEnter(15140110, "ContentSecurityPolicy", "Service", nameof (AddContentSecurityPolicyHeader));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IContentSecurityPolicyHeaderManagementService service = vssRequestContext.GetService<IContentSecurityPolicyHeaderManagementService>();
        if (requestContext.IsFeatureEnabled(ContentSecurityPolicyFeatureFlags.ContentSecurityPolicyReportOnlyFeatureFlag))
        {
          string name = "Content-Security-Policy-Report-Only";
          string reportOnlyHeaderValue = service.GetReportOnlyHeaderValue(vssRequestContext, context, true);
          requestContext.Trace(15140115, TraceLevel.Info, "ContentSecurityPolicy", "Service", string.Format("Adding CSP report-only header: {0}", (object) reportOnlyHeaderValue));
          context.Response.AddHeader(name, reportOnlyHeaderValue);
        }
        if (!requestContext.IsFeatureEnabled(ContentSecurityPolicyFeatureFlags.ContentSecurityPolicyFeatureFlag))
          return;
        string name1 = "Content-Security-Policy";
        string headerValue = service.GetHeaderValue(vssRequestContext, context);
        requestContext.Trace(15140120, TraceLevel.Info, "ContentSecurityPolicy", "Service", string.Format("Adding CSP header: {0}", (object) headerValue));
        context.Response.AddHeader(name1, headerValue);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15140125, "ContentSecurityPolicy", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(15140130, "ContentSecurityPolicy", "Service", nameof (AddContentSecurityPolicyHeader));
      }
    }
  }
}
