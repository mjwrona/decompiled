// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.ApiUserController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  [SupportedRouteArea("Api", NavigationContextLevels.Application | NavigationContextLevels.Collection)]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiUserController : AccountAreaController
  {
    private const string s_area = "HostingAccount";
    private const string s_layer = "ApiUserController";
    private const string Featureflag = "WebAccess.UserManagement";
    private const string s_MeteringServiceBaseRegistryKey = "/Service/Commerce/Metering";
    private const string s_earlyAdopterCutOffDateKey = "/Service/Commerce/Metering/EarlyAdopterSelectDate";
    private const string s_earlyAdopterDurationKey = "/Service/Commerce/Metering/EarlyAdopterIncentiveDuration";
    internal const string s_featureNameAddUserInNewAccountUserTable = "VisualStudio.AccountService.AddUserInNewAccountUserTable";
    private const string BillingPeriodWarningDateFormatString = "MMMM d";

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(504101, 504110)]
    [TfsHandleFeatureFlag("WebAccess.UserManagement", null)]
    public ActionResult GetAccountUsers()
    {
      try
      {
        this.TfsRequestContext.Trace(504101, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserController), "GetAccountUsers: Starting");
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
        Dictionary<string, int> licenseCount = new Dictionary<string, int>();
        Dictionary<Guid, AccountEntitlement> dictionary = new Dictionary<Guid, AccountEntitlement>();
        this.TfsRequestContext.Trace(504102, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserController), "GetAccountEntitlements: Reading user identities from account");
        IList<AccountEntitlement> accountEntitlements = this.TfsRequestContext.GetService<ILicensingEntitlementService>().GetAccountEntitlements(this.TfsRequestContext);
        if (accountEntitlements.Count > 0)
        {
          List<Guid> identityIds = new List<Guid>();
          foreach (AccountEntitlement accountEntitlement in (IEnumerable<AccountEntitlement>) accountEntitlements)
          {
            identityIds.Add(accountEntitlement.UserId);
            dictionary.Add(accountEntitlement.UserId, accountEntitlement);
          }
          this.TfsRequestContext.Trace(504103, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserController), "GetAccountUsers: reading user identities from identity service");
          IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate().To(TeamFoundationHostType.Deployment);
          identityList = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<Guid>) identityIds, QueryMembership.None, (IEnumerable<string>) new string[1]
          {
            "CustomNotificationAddresses"
          });
        }
        int daysLeft = 0;
        bool flag = !this.AllowEarlyAdopter(out daysLeft) && !this.TfsRequestContext.IsFeatureEnabled("WebAccess.AllowEarlyAdopterAfterExpiration");
        UserViewModel userViewModel = new UserViewModel();
        userViewModel.Users = new List<UserModel>();
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList)
        {
          AccountEntitlement accountEntitlement = (AccountEntitlement) null;
          if (!ServicePrincipals.IsServicePrincipal(this.TfsRequestContext, identity.Descriptor) && !IdentityHelper.IsAnonymousPrincipal(identity.Descriptor) && identity != null && dictionary.TryGetValue(identity.Id, out accountEntitlement))
          {
            UserModel userModel = new UserModel();
            userModel.Name = identity.DisplayName.Trim();
            userModel.UserId = identity.Id.ToString();
            if (flag && accountEntitlement.License == (License) AccountLicense.EarlyAdopter)
            {
              userModel.Status = AccountServerResources.UserStatusExpired;
              accountEntitlement.UserStatus = AccountUserStatus.Disabled;
            }
            else
              userModel.Status = ApiUserController.GetStatus((int) accountEntitlement.UserStatus, accountEntitlement.License);
            userModel.LicenseType = ApiUserController.GetLicenseType(accountEntitlement.License, accountEntitlement.UserStatus, licenseCount);
            userModel.isMsdn = accountEntitlement.License.Source.Equals((object) LicensingSource.Msdn);
            object displayName;
            if (identity.TryGetProperty("Mail", out displayName))
            {
              if (displayName.Equals((object) string.Empty))
                displayName = (object) identity.DisplayName;
              userModel.SignInAddress = displayName.ToString();
            }
            userViewModel.Users.Add(userModel);
          }
        }
        userViewModel.LicenseOverview = licenseCount;
        userViewModel.Users = userViewModel.Users.OrderBy<UserModel, string>((Func<UserModel, string>) (x => x.Name)).ToList<UserModel>();
        userViewModel.Licenses = this.GetLicenseData(userViewModel);
        JsonResult accountUsers = this.Json((object) userViewModel, JsonRequestBehavior.AllowGet);
        accountUsers.MaxJsonLength = new int?(33554432);
        return (ActionResult) accountUsers;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(504618, "HostingAccount", nameof (ApiUserController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.Trace(504641, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserController), "GetAccountUsers: completed.");
      }
    }

    [HttpPost]
    [TfsTraceFilter(508111, 508120)]
    [TfsHandleFeatureFlag("WebAccess.UserManagement", null)]
    public ActionResult AddUser(string userName, string licenseType)
    {
      try
      {
        this.TfsRequestContext.Trace(508112, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserController), "AddUser: Starting");
        if (string.IsNullOrEmpty(userName))
          throw new ArgumentException(nameof (userName));
        if (string.IsNullOrEmpty(licenseType))
          throw new ArgumentException("license");
        JsObject data = new JsObject();
        if (!ArgumentUtility.IsValidEmailAddress(userName))
        {
          data["error"] = (object) AccountServerResources.UserHubInvalidEmail;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        this.TfsRequestContext.Trace(508113, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserController), "AddUser: Check is user is admin");
        if (!this.HasPermissions())
        {
          data["error"] = (object) AccountServerResources.UserPermissionsError;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        ILicensingEntitlementService service = this.TfsRequestContext.GetService<ILicensingEntitlementService>();
        IList<AccountEntitlement> accountEntitlements = service.GetAccountEntitlements(this.TfsRequestContext);
        IList<AccountLicenseUsage> licensesUsage = service.GetLicensesUsage(this.TfsRequestContext);
        License license = ApiUserController.ParseLicense(licenseType);
        string errorMessage;
        if (!this.CheckLicenseAvailiability(license, (IEnumerable<AccountLicenseUsage>) licensesUsage, Guid.Empty, out errorMessage))
        {
          data["error"] = (object) errorMessage;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        string domain = "Windows Live ID";
        Guid organizationAadTenantId = this.TfsRequestContext.GetOrganizationAadTenantId();
        if (!organizationAadTenantId.Equals(Guid.Empty))
          domain = organizationAadTenantId.ToString();
        Microsoft.VisualStudio.Services.Identity.Identity identity;
        try
        {
          identity = this.CheckName(userName, domain);
          if (accountEntitlements.Where<AccountEntitlement>((Func<AccountEntitlement, bool>) (user => user.UserId.Equals(identity.Id))).Any<AccountEntitlement>())
            throw new IdentityAlreadyExistsException(string.Format(AccountServerResources.IdentityAlreadyExists, (object) userName));
        }
        catch (IdentityAlreadyExistsException ex)
        {
          data["error"] = (object) ex.Message;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        this.TfsRequestContext.Trace(508116, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserController), "AddUser: Add User to Account");
        service.AssignAccountEntitlement(this.TfsRequestContext, identity.Id, license);
        data["aad"] = (object) false;
        if (identity.IsExternalUser)
        {
          data["aad"] = (object) true;
          if (this.TfsRequestContext.Elevate().To(TeamFoundationHostType.Deployment).IsFeatureEnabled("WebAccess.EnableAddAadUserWarning"))
          {
            try
            {
              AadTenant tenant = this.TfsRequestContext.GetService<AadService>().GetTenant(this.TfsRequestContext, new GetTenantRequest()).Tenant;
              data["aadTenantName"] = (object) tenant.DisplayName;
            }
            catch (AadException ex)
            {
              this.TfsRequestContext.TraceException(508116, "HostingAccount", nameof (ApiUserController), (Exception) ex);
            }
            catch (Exception ex)
            {
              this.TfsRequestContext.Trace(508117, TraceLevel.Error, "HostingAccount", nameof (ApiUserController), "something realy went wrong, AadService should not throw the Exception:{0}", (object) ex);
            }
          }
        }
        return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(508118, "HostingAccount", nameof (ApiUserController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.Trace(508119, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserController), "AddUser: completed.");
      }
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(508141, 508150)]
    [TfsHandleFeatureFlag("WebAccess.UserManagement", null)]
    public ActionResult RefreshUserRights()
    {
      try
      {
        IVssRequestContext requestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
        string[] strArray = LicenseClaimCacheKeysGenerator.Generate(requestContext.ServiceHost.InstanceId, requestContext.GetUserId());
        if (strArray == null)
          return (ActionResult) this.Json((object) "success", JsonRequestBehavior.AllowGet);
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        foreach (string str in strArray)
        {
          string cacheKey = str;
          requestContext.TraceConditionally(508142, TraceLevel.Info, "HostingAccount", nameof (ApiUserController), (Func<string>) (() => "Removing cache entry for key: " + cacheKey));
          vssRequestContext.GetService<ILicenseClaimCacheService>().Remove(vssRequestContext, cacheKey);
        }
        EntitlementChangeMessage message = new EntitlementChangeMessage()
        {
          EntitlementChangeType = EntitlementChangeType.AccountEntitlement,
          AccountId = requestContext.ServiceHost.InstanceId,
          UserIds = new Guid[1]
          {
            requestContext.GetUserId()
          }
        };
        EntitlementChangeNotifier.Publish(requestContext, message, EntitlementChangePublisherType.SqlNotification);
        return (ActionResult) this.Json((object) "success", JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(508148, "HostingAccount", nameof (ApiUserController), ex);
        throw;
      }
    }

    [HttpPost]
    [TfsTraceFilter(508121, 508130)]
    [TfsHandleFeatureFlag("WebAccess.UserManagement", null)]
    public ActionResult EditUser(string userId, string licenseType)
    {
      try
      {
        this.TfsRequestContext.Trace(508122, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserController), "EditUser: Starting");
        if (string.IsNullOrEmpty(userId))
          throw new ArgumentException(nameof (userId));
        Guid userGuid;
        if (!Guid.TryParse(userId, out userGuid))
          throw new ArgumentException(nameof (userId));
        if (userGuid.Equals(Guid.Empty))
          throw new ArgumentException(nameof (userId));
        if (string.IsNullOrEmpty(licenseType))
          throw new ArgumentException("license");
        JsObject data = new JsObject();
        License license = ApiUserController.ParseLicense(licenseType);
        this.TfsRequestContext.Trace(508123, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserController), "EditUser: Validate Request Permissions");
        if (!this.HasPermissions())
        {
          data["error"] = (object) AccountServerResources.UserPermissionsError;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        ILicensingEntitlementService service = this.TfsRequestContext.GetService<ILicensingEntitlementService>();
        IList<AccountLicenseUsage> licensesUsage = service.GetLicensesUsage(this.TfsRequestContext);
        string errorMessage;
        if (!this.CheckLicenseAvailiability(license, (IEnumerable<AccountLicenseUsage>) licensesUsage, userGuid, out errorMessage))
        {
          data["error"] = (object) errorMessage;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        this.TfsRequestContext.Trace(508124, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserController), "EditUser: Verify User is in account");
        if (!ApiUserController.ValidateLicenseAssignment(service.GetAccountEntitlements(this.TfsRequestContext).Where<AccountEntitlement>((Func<AccountEntitlement, bool>) (user => user.UserId.Equals(userGuid))).First<AccountEntitlement>().License, license))
        {
          data["error"] = (object) AccountServerResources.NoMSDNToEligible;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        this.TfsRequestContext.Trace(508125, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserController), "EditUser: UpdateUser in Account");
        service.AssignAccountEntitlement(this.TfsRequestContext, userGuid, license);
        return (ActionResult) this.Json((object) "success", JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(508128, "HostingAccount", nameof (ApiUserController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.Trace(508129, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserController), "AddUser: completed.");
      }
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(508101, 508110)]
    [TfsHandleFeatureFlag("WebAccess.UserManagement", null)]
    public ActionResult RemoveUser(string userId)
    {
      try
      {
        this.TfsRequestContext.Trace(508102, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserController), "RemoveUser: Starting");
        this.TfsRequestContext.Elevate().To(TeamFoundationHostType.Deployment);
        Collection collection = this.TfsRequestContext.GetService<ICollectionService>().GetCollection(this.TfsRequestContext.Elevate(), (IEnumerable<string>) null);
        Guid userGuid;
        if (!Guid.TryParse(userId, out userGuid))
          throw new ArgumentException(nameof (userId));
        if (userGuid.Equals(collection.Owner))
          return (ActionResult) this.Json((object) "failure", JsonRequestBehavior.AllowGet);
        this.CreateCollectionContextAndExecute((Action<IVssRequestContext>) (requestContext =>
        {
          TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
          TeamFoundationIdentity readIdentity = service.ReadIdentities(requestContext, new Guid[1]
          {
            userGuid
          }, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null, IdentityPropertyScope.Local)[0];
          service.DeleteUser(requestContext, readIdentity.Descriptor, false);
        }));
        return (ActionResult) this.Json((object) "success", JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(508108, "HostingAccount", nameof (ApiUserController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.Trace(508109, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserController), "RemoveUser: completed.");
      }
    }

    private static string GetLicenseType(
      License license,
      AccountUserStatus userStatus,
      Dictionary<string, int> licenseCount)
    {
      Dictionary<License, string> stringDict = ApiUserController.LicenseToStringDict();
      string licenseType;
      int num1;
      switch (license.Source)
      {
        case LicensingSource.Account:
          licenseType = stringDict[license];
          AccountLicense accountLicense = (AccountLicense) license;
          if (userStatus.Equals((object) AccountUserStatus.Disabled) || userStatus.Equals((object) AccountUserStatus.PendingDisabled))
          {
            Dictionary<string, int> dictionary = licenseCount;
            int num2;
            int num3;
            if (!licenseCount.TryGetValue("Inactive", out num2))
              num3 = 1;
            else
              num2 = num3 = num2 + 1;
            dictionary["Inactive"] = num3;
            break;
          }
          if (accountLicense.License.Equals((object) AccountLicenseType.EarlyAdopter))
          {
            Dictionary<string, int> dictionary = licenseCount;
            string key = accountLicense.ToString();
            int num4;
            int num5;
            if (!licenseCount.TryGetValue(accountLicense.ToString(), out num4))
              num5 = 1;
            else
              num1 = num5 = num4 + 1;
            dictionary[key] = num5;
            break;
          }
          break;
        case LicensingSource.Msdn:
          licenseType = stringDict[license];
          MsdnLicense msdnLicense = (MsdnLicense) license;
          if (userStatus.Equals((object) AccountUserStatus.Disabled) || userStatus.Equals((object) AccountUserStatus.PendingDisabled))
          {
            Dictionary<string, int> dictionary = licenseCount;
            int num6;
            int num7;
            if (!licenseCount.TryGetValue("Inactive", out num6))
              num7 = 1;
            else
              num6 = num7 = num6 + 1;
            dictionary["Inactive"] = num7;
            break;
          }
          Dictionary<string, int> dictionary1 = licenseCount;
          string key1 = msdnLicense.ToString();
          int num8;
          int num9;
          if (!licenseCount.TryGetValue(msdnLicense.ToString(), out num8))
            num9 = 1;
          else
            num1 = num9 = num8 + 1;
          dictionary1[key1] = num9;
          break;
        default:
          licenseType = license.ToString();
          break;
      }
      return licenseType;
    }

    private static License ParseLicense(string license)
    {
      License license1;
      License.TryParse(license, true, out license1);
      return license1;
    }

    private static string GetStatus(int enumValue, License license)
    {
      string status = string.Empty;
      switch (enumValue)
      {
        case 0:
          status = string.Empty;
          break;
        case 1:
          status = !license.Source.Equals((object) LicensingSource.Msdn) || !((MsdnLicense) license).License.Equals((object) MsdnLicenseType.Eligible) ? string.Empty : AccountServerResources.UserStatusPending;
          break;
        case 2:
          status = !license.Source.Equals((object) LicensingSource.Msdn) ? AccountServerResources.UserStatusExpired : (!((MsdnLicense) license).License.Equals((object) MsdnLicenseType.Eligible) ? AccountServerResources.UserStatusMSDNExpired : AccountServerResources.DisabledEligibleMSDN);
          break;
        case 3:
          status = string.Empty;
          break;
        case 4:
          status = !license.Source.Equals((object) LicensingSource.Msdn) || !((MsdnLicense) license).License.Equals((object) MsdnLicenseType.Eligible) ? string.Empty : AccountServerResources.UserStatusPending;
          break;
        case 5:
          status = !license.Source.Equals((object) LicensingSource.Msdn) ? AccountServerResources.UserStatusExpired : (!((MsdnLicense) license).License.Equals((object) MsdnLicenseType.Eligible) ? AccountServerResources.UserStatusMSDNExpired : AccountServerResources.DisabledEligibleMSDN);
          break;
        case 6:
          status = !license.Source.Equals((object) LicensingSource.Msdn) || !((MsdnLicense) license).License.Equals((object) MsdnLicenseType.Eligible) ? AccountServerResources.UserStatusExpired : AccountServerResources.DisabledEligibleMSDN;
          break;
      }
      return status;
    }

    private bool HasPermissions() => this.IsAdmin() || this.IsProjectCollectionAdmin();

    private bool IsAdmin()
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      return vssRequestContext.GetService<IdentityService>().IsMember(vssRequestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, this.TfsRequestContext.UserContext);
    }

    private bool IsProjectCollectionAdmin()
    {
      bool isCollectionAdmin = false;
      this.CreateCollectionContextAndExecute((Action<IVssRequestContext>) (requestContext => isCollectionAdmin = requestContext.GetService<TeamFoundationIdentityService>().IsMember(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, requestContext.UserContext)));
      return isCollectionAdmin;
    }

    private List<LicenseModel> GetLicenseData(UserViewModel vm)
    {
      List<LicenseModel> licenseData = new List<LicenseModel>();
      List<LicenseModel> source = new List<LicenseModel>();
      Dictionary<string, LicenseModel> dictionary = new Dictionary<string, LicenseModel>();
      IEnumerable<ISubscriptionResource> resourceStatus = this.TfsRequestContext.GetService<IMeteringService>().GetResourceStatus(this.TfsRequestContext, true);
      List<string> stringList = new List<string>();
      ISubscriptionResource subscriptionResource1 = resourceStatus.Where<ISubscriptionResource>((Func<ISubscriptionResource, bool>) (resource => resource.Name.Equals((object) ResourceName.StandardLicense))).First<ISubscriptionResource>();
      ISubscriptionResource subscriptionResource2 = resourceStatus.Where<ISubscriptionResource>((Func<ISubscriptionResource, bool>) (resource => resource.Name.Equals((object) ResourceName.ProfessionalLicense))).First<ISubscriptionResource>();
      ISubscriptionResource subscriptionResource3 = resourceStatus.Where<ISubscriptionResource>((Func<ISubscriptionResource, bool>) (resource => resource.Name.Equals((object) ResourceName.AdvancedLicense))).First<ISubscriptionResource>();
      bool flag = false;
      foreach (AccountLicenseUsage accountLicenseUsage in (IEnumerable<AccountLicenseUsage>) this.TfsRequestContext.GetService<ILicensingEntitlementService>().GetLicensesUsage(this.TfsRequestContext))
      {
        if (accountLicenseUsage.License.Source.Equals((object) LicensingSource.Account))
        {
          LicenseModel licenseModel = new LicenseModel();
          if (accountLicenseUsage.ProvisionedCount == -1)
          {
            licenseModel.Available = int.MaxValue;
            licenseModel.Maximum = 0;
          }
          else
          {
            licenseModel.Available = accountLicenseUsage.ProvisionedCount - accountLicenseUsage.UsedCount;
            licenseModel.Maximum = accountLicenseUsage.ProvisionedCount;
          }
          licenseModel.InUse = accountLicenseUsage.UsedCount;
          string licenseEnum;
          licenseModel.LicenseType = ApiUserController.TransformText(accountLicenseUsage.License.License, out licenseEnum);
          licenseModel.LicenseEnum = licenseEnum;
          dictionary.Add(licenseEnum, licenseModel);
          source.Add(licenseModel);
          switch (accountLicenseUsage.License.License)
          {
            case 2:
              licenseModel.IncludedQuantity = subscriptionResource1.IncludedQuantity;
              if (accountLicenseUsage.UsedCount > subscriptionResource1.CommittedQuantity)
              {
                flag = true;
                continue;
              }
              continue;
            case 3:
              licenseModel.IncludedQuantity = subscriptionResource2.IncludedQuantity;
              if (accountLicenseUsage.UsedCount > subscriptionResource2.CommittedQuantity)
              {
                flag = true;
                continue;
              }
              continue;
            case 4:
              licenseModel.IncludedQuantity = subscriptionResource3.IncludedQuantity;
              if (accountLicenseUsage.UsedCount > subscriptionResource3.CommittedQuantity)
              {
                flag = true;
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      List<LicenseModel> list = source.ToList<LicenseModel>();
      licenseData.AddRange((IEnumerable<LicenseModel>) list);
      int daysLeft = 0;
      if (this.AllowEarlyAdopter(out daysLeft))
        licenseData.Add(new LicenseModel()
        {
          LicenseType = AccountServerResources.AccountEarlyAdopterLicense,
          Available = int.MaxValue,
          LicenseEnum = AccountLicense.EarlyAdopter.ToString()
        });
      licenseData.Add(new LicenseModel()
      {
        LicenseType = AccountServerResources.AccountEligibleMSDNLicense,
        Available = int.MaxValue,
        LicenseEnum = MsdnLicense.Eligible.ToString()
      });
      if (flag)
      {
        stringList.Add(string.Format(AccountServerResources.BillingPeriodWarning, (object) subscriptionResource1.ResetDate.ToString("MMMM d"), (object) subscriptionResource1.CommittedQuantity, (object) subscriptionResource2.CommittedQuantity, (object) subscriptionResource3.CommittedQuantity));
        vm.LicenseErrors = stringList;
      }
      if (this.TfsRequestContext.IsFeatureEnabled("WebAccess.EarlyAdopterNotification") && vm.LicenseOverview.ContainsKey(AccountLicense.EarlyAdopter.ToString()))
      {
        if (daysLeft > 1 && daysLeft != int.MaxValue)
        {
          stringList.Add(string.Format(AccountServerResources.EarlyAdopterMessage, (object) daysLeft));
          vm.LicenseErrors = stringList;
        }
        else
        {
          switch (daysLeft)
          {
            case 0:
              stringList.Add(string.Format(AccountServerResources.EarlyAdopterMessageToday));
              vm.LicenseErrors = stringList;
              break;
            case 1:
              stringList.Add(string.Format(AccountServerResources.EarlyAdopterMessageSingleDay));
              vm.LicenseErrors = stringList;
              break;
          }
        }
      }
      vm.licenseDictionary = dictionary;
      return licenseData;
    }

    private bool AllowEarlyAdopter(out int daysLeft)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate().To(TeamFoundationHostType.Deployment);
      CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
      DateTime dateTime = service.GetValue<DateTime>(vssRequestContext, (RegistryQuery) "/Service/Commerce/Metering/EarlyAdopterSelectDate", DateTime.UtcNow);
      TimeSpan timeSpan = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Service/Commerce/Metering/EarlyAdopterIncentiveDuration", new TimeSpan(142, 8, 0, 0));
      Policy<bool> policy = this.TfsRequestContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(this.TfsRequestContext.Elevate(), "Policy.IsInternal", false);
      Collection collection = this.TfsRequestContext.GetService<ICollectionService>().GetCollection(this.TfsRequestContext.Elevate(), (IEnumerable<string>) null);
      daysLeft = -1;
      bool effectiveValue = policy.EffectiveValue;
      if (((!(collection.DateCreated.ToUniversalTime() < dateTime) ? 0 : (DateTime.UtcNow < dateTime.Add(timeSpan) ? 1 : 0)) | (effectiveValue ? 1 : 0)) == 0)
        return false;
      daysLeft = (dateTime.Add(timeSpan) - DateTime.UtcNow).Days;
      if (effectiveValue)
        daysLeft = int.MaxValue;
      return true;
    }

    public static bool IsInternal(Microsoft.VisualStudio.Services.Account.Account account)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Account.Account>(account, nameof (account));
      bool result = false;
      object obj;
      return account.Properties != null && account.TryGetProperty(AccountPropertyConstants.Internal, out obj) && bool.TryParse(obj.ToString(), out result) && result;
    }

    private static bool ValidateLicenseAssignment(License currentLicense, License newLicense)
    {
      switch (currentLicense.Source)
      {
        case LicensingSource.Account:
          return true;
        case LicensingSource.Msdn:
          return !newLicense.Source.Equals((object) LicensingSource.Msdn);
        default:
          return true;
      }
    }

    [HttpGet]
    [TfsTraceFilter(500021, 500030)]
    [ValidateInput(false)]
    public Microsoft.VisualStudio.Services.Identity.Identity CheckName(string name, string domain)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (!string.IsNullOrEmpty(name))
      {
        if (name.Contains(":"))
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
      try
      {
        if (ArgumentUtility.IsValidEmailAddress(name))
          identity = this.ResolveNewUser(name, domain);
      }
      catch (Microsoft.TeamFoundation.Server.Core.MultipleIdentitiesFoundException ex)
      {
        identity = ApiUserController.HandleMultipleIdentitiesFoundException(ex);
      }
      return identity;
    }

    [HttpGet]
    [TfsTraceFilter(508131, 508140)]
    public ActionResult GetUserFeatureLicenses() => (ActionResult) this.Json((object) this.TfsWebContext.GetUserFeatureLicenses(), JsonRequestBehavior.AllowGet);

    private Microsoft.VisualStudio.Services.Identity.Identity ResolveNewUser(
      string newUser,
      string domain)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate().To(TeamFoundationHostType.Application);
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      string str = string.IsNullOrEmpty(newUser) ? newUser : newUser.Trim();
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (ArgumentUtility.IsValidEmailAddress(str))
      {
        identity = IdentityHelper.GetOrCreateBindPendingIdentity(this.TfsRequestContext, domain, str, callerName: nameof (ResolveNewUser));
        if (identity.IsBindPending)
          service.UpdateIdentities(vssRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
          {
            identity
          });
      }
      return identity;
    }

    private static bool IsValidCheckedName(TeamFoundationIdentity identity)
    {
      if (identity == null)
        return false;
      return !identity.IsContainer || string.Equals(identity.Descriptor.IdentityType, "System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase);
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity HandleMultipleIdentitiesFoundException(
      Microsoft.TeamFoundation.Server.Core.MultipleIdentitiesFoundException ex)
    {
      List<TeamFoundationIdentity> list = ((IEnumerable<TeamFoundationIdentity>) ex.MatchingIdentities).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (identity => ApiUserController.IsValidCheckedName(identity))).ToList<TeamFoundationIdentity>();
      if (list.Count == 1)
        return IdentityUtil.Convert(list[0]);
      if (list.Count > 1)
        throw new Microsoft.TeamFoundation.Server.Core.MultipleIdentitiesFoundException(ex.FactorValue, list.ToArray());
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    private bool CheckLicenseAvailiability(
      License userLicense,
      IEnumerable<AccountLicenseUsage> licenseInfo,
      Guid userId,
      out string errorMessage)
    {
      AccountLicenseUsage accountLicenseUsage = (AccountLicenseUsage) null;
      switch (userLicense.Source)
      {
        case LicensingSource.Account:
          Dictionary<AccountLicenseType, string> errorDict = ApiUserController.LicenseToErrorDict();
          AccountLicense accountLicense = (AccountLicense) userLicense;
          errorMessage = errorDict[accountLicense.License];
          if (accountLicense.License.Equals((object) AccountLicenseType.EarlyAdopter))
          {
            int daysLeft = 0;
            return this.AllowEarlyAdopter(out daysLeft);
          }
          accountLicenseUsage = licenseInfo.Where<AccountLicenseUsage>((Func<AccountLicenseUsage, bool>) (license => license.License.Source.Equals((object) LicensingSource.Account) && (AccountLicenseType) license.License.License == accountLicense.License)).First<AccountLicenseUsage>();
          if (userId != Guid.Empty && (License) accountLicense == (License) AccountLicense.Stakeholder && this.TfsRequestContext.GetService<ICollectionService>().GetCollection(this.TfsRequestContext.Elevate(), (IEnumerable<string>) null).Owner == userId)
          {
            errorMessage = AccountServerResources.AccountOwnerCannotBeAssignedStakeholderLicense;
            return false;
          }
          break;
        case LicensingSource.Msdn:
          MsdnLicense msdnLicense = (MsdnLicense) userLicense;
          errorMessage = AccountServerResources.AccountUnknownLicense;
          return msdnLicense.License.Equals((object) MsdnLicenseType.Eligible);
        default:
          errorMessage = AccountServerResources.NoLicensesMessage;
          break;
      }
      if (accountLicenseUsage == null)
      {
        errorMessage = AccountServerResources.AccountUnknownLicense;
        return false;
      }
      return accountLicenseUsage.ProvisionedCount == -1 || accountLicenseUsage.ProvisionedCount - accountLicenseUsage.UsedCount > 0;
    }

    private void CreateCollectionContextAndExecute(Action<IVssRequestContext> action)
    {
      if (this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        action(this.TfsRequestContext);
      }
      else
      {
        TeamProjectCollectionProperties collectionProperties = this.TfsRequestContext.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionProperties(this.TfsRequestContext.Elevate(), ServiceHostFilterFlags.None).Last<TeamProjectCollectionProperties>();
        IVssRequestContext vssRequestContext1 = this.TfsRequestContext.Elevate().To(TeamFoundationHostType.Deployment);
        using (IVssRequestContext vssRequestContext2 = vssRequestContext1.GetService<TeamFoundationHostManagementService>().BeginRequest(vssRequestContext1, collectionProperties.Id, RequestContextType.SystemContext, true, false))
          action(vssRequestContext2);
      }
    }

    private static string TransformText(int licenseType, out string licenseEnum)
    {
      string str;
      switch (Enum.IsDefined(typeof (AccountLicenseType), (object) licenseType) ? licenseType : 0)
      {
        case 1:
          str = AccountServerResources.AccountEarlyAdopterLicense;
          licenseEnum = AccountLicense.EarlyAdopter.ToString();
          break;
        case 2:
          str = AccountServerResources.AccountStandardLicense;
          licenseEnum = AccountLicense.Express.ToString();
          break;
        case 3:
          str = AccountServerResources.AccountStandardProLicense;
          licenseEnum = AccountLicense.Professional.ToString();
          break;
        case 4:
          str = AccountServerResources.AccountAdvancedLicense;
          licenseEnum = AccountLicense.Advanced.ToString();
          break;
        case 5:
          str = AccountServerResources.AccountStakeholderLicense;
          licenseEnum = AccountLicense.Stakeholder.ToString();
          break;
        default:
          str = AccountServerResources.AccountNoLicense;
          licenseEnum = License.None.ToString();
          break;
      }
      return str;
    }

    private static Dictionary<License, string> LicenseToStringDict() => new Dictionary<License, string>()
    {
      {
        (License) AccountLicense.EarlyAdopter,
        AccountServerResources.AccountEarlyAdopterLicense
      },
      {
        (License) AccountLicense.Stakeholder,
        AccountServerResources.AccountStakeholderLicense
      },
      {
        (License) AccountLicense.Express,
        AccountServerResources.AccountStandardLicense
      },
      {
        (License) AccountLicense.Professional,
        AccountServerResources.AccountStandardProLicense
      },
      {
        (License) AccountLicense.Advanced,
        AccountServerResources.AccountAdvancedLicense
      },
      {
        (License) MsdnLicense.Eligible,
        AccountServerResources.AccountEligibleMSDNLicense
      },
      {
        (License) MsdnLicense.Professional,
        AccountServerResources.MSDNPro
      },
      {
        (License) MsdnLicense.Platforms,
        AccountServerResources.MsdnPlatforms
      },
      {
        (License) MsdnLicense.TestProfessional,
        AccountServerResources.MSDNTestPro
      },
      {
        (License) MsdnLicense.Premium,
        AccountServerResources.MSDNPremium
      },
      {
        (License) MsdnLicense.Ultimate,
        AccountServerResources.MsdnUltimate
      },
      {
        (License) MsdnLicense.Enterprise,
        AccountServerResources.MsdnEnterprise
      }
    };

    private static Dictionary<AccountLicenseType, string> LicenseToErrorDict() => new Dictionary<AccountLicenseType, string>()
    {
      {
        AccountLicenseType.EarlyAdopter,
        AccountServerResources.NoEarlyAdopterLicenses
      },
      {
        AccountLicenseType.Stakeholder,
        AccountServerResources.NoStakeholderLicenses
      },
      {
        AccountLicenseType.Express,
        AccountServerResources.NoStandardLicenses
      },
      {
        AccountLicenseType.Professional,
        AccountServerResources.NoStandardWithVSPro
      },
      {
        AccountLicenseType.Advanced,
        AccountServerResources.NoAdvancedLicenses
      }
    };
  }
}
