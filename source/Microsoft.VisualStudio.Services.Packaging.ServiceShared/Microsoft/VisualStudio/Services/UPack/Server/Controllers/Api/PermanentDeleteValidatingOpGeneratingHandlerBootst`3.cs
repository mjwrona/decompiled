// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UPack.Server.Controllers.Api.PermanentDeleteValidatingOpGeneratingHandlerBootstrapper`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.UPack.Server.Controllers.Api
{
  public class PermanentDeleteValidatingOpGeneratingHandlerBootstrapper<TPackageId, TMetadataEntry, TReadMetadataAccessor> : 
    RequireAggHandlerBootstrapper<PackageRequest<TPackageId>, IPermanentDeleteOperationData, TReadMetadataAccessor>
    where TPackageId : IPackageIdentity
    where TMetadataEntry : IMetadataEntry<TPackageId>
    where TReadMetadataAccessor : class, IReadMetadataService<TPackageId, TMetadataEntry>
  {
    protected override IAsyncHandler<PackageRequest<TPackageId>, IPermanentDeleteOperationData> Bootstrap(
      TReadMetadataAccessor metadataAccessor)
    {
      return (IAsyncHandler<PackageRequest<TPackageId>, IPermanentDeleteOperationData>) new PermanentDeleteValidatingOpGeneratingHandler<TPackageId, TMetadataEntry>((IAsyncHandler<PackageRequest<TPackageId>, TMetadataEntry>) metadataAccessor.ToPointQueryHandler<TPackageId, TMetadataEntry>());
    }
  }
}
