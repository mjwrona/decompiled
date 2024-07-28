// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.DeletePackageVersion.PyPiDeleteOpGeneratorBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.Upstreams;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.DeletePackageVersion
{
  public class PyPiDeleteOpGeneratorBootstrapper : 
    RequireAggHandlerBootstrapper<IPackageRequest<PyPiPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData, IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>, IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion>>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiDeleteOpGeneratorBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<IPackageRequest<PyPiPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData> Bootstrap(
      IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry> metadataService,
      IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion> upstreamVersionListService)
    {
      return DeleteOpGeneratorBootstrapper.Create<PyPiPackageIdentity, IPyPiMetadataEntry>(this.requestContext, (IReadMetadataService<PyPiPackageIdentity, IPyPiMetadataEntry>) new PyPiUpstreamFetchingMetadataServiceBootstrapper(this.requestContext, metadataService, upstreamVersionListService).Bootstrap(), (IComparer<IPackageVersion>) new ReverseVersionComparer<PyPiPackageVersion>()).Bootstrap();
    }
  }
}
