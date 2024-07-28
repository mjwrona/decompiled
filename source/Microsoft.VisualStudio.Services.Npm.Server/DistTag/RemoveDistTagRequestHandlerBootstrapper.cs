// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.DistTag.RemoveDistTagRequestHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.DistTag
{
  public class RemoveDistTagRequestHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageNameRequest<string>, HttpResponseMessage>>
  {
    private readonly IVssRequestContext requestContext;

    public RemoveDistTagRequestHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageNameRequest<string>, HttpResponseMessage> Bootstrap()
    {
      RawPackageNameRequestConverter<NpmPackageName, string> converter = new RawPackageNameRequestConverter<NpmPackageName, string>(new NpmRawPackageNameRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap());
      IAsyncHandler<IPackageNameRequest<NpmPackageName>, IDictionary<string, string>> distTagProvider = NpmAggregationResolver.Bootstrap(this.requestContext).HandlerFor<IPackageNameRequest<NpmPackageName>, IDictionary<string, string>>((IRequireAggBootstrapper<IAsyncHandler<IPackageNameRequest<NpmPackageName>, IDictionary<string, string>>>) new DistTagProviderBootstrapper(this.requestContext));
      RemoveDistTagCiDataFacadeHandler requestToCiHandler = new RemoveDistTagCiDataFacadeHandler(this.requestContext);
      IAsyncHandler<IPackageNameRequest<NpmPackageName, string>, HttpResponseMessage> handler = new NpmWriteBootstrapper<IPackageNameRequest<NpmPackageName, string>, NpmDistTagRemoveOperationData, HttpResponseMessage>(this.requestContext, NoAggregationRequiredReturnSameInstanceBootstrapper.Create<IPackageNameRequest<NpmPackageName, string>, NpmDistTagRemoveOperationData>((IAsyncHandler<IPackageNameRequest<NpmPackageName, string>, NpmDistTagRemoveOperationData>) new RemoveDistTagValidatingOpGeneratingHandler(distTagProvider)), (IAsyncHandler<ICommitLogEntry, HttpResponseMessage>) new ByFuncAsyncHandler<ICommitLogEntry, HttpResponseMessage>((Func<ICommitLogEntry, HttpResponseMessage>) (c => new HttpResponseMessage(HttpStatusCode.Accepted))), (IAsyncHandler<(IPackageNameRequest<NpmPackageName, string>, NpmDistTagRemoveOperationData), ICiData>) requestToCiHandler, false).Bootstrap();
      return (IAsyncHandler<RawPackageNameRequest<string>, HttpResponseMessage>) converter.ThenDelegateTo<IRawPackageNameRequest<string>, IPackageNameRequest<NpmPackageName, string>, HttpResponseMessage>(handler);
    }
  }
}
