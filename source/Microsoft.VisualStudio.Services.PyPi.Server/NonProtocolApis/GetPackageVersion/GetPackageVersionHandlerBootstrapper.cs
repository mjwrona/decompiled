// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.GetPackageVersion.GetPackageVersionHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.GetPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.PyPi.Server.Converters.Identity;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.Converters;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.GetPackageVersion
{
  public class GetPackageVersionHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest<ShowDeletedBool>, Package>>
  {
    private readonly IVssRequestContext requestContext;

    public GetPackageVersionHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest<ShowDeletedBool>, Package> Bootstrap() => new RawPackageRequestWithDataConverter<PyPiPackageIdentity, ShowDeletedBool>(new PyPiRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap()).ThenDelegateTo<RawPackageRequest<ShowDeletedBool>, PackageRequest<PyPiPackageIdentity, ShowDeletedBool>, Package>((IAsyncHandler<PackageRequest<PyPiPackageIdentity, ShowDeletedBool>, Package>) new GetPackageVersionHandler<PyPiPackageIdentity, IPyPiMetadataEntry, Package>((IConverter<IPyPiMetadataEntry, Package>) new PyPiMetadataEntryToPackageConverter(), new PyPiMetadataHandlerBootstrapper(this.requestContext).Bootstrap()));
  }
}
