// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ReparentingHandler
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class ReparentingHandler : IErrorPageHandler
  {
    public void HandleError(IVssRequestContext requestContext, ErrorContext context)
    {
      if (context == null || string.IsNullOrEmpty(context.Message) || !context.Message.Contains("VS403201"))
        return;
      context.ErrorTitle = WebFrameworkResources.ReparentingTitle();
      context.ErrorDescription = WebFrameworkResources.ReparentingMessage();
      context.PrimaryAction = (ErrorAction) null;
    }
  }
}
