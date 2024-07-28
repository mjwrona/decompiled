// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.AuditOptInUtils
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.Redis;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class AuditOptInUtils
  {
    private const string CacheNamespaceId = "CD15398C-7A93-4B0D-AAF3-1277C1442A99";

    internal static bool GetOrAddOptInStatusFromCache(
      IVssRequestContext requestContext,
      Guid hostId,
      TimeSpan cacheTTL,
      string traceArea,
      string traceLayer)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IMutableDictionaryCacheContainer<Guid, bool> container = (IMutableDictionaryCacheContainer<Guid, bool>) null;
      try
      {
        IRedisCacheService service = vssRequestContext.GetService<IRedisCacheService>();
        if (service.IsEnabled(vssRequestContext))
        {
          ContainerSettings settings = new ContainerSettings()
          {
            KeyExpiry = new TimeSpan?(cacheTTL),
            CiAreaName = traceArea
          };
          container = service.GetVolatileDictionaryContainer<Guid, bool, AuditOptInUtils.AuditOptInSecurityToken>(vssRequestContext, new Guid("CD15398C-7A93-4B0D-AAF3-1277C1442A99"), settings);
          bool inStatusFromCache;
          if (container.TryGet<Guid, bool>(vssRequestContext, hostId, out inStatusFromCache))
          {
            requestContext.Trace(31000123, TraceLevel.Verbose, traceArea, traceLayer, string.Format("Audit opt-in status cache hit with opt-in status of {0}.", (object) inStatusFromCache));
            return inStatusFromCache;
          }
          requestContext.Trace(31000124, TraceLevel.Verbose, traceArea, traceLayer, "Audit opt-in status cache miss.");
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(31000120, TraceLevel.Error, traceArea, traceLayer, ex);
      }
      bool fromPolicyService = AuditOptInUtils.GetOptInStatusFromPolicyService(requestContext, hostId, traceArea, traceLayer);
      try
      {
        if (container != null)
          container.Set(vssRequestContext, (IDictionary<Guid, bool>) new Dictionary<Guid, bool>()
          {
            [hostId] = fromPolicyService
          });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(31000121, TraceLevel.Error, traceArea, traceLayer, ex);
      }
      return fromPolicyService;
    }

    private static bool GetOptInStatusFromPolicyService(
      IVssRequestContext requestContext,
      Guid hostId,
      string traceArea,
      string traceLayer)
    {
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && !requestContext.IsOrganizationAadBacked())
          return false;
        if (!requestContext.IsFeatureEnabled(FrameworkServerConstants.AuditNoOptedOutDeliveriesFeatureFlag))
          return true;
        Uri collectionUrl = AuditOptInUtils.GetCollectionUrl(requestContext, hostId, ServiceInstanceTypes.SPS);
        Policy policy = AuditOptInUtils.GetServiceHttpClient<OrganizationPolicyHttpClient>(requestContext, collectionUrl).GetPolicyAsync("Policy.LogAuditEvents", false.ToString()).SyncResult<Policy>();
        return policy.EffectiveValue == null || !(policy.EffectiveValue is string effectiveValue) || bool.Parse(effectiveValue);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(31000122, TraceLevel.Error, traceArea, traceLayer, ex);
        return true;
      }
    }

    private static Uri GetCollectionUrl(
      IVssRequestContext requestContext,
      Guid collectionId,
      Guid serviceTypeIdentifier)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Uri hostUri = vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, collectionId, serviceTypeIdentifier);
      return !(hostUri == (Uri) null) ? hostUri : throw new ArgumentException(string.Format("Unable to resolve a uri for the service type '{0}' and collection id '{1}'", (object) serviceTypeIdentifier, (object) collectionId));
    }

    private static T GetServiceHttpClient<T>(IVssRequestContext requestContext, Uri endpoint) where T : VssHttpClientBase
    {
      ApiResourceLocationCollection resourceLocations = requestContext.GetService<ILocationService>().GetResourceLocations(requestContext);
      return (requestContext.ClientProvider as ICreateClient).CreateClient<T>(requestContext, endpoint, nameof (AuditOptInUtils), resourceLocations);
    }

    internal sealed class AuditOptInSecurityToken
    {
    }
  }
}
