// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Contributions.HtmlContent.ContributionHtmlProviderTypesService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Contributions.HtmlContent
{
  internal class ContributionHtmlProviderTypesService : IVssFrameworkService
  {
    private IDictionary<string, Type> m_providerTypes;
    private const string s_area = "ContributionService";
    private const string s_layer = "HtmlProviderTypesService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(10013551, "ContributionService", "HtmlProviderTypesService", nameof (ServiceStart));
      systemRequestContext.CheckDeploymentRequestContext();
      try
      {
        using (IDisposableReadOnlyList<IContributionHtmlProvider> extensions = systemRequestContext.GetExtensions<IContributionHtmlProvider>(throwOnError: true))
        {
          this.m_providerTypes = (IDictionary<string, Type>) new Dictionary<string, Type>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          foreach (IContributionHtmlProvider contributionHtmlProvider in (IEnumerable<IContributionHtmlProvider>) extensions)
            this.m_providerTypes[contributionHtmlProvider.Name] = contributionHtmlProvider.GetType();
        }
      }
      finally
      {
        systemRequestContext.TraceLeave(10013552, "ContributionService", "HtmlProviderTypesService", nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IDictionary<string, Type> GetHtmlProviderTypes() => this.m_providerTypes;
  }
}
