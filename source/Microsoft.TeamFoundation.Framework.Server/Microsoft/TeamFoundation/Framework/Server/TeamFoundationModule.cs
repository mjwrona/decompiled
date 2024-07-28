// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationModule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationModule : IHttpModule
  {
    private AspNetRequestContext m_requestContext;
    private Exception m_requestException;
    private bool m_disableCachingCookieProtection;
    private static readonly HashSet<int> s_excludedStatusCodes = new HashSet<int>()
    {
      401,
      404
    };
    private static readonly HashSet<string> s_webServiceExtensions = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      ".asmx",
      ".svc"
    };
    private static readonly RegistryQuery s_maxDrainBytes = (RegistryQuery) "/Configuration/Afd/RequestDraining/MaxBytes";
    private static readonly RegistryQuery s_maxDrainTimeInMs = (RegistryQuery) "/Configuration/Afd/RequestDraining/MaxTimeInMs";
    private const string forceHeaderCultureFeatureFlag = "VisualStudio.Services.WebAccess.ForceHeaderCulture";
    private static readonly string s_Area = nameof (TeamFoundationModule);
    private static readonly string s_Layer = "WebServices";

    public string ModuleName => nameof (TeamFoundationModule);

    public void Init(HttpApplication application)
    {
      application.BeginRequest += this.WrapEventHandler(new EventHandler(this.Module_BeginRequest), new EventHandler(this.Module_BeginRequestFallback));
      application.EndRequest += this.WrapEventHandler(new EventHandler(this.Module_EndRequest));
      application.Error += this.WrapEventHandler(new EventHandler(this.Module_Error));
      application.PostAuthorizeRequest += this.WrapEventHandler(new EventHandler(this.Module_PostAuthorizeRequest));
      application.PreRequestHandlerExecute += this.WrapEventHandler(new EventHandler(this.Module_PreRequestHandlerExecute));
      application.PostRequestHandlerExecute += this.WrapEventHandler(new EventHandler(this.Module_PostRequestHandlerExecute));
      EventHandlerTaskAsyncHelper handlerTaskAsyncHelper1 = new EventHandlerTaskAsyncHelper(new TaskEventHandler(this.Module_BeginRequestAsync));
      application.AddOnBeginRequestAsync(handlerTaskAsyncHelper1.BeginEventHandler, handlerTaskAsyncHelper1.EndEventHandler);
      EventHandlerTaskAsyncHelper handlerTaskAsyncHelper2 = new EventHandlerTaskAsyncHelper(new TaskEventHandler(this.Module_AddOnPostAuthenticateRequestAsync));
      application.AddOnPostAuthenticateRequestAsync(handlerTaskAsyncHelper2.BeginEventHandler, handlerTaskAsyncHelper2.EndEventHandler);
      EventHandlerTaskAsyncHelper handlerTaskAsyncHelper3 = new EventHandlerTaskAsyncHelper(new TaskEventHandler(this.Module_AddOnPostLogRequestAsync));
      application.AddOnPostLogRequestAsync(handlerTaskAsyncHelper3.BeginEventHandler, handlerTaskAsyncHelper3.EndEventHandler);
    }

    private bool CallHandler(object sender, EventArgs e)
    {
      HttpContext context = ((HttpApplication) sender).Context;
      HttpCookie cookie = context.Request.Cookies["X-VSS-UseRequestRouting"];
      bool flag = context.Items.Contains((object) HttpContextConstants.IsStaticContentRequest);
      return (cookie == null ? 0 : (cookie.Value.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase) ? 1 : 0)) != 0 || !flag;
    }

    private EventHandler WrapEventHandler(EventHandler handler, EventHandler fallbackHandler = null) => (EventHandler) ((sender, e) =>
    {
      if (this.CallHandler(sender, e))
      {
        handler(sender, e);
      }
      else
      {
        EventHandler eventHandler = fallbackHandler;
        if (eventHandler == null)
          return;
        eventHandler(sender, e);
      }
    });

    protected void Module_BeginRequest(object sender, EventArgs e)
    {
      TeamFoundationTracingService.TraceEnterRaw(60010, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_BeginRequest), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      HttpApplicationWrapper application = new HttpApplicationWrapper((HttpApplication) sender);
      HttpContextBase context = application.Context;
      try
      {
        this.SetRequestId(context);
        context.Response.AddOnSendingHeaders(new Action<HttpContextBase>(this.Response_OnSendingHeaders));
        TeamFoundationTracingService.TraceRaw(60011, TraceLevel.Verbose, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "Module_BeginRequest starting. {0} {1}", (object) HttpContext.Current.Request.RequestType, (object) new UrlTracer(HttpContext.Current.Request.RawUrl));
        TeamFoundationApplicationCore.SetPreferredCulture(context, (IVssRequestContext) null);
        if (!TeamFoundationApplicationCore.DeploymentInitialized)
          TeamFoundationApplicationCore.ApplicationStart();
        if (TeamFoundationApplicationCore.DeploymentServiceHost.Status != TeamFoundationServiceHostStatus.Started)
        {
          string str = FrameworkResources.ApplicationIsNotProcessingRequests();
          string statusReason = TeamFoundationApplicationCore.DeploymentServiceHost.StatusReason;
          string responseText = (string) null;
          TeamFoundationTracingService.TraceRaw(60399, TraceLevel.Error, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, str);
          if (!string.IsNullOrEmpty(statusReason))
            str = FrameworkResources.HostOfflineWithAdministratorReasonFormatString((object) str, (object) FrameworkResources.HostOfflineAdministratorReason((object) statusReason));
          TeamFoundationApplicationCore.CompleteRequest((IHttpApplication) application, HttpStatusCode.ServiceUnavailable, (string) null, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
          {
            new KeyValuePair<string, string>("X-VSS-HostOfflineError", str)
          }, (Exception) null, str, responseText);
        }
        else
        {
          using (VssRequestContext systemContext = (VssRequestContext) TeamFoundationApplicationCore.DeploymentServiceHost.CreateSystemContext())
          {
            if (systemContext.ExecutionEnvironment.IsOnPremisesDeployment)
            {
              using (WindowsIdentity current = WindowsIdentity.GetCurrent())
              {
                if (current.ImpersonationLevel == TokenImpersonationLevel.Impersonation)
                {
                  TeamFoundationTracingService.TraceRaw(60397, TraceLevel.Error, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "ASP.net is incorrectly configured!");
                  using (WindowsIdentity.Impersonate(IntPtr.Zero))
                  {
                    TeamFoundationEventLog.Default.Log(FrameworkResources.RequestImpersonationNotSupported(), TeamFoundationEventId.AuthenticationTypeNotSupported, EventLogEntryType.Error);
                    TeamFoundationTracingService.TraceRaw(60015, TraceLevel.Error, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "An impersonated request was blocked because ASP.NET impersonation is incorrectly enabled on the web application");
                  }
                  TeamFoundationApplicationCore.CompleteRequest((IHttpApplication) application, HttpStatusCode.InternalServerError, (string) null, (IEnumerable<KeyValuePair<string, string>>) null, (Exception) null, FrameworkResources.GenericClientErrorMessage((object) DateTime.UtcNow), (string) null);
                  return;
                }
              }
            }
            IpHelper.ResolveClientIp((IVssRequestContext) systemContext, context.Request);
            TeamFoundationHostManagementService service = systemContext.GetService<TeamFoundationHostManagementService>();
            context.Response.AddHeader("X-TFS-ProcessId", service.ProcessId.ToString());
            if (TeamFoundationApplicationCore.UseStrictTransportSecurity && context.Request.IsSecureConnection)
              context.Response.AddHeader("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
            this.m_requestContext = (AspNetRequestContext) systemContext.GetService<IHostRoutingService>().BeginRequest((IVssRequestContext) systemContext, context).requestContext;
            if (this.m_requestContext == null)
            {
              TeamFoundationTracingService.TraceRaw(60454, TraceLevel.Warning, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "Host cannot be found for request {0}.", (object) context.Request.Path);
              TeamFoundationApplicationCore.CompleteRequest((IHttpApplication) application, HttpStatusCode.NotFound, (string) null, (IEnumerable<KeyValuePair<string, string>>) null, (Exception) null, FrameworkResources.ResourceCannotBeFound(), (string) null);
              return;
            }
          }
          this.m_requestContext.TraceEnter(60019, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_BeginRequest));
          this.m_requestContext.RootContext.Items[RequestContextItemsKeys.IsActivity] = (object) true;
          this.m_disableCachingCookieProtection = this.m_requestContext.IsFeatureEnabled("VisualStudio.Services.Caching.DisableCookieProtection");
          if (string.Equals(context.Request.Headers["X-VSS-RequestPriority"], "Low", StringComparison.OrdinalIgnoreCase))
            ((IRequestContextInternal) this.m_requestContext).RequestPriority = VssRequestContextPriority.Low;
          bool requestRouted = context.Items.Contains((object) HttpContextConstants.ArrRequestRouted);
          bool flag = this.SmartRouterBeginRequest(context, ref requestRouted);
          if (requestRouted)
          {
            context.Response.Cache.SetNoServerCaching();
            this.m_requestContext.WebRequestContextInternal().RequestRestrictions = new RequestRestrictions(RequiredAuthentication.Anonymous, AllowedHandler.All, flag ? "SmartRouterRequest" : "ArrRequest", true);
            if (string.IsNullOrEmpty(this.m_requestContext.ServiceName))
              this.m_requestContext.ServiceName = flag ? "SmartRouter" : "ARR";
            MethodInformation methodInformation = new MethodInformation(flag ? "SmartRouter.Proxy" : "ARR.Proxy", MethodType.Normal, EstimatedMethodCost.Moderate);
            methodInformation.AddParameter("RequestUri", (object) this.m_requestContext.RequestUri.AbsoluteUri);
            this.m_requestContext.EnterMethod(methodInformation);
          }
          else
          {
            context.Response.AddHeader("ActivityId", this.m_requestContext.ActivityId.ToString("D"));
            context.Response.AddHeader("X-TFS-Session", this.m_requestContext.UniqueIdentifier.ToString("D"));
            context.Response.AddHeader("X-VSS-E2EID", this.m_requestContext.E2EId.ToString("D"));
            context.Response.AddHeader("X-VSS-SenderDeploymentId", this.m_requestContext.ServiceInstanceId().ToString("D"));
            if (!string.IsNullOrEmpty(this.m_requestContext.OrchestrationId))
              context.Response.AddHeader("X-VSS-OrchestrationId", this.m_requestContext.OrchestrationId);
            if (this.m_requestContext.ServiceHost.ServiceHostInternal().Metabase != null)
            {
              MediaTypeWithQualityHeaderValue parsedValue;
              MediaTypeWithQualityHeaderValue[] array = ((IEnumerable<string>) context.Request.AcceptTypes ?? Enumerable.Empty<string>()).Select<string, MediaTypeWithQualityHeaderValue>((Func<string, MediaTypeWithQualityHeaderValue>) (unparsed => !MediaTypeWithQualityHeaderValue.TryParse(unparsed, out parsedValue) ? (MediaTypeWithQualityHeaderValue) null : parsedValue)).Where<MediaTypeWithQualityHeaderValue>((Func<MediaTypeWithQualityHeaderValue, bool>) (x => x != null)).ToArray<MediaTypeWithQualityHeaderValue>();
              this.m_requestContext.WebRequestContextInternal().RequestRestrictions = this.m_requestContext.ServiceHost.ServiceHostInternal().Metabase.GetRequestRestrictions(this.m_requestContext.RelativePath(), context.Request.UserAgent, (IEnumerable<MediaTypeWithQualityHeaderValue>) array);
            }
            else
              this.m_requestContext.WebRequestContextInternal().RequestRestrictions = new RequestRestrictions(RequiredAuthentication.ValidatedUser, AllowedHandler.All, "Dummy");
          }
          if (!requestRouted)
          {
            string contentType = context.Request.ContentType;
            string header = context.Request.Headers["Content-Encoding"];
            if (header != null && header.Equals("gzip", StringComparison.Ordinal) && contentType != null && contentType.StartsWith("application/soap+xml", StringComparison.Ordinal))
            {
              context.Request.Filter = (Stream) new GZipStream(context.Request.Filter, CompressionMode.Decompress);
              context.Request.Headers.Remove("Content-Encoding");
            }
            IVssRequestContext vssRequestContext = this.m_requestContext.To(TeamFoundationHostType.Deployment);
            CrossOriginManagementService service = vssRequestContext.GetService<CrossOriginManagementService>();
            if (this.m_requestContext.RequestRestrictions().AllowCORS)
            {
              HttpStatusCode? nullable = service.ProcessCORSOptionsRequest(vssRequestContext, context.Request, context.Response.Headers);
              if (nullable.HasValue)
              {
                if (string.IsNullOrEmpty(this.m_requestContext.ServiceName))
                  this.m_requestContext.ServiceName = "Web-Api";
                this.m_requestContext.EnterMethod(new MethodInformation("CORS.Preflight", MethodType.Normal, EstimatedMethodCost.VeryLow));
                context.Response.StatusCode = (int) nullable.Value;
                application.CompleteRequest();
                return;
              }
            }
          }
          if (TeamFoundationApplicationCore.s_sslOnly && !context.Request.IsSecureConnection && !this.m_requestContext.RequestRestrictions().AllowNonSsl)
          {
            TeamFoundationApplicationCore.ApplySslRestriction((IHttpApplication) application);
          }
          else
          {
            context.Items[(object) HttpContextConstants.IVssRequestContext] = (object) this.m_requestContext;
            if (context.Items.Contains((object) HttpContextConstants.ServiceHostRouteContext))
              this.m_requestContext.Items.Add(RequestContextItemsKeys.ServiceHostRouteContext, context.Items[(object) HttpContextConstants.ServiceHostRouteContext]);
            if (TeamFoundationApplicationCore.s_deploymentDatabase != null)
              TeamFoundationApplicationCore.SetCultures(context, (IVssRequestContext) this.m_requestContext);
            if (this.m_requestContext.RequestRestrictions().AllowedHandlers == AllowedHandler.None)
            {
              TeamFoundationTracingService.TraceRaw(60455, TraceLevel.Warning, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "Invalid Access Exception");
              TeamFoundationApplicationCore.CompleteRequest((IHttpApplication) application, HttpStatusCode.Forbidden, FrameworkResources.InvalidAccessException(), (IEnumerable<KeyValuePair<string, string>>) null, (Exception) null, FrameworkResources.InvalidAccessException(), (string) null);
            }
            else
            {
              if (this.m_requestContext.ServiceHost.ServiceHostInternal().RequestFilters != null)
              {
                foreach (ITeamFoundationRequestFilter requestFilter in (IEnumerable<ITeamFoundationRequestFilter>) this.m_requestContext.ServiceHost.ServiceHostInternal().RequestFilters)
                {
                  try
                  {
                    requestFilter.BeginRequest((IVssRequestContext) this.m_requestContext);
                  }
                  catch (RequestFilterException ex)
                  {
                    TeamFoundationTracingService.TraceRaw(60458, TraceLevel.Error, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "RequestFilter {0} failing request with {1}", (object) requestFilter.ToString(), (object) ex.Message);
                    TeamFoundationApplicationCore.CompleteRequest((IHttpApplication) application, ex.HttpStatusCode, ex.Message, (IEnumerable<KeyValuePair<string, string>>) null, (Exception) ex, (string) null, (string) null);
                    return;
                  }
                  catch (Exception ex)
                  {
                    TeamFoundationTracingService.TraceExceptionRaw(60024, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
                  }
                }
              }
              this.m_requestContext.Trace(60025, TraceLevel.Verbose, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "Beginning Request {0}", (object) this.m_requestContext.ContextId);
              this.m_requestContext.ServiceHost.BeginRequest((IVssRequestContext) this.m_requestContext);
            }
          }
        }
      }
      catch (ThreadAbortException ex)
      {
        TeamFoundationTracingService.TraceRaw(60030, TraceLevel.Warning, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "We probably called CompleteRequest, Redirect or Context.Response.End() during Application_BeginRequest.", (object) ex);
        throw;
      }
      catch (HostShutdownException ex)
      {
        this.TraceException(60450, (Exception) ex);
        throw;
      }
      catch (VssServiceResponseException ex)
      {
        this.TraceException(60451, (Exception) ex);
        throw;
      }
      catch (CircuitBreakerException ex)
      {
        this.TraceException(60452, (Exception) ex);
        throw;
      }
      catch (RequestFilterException ex)
      {
        if (this.m_requestContext != null)
          this.m_requestContext.TraceException(60453, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, (Exception) ex);
        TeamFoundationApplicationCore.CompleteRequest((IHttpApplication) application, ex.HttpStatusCode, ex.Message, (IEnumerable<KeyValuePair<string, string>>) null, (Exception) ex, (string) null, (string) null);
      }
      catch (Exception ex)
      {
        if (this.m_requestContext != null)
          this.m_requestContext.TraceException(60026, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
        TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) this.m_requestContext, (IHttpApplication) application, HttpStatusCode.InternalServerError, ex);
      }
      finally
      {
        if (this.m_requestContext != null)
          this.m_requestContext.TraceLeave(60027, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_BeginRequest));
        TeamFoundationTracingService.TraceLeaveRaw(60031, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_BeginRequest));
      }
    }

    protected void Module_BeginRequestFallback(object sender, EventArgs e) => new HttpApplicationWrapper((HttpApplication) sender).Context.Response.AddOnSendingHeaders(new Action<HttpContextBase>(this.Response_OnSendingHeaders));

    private void Module_Error(object sender, EventArgs e)
    {
      if (this.m_requestException != null)
        return;
      this.m_requestException = ((HttpApplication) sender).Server.GetLastError();
    }

    protected void Module_EndRequest(object sender, EventArgs e)
    {
      try
      {
      }
      finally
      {
        TeamFoundationTracingService.TraceEnterRaw(60040, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_EndRequest), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
        try
        {
          HttpApplication httpApplication = (HttpApplication) sender;
          HttpContext context = httpApplication.Context;
          try
          {
            try
            {
              if (this.m_requestContext != null)
              {
                if (this.m_requestContext.Method != null)
                  context.Items.Add((object) "VssfTelemetryProcessor.Command", (object) this.m_requestContext.Method.Name);
              }
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(60186, TraceLevel.Info, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
            }
            try
            {
              if (context.Response.StatusCode >= 400)
              {
                if (context.Request.HttpMethod != "GET")
                {
                  if (context.Request.HttpMethod != "HEAD")
                  {
                    if (context.Request.Headers["X-FD-Ref"] != null)
                    {
                      if (this.m_requestContext != null)
                      {
                        if (this.m_requestContext.IsFeatureEnabled("VisualStudio.Services.Afd.ErrorRequestDraining"))
                        {
                          IVssRegistryService service = this.m_requestContext.GetService<IVssRegistryService>();
                          int maxBytes = service.GetValue<int>((IVssRequestContext) this.m_requestContext, in TeamFoundationModule.s_maxDrainBytes, 33554432);
                          int maxMs = service.GetValue<int>((IVssRequestContext) this.m_requestContext, in TeamFoundationModule.s_maxDrainTimeInMs, 300000);
                          TeamFoundationModule.DrainRequestStream((IVssRequestContext) this.m_requestContext, context, maxBytes, maxMs);
                        }
                      }
                    }
                  }
                }
              }
            }
            catch (Exception ex)
            {
              if (this.m_requestContext != null)
                this.m_requestContext.TraceException(60184, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
              else
                TeamFoundationTracingService.TraceExceptionRaw(60184, TraceLevel.Error, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
            }
            try
            {
              if (httpApplication.Response.Headers["X-VSS-UserData"] == null)
              {
                Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) context.Items[(object) HttpContextConstants.UserIdentityInternal];
                if (httpApplication.Request.Headers.Get("X-VSS-UserData") != null)
                {
                  if (identity != null)
                  {
                    string str = identity.Id.ToString("D");
                    string property = identity.GetProperty<string>("Account", string.Empty);
                    if (!string.IsNullOrWhiteSpace(property))
                      str = str + ":" + property;
                    httpApplication.Response.Headers.Add("X-VSS-UserData", str);
                  }
                }
              }
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(60185, TraceLevel.Warning, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
            }
            if (this.m_requestException != null && (this.m_requestContext == null || this.m_requestContext.ServiceHost == null))
            {
              using (VssRequestContext systemContext = TeamFoundationApplicationCore.DeploymentServiceHost?.CreateSystemContext() as VssRequestContext)
              {
                if (systemContext != null)
                  TeamFoundationModule.LogAbortiveRequest((IVssRequestContext) systemContext, (HttpContextBase) new HttpContextWrapper(context), this.m_requestException, "Module_BeginRequest_Failure");
                else
                  TeamFoundationTracingService.TraceExceptionRaw(512837977, TraceLevel.Warning, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, this.m_requestException, "No deployment host was available for activity logging.");
              }
            }
            if (this.m_requestContext != null)
            {
              this.m_requestContext.Trace(60040, TraceLevel.Info, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_EndRequest));
              try
              {
                IVssRequestContext vssRequestContext = this.m_requestContext.To(TeamFoundationHostType.Deployment);
                vssRequestContext.GetService<IHostRoutingService>().EndRoutedContext(vssRequestContext, (HttpContextBase) new HttpContextWrapper(context));
              }
              catch (Exception ex)
              {
                this.m_requestContext.TraceException(60047, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
              }
              if (((IEnumerable<string>) httpApplication.Response.Cookies.AllKeys).Contains<string>("FedAuth"))
              {
                int num1 = 0;
                int num2 = 0;
                foreach (string allKey in httpApplication.Response.Cookies.AllKeys)
                {
                  if (allKey.StartsWith("FedAuth"))
                  {
                    ++num1;
                    string str = httpApplication.Response.Cookies[allKey]?.Value;
                    num2 += (str ?? "").Length;
                  }
                }
                this.m_requestContext.TraceAlways(60050, TraceLevel.Warning, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "The request wrote {0} new FedAuth cookies of total size {1}", (object) num1, (object) num2);
              }
              this.m_requestContext.RequestContextInternal().SetResponseCode(httpApplication.Response.StatusCode);
              if (this.m_requestContext.Method == null && !TeamFoundationModule.s_excludedStatusCodes.Contains(httpApplication.Response.StatusCode))
              {
                if (!this.m_requestContext.Items.TryGetValue<bool>(RequestContextItemsKeys.RequestCacheable, out bool _))
                {
                  try
                  {
                    this.m_requestContext.RootContext.EnterMethod(new MethodInformation("Module_EndRequest_NoAssociatedMethod", MethodType.Normal, EstimatedMethodCost.Moderate));
                  }
                  catch (Exception ex) when (ex is RequestCanceledException || ex is RequestFilterException)
                  {
                    TeamFoundationTracingService.TraceRaw(60043, TraceLevel.Info, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex.Message);
                  }
                  catch (Exception ex)
                  {
                    TeamFoundationTracingService.TraceExceptionRaw(60045, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
                  }
                }
              }
              if (this.m_requestContext.ServiceHost != null)
                this.m_requestContext.ServiceHost.ServiceHostInternal().ApplicationEndRequest((IVssRequestContext) this.m_requestContext);
            }
            if (context.Items != null)
            {
              if (context.Items.Contains((object) "RequestId"))
              {
                if (TeamFoundationApplicationCore.DeploymentServiceHost != null)
                  TeamFoundationApplicationCore.DeploymentServiceHostInternal.HostManagement.AssertNoLocksHeld(this.m_requestContext != null ? this.m_requestContext.ActivityId.ToString() : string.Empty);
              }
            }
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(60042, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
            throw;
          }
          finally
          {
            this.m_requestContext = (AspNetRequestContext) null;
            this.m_requestException = (Exception) null;
            if (context.Items != null)
            {
              context.Items[(object) HttpContextConstants.IVssRequestContext] = (object) null;
              TeamFoundationTracingService.TraceRaw(60041, TraceLevel.Verbose, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "Module_EndRequest ending. {0} {1}", (object) HttpContext.Current.Request.RequestType, (object) new UrlTracer(HttpContext.Current.Request.RawUrl));
            }
          }
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(60044, TraceLevel.Error, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
          throw;
        }
        finally
        {
          TeamFoundationTracingService.TraceLeaveRaw(60046, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_EndRequest));
        }
      }
    }

    internal static void LogAbortiveRequest(
      IVssRequestContext deploymentContext,
      HttpContextBase httpContext,
      Exception exception,
      string commandTag)
    {
      Guid guid1 = Guid.Empty;
      Guid result = Guid.Empty;
      string str1 = string.Empty;
      Guid guid2 = Guid.Empty;
      string str2 = string.Empty;
      string str3 = string.Empty;
      string str4 = string.Empty;
      string empty1 = string.Empty;
      try
      {
        guid1 = httpContext.Items[(object) HttpContextConstants.ServiceHostRouteContext] is HostRouteContext hostRouteContext ? hostRouteContext.HostId : deploymentContext.ServiceHost.InstanceId;
        Guid.TryParse(httpContext.Request.Headers["X-VSS-E2EID"] ?? string.Empty, out result);
        str1 = httpContext.Request.Headers["X-VSS-OrchestrationId"] ?? string.Empty;
        try
        {
          str3 = httpContext.Request.UrlReferrer?.AbsolutePath ?? string.Empty;
        }
        catch
        {
          str3 = httpContext.Request.Headers["Referer"] ?? string.Empty;
        }
        try
        {
          Uri url = httpContext.Request.Url;
          if (url != (Uri) null)
            str2 = !url.IsAbsoluteUri ? url.OriginalString : url.AbsolutePath;
        }
        catch
        {
        }
        str4 = httpContext.Request.UserAgent;
        string str5 = httpContext.Request.Headers["X-TFS-Session"] ?? httpContext.Request.Headers["X-TFS-Instance"] ?? httpContext.Request.Headers["X-VersionControl-Instance"];
        if (str5 != null)
        {
          string[] strArray = str5.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
          if (strArray.Length >= 1)
            guid2 = new Guid(strArray[0].Trim());
        }
        HttpCookie cookie = httpContext.Request.Cookies["VstsSession"];
        if (cookie != null)
        {
          if (!string.IsNullOrEmpty(cookie.Value))
          {
            try
            {
              empty1 = JsonConvert.DeserializeObject<SessionTrackingState>(Uri.UnescapeDataString(cookie.Value)).PersistentSessionId.ToString();
            }
            catch (JsonException ex)
            {
            }
          }
        }
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(21676910, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
      }
      int num1;
      switch (exception)
      {
        case ThreadAbortException _:
        case RequestCanceledException _:
        case HostShutdownException _:
        case VssServiceResponseException _:
        case CircuitBreakerException _:
        case RequestFilterException _:
          num1 = 1;
          break;
        case HttpException _:
          HttpResponseBase response1 = httpContext.Response;
          if ((response1 != null ? (response1.StatusCode >= 400 ? 1 : 0) : 0) != 0)
          {
            HttpResponseBase response2 = httpContext.Response;
            num1 = response2 != null ? (response2.StatusCode < 500 ? 1 : 0) : 0;
            break;
          }
          goto default;
        default:
          num1 = 0;
          break;
      }
      bool flag = num1 != 0;
      ActivityStatus activityStatus1 = ActivityStatus.Success;
      if (exception != null && !flag)
        activityStatus1 = ActivityStatus.Failed;
      TeamFoundationTracingService service = deploymentContext.GetService<TeamFoundationTracingService>();
      Guid hostId = guid1;
      string command = commandTag;
      DateTime startTime = deploymentContext.StartTime();
      long delayTime = deploymentContext.DelayTime();
      long executionTime = deploymentContext.ExecutionTime();
      Guid uniqueIdentifer = guid2;
      string userAgent = str4;
      string loggingExceptionName = TeamFoundationTracingService.GetActivityLoggingExceptionName(exception);
      string message = exception?.Message;
      Guid activityId = guid2;
      HttpResponseBase response3 = httpContext.Response;
      int responseCode = response3 != null ? response3.StatusCode : 500;
      Guid empty2 = Guid.Empty;
      Guid empty3 = Guid.Empty;
      Guid empty4 = Guid.Empty;
      Guid empty5 = Guid.Empty;
      long firstPage = deploymentContext.TimeToFirstPage();
      int activityStatus2 = (int) activityStatus1;
      int num2 = flag ? 1 : 0;
      DateTime hostStartTime = DateTime.FromFileTimeUtc(0L);
      Guid empty6 = Guid.Empty;
      string anonymousIdentifier = empty1;
      Guid E2EId = result;
      Guid empty7 = Guid.Empty;
      Guid empty8 = Guid.Empty;
      Guid empty9 = Guid.Empty;
      long queueTime = deploymentContext.QueueTime();
      string referrer = str3;
      string uriStem = str2;
      Guid empty10 = Guid.Empty;
      string orchestrationId = str1;
      Guid empty11 = Guid.Empty;
      service.TraceActivityLog(hostId, 0L, (string) null, command, -1, 1, startTime, delayTime, 0L, executionTime, (string) null, (string) null, uniqueIdentifer, userAgent, (string) null, loggingExceptionName, message, activityId, responseCode, empty2, empty3, empty4, empty5, firstPage, activityStatus2, (string) null, 0L, num2 != 0, 0, 0, 0, 0, (string) null, hostStartTime, (byte) 0, empty6, anonymousIdentifier, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0L, 0, E2EId, empty7, empty8, empty9, queueTime, 0.0, (string) null, referrer, uriStem, (byte) 0, empty10, 0L, 0L, 0L, 0L, orchestrationId, 0, 0, 0, 0L, (string) null, (string) null, (string) null, empty11);
    }

    private static void DrainRequestStream(
      IVssRequestContext requestContext,
      HttpContext context,
      int maxBytes,
      int maxMs)
    {
      try
      {
        using (requestContext.CreateTimeToFirstPageExclusionBlock())
        {
          if (context.Request.ReadEntityBodyMode != ReadEntityBodyMode.None && context.Request.ReadEntityBodyMode != ReadEntityBodyMode.Bufferless)
            return;
          CommandPropertiesSetter commandPropertiesDefaults = new CommandPropertiesSetter().WithExecutionMaxConcurrentRequests(64).WithExecutionTimeout(TimeSpan.MaxValue);
          CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) "TFModule.DrainRequestStream").AndCommandPropertiesDefaults(commandPropertiesDefaults);
          new CommandService(requestContext, setter, (Action) (() =>
          {
            requestContext.Trace(60380, TraceLevel.Info, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "Draining request stream");
            Stream bufferlessInputStream = context.Request.GetBufferlessInputStream(true);
            DateTime utcNow = DateTime.UtcNow;
            int num1 = 0;
            double totalMilliseconds;
            using (ByteArray byteArray = new ByteArray(16384))
            {
              int num2;
              do
              {
                num2 = bufferlessInputStream.Read(byteArray.Bytes, 0, byteArray.Bytes.Length);
                num1 += num2;
                totalMilliseconds = (DateTime.UtcNow - utcNow).TotalMilliseconds;
                if (num1 > maxBytes)
                {
                  requestContext.TraceAlways(60380, TraceLevel.Warning, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "Request stream drain max bytes reached. Bytes: {0}; Time: {1}ms", (object) num1, (object) totalMilliseconds);
                  return;
                }
                if (totalMilliseconds > (double) maxMs)
                {
                  requestContext.TraceAlways(60380, TraceLevel.Warning, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "Request stream drain max time reached. Bytes: {0}; Time: {1}ms", (object) num1, (object) totalMilliseconds);
                  return;
                }
              }
              while (num2 != 0);
            }
            requestContext.Trace(60380, TraceLevel.Info, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "Request stream drained. Bytes: {0}; Time: {1}ms", (object) num1, (object) totalMilliseconds);
          })).Execute();
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(60380, TraceLevel.Error, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
      }
    }

    private async Task Module_AddOnPostAuthenticateRequestAsync(object sender, EventArgs e)
    {
      HttpApplicationWrapper application = new HttpApplicationWrapper((HttpApplication) sender);
      HttpContextBase context = application.Context;
      if (this.m_requestContext == null)
        application = (HttpApplicationWrapper) null;
      else if (!this.CallHandler(sender, e))
      {
        application = (HttpApplicationWrapper) null;
      }
      else
      {
        this.m_requestContext.TraceEnter(60060, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_AddOnPostAuthenticateRequestAsync));
        try
        {
          if (TeamFoundationApplicationCore.s_deploymentDatabase == null)
          {
            this.m_requestContext.Trace(60061, TraceLevel.Verbose, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "Deployment database is null - we're done.");
            application = (HttpApplicationWrapper) null;
          }
          else
          {
            try
            {
              if (context.User != null && context.User.Identity.IsAuthenticated && !this.m_requestContext.ExecutionEnvironment.IsOnPremisesDeployment && !this.m_requestContext.IsFeatureEnabled("VisualStudio.Services.WebAccess.ForceHeaderCulture"))
              {
                this.m_requestContext.Trace(60067, TraceLevel.Verbose, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "Getting user culture.");
                CultureInfo culture = this.m_requestContext.GetService<IUserPreferencesService>().GetUserPreferences((IVssRequestContext) this.m_requestContext).Culture;
                if (culture != null)
                  Thread.CurrentThread.CurrentCulture = culture;
              }
              Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) context.Items[(object) HttpContextConstants.UserIdentityInternal];
              if (identity != null)
              {
                string str = identity.Id.ToString("D");
                string property = identity.GetProperty<string>("Account", string.Empty);
                if (!string.IsNullOrWhiteSpace(property))
                  str = str + ":" + property;
                application.Response.Headers.Add("X-VSS-UserData", str);
              }
              if (!context.Items.Contains((object) HttpContextConstants.ArrRequestRouted))
              {
                if (string.Equals("HEAD", application.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
                {
                  TeamFoundationAuthenticationService service = this.m_requestContext.GetService<TeamFoundationAuthenticationService>();
                  service.AddFederatedAuthHeaders((IVssRequestContext) this.m_requestContext, context.Response);
                  service.AddTenantInfoResponseHeader((IVssRequestContext) this.m_requestContext, context.Response);
                }
                IVssRequestContext vssRequestContext = this.m_requestContext.To(TeamFoundationHostType.Deployment);
                CrossOriginManagementService service1 = vssRequestContext.GetService<CrossOriginManagementService>();
                RequestRestrictions requestRestrictions = this.m_requestContext.RequestRestrictions();
                if (requestRestrictions.AllowCORS)
                  service1.ProcessCORSHeaders(vssRequestContext, context.Request, context.Response.Headers);
                if (requestRestrictions.RequiredAuthentication > RequiredAuthentication.Anonymous)
                  service1.AddXFrameOptionsHeader(context, false);
              }
            }
            catch (Exception ex)
            {
              this.m_requestContext.TraceException(60068, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
              TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) this.m_requestContext, (IHttpApplication) application, HttpStatusCode.InternalServerError, ex);
              application = (HttpApplicationWrapper) null;
              return;
            }
            if (this.m_requestContext.ServiceHost.ServiceHostInternal().RequestFilters == null)
            {
              application = (HttpApplicationWrapper) null;
            }
            else
            {
              foreach (ITeamFoundationRequestFilter requestFilter in (IEnumerable<ITeamFoundationRequestFilter>) this.m_requestContext.ServiceHost.ServiceHostInternal().RequestFilters)
              {
                this.m_requestContext.Trace(60069, TraceLevel.Verbose, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "Calling PostAuthenticate of the request filter {0}.", (object) requestFilter);
                try
                {
                  await requestFilter.PostAuthenticateRequest((IVssRequestContext) this.m_requestContext);
                }
                catch (RequestFilterException ex)
                {
                  this.m_requestContext.TraceException(60071, TraceLevel.Error, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, (Exception) ex);
                  TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) this.m_requestContext, (IHttpApplication) application, ex.HttpStatusCode, (Exception) ex);
                  application = (HttpApplicationWrapper) null;
                  return;
                }
                catch (Exception ex)
                {
                  this.m_requestContext.TraceException(60072, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
                }
              }
              application = (HttpApplicationWrapper) null;
            }
          }
        }
        finally
        {
          this.m_requestContext.TraceLeave(60073, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_AddOnPostAuthenticateRequestAsync));
        }
      }
    }

    private async Task Module_BeginRequestAsync(object sender, EventArgs e)
    {
      if (this.m_requestContext == null)
        return;
      this.m_requestContext.TraceEnter(60083, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_BeginRequestAsync));
      try
      {
        if (this.m_requestContext.ServiceHost.ServiceHostInternal().RequestFilters == null)
          return;
        foreach (ITeamFoundationRequestFilter requestFilter in (IEnumerable<ITeamFoundationRequestFilter>) this.m_requestContext.ServiceHost.ServiceHostInternal().RequestFilters)
        {
          this.m_requestContext.Trace(60084, TraceLevel.Verbose, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, "Calling BeginRequestAsync of the request filter {0}.", (object) requestFilter);
          try
          {
            await requestFilter.BeginRequestAsync((IVssRequestContext) this.m_requestContext);
          }
          catch (RequestFilterException ex)
          {
            this.m_requestContext.TraceException(60085, TraceLevel.Error, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, (Exception) ex);
            TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) this.m_requestContext, (IHttpApplication) new HttpApplicationWrapper((HttpApplication) sender), ex.HttpStatusCode, (Exception) ex);
          }
          catch (Exception ex)
          {
            this.m_requestContext.TraceException(60086, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
          }
        }
      }
      finally
      {
        this.m_requestContext.TraceLeave(60087, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_BeginRequestAsync));
      }
    }

    private async Task Module_AddOnPostLogRequestAsync(object sender, EventArgs e)
    {
      if (this.m_requestContext == null)
        return;
      this.m_requestContext.RequestContextInternal().ResetCancel();
      IVssServiceHost serviceHost = this.m_requestContext.ServiceHost;
      if ((serviceHost != null ? serviceHost.ServiceHostInternal()?.RequestFilters : (IDisposableReadOnlyList<ITeamFoundationRequestFilter>) null) == null)
        return;
      foreach (ITeamFoundationRequestFilter requestFilter in (IEnumerable<ITeamFoundationRequestFilter>) this.m_requestContext.ServiceHost.ServiceHostInternal().RequestFilters)
      {
        try
        {
          await requestFilter.PostLogRequestAsync((IVssRequestContext) this.m_requestContext);
        }
        catch (RequestFilterException ex)
        {
          this.m_requestContext.TraceException(60074, TraceLevel.Error, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, (Exception) ex);
          TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) this.m_requestContext, (IHttpApplication) new HttpApplicationWrapper((HttpApplication) sender), ex.HttpStatusCode, (Exception) ex);
          break;
        }
        catch (Exception ex)
        {
          this.m_requestContext.TraceException(60075, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex);
        }
      }
    }

    private bool SmartRouterBeginRequest(HttpContextBase httpContext, ref bool requestRouted)
    {
      if (requestRouted)
        return false;
      if (!this.m_requestContext.IsFeatureEnabled("VisualStudio.Services.Cloud.SmartRouter.Enabled"))
        return false;
      try
      {
        int num = this.m_requestContext.GetService<ISmartRouterRequestHandlerService>().BeginRequest((IVssWebRequestContext) this.m_requestContext) ? 1 : 0;
        if (num != 0)
          requestRouted = httpContext.Items.Contains((object) HttpContextConstants.ArrRequestRouted);
        return num != 0;
      }
      catch (Exception ex)
      {
        this.TraceException(60076, ex);
      }
      return false;
    }

    private void TraceException(int tracepoint, Exception exception)
    {
      if (this.m_requestContext != null)
        this.m_requestContext.TraceException(tracepoint, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, exception);
      else
        TeamFoundationTracingService.TraceRaw(tracepoint, TraceLevel.Error, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, exception.Message);
    }

    private void Module_PostAuthorizeRequest(object sender, EventArgs e)
    {
      try
      {
        this.m_requestContext.TraceEnter(60180, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_PostAuthorizeRequest));
      }
      finally
      {
        this.m_requestContext.TraceLeave(60181, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_PostAuthorizeRequest));
      }
    }

    private void Module_PreRequestHandlerExecute(object sender, EventArgs e)
    {
      try
      {
        this.m_requestContext.TraceEnter(60190, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_PreRequestHandlerExecute));
      }
      finally
      {
        this.m_requestContext.TraceLeave(60191, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_PreRequestHandlerExecute));
      }
    }

    private void Module_PostRequestHandlerExecute(object sender, EventArgs e)
    {
      try
      {
        this.m_requestContext.TraceEnter(60195, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_PostRequestHandlerExecute));
        TeamFoundationModule.PostRequestHandlerExecuteValidate((IVssWebRequestContext) this.m_requestContext);
        this.m_requestContext.LeaveMethod();
      }
      finally
      {
        this.m_requestContext.TraceLeave(60196, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, nameof (Module_PostRequestHandlerExecute));
      }
    }

    internal static void PostRequestHandlerExecuteValidate(IVssWebRequestContext requestContext)
    {
      HttpContextBase httpContext = requestContext.WebRequestContextInternal().HttpContext;
      if (requestContext.RequestTimer.PreControllerTime == 0L)
      {
        if (requestContext.Method != null && httpContext.Response.StatusCode >= 200 && httpContext.Response.StatusCode < 300 && !requestContext.ServiceName.Equals("SignalR", StringComparison.Ordinal) && !string.Equals(requestContext.Method.Name, "ARR.Proxy", StringComparison.Ordinal))
        {
          string format = "PreControllerTime was not set on request.";
          requestContext.TraceAlways(89366779, TraceLevel.Error, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, format);
        }
        else
          requestContext.RequestTimer.SetPreControllerTime();
      }
      if (requestContext.ServiceHost.DeploymentServiceHost.HasDatabaseAccess && !httpContext.Items.Contains((object) HttpContextConstants.ArrRequestRouted) && httpContext.Handler != null && (!TeamFoundationModule.s_webServiceExtensions.Contains(httpContext.Request.CurrentExecutionFilePathExtension) || !httpContext.Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase)) && (requestContext.Method != null || httpContext.Response.StatusCode > 0 && httpContext.Response.StatusCode < 400))
      {
        IdentityValidationStatus validationStatus = requestContext.RequestContextInternal().IdentityValidationStatus;
        string format = (string) null;
        if (!validationStatus.HasFlag((System.Enum) IdentityValidationStatus.Validated))
        {
          format = string.Format("Request to {0} completed without running through identity validation", (object) requestContext.RequestUri);
          requestContext.TraceAlways(72383215, TraceLevel.Error, TeamFoundationModule.s_Area, "AnonymousAccessKalypsoAlert", format);
        }
        if (!validationStatus.HasFlag((System.Enum) IdentityValidationStatus.DelayedIdentityValidation))
        {
          format = string.Format("Request to {0} completed without running through delayed identity validation", (object) requestContext.RequestUri);
          requestContext.TraceAlways(73383215, TraceLevel.Error, TeamFoundationModule.s_Area, "AnonymousAccessKalypsoAlert", format);
        }
        if (!string.IsNullOrEmpty(format) && requestContext.IsFeatureEnabled("VisualStudio.Services.DelayedIdentityValidation"))
          throw new RequestValidationException(FrameworkResources.RequestValidationException((object) format));
      }
      if (requestContext.Method != null || TeamFoundationModule.s_excludedStatusCodes.Contains(httpContext.Response.StatusCode))
        return;
      requestContext.RootContext.EnterMethod(new MethodInformation("Module_PostRequestHandlerExecute_NoAssociatedMethod", MethodType.Normal, EstimatedMethodCost.Moderate));
    }

    private void Response_OnSendingHeaders(HttpContextBase context)
    {
      try
      {
        HttpResponseBase response = context.Response;
        int count = response.Cookies.Count;
        response.Headers.Remove("Server");
        response.Headers.Remove("X-AspNet-Version");
        response.Headers.Remove("X-AspNetMvc-Version");
        response.Headers.Remove("X-Powered-By");
        if (count > 0 && !this.m_disableCachingCookieProtection)
        {
          response.Cache.SetCacheability(HttpCacheability.NoCache);
          response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
          response.Cache.SetExpires(DateTime.MinValue);
          response.Cache.SetNoStore();
        }
        context.Request.ServerVariables["VSS_RESPONSE_COOKIE_COUNT"] = count.ToString();
      }
      catch (Exception ex1)
      {
        try
        {
          TeamFoundationTracingService.TraceExceptionRaw(1209673774, TeamFoundationModule.s_Area, TeamFoundationModule.s_Layer, ex1);
        }
        catch (Exception ex2)
        {
        }
      }
    }

    public void Dispose()
    {
    }

    private void SetRequestId(HttpContextBase context)
    {
      if (context.Items.Contains((object) "RequestId"))
        return;
      context.Items[(object) "RequestId"] = (object) LockHelperContext.NewRequestId();
    }
  }
}
