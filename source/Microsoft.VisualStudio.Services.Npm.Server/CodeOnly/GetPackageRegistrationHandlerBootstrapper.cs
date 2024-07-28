// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.GetPackageRegistrationHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.DistTag;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Upstreams;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly
{
  public class GetPackageRegistrationHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<RawPackageNameRequest, string, INpmMetadataService, IUpstreamVersionListService<NpmPackageName, SemanticVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public GetPackageRegistrationHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<RawPackageNameRequest, string> Bootstrap(
      INpmMetadataService metadataService,
      IUpstreamVersionListService<NpmPackageName, SemanticVersion> upstreamVersionListService)
    {
      return (IAsyncHandler<RawPackageNameRequest, string>) new NpmRawPackageNameRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap().ThenDelegateTo<IRawPackageNameRequest, IPackageNameRequest<NpmPackageName>, string>((IAsyncHandler<IPackageNameRequest<NpmPackageName>, string>) new GetPackageRegistrationHandler(new NpmUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>) metadataService, upstreamVersionListService).Bootstrap(), (INpmUriBuilder) new NpmUriBuilderFacade(this.requestContext), (IAsyncHandler<IPackageNameRequest<NpmPackageName, MetadataDocument<INpmMetadataEntry>>, IDictionary<string, string>>) new DistTagProvider((IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>) metadataService), this.requestContext.GetTracerFacade()));
    }
  }
}
