// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.SupportedContextLevels
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class SupportedContextLevels : ActionFilterAttribute
  {
    public TeamFoundationHostType ContextLevels { get; private set; }

    public SupportedContextLevels(TeamFoundationHostType contextLevels) => this.ContextLevels = contextLevels;

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      if (!this.IsContextLevelSupported((filterContext.Controller as IPlatformController).TfsRequestContext))
        throw new HttpException(404, "Not Supported");
      base.OnActionExecuting(filterContext);
    }

    private bool IsContextLevelSupported(IVssRequestContext requestContext) => (requestContext.ServiceHost.HostType & this.ContextLevels) != 0;
  }
}
