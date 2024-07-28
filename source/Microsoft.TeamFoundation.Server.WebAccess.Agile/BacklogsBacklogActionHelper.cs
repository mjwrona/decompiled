// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.BacklogsBacklogActionHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  public class BacklogsBacklogActionHelper : BacklogsControllerHelper
  {
    private IBacklogViewModelProvider viewModelHelper = (IBacklogViewModelProvider) new BacklogViewModelProvider();

    protected BacklogsBacklogActionHelper()
    {
    }

    public BacklogsBacklogActionHelper(BacklogsController backlogsController)
      : base(backlogsController)
    {
    }

    public ViewResult GetBacklogView()
    {
      this.ControlDataHelper.AddTeamSettingsData();
      this.ControlDataHelper.AddSprintViewControlData(this.Settings.TeamSettings.GetBacklogIterationNode(this.TfsRequestContext).GetPath(this.TfsRequestContext), "Iteration");
      this.SetupRightSplitter(false);
      BacklogViewModel backlogViewModel = this.GetBacklogViewModel();
      // ISSUE: reference to a compiler-generated field
      if (BacklogsBacklogActionHelper.\u003C\u003Eo__3.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BacklogsBacklogActionHelper.\u003C\u003Eo__3.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, BacklogViewModel, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "BacklogPayload", typeof (BacklogsBacklogActionHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = BacklogsBacklogActionHelper.\u003C\u003Eo__3.\u003C\u003Ep__0.Target((CallSite) BacklogsBacklogActionHelper.\u003C\u003Eo__3.\u003C\u003Ep__0, this.Controller.ViewBag, backlogViewModel);
      this.ControlDataHelper.AddBacklogContributionModel(backlogViewModel.BacklogContext.LevelName);
      return this.View("Backlog");
    }

    public BacklogViewModel GetBacklogViewModel(IEnumerable<string> additionalFields = null) => this.viewModelHelper.GetBacklogViewModel(this.TfsRequestContext, this.TfsWebContext.Project, this.Controller.Team, this.Controller.RequestBacklogContext, additionalFields);

    internal ActionResult GetBacklogContributionsView(string level, bool? showParents)
    {
      using (this.TfsRequestContext.TraceBlock(290581, 290582, "Agile", TfsTraceLayers.Controller, "GetProductContributionsView"))
      {
        NameValueCollection queryString = HttpContext.Current.Request.QueryString;
        string selectedTabContributionId = queryString["contributionId"];
        if (string.IsNullOrEmpty(selectedTabContributionId))
          return (ActionResult) null;
        this.PrepareBacklogTabContributionData(queryString, selectedTabContributionId);
        return (ActionResult) this.View("BacklogContributedTab");
      }
    }

    internal ActionResult GetIterationContributionsView()
    {
      using (this.TfsRequestContext.TraceBlock(290583, 290584, "Agile", TfsTraceLayers.Controller, nameof (GetIterationContributionsView)))
      {
        NameValueCollection queryString = HttpContext.Current.Request.QueryString;
        string selectedTabContributionId = queryString["contributionId"];
        if (string.IsNullOrEmpty(selectedTabContributionId))
          return (ActionResult) null;
        this.PrepareIterationTabContributionData();
        this.ControlDataHelper.AddIterationTabContributionModel(queryString, selectedTabContributionId);
        return (ActionResult) this.View("IterationContributedTab");
      }
    }

    public ViewResult GetCapacityView(string iterationName)
    {
      TeamWITSettingsModel teamWitSettingsModel = this.ControlDataHelper.AddTeamSettingsData();
      TreeNode requestIterationNode = this.Controller.GetRequestIterationNode(iterationName);
      this.ControlDataHelper.AddSprintViewControlData(requestIterationNode.GetPath(this.TfsRequestContext), "Capacity");
      this.ControlDataHelper.AddSprintName(this.Controller.Team.Name);
      this.ControlDataHelper.AddSprintInformation(this.TfsRequestContext);
      this.ControlDataHelper.AddRequirementBacklogContextData();
      this.ControlDataHelper.AddIterationTabContributionModel();
      this.SetupRightSplitter(false);
      this.SetupCapacityViewData(teamWitSettingsModel, true);
      this.ChartHelper.AddBurnDownChartData((IDictionary<string, object>) this.Controller.ViewData, requestIterationNode);
      return this.View("Capacity");
    }

    public JsonResult GetBacklogPayload(IEnumerable<string> additionalFields = null)
    {
      DataContractJsonResult backlogPayload = new DataContractJsonResult((object) this.GetBacklogViewModel(additionalFields));
      backlogPayload.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (JsonResult) backlogPayload;
    }

    public virtual IEnumerable<IDataRecord> GetWorkItemData(
      IEnumerable<int> ids,
      IEnumerable<string> queryFields)
    {
      return this.Controller.WitService.GetWorkItems(this.TfsRequestContext, ids, queryFields);
    }

    private void SetupRightSplitter(bool isVisible)
    {
      if (isVisible)
        return;
      this.Controller.ViewData.AddSplitterClasses("RightHubSplitter", "no-split");
    }
  }
}
