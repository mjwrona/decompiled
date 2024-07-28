// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Controllers.Internal.MavenVersionsInternalController
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Aggregations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.PackageVersionsExposedToDownstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Maven.Server.Controllers.Internal
{
  [ClientInternalUseOnly(true)]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "maven", ResourceName = "versionsInternal", ResourceVersion = 1)]
  public class MavenVersionsInternalController : MavenBaseController
  {
    [HttpGet]
    public async Task<VersionsExposedToDownstreamsResponse> GetPackageVersionsExposedToDownstreamsAsync(
      string feed,
      string groupId,
      string artifactId,
      Guid aadTenantId)
    {
      MavenVersionsInternalController internalController = this;
      IFeedRequest feedRequest = internalController.GetFeedRequest(feed);
      new UpstreamVerificationHelperBootstrapper(internalController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(internalController.TfsRequestContext, feedRequest.Feed, aadTenantId);
      ISecuredObject securedObject = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feedRequest.Feed);
      GetPackageVersionsExposedToDownstreamForApiHandlerBootstrapper<MavenPackageIdentity, MavenPackageName, MavenPackageVersion, IMavenMetadataEntry> singleBootstrapper = new GetPackageVersionsExposedToDownstreamForApiHandlerBootstrapper<MavenPackageIdentity, MavenPackageName, MavenPackageVersion, IMavenMetadataEntry>(new MavenPackageNameRequestConverterBootstrapper(internalController.TfsRequestContext).Bootstrap());
      VersionsExposedToDownstreamsResponse downstreamsResponse = await MavenAggregationResolver.Bootstrap(internalController.TfsRequestContext).HandlerFor<IRawPackageNameRequest, VersionsExposedToDownstreamsResponse>((IRequireAggBootstrapper<IAsyncHandler<IRawPackageNameRequest, VersionsExposedToDownstreamsResponse>>) singleBootstrapper).Handle((IRawPackageNameRequest) new MavenRawPackageNameRequest(feedRequest, groupId, artifactId));
      downstreamsResponse.SetSecuredObject(securedObject);
      VersionsExposedToDownstreamsResponse downstreamsAsync = downstreamsResponse;
      securedObject = (ISecuredObject) null;
      return downstreamsAsync;
    }
  }
}
