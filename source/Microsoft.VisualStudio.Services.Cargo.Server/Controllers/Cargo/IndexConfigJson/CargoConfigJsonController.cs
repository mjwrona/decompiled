// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.IndexConfigJson.CargoConfigJsonController
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Cargo.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net.Http;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.IndexConfigJson
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "cargo", ResourceName = "configJson")]
  [ErrorInReasonPhraseExceptionFilter]
  public class CargoConfigJsonController : CargoApiController
  {
    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    public HttpResponseMessage GetCargoServiceIndex(string feedId)
    {
      FeedCore feed = this.GetFeedRequest(feedId).Feed;
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feed);
      CargoIndexConfigJson feedIndex = this.GetFeedIndex(this.TfsRequestContext, feed);
      feedIndex.SetSecuredObject(securedObjectReadOnly);
      return this.Request.CreateResponse<CargoIndexConfigJson>(feedIndex);
    }

    private CargoIndexConfigJson GetFeedIndex(IVssRequestContext requestContext, FeedCore feed)
    {
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
      string str1 = feed.FullyQualifiedId;
      SessionId sessionId;
      if (ProvenanceUtils.TryGetSessionId(requestContext, out sessionId))
        str1 = sessionId.Name;
      string str2 = this.CreateServiceEntryFromLocationService(requestContext, ResourceIds.CargoDownloadLocationId, (object) new
      {
        feedId = str1,
        packageName = "~crate~",
        packageVersion = "~version~"
      }).Replace("~crate~", "{crate}").Replace("~version~", "{version}");
      string fromLocationService = this.CreateServiceEntryFromLocationService(requestContext, ResourceIds.CargoRootLocationId, (object) new
      {
        feedId = str1
      });
      return new CargoIndexConfigJson()
      {
        DownloadLink = str2,
        ApiUrl = fromLocationService,
        AuthRequired = new bool?(true)
      };
    }

    private string CreateServiceEntryFromLocationService(
      IVssRequestContext requestContext,
      Guid resourceId,
      object routeValues)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "cargo", resourceId, this.ProjectId, routeValues).AbsoluteUri;
    }
  }
}
