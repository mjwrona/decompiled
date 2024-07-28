// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry.PermanentDeleteCiDataFacadeHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry
{
  public class PermanentDeleteCiDataFacadeHandler<TPackageId> : 
    IAsyncHandler<(PackageRequest<TPackageId> Request, IPermanentDeleteOperationData Op), ICiData>,
    IHaveInputType<(PackageRequest<TPackageId> Request, IPermanentDeleteOperationData Op)>,
    IHaveOutputType<ICiData>
    where TPackageId : IPackageIdentity
  {
    private readonly IVssRequestContext requestContext;

    public PermanentDeleteCiDataFacadeHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (PackageRequest<TPackageId> Request, IPermanentDeleteOperationData Op) input)
    {
      TPackageId packageId = input.Request.PackageId;
      return Task.FromResult<ICiData>((ICiData) new PermanentDeletePackageCiData(this.requestContext, input.Request.Protocol, ProtocolHelpers.NoProtocolVersion, input.Request.Feed, packageId.Name.NormalizedName, packageId.Version.NormalizedVersion));
    }
  }
}
