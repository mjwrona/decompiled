// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersion.UpdatePackageVersionValidatingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersion
{
  public class UpdatePackageVersionValidatingHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<PackageRequest<NpmPackageIdentity, PackageVersionDetails>, ICommitOperationData>>
  {
    private readonly IVssRequestContext requestContext;

    public UpdatePackageVersionValidatingHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<PackageRequest<NpmPackageIdentity, PackageVersionDetails>, ICommitOperationData> Bootstrap()
    {
      IAsyncHandler<IPackageRequest<NpmPackageIdentity>, INpmMetadataEntry> metadataHandler = new NpmMetadataHandlerBootstrapper(this.requestContext).Bootstrap();
      ViewsValidatingConverter<NpmPackageIdentity, PackageVersionDetails> requestToOpConverter = new ViewsValidatingConverter<NpmPackageIdentity, PackageVersionDetails>((IConverter<JsonPatchOperation, string>) new ViewPatchToViewIdOrNameValidatingConverter(), (IConverter<FeedViewRequest, FeedView>) new ViewIdResolver((IFeedService) new FeedServiceFacade(this.requestContext)));
      return UntilNonNullHandler.Create<PackageRequest<NpmPackageIdentity, PackageVersionDetails>, ICommitOperationData>((IAsyncHandler<PackageRequest<NpmPackageIdentity, PackageVersionDetails>, ICommitOperationData>) new PromoteValidatingHandler<NpmPackageIdentity, PackageVersionDetails, INpmMetadataEntry>((IAsyncHandler<PackageRequest<NpmPackageIdentity>, INpmMetadataEntry>) metadataHandler, (IConverter<PackageRequest<NpmPackageIdentity, PackageVersionDetails>, IViewOperationData>) requestToOpConverter), (IAsyncHandler<PackageRequest<NpmPackageIdentity, PackageVersionDetails>, ICommitOperationData>) new DeprecateValidatingHandler<PackageVersionDetails>((IAsyncHandler<PackageRequest<NpmPackageIdentity>, INpmMetadataEntry>) metadataHandler), (IAsyncHandler<PackageRequest<NpmPackageIdentity, PackageVersionDetails>, ICommitOperationData>) new ThrowWithBadInputMessageHandler<PackageRequest<NpmPackageIdentity, PackageVersionDetails>, ICommitOperationData>((IEnumerable<string>) new string[1]
      {
        "Views"
      }));
    }
  }
}
