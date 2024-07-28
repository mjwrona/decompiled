// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenDeleteCiDataFacadeHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenDeleteCiDataFacadeHandler : 
    IAsyncHandler<(IPackageRequest<MavenPackageIdentity, DeleteRequestAdditionalData> Request, IDeleteOperationData Operation), ICiData>,
    IHaveInputType<(IPackageRequest<MavenPackageIdentity, DeleteRequestAdditionalData> Request, IDeleteOperationData Operation)>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;

    public MavenDeleteCiDataFacadeHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (IPackageRequest<MavenPackageIdentity, DeleteRequestAdditionalData> Request, IDeleteOperationData Operation) input)
    {
      IDeleteOperationData operation = input.Operation;
      FeedCore feed = input.Request.Feed;
      return Task.FromResult<ICiData>((ICiData) new DeletePackageCiData(this.requestContext, (IProtocol) Protocol.Maven, Protocol.Maven.V1, feed, operation.Identity.Name.NormalizedName, operation.Identity.Version.NormalizedVersion, long.MinValue));
    }
  }
}
