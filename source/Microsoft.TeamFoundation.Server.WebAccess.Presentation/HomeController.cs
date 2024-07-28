// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Presentation.HomeController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Presentation, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4ED0029A-5609-48A8-995C-ADAB0E762821
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Presentation.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Boards.Settings;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using Microsoft.TeamFoundation.Server.WebAccess.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Presentation
{
  [SupportedRouteArea(NavigationContextLevels.ApplicationAll)]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  [RegisterHubMruPage(true)]
  [RemoveMruEntryOnRenamedProject]
  [RemoveMruEntryonDeletedProjectTeam]
  public class HomeController : TfsController
  {
    private const string s_GettingStartedVideoRegistryPath = "/Configuration/Account/WebAccess/GettingStartedVideo";
    private const string s_GettingStartedVideoHive = "/WebAccess/GettingStartedVideo";
    private const string s_FeatureFlagNewProjectCreateFlow = "WebAccess.Account.NewProjectCreateFlow";
    private const string s_CreateProjectUrl = "~/_createproject";
    private const string s_AccountFirst = "first";
    private const string s_AgileRedirection = "agile";
    private const string s_VersionControlRedirection = "vc";
    private const string s_LoadTestRedirection = "test";

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(530000, 530010)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult Index(string account, string scenario)
    {
      switch (this.NavigationContext.TopMostLevel)
      {
        case NavigationContextLevels.Deployment:
        case NavigationContextLevels.Application:
        case NavigationContextLevels.Collection:
          return this.TfsWebContext.IsHosted ? this.HostedApplicationIndex(new bool?(), new Guid?(), account, scenario) : this.ApplicationIndex();
        case NavigationContextLevels.Project:
          return this.NavigateToProjectOrTeam(true);
        case NavigationContextLevels.Team:
          return this.NavigateToProjectOrTeam(false);
        default:
          throw new NotImplementedException();
      }
    }

    private ActionResult NavigateToProjectOrTeam(bool isProject)
    {
      if (!this.TfsWebContext.FeatureContext.AreStandardFeaturesAvailable)
        return (ActionResult) this.RedirectToAction("index", "workItems");
      if (isProject && this.TfsWebContext.Team == null)
        throw new HttpException(404, string.Format((IFormatProvider) CultureInfo.InvariantCulture, PresentationServerResources.ErrorDefaultTeamNotFound, (object) this.TfsWebContext.NavigationContext.Project));
      try
      {
        return (ActionResult) this.View("~/_views/Home/ProjectIndex.aspx", (object) DashboardsViewHelper.GetViewModel(this.TfsWebContext));
      }
      catch (DashboardDoesNotExistException ex)
      {
        throw new HttpException(404, ex.Message, (Exception) ex);
      }
      catch (TeamNotFoundException ex)
      {
        throw new HttpException(404, ex.Message, (Exception) ex);
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult Check(bool includeCollectionCheck = false)
    {
      if (!this.TfsWebContext.IsHosted)
        return (ActionResult) this.HttpNotFound();
      if (!string.IsNullOrWhiteSpace(this.Request.QueryString["_"]) && includeCollectionCheck && !this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        TeamProjectCollectionProperties collectionProperties = this.TfsRequestContext.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionProperties(this.TfsRequestContext.Elevate(), ServiceHostFilterFlags.None).FirstOrDefault<TeamProjectCollectionProperties>();
        if (collectionProperties == null || collectionProperties.State != TeamFoundationServiceHostStatus.Started)
          return (ActionResult) new HttpStatusCodeResult(HttpStatusCode.NotFound);
      }
      return (ActionResult) new EmptyResult();
    }

    private HostProperties GetStartedCollection()
    {
      HostProperties collection = this.GetCollection();
      if (collection != null)
      {
        if (collection.Status == TeamFoundationServiceHostStatus.Started)
          return collection;
        IVssRequestContext requestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        ITeamFoundationHostManagementService service = this.TfsRequestContext.GetService<ITeamFoundationHostManagementService>();
        try
        {
          using (service.BeginRequest(requestContext, collection.Id, RequestContextType.UserContext))
            ;
        }
        catch (HostShutdownException ex)
        {
        }
        catch (HostCreationException ex)
        {
        }
      }
      return (HostProperties) null;
    }

    private HostProperties GetCollection()
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationHostManagementService service = vssRequestContext.GetService<ITeamFoundationHostManagementService>();
      return this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? service.QueryServiceHostPropertiesCached(vssRequestContext, this.TfsRequestContext.ServiceHost.InstanceId) : (HostProperties) service.QueryServiceHostProperties(vssRequestContext, this.TfsRequestContext.ServiceHost.OrganizationServiceHost.InstanceId, ServiceHostFilterFlags.IncludeChildren).Children.FirstOrDefault<TeamFoundationServiceHostProperties>();
    }

    private void SetVsLinksData(bool show = true)
    {
      if (!show)
        this.ViewData["VsLinksData"] = (object) this.Json((object) new
        {
          ShowVsLinks = false
        });
      else
        this.ViewData["VsLinksData"] = (object) this.Json((object) new
        {
          VsOpenLink = VisualStudioLinkHelper.GetOpenInVisualStudioUri(this.TfsWebContext, this.NavigationContext.TopMostLevel),
          ShowVsLinks = this.IsAdvancedHomepageEnabled()
        });
    }

    private bool IsAdvancedHomepageEnabled() => this.TfsWebContext.FeatureContext.IsFeatureAvailable(LicenseFeatures.AdvancedHomePageId);

    [NonAction]
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult ApplicationIndex()
    {
      this.EnsureCurrentUserHasGlobalReadAccess();
      this.SetAccountViewData(showVsLinks: false);
      return (ActionResult) this.View(nameof (ApplicationIndex));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult HostedApplicationIndex(
      bool? createTeamProject,
      Guid? collectionId,
      string account,
      string scenario)
    {
      if (!this.TfsWebContext.IsHosted)
        throw new HttpException(404, WACommonResources.PageNotFound);
      this.EnsureCurrentUserHasGlobalReadAccess();
      IVssRequestContext vssRequestContext1 = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext vssRequestContext2 = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      bool flag = this.ProjectExists(this.GetStartedCollection());
      if (createTeamProject.HasValue && createTeamProject.Value || !flag)
      {
        string createNewProjectUrl = ProjectCreationUrlHelper.GetCreateNewProjectUrl(this.TfsWebContext, this.TfsRequestContext);
        if (createNewProjectUrl != null)
          return (ActionResult) this.Redirect(createNewProjectUrl);
        if (this.TfsWebContext.IsFeatureAvailable("WebAccess.Account.NewProjectCreateFlow"))
          return (ActionResult) this.Redirect(VirtualPathUtility.ToAbsolute("~/_createproject" + this.TfsWebContext.TfsRequestContext.RequestUri().Query));
      }
      bool showVsLinks = this.IsAdvancedHomepageEnabled();
      this.SetAccountViewData(createTeamProject.HasValue && createTeamProject.Value, showVsLinks);
      this.SetStakeholderData();
      GettingStartedModel model = new GettingStartedModel();
      CachedRegistryService service1 = vssRequestContext1.GetService<CachedRegistryService>();
      model.IsProjectCreationLockdownMode = service1.GetValue<bool>(vssRequestContext1, (RegistryQuery) FrameworkServerConstants.ProjectCreationLockdownPath, false);
      CachedRegistryService service2 = vssRequestContext2.GetService<CachedRegistryService>();
      model.JobId = service2.GetValue<Guid>(vssRequestContext2, (RegistryQuery) "/Account/Configuration/ActivationJobId", false, new Guid());
      ILocationService service3 = vssRequestContext2.GetService<ILocationService>();
      model.AccountUrl = service3.GetLocationServiceUrl(vssRequestContext2, Microsoft.VisualStudio.Services.Location.LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);
      if (createTeamProject.HasValue)
      {
        model.CreateTeamProject = createTeamProject.Value;
        if (collectionId.HasValue)
        {
          model.CollectionId = collectionId.Value;
          model.CollectionExists = true;
        }
      }
      if (!model.CollectionExists)
      {
        if (this.NavigationContext.TopMostLevel == NavigationContextLevels.Collection)
        {
          model.CollectionExists = true;
        }
        else
        {
          List<TeamProjectCollectionProperties> collectionProperties = vssRequestContext2.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionProperties(vssRequestContext2.Elevate(), ServiceHostFilterFlags.None);
          model.CollectionExists = collectionProperties != null && collectionProperties.Count > 0 && collectionProperties[0].State == TeamFoundationServiceHostStatus.Started;
        }
      }
      if (model.CollectionExists)
        model.ProjectExists = flag;
      model.ShowGitProjectSupport = true;
      return (ActionResult) this.View(nameof (HostedApplicationIndex), (object) model);
    }

    private bool ProjectExists(HostProperties collection) => ((IEnumerable<CommonStructureProjectInfo>) this.GetProjectsInfo(collection)).Any<CommonStructureProjectInfo>();

    private CommonStructureProjectInfo[] GetProjectsInfo(HostProperties collection)
    {
      if (collection == null)
        return Array.Empty<CommonStructureProjectInfo>();
      IVssRequestContext requestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationHostManagementService service = this.TfsRequestContext.GetService<ITeamFoundationHostManagementService>();
      Func<IVssRequestContext, CommonStructureProjectInfo[]> func = (Func<IVssRequestContext, CommonStructureProjectInfo[]>) (collectionContext => collectionContext.GetService<ICommonStructureService>().GetWellFormedProjects(collectionContext.Elevate()));
      if (this.TfsRequestContext.ServiceHost.InstanceId == collection.Id)
        return func(this.TfsRequestContext);
      using (IVssRequestContext vssRequestContext = service.BeginRequest(requestContext, collection.Id, RequestContextType.UserContext))
        return func(vssRequestContext);
    }

    protected Dictionary<string, RouteRedirection> ProjectRedirections(object routeValues) => new Dictionary<string, RouteRedirection>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "agile",
        new RouteRedirection()
        {
          Action = "board",
          Controller = "backlogs",
          RouteValues = routeValues
        }
      },
      {
        "vc",
        new RouteRedirection()
        {
          Action = string.Empty,
          Controller = "versioncontrol",
          RouteValues = routeValues
        }
      },
      {
        "test",
        new RouteRedirection()
        {
          Action = string.Empty,
          Controller = "LoadTest",
          RouteValues = (object) new
          {
            routeArea = "",
            serviceHost = this.TfsWebContext.TfsRequestContext.ServiceHost.OrganizationServiceHost,
            project = string.Empty,
            team = string.Empty
          }
        }
      }
    };

    [OutputCache(CacheProfile = "NoCache")]
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult About() => (ActionResult) this.View();

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "NoCache")]
    public ActionResult GetCollectionData() => (ActionResult) this.Json((object) this.InitializeCollectionData(), JsonRequestBehavior.AllowGet);

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetUserCreateProjectPermission() => (ActionResult) this.Json((object) this.CanCreateProject(), JsonRequestBehavior.AllowGet);

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetUserCreateRepoPermission() => (ActionResult) this.Json((object) this.CanCreateRepo(), JsonRequestBehavior.AllowGet);

    public override string GetActivityLogCommandPrefix() => this.NavigationContext.TopMostLevel.ToString();

    private void EnsureCurrentUserHasGlobalReadAccess()
    {
      if (!this.TfsWebContext.CurrentUserHasGlobalReadAccess)
      {
        InvalidAccessException invalidAccessException = new InvalidAccessException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, PresentationServerResources.ErrorApplicationLevelAccessDenied, (object) this.TfsRequestContext.AuthenticatedUserName));
        invalidAccessException.ReportException = false;
        throw invalidAccessException;
      }
    }

    private void AddNotification(NotificationMessageModel message)
    {
      IList<NotificationMessageModel> notificationMessageModelList = (IList<NotificationMessageModel>) this.ViewData["NotificationMessages"];
      if (notificationMessageModelList == null)
        this.ViewData["NotificationMessages"] = (object) (List<NotificationMessageModel>) (notificationMessageModelList = (IList<NotificationMessageModel>) new List<NotificationMessageModel>());
      notificationMessageModelList.Add(message);
    }

    private bool IsNotificationDismissed(string notificationId)
    {
      using (WebUserSettingsHive userSettingsHive = new WebUserSettingsHive(this.TfsRequestContext))
        return userSettingsHive.ReadSetting<bool>("/notification/dismiss/" + notificationId, false);
    }

    private void SetStakeholderData() => this.ViewData["StakeholderData"] = (object) this.Json((object) new
    {
      ShowMessage = this.TfsRequestContext.IsStakeholder()
    });

    private void SetAccountViewData(bool openPCWOnLoad = false, bool showVsLinks = true)
    {
      bool flag1;
      using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(this.TfsRequestContext))
        flag1 = userSettingsHive.ReadSetting<bool>("/AccountHomepage/AboutTfs", true);
      CollectionStatus collectionStatus = this.InitializeCollectionData();
      this.ViewData["CollectionData"] = (object) this.Json((object) new
      {
        CollectionIsReady = collectionStatus.CollectionIsReady,
        ProjectExists = collectionStatus.ProjectExists
      });
      this.SetVsLinksData(showVsLinks);
      this.SetOpenPlatformAnnouncementData();
      this.SetPowerBIAnnouncementData();
      this.SetAzureBenefitsAnnouncementData();
      this.SetElsVsoIntegrationAnnouncementData();
      this.ViewData["TeamProjectsData"] = (object) this.Json((object) new
      {
        CanCreateProject = collectionStatus.CanCreateProject,
        ShowProjects = collectionStatus.ShowProjects,
        OpenPCWOnLoad = (openPCWOnLoad && !collectionStatus.ShowNewProjectControl)
      });
      this.ViewData["NewProjectControlData"] = (object) this.Json((object) new
      {
        ShowNewProjectControl = collectionStatus.ShowNewProjectControl,
        ShowNewProjectVisibilityDropDown = collectionStatus.ShowNewProjectVisibilityDropDown
      });
      this.ViewData["AboutTfsData"] = (object) this.Json((object) new
      {
        ShowAboutTfs = flag1
      });
      this.ViewData["AccountTrialData"] = (object) this.Json((object) new
      {
        CanStartAccountTrial = false
      });
      bool flag2 = this.CanSeeGettingStartedVideo();
      bool flag3 = false;
      if (flag2)
        flag3 = this.TfsWebContext.IsFeatureAvailable("WebAccess.Account.GettingStartedVideoIframe");
      this.ViewData["CanSeeGettingStartedVideo"] = (object) this.Json((object) new
      {
        CanSeeGettingStartedVideo = flag2,
        isIframeDailog = flag3
      });
      this.SetResourceUsageData();
    }

    private void SetOpenPlatformAnnouncementData()
    {
      int result = 0;
      bool flag = false;
      string announcementLink = PresentationServerResources.OpenPlatformAnnouncementLink;
      if (this.TfsWebContext.IsHosted && this.IsAdvancedHomepageEnabled() && this.TfsRequestContext.IsFeatureEnabled(PresentationServerResources.OpenPlatformAnnouncementControlFeatureFlag))
      {
        int num = 0;
        using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(this.TfsRequestContext))
          num = userSettingsHive.ReadSetting<int>("/Admin/OpenPlatformAnnouncement/UserLatest", 0);
        if (int.TryParse(PresentationServerResources.OpenPlatformAnnouncementId.Trim(), out result))
          flag = num != result;
      }
      this.ViewData["OpenPlatformAnnouncementData"] = (object) this.Json((object) new
      {
        AnnouncementType = "OpenPlatform",
        ShowAnnouncement = flag,
        AnnouncementId = result,
        Link = announcementLink
      });
    }

    private void SetPowerBIAnnouncementData()
    {
      int result = 0;
      bool flag = false;
      string announcementLink = PresentationServerResources.PowerBIAnnouncementLink;
      if (this.TfsWebContext.IsHosted && this.TfsRequestContext.IsFeatureEnabled(PresentationServerResources.PowerBIAnnouncementControlFeatureFlag))
      {
        int num = 0;
        using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(this.TfsRequestContext))
          num = userSettingsHive.ReadSetting<int>("/PowerBI/PowerBIAnnouncement/UserLatest", 0);
        if (int.TryParse(PresentationServerResources.PowerBIAnnouncementId.Trim(), out result))
          flag = num != result;
      }
      this.ViewData["PowerBIAnnouncementData"] = (object) this.Json((object) new
      {
        AnnouncementType = "PowerBI",
        ShowAnnouncement = flag,
        AnnouncementId = result,
        Link = announcementLink
      });
    }

    private void SetAzureBenefitsAnnouncementData()
    {
      int result = 0;
      bool flag = false;
      string announcementLink = PresentationServerResources.AzureBenefitsAnnouncementLink;
      if (this.TfsWebContext.IsHosted && this.TfsRequestContext.IsFeatureEnabled(PresentationServerResources.AzureBenefitsControlFeatureFlag))
      {
        int num = 0;
        using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(this.TfsRequestContext))
          num = userSettingsHive.ReadSetting<int>("/AccountHome/AzureBenefits", 0);
        if (int.TryParse(PresentationServerResources.AzureBenefitsAnnouncementId.Trim(), out result))
          flag = num != result;
      }
      this.ViewData["AzureBenefitsAnnouncementData"] = (object) this.Json((object) new
      {
        AnnouncementType = "AzureBenefits",
        ShowAnnouncement = flag,
        AnnouncementId = result,
        Link = announcementLink
      });
    }

    private void SetElsVsoIntegrationAnnouncementData()
    {
      int result = 0;
      bool flag = false;
      string str1 = PresentationServerResources.ElsVsoIntegrationAnnouncementLink;
      string str2 = "_blank";
      if (this.TfsWebContext.IsHosted && this.TfsRequestContext.IsFeatureEnabled(PresentationServerResources.ElsVsoIntegrationAnnouncementControlFeatureFlag))
      {
        int num = 0;
        using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(this.TfsRequestContext))
          num = userSettingsHive.ReadSetting<int>("/ELS/ElsVsoIntegrationAnnouncement/UserLatest", 0);
        if (int.TryParse(PresentationServerResources.ElsVsoIntegrationAnnouncementId.Trim(), out result))
          flag = num != result;
        if (this.TfsWebContext.IsFeatureAvailable("ELS.VSO.LoadTesting"))
        {
          str2 = "";
          str1 = this.Url.Action("index", "LoadTest");
        }
      }
      this.ViewData["ElsVsoIntegrationAnnouncementData"] = (object) this.Json((object) new
      {
        AnnouncementType = "ElsVsoIntegration",
        ShowAnnouncement = flag,
        AnnouncementId = result,
        Link = str1,
        NewTab = str2
      });
    }

    private void SetResourceUsageData() => this.ViewData["ResourceUsageData"] = (object) this.Json((object) new
    {
      ShowResourceUsage = (this.TfsWebContext.IsHosted && this.IsAdvancedHomepageEnabled() && this.TfsRequestContext.IsFeatureEnabled("WebAccess.ResourceUsage"))
    });

    private CollectionStatus InitializeCollectionData()
    {
      CollectionStatus collectionStatus = new CollectionStatus()
      {
        CollectionIsReady = true,
        CanCreateProject = false,
        ShowNewProjectControl = false,
        ShowNewProjectVisibilityDropDown = false,
        ShowProjects = true,
        HasCollectionPermission = true
      };
      if (this.TfsWebContext.IsHosted)
      {
        this.TfsRequestContext.To(TeamFoundationHostType.Application);
        HostProperties collection = this.GetStartedCollection();
        collectionStatus.CollectionIsReady = collection != null;
        collectionStatus.CanCreateProject = collectionStatus.CollectionIsReady;
        bool flag = this.ProjectExists(collection);
        collectionStatus.ShowNewProjectControl = !flag && collectionStatus.CollectionIsReady && this.CanCreateProject();
        collectionStatus.ProjectExists = flag;
        if (collection == null)
          collection = this.GetCollection();
        if (collection == null)
          collectionStatus.HasCollectionPermission = false;
        if (collectionStatus.ShowNewProjectControl)
        {
          collectionStatus.ShowNewProjectVisibilityDropDown = MicrosoftAadHelpers.IsMicrosoftTenant(this.TfsRequestContext);
          collectionStatus.ShowProjects = false;
        }
      }
      return collectionStatus;
    }

    private bool CanCreateProject()
    {
      if (this.TfsRequestContext.IsStakeholder())
        return false;
      HostProperties startedCollection = this.GetStartedCollection();
      if (startedCollection == null)
        return false;
      IVssRequestContext requestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      using (IVssRequestContext vssRequestContext = this.TfsRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(requestContext, startedCollection.Id, RequestContextType.UserContext))
        return vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.TeamProjectCollectionNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.TeamProjectCollectionNamespaceToken, AuthorizationNamespacePermissions.CreateProjects);
    }

    private bool CanCreateRepo() => false;

    private bool CanSeeGettingStartedVideo()
    {
      bool flag1 = this.TfsWebContext.IsFeatureAvailable("WebAccess.Account.GettingStartedVideo");
      if (!this.TfsWebContext.IsHosted)
        return false;
      bool flag2 = false;
      if (flag1)
      {
        using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(this.TfsRequestContext))
        {
          flag2 = userSettingsHive.ReadSetting<bool>("/WebAccess/GettingStartedVideo", false);
          if (flag2)
            userSettingsHive.WriteSetting<bool>("/WebAccess/GettingStartedVideo", false);
        }
      }
      return flag2 ? flag1 : this.IsFirstTimeLoadingAccountPage() & flag1;
    }

    private bool IsFirstTimeLoadingAccountPage()
    {
      if (this.IsNamespaceAdmin())
      {
        IVssRegistryService service = this.TfsRequestContext.GetService<IVssRegistryService>();
        if (service.GetValue<bool>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Account/WebAccess/GettingStartedVideo", false))
        {
          service.SetValue<bool>(this.TfsRequestContext, "/Configuration/Account/WebAccess/GettingStartedVideo", false);
          return true;
        }
      }
      return false;
    }

    private bool IsNamespaceAdmin()
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      return vssRequestContext.GetService<TeamFoundationIdentityService>().IsMember(vssRequestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, vssRequestContext.UserContext);
    }

    protected override void OnActionExecuted(ActionExecutedContext context)
    {
      base.OnActionExecuted(context);
      this.ViewData["IsReponsiveLayout"] = (object) true;
    }
  }
}
