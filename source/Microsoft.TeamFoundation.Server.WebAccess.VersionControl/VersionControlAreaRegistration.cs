// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.VersionControlAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using Microsoft.TeamFoundation.Server.WebAccess.Controllers;
using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.SourceControl.WebServer.Legacy;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.TeamFoundation.WebPlatform.Server;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Resources;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  public class VersionControlAreaRegistration : AreaRegistration
  {
    public const string c_repoRouteSegmentName = "Repo";

    public override string AreaName => "VersionControl";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterBundledArea(this.AreaName + "/Scripts", this.AreaName + "/Scripts/Resources", "TFS").RegisterResource("VersionControl", (Func<ResourceManager>) (() => VCResources.ResourceManager)).RegisterResource("VersionControl.Navigation", (Func<ResourceManager>) (() => VCNavigationResources.ResourceManager)).RegisterResource("Branches", (Func<ResourceManager>) (() => BranchResources.ResourceManager)).RegisterResource("ImportDialog", (Func<ResourceManager>) (() => ImportDialogResources.ResourceManager)).RegisterResource("ImportStatus", (Func<ResourceManager>) (() => ImportStatusResources.ResourceManager));
      BuiltinPluginManager.RegisterPlugin("VersionControl/Scripts/TFS.VersionControl.Registration.Artifacts", "TFS.OM.Common");
      BuiltinPluginManager.RegisterPluginBase("TFS.VersionControl", "VersionControl/Scripts/");
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
      using (IVssRequestContext vssRequestContext2 = vssRequestContext1.GetService<TeamFoundationHostManagementService>().BeginRequest(vssRequestContext1, locator.CollectionGuid, RequestContextType.UserContext, true, true))
      {
        string projectName = projectInfo?.Name;
        if (string.IsNullOrEmpty(projectName))
        {
          string str = controller.Request.QueryString["path"];
          if (!string.IsNullOrEmpty(str))
          {
            if (str.StartsWith("$/", StringComparison.OrdinalIgnoreCase))
              str = str.Substring("$/".Length);
            if (!string.IsNullOrEmpty(str))
            {
              int length = str.IndexOf('/');
              if (length > 0)
                projectName = str.Substring(0, length);
            }
          }
        }
        if (page.EndsWith("cs.aspx", StringComparison.OrdinalIgnoreCase) || page.EndsWith("ViewChangeset.aspx", StringComparison.OrdinalIgnoreCase))
        {
          int changesetId = TfsVersionControlProvider.GetChangesetId(controller.TfsRequestContext, controller.Request.Params, true);
          if (string.IsNullOrEmpty(projectName))
          {
            try
            {
              projectName = this.GetProjectNameForChangeset(vssRequestContext2, changesetId);
            }
            catch (Exception ex)
            {
              vssRequestContext2.TraceException(515180, VersionControlConstants.TraceArea, TfsTraceLayers.BusinessLogic, ex);
              projectName = (string) null;
            }
          }
          return controller.Url.ActionWithParameters("changeset", "versionControl", (object) new
          {
            routeArea = "",
            serviceHost = vssRequestContext2.ServiceHost,
            team = "",
            project = projectName,
            parameters = changesetId
          });
        }
        if (page.EndsWith("ss.aspx", StringComparison.OrdinalIgnoreCase) || page.EndsWith("ViewShelveset.aspx", StringComparison.OrdinalIgnoreCase))
        {
          string shelvesetId = TfsVersionControlProvider.TryParseShelvesetId(controller.TfsRequestContext, controller.Request.Params);
          VersionSpec versionSpecString = LegacyTfsModelExtensions.ParseVersionSpecString(ShelvesetVersionSpec.Identifier.ToString() + shelvesetId, (VersionSpec) null);
          if (versionSpecString != null && versionSpecString is ShelvesetVersionSpec)
          {
            ShelvesetVersionSpec versionSpec = (ShelvesetVersionSpec) versionSpecString;
            TfsChange tfsChange = TfsVersionControlProvider.GetShelvedChanges(vssRequestContext2, "$/", RecursionType.Full, 2, versionSpec, 1, 0, (string[]) null, out bool _).FirstOrDefault<TfsChange>();
            if (tfsChange != null && tfsChange.Item != null)
            {
              projectName = VersionControlPath.GetTeamProjectName(tfsChange.Item.ServerItem);
              TfsProjectHelpers.GetProjectFromName(vssRequestContext2, projectName);
            }
          }
          return controller.Url.ActionWithParameters("shelveset", "versionControl", (object) new
          {
            routeArea = "",
            serviceHost = vssRequestContext2.ServiceHost,
            team = "",
            project = projectName,
            ss = shelvesetId
          });
        }
        if (page.EndsWith("view.aspx", StringComparison.OrdinalIgnoreCase) || page.EndsWith("ViewSource.aspx", StringComparison.OrdinalIgnoreCase))
        {
          RouteValueDictionary routeValues = new RouteValueDictionary((object) new
          {
            routeArea = "",
            serviceHost = vssRequestContext2.ServiceHost,
            team = "",
            project = projectName
          });
          RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
          NameValueCollection queryString = controller.Request.QueryString;
          foreach (string key in queryString.Keys)
          {
            if (key.Equals("path", StringComparison.OrdinalIgnoreCase) || key.Equals("cs", StringComparison.OrdinalIgnoreCase) || key.Equals("version", StringComparison.OrdinalIgnoreCase))
              routeValueDictionary.Add(key, (object) queryString[key]);
            else if (key.Equals("item", StringComparison.OrdinalIgnoreCase))
            {
              int result = 0;
              if (int.TryParse(queryString[key], out result))
              {
                ItemModel itemModel = (ItemModel) TfsVersionControlProvider.GetItemsById(vssRequestContext2, new int[1]
                {
                  result
                }, 0, false).FirstOrDefault<TfsItem>();
                if (itemModel != null)
                  routeValueDictionary.Add("path", (object) itemModel.ServerItem);
              }
            }
          }
          StringBuilder stringBuilder = new StringBuilder(controller.Url.Action("index", "versionControl", routeValues) + "?_a=contents");
          UrlHelper url = controller.Url;
          if (routeValueDictionary.ContainsKey("path"))
            stringBuilder.Append("&path=" + Uri.EscapeDataString(routeValueDictionary["path"].ToString()));
          if (routeValueDictionary.ContainsKey("version"))
            stringBuilder.Append("&version=" + Uri.EscapeDataString(routeValueDictionary["version"].ToString()));
          else if (routeValueDictionary.ContainsKey("cs"))
            stringBuilder.Append("&version=" + Uri.EscapeDataString(routeValueDictionary["cs"].ToString()));
          return stringBuilder.ToString();
        }
        if (page.EndsWith("diff.aspx", StringComparison.OrdinalIgnoreCase) || page.EndsWith("Difference.aspx", StringComparison.OrdinalIgnoreCase))
        {
          NameValueCollection queryString = controller.Request.QueryString;
          RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
          foreach (string key in queryString.Keys)
          {
            if (key.Equals("opath", StringComparison.OrdinalIgnoreCase) || key.Equals("oversion", StringComparison.OrdinalIgnoreCase) || key.Equals("ocs", StringComparison.OrdinalIgnoreCase) || key.Equals("mpath", StringComparison.OrdinalIgnoreCase) || key.Equals("mversion", StringComparison.OrdinalIgnoreCase) || key.Equals("mss", StringComparison.OrdinalIgnoreCase) || key.Equals("mcs", StringComparison.OrdinalIgnoreCase))
              routeValueDictionary[key] = (object) queryString[key];
          }
          if (string.IsNullOrEmpty(projectName))
          {
            try
            {
              object obj;
              if (routeValueDictionary.TryGetValue("mcs", out obj))
              {
                int result;
                if (int.TryParse(obj.ToString(), out result))
                  projectName = this.GetProjectNameForChangeset(vssRequestContext2, result);
              }
            }
            catch (Exception ex)
            {
              vssRequestContext2.TraceException(515180, VersionControlConstants.TraceArea, TfsTraceLayers.BusinessLogic, ex);
              projectName = (string) null;
            }
          }
          RouteValueDictionary routeValues = new RouteValueDictionary((object) new
          {
            routeArea = "",
            serviceHost = vssRequestContext2.ServiceHost,
            team = "",
            project = projectName
          });
          StringBuilder stringBuilder = new StringBuilder(controller.Url.Action("index", "versionControl", routeValues) + "?_a=compare");
          UrlHelper url = controller.Url;
          if (routeValueDictionary.ContainsKey("opath"))
            stringBuilder.Append("&path=" + Uri.EscapeDataString(routeValueDictionary["opath"].ToString()));
          if (routeValueDictionary.ContainsKey("mpath"))
            stringBuilder.Append("&mpath=" + Uri.EscapeDataString(routeValueDictionary["mpath"].ToString()));
          if (routeValueDictionary.ContainsKey("oversion"))
            stringBuilder.Append("&oversion=" + Uri.EscapeDataString(routeValueDictionary["oversion"].ToString()));
          else if (routeValueDictionary.ContainsKey("ocs"))
            stringBuilder.Append("&oversion=" + Uri.EscapeDataString(routeValueDictionary["ocs"].ToString()));
          if (routeValueDictionary.ContainsKey("mversion"))
            stringBuilder.Append("&mversion=" + Uri.EscapeDataString(routeValueDictionary["mversion"].ToString()));
          else if (routeValueDictionary.ContainsKey("mcs"))
            stringBuilder.Append("&mversion=" + Uri.EscapeDataString(routeValueDictionary["mcs"].ToString()));
          else if (routeValueDictionary.ContainsKey("mss"))
            stringBuilder.Append("&mversion=S" + Uri.EscapeDataString(routeValueDictionary["mss"].ToString()));
          return stringBuilder.ToString();
        }
        if (!page.EndsWith("scc.aspx", StringComparison.OrdinalIgnoreCase))
        {
          if (!page.EndsWith("Explorer.aspx", StringComparison.OrdinalIgnoreCase))
            goto label_77;
        }
        RouteValueDictionary routeValues1 = new RouteValueDictionary((object) new
        {
          routeArea = "",
          serviceHost = vssRequestContext2.ServiceHost,
          team = "",
          project = projectName
        });
        NameValueCollection queryString1 = controller.Request.QueryString;
        RouteValueDictionary routeValueDictionary1 = new RouteValueDictionary();
        string stringToEscape = queryString1["path"];
        StringBuilder stringBuilder1 = new StringBuilder(controller.Url.Action("index", "versionControl", routeValues1));
        if (!string.IsNullOrEmpty(stringToEscape))
          stringBuilder1.Append("?_a=contents&path=" + Uri.EscapeDataString(stringToEscape));
        return stringBuilder1.ToString();
      }
label_77:
      return (string) null;
    }

    private string GetProjectNameForChangeset(
      IVssRequestContext collectionRequestContext,
      int changesetId)
    {
      string projectName = (string) null;
      Microsoft.TeamFoundation.VersionControl.Server.Change change = TfsVersionControlProvider.GetChangesetChanges(collectionRequestContext, changesetId, 1, out bool _).FirstOrDefault<Microsoft.TeamFoundation.VersionControl.Server.Change>();
      if (change != null)
      {
        projectName = VersionControlPath.GetTeamProjectName(change.Item.ServerItem);
        TfsProjectHelpers.GetProjectFromName(collectionRequestContext, projectName);
      }
      return projectName;
    }
  }
}
