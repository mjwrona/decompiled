// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ContributedPageActionResult
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public class ContributedPageActionResult : ActionResult
  {
    private IVssRequestContext m_requestContext;
    private Action<IDictionary<string, object>> m_populateTemplateContextAction;
    private static readonly UTF8Encoding s_utf8EncodingWithoutBom = new UTF8Encoding(false);

    public ContributedPageActionResult(
      IVssRequestContext requestContext,
      Action<IDictionary<string, object>> populateTemplateContextAction = null)
    {
      this.m_requestContext = requestContext;
      this.m_populateTemplateContextAction = populateTemplateContextAction;
    }

    public override void ExecuteResult(ControllerContext context)
    {
      using (PerformanceTimer.StartMeasure(this.m_requestContext, "ContributedPageAction.Execute"))
      {
        HttpRequestBase request = context.RequestContext.HttpContext.Request;
        IContributionRoutingService service1 = this.m_requestContext.GetService<IContributionRoutingService>();
        string requestedTemplateName = service1.GetRequestedTemplateName(this.m_requestContext);
        int requestedTemplateVersion = service1.GetRequestedTemplateVersion(this.m_requestContext);
        if (requestedTemplateVersion > 0 && requestedTemplateVersion != 2)
        {
          this.m_requestContext.Trace(100136380, TraceLevel.Info, "WebApi", nameof (ContributedPageActionResult), "Invalid FPS request version {0} did not match web platform version {1}.", (object) requestedTemplateVersion, (object) 2);
          throw new HttpException(400, "Invalid template version");
        }
        List<ContributedNavigation> selectedElements = this.m_requestContext.GetService<IContributionNavigationService>().GetSelectedElements(this.m_requestContext);
        if (selectedElements == null)
          return;
        IContributionTemplate contributionTemplate = (IContributionTemplate) null;
        UrlHelper urlHelper = new UrlHelper(context.RequestContext);
        Dictionary<string, object> templateContext = new Dictionary<string, object>()
        {
          {
            "shortcutIconUrl",
            (object) StaticResources.Content.GetLocation("icons/favicon.ico")
          },
          {
            "cultureName",
            (object) CultureInfo.CurrentUICulture.Name
          },
          {
            "flushAfterContent",
            (object) this.m_requestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.FlushPageAfterContent")
          },
          {
            "useHtml5Notation",
            (object) this.m_requestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.UseHtml5Notation")
          },
          {
            "useDropdownListComponent",
            (object) this.m_requestContext.IsFeatureEnabled("VisualStudio.Services.Webplatform.UseDropdownListComponent")
          },
          {
            "useRedesignedLinkComponent",
            (object) this.m_requestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.UseRedesignedLinkComponent")
          }
        };
        Action<IDictionary<string, object>> templateContextAction = this.m_populateTemplateContextAction;
        if (templateContextAction != null)
          templateContextAction((IDictionary<string, object>) templateContext);
        IContributionTemplateService service2 = this.m_requestContext.GetService<IContributionTemplateService>();
        IContributedRoute route = service1.GetRoute(this.m_requestContext);
        if (route != null)
          contributionTemplate = service2.GetTemplate(this.m_requestContext, route.ContributionId, requestedTemplateName);
        if (contributionTemplate == null)
        {
          for (int index = selectedElements.Count - 1; index >= 0; --index)
          {
            contributionTemplate = service2.GetTemplate(this.m_requestContext, selectedElements[index].Id, requestedTemplateName);
            if (contributionTemplate != null)
              break;
          }
        }
        if (contributionTemplate == null)
        {
          ContributedSite site = this.m_requestContext.GetService<IContributionManagementService>().GetSite(this.m_requestContext);
          if (site != null)
            contributionTemplate = service2.GetTemplate(this.m_requestContext, site.Id, requestedTemplateName);
        }
        if (contributionTemplate == null)
          return;
        HttpResponseBase response = context.RequestContext.HttpContext.Response;
        string a = contributionTemplate.ContentType;
        if (string.IsNullOrEmpty(a))
          a = "text/html";
        Encoding encoding = (!string.Equals(a, "text/html", StringComparison.OrdinalIgnoreCase) ? 0 : (!this.m_requestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.DoNotEmitBom") ? 1 : 0)) != 0 ? Encoding.UTF8 : (Encoding) ContributedPageActionResult.s_utf8EncodingWithoutBom;
        string str = a;
        response.ContentType = str;
        context.HttpContext.Response.ContentEncoding = encoding;
        context.HttpContext.Response.Charset = "utf-8";
        using (StreamWriter streamWriter = new StreamWriter(context.HttpContext.Response.OutputStream, encoding, (int) ushort.MaxValue, true))
          contributionTemplate.Render(this.m_requestContext, (TextWriter) streamWriter, (object) templateContext);
      }
    }
  }
}
