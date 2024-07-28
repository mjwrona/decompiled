// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ApiIdentityController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.Utils;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Directories.DiscoveryService;
using Microsoft.VisualStudio.Services.GroupLicensingRule;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Security.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [SupportedRouteArea("Api", NavigationContextLevels.ApplicationAll)]
  [OutputCache(CacheProfile = "NoCache")]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiIdentityController : AdminAreaController
  {
    private const string MaxAadResultsCountRegistry = "/Service/AAD/Settings/AadSearchMaxSize";
    private const string AadResultsCountRatioRegistry = "/Service/AAD/Settings/AadSearchUsersToGroupRatio";
    private const string UserHubFeatureFlag = "WebAccess.UserManagement";
    private const string UserHubUrl = "~/_user";
    private const int WebAccessExceptionEaten = 599999;
    internal const string DisableGroupMembershipSecurityPolicyFeatureFlag = "VisualStudio.Services.Identity.DisableGroupMembershipSecurityPolicy";
    private IVssRequestContext m_tfsRequestContext;

    protected override void Initialize(System.Web.Routing.RequestContext requestContext) => base.Initialize(requestContext);

    [HttpGet]
    [TfsTraceFilter(500020, 500030)]
    [ValidateInput(false)]
    public ActionResult CheckName(string name)
    {
      TeamFoundationIdentityService service = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
      JsObject data = new JsObject();
      TeamFoundationIdentity identity1 = (TeamFoundationIdentity) null;
      try
      {
        TeamFoundationIdentity identity2 = service.ReadIdentity(this.TfsRequestContext, IdentitySearchFactor.General, name, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
        if (this.IsValidCheckedName(identity2))
          identity1 = identity2;
        else if (this.IsHosted)
        {
          if (!string.IsNullOrEmpty(name))
          {
            try
            {
              MailAddress mailAddress = new MailAddress(name);
            }
            catch (FormatException ex)
            {
              throw new TeamFoundationServiceException(string.Format(WACommonResources.InvalidEmailAddressFormat, (object) name));
            }
          }
        }
      }
      catch (Microsoft.TeamFoundation.Server.Core.MultipleIdentitiesFoundException ex)
      {
        identity1 = this.HandleMultipleIdentitiesFoundException(ex);
      }
      if (identity1 != null && !string.Equals(identity1.Descriptor.IdentityType, "System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase))
      {
        IdentityViewModelBase identityViewModel = IdentityManagementHelpers.GetIdentityViewModel(identity1);
        data["identity"] = (object) identityViewModel.ToJson();
      }
      else if (!this.IsHosted)
      {
        TeamFoundationIdentity identity3 = (TeamFoundationIdentity) null;
        try
        {
          string factorValue = name;
          if (identity1 != null)
          {
            string attribute = identity1.GetAttribute("Domain", string.Empty);
            factorValue = identity1.GetAttribute("Account", name);
            if (!string.IsNullOrEmpty(attribute))
              factorValue = string.Format("{0}\\{1}", (object) attribute, (object) factorValue);
          }
          TeamFoundationIdentity identity4 = service.ReadIdentity(this.TfsRequestContext, IdentitySearchFactor.AccountName, factorValue, MembershipQuery.None, ReadIdentityOptions.IncludeReadFromSource, (IEnumerable<string>) null);
          if (this.IsValidCheckedName(identity4))
          {
            if (identity4.IsActive)
              identity3 = identity4;
          }
        }
        catch (Microsoft.TeamFoundation.Server.Core.MultipleIdentitiesFoundException ex)
        {
          identity3 = this.HandleMultipleIdentitiesFoundException(ex);
        }
        if (identity3 != null && identity3.IsActive && identity1 != null && identity1.IsActive && IdentityDescriptorComparer.Instance.Equals(identity3.Descriptor, identity1.Descriptor))
        {
          IdentityViewModelBase identityViewModel = IdentityManagementHelpers.GetIdentityViewModel(identity3);
          data["identity"] = (object) identityViewModel.ToJson();
        }
        else
        {
          if (identity3 == null)
            throw new IdentityNotFoundException(AdminServerResources.CheckNameError);
          data["resolvedName"] = string.IsNullOrEmpty(identity3.DisplayName) ? (object) identity3.GetAttribute("Account", (string) null) : (object) identity3.DisplayName;
        }
      }
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    private bool IsValidCheckedName(TeamFoundationIdentity identity)
    {
      if (identity == null)
        return false;
      return !identity.IsContainer || string.Equals(identity.Descriptor.IdentityType, "System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase);
    }

    private TeamFoundationIdentity HandleMultipleIdentitiesFoundException(
      Microsoft.TeamFoundation.Server.Core.MultipleIdentitiesFoundException ex)
    {
      List<TeamFoundationIdentity> foundationIdentityList = new List<TeamFoundationIdentity>();
      foreach (TeamFoundationIdentity matchingIdentity in ex.MatchingIdentities)
      {
        if (this.IsValidCheckedName(matchingIdentity))
          foundationIdentityList.Add(matchingIdentity);
      }
      if (foundationIdentityList.Count == 1)
        return foundationIdentityList[0];
      if (foundationIdentityList.Count > 1)
        throw new Microsoft.TeamFoundation.Server.Core.MultipleIdentitiesFoundException(ex.FactorValue, foundationIdentityList.ToArray());
      return (TeamFoundationIdentity) null;
    }

    [HttpPost]
    [AcceptNavigationLevels(NavigationContextLevels.Application)]
    public ActionResult AddLicenseMembers(
      string newUsersJson,
      string existingUsersJson,
      Guid licenseTypeId)
    {
      TeamFoundationIdentity licenseGroup = this.TfsRequestContext.GetService<TeamFoundationOnPremLicensingService>().GetLicenseGroup(this.TfsRequestContext, licenseTypeId);
      return (ActionResult) this.Json((object) this.AddIdentitiesInternal(newUsersJson, existingUsersJson, (IList<TeamFoundationIdentity>) new TeamFoundationIdentity[1]
      {
        licenseGroup
      }).ToJson());
    }

    [HttpPost]
    [AcceptNavigationLevels(NavigationContextLevels.ApplicationAll)]
    [TfsTraceFilter(500120, 500130)]
    [ValidateInput(false)]
    public ActionResult AddIdentities(
      string newUsersJson,
      string existingUsersJson,
      string groupsToJoinJson,
      string aadGroupsJson = null,
      bool isOrganizationLevel = false)
    {
      if (ApiIdentityController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      if (!string.IsNullOrEmpty(aadGroupsJson) && !string.Equals(aadGroupsJson.Trim(), "[]") && (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment || !this.TfsRequestContext.GetUserIdentity().IsExternalUser))
        return (ActionResult) this.Json((object) new MembershipModel()
        {
          GeneralErrors = {
            AdminServerResources.AADGroupNotSupported
          }
        });
      IList<TeamFoundationIdentity> foundationIdentityList = IdentityManagementHelpers.ResolveIdentities((ITfsController) this, IdentityManagementHelpers.ParseTfidsJson(groupsToJoinJson));
      IList<TeamFoundationIdentity> list1 = (IList<TeamFoundationIdentity>) foundationIdentityList.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (group => !ApiIdentityController.HasManageGroupMembershipPermission(this.TfsRequestContext, group))).ToList<TeamFoundationIdentity>();
      IList<TeamFoundationIdentity> list2 = (IList<TeamFoundationIdentity>) foundationIdentityList.Except<TeamFoundationIdentity>((IEnumerable<TeamFoundationIdentity>) list1).ToList<TeamFoundationIdentity>();
      MembershipModel membershipModel;
      if (list2.Count > 0)
      {
        List<string> failureMessages = (List<string>) null;
        List<TeamFoundationIdentity> aadGroupsWithCreationFailure = (List<TeamFoundationIdentity>) null;
        if (!string.IsNullOrEmpty(aadGroupsJson) && !string.Equals(aadGroupsJson.Trim(), "[]"))
        {
          IList<Guid> source = (IList<Guid>) new List<Guid>();
          Guid[] tfidsJson = IdentityManagementHelpers.ParseTfidsJson(aadGroupsJson);
          if (tfidsJson.Length != 0)
            source = this.TfsRequestContext.GetExtension<IAadGroupHelper>(ExtensionLifetime.Service).CreateAadGroupsInIms(this.TfsRequestContext.Elevate(), tfidsJson, out aadGroupsWithCreationFailure, out failureMessages);
          if (source.Count > 0)
          {
            Guid[] array = ((IEnumerable<Guid>) IdentityManagementHelpers.ParseTfidsJson(existingUsersJson)).Concat<Guid>((IEnumerable<Guid>) source.ToArray<Guid>()).ToArray<Guid>();
            existingUsersJson = !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? new JavaScriptSerializer().Serialize((object) array) : JsonConvert.SerializeObject((object) array);
          }
        }
        membershipModel = this.AddIdentitiesInternal(newUsersJson, existingUsersJson, list2);
        if (!string.IsNullOrEmpty(aadGroupsJson) && !string.Equals(aadGroupsJson.Trim(), "[]"))
        {
          if (failureMessages != null && failureMessages.Count > 0)
            membershipModel.GeneralErrors.AddRange((IEnumerable<string>) failureMessages);
          if (aadGroupsWithCreationFailure != null && aadGroupsWithCreationFailure.Count > 0)
          {
            IList<IdentityViewModelBase> collection = (IList<IdentityViewModelBase>) new List<IdentityViewModelBase>();
            foreach (TeamFoundationIdentity identity in aadGroupsWithCreationFailure)
              collection.Add(IdentityManagementHelpers.GetIdentityViewModel(identity));
            membershipModel.FailedAddedIdentities.AddRange((IEnumerable<IdentityViewModelBase>) collection);
          }
        }
      }
      else
      {
        membershipModel = new MembershipModel();
        membershipModel.GeneralErrors.Add(AdminServerResources.GroupsToJoinNotFound);
        this.Trace(564894, TraceLevel.Error, string.Format("Specified params - newUsersJson: {0}, existingUsersJson: {1}, groupsToJoinJson: {2}, aadGroupsJson: {3}, isOrganizationLevel: {4}.", (object) newUsersJson, (object) existingUsersJson, (object) groupsToJoinJson, (object) aadGroupsJson, (object) isOrganizationLevel));
      }
      if (list1 != null && list1.Count > 0)
      {
        string str = string.Join(", ", list1.Select<TeamFoundationIdentity, string>((Func<TeamFoundationIdentity, string>) (x => x.DisplayName)));
        if (membershipModel == null)
          membershipModel = new MembershipModel();
        membershipModel.GeneralErrors.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AdminServerResources.NoPermissionsToAddMembersToGroups, (object) str));
      }
      return (ActionResult) this.Json((object) membershipModel.ToJson());
    }

    private MembershipModel AddIdentitiesInternal(
      string newUsersJson,
      string existingUsersJson,
      IList<TeamFoundationIdentity> groupsToJoinIdentities)
    {
      IList<TeamFoundationIdentity> existingIdentities;
      IList<TeamFoundationIdentity> newUsersIdentities;
      MembershipModel membershipModel = IdentityManagementHelpers.GetMembershipModel((ITfsController) this, newUsersJson, existingUsersJson, out existingIdentities, out newUsersIdentities);
      CustomerIntelligenceService service = this.TfsRequestContext.GetService<CustomerIntelligenceService>();
      foreach (TeamFoundationIdentity groupsToJoinIdentity in (IEnumerable<TeamFoundationIdentity>) groupsToJoinIdentities)
      {
        foreach (TeamFoundationIdentity foundationIdentity in newUsersIdentities.Concat<TeamFoundationIdentity>((IEnumerable<TeamFoundationIdentity>) existingIdentities))
        {
          if (foundationIdentity != null)
          {
            int num = IdentityHelpers.AddMemberToGroup(this.TfsRequestContext, groupsToJoinIdentity, foundationIdentity, foundationIdentity, membershipModel) ? 1 : 0;
            if (num != 0 && this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
              this.RevaluateSingleExtensionAssignment(foundationIdentity);
            if (num != 0 && !foundationIdentity.IsContainer && !this.AssignLicenseToUser(membershipModel, foundationIdentity, groupsToJoinIdentity.Descriptor))
            {
              IdentityHelpers.RemoveMemberFromGroup(this.TfsRequestContext, groupsToJoinIdentity, foundationIdentity, foundationIdentity, membershipModel);
              if (membershipModel.AddedIdentities.Count > 0)
              {
                int index = membershipModel.AddedIdentities.Count - 1;
                membershipModel.FailedAddedIdentities.Add(membershipModel.AddedIdentities[index]);
                membershipModel.AddedIdentities.RemoveAt(index);
              }
            }
            string str = foundationIdentity.IsContainer ? "GroupAdded" : "MemberAdded";
            service.Publish(this.TfsRequestContext, CustomerIntelligenceArea.Account, Microsoft.TeamFoundation.Framework.Server.CustomerIntelligenceFeature.TraceAccount, CustomerIntelligenceProperty.Action, str);
          }
        }
      }
      if (!this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.AadGroupsAdminUi") && newUsersIdentities != null && newUsersIdentities.Count > 0)
        this.AddAADError(membershipModel, this.TfsRequestContext);
      return membershipModel;
    }

    [HttpPost]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(500140, 500150)]
    [ValidateInput(false)]
    public ActionResult AddTeamAdmins(Guid teamId, string newUsersJson, string existingUsersJson)
    {
      JsObject data = new JsObject();
      MembershipModel membershipModel = new MembershipModel();
      IList<TeamFoundationIdentity> existingIdentities = IdentityManagementHelpers.ResolveIdentities((ITfsController) this, IdentityManagementHelpers.ParseTfidsJson(existingUsersJson));
      IList<TeamFoundationIdentity> newUsersJson1 = IdentityManagementHelpers.ParseNewUsersJson((ITfsController) this, newUsersJson, membershipModel);
      WebApiTeam teamByGuid = this.TfsRequestContext.GetService<ITeamService>().GetTeamByGuid(this.TfsRequestContext, teamId);
      if (teamByGuid != null)
      {
        bool isTeam = this.TfsRequestContext.IsFeatureEnabled("WorkItemTracking.Server.CopyLocalPropertiesInIdentityConvertDisabled");
        TeamIdentityViewModel identityViewModel = IdentityManagementHelpers.GetIdentityViewModel<TeamIdentityViewModel>(IdentityUtil.Convert(teamByGuid.Identity), isTeam);
        this.AddTeamAdminsInternal(identityViewModel.Descriptor, newUsersJson1, existingIdentities, membershipModel);
        identityViewModel.PopulateTeamAdmins(this.TfsWebContext);
        data["admins"] = (object) identityViewModel.Administrators.Select<IdentityViewModelBase, JsObject>((Func<IdentityViewModelBase, JsObject>) (x => x.ToJson()));
      }
      else
        membershipModel.GeneralErrors.Add(AdminServerResources.TeamNotFound);
      if (membershipModel.HasErrors)
        data["membershipModel"] = (object) membershipModel.ToJson();
      return (ActionResult) this.Json((object) data);
    }

    public void AddTeamAdminsInternal(
      IdentityDescriptor descriptor,
      IList<TeamFoundationIdentity> newUsersIdentities,
      IList<TeamFoundationIdentity> existingIdentities,
      MembershipModel membershipModel,
      bool logAsEatenException = true)
    {
      ITeamFoundationIdentityService service = this.TfsRequestContext.GetService<ITeamFoundationIdentityService>();
      foreach (TeamFoundationIdentity foundationIdentity in newUsersIdentities.Concat<TeamFoundationIdentity>((IEnumerable<TeamFoundationIdentity>) existingIdentities))
      {
        try
        {
          service.AddGroupAdministrator(this.TfsRequestContext, descriptor, foundationIdentity.Descriptor);
          if (foundationIdentity.IsContainer || this.AssignLicenseToUser(membershipModel, foundationIdentity, descriptor))
          {
            service.EnsureIsMember(this.TfsRequestContext, descriptor, foundationIdentity.Descriptor);
            membershipModel.AddedIdentities.Add(IdentityManagementHelpers.GetIdentityViewModel(foundationIdentity));
          }
          else
            service.RemoveGroupAdministrator(this.TfsRequestContext, descriptor, foundationIdentity.Descriptor);
        }
        catch (AccessCheckException ex)
        {
          membershipModel.GeneralErrors.Add(ex.Message);
          membershipModel.GeneralErrors.Add(AdminServerResources.ErrorMessageAddAdmin);
          this.TraceException(logAsEatenException ? 599999 : 15166004, (Exception) ex);
          break;
        }
        catch (TeamFoundationServiceException ex)
        {
          IdentityViewModelBase identityViewModel = IdentityManagementHelpers.GetIdentityViewModel(foundationIdentity);
          identityViewModel.Errors.Add(ex.Message);
          membershipModel.FailedAddedIdentities.Add(identityViewModel);
          this.TraceException(logAsEatenException ? 599999 : 15166005, (Exception) ex);
        }
        catch (Exception ex)
        {
          this.TraceException(logAsEatenException ? 599999 : 15166006, ex);
        }
      }
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.AadGroupsAdminUi") || newUsersIdentities == null || newUsersIdentities.Count <= 0)
        return;
      this.AddAADError(membershipModel, this.TfsRequestContext);
    }

    public virtual void AddAADError(
      MembershipModel membershipModel,
      IVssRequestContext tfsRequestContext)
    {
      if (!this.IsHosted || !this.IsAADAccount())
        return;
      membershipModel.AADErrors.Add(LicenseHelpers.GetAadUserWarningMessage(tfsRequestContext));
    }

    private bool IsAADAccount() => this.TfsRequestContext.ServiceHost != null && this.TfsRequestContext.ServiceHost.OrganizationServiceHost != null && this.TfsRequestContext.IsOrganizationAadBacked();

    private bool AssignLicenseToUser(
      MembershipModel membershipModel,
      TeamFoundationIdentity tfidIdentity,
      IdentityDescriptor groupDescriptor)
    {
      if (!this.IsHosted || !this.TfsRequestContext.IsFeatureEnabled("WebAccess.UserManagement"))
        return true;
      if (this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        return true;
      try
      {
        Guid teamFoundationId = tfidIdentity.TeamFoundationId;
        if (teamFoundationId == Guid.Empty)
        {
          this.Trace(599999, TraceLevel.Error, "Identity " + tfidIdentity.DisplayName + " ID was empty.");
          return true;
        }
        if (!IdentityHelper.IsServiceIdentity(this.TfsRequestContext, (IReadOnlyVssIdentity) tfidIdentity))
        {
          bool isPublicResource = this.IsPublicResourceScoped(groupDescriptor);
          AccountEntitlement identity = LicenseAssignmentHelper.AssignLicenseToIdentity(this.TfsRequestContext, teamFoundationId, isPublicResource, true);
          if (identity != (AccountEntitlement) null && identity.License == (License) AccountLicense.Stakeholder)
          {
            string absolute = VirtualPathUtility.ToAbsolute("~/_user");
            if (membershipModel.StakeholderLicenceWarnings.Count == 0)
              membershipModel.StakeholderLicenceWarnings.Add(string.Format(AdminServerResources.GrantedStakeHolderLicenseMessage, (object) tfidIdentity.ProviderDisplayName, (object) absolute));
            this.Trace(599999, TraceLevel.Warning, "Added stakeholder warning message");
          }
        }
        return true;
      }
      catch (LicenseNotAvailableException ex)
      {
        if (membershipModel.LicenceErrors.Count == 0)
          membershipModel.LicenceErrors.Add(string.Format(AdminServerResources.NoLicenceAvailableErrorMessage, (object) VirtualPathUtility.ToAbsolute("~/_user")));
        this.TraceException(599999, (Exception) ex);
        return false;
      }
      catch (Exception ex)
      {
        if (membershipModel.LicenceErrors.Count == 0)
          membershipModel.LicenceErrors.Add(AdminServerResources.LicenceCreationUnexpectedErrorMessage);
        this.TraceException(599999, ex);
        return false;
      }
    }

    private bool IsPublicResourceScoped(IdentityDescriptor groupDescriptor)
    {
      try
      {
        ProjectInfo projectInfo;
        if (this.TfsRequestContext.GetService<IProjectService>().TryGetProject(this.TfsRequestContext, this.TfsRequestContext.GetService<IdentityService>().ReadIdentities(this.TfsRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          groupDescriptor
        }, Microsoft.VisualStudio.Services.Identity.QueryMembership.None, (IEnumerable<string>) null).Single<Microsoft.VisualStudio.Services.Identity.Identity>().LocalScopeId, out projectInfo))
          return projectInfo.Visibility == ProjectVisibility.Public;
      }
      catch (Exception ex)
      {
        this.TraceException(599999, ex);
      }
      return false;
    }

    [HttpGet]
    public ActionResult ReadAddableGroups(
      bool? joinGroups,
      bool? excludeTeams,
      Guid? joinGroupTfid,
      bool? showAllGroupsIfCollection,
      bool? joinGroupExpandParentScopes)
    {
      joinGroups = new bool?(joinGroups.GetValueOrDefault());
      excludeTeams = new bool?(excludeTeams.GetValueOrDefault());
      joinGroupTfid = new Guid?(joinGroupTfid ?? Guid.Empty);
      joinGroupExpandParentScopes = new bool?(joinGroupExpandParentScopes.GetValueOrDefault());
      TeamFoundationIdentityService service1 = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
      List<TeamFoundationIdentity> source1 = new List<TeamFoundationIdentity>();
      if (this.TfsWebContext.CurrentProjectUri != (Uri) null)
      {
        IEnumerable<TeamFoundationIdentity> foundationIdentities = (IEnumerable<TeamFoundationIdentity>) service1.ListApplicationGroups(this.TfsRequestContext, this.TfsWebContext.CurrentProjectUri.ToString(), ReadIdentityOptions.None, (IEnumerable<string>) null);
        if (joinGroups.Value)
          foundationIdentities = foundationIdentities.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (project => !IdentityHelper.IsEveryoneGroup(project.Descriptor)));
        if (excludeTeams.Value)
        {
          IEnumerable<TeamFoundationIdentity> source2 = (IEnumerable<TeamFoundationIdentity>) service1.ReadIdentities(this.TfsRequestContext, foundationIdentities.Select<TeamFoundationIdentity, Guid>((Func<TeamFoundationIdentity, Guid>) (x => x.TeamFoundationId)).ToArray<Guid>(), MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
          {
            TeamConstants.TeamPropertyName
          }, IdentityPropertyScope.Local);
          source1.AddRange(source2.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (x => x != null && !x.TryGetProperty(TeamConstants.TeamPropertyName, out object _))));
        }
        else
          source1.AddRange(foundationIdentities);
      }
      if (this.NavigationContext.TopMostLevel == NavigationContextLevels.Collection || !string.IsNullOrEmpty(this.NavigationContext.Project) && !joinGroups.Value)
      {
        List<TeamFoundationIdentity> foundationIdentityList = new List<TeamFoundationIdentity>();
        if (showAllGroupsIfCollection.HasValue && showAllGroupsIfCollection.Value)
        {
          foreach (CommonStructureProjectInfo project in this.TfsRequestContext.GetService<CommonStructureService>().GetProjects(this.TfsRequestContext))
          {
            TeamFoundationIdentity[] collection = service1.ListApplicationGroups(this.TfsRequestContext, project.Uri, ReadIdentityOptions.None, (IEnumerable<string>) null);
            foundationIdentityList.AddRange((IEnumerable<TeamFoundationIdentity>) collection);
          }
        }
        foundationIdentityList.AddRange((IEnumerable<TeamFoundationIdentity>) service1.ListApplicationGroups(this.TfsRequestContext, (string) null, ReadIdentityOptions.None, (IEnumerable<string>) null));
        foreach (TeamFoundationIdentity foundationIdentity in foundationIdentityList)
        {
          if (!IdentityDescriptorComparer.Instance.Equals(GroupWellKnownIdentityDescriptors.EveryoneGroup, foundationIdentity.Descriptor))
            source1.Add(foundationIdentity);
        }
      }
      if (!this.IsHosted && !joinGroups.Value)
      {
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
        foreach (TeamFoundationIdentity applicationGroup in vssRequestContext.GetService<TeamFoundationIdentityService>().ListApplicationGroups(vssRequestContext, (string) null, ReadIdentityOptions.None, (IEnumerable<string>) null))
        {
          if (!IdentityDescriptorComparer.Instance.Equals(GroupWellKnownIdentityDescriptors.EveryoneGroup, applicationGroup.Descriptor))
            source1.Add(applicationGroup);
        }
      }
      TeamFoundationFilteredIdentitiesList filteredIdentities = new TeamFoundationFilteredIdentitiesList();
      if (!joinGroupTfid.Value.Equals(Guid.Empty))
      {
        if (joinGroupExpandParentScopes.Value)
        {
          IdentityService service2 = this.TfsRequestContext.GetService<IdentityService>();
          Microsoft.VisualStudio.Services.Identity.Identity identity = service2.ReadIdentities(this.TfsRequestContext, (IList<Guid>) new Guid[1]
          {
            joinGroupTfid.Value
          }, Microsoft.VisualStudio.Services.Identity.QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          if (identity != null)
          {
            List<Guid> guidList = new List<Guid>();
            Guid property = identity.GetProperty<Guid>("LocalScopeId", Guid.Empty);
            if (property != Guid.Empty && property != this.TfsRequestContext.ServiceHost.InstanceId)
            {
              for (IdentityScope scope = service2.GetScope(this.TfsRequestContext, property); scope.Id != scope.SecuringHostId; scope = service2.GetScope(this.TfsRequestContext, scope.ParentId))
                guidList.Add(scope.Id);
              foreach (Microsoft.VisualStudio.Services.Identity.Identity listGroup in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) service2.ListGroups(this.TfsRequestContext, guidList.ToArray(), false, (IEnumerable<string>) null))
              {
                if (!IdentityHelper.IsEveryoneGroup(listGroup.Descriptor))
                  source1.Add(IdentityUtil.Convert(listGroup));
              }
            }
          }
        }
        filteredIdentities.Items = source1.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (g => !g.TeamFoundationId.Equals(joinGroupTfid.Value))).ToArray<TeamFoundationIdentity>();
      }
      else
        filteredIdentities.Items = source1.ToArray();
      Array.Sort<TeamFoundationIdentity>(filteredIdentities.Items, (IComparer<TeamFoundationIdentity>) new TeamFoundationIdentityDisplayNameComparer());
      return this.JsonFromFilteredIdentitiesList(filteredIdentities);
    }

    [HttpGet]
    public ActionResult ReadAddableAadGroups(
      Guid? joinGroupTfid,
      string searchQuery,
      string searchResultToken,
      int? pageSize,
      bool localAadGroups = true)
    {
      joinGroupTfid = new Guid?(joinGroupTfid ?? Guid.Empty);
      this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
      List<TeamFoundationIdentity> foundationIdentityList = new List<TeamFoundationIdentity>();
      TeamFoundationFilteredIdentitiesList filteredIdentitiesList = new TeamFoundationFilteredIdentitiesList();
      if (this.IsHosted & localAadGroups)
        filteredIdentitiesList = this.TfsRequestContext.GetExtension<IAadGroupHelper>(ExtensionLifetime.Service).GetAadGroupsFromIms(this.TfsRequestContext, searchQuery);
      else if (this.IsHosted && !localAadGroups)
      {
        int pageSize1 = IdentityManagementHelpers.GetPageSize(pageSize);
        try
        {
          IAadGroupHelper extension = this.TfsRequestContext.GetExtension<IAadGroupHelper>(ExtensionLifetime.Service);
          filteredIdentitiesList = !string.IsNullOrEmpty(searchQuery) ? extension.GetAadGroupsFromAad(this.TfsRequestContext, searchQuery) : extension.GetAadGroupsFromAad(this.TfsRequestContext, pageSize1, ref searchResultToken);
        }
        catch (AadCredentialsNotFoundException ex)
        {
          throw new TeamFoundationServiceException(AdminServerResources.AadAccessExceptionText, (Exception) ex);
        }
        catch (AadAccessException ex)
        {
          throw new TeamFoundationServiceException(AdminServerResources.AadAccessExceptionText, (Exception) ex);
        }
        catch (Exception ex)
        {
          throw new TeamFoundationServiceException(AdminServerResources.AADServiceError, ex);
        }
        Array.Sort<TeamFoundationIdentity>(filteredIdentitiesList.Items, (IComparer<TeamFoundationIdentity>) new TeamFoundationIdentityDisplayNameComparer());
      }
      IEnumerable<JsObject> jsObjects = ((IEnumerable<TeamFoundationIdentity>) filteredIdentitiesList.Items).Select<TeamFoundationIdentity, IdentityViewModelBase>((Func<TeamFoundationIdentity, IdentityViewModelBase>) (s => IdentityManagementHelpers.GetIdentityViewModel(s))).Select<IdentityViewModelBase, JsObject>((Func<IdentityViewModelBase, JsObject>) (s => s.ToJson()));
      JsObject data = new JsObject();
      data.Add("identities", (object) jsObjects);
      data.Add("hasMore", (object) filteredIdentitiesList.HasMoreItems);
      data.Add("totalIdentityCount", (object) filteredIdentitiesList.TotalItems);
      data.Add(nameof (searchResultToken), (object) searchResultToken);
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [TfsTraceFilter(500360, 500370)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection | NavigationContextLevels.Project)]
    public ActionResult ReadScopedGroupsJson(
      string lastSearchResult,
      string searchQuery,
      int? pageSize,
      bool isOrganizationLevel = false)
    {
      pageSize = new int?(IdentityManagementHelpers.GetPageSize(pageSize));
      TeamFoundationIdentityService service = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity[] source = service.ListApplicationGroups(this.TfsRequestContext, this.TfsWebContext.CurrentProjectUri != (Uri) null ? this.TfsWebContext.CurrentProjectUri.ToString() : (string) null, ReadIdentityOptions.None, (IEnumerable<string>) null);
      TeamFoundationIdentity[] foundationIdentityArray1 = service.ReadIdentities(this.TfsRequestContext, ((IEnumerable<TeamFoundationIdentity>) source).Select<TeamFoundationIdentity, Guid>((Func<TeamFoundationIdentity, Guid>) (x => x.TeamFoundationId)).ToArray<Guid>(), MembershipQuery.Direct, ReadIdentityOptions.None, (IEnumerable<string>) null);
      if (((IEnumerable<TeamFoundationIdentity>) foundationIdentityArray1).IsNullOrEmpty<TeamFoundationIdentity>())
        return (ActionResult) this.Json((object) IdentityManagementHelpers.BuildFilteredIdentitiesJsonViewModel((IEnumerable<TeamFoundationIdentity>) null, false, 0), JsonRequestBehavior.AllowGet);
      HashSet<IdentityDescriptor> identityDescriptorSet = new HashSet<IdentityDescriptor>(((IEnumerable<TeamFoundationIdentity>) foundationIdentityArray1).SelectMany<TeamFoundationIdentity, IdentityDescriptor>((Func<TeamFoundationIdentity, IEnumerable<IdentityDescriptor>>) (x => (IEnumerable<IdentityDescriptor>) x.Members)), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      if (!string.IsNullOrEmpty(lastSearchResult) || !string.IsNullOrEmpty(searchQuery))
        return this.ReadIdentitiesPageJson(lastSearchResult, searchQuery, "groups", pageSize);
      TeamFoundationIdentity[] foundationIdentityArray2 = service.ReadIdentities(this.TfsRequestContext, ((IEnumerable<TeamFoundationIdentity>) foundationIdentityArray1).Select<TeamFoundationIdentity, IdentityDescriptor>((Func<TeamFoundationIdentity, IdentityDescriptor>) (x => x.Descriptor)).ToArray<IdentityDescriptor>(), MembershipQuery.Direct, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
      {
        TeamConstants.TeamPropertyName
      }, IdentityPropertyScope.Local);
      if (this.IsHosted)
      {
        List<TeamFoundationIdentity> second = new List<TeamFoundationIdentity>();
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
        foreach (TeamFoundationIdentity applicationGroup in vssRequestContext.GetService<TeamFoundationIdentityService>().ListApplicationGroups(vssRequestContext, (string) null, ReadIdentityOptions.None, (IEnumerable<string>) null))
        {
          if (AadIdentityHelper.IsAadGroup(applicationGroup.Descriptor))
            second.Add(applicationGroup);
        }
        foundationIdentityArray2 = ((IEnumerable<TeamFoundationIdentity>) foundationIdentityArray2).Concat<TeamFoundationIdentity>((IEnumerable<TeamFoundationIdentity>) second).ToArray<TeamFoundationIdentity>();
      }
      Array.Sort<TeamFoundationIdentity>(foundationIdentityArray2, (IComparer<TeamFoundationIdentity>) new TeamFoundationIdentityDisplayNameComparer());
      int count = identityDescriptorSet.Count;
      int? nullable = pageSize;
      int valueOrDefault = nullable.GetValueOrDefault();
      TeamFoundationFilteredIdentitiesList filteredIdentities;
      if (count > valueOrDefault & nullable.HasValue)
      {
        filteredIdentities = new TeamFoundationFilteredIdentitiesList();
        filteredIdentities.HasMoreItems = true;
        filteredIdentities.Items = foundationIdentityArray2;
      }
      else
      {
        filteredIdentities = this.ReadIdentitiesPageInternalJson(lastSearchResult, searchQuery, "windowsGroups", pageSize);
        filteredIdentities.Items = ((IEnumerable<TeamFoundationIdentity>) foundationIdentityArray2).Concat<TeamFoundationIdentity>((IEnumerable<TeamFoundationIdentity>) filteredIdentities.Items).ToArray<TeamFoundationIdentity>();
      }
      return this.JsonFromFilteredIdentitiesList(filteredIdentities);
    }

    [HttpGet]
    [AcceptNavigationLevels(NavigationContextLevels.Collection | NavigationContextLevels.Project)]
    public ActionResult ReadScopedUsersJson(
      string lastSearchResult,
      string searchQuery,
      int? pageSize,
      bool isOrganizationLevel = false)
    {
      pageSize = new int?(IdentityManagementHelpers.GetPageSize(pageSize));
      TeamFoundationIdentityService service = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity[] foundationIdentityArray = service.ReadIdentities(this.TfsRequestContext, ((IEnumerable<TeamFoundationIdentity>) service.ListApplicationGroups(this.TfsRequestContext, this.TfsWebContext.CurrentProjectUri != (Uri) null ? this.TfsWebContext.CurrentProjectUri.ToString() : (string) null, ReadIdentityOptions.None, (IEnumerable<string>) null)).Select<TeamFoundationIdentity, Guid>((Func<TeamFoundationIdentity, Guid>) (x => x.TeamFoundationId)).ToArray<Guid>(), MembershipQuery.Expanded, ReadIdentityOptions.None, (IEnumerable<string>) null);
      if (((IEnumerable<TeamFoundationIdentity>) foundationIdentityArray).IsNullOrEmpty<TeamFoundationIdentity>())
        return (ActionResult) this.Json((object) IdentityManagementHelpers.BuildFilteredIdentitiesJsonViewModel((IEnumerable<TeamFoundationIdentity>) null, false, 0), JsonRequestBehavior.AllowGet);
      HashSet<IdentityDescriptor> identityDescriptorSet = new HashSet<IdentityDescriptor>(((IEnumerable<TeamFoundationIdentity>) foundationIdentityArray).SelectMany<TeamFoundationIdentity, IdentityDescriptor>((Func<TeamFoundationIdentity, IEnumerable<IdentityDescriptor>>) (x => x.Members.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (m => !string.Equals(m.IdentityType, "Microsoft.TeamFoundation.Identity"))))), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      if (string.IsNullOrEmpty(lastSearchResult) && string.IsNullOrEmpty(searchQuery))
      {
        int count = identityDescriptorSet.Count;
        int? nullable = pageSize;
        int valueOrDefault = nullable.GetValueOrDefault();
        if (count > valueOrDefault & nullable.HasValue)
          return this.JsonFromFilteredIdentitiesList(new TeamFoundationFilteredIdentitiesList()
          {
            HasMoreItems = true
          });
      }
      return this.ReadIdentitiesPageJson(lastSearchResult, searchQuery, "users", pageSize);
    }

    [HttpGet]
    [AcceptNavigationLevels(NavigationContextLevels.Team)]
    public ActionResult ReadScopedTeamJson(
      string lastSearchResult,
      string searchQuery,
      int? pageSize,
      bool? force,
      bool isOrganizationLevel = false)
    {
      force = new bool?(force.GetValueOrDefault());
      pageSize = new int?(IdentityManagementHelpers.GetPageSize(pageSize));
      TeamFoundationIdentity foundationIdentity = this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentity(this.TfsRequestContext, this.Team.Identity.Descriptor, MembershipQuery.Expanded, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
      {
        TeamConstants.TeamPropertyName
      }, IdentityPropertyScope.Local);
      TeamFoundationFilteredIdentitiesList filteredIdentities;
      if (string.IsNullOrEmpty(lastSearchResult) && string.IsNullOrEmpty(searchQuery) && !force.Value)
      {
        int count = foundationIdentity.Members.Count;
        int? nullable = pageSize;
        int valueOrDefault = nullable.GetValueOrDefault();
        if (count > valueOrDefault & nullable.HasValue)
        {
          filteredIdentities = new TeamFoundationFilteredIdentitiesList();
          filteredIdentities.HasMoreItems = true;
          filteredIdentities.Items = new TeamFoundationIdentity[1]
          {
            foundationIdentity
          };
          goto label_5;
        }
      }
      filteredIdentities = this.ReadGroupMembersInternal(lastSearchResult, searchQuery, true, foundationIdentity.TeamFoundationId, new MembershipQuery?(MembershipQuery.Expanded), pageSize);
      if (string.IsNullOrEmpty(lastSearchResult) && string.IsNullOrEmpty(searchQuery) && !force.Value)
      {
        List<TeamFoundationIdentity> foundationIdentityList = new List<TeamFoundationIdentity>((IEnumerable<TeamFoundationIdentity>) filteredIdentities.Items);
        foundationIdentityList.Insert(0, foundationIdentity);
        filteredIdentities.Items = foundationIdentityList.ToArray();
      }
label_5:
      return this.JsonFromFilteredIdentitiesList(filteredIdentities);
    }

    [HttpGet]
    [TfsTraceFilter(500340, 500350)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection | NavigationContextLevels.Project)]
    public ActionResult ReadScopedApplicationGroupsJson(bool isOrganizationLevel = false)
    {
      TeamFoundationIdentity[] applicationGroups = this.GetScopedApplicationGroups(isOrganizationLevel);
      Array.Sort<TeamFoundationIdentity>(applicationGroups, (IComparer<TeamFoundationIdentity>) new TeamFoundationIdentityDisplayNameComparer());
      return this.JsonFromFilteredIdentitiesList(new TeamFoundationFilteredIdentitiesList()
      {
        HasMoreItems = false,
        Items = applicationGroups
      });
    }

    internal TeamFoundationIdentity[] GetScopedApplicationGroups(bool isOrganizationLevel)
    {
      if (ApiIdentityController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      TeamFoundationIdentity[] source = this.TfsRequestContext.GetService<ITeamFoundationIdentityService>().ListApplicationGroups(this.TfsRequestContext, this.TfsWebContext.CurrentProjectUri != (Uri) null ? this.TfsWebContext.CurrentProjectUri.ToString() : (string) null, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
      {
        TeamConstants.TeamPropertyName
      }, IdentityPropertyScope.Local);
      if (this.TfsRequestContext.IsHosted())
      {
        List<TeamFoundationIdentity> foundationIdentityList = new List<TeamFoundationIdentity>();
        foreach (TeamFoundationIdentity foundationIdentity in ((IEnumerable<TeamFoundationIdentity>) source).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (x => x != null && !AadIdentityHelper.IsAadGroup(x.Descriptor))).ToArray<TeamFoundationIdentity>())
        {
          if (foundationIdentity.GetProperty<string>("RestrictedVisible", (string) null) != "RestrictedVisible")
            foundationIdentityList.Add(foundationIdentity);
          else
            this.TfsRequestContext.Trace(10050061, TraceLevel.Info, "General", nameof (ApiIdentityController), foundationIdentity.Descriptor.Identifier);
        }
        source = foundationIdentityList.ToArray();
      }
      return source;
    }

    [HttpGet]
    [AcceptNavigationLevels(NavigationContextLevels.Application)]
    public ActionResult ReadLicenseUsers(Guid licenseTypeId)
    {
      this.CheckManageLicensesPermission();
      TeamFoundationOnPremLicensingService service1 = this.TfsRequestContext.GetService<TeamFoundationOnPremLicensingService>();
      TeamFoundationIdentityService service2 = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid licenseTypeId1 = licenseTypeId;
      ICollection<IdentityDescriptor> licenseUsers = service1.GetLicenseUsers(tfsRequestContext, licenseTypeId1, MembershipQuery.Direct);
      TeamFoundationFilteredIdentitiesList filteredIdentities = new TeamFoundationFilteredIdentitiesList();
      filteredIdentities.Items = service2.ReadIdentities(this.TfsRequestContext, licenseUsers.ToArray<IdentityDescriptor>());
      Array.Sort<TeamFoundationIdentity>(filteredIdentities.Items, (IComparer<TeamFoundationIdentity>) new TeamFoundationIdentityDisplayNameComparer());
      return this.JsonFromFilteredIdentitiesList(filteredIdentities);
    }

    private ActionResult JsonFromFilteredIdentitiesList(
      TeamFoundationFilteredIdentitiesList filteredIdentities,
      bool filterServiceIdentities = false)
    {
      IEnumerable<TeamFoundationIdentity> foundationIdentities = (IEnumerable<TeamFoundationIdentity>) filteredIdentities.Items;
      bool hasMoreItems = filteredIdentities.HasMoreItems;
      int totalItems = filteredIdentities.TotalItems;
      if (foundationIdentities != null)
      {
        if (!this.TfsRequestContext.IsFeatureEnabled("Identity.AnonymousPrincipal"))
          foundationIdentities = foundationIdentities.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (s => !IdentityHelper.IsAnonymousPrincipal(s.Descriptor) && !IdentityHelper.IsAnonymousUsersGroup(s.Descriptor)));
        foundationIdentities = foundationIdentities.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (s => !ServicePrincipals.IsServicePrincipal(this.TfsRequestContext, s.Descriptor) && !IdentityHelper.IsWellKnownGroup(s.Descriptor, GroupWellKnownIdentityDescriptors.ServicePrincipalGroup)));
        if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment && this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Application))
          foundationIdentities = foundationIdentities.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (s => !IdentityHelper.IsWellKnownGroup(s.Descriptor, GroupWellKnownIdentityDescriptors.InvitedUsersGroup)));
        if (filterServiceIdentities)
          foundationIdentities = foundationIdentities.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (s => !IdentityHelper.IsServiceIdentity(this.TfsRequestContext, (IReadOnlyVssIdentity) IdentityUtil.Convert(this.TfsRequestContext, s))));
      }
      return (ActionResult) this.Json((object) IdentityManagementHelpers.BuildFilteredIdentitiesJsonViewModel(foundationIdentities, hasMoreItems, totalItems), JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [ValidateInput(false)]
    public ActionResult ReadIdentitiesPageJson(
      string lastSearchResult,
      string searchQuery,
      string membershipType,
      int? pageSize,
      bool filterServiceIdentities = false)
    {
      return this.JsonFromFilteredIdentitiesList(this.ReadIdentitiesPageInternalJson(lastSearchResult, searchQuery, membershipType, pageSize), filterServiceIdentities);
    }

    private TeamFoundationFilteredIdentitiesList ReadIdentitiesPageInternalJson(
      string lastSearchResult,
      string searchQuery,
      string membershipType,
      int? pageSize)
    {
      TeamFoundationIdentityService service = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
      pageSize = new int?(IdentityManagementHelpers.GetPageSize(pageSize));
      List<IdentityFilter> filters = new List<IdentityFilter>();
      if (!string.IsNullOrEmpty(searchQuery))
        filters.Add((IdentityFilter) new DisplayNameIdentityFilter(searchQuery, true));
      bool flag = false;
      if (!string.IsNullOrEmpty(membershipType))
      {
        switch (membershipType)
        {
          case "users":
            filters.Add((IdentityFilter) new MembershipTypeIdentityFilter(MembershipTypeIdentityFilter.MembershipType.Users));
            break;
          case "groups":
            filters.Add((IdentityFilter) new MembershipTypeIdentityFilter(MembershipTypeIdentityFilter.MembershipType.Groups));
            filters.Add((IdentityFilter) new AttributeMatchIdentityFilter("ScopeName", "TEAM FOUNDATION", false));
            break;
          case "windowsGroups":
            filters.Add((IdentityFilter) new MembershipTypeIdentityFilter(MembershipTypeIdentityFilter.MembershipType.Groups));
            filters.Add((IdentityFilter) new DescriptorTypeIdentityFilter("Microsoft.TeamFoundation.Identity", false));
            break;
          case "addableUsers":
            filters.Add((IdentityFilter) new DescriptorTypeIdentityFilter("Microsoft.TeamFoundation.Identity", false));
            flag = true;
            break;
          case "emailableUsers":
            filters.Add((IdentityFilter) new AttributeMatchIdentityFilter("Mail", "@", findAny: true));
            filters.Add((IdentityFilter) new MembershipTypeIdentityFilter(MembershipTypeIdentityFilter.MembershipType.Users));
            flag = true;
            break;
        }
      }
      else if (this.IsHosted)
        filters.Add((IdentityFilter) new AttributeMatchIdentityFilter("ScopeName", "TEAM FOUNDATION", false));
      string str = this.TfsWebContext.CurrentProjectUri != (Uri) null ? this.TfsWebContext.CurrentProjectUri.ToString() : (string) null;
      TeamFoundationFilteredIdentitiesList filteredIdentities = service.ReadFilteredIdentities(this.TfsRequestContext, flag ? (string) null : str, pageSize.Value, (IEnumerable<IdentityFilter>) filters, lastSearchResult, true, MembershipQuery.None, (IEnumerable<string>) new string[1]
      {
        TeamConstants.TeamPropertyName
      }, IdentityPropertyScope.Local);
      this.CheckForAccountName(this.TfsRequestContext, filteredIdentities, searchQuery, lastSearchResult, membershipType);
      return filteredIdentities;
    }

    private void CheckForAccountName(
      IVssRequestContext requestContext,
      TeamFoundationFilteredIdentitiesList filteredIdentities,
      string searchQuery,
      string lastSearchResult,
      string membershipType)
    {
      TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity checkedName = (TeamFoundationIdentity) null;
      if (string.IsNullOrEmpty(searchQuery))
        return;
      if (!string.IsNullOrEmpty(lastSearchResult))
        return;
      try
      {
        checkedName = service.ReadIdentity(this.TfsRequestContext, IdentitySearchFactor.AccountName, searchQuery, MembershipQuery.None, ReadIdentityOptions.TrueSid, (IEnumerable<string>) null);
        if (checkedName != null)
        {
          if (!checkedName.IsActive)
            checkedName = (TeamFoundationIdentity) null;
          else if (!service.IsMember(this.TfsRequestContext, GroupWellKnownIdentityDescriptors.EveryoneGroup, checkedName.Descriptor))
            checkedName = (TeamFoundationIdentity) null;
        }
      }
      catch (Exception ex)
      {
        this.TraceException(599999, ex);
      }
      if (checkedName != null)
      {
        switch (membershipType)
        {
          case "users":
            if (checkedName.IsContainer)
            {
              checkedName = (TeamFoundationIdentity) null;
              break;
            }
            break;
          case "groups":
          case "windowsGroups":
            if (!checkedName.IsContainer)
            {
              checkedName = (TeamFoundationIdentity) null;
              break;
            }
            break;
        }
      }
      if (checkedName == null)
        return;
      List<TeamFoundationIdentity> foundationIdentityList = new List<TeamFoundationIdentity>(((IEnumerable<TeamFoundationIdentity>) filteredIdentities.Items).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (x => !IdentityDescriptorComparer.Instance.Equals(x.Descriptor, checkedName.Descriptor))));
      foundationIdentityList.Insert(0, checkedName);
      filteredIdentities.Items = foundationIdentityList.ToArray();
    }

    [HttpGet]
    [TfsTraceFilter(500610, 500619)]
    public ActionResult ReadGroupMembers(
      string lastSearchResult,
      string searchQuery,
      bool readMembers,
      Guid scope,
      MembershipQuery? scopedMembershipQuery,
      int? pageSize,
      string membershipType = null,
      bool isOrganizationLevel = false)
    {
      if (ApiIdentityController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      if (!this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.AadGroupsAdminUi") || this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment || !this.IsAADAccount() || !readMembers)
        return this.ReadGroupMembersDefault(lastSearchResult, searchQuery, readMembers, scope, scopedMembershipQuery, pageSize, membershipType);
      TeamFoundationIdentity[] source = this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, new Guid[1]
      {
        scope
      }, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
      if (source == null || source.Length == 0 || ((IEnumerable<TeamFoundationIdentity>) source).First<TeamFoundationIdentity>() == null)
        return (ActionResult) new HttpStatusCodeResult(HttpStatusCode.NoContent);
      TeamFoundationIdentity group = ((IEnumerable<TeamFoundationIdentity>) source).First<TeamFoundationIdentity>();
      if (!group.IsContainer)
        throw new IdentityNotFoundException(AdminServerResources.CouldNotFindGroup);
      return !AadIdentityHelper.IsAadGroup(group.Descriptor) ? this.ReadGroupMembersDefault(lastSearchResult, searchQuery, readMembers, scope, scopedMembershipQuery, pageSize, membershipType) : this.ReadGroupMembersFromDds(this.TfsRequestContext, group, pageSize ?? 100);
    }

    private ActionResult ReadGroupMembersDefault(
      string lastSearchResult,
      string searchQuery,
      bool readMembers,
      Guid scope,
      MembershipQuery? scopedMembershipQuery,
      int? pageSize,
      string membershipType = null)
    {
      return this.JsonFromFilteredIdentitiesList(this.ReadGroupMembersInternal(lastSearchResult, searchQuery, readMembers, scope, scopedMembershipQuery, pageSize, membershipType));
    }

    [TfsTraceFilter(500800, 500809)]
    private TeamFoundationFilteredIdentitiesList ReadGroupMembersInternal(
      string lastSearchResult,
      string searchQuery,
      bool readMembers,
      Guid scope,
      MembershipQuery? scopedMembershipQuery,
      int? pageSize,
      string membershipType = null)
    {
      List<IdentityFilter> filters = new List<IdentityFilter>();
      if (!string.IsNullOrEmpty(searchQuery))
        filters.Add((IdentityFilter) new DisplayNameIdentityFilter(searchQuery, true));
      if (!string.IsNullOrEmpty(membershipType))
      {
        switch (membershipType.ToLower())
        {
          case "users":
            filters.Add((IdentityFilter) new MembershipTypeIdentityFilter(MembershipTypeIdentityFilter.MembershipType.Users));
            break;
          case "groups":
            filters.Add((IdentityFilter) new MembershipTypeIdentityFilter(MembershipTypeIdentityFilter.MembershipType.Groups));
            break;
        }
      }
      int pageSize1 = IdentityManagementHelpers.GetPageSize(pageSize);
      return this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadFilteredIdentitiesById(this.TfsRequestContext, new Guid[1]
      {
        scope
      }, pageSize1, (IEnumerable<IdentityFilter>) filters, lastSearchResult, true, (MembershipQuery) ((int) scopedMembershipQuery ?? 1), MembershipQuery.None, (readMembers ? 1 : 0) != 0, (IEnumerable<string>) null);
    }

    [TfsTraceFilter(500600, 500609)]
    private ActionResult ReadGroupMembersFromDds(
      IVssRequestContext requestContext,
      TeamFoundationIdentity group,
      int maxResults = 100,
      string pagingToken = null)
    {
      try
      {
        IDirectoryEntity fromTfIdentifier = DirectoryDiscoveryServiceHelper.GetEntityFromTFIdentifier(requestContext, group.TeamFoundationId.ToString());
        if (fromTfIdentifier == null)
          throw new DirectoryRelatedEntitiesUnavailableException(AdminServerResources.UnableToResolveGroup);
        string[] strArray = new string[7]
        {
          "DisplayName",
          "ScopeName",
          "Mail",
          "MailNickname",
          "SignInAddress",
          "Surname",
          "Description"
        };
        DirectoryDiscoveryService service = requestContext.GetService<DirectoryDiscoveryService>();
        IVssRequestContext context = requestContext;
        DirectoryGetRelatedEntitiesRequest request = new DirectoryGetRelatedEntitiesRequest();
        request.Directories = (IEnumerable<string>) new List<string>()
        {
          "vsd",
          "src"
        };
        request.Relation = "Member";
        request.Depth = 1;
        request.MaxResults = new int?(maxResults);
        request.MinResults = new int?(maxResults);
        request.EntityIds = (IEnumerable<string>) new List<string>()
        {
          fromTfIdentifier.EntityId
        };
        request.PropertiesToReturn = (IEnumerable<string>) strArray;
        DirectoryGetRelatedEntitiesResponse relatedEntities = service.GetRelatedEntities(context, request);
        DirectoryGetRelatedEntitiesResult relatedEntitiesResult;
        if (relatedEntities == null || relatedEntities.Results == null || relatedEntities.Results.Count == 0 || !relatedEntities.Results.TryGetValue(fromTfIdentifier.EntityId, out relatedEntitiesResult))
          throw new DirectoryRelatedEntitiesUnavailableException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AdminServerResources.UnableToRetrieveRelatedIdentities, (object) fromTfIdentifier.EntityId));
        if (relatedEntitiesResult.Exception != null)
          throw new DirectoryRelatedEntitiesUnavailableException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AdminServerResources.UnableToRetrieveRelatedIdentities, (object) fromTfIdentifier.EntityId), relatedEntitiesResult.Exception);
        return (ActionResult) this.ConstructJsonResponseForAdminUI(relatedEntities.Results[fromTfIdentifier.EntityId].Entities, relatedEntities.HasMoreResults);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(500608, "WebAccess", TfsTraceLayers.Content, ex);
        throw;
      }
    }

    private JsonResult ConstructJsonResponseForAdminUI(
      IEnumerable<IDirectoryEntity> entities,
      bool hasMoreResults = false)
    {
      IEnumerable<JsObject> jsObjects = entities.OrderBy<IDirectoryEntity, string>((Func<IDirectoryEntity, string>) (x => x.DisplayName)).Select<IDirectoryEntity, JsObject>((Func<IDirectoryEntity, JsObject>) (x => new DirectoryEntityViewModel(x).ToJson()));
      JsObject data = new JsObject();
      data.Add("identities", (object) jsObjects);
      data.Add("hasMore", (object) hasMoreResults);
      data.Add("totalIdentityCount", (object) entities.Count<IDirectoryEntity>());
      return this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [TfsTraceFilter(500380, 500390)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult ManageGroup(Guid? tfid, bool isOrganizationLevel = false)
    {
      if (ApiIdentityController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      IdentityViewModelBase identityViewModelBase = (IdentityViewModelBase) new GroupIdentityViewModel();
      if (tfid.HasValue)
      {
        try
        {
          identityViewModelBase = IdentityManagementHelpers.GetIdentityViewModel(this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, new Guid[1]
          {
            tfid.Value
          }, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
          {
            TeamConstants.TeamPropertyName
          }, IdentityPropertyScope.Local)[0]);
          if (identityViewModelBase is UserIdentityViewModel)
            identityViewModelBase.Errors.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AdminServerResources.UnableToFindGroup, (object) tfid.Value));
        }
        catch (IdentityServiceException ex)
        {
          identityViewModelBase.Errors.Add(ex.Message);
          this.TraceException(599999, (Exception) ex);
        }
        identityViewModelBase.IsForEdit = true;
      }
      return (ActionResult) this.Json((object) identityViewModelBase.ToJson(), JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    [AcceptNavigationLevels(NavigationContextLevels.ApplicationAll)]
    [TfsTraceFilter(500120, 500130)]
    [ValidateInput(false)]
    public ActionResult AddAadGroups(
      string newUsersJson,
      string existingUsersJson,
      string groupsToJoinJson)
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment || !this.TfsRequestContext.GetUserIdentity().IsExternalUser)
        return (ActionResult) this.Json((object) new MembershipModel()
        {
          GeneralErrors = {
            AdminServerResources.AADGroupNotSupported
          }
        });
      IList<TeamFoundationIdentity> foundationIdentityList = IdentityManagementHelpers.ResolveIdentities((ITfsController) this, IdentityManagementHelpers.ParseTfidsJson(groupsToJoinJson));
      IList<TeamFoundationIdentity> list1 = (IList<TeamFoundationIdentity>) foundationIdentityList.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (group => !ApiIdentityController.HasManageGroupMembershipPermission(this.TfsRequestContext, group))).ToList<TeamFoundationIdentity>();
      IList<TeamFoundationIdentity> list2 = (IList<TeamFoundationIdentity>) foundationIdentityList.Except<TeamFoundationIdentity>((IEnumerable<TeamFoundationIdentity>) list1).ToList<TeamFoundationIdentity>();
      MembershipModel membershipModel = (MembershipModel) null;
      if (list2.Count > 0)
      {
        IList<Guid> source = (IList<Guid>) new List<Guid>();
        Guid[] tfidsJson = IdentityManagementHelpers.ParseTfidsJson(existingUsersJson);
        List<string> failureMessages = (List<string>) null;
        List<TeamFoundationIdentity> aadGroupsWithCreationFailure = (List<TeamFoundationIdentity>) null;
        if (tfidsJson.Length != 0)
          source = this.TfsRequestContext.GetExtension<IAadGroupHelper>(ExtensionLifetime.Service).CreateAadGroupsInIms(this.TfsRequestContext.Elevate(), tfidsJson, out aadGroupsWithCreationFailure, out failureMessages);
        string existingUsersJson1 = string.Empty;
        if (source.Count > 0)
          existingUsersJson1 = !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? new JavaScriptSerializer().Serialize((object) source.ToArray<Guid>()) : JsonConvert.SerializeObject((object) source.ToArray<Guid>());
        membershipModel = this.AddIdentitiesInternal(string.Empty, existingUsersJson1, list2);
        if (failureMessages != null && failureMessages.Count > 0)
          membershipModel.GeneralErrors.AddRange((IEnumerable<string>) failureMessages);
        if (aadGroupsWithCreationFailure != null && aadGroupsWithCreationFailure.Count > 0)
        {
          List<IdentityViewModelBase> collection = new List<IdentityViewModelBase>();
          foreach (TeamFoundationIdentity identity in aadGroupsWithCreationFailure)
            collection.Add(IdentityManagementHelpers.GetIdentityViewModel(identity));
          membershipModel.FailedAddedIdentities.AddRange((IEnumerable<IdentityViewModelBase>) collection);
        }
      }
      if (list1 != null && list1.Count > 0)
      {
        string str = string.Join(", ", list1.Select<TeamFoundationIdentity, string>((Func<TeamFoundationIdentity, string>) (x => x.DisplayName)));
        if (membershipModel == null)
          membershipModel = new MembershipModel();
        membershipModel.GeneralErrors.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AdminServerResources.NoPermissionsToAddAADGroupToVSOGroups, (object) str));
      }
      return (ActionResult) this.Json((object) membershipModel.ToJson());
    }

    [ValidateInput(false)]
    [HttpPost]
    [TfsTraceFilter(500420, 500430)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult ManageGroup(
      string name,
      string description,
      Guid? tfid,
      bool isOrganizationLevel = false)
    {
      if (ApiIdentityController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException(AdminServerResources.NameRequired);
      TeamFoundationIdentityService service = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity foundationIdentity;
      if (tfid.HasValue)
      {
        foundationIdentity = service.ReadIdentities(this.TfsRequestContext, new Guid[1]
        {
          tfid.Value
        }, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
        {
          TeamConstants.TeamPropertyName
        }, IdentityPropertyScope.Local)[0];
        if (foundationIdentity == null)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AdminServerResources.UnableToFindGroup, (object) tfid.Value));
        if (GroupHelpers.IsReadOnlyTfsGroup(this.TfsRequestContext, foundationIdentity) || !GroupHelpers.IsApplicationGroup(foundationIdentity))
          throw new TeamFoundationServiceException(AdminServerResources.GroupNameCannotBeModified);
        if (!ApiIdentityController.HasManageGroupMembershipPermission(this.TfsRequestContext, foundationIdentity))
          throw new InvalidOperationException(AdminServerResources.NoPermissionsGroupProperties);
        bool flag = false;
        string name1;
        UserNameUtil.Parse(foundationIdentity.DisplayName, out name1, out string _);
        if (!string.Equals(name1, name))
        {
          if (foundationIdentity.TryGetProperty(TeamConstants.TeamPropertyName, out object _))
            TeamsUtility.CheckTeamName(name);
          service.UpdateApplicationGroup(this.TfsRequestContext, foundationIdentity.Descriptor, GroupProperty.Name, name);
          flag = true;
        }
        if (!string.Equals(foundationIdentity.GetAttribute("Description", string.Empty), description))
        {
          service.UpdateApplicationGroup(this.TfsRequestContext, foundationIdentity.Descriptor, GroupProperty.Description, description);
          flag = true;
        }
        if (flag)
          foundationIdentity = service.ReadIdentities(this.TfsRequestContext, new Guid[1]
          {
            tfid.Value
          }, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
          {
            TeamConstants.TeamPropertyName
          }, IdentityPropertyScope.Local)[0];
      }
      else
      {
        string scopeId = !(this.TfsWebContext.CurrentProjectUri == (Uri) null) ? this.TfsWebContext.CurrentProjectUri.ToString() : (string) null;
        foundationIdentity = service.CreateApplicationGroup(this.TfsRequestContext, scopeId, name, description);
      }
      return (ActionResult) this.Json((object) IdentityManagementHelpers.GetIdentityViewModel(foundationIdentity).ToJson());
    }

    [HttpPost]
    [TfsTraceFilter(500440, 500450)]
    [AcceptNavigationLevels(NavigationContextLevels.Application)]
    public ActionResult EditLicenseMembership(
      Guid licenseTypeId,
      string addItemsJson,
      string removeItemsJson,
      bool editMembers)
    {
      return (ActionResult) this.Json((object) this.EditMembershipInternal(this.TfsRequestContext.GetService<TeamFoundationOnPremLicensingService>().GetLicenseGroup(this.TfsRequestContext, licenseTypeId), addItemsJson, removeItemsJson, editMembers).ToJson());
    }

    [HttpPost]
    [TfsTraceFilter(500440, 500450)]
    [AcceptNavigationLevels(NavigationContextLevels.ApplicationAll)]
    public ActionResult EditMembership(
      Guid groupId,
      string addItemsJson,
      string removeItemsJson,
      bool editMembers,
      bool isOrganizationLevel = false)
    {
      if (ApiIdentityController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      return (ActionResult) this.Json((object) this.EditMembershipInternal(this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, new Guid[1]
      {
        groupId
      })[0], addItemsJson, removeItemsJson, editMembers).ToJson());
    }

    private void CheckIfAWellKnownGroupIsRemoved(
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      IdentityDescriptor wellKnownDescriptor,
      string parentGroupName)
    {
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
      {
        if (IdentityDescriptorComparer.Instance.Equals(identity.Descriptor, wellKnownDescriptor))
        {
          this.Trace(500484, TraceLevel.Error, string.Format("{0}: Cannot remove '{1}' group from '{2}' group", (object) "ExecuteProjectCollectionAdministratorsGroupMembershipChangePolicy", (object) identity.DisplayName, (object) parentGroupName));
          throw new InvalidOperationException(string.Format(AdminServerResources.CannotRemoveMembershipGroup, (object) identity.DisplayName, (object) parentGroupName));
        }
      }
    }

    private void ExecuteProjectCollectionAdministratorsGroupMembershipChangePolicy(
      IVssRequestContext requestContext,
      TeamFoundationIdentity group,
      IList<Guid> memberIds)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        return;
      requestContext.CheckProjectCollectionRequestContext();
      if (!IdentityHelper.IsWellKnownGroup(group.Descriptor, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup))
        return;
      IdentityDomain identityDomain = new IdentityDomain(requestContext);
      IdentityDescriptor y = identityDomain.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup);
      if (!IdentityDescriptorComparer.Instance.Equals(identityDomain.MapFromWellKnownIdentifier(group.Descriptor), y))
        return;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, memberIds, Microsoft.VisualStudio.Services.Identity.QueryMembership.None, (IEnumerable<string>) null);
      IdentityDescriptor wellKnownDescriptor1 = new IdentityDomain(requestContext.To(TeamFoundationHostType.Application)).MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup);
      this.CheckIfAWellKnownGroupIsRemoved((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList, wellKnownDescriptor1, group.DisplayName);
      IdentityDescriptor wellKnownDescriptor2 = identityDomain.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.ServiceUsersGroup);
      this.CheckIfAWellKnownGroupIsRemoved((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList, wellKnownDescriptor2, group.DisplayName);
    }

    private void ExecuteCollectionOwnerMembershipChangePolicy(
      IVssRequestContext requestContext,
      TeamFoundationIdentity group,
      IList<Guid> memberIds)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment || requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        return;
      requestContext.CheckProjectCollectionRequestContext();
      if (!IdentityHelper.IsWellKnownGroup(group.Descriptor, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup))
        return;
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
      if (group.GetProperty<Guid>("ScopeId", Guid.Empty) != context.ServiceHost.InstanceId || context.GetService<IOrganizationService>().GetOrganization(context, (IEnumerable<string>) null).IsActivated)
        return;
      IdentityService service = context.GetService<IdentityService>();
      Guid collectionOwner = this.GetCollectionOwner(requestContext);
      IVssRequestContext requestContext1 = context;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = service.ReadIdentities(requestContext1, (IList<Guid>) new List<Guid>()
      {
        collectionOwner
      }, Microsoft.VisualStudio.Services.Identity.QueryMembership.None, (IEnumerable<string>) null);
      if ((source != null ? (source.Count != 1 ? 1 : 0) : 1) != 0)
      {
        this.Trace(500473, TraceLevel.Error, "ExecuteAccountGroupMembershipChangePolicy: unique account owner identity not found at account level.");
      }
      else
      {
        Guid collectionOwnerGuid = source.Single<Microsoft.VisualStudio.Services.Identity.Identity>().Id;
        if (memberIds.Any<Guid>((Func<Guid, bool>) (x => x == collectionOwnerGuid)))
        {
          this.Trace(500474, TraceLevel.Error, "ExecuteAccountGroupMembershipChangePolicy: Cannot change the account owner's membership in the account Team Foundation Administrators group.");
          throw new InvalidOperationException(AdminServerResources.CannotDeleteAccountOwnerFromAccountAdminGroup);
        }
      }
    }

    private Guid GetCollectionOwner(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      return requestContext.GetService<ICollectionService>().GetCollection(requestContext.Elevate(), (IEnumerable<string>) null).Owner;
    }

    internal void ExecuteGroupMembershipChangePolicy(
      IVssRequestContext requestContext,
      TeamFoundationIdentity group,
      IList<Guid> memberIds)
    {
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DisableGroupMembershipSecurityPolicy"))
        return;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        this.Trace(500461, TraceLevel.Error, "ExecuteGroupMembershipChangePolicy: called at deployment level");
      else if (memberIds == null || group == null)
        this.Trace(500462, TraceLevel.Error, "ExecuteGroupMembershipChangePolicy: memberIds and group are required params and cannot be null");
      else if (memberIds.Count == 0)
      {
        this.Trace(500463, TraceLevel.Error, "ExecuteGroupMembershipChangePolicy: memberIds is empty");
      }
      else
      {
        this.ExecuteCollectionOwnerMembershipChangePolicy(requestContext, group, memberIds);
        this.ExecuteProjectCollectionAdministratorsGroupMembershipChangePolicy(requestContext, group, memberIds);
      }
    }

    private MembershipModel EditMembershipInternal(
      TeamFoundationIdentity anchorIdentity,
      string addItemsJson,
      string removeItemsJson,
      bool editMembers)
    {
      Guid[] tfidsJson1 = IdentityManagementHelpers.ParseTfidsJson(addItemsJson);
      Guid[] tfidsJson2 = IdentityManagementHelpers.ParseTfidsJson(removeItemsJson);
      this.ExecuteGroupMembershipChangePolicy(this.TfsRequestContext, anchorIdentity, (IList<Guid>) tfidsJson2);
      MembershipModel model = new MembershipModel();
      model.EditMembers = editMembers;
      try
      {
        TeamFoundationIdentityService service = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
        TeamFoundationIdentity[] foundationIdentityArray1 = service.ReadIdentities(this.TfsRequestContext, tfidsJson1);
        if (model.EditMembers)
          this.AddMembers(foundationIdentityArray1, anchorIdentity, model);
        else
          this.JoinGroups(foundationIdentityArray1, anchorIdentity, model);
        TeamFoundationIdentity[] foundationIdentityArray2 = service.ReadIdentities(this.TfsRequestContext, tfidsJson2);
        if (model.EditMembers)
          this.RemoveMembers(foundationIdentityArray2, anchorIdentity, model);
        else
          this.LeaveGroups(foundationIdentityArray2, anchorIdentity, model);
        this.RevaluateExtensionAssignments(foundationIdentityArray2);
      }
      catch (Exception ex)
      {
        model.GeneralErrors.Add(ex.Message);
        this.TraceException(599999, ex);
      }
      return model;
    }

    private void RevaluateExtensionAssignments(TeamFoundationIdentity[] identitiesToRemove)
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      foreach (TeamFoundationIdentity identity in identitiesToRemove)
        this.RevaluateSingleExtensionAssignment(identity);
    }

    private void RevaluateSingleExtensionAssignment(TeamFoundationIdentity identity)
    {
      try
      {
        License licenseForUser = this.TfsRequestContext.GetExtension<ILegacyLicensingHandler>().GetLicenseForUser(this.TfsRequestContext, identity.Descriptor);
        this.TfsRequestContext.GetService<IExtensionEntitlementService>().EvaluateExtensionAssignmentsOnAccessLevelChange(this.TfsRequestContext, IdentityUtil.Convert(identity), licenseForUser);
      }
      catch (Exception ex)
      {
        this.TraceException(500910, ex);
      }
    }

    private void AddMembers(
      TeamFoundationIdentity[] members,
      TeamFoundationIdentity group,
      MembershipModel model)
    {
      foreach (TeamFoundationIdentity member in members)
      {
        if (member != null)
          IdentityHelpers.AddMemberToGroup(this.TfsRequestContext, group, member, member, model);
      }
    }

    private void JoinGroups(
      TeamFoundationIdentity[] groups,
      TeamFoundationIdentity member,
      MembershipModel model)
    {
      foreach (TeamFoundationIdentity group in groups)
      {
        if (group != null)
          IdentityHelpers.AddMemberToGroup(this.TfsRequestContext, group, member, group, model);
      }
    }

    [HttpGet]
    [TfsTraceFilter(500641, 500650)]
    [AcceptNavigationLevels(NavigationContextLevels.ApplicationAll)]
    public ActionResult CanAddMemberToGroup(Guid groupId, bool isOrganizationLevel = false)
    {
      if (ApiIdentityController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      return (ActionResult) this.Json((object) new
      {
        canEdit = ApiIdentityController.HasManageGroupMembershipPermission(this.TfsRequestContext, this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, new Guid[1]
        {
          groupId
        })[0], true)
      }, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [TfsTraceFilter(500641, 500650)]
    [AcceptNavigationLevels(NavigationContextLevels.ApplicationAll)]
    public ActionResult IsGroupRuleBacked(Guid groupId)
    {
      bool flag = false;
      try
      {
        IGroupLicensingService service = this.TfsRequestContext.GetService<IGroupLicensingService>();
        TeamFoundationIdentity readIdentity = this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, new Guid[1]
        {
          groupId
        })[0];
        if (readIdentity.IsContainer)
          flag = service.GetGroupLicensingRule(this.TfsRequestContext, readIdentity.SubjectDescriptor) != (Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule) null;
      }
      catch
      {
      }
      return (ActionResult) this.Json((object) new
      {
        isGroupRuleBacked = flag
      }, JsonRequestBehavior.AllowGet);
    }

    private void RemoveMembers(
      TeamFoundationIdentity[] members,
      TeamFoundationIdentity group,
      MembershipModel model)
    {
      foreach (TeamFoundationIdentity member in members)
      {
        if (member != null)
          IdentityHelpers.RemoveMemberFromGroup(this.TfsRequestContext, group, member, member, model);
      }
    }

    private void LeaveGroups(
      TeamFoundationIdentity[] groups,
      TeamFoundationIdentity member,
      MembershipModel model)
    {
      foreach (TeamFoundationIdentity group in groups)
      {
        if (group != null)
          IdentityHelpers.RemoveMemberFromGroup(this.TfsRequestContext, group, member, group, model);
      }
    }

    [HttpPost]
    [TfsTraceFilter(500480, 500490)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult DeleteIdentity(Guid tfid, bool isOrganizationLevel = false)
    {
      if (ApiIdentityController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      TeamFoundationIdentityService service = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity readIdentity = service.ReadIdentities(this.TfsRequestContext, new Guid[1]
      {
        tfid
      }, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
      {
        TeamConstants.TeamPropertyName
      }, IdentityPropertyScope.Local)[0];
      if (readIdentity != null)
      {
        if (string.Equals(readIdentity.Descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
        {
          if (this.TfsWebContext.CurrentProjectUri != (Uri) null)
            this.ExecuteDeleteIdentityPolicy(this.TfsRequestContext, this.TfsWebContext.CurrentProjectGuid, new Func<Guid>(this.GetProjectScopeId), readIdentity);
          service.DeleteApplicationGroup(this.TfsRequestContext, readIdentity.Descriptor);
        }
        else
          service.DeleteUser(this.TfsRequestContext, readIdentity.Descriptor, true);
      }
      return (ActionResult) new EmptyResult();
    }

    internal void ExecuteDeleteIdentityPolicy(
      IVssRequestContext requestContext,
      Guid projectId,
      Func<Guid> projectScopeIdRetriever,
      TeamFoundationIdentity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Func<Guid>>(projectScopeIdRetriever, nameof (projectScopeIdRetriever));
      ArgumentUtility.CheckForNull<TeamFoundationIdentity>(identity, nameof (identity));
      if (this.IsDefaultTeam(requestContext, projectId, identity))
        throw new TeamFoundationServiceException(AdminResources.CannotDeleteDefaultTeam);
      if (ApiIdentityController.IsEndPointGroup(requestContext, projectScopeIdRetriever, identity))
      {
        this.Trace(500483, TraceLevel.Error, "ExecuteDeleteIdentityPolicy: Cannot remove endpoint groups");
        throw new TeamFoundationServiceException(AdminServerResources.CannotDeleteEndpointGroup);
      }
    }

    private bool IsDefaultTeam(
      IVssRequestContext requestContext,
      Guid projectId,
      TeamFoundationIdentity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<TeamFoundationIdentity>(identity, nameof (identity));
      Guid defaultTeamId = requestContext.GetService<ITeamService>().GetDefaultTeamId(requestContext, projectId);
      return identity.TeamFoundationId.Equals(defaultTeamId);
    }

    private static bool IsEndPointGroup(
      IVssRequestContext requestContext,
      Func<Guid> projectScopeIdRetriever,
      TeamFoundationIdentity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Func<Guid>>(projectScopeIdRetriever, nameof (projectScopeIdRetriever));
      ArgumentUtility.CheckForNull<TeamFoundationIdentity>(identity, nameof (identity));
      IdentityDescriptor descriptor = identity.Descriptor;
      if (descriptor != (IdentityDescriptor) null && string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
      {
        Guid scopeId = projectScopeIdRetriever();
        if (!scopeId.Equals(Guid.Empty))
          return IdentityHelpers.IsEndPointGroupDescriptor(requestContext, scopeId, descriptor);
      }
      return false;
    }

    private Guid GetProjectScopeId()
    {
      Guid projectScopeId = Guid.Empty;
      if (this.TfsWebContext.CurrentProjectUri != (Uri) null)
      {
        IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
        try
        {
          Guid projectId = ProjectInfo.GetProjectId(this.TfsWebContext.CurrentProjectUri.ToString());
          if (!projectId.Equals(Guid.Empty))
            projectScopeId = service.GetScope(this.TfsRequestContext.Elevate(), projectId).Id;
        }
        catch (Exception ex)
        {
          this.TraceException(500485, ex);
        }
      }
      return projectScopeId;
    }

    [HttpPost]
    [ValidateInput(false)]
    [TfsTraceFilter(500540, 500550)]
    [AcceptNavigationLevels(NavigationContextLevels.Project)]
    public ActionResult CreateTeam(
      string teamName,
      string teamDesc,
      string newUsersJson,
      string existingUsersJson,
      bool createArea,
      Guid? parentGroupGuid)
    {
      MembershipModel membershipModel = new MembershipModel();
      List<string> collection = new List<string>();
      IList<TeamFoundationIdentity> newUsersJson1 = IdentityManagementHelpers.ParseNewUsersJson((ITfsController) this, newUsersJson, membershipModel);
      IList<TeamFoundationIdentity> existingIdentities = IdentityManagementHelpers.ResolveIdentities((ITfsController) this, IdentityManagementHelpers.ParseTfidsJson(existingUsersJson));
      if (newUsersJson1.Count + existingIdentities.Count == 0)
        throw new InvalidOperationException(AdminResources.AdminRequired);
      try
      {
        TFCommonUtil.CheckGroupName(ref teamName);
        TFCommonUtil.CheckGroupDescription(ref teamDesc);
      }
      catch (Exception ex)
      {
        throw new HttpException(400, ex.Message);
      }
      ITeamService service1 = this.TfsRequestContext.GetService<ITeamService>();
      string projectUri = this.TfsWebContext.CurrentProjectUri.ToString();
      Guid currentProjectGuid = this.TfsWebContext.CurrentProjectGuid;
      WebApiTeam team;
      try
      {
        team = service1.CreateTeam(this.TfsRequestContext, projectUri, teamName, teamDesc);
      }
      catch (GroupCreationException ex)
      {
        this.TraceException(599999, (Exception) ex);
        TeamFoundationIdentity foundationIdentity = ((IEnumerable<TeamFoundationIdentity>) this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, IdentitySearchFactor.AccountName, teamName, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
        {
          TeamConstants.TeamPropertyName
        }, IdentityPropertyScope.Local)).FirstOrDefault<TeamFoundationIdentity>();
        throw new TeamAlreadyExistsException(foundationIdentity == null || !foundationIdentity.TryGetProperty(TeamConstants.TeamPropertyName, out object _) ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, AdminServerResources.GroupAlreadyExists, (object) teamName) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, AdminServerResources.TeamAlreadyExists, (object) teamName));
      }
      catch (Exception ex) when (ex is InvalidTeamNameException || ex is TeamLimitExceededException)
      {
        throw;
      }
      TeamFoundationIdentityService service2 = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
      try
      {
        TeamFoundationIdentity readIdentity1 = service2.ReadIdentities(this.TfsRequestContext, new Guid[1]
        {
          team.Id
        })[0];
        service2.EnsureIsMember(this.TfsRequestContext, readIdentity1.Descriptor, this.TfsRequestContext.UserContext);
        this.AddTeamAdminsInternal(readIdentity1.Descriptor, newUsersJson1, existingIdentities, membershipModel, false);
        if (membershipModel.HasErrors)
          collection.AddRange((IEnumerable<string>) membershipModel.GeneralErrors);
        if (parentGroupGuid.HasValue)
        {
          TeamFoundationIdentity readIdentity2 = service2.ReadIdentities(this.TfsRequestContext, new Guid[1]
          {
            parentGroupGuid.Value
          }, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null)[0];
          if (readIdentity2 != null)
          {
            this.Trace(15166000, TraceLevel.Info, string.Format("Adding user to as member of application group User: {0} Team: {1}", (object) readIdentity2.TeamFoundationId, (object) team.Id));
            service2.AddMemberToApplicationGroup(this.TfsRequestContext, readIdentity2.Descriptor, readIdentity1);
          }
        }
      }
      catch (TeamFoundationServerException ex)
      {
        this.TraceException(15166001, (Exception) ex);
        collection.Add(ex.Message);
      }
      catch (Exception ex)
      {
        this.TraceException(599999, ex);
      }
      try
      {
        this.Trace(15166002, TraceLevel.Info, "Setting default team settings.");
        TeamConfigurationHelper.SetDefaultSettings(this.TfsRequestContext, this.TfsWebContext.Project, service1.GetTeamInProject(this.TfsRequestContext, currentProjectGuid, team.Id.ToString()), createArea ? TeamAreaAction.CreateNew : TeamAreaAction.DoNothing);
      }
      catch (TeamFoundationServiceException ex)
      {
        this.TraceException(15166003, (Exception) ex);
        collection.Add(ex.Message);
      }
      IdentityViewModelBase identityViewModel = IdentityManagementHelpers.GetIdentityViewModel(service2.ReadIdentities(this.TfsRequestContext, new Guid[1]
      {
        team.Id
      }, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
      {
        TeamConstants.TeamPropertyName
      }, IdentityPropertyScope.Local)[0]);
      identityViewModel.Warnings.AddRange((IEnumerable<string>) collection);
      return (ActionResult) this.Json((object) identityViewModel.ToJson());
    }

    [HttpPost]
    [TfsTraceFilter(500580, 500590)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult RemoveTeamAdmin(Guid teamId, Guid tfidToRemove)
    {
      TeamFoundationIdentityService service1 = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity readIdentity = service1.ReadIdentities(this.TfsRequestContext, new Guid[1]
      {
        tfidToRemove
      })[0];
      if (readIdentity != null)
      {
        ITeamService service2 = this.TfsRequestContext.GetService<ITeamService>();
        WebApiTeam teamByGuid = service2.GetTeamByGuid(this.TfsRequestContext, teamId);
        if (teamByGuid == null)
          throw new InvalidOperationException(AdminServerResources.TeamNotFound);
        IReadOnlyCollection<Microsoft.VisualStudio.Services.Identity.Identity> teamAdmins = service2.GetTeamAdmins(this.TfsRequestContext, teamByGuid.Identity);
        if (teamAdmins.Count == 1 && teamAdmins.First<Microsoft.VisualStudio.Services.Identity.Identity>() != null && IdentityDescriptorComparer.Instance.Equals(teamAdmins.First<Microsoft.VisualStudio.Services.Identity.Identity>().Descriptor, readIdentity.Descriptor))
          throw new InvalidOperationException(AdminResources.AdminRequired);
        try
        {
          service1.RemoveGroupAdministrator(this.TfsRequestContext, teamByGuid.Identity.Descriptor, readIdentity.Descriptor);
        }
        catch (AccessCheckException ex)
        {
          throw new HttpException(403, string.Format(AdminServerResources.AccessDenied_RemoveTeamAdmin, (object) readIdentity.DisplayName));
        }
      }
      JsObject data = new JsObject();
      data["success"] = (object) true;
      return (ActionResult) this.Json((object) data);
    }

    [HttpGet]
    [TfsTraceFilter(500620, 500630)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult Display(Guid tfid, bool isOrganizationLevel = false)
    {
      if (ApiIdentityController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      TeamFoundationIdentity readIdentity = this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, new Guid[1]
      {
        tfid
      }, MembershipQuery.Direct, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
      {
        TeamConstants.TeamPropertyName
      }, IdentityPropertyScope.Local)[0];
      if (readIdentity == null)
        return this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.AadGroupsAdminUi") ? (ActionResult) new HttpStatusCodeResult(HttpStatusCode.NoContent) : (ActionResult) new EmptyResult();
      IdentityViewModelBase identityViewModel1 = IdentityImageUtility.GetIdentityViewModel(readIdentity);
      if (identityViewModel1 is GroupIdentityViewModel)
      {
        GroupIdentityViewModel identityViewModel2 = identityViewModel1 as GroupIdentityViewModel;
        if (identityViewModel2 is TeamIdentityViewModel)
          (identityViewModel2 as TeamIdentityViewModel).PopulateTeamAdmins(this.TfsWebContext);
        identityViewModel2.IsReadOnly = GroupHelpers.IsReadOnlyTfsGroup(this.TfsRequestContext, readIdentity);
      }
      TeamFoundationExecutionEnvironment executionEnvironment = this.TfsRequestContext.ExecutionEnvironment;
      if (executionEnvironment.IsOnPremisesDeployment)
      {
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
        TeamFoundationOnPremLicensingService service = vssRequestContext.GetService<TeamFoundationOnPremLicensingService>();
        identityViewModel1.LicenseNames = ((IEnumerable<ILicenseType>) service.GetLicensesForUser(vssRequestContext, readIdentity.Descriptor)).Select<ILicenseType, string>((Func<ILicenseType, string>) (x => x.Name));
      }
      Guid permissionSetId = AdminAreaController.GetPermissionSetId(this.NavigationContext, new Guid?());
      string permissionSetToken = AdminAreaController.GetDefaultPermissionSetToken(this.TfsRequestContext, this.TfsWebContext, permissionSetId);
      if (isOrganizationLevel)
      {
        executionEnvironment = this.TfsRequestContext.ExecutionEnvironment;
        if (executionEnvironment.IsHostedDeployment && this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Application))
          permissionSetId = NamespacePermissionSetConstants.OrganizationLevel;
      }
      SecurityNamespacePermissionsManager permissionsManager = this.GetSecurityNamespacePermissionsManager(this.TfsRequestContext, permissionSetId, permissionSetToken, new Guid?(tfid));
      PermissionsModel permissionsModel = new PermissionsModel(this.TfsRequestContext, identityViewModel1.Descriptor, permissionsManager);
      return (ActionResult) this.Json((object) new
      {
        identity = identityViewModel1.ToJson(),
        security = permissionsModel.ToJson()
      }, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(500651, 500660)]
    public ActionResult SearchIdentities(string searchFactor, string membershipType = "")
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(searchFactor, nameof (searchFactor));
      TeamFoundationIdentityService service = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
      int pageSize = IdentityManagementHelpers.GetPageSize(new int?());
      List<IdentityFilter> identityFilterList = new List<IdentityFilter>();
      identityFilterList.Add((IdentityFilter) new IdentityMruFilter(searchFactor));
      switch (membershipType)
      {
        case "groups":
          identityFilterList.Add((IdentityFilter) new MembershipTypeIdentityFilter(MembershipTypeIdentityFilter.MembershipType.Groups));
          break;
        case "users":
          identityFilterList.Add((IdentityFilter) new MembershipTypeIdentityFilter(MembershipTypeIdentityFilter.MembershipType.Users));
          break;
      }
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      int suggestedPageSize = pageSize;
      List<IdentityFilter> filters = identityFilterList;
      IEnumerable<IdentityRef> source = ((IEnumerable<TeamFoundationIdentity>) service.ReadFilteredIdentities(tfsRequestContext, (string) null, suggestedPageSize, (IEnumerable<IdentityFilter>) filters, (string) null, true, MembershipQuery.None, (IEnumerable<string>) null, IdentityPropertyScope.None).Items).Select<TeamFoundationIdentity, IdentityRef>((Func<TeamFoundationIdentity, IdentityRef>) (identity => new IdentityRef()
      {
        Id = identity.TeamFoundationId.ToString(),
        DisplayName = identity.DisplayName,
        UniqueName = identity.UniqueName,
        IsContainer = identity.IsContainer
      }));
      SecureJsonResult secureJsonResult = new SecureJsonResult();
      secureJsonResult.Data = (object) source.Select<IdentityRef, JsObject>((Func<IdentityRef, JsObject>) (identity => identity.ToJson()));
      secureJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) secureJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(500851, 500860)]
    public ActionResult SearchAadIdentities(string searchTerm, SearchIdentityType identityType = SearchIdentityType.All)
    {
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service1 = this.TfsRequestContext.GetService<IVssRegistryService>();
      int num1 = service1.GetValue<int>(this.TfsRequestContext, (RegistryQuery) "/Service/AAD/Settings/AadSearchMaxSize", false, 50);
      double num2 = service1.GetValue<double>(this.TfsRequestContext, (RegistryQuery) "/Service/AAD/Settings/AadSearchUsersToGroupRatio", false, 0.8);
      AadService service2 = tfsRequestContext.GetService<AadService>();
      List<IdentityRef> source1 = new List<IdentityRef>();
      if (identityType == SearchIdentityType.All || identityType == SearchIdentityType.Users)
      {
        AadService aadService = service2;
        IVssRequestContext context = tfsRequestContext;
        GetUsersRequest getUsersRequest = new GetUsersRequest();
        getUsersRequest.DisplayNamePrefixes = (IEnumerable<string>) new string[1]
        {
          searchTerm
        };
        getUsersRequest.SurnamePrefixes = (IEnumerable<string>) new string[1]
        {
          searchTerm
        };
        getUsersRequest.MailPrefixes = (IEnumerable<string>) new string[1]
        {
          searchTerm
        };
        getUsersRequest.MailNicknamePrefixes = (IEnumerable<string>) new string[1]
        {
          searchTerm
        };
        getUsersRequest.UserPrincipalNamePrefixes = (IEnumerable<string>) new string[1]
        {
          searchTerm
        };
        getUsersRequest.MaxResults = new int?((int) ((double) num1 * num2));
        GetUsersRequest request = getUsersRequest;
        IEnumerable<AadUser> source2 = aadService.GetUsers(context, request).Users.Where<AadUser>((Func<AadUser, bool>) (user => !string.IsNullOrWhiteSpace(user.SignInAddress)));
        source1.AddRange(source2.Select<AadUser, IdentityRef>((Func<AadUser, IdentityRef>) (user => new IdentityRef()
        {
          Id = user.ObjectId.ToString(),
          DisplayName = user.DisplayName,
          UniqueName = user.SignInAddress,
          IsContainer = false,
          IsAadIdentity = true
        })));
      }
      if (identityType == SearchIdentityType.All || identityType == SearchIdentityType.Groups)
      {
        AadService aadService = service2;
        IVssRequestContext context = tfsRequestContext;
        GetGroupsRequest getGroupsRequest = new GetGroupsRequest();
        getGroupsRequest.DisplayNamePrefixes = (IEnumerable<string>) new string[1]
        {
          searchTerm
        };
        getGroupsRequest.MailNicknamePrefixes = (IEnumerable<string>) new string[1]
        {
          searchTerm
        };
        getGroupsRequest.MaxResults = new int?(num1 - source1.Count);
        GetGroupsRequest request = getGroupsRequest;
        IEnumerable<AadGroup> groups = aadService.GetGroups(context, request).Groups;
        source1.AddRange(groups.Select<AadGroup, IdentityRef>((Func<AadGroup, IdentityRef>) (group => new IdentityRef()
        {
          Id = group.ObjectId.ToString(),
          DisplayName = group.DisplayName,
          UniqueName = group.MailNickname,
          IsContainer = true,
          IsAadIdentity = true
        })));
      }
      SecureJsonResult secureJsonResult = new SecureJsonResult();
      secureJsonResult.Data = (object) source1.Select<IdentityRef, JsObject>((Func<IdentityRef, JsObject>) (identity => identity.ToJson()));
      secureJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) secureJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "Default", Duration = 0)]
    [TfsTraceFilter(500951, 500960)]
    public ActionResult GetAadUserThumbnail(Guid userObjectId)
    {
      try
      {
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        AadService service = tfsRequestContext.GetService<AadService>();
        string empty = string.Empty;
        IVssRequestContext context = tfsRequestContext;
        GetUserThumbnailResponse userThumbnail = service.GetUserThumbnail<Guid>(context, new GetUserThumbnailRequest<Guid>()
        {
          Identifier = userObjectId
        });
        return userThumbnail.Thumbnail != null && userThumbnail.Thumbnail.Length != 0 ? (ActionResult) this.File(userThumbnail.Thumbnail, "image/jpeg") : (ActionResult) this.File(StaticResources.Versioned.Content.GetPhysicalLocation("User.svg"), "image/svg+xml");
      }
      catch (Exception ex)
      {
        return (ActionResult) this.File(StaticResources.Versioned.Content.GetPhysicalLocation("User.svg"), "image/svg+xml");
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(500651, 500660)]
    public ActionResult CheckNames(string[] searchFactors)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) searchFactors, nameof (searchFactors));
      List<TeamFoundationIdentity> source1 = new List<TeamFoundationIdentity>();
      foreach (IEnumerable<TeamFoundationIdentity> readIdentity in this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, IdentitySearchFactor.General, searchFactors))
        source1.AddRange(readIdentity.AsEnumerable<TeamFoundationIdentity>().Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (identity => identity != null)));
      IEnumerable<IdentityRef> source2 = source1.Distinct<TeamFoundationIdentity>((IEqualityComparer<TeamFoundationIdentity>) new TeamFoundationIdentityDisplayNameComparer()).Select<TeamFoundationIdentity, IdentityRef>((Func<TeamFoundationIdentity, IdentityRef>) (identity => new IdentityRef()
      {
        Id = identity.TeamFoundationId.ToString(),
        DisplayName = identity.DisplayName,
        UniqueName = identity.UniqueName,
        IsContainer = identity.IsContainer
      }));
      SecureJsonResult secureJsonResult = new SecureJsonResult();
      secureJsonResult.Data = (object) source2.Select<IdentityRef, JsObject>((Func<IdentityRef, JsObject>) (identity => identity.ToJson()));
      secureJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) secureJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "Default", Duration = 3600)]
    [TfsTraceFilter(500951, 500960)]
    public ActionResult IsGuestAadUser(string userEmail)
    {
      bool flag = true;
      try
      {
        AadService service = this.TfsRequestContext.GetService<AadService>();
        List<string> stringList = new List<string>()
        {
          userEmail
        };
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        GetUsersRequest request = new GetUsersRequest();
        request.MailPrefixes = (IEnumerable<string>) stringList;
        request.UserPrincipalNamePrefixes = (IEnumerable<string>) stringList;
        request.PagingToken = (string) null;
        request.MaxResults = new int?(1);
        GetUsersResponse users = service.GetUsers(tfsRequestContext, request);
        if (users != null && users.Users != null && users.Users.Count<AadUser>() > 0)
        {
          flag = false;
          AadUser aadUser = users.Users.FirstOrDefault<AadUser>();
          if (!string.IsNullOrEmpty(aadUser.UserType) && aadUser.UserType.Trim().ToLower().Equals("guest"))
            flag = true;
        }
        return (ActionResult) this.Json((object) new
        {
          isGuest = flag
        }, JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
        this.TraceException(599999, ex);
        return (ActionResult) this.Json((object) new
        {
          isGuest = flag
        }, JsonRequestBehavior.AllowGet);
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

    private static bool ShouldElevateToOrganization(
      bool isOrganizationLevel,
      IVssRequestContext requestContext)
    {
      return isOrganizationLevel && requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("VisualStudio.Services.Web.OrgAdmin.UserExperience");
    }

    private static bool HasManageGroupMembershipPermission(
      IVssRequestContext requestContext,
      TeamFoundationIdentity groupIdentity,
      bool alwaysAllowAdministrators = false)
    {
      if (groupIdentity == null)
        return false;
      return requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Application) ? requestContext.GetClient<SecurityHttpClient>(ServiceInstanceTypes.SPS).HasPermissionAsync(FrameworkSecurity.IdentitiesNamespaceId, IdentityUtil.CreateSecurityToken(groupIdentity), 8, alwaysAllowAdministrators).Result : GroupHelpers.HasManageGroupMembershipPermission(requestContext, groupIdentity, alwaysAllowAdministrators);
    }
  }
}
