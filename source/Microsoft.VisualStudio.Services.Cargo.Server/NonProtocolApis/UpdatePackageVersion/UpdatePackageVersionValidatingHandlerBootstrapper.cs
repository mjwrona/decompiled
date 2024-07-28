// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.UpdatePackageVersion.UpdatePackageVersionValidatingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Metadata;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.UpdatePackageVersion
{
  public class UpdatePackageVersionValidatingHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<PackageRequest<CargoPackageIdentity, PackageVersionDetails>, ICommitOperationData>>
  {
    private readonly IVssRequestContext requestContext;

    public UpdatePackageVersionValidatingHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<PackageRequest<CargoPackageIdentity, PackageVersionDetails>, ICommitOperationData> Bootstrap() => UntilNonNullHandler.Create<PackageRequest<CargoPackageIdentity, PackageVersionDetails>, ICommitOperationData>((IAsyncHandler<PackageRequest<CargoPackageIdentity, PackageVersionDetails>, ICommitOperationData>) new PromoteValidatingHandler<CargoPackageIdentity, PackageVersionDetails, ICargoMetadataEntry>(new CargoMetadataHandlerBootstrapper(this.requestContext).Bootstrap(), (IConverter<PackageRequest<CargoPackageIdentity, PackageVersionDetails>, IViewOperationData>) new ViewsValidatingConverter<CargoPackageIdentity, PackageVersionDetails>((IConverter<JsonPatchOperation, string>) new ViewPatchToViewIdOrNameValidatingConverter(), (IConverter<FeedViewRequest, FeedView>) new ViewIdResolver((IFeedService) new FeedServiceFacade(this.requestContext)))), (IAsyncHandler<PackageRequest<CargoPackageIdentity, PackageVersionDetails>, ICommitOperationData>) new ThrowWithBadInputMessageHandler<PackageRequest<CargoPackageIdentity, PackageVersionDetails>, ICommitOperationData>((IEnumerable<string>) new string[1]
    {
      "Views"
    }));
  }
}
