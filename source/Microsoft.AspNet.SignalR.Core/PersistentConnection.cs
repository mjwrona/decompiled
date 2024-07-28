// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.PersistentConnection
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Configuration;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNet.SignalR.Tracing;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR
{
  public abstract class PersistentConnection
  {
    private const string WebSocketsTransportName = "webSockets";
    private const string PingJsonPayload = "{ \"Response\": \"pong\" }";
    private const string StartJsonPayload = "{ \"Response\": \"started\" }";
    private static readonly char[] SplitChars = new char[1]
    {
      ':'
    };
    private static readonly ProtocolResolver _protocolResolver = new ProtocolResolver();
    private IConfigurationManager _configurationManager;
    private ITransportManager _transportManager;
    private bool _initialized;

    public virtual void Initialize(IDependencyResolver resolver)
    {
      if (resolver == null)
        throw new ArgumentNullException(nameof (resolver));
      if (this._initialized)
        return;
      this.Pool = resolver.Resolve<IMemoryPool>();
      this.MessageBus = resolver.Resolve<IMessageBus>();
      this.JsonSerializer = resolver.Resolve<JsonSerializer>();
      this.TraceManager = resolver.Resolve<ITraceManager>();
      this.Counters = resolver.Resolve<IPerformanceCounterManager>();
      this.AckHandler = resolver.Resolve<IAckHandler>();
      this.ProtectedData = resolver.Resolve<IProtectedData>();
      this.UserIdProvider = resolver.Resolve<IUserIdProvider>();
      this._configurationManager = resolver.Resolve<IConfigurationManager>();
      this._transportManager = resolver.Resolve<ITransportManager>();
      resolver.Resolve<AckSubscriber>();
      this._initialized = true;
    }

    public bool Authorize(IRequest request) => this.AuthorizeRequest(request);

    protected virtual TraceSource Trace => this.TraceManager["SignalR.PersistentConnection"];

    protected IProtectedData ProtectedData { get; private set; }

    public IMemoryPool Pool { get; set; }

    protected IMessageBus MessageBus { get; private set; }

    protected JsonSerializer JsonSerializer { get; private set; }

    protected IAckHandler AckHandler { get; private set; }

    protected ITraceManager TraceManager { get; private set; }

    protected IPerformanceCounterManager Counters { get; private set; }

    protected ITransport Transport { get; private set; }

    protected IUserIdProvider UserIdProvider { get; private set; }

    public IConnection Connection { get; private set; }

    public IConnectionGroupManager Groups { get; private set; }

    private string DefaultSignal => PrefixHelper.GetPersistentConnectionName(this.DefaultSignalRaw);

    private string DefaultSignalRaw => this.GetType().FullName;

    internal virtual string GroupPrefix => "pcg-";

    public Task ProcessRequest(IDictionary<string, object> environment)
    {
      HostContext context = new HostContext(environment);
      environment.DisableRequestCompression();
      environment.DisableResponseBuffering();
      OwinResponse owinResponse = new OwinResponse(environment);
      owinResponse.Headers.Set("X-Content-Type-Options", "nosniff");
      if (this.Authorize(context.Request))
        return this.ProcessRequest(context);
      owinResponse.StatusCode = context.Request.User == null || !context.Request.User.Identity.IsAuthenticated ? 401 : 403;
      return TaskAsyncHelper.Empty;
    }

    public virtual Task ProcessRequest(HostContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (!this._initialized)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ConnectionNotInitialized));
      if (PersistentConnection.IsNegotiationRequest(context.Request))
        return this.ProcessNegotiationRequest(context);
      if (PersistentConnection.IsPingRequest(context.Request))
        return PersistentConnection.ProcessPingRequest(context);
      this.Transport = this.GetTransport(context);
      if (this.Transport == null)
        return PersistentConnection.FailResponse(context.Response, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ProtocolErrorUnknownTransport));
      string connectionToken = context.Request.QueryString["connectionToken"];
      if (string.IsNullOrEmpty(connectionToken))
        return PersistentConnection.FailResponse(context.Response, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ProtocolErrorMissingConnectionToken));
      string connectionId;
      string message;
      int statusCode;
      if (!this.TryGetConnectionId(context, connectionToken, out connectionId, out message, out statusCode))
        return PersistentConnection.FailResponse(context.Response, message, statusCode);
      this.Transport.ConnectionId = connectionId;
      return this.Transport.GetGroupsToken().Then<string, PersistentConnection, HostContext, Task>((Func<string, PersistentConnection, HostContext, Task>) ((g, pc, c) => pc.ProcessRequestPostGroupRead(c, g)), this, context).FastUnwrap();
    }

    private Task ProcessRequestPostGroupRead(HostContext context, string groupsToken)
    {
      string connectionId = this.Transport.ConnectionId;
      IList<string> signals = this.GetSignals(this.UserIdProvider.GetUserId(context.Request), connectionId);
      IList<string> groups = this.AppendGroupPrefixes(context, connectionId, groupsToken);
      Microsoft.AspNet.SignalR.Infrastructure.Connection connection = this.CreateConnection(connectionId, signals, groups);
      this.Connection = (IConnection) connection;
      string connectionGroupName = PrefixHelper.GetPersistentConnectionGroupName(this.DefaultSignalRaw);
      this.Groups = (IConnectionGroupManager) new GroupManager((IConnection) connection, connectionGroupName);
      if (PersistentConnection.IsStartRequest(context.Request))
        return this.ProcessStartRequest(context, connectionId);
      this.Transport.Connected = (Func<Task>) (() => TaskAsyncHelper.FromMethod((Func<Task>) (() => this.OnConnected(context.Request, connectionId).OrEmpty())));
      this.Transport.Reconnected = (Func<Task>) (() => TaskAsyncHelper.FromMethod((Func<Task>) (() => this.OnReconnected(context.Request, connectionId).OrEmpty())));
      this.Transport.Received = (Func<string, Task>) (data =>
      {
        this.Counters.ConnectionMessagesSentTotal.Increment();
        this.Counters.ConnectionMessagesSentPerSec.Increment();
        return TaskAsyncHelper.FromMethod((Func<Task>) (() => this.OnReceived(context.Request, connectionId, data).OrEmpty()));
      });
      this.Transport.Disconnected = (Func<bool, Task>) (clean => TaskAsyncHelper.FromMethod((Func<Task>) (() => this.OnDisconnected(context.Request, connectionId, clean).OrEmpty())));
      return this.Transport.ProcessRequest((ITransportConnection) connection).OrEmpty().Catch<Task>(this.Trace, this.Counters.ErrorsAllTotal, this.Counters.ErrorsAllPerSec);
    }

    protected internal virtual bool TryGetConnectionId(
      HostContext context,
      string connectionToken,
      out string connectionId,
      out string message,
      out int statusCode)
    {
      string str = (string) null;
      connectionId = (string) null;
      message = (string) null;
      statusCode = 400;
      try
      {
        str = this.ProtectedData.Unprotect(connectionToken, "SignalR.ConnectionToken");
      }
      catch (Exception ex)
      {
        this.Trace.TraceInformation("Failed to process connectionToken {0}: {1}", (object) connectionToken, (object) ex);
      }
      if (string.IsNullOrEmpty(str))
      {
        message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ConnectionIdIncorrectFormat);
        return false;
      }
      string[] strArray = str.Split(PersistentConnection.SplitChars, 2);
      connectionId = strArray[0];
      if (string.Equals(strArray.Length > 1 ? strArray[1] : string.Empty, PersistentConnection.GetUserIdentity(context), StringComparison.OrdinalIgnoreCase))
        return true;
      message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_UnrecognizedUserIdentity);
      statusCode = 403;
      return false;
    }

    internal IList<string> VerifyGroups(string connectionId, string groupsToken)
    {
      if (string.IsNullOrEmpty(groupsToken))
        return ListHelper<string>.Empty;
      string str = (string) null;
      try
      {
        str = this.ProtectedData.Unprotect(groupsToken, "SignalR.Groups.v1.1");
      }
      catch (Exception ex)
      {
        this.Trace.TraceInformation("Failed to process groupsToken {0}: {1}", (object) groupsToken, (object) ex);
      }
      if (string.IsNullOrEmpty(str))
        return ListHelper<string>.Empty;
      string[] strArray = str.Split(PersistentConnection.SplitChars, 2);
      string a = strArray[0];
      string json = strArray.Length > 1 ? strArray[1] : string.Empty;
      string b = connectionId;
      return !string.Equals(a, b, StringComparison.OrdinalIgnoreCase) ? ListHelper<string>.Empty : (IList<string>) this.JsonSerializer.Parse<string[]>(json);
    }

    private IList<string> AppendGroupPrefixes(
      HostContext context,
      string connectionId,
      string groupsToken)
    {
      return (IList<string>) this.OnRejoiningGroups(context.Request, this.VerifyGroups(connectionId, groupsToken), connectionId).Select<string, string>((Func<string, string>) (g => this.GroupPrefix + g)).ToList<string>();
    }

    private Microsoft.AspNet.SignalR.Infrastructure.Connection CreateConnection(
      string connectionId,
      IList<string> signals,
      IList<string> groups)
    {
      return new Microsoft.AspNet.SignalR.Infrastructure.Connection(this.MessageBus, this.JsonSerializer, this.DefaultSignal, connectionId, signals, groups, this.TraceManager, this.AckHandler, this.Counters, this.ProtectedData, this.Pool);
    }

    private IList<string> GetDefaultSignals(string userId, string connectionId) => (IList<string>) new string[2]
    {
      this.DefaultSignal,
      PrefixHelper.GetConnectionId(connectionId)
    };

    protected virtual IList<string> GetSignals(string userId, string connectionId) => this.GetDefaultSignals(userId, connectionId);

    protected virtual bool AuthorizeRequest(IRequest request) => true;

    protected virtual IList<string> OnRejoiningGroups(
      IRequest request,
      IList<string> groups,
      string connectionId)
    {
      return groups;
    }

    protected virtual Task OnConnected(IRequest request, string connectionId) => TaskAsyncHelper.Empty;

    protected virtual Task OnReconnected(IRequest request, string connectionId) => TaskAsyncHelper.Empty;

    protected virtual Task OnReceived(IRequest request, string connectionId, string data) => TaskAsyncHelper.Empty;

    protected virtual Task OnDisconnected(IRequest request, string connectionId, bool stopCalled) => TaskAsyncHelper.Empty;

    private static Task ProcessPingRequest(HostContext context) => PersistentConnection.SendJsonResponse(context, "{ \"Response\": \"pong\" }");

    private Task ProcessNegotiationRequest(HostContext context)
    {
      TimeSpan? nullable1 = this._configurationManager.KeepAliveTimeout();
      string str1 = Guid.NewGuid().ToString("d");
      string data1 = str1 + ":" + PersistentConnection.GetUserIdentity(context);
      string str2 = context.Request.LocalPath.Replace("/negotiate", "");
      string str3 = this.ProtectedData.Protect(data1, "SignalR.ConnectionToken");
      string str4 = str1;
      TimeSpan timeSpan;
      double? nullable2;
      if (!nullable1.HasValue)
      {
        nullable2 = new double?();
      }
      else
      {
        timeSpan = nullable1.Value;
        nullable2 = new double?(timeSpan.TotalSeconds);
      }
      timeSpan = this._configurationManager.DisconnectTimeout;
      double totalSeconds1 = timeSpan.TotalSeconds;
      timeSpan = this._configurationManager.ConnectionTimeout;
      double totalSeconds2 = timeSpan.TotalSeconds;
      int num = !this._transportManager.SupportsTransport("webSockets") ? 0 : (context.Environment.SupportsWebSockets() ? 1 : 0);
      string str5 = PersistentConnection._protocolResolver.Resolve(context.Request).ToString();
      timeSpan = this._configurationManager.TransportConnectTimeout;
      double totalSeconds3 = timeSpan.TotalSeconds;
      timeSpan = this._configurationManager.LongPollDelay;
      double totalSeconds4 = timeSpan.TotalSeconds;
      var data2 = new
      {
        Url = str2,
        ConnectionToken = str3,
        ConnectionId = str4,
        KeepAliveTimeout = nullable2,
        DisconnectTimeout = totalSeconds1,
        ConnectionTimeout = totalSeconds2,
        TryWebSockets = num != 0,
        ProtocolVersion = str5,
        TransportConnectTimeout = totalSeconds3,
        LongPollDelay = totalSeconds4
      };
      return PersistentConnection.SendJsonResponse(context, this.JsonSerializer.Stringify((object) data2));
    }

    private Task ProcessStartRequest(HostContext context, string connectionId) => this.OnConnected(context.Request, connectionId).OrEmpty().Then<HostContext>((Func<HostContext, Task>) (c => PersistentConnection.SendJsonResponse(c, "{ \"Response\": \"started\" }")), context).Then<IPerformanceCounterManager>((Action<IPerformanceCounterManager>) (c => c.ConnectionsConnected.Increment()), this.Counters);

    private static Task SendJsonResponse(HostContext context, string jsonPayload)
    {
      string callback = context.Request.QueryString["callback"];
      if (string.IsNullOrEmpty(callback))
      {
        context.Response.ContentType = JsonUtility.JsonMimeType;
        return context.Response.End(jsonPayload);
      }
      string jsonpCallback = JsonUtility.CreateJsonpCallback(callback, jsonPayload);
      context.Response.ContentType = JsonUtility.JavaScriptMimeType;
      return context.Response.End(jsonpCallback);
    }

    private static string GetUserIdentity(HostContext context) => context.Request.User != null && context.Request.User.Identity.IsAuthenticated ? context.Request.User.Identity.Name ?? string.Empty : string.Empty;

    private static Task FailResponse(IResponse response, string message, int statusCode = 400)
    {
      response.StatusCode = statusCode;
      return response.End(message);
    }

    private static bool IsNegotiationRequest(IRequest request) => request.LocalPath.EndsWith("/negotiate", StringComparison.OrdinalIgnoreCase);

    private static bool IsStartRequest(IRequest request) => request.LocalPath.EndsWith("/start", StringComparison.OrdinalIgnoreCase);

    private static bool IsPingRequest(IRequest request) => request.LocalPath.EndsWith("/ping", StringComparison.OrdinalIgnoreCase);

    private ITransport GetTransport(HostContext context) => this._transportManager.GetTransport(context);
  }
}
