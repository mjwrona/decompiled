// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.FrameworkHostDomainStore
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class FrameworkHostDomainStore : 
    ArtifactsServiceBase,
    IHostDomainStoreService,
    IVssFrameworkService
  {
    protected override string ProductTraceArea => "DomainStoreService";

    public Task<IMultiDomainInfo> GetDefaultDomainForOrganizationAsync(
      IVssRequestContext requestContext)
    {
      throw new NotImplementedException();
    }

    public async Task<IMultiDomainInfo> GetDomainForOrganizationAsync(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      return (IMultiDomainInfo) await this.GetHttpClient(requestContext).GetDomainAsync(domainId.Serialize(), requestContext.CancellationToken);
    }

    public async Task<IEnumerable<IMultiDomainInfo>> GetDomainsForOrganizationAsync(
      IVssRequestContext requestContext)
    {
      return (IEnumerable<IMultiDomainInfo>) await this.GetHttpClient(requestContext).GetDomainsAsync(requestContext.CancellationToken);
    }

    private IHostDomainHttpClient GetHttpClient(IVssRequestContext requestContext) => (IHostDomainHttpClient) requestContext.GetClient<HostDomainHttpClient>(ServiceInstanceTypes.BlobStore);

    public async Task<IMultiDomainInfo> CreateProjectDomainsForAdminAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      ByteDomainId physicalDomainId,
      bool isDelete,
      bool forceDelete)
    {
      return await this.GetHttpClient(requestContext).CreateProjectDomainsForAdminAsync(projectId, physicalDomainId.Id.Serialize<byte>(), isDelete, forceDelete, requestContext.CancellationToken);
    }
  }
}
