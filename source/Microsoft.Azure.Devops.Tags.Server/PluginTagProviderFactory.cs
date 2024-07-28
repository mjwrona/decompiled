// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.PluginTagProviderFactory
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Tags.Server
{
  public class PluginTagProviderFactory : IDisposable
  {
    private string m_plugInDirectory;
    private bool m_discoveredPlugIns;
    private Dictionary<Guid, ITagProvider> m_tagProviders;
    private IDisposableReadOnlyList<ITagProvider> m_allPlugins;
    private ILockName m_tagProvidersLockName;

    public PluginTagProviderFactory(IVssServiceHost serviceHost)
    {
      ArgumentUtility.CheckForNull<IVssServiceHost>(serviceHost, nameof (serviceHost));
      this.m_plugInDirectory = serviceHost.PlugInDirectory ?? string.Empty;
      ArgumentUtility.CheckStringForNullOrEmpty(this.m_plugInDirectory, "serviceHost.PlugInDirectory");
      this.m_tagProviders = new Dictionary<Guid, ITagProvider>();
      this.m_tagProvidersLockName = serviceHost.CreateUniqueLockName(nameof (PluginTagProviderFactory));
    }

    public void Dispose()
    {
      if (this.m_tagProviders != null)
      {
        this.m_tagProviders.Clear();
        this.m_tagProviders = (Dictionary<Guid, ITagProvider>) null;
      }
      if (this.m_allPlugins == null)
        return;
      this.m_allPlugins.Dispose();
      this.m_allPlugins = (IDisposableReadOnlyList<ITagProvider>) null;
    }

    internal ITagProvider<T> GetOrCreateTagProvider<T>(
      IVssRequestContext requestContext,
      Guid artifactKind)
    {
      this.EnsureProvidersAreDiscovered(requestContext);
      ITagProvider tagProvider = (ITagProvider) null;
      return this.m_tagProviders.TryGetValue(artifactKind, out tagProvider) ? tagProvider as ITagProvider<T> : (ITagProvider<T>) null;
    }

    internal IEnumerable<ITagProvider> GetTagProviders(IVssRequestContext requestContext)
    {
      this.EnsureProvidersAreDiscovered(requestContext);
      return (IEnumerable<ITagProvider>) this.m_tagProviders.Values;
    }

    private void EnsureProvidersAreDiscovered(IVssRequestContext requestContext)
    {
      if (this.m_discoveredPlugIns)
        return;
      using (requestContext.Lock(this.m_tagProvidersLockName))
      {
        if (this.m_discoveredPlugIns)
          return;
        this.m_allPlugins = VssExtensionManagementService.GetExtensionsRaw<ITagProvider>(this.m_plugInDirectory);
        foreach (ITagProvider tagProvider in new List<ITagProvider>((IEnumerable<ITagProvider>) this.m_allPlugins))
        {
          if (tagProvider.ArtifactKind.Equals(Guid.Empty))
            TeamFoundationTrace.Error(TraceKeywordSets.General, "Tag provider {0} does not have valid artifact kind.", (object) tagProvider.GetType().FullName);
          else if (this.m_tagProviders.ContainsKey(tagProvider.ArtifactKind))
            TeamFoundationTrace.Error(TraceKeywordSets.General, "Tag provider for artifact kind {0} has already been registered.", (object) tagProvider.ArtifactKind);
          else
            this.m_tagProviders.Add(tagProvider.ArtifactKind, tagProvider);
        }
        this.m_discoveredPlugIns = true;
      }
    }
  }
}
