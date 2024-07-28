// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.UpdatePackageVersions.PyPiRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.PermanentDelete;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API;
using System;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.UpdatePackageVersions
{
  public class PyPiRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackagesBatchRequest<PyPiPackageIdentity>, BatchCommitOperationData, IPyPiMetadataAggregationAccessor>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper(
      IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
    }

    protected override IAsyncHandler<PackagesBatchRequest<PyPiPackageIdentity>, BatchCommitOperationData> Bootstrap(
      IPyPiMetadataAggregationAccessor metadataAccessor)
    {
      IAsyncHandler<IPackageRequest<PyPiPackageIdentity>, IPyPiMetadataEntry> pointQueryHandler = metadataAccessor.ToPointQueryHandler<PyPiPackageIdentity, IPyPiMetadataEntry>(true);
      RestoreToFeedValidatingOpGeneratingHandler<PyPiPackageIdentity, IPyPiMetadataEntry, IRestoreToFeedOperationData> individualOpGeneratingHandler = new RestoreToFeedValidatingOpGeneratingHandler<PyPiPackageIdentity, IPyPiMetadataEntry, IRestoreToFeedOperationData>((IAsyncHandler<PackageRequest<PyPiPackageIdentity>, IPyPiMetadataEntry>) pointQueryHandler, (IAsyncHandler<PackageRequest<PyPiPackageIdentity>, IRestoreToFeedOperationData>) new RestoreToFeedOpGeneratingHandler<PyPiPackageIdentity>());
      ByFuncConverter<PackagesBatchRequest<PyPiPackageIdentity>, PackagesBatchRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>> requestWithDataConverter = new ByFuncConverter<PackagesBatchRequest<PyPiPackageIdentity>, PackagesBatchRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>>((Func<PackagesBatchRequest<PyPiPackageIdentity>, PackagesBatchRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>>) (r => new PackagesBatchRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>(r, (IRecycleBinPackageVersionDetails) new PyPiRecycleBinPackageVersionDetails()
      {
        Deleted = new bool?(false)
      })));
      return UntilNonNullHandler.Create<PackagesBatchRequest<PyPiPackageIdentity>, BatchCommitOperationData>(PackagesBatchRequestOpGeneratingHandler.Create<PyPiPackageIdentity, IRecycleBinPackageVersionDetails, IRestoreToFeedOperationData>(BatchOperationType.RestoreToFeed, (IConverter<PackagesBatchRequest<PyPiPackageIdentity>, PackagesBatchRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>>) requestWithDataConverter, (IAsyncHandler<PackageRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>) individualOpGeneratingHandler), PackagesBatchRequestOpGeneratingHandler.Create<PyPiPackageIdentity, IPermanentDeleteOperationData>(BatchOperationType.PermanentDelete, (IAsyncHandler<PackageRequest<PyPiPackageIdentity>, IPermanentDeleteOperationData>) new PyPiPermanentDeleteValidatingOpGeneratingHandler((IAsyncHandler<PackageRequest<PyPiPackageIdentity>, IPyPiMetadataEntry>) pointQueryHandler)), (IAsyncHandler<PackagesBatchRequest<PyPiPackageIdentity>, BatchCommitOperationData>) new ThrowWithBadOperationMessageHandler<PyPiPackageIdentity>());
    }
  }
}
