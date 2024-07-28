// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders.SourceProviderService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders
{
  internal sealed class SourceProviderService : ISourceProviderService, IVssFrameworkService
  {
    private IDictionary<string, ISourceProvider> m_providerLookup;
    private IDisposableReadOnlyList<ISourceProvider> m_allProviders;

    public ISourceProvider GetSourceProvider(IVssRequestContext requestContext, string type)
    {
      ISourceProvider sourceProvider;
      if (!this.m_providerLookup.TryGetValue(type, out sourceProvider))
        throw new NotSupportedException("Repository type " + type + " is not supported");
      return sourceProvider;
    }

    private static bool IncludeProvider(IVssRequestContext requestContext, ISourceProvider provider)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        return provider.Attributes.Availability.HasFlag((Enum) SourceProviderAvailability.Hosted);
      return !requestContext.ExecutionEnvironment.IsOnPremisesDeployment || provider.Attributes.Availability.HasFlag((Enum) SourceProviderAvailability.OnPremises);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
      this.m_allProviders?.Dispose();
      this.m_allProviders = (IDisposableReadOnlyList<ISourceProvider>) null;
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      Dictionary<string, ISourceProvider> dictionary = new Dictionary<string, ISourceProvider>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_allProviders = requestContext.GetExtensions<ISourceProvider>((Func<ISourceProvider, bool>) (x => SourceProviderService.IncludeProvider(requestContext, x)));
      foreach (ISourceProvider allProvider in (IEnumerable<ISourceProvider>) this.m_allProviders)
      {
        if (!dictionary.ContainsKey(allProvider.Attributes.Name))
          dictionary.Add(allProvider.Attributes.Name, allProvider);
      }
      this.m_providerLookup = (IDictionary<string, ISourceProvider>) dictionary;
    }
  }
}
