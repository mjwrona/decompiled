// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetDeleteRequestHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetDeleteRequestHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<IRawPackageRequest, Package>>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetDeleteRequestHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<IRawPackageRequest, Package> Bootstrap() => ((IConverter<IRawPackageRequest, IPackageRequest<VssNuGetPackageIdentity>>) new NuGetRawPackageRequestConverter(new NuGetRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap())).ThenConvertBy<IRawPackageRequest, IPackageRequest<VssNuGetPackageIdentity>, IPackageRequest<VssNuGetPackageIdentity, DeleteRequestAdditionalData>>((IConverter<IPackageRequest<VssNuGetPackageIdentity>, IPackageRequest<VssNuGetPackageIdentity, DeleteRequestAdditionalData>>) new DeletePackageVersionRequestConverter<VssNuGetPackageIdentity>((ITimeProvider) new DefaultTimeProvider())).ThenDelegateTo<IRawPackageRequest, IPackageRequest<VssNuGetPackageIdentity, DeleteRequestAdditionalData>, Package>(new NuGetWriteBootstrapper<IPackageRequest<VssNuGetPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData, Package>(this.requestContext, (IRequireAggHandlerBootstrapper<IPackageRequest<VssNuGetPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>) new NuGetDeleteOpGeneratorBootstrapper(this.requestContext), (IAsyncHandler<ICommitLogEntry, Package>) new DeleteResultFromCommitEntryHandler(), (IAsyncHandler<(IPackageRequest<VssNuGetPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData), ICiData>) new GetNuGetDeleteCiDataFacadeHandler(this.requestContext), false).Bootstrap());
  }
}
