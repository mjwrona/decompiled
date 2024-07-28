// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.ValidateAnalyticsEnabledAndModelReadyAttributeAttribute
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class ValidateAnalyticsEnabledAndModelReadyAttributeAttribute : 
    ResolveTfsProjectFilterAttribute
  {
    public override void OnAuthorization(HttpActionContext actionContext)
    {
      base.OnAuthorization(actionContext);
      if ((actionContext.ControllerContext.Controller is ITfsProjectApiController controller ? controller.ProjectInfo : (ProjectInfo) null) == null)
        return;
      IVssRequestContext requestContext = this.GetRequestContext(actionContext);
      requestContext.ValidateAnalyticsEnabled();
      requestContext.GetService<AnalyticsService>().CheckModelReady(requestContext);
    }
  }
}
