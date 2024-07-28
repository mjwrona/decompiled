// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationEventLog
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationEventLog
  {
    public static readonly TeamFoundationEventLog Default = new TeamFoundationEventLog();
    private readonly string m_eventSource;
    private readonly string m_machineName;
    private readonly string m_processName;
    private readonly string m_appDomainName;
    private readonly int m_processId;
    private const string c_area = "Logging";
    private const string c_layer = "TeamFoundationEventLog";

    public TeamFoundationEventLog(string eventSource = "TFS Services")
    {
      this.m_eventSource = !string.IsNullOrEmpty(eventSource) ? eventSource : throw new ArgumentNullException(FrameworkResources.EventSourceMissingException());
      this.m_machineName = Environment.MachineName;
      Process currentProcess = Process.GetCurrentProcess();
      this.m_processName = currentProcess.ProcessName;
      this.m_processId = currentProcess.Id;
      this.m_appDomainName = Thread.GetDomain().FriendlyName;
    }

    public void LogException(string message, Exception exception) => this.LogException((IVssRequestContext) null, message, exception);

    public void LogException(
      IVssRequestContext requestContext,
      string message,
      Exception exception)
    {
      this.LogException(requestContext, message, exception, TeamFoundationEventLog.GetEventIdFromException(exception));
    }

    public void LogException(
      IVssRequestContext requestContext,
      string message,
      Exception exception,
      int eventId)
    {
      this.LogException(requestContext, message, exception, eventId, TeamFoundationEventLog.GetLogLevelFromException(exception));
    }

    public void LogException(
      IVssRequestContext requestContext,
      string message,
      Exception exception,
      EventLogEntryType level)
    {
      this.LogException(requestContext, message, exception, TeamFoundationEventLog.GetEventIdFromException(exception), level);
    }

    public void LogException(
      string message,
      Exception exception,
      int eventId,
      EventLogEntryType level)
    {
      this.LogException((IVssRequestContext) null, message, exception, eventId, level);
    }

    public void LogException(
      IVssRequestContext requestContext,
      string message,
      Exception exception,
      int eventId,
      EventLogEntryType level)
    {
      requestContext = requestContext ?? TeamFoundationEventLog.GetRequestContextFromException(exception);
      if (requestContext != null && requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        WhitelistingService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<WhitelistingService>();
        if (service != null && service.IsExceptionExpected(requestContext.Status ?? exception, requestContext.ServiceName))
          return;
      }
      string info = this.Log(requestContext, (string) null, message, exception, eventId, level, (object[]) null);
      if (!TeamFoundationTrace.IsTracing(TraceKeywordSets.General, TraceLevel.Error))
        return;
      TeamFoundationTrace.Error(info);
    }

    public void Log(string message, int eventId, EventLogEntryType level) => this.Log((IVssRequestContext) null, message, eventId, level);

    public void Log(IVssRequestContext requestContext, string message) => this.Log(requestContext, message, EventLogEntryType.Information);

    public void Log(IVssRequestContext requestContext, string message, int eventId) => this.Log(requestContext, message, eventId, EventLogEntryType.Information);

    public void Log(IVssRequestContext requestContext, string message, EventLogEntryType level) => this.Log(requestContext, message, TeamFoundationEventId.DefaultInformationalEventId, level);

    public void Log(
      IVssRequestContext requestContext,
      string message,
      int eventId,
      EventLogEntryType level)
    {
      this.Log(requestContext, (string) null, message, (Exception) null, eventId, level, (object[]) null);
    }

    public void Log(
      IVssRequestContext requestContext,
      string message,
      int eventId,
      EventLogEntryType level,
      params object[] eventValues)
    {
      this.Log(requestContext, (string) null, message, (Exception) null, eventId, level, eventValues);
    }

    public void Log(string messageBrief, string message, int eventId, EventLogEntryType level) => this.Log((IVssRequestContext) null, messageBrief, message, (Exception) null, eventId, level);

    public void Log(
      IVssRequestContext requestContext,
      string messageBrief,
      string message,
      int eventId,
      EventLogEntryType level)
    {
      this.Log(requestContext, messageBrief, message, (Exception) null, eventId, level, (object[]) null);
    }

    private string Log(
      IVssRequestContext requestContext,
      string messageBrief,
      string message,
      Exception ex,
      int eventId,
      EventLogEntryType level,
      params object[] eventValues)
    {
      try
      {
        message = this.FormatMessage(requestContext, messageBrief, message, ex, level);
        if (eventValues != null && eventValues.Length != 0)
        {
          EventInstance instance = new EventInstance((long) eventId, 0, level);
          object[] destinationArray = new object[1 + eventValues.Length];
          destinationArray[0] = (object) message;
          Array.Copy((Array) eventValues, 0, (Array) destinationArray, 1, eventValues.Length);
          EventLog.WriteEvent(this.m_eventSource, instance, destinationArray);
        }
        else
          EventLog.WriteEntry(this.m_eventSource, message, level, eventId);
        TeamFoundationTracingService.TraceRaw(9200, TraceLevel.Verbose, "Logging", nameof (TeamFoundationEventLog), "Wrote log entry. Event source: {0}; Level: {1}; EventId: {2}; Message: {3}", (object) this.m_eventSource, (object) level, (object) eventId, (object) message);
        EventLogEntryLoggedHandler onLog = this.OnLog;
        if (onLog != null)
          onLog(requestContext, this.m_eventSource, eventId, level, message, eventValues);
      }
      catch
      {
      }
      return message;
    }

    public event EventLogEntryLoggedHandler OnLog;

    public event EventLogMessageCreatedHandler OnMessageCreated;

    private string FormatMessage(
      IVssRequestContext requestContext,
      string messageBrief,
      string message,
      Exception ex,
      EventLogEntryType level)
    {
      if (ex != null || level <= EventLogEntryType.Warning)
      {
        if (string.IsNullOrEmpty(messageBrief))
          messageBrief = FrameworkResources.MessageBrief();
        StringBuilder sb = new StringBuilder();
        if (!string.IsNullOrEmpty(message))
          sb.Append(TFCommonResources.ExceptionReport((object) messageBrief, (object) DateTime.UtcNow, (object) this.m_machineName, (object) this.m_appDomainName, (object) TeamFoundationEventLog.GetAssemblyDisplayName(Assembly.GetCallingAssembly()), (object) TeamFoundationEventLog.GetServiceHostName(requestContext), (object) this.m_processName, (object) this.m_processId, (object) TeamFoundationEventLog.GetCurrentThreadId(), (object) TeamFoundationEventLog.GetIdentityName(), (object) message));
        else
          sb.Append(TFCommonResources.ExceptionReportNoDetails((object) messageBrief, (object) DateTime.UtcNow, (object) this.m_machineName, (object) this.m_appDomainName, (object) TeamFoundationEventLog.GetAssemblyDisplayName(Assembly.GetCallingAssembly()), (object) TeamFoundationEventLog.GetServiceHostName(requestContext), (object) this.m_processName, (object) this.m_processId, (object) TeamFoundationEventLog.GetCurrentThreadId(), (object) TeamFoundationEventLog.GetIdentityName()));
        EventLogMessageCreatedHandler onMessageCreated = this.OnMessageCreated;
        if (onMessageCreated != null)
          onMessageCreated((object) this, sb);
        message = sb.ToString();
        if (ex != null)
        {
          string str = TeamFoundationExceptionFormatter.FormatException(ex, TeamFoundationEventLog.ShouldOmitExceptionDetails(level));
          message = message + Environment.NewLine + str;
        }
      }
      else
        message = requestContext?.ServiceHost == null ? FrameworkResources.InformationalEventFormatNoHost((object) message, (object) Thread.GetDomain().FriendlyName) : FrameworkResources.InformationalEventFormat((object) message, (object) Thread.GetDomain().FriendlyName, (object) requestContext.ServiceHost);
      if (message.Length > 16384)
        message = message.Substring(0, 16384);
      message = SecretUtility.ScrubSecrets(message, false);
      return message;
    }

    private static string GetIdentityName()
    {
      string identityName;
      try
      {
        identityName = Thread.CurrentPrincipal.Identity.Name;
        if (string.IsNullOrEmpty(identityName))
          identityName = UserNameUtil.CurrentUserName;
      }
      catch
      {
        identityName = FrameworkResources.NotAvailable();
      }
      return identityName;
    }

    private static string GetServiceHostName(IVssRequestContext requestContext)
    {
      string empty = string.Empty;
      if (requestContext?.ServiceHost != null)
        empty = requestContext.ServiceHost.ToString();
      return empty;
    }

    private static string GetAssemblyDisplayName(Assembly assembly) => assembly.FullName + "; " + assembly.ImageRuntimeVersion;

    private static bool ShouldOmitExceptionDetails(EventLogEntryType level) => level != EventLogEntryType.Warning && level != EventLogEntryType.Error;

    private static IVssRequestContext GetRequestContextFromException(Exception exception)
    {
      IVssRequestContext contextFromException = (IVssRequestContext) null;
      if (exception is TeamFoundationServiceException serviceException)
        contextFromException = serviceException.RequestContext;
      return contextFromException;
    }

    private static EventLogEntryType GetLogLevelFromException(Exception exception)
    {
      EventLogEntryType levelFromException = EventLogEntryType.Error;
      if (exception is TeamFoundationServiceException serviceException)
        levelFromException = serviceException.LogLevel;
      return levelFromException;
    }

    private static int GetEventIdFromException(Exception exception)
    {
      int eventIdFromException = TeamFoundationEventId.DefaultExceptionEventId;
      switch (exception)
      {
        case TeamFoundationServiceException serviceException:
          eventIdFromException = serviceException.EventId;
          break;
        case ThreadAbortException _:
          eventIdFromException = TeamFoundationEventId.ThreadAbortException;
          break;
        case SqlException _:
          eventIdFromException = TeamFoundationEventId.UnexpectedDatabaseResultException;
          break;
      }
      return eventIdFromException;
    }

    [DllImport("kernel32")]
    private static extern int GetCurrentThreadId();
  }
}
