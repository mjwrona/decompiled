// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenBatchUpdateBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenBatchUpdateBootstrapper : 
    IBootstrapper<IAsyncHandler<MavenBatchRawRequest, ICommitLogEntry>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenBatchUpdateBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<MavenBatchRawRequest, ICommitLogEntry> Bootstrap() => new MavenBatchRawRequestValidatingConverter((IConverter<IRawPackageRequest, MavenPackageIdentity>) new MavenRawPackageRequestToIdentityConverter(), (IRegistryService) new RegistryServiceFacade(this.requestContext)).ThenDelegateTo<MavenBatchRawRequest, PackagesBatchRequest<MavenPackageIdentity>, ICommitLogEntry>(new MavenWriteBootstrapper<PackagesBatchRequest<MavenPackageIdentity>, BatchCommitOperationData>(this.requestContext, (IRequireAggHandlerBootstrapper<PackagesBatchRequest<MavenPackageIdentity>, BatchCommitOperationData>) new MavenBatchValidatingOpGeneratingHandlerBootstrapper(this.requestContext), (IAsyncHandler<(PackagesBatchRequest<MavenPackageIdentity>, BatchCommitOperationData), ICiData>) new BatchCiDataFacadeHandler<MavenPackageIdentity>(this.requestContext), false).Bootstrap());
  }
}
