// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenGetPackageVersionMetadataHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.GetPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenGetPackageVersionMetadataHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<MavenRawPackageRequest<ShowDeletedBool>, IMavenMetadataEntry>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenGetPackageVersionMetadataHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<MavenRawPackageRequest<ShowDeletedBool>, IMavenMetadataEntry> Bootstrap()
    {
      IConverter<RawPackageRequest<ShowDeletedBool>, PackageRequest<MavenPackageIdentity, ShowDeletedBool>> converter = new MavenRawPackageRequestWithDataConverterBootstrapper<ShowDeletedBool>(this.requestContext).Bootstrap();
      IAsyncHandler<PackageRequest<MavenPackageIdentity>, IMavenMetadataEntry> metadataService = new MavenMetadataHandlerBootstrapper(this.requestContext).Bootstrap();
      GetPackageVersionHandler<MavenPackageIdentity, IMavenMetadataEntry, IMavenMetadataEntry> handler = new GetPackageVersionHandler<MavenPackageIdentity, IMavenMetadataEntry, IMavenMetadataEntry>(ByFuncConverter.Identity<IMavenMetadataEntry>(), metadataService);
      return (IAsyncHandler<MavenRawPackageRequest<ShowDeletedBool>, IMavenMetadataEntry>) converter.ThenDelegateTo<RawPackageRequest<ShowDeletedBool>, PackageRequest<MavenPackageIdentity, ShowDeletedBool>, IMavenMetadataEntry>((IAsyncHandler<PackageRequest<MavenPackageIdentity, ShowDeletedBool>, IMavenMetadataEntry>) handler);
    }
  }
}
