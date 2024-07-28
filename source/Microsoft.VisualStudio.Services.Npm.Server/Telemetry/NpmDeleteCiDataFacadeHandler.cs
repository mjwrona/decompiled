// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Telemetry.NpmDeleteCiDataFacadeHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Telemetry
{
  public class NpmDeleteCiDataFacadeHandler : 
    IAsyncHandler<(IPackageRequest<NpmPackageIdentity, DeleteRequestAdditionalData> Request, IDeleteOperationData Operation), ICiData>,
    IHaveInputType<(IPackageRequest<NpmPackageIdentity, DeleteRequestAdditionalData> Request, IDeleteOperationData Operation)>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;

    public NpmDeleteCiDataFacadeHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (IPackageRequest<NpmPackageIdentity, DeleteRequestAdditionalData> Request, IDeleteOperationData Operation) input)
    {
      IDeleteOperationData operation = input.Operation;
      FeedCore feed = input.Request.Feed;
      return Task.FromResult<ICiData>((ICiData) new DeletePackageCiData(this.requestContext, (IProtocol) Protocol.npm, string.Empty, feed, operation.Identity.Name.NormalizedName, operation.Identity.Version.NormalizedVersion, long.MinValue));
    }
  }
}
