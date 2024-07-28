// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Readme.GetReadmeHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.IO;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Readme
{
  public class GetReadmeHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<RawPackageRequest, Stream, INpmMetadataService>
  {
    private readonly IVssRequestContext requestContext;

    public GetReadmeHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<RawPackageRequest, Stream> Bootstrap(INpmMetadataService agg1) => (IAsyncHandler<RawPackageRequest, Stream>) new NpmRawPackageRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap().ThenDelegateTo<IRawPackageRequest, PackageRequest<NpmPackageIdentity>, Stream>((IAsyncHandler<PackageRequest<NpmPackageIdentity>, Stream>) new GetReadmeHandler((IAsyncHandler<IPackageRequest<NpmPackageIdentity>, BlobIdentifier>) new GetReadmeBlobIdentifierHandler(agg1), new ContentBlobStoreFacadeBootstrapper(this.requestContext).Bootstrap()));
  }
}
