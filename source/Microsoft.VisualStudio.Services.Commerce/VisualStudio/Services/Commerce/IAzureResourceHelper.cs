// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IAzureResourceHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DefaultServiceImplementation(typeof (AzureResourceHelper))]
  internal interface IAzureResourceHelper : IVssFrameworkService
  {
    IDictionary<double, double> GetMeterPricing(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      Guid meterId,
      out string currencyCode,
      out string locale,
      Guid tenantId,
      Guid objectId);

    IEnumerable<AzureSubscriptionInfo> GetAzureSubscriptions(
      IVssRequestContext requestContext,
      bool doValidation,
      Guid? subscriptionId,
      Guid? tenantId = null);

    IEnumerable<ISubscriptionAccount> GetAzureSubscriptionsForUser(
      IVssRequestContext requestContext,
      Guid? subscriptionId,
      bool queryAcrossTenants = false);

    ISubscriptionAccount GetAzureSubscriptionForPurchase(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string galleryItemId,
      Guid? accountId = null);

    AzureSubscriptionInfo GetAzureSubscriptionForUser(
      IVssRequestContext requestContext,
      Guid subscriptionId);

    bool IsValidAadUser(IVssRequestContext requestContext);

    SubscriptionAuthorizationSource GetSubscriptionAuthorization(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      bool isBundle = false);

    List<string> GetEmailsOfAdminAndCoAdmins(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      Guid tenantId);
  }
}
