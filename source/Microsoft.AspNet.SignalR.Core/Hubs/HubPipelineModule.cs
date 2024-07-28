// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubPipelineModule
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public abstract class HubPipelineModule : IHubPipelineModule
  {
    public virtual Func<IHubIncomingInvokerContext, Task<object>> BuildIncoming(
      Func<IHubIncomingInvokerContext, Task<object>> invoke)
    {
      return (Func<IHubIncomingInvokerContext, Task<object>>) (async context =>
      {
        int num;
        if (num != 0 && !this.OnBeforeIncoming(context))
          return (object) null;
        try
        {
          return this.OnAfterIncoming(await invoke(context).OrEmpty<object>().PreserveCulture<object>(), context);
        }
        catch (Exception ex)
        {
          ExceptionContext exceptionContext = new ExceptionContext(ex);
          this.OnIncomingError(exceptionContext, context);
          Exception error = exceptionContext.Error;
          if (error == ex)
          {
            throw;
          }
          else
          {
            if (error != null)
              throw error;
            return exceptionContext.Result;
          }
        }
      });
    }

    public virtual Func<IHub, Task> BuildConnect(Func<IHub, Task> connect) => (Func<IHub, Task>) (hub => this.OnBeforeConnect(hub) ? connect(hub).OrEmpty().Then<IHub>((Action<IHub>) (h => this.OnAfterConnect(h)), hub) : TaskAsyncHelper.Empty);

    public virtual Func<IHub, Task> BuildReconnect(Func<IHub, Task> reconnect) => (Func<IHub, Task>) (hub => this.OnBeforeReconnect(hub) ? reconnect(hub).OrEmpty().Then<IHub>((Action<IHub>) (h => this.OnAfterReconnect(h)), hub) : TaskAsyncHelper.Empty);

    public virtual Func<IHub, bool, Task> BuildDisconnect(Func<IHub, bool, Task> disconnect) => (Func<IHub, bool, Task>) ((hub, stopCalled) => this.OnBeforeDisconnect(hub, stopCalled) ? disconnect(hub, stopCalled).OrEmpty().Then<IHub, bool>((Action<IHub, bool>) ((h, s) => this.OnAfterDisconnect(h, s)), hub, stopCalled) : TaskAsyncHelper.Empty);

    public virtual Func<HubDescriptor, IRequest, bool> BuildAuthorizeConnect(
      Func<HubDescriptor, IRequest, bool> authorizeConnect)
    {
      return (Func<HubDescriptor, IRequest, bool>) ((hubDescriptor, request) => this.OnBeforeAuthorizeConnect(hubDescriptor, request) && authorizeConnect(hubDescriptor, request));
    }

    public virtual Func<HubDescriptor, IRequest, IList<string>, IList<string>> BuildRejoiningGroups(
      Func<HubDescriptor, IRequest, IList<string>, IList<string>> rejoiningGroups)
    {
      return rejoiningGroups;
    }

    public virtual Func<IHubOutgoingInvokerContext, Task> BuildOutgoing(
      Func<IHubOutgoingInvokerContext, Task> send)
    {
      return (Func<IHubOutgoingInvokerContext, Task>) (context => this.OnBeforeOutgoing(context) ? send(context).OrEmpty().Then<IHubOutgoingInvokerContext>((Action<IHubOutgoingInvokerContext>) (ctx => this.OnAfterOutgoing(ctx)), context) : TaskAsyncHelper.Empty);
    }

    protected virtual bool OnBeforeAuthorizeConnect(HubDescriptor hubDescriptor, IRequest request) => true;

    protected virtual bool OnBeforeConnect(IHub hub) => true;

    protected virtual void OnAfterConnect(IHub hub)
    {
    }

    protected virtual bool OnBeforeReconnect(IHub hub) => true;

    protected virtual void OnAfterReconnect(IHub hub)
    {
    }

    protected virtual bool OnBeforeOutgoing(IHubOutgoingInvokerContext context) => true;

    protected virtual void OnAfterOutgoing(IHubOutgoingInvokerContext context)
    {
    }

    protected virtual bool OnBeforeDisconnect(IHub hub, bool stopCalled) => true;

    protected virtual void OnAfterDisconnect(IHub hub, bool stopCalled)
    {
    }

    protected virtual bool OnBeforeIncoming(IHubIncomingInvokerContext context) => true;

    protected virtual object OnAfterIncoming(object result, IHubIncomingInvokerContext context) => result;

    protected virtual void OnIncomingError(
      ExceptionContext exceptionContext,
      IHubIncomingInvokerContext invokerContext)
    {
    }
  }
}
