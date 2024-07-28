// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.CiData.CargoDeletePackageCiDataFacadeHandler
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.CiData
{
  public class CargoDeletePackageCiDataFacadeHandler : 
    IAsyncHandler<(IPackageRequest<CargoPackageIdentity, DeleteRequestAdditionalData> Request, IDeleteOperationData Op), ICiData>,
    IHaveInputType<(IPackageRequest<CargoPackageIdentity, DeleteRequestAdditionalData> Request, IDeleteOperationData Op)>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;

    public CargoDeletePackageCiDataFacadeHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (IPackageRequest<CargoPackageIdentity, DeleteRequestAdditionalData> Request, IDeleteOperationData Op) input)
    {
      (IPackageRequest<CargoPackageIdentity, DeleteRequestAdditionalData> Request, IDeleteOperationData Op) = input;
      return Task.FromResult<ICiData>((ICiData) new DeletePackageCiData(this.requestContext, (IProtocol) Protocol.Cargo, "1", Request.Feed, Op.Identity.Name.NormalizedName, Op.Identity.Version.NormalizedVersion, long.MinValue));
    }
  }
}
