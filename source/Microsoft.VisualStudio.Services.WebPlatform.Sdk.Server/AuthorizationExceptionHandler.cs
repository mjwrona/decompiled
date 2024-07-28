// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.AuthorizationExceptionHandler
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Error;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class AuthorizationExceptionHandler : IErrorPageHandler
  {
    public void HandleError(IVssRequestContext requestContext, ErrorContext context)
    {
      if (context.Exception == null || context.StatusCode != 401 && context.StatusCode != 403)
        return;
      context.ImageContentId = "Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Error.Content.Images.Error_401.png";
      context.ErrorTitle = context.StatusCode != 401 ? WebFrameworkResources.ForbiddenException_Title() : WebFrameworkResources.AuthorizationException_Title();
      string inputHtml = (string) null;
      if (requestContext != null)
        inputHtml = ErrorUtilities.GetUserEmail(requestContext);
      if (!string.IsNullOrEmpty(inputHtml))
      {
        context.EncodeErrorDescription = false;
        context.ErrorDescription = WebFrameworkResources.UnauthorizedMessage((object) SafeHtmlWrapper.MakeSafeWithHtmlEncode(inputHtml));
      }
      else
        context.ErrorDescription = WebFrameworkResources.AuthorizationException_NoEmail();
      context.SecondaryActions.Add(new ErrorAction()
      {
        Href = ErrorUtilities.ExtractSignoutURL(requestContext, context.RequestUri, context.StatusCode),
        Text = WebFrameworkResources.AuthorizationException_SecondaryActionText()
      });
    }
  }
}
