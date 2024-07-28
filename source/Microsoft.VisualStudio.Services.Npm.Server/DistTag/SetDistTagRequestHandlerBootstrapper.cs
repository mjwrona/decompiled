// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.DistTag.SetDistTagRequestHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Net;
using System.Net.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.DistTag
{
  public class SetDistTagRequestHandlerBootstrapper : 
    IBootstrapper<
    #nullable disable
    IAsyncHandler<RawPackageRequest<string>, HttpResponseMessage>>
  {
    private readonly IVssRequestContext requestContext;

    public SetDistTagRequestHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest<string>, HttpResponseMessage> Bootstrap()
    {
      IConverter<RawPackageRequest<string>, PackageRequest<NpmPackageIdentity, string>> converter = new NpmRawPackageRequestWithDataConverterBootstrapper<string>(this.requestContext).Bootstrap();
      SetDistTagCiDataFacadeHandler requestToCiHandler = new SetDistTagCiDataFacadeHandler(this.requestContext);
      IAsyncHandler<PackageRequest<NpmPackageIdentity, string>, HttpResponseMessage> handler = new NpmWriteBootstrapper<PackageRequest<NpmPackageIdentity, string>, NpmDistTagSetOperationData, HttpResponseMessage>(this.requestContext, NoAggregationRequiredReturnSameInstanceBootstrapper.Create<PackageRequest<NpmPackageIdentity, string>, NpmDistTagSetOperationData>(NpmAggregationResolver.Bootstrap(this.requestContext).HandlerFor<PackageRequest<NpmPackageIdentity, string>, NpmDistTagSetOperationData, INpmMetadataService>((Func<INpmMetadataService, IAsyncHandler<PackageRequest<NpmPackageIdentity, string>, NpmDistTagSetOperationData>>) (metadataService => (IAsyncHandler<PackageRequest<NpmPackageIdentity, string>, NpmDistTagSetOperationData>) new SetDistTagValidatingOpGeneratingHandler(metadataService)))), (IAsyncHandler<ICommitLogEntry, HttpResponseMessage>) new ByFuncAsyncHandler<ICommitLogEntry, HttpResponseMessage>((Func<ICommitLogEntry, HttpResponseMessage>) (c => new HttpResponseMessage(HttpStatusCode.Accepted))), (IAsyncHandler<(PackageRequest<NpmPackageIdentity, string>, NpmDistTagSetOperationData), ICiData>) requestToCiHandler, false).Bootstrap();
      return converter.ThenDelegateTo<RawPackageRequest<string>, PackageRequest<NpmPackageIdentity, string>, HttpResponseMessage>(handler);
    }
  }
}
