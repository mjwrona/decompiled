// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.NotFoundExceptionHandler
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Error;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class NotFoundExceptionHandler : IErrorPageHandler
  {
    public void HandleError(IVssRequestContext requestContext, ErrorContext context)
    {
      if (context.StatusCode != 404)
        return;
      context.ImageContentId = "Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Error.Content.Images.Error_404.png";
      context.ErrorTitle = WebFrameworkResources.NotFoundException_Title();
      context.ErrorDescription = WebFrameworkResources.NotFoundException_Message();
      context.PrimaryAction = new ErrorAction();
      context.PrimaryAction.Href = ErrorUtilities.GetRootUrl(requestContext);
      context.PrimaryAction.Text = WebFrameworkResources.GoBackHome_ActionText();
    }
  }
}
