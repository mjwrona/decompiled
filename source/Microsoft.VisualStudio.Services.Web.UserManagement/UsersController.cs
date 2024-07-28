// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Web.UserManagement.UsersController
// Assembly: Microsoft.VisualStudio.Services.Web.UserManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BDE82F4-5081-4A92-A83F-EE78FF05B171
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Web.UserManagement.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.CloudConnected;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Microsoft.VisualStudio.Services.Web.UserManagement
{
  [SupportedRouteArea(NavigationContextLevels.Application | NavigationContextLevels.Collection)]
  public class UsersController : UserManagementAreaController
  {
    private static Guid s_licensingSecurityNamespaceId = new Guid("453E2DB3-2E81-474F-874D-3BF51027F2EE");
    private const string s_accountEntitlementsToken = "/Entitlements/AccountEntitlements/";
    private const int s_allPermissions = 63;
    private const string s_area = "HostingAccount";
    private const string s_layer = "SigninController";
    private static readonly Regex NoSessionPattern = new Regex("(\\?nosession=1$)|(&nosession=1)|(nosession=1&)", RegexOptions.IgnoreCase);
    private const string basePath = "VisualStudio.LicensingAuthorization.UI";
    private const string s_enableUserhubAadFeatures = "VisualStudio.LicensingAuthorization.UI.EnableUserhubAadFeatures";
    private const string s_UserCardSubFeature = "VisualStudio.LicensingAuthorization.UI.EnableUserhubAadFeatures.UserCard";
    private const string s_userhubExtensionManagementFeature = "WebAccess.UserhubExtensionManagement";
    private const string s_userhubNavHeaderPath = "_admin/_userHub";
    private const string s_userhubContentPath = "/_users";
    private const string disableCommerceFeatureFlag = "VisualStudio.Services.Commerce.DisableOnPremCommerce";

    [AcceptVerbs(HttpVerbs.Get)]
    [ActionName("Index")]
    public ActionResult Index(bool synchronizeCommerceData = false)
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment && this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableOnPremCommerce"))
        return (ActionResult) this.HttpNotFound();
      bool flag1 = false;
      Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
      SecurityModel securityModel1 = new SecurityModel();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      try
      {
        this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        if (this.TfsRequestContext.UserContext == (IdentityDescriptor) null)
        {
          this.GetTfsUserhubUri(this.TfsRequestContext);
          string redirectLocation = this.TfsRequestContext.GetService<ITeamFoundationAuthenticationService>().GetSignInRedirectLocation(this.TfsRequestContext, true);
          // ISSUE: reference to a compiler-generated field
          if (UsersController.\u003C\u003Eo__0.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UsersController.\u003C\u003Eo__0.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ActionUrl", typeof (UsersController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj1 = UsersController.\u003C\u003Eo__0.\u003C\u003Ep__0.Target((CallSite) UsersController.\u003C\u003Eo__0.\u003C\u003Ep__0, this.ViewBag, redirectLocation);
          // ISSUE: reference to a compiler-generated field
          if (UsersController.\u003C\u003Eo__0.\u003C\u003Ep__1 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UsersController.\u003C\u003Eo__0.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, bool, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "AuthenticationFail", typeof (UsersController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj2 = UsersController.\u003C\u003Eo__0.\u003C\u003Ep__1.Target((CallSite) UsersController.\u003C\u003Eo__0.\u003C\u003Ep__1, this.ViewBag, true);
          return (ActionResult) this.View(nameof (Index));
        }
        // ISSUE: reference to a compiler-generated field
        if (UsersController.\u003C\u003Eo__0.\u003C\u003Ep__2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          UsersController.\u003C\u003Eo__0.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, bool, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "AuthenticationFail", typeof (UsersController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj3 = UsersController.\u003C\u003Eo__0.\u003C\u003Ep__2.Target((CallSite) UsersController.\u003C\u003Eo__0.\u003C\u003Ep__2, this.ViewBag, false);
        // ISSUE: reference to a compiler-generated field
        if (UsersController.\u003C\u003Eo__0.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          UsersController.\u003C\u003Eo__0.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, bool, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "HideHeader", typeof (UsersController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj4 = UsersController.\u003C\u003Eo__0.\u003C\u003Ep__3.Target((CallSite) UsersController.\u003C\u003Eo__0.\u003C\u003Ep__3, this.ViewBag, true);
        IVssRequestContext vssRequestContext1 = this.TfsRequestContext.Elevate().To(TeamFoundationHostType.Deployment);
        IVssRequestContext vssRequestContext2 = this.TfsRequestContext.To(TeamFoundationHostType.Application);
        bool flag2;
        if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          if (vssRequestContext2.IsOrganizationAadBacked())
          {
            flag1 = true;
            if (vssRequestContext2.IsFeatureEnabled("VisualStudio.LicensingAuthorization.UI.EnableUserhubAadFeatures"))
              dictionary["UserhubAad"] = true;
            if (vssRequestContext2.IsFeatureEnabled("VisualStudio.LicensingAuthorization.UI.EnableUserhubAadFeatures.UserCard"))
              dictionary["UserhubAad.UserCard"] = true;
          }
          flag2 = this.HasPermission(vssRequestContext2, 63, "/Entitlements/AccountEntitlements/");
          SecurityModel securityModel2 = securityModel1;
          Guid? masterId = this.TfsRequestContext.GetUserIdentity()?.MasterId;
          Guid accountOwner = this.GetAccountOwner();
          int num = masterId.HasValue ? (masterId.HasValue ? (masterId.GetValueOrDefault() == accountOwner ? 1 : 0) : 1) : 0;
          securityModel2.isAccountOwner = num != 0;
          this.ViewData["AccountName"] = (object) this.GetAccountName();
          this.ViewData["AccountUri"] = (object) this.GetAccountUri();
          IOfferSubscription offerSubscription = this.TfsRequestContext.GetService<IOfferSubscriptionService>().GetOfferSubscription(this.TfsRequestContext.Elevate(), "StandardLicense");
          securityModel1.displayManageLink = false;
          ResourceStatusReason disabledReason = offerSubscription.DisabledReason;
          if (!disabledReason.Equals((object) ResourceStatusReason.None))
          {
            disabledReason = offerSubscription.DisabledReason;
            if (!disabledReason.Equals((object) ResourceStatusReason.SubscriptionDisabled))
              goto label_29;
          }
          if (offerSubscription.DisabledResourceActionLink != (Uri) null)
          {
            securityModel1.displayManageLink = true;
            securityModel1.licenseLink = "<a href=" + offerSubscription.DisabledResourceActionLink.ToString() + " target=_blank> " + UserManagementResources.BuyVstsUsers + " </a>";
            DateTime dateTime1 = offerSubscription.ResetDate;
            dateTime1 = dateTime1.AddMonths(-1);
            string str1 = dateTime1.ToString("MMM d");
            DateTime dateTime2 = offerSubscription.ResetDate;
            dateTime2 = dateTime2.AddDays(-1.0);
            string str2 = dateTime2.ToString("MMM d");
            securityModel1.monthlyCycle = str1 + " - " + str2;
            securityModel1.noLicenseMessageLinked = "";
          }
        }
        else
        {
          if (!this.TfsRequestContext.GetService<ICloudConnectedService>().GetConnectedAccountId(this.TfsRequestContext).Equals(Guid.Empty) && !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableOnPremCommerce"))
            this.TfsRequestContext.GetService<ICommercePackageSynchronizationService>().SynchronizeCommerceData(this.TfsRequestContext);
          flag2 = this.TfsRequestContext.GetService<IdentityService>().IsMember(this.TfsRequestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, this.TfsRequestContext.UserContext);
          flag1 = true;
          if (vssRequestContext2.IsFeatureEnabled("VisualStudio.LicensingAuthorization.UI.EnableUserhubAadFeatures"))
            dictionary["UserhubAad"] = true;
          if (vssRequestContext2.IsFeatureEnabled("VisualStudio.LicensingAuthorization.UI.EnableUserhubAadFeatures.UserCard"))
            dictionary["UserhubAad.UserCard"] = true;
        }
label_29:
        if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          string str = vssRequestContext1.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext1, this.TfsRequestContext.ServiceHost.InstanceId, ServiceInstanceTypes.TFS).ToString();
          if (!str.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            str += "/";
          return (ActionResult) this.Redirect(string.Format("{0}{1}", (object) str, (object) "_user"));
        }
        if (this.Request.UrlReferrer == (Uri) null)
          return (ActionResult) this.Redirect(this.TfsRequestContext.GetService<ILocationService>().GetLocationServiceUrl(this.TfsRequestContext, ServiceInstanceTypes.TFS, AccessMappingConstants.PublicAccessMappingMoniker) + "_admin/_userHub");
        this.SetXFrameOptions(this.TfsRequestContext.RequestUri().AbsoluteUri);
        securityModel1.isAdmin = flag2;
        securityModel1.isAadAccount = flag1;
        securityModel1.FeatureFlags = dictionary;
        if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment & flag1)
        {
          ICommerceAccountHandler extension = vssRequestContext1.GetExtension<ICommerceAccountHandler>();
          securityModel1.tenantName = extension.GetTenantName(vssRequestContext1);
        }
        this.ViewData["PermissionsData"] = (object) new JavaScriptSerializer().Serialize((object) securityModel1.ToJson());
        if (!vssRequestContext2.IsFeatureEnabled("WebAccess.UserhubExtensionManagement"))
          return (ActionResult) this.View(nameof (Index));
        // ISSUE: reference to a compiler-generated field
        if (UsersController.\u003C\u003Eo__0.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          UsersController.\u003C\u003Eo__0.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "NextResetDate", typeof (UsersController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj5 = UsersController.\u003C\u003Eo__0.\u003C\u003Ep__4.Target((CallSite) UsersController.\u003C\u003Eo__0.\u003C\u003Ep__4, this.ViewBag, string.Empty);
        if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          AzureResourceAccount accountByCollectionId = vssRequestContext1.GetService<PlatformSubscriptionService>().GetAzureResourceAccountByCollectionId(vssRequestContext1, this.TfsRequestContext.ServiceHost.InstanceId);
          if (accountByCollectionId != null)
          {
            // ISSUE: reference to a compiler-generated field
            if (UsersController.\u003C\u003Eo__0.\u003C\u003Ep__5 == null)
            {
              // ISSUE: reference to a compiler-generated field
              UsersController.\u003C\u003Eo__0.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "NextResetDate", typeof (UsersController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj6 = UsersController.\u003C\u003Eo__0.\u003C\u003Ep__5.Target((CallSite) UsersController.\u003C\u003Eo__0.\u003C\u003Ep__5, this.ViewBag, this.CalculateNextResetDate(DateTime.UtcNow, accountByCollectionId.Created).ToString("o"));
          }
        }
        return (ActionResult) this.View("IndexWithExtension");
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(504109, "HostingAccount", "SigninController", ex);
        throw;
      }
    }

    internal DateTime CalculateNextResetDate(DateTime now, DateTime seedTime)
    {
      DateTime dateTime = new DateTime(now.Year, now.Month, 1, seedTime.Hour, seedTime.Minute, seedTime.Second);
      return now.Day == 1 && now < dateTime ? dateTime : dateTime.AddMonths(1);
    }

    private Guid GetAccountOwner()
    {
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      return this.TfsRequestContext.GetService<ICollectionService>().GetCollection(this.TfsRequestContext.Elevate(), (IEnumerable<string>) null).Owner;
    }

    private string GetAccountName()
    {
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      return this.TfsRequestContext.GetService<ICollectionService>().GetCollection(this.TfsRequestContext.Elevate(), (IEnumerable<string>) null).Name;
    }

    private string GetAccountUri()
    {
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, this.TfsRequestContext.ServiceHost.InstanceId).AbsoluteUri;
    }

    private bool HasPermission(
      IVssRequestContext accountRequestContext,
      int requestedPermissions,
      string token)
    {
      return accountRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(accountRequestContext, UsersController.s_licensingSecurityNamespaceId).HasPermission(accountRequestContext, token, requestedPermissions, false);
    }

    private UriBuilder GetTfsUserhubUri(IVssRequestContext tfsRequestContext)
    {
      IVssRequestContext vssRequestContext = tfsRequestContext.Elevate().To(TeamFoundationHostType.Deployment);
      string accountUrl = vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, tfsRequestContext.ServiceHost.InstanceId, ServiceInstanceTypes.TFS).ToString();
      this.SetXFrameOptions(accountUrl);
      if (!accountUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
        accountUrl += "/";
      // ISSUE: reference to a compiler-generated field
      if (UsersController.\u003C\u003Eo__6.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        UsersController.\u003C\u003Eo__6.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "TfsAccountUrl", typeof (UsersController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = UsersController.\u003C\u003Eo__6.\u003C\u003Ep__0.Target((CallSite) UsersController.\u003C\u003Eo__6.\u003C\u003Ep__0, this.ViewBag, accountUrl);
      return new UriBuilder(accountUrl + "_user");
    }

    private void SetXFrameOptions(string accountUrl)
    {
      if (!string.IsNullOrWhiteSpace(this.HttpContext.Response.Headers["X-FRAME-OPTIONS"]))
        this.HttpContext.Response.Headers.Remove("X-FRAME-OPTIONS");
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        this.HttpContext.Response.Headers.Add("X-FRAME-OPTIONS", "allow-from " + accountUrl);
        this.HttpContext.Response.Headers.Add("Content-Security-Policy", "frame-src " + accountUrl);
      }
      else
        this.HttpContext.Response.Headers.Add("Content-Security-Policy", "frame-src self");
    }
  }
}
