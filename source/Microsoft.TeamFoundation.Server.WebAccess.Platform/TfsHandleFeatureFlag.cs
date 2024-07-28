// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsHandleFeatureFlag
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class TfsHandleFeatureFlag : ActionFilterAttribute
  {
    private string flagName = string.Empty;
    private string excludedFlag;

    public TfsHandleFeatureFlag(string featureflag, string excludedFlag = null)
    {
      this.flagName = featureflag;
      this.excludedFlag = excludedFlag;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      IPlatformController controller = filterContext.Controller as IPlatformController;
      bool flag = controller.TfsRequestContext.IsFeatureEnabled(this.flagName);
      if (flag && !string.IsNullOrEmpty(this.excludedFlag))
        flag = !controller.TfsRequestContext.IsFeatureEnabled(this.excludedFlag);
      if (!flag)
        throw new HttpException(404, PlatformResources.PageNotFound);
    }
  }
}
