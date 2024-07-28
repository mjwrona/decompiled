// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IArmAdapterService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DefaultServiceImplementation(typeof (ArmAdapterService))]
  public interface IArmAdapterService : IVssFrameworkService
  {
    IEnumerable<AzureSubscriptionInfo> GetSubscriptionsForUser(
      IVssRequestContext requestContext,
      Guid? tenantId = null,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault);

    AzureSubscriptionInfo GetSubscriptionForUser(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault,
      Guid? tenantId = null);

    UsageAggregatesResponse GetBillingUsage(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      DateTime startTime,
      DateTime endTime,
      Guid? tenantId = null);

    AzureRateCard GetMeterPricing(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string locale,
      string currencyCode,
      string azureOfferCode,
      string regionInfo,
      Guid? tenantId = null);

    AzureAuthorizationResponseWrapper GetAzureSubscriptionAuthorization(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      Guid? tenantId = null);

    AzureRoleDefinitionResponseWrapper GetSubscriptionRoleDefinitions(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string filter = "",
      Guid? tenantId = null);

    AzureRoleAssignmentResponseWrapper GetSubscriptionRoleAssignments(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string filter = "",
      Guid? tenantId = null);

    ResourceGroupResponse CreateResourceGroup(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string location,
      Guid? tenantId = null);

    ResourceGroupResponse GetResourceGroup(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      Guid? tenantId = null);

    ResourceResponse GetResource(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string accountName,
      string extensionName,
      Guid? tenantId = null);

    ResourceResponse GetAccountResource(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string accountName,
      Guid? tenantId = null);

    ResourceResponse PutResource(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string accountName,
      string extensionName,
      ResourceRequest request,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault,
      Guid? tenantId = null);

    ResourceResponse PutResource(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string accountName,
      ResourceRequest request,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault,
      Guid? tenantId = null);

    ResourceResponse PatchResource(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string accountName,
      string extensionName,
      ResourceRequest request,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault,
      Guid? tenantId = null);

    ResourceResponse DeleteResource(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string accountName,
      string extensionName,
      Guid? tenantId = null);

    void RegisterSubscriptionAgainstResourceProvider(
      IVssRequestContext requestContext,
      Guid subscriptionId);

    AgreementResponse GetAgreement(
      IVssRequestContext requestContext,
      string subscriptionId,
      string publisherId,
      string offerId,
      string planId);

    AgreementResponse SignAgreement(
      IVssRequestContext requestContext,
      string subscriptionId,
      string publisherId,
      string offerId,
      string planId,
      Guid? tenantId = null);

    AgreementResponse CancelAgreement(
      IVssRequestContext requestContext,
      string subscriptionId,
      string publisherId,
      string offerId,
      string planId);

    AzureRoleAssignmentResponseWrapper GetSubscriptionRoleAssignmentsMSA(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string filter = "");
  }
}
