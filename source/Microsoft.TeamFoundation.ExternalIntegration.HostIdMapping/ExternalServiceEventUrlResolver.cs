// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.ExternalServiceEventUrlResolver
// Assembly: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D6C9F672-10C9-4D5D-90D5-E07E56C1D4C0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping
{
  [ExtensionPriority(50)]
  [ExtensionStrategy("Hosted")]
  public abstract class ExternalServiceEventUrlResolver : IUrlHostResolver
  {
    private string m_domain;

    protected virtual string Layer => nameof (ExternalServiceEventUrlResolver);

    protected virtual string Area => TracingPoints.ExternalServiceEventUrlHostRoutingArea;

    protected abstract string AccessMappingName { get; }

    protected abstract bool UseExactMatch { get; }

    public abstract string Name { get; }

    public void Initialize(IVssRequestContext requestContext)
    {
      AccessMapping accessMapping = requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, this.AccessMappingName);
      if (accessMapping == null)
        return;
      this.m_domain = new Uri(accessMapping.AccessPoint).Host;
    }

    public void Shutdown(IVssRequestContext requestContext)
    {
    }

    public bool TryResolveUriData(
      IVssRequestContext requestContext,
      Guid hostId,
      out IHostUriData uriData)
    {
      uriData = (IHostUriData) null;
      return false;
    }

    public bool TryResolveHost(
      IVssRequestContext requestContext,
      Uri requestUri,
      string applicationVirtualPath,
      out HostRouteContext routeContext)
    {
      routeContext = (HostRouteContext) null;
      if (this.m_domain == null || !requestUri.Host.Equals(this.m_domain, StringComparison.OrdinalIgnoreCase))
        return false;
      ExternalServiceEventUrlResolver.EventTracingInfo eventTracingInfo = new ExternalServiceEventUrlResolver.EventTracingInfo();
      try
      {
        IHostIdMappingProviderData providerData = this.GetProviderData(requestContext);
        bool flag = this.TryResolveHostInternal(requestContext, requestUri, applicationVirtualPath, ref routeContext, eventTracingInfo, providerData);
        eventTracingInfo.Routed = flag;
        eventTracingInfo.RouteHostId = routeContext?.HostId;
        int num = flag ? 1 : 0;
        if (routeContext == null)
        {
          ref HostRouteContext local = ref routeContext;
          ExternalServiceEventUrlResolver.ExternalServiceEventRouteContext eventRouteContext = new ExternalServiceEventUrlResolver.ExternalServiceEventRouteContext(this.AccessMappingName);
          eventRouteContext.HostId = requestContext.ServiceHost.InstanceId;
          eventRouteContext.VirtualPath = applicationVirtualPath;
          eventRouteContext.RouteFlags = RouteFlags.DeploymentHost;
          local = (HostRouteContext) eventRouteContext;
        }
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(TracingPoints.EventsRouting.RoutingException, this.Area, this.Layer, ex);
        eventTracingInfo.ExceptionMessage = ex.Message;
        throw;
      }
      finally
      {
        this.LogResolveHostTrace(requestContext, requestUri, eventTracingInfo);
      }
    }

    protected virtual void LogResolveHostTrace(
      IVssRequestContext requestContext,
      Uri requestUri,
      ExternalServiceEventUrlResolver.EventTracingInfo eventTracingInfo)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      dictionary[TracingPoints.EventProperties.RequestUri] = (object) requestUri;
      dictionary[TracingPoints.EventProperties.EventType] = (object) "EventRouted";
      dictionary[TracingPoints.EventProperties.EventId] = (object) requestContext.E2EId.ToString();
      string routedTo = TracingPoints.EventProperties.RoutedTo;
      string str;
      if (eventTracingInfo == null)
      {
        str = (string) null;
      }
      else
      {
        Guid? routeHostId = eventTracingInfo.RouteHostId;
        ref Guid? local = ref routeHostId;
        str = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
      }
      dictionary[routedTo] = (object) str;
      dictionary[TracingPoints.EventProperties.Payload] = (object) (eventTracingInfo ?? new ExternalServiceEventUrlResolver.EventTracingInfo()).Serialize<ExternalServiceEventUrlResolver.EventTracingInfo>(true);
      string format = dictionary.Serialize<Dictionary<string, object>>();
      requestContext.TraceAlways(TracingPoints.EventsRouting.EventReceived, TraceLevel.Verbose, this.Area, this.Layer, format);
    }

    protected abstract IHostIdMappingProviderData GetProviderData(IVssRequestContext requestContext);

    private bool TryResolveHostInternal(
      IVssRequestContext requestContext,
      Uri requestUri,
      string applicationVirtualPath,
      ref HostRouteContext routeContext,
      ExternalServiceEventUrlResolver.EventTracingInfo tracingInfo,
      IHostIdMappingProviderData providerData)
    {
      if (HttpContext.Current == null || HttpContext.Current.Request == null)
      {
        requestContext.Trace(TracingPoints.EventsRouting.RequestInvalid, TraceLevel.Error, this.Area, this.Layer, "Unknown Web Request. URL: " + requestUri.AbsoluteUri);
        return false;
      }
      if (providerData == null)
      {
        requestContext.Trace(TracingPoints.EventsRouting.RequestInvalid, TraceLevel.Error, this.Area, this.Layer, "Provider not found or unknown. URL: " + requestUri.AbsoluteUri);
        return false;
      }
      IDictionary<string, string> simpleNonEmptyHeaders = HttpContext.Current.Request.GetSimpleNonEmptyHeaders();
      tracingInfo.Headers = simpleNonEmptyHeaders;
      this.SanitizeHeadersForTracing(requestContext, providerData, tracingInfo);
      if (string.IsNullOrEmpty(providerData.ProviderId))
      {
        requestContext.Trace(TracingPoints.EventsRouting.RequestInvalid, TraceLevel.Error, this.Area, this.Layer, "Provider not found or unknown. URL: " + requestUri.AbsoluteUri);
        return false;
      }
      tracingInfo.Properties["providerId"] = providerData.ProviderId;
      string input;
      Guid result;
      if (simpleNonEmptyHeaders != null && !string.IsNullOrEmpty(providerData.DeliveryIdHeaderName) && simpleNonEmptyHeaders.TryGetValue(providerData.DeliveryIdHeaderName, out input) && Guid.TryParse(input, out result) && result != Guid.Empty)
        HttpContext.Current.Request.Headers["X-TFS-Session"] = input;
      Guid? hostId = this.GetHostId(requestContext, providerData.ProviderId, providerData.Routers, requestUri, tracingInfo);
      if (hostId.HasValue)
      {
        Guid? nullable = hostId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
        {
          ref HostRouteContext local = ref routeContext;
          ExternalServiceEventUrlResolver.ExternalServiceEventRouteContext eventRouteContext = new ExternalServiceEventUrlResolver.ExternalServiceEventRouteContext(this.AccessMappingName);
          eventRouteContext.HostId = hostId.Value;
          eventRouteContext.VirtualPath = applicationVirtualPath;
          eventRouteContext.RouteFlags = RouteFlags.CollectionHost;
          local = (HostRouteContext) eventRouteContext;
          requestContext.Trace(TracingPoints.EventsRouting.RequestRouted, TraceLevel.Info, this.Area, this.Layer, "The web request has been routed. URL: {0}; HostId: {1}.", (object) requestUri.AbsoluteUri, (object) routeContext.HostId);
          return true;
        }
      }
      return false;
    }

    private Guid? GetHostId(
      IVssRequestContext requestContext,
      string providerId,
      IReadOnlyList<IHostIdMappingRouter> routers,
      Uri requestUri,
      ExternalServiceEventUrlResolver.EventTracingInfo tracingInfo)
    {
      if (routers == null)
        return new Guid?();
      foreach (IHostIdMappingRouter router in (IEnumerable<IHostIdMappingRouter>) routers)
      {
        HostIdMappingData mappingData;
        if (router.TryExtractMappingData(requestContext, HttpContext.Current.Request, out mappingData))
        {
          tracingInfo.Properties["mappingProperty"] = mappingData?.PropertyName;
          tracingInfo.Properties["mappingId"] = mappingData?.Id;
          tracingInfo.Properties["mappingQualifier"] = mappingData?.Qualifier;
          Guid? hostId = requestContext.GetService<IHostIdMappingService>().GetHostId(requestContext, providerId, mappingData, this.UseExactMatch);
          if (hostId.HasValue)
          {
            Guid? nullable = hostId;
            Guid empty = Guid.Empty;
            if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
              return hostId;
          }
          requestContext.Trace(TracingPoints.EventsRouting.RequestInvalid, TraceLevel.Error, this.Area, this.Layer, string.Format("Mapping entry not found. MappingData: {0}", (object) mappingData));
          return new Guid?();
        }
      }
      requestContext.Trace(TracingPoints.EventsRouting.RequestInvalid, TraceLevel.Info, this.Area, this.Layer, string.Format("No router data found, or routing isn't required. Tried {0} router(s). URL: {1}", (object) routers.Count, (object) requestUri));
      return new Guid?();
    }

    private void SanitizeHeadersForTracing(
      IVssRequestContext requestContext,
      IHostIdMappingProviderData providerData,
      ExternalServiceEventUrlResolver.EventTracingInfo tracingInfo)
    {
      if (tracingInfo.Headers == null)
        return;
      string text1;
      if (tracingInfo.Headers.TryGetValue("Authorization", out text1))
        tracingInfo.Headers["Authorization"] = StringUtil.Truncate(text1, 15, true);
      string[] strArray = new string[2]
      {
        "X-Arr-Authorization",
        "X-Arr-Forwarded-For"
      };
      foreach (string key in strArray)
      {
        if (tracingInfo.Headers.ContainsKey(key))
          tracingInfo.Headers[key] = "*****";
      }
      IReadOnlyList<string> sensitiveHeaderNames = providerData.SensitiveHeaderNames;
      if (sensitiveHeaderNames != null)
      {
        foreach (string key in (IEnumerable<string>) sensitiveHeaderNames)
        {
          string text2;
          if (tracingInfo.Headers.TryGetValue(key, out text2))
            tracingInfo.Headers[key] = StringUtil.Truncate(text2, 15, true);
        }
      }
      if (!requestContext.IsFeatureEnabled("Build2.UseAllowedHeadersForExternalServiceEvents"))
        return;
      IReadOnlyList<string> allowedHeaders = providerData.AllowedHeaders;
      if (allowedHeaders == null)
        return;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (KeyValuePair<string, string> header in (IEnumerable<KeyValuePair<string, string>>) tracingInfo.Headers)
      {
        if (allowedHeaders.Contains<string>(header.Key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          dictionary.Add(header.Key, header.Value);
      }
      tracingInfo.Headers = (IDictionary<string, string>) dictionary;
    }

    internal class ExternalServiceEventRouteContext : HostRouteContext
    {
      public ExternalServiceEventRouteContext(string accessMappingMonikers) => this.AccessMappingMonikers = accessMappingMonikers;
    }

    [DataContract]
    public class EventTracingInfo
    {
      [DataMember(EmitDefaultValue = false)]
      public IDictionary<string, string> Headers { get; set; }

      [DataMember(EmitDefaultValue = false)]
      public IDictionary<string, string> Properties { get; set; } = (IDictionary<string, string>) new Dictionary<string, string>();

      [DataMember(EmitDefaultValue = true)]
      public bool Routed { get; set; }

      [DataMember(EmitDefaultValue = false)]
      public Guid? RouteHostId { get; set; }

      [DataMember(EmitDefaultValue = false)]
      public string ExceptionMessage { get; set; }
    }
  }
}
