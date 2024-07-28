// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.SecurityHeadersAttribute
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public class SecurityHeadersAttribute : ActionFilterAttribute
  {
    private bool _csp;
    private bool _xss;
    private bool _permissionsPolicy;

    public SecurityHeadersAttribute(bool csp = false, bool xss = false, bool permissionsPolicy = false)
    {
      this._csp = csp;
      this._xss = xss;
      this._permissionsPolicy = permissionsPolicy;
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
      using (WebPerformanceTimer.StartMeasure(context.RequestContext, "Controller.Attributes.IncludeSecurityHeaders"))
      {
        if (context.Controller is IPlatformController controller)
        {
          if (controller.TfsRequestContext != null)
          {
            IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
            if (this._csp && (tfsRequestContext.IsFeatureEnabled(ContentSecurityPolicyFeatureFlags.ContentSecurityPolicyFeatureFlag) || tfsRequestContext.IsFeatureEnabled(ContentSecurityPolicyFeatureFlags.ContentSecurityPolicyReportOnlyFeatureFlag)))
              tfsRequestContext.GetService<ContentSecurityPolicyManagementService>().AddContentSecurityPolicyHeader(tfsRequestContext, context.RequestContext.HttpContext);
            if (this._xss && tfsRequestContext.IsFeatureEnabled(XssProtectionFeatureFlags.XssProtectionFeatureFlag))
              tfsRequestContext.GetService<XssProtectionManagementService>().AddXssProtectionHeader(tfsRequestContext, context.RequestContext.HttpContext);
            if (this._permissionsPolicy)
            {
              if (tfsRequestContext.IsFeatureEnabled(PermissionsPolicyFeatureFlags.PermissionsPolicyHeaderFeatureFlag))
                tfsRequestContext.GetService<PermissionsPolicyManagementService>().AddPermisionsPolicyHeader(tfsRequestContext, context.RequestContext.HttpContext);
            }
          }
        }
      }
      base.OnActionExecuted(context);
    }
  }
}
