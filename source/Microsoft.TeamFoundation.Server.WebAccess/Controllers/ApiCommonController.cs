// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.ApiCommonController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.Server.WebAccess.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.Utils;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.IdentityImage;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers
{
  [SupportedRouteArea("Api", NavigationContextLevels.ApplicationAll)]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiCommonController : TfsController
  {
    private const string c_aboutTfsPath = "/AccountHomepage/AboutTfs";
    private const string c_announcementPath = "/WebAccess/Announcement/UserLatest";
    private const string c_getingStartedVideoPath = "/WebAccess/GettingStartedVideo";
    private const string c_openPlatformPath = "/Admin/OpenPlatformAnnouncement/UserLatest";
    private const string c_powerBIPath = "/PowerBI/PowerBIAnnouncement/UserLatest";
    private const string c_ElsVsoIntegrationPath = "/ELS/ElsVsoIntegrationAnnouncement/UserLatest";
    private const string c_analyticsAnnouncementType = "Analytics";
    private const string c_openPlatformAnnouncementType = "OpenPlatform";
    private const string c_powerBIAnnouncementType = "PowerBI";
    private const string c_ElsVsoIntegrationAnnouncementType = "ElsVsoIntegration";
    private const string c_howToPath = "/TeamHomepage/HowTo";
    private static string s_traceArea = "WebAccess.Common";
    private const string c_tfsNewsCacheKey = "TfsNewsCache";
    private const string c_tfsNewsFeedUrl = "https://go.microsoft.com/fwlink/?LinkId=322031";
    private const string c_profileImageServiceUpdateFeatureFlag = "WebAccess.Profile.ProfileImageService";
    private const string c_defaultGroupImage = "Team.svg";
    private const string c_defaultUserImage = "User.svg";
    private const string GetOrganizationNavigationInfoTimeout = "/WebAccess/GetOrganizationNavigationInfoTimeoutTimeout";
    private const string GetOrganizationNavigationInfoTimeoutCommandKey = "GetOrganizationNavigationInfoTimeout";
    private static readonly int defaultGetOrganizationNavigationInfoTimeoutTimeoutMiliseconds = 2000;
    private const string portalServiceId = "0000004D-0000-8888-8000-000000000000";
    private const int WebAccessExceptionEaten = 599999;
    private IVssRequestContext m_tfsRequestContext;
    private const int c_DdsGetAvatar_ClientCacheExpirationInterval_H = 48;
    private const int c_GetIdentityImage_CachedExpirationInterval_H = 8760;
    private const int c_GetCollectionAvatar_CachedExpirationInterval_H = 48;
    private const string c_buildMeterName = "Build";
    private const string c_loadTestMeterName = "LoadTest";

    public override string TraceArea => ApiCommonController.s_traceArea;

    [AcceptVerbs(HttpVerbs.Get)]
    [ValidateInput(false)]
    [TfsTraceFilter(505190, 505200)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin]
    public JsonResult TestMailSettings(string sendTo, string message)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationMailService service = vssRequestContext.GetService<ITeamFoundationMailService>();
      Exception exception = (Exception) null;
      string str;
      try
      {
        if (string.IsNullOrEmpty(sendTo))
          throw new ArgumentNullException(nameof (sendTo));
        MailMessage message1 = new MailMessage();
        message1.To.Add(sendTo);
        message1.Subject = WACommonResources.TestMailTitle;
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(WACommonResources.TestMailMessage);
        if (!string.IsNullOrEmpty(message))
          stringBuilder.AppendLine(message);
        message1.Body = stringBuilder.ToString();
        service.Send(vssRequestContext, message1);
        str = WACommonResources.TestMailSuccess;
      }
      catch (Exception ex)
      {
        exception = ex;
        str = ex.Message;
      }
      JsObject data = new JsObject();
      data.Add(nameof (message), (object) str);
      data.Add("exception", exception != null ? (object) exception.ToJson(true, true) : (object) (JsObject) null);
      return this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [TfsTraceFilter(505040, 505050)]
    [ValidateInput(false)]
    public ActionResult IdentityImage(
      Guid? id,
      bool? previewCandidateImage,
      string email,
      string identifier = null,
      bool resolveAmbiguous = true,
      IdentitySearchFilter identifierType = IdentitySearchFilter.AccountName,
      ImageSize size = ImageSize.Medium,
      string defaultGravatar = "",
      bool isOrganizationLevel = false,
      string t = null)
    {
      try
      {
        if (isOrganizationLevel && this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Web.OrgAdmin.UserExperience"))
          this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
        bool cacheBusted = t != null;
        return this.IdentityImageInternal(id, previewCandidateImage, email, identifier, resolveAmbiguous, identifierType, false, size, defaultGravatar, cacheBusted);
      }
      catch (CircuitBreakerException ex)
      {
        this.TfsRequestContext.TraceException(597582, "WebAccess", TfsTraceLayers.Controller, (Exception) ex);
        throw new IdentityImageFailureException((Exception) ex);
      }
    }

    [HttpGet]
    [TfsTraceFilter(505055, 505066)]
    [ValidateInput(false)]
    public ActionResult CollectionAvatar(Guid id, ImageSize size = ImageSize.Small)
    {
      IVssRequestContext deploymentContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      try
      {
        ICollectionAvatarService collectionAvatarService = deploymentContext.GetService<ICollectionAvatarService>();
        Microsoft.VisualStudio.Services.Organization.Client.Collection collection = collectionAvatarService.GetCollectionForRequestIdentityByCollectionId(deploymentContext, id, new string[1]
        {
          "SystemProperty.AvatarTimestamp"
        });
        return collection == null ? (ActionResult) new HttpNotFoundResult() : (ActionResult) new ETaggedResult((Func<string>) (() => collectionAvatarService.CalculateCollectionAvatarEtag(collection)), (Func<ActionResult>) (() => (ActionResult) this.File(collectionAvatarService.GetCollectionAvatarData(deploymentContext, collection, size), "image/png")), TimeSpan.FromHours(48.0), HttpCacheability.Public);
      }
      catch (CircuitBreakerException ex)
      {
        this.TfsRequestContext.TraceException(597582, "WebAccess", TfsTraceLayers.Controller, (Exception) ex);
        throw new CollectionAvatarFailureException((Exception) ex);
      }
    }

    [HttpGet]
    [TfsTraceFilter(505211, 505220)]
    [ValidateInput(false)]
    public ActionResult GetDdsAvatar(string id, bool isOrganizationLevel = false)
    {
      if (isOrganizationLevel && this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Web.OrgAdmin.UserExperience"))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      if (string.IsNullOrWhiteSpace(id))
        return this.GetDdsEntityDefaultImage("user");
      ActionResult ddsAvatarInternal = this.GetDdsAvatarInternal(this.TfsRequestContext, id);
      this.Response.Cache.SetCacheability(HttpCacheability.Public);
      this.Response.Cache.SetMaxAge(new TimeSpan(48, 0, 0));
      this.Response.Cache.SetExpires(DateTime.Now.AddHours(48.0));
      return ddsAvatarInternal;
    }

    [HttpGet]
    [TfsTraceFilter(505190, 505200)]
    [ValidateInput(false)]
    public ActionResult IdentityImageJson(
      Guid? id,
      string email,
      string identifier = null,
      bool resolveAmbiguous = true,
      IdentitySearchFilter identifierType = IdentitySearchFilter.AccountName,
      ImageSize size = ImageSize.Medium,
      string defaultGravatar = "")
    {
      return this.IdentityImageInternal(id, new bool?(false), email, identifier, resolveAmbiguous, identifierType, true, size, defaultGravatar);
    }

    private ActionResult IdentityImageInternal(
      Guid? id,
      bool? previewCandidateImage,
      string email,
      string identifier,
      bool resolveAmbiguous,
      IdentitySearchFilter identifierType,
      bool makeJson,
      ImageSize size,
      string defaultGravatar = "",
      bool cacheBusted = false)
    {
      IdentityImageService imageService = IdentityImageServiceUtil.GetIdentityImageService(this.TfsRequestContext);
      Guid resolvedId = Guid.Empty;
      if (id.HasValue)
      {
        resolvedId = id.Value;
      }
      else
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        if (!string.IsNullOrWhiteSpace(identifier))
          identity = imageService.GetIdentity(this.TfsRequestContext, identifier, identifierType, resolveAmbiguous);
        else if (!string.IsNullOrWhiteSpace(email))
          identity = imageService.GetIdentityFromEmail(this.TfsRequestContext, ref email, resolveAmbiguous);
        if (identity != null)
          resolvedId = identity.Id;
      }
      bool isContainer = false;
      ActionResult actionResult;
      if (resolvedId == Guid.Empty && !string.IsNullOrWhiteSpace(email) && !imageService.DisableGravatar)
      {
        actionResult = this.GetGravatar(imageService, email, makeJson, defaultGravatar);
      }
      else
      {
        Guid imageEtag = imageService.GetIdentityImageId(this.TfsRequestContext, resolvedId, out isContainer);
        if (previewCandidateImage.GetValueOrDefault())
        {
          this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
          this.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
          actionResult = this.GetImageData(imageService, resolvedId, true, isContainer, makeJson, size);
        }
        else if (imageEtag == Guid.Empty)
          actionResult = !this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment ? (ActionResult) null : this.GetImageData(imageService, resolvedId, false, isContainer, makeJson, size);
        else
          actionResult = (ActionResult) new ETaggedResult((Func<string>) (() => imageEtag.ToString()), (Func<ActionResult>) (() => this.GetImageData(imageService, resolvedId, false, isContainer, makeJson, size)), cacheBusted ? TimeSpan.FromHours(8760.0) : TimeSpan.FromHours(48.0), HttpCacheability.Public);
      }
      return actionResult ?? this.GetDdsEntityDefaultImage(isContainer ? "vsogroup" : "user", makeJson);
    }

    private ActionResult GetImageData(
      IdentityImageService imageService,
      Guid id,
      bool candidateImage,
      bool isContainer,
      bool makeJson,
      ImageSize size)
    {
      byte[] imageData;
      string contentType;
      imageService.GetIdentityImageData(this.TfsRequestContext, id, candidateImage, out imageData, out contentType, new ImageSize?(size));
      if (imageData == null)
        return this.GetDdsEntityDefaultImage(isContainer ? "vsogroup" : "user", makeJson);
      return makeJson ? (ActionResult) this.Json((object) new
      {
        imageSrc = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "data:{0};base64,{1}", (object) contentType, (object) Convert.ToBase64String(imageData))
      }, JsonRequestBehavior.AllowGet) : (ActionResult) this.File(imageData, contentType);
    }

    private ActionResult GetGravatar(
      IdentityImageService imageService,
      string email,
      bool makeJson,
      string defaultGravatar = "")
    {
      string gravatar = imageService.GetGravatar(this.TfsRequestContext, email, this.Url.RequestContext.HttpContext.Request.Url.Scheme, this.Url.RequestContext.HttpContext.Request.IsSecureConnection, defaultGravatar);
      if (string.IsNullOrWhiteSpace(gravatar))
        return (ActionResult) null;
      return makeJson ? (ActionResult) this.Json((object) new
      {
        imageSrc = gravatar
      }, JsonRequestBehavior.AllowGet) : (ActionResult) new RedirectResult(gravatar);
    }

    [HttpGet]
    [TfsTraceFilter(505060, 505070)]
    [AcceptNavigationLevels(NavigationContextLevels.Application)]
    [ValidateInput(false)]
    public ActionResult QueryJumpList(
      string navigationContextPackage,
      string searchQuery,
      bool showStoppedCollections)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      NavigationContextModel navigationContext = !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? new JavaScriptSerializer().Deserialize<NavigationContextModel>(navigationContextPackage) : JsonConvert.DeserializeObject<NavigationContextModel>(navigationContextPackage);
      List<CollectionJumpPointModel> collectionModels = new List<CollectionJumpPointModel>();
      List<CollectionJumpPointModel> source1 = new List<CollectionJumpPointModel>();
      List<ProjectJumpPointModel> source2 = new List<ProjectJumpPointModel>();
      List<TeamJumpPointModel> source3 = new List<TeamJumpPointModel>();
      foreach (TfsServiceHostDescriptor allCollection in this.TfsWebContext.GetAllCollections())
      {
        CollectionJumpPointModel collectionJumpPointModel = new CollectionJumpPointModel(this.TfsWebContext, navigationContext, allCollection);
        collectionModels.Add(collectionJumpPointModel);
        if (collectionJumpPointModel.Name.IndexOf(searchQuery, StringComparison.CurrentCultureIgnoreCase) > -1 && collectionJumpPointModel.IsOnline | showStoppedCollections)
          source1.Add(collectionJumpPointModel);
      }
      this.PopulateCollectionJumpPoints(navigationContext, (IEnumerable<CollectionJumpPointModel>) collectionModels, false, true);
      foreach (CollectionJumpPointModel collectionJumpPointModel in collectionModels)
      {
        foreach (ProjectJumpPointModel project in collectionJumpPointModel.Projects)
        {
          if (project.Name.IndexOf(searchQuery, StringComparison.CurrentCultureIgnoreCase) > -1)
            source2.Add(project);
          foreach (TeamJumpPointModel team in project.Teams)
          {
            if (team.Name.IndexOf(searchQuery, StringComparison.CurrentCultureIgnoreCase) > -1)
              source3.Add(team);
          }
        }
      }
      source1.Sort();
      source2.Sort();
      source3.Sort();
      JsObject data = new JsObject();
      data.Add("collections", (object) source1.Select<CollectionJumpPointModel, JsObject>((Func<CollectionJumpPointModel, JsObject>) (x => x.ToJson())));
      data.Add("projects", (object) source2.Select<ProjectJumpPointModel, JsObject>((Func<ProjectJumpPointModel, JsObject>) (x => x.ToJson())));
      data.Add("teams", (object) source3.Select<TeamJumpPointModel, JsObject>((Func<TeamJumpPointModel, JsObject>) (x => x.ToJson())));
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [TfsTraceFilter(505080, 505090)]
    public ActionResult GetJumpList(
      string navigationContextPackage,
      Guid? selectedHostId,
      bool showStoppedCollections,
      bool? showTeamsOnly,
      bool? ignoreDefaultLoad)
    {
      if (!showTeamsOnly.HasValue)
        showTeamsOnly = new bool?(false);
      if (!ignoreDefaultLoad.HasValue)
        ignoreDefaultLoad = new bool?(false);
      IEnumerable<TfsServiceHostDescriptor> allCollections = this.TfsWebContext.GetAllCollections();
      return this.GetJumpListInternal(navigationContextPackage, selectedHostId ?? this.TfsRequestContext.ServiceHost.InstanceId, allCollections, showStoppedCollections, showTeamsOnly.Value, ignoreDefaultLoad.Value, false);
    }

    [HttpGet]
    public ActionResult GetCollectionJumpList(
      string navigationContextPackage,
      Guid selectedHostId,
      bool populateAllTeams = false)
    {
      IEnumerable<TfsServiceHostDescriptor> collections = (IEnumerable<TfsServiceHostDescriptor>) new TfsServiceHostDescriptor[1]
      {
        this.TfsWebContext.GetCollectionProperties(selectedHostId)
      };
      return this.GetJumpListInternal(navigationContextPackage, selectedHostId, collections, false, false, false, populateAllTeams);
    }

    [HttpGet]
    public ActionResult GetProjectJumpList(
      string navigationContextPackage,
      string projectUri,
      Guid collectionId)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      NavigationContextModel navigationContext1 = !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? new JavaScriptSerializer().Deserialize<NavigationContextModel>(navigationContextPackage) : JsonConvert.DeserializeObject<NavigationContextModel>(navigationContextPackage);
      Dictionary<string, ProjectJumpPointModel> source = new Dictionary<string, ProjectJumpPointModel>();
      using (IVssRequestContext context = this.TfsRequestContext.GetService<TeamFoundationHostManagementService>().BeginRequest(this.TfsRequestContext, collectionId, RequestContextType.UserContext, true, true))
      {
        IProjectService service = context.GetService<IProjectService>();
        Guid projectId1 = TeamProjectModel.ExtractProjectId(projectUri);
        IVssRequestContext requestContext = context;
        Guid projectId2 = projectId1;
        ProjectInfo project = service.GetProject(requestContext, projectId2);
        CollectionJumpPointModel collection = new CollectionJumpPointModel(this.TfsWebContext, navigationContext1, this.TfsWebContext.GetCollectionProperties(collectionId));
        source[projectUri] = new ProjectJumpPointModel(this.TfsWebContext, navigationContext1, project, collection);
        NavigationContextModel navigationContext2 = navigationContext1;
        List<Guid> collections = new List<Guid>();
        collections.Add(collectionId);
        Dictionary<string, ProjectJumpPointModel> projectCache = source;
        this.PopulateProjectJumpPointsTeamInfo(navigationContext2, (IEnumerable<Guid>) collections, projectCache);
      }
      SecureJsonResult projectJumpList = new SecureJsonResult();
      projectJumpList.Data = (object) source.Select<KeyValuePair<string, ProjectJumpPointModel>, JsObject>((Func<KeyValuePair<string, ProjectJumpPointModel>, JsObject>) (x => x.Value.ToJson()));
      projectJumpList.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) projectJumpList;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    private ActionResult GetJumpListInternal(
      string navigationContextPackage,
      Guid selectedHostId,
      IEnumerable<TfsServiceHostDescriptor> collections,
      bool showStoppedCollections,
      bool showTeamsOnly,
      bool ignoreDefaultLoad,
      bool forceLoadAllTeams)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      NavigationContextModel navigationContext = !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? new JavaScriptSerializer().Deserialize<NavigationContextModel>(navigationContextPackage) : JsonConvert.DeserializeObject<NavigationContextModel>(navigationContextPackage);
      List<CollectionJumpPointModel> source = new List<CollectionJumpPointModel>();
      CollectionJumpPointModel collectionJumpPointModel1 = (CollectionJumpPointModel) null;
      foreach (TfsServiceHostDescriptor collection in collections)
      {
        if (collection != null)
        {
          CollectionJumpPointModel collectionJumpPointModel2 = new CollectionJumpPointModel(this.TfsWebContext, navigationContext, collection);
          if (collectionJumpPointModel2.IsOnline | showStoppedCollections)
          {
            if (collection.Id == selectedHostId)
              collectionJumpPointModel1 = collectionJumpPointModel2;
            source.Add(collectionJumpPointModel2);
          }
        }
      }
      source.Sort();
      if (collectionJumpPointModel1 == null && !ignoreDefaultLoad)
        collectionJumpPointModel1 = source.FirstOrDefault<CollectionJumpPointModel>();
      if (showTeamsOnly && collectionJumpPointModel1 != null)
      {
        source.Clear();
        source.Add(collectionJumpPointModel1);
      }
      if (collectionJumpPointModel1 != null && !ignoreDefaultLoad)
        this.PopulateCollectionJumpPoints(navigationContext, (IEnumerable<CollectionJumpPointModel>) new CollectionJumpPointModel[1]
        {
          collectionJumpPointModel1
        }, (showTeamsOnly ? 1 : 0) != 0, (forceLoadAllTeams ? 1 : 0) != 0);
      SecureJsonResult jumpListInternal = new SecureJsonResult();
      jumpListInternal.Data = (object) source.Select<CollectionJumpPointModel, JsObject>((Func<CollectionJumpPointModel, JsObject>) (x => x.ToJson()));
      jumpListInternal.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) jumpListInternal;
    }

    [HttpGet]
    [TfsTraceFilter(505080, 505090)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection | NavigationContextLevels.Project)]
    public ActionResult GetTeams(string project)
    {
      IReadOnlyCollection<WebApiTeam> webApiTeams = !string.IsNullOrEmpty(project) ? this.TfsRequestContext.GetService<ITeamService>().QueryTeamsInProject(this.TfsRequestContext, ProjectInfo.GetProjectId(project)) : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, WACommonResources.ProjectNotFound, (object) project));
      List<JsObject> jsObjectList = new List<JsObject>();
      UrlHelper url = this.TfsWebContext.Url;
      foreach (WebApiTeam webApiTeam in (IEnumerable<WebApiTeam>) webApiTeams)
      {
        JsObject jsObject = new JsObject();
        jsObject["teamHome"] = (object) url.RouteUrl((object) new
        {
          controller = "home",
          action = "index",
          routeArea = "",
          serviceHost = this.TfsRequestContext.ServiceHost.CollectionServiceHost,
          project = project,
          team = webApiTeam.Name
        });
        jsObject["name"] = (object) webApiTeam.Name;
        jsObjectList.Add(jsObject);
      }
      JsObject data = new JsObject();
      data.Add("projectName", (object) project);
      data.Add("projectHome", (object) url.RouteUrl((object) new
      {
        controller = "home",
        action = "index",
        routeArea = "",
        serviceHost = this.TfsRequestContext.ServiceHost.CollectionServiceHost,
        project = project
      }));
      data.Add("teams", (object) jsObjectList);
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    [TfsTraceFilter(505091, 505092)]
    [AcceptNavigationLevels(NavigationContextLevels.All)]
    public void RemoveNavigationMRUEntry(int mruEntryHashCode) => this.TfsWebContext.RemoveMRUNavigationEntry(mruEntryHashCode);

    [HttpPost]
    [AcceptNavigationLevels(NavigationContextLevels.All)]
    public void HideAboutTfsModule() => this.ToggleAboutTfsModule(false);

    [HttpPost]
    [AcceptNavigationLevels(NavigationContextLevels.All)]
    public void HideHowToModule() => this.ToggleHowToModule(false);

    [HttpPost]
    [AcceptNavigationLevels(NavigationContextLevels.Project)]
    public void HideWelcomeProjectModule() => this.ToggleWelcomeProjectModule(false);

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.All)]
    public ActionResult ShowAboutTfsModule()
    {
      this.ToggleAboutTfsModule(true);
      return (ActionResult) new RedirectResult(this.TfsWebContext.Url.RouteUrl((object) new
      {
        controller = "",
        action = "",
        routeArea = ""
      }));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.All)]
    public ActionResult ShowHowToModule()
    {
      this.ToggleHowToModule(true);
      return (ActionResult) new RedirectResult(this.TfsWebContext.Url.RouteUrl((object) new
      {
        controller = "",
        action = "",
        routeArea = ""
      }));
    }

    [AcceptNavigationLevels(NavigationContextLevels.Application | NavigationContextLevels.Collection)]
    [HttpGet]
    public ActionResult GetAllProjects()
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        IList<TeamProjectModel> teamProjectModelList = (IList<TeamProjectModel>) new List<TeamProjectModel>();
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
        TeamFoundationHostManagementService service = vssRequestContext.GetService<TeamFoundationHostManagementService>();
        foreach (TfsServiceHostDescriptor allCollection in this.TfsWebContext.GetAllCollections())
        {
          IEnumerable<CatalogNode> projectCatalogNodes = this.TfsWebContext.GetProjectCatalogNodes(allCollection.Id);
          if (projectCatalogNodes.Count<CatalogNode>() != 0)
          {
            using (IVssRequestContext requestContext = service.BeginUserRequest(vssRequestContext, allCollection.Id, vssRequestContext.UserContext, false))
            {
              foreach (CatalogNode node in projectCatalogNodes)
              {
                try
                {
                  teamProjectModelList.Add(new TeamProjectModel(requestContext, node)
                  {
                    CollectionName = allCollection.Name
                  });
                }
                catch (Exception ex)
                {
                  this.TfsRequestContext.TraceException(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, ex);
                }
              }
            }
          }
        }
        JsObject data = new JsObject();
        data.Add("projects", (object) teamProjectModelList);
        return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
      }
      JsObject data1 = new JsObject();
      data1.Add("projects", (object) null);
      return (ActionResult) this.Json((object) data1, JsonRequestBehavior.AllowGet);
    }

    [AcceptNavigationLevels(NavigationContextLevels.Deployment | NavigationContextLevels.Application | NavigationContextLevels.Collection)]
    [HttpGet]
    public ActionResult GetProject(string projectName)
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        TfsServiceHostDescriptor serviceHostDescriptor = this.TfsWebContext.GetAllCollections().FirstOrDefault<TfsServiceHostDescriptor>();
        if (serviceHostDescriptor != null)
        {
          IVssRequestContext vssRequestContext1 = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
          using (IVssRequestContext vssRequestContext2 = vssRequestContext1.GetService<TeamFoundationHostManagementService>().BeginRequest(vssRequestContext1, serviceHostDescriptor.Id, RequestContextType.UserContext, true, true))
          {
            ProjectInfo projectFromName = TfsProjectHelpers.GetProjectFromName(vssRequestContext2, projectName);
            JsObject data = new JsObject();
            data.Add("project", (object) projectFromName);
            data.Add("projectRef", (object) projectFromName.ToTeamProjectReference(vssRequestContext2));
            return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
          }
        }
      }
      JsObject data1 = new JsObject();
      data1.Add("project", (object) null);
      return (ActionResult) this.Json((object) data1, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [AcceptNavigationLevels(NavigationContextLevels.All)]
    public ActionResult GetNewFeaturesContext(string areaName)
    {
      var data = new{ IsEnabled = false };
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    private void PopulateCollectionJumpPoints(
      NavigationContextModel navigationContext,
      IEnumerable<CollectionJumpPointModel> collectionModels,
      bool showTeamsOnly,
      bool forceLoadAllTeams)
    {
      Dictionary<string, ProjectJumpPointModel> projectCache = new Dictionary<string, ProjectJumpPointModel>();
      List<Guid> source = new List<Guid>();
      Uri currentProjectUri = this.TfsWebContext.CurrentProjectUri;
      foreach (CollectionJumpPointModel collectionModel in collectionModels)
      {
        if (collectionModel.IsOnline)
        {
          foreach (CatalogNode projectCatalogNode in this.TfsWebContext.GetProjectCatalogNodes(collectionModel.Id))
          {
            ProjectJumpPointModel projectJumpPointModel = new ProjectJumpPointModel(this.TfsWebContext, navigationContext, projectCatalogNode, collectionModel);
            if (showTeamsOnly && TFStringComparer.ProjectUri.Equals((object) currentProjectUri, (object) projectJumpPointModel.Uri))
            {
              projectCache[projectJumpPointModel.Uri] = projectJumpPointModel;
              collectionModel.Projects.Add(projectJumpPointModel);
              source.Add(collectionModel.Id);
              break;
            }
            if (!showTeamsOnly)
            {
              projectCache[projectJumpPointModel.Uri] = projectJumpPointModel;
              collectionModel.Projects.Add(projectJumpPointModel);
              source.Add(collectionModel.Id);
            }
          }
          collectionModel.Projects.Sort();
          collectionModel.HasMore = false;
        }
      }
      if (projectCache.Count <= 0 || !(showTeamsOnly | forceLoadAllTeams) && projectCache.Count != 1)
        return;
      this.PopulateProjectJumpPointsTeamInfo(navigationContext, source.Distinct<Guid>(), projectCache);
    }

    private void ToggleAboutTfsModule(bool show)
    {
      using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(this.TfsRequestContext))
        userSettingsHive.WriteSetting<bool>("/AccountHomepage/AboutTfs", show);
    }

    private void ToggleHowToModule(bool show)
    {
      using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(this.TfsRequestContext))
        userSettingsHive.WriteSetting<bool>("/TeamHomepage/HowTo", show);
    }

    private void ToggleWelcomeProjectModule(bool show)
    {
      using (WebProjectSettingsHive projectSettingsHive = new WebProjectSettingsHive(this.TfsRequestContext, this.TfsWebContext.CurrentProjectGuid))
        projectSettingsHive.WriteSetting<bool>("/ProjectHomepage/Welcome", show);
    }

    private void PopulateProjectJumpPointsTeamInfo(
      NavigationContextModel navigationContext,
      IEnumerable<Guid> collections,
      Dictionary<string, ProjectJumpPointModel> projectCache)
    {
      IVssRequestContext vssRequestContext1 = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      TeamFoundationHostManagementService service1 = vssRequestContext1.GetService<TeamFoundationHostManagementService>();
      List<string> stringList = new List<string>();
      foreach (Guid collection in collections)
      {
        using (IVssRequestContext vssRequestContext2 = service1.BeginUserRequest(vssRequestContext1, collection, vssRequestContext1.UserContext, false))
        {
          ITeamService service2 = vssRequestContext2.GetService<ITeamService>();
          foreach (ProjectJumpPointModel project in projectCache.Values)
          {
            try
            {
              foreach (WebApiTeam webApiTeam in (IEnumerable<WebApiTeam>) service2.QueryTeamsInProject(vssRequestContext2, ProjectInfo.GetProjectId(project.Uri)))
              {
                TeamIdentityViewModel identityViewModel = IdentityImageUtility.GetIdentityViewModel<TeamIdentityViewModel>(IdentityUtil.Convert(webApiTeam.Identity), true);
                TeamJumpPointModel teamJumpPointModel = new TeamJumpPointModel(this.TfsWebContext, navigationContext, identityViewModel, project);
                project.Teams.Add(teamJumpPointModel);
              }
              project.Teams.Sort();
              project.TeamsPopulated = true;
              project.DefaultTeamGuid = service2.GetDefaultTeamId(vssRequestContext2, ProjectInfo.GetProjectId(project.Uri)).ToString();
            }
            catch (GroupSecuritySubsystemServiceException ex)
            {
              stringList.Add(project.Uri);
            }
            catch (UnauthorizedAccessException ex)
            {
              stringList.Add(project.Uri);
            }
          }
        }
      }
      for (int index1 = 0; index1 < stringList.Count; ++index1)
      {
        ProjectJumpPointModel projectJumpPointModel = projectCache[stringList[index1]];
        projectCache.Remove(stringList[index1]);
        for (int index2 = 0; index2 < projectJumpPointModel.Parent.Projects.Count; ++index2)
        {
          if (projectJumpPointModel.Parent.Projects[index2].Uri.Equals(stringList[index1]))
          {
            projectJumpPointModel.Parent.Projects.RemoveAt(index2);
            break;
          }
        }
      }
    }

    [HttpGet]
    [TfsTraceFilter(500660, 500670)]
    public ActionResult GetUserProfile()
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      return (ActionResult) this.Json((object) new UserProfileModel(this.TfsWebContext).ToJson(), JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    [ValidateInput(false)]
    [TfsTraceFilter(500680, 500690)]
    public ActionResult UpdateUserProfile(string updatePackage)
    {
      TeamFoundationIdentityService foundationIdentityService = !this.TfsRequestContext.IsImpersonating ? this.TfsRequestContext.GetService<TeamFoundationIdentityService>() : throw new InvalidOperationException();
      UserPreferencesModel preferencesModel = !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? new JavaScriptSerializer().Deserialize<UserPreferencesModel>(updatePackage) : JsonConvert.DeserializeObject<UserPreferencesModel>(updatePackage);
      if (!string.IsNullOrEmpty(preferencesModel.PreferredEmail))
      {
        MailAddress mailAddress = (MailAddress) null;
        if (ArgumentUtility.IsValidEmailAddress(preferencesModel.PreferredEmail))
        {
          try
          {
            mailAddress = new MailAddress(preferencesModel.PreferredEmail);
          }
          catch (FormatException ex)
          {
          }
        }
        if (mailAddress == null)
          throw new ArgumentException(string.Format(WACommonResources.InvalidEmailAddressFormat, (object) preferencesModel.PreferredEmail));
      }
      bool flag1 = false;
      if (preferencesModel.ResetDisplayName)
      {
        if (this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          foundationIdentityService.SetCustomDisplayName(this.TfsRequestContext, string.Empty);
      }
      else if (preferencesModel.CustomDisplayName != null && !string.Equals(this.TfsWebContext.CurrentUserIdentity.CustomDisplayName ?? this.TfsWebContext.CurrentUserIdentity.DisplayName, preferencesModel.CustomDisplayName, StringComparison.CurrentCulture))
      {
        if (string.IsNullOrWhiteSpace(preferencesModel.CustomDisplayName))
          throw new InvalidDisplayNameException(FrameworkResources.CustomDisplayNameError());
        flag1 = true;
      }
      UserPreferences userPreferences = preferencesModel.GetUserPreferences(this.HttpContext);
      this.TfsRequestContext.GetService<IUserPreferencesService>().SetUserPreferences(this.TfsRequestContext, userPreferences, false);
      if (preferencesModel.ResetEmail)
      {
        foundationIdentityService.SetPreferredEmailAddress(this.TfsRequestContext, string.Empty);
      }
      else
      {
        string preferredEmailAddress = foundationIdentityService.GetPreferredEmailAddress(this.TfsRequestContext, this.TfsWebContext.CurrentUserIdentity.TeamFoundationId, false);
        if (!string.IsNullOrWhiteSpace(preferencesModel.PreferredEmail) && !string.Equals(preferredEmailAddress, preferencesModel.PreferredEmail, StringComparison.CurrentCultureIgnoreCase))
          foundationIdentityService.SetPreferredEmailAddress(this.TfsRequestContext, preferencesModel.PreferredEmail);
      }
      if (flag1)
      {
        foundationIdentityService.SetCustomDisplayName(this.TfsRequestContext, preferencesModel.CustomDisplayName);
        if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          bool flag2 = Guid.TryParse(this.TfsWebContext.CurrentUserIdentity.GetAttribute("Domain", string.Empty), out Guid _);
          AuthenticatedUserCookie.Set(this.TfsRequestContext, preferencesModel.CustomDisplayName, new bool?(flag2));
          if (!this.TfsRequestContext.IsHosted() && this.TfsRequestContext.IsFeatureEnabled("WebAccess.Profile.ProfileImageService"))
            IdentityImageService.InvalidateIdentityImage(this.TfsRequestContext, this.TfsWebContext.CurrentIdentity);
        }
      }
      return (ActionResult) new EmptyResult();
    }

    [HttpGet]
    [TfsTraceFilter(505110, 505120)]
    public ActionResult MyTeams(int? maxResults)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      if (!maxResults.HasValue)
        maxResults = new int?(100);
      if (maxResults.Value < 0)
        maxResults = new int?(int.MaxValue);
      JsObject data = new JsObject();
      data.Add("collections", (object) this.TfsWebContext.GetAllCollections().Take<TfsServiceHostDescriptor>(maxResults.Value).Select<TfsServiceHostDescriptor, JsObject>((Func<TfsServiceHostDescriptor, JsObject>) (x => x.ToJson())));
      data.Add("teams", (object) this.TfsWebContext.GetMyTeams().Take<TeamData>(maxResults.Value).Select(teamData => new
      {
        team = teamData.Team.ToJson(),
        project = teamData.ProjectInfo.ToJson(),
        collection = teamData.ProjectCollection.ToJson()
      }));
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [ValidateInput(false)]
    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(505130, 505140)]
    public void ReadIdentitiesAsync(ICollection<Guid> ids, bool includePreferredEmail)
    {
      ArgumentUtility.CheckForNull<ICollection<Guid>>(ids, nameof (ids));
      this.AsyncManager.OutstandingOperations.Increment();
      this.AsyncManager.Parameters["identities"] = (object) ((IEnumerable<TeamFoundationIdentity>) this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, ids.ToArray<Guid>(), MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[0])).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (identity => identity != null));
      this.AsyncManager.Parameters[nameof (includePreferredEmail)] = (object) includePreferredEmail;
      this.AsyncManager.OutstandingOperations.Decrement();
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(505150, 505160)]
    public ActionResult ReadIdentitiesCompleted()
    {
      IEnumerable<TeamFoundationIdentity> parameter = this.AsyncManager.Parameters["identities"] as IEnumerable<TeamFoundationIdentity>;
      return !(bool) this.AsyncManager.Parameters["includePreferredEmail"] ? (ActionResult) this.Json((object) parameter.Select<TeamFoundationIdentity, JsObject>((Func<TeamFoundationIdentity, JsObject>) (identity => JsonExtensions.ToJson(IdentityUtil.Convert(identity))))) : (ActionResult) this.Json((object) parameter.Select<TeamFoundationIdentity, JsObject>((Func<TeamFoundationIdentity, JsObject>) (identity => IdentityUtil.Convert(identity).ToJsonIncludingPreferredEmail(this.TfsRequestContext))));
    }

    [ValidateInput(false)]
    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(505170, 505180)]
    public ActionResult Base64EncodeValue(string value)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (value));
      return (ActionResult) this.Json((object) Convert.ToBase64String(Encoding.Unicode.GetBytes(value)), JsonRequestBehavior.AllowGet);
    }

    [ValidateInput(false)]
    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult GetTeamProjectMruList(int maxCount = 2147483647)
    {
      IEnumerable<NavigationExtensions.MruItem> projectMruList = NavigationExtensions.GetProjectMRUList(this.TfsWebContext);
      if (projectMruList != null)
        projectMruList = projectMruList.Take<NavigationExtensions.MruItem>(maxCount);
      return (ActionResult) this.Json((object) new
      {
        entries = projectMruList
      }, JsonRequestBehavior.AllowGet);
    }

    private IEnumerable<MRUNavigationContextEntry> GetNavigationEntries(
      NavigationContextLevels targetLevel = NavigationContextLevels.Project)
    {
      return ((IEnumerable<MRUNavigationContextEntry>) this.TfsWebContext.MruNavigationContexts).Where<MRUNavigationContextEntry>((Func<MRUNavigationContextEntry, bool>) (mru => mru.TopMostLevel >= targetLevel));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult MruNavigationContexts(int maxCount = 2147483647) => (ActionResult) this.Json((object) this.GetNavigationEntries().Select<MRUNavigationContextEntry, JsObject>((Func<MRUNavigationContextEntry, JsObject>) (mru => mru.ToJson())), JsonRequestBehavior.AllowGet);

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult MruContextsWithAccountHubGroups(int maxCount = 2147483647)
    {
      List<object> data = new List<object>();
      data.AddRange((IEnumerable<object>) this.GetNavigationEntries().Take<MRUNavigationContextEntry>(maxCount).Select<MRUNavigationContextEntry, JsObject>((Func<MRUNavigationContextEntry, JsObject>) (mru => mru.ToJson())));
      HubsContext hubsContext = new HubsContext();
      hubsContext.PopulateFromContributions((WebContext) this.TfsWebContext, hubGroupsCollectionContributionId: HubsManagement.GetHubGroupsCollectionContributionId(NavigationContextLevels.Collection), navigationRelativeRequestUrl: string.Empty, navigationRootUrl: string.Empty);
      data.AddRange((IEnumerable<object>) hubsContext.HubGroups.Where<HubGroup>((Func<HubGroup, bool>) (hg => !hg.Hidden && !hg.NonCollapsible && hg.Uri != null)).Select(hg => new
      {
        id = hg.Id,
        name = hg.BuiltIn ? string.Format((IFormatProvider) CultureInfo.CurrentUICulture, WebAccessServerResources.AccountHubGroupsMyText, (object) hg.Name) : hg.Name,
        uri = hg.Uri,
        icon = hg.Icon,
        builtIn = hg.BuiltIn
      }));
      if (this.TfsWebContext.IsHosted)
      {
        object obj = this.PopulateOrgNavigationDetailsIfAvailable();
        if (obj != null)
          data.Add(obj);
      }
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult GetCollections() => (ActionResult) this.Json((object) TfsHelpers.GetAccessibleCollections(this.TfsRequestContext).Select<HostProperties, TfsServiceHostDescriptor>((Func<HostProperties, TfsServiceHostDescriptor>) (ch => new TfsServiceHostDescriptor(ch, ch.VirtualPath(this.TfsRequestContext)))).OrderBy<TfsServiceHostDescriptor, string>((Func<TfsServiceHostDescriptor, string>) (ch => ch.Name), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).Select(d => new
    {
      name = d.Name,
      uri = VirtualPathUtility.ToAbsolute(d.VirtualDirectory)
    }), JsonRequestBehavior.AllowGet);

    [ValidateInput(false)]
    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "Default")]
    public ActionResult GetNews(int maxCount = 2147483647)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      return (ActionResult) this.Json((object) vssRequestContext.GetService<NewsFeedService>().GetNewsResult(vssRequestContext, maxCount), JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetResourceUsage()
    {
      ResourceUsageModel data = new ResourceUsageModel()
      {
        Resources = (IList<ResourceModel>) new List<ResourceModel>()
      };
      foreach (IOfferSubscription resource in this.TfsRequestContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptions(this.TfsRequestContext).Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (o =>
      {
        if (o == null)
          return false;
        MeterCategory? category = o.OfferMeter?.Category;
        MeterCategory meterCategory = MeterCategory.Legacy;
        return category.GetValueOrDefault() == meterCategory & category.HasValue;
      })).ToList<IOfferSubscription>())
      {
        switch (resource.OfferMeter.Name)
        {
          case "Build":
            ResourceUsageModel resourceUsageModel1 = data;
            DateTime dateTime = resource.ResetDate;
            dateTime = dateTime.AddMonths(-1);
            string str1 = dateTime.ToString("MMM d");
            resourceUsageModel1.StartDate = str1;
            ResourceUsageModel resourceUsageModel2 = data;
            dateTime = resource.ResetDate;
            dateTime = dateTime.AddDays(-1.0);
            string str2 = dateTime.ToString("MMM d");
            resourceUsageModel2.EndDate = str2;
            data.Resources.Insert(0, ApiCommonController.CreateResourceModel(WACommonResources.ResourceUsageBuildName, WACommonResources.ResourceUsageBuildUnit, resource, 50000));
            continue;
          case "LoadTest":
            data.Resources.Add(ApiCommonController.CreateResourceModel(WACommonResources.ResourceUsageLoadTestingName, WACommonResources.ResourceUsageLoadTestingUnit, resource, 100000000));
            continue;
          default:
            continue;
        }
      }
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [ValidateInput(false)]
    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult HideAnnouncementModule(string announcementType, int id)
    {
      if (id <= 0)
        throw new ArgumentException("The id should not be less than equal to 0.", nameof (id));
      string announcementPath = this.GetAnnouncementPath(announcementType);
      using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(this.TfsRequestContext))
        userSettingsHive.WriteSetting<int>(announcementPath, id);
      return (ActionResult) this.Json((object) true);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.All)]
    public ActionResult ShowAnnouncementModules()
    {
      using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(this.TfsRequestContext))
      {
        userSettingsHive.WriteSetting<int>("/WebAccess/Announcement/UserLatest", 0);
        userSettingsHive.WriteSetting<int>("/Admin/OpenPlatformAnnouncement/UserLatest", 0);
        userSettingsHive.WriteSetting<int>("/PowerBI/PowerBIAnnouncement/UserLatest", 0);
        userSettingsHive.WriteSetting<int>("/ELS/ElsVsoIntegrationAnnouncement/UserLatest", 0);
      }
      return (ActionResult) new RedirectResult(this.TfsWebContext.Url.RouteUrl((object) new
      {
        controller = "",
        action = "",
        routeArea = ""
      }));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.All)]
    public ActionResult ShowVideoModules()
    {
      if (this.TfsWebContext.IsFeatureAvailable("WebAccess.Account.GettingStartedVideo"))
      {
        using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(this.TfsRequestContext))
          userSettingsHive.WriteSetting<bool>("/WebAccess/GettingStartedVideo", true);
      }
      return (ActionResult) new RedirectResult(this.TfsWebContext.Url.RouteUrl((object) new
      {
        controller = "",
        action = "",
        routeArea = ""
      }));
    }

    private object PopulateOrgNavigationDetailsIfAvailable()
    {
      Microsoft.VisualStudio.Services.Organization.Organization orgNavigationValue1 = ApiCommonController.GetCollectionToOrgNavigationValue<Microsoft.VisualStudio.Services.Organization.Organization>(this.TfsWebContext.TfsRequestContext, new Func<IVssRequestContext, Microsoft.VisualStudio.Services.Organization.Organization>(FunctionToGetOrganization));
      if (!orgNavigationValue1.IsActivated)
        return (object) null;
      string orgNavigationValue2 = ApiCommonController.GetCollectionToOrgNavigationValue<string>(this.TfsWebContext.TfsRequestContext, new Func<IVssRequestContext, string>(FunctionToGetPortalUrl));
      if (string.IsNullOrWhiteSpace(orgNavigationValue2))
        return (object) null;
      string name = orgNavigationValue1.Name;
      return string.IsNullOrWhiteSpace(name) ? (object) null : (object) new
      {
        isOrgHomePageUrl = true,
        orgHomePageUrl = orgNavigationValue2,
        title = string.Format(WebAccessServerResources.BrowseOrganizationFromCollectionLabel, (object) name)
      };

      static Microsoft.VisualStudio.Services.Organization.Organization FunctionToGetOrganization(
        IVssRequestContext requestContext)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
        using (vssRequestContext.CreateAsyncTimeOutScope(ApiCommonController.GetOrganizationInfoCallTimeout(vssRequestContext)))
          return vssRequestContext.GetService<IOrganizationService>().GetOrganization(vssRequestContext, (IEnumerable<string>) new List<string>());
      }

      static string FunctionToGetPortalUrl(IVssRequestContext requestContext)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
        using (vssRequestContext.CreateAsyncTimeOutScope(ApiCommonController.GetOrganizationInfoCallTimeout(vssRequestContext)))
          return vssRequestContext.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext, new Guid("0000004D-0000-8888-8000-000000000000"), AccessMappingConstants.ClientAccessMappingMoniker);
      }
    }

    private string GetAnnouncementPath(string announcementType)
    {
      switch (announcementType)
      {
        case "AzureBenefits":
          return "/AccountHome/AzureBenefits";
        case "Analytics":
          return "/WebAccess/Announcement/UserLatest";
        case "OpenPlatform":
          return "/Admin/OpenPlatformAnnouncement/UserLatest";
        case "PowerBI":
          return "/PowerBI/PowerBIAnnouncement/UserLatest";
        case "ElsVsoIntegration":
          return "/ELS/ElsVsoIntegrationAnnouncement/UserLatest";
        default:
          throw new ArgumentException("Invalid announcement type", nameof (announcementType));
      }
    }

    private static ResourceModel CreateResourceModel(
      string resourceTitle,
      string unit,
      IOfferSubscription resource,
      int resourceDefaultMaxLimit)
    {
      string str1 = resource.IncludedQuantity.ToString("N0");
      string str2 = resource.CommittedQuantity.ToString("N0");
      string str3 = resource.MaximumQuantity != resourceDefaultMaxLimit ? resource.MaximumQuantity.ToString("N0") : "0";
      return new ResourceModel()
      {
        Title = resourceTitle,
        CurrentQuantity = resource.IsPaidBillingEnabled ? str2 : (resource.CommittedQuantity < resource.IncludedQuantity ? str2 : str1),
        MaximumQuantity = str3,
        IncludedQuantity = str1,
        IsPaid = resource.IsPaidBillingEnabled,
        IsBlocked = !resource.IsUseable,
        Unit = unit,
        BlockedReason = !resource.IsUseable ? (resource.DisabledReason.HasFlag((Enum) ResourceStatusReason.NoAzureSubscription) ? WACommonResources.ResourceUsageBlockedNoSubscription : WACommonResources.ResourceUsageBlockedWithSubscription) : string.Empty
      };
    }

    [TfsTraceFilter(505221, 505230)]
    private ActionResult GetDdsAvatarInternal(
      IVssRequestContext requestContext,
      string tfId,
      bool getJson = false)
    {
      try
      {
        IDirectoryEntity fromTfIdentifier = DirectoryDiscoveryServiceHelper.GetEntityFromTFIdentifier(requestContext, tfId);
        if (fromTfIdentifier == null)
          return this.GetDdsEntityDefaultImage("user", getJson);
        string imageType = string.Empty;
        if (fromTfIdentifier is IDirectoryUser)
          imageType = "user";
        else if (fromTfIdentifier is IDirectoryGroup)
          imageType = !(fromTfIdentifier.OriginDirectory != "vsd") ? "vsogroup" : "aadgroup";
        byte[] entityAvatarBytes = DirectoryDiscoveryServiceHelper.GetEntityAvatarBytes(requestContext, fromTfIdentifier.EntityId);
        if (entityAvatarBytes == null)
          return this.GetDdsEntityDefaultImage(imageType, getJson);
        using (Stream stream = (Stream) new MemoryStream(entityAvatarBytes))
        {
          using (Image image = Image.FromStream(stream))
          {
            string lowerInvariant = new MediaTypeHeaderValue(image.GetMimeType()).MediaType.ToLowerInvariant();
            return getJson ? (ActionResult) this.Json((object) new
            {
              avatar = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "data:{0};base64,{1}", (object) lowerInvariant, (object) Convert.ToBase64String(entityAvatarBytes))
            }, JsonRequestBehavior.AllowGet) : (ActionResult) this.File(entityAvatarBytes, lowerInvariant);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(505229, ApiCommonController.s_traceArea, TfsTraceLayers.Controller, ex);
        return this.GetDdsEntityDefaultImage("user", getJson);
      }
    }

    [TfsTraceFilter(505251, 505260)]
    private ActionResult GetDdsEntityDefaultImage(string imageType, bool getJson = false)
    {
      string contentType;
      return getJson ? (ActionResult) this.Json((object) new
      {
        avatar = this.GetDdsEntityDefaultImageUri(imageType, (Func<string, string>) (img => StaticResources.Versioned.Content.GetLocalLocation(img)), out string _)
      }, JsonRequestBehavior.AllowGet) : (ActionResult) this.File(this.GetDdsEntityDefaultImageUri(imageType, (Func<string, string>) (img => StaticResources.Versioned.Content.GetPhysicalLocation(img)), out contentType), contentType);
    }

    [TfsTraceFilter(505261, 505270)]
    private string GetDdsEntityDefaultImageUri(
      string imageType,
      Func<string, string> uriBuilder,
      out string contentType)
    {
      contentType = "image/png";
      switch (imageType.ToLowerInvariant().Trim())
      {
        case "vsogroup":
          return uriBuilder("ip-content-vso-group-default.png");
        case "aadgroup":
          return uriBuilder("ip-content-aad-group-default.png");
        default:
          contentType = "image/svg+xml";
          return uriBuilder("User.svg");
      }
    }

    public new virtual IVssRequestContext TfsRequestContext
    {
      get
      {
        if (this.m_tfsRequestContext == null)
          this.m_tfsRequestContext = base.TfsRequestContext;
        return this.m_tfsRequestContext;
      }
      set => this.m_tfsRequestContext = value;
    }

    private static T GetCollectionToOrgNavigationValue<T>(
      IVssRequestContext requestContext,
      Func<IVssRequestContext, T> functionToFetchValue)
    {
      T result = default (T);
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "OpenALM.").AndCommandKey((CommandKey) "GetOrganizationNavigationInfoTimeout").AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(ApiCommonController.GetOrganizationInfoCallTimeout(requestContext)));
      CommandService commandService = new CommandService(requestContext, setter, (Action) (() => result = functionToFetchValue(requestContext)));
      try
      {
        commandService.Execute();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, ex);
      }
      return result;
    }

    private static TimeSpan GetOrganizationInfoCallTimeout(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return TimeSpan.FromMilliseconds((double) vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, (RegistryQuery) "/WebAccess/GetOrganizationNavigationInfoTimeoutTimeout", ApiCommonController.defaultGetOrganizationNavigationInfoTimeoutTimeoutMiliseconds));
    }

    public static class AzureBenefitsAnnouncement
    {
      public const string UserSettingsPath = "/AccountHome/AzureBenefits";
      public const string ViewDataKey = "AzureBenefitsAnnouncementData";
      public const string AnnouncementType = "AzureBenefits";
    }
  }
}
