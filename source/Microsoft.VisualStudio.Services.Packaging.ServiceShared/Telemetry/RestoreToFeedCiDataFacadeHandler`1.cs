// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry.RestoreToFeedCiDataFacadeHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry
{
  public class RestoreToFeedCiDataFacadeHandler<TPackageId> : 
    IAsyncHandler<(PackageRequest<TPackageId, IRecycleBinPackageVersionDetails> Request, IRestoreToFeedOperationData Op), ICiData>,
    IHaveInputType<(PackageRequest<TPackageId, IRecycleBinPackageVersionDetails> Request, IRestoreToFeedOperationData Op)>,
    IHaveOutputType<ICiData>
    where TPackageId : IPackageIdentity
  {
    private readonly IVssRequestContext requestContext;

    public RestoreToFeedCiDataFacadeHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (PackageRequest<TPackageId, IRecycleBinPackageVersionDetails> Request, IRestoreToFeedOperationData Op) input)
    {
      IVssRequestContext requestContext = this.requestContext;
      IProtocol protocol = input.Request.Protocol;
      string noProtocolVersion = ProtocolHelpers.NoProtocolVersion;
      FeedCore feed = input.Request.Feed;
      TPackageId packageId = input.Request.PackageId;
      string normalizedName = packageId.Name.NormalizedName;
      packageId = input.Request.PackageId;
      string normalizedVersion = packageId.Version.NormalizedVersion;
      return Task.FromResult<ICiData>((ICiData) new RestoreToFeedPackageCiData(requestContext, protocol, noProtocolVersion, feed, normalizedName, normalizedVersion));
    }
  }
}
