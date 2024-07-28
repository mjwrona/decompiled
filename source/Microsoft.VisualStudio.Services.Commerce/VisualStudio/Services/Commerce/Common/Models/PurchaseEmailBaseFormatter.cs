// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.Models.PurchaseEmailBaseFormatter
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Common.Extensions;
using Microsoft.VisualStudio.Services.Commerce.Common.Models.EmailFormatters;
using Microsoft.VisualStudio.Services.EmailNotification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce.Common.Models
{
  public abstract class PurchaseEmailBaseFormatter : CommerceEmailBaseFormatter
  {
    internal const string MeterNameResolveMethod = "MeterName";
    protected const string Layer = "PurchaseEmailBaseFormatter";
    protected const string Area = "Commerce";
    protected const string defaultLocaleCode = "en-us";

    public PurchaseEmailBaseFormatter()
    {
      this.HeaderType = EmailTemplateHeaderType.ServiceNotification;
      this.FooterType = EmailTemplateFooterType.ServiceNotification;
      this.IsHtml = true;
    }

    public abstract void SetEmailBody(ResourceRenewalGroup renewalGroup);

    public abstract void SetEmailSubject();

    public abstract void SetEmailAttributes(
      IVssRequestContext collectionContext,
      string offerMeterName,
      string galleryId,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      string offerCode,
      DateTime renewalDate,
      Guid? tenantId,
      Guid? objectId);

    protected string GetSubscriptionName(
      IVssRequestContext collectionContext,
      Guid azureSubscriptionId)
    {
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      AzureSubscriptionInfo subscriptionForUser = vssRequestContext.GetService<IAzureResourceHelper>().GetAzureSubscriptionForUser(vssRequestContext, azureSubscriptionId);
      return subscriptionForUser == null || string.IsNullOrWhiteSpace(subscriptionForUser.DisplayName) || !(subscriptionForUser.DisplayName != azureSubscriptionId.ToString()) ? azureSubscriptionId.ToString() : subscriptionForUser.DisplayName + " (" + azureSubscriptionId.ToString() + ")";
    }

    protected OfferMeter GetOfferMeter(IVssRequestContext collectionContext, string offerMeterName)
    {
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      return (OfferMeter) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, offerMeterName);
    }

    internal string GetPrice(
      IVssRequestContext collectionContext,
      string offerMeterName,
      Guid azureSubscriptionId,
      int quantity,
      string offerCode,
      Guid? tenantId,
      Guid? objectId)
    {
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      PurchasableOfferMeter purchasableOfferMeter = vssRequestContext.GetService<IOfferMeterService>().GetPurchasableOfferMeter(vssRequestContext, offerMeterName, "MeterName", new Guid?(azureSubscriptionId), true, tenantId, objectId);
      if (purchasableOfferMeter.MeterPricing.IsNullOrEmpty<KeyValuePair<double, double>>())
      {
        string message = HostingResources.PricingDataNotAvailable();
        vssRequestContext.Trace(5108447, TraceLevel.Info, "Commerce", nameof (PurchaseEmailBaseFormatter), message);
        return message;
      }
      string currencySymbol = purchasableOfferMeter.GetCurrencySymbol();
      if (purchasableOfferMeter.OfferMeterDefinition.IsFirstParty)
        return this.ComputeGraduatedPrice(purchasableOfferMeter.MeterPricing, quantity, currencySymbol);
      try
      {
        double price = purchasableOfferMeter.MeterPricing.Single<KeyValuePair<double, double>>((Func<KeyValuePair<double, double>, bool>) (x => object.Equals((object) Convert.ToDouble(quantity), (object) x.Key))).Value;
        return PurchaseEmailBaseFormatter.FormatPrice(currencySymbol, price);
      }
      catch (InvalidOperationException ex)
      {
        vssRequestContext.Trace(5108447, TraceLevel.Error, "Commerce", nameof (PurchaseEmailBaseFormatter), string.Format("Quantity {0} not found in pricing for offer meter: {1}.", (object) quantity, (object) offerMeterName));
        return HostingResources.PricingDataNotAvailable();
      }
    }

    internal string ComputeGraduatedPrice(
      IEnumerable<KeyValuePair<double, double>> offerMeter,
      int quantity,
      string currencySymbol)
    {
      KeyValuePair<double, double>[] array = offerMeter.ToArray<KeyValuePair<double, double>>();
      double num = 0.0;
      int index;
      for (index = 1; index < array.Length && array[index].Key <= (double) quantity; ++index)
        num += (array[index].Key - array[index - 1].Key) * array[index - 1].Value;
      double price = num + ((double) quantity - array[index - 1].Key) * array[index - 1].Value;
      return PurchaseEmailBaseFormatter.FormatPrice(currencySymbol, price);
    }

    internal static string FormatPrice(string currencySymbol, double price) => currencySymbol + string.Format("{0:0.00}", (object) price);

    protected string DisplayName
    {
      set => this.Attributes[nameof (DisplayName)] = value;
    }

    protected string SubscriptionName
    {
      set => this.Attributes[nameof (SubscriptionName)] = value;
    }

    protected string ExtensionName
    {
      set
      {
        string str;
        if (OfferMeterFriendlyName.FriendlyNameMap.TryGetValue(value, out str))
          this.Attributes[nameof (ExtensionName)] = str;
        else
          this.Attributes[nameof (ExtensionName)] = value;
      }
    }

    protected string Quantity
    {
      set => this.Attributes[nameof (Quantity)] = value;
    }

    protected string Price
    {
      set => this.Attributes[nameof (Price)] = value;
    }

    protected string AccountName
    {
      set => this.Attributes[nameof (AccountName)] = value;
    }

    public override string EmailType() => "VSOPurchase";
  }
}
