// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Telemetry.NpmDownloadPackageCiDataFacadeHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Telemetry
{
  public class NpmDownloadPackageCiDataFacadeHandler : 
    IAsyncHandler<IPackageFileRequest<NpmPackageIdentity>, ICiData>,
    IHaveInputType<IPackageFileRequest<NpmPackageIdentity>>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;
    private readonly ICache<string, object> requestContextItems;

    public NpmDownloadPackageCiDataFacadeHandler(
      IVssRequestContext requestContext,
      ICache<string, object> requestContextItems)
    {
      this.requestContext = requestContext;
      this.requestContextItems = requestContextItems;
    }

    public Task<ICiData> Handle(IPackageFileRequest<NpmPackageIdentity> request)
    {
      NpmPackageIdentity packageId = request.PackageId;
      return Task.FromResult<ICiData>((ICiData) new DownloadPackageCiData(this.requestContext, (IProtocol) Protocol.npm, ProtocolHelpers.NoProtocolVersion, request.Feed, packageId.Name.FullName, packageId.Version.NormalizedVersion, long.MinValue, this.GetPackageSource(), this.GetPackageStorageType(), this.GetDirectUpstreamSource(request.Feed), request.FilePath));
    }

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
      Guid? upstreamSourceId = val as Guid?;
      if (!upstreamSourceId.HasValue)
        return (UpstreamSource) null;
      IList<UpstreamSource> upstreamSources = feed.UpstreamSources;
      return upstreamSources == null ? (UpstreamSource) null : upstreamSources.FirstOrDefault<UpstreamSource>((Func<UpstreamSource, bool>) (u =>
      {
        Guid id = u.Id;
        Guid? nullable = upstreamSourceId;
        return nullable.HasValue && id == nullable.GetValueOrDefault();
      }));
    }
  }
}
