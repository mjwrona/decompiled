// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion.DeleteOpGeneratorBootstrapper`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion
{
  public class DeleteOpGeneratorBootstrapper<TPackageIdentity, TMetadataEntry> : 
    IBootstrapper<IAsyncHandler<IPackageRequest<TPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>>
    where TPackageIdentity : IPackageIdentity
    where TMetadataEntry : IMetadataEntry<TPackageIdentity>
  {
    private readonly IReadMetadataService<TPackageIdentity, TMetadataEntry> metadataService;
    private readonly IVssRequestContext requestContext;
    private readonly IComparer<IPackageVersion> reverseVersionComparer;

    public DeleteOpGeneratorBootstrapper(
      IVssRequestContext requestContext,
      IReadMetadataService<TPackageIdentity, TMetadataEntry> metadataService,
      IComparer<IPackageVersion> reverseVersionComparer)
    {
      this.requestContext = requestContext;
      this.metadataService = metadataService;
      this.reverseVersionComparer = reverseVersionComparer;
    }

    public IAsyncHandler<IPackageRequest<TPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData> Bootstrap() => (IAsyncHandler<IPackageRequest<TPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>) new DeleteOpGenerator<TPackageIdentity, TMetadataEntry>(this.metadataService.BulkFetchForSingleVersion<TPackageIdentity, TMetadataEntry>(this.reverseVersionComparer), new ScheduledPermanentDeleteDateCalculatingHandlerBootstrapper(this.requestContext).Bootstrap());
  }
}
