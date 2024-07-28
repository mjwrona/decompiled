// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ConnectedService.Server.IbizaConnectedServiceProviderService
// Assembly: Microsoft.TeamFoundation.ConnectedService.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEB400C-7A81-4197-B897-D0116BC50257
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ConnectedService.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.ConnectedService.Server
{
  public class IbizaConnectedServiceProviderService : IVssFrameworkService, IDisposable
  {
    private const string s_layer = "IbizaConnectedServiceProvider";
    private static readonly string s_area = typeof (IbizaConnectedServiceProvider).Namespace;
    private IDisposableReadOnlyList<IbizaConnectedServiceProvider> m_providerImpls;
    private Dictionary<string, IbizaConnectedServiceProvider> m_providerImplsById;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1063100, IbizaConnectedServiceProviderService.s_area, "IbizaConnectedServiceProvider", nameof (ServiceStart));
      try
      {
        this.LoadProviders(systemRequestContext);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1063100, IbizaConnectedServiceProviderService.s_area, "IbizaConnectedServiceProvider", ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(1063100, IbizaConnectedServiceProviderService.s_area, "IbizaConnectedServiceProvider", nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    void IDisposable.Dispose()
    {
      if (this.m_providerImpls == null)
        return;
      this.m_providerImpls.Dispose();
      this.m_providerImpls = (IDisposableReadOnlyList<IbizaConnectedServiceProvider>) null;
    }

    public IbizaConnectedServiceProvider GetProvider(string providerId)
    {
      IbizaConnectedServiceProvider provider;
      if (!this.m_providerImplsById.TryGetValue(providerId, out provider))
        throw new VssServiceException(string.Format(ConnectedServiceProviderResources.Error_ProviderNotFoundFormat, (object) providerId));
      return provider;
    }

    private void LoadProviders(IVssRequestContext systemRequestContext)
    {
      this.m_providerImpls = systemRequestContext.GetExtensions<IbizaConnectedServiceProvider>();
      this.m_providerImplsById = new Dictionary<string, IbizaConnectedServiceProvider>(this.m_providerImpls.Count, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (IbizaConnectedServiceProvider providerImpl in (IEnumerable<IbizaConnectedServiceProvider>) this.m_providerImpls)
      {
        if (this.m_providerImplsById.ContainsKey(providerImpl.Id))
          throw new VssServiceException(string.Format(ConnectedServiceProviderResources.Error_DuplicateProviderIdentifierFormat, (object) providerImpl.Id));
        this.m_providerImplsById.Add(providerImpl.Id, providerImpl);
      }
    }
  }
}
