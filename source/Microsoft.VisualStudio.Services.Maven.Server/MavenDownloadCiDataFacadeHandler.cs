// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenDownloadCiDataFacadeHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenDownloadCiDataFacadeHandler : 
    IAsyncHandler<MavenPackageFileResponse, ICiData>,
    IHaveInputType<MavenPackageFileResponse>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;

    public MavenDownloadCiDataFacadeHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(MavenPackageFileResponse response)
    {
      ICiData result = (ICiData) null;
      if (response.FileRequest.FilePath is IMavenFullyQualifiedFilePath filePath)
        result = !response.FileRequest.RequireContent ? (ICiData) new GetPackageCiData(this.requestContext, (IProtocol) Protocol.Maven, Protocol.Maven.V1, response.FileRequest.Feed, MavenIdentityUtility.GetProtocolSpecificName(filePath.PackageName), filePath.PackageVersion.NormalizedVersion) : (ICiData) new DownloadPackageFileCiData(this.requestContext, (IProtocol) Protocol.Maven, Protocol.Maven.V1, response.FileRequest.Feed, MavenIdentityUtility.GetProtocolSpecificName(filePath.PackageName), filePath.PackageVersion.NormalizedVersion, filePath.FileName, response.ContentSize);
      return Task.FromResult<ICiData>(result);
    }
  }
}
