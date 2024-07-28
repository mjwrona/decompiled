// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VisualStudioServicesApplication
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VisualStudioServicesApplication : HttpApplication
  {
    private static readonly object s_RegisterApplicationLock = new object();
    protected static bool s_isWebAppRegistered = false;
    protected static int s_registrationAttemptsRemaining = 10;
    private static DateTime s_nextRegistrationAttemptTime = DateTime.MinValue;
    private const int SecondsBetweenRegistrationAttempts = 60;
    private const string c_area = "VisualStudioServicesApplication";
    private const string c_layer = "WebServices";

    public VisualStudioServicesApplication()
    {
      string appSetting = TeamFoundationApplicationCore.AppSettings["tempDirectory"];
      if (string.IsNullOrEmpty(appSetting))
        return;
      Environment.SetEnvironmentVariable("TEMP", appSetting);
      Environment.SetEnvironmentVariable("TMP", appSetting);
    }

    private WebRequestContext RequestContext => (WebRequestContext) this.Context.Items[(object) HttpContextConstants.IVssRequestContext];

    public virtual IVssRequestContext VssRequestContext => (IVssRequestContext) this.Context.Items[(object) HttpContextConstants.IVssRequestContext];

    internal void ReportError(Exception exception)
    {
      if (TeamFoundationApplicationCore.DeploymentServiceHost == null)
      {
        if (exception is TeamFoundationServiceException serviceException && !serviceException.LogException)
          return;
        TeamFoundationEventLog.Default.LogException((IVssRequestContext) null, FrameworkResources.UnhandledExceptionError(), exception);
      }
      else
      {
        bool flag = true;
        ErrorReporterDelegate onReportError = this.OnReportError;
        if (onReportError != null)
        {
          try
          {
            foreach (ErrorReporterDelegate invocation in onReportError.GetInvocationList())
            {
              bool? nullable = invocation(exception);
              if (nullable.HasValue)
              {
                flag = nullable.Value;
                break;
              }
            }
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceRaw(60080, TraceLevel.Warning, nameof (VisualStudioServicesApplication), "WebServices", "Error reporter caused an exception. error: {0}; reporterError: {1}", (object) exception, (object) ex);
          }
        }
        if (!flag)
          return;
        ExceptionHandlerUtility.HandleException(exception);
      }
    }

    protected void Application_Start(object sender, EventArgs e)
    {
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.UnhandledExceptionHandler);
      try
      {
        TeamFoundationTraceListener.SetDefaultListener(string.Empty);
      }
      catch (ConfigurationErrorsException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(60392, nameof (VisualStudioServicesApplication), "WebServices", (Exception) ex);
        TeamFoundationEventLog.Default.Log(FrameworkResources.FailedToReadTracingConfiguration((object) ex.Message), TeamFoundationEventId.InvalidTracingConfiguration, EventLogEntryType.Warning);
      }
      try
      {
        TeamFoundationApplicationCore.ApplicationStart();
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(60394, nameof (VisualStudioServicesApplication), "WebServices", ex);
      }
    }

    protected void Application_End(object sender, EventArgs e)
    {
      if (TeamFoundationApplicationCore.DeploymentServiceHost != null)
        TeamFoundationApplicationCore.ApplicationEnd();
      string message = HostingEnvironment.ShutdownReason == ApplicationShutdownReason.None ? FrameworkResources.ApplicationRequestEndMessage() : FrameworkResources.ShutdownReason((object) HostingEnvironment.ShutdownReason.ToString());
      TeamFoundationEventLog.Default.Log(message, TeamFoundationEventId.ApplicationStopped, EventLogEntryType.Information);
    }

    protected void Application_Error(object sender, EventArgs e)
    {
      WebRequestContext requestContext = this.RequestContext;
      Exception lastError = this.Server.GetLastError();
      if (lastError == null)
        return;
      this.Server.ClearError();
      HttpApplicationWrapper application = new HttpApplicationWrapper((HttpApplication) sender);
      this.ProcessError((IVssRequestContext) requestContext, (IHttpApplication) application, lastError);
    }

    internal void ProcessError(
      IVssRequestContext requestContext,
      IHttpApplication application,
      Exception serverException)
    {
      HttpStatusCode statusCode = (HttpStatusCode) 0;
      KeyValuePair<string, string>[] extraHeaders = (KeyValuePair<string, string>[]) null;
      bool flag = true;
      if (requestContext != null)
        requestContext.RequestContextInternal().ResetCancel();
      switch (serverException)
      {
        case HttpException _:
          if (((HttpException) serverException).GetHttpCode() == 404)
          {
            flag = TeamFoundationApplicationCore.Log404NotFoundErrors;
            string str = (string) null;
            try
            {
              str = application.Request.Url.AbsoluteUri;
            }
            catch (Exception ex)
            {
            }
            if (requestContext != null)
            {
              requestContext.Trace(60002, TraceLevel.Error, nameof (VisualStudioServicesApplication), "WebServices", "404 Not Found. Requested Url: {0}", (object) str);
              break;
            }
            TeamFoundationTracingService.TraceRaw(60002, TraceLevel.Error, nameof (VisualStudioServicesApplication), "WebServices", "404 Not Found. Requested Url: {0}", (object) str);
            break;
          }
          break;
        case HostShutdownException _:
          statusCode = HttpStatusCode.ServiceUnavailable;
          extraHeaders = new KeyValuePair<string, string>[1]
          {
            new KeyValuePair<string, string>("X-VSS-HostOfflineError", serverException.Message)
          };
          break;
        case VssServiceResponseException _:
          statusCode = (serverException as VssServiceResponseException).HttpStatusCode;
          break;
        case CircuitBreakerException _:
          statusCode = HttpStatusCode.ServiceUnavailable;
          break;
        case RequestCanceledException _:
          statusCode = (serverException as RequestCanceledException).HttpStatusCode;
          break;
        case RequestFilterException _:
          statusCode = (serverException as RequestFilterException).HttpStatusCode;
          break;
      }
      if (flag)
      {
        if (requestContext != null)
          requestContext.Trace(60001, TraceLevel.Info, nameof (VisualStudioServicesApplication), "WebServices", "Application_Error triggered. Exception: {0}", (object) serverException);
        else
          TeamFoundationTracingService.TraceRaw(60001, TraceLevel.Info, nameof (VisualStudioServicesApplication), "WebServices", "Application_Error triggered. Exception: {0}", (object) serverException);
        if (TeamFoundationApplicationCore.DeploymentServiceHost != null)
          this.ReportError(serverException);
        else if (!(serverException is TeamFoundationServiceException serviceException) || serviceException.LogException)
          TeamFoundationEventLog.Default.LogException((IVssRequestContext) null, FrameworkResources.UnhandledExceptionError(), serverException);
      }
      TeamFoundationApplicationCore.CompleteRequest(application, statusCode, (string) null, (IEnumerable<KeyValuePair<string, string>>) extraHeaders, serverException, (string) null, (string) null);
    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {
      TeamFoundationTracingService.TraceEnterRaw(60010, nameof (VisualStudioServicesApplication), "WebServices", nameof (Application_BeginRequest), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      WebRequestContext requestContext = this.RequestContext;
      this.Context.Items[(object) "OnErrorFormatEvent"] = (object) this.OnFormatError;
      try
      {
        if (requestContext == null || VisualStudioServicesApplication.s_isWebAppRegistered || !requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          return;
        this.RegisterWebApplication();
      }
      catch (ThreadAbortException ex)
      {
        TeamFoundationTracingService.TraceRaw(60030, TraceLevel.Warning, nameof (VisualStudioServicesApplication), "WebServices", "We probably called CompleteRequest, Redirect or Context.Response.End() during Application_BeginRequest.", (object) ex);
        throw;
      }
      catch (UnauthorizedRequestException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(60032, nameof (VisualStudioServicesApplication), "WebServices", (Exception) ex);
        TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) requestContext, HttpApplicationFactory.Current, HttpStatusCode.Unauthorized, (Exception) ex);
      }
      catch (Exception ex)
      {
        if (requestContext != null)
          requestContext.TraceException(60026, nameof (VisualStudioServicesApplication), "WebServices", ex);
        else
          TeamFoundationTracingService.TraceExceptionRaw(60026, nameof (VisualStudioServicesApplication), "WebServices", ex);
        TeamFoundationApplicationCore.CompleteRequest((IVssRequestContext) requestContext, HttpApplicationFactory.Current, HttpStatusCode.InternalServerError, ex);
      }
      finally
      {
        if (requestContext != null)
          requestContext.TraceLeave(60027, nameof (VisualStudioServicesApplication), "WebServices", nameof (Application_BeginRequest));
        TeamFoundationTracingService.TraceLeaveRaw(60031, nameof (VisualStudioServicesApplication), "WebServices", nameof (Application_BeginRequest));
      }
    }

    protected void Application_EndRequest(object sender, EventArgs e)
    {
      TeamFoundationTracingService.TraceEnterRaw(60040, nameof (VisualStudioServicesApplication), "WebServices", nameof (Application_EndRequest), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      WebRequestContext requestContext = this.RequestContext;
      try
      {
        if (requestContext == null)
          return;
        requestContext.Trace(60040, TraceLevel.Info, nameof (VisualStudioServicesApplication), "WebServices", nameof (Application_EndRequest));
      }
      finally
      {
        TeamFoundationTracingService.TraceRaw(60041, TraceLevel.Verbose, nameof (VisualStudioServicesApplication), "WebServices", "Application_EndRequest starting. {0} {1}", (object) HttpContext.Current.Request.RequestType, (object) new UrlTracer(HttpContext.Current.Request.Url));
      }
    }

    protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
    {
    }

    protected void Application_PostAuthorizeRequest(object sender, EventArgs e)
    {
    }

    protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
    {
    }

    private void RegisterWebApplication()
    {
      if (VisualStudioServicesApplication.s_isWebAppRegistered || VisualStudioServicesApplication.s_registrationAttemptsRemaining <= 0 || !(DateTime.UtcNow > VisualStudioServicesApplication.s_nextRegistrationAttemptTime))
        return;
      if (!Monitor.TryEnter(VisualStudioServicesApplication.s_RegisterApplicationLock, 0))
        return;
      try
      {
        if (VisualStudioServicesApplication.s_isWebAppRegistered || VisualStudioServicesApplication.s_registrationAttemptsRemaining <= 0 || !(DateTime.UtcNow > VisualStudioServicesApplication.s_nextRegistrationAttemptTime))
          return;
        --VisualStudioServicesApplication.s_registrationAttemptsRemaining;
        VisualStudioServicesApplication.s_nextRegistrationAttemptTime = DateTime.UtcNow.AddSeconds(60.0);
        if (TeamFoundationApplicationCore.s_deploymentDatabase == null)
          return;
        this.OnFirstRequest();
      }
      catch (Exception ex)
      {
        EventLogEntryType level = VisualStudioServicesApplication.s_registrationAttemptsRemaining > 0 ? EventLogEntryType.Warning : EventLogEntryType.Error;
        TeamFoundationEventLog.Default.LogException(FrameworkResources.ErrorRegisteringWebApplication(), ex, TeamFoundationEventId.RegisterWebApplicationError, level);
      }
      finally
      {
        Monitor.Exit(VisualStudioServicesApplication.s_RegisterApplicationLock);
      }
    }

    protected virtual void OnFirstRequest()
    {
    }

    private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e) => this.ReportError((Exception) e.ExceptionObject);

    public virtual void AddExceptionHeadersToList(
      List<KeyValuePair<string, string>> headers,
      Exception ex)
    {
    }

    [Conditional("DEBUG")]
    private void LaunchDebugger()
    {
      if (Debugger.IsAttached)
        Debugger.Break();
      else
        Debugger.Launch();
    }

    public event ErrorReporterDelegate OnReportError;

    public event ErrorFormatterDelegate OnFormatError;
  }
}
