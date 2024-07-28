// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.DiagnosticTrace
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Common.Diagnostics;
using Microsoft.Azure.NotificationHubs.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal class DiagnosticTrace
  {
    private const string DefaultTraceListenerName = "Default";
    private const int MaxTraceSize = 65535;
    private bool tracingEnabled = true;
    private bool haveListeners;
    private object localSyncObject = new object();
    private DateTime lastFailure = DateTime.MinValue;
    private SourceLevels level;
    private bool calledShutdown;
    private bool shouldUseActivity;
    private string AppDomainFriendlyName;
    private PiiTraceSource traceSource;
    private TraceSourceKind traceSourceType = TraceSourceKind.PiiTraceSource;
    private const string subType = "";
    private const string version = "1";
    private const int traceFailureLogThreshold = 1;
    private string TraceSourceName = string.Empty;
    private string eventSourceName = string.Empty;
    private const string TraceRecordVersion = "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord";
    private const SourceLevels DefaultLevel = SourceLevels.Off;
    private static SortedList<TraceCode, string> traceCodes = new SortedList<TraceCode, string>();
    private static object classLockObject = new object();

    internal static string ProcessName
    {
      get
      {
        using (Process currentProcess = Process.GetCurrentProcess())
          return currentProcess.ProcessName;
      }
    }

    internal static int ProcessId
    {
      get
      {
        using (Process currentProcess = Process.GetCurrentProcess())
          return currentProcess.Id;
      }
    }

    internal PiiTraceSource TraceSource
    {
      get => this.traceSource;
      set => this.traceSource = value;
    }

    private static SourceLevels FixLevel(SourceLevels level)
    {
      if ((level & ~SourceLevels.Information & SourceLevels.Verbose) != SourceLevels.Off)
        level |= SourceLevels.Verbose;
      else if ((level & ~SourceLevels.Warning & SourceLevels.Information) != SourceLevels.Off)
        level |= SourceLevels.Information;
      else if ((level & ~SourceLevels.Error & SourceLevels.Warning) != SourceLevels.Off)
        level |= SourceLevels.Warning;
      if ((level & ~SourceLevels.Critical & SourceLevels.Error) != SourceLevels.Off)
        level |= SourceLevels.Error;
      if ((level & SourceLevels.Critical) != SourceLevels.Off)
        level |= SourceLevels.Critical;
      if (level == SourceLevels.ActivityTracing)
        level = SourceLevels.Off;
      return level;
    }

    private void SetLevel(SourceLevels level)
    {
      SourceLevels sourceLevels = DiagnosticTrace.FixLevel(level);
      this.level = sourceLevels;
      if (this.TraceSource == null)
        return;
      this.haveListeners = this.TraceSource.Listeners.Count > 0;
      if (this.TraceSource.Switch.Level != SourceLevels.Off && level == SourceLevels.Off)
      {
        PiiTraceSource traceSource = this.TraceSource;
        this.CreateTraceSource();
        traceSource.Close();
      }
      this.tracingEnabled = this.HaveListeners && sourceLevels != 0;
      this.TraceSource.Switch.Level = sourceLevels;
      this.shouldUseActivity = (sourceLevels & SourceLevels.ActivityTracing) != 0;
    }

    private void SetLevelThreadSafe(SourceLevels level)
    {
      lock (this.localSyncObject)
        this.SetLevel(level);
    }

    [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.Level instead")]
    internal SourceLevels Level
    {
      get
      {
        if (this.TraceSource != null && this.TraceSource.Switch.Level != this.level)
          this.level = this.TraceSource.Switch.Level;
        return this.level;
      }
      set => this.SetLevelThreadSafe(value);
    }

    internal static string CodeToString(TraceCode code)
    {
      string str = (string) null;
      if (!DiagnosticTrace.traceCodes.TryGetValue(code, out str))
      {
        lock (DiagnosticTrace.classLockObject)
        {
          if (!DiagnosticTrace.traceCodes.TryGetValue(code, out str))
          {
            str = code.ToString();
            DiagnosticTrace.traceCodes.Add(code, str);
          }
        }
      }
      return str;
    }

    internal static string GenerateTraceCode(TraceCode code)
    {
      string str;
      switch ((TraceCode) ((long) code & 4294901760L))
      {
        case TraceCode.Administration:
          str = "System.ServiceModel.Administration";
          break;
        case TraceCode.Diagnostics:
          str = "System.ServiceModel.Diagnostics";
          break;
        case TraceCode.Serialization:
          str = "System.Runtime.Serialization";
          break;
        case TraceCode.Channels:
          str = "System.ServiceModel.Channels";
          break;
        case TraceCode.ComIntegration:
          str = "System.ServiceModel.ComIntegration";
          break;
        case TraceCode.Security:
          str = "System.ServiceModel.Security";
          break;
        case TraceCode.ServiceModel:
        case TraceCode.ServiceModelTransaction:
          str = "System.ServiceModel";
          break;
        case TraceCode.Activation:
          str = "System.ServiceModel.Activation";
          break;
        case TraceCode.PortSharing:
          str = "System.ServiceModel.PortSharing";
          break;
        case TraceCode.TransactionBridge:
          str = "Microsoft.Transactions.TransactionBridge";
          break;
        case TraceCode.IdentityModel:
          str = "System.IdentityModel";
          break;
        case TraceCode.IdentityModelSelectors:
          str = "System.IdentityModel.Selectors";
          break;
        default:
          str = string.Empty;
          break;
      }
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "http://msdn.microsoft.com/{0}/library/{1}.{2}.aspx", new object[3]
      {
        (object) CultureInfo.CurrentCulture.Name,
        (object) str,
        (object) DiagnosticTrace.CodeToString(code)
      });
    }

    [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.HaveListeners instead")]
    internal bool HaveListeners => this.haveListeners;

    [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.ShouldTrace instead")]
    internal bool ShouldTrace(TraceEventType type) => this.TracingEnabled && this.TraceSource != null && (type & (TraceEventType) this.Level) != 0;

    [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.ShouldUseActivity instead")]
    internal bool ShouldUseActivity => this.shouldUseActivity;

    [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.TracingEnabled instead")]
    internal bool TracingEnabled => this.tracingEnabled && this.traceSource != null;

    [Obsolete("For SMDiagnostics.dll use only. Never 'new' this type up unless you are DiagnosticUtility.")]
    internal DiagnosticTrace(
      TraceSourceKind sourceType,
      string traceSourceName,
      string eventSourceName)
    {
      this.traceSourceType = sourceType;
      this.TraceSourceName = traceSourceName;
      this.eventSourceName = eventSourceName;
      this.AppDomainFriendlyName = AppDomain.CurrentDomain.FriendlyName;
      try
      {
        this.CreateTraceSource();
        AppDomain currentDomain = AppDomain.CurrentDomain;
        this.haveListeners = this.TraceSource.Listeners.Count > 0;
        this.tracingEnabled = this.HaveListeners;
        if (!this.TracingEnabled)
          return;
        currentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.UnhandledExceptionHandler);
        this.SetLevel(this.TraceSource.Switch.Level);
        currentDomain.DomainUnload += new EventHandler(this.ExitOrUnloadEventHandler);
        currentDomain.ProcessExit += new EventHandler(this.ExitOrUnloadEventHandler);
      }
      catch (ConfigurationErrorsException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          new EventLogger(this.eventSourceName, (object) null).LogEvent(TraceEventType.Error, EventLogCategory.Tracing, EventLogEventId.FailedToSetupTracing, false, ex.ToString());
      }
    }

    private void CreateTraceSource()
    {
      PiiTraceSource piiTraceSource = this.traceSourceType != TraceSourceKind.PiiTraceSource ? (PiiTraceSource) new DiagnosticTraceSource(this.TraceSourceName, this.eventSourceName, SourceLevels.Off) : new PiiTraceSource(this.TraceSourceName, this.eventSourceName, SourceLevels.Off);
      piiTraceSource.Listeners.Remove("Default");
      this.TraceSource = piiTraceSource;
    }

    internal void TraceEvent(
      TraceEventType type,
      TraceCode code,
      string description,
      TraceRecord trace,
      Exception exception,
      object source)
    {
      TraceXPathNavigator navigator = (TraceXPathNavigator) null;
      try
      {
        if (this.TraceSource == null)
          return;
        if (!this.HaveListeners)
          return;
        try
        {
          this.BuildTrace(type, code, description, trace, exception, source, out navigator);
        }
        catch (PlainXmlWriter.MaxSizeExceededException ex)
        {
          StringTraceRecord trace1 = new StringTraceRecord("TruncatedTraceId", DiagnosticTrace.GenerateTraceCode(code));
          this.TraceEvent(type, TraceCode.TraceTruncatedQuotaExceeded, Microsoft.Azure.NotificationHubs.SR.GetString(Resources.TraceCodeTraceTruncatedQuotaExceeded), (TraceRecord) trace1);
        }
        this.TraceSource.TraceData(type, (int) code, (object) navigator);
        if (this.calledShutdown)
          this.TraceSource.Flush();
        this.LastFailure = DateTime.MinValue;
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          this.LogTraceFailure(navigator == null ? string.Empty : navigator.ToString(), ex);
      }
    }

    internal void TraceEvent(TraceEventType type, TraceCode code, string description) => this.TraceEvent(type, code, description, (TraceRecord) null, (Exception) null, (object) null);

    internal void TraceEvent(
      TraceEventType type,
      TraceCode code,
      string description,
      TraceRecord trace)
    {
      this.TraceEvent(type, code, description, trace, (Exception) null, (object) null);
    }

    internal void TraceEvent(
      TraceEventType type,
      TraceCode code,
      string description,
      TraceRecord trace,
      Exception exception)
    {
      this.TraceEvent(type, code, description, trace, exception, (object) null);
    }

    internal void TraceEvent(
      TraceEventType type,
      TraceCode code,
      string description,
      TraceRecord trace,
      Exception exception,
      Guid activityId,
      object source)
    {
      using (!this.ShouldUseActivity || !(Guid.Empty == activityId) ? Activity.CreateActivity(activityId) : (Activity) null)
        this.TraceEvent(type, code, description, trace, exception, source);
    }

    internal static string CreateSourceString(object source) => source.GetType().ToString() + "/" + source.GetHashCode().ToString((IFormatProvider) CultureInfo.CurrentCulture);

    private static string LookupSeverity(TraceEventType type)
    {
      string str;
      switch (type)
      {
        case TraceEventType.Critical:
          str = "Critical";
          break;
        case TraceEventType.Error:
          str = "Error";
          break;
        case TraceEventType.Warning:
          str = "Warning";
          break;
        case TraceEventType.Information:
          str = "Information";
          break;
        case TraceEventType.Verbose:
          str = "Verbose";
          break;
        case TraceEventType.Start:
          str = "Start";
          break;
        case TraceEventType.Stop:
          str = "Stop";
          break;
        case TraceEventType.Suspend:
          str = "Suspend";
          break;
        case TraceEventType.Transfer:
          str = "Transfer";
          break;
        default:
          str = type.ToString();
          break;
      }
      return str;
    }

    private DateTime LastFailure
    {
      get => this.lastFailure;
      set => this.lastFailure = value;
    }

    private void LogTraceFailure(string traceString, Exception e)
    {
      TimeSpan timeSpan = TimeSpan.FromMinutes(10.0);
      try
      {
        lock (this.localSyncObject)
        {
          if (!(DateTime.UtcNow.Subtract(this.LastFailure) >= timeSpan))
            return;
          this.LastFailure = DateTime.UtcNow;
          EventLogger eventLogger = new EventLogger(this.eventSourceName, (object) this);
          if (e == null)
            eventLogger.LogEvent(TraceEventType.Error, EventLogCategory.Tracing, EventLogEventId.FailedToTraceEvent, false, traceString);
          else
            eventLogger.LogEvent(TraceEventType.Error, EventLogCategory.Tracing, EventLogEventId.FailedToTraceEventWithException, false, traceString, e.ToString());
        }
      }
      catch
      {
      }
    }

    private void ShutdownTracing()
    {
      if (this.TraceSource == null)
        return;
      if (this.calledShutdown)
        return;
      try
      {
        if (this.Level == SourceLevels.Off)
          return;
        if (this.ShouldTrace(TraceEventType.Information))
          this.TraceEvent(TraceEventType.Information, TraceCode.AppDomainUnload, Microsoft.Azure.NotificationHubs.SR.GetString(Resources.TraceCodeAppDomainUnload), (TraceRecord) new DictionaryTraceRecord((IDictionary) new Dictionary<string, string>(3)
          {
            ["AppDomain.FriendlyName"] = AppDomain.CurrentDomain.FriendlyName,
            ["ProcessName"] = DiagnosticTrace.ProcessName,
            ["ProcessId"] = DiagnosticTrace.ProcessId.ToString((IFormatProvider) CultureInfo.CurrentCulture)
          }), (Exception) null, (object) null);
        this.calledShutdown = true;
        this.TraceSource.Flush();
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          this.LogTraceFailure((string) null, ex);
      }
    }

    private void ExitOrUnloadEventHandler(object sender, EventArgs e) => this.ShutdownTracing();

    private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
    {
      Exception exceptionObject = (Exception) args.ExceptionObject;
      this.TraceEvent(TraceEventType.Critical, TraceCode.UnhandledException, Microsoft.Azure.NotificationHubs.SR.GetString(Resources.UnhandledException), (TraceRecord) null, exceptionObject, (object) null);
      this.ShutdownTracing();
    }

    private void BuildTrace(
      TraceEventType type,
      TraceCode code,
      string description,
      TraceRecord trace,
      Exception exception,
      object source,
      out TraceXPathNavigator navigator)
    {
      PlainXmlWriter xml = new PlainXmlWriter((int) ushort.MaxValue);
      navigator = xml.Navigator;
      this.BuildTrace(xml, type, code, description, trace, exception, source);
      if (this.TraceSource.ShouldLogPii)
        return;
      navigator.RemovePii(DiagnosticStrings.HeadersPaths);
    }

    private void BuildTrace(
      PlainXmlWriter xml,
      TraceEventType type,
      TraceCode code,
      string description,
      TraceRecord trace,
      Exception exception,
      object source)
    {
      xml.WriteStartElement("TraceRecord");
      xml.WriteAttributeString("xmlns", "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord");
      xml.WriteAttributeString("Severity", DiagnosticTrace.LookupSeverity(type));
      xml.WriteElementString("TraceIdentifier", DiagnosticTrace.GenerateTraceCode(code));
      xml.WriteElementString("Description", description);
      xml.WriteElementString("AppDomain", this.AppDomainFriendlyName);
      if (source != null)
        xml.WriteElementString("Source", DiagnosticTrace.CreateSourceString(source));
      if (trace != null)
      {
        xml.WriteStartElement("ExtendedData");
        xml.WriteAttributeString("xmlns", trace.EventId);
        trace.WriteTo((XmlWriter) xml);
        xml.WriteEndElement();
      }
      if (exception != null)
      {
        xml.WriteStartElement("Exception");
        this.AddExceptionToTraceString((XmlWriter) xml, exception);
        xml.WriteEndElement();
      }
      xml.WriteEndElement();
    }

    internal static Guid ActivityId
    {
      get
      {
        object activityId = (object) Trace.CorrelationManager.ActivityId;
        return activityId != null ? (Guid) activityId : Guid.Empty;
      }
      set => Trace.CorrelationManager.ActivityId = value;
    }

    private void AddExceptionToTraceString(XmlWriter xml, Exception exception)
    {
      xml.WriteElementString("ExceptionType", DiagnosticTrace.XmlEncode(exception.GetType().AssemblyQualifiedName));
      xml.WriteElementString("Message", DiagnosticTrace.XmlEncode(exception.Message));
      xml.WriteElementString("StackTrace", DiagnosticTrace.XmlEncode(DiagnosticTrace.StackTraceString(exception)));
      xml.WriteElementString("ExceptionString", DiagnosticTrace.XmlEncode(exception.ToString()));
      if (exception is Win32Exception win32Exception)
        xml.WriteElementString("NativeErrorCode", win32Exception.NativeErrorCode.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture));
      if (exception.Data != null && exception.Data.Count > 0)
      {
        xml.WriteStartElement("DataItems");
        foreach (object key in (IEnumerable) exception.Data.Keys)
        {
          xml.WriteStartElement("Data");
          xml.WriteElementString("Key", DiagnosticTrace.XmlEncode(key.ToString()));
          xml.WriteElementString("Value", DiagnosticTrace.XmlEncode(exception.Data[key].ToString()));
          xml.WriteEndElement();
        }
        xml.WriteEndElement();
      }
      if (exception.InnerException == null)
        return;
      xml.WriteStartElement("InnerException");
      this.AddExceptionToTraceString(xml, exception.InnerException);
      xml.WriteEndElement();
    }

    private static string StackTraceString(Exception exception)
    {
      string stackTrace = exception.StackTrace;
      if (string.IsNullOrEmpty(stackTrace))
      {
        StackFrame[] frames = new StackTrace(false).GetFrames();
        int skipFrames = 0;
        bool flag = false;
        foreach (StackFrame stackFrame in frames)
        {
          string name = stackFrame.GetMethod().Name;
          if (name == nameof (StackTraceString) || name == "AddExceptionToTraceString" || name == "BuildTrace" || name == "TraceEvent" || name == "TraceException")
            ++skipFrames;
          else if (name.StartsWith("ThrowHelper", StringComparison.Ordinal))
            ++skipFrames;
          else
            flag = true;
          if (flag)
            break;
        }
        stackTrace = new StackTrace(skipFrames, false).ToString();
      }
      return stackTrace;
    }

    internal static string XmlEncode(string text)
    {
      if (string.IsNullOrEmpty(text))
        return text;
      int length = text.Length;
      StringBuilder stringBuilder = new StringBuilder(length + 8);
      for (int index = 0; index < length; ++index)
      {
        char ch = text[index];
        switch (ch)
        {
          case '&':
            stringBuilder.Append("&amp;");
            break;
          case '<':
            stringBuilder.Append("&lt;");
            break;
          case '>':
            stringBuilder.Append("&gt;");
            break;
          default:
            stringBuilder.Append(ch);
            break;
        }
      }
      return stringBuilder.ToString();
    }
  }
}
