// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.AgileTileController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  [SupportedRouteArea(NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("EC7545A3-E5DB-40E8-B0D0-F64DF7619BBA", false)]
  [RequireTeam]
  [OutputCache(CacheProfile = "NoCache")]
  public class AgileTileController : AgileAreaController
  {
    private ChartHelper m_chartHelper;

    public virtual ChartHelper ChartHelper => this.m_chartHelper ?? (this.m_chartHelper = new ChartHelper(this.TfsWebContext.TfsRequestContext, this.Settings, this.Team));

    [HttpGet]
    [TfsTraceFilter(220501, 220502)]
    public ActionResult SprintCapacitySummary()
    {
      JsObject jsObject = new JsObject();
      object model;
      try
      {
        IWebTeamContext webTeamContext = this.TfsRequestContext.GetWebTeamContext();
        CapacityModel capacityModel = new CapacityModel(this.TfsRequestContext, this.Settings, webTeamContext.Project, webTeamContext.Team, this.GetRequestIterationNode());
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode requestIterationNode = this.GetRequestIterationNode();
        if (requestIterationNode != null)
        {
          jsObject.Add("sprintName", (object) requestIterationNode.GetName(this.TfsRequestContext));
          jsObject.Add("hideTitle", (object) true);
          jsObject.Add("addTitleToParent", (object) true);
          jsObject.Add("backlogIterationPath", (object) this.Settings.TeamSettings.GetBacklogIterationNode(this.TfsRequestContext).GetPath(this.TfsRequestContext));
          jsObject.Add("iterationPath", (object) requestIterationNode.GetPath(this.TfsRequestContext));
        }
        jsObject.Add("defaultWorkItemType", (object) this.Settings.BacklogConfiguration.RequirementBacklog.DefaultWorkItemType);
        jsObject.AddObject(capacityModel.ToJson());
        bool flag = false;
        if (capacityModel.TaskboardData != null && capacityModel.TaskboardData.WorkItemData != null && capacityModel.TaskboardData.WorkItemData.Count > 0)
          flag = true;
        jsObject.Add("hasItems", (object) flag);
        model = (object) jsObject;
      }
      catch (Exception ex)
      {
        model = (object) this.GetChartErrorModelFromException(ex);
      }
      return (ActionResult) this.View(model);
    }

    private SettingsErrorModel GetChartErrorModelFromException(Exception ex)
    {
      if (ex is SettingsException exception)
      {
        SettingsErrorModel modelFromException = AgileExceptionUtils.HandleAgileException(this.TfsWebContext, exception, this.Url);
        if (modelFromException != null)
        {
          this.Trace(220501, TraceLevel.Error, modelFromException.ErrorText);
          return modelFromException;
        }
      }
      else
        this.TfsRequestContext.TraceException(599999, "Agile", TfsTraceLayers.Controller, ex);
      return new SettingsErrorModel()
      {
        ErrorText = AgileServerResources.SiteError_UnhandledChartError
      };
    }
  }
}
