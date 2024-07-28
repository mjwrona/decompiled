// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ServiceUnavailableHandler
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class ServiceUnavailableHandler : IErrorPageHandler
  {
    public void HandleError(IVssRequestContext requestContext, ErrorContext context)
    {
      if (context.StatusCode != 503)
        return;
      context.ErrorTitle = WebFrameworkResources.ErrorName_ServiceUnavailableTitle();
      context.ErrorDescription = WebFrameworkResources.ErrorName_ServiceUnavailableMessage((object) (!requestContext.IsMicrosoftTenant() ? 242573 : 2213875));
      context.EncodeErrorDescription = false;
      context.PrimaryAction = new ErrorAction();
      context.PrimaryAction.Href = LinkUtilities.ForwardLink(!requestContext.IsMicrosoftTenant() ? 242573 : 2213875);
      context.PrimaryAction.Text = WebFrameworkResources.ViewStatus_ActionText();
    }
  }
}
