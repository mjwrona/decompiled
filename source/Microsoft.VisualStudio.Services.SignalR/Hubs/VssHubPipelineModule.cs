// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.Hubs.VssHubPipelineModule
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.VisualStudio.Services.SignalR.Hubs
{
  public class VssHubPipelineModule : HubPipelineModule
  {
    private IVssDeploymentServiceHost m_serviceHost;
    private const string c_area = "SignalR";
    private const string c_layer = "HubPipelineModule";
    private const string c_contextTokenName = "contextToken";

    public VssHubPipelineModule(IVssDeploymentServiceHost serviceHost)
    {
      ArgumentUtility.CheckForNull<IVssDeploymentServiceHost>(serviceHost, nameof (serviceHost));
      this.m_serviceHost = serviceHost;
    }

    public override Func<IHub, Task> BuildConnect(Func<IHub, Task> connect) => base.BuildConnect((Func<IHub, Task>) (hub => this.RunAsync((Func<Task>) (() => this.Connect(hub, connect)))));

    public override Func<IHub, bool, Task> BuildDisconnect(Func<IHub, bool, Task> disconnect) => base.BuildDisconnect((Func<IHub, bool, Task>) ((hub, stopCalled) => this.RunAsync((Func<Task>) (() => this.Disconnect(hub, stopCalled, disconnect)))));

    public override Func<IHubIncomingInvokerContext, Task<object>> BuildIncoming(
      Func<IHubIncomingInvokerContext, Task<object>> invoke)
    {
      return base.BuildIncoming((Func<IHubIncomingInvokerContext, Task<object>>) (context => this.RunAsync<object>((Func<Task<object>>) (() => this.Incoming(context, invoke)))));
    }

    public override Func<IHub, Task> BuildReconnect(Func<IHub, Task> reconnect) => base.BuildReconnect((Func<IHub, Task>) (hub => this.RunAsync((Func<Task>) (() => this.Reconnect(hub, reconnect)))));

    internal void Attach(IVssDeploymentServiceHost serviceHost)
    {
      if (this.m_serviceHost == serviceHost)
        return;
      this.m_serviceHost = serviceHost;
    }

    internal void Detach(IVssDeploymentServiceHost serviceHost)
    {
      if (this.m_serviceHost != serviceHost)
        return;
      this.m_serviceHost = (IVssDeploymentServiceHost) null;
    }

    private async Task Connect(IHub hub, Func<IHub, Task> connect)
    {
      IVssHub hub1 = this.EnsureVssHub(hub);
      string methodName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.Connect", (object) hub1.GetType().Name);
      using (this.CreateHubRequestContext(hub1, methodName))
        await connect(hub);
    }

    private async Task Disconnect(IHub hub, bool stopCalled, Func<IHub, bool, Task> disconnect)
    {
      IVssHub hub1 = this.EnsureVssHub(hub);
      string methodName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.Disconnect", (object) hub1.GetType().Name);
      using (this.CreateHubRequestContext(hub1, methodName, allowSystemContext: true))
        await disconnect(hub, stopCalled);
    }

    private async Task<object> Incoming(
      IHubIncomingInvokerContext context,
      Func<IHubIncomingInvokerContext, Task<object>> invoke)
    {
      IVssHub hub = this.EnsureVssHub(context.Hub);
      string methodName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) hub.GetType().Name, (object) context.MethodDescriptor.Name);
      string hostIdString = context.Args == null || context.Args.Count <= 0 ? (string) null : context.Args[0].ToString();
      object obj;
      using (this.CreateHubRequestContext(hub, methodName, hostIdString))
        obj = await invoke(context);
      return obj;
    }

    private async Task Reconnect(IHub hub, Func<IHub, Task> reconnect)
    {
      IVssHub hub1 = this.EnsureVssHub(hub);
      string methodName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.Reconnect", (object) hub1.GetType().Name);
      using (this.CreateHubRequestContext(hub1, methodName))
        await reconnect(hub);
    }

    private Task RunAsync(Func<Task> function) => HttpContext.Current == null ? new VssRequestSynchronizationContext().RunAsync(function) : function();

    private Task<TResult> RunAsync<TResult>(Func<Task<TResult>> function) => HttpContext.Current == null ? new VssRequestSynchronizationContext().RunAsync<TResult>(function) : function();

    private IVssHub EnsureVssHub(IHub hub) => hub is IVssHub vssHub ? vssHub : throw new ArgumentException("IHub implementation must implement the interface IVssHub");

    private VssHubPipelineModule.HubRequestContextWrapper CreateHubRequestContext(
      IVssHub hub,
      string methodName,
      string hostIdString = null,
      bool allowSystemContext = false)
    {
      ArgumentUtility.CheckForNull<IVssHub>(hub, nameof (hub));
      hostIdString = hub.Context.QueryString["contextToken"] ?? hostIdString;
      Guid result;
      if (!Guid.TryParse(hostIdString, out result))
      {
        if (hub.RequiresHostContext)
          throw new InvalidOperationException("No contextToken found on the SignalR request");
        result = this.m_serviceHost.InstanceId;
      }
      HttpContextBase httpContext = hub.Context.Request.GetHttpContext();
      IVssRequestContext requestContext = (IVssRequestContext) null;
      VssHubPipelineModule.HubRequestContextWrapper hubRequestContext = (VssHubPipelineModule.HubRequestContextWrapper) null;
      using (IVssRequestContext systemContext = this.m_serviceHost.CreateSystemContext())
      {
        try
        {
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = httpContext.Items[(object) "VssSignalRUserContext"] as Microsoft.VisualStudio.Services.Identity.Identity;
          ITeamFoundationHostManagementService service = systemContext.GetService<ITeamFoundationHostManagementService>();
          if (userIdentity != null)
          {
            requestContext = service.BeginUserRequest(systemContext, result, userIdentity);
            hubRequestContext = new VssHubPipelineModule.HubRequestContextWrapper(hub, httpContext, requestContext, methodName);
          }
          else if (allowSystemContext)
          {
            requestContext = service.BeginRequest(systemContext, result, RequestContextType.SystemContext);
            hubRequestContext = new VssHubPipelineModule.HubRequestContextWrapper(hub, httpContext, requestContext, methodName);
          }
        }
        catch (Exception ex)
        {
          systemContext.TraceException(10017102, "SignalR", "HubPipelineModule", ex);
          throw;
        }
        finally
        {
          if (hubRequestContext == null && requestContext != null)
            requestContext.Dispose();
        }
      }
      if (hubRequestContext != null)
        return hubRequestContext;
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to create a request context for {0} method {1}", (object) hub.GetType().Name, (object) methodName);
      TeamFoundationTracingService.TraceRaw(10017103, TraceLevel.Error, "SignalR", "HubPipelineModule", message);
      throw new InvalidOperationException(message);
    }

    private sealed class HubRequestContextWrapper : IDisposable
    {
      private bool m_disposed;
      private readonly IVssHub m_hub;
      private readonly HttpContextBase m_httpContext;
      private readonly IVssRequestContext m_requestContext;

      public HubRequestContextWrapper(
        IVssHub hub,
        HttpContextBase httpContext,
        IVssRequestContext requestContext,
        string methodName)
      {
        this.m_hub = hub;
        this.m_httpContext = httpContext;
        this.m_requestContext = requestContext;
        this.m_hub.VssRequestContext = requestContext;
        this.m_requestContext.ServiceHost.BeginRequest(this.m_requestContext);
        this.m_requestContext.ServiceName = "SignalR";
        this.m_requestContext.EnterMethod(new MethodInformation(methodName, MethodType.Normal, EstimatedMethodCost.Low));
      }

      public HttpContextBase HttpContext => this.m_httpContext;

      public IVssRequestContext RequestContext => this.m_requestContext;

      void IDisposable.Dispose()
      {
        if (this.m_disposed)
          return;
        this.m_disposed = true;
        this.m_requestContext.LeaveMethod();
        this.m_requestContext.ServiceHost.EndRequest(this.m_requestContext);
        this.m_requestContext.Dispose();
      }
    }
  }
}
