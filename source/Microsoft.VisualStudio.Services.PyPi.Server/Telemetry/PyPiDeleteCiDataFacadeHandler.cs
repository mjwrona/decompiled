// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Telemetry.PyPiDeleteCiDataFacadeHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Telemetry
{
  public class PyPiDeleteCiDataFacadeHandler : 
    IAsyncHandler<(IPackageRequest<PyPiPackageIdentity, DeleteRequestAdditionalData> Request, IDeleteOperationData Operation), ICiData>,
    IHaveInputType<(IPackageRequest<PyPiPackageIdentity, DeleteRequestAdditionalData> Request, IDeleteOperationData Operation)>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiDeleteCiDataFacadeHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (IPackageRequest<PyPiPackageIdentity, DeleteRequestAdditionalData> Request, IDeleteOperationData Operation) input)
    {
      IDeleteOperationData operation = input.Operation;
      FeedCore feed = input.Request.Feed;
      return Task.FromResult<ICiData>((ICiData) new DeletePackageCiData(this.requestContext, (IProtocol) Protocol.PyPi, "1", feed, operation.Identity.Name.NormalizedName, operation.Identity.Version.NormalizedVersion, long.MinValue));
    }
  }
}
