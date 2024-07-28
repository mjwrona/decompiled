// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.ReorderWorkItemsController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.Agile.Server.Reorder;
using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Server.Exceptions;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "workitemsorder")]
  [ControllerApiVersion(5.1)]
  public class ReorderWorkItemsController : TfsTeamApiController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ReorderWorkItemsInvalidOperationsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BacklogChangedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidHierarchyException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidReorderOperationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidIterationIdException>(HttpStatusCode.BadRequest);
    }

    [HttpPatch]
    [ClientLocationId("1c22b714-e7e4-41b9-85e0-56ee13ef55ed")]
    [ClientResponseType(typeof (IEnumerable<ReorderResult>), null, null)]
    [ClientExample("PATCH__work_workitemsorder.json", "Reorder Backlog Work Items", null, null)]
    public HttpResponseMessage ReorderBacklogWorkItems(ReorderOperation operation) => this.ReorderHelper(operation, new Guid?());

    [HttpPatch]
    [ClientLocationId("47755DB2-D7EB-405A-8C25-675401525FC9")]
    [ClientResponseType(typeof (IEnumerable<ReorderResult>), null, null)]
    [ClientExample("PATCH__work_iterations_iterationId_workitemsorder.json", "Reorder Iteration Work Items", null, null)]
    public HttpResponseMessage ReorderIterationWorkItems(
      ReorderOperation operation,
      Guid iterationId)
    {
      return this.ReorderHelper(operation, new Guid?(iterationId));
    }

    private HttpResponseMessage ReorderHelper(ReorderOperation operation, Guid? iterationId)
    {
      CommonStructureProjectInfo project = CommonStructureProjectInfo.ConvertProjectInfo(this.ProjectInfo);
      AgileSettings settings = new AgileSettings(this.TfsRequestContext, project, this.Team);
      WebAccessWorkItemService service = this.TfsRequestContext.GetService<WebAccessWorkItemService>();
      IDictionary queryContext = (IDictionary) new Hashtable((IEqualityComparer) StringComparer.OrdinalIgnoreCase);
      queryContext[(object) "me"] = (object) this.TfsRequestContext.GetUserIdentity().DisplayName;
      queryContext[(object) "project"] = (object) project.Name;
      return this.Request.CreateResponse<IEnumerable<ReorderResult>>(HttpStatusCode.OK, new ProductBacklogReorderHelper(this.TfsRequestContext, service, (IAgileSettings) settings, settings.TeamSettings.GetBacklogIterationNode(this.TfsRequestContext).GetPath(this.TfsRequestContext), this.ProjectInfo.Id, iterationId, queryContext).Reorder(operation));
    }
  }
}
