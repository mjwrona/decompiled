// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ConnectedService.Server.ConnectedServiceProviderService
// Assembly: Microsoft.TeamFoundation.ConnectedService.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEB400C-7A81-4197-B897-D0116BC50257
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ConnectedService.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.ConnectedService.Server
{
  public class ConnectedServiceProviderService : 
    IConnectedServiceProviderService,
    IVssFrameworkService,
    IDisposable
  {
    private const string s_layer = "ConnectedServiceProviderService";
    private static readonly string s_area = typeof (ConnectedServiceProviderService).Namespace;
    private IDisposableReadOnlyList<ConnectedServiceProvider> m_providerImpls;
    private Dictionary<string, ConnectedServiceProvider> m_providerImplsById;

    public IReadOnlyList<ConnectedServiceProvider> ProviderImplementations => (IReadOnlyList<ConnectedServiceProvider>) this.m_providerImpls.ToList<ConnectedServiceProvider>();

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1063100, ConnectedServiceProviderService.s_area, nameof (ConnectedServiceProviderService), nameof (ServiceStart));
      try
      {
        this.LoadProviders(systemRequestContext);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1063100, ConnectedServiceProviderService.s_area, nameof (ConnectedServiceProviderService), ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(1063100, ConnectedServiceProviderService.s_area, nameof (ConnectedServiceProviderService), nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IDisposable.Dispose()
    {
      if (this.m_providerImpls == null)
        return;
      this.m_providerImpls.Dispose();
      this.m_providerImpls = (IDisposableReadOnlyList<ConnectedServiceProvider>) null;
    }

    public ConnectedServiceProvider GetProvider(
      IVssRequestContext requestContext,
      string providerId)
    {
      ConnectedServiceProvider provider;
      if (!this.m_providerImplsById.TryGetValue(providerId, out provider))
        throw new VssServiceException(string.Format(ConnectedServiceProviderResources.Error_ProviderNotFoundFormat, (object) providerId));
      return provider;
    }

    public IEnumerable<ConnectedServiceProvider> GetProviders(IVssRequestContext requestContext) => (IEnumerable<ConnectedServiceProvider>) this.m_providerImpls;

    private void LoadProviders(IVssRequestContext systemRequestContext)
    {
      this.m_providerImpls = systemRequestContext.GetExtensions<ConnectedServiceProvider>();
      this.m_providerImplsById = new Dictionary<string, ConnectedServiceProvider>(this.m_providerImpls.Count, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ConnectedServiceProvider providerImpl in (IEnumerable<ConnectedServiceProvider>) this.m_providerImpls)
      {
        if (this.m_providerImplsById.ContainsKey(providerImpl.Id))
          throw new VssServiceException(string.Format(ConnectedServiceProviderResources.Error_DuplicateProviderIdentifierFormat, (object) providerImpl.Id));
        this.m_providerImplsById.Add(providerImpl.Id, providerImpl);
        try
        {
          providerImpl.Start(systemRequestContext);
        }
        catch (Exception ex)
        {
          throw new VssServiceException(string.Format(ConnectedServiceProviderResources.Error_FailedToStartProviderFormat, (object) providerImpl.GetType().AssemblyQualifiedName), ex);
        }
      }
    }
  }
}
