// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BacklogsController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Server.Services;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "backlogs")]
  public class BacklogsController : TfsTeamApiController
  {
    [HttpGet]
    [ClientLocationId("A93726F9-7867-4E38-B4F2-0BFAFC2F6A94")]
    [ClientExample("GET__work_backlogs.json", "Get backlogs", null, null)]
    [ClientDebounce(true)]
    public IEnumerable<Microsoft.TeamFoundation.Work.WebApi.BacklogLevelConfiguration> GetBacklogs()
    {
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration = this.GetBacklogConfiguration();
      WorkItemTrackingRequestContext witRequestContext = this.TfsRequestContext.WitContext();
      IWorkItemTypeService workItemTypeService = this.TfsRequestContext.GetService<IWorkItemTypeService>();
      return backlogConfiguration.GetAllBacklogLevels().Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration, Microsoft.TeamFoundation.Work.WebApi.BacklogLevelConfiguration>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration, Microsoft.TeamFoundation.Work.WebApi.BacklogLevelConfiguration>) (b => b.Convert(backlogConfiguration, witRequestContext, workItemTypeService, this.ProjectId)));
    }

    [HttpGet]
    [ClientLocationId("A93726F9-7867-4E38-B4F2-0BFAFC2F6A94")]
    [ClientExample("GET__work_backlogs_byBacklogId.json", "Get backlog", null, null)]
    public Microsoft.TeamFoundation.Work.WebApi.BacklogLevelConfiguration GetBacklog(string id)
    {
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration1 = this.GetBacklogConfiguration();
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration levelConfiguration = backlogConfiguration1.GetBacklogLevelConfiguration(id);
      WorkItemTrackingRequestContext trackingRequestContext = this.TfsRequestContext.WitContext();
      IWorkItemTypeService service = this.TfsRequestContext.GetService<IWorkItemTypeService>();
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration2 = backlogConfiguration1;
      WorkItemTrackingRequestContext witRequestContext = trackingRequestContext;
      IWorkItemTypeService workItemTypeService = service;
      Guid projectId = this.ProjectId;
      return levelConfiguration.Convert(backlogConfiguration2, witRequestContext, workItemTypeService, projectId);
    }

    [HttpGet]
    [ClientLocationId("7C468D96-AB1D-4294-A360-92F07E9CCD98")]
    [ClientExample("GET__work_backlogs_backlogItems_byBacklogId.json", "Get backlog workitems", null, null)]
    public BacklogLevelWorkItems GetBacklogLevelWorkItems(string backlogId)
    {
      AgileSettings agileSettings = new AgileSettings(this.TfsRequestContext, CommonStructureProjectInfo.ConvertProjectInfo(this.ProjectInfo), this.Team);
      IEnumerable<int> backlogLevelWorkItems = this.TfsRequestContext.GetService<IProductBacklogWorkItemService>().GetBacklogLevelWorkItems(this.TfsRequestContext, (IAgileSettings) agileSettings, backlogId);
      return new BacklogLevelWorkItems()
      {
        WorkItems = backlogLevelWorkItems.Select<int, WorkItemLink>((Func<int, WorkItemLink>) (t => new WorkItemLink()
        {
          Target = new WorkItemReference() { Id = t }
        }))
      };
    }

    private Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration GetBacklogConfiguration() => new AgileSettings(this.TfsRequestContext, CommonStructureProjectInfo.ConvertProjectInfo(this.ProjectInfo), this.Team).BacklogConfiguration;
  }
}
