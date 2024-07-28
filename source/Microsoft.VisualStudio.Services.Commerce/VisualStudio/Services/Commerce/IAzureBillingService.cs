// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IAzureBillingService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DefaultServiceImplementation("Microsoft.VisualStudio.Services.Commerce.AzureBillingService, Microsoft.VisualStudio.Services.Commerce.Cloud")]
  public interface IAzureBillingService : IVssFrameworkService
  {
    CommerceBillingSubscriptionInfo GetBillingSubscriptionInfo(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      CommerceBillingContextInfo billingContext,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault);

    CommerceBillingAccountInfo GetBillingAccountInfo(
      IVssRequestContext requestContext,
      CommerceBillingContextInfo billingContext,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault);

    bool IsMonetaryCapPresentForSubscription(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      CommerceBillingContextInfo billingContext,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault);

    CommerceBillingContextInfo GetBillingContextForSubscription(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault);

    SubscriptionStatus? GetStatusForSubscription(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      CommerceBillingContextInfo billingContext,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault);

    CommerceBillingPaymentInstrument GetPaymentInstrumentDetails(
      IVssRequestContext requestContext,
      string paymentInstrumentId,
      CommerceBillingContextInfo billingContext,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault);
  }
}
