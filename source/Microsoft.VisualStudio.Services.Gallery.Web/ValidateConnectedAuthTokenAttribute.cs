// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.ValidateConnectedAuthTokenAttribute
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.VisualStudio.Services.CloudConnected;
using Microsoft.VisualStudio.Services.Common;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public class ValidateConnectedAuthTokenAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);
      if (!(filterContext.Controller is WebPlatformAreaController controller) || !controller.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      string token = this.ReadAuthenticationToken(filterContext);
      if (string.IsNullOrEmpty(token))
        this.FailAuthValidation();
      if (controller.TfsRequestContext.GetService<IConnectedServerContextKeyService>().IsValidAuthToken(controller.TfsRequestContext, token))
        return;
      this.FailAuthValidation();
    }

    private void FailAuthValidation() => throw new HttpException(401, GalleryResources.AuthTokenValidationError);

    protected virtual string ReadAuthenticationToken(ActionExecutingContext filterContext)
    {
      ArgumentUtility.CheckForNull<ActionExecutingContext>(filterContext, nameof (filterContext));
      return filterContext.HttpContext.Request.Form.Get(CloudConnectedServerConstants.AuthenticationToken);
    }
  }
}
