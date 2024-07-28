// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AdminSecurityController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [ValidateInput(false)]
  [SupportedRouteArea("Admin", NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  [DemandFeature("65AC9DB3-BB0A-42fe-B584-A690FB0D817B", true)]
  public class AdminSecurityController : AdminAreaController
  {
    public AdminSecurityController() => this.m_executeContributedRequestHandlers = true;

    [HttpGet]
    [TfsTraceFilter(500180, 500190)]
    public ActionResult Index(
      Guid? permissionSet,
      string token,
      string style,
      string tokenDisplayVal,
      bool? showAllGroupsIfCollection,
      bool isOrganizationLevel = false,
      bool controlManagesFocus = true,
      bool allowAADSearchInHosted = false)
    {
      JsObject jsObject = new JsObject();
      jsObject["toggleButtonCollapsedTooltip"] = (object) AdminServerResources.ExpandIdentityPanel;
      jsObject["toggleButtonExpandedTooltip"] = (object) AdminServerResources.CollapseIdentityPanel;
      Dictionary<string, JsObject> dictionary = new Dictionary<string, JsObject>();
      dictionary.Add("LeftHubSplitter", jsObject);
      // ISSUE: reference to a compiler-generated field
      if (AdminSecurityController.\u003C\u003Eo__1.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AdminSecurityController.\u003C\u003Eo__1.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, Dictionary<string, JsObject>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "SplitterOptions", typeof (AdminSecurityController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = AdminSecurityController.\u003C\u003Eo__1.\u003C\u003Ep__0.Target((CallSite) AdminSecurityController.\u003C\u003Eo__1.\u003C\u003Ep__0, this.ViewBag, dictionary);
      if (!permissionSet.HasValue && string.IsNullOrEmpty(token))
      {
        if (isOrganizationLevel && !AdminSecurityController.IsOrganizationActivated(this.TfsRequestContext))
        {
          ManageViewModel model = new ManageViewModel();
          this.ViewData["IsInactiveOrganization"] = (object) true;
          return (ActionResult) this.View("OrganizationSecurity", (object) model);
        }
        this.ViewData["IsInactiveOrganization"] = (object) false;
        ManageViewModel manageViewModel = this.GetManageViewModel(isOrganizationLevel);
        return (ActionResult) this.View(AdminSecurityController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext) ? "OrganizationSecurity" : "Manage", (object) manageViewModel);
      }
      SecurityNamespacePermissionsManager permissionsManager;
      SecurityModel securityModel = this.CreateSecurityModel(permissionSet, token, tokenDisplayVal, out permissionsManager, showAllGroupsIfCollection, isOrganizationLevel, controlManagesFocus, allowAADSearchInHosted);
      this.ViewData["Title"] = (object) (securityModel.ViewTitle ?? AdminResources.Permissions);
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer"))
        this.ViewData["PermissionsData"] = (object) JsonConvert.SerializeObject((object) securityModel.ToJson());
      else
        this.ViewData["PermissionsData"] = (object) new JavaScriptSerializer().Serialize((object) securityModel.ToJson());
      if (!permissionsManager.UserHasReadAccess)
      {
        if (!string.IsNullOrEmpty(style) && style.Equals("min", StringComparison.OrdinalIgnoreCase))
          throw new TeamFoundationSecurityServiceException(AdminServerResources.InvalidPermissionToViewObject);
        this.ViewData["Error"] = (object) AdminServerResources.InvalidPermissionToViewObject;
        return (ActionResult) this.View("Error");
      }
      this.ViewData["NewAdminUi"] = (object) false;
      this.ViewData["MinTemplateNewAddDialog"] = (object) false;
      switch (string.IsNullOrEmpty(style) ? style : style.ToLowerInvariant())
      {
        case "vs":
          this.ViewData.Chromeless(true);
          return (ActionResult) this.View("Security", (object) securityModel);
        case "min":
          this.ViewData["MinTemplateNewAddDialog"] = (object) true;
          return (ActionResult) this.View("SecurityMin", (object) securityModel);
        case "mincontrol":
          this.ViewData["MinTemplateNewAddDialog"] = (object) true;
          return (ActionResult) this.View("SecurityMin", (object) securityModel);
        default:
          this.ViewData["NewAdminUi"] = (object) true;
          return (ActionResult) this.View("SecurityTfs", (object) securityModel);
      }
    }

    [HttpGet]
    public ActionResult Members() => (ActionResult) this.Redirect(this.Url.FragmentAction("members"));

    private ManageViewModel GetManageViewModel(bool isOrganizationLevel = false)
    {
      ManageViewModel manageViewModel = new ManageViewModel();
      if (this.TfsWebContext.NavigationContext.TopMostLevel == NavigationContextLevels.Team)
      {
        this.ViewData["Title"] = this.IsHosted ? (object) AdminResources.Users : (object) AdminServerResources.UsersAndGroups;
        manageViewModel.DefaultFilter = "users";
      }
      else
      {
        this.ViewData["Title"] = (object) AdminResources.Members;
        manageViewModel.DefaultFilter = "groups";
      }
      SecurityModel securityModel = this.CreateSecurityModel(new Guid?(), (string) null, (string) null, out SecurityNamespacePermissionsManager _, new bool?(), isOrganizationLevel);
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer"))
        this.ViewData["PermissionsData"] = (object) JsonConvert.SerializeObject((object) securityModel.ToJson());
      else
        this.ViewData["PermissionsData"] = (object) new JavaScriptSerializer().Serialize((object) securityModel.ToJson());
      if (this.IsHosted && this.TfsRequestContext.IsOrganizationAadBacked())
        manageViewModel.IsAadAccount = true;
      if (AdminSecurityController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
      {
        manageViewModel.DisplayScope = "";
      }
      else
      {
        this.ViewData["HasSingleCollectionAdmin"] = (object) false;
        switch (this.TfsWebContext.NavigationContext.TopMostLevel)
        {
          case NavigationContextLevels.Collection:
            manageViewModel.DisplayScope = this.TfsWebContext.TfsRequestContext.ServiceHost.CollectionServiceHost.Name;
            if (this.IsHosted && !this.TfsRequestContext.IsOrganizationAadBacked() && AdminSecurityController.GetLicensedUsersCount(this.TfsWebContext.TfsRequestContext) > 1 && AdminSecurityController.HasSingleProjectCollectionAdmin(this.TfsWebContext.TfsRequestContext))
            {
              this.ViewData["HasSingleCollectionAdmin"] = (object) true;
              break;
            }
            break;
          case NavigationContextLevels.Project:
            manageViewModel.DisplayScope = this.TfsWebContext.Project.Name;
            break;
          case NavigationContextLevels.Team:
            manageViewModel.DisplayScope = this.Team != null ? this.Team.Name : throw new IdentityNotFoundException(this.TfsWebContext.NavigationContext.Team);
            break;
        }
      }
      return manageViewModel;
    }

    internal static bool HasSingleProjectCollectionAdmin(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      int num1 = 0;
      int num2 = 0;
      int num3 = 10;
      List<IdentityDescriptor> descriptors = new List<IdentityDescriptor>()
      {
        GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
      };
      IdentityService service = requestContext.GetService<IdentityService>();
      HashSet<IdentityDescriptor> collection = new HashSet<IdentityDescriptor>();
      for (; num2 < num3 && descriptors.Count > 0; ++num2)
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) descriptors, QueryMembership.Direct, (IEnumerable<string>) null);
        if (identityList == null)
          return true;
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList)
        {
          if (identity != null && !identity.Members.IsNullOrEmpty<IdentityDescriptor>())
          {
            num1 += identity.Members.Count<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => x != (IdentityDescriptor) null && x.IsClaimsIdentityType() && !ServicePrincipals.IsServicePrincipal(requestContext, x)));
            if (num1 > 1)
              return false;
            collection.AddRange<IdentityDescriptor, HashSet<IdentityDescriptor>>(identity.Members.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => x != (IdentityDescriptor) null && x.IsTeamFoundationType())));
          }
        }
        descriptors.Clear();
        descriptors.AddRange((IEnumerable<IdentityDescriptor>) collection);
        collection.Clear();
      }
      return true;
    }

    private static int GetLicensedUsersCount(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext.Elevate(), (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.LicensedUsersGroup
      }, QueryMembership.Direct, (IEnumerable<string>) null, true)[0];
      return readIdentity == null || readIdentity.Members.IsNullOrEmpty<IdentityDescriptor>() ? 0 : readIdentity.Members.Count<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => x != (IdentityDescriptor) null && x.IsClaimsIdentityType() && !ServicePrincipals.IsServicePrincipal(requestContext, x)));
    }

    private static bool ShouldElevateToOrganization(
      bool isOrganizationLevel,
      IVssRequestContext requestContext)
    {
      return isOrganizationLevel && requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("VisualStudio.Services.Web.OrgAdmin.UserExperience");
    }

    private static bool IsOrganizationActivated(IVssRequestContext requestContext)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
      return context.GetService<IOrganizationService>().GetOrganization(context, (IEnumerable<string>) null).IsActivated;
    }
  }
}
