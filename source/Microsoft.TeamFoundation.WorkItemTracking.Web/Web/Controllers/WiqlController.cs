// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WiqlController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.Azure.Devops.Work.PlatformServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ValidateModel]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "wiql", ResourceVersion = 2)]
  public class WiqlController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5902000;
    private const string TotalCountParameterName = "X-Total-Count";

    public override string TraceArea => "wiql";

    [TraceFilter(5902000, 5902010)]
    [HttpGet]
    [HttpHead]
    [ClientHeadMethodInfo(MethodName = "GetQueryResultCount", HeaderParameterName = "X-Total-Count", ReturnType = ClientHeadMethodReturnType.Integer)]
    [ClientLocationId("A02355F5-5F8A-4671-8E32-369D23AAC83D")]
    [ClientExample("GET__wit_wiql__queryId_.json", null, null, null)]
    [PublicProjectRequestRestrictions(false, false, "5.0")]
    public WorkItemQueryResult QueryById(Guid id, bool timePrecision = false, [FromUri(Name = "$top")] int top = 2147483647)
    {
      if (Guid.Empty.Equals(id))
        throw new VssPropertyValidationException(nameof (id), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (id)));
      if (top <= 0)
        throw new VssPropertyValidationException("$top", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.QueryParameterOutOfRange((object) "$top"));
      bool flag = this.QueryService.HasCrossProjectQueryPermission(this.TfsRequestContext);
      QueryItem queryById = this.QueryItemService.GetQueryById(this.TfsRequestContext, id, new int?(0), true, filterUnderProjectId: flag ? new Guid?() : new Guid?(this.ProjectId));
      if (!(queryById is Query query))
        throw new QueryException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.FoldersCannotBeQueried());
      Guid? projectId = new Guid?(flag ? (this.ProjectId == Guid.Empty ? query.ProjectId : this.ProjectId) : this.ProjectId);
      WorkItemQueryResult queryResult = this.TfsRequestContext.GetService<PlatformWiqlService>().GetQueryResult(this.TfsRequestContext, query.Wiql, projectId, top, timePrecision, this.Team, this.ExcludeUrls, this.Request, false, new Guid?(queryById.Id));
      HttpContextFactory.Current.Response.Headers.Add("X-Total-Count", (queryResult.QueryResultType == QueryResultType.WorkItem ? queryResult.WorkItems.Count<WorkItemReference>() : queryResult.WorkItemRelations.Count<WorkItemLink>()).ToString());
      return queryResult;
    }

    [TraceFilter(5902010, 5902020)]
    [HttpPost]
    [ClientLocationId("1A9C53F7-F243-4447-B110-35EF023636E4")]
    [ClientExample("GET__wit_get_id_workitems.json", "Get results of a flat work item query.", null, null)]
    public WorkItemQueryResult QueryByWiql(Wiql wiql, [FromUri] bool timePrecision = false, [FromUri(Name = "$top")] int top = 2147483647)
    {
      if (wiql == null)
        throw new VssPropertyValidationException("query", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullQueryParameter());
      if (string.IsNullOrEmpty(wiql.Query))
        throw new VssPropertyValidationException("query", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullQueryParameter());
      if (top <= 0)
        throw new VssPropertyValidationException("$top", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.QueryParameterOutOfRange((object) "$top"));
      return this.TfsRequestContext.GetService<PlatformWiqlService>().GetQueryResult(this.TfsRequestContext, wiql.Query, this.ProjectId == Guid.Empty ? new Guid?() : new Guid?(this.ProjectId), top, timePrecision, this.Team, this.ExcludeUrls, this.Request, false);
    }
  }
}
