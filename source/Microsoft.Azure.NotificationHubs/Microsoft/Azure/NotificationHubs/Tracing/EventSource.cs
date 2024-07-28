// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.EventSource
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Eventing;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  internal class EventSource : IDisposable
  {
    private const int MaxTraceBufferLength = 7168;
    private const string EventTraceContinuedTemplate = "Event Trace Continued...";
    private readonly string m_name;
    internal int m_id;
    private readonly Guid m_guid;
    internal volatile EventSource.EventMetadata[] m_eventData;
    private volatile byte[] m_rawManifest;
    private bool m_eventSourceEnabled;
    internal EventLevel m_level;
    internal EventKeywords m_matchAnyKeyword;
    internal volatile ulong[] m_channelData;
    internal volatile EventDispatcher m_Dispatchers;
    private volatile EventSource.OverideEventProvider m_provider;
    private bool m_completelyInited;
    private bool m_ETWManifestSent;

    public string Name => this.m_name;

    public Guid Guid => this.m_guid;

    public bool IsEnabled() => this.m_eventSourceEnabled;

    public bool IsEnabled(int eventId)
    {
      if (!this.m_eventSourceEnabled || this.m_eventData == null)
        return false;
      return this.m_eventData[eventId].EnabledForETW || this.m_eventData[eventId].EnabledForAnyListener;
    }

    public bool IsEnabled(EventLevel level, EventKeywords keywords, EventChannel channel)
    {
      if (!this.m_eventSourceEnabled || this.m_level != EventLevel.LogAlways && this.m_level < level)
        return false;
      EventKeywords eventKeywords = (EventKeywords) this.m_channelData[(int) channel] | keywords;
      return this.m_matchAnyKeyword == EventKeywords.None || (eventKeywords & this.m_matchAnyKeyword) != 0;
    }

    public static Guid GetGuid(Type eventSourceType)
    {
      EventSourceAttribute customAttribute = (EventSourceAttribute) Attribute.GetCustomAttribute((MemberInfo) eventSourceType, typeof (EventSourceAttribute), false);
      string name = eventSourceType.Name;
      if (customAttribute != null)
      {
        if (customAttribute.Guid != null)
        {
          Guid empty = Guid.Empty;
          try
          {
            return new Guid(customAttribute.Guid);
          }
          catch (Exception ex)
          {
          }
        }
        if (customAttribute.Name != null)
          name = customAttribute.Name;
      }
      throw new ArgumentException(EventSourceSR.ProviderGuidNotSpecified((object) name));
    }

    public static string GetName(Type eventSourceType)
    {
      EventSourceAttribute customAttribute = (EventSourceAttribute) Attribute.GetCustomAttribute((MemberInfo) eventSourceType, typeof (EventSourceAttribute), false);
      return customAttribute != null && customAttribute.Name != null ? customAttribute.Name : eventSourceType.Name;
    }

    public static string GenerateManifest(
      Type eventSourceType,
      string assemblyPathToIncludeInManifest,
      string resourceFileName)
    {
      return Encoding.UTF8.GetString(EventSource.CreateManifestAndDescriptors(eventSourceType, assemblyPathToIncludeInManifest, resourceFileName, (EventSource) null));
    }

    public static void SendCommand(
      EventSource eventSource,
      EventCommand command,
      IDictionary<string, string> commandArguments)
    {
      if (eventSource == null)
        throw new ArgumentNullException(nameof (eventSource));
      EventSource.SendCommand(eventSource, (EventListener) null, command, true, EventLevel.LogAlways, EventKeywords.None, commandArguments);
    }

    public static IEnumerable<EventSource> GetSources()
    {
      List<EventSource> sources = new List<EventSource>();
      lock (EventListener.EventListenersLock)
      {
        foreach (WeakReference eventSource in EventListener.s_EventSources)
        {
          if (eventSource.Target is EventSource target)
            sources.Add(target);
        }
      }
      return (IEnumerable<EventSource>) sources;
    }

    public override string ToString() => "EventSource(" + this.Name + "," + (object) this.Guid + ")";

    protected EventSource(bool disableTracing = false)
    {
      Guid guid = EventSource.GetGuid(this.GetType());
      string name = EventSource.GetName(this.GetType());
      if (guid == Guid.Empty)
        throw new ArgumentNullException("EventSource.eventSourceGuid");
      if (name == null)
        throw new ArgumentNullException("EventSource.eventSourceName");
      if (disableTracing)
        return;
      this.m_name = name;
      this.m_guid = guid;
      this.m_provider = new EventSource.OverideEventProvider(this);
      this.m_provider.Register(guid);
      if (this.m_eventSourceEnabled && !this.m_ETWManifestSent)
      {
        this.SendManifest(this.m_rawManifest, (EventListener) null);
        this.m_ETWManifestSent = true;
      }
      EventListener.AddEventSource(this);
      this.m_completelyInited = true;
    }

    protected virtual void OnEventCommand(EventCommandEventArgs command)
    {
    }

    protected void WriteEvent(int eventId)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
        this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, 0, (IntPtr) 0);
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId);
    }

    protected unsafe void WriteEvent(int eventId, int arg1)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
      {
        EventProviderClone.EventData* data = stackalloc EventProviderClone.EventData[1];
        data->Ptr = (ulong) &arg1;
        data->Size = 4U;
        data->Reserved = 0U;
        this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, 1, (IntPtr) (void*) data);
      }
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId, (object) arg1);
    }

    protected unsafe void WriteEvent(int eventId, int arg1, int arg2)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
      {
        EventProviderClone.EventData* data = stackalloc EventProviderClone.EventData[2];
        data->Ptr = (ulong) &arg1;
        data->Size = 4U;
        data->Reserved = 0U;
        data[1].Ptr = (ulong) &arg2;
        data[1].Size = 4U;
        data[1].Reserved = 0U;
        this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, 2, (IntPtr) (void*) data);
      }
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId, (object) arg1, (object) arg2);
    }

    protected unsafe void WriteEvent(int eventId, int arg1, int arg2, int arg3)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
      {
        EventProviderClone.EventData* data = stackalloc EventProviderClone.EventData[3];
        data->Ptr = (ulong) &arg1;
        data->Size = 4U;
        data->Reserved = 0U;
        data[1].Ptr = (ulong) &arg2;
        data[1].Size = 4U;
        data[1].Reserved = 0U;
        data[2].Ptr = (ulong) &arg3;
        data[2].Size = 4U;
        data[2].Reserved = 0U;
        this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, 3, (IntPtr) (void*) data);
      }
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId, (object) arg1, (object) arg2, (object) arg3);
    }

    protected unsafe void WriteEvent(int eventId, long arg1)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
      {
        EventProviderClone.EventData* data = stackalloc EventProviderClone.EventData[1];
        data->Ptr = (ulong) &arg1;
        data->Size = 8U;
        data->Reserved = 0U;
        this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, 1, (IntPtr) (void*) data);
      }
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId, (object) arg1);
    }

    protected unsafe void WriteEvent(int eventId, long arg1, long arg2)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
      {
        EventProviderClone.EventData* data = stackalloc EventProviderClone.EventData[2];
        data->Ptr = (ulong) &arg1;
        data->Size = 8U;
        data->Reserved = 0U;
        data[1].Ptr = (ulong) &arg2;
        data[1].Size = 8U;
        data[1].Reserved = 0U;
        this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, 2, (IntPtr) (void*) data);
      }
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId, (object) arg1, (object) arg2);
    }

    protected unsafe void WriteEvent(int eventId, long arg1, long arg2, long arg3)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
      {
        EventProviderClone.EventData* data = stackalloc EventProviderClone.EventData[3];
        data->Ptr = (ulong) &arg1;
        data->Size = 8U;
        data->Reserved = 0U;
        data[1].Ptr = (ulong) &arg2;
        data[1].Size = 8U;
        data[1].Reserved = 0U;
        data[2].Ptr = (ulong) &arg3;
        data[2].Size = 8U;
        data[2].Reserved = 0U;
        this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, 3, (IntPtr) (void*) data);
      }
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId, (object) arg1, (object) arg2, (object) arg3);
    }

    protected unsafe void WriteEvent(int eventId, string arg1)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
      {
        if (arg1 == null)
          arg1 = "";
        fixed (char* chPtr = arg1)
        {
          EventProviderClone.EventData* data = stackalloc EventProviderClone.EventData[1];
          data->Ptr = (ulong) chPtr;
          data->Size = (uint) ((arg1.Length + 1) * 2);
          data->Reserved = 0U;
          this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, 1, (IntPtr) (void*) data);
        }
      }
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId, (object) arg1);
    }

    protected unsafe void WriteEvent(int eventId, string arg1, string arg2)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
      {
        if (arg1 == null)
          arg1 = "";
        if (arg2 == null)
          arg2 = "";
        fixed (char* chPtr1 = arg1)
          fixed (char* chPtr2 = arg2)
          {
            EventProviderClone.EventData* data = stackalloc EventProviderClone.EventData[2];
            data->Ptr = (ulong) chPtr1;
            data->Size = (uint) ((arg1.Length + 1) * 2);
            data->Reserved = 0U;
            data[1].Ptr = (ulong) chPtr2;
            data[1].Size = (uint) ((arg2.Length + 1) * 2);
            data[1].Reserved = 0U;
            this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, 2, (IntPtr) (void*) data);
          }
      }
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId, (object) arg1, (object) arg2);
    }

    protected unsafe void WriteEvent(int eventId, string arg1, string arg2, string arg3)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
      {
        if (arg1 == null)
          arg1 = "";
        if (arg2 == null)
          arg2 = "";
        if (arg3 == null)
          arg3 = "";
        fixed (char* chPtr1 = arg1)
          fixed (char* chPtr2 = arg2)
            fixed (char* chPtr3 = arg3)
            {
              EventProviderClone.EventData* data = stackalloc EventProviderClone.EventData[3];
              data->Ptr = (ulong) chPtr1;
              data->Size = (uint) ((arg1.Length + 1) * 2);
              data->Reserved = 0U;
              data[1].Ptr = (ulong) chPtr2;
              data[1].Size = (uint) ((arg2.Length + 1) * 2);
              data[1].Reserved = 0U;
              data[2].Ptr = (ulong) chPtr3;
              data[2].Size = (uint) ((arg3.Length + 1) * 2);
              data[2].Reserved = 0U;
              this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, 3, (IntPtr) (void*) data);
            }
      }
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId, (object) arg1, (object) arg2, (object) arg3);
    }

    protected unsafe void WriteEvent(int eventId, string arg1, int arg2)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
      {
        if (arg1 == null)
          arg1 = "";
        fixed (char* chPtr = arg1)
        {
          EventProviderClone.EventData* data = stackalloc EventProviderClone.EventData[2];
          data->Ptr = (ulong) chPtr;
          data->Size = (uint) ((arg1.Length + 1) * 2);
          data->Reserved = 0U;
          data[1].Ptr = (ulong) &arg2;
          data[1].Size = 4U;
          data[1].Reserved = 0U;
          this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, 2, (IntPtr) (void*) data);
        }
      }
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId, (object) arg1, (object) arg2);
    }

    protected unsafe void WriteEvent(int eventId, string arg1, int arg2, int arg3)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
      {
        if (arg1 == null)
          arg1 = "";
        fixed (char* chPtr = arg1)
        {
          EventProviderClone.EventData* data = stackalloc EventProviderClone.EventData[3];
          data->Ptr = (ulong) chPtr;
          data->Size = (uint) ((arg1.Length + 1) * 2);
          data->Reserved = 0U;
          data[1].Ptr = (ulong) &arg2;
          data[1].Size = 4U;
          data[1].Reserved = 0U;
          data[2].Ptr = (ulong) &arg3;
          data[2].Size = 4U;
          data[2].Reserved = 0U;
          this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, 3, (IntPtr) (void*) data);
        }
      }
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId, (object) arg1, (object) arg2, (object) arg3);
    }

    protected unsafe void WriteEvent(int eventId, string arg1, int arg2, long arg3)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
      {
        if (arg1 == null)
          arg1 = "";
        fixed (char* chPtr = arg1)
        {
          EventProviderClone.EventData* data = stackalloc EventProviderClone.EventData[3];
          data->Ptr = (ulong) chPtr;
          data->Size = (uint) ((arg1.Length + 1) * 2);
          data->Reserved = 0U;
          data[1].Ptr = (ulong) &arg2;
          data[1].Size = 4U;
          data[1].Reserved = 0U;
          data[2].Ptr = (ulong) &arg3;
          data[2].Size = 8U;
          data[2].Reserved = 0U;
          this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, 3, (IntPtr) (void*) data);
        }
      }
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId, (object) arg1, (object) arg2, (object) arg3);
    }

    protected unsafe void WriteEvent(int eventId, string arg1, long arg2)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
      {
        if (arg1 == null)
          arg1 = "";
        fixed (char* chPtr = arg1)
        {
          EventProviderClone.EventData* data = stackalloc EventProviderClone.EventData[2];
          data->Ptr = (ulong) chPtr;
          data->Size = (uint) ((arg1.Length + 1) * 2);
          data->Reserved = 0U;
          data[1].Ptr = (ulong) &arg2;
          data[1].Size = 8U;
          data[1].Reserved = 0U;
          this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, 2, (IntPtr) (void*) data);
        }
      }
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId, (object) arg1, (object) arg2);
    }

    [SecurityCritical]
    protected unsafe void WriteEventCore(
      int eventId,
      int eventDataCount,
      EventSource.EventData* data)
    {
      EventProviderClone.EventData* data1 = stackalloc EventProviderClone.EventData[eventDataCount];
      for (int index = 0; index < eventDataCount; ++index)
      {
        data1[index].Size = (uint) data[index].Size;
        data1[index].Ptr = (ulong) data[index].DataPointer.ToInt64();
        data1[index].Reserved = 0U;
      }
      if (this.m_provider != null)
        this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, eventDataCount, (IntPtr) (void*) data1);
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      object[] objArray = new object[eventDataCount];
      for (int parameterId = 0; parameterId < eventDataCount; ++parameterId)
        objArray[parameterId] = this.DecodeObject(eventId, parameterId, data[parameterId].Size, data[parameterId].DataPointer);
      this.WriteToAllListeners(eventId, objArray);
    }

    protected void WriteEvent(int eventId, params object[] args)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
        this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, args);
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId, args);
    }

    protected void WriteEvent(
      int eventId,
      EventTraceActivity traceActivityId,
      params object[] args)
    {
      if (this.m_eventData != null && this.m_eventData[eventId].EnabledForETW && this.m_provider != null)
      {
        if (traceActivityId != null)
        {
          int num = (int) this.m_provider.SetActivityId(ref traceActivityId.ActivityId);
        }
        this.m_provider.WriteEvent(ref this.m_eventData[eventId].Descriptor, args);
      }
      if (this.m_Dispatchers == null || !this.m_eventData[eventId].EnabledForAnyListener)
        return;
      this.WriteToAllListeners(eventId, args);
    }

    protected void WriteTransferEvent(
      int eventId,
      EventTraceActivity traceActivityId,
      EventTraceActivity relatedActivityId,
      params object[] args)
    {
      if (this.m_provider == null)
        return;
      this.m_provider.WriteTransfer(ref this.m_eventData[eventId].Descriptor, ref traceActivityId.ActivityId, ref relatedActivityId.ActivityId, args);
    }

    protected void SetActivityId(ref Guid activityId)
    {
      if (this.m_provider == null)
        return;
      int num = (int) this.m_provider.SetActivityId(ref activityId);
    }

    private bool DoDebugChecks() => true;

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.m_provider == null)
        return;
      this.m_provider.Dispose();
      this.m_provider = (EventSource.OverideEventProvider) null;
    }

    ~EventSource() => this.Dispose(false);

    [SecurityCritical]
    private unsafe object DecodeObject(
      int eventId,
      int parameterId,
      int dataBytes,
      IntPtr dataPointer)
    {
      for (Type enumType = this.m_eventData[eventId].Parameters[parameterId].ParameterType; !(enumType == typeof (IntPtr)); enumType = Enum.GetUnderlyingType(enumType))
      {
        if (enumType == typeof (int))
          return (object) *(int*) (void*) dataPointer;
        if (enumType == typeof (uint))
          return (object) *(uint*) (void*) dataPointer;
        if (enumType == typeof (long))
          return (object) *(long*) (void*) dataPointer;
        if (enumType == typeof (ulong))
          return (object) (ulong) *(long*) (void*) dataPointer;
        if (enumType == typeof (byte))
          return (object) *(byte*) (void*) dataPointer;
        if (enumType == typeof (sbyte))
          return (object) *(sbyte*) (void*) dataPointer;
        if (enumType == typeof (short))
          return (object) *(short*) (void*) dataPointer;
        if (enumType == typeof (ushort))
          return (object) *(ushort*) (void*) dataPointer;
        if (enumType == typeof (float))
          return (object) *(float*) (void*) dataPointer;
        if (enumType == typeof (double))
          return (object) *(double*) (void*) dataPointer;
        if (enumType == typeof (Decimal))
          return (object) *(Decimal*) (void*) dataPointer;
        if (enumType == typeof (bool))
          return (object) (bool) *(byte*) (void*) dataPointer;
        if (enumType == typeof (Guid))
          return (object) *(Guid*) (void*) dataPointer;
        if (enumType == typeof (char))
          return (object) (char) *(ushort*) (void*) dataPointer;
        if (!enumType.IsEnum)
          return (object) Marshal.PtrToStringUni(dataPointer, dataBytes / 2);
      }
      return (object) *(IntPtr*) (void*) dataPointer;
    }

    private EventDispatcher GetDispatcher(EventListener listener)
    {
      EventDispatcher dispatcher = this.m_Dispatchers;
      while (dispatcher != null && dispatcher.m_Listener != listener)
        dispatcher = dispatcher.m_Next;
      return dispatcher;
    }

    private void WriteToAllListeners(int eventId, params object[] args)
    {
      EventWrittenEventArgs eventData = new EventWrittenEventArgs(this);
      eventData.EventId = eventId;
      eventData.Payload = (IEnumerable<object>) args;
      for (EventDispatcher eventDispatcher = this.m_Dispatchers; eventDispatcher != null; eventDispatcher = eventDispatcher.m_Next)
      {
        if (eventDispatcher.m_EventEnabled[eventId])
          eventDispatcher.m_Listener.OnEventWritten(eventData);
      }
    }

    private bool IsEnabledByDefault(
      int eventNum,
      bool enable,
      EventLevel currentLevel,
      EventKeywords currentMatchAnyKeyword)
    {
      if (!enable)
        return false;
      int level = (int) this.m_eventData[eventNum].Descriptor.Level;
      EventKeywords keywords = (EventKeywords) this.m_eventData[eventNum].Descriptor.Keywords;
      int num = (int) currentLevel;
      return (level <= num || currentLevel == EventLevel.LogAlways) && (keywords == EventKeywords.None || (keywords & currentMatchAnyKeyword) != EventKeywords.None);
    }

    internal static void SendCommand(
      EventSource eventSource,
      EventListener eventListener,
      EventCommand command,
      bool enable,
      EventLevel level,
      EventKeywords matchAnyKeyword,
      IDictionary<string, string> commandArguments)
    {
      eventSource.SendCommand(eventListener, command, enable, level, matchAnyKeyword, commandArguments);
    }

    protected void WriteSBTraceEvent(
      int eventId,
      EventTraceActivity traceActivityId,
      params object[] list)
    {
      if (list == null || list.Length == 0)
        throw new ArgumentException("list is null or empty");
      int index1 = list.Length - 1;
      int stringLength = list[index1].ToStringLength();
      int num = 0;
      for (int index2 = 0; index2 < index1; ++index2)
      {
        if ((num += list[index2].ToStringLength()) > 7168)
        {
          this.WriteEvent(eventId, traceActivityId, list);
          return;
        }
      }
      if (stringLength + num < 7168)
      {
        this.WriteEvent(eventId, traceActivityId, list);
      }
      else
      {
        int startIndex = 0;
        int val1 = 7168 - num;
        string str1 = list[index1] != null ? list[index1].ToString() : string.Empty;
        for (; startIndex < stringLength; startIndex += val1)
        {
          string str2 = str1.Substring(startIndex, Math.Min(val1, stringLength - startIndex));
          list[index1] = startIndex == 0 ? (object) str2 : (object) ("Event Trace Continued..." + str2);
          this.WriteEvent(eventId, traceActivityId, list);
        }
      }
    }

    internal void SendCommand(
      EventListener listener,
      EventCommand command,
      bool enable,
      EventLevel level,
      EventKeywords matchAnyKeyword,
      IDictionary<string, string> commandArguments)
    {
      this.InsureInitialized();
      EventDispatcher dispatcher = this.GetDispatcher(listener);
      if (dispatcher == null && listener != null)
        throw new ArgumentException(EventSourceSR.Event_ListenerNotFound);
      if (commandArguments == null)
        commandArguments = (IDictionary<string, string>) new Dictionary<string, string>();
      switch (command)
      {
        case EventCommand.SendManifest:
          this.SendManifest(this.m_rawManifest, (EventListener) null);
          break;
        case EventCommand.Update:
          for (int index = 0; index < this.m_eventData.Length; ++index)
            this.EnableEventForDispatcher(dispatcher, index, this.IsEnabledByDefault(index, enable, level, matchAnyKeyword));
          command = EventCommand.Disable;
          if (enable)
          {
            command = EventCommand.Enable;
            if (!this.m_eventSourceEnabled)
            {
              this.m_level = level;
              this.m_matchAnyKeyword = matchAnyKeyword;
            }
            else
            {
              if (level > this.m_level)
                this.m_level = level;
              if (matchAnyKeyword == EventKeywords.None)
                this.m_matchAnyKeyword = EventKeywords.None;
              else if (this.m_matchAnyKeyword != EventKeywords.None)
                this.m_matchAnyKeyword |= matchAnyKeyword;
            }
            if (dispatcher != null)
            {
              if (!dispatcher.m_ManifestSent)
              {
                dispatcher.m_ManifestSent = true;
                this.SendManifest(this.m_rawManifest, dispatcher.m_Listener);
              }
            }
            else if (!this.m_ETWManifestSent && this.m_completelyInited)
            {
              this.m_ETWManifestSent = true;
              this.SendManifest(this.m_rawManifest, (EventListener) null);
            }
          }
          this.OnEventCommand(new EventCommandEventArgs(command, commandArguments, this, dispatcher));
          if (enable)
          {
            this.m_eventSourceEnabled = true;
            return;
          }
          if (dispatcher != null)
            dispatcher.m_ManifestSent = false;
          else
            this.m_ETWManifestSent = false;
          for (int index = 0; index < this.m_eventData.Length; ++index)
          {
            this.m_eventData[index].EnabledForAnyListener = false;
            for (EventDispatcher eventDispatcher = this.m_Dispatchers; eventDispatcher != null; eventDispatcher = eventDispatcher.m_Next)
            {
              if (eventDispatcher.m_EventEnabled[index])
              {
                this.m_eventData[index].EnabledForAnyListener = true;
                break;
              }
            }
          }
          if (this.AnyEventEnabled())
            return;
          this.m_level = EventLevel.LogAlways;
          this.m_matchAnyKeyword = EventKeywords.None;
          this.m_eventSourceEnabled = false;
          return;
      }
      this.OnEventCommand(new EventCommandEventArgs(command, commandArguments, (EventSource) null, (EventDispatcher) null));
    }

    [Conditional("DEBUG")]
    private void TraceControllerCommand(
      EventCommand command,
      EventLevel level,
      EventKeywords matchAnyKeyword)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Enabled channels  = ");
      for (int index = 0; index < this.m_channelData.Length; ++index)
      {
        if ((matchAnyKeyword & (EventKeywords) this.m_channelData[index]) != EventKeywords.None)
          stringBuilder.Append("0x").Append(index.ToString("x")).Append("{0x").Append(this.m_channelData[index].ToString("x")).Append("} ");
      }
    }

    internal bool EnableEventForDispatcher(EventDispatcher dispatcher, int eventId, bool value)
    {
      if (dispatcher == null)
      {
        if (eventId >= this.m_eventData.Length)
          return false;
        this.m_eventData[eventId].EnabledForETW = value;
      }
      else
      {
        if (eventId >= dispatcher.m_EventEnabled.Length)
          return false;
        dispatcher.m_EventEnabled[eventId] = value;
        if (value)
          this.m_eventData[eventId].EnabledForAnyListener = true;
      }
      return true;
    }

    private bool AnyEventEnabled()
    {
      for (int index = 0; index < this.m_eventData.Length; ++index)
      {
        if (this.m_eventData[index].EnabledForETW || this.m_eventData[index].EnabledForAnyListener)
          return true;
      }
      return false;
    }

    private void InsureInitialized()
    {
      lock (EventListener.EventListenersLock)
      {
        if (this.m_rawManifest == null)
          this.m_rawManifest = EventSource.CreateManifestAndDescriptors(this.GetType(), "", this);
        if (this.DoDebugChecks())
        {
          foreach (WeakReference eventSource in EventListener.s_EventSources)
          {
            if (eventSource.Target is EventSource target && target.Guid == this.m_guid && target != this)
              throw new ArgumentException(EventSourceSR.Event_SourceWithUsedGuid((object) this.m_guid));
          }
        }
        for (EventDispatcher eventDispatcher = this.m_Dispatchers; eventDispatcher != null; eventDispatcher = eventDispatcher.m_Next)
        {
          if (eventDispatcher.m_EventEnabled == null)
            eventDispatcher.m_EventEnabled = new bool[this.m_eventData.Length];
        }
      }
    }

    private unsafe bool SendManifest(byte[] rawManifest, EventListener Listener)
    {
      fixed (byte* numPtr = rawManifest)
      {
        EventDescriptor eventDescriptor = new EventDescriptor(65534, (byte) 1, (byte) 0, (byte) 0, (byte) 254, 65534, -1L);
        ManifestEnvelope manifestEnvelope = new ManifestEnvelope();
        manifestEnvelope.Format = ManifestEnvelope.ManifestFormats.SimpleXmlFormat;
        manifestEnvelope.MajorVersion = (byte) 1;
        manifestEnvelope.MinorVersion = (byte) 0;
        manifestEnvelope.Magic = (byte) 91;
        int length = rawManifest.Length;
        manifestEnvelope.TotalChunks = (ushort) ((length + 65279) / 65280);
        manifestEnvelope.ChunkNumber = (ushort) 0;
        EventProviderClone.EventData* data = stackalloc EventProviderClone.EventData[2];
        data->Ptr = (ulong) &manifestEnvelope;
        data->Size = (uint) sizeof (ManifestEnvelope);
        data->Reserved = 0U;
        data[1].Ptr = (ulong) numPtr;
        data[1].Reserved = 0U;
        bool flag = true;
        while (length > 0)
        {
          data[1].Size = (uint) Math.Min(length, 65280);
          if (Listener == null)
            flag = this.m_provider == null || !this.m_provider.WriteEvent(ref eventDescriptor, 2, (IntPtr) (void*) data);
          if (Listener != null)
          {
            byte[] destination1 = (byte[]) null;
            byte[] destination2 = (byte[]) null;
            if (destination1 == null)
            {
              destination1 = new byte[(int) data->Size];
              destination2 = new byte[(int) data[1].Size];
            }
            Marshal.Copy((IntPtr) (long) data->Ptr, destination1, 0, (int) data->Size);
            Marshal.Copy((IntPtr) (long) data[1].Ptr, destination2, 0, (int) data[1].Size);
            EventWrittenEventArgs eventData = new EventWrittenEventArgs(this);
            eventData.EventId = eventDescriptor.EventId;
            EventWrittenEventArgs writtenEventArgs = eventData;
            ReadOnlyCollection<object> readOnlyCollection = new ReadOnlyCollection<object>((IList<object>) new List<object>()
            {
              (object) destination1,
              (object) destination2
            });
            writtenEventArgs.Payload = (IEnumerable<object>) readOnlyCollection;
            Listener.OnEventWritten(eventData);
          }
          length -= 65280;
          data[1].Ptr += 65280UL;
          ++manifestEnvelope.ChunkNumber;
        }
        return flag;
      }
    }

    internal static byte[] CreateManifestAndDescriptors(
      Type eventSourceType,
      string eventSourceDllName,
      EventSource source)
    {
      Type eventSourceType1 = eventSourceType;
      string str = eventSourceDllName;
      EventSource source1 = source;
      return EventSource.CreateManifestAndDescriptors(eventSourceType1, str, str, source1);
    }

    internal static byte[] CreateManifestAndDescriptors(
      Type eventSourceType,
      string eventSourceDllName,
      string resourceFileName,
      EventSource source)
    {
      MethodInfo[] methods = eventSourceType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      int eventId = 1;
      EventSource.EventMetadata[] eventData = (EventSource.EventMetadata[]) null;
      Dictionary<string, string> eventsByName = (Dictionary<string, string>) null;
      if (source != null)
        eventData = new EventSource.EventMetadata[methods.Length];
      ResourceManager resources = (ResourceManager) null;
      EventSourceAttribute customAttribute = (EventSourceAttribute) Attribute.GetCustomAttribute((MemberInfo) eventSourceType, typeof (EventSourceAttribute), false);
      if (customAttribute != null && customAttribute.LocalizationResources != null)
        resources = new ResourceManager(customAttribute.LocalizationResources, eventSourceType.Assembly);
      ManifestBuilder manifest = new ManifestBuilder(EventSource.GetName(eventSourceType), EventSource.GetGuid(eventSourceType), eventSourceDllName, resourceFileName, resources);
      string[] strArray = new string[4]
      {
        "Keywords",
        "Tasks",
        "Opcodes",
        "Channels"
      };
      foreach (string str in strArray)
      {
        Type nestedType = eventSourceType.GetNestedType(str);
        if (nestedType != (Type) null)
        {
          foreach (FieldInfo field in nestedType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            EventSource.AddProviderEnumKind(manifest, field, str);
        }
      }
      SortedList<int, Tuple<MethodInfo, EventAttribute>> sortedList = new SortedList<int, Tuple<MethodInfo, EventAttribute>>();
      for (int index = 0; index < methods.Length; ++index)
      {
        MethodInfo methodInfo = methods[index];
        ParameterInfo[] parameters = methodInfo.GetParameters();
        EventAttribute eventAttribute = (EventAttribute) Attribute.GetCustomAttribute((MemberInfo) methodInfo, typeof (EventAttribute), false);
        string str = methodInfo.Name.Replace("EventWrite", string.Empty);
        if (methodInfo.ReturnType != typeof (void))
        {
          if (eventAttribute != null)
            throw new ArgumentException(EventSourceSR.Event_EventNotReturnVoid((object) str));
        }
        else if (!methodInfo.IsVirtual && !methodInfo.IsStatic)
        {
          if (eventAttribute == null)
          {
            if (Attribute.GetCustomAttribute((MemberInfo) methodInfo, typeof (NonEventAttribute), false) == null)
              eventAttribute = new EventAttribute(eventId);
            else
              continue;
          }
          else if (eventAttribute.EventId <= 0)
            throw new ArgumentException(EventSourceSR.Event_IllegalID);
          ++eventId;
          if (eventAttribute.Opcode == EventOpcode.Info && eventAttribute.Task == EventTask.None)
            eventAttribute.Task = (EventTask) (65534 - eventAttribute.EventId);
          sortedList.Add(eventAttribute.EventId, new Tuple<MethodInfo, EventAttribute>(methodInfo, eventAttribute));
          if (source != null)
          {
            eventAttribute.Keywords |= (EventKeywords) manifest.GetChannelKeyword(eventAttribute.Channel);
            if (source.DoDebugChecks())
              EventSource.DebugCheckEvent(ref eventsByName, eventData, methodInfo, eventAttribute);
            EventSource.AddEventDescriptor(ref eventData, eventAttribute, parameters);
          }
        }
      }
      foreach (KeyValuePair<int, Tuple<MethodInfo, EventAttribute>> keyValuePair in sortedList)
      {
        MethodInfo methodInfo = keyValuePair.Value.Item1;
        EventAttribute eventAttribute = keyValuePair.Value.Item2;
        string eventName = methodInfo.Name.Replace("EventWrite", string.Empty);
        manifest.StartEvent(eventName, eventAttribute);
        ParameterInfo[] parameters = methodInfo.GetParameters();
        for (int index = 0; index < parameters.Length; ++index)
        {
          if (!(parameters[index].ParameterType.Name == "EventTraceActivity"))
            manifest.AddEventParameter(parameters[index].ParameterType, parameters[index].Name);
        }
        manifest.EndEvent();
      }
      if (source != null)
      {
        EventSource.TrimEventDescriptors(ref eventData);
        source.m_eventData = eventData;
        source.m_channelData = manifest.GetChannelData();
      }
      return manifest.CreateManifest();
    }

    private static void AddProviderEnumKind(
      ManifestBuilder manifest,
      FieldInfo staticField,
      string providerEnumKind)
    {
      Type fieldType = staticField.FieldType;
      if (fieldType == typeof (EventOpcode))
      {
        if (!(providerEnumKind != "Opcodes"))
        {
          int rawConstantValue = (int) staticField.GetRawConstantValue();
          if (rawConstantValue <= 10)
            throw new ArgumentException(EventSourceSR.Event_IllegalOpcode);
          manifest.AddOpcode(staticField.Name, rawConstantValue);
          return;
        }
      }
      else if (fieldType == typeof (EventTask))
      {
        if (!(providerEnumKind != "Tasks"))
        {
          manifest.AddTask(staticField.Name, (int) staticField.GetRawConstantValue());
          return;
        }
      }
      else if (fieldType == typeof (EventKeywords))
      {
        if (!(providerEnumKind != "Keywords"))
        {
          manifest.AddKeyword(staticField.Name, (ulong) (long) staticField.GetRawConstantValue());
          return;
        }
      }
      else
      {
        if (!(fieldType == typeof (EventChannel)))
          return;
        if (!(providerEnumKind != "Channels"))
        {
          ChannelAttribute customAttribute = (ChannelAttribute) Attribute.GetCustomAttribute((MemberInfo) staticField, typeof (ChannelAttribute), false);
          byte rawConstantValue = (byte) staticField.GetRawConstantValue();
          manifest.AddChannel(staticField.Name, (int) rawConstantValue, customAttribute);
          return;
        }
      }
      throw new ArgumentException(EventSourceSR.Event_IllegalField((object) staticField.FieldType.Name, (object) providerEnumKind));
    }

    private static void AddEventDescriptor(
      ref EventSource.EventMetadata[] eventData,
      EventAttribute eventAttribute,
      ParameterInfo[] eventParameters)
    {
      if (eventData == null || eventData.Length <= eventAttribute.EventId)
      {
        EventSource.EventMetadata[] destinationArray = new EventSource.EventMetadata[Math.Max(eventData.Length + 16, eventAttribute.EventId + 1)];
        Array.Copy((Array) eventData, (Array) destinationArray, eventData.Length);
        eventData = destinationArray;
      }
      eventData[eventAttribute.EventId].Descriptor = new EventDescriptor(eventAttribute.EventId, eventAttribute.Version, (byte) eventAttribute.Channel, (byte) eventAttribute.Level, (byte) eventAttribute.Opcode, (int) eventAttribute.Task, (long) eventAttribute.Keywords);
      eventData[eventAttribute.EventId].Parameters = eventParameters;
      eventData[eventAttribute.EventId].Message = eventAttribute.Message;
    }

    private static void TrimEventDescriptors(ref EventSource.EventMetadata[] eventData)
    {
      int length = eventData.Length;
      while (0 < length)
      {
        --length;
        if (eventData[length].Descriptor.EventId != 0)
          break;
      }
      if (eventData.Length - length <= 2)
        return;
      EventSource.EventMetadata[] destinationArray = new EventSource.EventMetadata[length + 1];
      Array.Copy((Array) eventData, (Array) destinationArray, destinationArray.Length);
      eventData = destinationArray;
    }

    internal void AddListener(EventListener listener)
    {
      lock (EventListener.EventListenersLock)
      {
        bool[] eventEnabled = (bool[]) null;
        if (this.m_eventData != null)
          eventEnabled = new bool[this.m_eventData.Length];
        this.m_Dispatchers = new EventDispatcher(this.m_Dispatchers, eventEnabled, listener);
        listener.OnEventSourceCreated(this);
      }
    }

    private static void DebugCheckEvent(
      ref Dictionary<string, string> eventsByName,
      EventSource.EventMetadata[] eventData,
      MethodInfo method,
      EventAttribute eventAttribute)
    {
      int helperCallFirstArg = EventSource.GetHelperCallFirstArg(method);
      string key1 = method.Name.Replace("EventWrite", string.Empty);
      if (helperCallFirstArg >= 0 && eventAttribute.EventId != helperCallFirstArg)
        throw new ArgumentException(EventSourceSR.Event_IllegalEventArg((object) key1, (object) eventAttribute.EventId, (object) helperCallFirstArg));
      if (eventAttribute.EventId < eventData.Length && eventData[eventAttribute.EventId].Descriptor.EventId != 0)
        throw new ArgumentException(EventSourceSR.Event_UsedEventID((object) key1, (object) eventAttribute.EventId));
      if (eventsByName == null)
        eventsByName = new Dictionary<string, string>();
      if (eventsByName.ContainsKey(key1))
        throw new ArgumentException(EventSourceSR.Event_UsedEventName((object) key1));
      string key2;
      eventsByName[key2] = key2 = key1;
    }

    private static int GetHelperCallFirstArg(MethodInfo method)
    {
      byte[] ilAsByteArray = method.GetMethodBody().GetILAsByteArray();
      int helperCallFirstArg = -1;
      for (int index1 = 0; index1 < ilAsByteArray.Length; ++index1)
      {
        switch (ilAsByteArray[index1])
        {
          case 0:
          case 1:
          case 2:
          case 3:
          case 4:
          case 5:
          case 6:
          case 7:
          case 8:
          case 9:
          case 10:
          case 11:
          case 12:
          case 13:
          case 20:
          case 37:
          case 103:
          case 104:
          case 105:
          case 106:
          case 109:
          case 110:
          case 162:
            continue;
          case 14:
          case 16:
            ++index1;
            continue;
          case 21:
          case 22:
          case 23:
          case 24:
          case 25:
          case 26:
          case 27:
          case 28:
          case 29:
          case 30:
            if (index1 > 0 && ilAsByteArray[index1 - 1] == (byte) 2)
            {
              helperCallFirstArg = (int) ilAsByteArray[index1] - 22;
              continue;
            }
            continue;
          case 31:
            if (index1 > 0 && ilAsByteArray[index1 - 1] == (byte) 2)
              helperCallFirstArg = (int) ilAsByteArray[index1 + 1];
            ++index1;
            continue;
          case 32:
            index1 += 4;
            continue;
          case 40:
            index1 += 4;
            if (helperCallFirstArg >= 0)
            {
              for (int index2 = index1 + 1; index2 < ilAsByteArray.Length; ++index2)
              {
                if (ilAsByteArray[index2] == (byte) 42)
                  return helperCallFirstArg;
                if (ilAsByteArray[index2] != (byte) 0)
                  break;
              }
            }
            helperCallFirstArg = -1;
            continue;
          case 44:
          case 45:
            helperCallFirstArg = -1;
            ++index1;
            continue;
          case 57:
          case 58:
            helperCallFirstArg = -1;
            index1 += 4;
            continue;
          case 140:
          case 141:
            index1 += 4;
            continue;
          case 254:
            ++index1;
            if (index1 >= ilAsByteArray.Length || ilAsByteArray[index1] >= (byte) 6)
              break;
            continue;
        }
        return -1;
      }
      return -1;
    }

    private class OverideEventProvider : EventProviderClone
    {
      private EventSource m_eventSource;

      public OverideEventProvider(EventSource eventSource) => this.m_eventSource = eventSource;

      protected override void OnControllerCommand(
        ControllerCommand command,
        IDictionary<string, string> arguments)
      {
        this.m_eventSource.SendCommand((EventListener) null, (EventCommand) command, this.IsEnabled(), this.Level, this.MatchAnyKeyword, arguments);
      }
    }

    protected internal struct EventData
    {
      public int Size { get; set; }

      public IntPtr DataPointer { get; set; }
    }

    internal struct EventMetadata
    {
      public EventDescriptor Descriptor;
      public bool EnabledForAnyListener;
      public bool EnabledForETW;
      public string Message;
      public ParameterInfo[] Parameters;
    }
  }
}
