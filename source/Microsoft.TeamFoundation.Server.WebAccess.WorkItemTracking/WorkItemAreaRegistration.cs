// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.WorkItemAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using Microsoft.TeamFoundation.Server.WebAccess.Controllers;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections.Specialized;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking
{
  public class WorkItemAreaRegistration : AreaRegistration
  {
    public override string AreaName => "WorkItem";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterBundledArea("WorkItemTracking/Scripts", "WorkItemTracking/Scripts/Resources", "TFS").RegisterResource("WorkItemTracking", (Func<ResourceManager>) (() => WITResources.Manager)).RegisterResource("WorkItemTracking.Common", (Func<ResourceManager>) (() => CommonClientResourceStrings.ResourceManager)).RegisterResource("WorkItemTracking.Navigation", (Func<ResourceManager>) (() => WITNavigationResources.Manager)).RegisterResource("WorkItemTracking.ZeroData", (Func<ResourceManager>) (() => WITZeroDataResources.Manager)).RegisterResource("WorkItemTracking.RoosterJs", (Func<ResourceManager>) (() => RoosterJsResources.Manager)).RegisterResource("WorkItemTracking.RoosterJs.Emoji", (Func<ResourceManager>) (() => RoosterJsEmojiResources.Manager));
      BuiltinPluginManager.RegisterPlugin("WorkItemTracking/Scripts/TFS.WorkItemTracking.Global.Registration", "TFS.Host.UI");
      BuiltinPluginManager.RegisterPluginBase("TFS.WorkItemTracking", "WorkItemTracking/Scripts/");
      BuiltinPluginManager.RegisterPluginBase("TFS.WorkItems", "WorkItemTracking/Scripts/");
      CompatibilityController.RegisterCompatRouter(new CompatibilityController.RouteBackwardCompatibileUrl(this.RouteCompatibleUrl));
    }

    public string RouteCompatibleUrl(
      CompatibilityController controller,
      string page,
      TfsLocator locator,
      ProjectInfo projectInfo)
    {
      if (!locator.CanLocateCollection)
        return (string) null;
      IVssRequestContext vssRequestContext1 = controller.TfsRequestContext.To(TeamFoundationHostType.Application);
      using (IVssRequestContext vssRequestContext2 = vssRequestContext1.GetService<TeamFoundationHostManagementService>().BeginUserRequest(vssRequestContext1, locator.CollectionGuid, vssRequestContext1.UserContext, false))
      {
        if (page.EndsWith("wi.aspx", StringComparison.OrdinalIgnoreCase) || page.EndsWith("WorkItemEdit.aspx", StringComparison.OrdinalIgnoreCase))
        {
          if (controller.Request.QueryString["id"] != null)
          {
            int result1 = 0;
            if (!int.TryParse(controller.Request.QueryString["id"], out result1))
              throw new TeamFoundationServiceException(WITServerResources.InvalidWorkItemCompatUrl());
            string projectName = controller.Request.QueryString["projectId"];
            if (!string.IsNullOrEmpty(projectName))
            {
              Guid result2;
              if (Guid.TryParse(projectName, out result2))
                projectName = vssRequestContext2.GetService<IProjectService>().GetProjectName(vssRequestContext2, result2);
            }
            else
            {
              TeamFoundationWorkItemService service = vssRequestContext2.GetService<TeamFoundationWorkItemService>();
              WorkItem workItemById;
              try
              {
                workItemById = service.GetWorkItemById(vssRequestContext2, result1, false, false, false, WorkItemRetrievalMode.NonDeleted, false, false, new Guid?(), false, new DateTime?());
              }
              catch (WorkItemUnauthorizedAccessException ex)
              {
                ex.LogException = false;
                throw new HttpException(404, ex.Message, (Exception) ex);
              }
              projectName = workItemById.GetProjectName(vssRequestContext2);
            }
            RouteValueDictionary routeValues = new RouteValueDictionary();
            routeValues["routeArea"] = (object) "";
            routeValues["serviceHost"] = (object) vssRequestContext2.ServiceHost;
            routeValues["project"] = (object) projectName;
            routeValues["team"] = (object) "";
            routeValues["parameters"] = (object) result1;
            foreach (string allKey in controller.Request.QueryString.AllKeys)
            {
              if (allKey != null && !allKey.Equals("id", StringComparison.InvariantCultureIgnoreCase) && !allKey.Equals("projectid", StringComparison.InvariantCultureIgnoreCase) && !allKey.Equals("pcguid", StringComparison.InvariantCultureIgnoreCase))
                routeValues[allKey] = (object) controller.Request.QueryString[allKey];
            }
            return Microsoft.TeamFoundation.Server.WebAccess.UrlHelperExtensions.ActionWithParameters(controller.Url, "edit", "workitems", routeValues);
          }
          string str1 = controller.Request.QueryString["wit"];
          if (string.IsNullOrEmpty(str1))
            throw new TeamFoundationServiceException(WITServerResources.UnableToDetermineWorkItemType());
          if (projectInfo == null)
            throw new TeamFoundationServiceException(WITServerResources.UnableToDetermineTeamProjectFromCompatUrl());
          RouteValueDictionary routeValues1 = new RouteValueDictionary();
          routeValues1["routeArea"] = (object) "";
          routeValues1["serviceHost"] = (object) vssRequestContext2.ServiceHost;
          routeValues1["project"] = (object) projectInfo.Name;
          routeValues1["team"] = (object) "";
          routeValues1["parameters"] = (object) str1;
          string clientHost = controller.ClientHost;
          if (!string.IsNullOrEmpty(clientHost))
            routeValues1["clienthost"] = (object) clientHost;
          string str2 = controller.Request.QueryString["titleId"];
          if (!string.IsNullOrEmpty(str2))
            routeValues1["titleId"] = (object) str2;
          foreach (string name in (NameObjectCollectionBase) controller.Request.QueryString)
          {
            if (name.StartsWith("[", StringComparison.OrdinalIgnoreCase) && name.EndsWith("]", StringComparison.OrdinalIgnoreCase) || name.StartsWith("$", StringComparison.OrdinalIgnoreCase))
              routeValues1[name.Trim()] = (object) controller.Request.QueryString[name].Trim();
          }
          return Microsoft.TeamFoundation.Server.WebAccess.UrlHelperExtensions.ActionWithParameters(controller.Url, "create", "workitems", routeValues1);
        }
        if (!page.EndsWith("qr.aspx", StringComparison.OrdinalIgnoreCase) && !page.EndsWith("QueryResult.aspx", StringComparison.OrdinalIgnoreCase) && !page.EndsWith("qe.aspx", StringComparison.OrdinalIgnoreCase))
        {
          if (!page.EndsWith("EditQueryDialog.aspx", StringComparison.OrdinalIgnoreCase))
            goto label_62;
        }
        string str3 = controller.Request.QueryString["qid"];
        if (!string.IsNullOrEmpty(str3))
        {
          if (projectInfo == null)
            throw new TeamFoundationServiceException(WITServerResources.UnableToDetermineTeamProjectFromCompatUrl());
          return controller.Url.ActionWithParameters("resultsById", "workitems", (object) new
          {
            routeArea = "",
            serviceHost = vssRequestContext2.ServiceHost,
            team = "",
            project = projectInfo.Name,
            parameters = str3
          });
        }
        string stringToEscape = controller.Request.QueryString["path"];
        if (!string.IsNullOrEmpty(stringToEscape))
        {
          int length = !string.IsNullOrEmpty(stringToEscape) ? stringToEscape.IndexOf("/", StringComparison.OrdinalIgnoreCase) : throw new TeamFoundationServiceException(WITServerResources.InvalidQueryNamePathUrl());
          if (length < 0)
            throw new TeamFoundationServiceException(WITServerResources.InvalidQueryNamePathUrl());
          if (projectInfo != null)
          {
            if (stringToEscape.StartsWith(projectInfo.Name, StringComparison.CurrentCultureIgnoreCase))
              stringToEscape = stringToEscape.Substring(length + 1);
          }
          else
          {
            string projectName = stringToEscape.Substring(0, length);
            projectInfo = TfsProjectHelpers.GetProjectFromName(vssRequestContext2, projectName);
            stringToEscape = stringToEscape.Substring(length + 1);
          }
          string str4 = (string) null;
          if (page.EndsWith("qr.aspx", StringComparison.OrdinalIgnoreCase) || page.EndsWith("QueryResult.aspx", StringComparison.OrdinalIgnoreCase))
            str4 = "query";
          else if (page.EndsWith("qe.aspx", StringComparison.OrdinalIgnoreCase))
            str4 = "query-edit";
          return controller.Url.Action("index", "workitems", (object) new
          {
            routeArea = "",
            serviceHost = vssRequestContext2.ServiceHost,
            team = "",
            project = projectInfo.Name
          }) + "#_a=" + str4 + "&path=" + Uri.EscapeDataString(stringToEscape);
        }
        string str5 = controller.Request.QueryString["wiql"];
        if (!string.IsNullOrEmpty(str5))
        {
          string str6 = controller.Request.QueryString["name"];
          return controller.Url.Action("adHocQuery", "workitems", (object) new
          {
            routeArea = "",
            serviceHost = vssRequestContext2.ServiceHost,
            team = "",
            project = projectInfo.Name,
            wiql = str5,
            name = str6
          });
        }
      }
label_62:
      return (string) null;
    }
  }
}
