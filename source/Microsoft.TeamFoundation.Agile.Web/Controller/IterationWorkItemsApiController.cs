// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.IterationWorkItemsApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [ClientGroupByResource("iterations")]
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "workitems")]
  public class IterationWorkItemsApiController : TeamSettingsApiControllerBase
  {
    [HttpGet]
    [TraceFilter(100140101, 100140102)]
    [ClientExample("GET__work_teamsettings_iterations__iterationId__workitems.json", "Get workitems", null, null)]
    public IterationWorkItems GetIterationWorkItems(Guid iterationId)
    {
      IReadOnlyCollection<LinkQueryResultEntry> boardQueryResults = this.TfsRequestContext.GetService<IIterationBacklogService>().GetTaskBoardQueryResults(this.TfsRequestContext, CommonStructureProjectInfo.ConvertProjectInfo(this.ProjectInfo), this.Team, iterationId);
      IterationWorkItems iterationWorkItems = new IterationWorkItems();
      WorkItemTrackingRequestContext witRequestContext = this.TfsRequestContext.WitContext();
      iterationWorkItems.WorkItemRelations = boardQueryResults.Select<LinkQueryResultEntry, WorkItemLink>((Func<LinkQueryResultEntry, WorkItemLink>) (wil => WorkItemLinkFactory.Create(witRequestContext, wil.SourceId, wil.TargetId, (int) wil.LinkTypeId)));
      iterationWorkItems.Url = this.GetIterationWorkItemsLinkUrl(iterationId);
      iterationWorkItems.Links = this.GetReferenceLinks(iterationWorkItems.Url, TeamSettingsApiControllerBase.CommonUrlLink.Iteration, iterationId);
      return iterationWorkItems;
    }
  }
}
