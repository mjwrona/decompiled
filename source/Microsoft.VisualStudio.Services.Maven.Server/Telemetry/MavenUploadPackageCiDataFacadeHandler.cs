// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Telemetry.MavenUploadPackageCiDataFacadeHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData;
using Microsoft.VisualStudio.Services.Maven.Server.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Telemetry
{
  public class MavenUploadPackageCiDataFacadeHandler : 
    IAsyncHandler<(MavenStreamStorablePackageInfo Request, MavenCommitOperationData Op), ICiData>,
    IHaveInputType<(MavenStreamStorablePackageInfo Request, MavenCommitOperationData Op)>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;

    public MavenUploadPackageCiDataFacadeHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (MavenStreamStorablePackageInfo Request, MavenCommitOperationData Op) input)
    {
      MavenStreamStorablePackageInfo request = input.Request;
      return Task.FromResult<ICiData>((ICiData) new PushPackageCiData(this.requestContext, (IProtocol) Protocol.Maven, Protocol.Maven.V1, request.Feed, request.PackageId.Name.NormalizedName, request.PackageId.Version.NormalizedVersion, request.PackageSize));
    }
  }
}
