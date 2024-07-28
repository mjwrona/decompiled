// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.PipelineSourceProviderService
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PipelineSourceProviderService : IPipelineSourceProviderService, IVssFrameworkService
  {
    private const string c_layer = "PipelineSourceProviderService";
    private readonly List<IPipelineSourceProvider> m_providers;

    public PipelineSourceProviderService() => this.m_providers = new List<IPipelineSourceProvider>()
    {
      (IPipelineSourceProvider) new GitHubProvider(),
      (IPipelineSourceProvider) new JiraProvider()
    };

    public void ServiceStart(IVssRequestContext context)
    {
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public IEnumerable<IPipelineSourceProvider> Providers => (IEnumerable<IPipelineSourceProvider>) this.m_providers.AsReadOnly();

    public IPipelineSourceProvider GetProvider(string providerId, bool throwIfNotFound = true)
    {
      if (!string.IsNullOrEmpty(providerId))
      {
        foreach (IPipelineSourceProvider provider in this.Providers)
        {
          if (string.Equals(providerId, provider.ProviderId, StringComparison.OrdinalIgnoreCase))
            return provider;
        }
      }
      if (throwIfNotFound)
        throw new ArgumentException("Unknown pipeline source provider: " + providerId, nameof (providerId));
      return (IPipelineSourceProvider) null;
    }

    public bool TryGetProvider(string providerId, out IPipelineSourceProvider provider)
    {
      provider = this.GetProvider(providerId, false);
      return provider != null;
    }
  }
}
