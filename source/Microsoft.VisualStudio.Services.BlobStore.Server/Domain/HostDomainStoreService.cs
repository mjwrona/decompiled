// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Domain.HostDomainStoreService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Domain
{
  public class HostDomainStoreService : 
    BlobStoreServiceBase,
    IHostDomainStoreService,
    IVssFrameworkService
  {
    protected IHostDomainProvider HostDomainProvider { get; set; }

    protected override string ProductTraceArea => "DomainStoreService";

    public virtual void ConfigureService(IVssRequestContext systemRequestContext)
    {
      int num = systemRequestContext.GetService<IVssRegistryService>().GetValue<string>(systemRequestContext, (RegistryQuery) "/Configuration/BlobStore/HostDomainProviderImplementation", true, "REGISTRYHOSTDOMAINPROVIDER") == "REGISTRYHOSTDOMAINPROVIDER" ? 1 : 0;
      this.HostDomainProvider = (IHostDomainProvider) new RegistryHostDomainProvider();
      this.HostDomainProvider.InitializeAsync(systemRequestContext);
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      this.ConfigureService(systemRequestContext);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      base.ServiceEnd(systemRequestContext);
      this.HostDomainProvider?.Dispose();
    }

    public async Task<IMultiDomainInfo> GetDefaultDomainForOrganizationAsync(
      IVssRequestContext requestContext)
    {
      HostDomainStoreService domainStoreService = this;
      IMultiDomainInfo organizationAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, domainStoreService.traceData, 5701750, nameof (GetDefaultDomainForOrganizationAsync)))
      {
        if (!requestContext.AllowMultiDomainOperations((IDomainId) null))
          throw new AccountFeatureNotAvailableException("MultiDomain operations are not allowed.");
        organizationAsync = await domainStoreService.HostDomainProvider.GetDefaultDomainAsync(requestContext).ConfigureAwait(true);
      }
      return organizationAsync;
    }

    public async Task<IEnumerable<IMultiDomainInfo>> GetDomainsForOrganizationAsync(
      IVssRequestContext requestContext)
    {
      HostDomainStoreService domainStoreService = this;
      IEnumerable<IMultiDomainInfo> organizationAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, domainStoreService.traceData, 5701751, nameof (GetDomainsForOrganizationAsync)))
      {
        if (!requestContext.AllowMultiDomainOperations((IDomainId) null))
          throw new AccountFeatureNotAvailableException("MultiDomain operations are not allowed.");
        organizationAsync = await domainStoreService.HostDomainProvider.GetDomainsAsync(requestContext).ConfigureAwait(true);
      }
      return organizationAsync;
    }

    public async Task<IMultiDomainInfo> GetDomainForOrganizationAsync(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      HostDomainStoreService domainStoreService = this;
      IMultiDomainInfo organizationAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, domainStoreService.traceData, 5701752, nameof (GetDomainForOrganizationAsync)))
      {
        if (!requestContext.AllowMultiDomainOperations(domainId))
          throw new AccountFeatureNotAvailableException("MultiDomain operations are not allowed.");
        organizationAsync = await domainStoreService.HostDomainProvider.GetDomainAsync(requestContext, domainId).ConfigureAwait(true);
      }
      return organizationAsync;
    }

    public async Task<IMultiDomainInfo> CreateProjectDomainsForAdminAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      ByteDomainId physicalDomainId,
      bool isDelete,
      bool forceDelete)
    {
      HostDomainStoreService domainStoreService = this;
      IMultiDomainInfo domainsForAdminAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, domainStoreService.traceData, 5701753, nameof (CreateProjectDomainsForAdminAsync)))
      {
        if (!requestContext.AllowProjectDomainsForAdminOperations())
          throw new AccountFeatureNotAvailableException(Microsoft.VisualStudio.Services.BlobStore.Server.Resources.EnableFeatureFlagError((object) "ProjectDomains"));
        if (!requestContext.IsCollectionAdministrator())
          throw new RestrictedAccessException(Microsoft.VisualStudio.Services.BlobStore.Server.Resources.DirectAccessForbidden());
        domainsForAdminAsync = await domainStoreService.HostDomainProvider.CreateProjectDomainsForAdminAsync(requestContext, projectId, physicalDomainId, isDelete, forceDelete);
      }
      return domainsForAdminAsync;
    }
  }
}
