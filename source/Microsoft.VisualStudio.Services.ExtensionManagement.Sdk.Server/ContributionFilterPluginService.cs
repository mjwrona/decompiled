// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContributionFilterPluginService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ContributionFilterPluginService : IVssFrameworkService
  {
    private IDictionary<string, IContributionFilter> m_registeredFilters;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_registeredFilters = (IDictionary<string, IContributionFilter>) systemRequestContext.GetExtensions<IContributionFilter>(ExtensionLifetime.Service, throwOnError: true).ToDictionary<IContributionFilter, string>((Func<IContributionFilter, string>) (filter => filter.Name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IContributionFilter GetPlugin(IVssRequestContext requestContext, string pluginName)
    {
      IContributionFilter plugin = (IContributionFilter) null;
      if (pluginName != null)
        this.m_registeredFilters.TryGetValue(pluginName, out plugin);
      return plugin;
    }
  }
}
