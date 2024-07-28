// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.PermanentDelete.NpmPermanentDeleteRequestHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.PermanentDelete
{
  public class NpmPermanentDeleteRequestHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest, HttpResponseMessage>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmPermanentDeleteRequestHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest, HttpResponseMessage> Bootstrap()
    {
      IConverter<IRawPackageRequest, NpmPackageIdentity> identityConverter = new NpmRawPackageRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap().Select<IRawPackageRequest, PackageRequest<NpmPackageIdentity>, NpmPackageIdentity>((Func<PackageRequest<NpmPackageIdentity>, NpmPackageIdentity>) (r => r.PackageId));
      PermanentDeleteCiDataFacadeHandler<NpmPackageIdentity> requestToCiHandler = new PermanentDeleteCiDataFacadeHandler<NpmPackageIdentity>(this.requestContext);
      IAsyncHandler<PackageRequest<NpmPackageIdentity>, HttpResponseMessage> handler = new NpmWriteBootstrapper<PackageRequest<NpmPackageIdentity>, IPermanentDeleteOperationData, HttpResponseMessage>(this.requestContext, NoAggregationRequiredReturnSameInstanceBootstrapper.Create<PackageRequest<NpmPackageIdentity>, IPermanentDeleteOperationData>((IAsyncHandler<PackageRequest<NpmPackageIdentity>, IPermanentDeleteOperationData>) new NpmPermanentDeleteValidatingOpGeneratingHandler((IAsyncHandler<PackageRequest<NpmPackageIdentity>, INpmMetadataEntry>) new NpmMetadataHandlerBootstrapper(this.requestContext).Bootstrap())), (IAsyncHandler<ICommitLogEntry, HttpResponseMessage>) new ByFuncAsyncHandler<ICommitLogEntry, HttpResponseMessage>((Func<ICommitLogEntry, HttpResponseMessage>) (c => new HttpResponseMessage(HttpStatusCode.Accepted))), (IAsyncHandler<(PackageRequest<NpmPackageIdentity>, IPermanentDeleteOperationData), ICiData>) requestToCiHandler, false).Bootstrap();
      return (IAsyncHandler<RawPackageRequest, HttpResponseMessage>) new RawPackageRequestConverter<NpmPackageIdentity>(identityConverter).ThenDelegateTo<IRawPackageRequest, PackageRequest<NpmPackageIdentity>, HttpResponseMessage>(handler);
    }
  }
}
