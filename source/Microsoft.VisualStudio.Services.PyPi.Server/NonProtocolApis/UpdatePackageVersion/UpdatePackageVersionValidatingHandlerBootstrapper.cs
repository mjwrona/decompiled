// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.UpdatePackageVersion.UpdatePackageVersionValidatingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.UpdatePackageVersion
{
  public class UpdatePackageVersionValidatingHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<PackageRequest<PyPiPackageIdentity, PackageVersionDetails>, ICommitOperationData>>
  {
    private readonly IVssRequestContext requestContext;

    public UpdatePackageVersionValidatingHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<PackageRequest<PyPiPackageIdentity, PackageVersionDetails>, ICommitOperationData> Bootstrap() => UntilNonNullHandler.Create<PackageRequest<PyPiPackageIdentity, PackageVersionDetails>, ICommitOperationData>((IAsyncHandler<PackageRequest<PyPiPackageIdentity, PackageVersionDetails>, ICommitOperationData>) new PromoteValidatingHandler<PyPiPackageIdentity, PackageVersionDetails, IPyPiMetadataEntry>(new PyPiMetadataHandlerBootstrapper(this.requestContext).Bootstrap(), (IConverter<PackageRequest<PyPiPackageIdentity, PackageVersionDetails>, IViewOperationData>) new ViewsValidatingConverter<PyPiPackageIdentity, PackageVersionDetails>((IConverter<JsonPatchOperation, string>) new ViewPatchToViewIdOrNameValidatingConverter(), (IConverter<FeedViewRequest, FeedView>) new ViewIdResolver((IFeedService) new FeedServiceFacade(this.requestContext)))), (IAsyncHandler<PackageRequest<PyPiPackageIdentity, PackageVersionDetails>, ICommitOperationData>) new ThrowWithBadInputMessageHandler<PackageRequest<PyPiPackageIdentity, PackageVersionDetails>, ICommitOperationData>((IEnumerable<string>) new string[1]
    {
      "Views"
    }));
  }
}
