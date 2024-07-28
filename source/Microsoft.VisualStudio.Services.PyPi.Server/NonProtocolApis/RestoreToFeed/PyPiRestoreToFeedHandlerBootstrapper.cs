// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.RestoreToFeed.PyPiRestoreToFeedHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using Microsoft.VisualStudio.Services.PyPi.Server.Converters.Identity;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.RestoreToFeed
{
  public class PyPiRestoreToFeedHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest<IRecycleBinPackageVersionDetails>, HttpResponseMessage>>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiRestoreToFeedHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest<IRecycleBinPackageVersionDetails>, HttpResponseMessage> Bootstrap() => new PyPiRawPackageRequestWithDataConverterBootstrapper<IRecycleBinPackageVersionDetails>(this.requestContext).Bootstrap().ThenDelegateTo<RawPackageRequest<IRecycleBinPackageVersionDetails>, PackageRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>, HttpResponseMessage>(new PyPiWriteBootstrapper<PackageRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData, HttpResponseMessage>(this.requestContext, (IRequireAggHandlerBootstrapper<PackageRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>) new PyPiRestoreToFeedValidatingOpGeneratingHandlerBootstrapper(), (IAsyncHandler<ICommitLogEntry, HttpResponseMessage>) new ByFuncAsyncHandler<ICommitLogEntry, HttpResponseMessage>((Func<ICommitLogEntry, HttpResponseMessage>) (c => new HttpResponseMessage(HttpStatusCode.Accepted))), (IAsyncHandler<(PackageRequest<PyPiPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData), ICiData>) new RestoreToFeedCiDataFacadeHandler<PyPiPackageIdentity>(this.requestContext), false).Bootstrap());
  }
}
