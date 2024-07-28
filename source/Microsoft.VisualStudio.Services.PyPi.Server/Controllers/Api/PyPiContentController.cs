// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Api.PyPiContentController
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Api
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Python")]
  [VersionedApiControllerCustomName(Area = "pypi", ResourceName = "content", ResourceVersion = 1)]
  public class PyPiContentController : PyPiApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    [PackagingPublicProjectRequestRestrictions]
    public async 
    #nullable disable
    Task<HttpResponseMessage> DownloadPackage(
      string feedId,
      string packageName,
      string packageVersion,
      string fileName)
    {
      PyPiContentController contentController = this;
      HttpResponseMessage httpResponseMessage;
      using (contentController.EnterTracer(nameof (DownloadPackage)))
      {
        IFeedRequest feedRequest = contentController.GetFeedRequest(feedId);
        IFactory<IFeedRequest, Task<IAsyncHandler<RawPackageFileRequest, HttpResponseMessage>>> factory = PyPiAggregationResolver.Bootstrap(contentController.TfsRequestContext).FactoryFor<IAsyncHandler<RawPackageFileRequest, HttpResponseMessage>>((IRequireAggBootstrapper<IAsyncHandler<RawPackageFileRequest, HttpResponseMessage>>) new PyPiBlobDownloadPackageFileHandlerBootstrapper(contentController.TfsRequestContext));
        RawPackageFileRequest request = new RawPackageFileRequest(feedRequest, packageName, packageVersion, fileName);
        RawPackageFileRequest input = request;
        httpResponseMessage = await (await factory.Get((IFeedRequest) input)).Handle(request);
      }
      return httpResponseMessage;
    }
  }
}
