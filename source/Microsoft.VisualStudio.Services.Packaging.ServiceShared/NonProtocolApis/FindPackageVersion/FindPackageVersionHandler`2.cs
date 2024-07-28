// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.FindPackageVersion.FindPackageVersionHandler`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.FindPackageVersion
{
  public class FindPackageVersionHandler<TPackageId, TMetadataEntry> : 
    IAsyncHandler<PackageRequest<TPackageId>, NullResult>,
    IHaveInputType<PackageRequest<TPackageId>>,
    IHaveOutputType<NullResult>
    where TPackageId : IPackageIdentity
    where TMetadataEntry : IMetadataEntry
  {
    private readonly IAsyncHandler<PackageRequest<TPackageId>, TMetadataEntry> metadataService;
    private readonly IFactory<IFeedRequest, Task<IUpstreamMetadataManager>> upstreamMetadataManagerFactory;
    private readonly IPackagingTraces tracer;

    public FindPackageVersionHandler(
      IAsyncHandler<PackageRequest<TPackageId>, TMetadataEntry> metadataService,
      IFactory<IFeedRequest, Task<IUpstreamMetadataManager>> upstreamMetadataManagerFactory,
      IPackagingTraces tracer)
    {
      this.metadataService = metadataService;
      this.upstreamMetadataManagerFactory = upstreamMetadataManagerFactory;
      this.tracer = tracer;
    }

    public async Task<NullResult> Handle(PackageRequest<TPackageId> request)
    {
      TMetadataEntry metadataEntry = await this.metadataService.Handle(request);
      if ((object) metadataEntry == null)
      {
        RefreshPackageResult refreshPackageResult = await (await this.upstreamMetadataManagerFactory.Get((IFeedRequest) request)).RefreshPackageVersionAsync((IFeedRequest) request, (IPackageIdentity) request.PackageId, true);
        metadataEntry = await this.metadataService.Handle(request);
      }
      if ((object) metadataEntry == null || metadataEntry.IsDeleted())
        throw ExceptionHelper.PackageNotFound((IPackageIdentity) request.PackageId, request.Feed);
      return (NullResult) null;
    }
  }
}
