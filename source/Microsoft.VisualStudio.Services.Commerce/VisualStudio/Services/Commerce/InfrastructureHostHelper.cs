// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.InfrastructureHostHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.VisualStudio.Services.Commerce.Client;
using Microsoft.VisualStudio.Services.HostManagement;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Partitioning;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class InfrastructureHostHelper
  {
    private static readonly IConfigPrototype<bool> SubscriptionIdsToForceRouteCreateInfrastructureOrgPrototype = ConfigPrototype.Create<bool>("Commerce.SubscriptionIdsToForceRouteCreateInfrastructureOrg", false);
    private static readonly IConfigQueryable<bool> SubscriptionIdsToForceRouteCreateInfrastructureOrgConfig = ConfigProxy.Create<bool>(InfrastructureHostHelper.SubscriptionIdsToForceRouteCreateInfrastructureOrgPrototype);
    private static readonly IConfigPrototype<string> SpsInstanceIdToForceRouteCreateInfrastructureOrgPrototype = ConfigPrototype.Create<string>("Commerce.SpsInstanceIdToForceRouteCreateInfrastructureOrg", string.Empty);
    private static readonly IConfigQueryable<string> SpsInstanceIdToForceRouteCreateInfrastructureOrgConfig = ConfigProxy.Create<string>(InfrastructureHostHelper.SpsInstanceIdToForceRouteCreateInfrastructureOrgPrototype);
    private const string userScopedHiddenOrganizationHostPrefix = "AZR-SUB-APP";
    private const string userScopedHiddenCollectionHostPrefix = "AZR-SUBSCRIPTION";
    private const string hiddenCollectionRegionPath = "/Service/Commerce/Commerce/HiddenCollectionRegion";
    private const string hiddenCollectionRegionDefault = "SCUS";
    private const string Area = "Commerce";
    private const string Layer = "InfrastructureHostHelper";

    public static void WithInfrastructureCollectionContext(
      IVssRequestContext requestContext,
      Guid azureSubscriptionId,
      Microsoft.VisualStudio.Services.Identity.Identity requestorIdentity,
      Action<IVssRequestContext> action)
    {
      requestContext.TraceEnter(5108899, "Commerce", nameof (InfrastructureHostHelper), new object[2]
      {
        (object) azureSubscriptionId,
        (object) requestorIdentity
      }, nameof (WithInfrastructureCollectionContext));
      try
      {
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        IVssRequestContext vssRequestContext = requestContext1.Elevate();
        string str = "AZR-SUBSCRIPTION-" + azureSubscriptionId.ToString("N").ToUpper();
        Guid? nullable = InfrastructureHostHelper.ResolveCollectionHostName(requestContext1, str);
        if (!nullable.HasValue)
        {
          nullable = new Guid?(InfrastructureHostHelper.CreateAndLinkInfrastructureHosts(requestContext, azureSubscriptionId, str, requestorIdentity));
        }
        else
        {
          ISubscriptionService service = vssRequestContext.GetService<ISubscriptionService>();
          if (service.GetAccounts(vssRequestContext, azureSubscriptionId, AccountProviderNamespace.Marketplace).FirstOrDefault<ISubscriptionAccount>() == null)
          {
            if (requestContext1.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.StopNRDeletionForInfraHostFailures"))
            {
              service.LinkCollection(vssRequestContext, azureSubscriptionId, AccountProviderNamespace.Marketplace, nullable.Value, new Guid?(CommerceConstants.CommerceServiceGuid));
            }
            else
            {
              requestContext.Trace(5108921, TraceLevel.Error, "Commerce", nameof (InfrastructureHostHelper), "Name resolution entries for " + str + " already existed but there was no corresponding subscription account.");
              vssRequestContext.GetService<INameResolutionService>().DeleteEntry(vssRequestContext, "Collection", str);
              nullable = new Guid?(InfrastructureHostHelper.CreateAndLinkInfrastructureHosts(requestContext, azureSubscriptionId, str, requestorIdentity));
            }
          }
          else
            requestContext.Trace(5106902, TraceLevel.Info, "Commerce", nameof (InfrastructureHostHelper), string.Format("Found infrastructure host {0} for user {1} azure subscription {2}", (object) nullable, (object) requestorIdentity.MasterId, (object) azureSubscriptionId));
        }
        CollectionHelper.WithCollectionContext(requestContext, nullable.Value, action, method: nameof (WithInfrastructureCollectionContext));
      }
      finally
      {
        requestContext.TraceLeave(5108900, "Commerce", nameof (InfrastructureHostHelper), nameof (WithInfrastructureCollectionContext));
      }
    }

    public static Guid CreateInfrastructureOrganization(
      IVssRequestContext requestContext,
      string collectionHostName,
      string organizationHostName,
      string hostRegion,
      ServiceHostTags serviceHostTags)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      try
      {
        if (requestContext.IsSpsService())
        {
          if (requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.RemoveNRForInfraHostFailuresOnSps"))
          {
            requestContext.TraceAlways(5109145, TraceLevel.Info, "Commerce", nameof (InfrastructureHostHelper), new
            {
              Msg = "Cleaning nameresolution entry if exists.",
              collectionHostName = collectionHostName,
              organizationHostName = organizationHostName,
              hostRegion = hostRegion,
              serviceHostTags = serviceHostTags
            }.Serialize());
            vssRequestContext.GetService<INameResolutionService>().DeleteEntry(vssRequestContext, "Collection", collectionHostName);
            requestContext.TraceAlways(5109145, TraceLevel.Info, "Commerce", nameof (InfrastructureHostHelper), new
            {
              Msg = "Deleted the nameresolution entry.",
              collectionHostName = collectionHostName
            }.Serialize());
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5109145, TraceLevel.Error, "Commerce", nameof (InfrastructureHostHelper), ex);
      }
      Guid infrastructureOrganization = Guid.NewGuid();
      ServiceHostProperties hostProperties = new ServiceHostProperties()
      {
        HostId = infrastructureOrganization,
        Name = organizationHostName,
        HostType = ServiceHostType.Application,
        Region = hostRegion,
        Description = serviceHostTags.ToString()
      };
      Dictionary<string, string> servicingTokens = new Dictionary<string, string>()
      {
        {
          ServicingTokenConstants.CollectionName,
          collectionHostName
        },
        {
          ServicingTokenConstants.RequestForCreatingInfrastructureHost,
          true.ToString()
        },
        {
          ServicingTokenConstants.ServiceHostRegion,
          hostRegion
        },
        {
          ServicingTokenConstants.NoInstanceAllocations,
          true.ToString()
        }
      };
      ServicingJobDetail host = vssRequestContext.GetService<OrganizationHostCreationService>().CreateHost(vssRequestContext, hostProperties, (IDictionary<string, string>) servicingTokens, (Microsoft.VisualStudio.Services.Identity.Identity) null);
      requestContext.TraceAlways(5109145, TraceLevel.Info, "Commerce", nameof (InfrastructureHostHelper), new
      {
        Msg = "Servicing job run result",
        jobDetail = host
      }.Serialize());
      if (host.JobStatus != ServicingJobStatus.Complete)
        throw new InvalidOperationException("Organization host creation failed on operation: " + host.OperationClass);
      return infrastructureOrganization;
    }

    public static void CopyRequestContextItems(
      IVssRequestContext sourceRequestContext,
      IVssRequestContext targetRequestContext)
    {
      IDictionary<string, object> items = sourceRequestContext.Items;
      if (!targetRequestContext.Items.ContainsKey("Commerce.RequestSource") && items.ContainsKey("Commerce.RequestSource"))
        targetRequestContext.Items.Add("Commerce.RequestSource", items["Commerce.RequestSource"]);
      else if (items.ContainsKey("Commerce.RequestSource"))
        targetRequestContext.Items["Commerce.RequestSource"] = items["Commerce.RequestSource"];
      object obj1;
      targetRequestContext.Items.TryGetValue("Commerce.RequestSource", out obj1);
      sourceRequestContext.Trace(5108928, TraceLevel.Info, "Commerce", nameof (InfrastructureHostHelper), string.Format("Copied Items dictionary. Source identifier: {0}", obj1));
      if (!targetRequestContext.Items.ContainsKey("Commerce.DualWrite") && items.ContainsKey("Commerce.DualWrite"))
        targetRequestContext.Items.Add("Commerce.DualWrite", items["Commerce.DualWrite"]);
      else if (items.ContainsKey("Commerce.DualWrite"))
        targetRequestContext.Items["Commerce.DualWrite"] = items["Commerce.DualWrite"];
      object obj2;
      targetRequestContext.Items.TryGetValue("Commerce.DualWrite", out obj2);
      sourceRequestContext.Trace(5108928, TraceLevel.Info, "Commerce", nameof (InfrastructureHostHelper), string.Format("Copied Items dictionary. DualWrite identifier: {0}", obj2));
    }

    public static Guid? ResolveCollectionHostName(
      IVssRequestContext requestContext,
      string hostName)
    {
      if (!HostCreationHelper.CheckIfCollectionNameExists(requestContext, hostName))
        return new Guid?();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<INameResolutionService>().QueryEntry(vssRequestContext, "Collection", hostName)?.Value;
    }

    private static Guid CreateAndLinkInfrastructureHosts(
      IVssRequestContext requestContext,
      Guid azureSubscriptionId,
      string collectionHostName,
      Microsoft.VisualStudio.Services.Identity.Identity requestorIdentity)
    {
      Guid hostId = Guid.Empty;
      IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      try
      {
        requestContext.TraceEnter(5108924, "Commerce", nameof (InfrastructureHostHelper), nameof (CreateAndLinkInfrastructureHosts));
        ISubscriptionService service1 = requestContext.GetService<ISubscriptionService>();
        requestContext.Trace(5106901, TraceLevel.Info, "Commerce", nameof (InfrastructureHostHelper), string.Format("Creating infrastructure host for user {0} azure subscription {1}", (object) requestorIdentity.MasterId, (object) azureSubscriptionId));
        IVssRequestContext vssRequestContext2 = vssRequestContext1.Elevate();
        string str1 = "AZR-SUB-APP-" + azureSubscriptionId.ToString("N").ToUpper();
        string hostRegion = vssRequestContext1.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext1, (RegistryQuery) "/Service/Commerce/Commerce/HiddenCollectionRegion", "SCUS");
        ServiceHostTags serviceHostTags = new ServiceHostTags();
        serviceHostTags.AddTag(WellKnownServiceHostTags.IsInfrastructureHost);
        serviceHostTags.AddTag(WellKnownServiceHostTags.NoOrgMetadata);
        serviceHostTags.AddTag(CommerceWellKnownServiceHostTags.AssociatedWithAzureSubscription);
        Guid collectionId;
        try
        {
          if (vssRequestContext1.IsCommerceService())
          {
            if (!HostCreationHelper.CheckIfCollectionNameExists(vssRequestContext1, collectionHostName))
            {
              CommerceHostHelperHttpClient client = vssRequestContext2.GetClient<CommerceHostHelperHttpClient>();
              string str2 = InfrastructureHostHelper.SpsInstanceIdToForceRouteCreateInfrastructureOrgConfig.QueryByCtx<string>(vssRequestContext1);
              if (!str2.IsNullOrEmpty<char>() && InfrastructureHostHelper.SubscriptionIdsToForceRouteCreateInfrastructureOrgConfig.QueryById<bool>(vssRequestContext1, azureSubscriptionId))
              {
                PartitionContainer partitionContainer = vssRequestContext2.GetService<IPartitioningService>().GetPartitionContainer(vssRequestContext2, new Guid(str2)) ?? throw new InvalidOperationException("Unable to find a partition container with the ID: " + str2);
                client = HttpClientHelper.CreateClient<CommerceHostHelperHttpClient>(requestContext, new Uri(partitionContainer.Address));
                requestContext.TraceAlways(5108929, TraceLevel.Info, "Commerce", nameof (InfrastructureHostHelper), "Infrastructure host creation API call is configured to be routed to the sps service instance {0} for azure subs {1}", (object) str2, (object) azureSubscriptionId);
              }
              hostId = client.CreateInfrastructureOrganization(str1, collectionHostName, hostRegion, serviceHostTags.ToString(), (object) vssRequestContext2).SyncResult<Guid>();
              HostNameResolver.TryGetCollectionServiceHostId(vssRequestContext1, collectionHostName, out collectionId);
              vssRequestContext1.GetService<CommerceHostManagementService>().EnsureHostUpdated(requestContext, collectionId);
            }
          }
          else
            hostId = InfrastructureHostHelper.CreateInfrastructureOrganization(vssRequestContext2, collectionHostName, str1, hostRegion, serviceHostTags);
          if (!HostNameResolver.TryGetCollectionServiceHostId(vssRequestContext1, collectionHostName, out collectionId))
            throw new InvalidOperationException(string.Format("Unable to find collection host for newly created infrastructure host {0} ", (object) hostId) + string.Format("on subscription {0}.", (object) azureSubscriptionId));
          service1.LinkCollection(vssRequestContext2, azureSubscriptionId, AccountProviderNamespace.Marketplace, collectionId, new Guid?(CommerceConstants.SpsMasterId));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(5108926, "Commerce", nameof (InfrastructureHostHelper), ex);
          if (hostId != Guid.Empty)
            vssRequestContext1.GetService<IHostDeletionService>().DeleteHost(vssRequestContext1, hostId, DeleteHostResourceOptions.MarkForDeletion, HostDeletionReason.HostDeleted);
          throw;
        }
        if (!service1.GetAccounts(vssRequestContext2, azureSubscriptionId, AccountProviderNamespace.VisualStudioOnline).Any<ISubscriptionAccount>())
        {
          IArmAdapterService service2 = vssRequestContext1.GetService<IArmAdapterService>();
          if (vssRequestContext1.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.UseRequestIdentityToArmCall"))
            service2.RegisterSubscriptionAgainstResourceProvider(requestContext, azureSubscriptionId);
          else
            service2.RegisterSubscriptionAgainstResourceProvider(vssRequestContext1, azureSubscriptionId);
        }
        return collectionId;
      }
      finally
      {
        requestContext.TraceLeave(5108925, "Commerce", nameof (InfrastructureHostHelper), nameof (CreateAndLinkInfrastructureHosts));
      }
    }

    public static bool IsInfrastructureHost(IVssRequestContext requestContext, Guid collectionId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      CommerceHostManagementService service = vssRequestContext.GetService<CommerceHostManagementService>();
      HostProperties hostProperties = service.QueryServiceHostPropertiesCached(vssRequestContext, collectionId);
      if (hostProperties == null)
        return false;
      ServiceHostTags serviceHostTags = ServiceHostTags.FromString(service.QueryServiceHostPropertiesCached(vssRequestContext, hostProperties.ParentId)?.Description);
      return serviceHostTags != ServiceHostTags.EmptyServiceHostTags && serviceHostTags.HasTag(WellKnownServiceHostTags.IsInfrastructureHost);
    }
  }
}
