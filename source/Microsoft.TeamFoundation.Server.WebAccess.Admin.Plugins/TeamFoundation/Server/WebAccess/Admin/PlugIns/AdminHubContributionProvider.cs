// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.AdminHubContributionProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class AdminHubContributionProvider : IExtensionDataProvider
  {
    private static readonly HashSet<string> s_hubTypes = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "ms.vss-web.hub"
    };

    public string Name => "Admin.HubContributionProvider";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      AdminHubContributionData data = new AdminHubContributionData()
      {
        Contributions = new List<Contribution>()
      };
      string str = contribution.Properties.GetValue("hubGroupId").ToString();
      IContributionService service1 = requestContext.GetService<IContributionService>();
      IClientContributionProviderService service2 = requestContext.GetService<IClientContributionProviderService>();
      IVssRequestContext requestContext1 = requestContext;
      string[] contributionIds = new string[1]{ str };
      HashSet<string> hubTypes = AdminHubContributionProvider.s_hubTypes;
      foreach (Contribution queryContribution in service1.QueryContributions(requestContext1, (IEnumerable<string>) contributionIds, hubTypes, ContributionQueryOptions.IncludeChildren))
      {
        if (!queryContribution.Id.StartsWith("ms.", StringComparison.OrdinalIgnoreCase))
        {
          data.Contributions.Add(queryContribution);
          service2.AddContribution(requestContext, providerContext.SharedData, queryContribution.Id);
        }
      }
      return (object) data;
    }
  }
}
