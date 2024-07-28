// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContributionProviderService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ContributionProviderService : IVssFrameworkService, IContributionProviderService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IContributionProvider QueryProvider(
      IVssRequestContext requestContext,
      string providerName)
    {
      IDisposableReadOnlyList<IContributionSource> extensions = requestContext.GetExtensions<IContributionSource>(ExtensionLifetime.Service);
      List<IContributionSource> contributionSourceList = new List<IContributionSource>();
      contributionSourceList.AddRange((IEnumerable<IContributionSource>) extensions.OrderBy<IContributionSource, int>((Func<IContributionSource, int>) (c => c.Priority)));
      IContributionProvider contributionProvider = (IContributionProvider) null;
      foreach (IContributionSource contributionSource in contributionSourceList)
      {
        foreach (IContributionProvider queryProvider in contributionSource.QueryProviders(requestContext))
        {
          if (queryProvider.ProviderName.Equals(providerName))
          {
            contributionProvider = queryProvider;
            break;
          }
        }
      }
      return contributionProvider != null ? contributionProvider : throw new ProviderNotFoundException(providerName);
    }
  }
}
