// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Platform.PageHtmlExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Bundling;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Platform
{
  public static class PageHtmlExtensions
  {
    private const string c_pageContextVar = "__vssPageContext";

    public static MvcHtmlString PageInit(this HtmlHelper htmlHelper)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "Html.PageInit"))
      {
        StringBuilder stringBuilder = new StringBuilder();
        Microsoft.TeamFoundation.Server.WebAccess.PageContext pageContext = WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext);
        stringBuilder.AppendLine(htmlHelper.GlobalizationConfig().ToString());
        stringBuilder.AppendLine(htmlHelper.CoreScripts().ToString());
        if (!htmlHelper.ViewContext.HttpContext.UseNewPlatformHost() || !pageContext.Diagnostics.BundlingEnabled)
          stringBuilder.AppendLine(htmlHelper.ModuleLoaderConfig().ToString());
        return new MvcHtmlString(htmlHelper.PageContext().ToString() + stringBuilder.ToString());
      }
    }

    public static MvcHtmlString PageContext(this HtmlHelper htmlHelper)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "Html.PageContext"))
      {
        Microsoft.TeamFoundation.Server.WebAccess.PageContext pageContext = WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext);
        return htmlHelper.JavaScript("var " + "__vssPageContext" + " = " + RestApiJsonResult.SerializeRestApiData(pageContext.WebContext.TfsRequestContext, (object) pageContext) + ";");
      }
    }

    public static MvcHtmlString GlobalizationConfig(this HtmlHelper htmlHelper)
    {
      Microsoft.TeamFoundation.Server.WebAccess.PageContext pageContext = WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext);
      StringBuilder sbScriptBuilder = new StringBuilder();
      sbScriptBuilder.AppendFormat("var __cultureInfo = {0}.microsoftAjaxConfig.cultureInfo;", (object) "__vssPageContext");
      pageContext.MicrosoftAjaxConfig.ClientCultureInfo.AppendAdjustmentScript(sbScriptBuilder);
      return htmlHelper.JavaScript(sbScriptBuilder.ToString());
    }

    public static MvcHtmlString CoreScripts(this HtmlHelper htmlHelper)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "Html.CoreScripts"))
      {
        StringBuilder stringBuilder = new StringBuilder();
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        Microsoft.TeamFoundation.Server.WebAccess.PageContext pageContext = WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext);
        if (pageContext.CoreReferences.ThirdPartyScriptsBundle != null)
        {
          if (htmlHelper.ViewContext.HttpContext.UseNewPlatformHost())
            stringBuilder.AppendLine(BundlingHelper.GetScriptBundlesNewPlatformHtml(htmlHelper, (IEnumerable<BundledScriptFile>) new BundledScriptFile[1]
            {
              pageContext.CoreReferences.ThirdPartyScriptsBundle
            }, string.Empty).ToString());
          else
            stringBuilder.AppendLine(BundlingHelper.GetScriptBundlesHtml(htmlHelper, (IEnumerable<BundledScriptFile>) new BundledScriptFile[1]
            {
              pageContext.CoreReferences.ThirdPartyScriptsBundle
            }).ToString());
        }
        else if (pageContext.CoreReferences.CoreScriptsBundle != null)
        {
          stringBuilder.AppendLine(PageHtmlExtensions.CreateScriptTag(htmlHelper, pageContext.CoreReferences.CoreScriptsBundle.Url).ToString());
        }
        else
        {
          foreach (JavascriptFileReference script in pageContext.CoreReferences.Scripts)
            stringBuilder.AppendLine(PageHtmlExtensions.CreateScriptTag(htmlHelper, script.Url).ToString());
        }
        return MvcHtmlString.Create(stringBuilder.ToString());
      }
    }

    public static MvcHtmlString ModuleLoaderConfig(this HtmlHelper htmlHelper) => htmlHelper.JavaScript(htmlHelper.ModuleLoaderConfigContent());

    public static string ModuleLoaderConfigContent(this HtmlHelper htmlHelper)
    {
      RequestContext requestContext = htmlHelper.ViewContext.RequestContext;
      Microsoft.TeamFoundation.Server.WebAccess.PageContext pageContext = WebContextFactory.GetPageContext(requestContext);
      string str = "";
      if (pageContext.Diagnostics.CdnEnabled)
        str = PageHtmlExtensions.CreateCdnFallbackCode(requestContext);
      return str + string.Format("require.config({0}.moduleLoaderConfig);", (object) "__vssPageContext");
    }

    public static MvcHtmlString ScriptTag(
      this HtmlHelper htmlHelper,
      string name,
      string scriptUrl)
    {
      Microsoft.TeamFoundation.Server.WebAccess.PageContext pageContext = WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext);
      TagBuilder tagBuilder = new TagBuilder("script");
      tagBuilder.MergeAttribute("type", "text/javascript");
      tagBuilder.AddNonceAttribute(pageContext.WebContext.TfsRequestContext, pageContext.WebContext.RequestContext.HttpContext);
      if (htmlHelper.ViewContext.HttpContext.UseNewPlatformHost())
        tagBuilder.InnerHtml = string.Format("window.__loadScript({{ url:'{0}', contributionId:'{1}', contentType:'text/javascript', contentLength:{2} }});" + Environment.NewLine, (object) scriptUrl, (object) name, (object) 0);
      else
        tagBuilder.Attribute("src", scriptUrl);
      return tagBuilder.ToHtmlString();
    }

    private static TagBuilder CreateScriptTag(HtmlHelper htmlHelper, string src)
    {
      TagBuilder tagBuilder = new TagBuilder("script");
      tagBuilder.MergeAttribute("type", "text/javascript");
      IVssRequestContext requestContext = htmlHelper.ViewContext.TfsRequestContext(false);
      HttpContextBase httpContext = htmlHelper.ViewContext.HttpContext;
      if (requestContext != null && httpContext != null)
        tagBuilder.AddNonceAttribute(requestContext, httpContext);
      tagBuilder.MergeAttribute(nameof (src), src);
      return tagBuilder;
    }

    private static string CreateCdnFallbackCode(RequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.TfsRequestContext();
      string str = "";
      if (vssRequestContext != null && vssRequestContext.ExecutionEnvironment.IsSslOnly)
        str = ";secure";
      return string.Format("(function(){{\r\nvar unloadStarted = false;\r\nfunction onRequireError(e) {{\r\n    var msg = '{0}';\r\n    function reload(prompted) {{\r\n        document.cookie = '{1}={2};max-age={3};path=/{4}';\r\n        document.cookie = '{5}={6}-'+prompted+'-'+(e.requireType === 'requirenotloaded' ? 'require' : encodeURIComponent(JSON.stringify(e.requireModules)))+';path=/{7}';\r\n        document.location.reload(true);\r\n    }}\r\n    if (!e || !e.requireType || (e.requireType !== 'scripterror' && e.requireType !== 'requirenotloaded')) {{\r\n        throw e;\r\n    }}\r\n    if (unloadStarted) {{\r\n        return;\r\n    }}\r\n    if (!window.requiredModulesLoaded) {{\r\n        reload(false);\r\n    }}\r\n    else if (window.require && require.defined('VSS/Controls/Dialogs')) {{\r\n        require(['VSS/Controls/Dialogs'], function(d) {{\r\n            d.showMessageDialog(msg, {{ title: '{8}', buttons: [d.MessageDialog.buttons.yes, d.MessageDialog.buttons.no] }}).then(function() {{ reload(true) }});\r\n        }});\r\n    }}\r\n    else {{\r\n        if (confirm(msg)) {{\r\n            reload(true);\r\n        }}\r\n    }}\r\n}}\r\nwindow.addEventListener('beforeunload', function() {{\r\n    unloadStarted = true;\r\n}});\r\nif (window.requirejs) {{\r\n    requirejs.onError = onRequireError;\r\n}}\r\nelse {{\r\n    onRequireError({{requireType:'requirenotloaded'}});\r\n}}\r\n}})();", (object) HttpUtility.JavaScriptStringEncode(PlatformResources.CdnFallbackErrorMessage), (object) "TFS-CDN", (object) "disabled", (object) 28800, (object) str, (object) "TFS-CDNTRACE", (object) "report", (object) str, (object) HttpUtility.JavaScriptStringEncode(PlatformResources.CdnFallbackErrorTitle));
    }
  }
}
