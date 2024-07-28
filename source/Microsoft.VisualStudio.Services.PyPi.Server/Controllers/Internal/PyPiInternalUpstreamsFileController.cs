// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Internal.PyPiInternalUpstreamsFileController
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Internal
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "pypi", ResourceName = "pypiInternalFile", ResourceVersion = 1)]
  public class PyPiInternalUpstreamsFileController : PyPiApiController
  {
    [HttpGet]
    [ClientLocationId("DBB01711-83D6-4794-BA4C-BF5B11CD13C6")]
    [ClientResponseType(typeof (Stream), null, null)]
    public 
    #nullable disable
    Task<HttpResponseMessage> GetFileInternalAsync(
      string feedId,
      string packageName,
      string packageVersion,
      string fileName)
    {
      return this.GetFileInternalAsync(feedId, packageName, packageVersion, fileName);
    }

    [HttpGet]
    [ClientLocationId("DBB01711-83D6-4794-BA4C-BF5B11CD13C6")]
    [ClientResponseType(typeof (Stream), null, null)]
    public async Task<HttpResponseMessage> GetFileInternalAsync(
      string feedId,
      string packageName,
      string packageVersion,
      string fileName,
      Guid aadTenantId)
    {
      PyPiInternalUpstreamsFileController upstreamsFileController = this;
      HttpResponseMessage fileInternalAsync;
      using (upstreamsFileController.EnterTracer(nameof (GetFileInternalAsync)))
      {
        IFeedRequest feedRequest = upstreamsFileController.GetFeedRequest(feedId);
        new UpstreamVerificationHelperBootstrapper(upstreamsFileController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(upstreamsFileController.TfsRequestContext, feedRequest.Feed, aadTenantId);
        IFactory<IFeedRequest, Task<IAsyncHandler<RawPackageFileRequest, HttpResponseMessage>>> factory = PyPiAggregationResolver.Bootstrap(upstreamsFileController.TfsRequestContext).FactoryFor<IAsyncHandler<RawPackageFileRequest, HttpResponseMessage>>((IRequireAggBootstrapper<IAsyncHandler<RawPackageFileRequest, HttpResponseMessage>>) new PyPiBlobDownloadPackageFileHandlerBootstrapper(upstreamsFileController.TfsRequestContext));
        RawPackageFileRequest request = new RawPackageFileRequest(feedRequest, packageName, packageVersion, fileName);
        RawPackageFileRequest input = request;
        fileInternalAsync = await (await factory.Get((IFeedRequest) input)).Handle(request);
      }
      return fileInternalAsync;
    }
  }
}
