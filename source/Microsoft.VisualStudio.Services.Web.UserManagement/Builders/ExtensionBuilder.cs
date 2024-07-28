// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Web.UserManagement.Builders.ExtensionBuilder
// Assembly: Microsoft.VisualStudio.Services.Web.UserManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BDE82F4-5081-4A92-A83F-EE78FF05B171
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Web.UserManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConnected;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Licensing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Web.UserManagement.Builders
{
  public class ExtensionBuilder
  {
    private IVssRequestContext tfsRequestContext;
    private static string extensionStatusPreview = UserManagementResources.ExtensionPreviewState;
    private static string extensionStatusTrial = UserManagementResources.ExtensionTrialState;
    private static string extensionStatusTrialWithBuy = UserManagementResources.ExtensionTrialWithBuyState;
    private static readonly string s_marketplaceUrlFormatDefault = "https://marketplace.visualstudio.com/";
    private static readonly string s_marketplaceCategUrlHostedQuery = "hosting=cloud&sortBy=Downloads";
    private static readonly string s_marketplaceCategUrlOnPremQuery = "hosting=onpremises&sortBy=Downloads";
    private static readonly string s_marketplaceCategUrlPath = "/vsts/All%20categories";
    private static readonly string s_marketplaceUrlRegistryPath = "/Configuration/Service/Gallery/MarketplaceURL";
    private string marketplaceUrl;

    private string MarketplaceUrl
    {
      get
      {
        if (this.marketplaceUrl == null)
          this.marketplaceUrl = ExtensionBuilder.GetMarketplaceUrl(this.tfsRequestContext).ToString();
        return this.marketplaceUrl;
      }
    }

    public ExtensionBuilder(IVssRequestContext TfsRequestContext) => this.tfsRequestContext = TfsRequestContext;

    public IEnumerable<ExtensionViewModel> GetExtensions(
      IEnumerable<IOfferSubscription> offerSubscriptions)
    {
      List<ExtensionViewModel> list = offerSubscriptions.Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (o =>
      {
        if (o.OfferMeter.Category != MeterCategory.Extension || o.OfferMeter.IncludedInLicenseLevel != MinimumRequiredServiceLevel.None || o.IsPreview && o.IsTrialOrPreview)
          return false;
        return o.CommittedQuantity > 0 || this.IsThereAnyUserHaveExtension(o.OfferMeter.GalleryId) || o.IsTrialOrPreview;
      })).Select<IOfferSubscription, ExtensionViewModel>((Func<IOfferSubscription, ExtensionViewModel>) (o => this.GetExtensionViewModelFromOfferSubscription(o, this.tfsRequestContext))).ToList<ExtensionViewModel>();
      return (IEnumerable<ExtensionViewModel>) list.Concat<ExtensionViewModel>((IEnumerable<ExtensionViewModel>) this.GetAdditionalOnPremisesInstalledPaidExtensions(new HashSet<string>(list.Select<ExtensionViewModel, string>((Func<ExtensionViewModel, string>) (ext => ext.ExtensionId))))).ToList<ExtensionViewModel>();
    }

    public List<ExtensionViewModel> GetAdditionalOnPremisesInstalledPaidExtensions(
      HashSet<string> extensionIds)
    {
      if (!this.tfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return new List<ExtensionViewModel>();
      DateTime billingStartDate = DateTime.Now.AddYears(-1);
      return this.tfsRequestContext.GetExtension<IOnPremiseOfflineExtensionHandler>().GetAllInstalledPaidFirstPartyExtensions(this.tfsRequestContext).Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (ext => !extensionIds.Contains<string>(ext.Key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))).Select<KeyValuePair<string, string>, ExtensionViewModel>((Func<KeyValuePair<string, string>, ExtensionViewModel>) (ext => ExtensionBuilder.GetExtensionViewModelFromOfflineDate(ext.Key, ext.Value, new DateTime?(billingStartDate)))).ToList<ExtensionViewModel>();
    }

    internal static ExtensionViewModel GetExtensionViewModelFromOfflineDate(
      string extensionId,
      string displayName,
      DateTime? billingStartDate)
    {
      return new ExtensionViewModel()
      {
        ExtensionId = extensionId,
        DisplayName = displayName,
        IsFirstParty = true,
        BillingStartDate = new DateTime?(billingStartDate ?? DateTime.Now.AddYears(-1)),
        IsEarlyAdopter = false,
        AllOrNothing = false,
        ExtensionState = string.Empty,
        IsTrialExpiredWithNoPurchase = false,
        IsPurchaseCanceled = false,
        TrialPeriod = 0,
        GracePeriod = 0,
        IncludedQuantity = 0
      };
    }

    private bool IsPurchaseCanceledAndTrialExpired(IOfferSubscription subscription) => subscription.IsPurchaseCanceled && !subscription.IsTrialOrPreview;

    internal ExtensionViewModel GetExtensionViewModelFromOfferSubscription(
      IOfferSubscription subscription,
      IVssRequestContext requestContext)
    {
      bool flag1 = false;
      bool flag2 = false;
      string str = "";
      bool flag3 = false;
      ExtensionViewModel offerSubscription = new ExtensionViewModel();
      offerSubscription.ExtensionId = subscription.OfferMeter.GalleryId;
      offerSubscription.DisplayName = subscription.OfferMeter.Name;
      offerSubscription.IsFirstParty = subscription.OfferMeter.IsFirstParty;
      DateTime? nullable;
      if (subscription.OfferMeter.BillingStartDate.HasValue)
      {
        if (!subscription.IsTrialOrPreview)
        {
          offerSubscription.BillingStartDate = subscription.OfferMeter.BillingStartDate;
        }
        else
        {
          offerSubscription.BillingStartDate = subscription.TrialExpiryDate;
          DateTime? startDate = subscription.StartDate;
          if (startDate.HasValue)
          {
            startDate = subscription.StartDate;
            nullable = subscription.OfferMeter.BillingStartDate;
            if ((startDate.HasValue & nullable.HasValue ? (startDate.GetValueOrDefault() < nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
              flag2 = true;
          }
        }
      }
      else
        flag2 = true;
      if (OfferMeterAssignmentModel.Implicit.Equals((object) subscription.OfferMeter.AssignmentModel))
        flag1 = true;
      DateTime utcNow = DateTime.UtcNow;
      if (subscription.IsPreview)
        str = ExtensionBuilder.extensionStatusPreview;
      else if (subscription.IsTrialOrPreview && !subscription.IsPreview)
      {
        str = !subscription.IsPurchasedDuringTrial ? ExtensionBuilder.extensionStatusTrial : ExtensionBuilder.extensionStatusTrialWithBuy;
        if (!subscription.IsPaidBillingEnabled)
        {
          DateTime dateTime = utcNow;
          nullable = subscription.TrialExpiryDate;
          if ((nullable.HasValue ? (dateTime > nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
            flag3 = true;
        }
      }
      GetExtensionUrlsViewModel extensionUrlsViewModel = new GetExtensionUrlsViewModel()
      {
        MarketPlaceUrlWithServerKey = this.MarketplaceUrl
      };
      offerSubscription.ExtensionUrlsViewModel = extensionUrlsViewModel;
      offerSubscription.IsTrialExpiredWithNoPurchase = flag3;
      offerSubscription.IsPurchaseCanceled = subscription.IsPurchaseCanceled;
      offerSubscription.ExtensionState = str;
      offerSubscription.AllOrNothing = flag1;
      nullable = subscription.TrialExpiryDate;
      if (nullable.HasValue)
      {
        IVssDateTimeProvider defaultProvider = VssDateTimeProvider.DefaultProvider;
        ExtensionViewModel extensionViewModel = offerSubscription;
        nullable = subscription.TrialExpiryDate;
        int num;
        if ((nullable.Value - defaultProvider.UtcNow).TotalDays <= 0.0)
        {
          num = 0;
        }
        else
        {
          nullable = subscription.TrialExpiryDate;
          num = Convert.ToInt32((nullable.Value - defaultProvider.UtcNow).TotalDays);
        }
        extensionViewModel.TrialPeriod = num;
      }
      offerSubscription.GracePeriod = (int) subscription.OfferMeter.PreviewGraceDays;
      offerSubscription.IsEarlyAdopter = flag2;
      offerSubscription.ExtensionState = str;
      offerSubscription.IncludedQuantity = subscription.IncludedQuantity;
      return offerSubscription;
    }

    public bool IsThereAnyUserHaveExtension(string extensionId)
    {
      IExtensionEntitlementService platformExtensionEntitlementService = this.tfsRequestContext.GetService<IExtensionEntitlementService>();
      return this.tfsRequestContext.ExecutionEnvironment.IsHostedDeployment ? platformExtensionEntitlementService.GetExtensionStatusForUsers(this.tfsRequestContext, extensionId).Any<KeyValuePair<Guid, ExtensionAssignmentDetails>>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, bool>) (x => x.Value.AssignmentStatus != ExtensionAssignmentStatus.NotAssigned && x.Value.AssignmentStatus != 0)) : this.tfsRequestContext.GetExtension<ILegacyLicensingHandler>().GetLicensesForAllActiveUsers(this.tfsRequestContext, false).Select<KeyValuePair<Guid, License>, Guid>((Func<KeyValuePair<Guid, License>, Guid>) (u => u.Key)).ToList<Guid>().Any<Guid>((Func<Guid, bool>) (u => platformExtensionEntitlementService.GetExtensionsAssignedToUser(this.tfsRequestContext, u).Keys.Contains(extensionId)));
    }

    public static Uri GetMarketplaceUrl(IVssRequestContext requestContext)
    {
      UriBuilder uriBuilder = new UriBuilder(requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) ExtensionBuilder.s_marketplaceUrlRegistryPath, ExtensionBuilder.s_marketplaceUrlFormatDefault));
      uriBuilder.Path = ExtensionBuilder.s_marketplaceCategUrlPath;
      string str = ExtensionBuilder.s_marketplaceCategUrlHostedQuery;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        string token = requestContext.GetService<IConnectedServerContextKeyService>().GetToken(requestContext, (Dictionary<string, string>) null);
        str = ExtensionBuilder.s_marketplaceCategUrlOnPremQuery + "&serverKey=" + token;
      }
      uriBuilder.Query = str;
      return uriBuilder.Uri;
    }
  }
}
