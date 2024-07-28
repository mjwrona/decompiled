// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NonProtocolApis.DeletePackageVersion.NuGetDeleteOpGeneratorBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NonProtocolApis.DeletePackageVersion
{
  public class NuGetDeleteOpGeneratorBootstrapper : 
    RequireAggHandlerBootstrapper<IPackageRequest<VssNuGetPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData, INuGetMetadataService, IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetDeleteOpGeneratorBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<IPackageRequest<VssNuGetPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData> Bootstrap(
      INuGetMetadataService metadataService,
      IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> upstreamVersionListService)
    {
      return DeleteOpGeneratorBootstrapper.Create<VssNuGetPackageIdentity, INuGetMetadataEntry>(this.requestContext, (IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>) new NuGetUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, (IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) metadataService, upstreamVersionListService, true).Bootstrap(), (IComparer<IPackageVersion>) new ReverseVersionComparer<VssNuGetPackageVersion>()).Bootstrap();
    }
  }
}
