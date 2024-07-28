// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.CommitLog.Operations.MavenAddReqToOpHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Converters;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData;
using Microsoft.VisualStudio.Services.Maven.Server.Ingestion;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Upstreams;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;

namespace Microsoft.VisualStudio.Services.Maven.Server.CommitLog.Operations
{
  public class MavenAddReqToOpHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<MavenStreamStorablePackageInfo, MavenCommitOperationData, IMavenMetadataAggregationAccessor, IUpstreamVersionListService<MavenPackageName, MavenPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenAddReqToOpHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<MavenStreamStorablePackageInfo, MavenCommitOperationData> Bootstrap(
      IMavenMetadataAggregationAccessor metadataAccessor,
      IUpstreamVersionListService<MavenPackageName, MavenPackageVersion> upstreamVersionListService)
    {
      IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry> metadataService = new MavenUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>) metadataAccessor, upstreamVersionListService).Bootstrap();
      RegistryServiceFacade registryServiceFacade = new RegistryServiceFacade(this.requestContext);
      IContentBlobStore contentBlobStore = new ContentBlobStoreFacadeBootstrapper(this.requestContext).Bootstrap();
      FileAlreadyPublishedValidatingHandler<MavenPackageIdentity, IMavenMetadataEntry, MavenStreamStorablePackageInfo> validatingHandler1 = new FileAlreadyPublishedValidatingHandler<MavenPackageIdentity, IMavenMetadataEntry, MavenStreamStorablePackageInfo>();
      CurrentStateDelegatingHandler<MavenPackageIdentity, IMavenMetadataEntry, MavenStreamStorablePackageInfo> currentHandler = new CurrentStateDelegatingHandler<MavenPackageIdentity, IMavenMetadataEntry, MavenStreamStorablePackageInfo>((IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry>) metadataService.ToPointQueryHandler<MavenPackageIdentity, IMavenMetadataEntry>(), new IAsyncHandler<(MavenStreamStorablePackageInfo, IMavenMetadataEntry), NullResult>[1]
      {
        (IAsyncHandler<(MavenStreamStorablePackageInfo, IMavenMetadataEntry), NullResult>) validatingHandler1
      });
      NumVersionsOnPublishValidatingHandler<MavenPackageIdentity, IMavenMetadataEntry, MavenStreamStorablePackageInfo> validatingHandler2 = new NumVersionsOnPublishValidatingHandler<MavenPackageIdentity, IMavenMetadataEntry, MavenStreamStorablePackageInfo>((IReadMetadataService<MavenPackageIdentity, IMavenMetadataEntry>) metadataAccessor, PackageIngestionHelper.MaxVersionsPerPackageSettingDefinition.Bootstrap(this.requestContext));
      PutBlobAndReferenceHandler<MavenPackageIdentity, MavenStreamStorablePackageInfo> forwardingToThisHandler1 = new PutBlobAndReferenceHandler<MavenPackageIdentity, MavenStreamStorablePackageInfo>(contentBlobStore, (IConverter<IPackageFileRequest<MavenPackageIdentity>, string>) new MavenFromFileNameRefCalculatingConverter());
      IAsyncHandler<IStorablePackageInfo<MavenPackageIdentity, MavenPackageFileInfo>, MavenCommitOperationData> handler = new MavenCommitOperationDataFromRequestHandlerBootstrapper(this.requestContext).Bootstrap();
      NumVersionsOnPublishValidatingHandler<MavenPackageIdentity, IMavenMetadataEntry, MavenStreamStorablePackageInfo> forwardingToThisHandler2 = validatingHandler2;
      return currentHandler.ThenForwardOriginalRequestTo<MavenStreamStorablePackageInfo, NullResult>((IAsyncHandler<MavenStreamStorablePackageInfo, NullResult>) forwardingToThisHandler2).ThenForwardOriginalRequestTo<MavenStreamStorablePackageInfo, NullResult>((IAsyncHandler<MavenStreamStorablePackageInfo, NullResult>) forwardingToThisHandler1).ThenActuallyHandleWith<MavenStreamStorablePackageInfo, NullResult, MavenCommitOperationData>((IAsyncHandler<MavenStreamStorablePackageInfo, MavenCommitOperationData>) handler);
    }
  }
}
