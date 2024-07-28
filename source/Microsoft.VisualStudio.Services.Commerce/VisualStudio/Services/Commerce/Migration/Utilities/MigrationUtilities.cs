// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Migration.Utilities.MigrationUtilities
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce.Migration.Utilities
{
  public static class MigrationUtilities
  {
    private const string ResetStatusIdentifier = "Commerce.ResetResourceUsage";

    public static bool ShouldMigrate(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.DisableResourceMigration") && !requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.ResourceMigrationCompleted");

    public static bool ShouldDualWriteResourceUsage(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.DualWriteResourceUsage");

    public static bool ShouldDualWriteResourceAccount(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.DualWriteBillingInfo");

    public static bool ShouldDualWriteSubscription(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.DualWriteSubscriptionInfo");

    public static bool ThrowExceptionOnSignalling(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.EnableThrowingExceptionOnSignalling");

    public static bool ShouldDualWriteAccountTags(
      IVssRequestContext requestContext,
      Guid collectionId)
    {
      return requestContext.IsFeatureEnabled(collectionId, "Microsoft.Azure.DevOps.Commerce.DualWriteAccountTagsInfo");
    }

    public static bool ShouldDualWriteDefaultAccessLevel(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.DualWriteDefaultAccessLevel");

    public static void SetMigrationInProgress(IVssRequestContext requestContext) => MigrationUtilities.SetFeatureFlag(requestContext, "Microsoft.Azure.DevOps.Commerce.ResourceMigrationInProgress", FeatureAvailabilityState.On);

    public static void SetMigrationCompleted(IVssRequestContext requestContext) => MigrationUtilities.SetFeatureFlag(requestContext, "Microsoft.Azure.DevOps.Commerce.ResourceMigrationCompleted", FeatureAvailabilityState.On);

    public static void ResetMigrationState(IVssRequestContext requestContext)
    {
      MigrationUtilities.SetFeatureFlag(requestContext, "Microsoft.Azure.DevOps.Commerce.ResourceMigrationInProgress", FeatureAvailabilityState.Undefined);
      MigrationUtilities.SetFeatureFlag(requestContext, "Microsoft.Azure.DevOps.Commerce.ResourceMigrationCompleted", FeatureAvailabilityState.Undefined);
      MigrationUtilities.SetFeatureFlag(requestContext, "Microsoft.Azure.DevOps.Commerce.DisableResourceMigration", FeatureAvailabilityState.Undefined);
    }

    public static void ResetMigrationInProgress(IVssRequestContext requestContext) => MigrationUtilities.SetFeatureFlag(requestContext, "Microsoft.Azure.DevOps.Commerce.ResourceMigrationInProgress", FeatureAvailabilityState.Off);

    private static void SetFeatureFlag(
      IVssRequestContext requestContext,
      string featureName,
      FeatureAvailabilityState state)
    {
      requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().SetFeatureState(requestContext, featureName, state);
    }

    internal static void DualWriteResourceUsage(
      IVssRequestContext collectionContext,
      IEnumerable<SubscriptionResourceUsage> resourceUsage)
    {
      MigrationUtilities.ExecuteInTryCatch(collectionContext, 5109284, "Commerce", nameof (DualWriteResourceUsage), (Action) (() =>
      {
        if (!MigrationUtilities.ShouldDualWriteResourceUsage(collectionContext))
          return;
        IMigratorService service = collectionContext.GetService<IMigratorService>();
        service.DualWriteResourceUsage(collectionContext, resourceUsage, collectionContext.ServiceHost.InstanceId);
        IEnumerable<int> meterIds = resourceUsage.Select<SubscriptionResourceUsage, int>((Func<SubscriptionResourceUsage, int>) (x => x.ResourceId));
        if (MigrationUtilities.GetResetStatus(collectionContext))
          meterIds = collectionContext.GetService<PlatformOfferSubscriptionService>().GetMeterUsages(collectionContext, new int?(), new ResourceRenewalGroup?(), false).Select<OfferSubscriptionInternal, int>((Func<OfferSubscriptionInternal, int>) (x => x.MeterId));
        IEnumerable<int> ints = AllowListValidator.RetrieveAllowedMeterIds(collectionContext, meterIds);
        if (ints.IsNullOrEmpty<int>())
          return;
        service.RemoveStaleOrganization(collectionContext, collectionContext.ServiceHost.InstanceId, ints);
      }), MigrationUtilities.ThrowExceptionOnSignalling(collectionContext));
    }

    internal static void DualWriteResourceAccount(
      IVssRequestContext deploymentcontext,
      AzureResourceAccount resourceAccount,
      bool isLinkOperation)
    {
      MigrationUtilities.ExecuteInTryCatch(deploymentcontext, 5109283, "Commerce", nameof (DualWriteResourceAccount), (Action) (() =>
      {
        if (!MigrationUtilities.ShouldDualWriteResourceAccount(deploymentcontext))
          return;
        deploymentcontext.GetService<IMigratorService>().DualWriteResourceAccounts(deploymentcontext, resourceAccount, resourceAccount.CollectionId, isLinkOperation);
      }), MigrationUtilities.ThrowExceptionOnSignalling(deploymentcontext));
    }

    public static void DualWriteAccountTags(
      IVssRequestContext deploymentcontext,
      Guid organizationId,
      Dictionary<string, string> tags)
    {
      MigrationUtilities.ExecuteInTryCatch(deploymentcontext, 5109295, "Commerce", nameof (DualWriteAccountTags), (Action) (() =>
      {
        if (!MigrationUtilities.ShouldDualWriteAccountTags(deploymentcontext, organizationId))
          return;
        deploymentcontext.GetService<IMigratorService>().DualWriteAccountTags(deploymentcontext, organizationId, tags);
      }), MigrationUtilities.ThrowExceptionOnSignalling(deploymentcontext));
    }

    public static void DualWriteDefaultAccessLevel(
      IVssRequestContext deploymentContext,
      Guid organizationId,
      int accessLevel)
    {
      MigrationUtilities.ExecuteInTryCatch(deploymentContext, 5109296, "Commerce", nameof (DualWriteDefaultAccessLevel), (Action) (() =>
      {
        if (!MigrationUtilities.ShouldDualWriteDefaultAccessLevel(deploymentContext))
          return;
        deploymentContext.GetService<IMigratorService>().DualWriteDefaultAccessLevel(deploymentContext, organizationId, accessLevel);
      }), MigrationUtilities.ThrowExceptionOnSignalling(deploymentContext));
    }

    internal static void DualWriteSubscription(
      IVssRequestContext deploymentcontext,
      AzureSubscriptionInternal subscriptionInternal)
    {
      MigrationUtilities.ExecuteInTryCatch(deploymentcontext, 5109282, "Commerce", nameof (DualWriteSubscription), (Action) (() =>
      {
        if (!MigrationUtilities.ShouldDualWriteSubscription(deploymentcontext))
          return;
        deploymentcontext.GetService<IMigratorService>().DualWriteSubscription(deploymentcontext, subscriptionInternal, subscriptionInternal.AzureSubscriptionId);
      }), MigrationUtilities.ThrowExceptionOnSignalling(deploymentcontext));
    }

    internal static void ClearStaleOrganizationUponUnlink(IVssRequestContext collectionContext) => MigrationUtilities.ExecuteInTryCatch(collectionContext, 5109284, "Commerce", nameof (ClearStaleOrganizationUponUnlink), (Action) (() =>
    {
      if (!MigrationUtilities.ShouldDualWriteResourceUsage(collectionContext))
        return;
      IMigratorService service = collectionContext.GetService<IMigratorService>();
      if (!MigrationUtilities.GetResetStatus(collectionContext))
        return;
      IEnumerable<int> ints = AllowListValidator.RetrieveAllowedMeterIds(collectionContext, collectionContext.GetService<PlatformOfferSubscriptionService>().GetMeterUsages(collectionContext, new int?(), new ResourceRenewalGroup?(), false).Select<OfferSubscriptionInternal, int>((Func<OfferSubscriptionInternal, int>) (x => x.MeterId)));
      if (ints.IsNullOrEmpty<int>())
        return;
      service.RemoveStaleOrganization(collectionContext, collectionContext.ServiceHost.InstanceId, ints);
    }), MigrationUtilities.ThrowExceptionOnSignalling(collectionContext));

    private static void SetStaleOrganizationInternal(
      IVssRequestContext collectionContext,
      IEnumerable<int> meterIds)
    {
      MigrationUtilities.ExecuteInTryCatch(collectionContext, 5109284, "Commerce", "SetStaleOrganization", (Action) (() =>
      {
        if (!MigrationUtilities.ShouldDualWriteResourceUsage(collectionContext))
          return;
        collectionContext.GetService<IMigratorService>().SetStaleOrganization(collectionContext, collectionContext.ServiceHost.InstanceId, meterIds);
      }), MigrationUtilities.ThrowExceptionOnSignalling(collectionContext));
    }

    internal static void SetStaleOrganization(
      IVssRequestContext collectionContext,
      IEnumerable<int> meterIds,
      bool resetResource = false)
    {
      IEnumerable<int> ints = AllowListValidator.RetrieveAllowedMeterIds(collectionContext, meterIds);
      if (ints.IsNullOrEmpty<int>())
        return;
      MigrationUtilities.SetStaleOrganizationInternal(collectionContext, ints);
      MigrationUtilities.SetResetStatus(collectionContext, resetResource);
    }

    internal static void SetResetStatus(IVssRequestContext requestContext, bool resetResource)
    {
      if (!requestContext.Items.ContainsKey("Commerce.ResetResourceUsage"))
        requestContext.Items.Add("Commerce.ResetResourceUsage", (object) resetResource);
      else
        requestContext.Items["Commerce.ResetResourceUsage"] = (object) resetResource;
    }

    internal static bool GetResetStatus(IVssRequestContext requestContext)
    {
      bool resetStatus = false;
      if (!requestContext.TryGetItem<bool>("Commerce.ResetResourceUsage", out resetStatus) && !requestContext.RootContext.TryGetItem<bool>("Commerce.ResetResourceUsage", out resetStatus))
        resetStatus = false;
      return resetStatus;
    }

    internal static void SetStaleOrganization(IVssRequestContext collectionContext, int meterId)
    {
      if (!AllowListValidator.IsAllowed(collectionContext, meterId))
        return;
      MigrationUtilities.SetStaleOrganizationInternal(collectionContext, (IEnumerable<int>) new List<int>()
      {
        meterId
      });
    }

    private static void ExecuteInTryCatch(
      IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      Action run,
      bool throwException = true)
    {
      try
      {
        run();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(tracepoint, area, layer, ex);
        if (!throwException)
          return;
        throw;
      }
    }
  }
}
