// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.RoutingExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public static class RoutingExtensions
  {
    public static MvcHtmlString GenerateActionUrls(this HtmlHelper htmlHelper)
    {
      WebContext webContext = (WebContext) htmlHelper.ViewContext.TfsWebContext();
      IContributionRoutingService service = webContext.TfsRequestContext.GetService<IContributionRoutingService>();
      UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
      JsObject jsObject = new JsObject();
      jsObject["PersonalAccessToken"] = (object) new RoutingExtensions.ControllerInfo()
      {
        RouteId = "ms.vss-tfs-web.user-settings-tokens-route",
        Actions = new List<string>()
        {
          "Edit",
          "List",
          "Revoke",
          "Regenerate",
          "Index",
          "RevokeAll"
        }
      };
      jsObject["PublicKey"] = (object) new RoutingExtensions.ControllerInfo()
      {
        RouteId = "ms.vss-tfs-web.user-settings-keys-route",
        Actions = new List<string>()
        {
          "Edit",
          "List",
          "Revoke",
          "Index",
          "GetServerFingerprint"
        }
      };
      jsObject["OAuthAuthorizations"] = (object) new RoutingExtensions.ControllerInfo()
      {
        RouteId = "ms.vss-tfs-web.user-settings-oauth-route",
        Actions = new List<string>()
        {
          "Index",
          "List",
          "Revoke",
          "RevokeAll"
        }
      };
      List<string> values1 = new List<string>();
      foreach (string key in jsObject.Keys)
      {
        StringBuilder stringBuilder = new StringBuilder();
        RoutingExtensions.ControllerInfo controllerInfo = jsObject[key] as RoutingExtensions.ControllerInfo;
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {{", (object) key));
        List<string> values2 = new List<string>();
        foreach (string action in controllerInfo.Actions)
        {
          string str = service.RouteUrl(webContext.TfsRequestContext, controllerInfo.RouteId, new RouteValueDictionary((object) new
          {
            action = action
          }));
          if (string.IsNullOrEmpty(str))
            str = urlHelper.Action(action, key, (RouteValueDictionary) null);
          values2.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: '{1}'", (object) action, (object) str));
        }
        stringBuilder.AppendLine(string.Join("," + Environment.NewLine, (IEnumerable<string>) values2));
        stringBuilder.AppendLine("}");
        values1.Add(stringBuilder.ToString());
      }
      return MvcHtmlString.Create(string.Join(",", (IEnumerable<string>) values1));
    }

    private class ControllerInfo
    {
      public string RouteId { get; set; }

      public List<string> Actions { get; set; }
    }
  }
}
