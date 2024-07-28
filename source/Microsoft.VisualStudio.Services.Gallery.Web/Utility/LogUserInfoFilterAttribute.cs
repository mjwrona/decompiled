// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Utility.LogUserInfoFilterAttribute
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Utility
{
  public class LogUserInfoFilterAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      if (filterContext.Controller is IPlatformController controller && this.ShouldSendTelemetry(filterContext))
      {
        IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
        HttpBrowserCapabilitiesBase browser = filterContext.HttpContext.Request.Browser;
        ClientTraceData properties = new ClientTraceData();
        properties.Add(MarketPlaceIntelligencePropertyName.BrowserName, (object) browser.Browser);
        properties.Add(MarketPlaceIntelligencePropertyName.BrowserVersion, (object) browser.Version);
        properties.Add(MarketPlaceIntelligencePropertyName.BrowserPlatform, (object) browser.Platform);
        HttpCookie httpCookie = filterContext.HttpContext.Request.Cookies.Get(Cookies.GalleryUserIdentity);
        if (httpCookie != null)
          properties.Add(MarketPlaceIntelligencePropertyName.UserID, (object) httpCookie.Value);
        if (filterContext.ActionDescriptor != null)
          properties.Add(MarketPlaceIntelligencePropertyName.UserAction, (object) filterContext.ActionDescriptor.ActionName);
        tfsRequestContext.GetService<ClientTraceService>().Publish(tfsRequestContext, MarketPlaceCustomerIntelligenceArea.MarketPlace, MarketPlaceIntelligenceFeature.AllFeatures, properties);
      }
      base.OnActionExecuted(filterContext);
    }

    private bool ShouldSendTelemetry(ActionExecutedContext filterContext) => filterContext.HttpContext.Request.UserAgent == null || !filterContext.HttpContext.Request.UserAgent.ToUpperInvariant().Contains("GomezAgent".ToUpperInvariant());
  }
}
