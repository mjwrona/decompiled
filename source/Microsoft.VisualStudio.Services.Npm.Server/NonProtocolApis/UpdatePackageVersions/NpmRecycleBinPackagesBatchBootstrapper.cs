// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersions.NpmRecycleBinPackagesBatchBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.UPack.Server;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersions
{
  public class NpmRecycleBinPackagesBatchBootstrapper : 
    IBootstrapper<IAsyncHandler<BatchRawRequest, HttpResponseMessage>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmRecycleBinPackagesBatchBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<BatchRawRequest, HttpResponseMessage> Bootstrap() => (IAsyncHandler<BatchRawRequest, HttpResponseMessage>) new RawBatchPackageRequestValidatingConverter<NpmPackageIdentity>(new NpmRawPackageRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap().Select<IRawPackageRequest, PackageRequest<NpmPackageIdentity>, NpmPackageIdentity>((Func<PackageRequest<NpmPackageIdentity>, NpmPackageIdentity>) (r => r.PackageId)), (IRegistryService) new RegistryServiceFacade(this.requestContext)).ThenDelegateTo<IBatchRawRequest, PackagesBatchRequest<NpmPackageIdentity>, HttpResponseMessage>(new NpmWriteBootstrapper<PackagesBatchRequest<NpmPackageIdentity>, BatchCommitOperationData, HttpResponseMessage>(this.requestContext, NoAggregationRequiredReturnSameInstanceBootstrapper.Create<PackagesBatchRequest<NpmPackageIdentity>, BatchCommitOperationData>(new NpmRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper(this.requestContext).Bootstrap()), (IAsyncHandler<ICommitLogEntry, HttpResponseMessage>) new ByFuncAsyncHandler<ICommitLogEntry, HttpResponseMessage>((Func<ICommitLogEntry, HttpResponseMessage>) (c => new HttpResponseMessage(HttpStatusCode.Accepted))), (IAsyncHandler<(PackagesBatchRequest<NpmPackageIdentity>, BatchCommitOperationData), ICiData>) new BatchCiDataFacadeHandler<NpmPackageIdentity>(this.requestContext), false).Bootstrap());
  }
}
