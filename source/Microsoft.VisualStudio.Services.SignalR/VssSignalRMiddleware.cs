// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.VssSignalRMiddleware
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.SignalR.Contracts;
using Microsoft.VisualStudio.Services.SignalR.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.VisualStudio.Services.SignalR
{
  public class VssSignalRMiddleware : OwinMiddleware
  {
    private readonly PathString m_applicationPath;
    private const string c_area = "SignalR";
    private const string c_layer = "Middleware";
    private const int c_defaultHostedExecutionMaxRequests = 100;
    private const int c_defaultHostedExecutionMaxConcurrentRequests = 60;
    private const long c_defaultHostedExecutionTimeoutTicks = 100000000;
    private const string c_signalR = "SignalR";
    private const string c_negotiate = "Negotiate";
    private const string c_connect = "Connect";
    private const string c_reConnect = "Reconnect";
    private const string c_poll = "Poll";
    private const string c_start = "Start";
    private const string c_slash = "/";
    private const string c_connectionData = "connectionData";
    private const string c_connectionDataValue = "[{'name':'builddetailhub'}]";
    private const string c_transport = "transport";
    private const string c_transportValue = "websockets";
    private const string c_connectionToken = "connectionToken";
    private TimeSpan m_defaultTTL = TimeSpan.FromMinutes(4.0);
    private static Lazy<JsonSerializerSettings> s_jsonSerializerSettings;

    public VssSignalRMiddleware(OwinMiddleware next, string applicationPath)
      : base(next)
    {
      if (!applicationPath.StartsWith("/"))
        applicationPath = "/" + applicationPath;
      this.m_applicationPath = new PathString(applicationPath);
    }

    public override async Task Invoke(IOwinContext context)
    {
      HttpContextBase httpContext = context.Request.GetHttpContext();
      IVssRequestContext requestContext = httpContext.GetVssRequestContext();
      requestContext.ServiceName = "SignalR";
      MethodInformation methodInformation = VssSignalRMiddleware.GetMethodInformation(context, this.GetAction(context));
      requestContext.EnterMethod(methodInformation);
      IdentityValidationResult identityValidationResult = this.ValidateIdentity(requestContext, httpContext);
      if (!identityValidationResult.IsSuccess)
      {
        requestContext.TraceDataConditionally(10017112, TraceLevel.Verbose, "SignalR", "Middleware", string.Empty, (Func<object>) (() => (object) identityValidationResult), nameof (Invoke));
        context.Response.StatusCode = (int) identityValidationResult.HttpStatusCode;
        methodInformation = (MethodInformation) null;
      }
      else
      {
        string str = httpContext.Request.QueryString.Get("serverProtocol");
        bool requestWillBeHandled = false;
        if ("urlSigning".Equals(str, StringComparison.OrdinalIgnoreCase))
        {
          if (methodInformation.Name.EndsWith("Negotiate", StringComparison.OrdinalIgnoreCase))
          {
            requestWillBeHandled = true;
            await this.InvokeWrapper(requestContext, httpContext, (Func<Task>) (async () =>
            {
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              CancellationTokenRegistration tokenRegistration = requestContext.CancellationToken.Register(VssSignalRMiddleware.\u003C\u003EO.\u003C0\u003E__CancelRequest ?? (VssSignalRMiddleware.\u003C\u003EO.\u003C0\u003E__CancelRequest = new Action<object>(VssSignalRMiddleware.CancelRequest)), (object) httpContext, true);
              try
              {
                Stream originalStream = context.Response.Body;
                SignalRNegotiateResponse rnegotiateResponse = (SignalRNegotiateResponse) null;
                using (MemoryStream newStream = new MemoryStream())
                {
                  context.Response.Body = (Stream) newStream;
                  await this.Next.Invoke(context);
                  newStream.Seek(0L, SeekOrigin.Begin);
                  using (StreamReader streamReader = new StreamReader((Stream) newStream))
                    rnegotiateResponse = JsonUtilities.Deserialize<SignalRNegotiateResponse>(streamReader.ReadToEnd());
                }
                UrlSigningProtocolResponse protocolResponse = new UrlSigningProtocolResponse();
                if (rnegotiateResponse != null && rnegotiateResponse.TryWebSockets)
                {
                  Uri uri = requestContext.RequestUri();
                  NameValueCollection queryString = HttpUtility.ParseQueryString(uri.Query);
                  queryString.Add("connectionData", "[{'name':'builddetailhub'}]");
                  queryString.Add("connectionToken", rnegotiateResponse.ConnectionToken);
                  queryString.Add("transport", "websockets");
                  string urlWithParams = this.GetUrlWithParams(this.GetBaseUrl(uri, "Negotiate", "Start"), queryString);
                  protocolResponse.Url = this.GetSignedUrl(requestContext, urlWithParams);
                }
                context.Response.Body = originalStream;
                await context.Response.WriteAsync(JsonConvert.SerializeObject((object) protocolResponse, this.GetJsonSerializerSettings()));
                originalStream = (Stream) null;
              }
              finally
              {
                tokenRegistration.Dispose();
              }
              tokenRegistration = new CancellationTokenRegistration();
            }));
          }
          else if (methodInformation.Name.EndsWith("Start", StringComparison.OrdinalIgnoreCase))
          {
            requestWillBeHandled = true;
            await this.InvokeWrapper(requestContext, httpContext, (Func<Task>) (async () =>
            {
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              CancellationTokenRegistration tokenRegistration = requestContext.CancellationToken.Register(VssSignalRMiddleware.\u003C\u003EO.\u003C0\u003E__CancelRequest ?? (VssSignalRMiddleware.\u003C\u003EO.\u003C0\u003E__CancelRequest = new Action<object>(VssSignalRMiddleware.CancelRequest)), (object) httpContext, true);
              try
              {
                Stream originalStream = context.Response.Body;
                SignalRStartResponse signalRstartResponse = (SignalRStartResponse) null;
                using (MemoryStream newStream = new MemoryStream())
                {
                  context.Response.Body = (Stream) newStream;
                  await this.Next.Invoke(context);
                  newStream.Seek(0L, SeekOrigin.Begin);
                  using (StreamReader streamReader = new StreamReader((Stream) newStream))
                    signalRstartResponse = JsonUtilities.Deserialize<SignalRStartResponse>(streamReader.ReadToEnd());
                }
                UrlSigningStartResponse signingStartResponse = new UrlSigningStartResponse();
                if (signalRstartResponse != null)
                {
                  Uri uri = requestContext.RequestUri();
                  NameValueCollection queryString = HttpUtility.ParseQueryString(uri.Query);
                  string urlWithParams = this.GetUrlWithParams(this.GetBaseUrl(uri, "Start", "Connect", true), queryString);
                  signingStartResponse.Url = this.GetSignedUrl(requestContext, urlWithParams);
                  string baseUrl = this.GetBaseUrl(uri, "Start", "Reconnect", true);
                  signingStartResponse.ReconnectBaseUrl = this.GetUrlWithParams(baseUrl, queryString);
                }
                context.Response.Body = originalStream;
                await context.Response.WriteAsync(JsonConvert.SerializeObject((object) signingStartResponse, this.GetJsonSerializerSettings()));
                originalStream = (Stream) null;
              }
              finally
              {
                tokenRegistration.Dispose();
              }
              tokenRegistration = new CancellationTokenRegistration();
            }));
          }
        }
        if (requestWillBeHandled)
          methodInformation = (MethodInformation) null;
        else if (methodInformation.Name.EndsWith("Reconnect", StringComparison.OrdinalIgnoreCase))
        {
          Func<Task> func;
          Func<Task> run = (Func<Task>) (async () => await this.InvokeWrapper(requestContext, httpContext, func ?? (func = (Func<Task>) (async () =>
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            CancellationTokenRegistration tokenRegistration = requestContext.CancellationToken.Register(VssSignalRMiddleware.\u003C\u003EO.\u003C0\u003E__CancelRequest ?? (VssSignalRMiddleware.\u003C\u003EO.\u003C0\u003E__CancelRequest = new Action<object>(VssSignalRMiddleware.CancelRequest)), (object) httpContext, true);
            try
            {
              await this.Next.Invoke(context);
            }
            finally
            {
              tokenRegistration.Dispose();
            }
            tokenRegistration = new CancellationTokenRegistration();
          }))));
          Func<Task> fallback = (Func<Task>) (() =>
          {
            context.Response.StatusCode = 429;
            return Task.CompletedTask;
          });
          CommandPropertiesSetter commandPropertiesDefaults = new CommandPropertiesSetter();
          if (requestContext.ExecutionEnvironment.IsHostedDeployment)
          {
            commandPropertiesDefaults.WithExecutionMaxRequests(100);
            commandPropertiesDefaults.WithExecutionMaxConcurrentRequests(60);
            commandPropertiesDefaults.WithExecutionTimeout(new TimeSpan(100000000L));
          }
          else
          {
            commandPropertiesDefaults.WithExecutionMaxRequests(int.MaxValue);
            commandPropertiesDefaults.WithExecutionTimeout(TimeSpan.MaxValue);
            commandPropertiesDefaults.WithCircuitBreakerForceClosed(true);
          }
          await new CommandServiceAsync(requestContext, new CommandSetter((CommandGroupKey) "SignalR").AndCommandPropertiesDefaults(commandPropertiesDefaults).AndCommandKey((CommandKey) methodInformation.Name), run, fallback, true).Execute();
          methodInformation = (MethodInformation) null;
        }
        else
        {
          await this.InvokeWrapper(requestContext, httpContext, (Func<Task>) (async () =>
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            CancellationTokenRegistration tokenRegistration = requestContext.CancellationToken.Register(VssSignalRMiddleware.\u003C\u003EO.\u003C0\u003E__CancelRequest ?? (VssSignalRMiddleware.\u003C\u003EO.\u003C0\u003E__CancelRequest = new Action<object>(VssSignalRMiddleware.CancelRequest)), (object) httpContext, true);
            try
            {
              await this.Next.Invoke(context);
            }
            finally
            {
              tokenRegistration.Dispose();
            }
            tokenRegistration = new CancellationTokenRegistration();
          }));
          methodInformation = (MethodInformation) null;
        }
      }
    }

    private async Task InvokeWrapper(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      Func<Task> invoke)
    {
      try
      {
        await invoke();
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(103002, "SignalR", "Middleware", ex);
        StringBuilder stringBuilder = new StringBuilder("There was an exception invoking next middleware component");
        stringBuilder.AppendLine(string.Format("RequestUri: {0}", (object) requestContext.RequestUri()));
        stringBuilder.AppendLine(string.Format("IsClientConnected: {0}", (object) httpContext.Response.IsClientConnected));
        stringBuilder.AppendLine(string.Format("IsCancellationRequested: {0}", (object) requestContext.CancellationToken.IsCancellationRequested));
        TeamFoundationTracingService.TraceRaw(103002, TraceLevel.Error, "SignalR", "Middleware", stringBuilder.ToString());
        throw;
      }
    }

    private JsonSerializerSettings GetJsonSerializerSettings()
    {
      if (VssSignalRMiddleware.s_jsonSerializerSettings == null)
        VssSignalRMiddleware.s_jsonSerializerSettings = new Lazy<JsonSerializerSettings>(new Func<JsonSerializerSettings>(this.CreateJsonSerializerSettings));
      return VssSignalRMiddleware.s_jsonSerializerSettings.Value;
    }

    private JsonSerializerSettings CreateJsonSerializerSettings() => new JsonSerializerSettings()
    {
      ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
    };

    private string GetUrlWithParams(string baseUrl, NameValueCollection queryParams)
    {
      UriBuilder uriBuilder = new UriBuilder(baseUrl);
      foreach (string allKey in queryParams.AllKeys)
      {
        if (allKey != null)
          uriBuilder.AppendQuery(allKey, queryParams[allKey]);
      }
      return uriBuilder.AbsoluteUri();
    }

    private string GetSignedUrl(IVssRequestContext requestContext, string urlToSign)
    {
      IUrlSigningService service = requestContext.GetService<IUrlSigningService>();
      DateTime dateTime = DateTime.UtcNow.Add(this.m_defaultTTL);
      IVssRequestContext requestContext1 = requestContext;
      Uri uri = new Uri(urlToSign);
      DateTime expires = dateTime;
      return service.Sign(requestContext1, uri, expires);
    }

    protected virtual IdentityValidationResult ValidateIdentity(
      IVssRequestContext requestContext,
      HttpContextBase httpContext)
    {
      return requestContext.IsValidIdentity();
    }

    protected virtual string GetAction(IOwinContext context)
    {
      string action = context.Request.Path.Value;
      PathString remaining;
      if (context.Request.Path.StartsWithSegments(this.m_applicationPath, out remaining) && remaining.HasValue)
        action = remaining.Value.Trim('/');
      return action;
    }

    private static void CancelRequest(object state)
    {
      HttpContextBase httpContext = (HttpContextBase) state;
      IVssRequestContext vssRequestContext = httpContext.GetVssRequestContext();
      if (vssRequestContext.ServiceHost.Status != TeamFoundationServiceHostStatus.Stopping)
        return;
      TeamFoundationTracingService.TraceRaw(10017105, TraceLevel.Error, "SignalR", "Middleware", "Cancelling request {0}", (object) vssRequestContext.RequestUri());
      try
      {
        httpContext.Response.StatusCode = 500;
      }
      catch (HttpException ex)
      {
      }
      try
      {
        httpContext.Response.Close();
        TeamFoundationTracingService.TraceRaw(10017105, TraceLevel.Error, "SignalR", "Middleware", "Response closed.");
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(10017105, "SignalR", "Middleware", ex);
      }
      finally
      {
        try
        {
          httpContext.ApplicationInstance.CompleteRequest();
          TeamFoundationTracingService.TraceRaw(10017105, TraceLevel.Error, "SignalR", "Middleware", "Response completed.");
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(10017105, "SignalR", "Middleware", ex);
        }
      }
    }

    private string GetBaseUrl(Uri uri, string toReplace, string toReplaceWith, bool isWebSocket = false)
    {
      string str = (uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.SafeUnescaped) + "/").Replace("/" + toReplace.ToLower() + "/", "/" + toReplaceWith.ToLower());
      return isWebSocket ? str.Replace("https://", "wss://") : str;
    }

    private static MethodInformation GetMethodInformation(IOwinContext context, string action)
    {
      string str1 = action.Substring(0, 1).ToUpperInvariant() + action.Substring(1);
      string str2 = context.Request.Query.Get("connectionData");
      string webMethodName = str1;
      if (!string.IsNullOrEmpty(str2))
      {
        IHubDescriptorProvider descriptorProvider = GlobalHost.DependencyResolver.Resolve<IHubDescriptorProvider>();
        IEnumerable<VssSignalRMiddleware.ClientHubInfo> source = JsonConvert.DeserializeObject<IEnumerable<VssSignalRMiddleware.ClientHubInfo>>(str2);
        if (source != null)
        {
          VssSignalRMiddleware.ClientHubInfo clientHubInfo = source.FirstOrDefault<VssSignalRMiddleware.ClientHubInfo>();
          HubDescriptor descriptor;
          if (clientHubInfo != null && descriptorProvider.TryGetHub(clientHubInfo.Name, out descriptor))
            webMethodName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) descriptor.Name, (object) str1);
        }
      }
      bool isLongRunning = str1.Equals("Connect", StringComparison.Ordinal) || str1.Equals("Reconnect", StringComparison.Ordinal) || str1.Equals("Poll", StringComparison.Ordinal);
      return new MethodInformation(webMethodName, MethodType.Normal, EstimatedMethodCost.Low, false, isLongRunning);
    }

    private class ClientHubInfo
    {
      public string Name { get; set; }
    }
  }
}
