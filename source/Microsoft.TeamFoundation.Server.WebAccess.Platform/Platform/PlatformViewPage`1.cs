// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Platform.PlatformViewPage`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Net;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Platform
{
  public class PlatformViewPage<TModel> : ViewPage<TModel>
  {
    public WebContext WebContext => this.ViewContext.WebContext();

    protected virtual void CheckBrowserVersion()
    {
      BrowserFilterService service = this.WebContext.TfsRequestContext.To(TeamFoundationHostType.Application).GetService<BrowserFilterService>();
      switch (service.GetSupportLevel(this.ViewContext.HttpContext.Request))
      {
        case BrowserSupportLevel.Blocked:
          throw new BrowserNotSupportedException(HttpStatusCode.BadRequest)
          {
            DetailedMessage = service.GetBlockedMessage()
          };
        case BrowserSupportLevel.UnSupported:
          this.Html.AddGlobalMessage(service.GetUnsupportedMessage(), WebMessageLevel.Warning, true);
          break;
      }
    }

    public override void InitHelpers()
    {
      this.WebContext.TfsRequestContext.TraceEnter(0, "WebAccess", TfsTraceLayers.Framework, "PlatformViewPage.InitHelpers");
      try
      {
        base.InitHelpers();
        this.CheckBrowserVersion();
      }
      finally
      {
        this.WebContext.TfsRequestContext.TraceLeave(0, "WebAccess", TfsTraceLayers.Framework, "PlatformViewPage.InitHelpers");
      }
    }
  }
}
