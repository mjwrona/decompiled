// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.CiData.CargoDownloadPackageCiDataFacadeHandler
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.CiData
{
  public class CargoDownloadPackageCiDataFacadeHandler : 
    IAsyncHandler<IPackageFileRequest<CargoPackageIdentity>, ICiData>,
    IHaveInputType<IPackageFileRequest<CargoPackageIdentity>>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;
    private readonly ICache<string, object> requestContextItems;

    public CargoDownloadPackageCiDataFacadeHandler(
      IVssRequestContext requestContext,
      ICache<string, object> requestContextItems)
    {
      this.requestContext = requestContext;
      this.requestContextItems = requestContextItems;
    }

    public Task<ICiData> Handle(IPackageFileRequest<CargoPackageIdentity> request)
    {
      CargoPackageIdentity packageId = request.PackageId;
      return Task.FromResult<ICiData>((ICiData) new DownloadPackageCiData(this.requestContext, (IProtocol) Protocol.Cargo, ProtocolHelpers.NoProtocolVersion, request.Feed, packageId.Name.NormalizedName, packageId.Version.NormalizedVersion, long.MinValue, this.GetPackageSource(), this.GetPackageStorageType(), this.GetDirectUpstreamSource(request.Feed), request.FilePath));
    }

    private string? GetPackageSource()
    {
      object val;
      return !this.requestContextItems.TryGet("Packaging.Properties.PackageSource", out val) ? (string) null : val as string;
    }

    private string? GetPackageStorageType()
    {
      object val;
      return !this.requestContextItems.TryGet("Packaging.Properties.PackageStorageType", out val) ? (string) null : val as string;
    }

    private UpstreamSource? GetDirectUpstreamSource(FeedCore feed)
    {
      object val;
      if (!this.requestContextItems.TryGet("Packaging.Properties.DirectUpstreamSourceId", out val))
        return (UpstreamSource) null;
      if (!(val is Guid guid))
        return (UpstreamSource) null;
      Guid upstreamSourceId = guid;
      IList<UpstreamSource> upstreamSources = feed.UpstreamSources;
      return upstreamSources == null ? (UpstreamSource) null : upstreamSources.FirstOrDefault<UpstreamSource>((Func<UpstreamSource, bool>) (u => u.Id == upstreamSourceId));
    }
  }
}
