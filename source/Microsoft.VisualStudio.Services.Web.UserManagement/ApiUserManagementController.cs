// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Web.UserManagement.ApiUserManagementController
// Assembly: Microsoft.VisualStudio.Services.Web.UserManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BDE82F4-5081-4A92-A83F-EE78FF05B171
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Web.UserManagement.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.CloudConnected;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Invitation;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Web.UserManagement.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Microsoft.VisualStudio.Services.Web.UserManagement
{
  [SupportedRouteArea(NavigationContextLevels.All)]
  public class ApiUserManagementController : UserManagementAreaController
  {
    private const string s_area = "HostingAccount";
    private const string s_layer = "ApiUserManagementController";
    private const string Featureflag = "WebAccess.UserManagementInSps";
    internal const string s_featureNameAddUserInNewAccountUserTable = "VisualStudio.AccountService.AddUserInNewAccountUserTable";
    private const string s_userhubExtensionManagementFeature = "WebAccess.UserhubExtensionManagement";
    private const string s_featureAllowEarlyAdopterAfterExpiration = "WebAccess.AllowEarlyAdopterAfterExpiration";
    private const string BillingPeriodWarningDateFormatString = "MMMM d";
    private const string s_perUserLastAccessedDateSetting = "/Service/Licensing/AccountEntitlement/WebAccess.UserManagementInSps/PreUserLastAccessedDate";
    private static DateTime s_perUserLastAccessedDate = DateTime.MinValue;
    private static Guid s_licensingSecurityNamespaceId = new Guid("453E2DB3-2E81-474F-874D-3BF51027F2EE");
    private const string s_accountEntitlementsToken = "/Entitlements/AccountEntitlements/";
    private const int s_allPermissions = 63;
    private const int s_readPermission = 1;
    internal const string s_defaultImageDataUriType = "Png";
    internal const int s_defaultImageSize = 34;
    private const int maxJsonLength = 33554432;
    private const string s_userLicenses = "user-licenses";

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(504101, 504110)]
    [TfsHandleFeatureFlag("WebAccess.UserManagementInSps", null)]
    public ActionResult GetAccountUsers()
    {
      try
      {
        this.TfsRequestContext.Trace(504101, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "GetAccountUsers: Starting");
        IList<Microsoft.VisualStudio.Services.Identity.Identity> tfidentities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
        Dictionary<string, int> licenseCount = new Dictionary<string, int>();
        Dictionary<Guid, AccountEntitlement> licensedUsersMap = new Dictionary<Guid, AccountEntitlement>();
        this.TfsRequestContext.Trace(504102, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "GetAccountEntitlements: Reading user identities from account");
        Stopwatch stopwatch = Stopwatch.StartNew();
        this.CheckPermission(this.TfsRequestContext, 1);
        this.TfsRequestContext.Trace(540105, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "ApiUserManagementController:GetAccountUsers:CheckPermission ElapsedTime: {0}ms", (object) stopwatch.ElapsedMilliseconds);
        ILicensingEntitlementService service = this.TfsRequestContext.GetService<ILicensingEntitlementService>();
        stopwatch.Restart();
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        IList<AccountEntitlement> accountEntitlements = service.GetAccountEntitlements(tfsRequestContext);
        this.TfsRequestContext.Trace(540105, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "ApiUserManagementController:GetAccountUsers:GetAccountEntitlements ElapsedTime: {0}ms", (object) stopwatch.ElapsedMilliseconds);
        stopwatch.Restart();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> userLicense = this.GetUserLicense(tfidentities, licensedUsersMap, accountEntitlements);
        this.TfsRequestContext.Trace(540105, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "ApiUserManagementController:GetAccountUsers:GetUserLicense ElapsedTime: {0}ms", (object) stopwatch.ElapsedMilliseconds);
        stopwatch.Restart();
        this.SetPerUserLastAccessedDate();
        this.TfsRequestContext.Trace(540105, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "ApiUserManagementController:GetAccountUsers:SetPerUserLastAccessedDate ElapsedTime: {0}ms", (object) stopwatch.ElapsedMilliseconds);
        stopwatch.Stop();
        bool earlyAdopterLicenseExpired = this.IsEarlyAdopterLicenseExpired();
        JsonResult accountUsers = this.Json((object) this.GetUserList(userLicense, licenseCount, licensedUsersMap, earlyAdopterLicenseExpired), JsonRequestBehavior.AllowGet);
        accountUsers.MaxJsonLength = new int?(33554432);
        return (ActionResult) accountUsers;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(504618, "HostingAccount", nameof (ApiUserManagementController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.Trace(504641, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "GetAccountUsers: completed.");
      }
    }

    private UserViewModel GetUserList(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> tfidentities,
      Dictionary<string, int> licenseCount,
      Dictionary<Guid, AccountEntitlement> licensedUsersMap,
      bool earlyAdopterLicenseExpired)
    {
      UserViewModel vm = new UserViewModel();
      vm.Users = new List<UserModel>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity tfidentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) tfidentities)
      {
        AccountEntitlement accountEntitlement = (AccountEntitlement) null;
        if (tfidentity != null && !ServicePrincipals.IsServicePrincipal(this.TfsRequestContext, tfidentity.Descriptor) && !IdentityDescriptorComparer.Instance.Equals(tfidentity.Descriptor, UserWellKnownIdentityDescriptors.AnonymousPrincipal) && tfidentity != null && licensedUsersMap.TryGetValue(tfidentity.MasterId, out accountEntitlement))
        {
          UserModel userModel = new UserModel();
          userModel.Name = tfidentity.DisplayName.Trim();
          userModel.UserId = tfidentity.MasterId.ToString();
          if (earlyAdopterLicenseExpired && accountEntitlement.License == (License) AccountLicense.EarlyAdopter)
          {
            userModel.Status = UserManagementResources.UserStatusExpired;
            accountEntitlement.UserStatus = AccountUserStatus.Disabled;
          }
          else
            userModel.Status = ApiUserManagementController.GetStatus((int) accountEntitlement.UserStatus, accountEntitlement.License);
          userModel.LicenseType = ApiUserManagementController.GetLicenseType(accountEntitlement.License, accountEntitlement.UserStatus, licenseCount);
          userModel.isMsdn = accountEntitlement.License.Source.Equals((object) LicensingSource.Msdn);
          object displayName;
          if (tfidentity.TryGetProperty("Mail", out displayName))
          {
            if (displayName.Equals((object) string.Empty) && tfidentity.TryGetProperty("Account", out displayName) && displayName.Equals((object) string.Empty))
              displayName = (object) tfidentity.DisplayName;
            object obj;
            if (tfidentity.TryGetProperty("Domain", out obj) && !obj.Equals((object) string.Empty))
            {
              if (obj.Equals((object) "Windows Live ID"))
                userModel.UPN = string.Format("{0}\\{1}", obj, displayName);
              else if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
              {
                string tenantDisplayName = this.GetTenantDisplayName();
                userModel.UPN = string.Format("{0}\\{1}", (object) tenantDisplayName, displayName);
              }
            }
            userModel.SignInAddress = displayName.ToString();
          }
          userModel.LastAccessed = ApiUserManagementController.GetUserLastAccessedDate(accountEntitlement.LastAccessedDate, accountEntitlement.AssignmentDate);
          vm.Users.Add(userModel);
        }
      }
      vm.LicenseOverview = licenseCount;
      vm.Users = vm.Users.OrderBy<UserModel, string>((Func<UserModel, string>) (x => x.Name)).ToList<UserModel>();
      vm.Licenses = this.GetLicenseData(vm);
      return vm;
    }

    private IList<Microsoft.VisualStudio.Services.Identity.Identity> GetUserLicense(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> tfidentities,
      Dictionary<Guid, AccountEntitlement> licensedUsersMap,
      IList<AccountEntitlement> licensedUsers)
    {
      if (licensedUsers.Any<AccountEntitlement>())
      {
        List<Guid> masterIds = new List<Guid>();
        foreach (AccountEntitlement licensedUser in (IEnumerable<AccountEntitlement>) licensedUsers)
        {
          masterIds.Add(licensedUser.UserId);
          licensedUsersMap.Add(licensedUser.UserId, licensedUser);
        }
        this.TfsRequestContext.Trace(504104, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "GetAccountUsers: reading user identities from identity service");
        tfidentities = this.TfsRequestContext.GetExtension<IReadIdentitiesByMasterIdExtension>(ExtensionLifetime.Service).ReadIdentitiesByMasterId(this.TfsRequestContext, (IList<Guid>) masterIds, QueryMembership.None, (IEnumerable<string>) new string[1]
        {
          "CustomNotificationAddresses"
        });
      }
      return tfidentities;
    }

    protected virtual bool IsEarlyAdopterLicenseExpired() => this.IsEarlyAdopterExpired();

    protected virtual void SetPerUserLastAccessedDate()
    {
      if (!(ApiUserManagementController.s_perUserLastAccessedDate == DateTime.MinValue))
        return;
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      ApiUserManagementController.s_perUserLastAccessedDate = vssRequestContext.GetService<CachedRegistryService>().GetValue<DateTime>(vssRequestContext, (RegistryQuery) "/Service/Licensing/AccountEntitlement/WebAccess.UserManagementInSps/PreUserLastAccessedDate", DateTime.UtcNow);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(504601, 504610)]
    [TfsHandleFeatureFlag("WebAccess.UserManagementInSps", null)]
    public ActionResult GetAccountAndUserExtensionToCSV()
    {
      try
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> tfidentities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
        Dictionary<string, int> licenseCount = new Dictionary<string, int>();
        Dictionary<Guid, AccountEntitlement> licensedUsersMap = new Dictionary<Guid, AccountEntitlement>();
        this.CheckPermission(this.TfsRequestContext, 1);
        IList<AccountEntitlement> accountEntitlements = this.TfsRequestContext.GetService<ILicensingEntitlementService>().GetAccountEntitlements(this.TfsRequestContext);
        IList<Microsoft.VisualStudio.Services.Identity.Identity> userLicense = this.GetUserLicense(tfidentities, licensedUsersMap, accountEntitlements);
        this.SetPerUserLastAccessedDate();
        bool earlyAdopterLicenseExpired = this.IsEarlyAdopterLicenseExpired();
        UserViewModel userList = this.GetUserList(userLicense, licenseCount, licensedUsersMap, earlyAdopterLicenseExpired);
        IEnumerable<ExtensionViewModel> extensions = new ExtensionBuilder(this.TfsRequestContext).GetExtensions(this.TfsRequestContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptions(this.TfsRequestContext));
        string csv = this.ConvertObjectArrayToCSV(userList.Users, extensions);
        string str = this.TfsRequestContext.ServiceHost.CollectionServiceHost.Name + "-user-licenses";
        Encoding encoding = (Encoding) new UTF8Encoding(true);
        StreamReader streamReader = new StreamReader((Stream) new MemoryStream(encoding.GetBytes(csv)));
        this.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.csv", (object) str));
        this.Response.ContentType = "text/csv";
        this.Response.ContentEncoding = encoding;
        this.Response.BinaryWrite(encoding.GetPreamble());
        this.Response.Write(streamReader.ReadToEnd());
        this.Response.End();
        CustomerIntelligenceService service = this.TfsRequestContext.GetService<CustomerIntelligenceService>();
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add(CustomerIntelligenceProperty.NumberOfUsersInTheAccount, (double) userList.Users.Count);
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string acquisition = CustomerIntelligenceArea.Acquisition;
        string exportCsv = CustomerIntelligenceFeature.ExportCSV;
        CustomerIntelligenceData properties = intelligenceData;
        service.Publish(tfsRequestContext, acquisition, exportCsv, properties);
        return (ActionResult) this.View();
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(504608, "HostingAccount", nameof (ApiUserManagementController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.Trace(504609, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "GetAccountAndUserExtension: completed.");
      }
    }

    private string ConvertObjectArrayToCSV(
      List<UserModel> userList,
      IEnumerable<ExtensionViewModel> extensionViewModels)
    {
      List<HashSet<string>> userSets = new List<HashSet<string>>();
      List<string> displayNames = new List<string>()
      {
        UserManagementResources.UserGridColName,
        UserManagementResources.Username,
        UserManagementResources.LicenseLabel,
        UserManagementResources.UserGridColStatus,
        UserManagementResources.UserGridColLastAccessed
      };
      foreach (ExtensionViewModel extensionViewModel in extensionViewModels)
      {
        List<UserModel> users = this.GetUsers(extensionViewModel.ExtensionId, this.TfsRequestContext).Users;
        userSets.Add(this.StoreUserIds(users));
        displayNames.Add(extensionViewModel.DisplayName);
      }
      return this.ConvertToString(displayNames, userList, userSets);
    }

    private string ConvertToString(
      List<string> displayNames,
      List<UserModel> userList,
      List<HashSet<string>> userSets)
    {
      string separator = ",";
      string newLine = Environment.NewLine;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(string.Join(separator, (IEnumerable<string>) displayNames));
      stringBuilder.Append(newLine);
      foreach (UserModel user1 in userList)
      {
        UserModel user = user1;
        List<string> values = new List<string>()
        {
          this.StringToCSVEscape(user.Name),
          string.IsNullOrEmpty(user.UPN) ? user.SignInAddress : user.UPN,
          user.LicenseType,
          user.Status,
          user.LastAccessed
        };
        values.AddRange(userSets.Select<HashSet<string>, string>((Func<HashSet<string>, string>) (x => !x.Contains(user.UserId) ? "0" : "1")));
        stringBuilder.Append(string.Join(separator, (IEnumerable<string>) values));
        stringBuilder.Append(newLine);
      }
      return stringBuilder.ToString();
    }

    private string StringToCSVEscape(string username)
    {
      if ((username.Contains(",") ? 1 : (username.Contains("\"") ? 1 : 0)) == 0)
        return username;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("\"");
      stringBuilder.Append(username.Replace("\"", "\"\""));
      stringBuilder.Append("\"");
      return stringBuilder.ToString();
    }

    private HashSet<string> StoreUserIds(List<UserModel> users)
    {
      HashSet<string> stringSet = new HashSet<string>();
      foreach (UserModel user in users)
        stringSet.Add(user.UserId);
      return stringSet;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(504611, 504620)]
    [TfsHandleFeatureFlag("WebAccess.UserhubExtensionManagement", null)]
    public ActionResult GetAccountExtensionUsersToCSV(string extensionId)
    {
      try
      {
        this.CheckExtensionIdIsValid(extensionId);
        this.TfsRequestContext.Trace(504611, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "GetAccountExtensionUsersToCSV: Starting");
        this.TfsRequestContext.Trace(504612, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "GetAccountEntitlements: Reading user identities from account");
        ExtensionUsersViewModel users = this.GetUsers(extensionId, this.TfsRequestContext);
        string name = this.TfsRequestContext.ServiceHost.CollectionServiceHost.Name;
        string csv = this.ConvertUserDataToCSV(users, name);
        string displayName = users.ExtensionAvailabilityViewModel.ExtensionViewModel.DisplayName;
        this.Response.AddHeader("Content-Disposition", "attachment; filename=" + (name + "-" + displayName + ".csv"));
        this.Response.ContentType = "text/csv";
        this.Response.Write(csv);
        this.Response.End();
        return (ActionResult) new EmptyResult();
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(504618, "HostingAccount", nameof (ApiUserManagementController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.Trace(504941, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "GetAccountExtensionUsersToCSV: completed.");
      }
    }

    private string ConvertUserDataToCSV(ExtensionUsersViewModel vw, string collectionName)
    {
      List<string> values1 = new List<string>()
      {
        UserManagementResources.UserGridColName,
        UserManagementResources.Username,
        UserManagementResources.UserGridColExtStatus
      };
      string separator = ",";
      string newLine = Environment.NewLine;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(string.Join(separator, (IEnumerable<string>) values1));
      stringBuilder.Append(newLine);
      foreach (UserModel user in vw.Users)
      {
        string extensionStatus = this.getExtensionStatus(user, collectionName, vw);
        List<string> values2 = new List<string>()
        {
          this.StringToCSVEscape(user.Name),
          string.IsNullOrEmpty(user.UPN) ? user.SignInAddress : user.UPN,
          extensionStatus
        };
        stringBuilder.Append(string.Join(separator, (IEnumerable<string>) values2));
        stringBuilder.Append(newLine);
      }
      return stringBuilder.ToString();
    }

    private string getExtensionStatus(
      UserModel user,
      string collectionName,
      ExtensionUsersViewModel vw)
    {
      if (user.isMsdn)
        return UserManagementResources.IncludedWithSubscription;
      if (user.IsRoaming)
        return string.Format("Sourced from {0}", (object) collectionName);
      if (vw.ExtensionAvailabilityViewModel.ExtensionViewModel.ExtensionState.Equals("Trial"))
        return string.Format("Trial (ends {0})", (object) string.Format(vw.ExtensionAvailabilityViewModel.ExtensionViewModel.BillingStartDate.ToString(), (object) "M/d/yyyy"));
      return !this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment ? string.Empty : UserManagementResources.UserSectionPaidMonthly;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(504901, 504910)]
    [TfsHandleFeatureFlag("WebAccess.UserhubExtensionManagement", null)]
    public ActionResult GetAccountExtensionUsers(string extensionId)
    {
      try
      {
        this.CheckExtensionIdIsValid(extensionId);
        this.TfsRequestContext.Trace(504901, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "GetAccountExtensionUsers: Starting");
        this.TfsRequestContext.Trace(504902, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "GetAccountEntitlements: Reading user identities from account");
        ExtensionUsersViewModel users = this.GetUsers(extensionId, this.TfsRequestContext);
        if (this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment && !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableOnPremCommerce"))
        {
          IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.ProjectCollection);
          DateTime? gracePeriodEndDate = vssRequestContext.GetService<ICommercePackageSynchronizationService>().GetGracePeriodEndDate(vssRequestContext);
          if (gracePeriodEndDate.HasValue)
          {
            DateTime dateTime = gracePeriodEndDate.Value;
            users.GracePeriodEndDate = dateTime.ToString("o");
            if (dateTime < DateTime.UtcNow)
              users.GracePeriodExpired = true;
            else
              users.InGracePeriod = true;
          }
        }
        JsonResult accountExtensionUsers = this.Json((object) users, JsonRequestBehavior.AllowGet);
        accountExtensionUsers.MaxJsonLength = new int?(33554432);
        return (ActionResult) accountExtensionUsers;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(504918, "HostingAccount", nameof (ApiUserManagementController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.Trace(504941, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "GetAccountUsers: completed.");
      }
    }

    private ExtensionUsersViewModel GetUsers(string extensionId, IVssRequestContext requestContext)
    {
      List<UserModel> source = new List<UserModel>();
      int usedLicenseCount = 0;
      IDictionary<Guid, ExtensionAssignmentDetails> dictionary1 = this.FilterUsersByExtension(extensionId, out usedLicenseCount);
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> tfidentities = this.GetTfidentities((IList<Guid>) dictionary1.Keys.ToList<Guid>());
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        ILicensingEntitlementService service = requestContext.GetService<ILicensingEntitlementService>();
        this.CheckPermission(requestContext, 1);
        this.SetPerUserLastAccess();
        IVssRequestContext requestContext1 = requestContext;
        Dictionary<Guid, AccountEntitlement> dictionary2 = service.GetAccountEntitlements(requestContext1).ToDictionary<AccountEntitlement, Guid>((Func<AccountEntitlement, Guid>) (user => user.UserId));
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in tfidentities)
        {
          AccountEntitlement licensedUser = (AccountEntitlement) null;
          ExtensionAssignmentDetails assignmentDetails;
          if (dictionary2.TryGetValue(identity.MasterId, out licensedUser) && dictionary1.TryGetValue(identity.MasterId, out assignmentDetails))
            source.Add(this.MapToUserModel(this.TfsRequestContext, identity, licensedUser, assignmentDetails));
        }
      }
      else
      {
        ILegacyLicensingHandler extension = this.TfsRequestContext.GetExtension<ILegacyLicensingHandler>();
        this.CheckPermission(this.TfsRequestContext, 1);
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        ICollection<Guid> keys = dictionary1.Keys;
        Dictionary<Guid, License> licensesForUsers = extension.GetLicensesForUsers(tfsRequestContext, (IEnumerable<Guid>) keys);
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in tfidentities)
        {
          License license = (License) null;
          ExtensionAssignmentDetails assignmentDetails;
          if (licensesForUsers.TryGetValue(identity.MasterId, out license) && dictionary1.TryGetValue(identity.MasterId, out assignmentDetails))
            source.Add(this.MapToUserModel(this.TfsRequestContext, identity, license, assignmentDetails));
        }
      }
      return new ExtensionUsersViewModel()
      {
        Users = source.OrderBy<UserModel, string>((Func<UserModel, string>) (x => x.Name)).ToList<UserModel>(),
        ExtensionAvailabilityViewModel = this.GetExtensionAvailability(extensionId, new int?(usedLicenseCount))
      };
    }

    private static Guid ParseInput(string param, string paramDisplayNameForErrorMessage)
    {
      if (string.IsNullOrEmpty(param))
        throw new ArgumentException(paramDisplayNameForErrorMessage);
      Guid result;
      if (!Guid.TryParse(param, out result))
        throw new ArgumentException(paramDisplayNameForErrorMessage);
      return !result.Equals(Guid.Empty) ? result : throw new ArgumentException(paramDisplayNameForErrorMessage);
    }

    private IDictionary<Guid, ExtensionAssignmentDetails> FilterUsersByExtension(
      string extensionId,
      out int usedLicenseCount)
    {
      IDictionary<Guid, ExtensionAssignmentDetails> extensionStatusForUsers = this.TfsRequestContext.GetService<IExtensionEntitlementService>().GetExtensionStatusForUsers(this.TfsRequestContext, extensionId);
      usedLicenseCount = extensionStatusForUsers.Count<KeyValuePair<Guid, ExtensionAssignmentDetails>>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, bool>) (s => s.Value.AssignmentStatus == ExtensionAssignmentStatus.AccountAssignment || s.Value.AssignmentStatus == ExtensionAssignmentStatus.ImplicitAssignment));
      return (IDictionary<Guid, ExtensionAssignmentDetails>) extensionStatusForUsers.Where<KeyValuePair<Guid, ExtensionAssignmentDetails>>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, bool>) (x => x.Value.AssignmentStatus == ExtensionAssignmentStatus.AccountAssignment || x.Value.AssignmentStatus == ExtensionAssignmentStatus.RoamingAccountAssignment || x.Value.AssignmentStatus == ExtensionAssignmentStatus.BundleAssignment || x.Value.AssignmentStatus == ExtensionAssignmentStatus.ImplicitAssignment)).ToDictionary<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid, ExtensionAssignmentDetails>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, ExtensionAssignmentDetails>) (y => y.Value));
    }

    private UserModel MapToUserModel(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      AccountEntitlement licensedUser,
      ExtensionAssignmentDetails assignmentDetails = null)
    {
      if (assignmentDetails == null)
        assignmentDetails = new ExtensionAssignmentDetails(ExtensionAssignmentStatus.NotAssigned);
      UserModel userInstance = new UserModel()
      {
        Name = identity.DisplayName.Trim(),
        UserId = identity.MasterId.ToString()
      };
      if (this.IsEarlyAdopterExpired() && licensedUser.License == (License) AccountLicense.EarlyAdopter)
      {
        userInstance.Status = UserManagementResources.UserStatusExpired;
        licensedUser.UserStatus = AccountUserStatus.Disabled;
      }
      else
        userInstance.Status = ApiUserManagementController.GetStatus((int) licensedUser.UserStatus, licensedUser.License);
      Dictionary<string, int> licenseCount = new Dictionary<string, int>();
      userInstance.LicenseType = ApiUserManagementController.GetLicenseType(licensedUser.License, licensedUser.UserStatus, licenseCount);
      userInstance.isMsdn = assignmentDetails.AssignmentStatus == ExtensionAssignmentStatus.BundleAssignment || licensedUser.License.Source.Equals((object) LicensingSource.Msdn) && assignmentDetails.AssignmentStatus == ExtensionAssignmentStatus.ImplicitAssignment;
      this.FindAndSetSignInAddressForUserInViewModel(requestContext, identity, userInstance);
      userInstance.LastAccessed = ApiUserManagementController.GetUserLastAccessedDate(licensedUser.LastAccessedDate, licensedUser.AssignmentDate);
      userInstance.IsRoaming = assignmentDetails.AssignmentStatus == ExtensionAssignmentStatus.RoamingAccountAssignment;
      userInstance.SourceCollectionName = assignmentDetails.SourceCollectionName;
      return userInstance;
    }

    private UserModel MapToUserModel(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      License license,
      ExtensionAssignmentDetails assignmentDetails = null)
    {
      if (assignmentDetails == null)
        assignmentDetails = new ExtensionAssignmentDetails(ExtensionAssignmentStatus.NotAssigned);
      UserModel userInstance = new UserModel()
      {
        Name = identity.DisplayName.Trim(),
        UserId = identity.MasterId.ToString()
      };
      userInstance.Status = string.Empty;
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      userInstance.LicenseType = ApiUserManagementController.LicenseToStringDict()[license];
      userInstance.isMsdn = assignmentDetails.AssignmentStatus == ExtensionAssignmentStatus.BundleAssignment;
      this.FindAndSetSignInAddressForUserInViewModel(requestContext, identity, userInstance);
      userInstance.LastAccessed = string.Empty;
      userInstance.IsRoaming = assignmentDetails.AssignmentStatus == ExtensionAssignmentStatus.RoamingAccountAssignment;
      userInstance.SourceCollectionName = assignmentDetails.SourceCollectionName;
      return userInstance;
    }

    private void SetPerUserLastAccess()
    {
      if (ApiUserManagementController.s_perUserLastAccessedDate != DateTime.MinValue)
        return;
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      ApiUserManagementController.s_perUserLastAccessedDate = vssRequestContext.GetService<CachedRegistryService>().GetValue<DateTime>(vssRequestContext, (RegistryQuery) "/Service/Licensing/AccountEntitlement/WebAccess.UserManagementInSps/PreUserLastAccessedDate", DateTime.UtcNow);
    }

    private bool IsEarlyAdopterExpired() => !this.IsEarlyAdopterOrganization() && !this.TfsRequestContext.IsFeatureEnabled("WebAccess.AllowEarlyAdopterAfterExpiration");

    private ExtensionAvailabilityViewModel GetExtensionAvailability(
      string extensionId,
      int? usedLicenseCount)
    {
      this.CheckPermission(this.TfsRequestContext, 1);
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        this.SetPerUserLastAccess();
      if (!usedLicenseCount.HasValue)
        usedLicenseCount = new int?(this.TfsRequestContext.GetService<IExtensionEntitlementService>().GetExtensionStatusForUsers(this.TfsRequestContext, extensionId).Count<KeyValuePair<Guid, ExtensionAssignmentDetails>>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, bool>) (s => s.Value.AssignmentStatus == ExtensionAssignmentStatus.AccountAssignment || s.Value.AssignmentStatus == ExtensionAssignmentStatus.ImplicitAssignment)));
      if (this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment && this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableOnPremCommerce"))
        return this.GetOfflineExtensionAvailabilityViewModel(extensionId, usedLicenseCount.Value);
      IOfferSubscription subscription = this.TfsRequestContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptions(this.TfsRequestContext).FirstOrDefault<IOfferSubscription>((Func<IOfferSubscription, bool>) (o => o.OfferMeter.GalleryId == extensionId));
      if (subscription == null)
        return this.GetOfflineExtensionAvailabilityViewModel(extensionId, usedLicenseCount.Value);
      ExtensionBuilder extensionBuilder = new ExtensionBuilder(this.TfsRequestContext);
      return new ExtensionAvailabilityViewModel()
      {
        ExtensionViewModel = extensionBuilder.GetExtensionViewModelFromOfferSubscription(subscription, this.TfsRequestContext),
        Total = subscription.CommittedQuantity,
        InUse = usedLicenseCount.Value,
        IncludedQuantity = subscription.IncludedQuantity
      };
    }

    private ExtensionAvailabilityViewModel GetOfflineExtensionAvailabilityViewModel(
      string extensionId,
      int usedLicenseCount,
      int includedQuantity = 0)
    {
      bool? nullable = this.IsConnectedOnPremisesServer(this.TfsRequestContext);
      if (!nullable.HasValue || nullable.Value)
        return new ExtensionAvailabilityViewModel();
      string extensionDisplayName = this.TfsRequestContext.GetExtension<IOnPremiseOfflineExtensionHandler>().GetExtensionDisplayName(this.TfsRequestContext, extensionId);
      return new ExtensionAvailabilityViewModel()
      {
        ExtensionViewModel = ExtensionBuilder.GetExtensionViewModelFromOfflineDate(extensionId, extensionDisplayName, new DateTime?()),
        Total = 0,
        InUse = usedLicenseCount,
        IncludedQuantity = includedQuantity
      };
    }

    private bool? IsConnectedOnPremisesServer(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? new bool?(!requestContext.GetService<ICloudConnectedService>().GetConnectedAccountId(requestContext).Equals(Guid.Empty)) : new bool?();

    private IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetTfidentities(
      IList<Guid> licensedUsersMap)
    {
      if (licensedUsersMap.Count <= 0)
        return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.TfsRequestContext.Trace(504904, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "Reading user identities from identity service");
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) this.TfsRequestContext.GetExtension<IReadIdentitiesByMasterIdExtension>(ExtensionLifetime.Service).ReadIdentitiesByMasterId(this.TfsRequestContext, licensedUsersMap, QueryMembership.None, (IEnumerable<string>) new string[1]
      {
        "CustomNotificationAddresses"
      });
    }

    public void FindAndSetSignInAddressForUserInViewModel(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      UserModel userInstance)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        object displayName;
        if (!identity.TryGetProperty("Mail", out displayName))
          return;
        if (displayName.Equals((object) string.Empty) && identity.TryGetProperty("Account", out displayName) && displayName.Equals((object) string.Empty))
          displayName = (object) identity.DisplayName;
        userInstance.SignInAddress = displayName.ToString();
      }
      else
      {
        object obj1;
        if (identity.TryGetProperty("Account", out obj1))
        {
          object obj2;
          if (identity.TryGetProperty("Domain", out obj2))
            userInstance.SignInAddress = obj2.ToString() + "\\" + obj1.ToString();
          else
            userInstance.SignInAddress = obj1.ToString();
        }
        else
          userInstance.SignInAddress = identity.DisplayName;
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(505201, 505210)]
    [TfsHandleFeatureFlag("WebAccess.UserhubExtensionManagement", null)]
    public ActionResult GetExtensionEligibleUsers(string extensionId)
    {
      try
      {
        this.CheckExtensionIdIsValid(extensionId);
        this.TfsRequestContext.Trace(505201, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "GetExtensionEligibleUsers: Starting");
        this.TfsRequestContext.Trace(505202, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "GetAccountEntitlements: Reading user identities from account");
        this.CheckPermission(this.TfsRequestContext, 1);
        List<UserModel> source = new List<UserModel>();
        if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          Dictionary<Guid, AccountEntitlement> dictionary = this.TfsRequestContext.GetService<ILicensingEntitlementService>().GetAccountEntitlements(this.TfsRequestContext).ToDictionary<AccountEntitlement, Guid>((Func<AccountEntitlement, Guid>) (user => user.UserId));
          foreach (Microsoft.VisualStudio.Services.Identity.Identity tfidentity in this.GetTfidentities((IList<Guid>) this.FilterEligibleUsers(extensionId, dictionary).Keys.ToList<Guid>()))
          {
            AccountEntitlement licensedUser = (AccountEntitlement) null;
            if (dictionary.TryGetValue(tfidentity.MasterId, out licensedUser))
              source.Add(this.MapToUserModel(this.TfsRequestContext, tfidentity, licensedUser));
          }
        }
        else
        {
          ILegacyLicensingHandler extension = this.TfsRequestContext.GetExtension<ILegacyLicensingHandler>();
          IList<Guid> usersForExtension = this.TfsRequestContext.GetService<IExtensionEntitlementService>().GetEligibleUsersForExtension(this.TfsRequestContext, extensionId, ExtensionFilterOptions.None);
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          IList<Guid> userIds = usersForExtension;
          Dictionary<Guid, License> licensesForUsers = extension.GetLicensesForUsers(tfsRequestContext, (IEnumerable<Guid>) userIds);
          foreach (Microsoft.VisualStudio.Services.Identity.Identity tfidentity in this.GetTfidentities(usersForExtension))
          {
            License license = (License) null;
            if (licensesForUsers.TryGetValue(tfidentity.MasterId, out license))
              source.Add(this.MapToUserModel(this.TfsRequestContext, tfidentity, license));
          }
        }
        ExtensionUsersViewModel data = new ExtensionUsersViewModel();
        data.Users = source.OrderBy<UserModel, string>((Func<UserModel, string>) (x => x.Name)).ToList<UserModel>();
        ExtensionAvailabilityViewModel extensionAvailability = this.GetExtensionAvailability(extensionId, new int?());
        data.ExtensionAvailabilityViewModel = extensionAvailability;
        JsonResult extensionEligibleUsers = this.Json((object) data, JsonRequestBehavior.AllowGet);
        extensionEligibleUsers.MaxJsonLength = new int?(33554432);
        return (ActionResult) extensionEligibleUsers;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(504918, "HostingAccount", nameof (ApiUserManagementController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.Trace(504941, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "GetAccountUsers: completed.");
      }
    }

    private Dictionary<Guid, AccountEntitlement> FilterEligibleUsers(
      string extensionId,
      Dictionary<Guid, AccountEntitlement> licensedUsersMap)
    {
      IList<Guid> eligibleUsersForExtension = this.TfsRequestContext.GetService<IExtensionEntitlementService>().GetEligibleUsersForExtension(this.TfsRequestContext, extensionId, ExtensionFilterOptions.None);
      return licensedUsersMap.Where<KeyValuePair<Guid, AccountEntitlement>>((Func<KeyValuePair<Guid, AccountEntitlement>, bool>) (p => eligibleUsersForExtension.Contains(p.Key))).ToDictionary<KeyValuePair<Guid, AccountEntitlement>, Guid, AccountEntitlement>((Func<KeyValuePair<Guid, AccountEntitlement>, Guid>) (p => p.Key), (Func<KeyValuePair<Guid, AccountEntitlement>, AccountEntitlement>) (p => p.Value));
    }

    [HttpPost]
    [TfsTraceFilter(504321, 504330)]
    [TfsHandleFeatureFlag("WebAccess.UserManagementInSps", null)]
    [ValidateAntiForgeryToken]
    public ActionResult AddMultipleUsers(string serializedUsers)
    {
      if (string.IsNullOrEmpty(serializedUsers))
        throw new ArgumentException(nameof (serializedUsers));
      try
      {
        this.TfsRequestContext.TraceEnter(504321, "HostingAccount", nameof (ApiUserManagementController), nameof (AddMultipleUsers));
        List<AddMultipleUsersModel> multipleUsersModelList = new JavaScriptSerializer().Deserialize<List<AddMultipleUsersModel>>(serializedUsers);
        if (multipleUsersModelList == null || multipleUsersModelList.Count == 0)
        {
          JsObject data = new JsObject();
          data["error"] = (object) UserManagementResources.AddMultipleUsersInvalidInput;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        string tenantDisplayName = this.GetTenantDisplayName();
        this.TfsRequestContext.Trace(504322, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "AddMultipleUsers: Adding multiple users to account {0} ({1}) in tenant {2} ({3})", (object) this.TfsRequestContext.ServiceHost.Name, (object) this.TfsRequestContext.ServiceHost.InstanceId, (object) tenantDisplayName, (object) this.TfsRequestContext.GetOrganizationAadTenantId());
        JsObject data1 = new JsObject();
        foreach (AddMultipleUsersModel multipleUsersModel in multipleUsersModelList)
        {
          try
          {
            data1[multipleUsersModel.UserName] = (object) this.AddUserToAccount(tenantDisplayName, multipleUsersModel.UserName, multipleUsersModel.LicenseType, multipleUsersModel.DisplayName, multipleUsersModel.ObjectId);
          }
          catch (Exception ex)
          {
            this.TfsRequestContext.TraceException(504327, "HostingAccount", nameof (ApiUserManagementController), ex);
            if (!string.IsNullOrEmpty(multipleUsersModel.UserName))
            {
              if (ex is ArgumentException)
              {
                JsObject jsObject = new JsObject();
                jsObject["error"] = (object) UserManagementResources.AddMultipleUsersInvalidArguments;
                data1[multipleUsersModel.UserName] = (object) jsObject;
              }
              else
              {
                JsObject jsObject = new JsObject();
                jsObject["error"] = (object) UserManagementResources.AddMultipleUsersErrorAddingUser;
                data1[multipleUsersModel.UserName] = (object) jsObject;
              }
            }
          }
        }
        return (ActionResult) this.Json((object) data1, JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(504328, "HostingAccount", nameof (ApiUserManagementController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(504329, "HostingAccount", nameof (ApiUserManagementController), nameof (AddMultipleUsers));
      }
    }

    [HttpPost]
    [TfsTraceFilter(505321, 505330)]
    [TfsHandleFeatureFlag("WebAccess.UserhubExtensionManagement", null)]
    [ValidateAntiForgeryToken]
    public ActionResult AddMultipleUsersToExtension(
      string serializedUsers,
      string extensionId,
      string ciData = null)
    {
      this.CheckExtensionIdIsValid(extensionId);
      if (string.IsNullOrEmpty(serializedUsers))
        throw new ArgumentException(nameof (serializedUsers));
      this.CheckPermission(this.TfsRequestContext, 63);
      try
      {
        this.TfsRequestContext.TraceEnter(504321, "HostingAccount", nameof (ApiUserManagementController), nameof (AddMultipleUsersToExtension));
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        List<AddMultipleUsersToExtensionModel> toExtensionModelList = scriptSerializer.Deserialize<List<AddMultipleUsersToExtensionModel>>(serializedUsers);
        AssignExtensionCiData ciData1 = ciData == null ? (AssignExtensionCiData) null : scriptSerializer.Deserialize<AssignExtensionCiData>(ciData);
        if (toExtensionModelList == null || toExtensionModelList.Count == 0)
        {
          JsObject data = new JsObject();
          data["error"] = (object) UserManagementResources.AddMultipleUsersToExtensionInvalidInput;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        Dictionary<Guid, string> userGuidNameMap = new Dictionary<Guid, string>();
        JsObject jsObject1 = new JsObject();
        foreach (AddMultipleUsersToExtensionModel toExtensionModel in toExtensionModelList)
        {
          try
          {
            Guid input = ApiUserManagementController.ParseInput(toExtensionModel.ObjectId, "userId");
            userGuidNameMap.Add(input, toExtensionModel.UserName);
          }
          catch (ArgumentException ex)
          {
            JsObject jsObject2 = new JsObject();
            jsObject2["error"] = (object) string.Format(UserManagementResources.AddMultipleUsersToExtensionInvalidUserId, toExtensionModel.DisplayName.IsNullOrEmpty<char>() ? (object) toExtensionModel.DisplayName : (object) toExtensionModel.UserName);
            JsObject jsObject3 = jsObject2;
            jsObject1[toExtensionModel.UserName] = (object) jsObject3;
          }
        }
        ICollection<ExtensionOperationResult> users = this.TfsRequestContext.GetService<IExtensionEntitlementService>().AssignExtensionToUsers(this.TfsRequestContext, extensionId, (IList<Guid>) userGuidNameMap.Keys.ToList<Guid>());
        this.PublishAssignUsersToExtensionCiEvent(toExtensionModelList.Count, toExtensionModelList.Count - users.Count<ExtensionOperationResult>((Func<ExtensionOperationResult, bool>) (x => x.Result == OperationResult.Error)), extensionId, ciData1, "Assign");
        return (ActionResult) this.Json((object) ApiUserManagementController.MergeErrorsJsObject(ApiUserManagementController.AddErrorsFromExtensionLicensingServiceResult(users, userGuidNameMap), jsObject1), JsonRequestBehavior.AllowGet);
      }
      catch (LicenseNotAvailableException ex)
      {
        JsObject data = new JsObject();
        data["error"] = (object) ex.Message;
        return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(505328, "HostingAccount", nameof (ApiUserManagementController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(505329, "HostingAccount", nameof (ApiUserManagementController), nameof (AddMultipleUsersToExtension));
      }
    }

    private void CheckExtensionIdIsValid(string extensionId)
    {
      if (extensionId.IsNullOrEmpty<char>())
        throw new ArgumentException(nameof (extensionId));
    }

    private static JsObject MergeErrorsJsObject(params JsObject[] jsObjects)
    {
      JsObject jsObject1 = new JsObject();
      foreach (JsObject jsObject2 in jsObjects)
        jsObject1.AddObject(jsObject2);
      return jsObject1;
    }

    private static JsObject AddErrorsFromExtensionLicensingServiceResult(
      ICollection<ExtensionOperationResult> results,
      Dictionary<Guid, string> userGuidNameMap)
    {
      JsObject jsObject1 = new JsObject();
      foreach (ExtensionOperationResult result in (IEnumerable<ExtensionOperationResult>) results)
      {
        JsObject jsObject2 = new JsObject();
        jsObject2["error"] = (object) result.Message;
        JsObject jsObject3 = jsObject2;
        string empty = string.Empty;
        if (userGuidNameMap.TryGetValue(result.UserId, out empty))
          jsObject1[empty] = (object) jsObject3;
        else
          jsObject1[result.UserId.ToString()] = (object) jsObject3;
      }
      return jsObject1;
    }

    [HttpPost]
    [TfsTraceFilter(504801, 504810)]
    [TfsHandleFeatureFlag("WebAccess.UserManagementInSps", null)]
    [ValidateAntiForgeryToken]
    public ActionResult ResendInvitation(string userId)
    {
      try
      {
        if (string.IsNullOrEmpty(userId))
          throw new ArgumentException("serializedUsers");
        this.TfsRequestContext.TraceEnter(504801, "HostingAccount", nameof (ApiUserManagementController), "ResendInvitation: started");
        Guid result;
        if (!Guid.TryParse(userId, out result))
          throw new ArgumentException(nameof (userId));
        if (result.Equals(Guid.Empty))
          throw new ArgumentException(nameof (userId));
        this.TfsRequestContext.GetService<IInvitationService>().SendInvitation(this.TfsRequestContext, result, (InvitationData) null);
        return (ActionResult) this.Json((object) "success", JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(504808, "HostingAccount", nameof (ApiUserManagementController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.Trace(504809, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "ResendInvitation: completed.");
      }
    }

    [HttpPost]
    [TfsTraceFilter(508121, 508130)]
    [TfsHandleFeatureFlag("WebAccess.UserManagementInSps", null)]
    [ValidateAntiForgeryToken]
    public ActionResult EditUser(string userId, string licenseType)
    {
      try
      {
        this.TfsRequestContext.Trace(508122, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "EditUser: Starting");
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
        License license = ApiUserManagementController.ParseLicense(licenseType);
        this.TfsRequestContext.Trace(508123, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "EditUser: Validate Request Permissions");
        if (!this.HasAllPermissions())
        {
          data["error"] = (object) UserManagementResources.UserPermissionsError;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        ILicensingEntitlementService service = this.TfsRequestContext.GetService<ILicensingEntitlementService>();
        IList<AccountLicenseUsage> licensesUsage = service.GetLicensesUsage(this.TfsRequestContext);
        string errorMessage;
        if (!this.CheckLicenseAvailability(license, (IEnumerable<AccountLicenseUsage>) licensesUsage, userGuid, out errorMessage))
        {
          data["error"] = (object) errorMessage;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        this.TfsRequestContext.Trace(508124, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "EditUser: Verify User is in account");
        if (!ApiUserManagementController.ValidateLicenseAssignment(service.GetAccountEntitlements(this.TfsRequestContext).Where<AccountEntitlement>((Func<AccountEntitlement, bool>) (user => user.UserId.Equals(userGuid))).First<AccountEntitlement>().License, license))
        {
          data["error"] = (object) UserManagementResources.NoMSDNToEligible;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        this.TfsRequestContext.Trace(508125, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "EditUser: UpdateUser in Account");
        service.AssignAccountEntitlement(this.TfsRequestContext, userGuid, license);
        return (ActionResult) this.Json((object) "success", JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(508128, "HostingAccount", nameof (ApiUserManagementController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.Trace(508129, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "EditUser: completed.");
      }
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(508101, 508110)]
    [TfsHandleFeatureFlag("WebAccess.UserManagementInSps", null)]
    [ValidateAntiForgeryToken]
    public ActionResult RemoveUser(string userId)
    {
      this.TfsRequestContext.CheckOnPremisesDeployment();
      try
      {
        this.TfsRequestContext.Trace(508102, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "RemoveUser: Starting");
        Guid result;
        if (!Guid.TryParse(userId, out result))
          throw new ArgumentException(nameof (userId));
        Guid accountOwner = this.GetAccountOwner();
        if (result.Equals(accountOwner))
        {
          JsObject data = new JsObject();
          data["error"] = (object) UserManagementResources.RemoveAccountOwner;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        this.TfsRequestContext.Trace(508103, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "RemoveUser: Validate Request Permissions");
        if (!this.HasAllPermissions())
        {
          JsObject data = new JsObject();
          data["error"] = (object) UserManagementResources.UserPermissionsError;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
        IdentityService service1 = vssRequestContext.GetService<IdentityService>();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service1.ReadIdentities(vssRequestContext, (IList<Guid>) new Guid[1]
        {
          result
        }, QueryMembership.Direct, (IEnumerable<string>) null, true);
        Microsoft.VisualStudio.Services.Identity.Identity identity = identityList != null && identityList.Count != 0 && identityList[0] != null ? identityList[0] : throw new IdentityNotFoundException(result);
        if (IdentityDescriptorComparer.Instance.Equals(identity.Descriptor, this.TfsRequestContext.UserContext))
        {
          this.TfsRequestContext.Trace(508104, TraceLevel.Error, "HostingAccount", nameof (ApiUserManagementController), "RemoveUser was called on self. User context: {0}, Target user: {1}, {2}", (object) this.TfsRequestContext.UserContext.Identifier.ToString(), (object) identity.Descriptor.Identifier.ToString(), (object) result.ToString());
          JsObject data = new JsObject();
          data["error"] = (object) UserManagementResources.AccountOwnerCannotDeleteItself;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        AccountUser accountUser = new AccountUser(this.TfsRequestContext.ServiceHost.InstanceId, result);
        PlatformAccountMembershipService service2 = vssRequestContext.GetService<PlatformAccountMembershipService>();
        try
        {
          service2.RemoveUserFromAccount(vssRequestContext, accountUser);
          service1.RemoveMemberFromGroup(vssRequestContext, GroupWellKnownIdentityDescriptors.SecurityServiceGroup, identity.Descriptor);
          foreach (IdentityDescriptor groupDescriptor in (IEnumerable<IdentityDescriptor>) identity.MemberOf)
          {
            try
            {
              service1.RemoveMemberFromGroup(vssRequestContext, groupDescriptor, identity.Descriptor);
            }
            catch (Exception ex)
            {
              TeamFoundationTrace.TraceException(ex);
            }
          }
        }
        catch (AccountUserNotFoundException ex)
        {
          TeamFoundationTrace.Info(ex.Message);
        }
        return (ActionResult) this.Json((object) "success", JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(508108, "HostingAccount", nameof (ApiUserManagementController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.Trace(508109, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "RemoveUser: completed.");
      }
    }

    [HttpPost]
    [TfsTraceFilter(508301, 508310)]
    [TfsHandleFeatureFlag("WebAccess.UserhubExtensionManagement", null)]
    [ValidateAntiForgeryToken]
    public ActionResult RemoveUserFromExtension(
      List<string> userIds,
      string extensionId,
      string ciData = null)
    {
      try
      {
        this.TfsRequestContext.Trace(508302, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "RemoveUserFromExtension: Starting");
        this.CheckExtensionIdIsValid(extensionId);
        List<Guid> userIds1 = new List<Guid>();
        foreach (string userId in userIds)
        {
          Guid input = ApiUserManagementController.ParseInput(userId, "userId");
          userIds1.Add(input);
        }
        this.TfsRequestContext.Trace(508303, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "RemoveUserFromExtension: Validate Request Permissions");
        if (!this.HasAllPermissions())
        {
          JsObject data = new JsObject();
          data["error"] = (object) UserManagementResources.UserPermissionsError;
          return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
        }
        ICollection<ExtensionOperationResult> source = this.TfsRequestContext.GetService<IExtensionEntitlementService>().UnassignExtensionFromUsers(this.TfsRequestContext, extensionId, (IList<Guid>) userIds1, LicensingSource.Account);
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        AssignExtensionCiData ciData1 = ciData == null ? (AssignExtensionCiData) null : scriptSerializer.Deserialize<AssignExtensionCiData>(ciData);
        this.PublishAssignUsersToExtensionCiEvent(userIds1.Count, userIds1.Count - source.Count<ExtensionOperationResult>((Func<ExtensionOperationResult, bool>) (x => x.Result == OperationResult.Error)), extensionId, ciData1, "Unassign");
        ExtensionOperationResult extensionOperationResult = source.FirstOrDefault<ExtensionOperationResult>((Func<ExtensionOperationResult, bool>) (r => r.Result != 0));
        if (extensionOperationResult == null)
          return (ActionResult) this.Json((object) "success", JsonRequestBehavior.AllowGet);
        JsObject data1 = new JsObject();
        data1["error"] = (object) extensionOperationResult.Message;
        return (ActionResult) this.Json((object) data1, JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(508308, "HostingAccount", nameof (ApiUserManagementController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.Trace(508309, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "RemoveUserFromExtension: completed.");
      }
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(508151, 508160)]
    [TfsHandleFeatureFlag("WebAccess.UserManagementInSps", null)]
    [ValidateAntiForgeryToken]
    public ActionResult ApiMetrics(string featureName, string serializedData)
    {
      if (string.IsNullOrEmpty(featureName))
        throw new ArgumentException(nameof (featureName));
      try
      {
        this.TfsRequestContext.TraceEnter(508151, "HostingAccount", nameof (ApiUserManagementController), "LogMetrics: Starting");
        JsObject data1 = new JsObject();
        Dictionary<string, object> data2 = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(serializedData);
        if (data2 == null || data2.Count == 0)
        {
          data1["warning"] = (object) UserManagementResources.ApiMetricsInvalidInput;
          data1["result"] = (object) false;
          return (ActionResult) this.Json((object) data1, JsonRequestBehavior.AllowGet);
        }
        Dictionary<string, object> dictionary = new Dictionary<string, object>()
        {
          {
            "StartTime",
            (object) (!data2.ContainsKey("StartTime") || data2["StartTime"] == null ? new DateTime() : new DateTime(1970, 1, 1).AddMilliseconds((double) long.Parse(data2["StartTime"].ToString())))
          },
          {
            "ExecutionTime",
            (object) (!data2.ContainsKey("ExecutionTime") || data2["ExecutionTime"] == null ? 0L : long.Parse(data2["ExecutionTime"].ToString()))
          },
          {
            "MethodName",
            !data2.ContainsKey("MethodName") || data2["MethodName"] == null ? (object) string.Empty : (object) data2["MethodName"].ToString()
          },
          {
            "Input",
            !data2.ContainsKey("Input") || data2["Input"] == null ? (object) string.Empty : (object) data2["Input"].ToString()
          },
          {
            "QuerySize",
            (object) (!data2.ContainsKey("QuerySize") || data2["QuerySize"] == null ? 0L : long.Parse(data2["QuerySize"].ToString()))
          },
          {
            "Message",
            !data2.ContainsKey("Message") || data2["Message"] == null ? (object) string.Empty : (object) data2["Message"].ToString()
          }
        };
        CustomerIntelligenceData properties = new CustomerIntelligenceData((IDictionary<string, object>) data2);
        this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, "VisualStudio.Services.ApiMetrics", featureName, properties);
        data1["result"] = (object) true;
        return (ActionResult) this.Json((object) data1, JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(508158, "HostingAccount", nameof (ApiUserManagementController), ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(508159, "HostingAccount", nameof (ApiUserManagementController), "LogMetrics: completed.");
      }
    }

    private static void ValidateUserParameters(string userName, string licenseType)
    {
      if (string.IsNullOrEmpty(userName))
        throw new ArgumentException(nameof (userName));
      if (string.IsNullOrEmpty(licenseType))
        throw new ArgumentException(nameof (licenseType));
    }

    private JsObject AddUserToAccount(
      string tenantDisplayName,
      string userName,
      string licenseType,
      string displayName,
      string objectId)
    {
      ApiUserManagementController.ValidateUserParameters(userName, licenseType);
      JsObject account = new JsObject();
      if (!ArgumentUtility.IsValidEmailAddress(userName))
      {
        account["error"] = (object) UserManagementResources.UserHubInvalidEmail;
        return account;
      }
      this.TfsRequestContext.Trace(508113, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "AddUserToAccount: Check if user is admin");
      if (!ApiUserManagementController.HasPermission(this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment ? this.TfsRequestContext.To(TeamFoundationHostType.Application) : this.TfsRequestContext, 63, "/Entitlements/AccountEntitlements/"))
      {
        account["error"] = (object) UserManagementResources.UserPermissionsError;
        return account;
      }
      ILicensingEntitlementService service1 = this.TfsRequestContext.GetService<ILicensingEntitlementService>();
      IList<AccountLicenseUsage> licensesUsage = service1.GetLicensesUsage(this.TfsRequestContext);
      IList<AccountEntitlement> accountEntitlements = service1.GetAccountEntitlements(this.TfsRequestContext);
      License license = ApiUserManagementController.ParseLicense(licenseType);
      string errorMessage;
      if (!this.CheckLicenseAvailability(license, (IEnumerable<AccountLicenseUsage>) licensesUsage, Guid.Empty, out errorMessage))
      {
        account["error"] = (object) errorMessage;
        return account;
      }
      string domain = "Windows Live ID";
      Guid organizationAadTenantId = this.TfsRequestContext.GetOrganizationAadTenantId();
      if (!organizationAadTenantId.Equals(Guid.Empty))
        domain = organizationAadTenantId.ToString();
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      try
      {
        this.TfsRequestContext.Trace(508115, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "AddUserToAccount: Resolving userName: {0}, domain: {1}, displayName: {2}, objectId: {3}", (object) userName, (object) domain, (object) displayName, (object) objectId);
        identity = ApiUserManagementController.CheckName(this.TfsRequestContext, userName, domain, displayName, objectId);
        if (accountEntitlements.Where<AccountEntitlement>((Func<AccountEntitlement, bool>) (user => user.UserId.Equals(identity.MasterId))).Any<AccountEntitlement>())
          throw new IdentityAlreadyExistsException(string.Format(UserManagementResources.IdentityAlreadyExists, (object) userName));
        CustomerIntelligenceService service2 = this.TfsRequestContext.GetService<CustomerIntelligenceService>();
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.TfsRequestContext.GetUserIdentity();
        VisualStudioOnlineServiceLevel valueOrDefault = service1.GetAccountEntitlement(this.TfsRequestContext, userIdentity.MasterId)?.Rights?.Level.GetValueOrDefault();
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "userId",
            (object) userIdentity.Id
          },
          {
            nameof (licenseType),
            (object) licenseType
          },
          {
            "accountRights",
            (object) valueOrDefault
          }
        });
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string acquisition = CustomerIntelligenceArea.Acquisition;
        string addUserToAccount = CustomerIntelligenceFeature.AddUserToAccount;
        CustomerIntelligenceData properties = intelligenceData;
        service2.Publish(tfsRequestContext, acquisition, addUserToAccount, properties);
      }
      catch (IdentityAlreadyExistsException ex)
      {
        account["error"] = (object) ex.Message;
        return account;
      }
      this.TfsRequestContext.Trace(508116, TraceLevel.Verbose, "HostingAccount", nameof (ApiUserManagementController), "AddUserToAccount: Adding user {0} (Domain: {1}) to Account {2} ({3})", (object) identity.MasterId, (object) domain, (object) this.TfsRequestContext.ServiceHost.Name, (object) this.TfsRequestContext.ServiceHost.InstanceId);
      service1.AssignAccountEntitlement(this.TfsRequestContext, identity.MasterId, license);
      account["aad"] = (object) false;
      if (identity.IsExternalUser)
      {
        account["aad"] = (object) true;
        if (this.TfsRequestContext.IsFeatureEnabled("WebAccess.EnableAddAadUserWarning") && !string.IsNullOrEmpty(tenantDisplayName))
          account["aadTenantName"] = (object) tenantDisplayName;
      }
      return account;
    }

    private string GetTenantDisplayName()
    {
      if (!this.TfsRequestContext.IsOrganizationAadBacked())
        return string.Empty;
      try
      {
        IVssRequestContext context = this.TfsRequestContext.To(TeamFoundationHostType.Application);
        return context.GetService<AadService>().GetTenant(context, new GetTenantRequest()).Tenant.DisplayName;
      }
      catch (AadException ex)
      {
        this.TfsRequestContext.TraceException(508116, "HostingAccount", nameof (ApiUserManagementController), (Exception) ex);
        throw;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.Trace(508117, TraceLevel.Error, "HostingAccount", nameof (ApiUserManagementController), "something realy went wrong, AadService should not throw the Exception:{0}", (object) ex);
        throw;
      }
    }

    private static string GetLicenseType(
      License license,
      AccountUserStatus userStatus,
      Dictionary<string, int> licenseCount)
    {
      Dictionary<License, string> stringDict = ApiUserManagementController.LicenseToStringDict();
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
          status = !license.Source.Equals((object) LicensingSource.Msdn) || !((MsdnLicense) license).License.Equals((object) MsdnLicenseType.Eligible) ? string.Empty : UserManagementResources.UserStatusMsdnNotValidated;
          break;
        case 2:
          status = !license.Source.Equals((object) LicensingSource.Msdn) ? UserManagementResources.UserStatusExpired : (!((MsdnLicense) license).License.Equals((object) MsdnLicenseType.Eligible) ? UserManagementResources.UserStatusMSDNExpired : UserManagementResources.DisabledEligibleMSDN);
          break;
        case 3:
          status = string.Empty;
          break;
        case 4:
          status = !license.Source.Equals((object) LicensingSource.Msdn) || !((MsdnLicense) license).License.Equals((object) MsdnLicenseType.Eligible) ? string.Empty : UserManagementResources.UserStatusPending;
          break;
        case 5:
          status = !license.Source.Equals((object) LicensingSource.Msdn) ? UserManagementResources.UserStatusExpired : (!((MsdnLicense) license).License.Equals((object) MsdnLicenseType.Eligible) ? UserManagementResources.UserStatusMSDNExpired : UserManagementResources.DisabledEligibleMSDN);
          break;
        case 6:
          if (license.Source.Equals((object) LicensingSource.Msdn))
          {
            if (((MsdnLicense) license).License.Equals((object) MsdnLicenseType.Eligible))
            {
              status = UserManagementResources.DisabledEligibleMSDN;
              break;
            }
          }
          else if (License.None.Equals(license))
          {
            status = string.Empty;
            break;
          }
          status = UserManagementResources.UserStatusExpired;
          break;
      }
      return status;
    }

    private static string GetUserLastAccessedDate(
      DateTimeOffset lastAccessedDate,
      DateTimeOffset assignmentDate)
    {
      if (lastAccessedDate.Date != DateTime.MinValue)
        return lastAccessedDate.Date.ToString(CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.LCID).DateTimeFormat.ShortDatePattern);
      return assignmentDate < (DateTimeOffset) ApiUserManagementController.s_perUserLastAccessedDate ? string.Empty : UserManagementResources.UserGridNeverMessage;
    }

    private bool HasAllPermissions() => this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment ? ApiUserManagementController.HasPermission(this.TfsRequestContext.To(TeamFoundationHostType.Application), 63, "/Entitlements/AccountEntitlements/") : ApiUserManagementController.HasPermission(this.TfsRequestContext, 63, "/Entitlements/AccountEntitlements/");

    private void CheckPermission(
      IVssRequestContext requestContext,
      int requestedPermissions,
      string token = "/Entitlements/AccountEntitlements/")
    {
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        if (!ApiUserManagementController.HasPermission(requestContext.To(TeamFoundationHostType.Application), requestedPermissions, token))
          throw new InvalidAccessException(UserManagementResources.AccessDeniedNoPermission);
      }
      else
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
        if (!ApiUserManagementController.HasPermission(requestContext, requestedPermissions, token))
          throw new InvalidAccessException(UserManagementResources.AccessDeniedNoPermission);
      }
    }

    private static bool HasPermission(
      IVssRequestContext accountRequestContext,
      int requestedPermissions,
      string token)
    {
      return accountRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(accountRequestContext, ApiUserManagementController.s_licensingSecurityNamespaceId).Secured().HasPermission(accountRequestContext, token, requestedPermissions, false);
    }

    private List<LicenseModel> GetLicenseData(UserViewModel vm)
    {
      List<LicenseModel> licenseData = new List<LicenseModel>();
      Dictionary<string, LicenseModel> dictionary = new Dictionary<string, LicenseModel>();
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      List<IOfferSubscription> list = vssRequestContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptions(vssRequestContext, true).Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (m => m.OfferMeter.Category == MeterCategory.Legacy)).ToList<IOfferSubscription>();
      List<string> stringList = new List<string>();
      IOfferSubscription offerSubscription1 = list.Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (resource => resource.OfferMeter.Name == "StandardLicense")).First<IOfferSubscription>();
      IOfferSubscription offerSubscription2 = list.Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (resource => resource.OfferMeter.Name == "ProfessionalLicense")).FirstOrDefault<IOfferSubscription>();
      IOfferSubscription offerSubscription3 = list.Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (resource => resource.OfferMeter.Name == "AdvancedLicense")).FirstOrDefault<IOfferSubscription>();
      bool flag1 = false;
      int committedQuantity1 = offerSubscription2 != null ? offerSubscription2.CommittedQuantity : 0;
      int committedQuantity2 = offerSubscription3 != null ? offerSubscription3.CommittedQuantity : 0;
      IList<AccountLicenseUsage> licensesUsage = this.TfsRequestContext.GetService<ILicensingEntitlementService>().GetLicensesUsage(this.TfsRequestContext);
      bool flag2 = this.IsEarlyAdopterOrganization();
      foreach (AccountLicenseUsage accountLicenseUsage in (IEnumerable<AccountLicenseUsage>) licensesUsage)
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
          licenseModel.LicenseType = ApiUserManagementController.TransformText(accountLicenseUsage.License.License, out licenseEnum);
          licenseModel.LicenseEnum = licenseEnum;
          dictionary.Add(licenseEnum, licenseModel);
          licenseData.Add(licenseModel);
          switch (accountLicenseUsage.License.License)
          {
            case 1:
              licenseModel.IncludedQuantity = flag2 ? int.MaxValue : 0;
              licenseModel.Available = flag2 ? int.MaxValue : licenseModel.Available;
              licenseModel.Maximum = flag2 ? int.MaxValue : licenseModel.Maximum;
              continue;
            case 2:
              licenseModel.IncludedQuantity = offerSubscription1.IncludedQuantity;
              if (accountLicenseUsage.UsedCount > offerSubscription1.CommittedQuantity)
              {
                flag1 = true;
                continue;
              }
              continue;
            case 3:
              licenseModel.IncludedQuantity = offerSubscription2 != null ? offerSubscription2.IncludedQuantity : 0;
              if (accountLicenseUsage.UsedCount > committedQuantity1)
              {
                flag1 = true;
                continue;
              }
              continue;
            case 4:
              licenseModel.IncludedQuantity = offerSubscription3 != null ? offerSubscription3.IncludedQuantity : 0;
              if (accountLicenseUsage.UsedCount > committedQuantity2)
              {
                flag1 = true;
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      LicenseModel licenseModel1 = new LicenseModel()
      {
        LicenseType = UserManagementResources.AccountEligibleMSDNLicense,
        Available = int.MaxValue,
        LicenseEnum = MsdnLicense.Eligible.ToString()
      };
      licenseData.Add(licenseModel1);
      if (flag2)
      {
        LicenseModel licenseModel2 = new LicenseModel()
        {
          LicenseType = UserManagementResources.AccountEarlyAdopterLicense,
          Available = int.MaxValue,
          LicenseEnum = AccountLicense.EarlyAdopter.ToString(),
          IncludedQuantity = int.MaxValue,
          Maximum = int.MaxValue
        };
        licenseData.Clear();
        licenseData.Add(licenseModel2);
      }
      if (!flag2 & flag1)
      {
        stringList.Add(string.Format(UserManagementResources.BillingPeriodWarning, (object) offerSubscription1.ResetDate.ToString("MMMM d"), (object) offerSubscription1.CommittedQuantity, committedQuantity1 > 0 ? (object) string.Format(UserManagementResources.BillingPeriodWarningProfessionalLicenseCount, (object) committedQuantity1) : (object) string.Empty, committedQuantity2 > 0 ? (object) string.Format(UserManagementResources.BillingPeriodWarningAdvancedLicenseCount, (object) committedQuantity2) : (object) string.Empty));
        vm.LicenseErrors = stringList;
      }
      if (!flag2 && vm.LicenseOverview.ContainsKey(AccountLicense.EarlyAdopter.ToString()))
      {
        stringList.Add(string.Format(UserManagementResources.EarlyAdopterExpiredMessage));
        vm.LicenseErrors = stringList;
      }
      vm.licenseDictionary = dictionary;
      return licenseData;
    }

    private bool IsEarlyAdopterOrganization()
    {
      IVssRequestContext context = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      return context.GetService<IOrganizationPolicyService>().GetPolicy<bool>(context.Elevate(), "Policy.IsInternal", false).EffectiveValue;
    }

    private Guid GetAccountOwner()
    {
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      return this.TfsRequestContext.GetService<ICollectionService>().GetCollection(this.TfsRequestContext.Elevate(), (IEnumerable<string>) null).Owner;
    }

    private static bool IsInternal(Microsoft.VisualStudio.Services.Account.Account account)
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

    private static Microsoft.VisualStudio.Services.Identity.Identity CheckName(
      IVssRequestContext accountRequestContext,
      string name,
      string domain,
      string displayName,
      string objectId)
    {
      accountRequestContext.GetService<IdentityService>();
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
          identity = ApiUserManagementController.ResolveNewUser(accountRequestContext, name, domain, displayName, objectId);
      }
      catch (Exception ex)
      {
        identity = ApiUserManagementController.HandleMultipleIdentitiesFoundException(ex);
      }
      return identity;
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity ResolveNewUser(
      IVssRequestContext accountRequestContext,
      string newUser,
      string domain,
      string displayName,
      string objectId)
    {
      IVssRequestContext vssRequestContext = accountRequestContext.Elevate().To(TeamFoundationHostType.Deployment);
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      string str = string.IsNullOrEmpty(newUser) ? newUser : newUser.Trim();
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (ArgumentUtility.IsValidEmailAddress(str))
      {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(displayName))
          dictionary.Add("ProviderDisplayName", (object) displayName);
        if (!string.IsNullOrEmpty(objectId))
          dictionary.Add("http://schemas.microsoft.com/identity/claims/objectidentifier", (object) objectId);
        identity = IdentityHelper.GetOrCreateBindPendingIdentity(accountRequestContext, domain, str, properties: dictionary.Count > 0 ? (IDictionary<string, object>) dictionary : (IDictionary<string, object>) null, callerName: nameof (ResolveNewUser));
        if (identity.IsBindPending)
          service.UpdateIdentities(vssRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
          {
            identity
          });
      }
      return identity;
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity HandleMultipleIdentitiesFoundException(
      Exception ex)
    {
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    private bool CheckLicenseAvailability(
      License userLicense,
      IEnumerable<AccountLicenseUsage> licenseInfo,
      Guid userId,
      out string errorMessage)
    {
      AccountLicenseUsage accountLicenseUsage = (AccountLicenseUsage) null;
      switch (userLicense.Source)
      {
        case LicensingSource.Account:
          Dictionary<AccountLicenseType, string> errorDict = ApiUserManagementController.LicenseToErrorDict();
          AccountLicense accountLicense = (AccountLicense) userLicense;
          errorMessage = errorDict[accountLicense.License];
          if (accountLicense.License.Equals((object) AccountLicenseType.EarlyAdopter))
            return this.IsEarlyAdopterOrganization();
          accountLicenseUsage = licenseInfo.Where<AccountLicenseUsage>((Func<AccountLicenseUsage, bool>) (license => license.License.Source.Equals((object) LicensingSource.Account) && (AccountLicenseType) license.License.License == accountLicense.License)).First<AccountLicenseUsage>();
          if (userId != Guid.Empty && (License) accountLicense == (License) AccountLicense.Stakeholder && this.GetAccountOwner() == userId)
          {
            errorMessage = UserManagementResources.AccountOwnerCannotBeAssignedStakeholderLicense;
            return false;
          }
          break;
        case LicensingSource.Msdn:
          MsdnLicense msdnLicense = (MsdnLicense) userLicense;
          errorMessage = UserManagementResources.AccountUnknownLicense;
          return msdnLicense.License.Equals((object) MsdnLicenseType.Eligible);
        default:
          errorMessage = UserManagementResources.NoLicensesMessage;
          break;
      }
      if (accountLicenseUsage == null)
      {
        errorMessage = UserManagementResources.AccountUnknownLicense;
        return false;
      }
      return accountLicenseUsage.ProvisionedCount == -1 || accountLicenseUsage.ProvisionedCount - accountLicenseUsage.UsedCount > 0;
    }

    private static string TransformText(int licenseType, out string licenseEnum)
    {
      string str;
      switch (Enum.IsDefined(typeof (AccountLicenseType), (object) licenseType) ? licenseType : 0)
      {
        case 1:
          str = UserManagementResources.AccountEarlyAdopterLicense;
          licenseEnum = AccountLicense.EarlyAdopter.ToString();
          break;
        case 2:
          str = UserManagementResources.AccountStandardLicense;
          licenseEnum = AccountLicense.Express.ToString();
          break;
        case 3:
          str = UserManagementResources.AccountStandardProLicense;
          licenseEnum = AccountLicense.Professional.ToString();
          break;
        case 4:
          str = UserManagementResources.AccountAdvancedLicense;
          licenseEnum = AccountLicense.Advanced.ToString();
          break;
        case 5:
          str = UserManagementResources.AccountStakeholderLicense;
          licenseEnum = AccountLicense.Stakeholder.ToString();
          break;
        default:
          str = UserManagementResources.AccountNoLicense;
          licenseEnum = License.None.ToString();
          break;
      }
      return str;
    }

    private void PublishAssignUsersToExtensionCiEvent(
      int userCount,
      int successCount,
      string extensionId,
      AssignExtensionCiData ciData,
      string assignType)
    {
      try
      {
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
        CustomerIntelligenceService service = vssRequestContext.GetService<CustomerIntelligenceService>();
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "UserId",
            (object) vssRequestContext.GetUserId()
          },
          {
            "UserCUID",
            (object) vssRequestContext.GetUserCuid()
          },
          {
            "ExtensionId",
            (object) extensionId
          },
          {
            "AssignSource",
            ciData == null ? (object) (string) null : (object) ciData.AssignSource
          },
          {
            "AccountId",
            (object) vssRequestContext.ServiceHost.InstanceId
          },
          {
            "AvailableLicenses",
            (object) (ciData == null ? -1 : ciData.TotalLicenses - ciData.InUseLicenses)
          },
          {
            "NumberAssignedRequested",
            (object) userCount
          },
          {
            "NumberAssignedSuccess",
            (object) successCount
          },
          {
            "Type",
            (object) assignType
          }
        });
        IVssRequestContext requestContext = vssRequestContext;
        string licensing = CustomerIntelligenceArea.Licensing;
        CustomerIntelligenceData properties = intelligenceData;
        service.Publish(requestContext, licensing, "WebAccess.UserhubExtensionManagement", properties);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(505327, "HostingAccount", nameof (ApiUserManagementController), ex);
      }
    }

    private static Dictionary<License, string> LicenseToStringDict() => new Dictionary<License, string>()
    {
      {
        (License) AccountLicense.EarlyAdopter,
        UserManagementResources.AccountEarlyAdopterLicense
      },
      {
        (License) AccountLicense.Stakeholder,
        UserManagementResources.AccountStakeholderLicense
      },
      {
        (License) AccountLicense.Express,
        UserManagementResources.AccountStandardLicense
      },
      {
        (License) AccountLicense.Professional,
        UserManagementResources.AccountStandardProLicense
      },
      {
        (License) AccountLicense.Advanced,
        UserManagementResources.AccountAdvancedLicense
      },
      {
        (License) MsdnLicense.Eligible,
        UserManagementResources.AccountEligibleMSDNLicense
      },
      {
        (License) MsdnLicense.Professional,
        UserManagementResources.MSDNPro
      },
      {
        (License) MsdnLicense.Platforms,
        UserManagementResources.MsdnPlatforms
      },
      {
        (License) MsdnLicense.TestProfessional,
        UserManagementResources.MSDNTestPro
      },
      {
        (License) MsdnLicense.Premium,
        UserManagementResources.MSDNPremium
      },
      {
        (License) MsdnLicense.Ultimate,
        UserManagementResources.MsdnUltimate
      },
      {
        (License) MsdnLicense.Enterprise,
        UserManagementResources.MsdnEnterprise
      }
    };

    private static Dictionary<AccountLicenseType, string> LicenseToErrorDict() => new Dictionary<AccountLicenseType, string>()
    {
      {
        AccountLicenseType.EarlyAdopter,
        UserManagementResources.NoEarlyAdopterLicenses
      },
      {
        AccountLicenseType.Stakeholder,
        UserManagementResources.NoStakeholderLicenses
      },
      {
        AccountLicenseType.Express,
        UserManagementResources.NoStandardLicenses
      },
      {
        AccountLicenseType.Professional,
        UserManagementResources.NoStandardWithVSPro
      },
      {
        AccountLicenseType.Advanced,
        UserManagementResources.NoAdvancedLicenses
      }
    };

    internal static class ApiMetricsConstants
    {
      internal const string Area = "VisualStudio.Services.ApiMetrics";
      internal const string Layer = "MvcController";
    }
  }
}
