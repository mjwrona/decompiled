// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Controllers.PyPiDownloadController
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.PyPi.Server.Attributes;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Controllers
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "pypi", ResourceName = "download")]
  [ErrorInReasonPhraseExceptionFilter]
  public class PyPiDownloadController : PyPiApiController
  {
    protected override bool ExemptFromGlobalExceptionFormatting { get; } = true;

    [HttpGet]
    [PipNoRedirectPublicProjectRequestRestrictions]
    [PackagingPublicProjectRequestRestrictions]
    public async 
    #nullable disable
    Task<HttpResponseMessage> GetFileAsync(
      string feedId,
      string packageName,
      string packageVersion,
      string fileName)
    {
      PyPiDownloadController downloadController = this;
      HttpResponseMessage fileAsync;
      using (downloadController.EnterTracer(nameof (GetFileAsync)))
      {
        IFeedRequest feed = downloadController.GetFeedRequest(feedId);
        IRequireAggHandlerBootstrapper<RawPackageFileRequest, HttpResponseMessage> handlerBootstrapper1 = (IRequireAggHandlerBootstrapper<RawPackageFileRequest, HttpResponseMessage>) new PyPiBlobDownloadGpgSignatureFileHandlerBootstrapper(downloadController.TfsRequestContext);
        IRequireAggHandlerBootstrapper<RawPackageFileRequest, HttpResponseMessage> handlerBootstrapper2 = (IRequireAggHandlerBootstrapper<RawPackageFileRequest, HttpResponseMessage>) new PyPiBlobDownloadPackageFileHandlerBootstrapper(downloadController.TfsRequestContext);
        IRequireAggHandlerBootstrapper<RawPackageFileRequest, HttpResponseMessage> singleBootstrapper = PyPiDownloadGpgSignatureHandler.IsSignatureFile(fileName) ? handlerBootstrapper1 : handlerBootstrapper2;
        IFactory<IFeedRequest, Task<IAsyncHandler<RawPackageFileRequest, HttpResponseMessage>>> factory = PyPiAggregationResolver.Bootstrap(downloadController.TfsRequestContext).FactoryFor<IAsyncHandler<RawPackageFileRequest, HttpResponseMessage>>((IRequireAggBootstrapper<IAsyncHandler<RawPackageFileRequest, HttpResponseMessage>>) singleBootstrapper);
        RawPackageFileRequest request = new RawPackageFileRequest(feed, packageName, packageVersion, fileName);
        RawPackageFileRequest input = request;
        HttpResponseMessage result = await (await factory.Get((IFeedRequest) input)).Handle(request);
        if (result.Content != null)
        {
          ISecuredObject securedObject = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feed.Feed);
          HttpResponseMessage httpResponseMessage = result;
          httpResponseMessage.Content = (HttpContent) new VssServerStreamContent(await result.Content.ReadAsStreamAsync(), (object) securedObject);
          httpResponseMessage = (HttpResponseMessage) null;
          securedObject = (ISecuredObject) null;
        }
        fileAsync = result;
      }
      return fileAsync;
    }
  }
}
