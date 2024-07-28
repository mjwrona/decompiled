// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ContributionsHtmlExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Bundling;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class ContributionsHtmlExtensions
  {
    private const string c_contributedScriptBundles = "__contributedScriptBundles";

    internal static List<DynamicScriptBundle> GetContributedScriptBundles(this HtmlHelper htmlHelper)
    {
      if (!(htmlHelper.ViewData["__contributedScriptBundles"] is List<DynamicScriptBundle> contributedScriptBundles))
      {
        contributedScriptBundles = new List<DynamicScriptBundle>();
        htmlHelper.ViewData["__contributedScriptBundles"] = (object) contributedScriptBundles;
      }
      return contributedScriptBundles;
    }

    public static void IncludeContributions(
      this HtmlHelper htmlHelper,
      params string[] contributions)
    {
      using (WebPerformanceTimer.StartMeasure(htmlHelper, "Html.IncludeContributions", string.Join(", ", contributions)))
      {
        htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (IncludeContributions));
        try
        {
          WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext).AddContributions((IEnumerable<string>) contributions);
        }
        finally
        {
          htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (IncludeContributions));
        }
      }
    }

    public static MvcHtmlString ScriptContributions(this HtmlHelper htmlHelper)
    {
      ContributionsPageData contributionsPageData = ContributionsHtmlExtensions.GetContributionsPageData(WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext));
      return JsonIslandHtmlExtensions.RestApiJsonIsland(htmlHelper, (object) contributionsPageData, (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "class",
          (object) "vss-contribution-data"
        }
      });
    }

    public static ContributionsPageData GetContributionsPageData(PageContext pageContext)
    {
      IVssRequestContext tfsRequestContext = pageContext.WebContext.TfsRequestContext;
      using (PerformanceTimer.StartMeasure(tfsRequestContext, "Html.ScriptContributions"))
      {
        tfsRequestContext.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, "ScriptContributions");
        try
        {
          return new ContributionsPageData()
          {
            Contributions = (IEnumerable<PageContribution>) pageContext.Contributions.Where<Contribution>((Func<Contribution, bool>) (c => !string.Equals("ms.vss-web.content", c.Type, StringComparison.OrdinalIgnoreCase))).Select<Contribution, PageContribution>((Func<Contribution, PageContribution>) (c => PageContribution.FromContribution(c))).ToArray<PageContribution>(),
            ProviderDetails = (IDictionary<string, PageContributionProviderDetails>) ContributionsHtmlExtensions.FilterContributionProviderDetails(tfsRequestContext, pageContext),
            QueriedContributionIds = pageContext.QueriedContributionIds
          };
        }
        finally
        {
          tfsRequestContext.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, "ScriptContributions");
        }
      }
    }

    private static Dictionary<string, PageContributionProviderDetails> FilterContributionProviderDetails(
      IVssRequestContext collectionContext,
      PageContext pageContext)
    {
      Dictionary<string, PageContributionProviderDetails> dictionary = new Dictionary<string, PageContributionProviderDetails>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IContributionService service = collectionContext.GetService<IContributionService>();
      foreach (Contribution contribution in pageContext.Contributions)
      {
        ContributionIdentifier contributionIdentifier = new ContributionIdentifier(contribution.Id);
        string key = string.Format("{0}.{1}", (object) contributionIdentifier.PublisherName, (object) contributionIdentifier.ExtensionName);
        if (!dictionary.TryGetValue(key, out PageContributionProviderDetails _))
          dictionary[key] = PageContributionProviderDetails.FromContributionProviderDetails(service.QueryContributionProviderDetails(collectionContext, contribution.Id));
      }
      return dictionary;
    }

    public static MvcHtmlString InjectDataProviderData(this HtmlHelper htmlHelper)
    {
      PageContext pageContext = WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext);
      IList<Contribution> dataProviderContributions;
      DataProviderResult dataProviderData = ContributionsHtmlExtensions.GetDataProviderData(pageContext, out dataProviderContributions);
      if (dataProviderData == null)
        return MvcHtmlString.Empty;
      StringBuilder builder = new StringBuilder();
      foreach (ContributedServiceContext contributedServiceContext in ContributedServiceContextData.GetContributedServiceContexts(dataProviderData, (IEnumerable<Contribution>) dataProviderContributions))
      {
        if (contributedServiceContext.Bundles != null)
        {
          if (contributedServiceContext.Bundles.ScriptsExcludedByPath != null)
            htmlHelper.RegisterViewScriptModules(contributedServiceContext.Bundles.ScriptsExcludedByPath.ToArray());
          if (contributedServiceContext.Bundles.Styles != null)
          {
            foreach (DynamicCSSBundle style in contributedServiceContext.Bundles.Styles)
            {
              string str = style.Uri;
              string fallbackThemeUrl = style.FallbackThemeUri;
              if (pageContext.WebContext.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && !Uri.IsWellFormedUriString(str, UriKind.Absolute))
              {
                str = contributedServiceContext.ServiceRootUrl.TrimEnd('/') + "/" + str.TrimStart('/');
                if (!string.IsNullOrEmpty(fallbackThemeUrl))
                  fallbackThemeUrl = contributedServiceContext.ServiceRootUrl.TrimEnd('/') + "/" + fallbackThemeUrl.TrimStart('/');
              }
              BundlingHelper.AppendBundleCSS(pageContext.WebContext, htmlHelper, builder, str, fallbackThemeUrl, (IEnumerable<string>) style.CssFiles, (string) null, style.ContentLength);
            }
            contributedServiceContext.Bundles.Styles = (List<DynamicCSSBundle>) null;
          }
          if (contributedServiceContext.Bundles.Scripts != null)
          {
            List<DynamicScriptBundle> contributedScriptBundles = htmlHelper.GetContributedScriptBundles();
            foreach (DynamicScriptBundle script in contributedServiceContext.Bundles.Scripts)
            {
              if (pageContext.WebContext.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && !Uri.IsWellFormedUriString(script.Uri, UriKind.Absolute))
                script.Uri = contributedServiceContext.ServiceRootUrl.TrimEnd('/') + "/" + script.Uri.TrimStart('/');
              contributedScriptBundles.Add(script);
            }
          }
        }
      }
      builder.Append(JsonIslandHtmlExtensions.RestApiJsonIsland(htmlHelper, (object) dataProviderData, (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "class",
          (object) "vss-web-page-data"
        }
      }).ToString());
      return new MvcHtmlString(builder.ToString());
    }

    public static DataProviderResult GetDataProviderData(
      PageContext pageContext,
      out IList<Contribution> dataProviderContributions)
    {
      IVssRequestContext tfsRequestContext = pageContext.WebContext.TfsRequestContext;
      using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(tfsRequestContext, "Html.InjectDataProviderData"))
      {
        tfsRequestContext.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, "InjectDataProviderData");
        try
        {
          dataProviderContributions = (IList<Contribution>) pageContext.Contributions.Where<Contribution>((Func<Contribution, bool>) (c => c.IsOfType("ms.vss-web.data-provider"))).ToList<Contribution>();
          performanceTimer.AddProperty("DataProviderCount", (object) dataProviderContributions.Count);
          DataProviderResult dataProviderData = WebPageDataProviderUtil.ResolveWebDataProviders(pageContext, (IEnumerable<Contribution>) dataProviderContributions);
          if (dataProviderData != null)
          {
            performanceTimer.AddProperty("ResolvedDataProviderCount", (object) dataProviderData.ResolvedProviders.Count);
            performanceTimer.AddProperty("DataProviderErrorsCount", (object) dataProviderData.ResolvedProviders.Count<ResolvedDataProvider>((Func<ResolvedDataProvider, bool>) (p => !string.IsNullOrEmpty(p.Error))));
          }
          return dataProviderData;
        }
        finally
        {
          tfsRequestContext.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, "InjectDataProviderData");
        }
      }
    }

    public static MvcHtmlString NewPlatformHeadContent(this HtmlHelper htmlHelper)
    {
      IVssRequestContext tfsRequestContext = WebContextFactory.GetPageContext(htmlHelper.ViewContext.RequestContext).WebContext.TfsRequestContext;
      Contribution templateContribution = tfsRequestContext.GetService<IContributionService>().QueryContribution(tfsRequestContext, "ms.vss-web.legacy-master-page-shim-template");
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) memoryStream, (Encoding) new UTF8Encoding(false), 1024, true))
          tfsRequestContext.GetService<IContributionTemplateService>().GetTemplate(tfsRequestContext, templateContribution, string.Empty).Render(tfsRequestContext, (TextWriter) streamWriter);
        return new MvcHtmlString(Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int) memoryStream.Position));
      }
    }
  }
}
