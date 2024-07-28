// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AssignmentBillingHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class AssignmentBillingHelper
  {
    private const string PreviewSubsriptionsRegistryPath = "/Service/Commerce/AssignmentBilling/Subscription/";
    private const string PreviewDailySubsriptionsRegistryPath = "/Service/Commerce/AssignmentBilling/Daily/Subscription/";
    private const string MaxAccountsCountRegistryPath = "/Service/Commerce/AssignmentBilling/MaxAccountsCount";
    private const string LicensingBatchSizeRegistryPath = "/Service/Commerce/AssignmentBilling/Licensing/BatchSize";
    private const string LicensingMaxQuantityRegistryPath = "/Service/Commerce/AssignmentBilling/Licensing/MaxQuantity";
    private const int LicensingBatchSizeDefault = 5000;
    private const int LicensingMaxQuantityDefault = 100000;
    private const int MaxAccountsCountDefault = 1000;

    public static IEnumerable<Guid> GetPreviewSubscriptions(
      IVssRequestContext requestContext,
      DateTime today)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Service/Commerce/AssignmentBilling/Subscription/*").Where<RegistryEntry>((Func<RegistryEntry, bool>) (x => DateTime.Parse(x.Value) < today)).Select<RegistryEntry, Guid>((Func<RegistryEntry, Guid>) (x => Guid.Parse(x.Name)));
    }

    public static bool IsAssignmentBillingEnabledForMeter(
      IVssRequestContext requestContext,
      Guid platformMeterId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IOfferMeterService service = vssRequestContext.GetService<IOfferMeterService>();
      return platformMeterId == service.GetOfferMeter(vssRequestContext, "ms.vss-vstsuser").PlatformMeterId || requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableAssignmentBillingForAdvanced") && platformMeterId == service.GetOfferMeter(vssRequestContext, "AdvancedLicense").PlatformMeterId || requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableAssignmentBillingExtensions") && (platformMeterId == service.GetOfferMeter(vssRequestContext, "ms.vss-testmanager-web").PlatformMeterId || platformMeterId == service.GetOfferMeter(vssRequestContext, "ms.feed").PlatformMeterId);
    }

    public static bool IsAssignmentBillingEnabledForSubscription(
      IVssRequestContext requestContext,
      Guid subscriptionId)
    {
      return requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableAssignmentBillingJob") && requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableAssignmentBilling") && AssignmentBillingHelper.IsSubscriptionEnabled(requestContext, subscriptionId);
    }

    public static void SetSubscriptionBillingDate(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      DateTime today)
    {
      IVssRegistryService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>();
      RegistryEntry registryEntry = service.ReadEntries(requestContext, (RegistryQuery) string.Format("{0}{1}", (object) "/Service/Commerce/AssignmentBilling/Subscription/", (object) subscriptionId)).SingleOrDefault<RegistryEntry>();
      if (registryEntry == null)
        return;
      registryEntry.SetValue<DateTime>(today);
      service.WriteEntries(requestContext, (IEnumerable<RegistryEntry>) new List<RegistryEntry>()
      {
        registryEntry
      });
    }

    public static int GetLicensingBatchSize(IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Commerce/AssignmentBilling/Licensing/BatchSize", 5000);

    public static int GetLicensingMaxQuantity(IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Commerce/AssignmentBilling/Licensing/MaxQuantity", 100000);

    public static int GetMaxAccountsCount(IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Commerce/AssignmentBilling/MaxAccountsCount", 1000);

    public static bool IsDailyBillingEnabled(IVssRequestContext requestContext, Guid subscriptionId) => requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) string.Format("{0}{1}", (object) "/Service/Commerce/AssignmentBilling/Daily/Subscription/", (object) subscriptionId)).SingleOrDefault<RegistryEntry>()?.Value == "1";

    private static bool IsSubscriptionEnabled(
      IVssRequestContext requestContext,
      Guid subscriptionId)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) string.Format("{0}{1}", (object) "/Service/Commerce/AssignmentBilling/Subscription/", (object) subscriptionId)).SingleOrDefault<RegistryEntry>() != null;
    }
  }
}
