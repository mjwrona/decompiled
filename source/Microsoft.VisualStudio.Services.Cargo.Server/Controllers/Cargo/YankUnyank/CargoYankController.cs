// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.YankUnyank.CargoYankController
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.ListUnlistPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Threading.Tasks;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.YankUnyank
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "cargo", ResourceName = "yank")]
  [ErrorInReasonPhraseExceptionFilter]
  public class CargoYankController : CargoApiController
  {
    [HttpDelete]
    public async Task<CargoOkSuccessResult> YankAsync(
      string feedId,
      string packageName,
      string packageVersion)
    {
      CargoYankController cargoYankController = this;
      return await CargoYankUnyankControllerHandlerBootstrapper.Bootstrap(cargoYankController.TfsRequestContext).Handle((IPackageRequest<CargoPackageIdentity, ListingOperationRequestAdditionalData>) cargoYankController.GetPackageRequest(feedId, packageName, packageVersion).WithData<CargoPackageIdentity, ListingOperationRequestAdditionalData>(new ListingOperationRequestAdditionalData(ListingDirection.Delist)));
    }
  }
}
