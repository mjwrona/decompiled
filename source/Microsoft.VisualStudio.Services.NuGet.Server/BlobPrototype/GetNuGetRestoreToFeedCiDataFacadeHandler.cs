// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.GetNuGetRestoreToFeedCiDataFacadeHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.OperationsData;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class GetNuGetRestoreToFeedCiDataFacadeHandler : 
    IAsyncHandler<(PackageRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails> Request, NuGetRestoreToFeedOperationData Op), ICiData>,
    IHaveInputType<(PackageRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails> Request, NuGetRestoreToFeedOperationData Op)>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;

    public GetNuGetRestoreToFeedCiDataFacadeHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (PackageRequest<VssNuGetPackageIdentity, IRecycleBinPackageVersionDetails> Request, NuGetRestoreToFeedOperationData Op) input)
    {
      VssNuGetPackageIdentity packageId = input.Request.PackageId;
      return Task.FromResult<ICiData>((ICiData) NuGetCiDataFactory.GetNuGetRestoreToFeedCiData(this.requestContext, "none", input.Request.Feed, packageId.Name.DisplayName, packageId.Version.DisplayVersion));
    }
  }
}
