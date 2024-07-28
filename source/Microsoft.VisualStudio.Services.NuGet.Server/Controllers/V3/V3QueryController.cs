// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V3.V3QueryController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation;
using Microsoft.VisualStudio.Services.NuGet.Server.Telemetry;
using Microsoft.VisualStudio.Services.NuGet.Server.Utils;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V3
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "query2")]
  public class V3QueryController : NuGetApiController
  {
    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(5722700)]
    public async Task<QueryResult> ExecuteQueryAsync(
      string feedId,
      string q = "",
      int skip = 0,
      int take = 20,
      bool prerelease = false,
      string semVerLevel = "1.0.0")
    {
      V3QueryController v3QueryController = this;
      IFeedRequest feedRequest = v3QueryController.GetFeedRequest(feedId);
      if (skip < 0)
        throw new InvalidUserRequestException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_ArgumentOutOfRange((object) nameof (skip)));
      if (take < 0 || take > 10000)
        throw new InvalidUserRequestException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_ArgumentOutOfRange((object) nameof (take)));
      v3QueryController.PublishQueryTelemetry(feedRequest.Feed);
      FeedRequest<NuGetSearchQueryInputs> request = new FeedRequest<NuGetSearchQueryInputs>(feedRequest, new NuGetSearchQueryInputs(q, (string) null, (string) null, false, prerelease, semVerLevel, skip, take, NuGetSearchResultShape.Packages, true));
      NuGetExtractInnerFileFromNupkgUriCalculator innerFileUriCalculator = new NuGetExtractInnerFileFromNupkgUriCalculator(v3QueryController.TfsRequestContext.GetLocationFacade());
      IAsyncHandler<FeedRequest<NuGetSearchQueryInputs>, NuGetSearchResultsInfo> handler1 = new NuGetPackageSearch3Bootstrapper(v3QueryController.TfsRequestContext, true).Bootstrap();
      V3QueryResultBuilderHandler handler2 = new V3QueryResultBuilderHandler((INuGetIconUriCalculator) new NuGetIconUriCalculator((INuGetExtractInnerFileFromNupkgUriCalculator) innerFileUriCalculator), (INuGetLicenseUriCalculator) new NuGetLicenseUriCalculator((INuGetExtractInnerFileFromNupkgUriCalculator) innerFileUriCalculator));
      QueryResult queryResult1 = await handler1.KeepInput<FeedRequest<NuGetSearchQueryInputs>, NuGetSearchResultsInfo, IFeedRequest<NuGetSearchResultsInfo>>((Func<FeedRequest<NuGetSearchQueryInputs>, NuGetSearchResultsInfo, IFeedRequest<NuGetSearchResultsInfo>>) ((searchRequest, response) => searchRequest.WithData<NuGetSearchResultsInfo>(response))).ThenDelegateTo<FeedRequest<NuGetSearchQueryInputs>, IFeedRequest<NuGetSearchResultsInfo>, QueryResult>((IAsyncHandler<IFeedRequest<NuGetSearchResultsInfo>, QueryResult>) handler2).Handle(request);
      queryResult1.SetSecuredObject(FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feedRequest.Feed));
      v3QueryController.TfsRequestContext.UpdateTimeToFirstPage();
      QueryResult queryResult2 = queryResult1;
      feedRequest = (IFeedRequest) null;
      return queryResult2;
    }

    protected virtual void PublishQueryTelemetry(FeedCore feed)
    {
      QueryPackageCiData nuGetQueryCiData = NuGetCiDataFactory.GetNuGetQueryCiData(this.TfsRequestContext, "v3", feed);
      new NuGetPackagingTelemetryBuilder().Build(this.TfsRequestContext).Publish(this.TfsRequestContext, (ICiData) nuGetQueryCiData);
    }
  }
}
