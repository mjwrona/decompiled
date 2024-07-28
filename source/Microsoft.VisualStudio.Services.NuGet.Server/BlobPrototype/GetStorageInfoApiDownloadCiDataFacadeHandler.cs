// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.GetStorageInfoApiDownloadCiDataFacadeHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Telemetry;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class GetStorageInfoApiDownloadCiDataFacadeHandler : 
    IAsyncHandler<(PackageRequest<VssNuGetPackageIdentity> Request, NuGetStorageInfo Result), ICiData>,
    IHaveInputType<(PackageRequest<VssNuGetPackageIdentity> Request, NuGetStorageInfo Result)>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;
    private readonly ICache<string, object> requestContextItems;
    private const string ProtocolVersion = "storageInfoApi";

    public GetStorageInfoApiDownloadCiDataFacadeHandler(
      IVssRequestContext requestContext,
      ICache<string, object> requestContextItems)
    {
      this.requestContext = requestContext;
      this.requestContextItems = requestContextItems;
    }

    public Task<ICiData> Handle(
      (PackageRequest<VssNuGetPackageIdentity> Request, NuGetStorageInfo Result) input)
    {
      return Task.FromResult<ICiData>((ICiData) NuGetCiDataFactory.GetNuGetDownloadCiData(this.requestContext, "storageInfoApi", input.Request.Feed, input.Request.PackageId.Name.DisplayName, input.Request.PackageId.Version.NormalizedOriginalCaseVersion, long.MinValue, this.GetPackageSource(), input.Result.Package.StorageType.ToTelemetry()));
    }

    private string GetPackageSource()
    {
      object val;
      return !this.requestContextItems.TryGet("Packaging.Properties.PackageSource", out val) ? (string) null : val as string;
    }
  }
}
