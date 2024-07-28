// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubDispatcher
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class HubDispatcher : PersistentConnection
  {
    private const string HubsSuffix = "/hubs";
    private const string JsSuffix = "/js";
    private readonly List<HubDescriptor> _hubs = new List<HubDescriptor>();
    private readonly bool _enableJavaScriptProxies;
    private readonly bool _enableDetailedErrors;
    private IJavaScriptProxyGenerator _proxyGenerator;
    private IHubManager _manager;
    private IHubRequestParser _requestParser;
    private JsonSerializer _serializer;
    private IParameterResolver _binder;
    private IHubPipelineInvoker _pipelineInvoker;
    private IPerformanceCounterManager _counters;
    private bool _isDebuggingEnabled;
    private static readonly MethodInfo _continueWithMethod = typeof (HubDispatcher).GetMethod("ContinueWith", BindingFlags.Static | BindingFlags.NonPublic);

    public HubDispatcher(HubConfiguration configuration)
    {
      this._enableJavaScriptProxies = configuration != null ? configuration.EnableJavaScriptProxies : throw new ArgumentNullException(nameof (configuration));
      this._enableDetailedErrors = configuration.EnableDetailedErrors;
    }

    protected override TraceSource Trace => this.TraceManager["SignalR.HubDispatcher"];

    internal override string GroupPrefix => "hg-";

    public override void Initialize(IDependencyResolver resolver)
    {
      if (resolver == null)
        throw new ArgumentNullException(nameof (resolver));
      this._proxyGenerator = this._enableJavaScriptProxies ? resolver.Resolve<IJavaScriptProxyGenerator>() : (IJavaScriptProxyGenerator) new EmptyJavaScriptProxyGenerator();
      this._manager = resolver.Resolve<IHubManager>();
      this._binder = resolver.Resolve<IParameterResolver>();
      this._requestParser = resolver.Resolve<IHubRequestParser>();
      this._serializer = resolver.Resolve<JsonSerializer>();
      this._pipelineInvoker = resolver.Resolve<IHubPipelineInvoker>();
      this._counters = resolver.Resolve<IPerformanceCounterManager>();
      base.Initialize(resolver);
    }

    protected override bool AuthorizeRequest(IRequest request)
    {
      string json = request != null ? request.QueryString["connectionData"] : throw new ArgumentNullException(nameof (request));
      if (!string.IsNullOrEmpty(json))
      {
        IEnumerable<HubDispatcher.ClientHubInfo> source = this.JsonSerializer.Parse<IEnumerable<HubDispatcher.ClientHubInfo>>(json);
        if (source != null && source.Any<HubDispatcher.ClientHubInfo>())
        {
          Dictionary<string, HubDescriptor> dictionary = new Dictionary<string, HubDescriptor>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          foreach (HubDispatcher.ClientHubInfo clientHubInfo in source)
          {
            if (dictionary.ContainsKey(clientHubInfo.Name))
              throw new InvalidOperationException(Resources.Error_DuplicateHubNamesInConnectionData);
            HubDescriptor hubDescriptor = this._manager.EnsureHub(clientHubInfo.Name, this._counters.ErrorsHubResolutionTotal, this._counters.ErrorsHubResolutionPerSec, this._counters.ErrorsAllTotal, this._counters.ErrorsAllPerSec);
            if (this._pipelineInvoker.AuthorizeConnect(hubDescriptor, request))
              dictionary.Add(hubDescriptor.Name, hubDescriptor);
          }
          this._hubs.AddRange((IEnumerable<HubDescriptor>) dictionary.Values);
          return this._hubs.Count > 0;
        }
      }
      return base.AuthorizeRequest(request);
    }

    protected override Task OnReceived(IRequest request, string connectionId, string data)
    {
      HubRequest hubRequest = this._requestParser.Parse(data, this._serializer);
      HubDescriptor descriptor = this._manager.EnsureHub(hubRequest.Hub, this._counters.ErrorsHubInvocationTotal, this._counters.ErrorsHubInvocationPerSec, this._counters.ErrorsAllTotal, this._counters.ErrorsAllPerSec);
      IJsonValue[] parameterValues = hubRequest.ParameterValues;
      MethodDescriptor methodDescriptor = this._manager.GetHubMethod(descriptor.Name, hubRequest.Method, (IList<IJsonValue>) parameterValues);
      if (methodDescriptor == null)
      {
        IEnumerable<MethodDescriptor> hubMethods = this._manager.GetHubMethods(descriptor.Name, (Func<MethodDescriptor, bool>) (m => m.Name == hubRequest.Method));
        methodDescriptor = (MethodDescriptor) new NullMethodDescriptor(descriptor, hubRequest.Method, hubMethods);
      }
      StateChangeTracker tracker = new StateChangeTracker(hubRequest.State);
      IHub hub = this.CreateHub(request, descriptor, connectionId, tracker, true);
      return this.InvokeHubPipeline(hub, parameterValues, methodDescriptor, hubRequest, tracker).ContinueWithPreservedCulture((Action<Task>) (task => hub.Dispose()), TaskContinuationOptions.ExecuteSynchronously);
    }

    private Task InvokeHubPipeline(
      IHub hub,
      IJsonValue[] parameterValues,
      MethodDescriptor methodDescriptor,
      HubRequest hubRequest,
      StateChangeTracker tracker)
    {
      HubInvocationProgress progress = HubDispatcher.GetProgressInstance(methodDescriptor, (Func<object, Task>) (value => this.SendProgressUpdate(hub.Context.ConnectionId, tracker, value, hubRequest)), this.Trace);
      Task<object> task1;
      try
      {
        IList<object> objectList = this._binder.ResolveMethodParameters(methodDescriptor, (IList<IJsonValue>) parameterValues);
        if (progress != null)
          objectList = (IList<object>) objectList.Concat<object>((IEnumerable<object>) new HubInvocationProgress[1]
          {
            progress
          }).ToList<object>();
        task1 = this._pipelineInvoker.Invoke((IHubIncomingInvokerContext) new HubInvokerContext(hub, tracker, methodDescriptor, objectList));
      }
      catch (Exception ex)
      {
        task1 = TaskAsyncHelper.FromError<object>(ex);
      }
      return task1.ContinueWithPreservedCulture<object, Task>((Func<Task<object>, Task>) (task =>
      {
        progress?.SetComplete();
        if (task.IsFaulted)
          return this.ProcessResponse(tracker, (object) null, hubRequest, (Exception) task.Exception);
        return task.IsCanceled ? this.ProcessResponse(tracker, (object) null, hubRequest, (Exception) new OperationCanceledException()) : this.ProcessResponse(tracker, task.Result, hubRequest, (Exception) null);
      })).FastUnwrap();
    }

    private static HubInvocationProgress GetProgressInstance(
      MethodDescriptor methodDescriptor,
      Func<object, Task> sendProgressFunc,
      TraceSource traceSource)
    {
      HubInvocationProgress progressInstance = (HubInvocationProgress) null;
      if (methodDescriptor.ProgressReportingType != (Type) null)
        progressInstance = HubInvocationProgress.Create(methodDescriptor.ProgressReportingType, sendProgressFunc, traceSource);
      return progressInstance;
    }

    public override Task ProcessRequest(HostContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      string str = context.Request.LocalPath.TrimEnd('/');
      int num = -1;
      if (str.EndsWith("/hubs", StringComparison.OrdinalIgnoreCase))
        num = "/hubs".Length;
      else if (str.EndsWith("/js", StringComparison.OrdinalIgnoreCase))
        num = "/js".Length;
      if (num != -1)
      {
        string serviceUrl = str.Substring(0, str.Length - num);
        context.Response.ContentType = JsonUtility.JavaScriptMimeType;
        return context.Response.End(this._proxyGenerator.GenerateProxy(serviceUrl));
      }
      this._isDebuggingEnabled = context.Environment.IsDebugEnabled();
      return base.ProcessRequest(context);
    }

    internal static Task Connect(IHub hub) => hub.OnConnected();

    internal static Task Reconnect(IHub hub) => hub.OnReconnected();

    internal static Task Disconnect(IHub hub, bool stopCalled) => hub.OnDisconnected(stopCalled);

    internal static Task<object> Incoming(IHubIncomingInvokerContext context)
    {
      DispatchingTaskCompletionSource<object> tcs = new DispatchingTaskCompletionSource<object>();
      try
      {
        object result = context.MethodDescriptor.Invoker(context.Hub, context.Args.ToArray<object>());
        Type returnType = context.MethodDescriptor.ReturnType;
        if (typeof (Task).IsAssignableFrom(returnType))
        {
          Task task = (Task) result;
          if (!returnType.IsGenericType)
          {
            task.ContinueWith(tcs);
          }
          else
          {
            Type type1 = ((IEnumerable<Type>) returnType.GetGenericArguments()).Single<Type>();
            Type type2 = typeof (Task<>).MakeGenericType(type1);
            ParameterExpression parameterExpression;
            Expression.Lambda<Action<object>>((Expression) Expression.Call(HubDispatcher._continueWithMethod.MakeGenericMethod(type1), (Expression) Expression.Convert((Expression) parameterExpression, type2), (Expression) Expression.Constant((object) tcs)), parameterExpression).Compile()(result);
          }
        }
        else
          tcs.TrySetResult(result);
      }
      catch (Exception ex)
      {
        tcs.TrySetUnwrappedException(ex);
      }
      return tcs.Task;
    }

    internal static Task Outgoing(IHubOutgoingInvokerContext context)
    {
      ConnectionMessage connectionMessage = context.GetConnectionMessage();
      return context.Connection.Send(connectionMessage);
    }

    protected override Task OnConnected(IRequest request, string connectionId) => this.ExecuteHubEvent(request, connectionId, (Func<IHub, Task>) (hub => this._pipelineInvoker.Connect(hub)));

    protected override Task OnReconnected(IRequest request, string connectionId) => this.ExecuteHubEvent(request, connectionId, (Func<IHub, Task>) (hub => this._pipelineInvoker.Reconnect(hub)));

    protected override IList<string> OnRejoiningGroups(
      IRequest request,
      IList<string> groups,
      string connectionId)
    {
      return (IList<string>) this._hubs.Select<HubDescriptor, IEnumerable<string>>((Func<HubDescriptor, IEnumerable<string>>) (hubDescriptor =>
      {
        string groupPrefix = hubDescriptor.Name + ".";
        List<string> list = groups.Where<string>((Func<string, bool>) (g => g.StartsWith(groupPrefix, StringComparison.OrdinalIgnoreCase))).Select<string, string>((Func<string, string>) (g => g.Substring(groupPrefix.Length))).ToList<string>();
        return this._pipelineInvoker.RejoiningGroups(hubDescriptor, request, (IList<string>) list).Select<string, string>((Func<string, string>) (g => groupPrefix + g));
      })).SelectMany<IEnumerable<string>, string>((Func<IEnumerable<string>, IEnumerable<string>>) (groupsToRejoin => groupsToRejoin)).ToList<string>();
    }

    protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled) => this.ExecuteHubEvent(request, connectionId, (Func<IHub, Task>) (hub => this._pipelineInvoker.Disconnect(hub, stopCalled)));

    protected override IList<string> GetSignals(string userId, string connectionId) => (IList<string>) this._hubs.SelectMany<HubDescriptor, string>((Func<HubDescriptor, IEnumerable<string>>) (info =>
    {
      List<string> signals = new List<string>()
      {
        PrefixHelper.GetHubName(info.Name),
        PrefixHelper.GetHubConnectionId(info.CreateQualifiedName(connectionId))
      };
      if (!string.IsNullOrEmpty(userId))
        signals.Add(PrefixHelper.GetHubUserId(info.CreateQualifiedName(userId)));
      return (IEnumerable<string>) signals;
    })).Concat<string>((IEnumerable<string>) new string[1]
    {
      PrefixHelper.GetConnectionId(connectionId)
    }).ToList<string>();

    private Task ExecuteHubEvent(IRequest request, string connectionId, Func<IHub, Task> action)
    {
      List<IHub> hubs = this.GetHubs(request, connectionId).ToList<IHub>();
      Task[] array = hubs.Select<IHub, Task>((Func<IHub, Task>) (instance => action(instance).OrEmpty().Catch<Task>(this.Trace))).ToArray<Task>();
      if (array.Length == 0)
      {
        HubDispatcher.DisposeHubs((IEnumerable<IHub>) hubs);
        return TaskAsyncHelper.Empty;
      }
      DispatchingTaskCompletionSource<object> tcs = new DispatchingTaskCompletionSource<object>();
      Task.Factory.ContinueWhenAll(array, (Action<Task[]>) (tasks =>
      {
        HubDispatcher.DisposeHubs((IEnumerable<IHub>) hubs);
        Task task = ((IEnumerable<Task>) tasks).FirstOrDefault<Task>((Func<Task, bool>) (t => t.IsFaulted));
        if (task != null)
          tcs.TrySetUnwrappedException((Exception) task.Exception);
        else if (((IEnumerable<Task>) tasks).Any<Task>((Func<Task, bool>) (t => t.IsCanceled)))
          tcs.TrySetCanceled();
        else
          tcs.TrySetResult((object) null);
      }));
      return (Task) tcs.Task;
    }

    private IHub CreateHub(
      IRequest request,
      HubDescriptor descriptor,
      string connectionId,
      StateChangeTracker tracker = null,
      bool throwIfFailedToCreate = false)
    {
      try
      {
        IHub hub = this._manager.ResolveHub(descriptor.Name);
        if (hub != null)
        {
          tracker = tracker ?? new StateChangeTracker();
          hub.Context = new HubCallerContext(request, connectionId);
          hub.Clients = (IHubCallerConnectionContext<object>) new HubConnectionContext(this._pipelineInvoker, this.Connection, descriptor.Name, connectionId, tracker);
          hub.Groups = (IGroupManager) new GroupManager(this.Connection, PrefixHelper.GetHubGroupName(descriptor.Name));
        }
        return hub;
      }
      catch (Exception ex)
      {
        this.Trace.TraceInformation("Error creating Hub {0}. " + ex.Message, (object) descriptor.Name);
        if (!throwIfFailedToCreate)
          return (IHub) null;
        throw;
      }
    }

    private IEnumerable<IHub> GetHubs(IRequest request, string connectionId) => this._hubs.Select<HubDescriptor, IHub>((Func<HubDescriptor, IHub>) (descriptor => this.CreateHub(request, descriptor, connectionId))).Where<IHub>((Func<IHub, bool>) (hub => hub != null));

    private static void DisposeHubs(IEnumerable<IHub> hubs)
    {
      foreach (IDisposable hub in hubs)
        hub.Dispose();
    }

    private Task SendProgressUpdate(
      string connectionId,
      StateChangeTracker tracker,
      object value,
      HubRequest request)
    {
      HubResponse hubResponse = new HubResponse()
      {
        State = tracker.GetChanges(),
        Progress = (object) new{ I = request.Id, D = value },
        Id = "P|" + request.Id
      };
      return this.Connection.Send(connectionId, (object) hubResponse);
    }

    private Task ProcessResponse(
      StateChangeTracker tracker,
      object result,
      HubRequest request,
      Exception error)
    {
      HubResponse hubResponse = new HubResponse()
      {
        State = tracker.GetChanges(),
        Result = result,
        Id = request.Id
      };
      if (error != null)
      {
        this._counters.ErrorsHubInvocationTotal.Increment();
        this._counters.ErrorsHubInvocationPerSec.Increment();
        this._counters.ErrorsAllTotal.Increment();
        this._counters.ErrorsAllPerSec.Increment();
        HubException innerException = error.InnerException as HubException;
        if (this._enableDetailedErrors || innerException != null)
        {
          Exception exception = error.InnerException ?? error;
          hubResponse.StackTrace = this._isDebuggingEnabled ? exception.StackTrace : (string) null;
          hubResponse.Error = exception.Message;
          if (innerException != null)
          {
            hubResponse.IsHubException = new bool?(true);
            hubResponse.ErrorData = innerException.ErrorData;
          }
        }
        else
          hubResponse.Error = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_HubInvocationFailed, new object[2]
          {
            (object) request.Hub,
            (object) request.Method
          });
      }
      this.Trace.TraceVerbose("Sending hub invocation result to connection {0} using transport {1}", (object) this.Transport.ConnectionId, (object) this.Transport.GetType().Name);
      return this.Transport.Send((object) hubResponse);
    }

    private static void ContinueWith<T>(Task<T> task, DispatchingTaskCompletionSource<object> tcs)
    {
      if (task.IsCompleted)
        HubDispatcher.ContinueSync<T>(task, tcs);
      else
        HubDispatcher.ContinueAsync<T>(task, tcs);
    }

    private static void ContinueSync<T>(Task<T> task, DispatchingTaskCompletionSource<object> tcs)
    {
      if (task.IsFaulted)
        tcs.TrySetUnwrappedException((Exception) task.Exception);
      else if (task.IsCanceled)
        tcs.TrySetCanceled();
      else
        tcs.TrySetResult((object) task.Result);
    }

    private static void ContinueAsync<T>(Task<T> task, DispatchingTaskCompletionSource<object> tcs) => task.ContinueWithPreservedCulture<T>((Action<Task<T>>) (t =>
    {
      if (t.IsFaulted)
        tcs.TrySetUnwrappedException((Exception) t.Exception);
      else if (t.IsCanceled)
        tcs.TrySetCanceled();
      else
        tcs.TrySetResult((object) t.Result);
    }));

    private class ClientHubInfo
    {
      public string Name { get; set; }
    }
  }
}
