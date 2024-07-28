// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.AddFileRequestToMavenPackageFileResponse
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class AddFileRequestToMavenPackageFileResponse : 
    IAsyncHandler<(MavenFileRequest, MavenPackageFileResponse)>,
    IAsyncHandler<(MavenFileRequest, MavenPackageFileResponse), NullResult>,
    IHaveInputType<(MavenFileRequest, MavenPackageFileResponse)>,
    IHaveOutputType<NullResult>
  {
    public Task<NullResult> Handle(
      (MavenFileRequest, MavenPackageFileResponse) request)
    {
      request.Item2.FileRequest = request.Item1;
      return NullResult.NullTask;
    }
  }
}
