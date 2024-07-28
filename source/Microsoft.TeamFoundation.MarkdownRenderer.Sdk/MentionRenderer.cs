// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.MarkdownRenderer.MentionRenderer
// Assembly: Microsoft.TeamFoundation.MarkdownRenderer.Sdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EF23BBEE-E2C7-4A34-A6FB-0292D3B7C69D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.MarkdownRenderer.Sdk.dll

using Markdig.Renderers;
using Markdig.Renderers.Html;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.MarkdownRenderer
{
  public class MentionRenderer : HtmlObjectRenderer<InlineMention>
  {
    private readonly IVssRequestContext CachedRequestContext;
    private Guid ProjectId;

    public MentionRenderer(IVssRequestContext requestcontext, Guid projectId)
    {
      this.CachedRequestContext = requestcontext;
      this.ProjectId = projectId;
    }

    protected override void Write(HtmlRenderer renderer, InlineMention obj)
    {
      Microsoft.TeamFoundation.Mention.Server.Mention mention = obj.Mention;
      string url = string.Empty;
      string className = string.Empty;
      string text = mention.RawText;
      string version = "1.0";
      if (mention.ArtifactType == "WorkItem")
      {
        className = "mention-widget-workitem";
        url = this.GetMentionedWorkItemUrl(mention);
      }
      else if (mention.ArtifactType == "Person")
      {
        Guid result;
        if (mention.RawText != null && mention.RawText.StartsWith("@") && Guid.TryParse(mention.RawText.Substring(1), out result))
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.CachedRequestContext.GetService<IdentityService>().ReadIdentities(this.CachedRequestContext, (IList<Guid>) new List<Guid>()
          {
            result
          }, QueryMembership.None, (IEnumerable<string>) null);
          if (identityList != null && identityList.Count > 0)
            text = string.Format("@" + identityList[0]?.DisplayName);
        }
        version = "2.0";
      }
      else if (mention.ArtifactType == "PullRequest")
        url = this.GetPullRequestUrl(mention);
      this.RenderMentionLink(renderer, mention, text, url, className, version);
    }

    private string GetMentionedWorkItemUrl(Microsoft.TeamFoundation.Mention.Server.Mention mention)
    {
      IContributionRoutingService service = this.CachedRequestContext.GetService<IContributionRoutingService>();
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "project",
          (object) this.ProjectId.ToString()
        },
        {
          "id",
          (object) mention.ArtifactId.ToString()
        }
      };
      try
      {
        return service.RouteUrl(this.CachedRequestContext, "ms.vss-work-web.work-items-form-route", new RouteValueDictionary((IDictionary<string, object>) dictionary));
      }
      catch (InvalidCastException ex)
      {
        return this.CreateUrlWithoutInternalContextAvailable(string.Format("/{0}/_workitems/edit/{1}", (object) this.ProjectId, (object) mention.ArtifactId));
      }
    }

    private string GetPullRequestUrl(Microsoft.TeamFoundation.Mention.Server.Mention mention)
    {
      IContributionRoutingService service = this.CachedRequestContext.GetService<IContributionRoutingService>();
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "project",
          (object) this.ProjectId.ToString()
        },
        {
          "parameters",
          (object) mention.ArtifactId.ToString()
        }
      };
      try
      {
        return service.RouteUrl(this.CachedRequestContext, "ms.vss-code-web.pull-request-review-route", new RouteValueDictionary((IDictionary<string, object>) dictionary));
      }
      catch (InvalidCastException ex)
      {
        return this.CreateUrlWithoutInternalContextAvailable(string.Format("_git/{0}/pullrequest/{1}", (object) this.ProjectId, (object) mention.ArtifactId));
      }
    }

    private string CreateUrlWithoutInternalContextAvailable(string urlString)
    {
      ILocationService service = this.CachedRequestContext.GetService<ILocationService>();
      Guid serviceAreaIdentifier = this.CachedRequestContext.ExecutionEnvironment.IsOnPremisesDeployment ? ServiceInstanceTypes.TFSOnPremises : ServiceInstanceTypes.TFS;
      try
      {
        return UriUtility.Combine(new Uri(service.GetLocationServiceUrl(this.CachedRequestContext, serviceAreaIdentifier, AccessMappingConstants.ClientAccessMappingMoniker)), urlString, true).AbsoluteUri;
      }
      catch (Exception ex)
      {
        this.CachedRequestContext.Trace(101949, TraceLevel.Error, nameof (MentionRenderer), "Service", "Failed finding location for  {0}", (object) serviceAreaIdentifier);
        return string.Empty;
      }
    }

    private void RenderMentionLink(
      HtmlRenderer renderer,
      Microsoft.TeamFoundation.Mention.Server.Mention mention,
      string text,
      string url,
      string className,
      string version)
    {
      renderer.Write("<a");
      renderer.Write(" href=\"").WriteEscape(url).Write("\"");
      renderer.Write(" data-vss-mention=\"version:" + version + "," + mention.ArtifactId + "\"");
      renderer.Write(" class=\"mention-link " + className + "\">");
      renderer.Write(text);
      renderer.Write("</a>");
    }
  }
}
