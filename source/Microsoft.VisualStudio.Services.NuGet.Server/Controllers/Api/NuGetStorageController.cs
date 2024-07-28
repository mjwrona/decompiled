// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api.NuGetStorageController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V3StorageIds;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "storage")]
  public class NuGetStorageController : NuGetApiController
  {
    [HttpGet]
    [FeatureEnabled("NuGet.Service.EnableNuGetLargePackages")]
    [ClientIgnore]
    [ClientResponseType(typeof (NuGetStorageInfo), null, null)]
    public async Task<HttpResponseMessage> GetNuGetPackageContentStorageInfo(
      string feedId,
      string packageName,
      string packageVersion)
    {
      NuGetStorageController storageController = this;
      IFeedRequest feedRequest = storageController.GetFeedRequest(feedId);
      IAsyncHandler<IRawPackageRequest, ContentResult> currentHandler = IngestRawPackageIfNotAlreadyIngestedBootstrapper.Create(storageController.TfsRequestContext, BlockedIdentityContext.Download).Bootstrap();
      IAsyncHandler<IRawPackageRequest, HttpResponseMessage> handler = NuGetAggregationResolver.Bootstrap(storageController.TfsRequestContext).HandlerFor<IRawPackageRequest, HttpResponseMessage>((IRequireAggBootstrapper<IAsyncHandler<IRawPackageRequest, HttpResponseMessage>>) new V3BlobGetPackageStorageIdAsyncHandlerBootstrapper(storageController.TfsRequestContext));
      HttpResponseMessage contentStorageInfo;
      try
      {
        contentStorageInfo = await currentHandler.ThenActuallyHandleWith<IRawPackageRequest, ContentResult, HttpResponseMessage>(handler).Handle((IRawPackageRequest) new RawPackageRequest(feedRequest, packageName, packageVersion));
      }
      catch (Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException ex)
      {
        throw new Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.PackageNotFoundException(ex.Message, (Exception) ex);
      }
      return contentStorageInfo;
    }
  }
}
