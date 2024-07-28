// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.DeletePackageVersion.NpmDeleteOpGeneratorBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Upstreams;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.DeletePackageVersion
{
  public class NpmDeleteOpGeneratorBootstrapper : 
    RequireAggHandlerBootstrapper<IPackageRequest<NpmPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData, INpmMetadataService, IUpstreamVersionListService<NpmPackageName, SemanticVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmDeleteOpGeneratorBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<IPackageRequest<NpmPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData> Bootstrap(
      INpmMetadataService metadataService,
      IUpstreamVersionListService<NpmPackageName, SemanticVersion> upstreamVersionListService)
    {
      return DeleteOpGeneratorBootstrapper.Create<NpmPackageIdentity, INpmMetadataEntry>(this.requestContext, (IReadMetadataService<NpmPackageIdentity, INpmMetadataEntry>) new NpmUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>) metadataService, upstreamVersionListService).Bootstrap(), (IComparer<IPackageVersion>) new ReverseVersionComparer<SemanticVersion>()).Bootstrap();
    }
  }
}
