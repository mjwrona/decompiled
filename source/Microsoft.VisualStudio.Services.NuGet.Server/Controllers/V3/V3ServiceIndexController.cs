// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V3.V3ServiceIndexController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V3
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "index.json")]
  public class V3ServiceIndexController : NuGetApiController
  {
    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(5721300)]
    public HttpResponseMessage GetFeedsIndex(string feedId)
    {
      NuGetServiceIndexGenerator serviceIndexGenerator = new NuGetServiceIndexGenerator(this.TfsRequestContext.GetLocationFacade(), this.TfsRequestContext.GetExecutionEnvironmentFacade(), (IProvenanceFacade) new ProvenanceFacade(this.TfsRequestContext), FeatureEnabledConstants.MatchUserUriPrefix.Bootstrap(this.TfsRequestContext), FeatureEnabledConstants.EnableNuGetLargePackages.Bootstrap(this.TfsRequestContext), FeatureEnabledConstants.CommitLogController.Bootstrap(this.TfsRequestContext));
      IFeedRequest feedRequest1 = this.GetFeedRequest(feedId);
      IFeedRequest feedRequest2 = feedRequest1;
      ServiceIndex feedIndex = serviceIndexGenerator.GetFeedIndex(feedRequest2);
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feedRequest1.Feed);
      feedIndex.SetSecuredObject(securedObjectReadOnly);
      HttpResponseMessage response = this.Request.CreateResponse<ServiceIndex>(feedIndex);
      if (feedRequest1.Feed.Project != (ProjectReference) null)
        PublicAuthUtils.AddAuthHeadersToResponse(this.TfsRequestContext, response);
      return response;
    }
  }
}
