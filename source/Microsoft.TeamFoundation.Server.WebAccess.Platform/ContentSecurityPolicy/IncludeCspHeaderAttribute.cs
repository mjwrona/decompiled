// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ContentSecurityPolicy.IncludeCspHeaderAttribute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.ContentSecurityPolicy
{
  public class IncludeCspHeaderAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuted(ActionExecutedContext context)
    {
      using (WebPerformanceTimer.StartMeasure(context.RequestContext, "Controller.Attributes.IncludeCspHeader"))
      {
        if (context.Controller is IPlatformController controller)
        {
          if (controller.TfsRequestContext != null)
          {
            IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
            if (WebContextFactory.GetWebContext(context.RequestContext).NavigationContext.Area != "Api")
            {
              if (!tfsRequestContext.IsFeatureEnabled(ContentSecurityPolicyFeatureFlags.ContentSecurityPolicyFeatureFlag))
              {
                if (!tfsRequestContext.IsFeatureEnabled(ContentSecurityPolicyFeatureFlags.ContentSecurityPolicyReportOnlyFeatureFlag))
                  goto label_8;
              }
              tfsRequestContext.GetService<ContentSecurityPolicyManagementService>().AddContentSecurityPolicyHeader(tfsRequestContext, context.RequestContext.HttpContext);
            }
          }
        }
      }
label_8:
      base.OnActionExecuted(context);
    }
  }
}
