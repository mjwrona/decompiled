// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.SameAadTenantValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public class SameAadTenantValidatingHandler : 
    IAsyncHandler<UpstreamSource>,
    IAsyncHandler<UpstreamSource, NullResult>,
    IHaveInputType<UpstreamSource>,
    IHaveOutputType<NullResult>
  {
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IUrlHostResolutionService urlHostResolutionService;
    private readonly ICrossCollectionTenantIdService tenantIdService;

    public SameAadTenantValidatingHandler(
      IExecutionEnvironment executionEnvironment,
      IUrlHostResolutionService urlHostResolutionService,
      ICrossCollectionTenantIdService tenantIdService)
    {
      this.executionEnvironment = executionEnvironment;
      this.urlHostResolutionService = urlHostResolutionService;
      this.tenantIdService = tenantIdService;
    }

    public async Task<NullResult> Handle(UpstreamSource upstreamSource)
    {
      Guid guid = upstreamSource.InternalUpstreamCollectionId.Value;
      if (this.urlHostResolutionService.GetHostUri(guid, ServiceInstanceTypes.SPS) == (Uri) null)
        throw new UpstreamOrganizationNotAccessibleException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InternalUpstreamSourceNotFound((object) upstreamSource.Name));
      if (upstreamSource.GetProjectId().HasValue)
        return (NullResult) null;
      Guid forCollectionAsync = await this.tenantIdService.GetTenantIdForCollectionAsync(guid);
      bool flag = this.executionEnvironment.IsOrganizationAadBacked();
      Guid organizationAadTenantId = this.executionEnvironment.GetOrganizationAadTenantId();
      if (!flag || organizationAadTenantId != forCollectionAsync)
        throw new UpstreamOrganizationNotAccessibleException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InternalUpstreamSourceNotFound((object) upstreamSource.Name));
      return (NullResult) null;
    }
  }
}
