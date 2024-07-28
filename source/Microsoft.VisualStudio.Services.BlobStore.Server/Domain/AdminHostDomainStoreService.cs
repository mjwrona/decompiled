// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Domain.AdminHostDomainStoreService
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
  public class AdminHostDomainStoreService : 
    HostDomainStoreService,
    IAdminDomainStoreService,
    IHostDomainStoreService,
    IVssFrameworkService
  {
    private IAdminHostDomainProvider AdminHostDomainProvider;

    public override void ConfigureService(IVssRequestContext systemRequestContext)
    {
      base.ConfigureService(systemRequestContext);
      this.AdminHostDomainProvider = this.HostDomainProvider as IAdminHostDomainProvider;
    }

    public virtual async Task<IEnumerable<PhysicalDomainInfo>> GetPhysicalDomainsForOrganizationForAdminAsync(
      IVssRequestContext requestContext)
    {
      AdminHostDomainStoreService domainStoreService = this;
      IEnumerable<PhysicalDomainInfo> domainsForAdminAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, domainStoreService.traceData, 5701780, nameof (GetPhysicalDomainsForOrganizationForAdminAsync)))
      {
        if (!requestContext.AllowHostDomainAdminOperations())
          throw new AccountFeatureNotAvailableException("MultiDomain operations are not allowed.");
        if (domainStoreService.AdminHostDomainProvider == null)
          throw new NotImplementedException(domainStoreService.HostDomainProvider.GetType().Name + " does not implement IAdminHostDomainProvider.GetPhysicalDomainsForAdminAsync");
        domainsForAdminAsync = await domainStoreService.AdminHostDomainProvider.GetPhysicalDomainsForAdminAsync(requestContext);
      }
      return domainsForAdminAsync;
    }

    public virtual async Task<IEnumerable<ProjectDomainInfo>> GetProjectDomainsForOrganizationForAdminAsync(
      IVssRequestContext requestContext)
    {
      AdminHostDomainStoreService domainStoreService = this;
      IEnumerable<ProjectDomainInfo> domainsForAdminAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, domainStoreService.traceData, 5701780, nameof (GetProjectDomainsForOrganizationForAdminAsync)))
      {
        if (!requestContext.AllowProjectDomainsForAdminOperations())
          throw new AccountFeatureNotAvailableException("MultiDomain operations are not allowed.");
        if (domainStoreService.AdminHostDomainProvider == null)
          throw new NotImplementedException(domainStoreService.HostDomainProvider.GetType().Name + " does not implement IAdminHostDomainProvider.GetPhysicalDomainsForAdminAsync");
        domainsForAdminAsync = await domainStoreService.AdminHostDomainProvider.GetProjectDomainsForAdminAsync(requestContext);
      }
      return domainsForAdminAsync;
    }
  }
}
