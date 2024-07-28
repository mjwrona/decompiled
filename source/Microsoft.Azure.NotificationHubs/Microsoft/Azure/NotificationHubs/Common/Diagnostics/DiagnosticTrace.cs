// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Diagnostics.DiagnosticTrace
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Tracing;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Azure.NotificationHubs.Common.Diagnostics
{
  internal sealed class DiagnosticTrace
  {
    private const string DefaultTraceListenerName = "Default";
    private const string TraceRecordVersion = "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord";
    private const int WindowsVistaMajorNumber = 6;
    private const string EventSourceVersion = "4.0.0.0";
    private const ushort TracingEventLogCategory = 4;
    private const string defaultEtwId = "A307C7A2-A4CD-4D22-8093-94DB72934152";
    [SecurityCritical]
    private static Guid defaultEtwProviderId = MessagingClientEtwProvider.IsEtwEnabled() ? MessagingClientEtwProvider.Provider.Guid : new Guid("A307C7A2-A4CD-4D22-8093-94DB72934152");
    private static Hashtable etwProviderCache = new Hashtable();
    private static bool isVistaOrGreater = Environment.OSVersion.Version.Major >= 6;
    private static string appDomainFriendlyName = AppDomain.CurrentDomain.FriendlyName;
    private bool calledShutdown;
    private bool haveListeners;
    private object thisLock;
    private SourceLevels level;
    private DiagnosticTraceSource traceSource;
    [SecurityCritical]
    private EtwProvider etwProvider;
    private string TraceSourceName;
    [SecurityCritical]
    private string eventSourceName;

    [SecurityCritical]
    static DiagnosticTrace()
    {
    }

    public DiagnosticTrace(string traceSourceName, Guid etwProviderId)
    {
      try
      {
        this.thisLock = new object();
        this.TraceSourceName = traceSourceName;
        this.eventSourceName = this.TraceSourceName + " " + "4.0.0.0";
        this.LastFailure = DateTime.MinValue;
        this.CreateTraceSource();
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          new EventLogger(this.eventSourceName, (DiagnosticTrace) null).LogEvent(TraceEventType.Error, (ushort) 4, 3221291108U, false, ex.ToString());
      }
      try
      {
        this.CreateEtwProvider(etwProviderId);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          this.etwProvider = (EtwProvider) null;
          new EventLogger(this.eventSourceName, (DiagnosticTrace) null).LogEvent(TraceEventType.Error, (ushort) 4, 3221291108U, false, ex.ToString());
        }
      }
      if (!this.TracingEnabled && !this.EtwTracingEnabled)
        return;
      this.AddDomainEventHandlersForCleanup();
    }

    public static Guid DefaultEtwProviderId
    {
      get => DiagnosticTrace.defaultEtwProviderId;
      [SecurityCritical] set => DiagnosticTrace.defaultEtwProviderId = value;
    }

    private DateTime LastFailure { get; set; }

    public DiagnosticTraceSource TraceSource => this.traceSource;

    public EtwProvider EtwProvider
    {
      [SecurityCritical] get => this.etwProvider;
    }

    public bool IsEtwProviderEnabled => this.EtwTracingEnabled && this.etwProvider.IsEnabled();

    public bool HaveListeners => this.haveListeners;

    public static Guid ActivityId
    {
      get
      {
        object activityId = (object) Trace.CorrelationManager.ActivityId;
        return activityId != null ? (Guid) activityId : Guid.Empty;
      }
      set => Trace.CorrelationManager.ActivityId = value;
    }

    public SourceLevels Level
    {
      get
      {
        if (this.TraceSource != null)
          this.level = this.TraceSource.Switch.Level;
        return this.level;
      }
    }

    public bool TracingEnabled => this.traceSource != null;

    private bool EtwTracingEnabled => this.etwProvider != null;

    private static string ProcessName
    {
      get
      {
        using (Process currentProcess = Process.GetCurrentProcess())
          return currentProcess.ProcessName;
      }
    }

    private static int ProcessId
    {
      get
      {
        using (Process currentProcess = Process.GetCurrentProcess())
          return currentProcess.Id;
      }
    }

    public bool ShouldTrace(TraceEventLevel eventLevel) => this.ShouldTraceToTraceSource(eventLevel) || this.ShouldTraceToEtw(eventLevel);

    public bool ShouldTraceToTraceSource(TraceEventLevel eventLevel) => this.HaveListeners && this.TraceSource != null && (TraceLevelHelper.GetTraceEventType(eventLevel) & (TraceEventType) this.Level) != 0;

    public bool ShouldTraceToEtw(TraceEventLevel traceEventLevel) => this.EtwProvider != null && this.EtwProvider.IsEnabled((byte) traceEventLevel, 0L);

    public void Event(
      int eventId,
      TraceEventLevel traceEventLevel,
      TraceChannel channel,
      string description)
    {
      if (!this.TracingEnabled)
        return;
      System.Diagnostics.Eventing.EventDescriptor eventDescriptor = DiagnosticTrace.GetEventDescriptor(eventId, channel, traceEventLevel);
      this.Event(ref eventDescriptor, description);
    }

    [SecurityCritical]
    public void Event(ref System.Diagnostics.Eventing.EventDescriptor eventDescriptor, string description)
    {
      if (!this.TracingEnabled)
        return;
      TracePayload serializedPayload = DiagnosticTrace.GetSerializedPayload((object) null, (TraceRecord) null, (Exception) null);
      this.WriteTraceSource(ref eventDescriptor, description, serializedPayload);
    }

    [SecurityCritical]
    public void WriteTraceSource(
      ref System.Diagnostics.Eventing.EventDescriptor eventDescriptor,
      string description,
      TracePayload payload)
    {
      if (!this.TracingEnabled)
        return;
      XPathNavigator data = (XPathNavigator) null;
      try
      {
        string s = DiagnosticTrace.BuildTrace(ref eventDescriptor, description, payload);
        XmlDocument xmlDocument = new XmlDocument();
        StringReader input = new StringReader(s);
        using (XmlReader reader = XmlReader.Create((TextReader) input, new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit
        }))
          xmlDocument.Load(reader);
        data = xmlDocument.CreateNavigator();
        this.TraceSource.TraceData(TraceLevelHelper.GetTraceEventType(eventDescriptor.Level, eventDescriptor.Opcode), eventDescriptor.EventId, (object) data);
        if (!this.calledShutdown)
          return;
        this.TraceSource.Flush();
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          this.LogTraceFailure(data == null ? string.Empty : data.ToString(), ex);
      }
    }

    [SecurityCritical]
    private static string BuildTrace(
      ref System.Diagnostics.Eventing.EventDescriptor eventDescriptor,
      string description,
      TracePayload payload)
    {
      StringBuilder sb = new StringBuilder();
      XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) new StringWriter(sb, (IFormatProvider) CultureInfo.CurrentCulture));
      xmlTextWriter.WriteStartElement("TraceRecord");
      xmlTextWriter.WriteAttributeString("xmlns", "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord");
      xmlTextWriter.WriteAttributeString("Severity", TraceLevelHelper.LookupSeverity((TraceEventLevel) eventDescriptor.Level, (TraceEventOpcode) eventDescriptor.Opcode));
      xmlTextWriter.WriteAttributeString("Channel", DiagnosticTrace.LookupChannel((TraceChannel) eventDescriptor.Channel));
      xmlTextWriter.WriteElementString("TraceIdentifier", DiagnosticTrace.GenerateTraceCode(ref eventDescriptor));
      xmlTextWriter.WriteElementString("Description", description);
      xmlTextWriter.WriteElementString("AppDomain", payload.AppDomainFriendlyName);
      if (!string.IsNullOrEmpty(payload.EventSource))
        xmlTextWriter.WriteElementString("Source", payload.EventSource);
      if (!string.IsNullOrEmpty(payload.ExtendedData))
        xmlTextWriter.WriteRaw(payload.ExtendedData);
      if (!string.IsNullOrEmpty(payload.SerializedException))
        xmlTextWriter.WriteRaw(payload.SerializedException);
      xmlTextWriter.WriteEndElement();
      return sb.ToString();
    }

    [SecurityCritical]
    private static string GenerateTraceCode(ref System.Diagnostics.Eventing.EventDescriptor eventDescriptor) => eventDescriptor.EventId.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    private static string LookupChannel(TraceChannel traceChannel)
    {
      string str;
      switch (traceChannel)
      {
        case TraceChannel.Application:
          str = "Application";
          break;
        case TraceChannel.Admin:
          str = "Admin";
          break;
        case TraceChannel.Operational:
          str = "Operational";
          break;
        case TraceChannel.Analytic:
          str = "Analytic";
          break;
        case TraceChannel.Debug:
          str = "Debug";
          break;
        case TraceChannel.Perf:
          str = "Perf";
          break;
        default:
          str = traceChannel.ToString();
          break;
      }
      return str;
    }

    public static TracePayload GetSerializedPayload(
      object source,
      TraceRecord traceRecord,
      Exception exception)
    {
      return DiagnosticTrace.GetSerializedPayload(source, traceRecord, exception, false);
    }

    public static TracePayload GetSerializedPayload(
      object source,
      TraceRecord traceRecord,
      Exception exception,
      bool getServiceReference)
    {
      string eventSource = (string) null;
      string extendedData = (string) null;
      string serializedException = (string) null;
      if (source != null)
        eventSource = DiagnosticTrace.CreateSourceString(source);
      if (traceRecord != null)
      {
        StringBuilder sb = new StringBuilder();
        XmlTextWriter writer = new XmlTextWriter((TextWriter) new StringWriter(sb, (IFormatProvider) CultureInfo.CurrentCulture));
        writer.WriteStartElement("ExtendedData");
        traceRecord.WriteTo((XmlWriter) writer);
        writer.WriteEndElement();
        extendedData = sb.ToString();
      }
      if (exception != null)
        serializedException = DiagnosticTrace.ExceptionToTraceString(exception);
      return new TracePayload(serializedException, eventSource, DiagnosticTrace.appDomainFriendlyName, extendedData, string.Empty);
    }

    public bool IsEtwEventEnabled(ref System.Diagnostics.Eventing.EventDescriptor eventDescriptor) => this.EtwTracingEnabled && this.etwProvider.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords);

    public static string XmlEncode(string text)
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

    private void CreateTraceSource()
    {
      if (string.IsNullOrEmpty(this.TraceSourceName))
        return;
      this.traceSource = new DiagnosticTraceSource(this.TraceSourceName);
      if (this.traceSource == null)
        return;
      this.traceSource.Listeners.Remove("Default");
      this.haveListeners = this.traceSource.Listeners.Count > 0;
      this.level = this.traceSource.Switch.Level;
    }

    [Obsolete("For SMDiagnostics.dll use only")]
    private void AddDomainEventHandlersForCleanup()
    {
      AppDomain currentDomain = AppDomain.CurrentDomain;
      if (!this.TracingEnabled)
        return;
      currentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.UnhandledExceptionHandler);
      currentDomain.DomainUnload += new EventHandler(this.ExitOrUnloadEventHandler);
      currentDomain.ProcessExit += new EventHandler(this.ExitOrUnloadEventHandler);
    }

    private void CreateEtwProvider(Guid etwProviderId)
    {
      if (!(etwProviderId != Guid.Empty) || !DiagnosticTrace.isVistaOrGreater)
        return;
      this.etwProvider = (EtwProvider) DiagnosticTrace.etwProviderCache[(object) etwProviderId];
      if (this.etwProvider != null)
        return;
      lock (DiagnosticTrace.etwProviderCache)
      {
        this.etwProvider = (EtwProvider) DiagnosticTrace.etwProviderCache[(object) etwProviderId];
        if (this.etwProvider != null)
          return;
        this.etwProvider = new EtwProvider(etwProviderId);
        DiagnosticTrace.etwProviderCache.Add((object) etwProviderId, (object) this.etwProvider);
      }
    }

    private void ExitOrUnloadEventHandler(object sender, EventArgs e) => this.ShutdownTracing();

    private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
    {
      TraceCore.UnhandledException(this, (Exception) args.ExceptionObject);
      this.ShutdownTracing();
    }

    private static string CreateSourceString(object source) => source.GetType().ToString() + "/" + source.GetHashCode().ToString((IFormatProvider) CultureInfo.CurrentCulture);

    [SecurityCritical]
    private static System.Diagnostics.Eventing.EventDescriptor GetEventDescriptor(
      int eventId,
      TraceChannel channel,
      TraceEventLevel traceEventLevel)
    {
      long keywords = 0;
      switch (channel)
      {
        case TraceChannel.Admin:
          keywords |= long.MinValue;
          break;
        case TraceChannel.Operational:
          keywords |= 4611686018427387904L;
          break;
        case TraceChannel.Analytic:
          keywords |= 2305843009213693952L;
          break;
        case TraceChannel.Debug:
          keywords |= 72057594037927936L;
          break;
        case TraceChannel.Perf:
          keywords |= 576460752303423488L;
          break;
      }
      return new System.Diagnostics.Eventing.EventDescriptor(eventId, (byte) 0, (byte) channel, (byte) traceEventLevel, (byte) 0, 0, keywords);
    }

    private static string ExceptionToTraceString(Exception exception)
    {
      StringBuilder sb = new StringBuilder();
      XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) new StringWriter(sb, (IFormatProvider) CultureInfo.CurrentCulture));
      xmlTextWriter.WriteStartElement("Exception");
      xmlTextWriter.WriteElementString("ExceptionType", DiagnosticTrace.XmlEncode(exception.GetType().AssemblyQualifiedName));
      xmlTextWriter.WriteElementString("Message", DiagnosticTrace.XmlEncode(exception.Message));
      xmlTextWriter.WriteElementString("StackTrace", DiagnosticTrace.XmlEncode(DiagnosticTrace.StackTraceString(exception)));
      xmlTextWriter.WriteElementString("ExceptionString", DiagnosticTrace.XmlEncode(exception.ToString()));
      if (exception is Win32Exception win32Exception)
        xmlTextWriter.WriteElementString("NativeErrorCode", win32Exception.NativeErrorCode.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture));
      if (exception.Data != null && exception.Data.Count > 0)
      {
        xmlTextWriter.WriteStartElement("DataItems");
        foreach (object key in (IEnumerable) exception.Data.Keys)
        {
          xmlTextWriter.WriteStartElement("Data");
          xmlTextWriter.WriteElementString("Key", DiagnosticTrace.XmlEncode(key.ToString()));
          xmlTextWriter.WriteElementString("Value", DiagnosticTrace.XmlEncode(exception.Data[key].ToString()));
          xmlTextWriter.WriteEndElement();
        }
        xmlTextWriter.WriteEndElement();
      }
      if (exception.InnerException != null)
      {
        xmlTextWriter.WriteStartElement("InnerException");
        xmlTextWriter.WriteRaw(DiagnosticTrace.ExceptionToTraceString(exception.InnerException));
        xmlTextWriter.WriteEndElement();
      }
      xmlTextWriter.WriteEndElement();
      return sb.ToString();
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
          if (name == nameof (StackTraceString) || name == "AddExceptionToTraceString" || name == "GetAdditionalPayload")
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

    private void LogTraceFailure(string traceString, Exception exception)
    {
      TimeSpan timeSpan = TimeSpan.FromMinutes(10.0);
      try
      {
        lock (this.thisLock)
        {
          if (!(DateTime.UtcNow.Subtract(this.LastFailure) >= timeSpan))
            return;
          this.LastFailure = DateTime.UtcNow;
          EventLogger eventLogger = EventLogger.UnsafeCreateEventLogger(this.eventSourceName, this);
          if (exception == null)
            eventLogger.UnsafeLogEvent(TraceEventType.Error, (ushort) 4, 3221291112U, false, traceString);
          else
            eventLogger.UnsafeLogEvent(TraceEventType.Error, (ushort) 4, 3221291113U, false, traceString, exception.ToString());
        }
      }
      catch (Exception ex)
      {
        if (!Fx.IsFatal(ex))
          return;
        throw;
      }
    }

    private void ShutdownTracing()
    {
      if (this.calledShutdown)
        return;
      this.calledShutdown = true;
      this.ShutdownTraceSource();
      this.ShutdownEtwProvider();
    }

    private void ShutdownTraceSource()
    {
      try
      {
        MessagingClientEtwProvider.Provider.EventWriteAppDomainUnload(AppDomain.CurrentDomain.FriendlyName, DiagnosticTrace.ProcessName, DiagnosticTrace.ProcessId);
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

    private void ShutdownEtwProvider()
    {
      try
      {
        if (this.etwProvider == null)
          return;
        this.etwProvider.Dispose();
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          this.LogTraceFailure((string) null, ex);
      }
    }
  }
}
