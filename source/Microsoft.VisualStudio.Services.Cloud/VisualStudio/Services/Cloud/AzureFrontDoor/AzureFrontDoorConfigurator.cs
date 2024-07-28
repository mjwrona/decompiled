// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureFrontDoor.AzureFrontDoorConfigurator
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Identity.Client;
using Microsoft.Rest;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.AfdClient;
using Microsoft.VisualStudio.Services.AfdClient.Handlers;
using Microsoft.VisualStudio.Services.AfdClient.Models;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud.AzureFrontDoor
{
  public static class AzureFrontDoorConfigurator
  {
    private const string c_alertTsgLink = "https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki?pagePath=/Live Site/TSGs/Alerts TSGs/Framework Alerts/TSG: AzureFrontDoor - EdgeCompositeReliability&wikiVersion=GBwikiMaster";
    private const string c_alertDashboardLinkFormat = "https://www.afdcp.com/Configuration/{0}/{1}/_Monitoring";
    private const string c_errorPageBreakValue = "<!--BREAKHERE-->";
    private const string c_routeKeyName = "EndpointResponseRouteKey";
    private const string c_lookupFailureAction = "ForwardToDefaultEndpointPool";
    private const string c_endpointSelectionStrategy = "FirstHealthyEndpoint";
    private const int c_probeIntervalDefault = 15;
    private const int c_probeSuccessCount = 1;
    private const int c_probeSampleSize = 4;
    public const string RedirectRouteNameSuffix = "-HttpsRedirect";

    public static List<Guid> ConfigureAfd(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      AfdConfigData config,
      ITFLogger logger,
      bool validateOnly = false)
    {
      logger.Info("Entering ConfigureAfdInternal with config:" + Environment.NewLine + JsonConvert.SerializeObject((object) config));
      AfdRetryHelper.ExecuteWithRetries<Tenant>((Func<Tenant>) (() => TenantsExtensions.Read(client.Tenants, config.Tenant)), logger);
      AfdRetryHelper.ExecuteWithRetries<Partner>((Func<Partner>) (() => PartnersExtensions.Read(client.Partners, config.Partner, config.Tenant)), logger);
      List<Guid> trackedChanges = new List<Guid>();
      List<Host> hostsForRoute = AzureFrontDoorConfigurator.ValidateHostsCreated(config, client, logger);
      AzureFrontDoorConfigurator.ValidateReservedRoutesCreated(config, client, hostsForRoute, logger);
      Endpoint endpoint = AzureFrontDoorConfigurator.EnsureEndpointCreated(config, client, trackedChanges, logger, validateOnly);
      EndpointPool endpointPool1 = AzureFrontDoorConfigurator.EnsureEndpointPoolCreated(config, client, trackedChanges, endpoint, logger, validateOnly);
      EndpointPool endpointPool2 = AzureFrontDoorConfigurator.EnsureLookupServiceEndpointPoolCreated(config, client, trackedChanges, endpointPool1, logger, validateOnly);
      RouteKeyConfiguration keyConfiguration1 = AzureFrontDoorConfigurator.EnsureRouteKeyConfigCreated(config, client, trackedChanges, logger, validateOnly);
      RouteKeyConfiguration keyConfiguration2 = AzureFrontDoorConfigurator.EnsureEndpointLookupRouteKeyConfigCreated(config, client, trackedChanges, endpointPool2, logger, validateOnly);
      AzureFrontDoorConfigurator.EnsureRoutesCreated(config, client, trackedChanges, hostsForRoute, endpointPool1, config.UseEndpointLookupRouteKey ? keyConfiguration2 : keyConfiguration1, logger, validateOnly);
      AzureFrontDoorConfigurator.EnsureErrorPageConfigured(config, client, trackedChanges, logger, validateOnly);
      if (config.CreateAlerts)
        AzureFrontDoorConfigurator.EnsurePartnerAlertCreated(config, client, trackedChanges, logger, validateOnly);
      return trackedChanges;
    }

    public static List<Guid> CleanupRoutes(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      AfdConfigData config,
      ITFLogger logger)
    {
      Endpoint existingEndpoint = AzureFrontDoorConfigurator.GetExistingEndpoint(config, client, logger);
      List<Guid> trackedChanges = new List<Guid>();
      if (existingEndpoint == null)
      {
        logger.Info("Endpoint " + config.EndpointName + " does not exist. Stopping cleanup.");
        return trackedChanges;
      }
      EndpointPool existingEndpointPool = AzureFrontDoorConfigurator.GetExistingEndpointPool(config, client, config.EndpointPool, logger);
      int num;
      if (existingEndpointPool != null)
      {
        if (existingEndpointPool.Endpoints.Count == 1)
        {
          int? id1 = existingEndpointPool.Endpoints[0].Info.Id;
          int? id2 = existingEndpoint.Id;
          num = id1.GetValueOrDefault() == id2.GetValueOrDefault() & id1.HasValue == id2.HasValue ? 1 : 0;
        }
        else
          num = 0;
      }
      else
        num = 1;
      if (num != 0)
      {
        AzureFrontDoorConfigurator.DeleteRoute(config, client, trackedChanges, logger, true);
        AzureFrontDoorConfigurator.DeleteRoute(config, client, trackedChanges, logger, false);
      }
      return trackedChanges;
    }

    public static List<Guid> CleanupEndpoints(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      AfdConfigData config,
      ITFLogger logger)
    {
      Endpoint existingEndpoint = AzureFrontDoorConfigurator.GetExistingEndpoint(config, client, logger);
      List<Guid> trackedChanges = new List<Guid>();
      if (existingEndpoint == null)
      {
        logger.Info("Endpoint " + config.EndpointName + " does not exist. Stopping cleanup.");
        return trackedChanges;
      }
      EndpointPool existingEndpointPool1 = AzureFrontDoorConfigurator.GetExistingEndpointPool(config, client, config.EndpointPool, logger);
      if (existingEndpointPool1 != null)
        AzureFrontDoorConfigurator.RemoveEndpointFromPool(config, client, trackedChanges, existingEndpointPool1, existingEndpoint, logger);
      EndpointPool existingEndpointPool2 = AzureFrontDoorConfigurator.GetExistingEndpointPool(config, client, config.LookupServiceEndpointPool, logger);
      if (existingEndpointPool2 != null)
        AzureFrontDoorConfigurator.RemoveEndpointFromPool(config, client, trackedChanges, existingEndpointPool2, existingEndpoint, logger);
      try
      {
        AzureFrontDoorConfigurator.DeleteEndpoint(config, client, trackedChanges, logger);
      }
      catch (AfdErrorResponseException ex) when (ex.ResponseCode == HttpStatusCode.Forbidden)
      {
        logger.Warning(string.Format("Endpoint {0} was not cleaned up. Exception: {1}", (object) config.EndpointName, (object) ex));
      }
      return trackedChanges;
    }

    public static List<Guid> CleanupTestOrphans(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client1,
      Tenant tenant,
      Partner partner,
      ITFLogger logger)
    {
      logger.Info("Cleaning Partner " + partner.DisplayName);
      List<Route> list1 = AzureFrontDoorConfigurator.ReadAllByPages<Route, PaginatedRoute>(client1, (Func<Microsoft.VisualStudio.Services.AfdClient.AfdClient, PaginatedRoute>) (client2 => RoutesExtensions.ReadAll(client2.Routes, tenant.Id, partner.Id)), (Func<PaginatedRoute, IList<Route>>) (page => page.Value), (Func<PaginatedRoute, PaginatedUrlReference>) (page => page.NextLink), logger).ToList<Route>();
      List<EndpointPool> list2 = AzureFrontDoorConfigurator.ReadAllByPages<EndpointPool, PaginatedEndpointPool>(client1, (Func<Microsoft.VisualStudio.Services.AfdClient.AfdClient, PaginatedEndpointPool>) (client3 => EndpointPoolsExtensions.ReadAll(client3.EndpointPools, tenant.Id, partner.Id)), (Func<PaginatedEndpointPool, IList<EndpointPool>>) (page => page.Value), (Func<PaginatedEndpointPool, PaginatedUrlReference>) (page => page.NextLink), logger).ToList<EndpointPool>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      List<Endpoint> list3 = AzureFrontDoorConfigurator.ReadAllByPages<Endpoint, PaginatedEndpoint>(client1, (Func<Microsoft.VisualStudio.Services.AfdClient.AfdClient, PaginatedEndpoint>) (client4 => EndpointsExtensions.ReadAll(client4.Endpoints, tenant.Id, partner.Id)), (Func<PaginatedEndpoint, IList<Endpoint>>) (page => page.Value), (Func<PaginatedEndpoint, PaginatedUrlReference>) (page => page.NextLink), logger).ToList<Endpoint>().Where<Endpoint>(AzureFrontDoorConfigurator.\u003C\u003EO.\u003C0\u003E__EndpointIsInvalid ?? (AzureFrontDoorConfigurator.\u003C\u003EO.\u003C0\u003E__EndpointIsInvalid = new Func<Endpoint, bool>(AzureFrontDoorConfigurator.EndpointIsInvalid))).ToList<Endpoint>();
      List<Guid> trackedChanges = new List<Guid>();
      List<EndpointPool> poolsToRemove = AzureFrontDoorConfigurator.RemoveEndpointsFromPools(client1, trackedChanges, tenant, partner, logger, (IList<EndpointPool>) list2, (IList<Endpoint>) list3);
      AzureFrontDoorConfigurator.DeleteEndpointPoolsAndRoutes(client1, trackedChanges, tenant, partner, (IList<Route>) list1, (IList<EndpointPool>) poolsToRemove, logger);
      AzureFrontDoorConfigurator.DeleteEndpoints(client1, trackedChanges, tenant, partner, list3, logger);
      logger.Info("Cleaned up test orphans in partner " + partner.DisplayName + ".");
      return trackedChanges;
    }

    private static bool EndpointIsInvalid(Endpoint endpoint)
    {
      IPAddress ip;
      if (!IPAddress.TryParse(endpoint.Hostname, out ip))
        return !AzureDnsUtils.IsValidDnsEntry(endpoint.Hostname);
      IPHostEntry entry;
      return !AzureDnsUtils.IsValidDnsEntry(endpoint.Name, out entry) || ((IEnumerable<IPAddress>) entry.AddressList).All<IPAddress>((Func<IPAddress, bool>) (a => !a.Equals((object) ip)));
    }

    private static List<EndpointPool> RemoveEndpointsFromPools(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      Tenant tenant,
      Partner partner,
      ITFLogger logger,
      IList<EndpointPool> allPools,
      IList<Endpoint> endpointsToRemove)
    {
      HashSet<int> endpointRemoveSet = new HashSet<int>(endpointsToRemove.Select<Endpoint, int>((Func<Endpoint, int>) (e => e.Id.Value)));
      List<EndpointPool> endpointPoolList = new List<EndpointPool>();
      foreach (EndpointPool allPool in (IEnumerable<EndpointPool>) allPools)
      {
        List<UrlReferenceEndpointReferenceInfo> list = allPool.Endpoints.Where<UrlReferenceEndpointReferenceInfo>((Func<UrlReferenceEndpointReferenceInfo, bool>) (e => !endpointRemoveSet.Contains(e.Info.Id.Value))).ToList<UrlReferenceEndpointReferenceInfo>();
        if (list.Count != allPool.Endpoints.Count)
        {
          if (list.Count == 0)
          {
            endpointPoolList.Add(allPool);
          }
          else
          {
            logger.Info(string.Format("Updating endpoint pool {0} with {1} endpoints.", (object) allPool.Name, (object) list.Count));
            allPool.Endpoints = (IList<UrlReferenceEndpointReferenceInfo>) list;
            HttpOperationExtensions.AddChangeIdsTo(client.EndpointPools.UpdateWithHttpMessagesAsync(allPool.Id.Value, allPool, tenant.Id, partner.Id, (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
          }
        }
      }
      return endpointPoolList;
    }

    private static void DeleteEndpointPoolsAndRoutes(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      Tenant tenant,
      Partner partner,
      IList<Route> allRoutes,
      IList<EndpointPool> poolsToRemove,
      ITFLogger logger)
    {
      HashSet<int> intSet1 = new HashSet<int>(poolsToRemove.Select<EndpointPool, int>((Func<EndpointPool, int>) (e => e.Id.Value)));
      foreach (Route allRoute in (IEnumerable<Route>) allRoutes)
      {
        IList<VariantAwareObjectItemRouteData> routeData = allRoute.RouteData;
        ForwardingData forwardingData = routeData != null ? routeData.FirstOrDefault<VariantAwareObjectItemRouteData>()?.Value?.ForwardingData : (ForwardingData) null;
        if (forwardingData != null)
        {
          HashSet<int> intSet2 = intSet1;
          int? id1 = forwardingData.EndpointPool.Info.Id;
          int num1 = id1.Value;
          if (intSet2.Contains(num1))
          {
            logger.Info("Deleting route " + allRoute.Name + " since its endpoint pool should be deleted.");
            IRoutes routes1 = client.Routes;
            id1 = allRoute.Id;
            int num2 = id1.Value;
            string id2 = tenant.Id;
            string id3 = partner.Id;
            CancellationToken cancellationToken1 = new CancellationToken();
            HttpOperationExtensions.AddChangeIdsTo(routes1.DeleteWithHttpMessagesAsync(num2, id2, id3, (Dictionary<string, List<string>>) null, cancellationToken1), trackedChanges);
            string redirectRouteName = allRoute.Name + "-HttpsRedirect";
            Route route = allRoutes.FirstOrDefault<Route>((Func<Route, bool>) (r => string.Equals(r.Name, redirectRouteName, StringComparison.OrdinalIgnoreCase)));
            if (route != null)
            {
              logger.Info("Deleting HTTPS redirect route " + route.Name + ".");
              IRoutes routes2 = client.Routes;
              id1 = route.Id;
              int num3 = id1.Value;
              string id4 = tenant.Id;
              string id5 = partner.Id;
              CancellationToken cancellationToken2 = new CancellationToken();
              HttpOperationExtensions.AddChangeIdsTo(routes2.DeleteWithHttpMessagesAsync(num3, id4, id5, (Dictionary<string, List<string>>) null, cancellationToken2), trackedChanges);
            }
          }
        }
      }
      foreach (EndpointPool endpointPool in (IEnumerable<EndpointPool>) poolsToRemove)
      {
        AzureFrontDoorConfigurator.DeleteRouteKeys(client, trackedChanges, tenant.Id, partner.Id, endpointPool, logger);
        logger.Info("Deleting endpoint pool " + endpointPool.Name + " because it contains all invalid endpoints.");
        HttpOperationExtensions.AddChangeIdsTo(client.EndpointPools.DeleteWithHttpMessagesAsync(endpointPool.Id.Value, tenant.Id, partner.Id, (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
      }
    }

    private static void DeleteEndpoints(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      Tenant tenant,
      Partner partner,
      List<Endpoint> endpointsToRemove,
      ITFLogger logger)
    {
      foreach (Endpoint endpoint in endpointsToRemove)
      {
        logger.Info("Deleting invalid endpoint " + endpoint.Name + ".");
        HttpOperationExtensions.AddChangeIdsTo(client.Endpoints.DeleteWithHttpMessagesAsync(endpoint.Id.Value, tenant.Id, partner.Id, (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
      }
    }

    private static void RemoveEndpointFromPool(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      EndpointPool endpointPool,
      Endpoint endpoint,
      ITFLogger logger)
    {
      if (endpointPool.Endpoints.Count == 1)
      {
        int? id1 = endpointPool.Endpoints[0].Info.Id;
        int? id2 = endpoint.Id;
        if (id1.GetValueOrDefault() == id2.GetValueOrDefault() & id1.HasValue == id2.HasValue)
        {
          AzureFrontDoorConfigurator.DeleteRouteKeys(client, trackedChanges, config.Tenant, config.Partner, endpointPool, logger);
          AzureFrontDoorConfigurator.DeleteEndpointPool(config, client, trackedChanges, endpointPool.Name, logger);
          return;
        }
      }
      int count = endpointPool.Endpoints.Count;
      endpointPool.Endpoints = (IList<UrlReferenceEndpointReferenceInfo>) endpointPool.Endpoints.Where<UrlReferenceEndpointReferenceInfo>((Func<UrlReferenceEndpointReferenceInfo, bool>) (e =>
      {
        int? id3 = e.Info.Id;
        int? id4 = endpoint.Id;
        return !(id3.GetValueOrDefault() == id4.GetValueOrDefault() & id3.HasValue == id4.HasValue);
      })).ToList<UrlReferenceEndpointReferenceInfo>();
      if (endpointPool.Endpoints.Count == count)
      {
        logger.Info("Endpoint " + endpoint.Name + " is already not a member of Endpoint Pool " + endpointPool.Name + ".");
      }
      else
      {
        logger.Info("Removing Endpoint " + endpoint.Name + " from Endpoint Pool " + endpointPool.Name + ".");
        HttpOperationExtensions.AddChangeIdsTo(client.EndpointPools.UpdateWithHttpMessagesAsync(endpointPool.Id.Value, endpointPool, config.Tenant, config.Partner, (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
      }
    }

    private static void DeleteRouteKeys(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client1,
      List<Guid> trackedChanges,
      string tenant,
      string partner,
      EndpointPool endpointPool,
      ITFLogger logger)
    {
      foreach (RouteKeyConfiguration readAllByPage in AzureFrontDoorConfigurator.ReadAllByPages<RouteKeyConfiguration, PaginatedRouteKeyConfiguration>(client1, (Func<Microsoft.VisualStudio.Services.AfdClient.AfdClient, PaginatedRouteKeyConfiguration>) (client2 => RouteKeyConfigurationOperationsExtensions.ReadAll(client2.RouteKeyConfiguration, tenant, partner)), (Func<PaginatedRouteKeyConfiguration, IList<RouteKeyConfiguration>>) (page => page.Value), (Func<PaginatedRouteKeyConfiguration, PaginatedUrlReference>) (page => page.NextLink), logger))
      {
        int? id1 = (int?) readAllByPage.LookupServiceStrategyConfig?.EndpointPool?.Info?.Id;
        int? id2 = endpointPool.Id;
        if (id1.GetValueOrDefault() == id2.GetValueOrDefault() & id1.HasValue == id2.HasValue)
        {
          logger.Info("Removing Route Key " + readAllByPage.Name + ".");
          IRouteKeyConfigurationOperations keyConfiguration = client1.RouteKeyConfiguration;
          string str1 = tenant;
          string str2 = partner;
          id2 = readAllByPage.Id;
          int num = id2.Value;
          CancellationToken cancellationToken = new CancellationToken();
          HttpOperationExtensions.AddChangeIdsTo(keyConfiguration.DeleteWithHttpMessagesAsync(str1, str2, num, (Dictionary<string, List<string>>) null, cancellationToken), trackedChanges);
        }
      }
    }

    private static void DeleteRoute(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      ITFLogger logger,
      bool httpsRedirect)
    {
      Route existingRoute = AzureFrontDoorConfigurator.GetExistingRoute(config, client, httpsRedirect, logger);
      if (existingRoute == null)
      {
        logger.Info("Route does not exist - nothing to delete.");
      }
      else
      {
        logger.Info("Deleting Route " + existingRoute.Name + ".");
        HttpOperationExtensions.AddChangeIdsTo(client.Routes.DeleteWithHttpMessagesAsync(existingRoute.Id.Value, config.Tenant, config.Partner, (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
      }
    }

    private static void DeleteEndpointPool(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      string endpointPoolName,
      ITFLogger logger)
    {
      EndpointPool existingEndpointPool = AzureFrontDoorConfigurator.GetExistingEndpointPool(config, client, endpointPoolName, logger);
      if (existingEndpointPool == null)
      {
        logger.Info("Endpoint Pool does not exist - nothing to delete.");
      }
      else
      {
        logger.Info("Deleting Endpoint Pool " + existingEndpointPool.Name + ".");
        HttpOperationExtensions.AddChangeIdsTo(client.EndpointPools.DeleteWithHttpMessagesAsync(existingEndpointPool.Id.Value, config.Tenant, config.Partner, (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
      }
    }

    private static void DeleteEndpoint(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      ITFLogger logger)
    {
      Endpoint existingEndpoint = AzureFrontDoorConfigurator.GetExistingEndpoint(config, client, logger);
      if (existingEndpoint == null)
      {
        logger.Info("Endpoint does not exist - nothing to delete.");
      }
      else
      {
        logger.Info("Deleting Endpoint " + existingEndpoint.Name + ".");
        HttpOperationExtensions.AddChangeIdsTo(client.Endpoints.DeleteWithHttpMessagesAsync(existingEndpoint.Id.Value, config.Tenant, config.Partner, (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
      }
    }

    private static PartnerAlert EnsurePartnerAlertCreated(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client1,
      List<Guid> trackedChanges,
      ITFLogger logger,
      bool validateOnly)
    {
      double num = config.ReliabilityAlertThreshold / 100.0 * 15.0;
      string str1 = string.Format("Fires if <{0}% of requests are successful over 15 minutes.", (object) config.ReliabilityAlertThreshold);
      IEnumerable<PartnerAlert> source = AzureFrontDoorConfigurator.ReadAllByPages<PartnerAlert, PaginatedPartnerAlert>(client1, (Func<Microsoft.VisualStudio.Services.AfdClient.AfdClient, PaginatedPartnerAlert>) (client2 => PartnerAlertsExtensions.ReadAll(client2.PartnerAlerts, config.Tenant, config.Partner)), (Func<PaginatedPartnerAlert, IList<PartnerAlert>>) (page => page.Value), (Func<PaginatedPartnerAlert, PaginatedUrlReference>) (page => page.NextLink), logger);
      string existingName = config.Partner + "_" + config.ReliabilityAlertName;
      Func<PartnerAlert, bool> predicate = (Func<PartnerAlert, bool>) (a => string.Equals(a.Name, existingName, StringComparison.OrdinalIgnoreCase));
      PartnerAlert partnerAlert1 = source.FirstOrDefault<PartnerAlert>(predicate);
      if (partnerAlert1 == null)
      {
        if (validateOnly)
          throw new Exception("AFD Configuration Invalid: Partner Alert " + existingName + " does not exist.");
        logger.Info("Creating 1 new AFD Partner Alert.");
        PartnerAlert partnerAlert2 = new PartnerAlert()
        {
          Name = config.ReliabilityAlertName,
          Description = str1,
          Enabled = config.AlertsEnabled,
          MetricName = "EdgeReliability",
          MetricNamespace = "FrontdoorV2",
          Severity = 3,
          Threshold = num,
          Aggregation = "ReliabilityPercentage",
          FrequencyInMinutes = 5,
          SamplingDataWindowInMinutes = 15,
          TsgLink = "https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki?pagePath=/Live Site/TSGs/Alerts TSGs/Framework Alerts/TSG: AzureFrontDoor - EdgeCompositeReliability&wikiVersion=GBwikiMaster",
          Dashboard = string.Format("https://www.afdcp.com/Configuration/{0}/{1}/_Monitoring", (object) config.Tenant, (object) config.Partner),
          Comparator = "<"
        };
        partnerAlert1 = HttpOperationExtensions.AddChangeIdsTo<PartnerAlert>(client1.PartnerAlerts.CreateWithHttpMessagesAsync(config.Tenant, config.Partner, partnerAlert2, (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
      }
      else if (partnerAlert1.Threshold != num || partnerAlert1.Enabled != config.AlertsEnabled)
      {
        if (validateOnly)
          throw new Exception("AFD Configuration Invalid: Partner Alert " + existingName + " has an invalid threshold or enabled setting.");
        logger.Info(string.Format("Updating Partner Alert {0} with new Threshold: {1} and Enabled: {2}.", (object) existingName, (object) num, (object) config.AlertsEnabled));
        partnerAlert1.Threshold = num;
        partnerAlert1.Description = str1;
        partnerAlert1.Enabled = config.AlertsEnabled;
        IPartnerAlerts partnerAlerts = client1.PartnerAlerts;
        string tenant = config.Tenant;
        string partner = config.Partner;
        Guid? uniqueId = partnerAlert1.UniqueId;
        ref Guid? local = ref uniqueId;
        string str2 = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
        PartnerAlert partnerAlert3 = partnerAlert1;
        CancellationToken cancellationToken = new CancellationToken();
        HttpOperationExtensions.AddChangeIdsTo(partnerAlerts.PutWithHttpMessagesAsync(tenant, partner, str2, partnerAlert3, (Dictionary<string, List<string>>) null, cancellationToken), trackedChanges);
      }
      else
        logger.Info("AFD Partner Alert is already configured.");
      return partnerAlert1;
    }

    private static void EnsureErrorPageConfigured(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      ITFLogger logger,
      bool validateOnly = false)
    {
      ErrorPage errorPage = AzureFrontDoorConfigurator.GetErrorPage();
      ErrorPageData errorPageData1 = errorPage.ErrorPageData.First<VariantAwareObjectItemErrorPageData>().Value;
      ErrorPageData errorPageData2 = AfdRetryHelper.ExecuteWithRetries<ErrorPage>((Func<ErrorPage>) (() => ErrorPageOperationsExtensions.Read(client.ErrorPage, config.Tenant, config.Partner)), logger).ErrorPageData.First<VariantAwareObjectItemErrorPageData>().Value;
      if (errorPageData1.HtmlPrologue != errorPageData2.HtmlPrologue || errorPageData1.HtmlEpilogue != errorPageData2.HtmlEpilogue || errorPageData1.DefaultErrorPageTemplate.ErrorPageStatusCode != errorPageData2.DefaultErrorPageTemplate.ErrorPageStatusCode || errorPageData1.DosErrorPageTemplate.ErrorPageStatusCode != errorPageData2.DosErrorPageTemplate.ErrorPageStatusCode)
      {
        if (validateOnly)
        {
          logger.Info("Error page is not up-to-date.");
        }
        else
        {
          logger.Info("Updating to new error page.");
          HttpOperationExtensions.AddChangeIdsTo(client.ErrorPage.UpdateWithHttpMessagesAsync(config.Tenant, config.Partner, errorPage, (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
        }
      }
      else
        logger.Info("Error page is already up to date.");
    }

    private static ErrorPage GetErrorPage()
    {
      using (Stream manifestResourceStream = typeof (AzureFrontDoorConfigurator).Assembly.GetManifestResourceStream("Microsoft.VisualStudio.Services.Cloud.AzureFrontDoor.AfdErrorPage.html"))
      {
        using (StreamReader streamReader = new StreamReader(manifestResourceStream))
        {
          string end = streamReader.ReadToEnd();
          string[] strArray = end.Contains("<!--BREAKHERE-->") ? end.Split(new string[1]
          {
            "<!--BREAKHERE-->"
          }, StringSplitOptions.RemoveEmptyEntries) : throw new InvalidOperationException("Input file must contain the case sensitive XML comment '<!--BREAKHERE-->' to separate the HTML prologue and epilogue (before and after AFD reference Ids).");
          return strArray.Length == 2 ? new ErrorPage()
          {
            ErrorPageData = (IList<VariantAwareObjectItemErrorPageData>) new List<VariantAwareObjectItemErrorPageData>()
            {
              new VariantAwareObjectItemErrorPageData()
              {
                Value = new ErrorPageData()
                {
                  HtmlPrologue = strArray[0],
                  HtmlEpilogue = strArray[1],
                  DefaultErrorPageTemplate = new ErrorPageTemplate("ServiceUnavailable"),
                  DosErrorPageTemplate = new ErrorPageTemplate("TooManyRequests")
                }
              }
            }
          } : throw new InvalidOperationException("Input file must only contain the case sensitive XML comment '<!--BREAKHERE-->' once.");
        }
      }
    }

    private static void EnsureRoutesCreated(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      List<Host> hostsForRoute,
      EndpointPool endpointPool,
      RouteKeyConfiguration routeKeyConfig,
      ITFLogger logger,
      bool validateOnly = false)
    {
      AzureFrontDoorConfigurator.EnsureRouteCreated(config, client, trackedChanges, hostsForRoute, endpointPool, routeKeyConfig, logger, false, validateOnly);
      if (!config.UseHttpsRedirectRoutes)
        return;
      AzureFrontDoorConfigurator.EnsureRouteCreated(config, client, trackedChanges, hostsForRoute, endpointPool, routeKeyConfig, logger, true, validateOnly);
    }

    private static Route EnsureRouteCreated(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      List<Host> hostsForRoute,
      EndpointPool endpointPool,
      RouteKeyConfiguration routeKeyConfig,
      ITFLogger logger,
      bool httpsRedirect,
      bool validateOnly = false)
    {
      if (!config.UseHttpsRedirectRoutes & httpsRedirect)
        throw new InvalidOperationException("Cannot create HttpsRedirect routes when AzureFrontDoorUseHttpsRedirectRoutes is disabled.");
      string routeName = config.GetRouteName(httpsRedirect);
      Route existingRoute = AzureFrontDoorConfigurator.GetExistingRoute(config, client, httpsRedirect, logger);
      if (existingRoute == null)
      {
        if (validateOnly)
          throw new Exception("AFD Configuration Invalid: Route " + routeName + " does not exist.");
        logger.Info(string.Format("Creating 1 new AFD Route with {0} Hosts.", (object) hostsForRoute.Count));
        List<UrlReferenceHostReferenceInfo> list = hostsForRoute.Select<Host, UrlReferenceHostReferenceInfo>((Func<Host, UrlReferenceHostReferenceInfo>) (h => new UrlReferenceHostReferenceInfo(new ReferenceInfo(config.Partner, config.Tenant, h.Id), (string) null))).ToList<UrlReferenceHostReferenceInfo>();
        List<string> stringList1;
        if (!config.UseHttpsRedirectRoutes)
        {
          stringList1 = new List<string>()
          {
            "Http",
            "Https"
          };
        }
        else
        {
          stringList1 = new List<string>();
          stringList1.Add(httpsRedirect ? "Http" : "Https");
        }
        List<string> stringList2 = stringList1;
        RouteData routeData;
        if (httpsRedirect)
          routeData = new RouteData()
          {
            RouteType = "Redirect",
            RedirectData = new RedirectData()
            {
              RedirectType = "Found",
              DestinationScheme = "Https",
              DestinationHost = "",
              DestinationPath = config.HttpsRedirectDestinationPath,
              PreservePath = new bool?(true),
              PreserveQueryString = new bool?(true)
            }
          };
        else
          routeData = new RouteData()
          {
            RouteType = "Forwarding",
            ForwardingData = new ForwardingData()
            {
              Protocol = "Https",
              EndpointPool = new UrlReferenceEndpointPoolReferenceInfo(new ReferenceInfo(config.Partner, config.Tenant, endpointPool.Id), (string) null),
              SupportsWebSockets = new bool?(config.UseWebSockets)
            },
            RouteKeyConfiguration = new UrlReferenceRouteKeyConfigurationReferenceInfo(new ReferenceInfo(config.Partner, config.Tenant, routeKeyConfig.Id), (string) null)
          };
        List<VariantAwareObjectItemRouteData> objectItemRouteDataList = new List<VariantAwareObjectItemRouteData>()
        {
          new VariantAwareObjectItemRouteData((IList<VariantFilter>) null, routeData)
        };
        Route route = new Route(routeName, (IList<UrlReferenceHostReferenceInfo>) list, (IList<string>) config.Paths, (IList<string>) stringList2, (IList<VariantAwareObjectItemRouteData>) objectItemRouteDataList, new bool?(), new bool?(), new bool?(), (UrlReferenceWorkflowReferenceInfo) null, new int?());
        existingRoute = HttpOperationExtensions.AddChangeIdsTo<Route>(client.Routes.CreateWithHttpMessagesAsync(route, config.Tenant, config.Partner, (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
      }
      else
      {
        bool flag = false;
        List<UrlReferenceHostReferenceInfo> list1 = hostsForRoute.Where<Host>((Func<Host, bool>) (h => existingRoute.Hosts.All<UrlReferenceHostReferenceInfo>((Func<UrlReferenceHostReferenceInfo, bool>) (rh =>
        {
          int? id1 = rh.Info.Id;
          int? id2 = h.Id;
          return !(id1.GetValueOrDefault() == id2.GetValueOrDefault() & id1.HasValue == id2.HasValue);
        })))).Select<Host, UrlReferenceHostReferenceInfo>((Func<Host, UrlReferenceHostReferenceInfo>) (h => new UrlReferenceHostReferenceInfo(new ReferenceInfo(config.Partner, config.Tenant, h.Id), (string) null))).ToList<UrlReferenceHostReferenceInfo>();
        if (list1.Count > 0)
        {
          if (validateOnly)
            throw new Exception("AFD Configuration Invalid: Route " + routeName + " does not have proper Hosts.");
          logger.Info(string.Format("Adding {0} hosts to AFD Route.", (object) list1.Count));
          foreach (UrlReferenceHostReferenceInfo hostReferenceInfo in list1)
            existingRoute.Hosts.Add(hostReferenceInfo);
          flag = true;
        }
        else
          logger.Info("AFD Route hosts are already configured.");
        List<string> list2 = config.Paths.Except<string>((IEnumerable<string>) existingRoute.Paths, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>();
        if (list2.Count > 0)
        {
          if (validateOnly)
            throw new Exception("AFD Configuration Invalid: Route " + routeName + " does not have proper paths.");
          logger.Info(string.Format("Adding {0} paths to AFD Route.", (object) list2.Count));
          foreach (string str in list2)
            existingRoute.Paths.Add(str);
          flag = true;
        }
        else
          logger.Info("AFD Route paths are already configured.");
        foreach (VariantAwareObjectItemRouteData objectItemRouteData in (IEnumerable<VariantAwareObjectItemRouteData>) existingRoute.RouteData)
        {
          ForwardingData forwardingData = objectItemRouteData.Value.ForwardingData;
          if (forwardingData != null)
          {
            bool? supportsWebSockets = forwardingData.SupportsWebSockets;
            bool useWebSockets = config.UseWebSockets;
            if (!(supportsWebSockets.GetValueOrDefault() == useWebSockets & supportsWebSockets.HasValue))
            {
              if (validateOnly)
              {
                logger.Info("Route " + routeName + " does not have the expected value for SupportsWebSockets.");
              }
              else
              {
                forwardingData.SupportsWebSockets = new bool?(config.UseWebSockets);
                flag = true;
              }
            }
          }
          UrlReferenceRouteKeyConfigurationReferenceInfo keyConfiguration = objectItemRouteData.Value.RouteKeyConfiguration;
          if (keyConfiguration != null)
          {
            int? id3 = keyConfiguration.Info.Id;
            int? id4 = routeKeyConfig.Id;
            if (!(id3.GetValueOrDefault() == id4.GetValueOrDefault() & id3.HasValue == id4.HasValue))
            {
              if (validateOnly)
              {
                logger.Info("Route " + routeName + " does not have the expected RouteKeyConfiguration.");
              }
              else
              {
                objectItemRouteData.Value.RouteKeyConfiguration = new UrlReferenceRouteKeyConfigurationReferenceInfo(new ReferenceInfo(config.Partner, config.Tenant, routeKeyConfig.Id), (string) null);
                flag = true;
              }
            }
          }
        }
        if (flag)
        {
          logger.Info("Updating route " + routeName + ".");
          HttpOperationExtensions.AddChangeIdsTo(client.Routes.UpdateWithHttpMessagesAsync(existingRoute.Id.Value, existingRoute, config.Tenant, config.Partner, (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
        }
      }
      return existingRoute;
    }

    private static Route GetExistingRoute(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client1,
      bool httpsRedirect,
      ITFLogger logger)
    {
      return AzureFrontDoorConfigurator.ReadAllByPages<Route, PaginatedRoute>(client1, (Func<Microsoft.VisualStudio.Services.AfdClient.AfdClient, PaginatedRoute>) (client2 => RoutesExtensions.ReadAll(client2.Routes, config.Tenant, config.Partner)), (Func<PaginatedRoute, IList<Route>>) (page => page.Value), (Func<PaginatedRoute, PaginatedUrlReference>) (page => page.NextLink), logger).FirstOrDefault<Route>((Func<Route, bool>) (r => string.Equals(r.Name, config.GetRouteName(httpsRedirect), StringComparison.OrdinalIgnoreCase)));
    }

    private static RouteKeyConfiguration EnsureRouteKeyConfigCreated(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      ITFLogger logger,
      bool validateOnly = false)
    {
      RouteKeyConfiguration routeKey = new RouteKeyConfiguration()
      {
        Name = "EndpointResponseRouteKey",
        LookupType = "CacheLookup",
        LookupFailureAction = "ForwardToDefaultEndpointPool",
        RouteTupleUnhealthyRoutingAction = AzureFrontDoorConfigurator.GetUnhealthyRoutingAction(config),
        RouteTupleEndpointSelectionStrategy = "FirstHealthyEndpoint"
      };
      return AzureFrontDoorConfigurator.EnsureRouteKeyConfigCreated(config, client, trackedChanges, routeKey, logger, validateOnly);
    }

    private static RouteKeyConfiguration EnsureEndpointLookupRouteKeyConfigCreated(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      EndpointPool endpointPool,
      ITFLogger logger,
      bool validateOnly = false)
    {
      RouteKeyConfiguration routeKey = new RouteKeyConfiguration()
      {
        Name = "EndpointLookupRouteKey-" + endpointPool.Name,
        LookupType = "CacheLookupAndRemoteLookupService",
        LookupServiceStrategyConfig = new LookupServiceStrategyConfig()
        {
          Path = config.LookupServiceEndpoint,
          EndpointPool = new UrlReferenceEndpointPoolReferenceInfo(new ReferenceInfo(config.Partner, config.Tenant, endpointPool.Id), (string) null),
          TimeoutInMs = 10000,
          UseHttps = false
        },
        LookupFailureAction = "ForwardToDefaultEndpointPool",
        RouteTupleUnhealthyRoutingAction = AzureFrontDoorConfigurator.GetUnhealthyRoutingAction(config),
        RouteTupleEndpointSelectionStrategy = "FirstHealthyEndpoint"
      };
      return AzureFrontDoorConfigurator.EnsureRouteKeyConfigCreated(config, client, trackedChanges, routeKey, logger, validateOnly);
    }

    private static string GetUnhealthyRoutingAction(AfdConfigData config) => !config.Partner.EndsWith("test", StringComparison.OrdinalIgnoreCase) ? "FailRequest" : "LoadBalancingBasedRouting";

    private static RouteKeyConfiguration EnsureRouteKeyConfigCreated(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client1,
      List<Guid> trackedChanges,
      RouteKeyConfiguration routeKey,
      ITFLogger logger,
      bool validateOnly = false)
    {
      RouteKeyConfiguration keyConfiguration = AzureFrontDoorConfigurator.ReadAllByPages<RouteKeyConfiguration, PaginatedRouteKeyConfiguration>(client1, (Func<Microsoft.VisualStudio.Services.AfdClient.AfdClient, PaginatedRouteKeyConfiguration>) (client2 => RouteKeyConfigurationOperationsExtensions.ReadAll(client2.RouteKeyConfiguration, config.Tenant, config.Partner)), (Func<PaginatedRouteKeyConfiguration, IList<RouteKeyConfiguration>>) (page => page.Value), (Func<PaginatedRouteKeyConfiguration, PaginatedUrlReference>) (page => page.NextLink), logger).FirstOrDefault<RouteKeyConfiguration>((Func<RouteKeyConfiguration, bool>) (k => string.Equals(k.Name, routeKey.Name, StringComparison.OrdinalIgnoreCase)));
      if (keyConfiguration == null)
      {
        if (validateOnly)
          throw new Exception("AFD Configuration Invalid: Route Key Configuration " + routeKey.Name + " does not exist.");
        logger.Info("Creating 1 new Route Key Configuration");
        keyConfiguration = HttpOperationExtensions.AddChangeIdsTo<RouteKeyConfiguration>(client1.RouteKeyConfiguration.CreateWithHttpMessagesAsync(routeKey, config.Tenant, config.Partner, (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
      }
      else if (keyConfiguration.LookupFailureAction != routeKey.LookupFailureAction || keyConfiguration.RouteTupleUnhealthyRoutingAction != routeKey.RouteTupleUnhealthyRoutingAction || AzureFrontDoorConfigurator.ShouldUpdateLookupServiceStrategyConfig(keyConfiguration.LookupServiceStrategyConfig, routeKey.LookupServiceStrategyConfig))
      {
        if (validateOnly)
        {
          logger.Warning("Route Key Configuration " + keyConfiguration.Name + " needs to be updated using Configure-Afd.");
        }
        else
        {
          keyConfiguration.LookupFailureAction = routeKey.LookupFailureAction;
          keyConfiguration.RouteTupleUnhealthyRoutingAction = routeKey.RouteTupleUnhealthyRoutingAction;
          if (keyConfiguration.LookupServiceStrategyConfig == null || routeKey.LookupServiceStrategyConfig == null)
          {
            keyConfiguration.LookupServiceStrategyConfig = routeKey.LookupServiceStrategyConfig;
          }
          else
          {
            keyConfiguration.LookupServiceStrategyConfig.Path = routeKey.LookupServiceStrategyConfig.Path;
            keyConfiguration.LookupServiceStrategyConfig.TimeoutInMs = routeKey.LookupServiceStrategyConfig.TimeoutInMs;
          }
          HttpOperationExtensions.AddChangeIdsTo(client1.RouteKeyConfiguration.UpdateWithHttpMessagesAsync(keyConfiguration, config.Tenant, config.Partner, keyConfiguration.Id.Value.ToString(), (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
        }
      }
      else
        logger.Info("Route Key Configuration is already configured.");
      return keyConfiguration;
    }

    private static bool ShouldUpdateLookupServiceStrategyConfig(
      LookupServiceStrategyConfig existingConfig,
      LookupServiceStrategyConfig newConfig)
    {
      if (existingConfig == null && newConfig == null)
        return false;
      return existingConfig == null || newConfig == null || !string.Equals(existingConfig.Path, newConfig.Path, StringComparison.OrdinalIgnoreCase) || existingConfig.TimeoutInMs != newConfig.TimeoutInMs;
    }

    private static EndpointPool EnsureEndpointPoolCreated(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      Endpoint endpoint,
      ITFLogger logger,
      bool validateOnly = false)
    {
      Func<EndpointPool, bool> updateEndpoints = (Func<EndpointPool, bool>) (endpointPool =>
      {
        if (!endpointPool.Endpoints.All<UrlReferenceEndpointReferenceInfo>((Func<UrlReferenceEndpointReferenceInfo, bool>) (e =>
        {
          int? id3 = e.Info.Id;
          int? id4 = endpoint.Id;
          return !(id3.GetValueOrDefault() == id4.GetValueOrDefault() & id3.HasValue == id4.HasValue);
        })))
          return false;
        endpointPool.Endpoints.Add(new UrlReferenceEndpointReferenceInfo(new ReferenceInfo(config.Partner, config.Tenant, endpoint.Id), (string) null));
        return true;
      });
      return AzureFrontDoorConfigurator.EnsureEndpointPoolCreated(config, client, trackedChanges, config.EndpointPool, config.HealthCheckEndpoint, updateEndpoints, logger, validateOnly);
    }

    private static EndpointPool EnsureLookupServiceEndpointPoolCreated(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      EndpointPool endpointPool,
      ITFLogger logger,
      bool validateOnly = false)
    {
      Func<EndpointPool, bool> updateEndpoints = (Func<EndpointPool, bool>) (lookupEndpointPool =>
      {
        int num = lookupEndpointPool.Endpoints.Select<UrlReferenceEndpointReferenceInfo, int?>((Func<UrlReferenceEndpointReferenceInfo, int?>) (e => e.Info.Id)).Intersect<int?>(endpointPool.Endpoints.Select<UrlReferenceEndpointReferenceInfo, int?>((Func<UrlReferenceEndpointReferenceInfo, int?>) (e => e.Info.Id))).Count<int?>();
        if (num >= lookupEndpointPool.Endpoints.Count && num >= endpointPool.Endpoints.Count)
          return false;
        lookupEndpointPool.Endpoints = endpointPool.Endpoints;
        return true;
      });
      return AzureFrontDoorConfigurator.EnsureEndpointPoolCreated(config, client, trackedChanges, config.LookupServiceEndpointPool, config.LookupServiceHealthCheckEndpoint, updateEndpoints, logger, validateOnly);
    }

    private static EndpointPool EnsureEndpointPoolCreated(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      string endpointPoolName,
      string healthCheckEndpoint,
      Func<EndpointPool, bool> updateEndpoints,
      ITFLogger logger,
      bool validateOnly = false)
    {
      int? nullable1 = config.HealthProbeInterval;
      int num1 = nullable1 ?? 15;
      EndpointPool existingEndpointPool = AzureFrontDoorConfigurator.GetExistingEndpointPool(config, client, endpointPoolName, logger);
      if (existingEndpointPool == null)
      {
        if (validateOnly)
          throw new Exception("AFD Configuration Invalid: EndpointPool " + endpointPoolName + " does not exist.");
        logger.Info("Creating 1 new AFD Endpoint Pool.");
        string str1 = healthCheckEndpoint;
        nullable1 = new int?(num1);
        bool? nullable2 = new bool?(false);
        int? nullable3 = nullable1;
        int? nullable4 = new int?();
        HealthProbeSettings healthProbeSettings1 = new HealthProbeSettings(str1, nullable2, nullable3, "GET", nullable4);
        LoadBalancingSettings balancingSettings1 = new LoadBalancingSettings()
        {
          Algorithm = "CapacityBased",
          SampleSize = new int?(4),
          SuccessfulSamplesRequired = new int?(1),
          LatencyPercentile = "Percentile50",
          QualityOfResponseFailbackTestingTrafficPercentage = new int?(0)
        };
        InlineErrorDetectionSettings detectionSettings1 = new InlineErrorDetectionSettings()
        {
          DetectedErrorTypes = "None",
          FailoverThresholdPercentage = new int?(0)
        };
        SetApplicationEndpointSettings endpointSettings1 = new SetApplicationEndpointSettings(new bool?(false), (IList<string>) null);
        string str2 = endpointPoolName;
        List<UrlReferenceEndpointReferenceInfo> endpointReferenceInfoList = new List<UrlReferenceEndpointReferenceInfo>();
        LoadBalancingSettings balancingSettings2 = balancingSettings1;
        HealthProbeSettings healthProbeSettings2 = healthProbeSettings1;
        InlineErrorDetectionSettings detectionSettings2 = detectionSettings1;
        SetApplicationEndpointSettings endpointSettings2 = endpointSettings1;
        nullable1 = new int?();
        int? nullable5 = nullable1;
        EndpointPool endpointPool = new EndpointPool(str2, (IList<UrlReferenceEndpointReferenceInfo>) endpointReferenceInfoList, balancingSettings2, healthProbeSettings2, detectionSettings2, endpointSettings2, nullable5);
        int num2 = updateEndpoints(endpointPool) ? 1 : 0;
        existingEndpointPool = HttpOperationExtensions.AddChangeIdsTo<EndpointPool>(client.EndpointPools.CreateWithHttpMessagesAsync(endpointPool, config.Tenant, config.Partner, (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
      }
      else
      {
        bool flag = updateEndpoints(existingEndpointPool);
        nullable1 = existingEndpointPool.HealthProbeSettings.IntervalInSeconds;
        int num3 = num1;
        if (nullable1.GetValueOrDefault() == num3 & nullable1.HasValue)
        {
          nullable1 = existingEndpointPool.LoadBalancingSettings.SampleSize;
          int num4 = 4;
          if (nullable1.GetValueOrDefault() == num4 & nullable1.HasValue)
          {
            nullable1 = existingEndpointPool.LoadBalancingSettings.SuccessfulSamplesRequired;
            int num5 = 1;
            if (nullable1.GetValueOrDefault() == num5 & nullable1.HasValue)
              goto label_8;
          }
        }
        existingEndpointPool.HealthProbeSettings.IntervalInSeconds = new int?(num1);
        existingEndpointPool.LoadBalancingSettings.SampleSize = new int?(4);
        existingEndpointPool.LoadBalancingSettings.SuccessfulSamplesRequired = new int?(1);
        flag = true;
label_8:
        if (flag)
        {
          if (validateOnly)
          {
            logger.Warning("AFD Endpoint Pool " + existingEndpointPool.Name + " needs to be updated using Configure-Afd.");
          }
          else
          {
            logger.Info("Updating AFD Endpoint Pool.");
            IEndpointPools endpointPools = client.EndpointPools;
            nullable1 = existingEndpointPool.Id;
            int num6 = nullable1.Value;
            EndpointPool endpointPool = existingEndpointPool;
            string tenant = config.Tenant;
            string partner = config.Partner;
            CancellationToken cancellationToken = new CancellationToken();
            HttpOperationExtensions.AddChangeIdsTo(endpointPools.UpdateWithHttpMessagesAsync(num6, endpointPool, tenant, partner, (Dictionary<string, List<string>>) null, cancellationToken), trackedChanges);
          }
        }
        else
          logger.Info("AFD Endpoint Pool is already configured.");
      }
      return existingEndpointPool;
    }

    private static EndpointPool GetExistingEndpointPool(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client1,
      string endpointPoolName,
      ITFLogger logger)
    {
      return AzureFrontDoorConfigurator.ReadAllByPages<EndpointPool, PaginatedEndpointPool>(client1, (Func<Microsoft.VisualStudio.Services.AfdClient.AfdClient, PaginatedEndpointPool>) (client2 => EndpointPoolsExtensions.ReadAll(client2.EndpointPools, config.Tenant, config.Partner)), (Func<PaginatedEndpointPool, IList<EndpointPool>>) (page => page.Value), (Func<PaginatedEndpointPool, PaginatedUrlReference>) (page => page.NextLink), logger).FirstOrDefault<EndpointPool>((Func<EndpointPool, bool>) (p => string.Equals(p.Name, endpointPoolName, StringComparison.OrdinalIgnoreCase)));
    }

    private static Endpoint EnsureEndpointCreated(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      List<Guid> trackedChanges,
      ITFLogger logger,
      bool validateOnly = false)
    {
      Endpoint existingEndpoint = AzureFrontDoorConfigurator.GetExistingEndpoint(config, client, logger);
      if (existingEndpoint == null)
      {
        if (validateOnly)
          throw new Exception("AFD Configuration Invalid: Endpoint " + config.EndpointName + " does not exist.");
        logger.Info("Creating 1 new AFD Endpoint.");
        string endpointName = config.EndpointName;
        string endpointAddress = config.EndpointAddress;
        int? nullable1 = new int?(80);
        int? nullable2 = new int?(443);
        bool? nullable3 = new bool?(true);
        bool? nullable4 = new bool?(false);
        int? nullable5 = new int?();
        bool? nullable6 = nullable4;
        int? nullable7 = new int?();
        int? nullable8 = new int?();
        int? nullable9 = new int?();
        Endpoint endpoint = new Endpoint(endpointName, endpointAddress, nullable1, nullable2, nullable3, nullable5, nullable6, (string) null, (string) null, (string) null, nullable7, nullable8, nullable9);
        existingEndpoint = HttpOperationExtensions.AddChangeIdsTo<Endpoint>(client.Endpoints.CreateWithHttpMessagesAsync(endpoint, config.Tenant, config.Partner, (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
      }
      else if (!existingEndpoint.Hostname.Equals(config.EndpointAddress, StringComparison.OrdinalIgnoreCase))
      {
        if (validateOnly)
          throw new Exception("AFD Configuration Invalid: Endpoint " + existingEndpoint.Name + " hostname is invalid.");
        logger.Info("Updating 1 AFD Endpoint");
        existingEndpoint.Hostname = config.EndpointAddress;
        existingEndpoint.EnforceCertificateSubjectNameCheck = new bool?(false);
        HttpOperationExtensions.AddChangeIdsTo(client.Endpoints.PutWithHttpMessagesAsync(existingEndpoint.Id.Value, existingEndpoint, config.Tenant, config.Partner, (Dictionary<string, List<string>>) null, new CancellationToken()), trackedChanges);
      }
      else
        logger.Info("AFD Endpoint is already configured.");
      return existingEndpoint;
    }

    private static Endpoint GetExistingEndpoint(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client1,
      ITFLogger logger)
    {
      return AzureFrontDoorConfigurator.ReadAllByPages<Endpoint, PaginatedEndpoint>(client1, (Func<Microsoft.VisualStudio.Services.AfdClient.AfdClient, PaginatedEndpoint>) (client2 => EndpointsExtensions.ReadAll(client2.Endpoints, config.Tenant, config.Partner)), (Func<PaginatedEndpoint, IList<Endpoint>>) (page => page.Value), (Func<PaginatedEndpoint, PaginatedUrlReference>) (page => page.NextLink), logger).FirstOrDefault<Endpoint>((Func<Endpoint, bool>) (e => string.Equals(e.Name, config.EndpointName, StringComparison.OrdinalIgnoreCase)));
    }

    private static void ValidateReservedRoutesCreated(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client1,
      List<Host> hostsForRoute,
      ITFLogger logger)
    {
      List<ReservedRoute> reservedRoutes = AzureFrontDoorConfigurator.ReadAllByPages<ReservedRoute, PaginatedReservedRoute>(client1, (Func<Microsoft.VisualStudio.Services.AfdClient.AfdClient, PaginatedReservedRoute>) (client2 => ReservedRoutesExtensions.ReadAll(client2.ReservedRoutes, config.Tenant)), (Func<PaginatedReservedRoute, IList<ReservedRoute>>) (page => page.Value), (Func<PaginatedReservedRoute, PaginatedUrlReference>) (page => page.NextLink), logger).ToList<ReservedRoute>();
      List<string> paths = config.Paths;
      if (reservedRoutes.Where<ReservedRoute>((Func<ReservedRoute, bool>) (r => !string.Equals(r.PartnerId, config.Partner, StringComparison.OrdinalIgnoreCase) && r.Hosts.Any<UrlReferenceHostReferenceInfo>((Func<UrlReferenceHostReferenceInfo, bool>) (h => hostsForRoute.Any<Host>((Func<Host, bool>) (ah =>
      {
        int? id1 = ah.Id;
        int? id2 = h.Info.Id;
        return id1.GetValueOrDefault() == id2.GetValueOrDefault() & id1.HasValue == id2.HasValue;
      })))) && r.Paths.Any<string>((Func<string, bool>) (p => paths.Any<string>((Func<string, bool>) (ap => string.Equals(p, ap, StringComparison.OrdinalIgnoreCase))))))).Any<ReservedRoute>())
        throw new Exception("AFD Configuration Invalid: One or more Hosts are already reserved by other Partners.");
      if (hostsForRoute.Where<Host>((Func<Host, bool>) (h => reservedRoutes.SelectMany<ReservedRoute, UrlReferenceHostReferenceInfo>((Func<ReservedRoute, IEnumerable<UrlReferenceHostReferenceInfo>>) (v => (IEnumerable<UrlReferenceHostReferenceInfo>) v.Hosts)).All<UrlReferenceHostReferenceInfo>((Func<UrlReferenceHostReferenceInfo, bool>) (rh =>
      {
        int? id3 = rh.Info.Id;
        int? id4 = h.Id;
        return !(id3.GetValueOrDefault() == id4.GetValueOrDefault() & id3.HasValue == id4.HasValue);
      })))).Select<Host, UrlReferenceHostReferenceInfo>((Func<Host, UrlReferenceHostReferenceInfo>) (h => new UrlReferenceHostReferenceInfo(new ReferenceInfo(config.Partner, config.Tenant, h.Id), (string) null))).ToList<UrlReferenceHostReferenceInfo>().Count > 0)
        throw new Exception("AFD Configuration Invalid: Reserved Routes for Hosts are not configured for Partner " + config.Partner + ".");
      logger.Info("AFD Reserved Routes are already configured.");
    }

    private static List<Host> ValidateHostsCreated(
      AfdConfigData config,
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client1,
      ITFLogger logger)
    {
      HashSet<string> configuredHostNames = new HashSet<string>((IEnumerable<string>) config.Hosts, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<Host> list = AzureFrontDoorConfigurator.ReadAllByPages<Host, PaginatedHost>(client1, (Func<Microsoft.VisualStudio.Services.AfdClient.AfdClient, PaginatedHost>) (client2 => HostsExtensions.ReadAll(client2.Hosts, config.Tenant, (string) null)), (Func<PaginatedHost, IList<Host>>) (page => page.Value), (Func<PaginatedHost, PaginatedUrlReference>) (page => page.NextLink), logger).Where<Host>((Func<Host, bool>) (afdHost => configuredHostNames.Contains<string>(afdHost.Hostname, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))).ToList<Host>();
      if (configuredHostNames.Except<string>(list.Select<Host, string>((Func<Host, string>) (h => h.Hostname)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Any<string>())
        throw new Exception("AFD Configuration Invalid: One or more Hosts are not configured for Tenant " + config.Tenant + ".");
      logger.Info("AFD Hosts are already configured.");
      return list;
    }

    private static IEnumerable<T> ReadAllByPages<T, TPaginated>(
      Microsoft.VisualStudio.Services.AfdClient.AfdClient client,
      Func<Microsoft.VisualStudio.Services.AfdClient.AfdClient, TPaginated> readAll,
      Func<TPaginated, IList<T>> pageToItems,
      Func<TPaginated, PaginatedUrlReference> pageToNextPageUrl,
      ITFLogger logger)
    {
      TPaginated page = AfdRetryHelper.ExecuteWithRetries<TPaginated>((Func<TPaginated>) (() => readAll(client)), logger);
      foreach (T obj in (IEnumerable<T>) pageToItems(page))
        yield return obj;
      PaginatedUrlReference nextPageUrl = pageToNextPageUrl(page);
      while (nextPageUrl != null)
      {
        logger.Info("AFD page read at $" + nextPageUrl.Url);
        string result;
        using (HttpRequestMessage request = new HttpRequestMessage()
        {
          Method = HttpMethod.Get,
          RequestUri = new Uri(nextPageUrl.Url)
        })
        {
          using (HttpResponseMessage httpResponseMessage = AfdRetryHelper.ExecuteWithRetries<HttpResponseMessage>((Func<HttpResponseMessage>) (() => ((ServiceClient<Microsoft.VisualStudio.Services.AfdClient.AfdClient>) client).HttpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult()), logger))
            result = httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        page = JsonUtilities.Deserialize<TPaginated>(result);
        IList<T> objList = pageToItems(page);
        nextPageUrl = pageToNextPageUrl(page);
        foreach (T obj in (IEnumerable<T>) objList)
          yield return obj;
      }
    }

    public static Microsoft.VisualStudio.Services.AfdClient.AfdClient GetAfdClient(ITFLogger logger)
    {
      AuthenticationResult authResult = ((AzureTokenProviderBase) new AzureTokenProvider("https://login.microsoftonline.com", "microsoft.com", "https://msedge.corp.microsoft.com", "6774a32b-c8be-4b8f-97ba-9146888d2658", "urn:ietf:wg:oauth:2.0:oob", false, logger)).GetAuthResult();
      logger.Info("Authenticating to AFD through user with Unique Id: " + authResult.UniqueId + " and Given Name: " + authResult.Account?.Username);
      return new Microsoft.VisualStudio.Services.AfdClient.AfdClient(new DelegatingHandler[3]
      {
        (DelegatingHandler) new TopNHandler(5000),
        (DelegatingHandler) new AfdErrorResponseHandler(),
        (DelegatingHandler) new AccessTokenAuthHandler((HttpMessageHandler) new HttpClientHandler(), authResult.TokenType, authResult.AccessToken)
      });
    }
  }
}
