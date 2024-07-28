// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ArtifactUriQueryController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters.ArtifactLinkFactories;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "artifactUriQuery", ResourceVersion = 1)]
  [ControllerApiVersion(3.2)]
  [ClientIgnoreRouteScopes(ClientRouteScopes.Project)]
  public class ArtifactUriQueryController : WorkItemTrackingApiController
  {
    private const int c_maxAllowedArtifactUris = 200;
    private const int TraceRange = 5990200;

    [HttpPost]
    [TraceFilter(5990200, 5990210)]
    [ClientResponseType(typeof (ArtifactUriQueryResult), null, null)]
    [ClientExample("POST__wit_artifacturiquery.json", "Query work items for artifacts", null, null)]
    [PublicProjectRequestRestrictions(false, false, "5.0")]
    public HttpResponseMessage QueryWorkItemsForArtifactUris([FromBody] ArtifactUriQuery artifactUriQuery)
    {
      if (artifactUriQuery == null)
        throw new VssPropertyValidationException(nameof (artifactUriQuery), ResourceStrings.NullOrEmptyParameter((object) nameof (artifactUriQuery)));
      if (artifactUriQuery.ArtifactUris == null)
        throw new VssPropertyValidationException("ArtifactUris", ResourceStrings.NullOrEmptyParameter((object) "ArtifactUris"));
      if (artifactUriQuery.ArtifactUris.Count<string>() > 200)
        throw new VssPropertyValidationException("ArtifactUris", ResourceStrings.QueryArtifactLinksExceedingLimit((object) 200));
      ITeamFoundationWorkItemService workItemService = this.WorkItemService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      IEnumerable<string> artifactUris = artifactUriQuery.ArtifactUris;
      Guid? nullableProjectId = this.GetNullableProjectId();
      DateTime? asOfDate = new DateTime?();
      Guid? filterUnderProjectId = nullableProjectId;
      return this.Request.CreateResponse<ArtifactUriQueryResult>(HttpStatusCode.OK, ArtifactUriQueryResultFactory.Create(this.WitRequestContext, workItemService.GetWorkItemIdsForArtifactUris(tfsRequestContext, artifactUris, asOfDate, filterUnderProjectId), this.GetNullableProjectId()));
    }

    public override string TraceArea => "artifactUriQuery";
  }
}
