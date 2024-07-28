// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.CiData.CargoYankCiDataFacadeHandler
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.ListUnlistPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.CiData
{
  public class CargoYankCiDataFacadeHandler : 
    IAsyncHandler<(IPackageRequest<CargoPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData), ICiData>,
    IHaveInputType<(IPackageRequest<CargoPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData)>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;

    public CargoYankCiDataFacadeHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (IPackageRequest<CargoPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData) input)
    {
      (IPackageRequest<CargoPackageIdentity, ListingOperationRequestAdditionalData> packageRequest, IListingStateChangeOperationData changeOperationData) = input;
      ICiData result;
      switch (changeOperationData)
      {
        case IDelistOperationData _:
          result = (ICiData) new UnlistPackageCiData(this.requestContext, (IProtocol) Protocol.Cargo, "1", packageRequest.Feed, changeOperationData.Identity.Name.NormalizedName, changeOperationData.Identity.Version.NormalizedVersion, long.MinValue);
          break;
        case IRelistOperationData _:
          result = (ICiData) new RelistPackageCiData(this.requestContext, (IProtocol) Protocol.Cargo, "1", packageRequest.Feed, changeOperationData.Identity.Name.NormalizedName, changeOperationData.Identity.Version.NormalizedVersion, long.MinValue);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      return Task.FromResult<ICiData>(result);
    }
  }
}
