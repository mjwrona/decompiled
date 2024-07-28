// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Telemetry.PyPiDownloadPackageCiDataFacadeHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Telemetry
{
  public class PyPiDownloadPackageCiDataFacadeHandler : 
    IAsyncHandler<IPackageFileRequest<PyPiPackageIdentity>, ICiData>,
    IHaveInputType<IPackageFileRequest<PyPiPackageIdentity>>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;
    private readonly ICache<string, object> requestContextItems;

    public PyPiDownloadPackageCiDataFacadeHandler(
      IVssRequestContext requestContext,
      ICache<string, object> requestContextItems)
    {
      this.requestContext = requestContext;
      this.requestContextItems = requestContextItems;
    }

    public Task<ICiData> Handle(IPackageFileRequest<PyPiPackageIdentity> request) => Task.FromResult<ICiData>((ICiData) new DownloadPackageCiData(this.requestContext, (IProtocol) Protocol.PyPi, "1", request.Feed, request.PackageId.Name.NormalizedName, request.PackageId.Version.NormalizedVersion, long.MinValue, this.GetPackageSource(), this.GetPackageStorageType(), this.GetDirectUpstreamSource(request.Feed), request.FilePath));

    private string GetPackageSource()
    {
      object val;
      return !this.requestContextItems.TryGet("Packaging.Properties.PackageSource", out val) ? (string) null : val as string;
    }

    private string GetPackageStorageType()
    {
      object val;
      return !this.requestContextItems.TryGet("Packaging.Properties.PackageStorageType", out val) ? (string) null : val as string;
    }

    private UpstreamSource GetDirectUpstreamSource(FeedCore feed)
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
