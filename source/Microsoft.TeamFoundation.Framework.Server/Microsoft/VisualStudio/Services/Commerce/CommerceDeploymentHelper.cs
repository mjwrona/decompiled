// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceDeploymentHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class CommerceDeploymentHelper
  {
    private static readonly Guid GenevaServiceGuid = new Guid("0000005B-0000-8888-8000-000000000000");
    private static readonly Guid BlobstoreServiceGuid = new Guid("00000019-0000-8888-8000-000000000000");
    private const string UseCommerceAccountResourceClient = "VisualStudio.Services.Commerce.UseCommerceAccountResourceClient";
    private const string UseCommerceDistributor = "VisualStudio.Services.Commerce.UseCommerceDistributor";
    private const string UseCommerceExtensionResourceClient = "VisualStudio.Services.Commerce.UseCommerceExtensionResourceClient";
    private const string UseCommerceMeteringClient = "VisualStudio.Services.Commerce.UseCommerceMeteringClient";
    private const string UseCommerceOfferMeterClient = "VisualStudio.Services.Commerce.UseCommerceOfferMeterClient";
    private const string UseCommerceOfferSubscriptionClient = "VisualStudio.Services.Commerce.UseCommerceOfferSubscriptionClient";
    private const string UseCommerceSubscriptionClient = "VisualStudio.Services.Commerce.UseCommerceSubscriptionClient";
    private const string UseCommercePurchaseRequestClient = "VisualStudio.Services.Commerce.UseCommercePurchaseRequestClient";
    private const string UseCommerceReportingClient = "VisualStudio.Services.Commerce.UseCommerceReportingClient";
    private const string UseCommercePackageClient = "VisualStudio.Services.Commerce.UseCommercePackageClient";
    private const string UseCommerceResourceClient = "VisualStudio.Services.Commerce.UseCommerceResourceClient";
    private const string UseCommerceSubscriptionResourceClient = "VisualStudio.Services.Commerce.UseCommerceSubscriptionResourceClient";
    private const string EnableCommerceMasterFeatureFlag = "VisualStudio.Services.Commerce.EnableCommerceMaster";
    private const string EnableCommerceClientsRoutingFeatureFlag = "Microsoft.VisualStudio.Services.Commerce.EnableRoutingClientsToCommerce";
    private const string EnableCsmCommerceMasterFeatureFlag = "VisualStudio.Services.Commerce.EnableCsmCommerceMaster";
    private const string DisableHostTypeCheckFeatureFlag = "VisualStudio.Services.Commerce.DisableHostTypeCheck";
    private const string DisableCommerceFrameworkServicesToSpsFeatureFlag = "Microsoft.VisualStudio.Services.Commerce.DisableCommerceFrameworkServicesToSps";
    private const string EnableCommerceCsmClientsRoutingFeatureFlag = "Microsoft.VisualStudio.Services.Commerce.EnableRoutingCsmClientsToCommerce";
    public const string BlockVisualStudioAnnualPurchase = "Microsoft.Azure.DevOps.Commerce.BlockVisualStudioAnnualPurchase";
    private const string Area = "Commerce";
    private const string Layer = "CommerceDeploymentHelper";
    private static HashSet<string> offlineHandledOfferMeters = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "StandardLicense",
      "ms.vss-vstsuser",
      "AdvancedLicense"
    };

    public static void ExecuteFrameworkServiceWithFallback(
      IVssRequestContext requestContext,
      Action primary,
      Action fallback,
      string area,
      string layer,
      int tracepoint,
      bool executePrimary = false)
    {
      bool flag = !executePrimary;
      try
      {
        if (executePrimary)
        {
          requestContext.TraceAlways(tracepoint, TraceLevel.Info, area, layer, "Method routed to commerce service..");
          primary();
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(123456789, TraceLevel.Warning, area, layer, "Exception occurred while executing the Framework service method on Commerce Service so falling back to SPS...  Message - " + ex.Message + ", Stacktrace - " + ex.StackTrace);
        if (CommerceDeploymentHelper.IsSpsFallbackDisabled(requestContext))
        {
          requestContext.TraceAlways(1234567890, TraceLevel.Error, area, layer, "Fallback to SPS disabled for the caller " + requestContext.UserAgent + "... ");
          throw;
        }
        else
          flag = true;
      }
      if (!flag)
        return;
      fallback();
    }

    public static T ExecuteFrameworkServiceWithFallback<T>(
      IVssRequestContext requestContext,
      Func<T> primary,
      Func<T> fallback,
      string area,
      string layer,
      int tracepoint,
      bool executePrimary = false)
    {
      try
      {
        if (executePrimary)
        {
          requestContext.TraceAlways(tracepoint, TraceLevel.Info, area, layer, "Method routed to commerce service..");
          return primary();
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(123456789, TraceLevel.Warning, area, layer, "Exception occurred while executing the Framework service method on Commerce Service so falling back to SPS...  Message - " + ex.Message + ", Stacktrace - " + ex.StackTrace);
        if (CommerceDeploymentHelper.IsSpsFallbackDisabled(requestContext))
        {
          requestContext.TraceAlways(1234567890, TraceLevel.Error, area, layer, "Fallback to SPS disabled for the caller " + requestContext.UserAgent + "... ");
          throw;
        }
      }
      return fallback();
    }

    public static bool IsCommerceServiceAccountResourceEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.UseCommerceAccountResourceClient");

    public static bool IsCommerceServiceDistributorEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.UseCommerceDistributor");

    public static bool IsCommerceServiceExtensionResourceEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.UseCommerceExtensionResourceClient");

    public static bool IsCommerceServiceMeteringEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.UseCommerceMeteringClient");

    public static bool IsCommerceServiceOfferMetersEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.UseCommerceOfferMeterClient");

    public static bool IsCommerceServiceOfferSubscriptionsEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.UseCommerceOfferSubscriptionClient");

    public static bool IsCommerceServiceReportingEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.UseCommerceReportingClient");

    public static bool IsCommerceServicePackageEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.UseCommercePackageClient");

    public static bool IsCommerceServiceResourceEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.UseCommerceResourceClient");

    public static bool IsCommerceServiceSubscriptionResourceEnabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.UseCommerceSubscriptionResourceClient");
    }

    public static bool IsCommerceServiceSubscriptionsEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.UseCommerceSubscriptionClient");

    public static bool IsCommerceServicePurchaseRequestEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.UseCommercePurchaseRequestClient");

    public static bool IsCommerceMasterEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableCommerceMaster");

    public static bool IsCsmCommerceMasterEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableCsmCommerceMaster");

    public static bool IsHostTypeCheckEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableHostTypeCheck");

    public static bool IsBlockVisualStudioAnnualPurchaseEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.BlockVisualStudioAnnualPurchase");

    public static bool IsMissingLocalDeployment(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return !vssRequestContext.ServiceHost.IsProduction && string.IsNullOrEmpty(vssRequestContext.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext, Guid.Parse("000080C1-AA68-4FCE-BBC5-C68D94BFF8BE"), AccessMappingConstants.PublicAccessMappingMoniker));
    }

    public static bool IsOfferMeterHandledOffline(string offerMeterNameOrGalleryId) => CommerceDeploymentHelper.offlineHandledOfferMeters.Contains(offerMeterNameOrGalleryId);

    public static bool IsCommerceForwardingEnabled(
      IVssRequestContext requestContext,
      Guid collectionId,
      Guid? subscriptionId = null,
      string layer = null,
      string resourceName = null)
    {
      IVssRegistryService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>();
      if (collectionId != Guid.Empty)
      {
        RegistryEntry registryEntry = service.ReadEntries(requestContext, (RegistryQuery) string.Format("/Service/Commerce/Forwarding/Account/{0}", (object) collectionId)).SingleOrDefault<RegistryEntry>();
        if (registryEntry?.Value == "1")
          return true;
        if (registryEntry?.Value == "0")
          return false;
      }
      if (subscriptionId.HasValue)
      {
        RegistryEntry registryEntry = service.ReadEntries(requestContext, (RegistryQuery) string.Format("/Service/Commerce/Forwarding/Subscription/{0}", (object) subscriptionId.Value)).SingleOrDefault<RegistryEntry>();
        if (registryEntry?.Value == "1")
          return true;
        if (registryEntry?.Value == "0")
          return false;
      }
      if (layer != null)
      {
        RegistryEntry registryEntry = service.ReadEntries(requestContext, (RegistryQuery) ("/Service/Commerce/Forwarding/" + layer + "/All")).SingleOrDefault<RegistryEntry>();
        if (registryEntry?.Value == "1")
          return true;
        if (registryEntry?.Value == "0")
          return false;
      }
      if (resourceName != null)
      {
        RegistryEntry registryEntry = service.ReadEntries(requestContext, (RegistryQuery) ("/Service/Commerce/Forwarding/Account/" + resourceName)).SingleOrDefault<RegistryEntry>();
        if (registryEntry?.Value == "1")
          return true;
        if (registryEntry?.Value == "0")
          return false;
      }
      return service.ReadEntries(requestContext, (RegistryQuery) "/Service/Commerce/Forwarding/All").SingleOrDefault<RegistryEntry>()?.Value == "1";
    }

    public static bool IsIdentitySearchServiceUsageEnabled(
      IVssRequestContext requestContext,
      string accountName)
    {
      IVssRegistryService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>();
      if (!string.IsNullOrEmpty(accountName))
      {
        RegistryEntry registryEntry = service.ReadEntries(requestContext, (RegistryQuery) ("/Service/Commerce/IdentitySearchUsage/AccountName/" + accountName)).SingleOrDefault<RegistryEntry>();
        if (registryEntry?.Value == "1")
          return true;
        if (registryEntry?.Value == "0")
          return false;
      }
      return service.ReadEntries(requestContext, (RegistryQuery) "/Service/Commerce/IdentitySearchUsage/All").SingleOrDefault<RegistryEntry>()?.Value == "1";
    }

    public static bool IsNewGetAccountsEnabled(IVssRequestContext requestContext, Guid memberId)
    {
      IVssRegistryService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>();
      if (memberId != Guid.Empty)
      {
        RegistryEntry registryEntry = service.ReadEntries(requestContext, (RegistryQuery) string.Format("/Service/Commerce/NewGetAccounts/{0}", (object) memberId)).SingleOrDefault<RegistryEntry>();
        if (registryEntry?.Value == "1")
          return true;
        if (registryEntry?.Value == "0")
          return false;
      }
      return service.ReadEntries(requestContext, (RegistryQuery) "/Service/Commerce/NewGetAccounts/All").SingleOrDefault<RegistryEntry>()?.Value == "1";
    }

    public static Microsoft.VisualStudio.Services.Organization.Collection GetCollectionFromSps(
      IVssRequestContext context,
      Guid collectionId)
    {
      return CommerceDeploymentHelper.GetSpsCollectionClient<OrganizationHttpClient>(context, collectionId, "Organization").GetCollectionAsync("Me").SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Collection>().ToServer();
    }

    public static Microsoft.VisualStudio.Services.Organization.Organization GetOrganizationFromSps(
      IVssRequestContext context,
      Guid collectionId)
    {
      return CommerceDeploymentHelper.GetSpsCollectionClient<OrganizationHttpClient>(context, collectionId, "Organization").GetOrganizationAsync("Me").SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Organization>().ToServer();
    }

    internal static bool IsPortalStaticPageEnabled(
      IVssRequestContext requestContext,
      Guid directoryId,
      int exceptionTracepoint,
      string area,
      string layer)
    {
      IVssRegistryService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>();
      try
      {
        if (directoryId != Guid.Empty)
        {
          int result;
          if (int.TryParse(service.ReadEntries(requestContext, (RegistryQuery) "/Service/Commerce/BillingTab/PortalStaticPage/EnhabledPercentage").SingleOrDefault<RegistryEntry>()?.Value, out result))
            return directoryId.GetHashCode() % 6 + 5 <= result;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(exceptionTracepoint, area, layer, ex);
        return false;
      }
      return false;
    }

    public static Guid GetOrganizationTenantId(IVssRequestContext context, Guid collectionId)
    {
      Microsoft.VisualStudio.Services.Organization.Organization organizationFromSps = CommerceDeploymentHelper.GetOrganizationFromSps(context, collectionId);
      return organizationFromSps == null ? Guid.Empty : organizationFromSps.TenantId;
    }

    public static bool IsProjectCollectionAdmin(
      IVssRequestContext collectionContext,
      Guid memberId,
      int tracepoint,
      string area,
      string layer)
    {
      try
      {
        IdentityService service = collectionContext.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(collectionContext, (IList<Guid>) new Guid[1]
        {
          memberId
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        return identity != null && CommerceDeploymentHelper.IsProjectCollectionAdmin(collectionContext, identity, tracepoint, area, layer, service);
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(tracepoint, area, layer, ex);
        throw;
      }
    }

    public static bool IsProjectCollectionAdmin(
      IVssRequestContext collectionContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      int tracepoint,
      string area,
      string layer,
      IdentityService identityService = null)
    {
      try
      {
        identityService = identityService ?? collectionContext.GetService<IdentityService>();
        return ServicePrincipals.IsServicePrincipal(collectionContext, identity.Descriptor) || identityService.IsMember(collectionContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, identity.Descriptor);
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(tracepoint, area, layer, ex);
        throw;
      }
    }

    public static bool IsProjectCollectionAdmin(
      IVssRequestContext collectionContext,
      SubjectDescriptor subjectDescriptor,
      int tracepoint,
      string area,
      string layer)
    {
      try
      {
        IdentityService service = collectionContext.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(collectionContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
        {
          subjectDescriptor
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        return identity != null && (ServicePrincipals.IsServicePrincipal(collectionContext, identity.Descriptor) || service.IsMember(collectionContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, identity.Descriptor));
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(tracepoint, area, layer, ex);
        throw;
      }
    }

    public static bool IsProjectCollectionAdmin(
      IVssRequestContext requestContext,
      Guid memberId,
      Guid collectionId,
      bool useIms = false)
    {
      GraphHttpClient collectionClient = CommerceDeploymentHelper.GetSpsCollectionClient<GraphHttpClient>(requestContext, collectionId, "Graph");
      GraphDescriptorResult descriptorResult = collectionClient.GetDescriptorAsync(memberId).SyncResult<GraphDescriptorResult>();
      if (descriptorResult == null)
        return false;
      return descriptorResult.Value.IsSystemType() || collectionClient.CheckMembershipExistenceAsync((string) descriptorResult.Value, (string) GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup.ToSubjectDescriptor(requestContext)).SyncResult<bool>();
    }

    public static bool IsProjectCollectionAdmin(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      Guid collectionId,
      bool useIms = false)
    {
      GraphHttpClient collectionClient = CommerceDeploymentHelper.GetSpsCollectionClient<GraphHttpClient>(requestContext, collectionId, "Graph");
      if (subjectDescriptor == new SubjectDescriptor())
        return false;
      return subjectDescriptor.IsSystemType() || collectionClient.CheckMembershipExistenceAsync((string) subjectDescriptor, (string) GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup.ToSubjectDescriptor(requestContext)).SyncResult<bool>();
    }

    private static TClient GetSpsCollectionClient<TClient>(
      IVssRequestContext context,
      Guid collectionId,
      string area)
      where TClient : class, IVssHttpClient
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      Uri hostUri = vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, collectionId, ServiceInstanceTypes.SPS);
      return (context.ClientProvider as ICreateClient).CreateClient<TClient>(context, hostUri, area, (ApiResourceLocationCollection) null);
    }

    public static bool IsScopeCheckEnabled(IVssRequestContext requestContext, Guid? subscriptionId = null)
    {
      IVssRegistryService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>();
      if (subscriptionId.HasValue)
      {
        RegistryEntry registryEntry = service.ReadEntries(requestContext, (RegistryQuery) string.Format("/Service/Commerce/ScopeCheck/Subscription/{0}", (object) subscriptionId.Value)).SingleOrDefault<RegistryEntry>();
        if (registryEntry?.Value == "1")
          return true;
        if (registryEntry?.Value == "0")
          return false;
      }
      return service.ReadEntries(requestContext, (RegistryQuery) "/Service/Commerce/ScopeCheck/All").SingleOrDefault<RegistryEntry>()?.Value == "1";
    }

    public static string GetHostName(IVssRequestContext requestContext, Guid hostId) => requestContext.GetService<INameResolutionService>().GetPrimaryEntryForValue(requestContext, hostId)?.Name;

    public static bool IsBillingDisabled(IVssRequestContext requestContext, string userAgent)
    {
      IVssRegistryService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>();
      bool flag = false;
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery query = (RegistryQuery) "/Service/Commerce/DisableBillingScaleUnitsList";
      RegistryEntry registryEntry = service.ReadEntries(requestContext1, query).SingleOrDefault<RegistryEntry>();
      if (registryEntry != null && !registryEntry.Value.IsNullOrEmpty<char>())
        flag = ((IEnumerable<string>) registryEntry.Value.Split(';')).Any<string>((Func<string, bool>) (x => userAgent.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0));
      return flag;
    }

    public static IEnumerable<IOfferMeter> OfflineOfferMeters => (IEnumerable<IOfferMeter>) new List<IOfferMeter>()
    {
      (IOfferMeter) new OfferMeter()
      {
        MeterId = 1,
        Name = "StandardLicense",
        BillingState = MeterBillingState.Paid,
        CommittedQuantity = 5,
        CurrentQuantity = 5,
        GalleryId = "ms.vss-vstsuser",
        IncludedQuantity = 5,
        MaximumQuantity = 10005,
        AbsoluteMaximumQuantity = int.MaxValue,
        RenewalFrequency = MeterRenewalFrequecy.Monthly,
        Status = MeterState.Active,
        MinimumRequiredAccessLevel = MinimumRequiredServiceLevel.Express,
        AutoAssignOnAccess = true
      },
      (IOfferMeter) new OfferMeter()
      {
        MeterId = 2,
        Name = "AdvancedLicense",
        BillingState = MeterBillingState.Paid,
        MaximumQuantity = 100,
        AbsoluteMaximumQuantity = int.MaxValue,
        RenewalFrequency = MeterRenewalFrequecy.Monthly,
        Status = MeterState.Active,
        MinimumRequiredAccessLevel = MinimumRequiredServiceLevel.Express,
        AutoAssignOnAccess = true
      },
      (IOfferMeter) new OfferMeter()
      {
        MeterId = 9,
        Name = "Test Manager",
        BillingState = MeterBillingState.Paid,
        MaximumQuantity = 100,
        AbsoluteMaximumQuantity = int.MaxValue,
        RenewalFrequency = MeterRenewalFrequecy.Monthly,
        Status = MeterState.Active,
        MinimumRequiredAccessLevel = MinimumRequiredServiceLevel.Express,
        AutoAssignOnAccess = false
      }
    };

    public static bool IsCommerceServiceRoutingEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Commerce.EnableRoutingClientsToCommerce");

    public static bool IsCsmCommerceServiceRoutingEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Commerce.EnableRoutingCsmClientsToCommerce");

    public static bool IsSpsFallbackDisabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Commerce.DisableCommerceFrameworkServicesToSps");

    public static Guid? ResolveCollectionHostName(
      IVssRequestContext requestContext,
      string hostName)
    {
      if (!HostCreationHelper.CheckIfCollectionNameExists(requestContext, hostName))
        return new Guid?();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<INameResolutionService>().QueryEntry(vssRequestContext, "Collection", hostName)?.Value;
    }

    public static bool IsCallerGenevaActionsServicePrincipal(this IVssRequestContext requestContext)
    {
      Guid spGuid;
      return ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext) && ServicePrincipals.TryParse(requestContext.UserContext, out spGuid, out Guid _) && spGuid == CommerceDeploymentHelper.GenevaServiceGuid;
    }

    public static bool IsBlockingYearlyBundleEnabledForSubscription(
      IVssRequestContext requestContext,
      Guid azureSubscriptionId)
    {
      IVssRegistryService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>();
      if (azureSubscriptionId != Guid.Empty)
      {
        RegistryEntry registryEntry = service.ReadEntries(requestContext, (RegistryQuery) string.Format("/Service/Commerce/BundlePurchaseEligibility/Subscription/{0}", (object) azureSubscriptionId)).SingleOrDefault<RegistryEntry>();
        if (registryEntry?.Value == "1")
          return true;
        if (registryEntry?.Value == "0")
          return false;
      }
      return service.ReadEntries(requestContext, (RegistryQuery) "/Service/Commerce/BundlePurchaseEligibility/All").SingleOrDefault<RegistryEntry>()?.Value == "1";
    }

    public static bool IsCallerBlobstoreServicePrincipal(IVssRequestContext requestContext)
    {
      Guid spGuid;
      return ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext) && ServicePrincipals.TryParse(requestContext.UserContext, out spGuid, out Guid _) && spGuid == CommerceDeploymentHelper.BlobstoreServiceGuid;
    }

    public static bool IsProjectCollectionValidUser(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      Guid collectionId)
    {
      GraphHttpClient collectionClient = CommerceDeploymentHelper.GetSpsCollectionClient<GraphHttpClient>(requestContext, collectionId, "Graph");
      if (subjectDescriptor == new SubjectDescriptor())
        return false;
      return subjectDescriptor.IsSystemType() || collectionClient.CheckMembershipExistenceAsync((string) subjectDescriptor, (string) GroupWellKnownIdentityDescriptors.EveryoneGroup.ToSubjectDescriptor(requestContext)).SyncResult<bool>();
    }
  }
}
