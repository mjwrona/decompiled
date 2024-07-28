// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.DeletePackageVersion.PyPiDeleteRequestHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.PyPi.Server.Converters.Identity;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.Telemetry;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.DeletePackageVersion
{
  public class PyPiDeleteRequestHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest, Package>>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiDeleteRequestHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest, Package> Bootstrap() => (IAsyncHandler<RawPackageRequest, Package>) ((IConverter<IRawPackageRequest, IPackageRequest<PyPiPackageIdentity>>) new PyPiRawPackageRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap()).ThenConvertBy<IRawPackageRequest, IPackageRequest<PyPiPackageIdentity>, IPackageRequest<PyPiPackageIdentity, DeleteRequestAdditionalData>>((IConverter<IPackageRequest<PyPiPackageIdentity>, IPackageRequest<PyPiPackageIdentity, DeleteRequestAdditionalData>>) new DeletePackageVersionRequestConverter<PyPiPackageIdentity>((ITimeProvider) new DefaultTimeProvider())).ThenDelegateTo<IRawPackageRequest, IPackageRequest<PyPiPackageIdentity, DeleteRequestAdditionalData>, Package>(new PyPiWriteBootstrapper<IPackageRequest<PyPiPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData, Package>(this.requestContext, (IRequireAggHandlerBootstrapper<IPackageRequest<PyPiPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>) new PyPiDeleteOpGeneratorBootstrapper(this.requestContext), (IAsyncHandler<ICommitLogEntry, Package>) new DeleteResultFromCommitEntryHandler(), (IAsyncHandler<(IPackageRequest<PyPiPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData), ICiData>) new PyPiDeleteCiDataFacadeHandler(this.requestContext), false).Bootstrap());
  }
}
