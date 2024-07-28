// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Contributions.HtmlContent.ContributionHtmlProviderService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Contributions.HtmlContent
{
  internal class ContributionHtmlProviderService : 
    IContributionHtmlProviderService,
    IVssFrameworkService
  {
    private IDictionary<string, Type> m_providerTypes;
    private const string s_htmlProviderRequestContextKey = "HtmlProviders";
    private const string s_area = "ContributionService";
    private const string s_layer = "HtmlProviderService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(10013541, "ContributionService", "HtmlProviderService", nameof (ServiceStart));
      try
      {
        this.m_providerTypes = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<ContributionHtmlProviderTypesService>().GetHtmlProviderTypes();
      }
      finally
      {
        systemRequestContext.TraceLeave(10013542, "ContributionService", "HtmlProviderService", nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ContributionHtmlProviderResult GetHtml(
      IVssRequestContext requestContext,
      ContributionHtmlProviderContext providerContext,
      ContributionNode htmlProviderContribution,
      ContributionNode targetContribution)
    {
      ContributionHtmlProviderResult html = new ContributionHtmlProviderResult();
      using (PerformanceTimer.StartMeasure(requestContext, "ContributionHtmlProviderService.GetHtml", htmlProviderContribution.Id))
      {
        string property = htmlProviderContribution.Contribution.GetProperty<string>("name");
        if (string.IsNullOrEmpty(property))
        {
          html.Error = string.Format("Html provider contribution '{0}' is missing property '{1}'.", (object) htmlProviderContribution.Id, (object) "name");
        }
        else
        {
          IContributionHtmlProvider htmlProvider = this.GetHtmlProvider(requestContext, property);
          if (htmlProvider != null)
          {
            try
            {
              html.Html = htmlProvider.GetHtml(requestContext, providerContext, targetContribution);
            }
            catch (Exception ex)
            {
              html.Error = ex.Message;
              requestContext.TraceException(10013543, "ContributionService", "HtmlProviderService", ex);
            }
          }
          else
            html.Error = string.Format("Html provider contribution '{0}' specifies a provider '{1}' which does not exist.", (object) htmlProviderContribution.Id, (object) property);
        }
      }
      return html;
    }

    private IContributionHtmlProvider GetHtmlProvider(
      IVssRequestContext requestContext,
      string providerName)
    {
      IContributionHtmlProvider htmlProvider = (IContributionHtmlProvider) null;
      Dictionary<string, IContributionHtmlProvider> dictionary;
      if (!requestContext.Items.TryGetValue<Dictionary<string, IContributionHtmlProvider>>("HtmlProviders", out dictionary))
      {
        dictionary = new Dictionary<string, IContributionHtmlProvider>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        requestContext.Items["HtmlProviders"] = (object) dictionary;
      }
      Type type;
      if (!dictionary.TryGetValue(providerName, out htmlProvider) && this.m_providerTypes.TryGetValue(providerName, out type))
      {
        htmlProvider = (IContributionHtmlProvider) Activator.CreateInstance(type);
        if (htmlProvider != null)
          dictionary[providerName] = htmlProvider;
      }
      return htmlProvider;
    }
  }
}
