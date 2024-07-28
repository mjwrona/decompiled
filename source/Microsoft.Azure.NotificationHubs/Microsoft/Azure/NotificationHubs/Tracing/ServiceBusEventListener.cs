// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.ServiceBusEventListener
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Eventing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security;
using System.Text;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  internal class ServiceBusEventListener : EventListener
  {
    private static EventDescriptor manifestRequestDescriptor = new EventDescriptor(65534, (byte) 1, (byte) 0, (byte) 0, (byte) 254, 65534, -1L);
    private const string UtcDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'";
    private const string DiagnosticsTracingSettingsKey = "Microsoft.Notifications.Tracing.UseDiagnosticsTracing";
    private const string TraceSourceName = "Microsoft.Notifications";
    private const string TruncatedString = "#TRUNCATED#\" />";
    private const int MaxTraceSize = 31744;
    private const char HighChar = '\uDFFF';
    private const char LowChar = '\uD800';
    private static readonly XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
    {
      OmitXmlDeclaration = true
    };
    private static readonly TraceSource traceSource = new TraceSource("Microsoft.Notifications");
    private EventSource eventSource;
    private static object initializeLock = new object();
    private static bool useDiagnosticsTracing;
    private static bool initialized;
    private IDictionary<int, string> messageDictionary;

    internal Guid ProviderId { get; private set; }

    internal bool EnableDiagnosticsTracing { get; set; }

    public ServiceBusEventListener(Guid providerId) => this.ProviderId = providerId;

    protected internal override void OnEventSourceCreated(EventSource eventSource)
    {
      if (eventSource != null && this.ProviderId == eventSource.Guid)
      {
        this.eventSource = eventSource;
        this.messageDictionary = ServiceBusEventListener.GetEventMessageDictionary(this.eventSource.GetType());
        if (this.IsEnabled())
          this.EnableEvents(eventSource, EventLevel.Informational, EventKeywords.All);
      }
      base.OnEventSourceCreated(eventSource);
    }

    public bool IsEnabled()
    {
      ServiceBusEventListener.EnsureInitialized();
      return ServiceBusEventListener.useDiagnosticsTracing || this.EnableDiagnosticsTracing;
    }

    public bool IsEnabled(byte level, long keywords)
    {
      ServiceBusEventListener.EnsureInitialized();
      return this.EnableDiagnosticsTracing || ServiceBusEventListener.useDiagnosticsTracing;
    }

    public void WriteEvent(EventWrittenEventArgs eventDescriptor)
    {
      ServiceBusEventListener.EnsureInitialized();
      if (ServiceBusEventListener.useDiagnosticsTracing)
      {
        this.WriteToDiagnosticsTrace(eventDescriptor, true);
      }
      else
      {
        if (!this.EnableDiagnosticsTracing)
          return;
        this.WriteToDiagnosticsTrace(eventDescriptor, false);
      }
    }

    private static void EnsureInitialized()
    {
      if (ServiceBusEventListener.initialized)
        return;
      ServiceBusEventListener.InitializeCore();
    }

    private static void InitializeCore()
    {
      lock (ServiceBusEventListener.initializeLock)
      {
        if (ServiceBusEventListener.initialized)
          return;
        string appSetting = ConfigurationManager.AppSettings["Microsoft.Notifications.Tracing.UseDiagnosticsTracing"];
        bool result = false;
        if (appSetting != null && bool.TryParse(appSetting, out result))
          ServiceBusEventListener.useDiagnosticsTracing = result;
        ServiceBusEventListener.initialized = true;
      }
    }

    private static IDictionary<int, string> GetEventMessageDictionary(Type eventSourceType)
    {
      Dictionary<int, string> messageDictionary = new Dictionary<int, string>();
      MethodInfo[] methods = eventSourceType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      ResourceManager resourceManager = new ResourceManager(((EventSourceAttribute) Attribute.GetCustomAttribute((MemberInfo) eventSourceType, typeof (EventSourceAttribute), false)).LocalizationResources, eventSourceType.Assembly);
      for (int index = 0; index < methods.Length; ++index)
      {
        MethodInfo element = methods[index];
        element.GetParameters();
        EventAttribute customAttribute = (EventAttribute) Attribute.GetCustomAttribute((MemberInfo) element, typeof (EventAttribute), false);
        if (customAttribute != null)
        {
          string str = element.Name.Replace("EventWrite", string.Empty);
          messageDictionary[customAttribute.EventId] = resourceManager.GetString("event_" + str);
        }
      }
      return (IDictionary<int, string>) messageDictionary;
    }

    private string ConvertMessageToXmlFormattedString(EventWrittenEventArgs eventDescriptor)
    {
      StringBuilder output = new StringBuilder();
      using (XmlWriter xmlWriter = XmlWriter.Create(output, ServiceBusEventListener.xmlWriterSettings))
      {
        xmlWriter.WriteStartElement("Trc");
        xmlWriter.WriteAttributeString("Id", eventDescriptor.EventId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        xmlWriter.WriteAttributeString("Ch", ServiceBusEventListener.GetEventChannel(eventDescriptor).ToString());
        xmlWriter.WriteAttributeString("Lvl", ServiceBusEventListener.GetEventLevel(eventDescriptor).ToString());
        xmlWriter.WriteAttributeString("Kw", eventDescriptor.Keywords.ToString("x"));
        xmlWriter.WriteAttributeString("UTC", DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'", (IFormatProvider) CultureInfo.InvariantCulture));
        string message = this.messageDictionary[eventDescriptor.EventId];
        if (message != null)
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, ServiceBusEventListener.GetObjectAgsWithoutActivity(eventDescriptor.Payload));
          xmlWriter.WriteAttributeString("Msg", SecurityElement.Escape(str));
          xmlWriter.WriteEndElement();
        }
      }
      string xmlFormattedString = output.ToString();
      if (xmlFormattedString.Length > 31744)
        xmlFormattedString = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", new object[2]
        {
          (object) xmlFormattedString.Substring(0, 31744 - "#TRUNCATED#\" />".Length),
          (object) "#TRUNCATED#\" />"
        });
      return xmlFormattedString;
    }

    private static object[] GetObjectAgsWithoutActivity(IEnumerable<object> readOnlyCollection) => readOnlyCollection.Where<object>((Func<object, bool>) (e => !(e is EventTraceActivity))).ToArray<object>();

    private bool WriteToDiagnosticsTrace(
      EventWrittenEventArgs eventDescriptor,
      bool writeToConfigSource)
    {
      TraceEventType eventLevel = ServiceBusEventListener.GetEventLevel(eventDescriptor);
      ServiceBusEventListener.TraceEventChannel eventChannel = ServiceBusEventListener.GetEventChannel(eventDescriptor);
      if (writeToConfigSource && (eventChannel == ServiceBusEventListener.TraceEventChannel.Operational || eventChannel == ServiceBusEventListener.TraceEventChannel.Admin))
      {
        string xmlFormattedString = this.ConvertMessageToXmlFormattedString(eventDescriptor);
        ServiceBusEventListener.traceSource.TraceEvent(eventLevel, eventDescriptor.EventId, xmlFormattedString);
      }
      if (this.EnableDiagnosticsTracing)
      {
        string xmlFormattedString = this.ConvertMessageToXmlFormattedString(eventDescriptor);
        switch (eventLevel)
        {
          case TraceEventType.Critical:
          case TraceEventType.Error:
            Trace.TraceError(xmlFormattedString);
            break;
          case TraceEventType.Warning:
            Trace.TraceWarning(xmlFormattedString);
            break;
          default:
            Trace.TraceInformation(xmlFormattedString);
            break;
        }
      }
      return true;
    }

    private static ServiceBusEventListener.TraceEventChannel GetEventChannel(
      EventWrittenEventArgs eventDescriptor)
    {
      ServiceBusEventListener.TraceEventChannel eventChannel = ServiceBusEventListener.TraceEventChannel.Debug;
      switch (eventDescriptor.Channel)
      {
        case (EventChannel) 16:
          eventChannel = ServiceBusEventListener.TraceEventChannel.Admin;
          break;
        case (EventChannel) 17:
          eventChannel = ServiceBusEventListener.TraceEventChannel.Operational;
          break;
        case (EventChannel) 18:
          eventChannel = ServiceBusEventListener.TraceEventChannel.Analytic;
          break;
        case (EventChannel) 19:
          eventChannel = ServiceBusEventListener.TraceEventChannel.Debug;
          break;
      }
      return eventChannel;
    }

    private static TraceEventType GetEventLevel(EventWrittenEventArgs eventDescriptor)
    {
      TraceEventType eventLevel = TraceEventType.Verbose;
      switch (eventDescriptor.Level)
      {
        case EventLevel.LogAlways:
          eventLevel = TraceEventType.Verbose;
          break;
        case EventLevel.Critical:
          eventLevel = TraceEventType.Critical;
          break;
        case EventLevel.Error:
          eventLevel = TraceEventType.Error;
          break;
        case EventLevel.Warning:
          eventLevel = TraceEventType.Warning;
          break;
        case EventLevel.Informational:
          eventLevel = TraceEventType.Information;
          break;
        case EventLevel.Verbose:
          eventLevel = TraceEventType.Verbose;
          break;
      }
      return eventLevel;
    }

    protected internal override void OnEventWritten(EventWrittenEventArgs eventData)
    {
      if (eventData.EventId == ServiceBusEventListener.manifestRequestDescriptor.EventId)
        return;
      this.WriteEvent(eventData);
    }

    private enum TraceEventChannel
    {
      Admin = 1,
      Operational = 2,
      Analytic = 3,
      Debug = 4,
    }
  }
}
