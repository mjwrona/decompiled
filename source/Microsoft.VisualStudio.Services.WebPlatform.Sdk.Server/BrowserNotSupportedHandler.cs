// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.BrowserNotSupportedHandler
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class BrowserNotSupportedHandler : IErrorPageHandler
  {
    public void HandleError(IVssRequestContext requestContext, ErrorContext context)
    {
      if (context.Exception == null || !(context.Exception is BrowserNotSupportedException))
        return;
      context.ErrorDescription = ((BrowserNotSupportedException) context.Exception).DetailedMessage;
      context.EncodeErrorDescription = false;
      context.StatusCode = 400;
    }
  }
}
