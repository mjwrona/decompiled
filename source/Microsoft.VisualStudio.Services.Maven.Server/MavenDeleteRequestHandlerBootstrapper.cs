// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenDeleteRequestHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenDeleteRequestHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<MavenRawPackageRequest, ICommitLogEntry>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenDeleteRequestHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<MavenRawPackageRequest, ICommitLogEntry> Bootstrap() => (IAsyncHandler<MavenRawPackageRequest, ICommitLogEntry>) ((IConverter<IRawPackageRequest, IPackageRequest<MavenPackageIdentity>>) new MavenRawPackageRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap()).ThenConvertBy<IRawPackageRequest, IPackageRequest<MavenPackageIdentity>, IPackageRequest<MavenPackageIdentity, DeleteRequestAdditionalData>>((IConverter<IPackageRequest<MavenPackageIdentity>, IPackageRequest<MavenPackageIdentity, DeleteRequestAdditionalData>>) new DeletePackageVersionRequestConverter<MavenPackageIdentity>((ITimeProvider) new DefaultTimeProvider())).ThenDelegateTo<IRawPackageRequest, IPackageRequest<MavenPackageIdentity, DeleteRequestAdditionalData>, ICommitLogEntry>(new MavenWriteBootstrapper<IPackageRequest<MavenPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>(this.requestContext, (IRequireAggHandlerBootstrapper<IPackageRequest<MavenPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>) new MavenDeleteOpGeneratorBootstrapper(this.requestContext), (IAsyncHandler<(IPackageRequest<MavenPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData), ICiData>) new MavenDeleteCiDataFacadeHandler(this.requestContext), false).Bootstrap());
  }
}
