// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.CrossCollectionTenantIdCacheVssService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework
{
  public class CrossCollectionTenantIdCacheVssService : 
    VssMemoryCacheService<Guid, Guid>,
    ICrossCollectionTenantIdCacheVssService,
    IVssFrameworkService
  {
    private static readonly MemoryCacheConfiguration<Guid, Guid> DefaultMemoryCacheConfiguration = new MemoryCacheConfiguration<Guid, Guid>().WithCleanupInterval(TimeSpan.FromMinutes(30.0)).WithExpiryInterval(TimeSpan.FromMinutes(30.0));
    private readonly ConcurrencyConsolidator<Guid, Guid> concurrencyConsolidator = new ConcurrencyConsolidator<Guid, Guid>(false, 2);

    public CrossCollectionTenantIdCacheVssService()
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, CrossCollectionTenantIdCacheVssService.DefaultMemoryCacheConfiguration)
    {
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      base.ServiceStart(systemRequestContext);
    }

    public async Task<Guid> GetTenantIdForCollectionAsync(
      IVssRequestContext requestContext,
      Guid collectionId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CrossCollectionTenantIdCacheVssService idCacheVssService = this;
      requestContext.CheckDeploymentRequestContext();
      Guid guid;
      return idCacheVssService.TryGetValue(requestContext, collectionId, out guid) ? guid : await idCacheVssService.concurrencyConsolidator.RunOnceAsync(collectionId, (Func<Task<Guid>>) (async () =>
      {
        Collection collectionAsync = await (await new CrossCollectionClientCreatorBootstrapper(requestContext).Bootstrap().CreateClientAsync<OrganizationHttpClient>(collectionId, ServiceInstanceTypes.SPS, true, cancellationToken)).GetCollectionAsync("Me", cancellationToken: cancellationToken);
        this.Set(requestContext, collectionId, collectionAsync.TenantId);
        return collectionAsync.TenantId;
      }));
    }
  }
}
