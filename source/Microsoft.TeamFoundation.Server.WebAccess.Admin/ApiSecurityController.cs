// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ApiSecurityController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.AdminEngagement.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Organization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [ValidateInput(false)]
  [SupportedRouteArea("Api", NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  public class ApiSecurityController : AdminAreaController
  {
    private const string UserHubFeatureFlag = "WebAccess.UserManagement";
    private const string UserHubUrl = "~/_user";
    private const int WebAccessExceptionEaten = 599999;
    private IVssRequestContext m_tfsRequestContext;

    [HttpGet]
    [TfsTraceFilter(500200, 500210)]
    public ActionResult TracePermission(string data, bool isOrganizationLevel = false)
    {
      if (ApiSecurityController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      ArgumentUtility.CheckStringForNullOrEmpty(data, nameof (data));
      PermissionUpdates objectToCheck = !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? new JavaScriptSerializer().Deserialize<PermissionUpdates>(data) : JsonConvert.DeserializeObject<PermissionUpdates>(data);
      this.CheckObjectExists((object) objectToCheck, nameof (data));
      this.CheckObjectExists((object) objectToCheck.Updates, nameof (data));
      if (objectToCheck.Updates.Count == 0 || objectToCheck.Updates[0] == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.InvalidParameter, (object) nameof (data)));
      SecurityNamespacePermissionsManager permissionsManager = this.GetSecurityNamespacePermissionsManager(this.TfsRequestContext, objectToCheck.PermissionSetId, objectToCheck.PermissionSetToken);
      this.CheckObjectExists((object) permissionsManager, nameof (data));
      IdentityDescriptor descriptor = new IdentityDescriptor(objectToCheck.DescriptorIdentityType, objectToCheck.DescriptorIdentifier);
      TracePermissionModel trace = permissionsManager.GetTrace(this.TfsRequestContext, descriptor, objectToCheck.Updates[0]);
      trace.TitlePrefix = AdminAreaController.GetPermissionTitlePrefix(objectToCheck.PermissionSetId);
      trace.Title = AdminAreaController.GetPermissionTitle(this.TfsWebContext, objectToCheck.PermissionSetId, objectToCheck.PermissionSetToken, objectToCheck.TokenDisplayName);
      return (ActionResult) this.View((object) trace);
    }

    [HttpGet]
    [TfsTraceFilter(500220, 500230)]
    public ActionResult ReadExplicitIdentitiesJson(Guid permissionSetId, string permissionSetToken)
    {
      try
      {
        SecurityNamespacePermissionsManager permissionsManager = this.GetSecurityNamespacePermissionsManager(this.TfsRequestContext, permissionSetId, permissionSetToken);
        this.CheckObjectExists((object) permissionsManager, nameof (permissionSetId));
        List<TeamFoundationIdentity> foundationIdentityList = new List<TeamFoundationIdentity>((IEnumerable<TeamFoundationIdentity>) permissionsManager.GetIdentities(this.TfsRequestContext));
        foundationIdentityList.Sort((IComparer<TeamFoundationIdentity>) new TeamFoundationIdentityDisplayNameComparer());
        List<IdentityViewModelBase> source = new List<IdentityViewModelBase>();
        foreach (TeamFoundationIdentity foundationIdentity in foundationIdentityList)
        {
          if (permissionsManager.ShouldIncludeIdentity(foundationIdentity))
            source.Add(IdentityImageUtility.GetIdentityViewModel(foundationIdentity));
        }
        JsObject data = new JsObject();
        data.Add("identities", (object) source.Select<IdentityViewModelBase, JsObject>((Func<IdentityViewModelBase, JsObject>) (x => x.ToJson())));
        return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
      }
      catch (UnauthorizedAccessException ex)
      {
        this.TraceException(599999, (Exception) ex);
        return (ActionResult) new EmptyResult();
      }
    }

    [HttpGet]
    [TfsTraceFilter(500500, 500510)]
    public ActionResult DisplayPermissions(
      Guid tfid,
      string descriptorIdentityType,
      string descriptorIdentifier,
      Guid permissionSetId,
      string permissionSetToken)
    {
      IdentityDescriptor descriptor;
      if (!string.IsNullOrEmpty(descriptorIdentityType) && !string.IsNullOrEmpty(descriptorIdentifier))
      {
        descriptor = new IdentityDescriptor(descriptorIdentityType, descriptorIdentifier);
      }
      else
      {
        TeamFoundationIdentity readIdentity = this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, new Guid[1]
        {
          tfid
        }, MembershipQuery.Direct, ReadIdentityOptions.None, (IEnumerable<string>) null)[0];
        if (readIdentity == null)
          return (ActionResult) this.Content(string.Empty);
        descriptor = readIdentity.Descriptor;
      }
      return (ActionResult) this.Json((object) this.GetPermissionsModel(descriptor, permissionSetId, permissionSetToken).ToJson(), JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    [TfsTraceFilter(500560, 500570)]
    public ActionResult ManagePermissions(string updatePackage, bool isOrganizationLevel = false)
    {
      if (ApiSecurityController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      ArgumentUtility.CheckStringForNullOrEmpty(updatePackage, nameof (updatePackage));
      this.ViewData["DisplayForEdit"] = (object) true;
      PermissionUpdates objectToCheck = !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? new JavaScriptSerializer().Deserialize<PermissionUpdates>(updatePackage) : JsonConvert.DeserializeObject<PermissionUpdates>(updatePackage);
      this.CheckObjectExists((object) objectToCheck, nameof (updatePackage));
      string permissionSetToken = objectToCheck.PermissionSetToken;
      IdentityDescriptor descriptor = new IdentityDescriptor(objectToCheck.DescriptorIdentityType, objectToCheck.DescriptorIdentifier);
      SecurityNamespacePermissionsManager permissionsManager = this.GetSecurityNamespacePermissionsManager(this.TfsRequestContext, objectToCheck.PermissionSetId, permissionSetToken, new Guid?(objectToCheck.TeamFoundationId));
      this.CheckObjectExists((object) permissionsManager, nameof (updatePackage));
      IList<SettableAction> permissions = permissionsManager.GetPermissions(this.TfsRequestContext, descriptor);
      if (objectToCheck.IsRemovingIdentity)
      {
        permissionsManager.RemoveIdentity(this.TfsRequestContext, descriptor);
      }
      else
      {
        IList<PermissionUpdate> updates = this.ProcessPermissions(permissionsManager, (IList<PermissionUpdate>) objectToCheck.Updates, permissions, true);
        this.ProcessPermissions(permissionsManager, updates, permissions, false);
      }
      PermissionsModel permissionsModel = this.GetPermissionsModel(descriptor, objectToCheck.PermissionSetId, permissionSetToken, new Guid?(objectToCheck.TeamFoundationId));
      this.AssignLicenceToNewUser(permissionsModel, permissionsManager, (IList<PermissionUpdate>) objectToCheck.Updates, permissions);
      return (ActionResult) this.Json((object) permissionsModel.ToJson());
    }

    private IList<PermissionUpdate> ProcessPermissions(
      SecurityNamespacePermissionsManager manager,
      IList<PermissionUpdate> updates,
      IList<SettableAction> actions,
      bool checkForDelayProcessing)
    {
      IList<PermissionUpdate> permissionUpdateList = (IList<PermissionUpdate>) new List<PermissionUpdate>();
      foreach (PermissionUpdate update in (IEnumerable<PermissionUpdate>) updates)
      {
        foreach (SettableAction action in (IEnumerable<SettableAction>) actions)
        {
          if (action.Token.Equals(update.Token) && action.NamespaceId == update.NamespaceId && action.ActionDefinition.Bit == update.PermissionBit)
          {
            if (checkForDelayProcessing && manager.IsWritePermission(this.TfsRequestContext, update.NamespaceId, update.PermissionBit))
            {
              permissionUpdateList.Add(update);
              break;
            }
            manager.SetPermission(this.TfsRequestContext, action, update.PermissionId == PermissionValue.Allow, update.PermissionId == PermissionValue.Deny);
            break;
          }
        }
      }
      return permissionUpdateList;
    }

    private PermissionsModel GetPermissionsModel(
      IdentityDescriptor descriptor,
      Guid permissionSetId,
      string token,
      Guid? teamId = null)
    {
      SecurityNamespacePermissionsManager permissionsManager = this.GetSecurityNamespacePermissionsManager(this.TfsRequestContext, permissionSetId, token, teamId);
      this.CheckObjectExists((object) permissionsManager, nameof (permissionSetId));
      return new PermissionsModel(this.TfsRequestContext, descriptor, permissionsManager);
    }

    [HttpPost]
    [TfsTraceFilter(500520, 500530)]
    public ActionResult ChangeInheritance(
      Guid permissionSet,
      string token,
      bool inheritPermissions)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(token, nameof (token));
      ArgumentUtility.CheckForEmptyGuid(permissionSet, nameof (permissionSet));
      SecurityNamespacePermissionsManager permissionsManager = this.GetSecurityNamespacePermissionsManager(this.TfsRequestContext, permissionSet, token);
      this.CheckObjectExists((object) permissionsManager, nameof (permissionSet));
      if (permissionsManager.CanTokenInheritPermissions && permissionsManager.InheritPermissions != inheritPermissions)
      {
        permissionsManager.InheritPermissions = inheritPermissions;
        permissionsManager.ChangeInheritance(this.TfsRequestContext, inheritPermissions);
      }
      return (ActionResult) new EmptyResult();
    }

    [HttpPost]
    [TfsTraceFilter(500400, 500410)]
    public ActionResult AddIdentityForPermissions(string newUsersJson, string existingUsersJson)
    {
      MembershipModel membershipModel = new MembershipModel();
      IList<TeamFoundationIdentity> newUsersJson1 = IdentityManagementHelpers.ParseNewUsersJson((ITfsController) this, newUsersJson, membershipModel);
      if (membershipModel.FailedAddedIdentities.Count > 0)
        throw new ArgumentException(membershipModel.FailedAddedIdentities[0].Errors[0]);
      IList<TeamFoundationIdentity> source = IdentityManagementHelpers.ResolveIdentities((ITfsController) this, IdentityManagementHelpers.ParseTfidsJson(existingUsersJson));
      IdentityViewModelBase identityViewModel = IdentityImageUtility.GetIdentityViewModel((newUsersJson1.FirstOrDefault<TeamFoundationIdentity>() ?? source.FirstOrDefault<TeamFoundationIdentity>()) ?? throw new ArgumentException(AdminServerResources.NoIdentitySpecified));
      identityViewModel.IncludeDescriptor = true;
      JsObject data = new JsObject();
      data["AddedIdentity"] = (object) identityViewModel.ToJson();
      return (ActionResult) this.Json((object) data);
    }

    private void CheckObjectExists(object objectToCheck, string argument)
    {
      if (objectToCheck == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.InvalidParameter, (object) argument));
    }

    private void AssignLicenceToNewUser(
      PermissionsModel model,
      SecurityNamespacePermissionsManager manager,
      IList<PermissionUpdate> updates,
      IList<SettableAction> actions)
    {
      if (!this.IsHosted || !this.TfsRequestContext.IsFeatureEnabled("WebAccess.UserManagement"))
        return;
      if (this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        return;
      try
      {
        TeamFoundationIdentity identity = this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentity(this.TfsRequestContext, model.Descriptor, MembershipQuery.None, ReadIdentityOptions.None);
        if (identity == null || identity.IsContainer && !string.Equals(identity.Descriptor.IdentityType, "System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase) || IdentityHelper.IsServiceIdentity(this.TfsRequestContext, (IReadOnlyVssIdentity) IdentityUtil.Convert(identity)))
          return;
        this.TfsRequestContext.GetService<ILicensingEntitlementService>().AssignAvailableAccountEntitlement(this.TfsRequestContext, model.TeamFoundationId);
        if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.AadGroupsAdminUi") || !this.TfsRequestContext.IsOrganizationAadBacked())
          return;
        model.AADErrors.Add(LicenseHelpers.GetAadUserWarningMessage(this.TfsRequestContext));
      }
      catch (LicenseNotAvailableException ex)
      {
        foreach (PermissionUpdate update in (IEnumerable<PermissionUpdate>) updates)
          update.PermissionId = PermissionValue.NotSet;
        IList<PermissionUpdate> updates1 = this.ProcessPermissions(manager, updates, actions, true);
        this.ProcessPermissions(manager, updates1, actions, false);
        model.LicenseErrors.Add(string.Format(AdminServerResources.NoLicenceAvailableErrorMessage, (object) VirtualPathUtility.ToAbsolute("~/_user")));
        this.TraceException(599999, (Exception) ex);
      }
    }

    private TeamFoundationIdentity ProcessNewUser(
      IVssRequestContext requestContext,
      string newUser,
      out string error)
    {
      MembershipModel membershipModel = new MembershipModel();
      TeamFoundationIdentity foundationIdentity = IdentityManagementHelpers.ProcessNewUsers((ITfsController) this, new string[1]
      {
        newUser
      }, membershipModel).FirstOrDefault<TeamFoundationIdentity>();
      if (foundationIdentity == null)
      {
        error = membershipModel.FailedAddedIdentities.First<IdentityViewModelBase>().Errors.First<string>();
        return foundationIdentity;
      }
      error = string.Empty;
      return foundationIdentity;
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

    private static bool ShouldElevateToOrganization(
      bool isOrganizationLevel,
      IVssRequestContext requestContext)
    {
      return isOrganizationLevel && requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("VisualStudio.Services.Web.OrgAdmin.UserExperience");
    }
  }
}
