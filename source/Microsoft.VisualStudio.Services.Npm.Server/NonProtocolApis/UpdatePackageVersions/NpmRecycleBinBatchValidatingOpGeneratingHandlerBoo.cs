// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersions.NpmRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.PermanentDelete;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersions
{
  public class NpmRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<PackagesBatchRequest<NpmPackageIdentity>, BatchCommitOperationData>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper(
      IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
    }

    public IAsyncHandler<PackagesBatchRequest<NpmPackageIdentity>, BatchCommitOperationData> Bootstrap()
    {
      IAsyncHandler<IPackageRequest<NpmPackageIdentity>, INpmMetadataEntry> asyncHandler = new NpmMetadataHandlerBootstrapper(this.requestContext).Bootstrap();
      RestoreToFeedValidatingOpGeneratingHandler<NpmPackageIdentity, INpmMetadataEntry, IRestoreToFeedOperationData> individualOpGeneratingHandler = new RestoreToFeedValidatingOpGeneratingHandler<NpmPackageIdentity, INpmMetadataEntry, IRestoreToFeedOperationData>((IAsyncHandler<PackageRequest<NpmPackageIdentity>, INpmMetadataEntry>) asyncHandler, (IAsyncHandler<PackageRequest<NpmPackageIdentity>, IRestoreToFeedOperationData>) new RestoreToFeedOpGeneratingHandler<NpmPackageIdentity>());
      ByFuncConverter<PackagesBatchRequest<NpmPackageIdentity>, PackagesBatchRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>> requestWithDataConverter = new ByFuncConverter<PackagesBatchRequest<NpmPackageIdentity>, PackagesBatchRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>>((Func<PackagesBatchRequest<NpmPackageIdentity>, PackagesBatchRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>>) (r => new PackagesBatchRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>(r, (IRecycleBinPackageVersionDetails) new NpmRecycleBinPackageVersionDetails()
      {
        Deleted = new bool?(false)
      })));
      return UntilNonNullHandler.Create<PackagesBatchRequest<NpmPackageIdentity>, BatchCommitOperationData>(PackagesBatchRequestOpGeneratingHandler.Create<NpmPackageIdentity, IRecycleBinPackageVersionDetails, IRestoreToFeedOperationData>(BatchOperationType.RestoreToFeed, (IConverter<PackagesBatchRequest<NpmPackageIdentity>, PackagesBatchRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>>) requestWithDataConverter, (IAsyncHandler<PackageRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>) individualOpGeneratingHandler), PackagesBatchRequestOpGeneratingHandler.Create<NpmPackageIdentity, IPermanentDeleteOperationData>(BatchOperationType.PermanentDelete, (IAsyncHandler<PackageRequest<NpmPackageIdentity>, IPermanentDeleteOperationData>) new NpmPermanentDeleteValidatingOpGeneratingHandler((IAsyncHandler<PackageRequest<NpmPackageIdentity>, INpmMetadataEntry>) asyncHandler)), (IAsyncHandler<PackagesBatchRequest<NpmPackageIdentity>, BatchCommitOperationData>) new ThrowWithBadOperationMessageHandler<NpmPackageIdentity>());
    }
  }
}
