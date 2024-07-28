// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.VssRequestContextLicensingExtensions
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class VssRequestContextLicensingExtensions
  {
    internal static readonly JsonSerializerSettings DefaultTracePropertiesSerializerSettings = new JsonSerializerSettings();
    private const string TraceArgumentDelimiter = ", ";

    public static VssRequestContextLicensingExtensions.VssRequestContextHolder ToOrganization(
      this IVssRequestContext requestContext,
      Guid organizationHostId,
      RequestContextType? requestContextType = null)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application) && requestContext.ServiceHost.InstanceId == organizationHostId)
        return new VssRequestContextLicensingExtensions.VssRequestContextHolder(requestContext, false);
      IVssRequestContext rootContext = requestContext.RootContext;
      if (rootContext != null && rootContext.ServiceHost.Is(TeamFoundationHostType.Application) && rootContext.ServiceHost.InstanceId == organizationHostId)
        return new VssRequestContextLicensingExtensions.VssRequestContextHolder(rootContext, false);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return new VssRequestContextLicensingExtensions.VssRequestContextHolder(vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(vssRequestContext, organizationHostId, (RequestContextType) ((int) requestContextType ?? (int) requestContext.GetRequestContextType())), true);
    }

    public static IVssRequestContext ToDeployment(this IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment);

    public static VssRequestContextLicensingExtensions.VssRequestContextHolder ToCollection(
      this IVssRequestContext requestContext,
      Guid hostId,
      RequestContextType? requestContextType = null)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.ServiceHost.InstanceId == hostId)
        return new VssRequestContextLicensingExtensions.VssRequestContextHolder(requestContext, false);
      IVssRequestContext rootContext = requestContext.RootContext;
      if (rootContext != null && rootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && rootContext.ServiceHost.InstanceId == hostId)
        return new VssRequestContextLicensingExtensions.VssRequestContextHolder(rootContext, false);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return new VssRequestContextLicensingExtensions.VssRequestContextHolder(vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(vssRequestContext, hostId, (RequestContextType) ((int) requestContextType ?? (int) requestContext.GetRequestContextType())), true);
    }

    public static VssRequestContextLicensingExtensions.VssRequestContextHolder ToHost(
      this IVssRequestContext requestContext,
      Guid hostId)
    {
      if (requestContext.ServiceHost.InstanceId == hostId)
        return new VssRequestContextLicensingExtensions.VssRequestContextHolder(requestContext, false);
      IVssRequestContext rootContext = requestContext.RootContext;
      if (rootContext != null && rootContext.ServiceHost.InstanceId == hostId)
        return new VssRequestContextLicensingExtensions.VssRequestContextHolder(rootContext, false);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return new VssRequestContextLicensingExtensions.VssRequestContextHolder(vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(vssRequestContext, hostId, requestContext.GetRequestContextType()), true);
    }

    public static IDisposable TraceMember(
      this IVssRequestContext requestContext,
      int enterTracepoint,
      int leaveTracepoint,
      string area,
      string layer,
      [CallerMemberName] string memberName = null)
    {
      return (IDisposable) new VssRequestContextLicensingExtensions.MethodBoundary(requestContext, enterTracepoint, leaveTracepoint, area, layer, memberName);
    }

    public static RequestContextType GetRequestContextType(this IVssRequestContext requestContext)
    {
      if (requestContext.IsUserContext)
        return RequestContextType.UserContext;
      return requestContext.IsServicingContext ? RequestContextType.ServicingContext : RequestContextType.SystemContext;
    }

    public static void TraceProperties<T>(
      this IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      T obj,
      string message = null,
      params object[] args)
    {
      requestContext.TraceProperties<T>(tracepoint, area, layer, obj, VssRequestContextLicensingExtensions.DefaultTracePropertiesSerializerSettings, message, args);
    }

    public static void TraceProperties<T>(
      this IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      T obj,
      JsonSerializerSettings serializerSettings,
      string message = null,
      params object[] args)
    {
      if (!VssRequestContextLicensingExtensions.IsTracingProperties<T>(requestContext, tracepoint, area, layer, TraceLevel.Verbose, obj))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrEmpty(message))
        stringBuilder.Append(string.Format(message + " ", args));
      stringBuilder.Append(VssRequestContextLicensingExtensions.SerializeObjectProperties<T>(obj, serializerSettings));
      requestContext.Trace(tracepoint, TraceLevel.Verbose, area, layer, stringBuilder.ToString());
    }

    public static void TraceEnter(
      this IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      object[] args,
      [CallerMemberName] string methodName = null)
    {
      requestContext.TraceEnter(tracepoint, area, layer, methodName);
      if (!requestContext.IsTracing(tracepoint, TraceLevel.Verbose, area, layer))
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      requestContext.Trace(tracepoint, TraceLevel.Verbose, area, layer, string.Join(", ", ((IEnumerable<object>) args).Select<object, string>(VssRequestContextLicensingExtensions.\u003C\u003EO.\u003C0\u003E__SerializeObject ?? (VssRequestContextLicensingExtensions.\u003C\u003EO.\u003C0\u003E__SerializeObject = new Func<object, string>(JsonConvert.SerializeObject)))));
    }

    private static bool IsTracingProperties<T>(
      IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      TraceLevel level,
      T obj)
    {
      if (!requestContext.RequestTracer.IsTracing(tracepoint, level, area, layer))
        return false;
      if ((object) obj != null)
        return true;
      requestContext.Trace(tracepoint, level, area, layer, "Instance of " + typeof (T).Name + " was null.");
      return false;
    }

    private static string SerializeObjectProperties<T>(
      T obj,
      JsonSerializerSettings jsonSerializerSettings)
    {
      return JsonConvert.SerializeObject((object) obj, jsonSerializerSettings);
    }

    public class VssRequestContextHolder : IDisposable
    {
      private readonly bool m_ownsRequest;

      public VssRequestContextHolder(IVssRequestContext requestContext, bool ownsRequest)
      {
        this.RequestContext = requestContext;
        this.m_ownsRequest = ownsRequest;
      }

      public IVssRequestContext RequestContext { get; private set; }

      public void Dispose()
      {
        IVssRequestContext requestContext = this.RequestContext;
        if (requestContext == null)
          return;
        this.RequestContext = (IVssRequestContext) null;
        if (!this.m_ownsRequest)
          return;
        requestContext.Dispose();
      }
    }

    private class MethodBoundary : IDisposable
    {
      private IVssRequestContext m_requestContext;
      private readonly int m_enterTracepoint;
      private readonly int m_leaveTracepoint;
      private readonly string m_area;
      private readonly string m_layer;
      private readonly string m_memberName;

      public MethodBoundary(
        IVssRequestContext requestContext,
        int enterTracepoint,
        int leaveTracepoint,
        string area,
        string layer,
        string memberName)
      {
        this.m_requestContext = requestContext;
        this.m_enterTracepoint = enterTracepoint;
        this.m_leaveTracepoint = leaveTracepoint;
        this.m_area = area;
        this.m_layer = layer;
        this.m_memberName = memberName;
        this.OnEnter();
      }

      private void OnEnter()
      {
        if (this.m_requestContext == null)
          return;
        this.m_requestContext.TraceEnter(this.m_enterTracepoint, this.m_area, this.m_layer, this.m_memberName);
      }

      private void OnLeave()
      {
        if (this.m_requestContext == null)
          return;
        this.m_requestContext.TraceLeave(this.m_leaveTracepoint, this.m_area, this.m_layer, this.m_memberName);
      }

      void IDisposable.Dispose()
      {
        this.OnLeave();
        this.m_requestContext = (IVssRequestContext) null;
      }
    }
  }
}
