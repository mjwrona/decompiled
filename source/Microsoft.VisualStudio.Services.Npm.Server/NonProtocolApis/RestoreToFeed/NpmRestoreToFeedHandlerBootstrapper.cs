// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.RestoreToFeed.NpmRestoreToFeedHandlerBootstrapper
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
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.RestoreToFeed
{
  public class NpmRestoreToFeedHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest<IRecycleBinPackageVersionDetails>, HttpResponseMessage>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmRestoreToFeedHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest<IRecycleBinPackageVersionDetails>, HttpResponseMessage> Bootstrap()
    {
      IConverter<RawPackageRequest<IRecycleBinPackageVersionDetails>, PackageRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>> converter = new NpmRawPackageRequestWithDataConverterBootstrapper<IRecycleBinPackageVersionDetails>(this.requestContext).Bootstrap();
      RestoreToFeedCiDataFacadeHandler<NpmPackageIdentity> requestToCiHandler = new RestoreToFeedCiDataFacadeHandler<NpmPackageIdentity>(this.requestContext);
      IAsyncHandler<PackageRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>, HttpResponseMessage> handler = new NpmWriteBootstrapper<PackageRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData, HttpResponseMessage>(this.requestContext, NoAggregationRequiredReturnSameInstanceBootstrapper.Create<PackageRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>(new NpmRestoreToFeedValidatingOpGeneratingHandlerBootstrapper(this.requestContext).Bootstrap()), (IAsyncHandler<ICommitLogEntry, HttpResponseMessage>) new ByFuncAsyncHandler<ICommitLogEntry, HttpResponseMessage>((Func<ICommitLogEntry, HttpResponseMessage>) (c => new HttpResponseMessage(HttpStatusCode.Accepted))), (IAsyncHandler<(PackageRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData), ICiData>) requestToCiHandler, false).Bootstrap();
      return converter.ThenDelegateTo<RawPackageRequest<IRecycleBinPackageVersionDetails>, PackageRequest<NpmPackageIdentity, IRecycleBinPackageVersionDetails>, HttpResponseMessage>(handler);
    }
  }
}
