// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.XssProtectionManagementService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Web;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public class XssProtectionManagementService : IVssFrameworkService
  {
    private const string c_XssProtectionResponseHeader = "X-XSS-Protection";
    private const string c_area = "XssProtection";
    private const string c_layer = "Service";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(15320100, "XssProtection", "Service", nameof (ServiceStart));
      systemRequestContext.TraceLeave(15320101, "XssProtection", "Service", nameof (ServiceStart));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(15320102, "XssProtection", "Service", nameof (ServiceEnd));
      systemRequestContext.TraceLeave(15320103, "XssProtection", "Service", nameof (ServiceEnd));
    }

    public void AddXssProtectionHeader(IVssRequestContext requestContext, HttpContextBase context)
    {
      requestContext.TraceEnter(15320110, "XssProtection", "Service", nameof (AddXssProtectionHeader));
      try
      {
        requestContext.To(TeamFoundationHostType.Deployment);
        if (!requestContext.IsFeatureEnabled(XssProtectionFeatureFlags.XssProtectionFeatureFlag))
          return;
        string name = "X-XSS-Protection";
        string str = "1; mode=block";
        requestContext.Trace(15320120, TraceLevel.Info, "XssProtection", "Service", string.Format("Adding XSS Protection header: {0}", (object) str));
        context.Response.AddHeader(name, str);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15320125, "XssProtection", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(15320130, "XssProtection", "Service", nameof (AddXssProtectionHeader));
      }
    }
  }
}
