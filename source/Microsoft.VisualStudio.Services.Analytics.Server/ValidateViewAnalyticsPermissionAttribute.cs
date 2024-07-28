// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.ValidateViewAnalyticsPermissionAttribute
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class ValidateViewAnalyticsPermissionAttribute : ResolveTfsProjectFilterAttribute
  {
    public override void OnAuthorization(HttpActionContext actionContext)
    {
      base.OnAuthorization(actionContext);
      IVssRequestContext requestContext1 = this.GetRequestContext(actionContext);
      IAnalyticsSecurityService service = requestContext1.GetService<IAnalyticsSecurityService>();
      ProjectInfo projectInfo = actionContext.ControllerContext.Controller is ITfsProjectApiController controller ? controller.ProjectInfo : (ProjectInfo) null;
      IVssRequestContext requestContext2 = requestContext1;
      Guid projectId = projectInfo != null ? projectInfo.Id : Guid.Empty;
      if (!service.HasReadPermission(requestContext2, projectId))
        throw new AnalyticsAccessCheckException(projectInfo != null ? AnalyticsResources.NO_VIEW_ANALYTICS_PERMISSION((object) projectInfo.Name) : AnalyticsResources.NO_VIEW_ANALYTICS_PERMISSION_COLLECTION());
    }
  }
}
