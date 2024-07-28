// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.PyPiPackageIngesterBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.PyPi.Server.Ingestion2;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.Telemetry;
using System;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion
{
  public class PyPiPackageIngesterBootstrapper : 
    IBootstrapper<IAsyncHandler<PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>, NullResult>>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiPackageIngesterBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>, NullResult> Bootstrap() => new PyPiWriteBootstrapper<PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>, IAddOperationData, NullResult>(this.requestContext, (IRequireAggHandlerBootstrapper<PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>, IAddOperationData>) new PyPiIngester2HandlerBootstrapper(this.requestContext), (IAsyncHandler<ICommitLogEntry, NullResult>) new ByFuncAsyncHandler<ICommitLogEntry, NullResult>((Func<ICommitLogEntry, NullResult>) (commitLogEntry => (NullResult) null)), (IAsyncHandler<(PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData>, IAddOperationData), ICiData>) new PyPiUploadPackage2CiDataFacadeHandler(this.requestContext), true).Bootstrap();
  }
}
