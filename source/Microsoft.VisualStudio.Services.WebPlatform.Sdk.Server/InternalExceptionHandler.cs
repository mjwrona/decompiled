// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.InternalExceptionHandler
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Error;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class InternalExceptionHandler : IErrorPageHandler
  {
    public void HandleError(IVssRequestContext requestContext, ErrorContext context)
    {
      if (context.StatusCode != 500)
        return;
      context.ErrorTitle = WebFrameworkResources.InternalException_Title();
      context.ErrorDescription = WebFrameworkResources.GenericError_Desc();
      context.PrimaryAction = new ErrorAction();
      context.PrimaryAction.Href = LinkUtilities.ForwardLink(328778);
      context.PrimaryAction.Text = WebFrameworkResources.ContactSupport_ActionText();
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Framework.EnalbeSignoutForInternalException") || context.Exception == null || !(context.Exception is IdentityLoopingLoginException))
        return;
      context.SecondaryActions.Add(new ErrorAction()
      {
        Href = ErrorUtilities.ExtractSignoutURL(requestContext, context.RequestUri, context.StatusCode),
        Text = WebFrameworkResources.AuthorizationException_SecondaryActionText()
      });
    }
  }
}
