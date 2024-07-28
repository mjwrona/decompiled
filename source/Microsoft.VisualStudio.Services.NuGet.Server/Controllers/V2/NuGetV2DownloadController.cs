// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2.NuGetV2DownloadController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2
{
  [ClientIgnore]
  public class NuGetV2DownloadController : NuGetApiController
  {
    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    public async Task<HttpResponseMessage> GetNupkgAsync(string feedId, string id, string version)
    {
      NuGetV2DownloadController downloadController = this;
      IFeedRequest feedRequest = downloadController.GetFeedRequest(feedId);
      return await new GetFileAsyncBootstrapper(downloadController.TfsRequestContext).Bootstrap().Handle((IRawPackageInnerFileRequest<NuGetGetFileData>) new RawPackageInnerFileRequest<NuGetGetFileData>(feedRequest, id, version, id + "." + version + ".nupkg", (string) null, new NuGetGetFileData("v2")));
    }
  }
}
