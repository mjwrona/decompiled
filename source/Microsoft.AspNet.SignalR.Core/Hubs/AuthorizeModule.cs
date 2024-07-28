// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.AuthorizeModule
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class AuthorizeModule : HubPipelineModule
  {
    private readonly IAuthorizeHubConnection _globalConnectionAuthorizer;
    private readonly IAuthorizeHubMethodInvocation _globalInvocationAuthorizer;
    private readonly ConcurrentDictionary<Type, IEnumerable<IAuthorizeHubConnection>> _connectionAuthorizersCache;
    private readonly ConcurrentDictionary<Type, IEnumerable<IAuthorizeHubMethodInvocation>> _classInvocationAuthorizersCache;
    private readonly ConcurrentDictionary<MethodDescriptor, IEnumerable<IAuthorizeHubMethodInvocation>> _methodInvocationAuthorizersCache;

    public AuthorizeModule()
      : this((IAuthorizeHubConnection) null, (IAuthorizeHubMethodInvocation) null)
    {
    }

    public AuthorizeModule(
      IAuthorizeHubConnection globalConnectionAuthorizer,
      IAuthorizeHubMethodInvocation globalInvocationAuthorizer)
    {
      this._globalConnectionAuthorizer = globalConnectionAuthorizer;
      this._globalInvocationAuthorizer = globalInvocationAuthorizer;
      this._connectionAuthorizersCache = new ConcurrentDictionary<Type, IEnumerable<IAuthorizeHubConnection>>();
      this._classInvocationAuthorizersCache = new ConcurrentDictionary<Type, IEnumerable<IAuthorizeHubMethodInvocation>>();
      this._methodInvocationAuthorizersCache = new ConcurrentDictionary<MethodDescriptor, IEnumerable<IAuthorizeHubMethodInvocation>>();
    }

    public override Func<HubDescriptor, IRequest, bool> BuildAuthorizeConnect(
      Func<HubDescriptor, IRequest, bool> authorizeConnect)
    {
      return base.BuildAuthorizeConnect((Func<HubDescriptor, IRequest, bool>) ((hubDescriptor, request) => authorizeConnect(hubDescriptor, request) && (this._globalConnectionAuthorizer == null || this._globalConnectionAuthorizer.AuthorizeHubConnection(hubDescriptor, request)) && this._connectionAuthorizersCache.GetOrAdd(hubDescriptor.HubType, (Func<Type, IEnumerable<IAuthorizeHubConnection>>) (hubType => hubType.GetCustomAttributes(typeof (IAuthorizeHubConnection), true).Cast<IAuthorizeHubConnection>())).All<IAuthorizeHubConnection>((Func<IAuthorizeHubConnection, bool>) (a => a.AuthorizeHubConnection(hubDescriptor, request)))));
    }

    public override Func<IHubIncomingInvokerContext, Task<object>> BuildIncoming(
      Func<IHubIncomingInvokerContext, Task<object>> invoke)
    {
      return base.BuildIncoming((Func<IHubIncomingInvokerContext, Task<object>>) (context =>
      {
        if ((this._globalInvocationAuthorizer == null || this._globalInvocationAuthorizer.AuthorizeHubMethodInvocation(context, false)) && this._classInvocationAuthorizersCache.GetOrAdd(context.Hub.GetType(), (Func<Type, IEnumerable<IAuthorizeHubMethodInvocation>>) (hubType => hubType.GetCustomAttributes(typeof (IAuthorizeHubMethodInvocation), true).Cast<IAuthorizeHubMethodInvocation>())).All<IAuthorizeHubMethodInvocation>((Func<IAuthorizeHubMethodInvocation, bool>) (a => a.AuthorizeHubMethodInvocation(context, false))) && (context.MethodDescriptor is NullMethodDescriptor || this._methodInvocationAuthorizersCache.GetOrAdd(context.MethodDescriptor, (Func<MethodDescriptor, IEnumerable<IAuthorizeHubMethodInvocation>>) (methodDescriptor => methodDescriptor.Attributes.OfType<IAuthorizeHubMethodInvocation>())).All<IAuthorizeHubMethodInvocation>((Func<IAuthorizeHubMethodInvocation, bool>) (a => a.AuthorizeHubMethodInvocation(context, true)))))
          return invoke(context);
        return TaskAsyncHelper.FromError<object>((Exception) new NotAuthorizedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_CallerNotAuthorizedToInvokeMethodOn, new object[2]
        {
          (object) context.MethodDescriptor.Name,
          (object) context.MethodDescriptor.Hub.Name
        })));
      }));
    }
  }
}
