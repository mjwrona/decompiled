// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Telemetry.PyPiUploadPackage2CiDataFacadeHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.PyPi.Server.Ingestion;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Telemetry
{
  public class PyPiUploadPackage2CiDataFacadeHandler : 
    IAsyncHandler<(PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData> Request, IAddOperationData Op), ICiData>,
    IHaveInputType<(PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData> Request, IAddOperationData Op)>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiUploadPackage2CiDataFacadeHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData> Request, IAddOperationData Op) input)
    {
      (PackageIngestionRequest<PyPiPackageIdentity, PackageIngestionFormData> Request, IAddOperationData Op) = input;
      return Task.FromResult<ICiData>((ICiData) new PushPackageCiData(this.requestContext, (IProtocol) Protocol.PyPi, "1", Request.Feed, Op.Identity.Name.NormalizedName, Op.Identity.Version.NormalizedVersion, Op.PackageSize));
    }
  }
}
