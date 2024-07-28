// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NupkgNameValidator
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.Controllers;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NupkgNameValidator : 
    IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage>,
    IHaveInputType<IPackageFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>>,
    IHaveOutputType<HttpResponseMessage>
  {
    public async Task<HttpResponseMessage> Handle(
      IPackageFileRequest<VssNuGetPackageIdentity, NuGetGetFileData> request)
    {
      string expectedFileName = this.GetNupkgExpectedFileName(request.PackageId);
      if (!request.FilePath.ToLowerInvariant().Equals(expectedFileName))
        throw ControllerExceptionHelper.PackageSubresourceNotFound(request.FilePath, request.PackageId, expectedFileName);
      return await Task.FromResult<HttpResponseMessage>((HttpResponseMessage) null);
    }

    private string GetNupkgExpectedFileName(VssNuGetPackageIdentity packageIdentity) => string.Format("{0}.{1}{2}", (object) packageIdentity.Name.NormalizedName, (object) packageIdentity.Version.NormalizedVersion, (object) ".nupkg");
  }
}
