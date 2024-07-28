// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.AgileAreaController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  public abstract class AgileAreaController : TfsAreaController
  {
    private WebAccessWorkItemService m_witService;
    private IAgileSettings m_agileSettings;
    private RequestedTeamIterationService m_requestedIterationService;
    private ISettingsUtilities m_userSettings;
    private Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode m_requestIterationNode;

    public override string AreaName => "Agile";

    public override string TraceArea => "Agile";

    public virtual WebAccessWorkItemService WitService
    {
      get
      {
        if (this.m_witService == null)
          this.m_witService = this.TfsRequestContext.GetService<WebAccessWorkItemService>();
        return this.m_witService;
      }
    }

    protected virtual ISettingsUtilities SettingsHelpers
    {
      get
      {
        if (this.m_userSettings == null)
          this.m_userSettings = (ISettingsUtilities) new SettingsUtilities();
        return this.m_userSettings;
      }
    }

    protected virtual RequestedTeamIterationService RequestedIterationService
    {
      get
      {
        if (this.m_requestedIterationService == null)
          this.m_requestedIterationService = new RequestedTeamIterationService();
        return this.m_requestedIterationService;
      }
    }

    public virtual IAgileSettings Settings
    {
      get
      {
        if (this.m_agileSettings == null)
          this.m_agileSettings = (IAgileSettings) new AgileSettings(this.TfsRequestContext, CommonStructureProjectInfo.ConvertProjectInfo(this.TfsWebContext.Project), this.Team);
        return this.m_agileSettings;
      }
    }

    public virtual WebApiTeam Team => this.TfsRequestContext.GetWebTeamContext().Team;

    public virtual T GetTeamUserSettingValue<T>(string settingName) => this.SettingsHelpers.GetTeamUserSettingValue<T>(this.TfsRequestContext, this.Team, settingName);

    public virtual T GetTeamUserSettingValue<T>(string settingName, T defaultValue) => this.SettingsHelpers.GetTeamUserSettingValue<T>(this.TfsRequestContext, this.Team, settingName, defaultValue);

    protected override void OnException(ExceptionContext filterContext)
    {
      if (filterContext.Exception is SettingsException)
      {
        SettingsErrorModel model = AgileExceptionUtils.HandleAgileException(this.TfsWebContext, (SettingsException) filterContext.Exception, this.Url);
        if (model == null)
          return;
        this.Trace(280001, TraceLevel.Warning, string.Join(",", model.ExceptionMessages));
        filterContext.ExceptionHandled = true;
        filterContext.RequestContext.HttpContext.Response.Clear();
        filterContext.Result = (ActionResult) this.View("Error", (object) model);
      }
      else if (filterContext.Exception is AgileInvalidIterationPathException)
      {
        filterContext.ExceptionHandled = true;
        SettingsErrorModel model = new SettingsErrorModel()
        {
          ErrorText = filterContext.Exception.Message
        };
        this.Trace(280002, TraceLevel.Warning, filterContext.Exception.Message);
        model.SetLink(this.Url.Action("index", "iterations", (object) new
        {
          routeArea = "Admin"
        }), AgileServerResources.SiteError_ViewTeamIterations);
        model.SetSecondaryLink(this.Url.Action("backlog", "backlogs", (object) new
        {
          routeArea = ""
        }), AgileServerResources.SiteError_ViewBacklog);
        filterContext.RequestContext.HttpContext.Response.Clear();
        filterContext.Result = (ActionResult) this.View("Error", (object) model);
      }
      else if (filterContext.Exception is BacklogInvalidContextException)
      {
        string str1 = filterContext.Controller.ControllerContext.RouteData.Values["action"].ToString();
        filterContext.ExceptionHandled = true;
        List<string> stringList = new List<string>();
        IEnumerable<BacklogLevelConfiguration> levelConfigurations;
        if (this.TfsWebContext.FeatureContext.GetFeatureMode(LicenseFeatures.PortfolioBacklogManagementId) != FeatureMode.Off)
          levelConfigurations = this.Settings.BacklogConfiguration.PortfolioBacklogs.Concat<BacklogLevelConfiguration>((IEnumerable<BacklogLevelConfiguration>) new BacklogLevelConfiguration[1]
          {
            this.Settings.BacklogConfiguration.RequirementBacklog
          });
        else
          levelConfigurations = (IEnumerable<BacklogLevelConfiguration>) new BacklogLevelConfiguration[1]
          {
            this.Settings.BacklogConfiguration.RequirementBacklog
          };
        foreach (BacklogLevelConfiguration levelConfiguration in levelConfigurations)
        {
          if (this.Settings.BacklogConfiguration.IsBacklogVisible(levelConfiguration.Id))
          {
            TagBuilder tagBuilder = new TagBuilder("a");
            string str2 = this.TfsWebContext.Url.Action(str1 + "/" + levelConfiguration.Name, "backlogs", (object) new
            {
              routeArea = ""
            });
            tagBuilder.MergeAttribute("href", str2);
            tagBuilder.Text(levelConfiguration.Name);
            stringList.Add(tagBuilder.ToString());
          }
        }
        BacklogContextErrorModel model = new BacklogContextErrorModel()
        {
          InvalidPluralName = ((BacklogInvalidContextException) filterContext.Exception).InvalidPluralName,
          HubLinks = (IEnumerable<string>) stringList
        };
        filterContext.RequestContext.HttpContext.Response.Clear();
        filterContext.Result = (ActionResult) this.View("BacklogContextError", (object) model);
      }
      else if (filterContext.Exception is UnauthorizedAccessException)
      {
        base.OnException(filterContext);
      }
      else
      {
        if (filterContext.Exception is Microsoft.TeamFoundation.Framework.Server.TeamFoundationServiceException && ((Microsoft.TeamFoundation.Framework.Server.TeamFoundationServiceException) filterContext.Exception).HttpStatusCode != HttpStatusCode.InternalServerError)
        {
          Microsoft.TeamFoundation.Framework.Server.TeamFoundationServiceException exception = (Microsoft.TeamFoundation.Framework.Server.TeamFoundationServiceException) filterContext.Exception;
          throw new HttpException((int) exception.HttpStatusCode, exception.Message, (Exception) exception);
        }
        this.Trace(280003, TraceLevel.Warning, filterContext.Exception.Message);
        base.OnException(filterContext);
      }
    }

    internal Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode GetIterationNodeForPath(
      string path)
    {
      return this.RequestedIterationService.GetRequestedIterationNode(this.TfsWebContext.TfsRequestContext, this.Settings, path, true, false);
    }

    internal Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode GetRequestIterationNode(
      string iteration = null)
    {
      if (this.m_requestIterationNode == null)
      {
        string pathOrId = iteration ?? this.RouteData.Values["parameters"]?.ToString();
        this.m_requestIterationNode = !string.IsNullOrWhiteSpace(pathOrId) ? this.RequestedIterationService.GetRequestedIterationNode(this.TfsWebContext.TfsRequestContext, this.Settings, pathOrId, true, false) : this.Settings.TeamSettings.GetCurrentIterationNode(this.TfsWebContext.TfsRequestContext, this.TfsWebContext.Project.Id);
      }
      return this.m_requestIterationNode;
    }

    protected virtual void CheckUserIsTeamAdmin(string invalidPermissionMessage)
    {
      if (!this.TfsRequestContext.GetService<ITeamService>().UserIsTeamAdmin(this.TfsRequestContext, this.Team.Identity))
      {
        Microsoft.TeamFoundation.Framework.Client.TeamFoundationServiceException serviceException = new Microsoft.TeamFoundation.Framework.Client.TeamFoundationServiceException(invalidPermissionMessage);
        serviceException.LogException = false;
        throw serviceException;
      }
    }

    protected virtual void CheckUserHasStakeholderLicense(string invalidLicenseMessage)
    {
      if (this.TfsRequestContext.IsStakeholder())
      {
        Microsoft.TeamFoundation.Framework.Client.TeamFoundationServiceException serviceException = new Microsoft.TeamFoundation.Framework.Client.TeamFoundationServiceException(invalidLicenseMessage);
        serviceException.LogException = false;
        throw serviceException;
      }
    }
  }
}
