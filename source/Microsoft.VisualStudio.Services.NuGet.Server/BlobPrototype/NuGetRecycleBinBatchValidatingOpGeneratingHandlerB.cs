// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.OperationsData;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackagesBatchRequest<VssNuGetPackageIdentity>, BatchCommitOperationData, INuGetMetadataService>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper(
      IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
    }

    protected override IAsyncHandler<PackagesBatchRequest<VssNuGetPackageIdentity>, BatchCommitOperationData> Bootstrap(
      INuGetMetadataService metadataAccessor)
    {
      IAsyncHandler<IPackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry> pointQueryHandler = metadataAccessor.ToPointQueryHandler<VssNuGetPackageIdentity, INuGetMetadataEntry>(true);
      ByFuncConverter<PackagesBatchRequest<VssNuGetPackageIdentity>, PackagesBatchRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>> requestWithDataConverter = new ByFuncConverter<PackagesBatchRequest<VssNuGetPackageIdentity>, PackagesBatchRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>>((Func<PackagesBatchRequest<VssNuGetPackageIdentity>, PackagesBatchRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>>) (r => new PackagesBatchRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>(r, (IRecycleBinPackageVersionDetails) new NuGetRecycleBinPackageVersionDetails()
      {
        Deleted = new bool?(false)
      })));
      return UntilNonNullHandler.Create<PackagesBatchRequest<VssNuGetPackageIdentity>, BatchCommitOperationData>(PackagesBatchRequestOpGeneratingHandler.Create<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails, NuGetRestoreToFeedOperationData>(BatchOperationType.RestoreToFeed, (IConverter<PackagesBatchRequest<VssNuGetPackageIdentity>, PackagesBatchRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>>) requestWithDataConverter, (IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails>, NuGetRestoreToFeedOperationData>) new NuGetRestoreToFeedOperationDataGeneratingHandlerBootstrapper(this.requestContext, (IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry>) pointQueryHandler).Bootstrap()), PackagesBatchRequestOpGeneratingHandler.Create<VssNuGetPackageIdentity, IPermanentDeleteOperationData>(BatchOperationType.PermanentDelete, (IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, IPermanentDeleteOperationData>) new PermanentDeleteValidatingOpGeneratingHandler<VssNuGetPackageIdentity, INuGetMetadataEntry>((IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry>) pointQueryHandler)), (IAsyncHandler<PackagesBatchRequest<VssNuGetPackageIdentity>, BatchCommitOperationData>) new ThrowWithBadOperationMessageHandler<VssNuGetPackageIdentity>());
    }
  }
}
