// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.PlatformHtmlExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.UI;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class PlatformHtmlExtensions
  {
    public const string Verification_Prefix = "__RequestVerificationToken";
    public const string Verification2_Prefix = "__RequestVerificationToken2";
    private const string AntiForgeryTokenRequestKey = "WebPlatform.AntiForgeryTokens";
    internal static string ScriptContentFolder = "_scripts";
    internal const string FragmentActionParamKey = "_a";

    public static string GetAntiForgeryTokenName(string appPath) => !string.IsNullOrEmpty(appPath) && !(appPath == "/") ? "__RequestVerificationToken_" + HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(appPath)) : "__RequestVerificationToken";

    public static IDictionary<string, string> GetTfsAntiForgeryTokens(
      IVssRequestContext requestContext,
      HttpContextBase httpContext)
    {
      object obj;
      IDictionary<string, string> antiForgeryTokens;
      if (requestContext.Items.TryGetValue("WebPlatform.AntiForgeryTokens", out obj))
      {
        antiForgeryTokens = obj as IDictionary<string, string>;
      }
      else
      {
        antiForgeryTokens = PlatformHtmlExtensions.GetTfsAntiForgeryTokens(httpContext);
        requestContext.Items["WebPlatform.AntiForgeryTokens"] = (object) antiForgeryTokens;
      }
      return antiForgeryTokens;
    }

    public static IDictionary<string, string> GetTfsAntiForgeryTokens(HttpContextBase httpContext) => PlatformHtmlExtensions.GetTfsAntiForgeryTokens(httpContext, false);

    private static IDictionary<string, string> GetTfsAntiForgeryTokens(
      HttpContextBase httpContext,
      bool isRetry)
    {
      try
      {
        Dictionary<string, string> antiForgeryTokens = new Dictionary<string, string>();
        string forgeryTokenName = PlatformHtmlExtensions.GetAntiForgeryTokenName(httpContext.Request.ApplicationPath);
        HttpCookie cookie1 = httpContext.Request.Cookies[forgeryTokenName];
        bool flag = cookie1 == null;
        string newCookieToken;
        string formToken;
        AntiForgery.GetTokens(cookie1?.Value, out newCookieToken, out formToken);
        if (newCookieToken != null)
        {
          HttpCookie cookie2 = new HttpCookie(forgeryTokenName, newCookieToken);
          cookie2.HttpOnly = true;
          if (httpContext.Request.IsSecureConnection)
            cookie2.Secure = true;
          httpContext.Response.Cookies.Set(cookie2);
        }
        antiForgeryTokens["__RequestVerificationToken"] = formToken;
        if (flag)
        {
          string name = "__RequestVerificationToken2" + Guid.NewGuid().ToString();
          HttpCookie cookie3 = new HttpCookie(name, newCookieToken);
          cookie3.HttpOnly = true;
          if (httpContext.Request.IsSecureConnection)
            cookie3.Secure = true;
          httpContext.Response.Cookies.Add(cookie3);
          antiForgeryTokens["__RequestVerificationToken2"] = name;
        }
        return (IDictionary<string, string>) antiForgeryTokens;
      }
      catch (HttpAntiForgeryException ex)
      {
        HttpRequestBase request = httpContext.Request;
        if (!isRetry && ex.InnerException is HttpException && ex.InnerException.InnerException is ViewStateException)
        {
          string forgeryTokenName = PlatformHtmlExtensions.GetAntiForgeryTokenName(request.ApplicationPath);
          request.Cookies.Remove(forgeryTokenName);
          return PlatformHtmlExtensions.GetTfsAntiForgeryTokens(httpContext, true);
        }
        throw;
      }
    }

    public static MvcHtmlString TfsAntiForgeryToken(this HtmlHelper htmlHelper)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "Html.TfsAntiForgeryToken"))
      {
        htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (TfsAntiForgeryToken));
        try
        {
          string forgeryTokenName = PlatformHtmlExtensions.GetAntiForgeryTokenName(htmlHelper.ViewContext.HttpContext.Request.ApplicationPath);
          HttpContextBase httpContext = htmlHelper.ViewContext.HttpContext;
          int num = !((IEnumerable<string>) httpContext.Request.Cookies.AllKeys).Contains<string>(forgeryTokenName) ? 1 : 0;
          MvcHtmlString mvcHtmlString = htmlHelper.AntiForgeryToken();
          if (num != 0)
          {
            string name = "__RequestVerificationToken2" + Guid.NewGuid().ToString();
            string str = httpContext.Response.Cookies[forgeryTokenName].Value;
            HttpCookie cookie = new HttpCookie(name, str);
            cookie.HttpOnly = true;
            if (httpContext.Request.IsSecureConnection)
              cookie.Secure = true;
            CookieModifier.AddSameSiteNoneToCookie(htmlHelper.ViewContext.RequestContext.TfsRequestContext(), cookie);
            httpContext.Response.Cookies.Add(cookie);
            mvcHtmlString = new MvcHtmlString(mvcHtmlString.ToString() + "<input name=\"__RequestVerificationToken2\" type=\"hidden\" value=\"" + name + "\" />");
          }
          if (((IEnumerable<string>) httpContext.Response.Cookies.AllKeys).Contains<string>(forgeryTokenName))
          {
            HttpCookie cookie = httpContext.Response.Cookies[forgeryTokenName];
            if (httpContext.Request.IsSecureConnection)
              cookie.Secure = true;
            CookieModifier.AddSameSiteNoneToCookie(htmlHelper.ViewContext.RequestContext.TfsRequestContext(), cookie);
          }
          return mvcHtmlString;
        }
        catch (HttpAntiForgeryException ex)
        {
          HttpRequestBase request = htmlHelper.ViewContext.HttpContext.Request;
          if (ex.InnerException is HttpException && ex.InnerException.InnerException is ViewStateException)
          {
            string forgeryTokenName = PlatformHtmlExtensions.GetAntiForgeryTokenName(request.ApplicationPath);
            request.Cookies.Remove(forgeryTokenName);
            return htmlHelper.TfsAntiForgeryToken();
          }
          throw;
        }
        finally
        {
          htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (TfsAntiForgeryToken));
        }
      }
    }

    public static MvcHtmlString WebContext(this HtmlHelper htmlHelper) => htmlHelper.WebContext(htmlHelper.ViewContext.WebContext());

    public static MvcHtmlString WebContext(this HtmlHelper htmlHelper, Microsoft.TeamFoundation.Server.WebAccess.WebContext model) => PlatformHtmlExtensions.WebContext(htmlHelper, model, (IDictionary<string, object>) null);

    public static MvcHtmlString WebContext(
      this HtmlHelper htmlHelper,
      Microsoft.TeamFoundation.Server.WebAccess.WebContext model,
      object htmlAttributes)
    {
      return PlatformHtmlExtensions.WebContext(htmlHelper, model, (IDictionary<string, object>) new RouteValueDictionary(htmlAttributes));
    }

    public static MvcHtmlString WebContext(
      this HtmlHelper htmlHelper,
      Microsoft.TeamFoundation.Server.WebAccess.WebContext model,
      IDictionary<string, object> htmlAttributes)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (WebContext));
      try
      {
        StringBuilder stringBuilder = new StringBuilder();
        TagBuilder tagBuilder = new TagBuilder("script");
        tagBuilder.MergeAttribute("type", "application/json");
        if (htmlAttributes != null)
          tagBuilder.MergeAttributes<string, object>(htmlAttributes);
        tagBuilder.AddCssClass("tfs-context");
        tagBuilder.InnerHtml = RestApiJsonResult.SerializeRestApiData(model.TfsRequestContext, (object) model);
        stringBuilder.AppendLine(tagBuilder.ToString(TagRenderMode.Normal));
        return MvcHtmlString.Create(stringBuilder.ToString());
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (WebContext));
      }
    }

    public static string ThemeName(this UrlHelper urlHelper) => WebContextFactory.GetWebContext(urlHelper.RequestContext).Globalization.Theme;

    public static string Themed(this UrlHelper urlHelper, string fileName)
    {
      IVssRequestContext tfsRequestContext = WebContextFactory.GetWebContext(urlHelper.RequestContext).TfsRequestContext;
      return StaticResources.Versioned.Themes.GetLocation(urlHelper.ThemeName() + "/" + fileName);
    }

    internal static string GetThemedPartialPath(string bundleName, string themeName) => "/App_Themes/" + themeName + "/" + bundleName;

    public static string GetThemedCssFileUrl(RequestContext requestContext, string cssFileName) => StaticResources.Versioned.Themes.GetLocation(string.Format("{0}/{1}", (object) new UrlHelper(requestContext).ThemeName(), (object) cssFileName));

    public static IHtmlString RenderThemedCssFile(this HtmlHelper htmlHelper, string cssFileName)
    {
      string str1 = new UrlHelper(htmlHelper.ViewContext.RequestContext).ThemeName();
      string relativePath = string.Format("{0}/{1}", (object) str1, (object) cssFileName);
      IVssRequestContext tfsRequestContext = WebContextFactory.GetWebContext(htmlHelper.ViewContext.RequestContext).TfsRequestContext;
      IVssRequestContext requestContext = tfsRequestContext;
      IHtmlString htmlString1 = Styles.Render(StaticResources.Versioned.Themes.GetLocation(relativePath, requestContext));
      if (str1 == ThemesUtility.DefaultThemeName && htmlString1 != null && !string.IsNullOrEmpty(htmlString1.ToString()))
      {
        IHtmlString htmlString2 = Styles.Url(StaticResources.Versioned.Themes.GetLocation(string.Format("{0}/{1}", (object) ThemesUtility.HighContrastThemeName, (object) cssFileName), tfsRequestContext));
        if (!string.IsNullOrEmpty(htmlString2.ToString()))
        {
          string str2 = htmlString1.ToString();
          int num = str2.LastIndexOf("/>");
          if (num > -1)
            return (IHtmlString) MvcHtmlString.Create(string.Format("{0} data-highcontrast=\"{1}\"{2}", (object) str2.Substring(0, num), (object) htmlString2.ToString(), (object) str2.Substring(num)));
        }
      }
      return htmlString1;
    }

    public static TagBuilder GetCssStyleTag(
      this HtmlHelper htmlHelper,
      string linkUrl,
      string highContrastLinkUrl)
    {
      TagBuilder cssStyleTag = new TagBuilder("link");
      cssStyleTag.MergeAttribute("rel", "stylesheet");
      cssStyleTag.MergeAttribute("href", linkUrl);
      if (!string.IsNullOrEmpty(highContrastLinkUrl))
        cssStyleTag.MergeAttribute("data-highcontrast", highContrastLinkUrl);
      return cssStyleTag;
    }

    public static string LocalVersionedStaticContentRoot(
      string staticContentVersion = null,
      StaticResourcePathKind pathKind = StaticResourcePathKind.Relative)
    {
      if (string.IsNullOrEmpty(staticContentVersion))
        staticContentVersion = StaticResources.Versioned.Version;
      return StaticResources.GetStaticUrl(path: "tfs/", pathKind: pathKind) + staticContentVersion;
    }

    public static string TfsScriptContent(
      this UrlHelper urlHelper,
      string relativeFilePath,
      bool debugMode,
      bool localOnly = false,
      string staticContentVersion = null)
    {
      if (localOnly)
      {
        string str = PlatformHtmlExtensions.LocalVersionedStaticContentRoot(staticContentVersion);
        return urlHelper.Content(string.Format("{0}/{1}/{2}", (object) str, (object) PlatformHtmlExtensions.ScriptContentFolder, (object) ("TFS/" + (debugMode ? "debug/" : "min/") + relativeFilePath)));
      }
      IVssRequestContext requestContext = urlHelper.RequestContext.TfsRequestContext();
      return StaticResources.Versioned.Scripts.TFS.GetLocation((debugMode ? "debug/" : "min/") + relativeFilePath, requestContext, staticContentVersion);
    }

    private static MvcHtmlString CreateScriptTag(StringBuilder script)
    {
      TagBuilder tagBuilder = new TagBuilder(nameof (script));
      tagBuilder.MergeAttribute("type", "text/javascript");
      tagBuilder.InnerHtml = script.ToString();
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    public static TagBuilder CreateScriptTag(string src)
    {
      TagBuilder scriptTag = new TagBuilder("script");
      scriptTag.MergeAttribute("type", "text/javascript");
      scriptTag.MergeAttribute(nameof (src), src);
      return scriptTag;
    }

    public static bool DebugEnabled(this HtmlHelper htmlHelper) => htmlHelper.ViewContext.WebContext().Diagnostics.DebugMode;

    public static bool IsTracePointCollectorEnabled(this HtmlHelper htmlHelper) => htmlHelper.ViewContext.WebContext().Diagnostics.TracePointCollectionEnabled;
  }
}
