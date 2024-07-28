// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.NuGetPackageSearch3Bootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3.OData;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3
{
  public class NuGetPackageSearch3Bootstrapper : 
    IBootstrapper<IAsyncHandler<FeedRequest<NuGetSearchQueryInputs>, NuGetSearchResultsInfo>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly bool supportSearchingUpstreams;

    public NuGetPackageSearch3Bootstrapper(
      IVssRequestContext requestContext,
      bool supportSearchingUpstreams)
    {
      this.requestContext = requestContext;
      this.supportSearchingUpstreams = supportSearchingUpstreams;
    }

    public IAsyncHandler<FeedRequest<NuGetSearchQueryInputs>, NuGetSearchResultsInfo> Bootstrap()
    {
      V2FeedPackageModelInfo instance = V2FeedPackageModelInfo.Instance;
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      NuGetSearchQueryBuilderConverter queryBuilderConverter = new NuGetSearchQueryBuilderConverter((IFilterRecognizer) new FilterRecognizer(instance), (IOrderByRecognizer) new OrderByRecognizer(instance), tracerFacade);
      IConverter<FeedRequest<NuGetSearchQueryInputs>, FeedRequest<NuGetSearchQuery>> converter = ConvertFrom.InputTypeOf<FeedRequest<NuGetSearchQueryInputs>>((IBootstrapper<IHaveInputType<FeedRequest<NuGetSearchQueryInputs>>>) this).By<FeedRequest<NuGetSearchQueryInputs>, FeedRequest<NuGetSearchQuery>>((Func<FeedRequest<NuGetSearchQueryInputs>, FeedRequest<NuGetSearchQuery>>) (x => new FeedRequest<NuGetSearchQuery>((IFeedRequest) x, queryBuilderConverter.Convert(x.AdditionalData))));
      IAsyncHandler<FeedRequest<NuGetSearchQuery>, NuGetSearchResultsInfo> asyncHandler = NuGetAggregationResolver.Bootstrap(this.requestContext).HandlerFor<FeedRequest<NuGetSearchQuery>, NuGetSearchResultsInfo>((IRequireAggBootstrapper<IAsyncHandler<FeedRequest<NuGetSearchQuery>, NuGetSearchResultsInfo>>) new NaivePackageMetadataSearchHandlerBootstrapper(this.requestContext, this.supportSearchingUpstreams));
      NuGetSearchTelemetryRecorder forwardingToThisHandler = new NuGetSearchTelemetryRecorder(this.requestContext.GetTracerFacade(), this.requestContext.GetFeatureFlagFacade(), (IVersionCountsImplementationMetricsRecorder) new PackagingTracesVersionCountsImplementationMetricsRecorder((IPackagingTraces) new PackagingTracesFacade(this.requestContext)));
      IAsyncHandler<FeedRequest<NuGetSearchQuery>, NuGetSearchResultsInfo> handler = asyncHandler;
      return converter.ThenDelegateTo<FeedRequest<NuGetSearchQueryInputs>, FeedRequest<NuGetSearchQuery>, NuGetSearchResultsInfo>(handler).ThenForwardResultTo<FeedRequest<NuGetSearchQueryInputs>, NuGetSearchResultsInfo>((IAsyncHandler<NuGetSearchResultsInfo>) forwardingToThisHandler);
    }
  }
}
