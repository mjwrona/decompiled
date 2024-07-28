// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.EventProviderClone
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
  internal class EventProviderClone : IDisposable
  {
    private EventProviderClone.ManifestEtw.EtwEnableCallback m_etwCallback;
    private long m_regHandle;
    private byte m_level;
    private long m_anyKeywordMask;
    private long m_allKeywordMask;
    private int m_enabled;
    private Guid m_providerId;
    private int m_disposed;
    [ThreadStatic]
    private static EventProviderClone.WriteEventErrorCode s_returnCode;
    private const int s_basicTypeAllocationBufferSize = 16;
    private const int s_etwMaxMumberArguments = 32;
    private const int s_etwAPIMaxStringCount = 16;
    private const int s_maxEventDataDescriptors = 128;
    private const int s_traceEventMaximumSize = 65482;
    private const int s_traceEventMaximumStringSize = 32724;
    internal const string ADVAPI32 = "advapi32.dll";

    [SecurityCritical]
    [PermissionSet(SecurityAction.Demand, Unrestricted = true)]
    protected EventProviderClone(Guid providerGuid)
    {
      this.m_providerId = providerGuid;
      this.Register(providerGuid);
    }

    internal EventProviderClone()
    {
    }

    [SecurityCritical]
    internal void Register(Guid providerGuid)
    {
      this.m_providerId = providerGuid;
      this.m_etwCallback = new EventProviderClone.ManifestEtw.EtwEnableCallback(this.EtwEnableCallBack);
      uint num = this.EventRegister(ref this.m_providerId, this.m_etwCallback);
      if (num != 0U)
        throw new InvalidOperationException(EventSourceSR.Event_FailedWithErrorCode((object) num));
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    [SecurityCritical]
    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposed == 1 || Interlocked.Exchange(ref this.m_disposed, 1) != 0)
        return;
      this.m_enabled = 0;
      this.Deregister();
    }

    public virtual void Close() => this.Dispose();

    ~EventProviderClone() => this.Dispose(false);

    [SecurityCritical]
    private void Deregister()
    {
      if (this.m_regHandle == 0L)
        return;
      int num = (int) this.EventUnregister();
      this.m_regHandle = 0L;
    }

    [SecurityCritical]
    private unsafe void EtwEnableCallBack(
      [In] ref Guid sourceId,
      [In] int isEnabled,
      [In] byte setLevel,
      [In] long anyKeyword,
      [In] long allKeyword,
      [In] EventProviderClone.ManifestEtw.EVENT_FILTER_DESCRIPTOR* filterData,
      [In] void* callbackContext)
    {
      this.m_enabled = isEnabled;
      this.m_level = setLevel;
      this.m_anyKeywordMask = anyKeyword;
      this.m_allKeywordMask = allKeyword;
      ControllerCommand command = ControllerCommand.Update;
      IDictionary<string, string> arguments = (IDictionary<string, string>) null;
      byte[] data;
      int dataStart;
      if (this.GetDataFromController(filterData, out command, out data, out dataStart))
      {
        arguments = (IDictionary<string, string>) new Dictionary<string, string>(4);
        int num1;
        for (; dataStart < data.Length; dataStart = num1 + 1)
        {
          int num2 = EventProviderClone.FindNull(data, dataStart);
          int num3 = num2 + 1;
          num1 = EventProviderClone.FindNull(data, num3);
          if (num1 < data.Length)
          {
            string key = Encoding.UTF8.GetString(data, dataStart, num2 - dataStart);
            string str = Encoding.UTF8.GetString(data, num3, num1 - num3);
            arguments[key] = str;
          }
        }
      }
      this.OnControllerCommand(command, arguments);
    }

    protected virtual void OnControllerCommand(
      ControllerCommand command,
      IDictionary<string, string> arguments)
    {
    }

    protected EventLevel Level
    {
      get => (EventLevel) this.m_level;
      set => this.m_level = (byte) value;
    }

    protected EventKeywords MatchAnyKeyword
    {
      get => (EventKeywords) this.m_anyKeywordMask;
      set => this.m_anyKeywordMask = (long) value;
    }

    protected EventKeywords MatchAllKeyword
    {
      get => (EventKeywords) this.m_allKeywordMask;
      set => this.m_allKeywordMask = (long) value;
    }

    private static int FindNull(byte[] buffer, int idx)
    {
      while (idx < buffer.Length && buffer[idx] != (byte) 0)
        ++idx;
      return idx;
    }

    private unsafe bool GetDataFromController(
      EventProviderClone.ManifestEtw.EVENT_FILTER_DESCRIPTOR* filterData,
      out ControllerCommand command,
      out byte[] data,
      out int dataStart)
    {
      data = (byte[]) null;
      if ((IntPtr) filterData == IntPtr.Zero)
      {
        string str = "\\Microsoft\\Windows\\CurrentVersion\\Winevt\\Publishers\\{" + (object) this.m_providerId + "}";
        string keyName = Marshal.SizeOf(typeof (IntPtr)) != 8 ? "HKEY_LOCAL_MACHINE\\Software" + str : "HKEY_LOCAL_MACHINE\\Software\\Wow6432Node" + str;
        data = Registry.GetValue(keyName, "ControllerData", (object) null) as byte[];
        if (data != null && data.Length >= 4)
        {
          command = (ControllerCommand) (((((int) data[3] << 8) + (int) data[2] << 8) + (int) data[1] << 8) + (int) data[0]);
          dataStart = 4;
          return true;
        }
        dataStart = 0;
        command = ControllerCommand.Update;
        return false;
      }
      if (filterData->Ptr != 0L && 0 < filterData->Size && filterData->Size <= 1024)
      {
        data = new byte[filterData->Size];
        Marshal.Copy((IntPtr) filterData->Ptr, data, 0, data.Length);
      }
      command = (ControllerCommand) filterData->Type;
      dataStart = 0;
      return true;
    }

    public bool IsEnabled() => this.m_enabled != 0;

    public bool IsEnabled(byte level, long keywords) => this.m_enabled != 0 && ((int) level <= (int) this.m_level || this.m_level == (byte) 0) && (keywords == 0L || (keywords & this.m_anyKeywordMask) != 0L && (keywords & this.m_allKeywordMask) == this.m_allKeywordMask);

    public static EventProviderClone.WriteEventErrorCode GetLastWriteEventError() => EventProviderClone.s_returnCode;

    private static void SetLastError(int error)
    {
      switch (error)
      {
        case 8:
          EventProviderClone.s_returnCode = EventProviderClone.WriteEventErrorCode.NoFreeBuffers;
          break;
        case 234:
        case 534:
          EventProviderClone.s_returnCode = EventProviderClone.WriteEventErrorCode.EventTooBig;
          break;
      }
    }

    [SecurityCritical]
    private static unsafe string EncodeObject(
      ref object data,
      EventProviderClone.EventData* dataDescriptor,
      byte* dataBuffer)
    {
      while (true)
      {
        dataDescriptor->Reserved = 0U;
        if (!(data is string str))
        {
          if (!(data is IntPtr))
          {
            if (!(data is int))
            {
              if (!(data is long))
              {
                if (!(data is uint))
                {
                  if (!(data is ulong))
                  {
                    if (!(data is char))
                    {
                      if (!(data is byte))
                      {
                        if (!(data is short))
                        {
                          if (!(data is sbyte))
                          {
                            if (!(data is ushort))
                            {
                              if (!(data is float))
                              {
                                if (!(data is double))
                                {
                                  if (!(data is bool))
                                  {
                                    if (!(data is Guid))
                                    {
                                      if (!(data is Decimal))
                                      {
                                        if (!(data is bool))
                                        {
                                          if (data is Enum)
                                          {
                                            Type underlyingType = Enum.GetUnderlyingType(data.GetType());
                                            if (underlyingType == typeof (int))
                                            {
                                              ref object local = ref data;
                                              local = (object) ((IConvertible) local).ToInt32((IFormatProvider) null);
                                            }
                                            else if (underlyingType == typeof (long))
                                            {
                                              ref object local = ref data;
                                              local = (object) ((IConvertible) local).ToInt64((IFormatProvider) null);
                                            }
                                            else
                                              goto label_39;
                                          }
                                          else
                                            goto label_39;
                                        }
                                        else
                                          goto label_33;
                                      }
                                      else
                                        goto label_31;
                                    }
                                    else
                                      goto label_29;
                                  }
                                  else
                                    goto label_27;
                                }
                                else
                                  goto label_25;
                              }
                              else
                                goto label_23;
                            }
                            else
                              goto label_21;
                          }
                          else
                            goto label_19;
                        }
                        else
                          goto label_17;
                      }
                      else
                        goto label_15;
                    }
                    else
                      goto label_13;
                  }
                  else
                    goto label_11;
                }
                else
                  goto label_9;
              }
              else
                goto label_7;
            }
            else
              goto label_5;
          }
          else
            goto label_3;
        }
        else
          break;
      }
      dataDescriptor->Size = (uint) ((str.Length + 1) * 2);
      return str;
label_3:
      dataDescriptor->Size = (uint) sizeof (IntPtr);
      IntPtr* numPtr1 = (IntPtr*) dataBuffer;
      *numPtr1 = (IntPtr) data;
      dataDescriptor->Ptr = (ulong) numPtr1;
      goto label_40;
label_5:
      dataDescriptor->Size = 4U;
      int* numPtr2 = (int*) dataBuffer;
      *numPtr2 = (int) data;
      dataDescriptor->Ptr = (ulong) numPtr2;
      goto label_40;
label_7:
      dataDescriptor->Size = 8U;
      long* numPtr3 = (long*) dataBuffer;
      *numPtr3 = (long) data;
      dataDescriptor->Ptr = (ulong) numPtr3;
      goto label_40;
label_9:
      dataDescriptor->Size = 4U;
      uint* numPtr4 = (uint*) dataBuffer;
      *numPtr4 = (uint) data;
      dataDescriptor->Ptr = (ulong) numPtr4;
      goto label_40;
label_11:
      dataDescriptor->Size = 8U;
      ulong* numPtr5 = (ulong*) dataBuffer;
      *numPtr5 = (ulong) data;
      dataDescriptor->Ptr = (ulong) numPtr5;
      goto label_40;
label_13:
      dataDescriptor->Size = 2U;
      char* chPtr = (char*) dataBuffer;
      *chPtr = (char) data;
      dataDescriptor->Ptr = (ulong) chPtr;
      goto label_40;
label_15:
      dataDescriptor->Size = 1U;
      byte* numPtr6 = dataBuffer;
      *numPtr6 = (byte) data;
      dataDescriptor->Ptr = (ulong) numPtr6;
      goto label_40;
label_17:
      dataDescriptor->Size = 2U;
      short* numPtr7 = (short*) dataBuffer;
      *numPtr7 = (short) data;
      dataDescriptor->Ptr = (ulong) numPtr7;
      goto label_40;
label_19:
      dataDescriptor->Size = 1U;
      sbyte* numPtr8 = (sbyte*) dataBuffer;
      *numPtr8 = (sbyte) data;
      dataDescriptor->Ptr = (ulong) numPtr8;
      goto label_40;
label_21:
      dataDescriptor->Size = 2U;
      ushort* numPtr9 = (ushort*) dataBuffer;
      *numPtr9 = (ushort) data;
      dataDescriptor->Ptr = (ulong) numPtr9;
      goto label_40;
label_23:
      dataDescriptor->Size = 4U;
      float* numPtr10 = (float*) dataBuffer;
      *numPtr10 = (float) data;
      dataDescriptor->Ptr = (ulong) numPtr10;
      goto label_40;
label_25:
      dataDescriptor->Size = 8U;
      double* numPtr11 = (double*) dataBuffer;
      *numPtr11 = (double) data;
      dataDescriptor->Ptr = (ulong) numPtr11;
      goto label_40;
label_27:
      dataDescriptor->Size = 4U;
      int* numPtr12 = (int*) dataBuffer;
      *numPtr12 = !(bool) data ? 0 : 1;
      dataDescriptor->Ptr = (ulong) numPtr12;
      goto label_40;
label_29:
      dataDescriptor->Size = (uint) sizeof (Guid);
      Guid* guidPtr = (Guid*) dataBuffer;
      *guidPtr = (Guid) data;
      dataDescriptor->Ptr = (ulong) guidPtr;
      goto label_40;
label_31:
      dataDescriptor->Size = 16U;
      Decimal* numPtr13 = (Decimal*) dataBuffer;
      *numPtr13 = (Decimal) data;
      dataDescriptor->Ptr = (ulong) numPtr13;
      goto label_40;
label_33:
      dataDescriptor->Size = 1U;
      bool* flagPtr = (bool*) dataBuffer;
      *flagPtr = (bool) data;
      dataDescriptor->Ptr = (ulong) flagPtr;
      goto label_40;
label_39:
      string str1 = data != null ? data.ToString() : "";
      dataDescriptor->Size = (uint) ((str1.Length + 1) * 2);
      return str1;
label_40:
      return (string) null;
    }

    [SecurityCritical]
    public unsafe bool WriteMessageEvent(string eventMessage, byte eventLevel, long eventKeywords)
    {
      if (eventMessage == null)
        throw new ArgumentNullException(nameof (eventMessage));
      if (this.IsEnabled(eventLevel, eventKeywords))
      {
        if (eventMessage.Length > 32724)
        {
          EventProviderClone.s_returnCode = EventProviderClone.WriteEventErrorCode.EventTooBig;
          return false;
        }
        int error;
        fixed (char* message = eventMessage)
          error = (int) this.EventWriteString(eventLevel, eventKeywords, message);
        if (error != 0)
        {
          EventProviderClone.SetLastError(error);
          return false;
        }
      }
      return true;
    }

    public bool WriteMessageEvent(string eventMessage) => this.WriteMessageEvent(eventMessage, (byte) 0, 0L);

    [SecurityCritical]
    public unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, params object[] eventPayload)
    {
      uint error = 0;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        int userDataCount = 0;
        if (eventPayload == null || eventPayload.Length == 0 || eventPayload.Length == 1)
        {
          string str = (string) null;
          byte* dataBuffer = stackalloc byte[16];
          EventProviderClone.EventData eventData;
          eventData.Size = 0U;
          if (eventPayload != null && eventPayload.Length != 0)
          {
            str = EventProviderClone.EncodeObject(ref eventPayload[0], &eventData, dataBuffer);
            userDataCount = 1;
          }
          if (eventData.Size > 65482U)
          {
            EventProviderClone.s_returnCode = EventProviderClone.WriteEventErrorCode.EventTooBig;
            return false;
          }
          if (str != null)
          {
            fixed (char* chPtr = str)
            {
              eventData.Ptr = (ulong) chPtr;
              error = this.EventWrite(ref eventDescriptor, (uint) userDataCount, &eventData);
            }
          }
          else
            error = userDataCount != 0 ? this.EventWrite(ref eventDescriptor, (uint) userDataCount, &eventData) : this.EventWrite(ref eventDescriptor, 0U, (EventProviderClone.EventData*) null);
        }
        else
        {
          int length = eventPayload.Length;
          if (length > 32)
            throw new ArgumentOutOfRangeException(nameof (eventPayload), EventSourceSR.ArgumentOutOfRange_MaxArgExceeded((object) 32));
          uint num = 0;
          int index1 = 0;
          int[] numArray = new int[16];
          string[] strArray = new string[16];
          EventProviderClone.EventData* userData = stackalloc EventProviderClone.EventData[length];
          EventProviderClone.EventData* dataDescriptor = userData;
          byte* dataBuffer = stackalloc byte[16 * length];
          for (int index2 = 0; index2 < eventPayload.Length; ++index2)
          {
            if (eventPayload[index2] != null)
            {
              string str = EventProviderClone.EncodeObject(ref eventPayload[index2], dataDescriptor, dataBuffer);
              dataBuffer += 16;
              num += dataDescriptor->Size;
              ++dataDescriptor;
              if (str != null)
              {
                if (index1 >= 16)
                  throw new ArgumentOutOfRangeException(nameof (eventPayload), EventSourceSR.ArgumentOutOfRange_MaxStringsExceeded((object) 16));
                strArray[index1] = str;
                numArray[index1] = index2;
                ++index1;
              }
            }
          }
          if (num > 65482U)
          {
            EventProviderClone.s_returnCode = EventProviderClone.WriteEventErrorCode.EventTooBig;
            return false;
          }
          fixed (char* chPtr1 = strArray[0])
            fixed (char* chPtr2 = strArray[1])
              fixed (char* chPtr3 = strArray[2])
                fixed (char* chPtr4 = strArray[3])
                  fixed (char* chPtr5 = strArray[4])
                    fixed (char* chPtr6 = strArray[5])
                      fixed (char* chPtr7 = strArray[6])
                        fixed (char* chPtr8 = strArray[7])
                          fixed (char* chPtr9 = strArray[8])
                            fixed (char* chPtr10 = strArray[9])
                              fixed (char* chPtr11 = strArray[10])
                                fixed (char* chPtr12 = strArray[11])
                                  fixed (char* chPtr13 = strArray[12])
                                    fixed (char* chPtr14 = strArray[13])
                                      fixed (char* chPtr15 = strArray[14])
                                        fixed (char* chPtr16 = strArray[15])
                                        {
                                          EventProviderClone.EventData* eventDataPtr = userData;
                                          if (strArray[0] != null)
                                            eventDataPtr[numArray[0]].Ptr = (ulong) chPtr1;
                                          if (strArray[1] != null)
                                            eventDataPtr[numArray[1]].Ptr = (ulong) chPtr2;
                                          if (strArray[2] != null)
                                            eventDataPtr[numArray[2]].Ptr = (ulong) chPtr3;
                                          if (strArray[3] != null)
                                            eventDataPtr[numArray[3]].Ptr = (ulong) chPtr4;
                                          if (strArray[4] != null)
                                            eventDataPtr[numArray[4]].Ptr = (ulong) chPtr5;
                                          if (strArray[5] != null)
                                            eventDataPtr[numArray[5]].Ptr = (ulong) chPtr6;
                                          if (strArray[6] != null)
                                            eventDataPtr[numArray[6]].Ptr = (ulong) chPtr7;
                                          if (strArray[7] != null)
                                            eventDataPtr[numArray[7]].Ptr = (ulong) chPtr8;
                                          if (strArray[8] != null)
                                            eventDataPtr[numArray[8]].Ptr = (ulong) chPtr9;
                                          if (strArray[9] != null)
                                            eventDataPtr[numArray[9]].Ptr = (ulong) chPtr10;
                                          if (strArray[10] != null)
                                            eventDataPtr[numArray[10]].Ptr = (ulong) chPtr11;
                                          if (strArray[11] != null)
                                            eventDataPtr[numArray[11]].Ptr = (ulong) chPtr12;
                                          if (strArray[12] != null)
                                            eventDataPtr[numArray[12]].Ptr = (ulong) chPtr13;
                                          if (strArray[13] != null)
                                            eventDataPtr[numArray[13]].Ptr = (ulong) chPtr14;
                                          if (strArray[14] != null)
                                            eventDataPtr[numArray[14]].Ptr = (ulong) chPtr15;
                                          if (strArray[15] != null)
                                            eventDataPtr[numArray[15]].Ptr = (ulong) chPtr16;
                                          error = this.EventWrite(ref eventDescriptor, (uint) length, userData);
                                        }
        }
      }
      if (error == 0U)
        return true;
      EventProviderClone.SetLastError((int) error);
      return false;
    }

    [SecurityCritical]
    public unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, string data)
    {
      uint error = 0;
      if (data == null)
        throw new ArgumentNullException("dataString");
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        if (data.Length > 32724)
        {
          EventProviderClone.s_returnCode = EventProviderClone.WriteEventErrorCode.EventTooBig;
          return false;
        }
        EventProviderClone.EventData eventData;
        eventData.Size = (uint) ((data.Length + 1) * 2);
        eventData.Reserved = 0U;
        fixed (char* chPtr = data)
        {
          eventData.Ptr = (ulong) chPtr;
          error = this.EventWrite(ref eventDescriptor, 1U, &eventData);
        }
      }
      if (error == 0U)
        return true;
      EventProviderClone.SetLastError((int) error);
      return false;
    }

    [SecurityCritical]
    protected internal unsafe bool WriteEvent(
      ref EventDescriptor eventDescriptor,
      int dataCount,
      IntPtr data)
    {
      uint error = this.EventWrite(ref eventDescriptor, (uint) dataCount, (EventProviderClone.EventData*) (void*) data);
      if (error == 0U)
        return true;
      EventProviderClone.SetLastError((int) error);
      return false;
    }

    [SecurityCritical]
    protected internal unsafe bool WriteTransfer(
      ref EventDescriptor eventDescriptor,
      ref Guid activityId,
      ref Guid relatedActivityId,
      params object[] eventPayload)
    {
      uint error = 0;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        int userDataCount = 0;
        if (eventPayload == null || eventPayload.Length == 0 || eventPayload.Length == 1)
        {
          string str = (string) null;
          byte* dataBuffer = stackalloc byte[16];
          EventProviderClone.EventData eventData;
          eventData.Size = 0U;
          if (eventPayload != null && eventPayload.Length != 0)
          {
            str = EventProviderClone.EncodeObject(ref eventPayload[0], &eventData, dataBuffer);
            userDataCount = 1;
          }
          if (eventData.Size > 65482U)
          {
            EventProviderClone.s_returnCode = EventProviderClone.WriteEventErrorCode.EventTooBig;
            return false;
          }
          if (str != null)
          {
            fixed (char* chPtr = str)
            {
              eventData.Ptr = (ulong) chPtr;
              error = this.EventWriteTransfer(ref eventDescriptor, ref activityId, ref relatedActivityId, (uint) userDataCount, &eventData);
            }
          }
          else
            error = userDataCount != 0 ? this.EventWriteTransfer(ref eventDescriptor, ref activityId, ref relatedActivityId, (uint) userDataCount, &eventData) : this.EventWriteTransfer(ref eventDescriptor, ref activityId, ref relatedActivityId, 0U, (EventProviderClone.EventData*) null);
        }
        else
        {
          int length = eventPayload.Length;
          if (length > 32)
            throw new ArgumentOutOfRangeException(nameof (eventPayload), EventSourceSR.ArgumentOutOfRange_MaxArgExceeded((object) 32));
          uint num = 0;
          int index1 = 0;
          int[] numArray = new int[16];
          string[] strArray = new string[16];
          EventProviderClone.EventData* userData = stackalloc EventProviderClone.EventData[length];
          EventProviderClone.EventData* dataDescriptor = userData;
          byte* dataBuffer = stackalloc byte[16 * length];
          for (int index2 = 0; index2 < eventPayload.Length; ++index2)
          {
            if (eventPayload[index2] != null)
            {
              string str = EventProviderClone.EncodeObject(ref eventPayload[index2], dataDescriptor, dataBuffer);
              dataBuffer += 16;
              num += dataDescriptor->Size;
              ++dataDescriptor;
              if (str != null)
              {
                if (index1 >= 16)
                  throw new ArgumentOutOfRangeException(nameof (eventPayload), EventSourceSR.ArgumentOutOfRange_MaxStringsExceeded((object) 16));
                strArray[index1] = str;
                numArray[index1] = index2;
                ++index1;
              }
            }
          }
          if (num > 65482U)
          {
            EventProviderClone.s_returnCode = EventProviderClone.WriteEventErrorCode.EventTooBig;
            return false;
          }
          fixed (char* chPtr1 = strArray[0])
            fixed (char* chPtr2 = strArray[1])
              fixed (char* chPtr3 = strArray[2])
                fixed (char* chPtr4 = strArray[3])
                  fixed (char* chPtr5 = strArray[4])
                    fixed (char* chPtr6 = strArray[5])
                      fixed (char* chPtr7 = strArray[6])
                        fixed (char* chPtr8 = strArray[7])
                        {
                          EventProviderClone.EventData* eventDataPtr = userData;
                          if (strArray[0] != null)
                            eventDataPtr[numArray[0]].Ptr = (ulong) chPtr1;
                          if (strArray[1] != null)
                            eventDataPtr[numArray[1]].Ptr = (ulong) chPtr2;
                          if (strArray[2] != null)
                            eventDataPtr[numArray[2]].Ptr = (ulong) chPtr3;
                          if (strArray[3] != null)
                            eventDataPtr[numArray[3]].Ptr = (ulong) chPtr4;
                          if (strArray[4] != null)
                            eventDataPtr[numArray[4]].Ptr = (ulong) chPtr5;
                          if (strArray[5] != null)
                            eventDataPtr[numArray[5]].Ptr = (ulong) chPtr6;
                          if (strArray[6] != null)
                            eventDataPtr[numArray[6]].Ptr = (ulong) chPtr7;
                          if (strArray[7] != null)
                            eventDataPtr[numArray[7]].Ptr = (ulong) chPtr8;
                          error = this.EventWriteTransfer(ref eventDescriptor, ref activityId, ref relatedActivityId, (uint) length, userData);
                        }
        }
      }
      if (error == 0U)
        return true;
      EventProviderClone.SetLastError((int) error);
      return false;
    }

    private unsafe uint EventRegister(
      ref Guid providerId,
      EventProviderClone.ManifestEtw.EtwEnableCallback enableCallback)
    {
      this.m_providerId = providerId;
      this.m_etwCallback = enableCallback;
      return EventProviderClone.ManifestEtw.EventRegister(ref providerId, enableCallback, (void*) null, ref this.m_regHandle);
    }

    private uint EventUnregister()
    {
      int num = (int) EventProviderClone.ManifestEtw.EventUnregister(this.m_regHandle);
      this.m_regHandle = 0L;
      return (uint) num;
    }

    private unsafe uint EventWrite(
      ref EventDescriptor eventDescriptor,
      uint userDataCount,
      EventProviderClone.EventData* userData)
    {
      return EventProviderClone.ManifestEtw.EventWrite(this.m_regHandle, ref eventDescriptor, userDataCount, userData);
    }

    private unsafe uint EventWriteTransfer(
      ref EventDescriptor eventDescriptor,
      ref Guid activityId,
      ref Guid relatedActivityId,
      uint userDataCount,
      EventProviderClone.EventData* userData)
    {
      return EventProviderClone.ManifestEtw.EventWriteTransfer(this.m_regHandle, ref eventDescriptor, ref activityId, ref relatedActivityId, userDataCount, userData);
    }

    private unsafe uint EventWriteString(byte level, long keywords, char* message) => EventProviderClone.ManifestEtw.EventWriteString(this.m_regHandle, level, keywords, message);

    [SecurityCritical]
    protected internal uint SetActivityId(ref Guid activityId) => EventProviderClone.ManifestEtw.EventActivityIdControl(2, ref activityId);

    [SuppressUnmanagedCodeSecurity]
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern void ZeroMemory(IntPtr handle, int length);

    internal struct EventData
    {
      internal ulong Ptr;
      internal uint Size;
      internal uint Reserved;
    }

    public enum WriteEventErrorCode
    {
      NoError,
      NoFreeBuffers,
      EventTooBig,
    }

    private enum ActivityControl : uint
    {
      EVENT_ACTIVITY_CTRL_GET_ID = 1,
      EVENT_ACTIVITY_CTRL_SET_ID = 2,
      EVENT_ACTIVITY_CTRL_CREATE_ID = 3,
      EVENT_ACTIVITY_CTRL_GET_SET_ID = 4,
      EVENT_ACTIVITY_CTRL_CREATE_SET_ID = 5,
    }

    [SuppressUnmanagedCodeSecurity]
    internal static class ManifestEtw
    {
      private static readonly bool IsVistaOrGreater = Environment.OSVersion.Version.Major >= 6;
      private const int WindowsVistaMajorNumber = 6;
      internal const int ERROR_ARITHMETIC_OVERFLOW = 534;
      internal const int ERROR_NOT_ENOUGH_MEMORY = 8;
      internal const int ERROR_MORE_DATA = 234;

      [DllImport("advapi32.dll", EntryPoint = "EventRegister", CharSet = CharSet.Unicode)]
      private static extern unsafe uint _EventRegister(
        [In] ref Guid providerId,
        [In] EventProviderClone.ManifestEtw.EtwEnableCallback enableCallback,
        [In] void* callbackContext,
        [In, Out] ref long registrationHandle);

      internal static unsafe uint EventRegister(
        ref Guid providerId,
        EventProviderClone.ManifestEtw.EtwEnableCallback enableCallback,
        void* callbackContext,
        ref long registrationHandle)
      {
        return EventProviderClone.ManifestEtw.IsVistaOrGreater ? EventProviderClone.ManifestEtw._EventRegister(ref providerId, enableCallback, callbackContext, ref registrationHandle) : 0U;
      }

      [DllImport("advapi32.dll", EntryPoint = "EventUnregister", CharSet = CharSet.Unicode)]
      private static extern uint _EventUnregister([In] long registrationHandle);

      internal static uint EventUnregister(long registrationHandle) => EventProviderClone.ManifestEtw.IsVistaOrGreater ? EventProviderClone.ManifestEtw._EventUnregister(registrationHandle) : 0U;

      [DllImport("advapi32.dll", EntryPoint = "EventWrite", CharSet = CharSet.Unicode)]
      private static extern unsafe uint _EventWrite(
        [In] long registrationHandle,
        [In] ref EventDescriptor eventDescriptor,
        [In] uint userDataCount,
        [In] EventProviderClone.EventData* userData);

      internal static unsafe uint EventWrite(
        [In] long registrationHandle,
        [In] ref EventDescriptor eventDescriptor,
        [In] uint userDataCount,
        [In] EventProviderClone.EventData* userData)
      {
        return EventProviderClone.ManifestEtw.IsVistaOrGreater ? EventProviderClone.ManifestEtw._EventWrite(registrationHandle, ref eventDescriptor, userDataCount, userData) : 0U;
      }

      [DllImport("advapi32.dll", EntryPoint = "EventWriteTransfer", CharSet = CharSet.Unicode)]
      private static extern unsafe uint _EventWriteTransfer(
        [In] long registrationHandle,
        [In] ref EventDescriptor eventDescriptor,
        [In] ref Guid activityId,
        [In] ref Guid relatedActivityId,
        [In] uint userDataCount,
        [In] EventProviderClone.EventData* userData);

      internal static unsafe uint EventWriteTransfer(
        long registrationHandle,
        ref EventDescriptor eventDescriptor,
        ref Guid activityId,
        ref Guid relatedActivityId,
        uint userDataCount,
        EventProviderClone.EventData* userData)
      {
        return EventProviderClone.ManifestEtw.IsVistaOrGreater ? EventProviderClone.ManifestEtw._EventWriteTransfer(registrationHandle, ref eventDescriptor, ref activityId, ref relatedActivityId, userDataCount, userData) : 0U;
      }

      [DllImport("advapi32.dll", EntryPoint = "EventWriteString", CharSet = CharSet.Unicode)]
      private static extern unsafe uint _EventWriteString(
        [In] long registrationHandle,
        [In] byte level,
        [In] long keywords,
        [In] char* message);

      internal static unsafe uint EventWriteString(
        [In] long registrationHandle,
        [In] byte level,
        [In] long keywords,
        [In] char* message)
      {
        return EventProviderClone.ManifestEtw.IsVistaOrGreater ? EventProviderClone.ManifestEtw._EventWriteString(registrationHandle, level, keywords, message) : 0U;
      }

      [DllImport("advapi32.dll", EntryPoint = "EventActivityIdControl", CharSet = CharSet.Unicode)]
      private static extern uint _EventActivityIdControl([In] int ControlCode, [In, Out] ref Guid ActivityId);

      internal static uint EventActivityIdControl(int ControlCode, ref Guid ActivityId) => EventProviderClone.ManifestEtw.IsVistaOrGreater ? EventProviderClone.ManifestEtw._EventActivityIdControl(ControlCode, ref ActivityId) : 0U;

      internal unsafe delegate void EtwEnableCallback(
        [In] ref Guid sourceId,
        [In] int isEnabled,
        [In] byte level,
        [In] long matchAnyKeywords,
        [In] long matchAllKeywords,
        [In] EventProviderClone.ManifestEtw.EVENT_FILTER_DESCRIPTOR* filterData,
        [In] void* callbackContext);

      internal struct EVENT_FILTER_DESCRIPTOR
      {
        public long Ptr;
        public int Size;
        public int Type;
      }
    }
  }
}
