// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index.CargoIndexController
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "cargo", ResourceName = "index")]
  [ErrorInReasonPhraseExceptionFilter]
  public class CargoIndexController : CargoApiController
  {
    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    public async Task<HttpResponseMessage> GetIndexPackageOnePrefix(
      string feedId,
      string prefix1,
      string packageName)
    {
      return await this.GetIndexPackageTwoPrefix(feedId, prefix1, (string) null, packageName);
    }

    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    public async Task<HttpResponseMessage> GetIndexPackageTwoPrefix(
      string feedId,
      string prefix1,
      string? prefix2,
      string packageName)
    {
      CargoIndexController cargoIndexController = this;
      // ISSUE: reference to a compiler-generated method
      IFactory<IFeedRequest, Task<CargoIndexHandler>> factory = CargoAggregationResolver.Bootstrap(cargoIndexController.TfsRequestContext).FactoryFor<CargoIndexHandler, IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>, IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>>(new Func<IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>, IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>, CargoIndexHandler>(cargoIndexController.\u003CGetIndexPackageTwoPrefix\u003Eb__1_0));
      IPackageNameRequest<CargoPackageName> request = cargoIndexController.GetPackageNameRequest(feedId, packageName);
      IPackageNameRequest<CargoPackageName> input = request;
      string content = await (await factory.Get((IFeedRequest) input)).Handle(request);
      HttpResponseMessage response = cargoIndexController.Request.CreateResponse();
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(request.Feed);
      response.Content = (HttpContent) new VssServerStringContent(content, Encoding.UTF8, (object) securedObjectReadOnly);
      HttpResponseMessage packageTwoPrefix = response;
      request = (IPackageNameRequest<CargoPackageName>) null;
      return packageTwoPrefix;
    }
  }
}
