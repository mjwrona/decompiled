// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Controllers.PyPiSimpleController
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
using Microsoft.VisualStudio.Services.PyPi.Server.SimpleGetPackage;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Controllers
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "pypi", ResourceName = "simple")]
  [ErrorInReasonPhraseExceptionFilter]
  public class PyPiSimpleController : PyPiApiController
  {
    protected override bool ExemptFromGlobalExceptionFormatting { get; } = true;

    [HttpGet]
    [PipNoRedirectPublicProjectRequestRestrictions]
    [PackagingPublicProjectRequestRestrictions]
    public async 
    #nullable disable
    Task<HttpResponseMessage> GetPackageAsync(string feedId, string packageName)
    {
      PyPiSimpleController simpleController = this;
      using (simpleController.EnterTracer(nameof (GetPackageAsync)))
      {
        Uri input1 = simpleController.TfsRequestContext.RequestUri();
        Uri uri = input1.EnsurePathEndsInSlash();
        if (input1 != uri)
          return new HttpResponseMessage(HttpStatusCode.MovedPermanently)
          {
            Headers = {
              Location = uri
            }
          };
        IFeedRequest feed = simpleController.GetFeedRequest(feedId);
        IFactory<IFeedRequest, Task<IAsyncHandler<RawPackageNameRequest, HttpResponseMessage>>> factory = PyPiAggregationResolver.Bootstrap(simpleController.TfsRequestContext).FactoryFor<IAsyncHandler<RawPackageNameRequest, HttpResponseMessage>>((IRequireAggBootstrapper<IAsyncHandler<RawPackageNameRequest, HttpResponseMessage>>) new PyPiBlobGetSimplePackageHandlerBootstrapper(simpleController.TfsRequestContext));
        RawPackageNameRequest request = new RawPackageNameRequest(feed, packageName);
        RawPackageNameRequest input2 = request;
        HttpResponseMessage result = await (await factory.Get((IFeedRequest) input2)).Handle(request);
        if (result.Content != null)
        {
          ISecuredObject securedObject = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feed.Feed);
          HttpResponseMessage httpResponseMessage = result;
          httpResponseMessage.Content = (HttpContent) new VssServerStreamContent(await result.Content.ReadAsStreamAsync(), (object) securedObject);
          httpResponseMessage = (HttpResponseMessage) null;
          securedObject = (ISecuredObject) null;
        }
        return result;
      }
    }

    [HttpGet]
    public HttpResponseMessage GetFeed(string feedId)
    {
      HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound)
      {
        Content = (HttpContent) new StringContent(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_ApiEndpointNotSupported())
      };
      PublicAuthUtils.AddAuthHeadersToResponse(this.TfsRequestContext, response);
      return response;
    }
  }
}
