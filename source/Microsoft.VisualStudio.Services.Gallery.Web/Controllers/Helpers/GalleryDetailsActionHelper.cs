// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Helpers.GalleryDetailsActionHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConnected;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Commerce.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Remote;
using Microsoft.VisualStudio.Services.Gallery.Server.Remote.ExtensionManagement;
using Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Models;
using Microsoft.VisualStudio.Services.Gallery.Web.Models;
using Microsoft.VisualStudio.Services.Gallery.Web.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Helpers
{
  public class GalleryDetailsActionHelper : GalleryControllerHelper
  {
    private const string c_primaryInstance = "IsPrimaryInstance";
    private const string c_GalleryNewJourneyId = "Gallery-Service-NewJourneyId";
    private const string c_msdnIdentifier = "00000027-0000-8888-8000-000000000000";
    private const string c_LocationcacheKey = "location";
    private const string c_xamarinUniversityItemName = "ms.xamarin-university";
    private const string c_CallNewGetAccountsVersionAPI = "CallNewGetAccountsVersionAPI";

    public virtual ICommerceDataProvider CommerceDataProvider => this.Controller.CommerceDataProvider;

    public GalleryDetailsActionHelper(Microsoft.VisualStudio.Services.Gallery.Web.GalleryController controller)
      : base(controller)
    {
    }

    public virtual string GetOnPremToHostedRedirectURL(
      PublishedExtension extension,
      string itemName)
    {
      if (extension == null)
        return (string) null;
      string token = this.TfsRequestContext.GetService<IConnectedServerContextKeyService>().GetToken(this.TfsRequestContext, (Dictionary<string, string>) null);
      if (string.IsNullOrEmpty(token))
        return (string) null;
      return !this.OnPremServerHasInternetAccess(CloudConnectedUtilities.DecodeToken(token)) ? (string) null : string.Format("{0}items?itemName={1}&serverKey={2}", (object) this.GetMarketplaceProductionUrl(), (object) UriUtility.UrlEncode(itemName), (object) UriUtility.UrlEncode(token));
    }

    public void LoadExtensionVsixId(PublishedExtension extension)
    {
      if (extension == null || extension.Metadata == null)
        return;
      foreach (ExtensionMetadata extensionMetadata in extension.Metadata)
      {
        if (extensionMetadata.Key.Equals("VsixId"))
          this.ViewData["vsixId"] = (object) extensionMetadata.Value;
      }
    }

    public bool IsExtensionVsipPartner(PublishedExtension extension)
    {
      if (extension == null || extension.Metadata == null)
        return false;
      foreach (ExtensionMetadata extensionMetadata in extension.Metadata)
      {
        if (extensionMetadata.Key.Equals("Affiliation") && extensionMetadata.Value.Equals("VSIPPartner"))
          return true;
      }
      return false;
    }

    public void LoadExecutionEnvironmentData(
      PublishedExtension extension,
      string itemName,
      string installContext)
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        string onPremRedirectUrl = this.GetOnPremRedirectURL(itemName, extension);
        if (!string.IsNullOrEmpty(onPremRedirectUrl))
          this.ViewData["onprem-redirect-url"] = (object) onPremRedirectUrl;
        this.ViewData["user-login-url"] = (object) this.GetSignInRedirectUrl();
        this.ViewData["gallery-browse-url"] = (object) this.GetGalleryHostName();
      }
      else
      {
        this.LoadConnectedServerData(extension, itemName, installContext);
        this.ViewData["is-modal-install"] = (object) true;
        this.ViewData["marketplace-production-url"] = (object) this.GetMarketplaceProductionUrl();
      }
    }

    protected virtual bool ShowVSItemLink()
    {
      string name = nameof (ShowVSItemLink);
      bool result = false;
      HttpCookie cookie = this.Request.Cookies[name];
      if (cookie != null)
        bool.TryParse(cookie.Value, out result);
      if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.ShowVSItemLink"))
        result = true;
      return result;
    }

    public void LoadDefaultViewData(PublishedExtensionResult extensionResult)
    {
      this.ViewData["vss-extension"] = (object) GalleryServerUtil.SetEffectiveDisplayedStarRating(this.TfsRequestContext, new List<PublishedExtension>()
      {
        extensionResult.Extension
      })[0];
      this.ViewData["vss-extension-token"] = (object) extensionResult.ExtensionAssetsToken;
      this.ViewData["is-modal-install"] = (object) false;
      this.ViewData["needs-aad-auth"] = (object) false;
    }

    public void LoadCspData()
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.TfsRequestContext.GetUserIdentity();
      if (userIdentity == null)
        return;
      this.ViewData["is-csp-user"] = (object) userIdentity.IsCspPartnerUser;
    }

    public bool LoadMsdnData(PublishedExtension extension)
    {
      string str = (string) null;
      if (extension != null && GalleryUtil.IsVSSubsInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets) && this.CheckIfHostedDeployment())
      {
        LocationUrlMemoryCache service = this.TfsRequestContext.GetService<LocationUrlMemoryCache>();
        try
        {
          if (!service.TryGetValue(this.TfsRequestContext, "location", out str))
          {
            ServiceDefinition serviceDefinition1 = this.TfsRequestContext.GetService<ILocationService>().GetLocationData(this.TfsRequestContext, ServiceInstanceTypes.SPS).FindServiceDefinitions(this.TfsRequestContext, "LocationService2").Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (serviceDefinition =>
            {
              Guid parentIdentifier = serviceDefinition.ParentIdentifier;
              return string.Equals(serviceDefinition.ParentIdentifier.ToString(), "00000027-0000-8888-8000-000000000000", StringComparison.OrdinalIgnoreCase) && serviceDefinition.Properties.ContainsKey("IsPrimaryInstance") && string.Equals(serviceDefinition.Properties["IsPrimaryInstance"].ToString(), "true", StringComparison.OrdinalIgnoreCase);
            })).FirstOrDefault<ServiceDefinition>();
            if (serviceDefinition1 != null)
            {
              if (serviceDefinition1.LocationMappings != null)
              {
                IEnumerable<LocationMapping> source = serviceDefinition1.LocationMappings.Where<LocationMapping>((Func<LocationMapping, bool>) (locationMapping => string.Equals(locationMapping.AccessMappingMoniker, AccessMappingConstants.PublicAccessMappingMoniker)));
                if (source != null)
                {
                  if (source.Count<LocationMapping>() > 0)
                  {
                    LocationMapping locationMapping = source.FirstOrDefault<LocationMapping>();
                    str = locationMapping.Location;
                    service.Set(this.TfsRequestContext, "location", locationMapping.Location);
                  }
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          this.TfsRequestContext.TraceException(12061070, this.AreaName, this.LayerName, ex);
        }
        this.ViewData["msdn-url"] = (object) str;
      }
      return str != null;
    }

    public virtual GalleryDetailsActionHelper.AccountContext GetAccountsData(
      PublishedExtension extension,
      string accountId)
    {
      string fullyQualifiedName = extension.GetFullyQualifiedName();
      ISubscriptionService service = this.TfsRequestContext.GetService<ISubscriptionService>();
      IEnumerable<ISubscriptionAccount> source = (IEnumerable<ISubscriptionAccount>) null;
      SubscriptionHttpClient subscriptionHttpClient = (SubscriptionHttpClient) null;
      bool featureFlagStatus = this.GetCookieOrFeatureFlagStatus("CallNewGetAccountsVersionAPI", "Microsoft.VisualStudio.Services.Gallery.CallNewGetAccountsVersionAPI");
      try
      {
        if (featureFlagStatus)
        {
          subscriptionHttpClient = this.TfsRequestContext.To(TeamFoundationHostType.Deployment).GetClient<SubscriptionHttpClient>();
          source = (IEnumerable<ISubscriptionAccount>) subscriptionHttpClient.GetAccountsByIdentityForOfferIdV2Async(AccountProviderNamespace.VisualStudioOnline, this.TfsRequestContext.GetUserIdentity().Id, false, false, true, (IEnumerable<Guid>) new Guid[3]
          {
            ServiceInstanceTypes.TFS,
            ServiceInstanceTypes.SPS,
            CommerceServiceInstanceTypes.Commerce
          }, fullyQualifiedName, new bool?(false), new bool?(true)).SyncResult<List<SubscriptionAccount>>();
        }
        else
          source = service.GetAccounts(this.TfsRequestContext, AccountProviderNamespace.VisualStudioOnline, new Guid?(this.TfsRequestContext.GetUserIdentity().Id), false, includeMSAAccounts: true, serviceOwners: (IEnumerable<Guid>) new Guid[3]
          {
            ServiceInstanceTypes.TFS,
            ServiceInstanceTypes.SPS,
            CommerceServiceInstanceTypes.Commerce
          }, galleryId: fullyQualifiedName, queryAccountsByUpn: true);
      }
      catch (VssServiceResponseException ex)
      {
        this.TfsRequestContext.TraceException(12061129, this.AreaName, this.LayerName, (Exception) ex);
      }
      catch (VssServiceException ex)
      {
        this.TfsRequestContext.TraceException(12061129, this.AreaName, this.LayerName, (Exception) ex);
      }
      Dictionary<string, AcquisitionOptions> dictionary = new Dictionary<string, AcquisitionOptions>();
      ISubscriptionAccount subscriptionAccount1 = (ISubscriptionAccount) null;
      if (source != null && source.Count<ISubscriptionAccount>() > 0)
      {
        if (!extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public))
          source = source.Where<ISubscriptionAccount>((Func<ISubscriptionAccount, bool>) (account => extension.SharedWith != null && extension.SharedWith.Any<ExtensionShare>((Func<ExtensionShare, bool>) (shared => string.Equals(shared.Id, account.AccountId.ToString(), StringComparison.OrdinalIgnoreCase)))));
        source = (IEnumerable<ISubscriptionAccount>) source.OrderBy<ISubscriptionAccount, string>((Func<ISubscriptionAccount, string>) (account => account.AccountName), (IComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
      if (source != null && source.Count<ISubscriptionAccount>() > 0)
      {
        if (accountId != null)
        {
          foreach (ISubscriptionAccount subscriptionAccount2 in source)
          {
            if (string.Equals(subscriptionAccount2.AccountId.ToString(), accountId, StringComparison.OrdinalIgnoreCase))
            {
              subscriptionAccount1 = subscriptionAccount2;
              break;
            }
          }
        }
        if (subscriptionAccount1 == null)
        {
          if (!featureFlagStatus)
            subscriptionAccount1 = source.FirstOrDefault<ISubscriptionAccount>((Func<ISubscriptionAccount, bool>) (account => object.Equals((object) account.AccountTenantId, (object) this.GetAuthenticatedTenantId())));
          if (subscriptionAccount1 == null)
            subscriptionAccount1 = source.First<ISubscriptionAccount>();
        }
        if (featureFlagStatus)
        {
          string organizationId = string.Format("{0}", (object) subscriptionAccount1.AccountId, (object) CultureInfo.InvariantCulture);
          subscriptionAccount1.AccountTenantId = subscriptionHttpClient.GetOrganizationTenantIdAsync(organizationId).SyncResult<Guid>();
        }
      }
      return new GalleryDetailsActionHelper.AccountContext()
      {
        accounts = source,
        selectedAccount = subscriptionAccount1
      };
    }

    public virtual void LoadViewDataForInstall(PublishedExtension extension)
    {
      GalleryDetailsActionHelper.InstallDataContext installDataContext = this.BuildInstallContext(extension);
      if (extension != null && (extension.InstallationTargets == null || GalleryUtil.InstallationTargetsHasVSTS((IEnumerable<InstallationTarget>) extension.InstallationTargets)))
        installDataContext.Scopes = this.GetExtensionScopes(extension);
      this.ViewData["vss-item-scope"] = (object) installDataContext.Scopes;
      this.ViewData["vss-subscription-creation"] = (object) installDataContext.VSSSubscriptionCreationUrl;
      this.ViewData["vss-project-collections"] = (object) installDataContext.VSSProjectCollections;
      this.ViewData["vss-target-id"] = (object) installDataContext.VSSInstallTargetId;
    }

    public List<Scope> GetExtensionScopes(PublishedExtension publishedExtension)
    {
      List<Scope> extensionScopes = new List<Scope>();
      if (publishedExtension != null && publishedExtension.Versions != null)
      {
        string version = publishedExtension.Versions.First<ExtensionVersion>().Version;
        ExtensionManifest extensionManifest = this.LoadManifest(this.TfsRequestContext.Elevate(), publishedExtension, version);
        if (extensionManifest != null && extensionManifest.Scopes != null)
          extensionScopes = ((IEnumerable<AuthorizationScopeDefinition>) AuthorizationScopeDefinitions.Default.scopes).Where<AuthorizationScopeDefinition>((Func<AuthorizationScopeDefinition, bool>) (x => x.availability == AuthorizationScopeAvailability.Public && extensionManifest.Scopes.Contains<string>(x.scope, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))).Select<AuthorizationScopeDefinition, Scope>((Func<AuthorizationScopeDefinition, Scope>) (x => new Scope()
          {
            Description = x.description,
            Title = x.title,
            Value = x.scope
          })).OrderBy<Scope, string>((Func<Scope, string>) (x => x.Title)).ToList<Scope>();
      }
      return extensionScopes;
    }

    public void LoadExtensionProperties(PublishedExtension extension)
    {
      if (extension == null)
        return;
      IDictionary<string, string> extensionProperties = GalleryUtil.GetExtensionProperties(extension);
      if (extensionProperties == null)
        return;
      this.ViewData["vss-item-properties"] = (object) extensionProperties;
    }

    public void LoadIsMigratedProperty(PublishedExtension extension)
    {
      if (extension == null || !extension.IsVsExtension())
        return;
      this.ViewData["is-migrated"] = (object) extension.IsMigratedFromVSGallery();
    }

    public void LoadBadges(PublishedExtension extension)
    {
      if (extension == null)
        return;
      string version = extension.Versions.First<ExtensionVersion>().Version;
      string targetPlatform = extension.Versions.First<ExtensionVersion>().TargetPlatform;
      List<Badge> extensionBadges = GalleryServerUtil.GetExtensionBadges(this.TfsRequestContext.Elevate(), extension, version, Guid.Empty, targetPlatform);
      if (extensionBadges.Count == 0)
        return;
      this.ViewData["vss-item-badges"] = (object) extensionBadges;
    }

    public void LoadUserReviews(PublishedExtension extension)
    {
      if (this.IsSignedInContext())
      {
        Guid userId = this.TfsRequestContext.GetUserId();
        List<Review> reviewsByUserId = this.TfsRequestContext.GetService<IRatingAndReviewService>().GetReviewsByUserId(this.TfsRequestContext, userId, extension);
        if (reviewsByUserId != null && reviewsByUserId.Count > 0)
          this.ViewData["pinned-user-review"] = (object) reviewsByUserId[0];
        this.LoadCanUpdateExtension(extension);
      }
      this.LoadUserHasPublisherRoleReader(extension);
    }

    public void LoadCanUpdateExtension(PublishedExtension extension) => this.ViewData["can-update-extension"] = (object) GallerySecurity.HasExtensionPermission(this.TfsRequestContext, extension, (string) null, PublisherPermissions.UpdateExtension, false);

    public void LoadMiscellaneousItemData(PublishedExtensionResult extensionResult)
    {
      PublishedExtension extension = extensionResult.Extension;
      string version = extension.Versions.First<ExtensionVersion>().Version;
      this.ViewData["vss-item-overview"] = (object) this.GetAssetContent(extension.Publisher.PublisherName, extension.ExtensionName, version, "Microsoft.VisualStudio.Services.Content.Details", extensionResult.ExtensionAssetsToken);
      this.ViewData["vss-item-pricing"] = (object) this.GetAssetContent(extension.Publisher.PublisherName, extension.ExtensionName, version, "Microsoft.VisualStudio.Services.Content.Pricing", extensionResult.ExtensionAssetsToken);
      this.ViewData["vss-item-acquisition-customization"] = (object) this.GetAssetContent(extension.Publisher.PublisherName, extension.ExtensionName, version, "Microsoft.VisualStudio.Services.Content.AcquisitionCustomization", extensionResult.ExtensionAssetsToken);
    }

    public void LoadCommerceData(
      PublishedExtension extension,
      string itemName,
      bool isDetailEndpoint = true)
    {
      if (extension == null || !this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment || !extension.IsPaid())
        return;
      IOfferMeter offerDetails = this.CommerceDataProvider.GetOfferDetails(this.TfsRequestContext, itemName);
      this.ViewData["vss-extension-offer"] = (object) this.GetExtensionOfferDetails(offerDetails);
      if (!isDetailEndpoint || offerDetails == null)
        return;
      IEnumerable<IOfferMeterPrice> offerMeterPrice = this.CommerceDataProvider.GetOfferMeterPrice(this.TfsRequestContext.Elevate(), itemName);
      this.ViewData["vss-extension-offer-meter-price"] = (object) this.FilterOfferMeterPricing(this.TfsRequestContext, offerMeterPrice);
      this.LoadCurrencyInfo(offerMeterPrice);
    }

    public void GetPresentTargetPlatformsPairs(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      this.ViewData["target-platforms"] = (object) GalleryServerUtil.GetAllVSCodeTargetPlatformPairs(requestContext);
    }

    public void LoadWorksWithString(PublishedExtension extension)
    {
      if (extension == null)
        return;
      IEnumerable<string> worksWithString = new DetailsPageWorksWithStringComposer().GetWorksWithString(this.TfsRequestContext, extension);
      if (worksWithString == null || worksWithString.Count<string>() <= 0)
        return;
      this.ViewData["worksWith"] = (object) worksWithString;
    }

    public AcquisitionOptions GetDefaultOptionsAndAccountDetails(
      PublishedExtension extension,
      ref ISubscriptionAccount selectedAccount)
    {
      string journeyId = this.GetJourneyId();
      this.TfsRequestContext.TraceEnter(12062048, this.AreaName, this.LayerName, "GetDefaultOptions");
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      IVssRequestContext vssRequestContext = (IVssRequestContext) null;
      AcquisitionOptions andAccountDetails = (AcquisitionOptions) null;
      string fullyQualifiedName = extension.GetFullyQualifiedName();
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity1 = this.TfsRequestContext.GetUserIdentity();
        if (selectedAccount.AccountTenantId == Guid.Empty)
        {
          Microsoft.VisualStudio.Services.Identity.Identity primaryMsaIdentity = this.GetPrimaryMsaIdentity(this.TfsRequestContext, userIdentity1);
          if (primaryMsaIdentity != null)
          {
            this.TfsRequestContext.Trace(12062048, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("fetchDefaultAcquisitionOption account = {0}, msaId = {1}, currentId= {2}, journeyId={3}", (object) selectedAccount.AccountId, (object) primaryMsaIdentity.Id, (object) userIdentity1.Id, (object) journeyId));
            vssRequestContext = this.TfsRequestContext.CreateUserContext(primaryMsaIdentity.Descriptor);
          }
          else
          {
            this.TfsRequestContext.Trace(12062048, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("fetchDefaultAcquisitionOption account = {0}, currentId= {1}, journeyId={2}", (object) selectedAccount.AccountId, (object) userIdentity1.Id, (object) journeyId));
            vssRequestContext = this.TfsRequestContext;
          }
        }
        else
        {
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity2 = this.TfsRequestContext.GetUserIdentity();
          string property1 = userIdentity2.GetProperty<string>("Domain", string.Empty);
          Guid result1;
          if (Guid.TryParse(property1, out result1) && result1 == selectedAccount.AccountTenantId)
          {
            this.TfsRequestContext.Trace(12062048, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("fetchDefaultAcquisitionOption account = {0}, accountTenant = {1}, currentId= {2} , currentTenant = {3}, journeyId={4}", (object) selectedAccount.AccountId, (object) selectedAccount.AccountTenantId, (object) userIdentity1.Id, (object) property1, (object) journeyId));
            vssRequestContext = this.TfsRequestContext;
          }
          else
          {
            this.TfsRequestContext.Trace(12062048, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("fetchDefaultAcquisitionOption account = {0}, accountTenant = {1}, currentId= {2} , currentTenant = {3}, journeyId={4}", (object) selectedAccount.AccountId, (object) selectedAccount.AccountTenantId, (object) userIdentity1.Id, (object) property1, (object) journeyId));
            string property2 = userIdentity2?.GetProperty<string>("Mail", string.Empty);
            IList<Microsoft.VisualStudio.Services.Identity.Identity> source = service.ReadIdentities(this.TfsRequestContext, IdentitySearchFilter.MailAddress, property2, QueryMembership.None, (IEnumerable<string>) null);
            if (source != null)
            {
              this.TfsRequestContext.Trace(12062048, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("fetchDefaultAcquisitionOption account = {0}, accountTenant = {1}, identitiesCount = {2}, journeyId={3}", (object) selectedAccount.AccountId, (object) selectedAccount.AccountTenantId, (object) userIdentity1.Id, (object) source.Count<Microsoft.VisualStudio.Services.Identity.Identity>(), (object) journeyId));
              for (int index = 0; index < source.Count<Microsoft.VisualStudio.Services.Identity.Identity>(); ++index)
              {
                string property3 = source[index].GetProperty<string>("Domain", string.Empty);
                this.TfsRequestContext.Trace(12062048, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("fetchDefaultAcquisitionOption account = {0}, accountTenant = {1}, Id= {2} , userTenant = {3}, journeyId={4}", (object) selectedAccount.AccountId, (object) selectedAccount.AccountTenantId, (object) source[index].Id, (object) property3, (object) journeyId));
                Guid result2;
                if (!string.Equals("Windows Live ID", property3, StringComparison.InvariantCulture) && Guid.TryParse(property3, out result2) && result2 == selectedAccount.AccountTenantId)
                {
                  vssRequestContext = this.TfsRequestContext.CreateUserContext(source[index].Descriptor);
                  break;
                }
              }
            }
          }
        }
        if (vssRequestContext != null)
        {
          if (this.GetCookieOrFeatureFlagStatus("CallNewGetAccountsVersionAPI", "Microsoft.VisualStudio.Services.Gallery.CallNewGetAccountsVersionAPI"))
          {
            SubscriptionHttpClient client = this.TfsRequestContext.To(TeamFoundationHostType.Deployment).GetClient<SubscriptionHttpClient>();
            selectedAccount = (ISubscriptionAccount) client.GetAccountDetailsAsync(selectedAccount.AccountId, (IEnumerable<Guid>) new Guid[3]
            {
              ServiceInstanceTypes.TFS,
              ServiceInstanceTypes.SPS,
              CommerceServiceInstanceTypes.Commerce
            }).SyncResult<SubscriptionAccount>();
          }
          IRemoteServiceClientFactory serviceClientFactory = this.getRemoteServiceClientFactory();
          Stopwatch stopwatch = new Stopwatch();
          stopwatch.Start();
          IVssRequestContext requestContext = vssRequestContext;
          Guid accountId = selectedAccount.AccountId;
          Guid serviceOwner = ExtensionConstants.ServiceOwner;
          IExtensionManagementClient emsClient = serviceClientFactory.GetEMSClient(requestContext, accountId, serviceOwner);
          int num1;
          if (selectedAccount.SubscriptionId.HasValue)
          {
            Guid? subscriptionId = selectedAccount.SubscriptionId;
            Guid empty = Guid.Empty;
            num1 = subscriptionId.HasValue ? (subscriptionId.HasValue ? (subscriptionId.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1;
          }
          else
            num1 = 0;
          bool flag = num1 != 0;
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          string itemName = fullyQualifiedName;
          int num2 = selectedAccount.IsAccountOwner ? 1 : 0;
          int num3 = flag ? 1 : 0;
          andAccountDetails = emsClient.GetAcquisitionOptionsSync(tfsRequestContext, itemName, num2 != 0, num3 != 0);
          this.TfsRequestContext.Trace(12062048, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("fetchDefaultAcquisitionOption account = {0}, options = {1}, journeyId={2}", (object) selectedAccount.AccountId, (object) andAccountDetails.ToString(), (object) journeyId));
          stopwatch.Stop();
          this.PublishGetDefaultAcquisitionTimeEvent(fullyQualifiedName, stopwatch.ElapsedMilliseconds);
        }
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(12062048, this.AreaName, this.LayerName, ex);
        andAccountDetails = (AcquisitionOptions) null;
      }
      this.TfsRequestContext.TraceLeave(12062048, this.AreaName, this.LayerName, "GetDefaultOptions");
      return andAccountDetails;
    }

    public void LoadUserHasPublisherRoleReader(PublishedExtension extension)
    {
      if (extension == null || !this.IsSignedInContext())
      {
        this.ViewData["has-publisher-role-reader"] = (object) false;
      }
      else
      {
        IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
        this.ViewData["has-publisher-role-reader"] = (object) GallerySecurity.HasPublisherPermission(this.TfsRequestContext, vssRequestContext.GetService<IPublisherService>().QueryPublisher(vssRequestContext, extension.Publisher.PublisherName, PublisherQueryFlags.None), PublisherPermissions.Read | PublisherPermissions.PrivateRead | PublisherPermissions.ViewPermissions);
      }
    }

    protected virtual ExtensionManifest LoadManifest(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string version)
    {
      IPublisherAssetService service = requestContext.GetService<IPublisherAssetService>();
      AssetInfo[] assetTypes = new AssetInfo[1]
      {
        new AssetInfo("Microsoft.VisualStudio.Services.Manifest", (string) null)
      };
      ExtensionFile assetFile = service.QueryAsset(requestContext, extension.Publisher.PublisherName, extension.ExtensionName, version, Guid.Empty, (IEnumerable<AssetInfo>) assetTypes, (string) null, (string) null, true).AssetFile;
      if (assetFile == null)
        return (ExtensionManifest) null;
      using (Stream manifestStream = requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(this.TfsRequestContext, (long) assetFile.FileId, false, out byte[] _, out long _, out CompressionType _))
        return ExtensionUtil.LoadManifest(extension.Publisher.PublisherName, extension.ExtensionName, version, manifestStream, (IDictionary<string, object>) ExtensionUtil.GetExtensionProperties(extension.Flags));
    }

    private string GetJourneyId()
    {
      string journeyId = (string) null;
      HttpCookie cookie = this.Request.Cookies["Gallery-Service-NewJourneyId"];
      if (cookie != null)
        journeyId = cookie.Value;
      return journeyId;
    }

    internal virtual IRemoteServiceClientFactory getRemoteServiceClientFactory() => (IRemoteServiceClientFactory) new RemoteServiceClientFactory();

    private void PublishGetDefaultAcquisitionTimeEvent(string itemName, long elapsedTime)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("eventType", "GetAcquisitionoption");
      properties.Add(nameof (itemName), itemName);
      properties.Add("ElapsedInMs", (double) elapsedTime);
      this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, "Microsoft.VisualStudio.Services.Gallery", "AcquisitionPagePerformance", properties);
    }

    private IEnumerable<RegionInfo> GetRegionInfoList()
    {
      List<RegionInfo> regionInfoList = new List<RegionInfo>();
      CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
      if (cultures != null)
      {
        foreach (CultureInfo cultureInfo in cultures)
        {
          try
          {
            RegionInfo regionInfo = new RegionInfo(cultureInfo.LCID);
            regionInfoList.Add(regionInfo);
          }
          catch (Exception ex)
          {
            this.TfsRequestContext.TraceException(12062001, this.AreaName, this.LayerName, ex);
          }
        }
      }
      return (IEnumerable<RegionInfo>) regionInfoList;
    }

    private OfferMeterEntry PopulateOfferMeterEntry(
      string currencyCode,
      string regionCode,
      List<MeterPrice> meterPrices,
      IEnumerable<RegionInfo> regions)
    {
      RegionInfo regionInfo = regions.Where<RegionInfo>((Func<RegionInfo, bool>) (reg => string.Equals(reg.ISOCurrencySymbol, currencyCode, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<RegionInfo>();
      if (regionInfo == null)
      {
        this.TfsRequestContext.TraceAlways(12062001, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("Info not found for Currency = {0} , Region = {1}", (object) currencyCode, (object) regionCode));
        regionInfo = new RegionInfo(regionCode);
      }
      List<MeterPrice> source = new List<MeterPrice>();
      source.AddRange((IEnumerable<MeterPrice>) meterPrices);
      List<MeterPrice> list = source.GroupBy<MeterPrice, double>((Func<MeterPrice, double>) (meterPrice => meterPrice.Quantity)).Select<IGrouping<double, MeterPrice>, MeterPrice>((Func<IGrouping<double, MeterPrice>, MeterPrice>) (meterPrice => meterPrice.First<MeterPrice>())).ToList<MeterPrice>();
      return new OfferMeterEntry()
      {
        CurrencyCode = currencyCode,
        Region = regionCode,
        CurrencyName = regionInfo.CurrencyEnglishName,
        CurrencySymbol = regionInfo.CurrencySymbol,
        MeterPrices = list
      };
    }

    private void LoadCurrencyInfo(IEnumerable<IOfferMeterPrice> offerMeterPrices)
    {
      if (offerMeterPrices == null || offerMeterPrices.Count<IOfferMeterPrice>() <= 0)
        return;
      IEnumerable<RegionInfo> regionInfoList = this.GetRegionInfoList();
      List<IOfferMeterPrice> list = offerMeterPrices.ToList<IOfferMeterPrice>();
      list.Sort((Comparison<IOfferMeterPrice>) ((x, y) =>
      {
        int num = string.Compare(x.CurrencyCode, y.CurrencyCode, StringComparison.OrdinalIgnoreCase);
        if (num != 0)
          return num;
        return x.Quantity == y.Quantity ? (x.Price > y.Price ? -1 : 1) : (x.Quantity > y.Quantity ? 1 : -1);
      }));
      List<OfferMeterEntry> offerMeterEntryList = new List<OfferMeterEntry>();
      string currencyCode = list[0].CurrencyCode;
      string region = list[0].Region;
      List<MeterPrice> meterPrices = new List<MeterPrice>()
      {
        new MeterPrice()
        {
          Price = list[0].Price,
          Quantity = list[0].Quantity
        }
      };
      for (int index = 1; index < list.Count; ++index)
      {
        MeterPrice meterPrice = new MeterPrice()
        {
          Price = list[index].Price,
          Quantity = list[index].Quantity
        };
        if (string.Compare(currencyCode, list[index].CurrencyCode, StringComparison.OrdinalIgnoreCase) == 0)
        {
          meterPrices.Add(meterPrice);
        }
        else
        {
          offerMeterEntryList.Add(this.PopulateOfferMeterEntry(currencyCode, region, meterPrices, regionInfoList));
          meterPrices.Clear();
          meterPrices.Add(meterPrice);
          currencyCode = list[index].CurrencyCode;
          region = list[index].Region;
        }
      }
      offerMeterEntryList.Add(this.PopulateOfferMeterEntry(currencyCode, region, meterPrices, regionInfoList));
      this.ViewData["vss-extension-offer-meter-price-currency"] = (object) offerMeterEntryList;
    }

    private IEnumerable<IOfferMeterPrice> FilterOfferMeterPricing(
      IVssRequestContext requestContext,
      IEnumerable<IOfferMeterPrice> offerMeterPricing)
    {
      bool flag = requestContext.UserContext != (IdentityDescriptor) null;
      if (offerMeterPricing == null)
        return offerMeterPricing;
      List<IOfferMeterPrice> list1 = offerMeterPricing.ToList<IOfferMeterPrice>();
      string str = string.Empty;
      if (flag)
        str = this.GetPreferredLanguageForAuthenticatedUser(requestContext);
      if (string.IsNullOrEmpty(str) && this.HttpContext.Request.UserLanguages != null && this.HttpContext.Request.UserLanguages.Length != 0)
        str = this.HttpContext.Request.UserLanguages[0];
      if (!string.IsNullOrEmpty(str))
      {
        requestContext.Trace(12062001, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("FilterOfferMeterPricing preferredUserLanguage = {0}", (object) str));
        try
        {
          CultureInfo preferredCulture;
          TeamFoundationApplicationCore.GetPreferredCulture(new string[1]
          {
            str
          }, (ISet<int>) null, (ISet<int>) null, out preferredCulture);
          if (preferredCulture != null)
          {
            RegionInfo regionInfo = new RegionInfo(preferredCulture.LCID);
            string isoCurrencySymbol = regionInfo.ISOCurrencySymbol;
            requestContext.Trace(12062001, TraceLevel.Info, this.AreaName, this.LayerName, string.Format("FilterOfferMeterPricing region code = {0}", (object) regionInfo.TwoLetterISORegionName));
            if (!string.IsNullOrEmpty(isoCurrencySymbol))
            {
              List<IOfferMeterPrice> list2 = list1.Where<IOfferMeterPrice>((Func<IOfferMeterPrice, bool>) (offerMeterPrice => string.Equals(offerMeterPrice.Region, regionInfo.TwoLetterISORegionName, StringComparison.OrdinalIgnoreCase))).ToList<IOfferMeterPrice>();
              if (list2.Count > 0)
                return (IEnumerable<IOfferMeterPrice>) list2;
            }
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12062001, this.AreaName, this.LayerName, ex);
        }
      }
      IEnumerable<IOfferMeterPrice> source = list1.Where<IOfferMeterPrice>((Func<IOfferMeterPrice, bool>) (offerMeterPrice => string.Equals(offerMeterPrice.Region, "US", StringComparison.OrdinalIgnoreCase)));
      return source.Count<IOfferMeterPrice>() > 0 ? source : (IEnumerable<IOfferMeterPrice>) list1;
    }

    private string GetPreferredLanguageForAuthenticatedUser(IVssRequestContext requestContext)
    {
      string authenticatedUser = "";
      try
      {
        UserAttribute attribute = this.TfsRequestContext.Elevate().GetService<IUserService>().GetAttribute(requestContext, requestContext.UserSubjectDescriptor(), WellKnownUserAttributeNames.TFSCulture);
        if (attribute != null)
          authenticatedUser = attribute.Value;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062001, this.AreaName, this.LayerName, ex);
      }
      requestContext.Trace(12062001, TraceLevel.Info, this.AreaName, this.LayerName, "GetPreferredLanguageForAuthenticatedUser preferredUserLanguage = {0}", (object) authenticatedUser);
      return authenticatedUser;
    }

    private ExtensionOfferDetails GetExtensionOfferDetails(IOfferMeter offerMeter)
    {
      ExtensionOfferDetails extensionOfferDetails;
      if (offerMeter == null)
        extensionOfferDetails = new ExtensionOfferDetails()
        {
          HasAssociatedOffer = false,
          HasPublicPlans = false,
          HasPlans = false,
          IsAccountBased = false,
          IncludedQuantity = 0
        };
      else if (offerMeter.FixedQuantityPlans == null)
      {
        extensionOfferDetails = new ExtensionOfferDetails()
        {
          HasAssociatedOffer = true,
          HasPublicPlans = false,
          HasPlans = false,
          IsAccountBased = offerMeter.AssignmentModel == OfferMeterAssignmentModel.Implicit,
          IncludedQuantity = offerMeter.IncludedQuantity
        };
      }
      else
      {
        extensionOfferDetails = new ExtensionOfferDetails()
        {
          HasAssociatedOffer = true,
          HasPlans = true,
          IsAccountBased = offerMeter.AssignmentModel == OfferMeterAssignmentModel.Implicit,
          IncludedQuantity = offerMeter.IncludedQuantity
        };
        IEnumerable<AzureOfferPlanDefinition> fixedQuantityPlans = offerMeter.FixedQuantityPlans;
        extensionOfferDetails.HasPublicPlans = fixedQuantityPlans.Any<AzureOfferPlanDefinition>((Func<AzureOfferPlanDefinition, bool>) (plan => plan.IsPublic));
      }
      return extensionOfferDetails;
    }

    private string GetAppId(PublishedExtension extension) => !string.Equals("ms.xamarin-university", extension.GetFullyQualifiedName(), StringComparison.OrdinalIgnoreCase) ? (!GalleryUtil.IsVSSubsInstallationTarget((IEnumerable<InstallationTarget>) extension.InstallationTargets) ? "VSTS" : "VisualStudioSub") : "Xamarin";

    private GalleryDetailsActionHelper.InstallDataContext BuildInstallContext(
      PublishedExtension extension)
    {
      GalleryDetailsActionHelper.InstallDataContext installDataContext = new GalleryDetailsActionHelper.InstallDataContext();
      string str = this.TfsRequestContext.GetService<IVssRegistryService>().GetValue(this.TfsRequestContext, (RegistryQuery) "/Configuration/SubscriptionCreation/Url", "https://account.windowsazure.com/signup?offer=MS-AZR-0003P&appId=101");
      NameValueCollection queryString = HttpUtility.ParseQueryString(new Uri(str).Query);
      if (extension != null)
      {
        string appId = this.GetAppId(extension);
        queryString["appId"] = appId;
      }
      UriBuilder uriBuilder = new UriBuilder(str)
      {
        Query = queryString.ToString()
      };
      installDataContext.VSSSubscriptionCreationUrl = uriBuilder.ToString();
      if (!this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        ICollectionProvider extension1 = this.TfsRequestContext.GetExtension<ICollectionProvider>(ExtensionLifetime.Service);
        if (extension1 != null)
        {
          IEnumerable<GalleryProjectCollection> collections = extension1.GetCollections(this.WebContext);
          if (collections != null)
            installDataContext.VSSProjectCollections = collections.Where<GalleryProjectCollection>((Func<GalleryProjectCollection, bool>) (host => host.Status == TeamFoundationServiceHostStatus.Started)).Select<GalleryProjectCollection, GalleryDetailsActionHelper.GalleryServiceHost>((Func<GalleryProjectCollection, GalleryDetailsActionHelper.GalleryServiceHost>) (host => new GalleryDetailsActionHelper.GalleryServiceHost()
            {
              Id = host.Id,
              Name = host.Name,
              Uri = Uri.EscapeUriString(host.Uri)
            })).OrderBy<GalleryDetailsActionHelper.GalleryServiceHost, string>((Func<GalleryDetailsActionHelper.GalleryServiceHost, string>) (host => host.Name), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).ToList<GalleryDetailsActionHelper.GalleryServiceHost>();
        }
      }
      installDataContext.VSSInstallTargetId = (string) null;
      HttpCookie cookie = this.Request.Cookies["targetId"];
      if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
        installDataContext.VSSInstallTargetId = cookie.Value;
      return installDataContext;
    }

    private Guid QueuePublishJob(string publisherName, string extensionName)
    {
      ITeamFoundationJobService service = this.TfsRequestContext.GetService<ITeamFoundationJobService>();
      PublishExtensionJobData objectToSerialize = new PublishExtensionJobData()
      {
        PublisherName = publisherName,
        ExtensionName = extensionName,
        OnlyUpdateForNewVersion = true,
        BlockPreviewToPaidUpgrade = false
      };
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize);
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Publish extension : {0}.{1}", (object) objectToSerialize.PublisherName, (object) objectToSerialize.ExtensionName);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string jobName = str;
      XmlNode jobData = xml;
      return service.QueueOneTimeJob(tfsRequestContext, jobName, "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.PublishExtensionJob", jobData, true);
    }

    private void LoadConnectedServerData(
      PublishedExtension extension,
      string itemName,
      string installContext)
    {
      if ((!(this.TfsRequestContext.UserContext != (IdentityDescriptor) null) ? 0 : (installContext != null ? 1 : 0)) != 0)
      {
        ExtensionIdentifier extensionIdentifier = new ExtensionIdentifier(itemName);
        Dictionary<string, string> dictionary = CloudConnectedUtilities.DecodeToken(UriUtility.UrlDecode(installContext));
        this.ViewData["vss-install-context"] = (object) dictionary.Serialize<Dictionary<string, string>>();
        if (dictionary.ContainsKey(CloudConnectedServerConstants.CollectionId))
        {
          this.SetTargetIdCookie(dictionary[CloudConnectedServerConstants.CollectionId]);
          this.ViewData["vss-target-id"] = (object) dictionary[CloudConnectedServerConstants.CollectionId];
        }
        this.ViewData["vss-publish-jobid"] = (object) this.QueuePublishJob(extensionIdentifier.PublisherName, extensionIdentifier.ExtensionName);
        if (extension == null)
          this.LoadViewDataForInstall(extension);
      }
      if (!this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      string token = this.TfsRequestContext.GetService<IConnectedServerContextKeyService>().GetToken(this.TfsRequestContext, (Dictionary<string, string>) null);
      if (!string.IsNullOrEmpty(token))
        this.ViewData["is-connected-server"] = (object) this.OnPremServerHasInternetAccess(CloudConnectedUtilities.DecodeToken(token));
      this.ViewData["market-browse-url"] = (object) (this.GetMarketBrowseURL() + "&serverKey=" + token);
    }

    private bool IsSignedInContext() => this.TfsRequestContext.UserContext != (IdentityDescriptor) null;

    public class AccountContext
    {
      public IEnumerable<ISubscriptionAccount> accounts;
      public ISubscriptionAccount selectedAccount;
    }

    [DataContract]
    private class InstallDataContext
    {
      [DataMember(Name = "scopes")]
      public List<Scope> Scopes { get; set; }

      [DataMember(Name = "subscriptionurl")]
      public string VSSSubscriptionCreationUrl { get; set; }

      [DataMember(Name = "collections")]
      public List<GalleryDetailsActionHelper.GalleryServiceHost> VSSProjectCollections { get; set; }

      [DataMember(Name = "targetId")]
      public string VSSInstallTargetId { get; set; }
    }

    [DataContract]
    private class GalleryServiceHost
    {
      [DataMember(Name = "id")]
      public Guid Id { get; set; }

      [DataMember(Name = "name")]
      public string Name { get; set; }

      [DataMember(Name = "uri")]
      public string Uri { get; set; }
    }
  }
}
