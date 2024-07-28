// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationApplicationCore
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationApplicationCore
  {
    private static Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost s_deploymentServiceHost;
    internal static ISqlConnectionInfo s_deploymentDatabase;
    private static Guid s_deploymentId;
    internal static string s_deploymentPluginDirectory;
    private const string c_defaultPluginDirectory = "bin\\Plugins";
    private static bool s_log404NotFoundErrors;
    private static readonly char[] s_invalidStatusDescriptionChars = new char[2]
    {
      '\r',
      '\n'
    };
    private static IDisposableReadOnlyList<IHttpApplicationExtension> s_applicationExtensions;
    private static readonly ReaderWriterLock s_startupLock = new ReaderWriterLock();
    private static DateTime s_lastInitializationLoggedAttempt = DateTime.MinValue;
    private static DateTime s_requestExceptionLoggedAttempt = DateTime.MinValue;
    private static readonly TimeSpan s_startupLogFrequency = TimeSpan.FromSeconds(30.0);
    private static IApplicationSettings s_appSettings;
    internal static bool s_sslOnly;
    private static string s_publicUrlHost;
    private static int s_publicUrlPort;
    private static Stopwatch s_publicUrlHostStopwatch;
    private const string c_Area = "TeamFoundationApplicationCore";
    private const string c_Layer = "WebServices";

    public static void ApplicationEnd()
    {
      try
      {
        TeamFoundationApplicationCore.s_startupLock.AcquireWriterLock(-1);
        try
        {
          TeamFoundationApplicationCore.StopApplicationExtensions();
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(60008, nameof (TeamFoundationApplicationCore), "WebServices", ex);
        }
        TeamFoundationApplicationCore.s_deploymentServiceHost.Dispose();
      }
      finally
      {
        TeamFoundationApplicationCore.s_deploymentServiceHost = (Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost) null;
        if (TeamFoundationApplicationCore.s_startupLock.IsWriterLockHeld)
          TeamFoundationApplicationCore.s_startupLock.ReleaseWriterLock();
      }
    }

    public static void ApplicationStart()
    {
      string empty = string.Empty;
      try
      {
        TeamFoundationApplicationCore.s_startupLock.AcquireWriterLock(-1);
        if (TeamFoundationApplicationCore.DeploymentInitialized)
          return;
        TeamFoundationTracingService.TraceRaw(60004, TraceLevel.Verbose, nameof (TeamFoundationApplicationCore), "WebServices", "Enter ApplicationStart()");
        Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost serviceHost = (Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost) null;
        try
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          TeamFoundationEventLog.Default.OnMessageCreated += TeamFoundationApplicationCore.\u003C\u003EO.\u003C0\u003E__EventLog_OnMessageCreated ?? (TeamFoundationApplicationCore.\u003C\u003EO.\u003C0\u003E__EventLog_OnMessageCreated = new EventLogMessageCreatedHandler(TeamFoundationApplicationCore.EventLog_OnMessageCreated));
          string appSetting = WebConfigurationManager.AppSettings[FrameworkServerConstants.ApplicationSettingsReader];
          if (!string.IsNullOrEmpty(appSetting))
            TeamFoundationApplicationCore.AppSettings = (IApplicationSettings) Activator.CreateInstance(Type.GetType(appSetting));
          TeamFoundationApplicationCore.s_sslOnly = string.Equals(TeamFoundationApplicationCore.AppSettings[FrameworkServerConstants.SslOnlyAppSettingsKey], "true", StringComparison.OrdinalIgnoreCase);
          string connectionString = TeamFoundationApplicationCore.AppSettings.ConfigDbConnectionString;
          string configDbUserId = TeamFoundationApplicationCore.AppSettings.ConfigDbUserId;
          string configDbPassword = TeamFoundationApplicationCore.AppSettings.ConfigDbPassword;
          if (string.IsNullOrEmpty(connectionString))
            TeamFoundationApplicationCore.s_deploymentDatabase = (ISqlConnectionInfo) null;
          else if (string.IsNullOrEmpty(configDbPassword))
          {
            TeamFoundationApplicationCore.s_deploymentDatabase = SqlConnectionInfoFactory.Create(connectionString);
          }
          else
          {
            SecureString password = EncryptionUtility.DecryptSecret(configDbPassword);
            TeamFoundationApplicationCore.s_deploymentDatabase = SqlConnectionInfoFactory.Create(connectionString, configDbUserId, password);
          }
          TeamFoundationApplicationCore.CheckForReadOnlyDb();
          TeamFoundationApplicationCore.s_deploymentId = TeamFoundationApplicationCore.AppSettings.InstanceId;
          TeamFoundationApplicationCore.s_deploymentPluginDirectory = TeamFoundationApplicationCore.AppSettings[FrameworkServerConstants.ApplicationPluginDirectory] ?? "bin\\Plugins";
          if (!Path.IsPathRooted(TeamFoundationApplicationCore.s_deploymentPluginDirectory))
            TeamFoundationApplicationCore.s_deploymentPluginDirectory = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, TeamFoundationApplicationCore.s_deploymentPluginDirectory);
          VssExtensionManagementService.DefaultPluginPath = TeamFoundationApplicationCore.s_deploymentPluginDirectory;
          TeamFoundationServiceHostProperties serviceHostProperties1 = new TeamFoundationServiceHostProperties();
          serviceHostProperties1.HostType = TeamFoundationHostType.Application | TeamFoundationHostType.Deployment;
          serviceHostProperties1.Id = TeamFoundationApplicationCore.s_deploymentId;
          serviceHostProperties1.PlugInDirectory = TeamFoundationApplicationCore.s_deploymentPluginDirectory;
          serviceHostProperties1.PhysicalDirectory = HostingEnvironment.ApplicationPhysicalPath;
          TeamFoundationServiceHostProperties serviceHostProperties2 = serviceHostProperties1;
          if (TeamFoundationApplicationCore.s_deploymentDatabase == null)
            serviceHostProperties2.Status = TeamFoundationServiceHostStatus.Started;
          serviceHost = new Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost((HostProperties) serviceHostProperties2, TeamFoundationApplicationCore.s_deploymentDatabase, new DeploymentServiceHostOptions(HostProcessType.ApplicationTier));
          if (TeamFoundationApplicationCore.s_deploymentDatabase != null)
          {
            using (IVssRequestContext systemContext = serviceHost.CreateSystemContext(false))
            {
              new TeamFoundationTraceConfiguration().Initialize(systemContext);
              int deploymentType = (int) systemContext.GetService<TeamFoundationHostManagementService>().DeploymentType;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              systemContext.GetService<TeamFoundationSqlNotificationService>().RegisterNotification(systemContext, "Default", SqlNotificationEventClasses.Recycle, TeamFoundationApplicationCore.\u003C\u003EO.\u003C1\u003E__RecycleCallback ?? (TeamFoundationApplicationCore.\u003C\u003EO.\u003C1\u003E__RecycleCallback = new SqlNotificationCallback(TeamFoundationApplicationCore.RecycleCallback)), false);
            }
            using (IVssRequestContext servicingContext = serviceHost.CreateServicingContext())
            {
              SqlRegistryService service = servicingContext.GetService<SqlRegistryService>();
              try
              {
                TeamFoundationApplicationCore.s_log404NotFoundErrors = service.GetValue<bool>(servicingContext, (RegistryQuery) FrameworkServerConstants.Log404NotFoundErrors, false);
              }
              catch (Exception ex)
              {
                servicingContext.TraceException(60003, nameof (TeamFoundationApplicationCore), "WebServices", ex);
              }
            }
          }
          serviceHost.ServiceHostInternal().RequestFilters = VssExtensionManagementService.GetExtensionsRaw<ITeamFoundationRequestFilter>(TeamFoundationApplicationCore.s_deploymentPluginDirectory);
          using (IVssRequestContext systemContext = serviceHost.CreateSystemContext(true))
          {
            WebApiConfiguration.Initialize(systemContext);
            CachedRegistryService service = systemContext.GetService<CachedRegistryService>();
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            service.RegisterNotification(systemContext, TeamFoundationApplicationCore.\u003C\u003EO.\u003C2\u003E__OnRegistryChanged ?? (TeamFoundationApplicationCore.\u003C\u003EO.\u003C2\u003E__OnRegistryChanged = new RegistrySettingsChangedCallback(TeamFoundationApplicationCore.OnRegistryChanged)), FrameworkServerConstants.UseStrictTransportSecurity);
            TeamFoundationApplicationCore.UseStrictTransportSecurity = service.GetValue<bool>(systemContext, (RegistryQuery) FrameworkServerConstants.UseStrictTransportSecurity, false);
            systemContext.Trace(60005, TraceLevel.Info, nameof (TeamFoundationApplicationCore), "WebServices", "Loading plugins for IHttpApplicationExtension in {0}", (object) serviceHost.PlugInDirectory);
            TeamFoundationApplicationCore.s_applicationExtensions = systemContext.GetExtensions<IHttpApplicationExtension>();
            if (TeamFoundationApplicationCore.s_applicationExtensions != null && TeamFoundationApplicationCore.s_applicationExtensions.Count > 0)
            {
              foreach (IHttpApplicationExtension applicationExtension in (IEnumerable<IHttpApplicationExtension>) TeamFoundationApplicationCore.s_applicationExtensions)
              {
                systemContext.Trace(60006, TraceLevel.Info, nameof (TeamFoundationApplicationCore), "WebServices", "Initializing extension {0}", (object) applicationExtension);
                applicationExtension.Start(systemContext);
              }
            }
            if (!(HostingEnvironment.VirtualPathProvider is TeamFoundationVirtualPathProvider))
              HostingEnvironment.RegisterVirtualPathProvider((VirtualPathProvider) new TeamFoundationVirtualPathProvider(systemContext));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          serviceHost.StatusChanged += TeamFoundationApplicationCore.\u003C\u003EO.\u003C3\u003E__OnDeploymentHostStop ?? (TeamFoundationApplicationCore.\u003C\u003EO.\u003C3\u003E__OnDeploymentHostStop = new EventHandler<HostStatusChangedEventArgs>(TeamFoundationApplicationCore.OnDeploymentHostStop));
          TeamFoundationApplicationCore.s_deploymentServiceHost = serviceHost;
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(60465, nameof (TeamFoundationApplicationCore), "WebServices", ex);
          empty += TeamFoundationExceptionFormatter.FormatException(ex, false);
          if (ex is TeamFoundationServiceException serviceException)
            serviceException.LogException = false;
          if (serviceHost != null && TeamFoundationApplicationCore.s_deploymentServiceHost != serviceHost)
            serviceHost.Dispose();
          throw;
        }
        finally
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Server Version = {0}", (object) Assembly.GetExecutingAssembly().FullName));
          stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Service Account = {0}", (object) UserNameUtil.CurrentUserName));
          stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Machine Name = {0}", (object) Environment.MachineName));
          string message = FrameworkResources.ApplicationRequestStartMessage((object) stringBuilder.ToString(), (object) empty);
          if (DateTime.UtcNow.Subtract(TeamFoundationApplicationCore.s_lastInitializationLoggedAttempt) > TeamFoundationApplicationCore.s_startupLogFrequency)
          {
            TeamFoundationEventLog.Default.Log(message, TeamFoundationEventId.ApplicationStarted, TeamFoundationApplicationCore.DeploymentInitialized ? EventLogEntryType.Information : EventLogEntryType.Error);
            TeamFoundationApplicationCore.s_lastInitializationLoggedAttempt = DateTime.UtcNow;
          }
        }
        TeamFoundationTracingService.TraceRaw(60009, TraceLevel.Verbose, nameof (TeamFoundationApplicationCore), "WebServices", "Exit ApplicationStart()");
      }
      finally
      {
        if (TeamFoundationApplicationCore.s_startupLock.IsWriterLockHeld)
          TeamFoundationApplicationCore.s_startupLock.ReleaseWriterLock();
      }
    }

    private static void EventLog_OnMessageCreated(object sender, StringBuilder sb)
    {
      if (HttpContext.Current?.Request == null)
        return;
      sb.AppendLine();
      sb.AppendLine();
      HttpRequest request = HttpContext.Current.Request;
      IIdentity identity = (IIdentity) null;
      try
      {
        if (request.IsAuthenticated)
          identity = (IIdentity) request.LogonUserIdentity;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
      string str1;
      try
      {
        str1 = request.Url?.AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        str1 = FrameworkResources.NotAvailable();
      }
      string str2;
      try
      {
        str2 = request.RequestType;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        str2 = FrameworkResources.NotAvailable();
      }
      string str3;
      try
      {
        str3 = request.UserAgent;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        str3 = FrameworkResources.NotAvailable();
      }
      string str4;
      try
      {
        str4 = request.Path;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        str4 = FrameworkResources.NotAvailable();
      }
      string str5;
      try
      {
        str5 = request.IsLocal.ToString();
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        str5 = FrameworkResources.NotAvailable();
      }
      string str6;
      try
      {
        str6 = request.UserHostAddress;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        str6 = FrameworkResources.NotAvailable();
      }
      sb.AppendLine(FrameworkResources.WebRequestDetails((object) str1, (object) str2, (object) str3, (object) FrameworkResources.NotAvailable(), (object) str4, (object) str5, (object) str6, (object) IdentityDisplayName(identity), (object) AuthenticationType(identity)));

      static string IdentityDisplayName(IIdentity identity)
      {
        if (identity == null)
          return FrameworkResources.NotAvailable();
        try
        {
          return identity.Name;
        }
        catch
        {
          return FrameworkResources.NotAvailable();
        }
      }

      static string AuthenticationType(IIdentity identity)
      {
        if (identity == null)
          return FrameworkResources.NotAvailable();
        try
        {
          return identity.AuthenticationType;
        }
        catch
        {
          return FrameworkResources.NotAvailable();
        }
      }
    }

    private static void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceEnter(60476, nameof (TeamFoundationApplicationCore), "WebServices", nameof (OnRegistryChanged));
      try
      {
        TeamFoundationApplicationCore.UseStrictTransportSecurity = changedEntries.GetValueFromPath<bool>(FrameworkServerConstants.UseStrictTransportSecurity, false);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(60477, nameof (TeamFoundationApplicationCore), "WebServices", ex);
      }
      finally
      {
        requestContext.TraceLeave(60476, nameof (TeamFoundationApplicationCore), "WebServices", nameof (OnRegistryChanged));
      }
    }

    private static void OnDeploymentHostStop(object sender, HostStatusChangedEventArgs eventArgs)
    {
      if (eventArgs.Status == TeamFoundationServiceHostStatus.Stopping)
      {
        TeamFoundationApplicationCore.StopApplicationExtensions();
      }
      else
      {
        if (eventArgs.Status != TeamFoundationServiceHostStatus.Stopped)
          return;
        ThreadPool.QueueUserWorkItem((WaitCallback) (state =>
        {
          try
          {
            TeamFoundationApplicationCore.s_startupLock.AcquireWriterLock(-1);
            if (TeamFoundationApplicationCore.s_deploymentServiceHost == null)
              return;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            TeamFoundationApplicationCore.s_deploymentServiceHost.StatusChanged -= TeamFoundationApplicationCore.\u003C\u003EO.\u003C3\u003E__OnDeploymentHostStop ?? (TeamFoundationApplicationCore.\u003C\u003EO.\u003C3\u003E__OnDeploymentHostStop = new EventHandler<HostStatusChangedEventArgs>(TeamFoundationApplicationCore.OnDeploymentHostStop));
            TeamFoundationApplicationCore.s_deploymentServiceHost.Dispose();
            TeamFoundationApplicationCore.s_deploymentServiceHost = (Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHost) null;
          }
          finally
          {
            TeamFoundationApplicationCore.s_startupLock.ReleaseWriterLock();
          }
        }));
      }
    }

    private static void StopApplicationExtensions()
    {
      if (TeamFoundationApplicationCore.s_applicationExtensions == null)
        return;
      try
      {
        using (IVssRequestContext systemContext = TeamFoundationApplicationCore.s_deploymentServiceHost.CreateSystemContext(false))
        {
          foreach (IHttpApplicationExtension applicationExtension in (IEnumerable<IHttpApplicationExtension>) TeamFoundationApplicationCore.s_applicationExtensions)
          {
            try
            {
              applicationExtension.Stop(systemContext);
            }
            catch (Exception ex)
            {
              systemContext.TraceException(60007, nameof (TeamFoundationApplicationCore), "WebServices", ex);
            }
          }
        }
      }
      finally
      {
        TeamFoundationApplicationCore.s_applicationExtensions.Dispose();
        TeamFoundationApplicationCore.s_applicationExtensions = (IDisposableReadOnlyList<IHttpApplicationExtension>) null;
      }
    }

    internal static void FormatError(
      HttpContextBase context,
      IHttpApplication application,
      HttpStatusCode statusCode,
      string statusDescription,
      IEnumerable<KeyValuePair<string, string>> extraHeaders,
      Exception exception,
      string errorMessage,
      string responseText,
      ErrorFormatterDelegate errorFormatter)
    {
      if (!context.Response.BufferOutput)
        return;
      context.Response.Clear();
      TeamFoundationTracingService.TraceRaw(60361, TraceLevel.Warning, nameof (TeamFoundationApplicationCore), "WebServices", "Setting error message");
      List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
      if (exception != null)
      {
        if (string.IsNullOrEmpty(errorMessage))
          errorMessage = UserFriendlyError.GetMessageFromException(exception);
        if (statusCode == (HttpStatusCode) 0 && exception is HttpException)
          statusCode = (HttpStatusCode) ((HttpException) exception).GetHttpCode();
        application.AddExceptionHeadersToList(headers, exception);
      }
      errorMessage = !string.IsNullOrEmpty(errorMessage) ? SecretUtility.ScrubSecrets(errorMessage) : FrameworkResources.TeamFoundationUnavilable();
      responseText = SecretUtility.ScrubSecrets(responseText);
      TeamFoundationTracingService.TraceRaw(60364, TraceLevel.Warning, nameof (TeamFoundationApplicationCore), "WebServices", "Setting error message headers with {0} extra headers", (object) (extraHeaders != null ? extraHeaders.Count<KeyValuePair<string, string>>() : 0));
      string str = errorMessage;
      if (str.IndexOfAny(TeamFoundationApplicationCore.s_invalidStatusDescriptionChars) != -1)
      {
        foreach (char statusDescriptionChar in TeamFoundationApplicationCore.s_invalidStatusDescriptionChars)
          str = str.Replace(statusDescriptionChar, ' ');
      }
      if (!string.IsNullOrEmpty(str))
        headers.Add(new KeyValuePair<string, string>("X-TFS-ServiceError", HttpUtility.UrlEncode(str, (Encoding) new UTF8Encoding(false))));
      if (statusCode == (HttpStatusCode) 0)
        statusCode = HttpStatusCode.InternalServerError;
      try
      {
        context.Response.StatusCode = (int) statusCode;
        context.Response.TrySkipIisCustomErrors = true;
        if (!string.IsNullOrEmpty(statusDescription))
          context.Response.StatusDescription = statusDescription.Substring(0, Math.Min(statusDescription.Length, 512));
        if (extraHeaders != null)
          headers.AddRange(extraHeaders);
        foreach (KeyValuePair<string, string> keyValuePair in headers)
          context.Response.AddHeader(keyValuePair.Key, keyValuePair.Value);
      }
      catch (HttpException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(60360, TraceLevel.Info, nameof (TeamFoundationApplicationCore), "WebServices", (Exception) ex);
      }
      bool flag = false;
      if (errorFormatter != null)
      {
        TeamFoundationTracingService.TraceRaw(60365, TraceLevel.Warning, nameof (TeamFoundationApplicationCore), "WebServices", "Handling error formatter");
        try
        {
          foreach (ErrorFormatterDelegate invocation in errorFormatter.GetInvocationList())
          {
            TeamFoundationTracingService.TraceRaw(60366, TraceLevel.Warning, nameof (TeamFoundationApplicationCore), "WebServices", "Calling handler {0}", (object) invocation.Method.Name);
            if (invocation(context, statusCode, exception, errorMessage, responseText))
            {
              flag = true;
              break;
            }
          }
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceRaw(60080, TraceLevel.Warning, nameof (TeamFoundationApplicationCore), "WebServices", "Error formatter caused and exception. code {0}; message {1}; description: {2}, formatterError: {3}", (object) statusCode, (object) errorMessage, (object) responseText, (object) ex);
        }
      }
      if (!flag)
      {
        try
        {
          if (context.Response.ContentType.IndexOf("application/json", StringComparison.OrdinalIgnoreCase) < 0)
          {
            if (context.Response.StatusCode != 404)
            {
              if (MediaTypeFormatUtility.DoesRequestAcceptMediaType(context.Request.AcceptTypes, "application/json", "text/plain"))
              {
                if (TeamFoundationApplicationCore.IsJsonAcceptHeaderBehaviourDisabled(context))
                  goto label_43;
              }
              else
                goto label_43;
            }
            else
              goto label_43;
          }
          TeamFoundationApplicationCore.FormatErrorAsJson(context, exception, errorMessage, responseText);
          flag = true;
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceRaw(60081, TraceLevel.Warning, nameof (TeamFoundationApplicationCore), "WebServices", "Failure occurred wrapping exception. code {0}; message {1}; description: {2}, Exception: {3}", (object) statusCode, (object) errorMessage, (object) responseText, (object) ex);
        }
      }
label_43:
      if (flag)
        return;
      TeamFoundationApplicationCore.FormatErrorAsTextPlain(context, errorMessage, responseText);
    }

    private static void FormatErrorAsJson(
      HttpContextBase context,
      Exception exception,
      string errorMessage,
      string responseText)
    {
      if (context.Response.ContentType != "application/json")
      {
        try
        {
          context.Response.ContentType = "application/json";
        }
        catch (HttpException ex)
        {
        }
      }
      bool includeErrorDetail = false;
      Version apiVersion = context.Request.GetApiVersion();
      if (exception == null)
        exception = (Exception) new VssServiceException(responseText ?? errorMessage);
      WrappedException wrappedException = new WrappedException(exception, includeErrorDetail, apiVersion);
      if (!string.IsNullOrWhiteSpace(responseText))
        wrappedException.Message = responseText;
      else if (!string.IsNullOrWhiteSpace(errorMessage))
        wrappedException.Message = errorMessage;
      new VssJsonMediaTypeFormatter().WriteToStream(typeof (WrappedException), (object) wrappedException, context.Response.OutputStream, Encoding.UTF8);
    }

    private static bool IsJsonAcceptHeaderBehaviourDisabled(HttpContextBase httpContext)
    {
      Version version = (Version) null;
      try
      {
        version = httpContext.Request.GetApiVersion();
      }
      catch (VssInvalidApiResourceVersionException ex)
      {
      }
      return version != (Version) null && version < new Version(5, 0);
    }

    private static void FormatErrorAsTextPlain(
      HttpContextBase context,
      string errorMessage,
      string responseText)
    {
      if (context.Response.ContentType != "text/plain")
      {
        try
        {
          context.Response.ContentType = "text/plain";
        }
        catch (HttpException ex)
        {
        }
      }
      TeamFoundationTracingService.TraceRaw(60367, TraceLevel.Warning, nameof (TeamFoundationApplicationCore), "WebServices", "Writing response text");
      if (!string.IsNullOrWhiteSpace(responseText))
      {
        context.Response.Write(responseText);
      }
      else
      {
        if (string.IsNullOrWhiteSpace(errorMessage))
          return;
        context.Response.Write(errorMessage);
      }
    }

    public static void CompleteRequest(
      IVssRequestContext requestContext,
      IHttpApplication application,
      HttpStatusCode statusCode,
      Exception exception)
    {
      TeamFoundationTracingService.TraceExceptionRaw(60350, TraceLevel.Warning, nameof (TeamFoundationApplicationCore), "WebServices", exception);
      if (!TeamFoundationApplicationCore.DeploymentInitialized && DateTime.UtcNow.Subtract(TeamFoundationApplicationCore.s_requestExceptionLoggedAttempt) > TeamFoundationApplicationCore.s_startupLogFrequency)
      {
        TeamFoundationApplicationCore.s_requestExceptionLoggedAttempt = DateTime.UtcNow;
        TeamFoundationEventLog.Default.LogException(requestContext, FrameworkResources.ServerFatalInitError(), exception);
      }
      if (requestContext != null && requestContext.Status == null)
        requestContext.Status = exception;
      TeamFoundationApplicationCore.CompleteRequest(application, statusCode, (string) null, (IEnumerable<KeyValuePair<string, string>>) null, exception, (string) null, (string) null);
    }

    public static void CompleteRequest(
      IHttpApplication application,
      HttpStatusCode statusCode,
      string statusDescription,
      IEnumerable<KeyValuePair<string, string>> extraHeaders,
      Exception exception,
      string errorMessage,
      string responseText)
    {
      HttpContextBase context = application.Context;
      if (statusCode == HttpStatusCode.Unauthorized && TeamFoundationApplicationCore.s_sslOnly && !context.Request.IsSecureConnection)
      {
        TeamFoundationTracingService.TraceRaw(60376, TraceLevel.Warning, nameof (TeamFoundationApplicationCore), "WebServices", "Applying SSL restriction");
        TeamFoundationApplicationCore.ApplySslRestriction(application);
      }
      else
      {
        if (statusCode == HttpStatusCode.Forbidden && context.Request.Headers["X-TFS-FedAuthRedirect"] != "Suppress" && !string.IsNullOrEmpty(context.Response.Headers["X-TFS-FedAuthRedirect"]))
        {
          TeamFoundationTracingService.TraceRaw(60377, TraceLevel.Warning, nameof (TeamFoundationApplicationCore), "WebServices", "Redirecting Response");
          context.Response.Redirect(context.Response.Headers["X-TFS-FedAuthRedirect"]);
        }
        ErrorFormatterDelegate errorFormatter = (ErrorFormatterDelegate) null;
        if (context.Items.Contains((object) "OnErrorFormatEvent"))
          errorFormatter = context.Items[(object) "OnErrorFormatEvent"] as ErrorFormatterDelegate;
        TeamFoundationTracingService.TraceRaw(60378, TraceLevel.Warning, nameof (TeamFoundationApplicationCore), "WebServices", "Formatting error");
        TeamFoundationApplicationCore.FormatError(context, application, statusCode, statusDescription, extraHeaders, exception, errorMessage, responseText, errorFormatter);
        TeamFoundationTracingService.TraceRaw(60379, TraceLevel.Warning, nameof (TeamFoundationApplicationCore), "WebServices", "Request not processed -- code {0}; message {1}; description: {2}", (object) statusCode, (object) errorMessage, (object) responseText);
        if (context.Items[(object) HttpContextConstants.IVssRequestContext] is IVssRequestContext requestContext && context.Response.StatusCode == 401)
        {
          requestContext.TraceConditionally(60033, TraceLevel.Info, nameof (TeamFoundationApplicationCore), "WebServices", (Func<string>) (() => "Completing response with status 401 --- " + EnvironmentWrapper.ToReadableStackTrace()));
          if (exception != null)
            requestContext.TraceException(60034, TraceLevel.Info, nameof (TeamFoundationApplicationCore), "WebServices", exception);
        }
        application.CompleteRequest();
      }
    }

    public static void ApplySslRestriction(IHttpApplication application)
    {
      HttpContextBase context = application.Context;
      if (context.Request.Path.EndsWith(".asmx", StringComparison.OrdinalIgnoreCase) || context.Request.Path.EndsWith(".ashx", StringComparison.OrdinalIgnoreCase))
      {
        TeamFoundationTracingService.TraceRaw(60370, TraceLevel.Warning, nameof (TeamFoundationApplicationCore), "WebServices", "Resource {0} cannot be found", (object) context.Request.Path);
        TeamFoundationApplicationCore.CompleteRequest(application, HttpStatusCode.NotFound, (string) null, (IEnumerable<KeyValuePair<string, string>>) null, (Exception) null, FrameworkResources.ResourceCannotBeFound(), (string) null);
      }
      else
      {
        UriBuilder uriBuilder = new UriBuilder(application.Request.Url.AbsoluteUri)
        {
          Scheme = Uri.UriSchemeHttps
        };
        string publicUrlHost;
        int publicUrlPort;
        if (TeamFoundationApplicationCore.GetHttpsPublicUrlHostAndPort(out publicUrlHost, out publicUrlPort))
        {
          if (!TeamFoundationApplicationCore.s_deploymentServiceHost.IsHosted)
            uriBuilder.Host = publicUrlHost;
          uriBuilder.Port = publicUrlPort;
        }
        else
          TeamFoundationApplicationCore.CompleteRequest(application, HttpStatusCode.NotFound, (string) null, (IEnumerable<KeyValuePair<string, string>>) null, (Exception) null, FrameworkResources.ResourceCannotBeFound(), (string) null);
        context.Response.Redirect(uriBuilder.Uri.AbsoluteUri);
      }
    }

    private static bool GetHttpsPublicUrlHostAndPort(
      out string publicUrlHost,
      out int publicUrlPort)
    {
      if (TeamFoundationApplicationCore.s_publicUrlHost == null || TeamFoundationApplicationCore.s_publicUrlHostStopwatch == null || TeamFoundationApplicationCore.s_publicUrlHostStopwatch.ElapsedMilliseconds > 15000L)
      {
        using (IVssRequestContext systemContext = TeamFoundationApplicationCore.s_deploymentServiceHost.CreateSystemContext(true))
        {
          string locationServiceUrl = systemContext.GetService<ILocationService>().GetLocationServiceUrl(systemContext, Microsoft.VisualStudio.Services.Location.LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.PublicAccessMappingMoniker);
          try
          {
            UriBuilder uriBuilder = new UriBuilder(locationServiceUrl);
            if (uriBuilder.Scheme == Uri.UriSchemeHttps)
            {
              TeamFoundationApplicationCore.s_publicUrlPort = uriBuilder.Port;
              TeamFoundationApplicationCore.s_publicUrlHost = uriBuilder.Host;
              if (TeamFoundationApplicationCore.s_publicUrlHostStopwatch == null)
                TeamFoundationApplicationCore.s_publicUrlHostStopwatch = Stopwatch.StartNew();
              else
                TeamFoundationApplicationCore.s_publicUrlHostStopwatch.Restart();
            }
            else
            {
              TeamFoundationApplicationCore.s_publicUrlPort = 0;
              TeamFoundationApplicationCore.s_publicUrlHost = (string) null;
              systemContext.Trace(60011, TraceLevel.Error, nameof (TeamFoundationApplicationCore), "WebServices", "Since public Url is not setup to use https, http request cannot be redirected.");
            }
          }
          catch (Exception ex)
          {
            systemContext.Trace(60012, TraceLevel.Error, nameof (TeamFoundationApplicationCore), "WebServices", "An error occurred while parsing the following public Url: '{0}'. The following error reported: '{1}'", (object) locationServiceUrl, (object) ex.Message);
            systemContext.TraceCatch(60013, nameof (TeamFoundationApplicationCore), "WebServices", ex);
            TeamFoundationApplicationCore.s_publicUrlHost = (string) null;
            TeamFoundationApplicationCore.s_publicUrlPort = 0;
          }
        }
      }
      publicUrlPort = TeamFoundationApplicationCore.s_publicUrlPort;
      publicUrlHost = TeamFoundationApplicationCore.s_publicUrlHost;
      return TeamFoundationApplicationCore.s_publicUrlHost != null;
    }

    private static void RecycleCallback(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      RecycleRole recycleRole = RecycleRole.All;
      Guid result = Guid.Empty;
      if (eventData != null)
        recycleRole = Guid.TryParse(eventData, out result) ? RecycleRole.None : (RecycleRole) System.Enum.Parse(typeof (RecycleRole), eventData);
      if (!recycleRole.HasFlag((System.Enum) RecycleRole.AT) && (TeamFoundationApplicationCore.s_deploymentServiceHost == null || TeamFoundationApplicationCore.s_deploymentServiceHost.DeploymentServiceHostInternal().HostManagement == null || !(result != Guid.Empty) || !(result == TeamFoundationApplicationCore.s_deploymentServiceHost.DeploymentServiceHostInternal().HostManagement.ProcessId)))
        return;
      HostingEnvironment.InitiateShutdown();
    }

    internal static void ApplyLicensePrincipals(IVssRequestContext requestContext)
    {
      if (!StakeholderLicensingHelper.EnableStakeholderLicenseCheck(requestContext))
        return;
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      IStakeholderLicenseAdapter extension = requestContext1.GetExtension<IStakeholderLicenseAdapter>(ExtensionLifetime.Service, throwOnError: true);
      IReadOnlyList<IRequestActor> actors = requestContext.RequestContextInternal().Actors;
      if (actors == null || actors.Count <= 0)
        return;
      foreach (IRequestActor actor in (IEnumerable<IRequestActor>) requestContext.RequestContextInternal().Actors)
      {
        if (actor != null)
        {
          bool? nullable = (bool?) extension?.HasStakeholderRights(requestContext, actor.Descriptor);
          if (nullable.HasValue && nullable.Value)
          {
            Guid license = requestContext1.ExecutionEnvironment.IsHostedDeployment ? HostedLicenseName.Stakeholder : OnPremLicenseName.Limited;
            if (!actor.TryAppendPrincipal(SubjectType.License, TeamFoundationApplicationCore.CreateEvaluationPrincipal(requestContext, license)))
              requestContext.Trace(60491, TraceLevel.Warning, nameof (TeamFoundationApplicationCore), "WebServices", string.Format("Attempting to add second principal failed for the subject type {0} to actor identifier {1} with descriptor {2}", (object) SubjectType.License, (object) actor.Identifier, (object) actor.Descriptor));
          }
        }
      }
    }

    private static EvaluationPrincipal CreateEvaluationPrincipal(
      IVssRequestContext requestContext,
      Guid license)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      IVssSecuritySubjectService service = vssRequestContext.GetService<IVssSecuritySubjectService>();
      SecuritySubjectEntry securitySubjectEntry1 = service.GetSecuritySubjectEntry(vssRequestContext, license);
      if (securitySubjectEntry1 == null || securitySubjectEntry1.SubjectType != SecuritySubjectType.License)
        throw new InvalidConfigurationException("Could not determine Stakeholder Subject.");
      List<IdentityDescriptor> roleDescriptors = (List<IdentityDescriptor>) null;
      if (license.Equals(HostedLicenseName.Stakeholder) && StakeholderLicensingHelper.IsBuildAndReleaseEnabledForStakeholders(requestContext))
      {
        SecuritySubjectEntry securitySubjectEntry2 = service.GetSecuritySubjectEntry(vssRequestContext, HostedLicenseName.Limited);
        if (securitySubjectEntry2 == null || securitySubjectEntry2.SubjectType != SecuritySubjectType.License)
          requestContext.Trace(60014, TraceLevel.Error, nameof (TeamFoundationApplicationCore), "WebServices", "Could not find Limited Subject.");
        else
          roleDescriptors = new List<IdentityDescriptor>()
          {
            securitySubjectEntry1.ToDescriptor(),
            securitySubjectEntry2.ToDescriptor()
          };
      }
      return new EvaluationPrincipal(securitySubjectEntry1.ToDescriptor(), (IEnumerable<IdentityDescriptor>) roleDescriptors);
    }

    internal static void ReplaceStakeholderPrincipal(IVssRequestContext requestContext)
    {
      IReadOnlyList<IRequestActor> actors = requestContext.RequestContextInternal().Actors;
      if (actors == null || actors.Count <= 0)
        return;
      foreach (IRequestActor actor in (IEnumerable<IRequestActor>) requestContext.RequestContextInternal().Actors)
      {
        if (actor != null)
        {
          EvaluationPrincipal evaluationPrincipal = TeamFoundationApplicationCore.CreateEvaluationPrincipal(requestContext, HostedLicenseName.Stakeholder);
          actor.TryReplacePrincipal(SubjectType.License, evaluationPrincipal);
        }
      }
    }

    private static void CheckForReadOnlyDb()
    {
      if (TeamFoundationApplicationCore.s_deploymentDatabase == null)
        return;
      using (ExtendedAttributeComponent componentRaw = TeamFoundationApplicationCore.s_deploymentDatabase.CreateComponentRaw<ExtendedAttributeComponent>())
      {
        if (componentRaw.ReadDeploymentTypeStamp() != DeploymentType.DevFabric)
          return;
      }
      using (ReadWriteCheckComponent componentRaw = TeamFoundationApplicationCore.s_deploymentDatabase.CreateComponentRaw<ReadWriteCheckComponent>())
      {
        if (componentRaw.IsReadOnly())
          throw new DatabaseReadOnlyException((SqlException) null);
      }
    }

    public static void SetPreferredCulture(
      HttpContextBase context,
      IVssRequestContext requestContext,
      bool forceHeaderCulture = false)
    {
      TeamFoundationApplicationCore.SetPreferredCulture(context.Request.UserLanguages, requestContext, forceHeaderCulture);
    }

    internal static void SetPreferredCulture(
      string[] userLanguages,
      IVssRequestContext requestContext,
      bool forceHeaderCulture = false)
    {
      CultureInfo cultureInfo1 = (CultureInfo) null;
      CultureInfo cultureInfo2 = (CultureInfo) null;
      if (requestContext?.UserContext != (IdentityDescriptor) null && !forceHeaderCulture)
      {
        UserPreferences userPreferences = requestContext.GetService<IUserPreferencesService>().GetUserPreferences(requestContext);
        cultureInfo2 = userPreferences?.Language;
        cultureInfo1 = userPreferences?.Culture;
      }
      if (cultureInfo2 == null || cultureInfo1 == null)
      {
        CultureInfo preferredCulture1;
        CultureInfo preferredCulture2 = TeamFoundationApplicationCore.GetPreferredCulture(userLanguages, (ISet<int>) null, (ISet<int>) null, out preferredCulture1);
        cultureInfo2 = cultureInfo2 ?? preferredCulture2;
        cultureInfo1 = cultureInfo1 ?? preferredCulture1;
      }
      if (cultureInfo2 != null)
        Thread.CurrentThread.CurrentUICulture = cultureInfo2;
      if (cultureInfo1 == null)
        return;
      Thread.CurrentThread.CurrentCulture = cultureInfo1;
    }

    public static void SetCultures(HttpContextBase context, IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      bool premisesDeployment = requestContext.ExecutionEnvironment.IsOnPremisesDeployment;
      CultureInfo cultureInfo = (CultureInfo) null;
      try
      {
        if (!premisesDeployment)
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          ISet<int> installedLanguages = vssRequestContext.GetService<IInstalledLanguageService>().GetInstalledLanguages(vssRequestContext);
          cultureInfo = TeamFoundationApplicationCore.GetPreferredCulture(context.Request.UserLanguages, installedLanguages, (ISet<int>) null, out CultureInfo _);
        }
        if (cultureInfo == null)
          cultureInfo = requestContext.ServiceHost.GetCulture(requestContext);
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(60485, nameof (TeamFoundationApplicationCore), "WebServices", ex);
      }
    }

    private static ISet<string> GetInstalledLanguageNames(
      ISet<int> installedLanguages,
      ISet<int> userInstalledLanguages)
    {
      HashSet<string> installedLanguageNames = (HashSet<string>) null;
      if (installedLanguages != null || userInstalledLanguages != null)
      {
        HashSet<int> source = new HashSet<int>();
        if (installedLanguages != null)
          source.UnionWith((IEnumerable<int>) installedLanguages);
        if (userInstalledLanguages != null)
          source.UnionWith((IEnumerable<int>) userInstalledLanguages);
        installedLanguageNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        using (IEnumerator<CultureInfo> enumerator = source.Select<int, CultureInfo>((Func<int, CultureInfo>) (lcid => CultureInfo.GetCultureInfo(lcid))).GetEnumerator())
        {
label_10:
          while (enumerator.MoveNext())
          {
            CultureInfo cultureInfo = enumerator.Current;
            while (true)
            {
              if (cultureInfo != CultureInfo.InvariantCulture && !string.Equals(cultureInfo.Name, "zh"))
              {
                installedLanguageNames.Add(cultureInfo.Name);
                cultureInfo = cultureInfo.Parent;
              }
              else
                goto label_10;
            }
          }
        }
      }
      return (ISet<string>) installedLanguageNames;
    }

    private static IEnumerable<string> GetSortedUserLanguages(IEnumerable<string> userLanguages) => userLanguages.Select<string, KeyValuePair<string, double>>((Func<string, KeyValuePair<string, double>>) (lang =>
    {
      string[] strArray = lang.Split(';', '=');
      double result = 1.0;
      if (strArray.Length > 2 && !double.TryParse(strArray[2], NumberStyles.Any, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
        result = 0.0;
      return new KeyValuePair<string, double>(strArray[0], result);
    })).OrderByDescending<KeyValuePair<string, double>, double>((Func<KeyValuePair<string, double>, double>) (pair => pair.Value)).Select<KeyValuePair<string, double>, string>((Func<KeyValuePair<string, double>, string>) (pair => pair.Key));

    public static CultureInfo GetPreferredCulture(
      string[] userLanguages,
      ISet<int> installedLanguages,
      ISet<int> userInstalledLanguages,
      out CultureInfo preferredCulture)
    {
      preferredCulture = (CultureInfo) null;
      CultureInfo preferredCulture1 = (CultureInfo) null;
      ISet<string> stringSet = (ISet<string>) null;
      if (userLanguages != null)
      {
        stringSet = TeamFoundationApplicationCore.GetInstalledLanguageNames(installedLanguages, userInstalledLanguages);
        foreach (string sortedUserLanguage in TeamFoundationApplicationCore.GetSortedUserLanguages((IEnumerable<string>) userLanguages))
        {
          CultureInfo cultureInfo;
          try
          {
            cultureInfo = CultureInfo.GetCultureInfo(sortedUserLanguage);
          }
          catch (CultureNotFoundException ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(60100, TraceLevel.Warning, nameof (TeamFoundationApplicationCore), "WebServices", (Exception) ex);
            continue;
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(60101, nameof (TeamFoundationApplicationCore), "WebServices", ex);
            continue;
          }
          if (preferredCulture == null)
            preferredCulture = cultureInfo;
          if (stringSet != null)
          {
            if (stringSet.Contains(sortedUserLanguage))
            {
              preferredCulture1 = cultureInfo;
              break;
            }
          }
          else
            break;
        }
      }
      if (preferredCulture1 == null && stringSet == null)
        preferredCulture1 = preferredCulture;
      if (preferredCulture != null && preferredCulture.IsNeutralCulture)
        preferredCulture = CultureInfo.CreateSpecificCulture(preferredCulture.Name);
      if (preferredCulture1 != null && preferredCulture1.IsNeutralCulture)
        preferredCulture1 = CultureInfo.CreateSpecificCulture(preferredCulture1.Name);
      return preferredCulture1;
    }

    public static bool DeploymentInitialized => TeamFoundationApplicationCore.s_deploymentServiceHost != null;

    public static IVssDeploymentServiceHost DeploymentServiceHost => (IVssDeploymentServiceHost) TeamFoundationApplicationCore.s_deploymentServiceHost;

    internal static IDeploymentServiceHostInternal DeploymentServiceHostInternal => (IDeploymentServiceHostInternal) TeamFoundationApplicationCore.s_deploymentServiceHost;

    public static bool Log404NotFoundErrors
    {
      get => TeamFoundationApplicationCore.s_log404NotFoundErrors;
      set => TeamFoundationApplicationCore.s_log404NotFoundErrors = value;
    }

    public static bool SslOnly => TeamFoundationApplicationCore.s_sslOnly;

    public static bool UseStrictTransportSecurity { get; private set; }

    internal static IApplicationSettings AppSettings
    {
      get
      {
        if (TeamFoundationApplicationCore.s_appSettings == null)
          TeamFoundationApplicationCore.s_appSettings = (IApplicationSettings) new WebApplicationSettings();
        return TeamFoundationApplicationCore.s_appSettings;
      }
      private set => TeamFoundationApplicationCore.s_appSettings = value;
    }
  }
}
