// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ApiBrowseController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [SupportedRouteArea("Api", NavigationContextLevels.ApplicationAll)]
  [OutputCache(CacheProfile = "NoCache")]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiBrowseController : AdminAreaController
  {
    [HttpGet]
    [AcceptNavigationLevels(NavigationContextLevels.Collection)]
    public ActionResult GetTeamProjects() => (ActionResult) this.Json((object) new CollectionViewModel(this.TfsWebContext), JsonRequestBehavior.AllowGet);

    [HttpGet]
    [AcceptNavigationLevels(NavigationContextLevels.Project)]
    public ActionResult GetTeams() => (ActionResult) this.Json((object) new TeamProjectModel(this.TfsWebContext, true, this.TfsWebContext.TfsRequestContext.ServiceHost.Name), JsonRequestBehavior.AllowGet);

    [HttpGet]
    [AcceptNavigationLevels(NavigationContextLevels.Collection)]
    public ActionResult GetCollectionProperties()
    {
      CollectionViewModel collectionViewModel = new CollectionViewModel(this.TfsWebContext);
      JsObject data = new JsObject();
      data["currentIdentity"] = (object) JsonExtensions.ToJson(IdentityUtil.Convert(this.TfsWebContext.CurrentUserIdentity));
      data["serviceHost"] = (object) collectionViewModel.CollectionServiceHost.ToJson();
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [AcceptNavigationLevels(NavigationContextLevels.Application)]
    public ActionResult GetCollectionProperties(Guid collectionId)
    {
      TfsServiceHostDescriptor collectionProperties = this.TfsWebContext.GetCollectionProperties(collectionId);
      if (collectionProperties == null)
        return (ActionResult) this.HttpNotFound();
      CollectionViewModel collectionViewModel = new CollectionViewModel(this.TfsWebContext, collectionProperties);
      JsObject data = new JsObject();
      data["name"] = (object) collectionViewModel.DisplayName;
      data[nameof (collectionId)] = (object) collectionViewModel.CollectionId;
      data["status"] = (object) collectionViewModel.Status;
      data["projectCount"] = (object) collectionViewModel.TeamProjects.Count;
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    public ActionResult BrowseControl(
      bool? hideDefaultTeam,
      bool? showTeamsOnly,
      bool? ignoreDefaultLoad,
      string selectedTeam = null)
    {
      if (!hideDefaultTeam.HasValue)
        hideDefaultTeam = new bool?(false);
      if (!showTeamsOnly.HasValue)
        showTeamsOnly = new bool?(false);
      if (!ignoreDefaultLoad.HasValue)
        ignoreDefaultLoad = new bool?(false);
      return (ActionResult) this.View((object) new BrowseControlModel()
      {
        ShowTasks = false,
        UseHashNavigation = false,
        HideDefaultTeam = hideDefaultTeam.Value,
        ShowOnlyTeams = showTeamsOnly.Value,
        IgnoreDefaultLoad = ignoreDefaultLoad.Value,
        SelectedTeam = selectedTeam
      });
    }

    [HttpGet]
    public ActionResult Browse(bool showTasks, Guid? collectionId)
    {
      this.ViewData[nameof (showTasks)] = (object) showTasks;
      switch (this.NavigationContext.TopMostLevel)
      {
        case NavigationContextLevels.Application:
          TfsServiceHostDescriptor collectionProperties = this.TfsWebContext.GetCollectionProperties(collectionId.Value);
          if (collectionProperties == null)
            return (ActionResult) this.HttpNotFound();
          return (ActionResult) this.View("BrowseCollection", (object) new CollectionViewModel(this.TfsWebContext, collectionProperties)
          {
            CanCreateProjects = this.CanCreateProjects
          });
        case NavigationContextLevels.Collection:
          return (ActionResult) this.View("BrowseCollection", (object) new CollectionViewModel(this.TfsWebContext)
          {
            CanCreateProjects = this.CanCreateProjects
          });
        case NavigationContextLevels.Project:
          return (ActionResult) this.View("BrowseProject", (object) new TeamProjectModel(this.TfsWebContext, this.TfsWebContext.TfsRequestContext.ServiceHost.Name));
        case NavigationContextLevels.Team:
          return (ActionResult) this.View("BrowseTeam", (object) new TeamViewModel(this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, new IdentityDescriptor[1]
          {
            this.Team.Identity.Descriptor
          }, MembershipQuery.Direct, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
          {
            TeamConstants.TeamPropertyName
          }, IdentityPropertyScope.Local)[0]));
        default:
          return (ActionResult) new EmptyResult();
      }
    }
  }
}
