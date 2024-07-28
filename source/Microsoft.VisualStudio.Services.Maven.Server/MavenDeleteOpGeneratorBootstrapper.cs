// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenDeleteOpGeneratorBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Upstreams;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenDeleteOpGeneratorBootstrapper : 
    RequireAggHandlerBootstrapper<IPackageRequest<MavenPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData, IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>, IUpstreamVersionListService<MavenPackageName, MavenPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenDeleteOpGeneratorBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<IPackageRequest<MavenPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData> Bootstrap(
      IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry> metadataService,
      IUpstreamVersionListService<MavenPackageName, MavenPackageVersion> upstreamVersionListService)
    {
      return DeleteOpGeneratorBootstrapper.Create<MavenPackageIdentity, IMavenMetadataEntry>(this.requestContext, (IReadMetadataService<MavenPackageIdentity, IMavenMetadataEntry>) new MavenUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, metadataService, upstreamVersionListService).Bootstrap(), (IComparer<IPackageVersion>) new ReverseVersionComparer<MavenPackageVersion>()).Bootstrap();
    }
  }
}
