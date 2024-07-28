// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildSourceProviderFactory
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class BuildSourceProviderFactory : IBuildSourceProviderFactory, IDisposable
  {
    private IDisposableReadOnlyList<IBuildSourceProvider> m_extensions;
    private Dictionary<string, IBuildSourceProvider> m_sourceProviderMap;
    private IBuildSourceProvider[] m_sourceProviders;

    public BuildSourceProviderFactory()
    {
    }

    protected BuildSourceProviderFactory(List<IBuildSourceProvider> providers) => this.m_extensions = (IDisposableReadOnlyList<IBuildSourceProvider>) new DisposableCollection<IBuildSourceProvider>((IReadOnlyList<IBuildSourceProvider>) providers);

    public void Dispose()
    {
      if (this.m_extensions == null)
        return;
      this.m_extensions.Dispose();
      this.m_sourceProviderMap = (Dictionary<string, IBuildSourceProvider>) null;
      this.m_sourceProviders = (IBuildSourceProvider[]) null;
    }

    public IBuildSourceProvider GetSourceProvider(
      IVssRequestContext requestContext,
      string repositoryType,
      bool throwIfNotFound)
    {
      ArgumentUtility.CheckForNull<string>(repositoryType, nameof (repositoryType));
      this.Initialize(requestContext);
      IBuildSourceProvider sourceProvider = (IBuildSourceProvider) null;
      if (((!this.m_sourceProviderMap.TryGetValue(repositoryType, out sourceProvider) ? 1 : (sourceProvider == null ? 1 : 0)) & (throwIfNotFound ? 1 : 0)) != 0)
        throw new BuildRepositoryTypeNotSupportedException(BuildServerResources.BuildRepositoryTypeNotSupported((object) repositoryType));
      return sourceProvider;
    }

    public IEnumerable<IBuildSourceProvider> GetSourceProviders(IVssRequestContext requestContext)
    {
      this.Initialize(requestContext);
      return (IEnumerable<IBuildSourceProvider>) this.m_sourceProviders;
    }

    private void Initialize(IVssRequestContext requestContext)
    {
      if (this.m_extensions == null)
        this.m_extensions = requestContext.GetExtensions<IBuildSourceProvider>();
      if (this.m_sourceProviderMap != null)
        return;
      this.m_sourceProviders = this.m_extensions.GetSourceProvidersForExecutionEnvironment(requestContext).ToArray<IBuildSourceProvider>();
      Dictionary<string, IBuildSourceProvider> dictionary = new Dictionary<string, IBuildSourceProvider>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (IBuildSourceProvider sourceProvider in this.m_sourceProviders)
      {
        string name = sourceProvider.GetAttributes(requestContext).Name;
        if (!dictionary.ContainsKey(name))
          dictionary.Add(name, sourceProvider);
      }
      this.m_sourceProviderMap = dictionary;
    }
  }
}
