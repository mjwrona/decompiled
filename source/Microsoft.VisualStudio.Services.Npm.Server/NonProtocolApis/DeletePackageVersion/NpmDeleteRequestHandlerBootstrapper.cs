// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.DeletePackageVersion.NpmDeleteRequestHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Telemetry;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.DeletePackageVersion
{
  public class NpmDeleteRequestHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest, Package>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmDeleteRequestHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest, Package> Bootstrap() => (IAsyncHandler<RawPackageRequest, Package>) ((IConverter<IRawPackageRequest, IPackageRequest<NpmPackageIdentity>>) new NpmRawPackageRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap()).ThenConvertBy<IRawPackageRequest, IPackageRequest<NpmPackageIdentity>, IPackageRequest<NpmPackageIdentity, DeleteRequestAdditionalData>>((IConverter<IPackageRequest<NpmPackageIdentity>, IPackageRequest<NpmPackageIdentity, DeleteRequestAdditionalData>>) new DeletePackageVersionRequestConverter<NpmPackageIdentity>((ITimeProvider) new DefaultTimeProvider())).ThenDelegateTo<IRawPackageRequest, IPackageRequest<NpmPackageIdentity, DeleteRequestAdditionalData>, Package>(new NpmWriteBootstrapper<IPackageRequest<NpmPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData, Package>(this.requestContext, (IRequireAggHandlerBootstrapper<IPackageRequest<NpmPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>) new NpmDeleteOpGeneratorBootstrapper(this.requestContext), (IAsyncHandler<ICommitLogEntry, Package>) new DeleteResultFromCommitEntryHandler(), (IAsyncHandler<(IPackageRequest<NpmPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData), ICiData>) new NpmDeleteCiDataFacadeHandler(this.requestContext), false).Bootstrap());
  }
}
