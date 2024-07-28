// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenUpdateValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenUpdateValidatingOpGeneratingHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackageRequest<MavenPackageIdentity, PackageVersionDetails>, ICommitOperationData, IMavenMetadataAggregationAccessor>
  {
    private readonly IVssRequestContext requestContext;

    public MavenUpdateValidatingOpGeneratingHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<PackageRequest<MavenPackageIdentity, PackageVersionDetails>, ICommitOperationData> Bootstrap(
      IMavenMetadataAggregationAccessor metadataAccessor)
    {
      return UntilNonNullHandler.Create<PackageRequest<MavenPackageIdentity, PackageVersionDetails>, ICommitOperationData>((IAsyncHandler<PackageRequest<MavenPackageIdentity, PackageVersionDetails>, ICommitOperationData>) new PromoteValidatingHandler<MavenPackageIdentity, PackageVersionDetails, IMavenMetadataEntry>((IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry>) metadataAccessor.ToPointQueryHandler<MavenPackageIdentity, IMavenMetadataEntry>(), (IConverter<PackageRequest<MavenPackageIdentity, PackageVersionDetails>, IViewOperationData>) new ViewsValidatingConverter<MavenPackageIdentity, PackageVersionDetails>((IConverter<JsonPatchOperation, string>) new ViewPatchToViewIdOrNameValidatingConverter(), (IConverter<FeedViewRequest, FeedView>) new ViewIdResolver((IFeedService) new FeedServiceFacade(this.requestContext)))), (IAsyncHandler<PackageRequest<MavenPackageIdentity, PackageVersionDetails>, ICommitOperationData>) new ThrowWithBadInputMessageHandler<PackageRequest<MavenPackageIdentity, PackageVersionDetails>, ICommitOperationData>((IEnumerable<string>) new string[1]
      {
        "Views"
      }));
    }
  }
}
