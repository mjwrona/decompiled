// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers.UpstreamStatusHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9BF15B3F-0578-4452-9C4B-B5237E218DF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.dll

using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers
{
  public class UpstreamStatusHandler : 
    IAsyncHandler<IProtocolAgnosticFeedRequest, IEnumerable<UpstreamHealthStatus>>,
    IHaveInputType<IProtocolAgnosticFeedRequest>,
    IHaveOutputType<IEnumerable<UpstreamHealthStatus>>
  {
    private readonly IUpstreamStatusAnalyzer statusAnalyzer;
    private readonly IFeatureFlagService featureFlagService;

    public UpstreamStatusHandler(
      IUpstreamStatusAnalyzer statusAnalyzer,
      IFeatureFlagService featureFlagService)
    {
      this.statusAnalyzer = statusAnalyzer;
      this.featureFlagService = featureFlagService;
    }

    public async Task<IEnumerable<UpstreamHealthStatus>> Handle(IProtocolAgnosticFeedRequest request) => !request.Feed.UpstreamEnabled || request.Feed.UpstreamSources.Count <= 0 ? (IEnumerable<UpstreamHealthStatus>) null : await this.statusAnalyzer.GetUpstreamStatusesForFeed(request);
  }
}
