// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceVssRequestContextExtensions
// Assembly: Microsoft.VisualStudio.Services.Commerce.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A442E579-88AD-441C-B92A-FDB0C6C9E30B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class CommerceVssRequestContextExtensions
  {
    private static readonly JsonSerializerSettings DefaultTracePropertiesSerializerSettings = new JsonSerializerSettings();
    private const string TraceArgumentDelimiter = ", ";

    public static CommerceVssRequestContextExtensions.VssRequestContextHolder ToOrganization(
      this IVssRequestContext requestContext,
      Guid organizationHostId,
      RequestContextType? requestContextType = null)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application) && requestContext.ServiceHost.InstanceId == organizationHostId)
        return new CommerceVssRequestContextExtensions.VssRequestContextHolder(requestContext, false);
      IVssRequestContext rootContext = requestContext.RootContext;
      if (rootContext != null && rootContext.ServiceHost.Is(TeamFoundationHostType.Application) && rootContext.ServiceHost.InstanceId == organizationHostId)
        return new CommerceVssRequestContextExtensions.VssRequestContextHolder(rootContext, false);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return new CommerceVssRequestContextExtensions.VssRequestContextHolder(vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(vssRequestContext, organizationHostId, (RequestContextType) ((int) requestContextType ?? (int) requestContext.GetRequestContextType())), true);
    }

    public static IVssRequestContext ToDeployment(this IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment);

    public static CommerceVssRequestContextExtensions.VssRequestContextHolder ToCollection(
      this IVssRequestContext requestContext,
      Guid hostId,
      RequestContextType? requestContextType = null)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.ServiceHost.InstanceId == hostId)
        return new CommerceVssRequestContextExtensions.VssRequestContextHolder(requestContext, false);
      IVssRequestContext rootContext = requestContext.RootContext;
      if (rootContext != null && rootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && rootContext.ServiceHost.InstanceId == hostId)
        return new CommerceVssRequestContextExtensions.VssRequestContextHolder(rootContext, false);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return new CommerceVssRequestContextExtensions.VssRequestContextHolder(vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(vssRequestContext, hostId, (RequestContextType) ((int) requestContextType ?? (int) requestContext.GetRequestContextType())), true);
    }

    public static CommerceVssRequestContextExtensions.VssRequestContextHolder ToCollection(
      this IVssRequestContext requestContext,
      Guid hostId,
      IdentityDescriptor identityDescriptor,
      RequestContextType? requestContextType = null)
    {
      if (identityDescriptor == (IdentityDescriptor) null)
        return requestContext.ToCollection(hostId, requestContextType);
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.ServiceHost.InstanceId == hostId)
        return new CommerceVssRequestContextExtensions.VssRequestContextHolder(requestContext, false);
      IVssRequestContext rootContext = requestContext.RootContext;
      if (rootContext != null && rootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && rootContext.ServiceHost.InstanceId == hostId)
        return new CommerceVssRequestContextExtensions.VssRequestContextHolder(rootContext, false);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return new CommerceVssRequestContextExtensions.VssRequestContextHolder(vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginUserRequest(vssRequestContext, hostId, identityDescriptor, true), true);
    }

    public static CommerceVssRequestContextExtensions.VssRequestContextHolder ToHost(
      this IVssRequestContext requestContext,
      Guid hostId,
      RequestContextType? contextType = null)
    {
      if (requestContext.ServiceHost.InstanceId == hostId)
        return new CommerceVssRequestContextExtensions.VssRequestContextHolder(requestContext, false);
      IVssRequestContext rootContext = requestContext.RootContext;
      if (rootContext != null && rootContext.ServiceHost.InstanceId == hostId)
        return new CommerceVssRequestContextExtensions.VssRequestContextHolder(rootContext, false);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return new CommerceVssRequestContextExtensions.VssRequestContextHolder(vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(vssRequestContext, hostId, (RequestContextType) ((int) contextType ?? (int) requestContext.GetRequestContextType())), true);
    }

    public static RequestContextType GetRequestContextType(this IVssRequestContext requestContext)
    {
      if (requestContext.IsUserContext)
        return RequestContextType.UserContext;
      return requestContext.IsServicingContext ? RequestContextType.ServicingContext : RequestContextType.SystemContext;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TraceProperties<T>(
      this IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      T obj,
      string message = null,
      params object[] args)
    {
      requestContext.TraceProperties<T>(tracepoint, area, layer, obj, CommerceVssRequestContextExtensions.DefaultTracePropertiesSerializerSettings, message, args);
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
      if (!CommerceVssRequestContextExtensions.IsTracingProperties<T>(requestContext, tracepoint, area, layer, TraceLevel.Verbose, obj))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrEmpty(message))
        stringBuilder.Append(string.Format(message + " ", args));
      stringBuilder.Append(CommerceVssRequestContextExtensions.SerializeObjectProperties<T>(obj, serializerSettings));
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
      requestContext.Trace(tracepoint, TraceLevel.Verbose, area, layer, string.Join(", ", ((IEnumerable<object>) args).Select<object, string>(CommerceVssRequestContextExtensions.\u003C\u003EO.\u003C0\u003E__SerializeObject ?? (CommerceVssRequestContextExtensions.\u003C\u003EO.\u003C0\u003E__SerializeObject = new Func<object, string>(JsonConvert.SerializeObject)))));
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
  }
}
