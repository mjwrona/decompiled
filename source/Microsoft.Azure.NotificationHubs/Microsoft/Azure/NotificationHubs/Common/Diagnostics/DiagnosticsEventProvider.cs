// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Diagnostics.DiagnosticsEventProvider
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Diagnostics;
using System.Diagnostics.Eventing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Common.Diagnostics
{
  [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
  internal class DiagnosticsEventProvider : IDisposable
  {
    [SecurityCritical]
    private Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EtwEnableCallback etwCallback;
    private long traceRegistrationHandle;
    private byte currentTraceLevel;
    private long anyKeywordMask;
    private long allKeywordMask;
    private bool isProviderEnabled;
    private Guid providerId;
    private int isDisposed;
    [ThreadStatic]
    private static DiagnosticsEventProvider.WriteEventErrorCode errorCode;
    private const int basicTypeAllocationBufferSize = 16;
    private const int etwMaxNumberArguments = 32;
    private const int etwAPIMaxStringCount = 8;
    private const int maxEventDataDescriptors = 128;
    private const int traceEventMaximumSize = 65482;
    private const int traceEventMaximumStringSize = 32724;
    private const int WindowsVistaMajorNumber = 6;

    [SecurityCritical]
    [PermissionSet(SecurityAction.Demand, Unrestricted = true)]
    protected DiagnosticsEventProvider(Guid providerGuid)
    {
      this.providerId = providerGuid;
      this.EtwRegister();
    }

    [SecurityCritical]
    private unsafe void EtwRegister()
    {
      this.etwCallback = new Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EtwEnableCallback(this.EtwEnableCallBack);
      uint num = Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventRegister(ref this.providerId, this.etwCallback, (void*) null, ref this.traceRegistrationHandle);
      if (num != 0U)
        throw new InvalidOperationException(SRCore.EtwRegistrationFailed((object) num.ToString("x", (IFormatProvider) CultureInfo.CurrentCulture)));
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.isDisposed == 1 || Interlocked.Exchange(ref this.isDisposed, 1) != 0)
        return;
      this.isProviderEnabled = false;
      this.Deregister();
    }

    public virtual void Close() => this.Dispose();

    ~DiagnosticsEventProvider() => this.Dispose(false);

    [SecurityCritical]
    private void Deregister()
    {
      if (this.traceRegistrationHandle == 0L)
        return;
      int num = (int) Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventUnregister(this.traceRegistrationHandle);
      this.traceRegistrationHandle = 0L;
    }

    [SecurityCritical]
    private unsafe void EtwEnableCallBack(
      [In] ref Guid sourceId,
      [In] int isEnabled,
      [In] byte setLevel,
      [In] long anyKeyword,
      [In] long allKeyword,
      [In] void* filterData,
      [In] void* callbackContext)
    {
      this.isProviderEnabled = isEnabled != 0;
      this.currentTraceLevel = setLevel;
      this.anyKeywordMask = anyKeyword;
      this.allKeywordMask = allKeyword;
      this.OnControllerCommand();
    }

    protected virtual void OnControllerCommand()
    {
    }

    public bool IsEnabled() => this.isProviderEnabled;

    public bool IsEnabled(byte level, long keywords) => this.isProviderEnabled && ((int) level <= (int) this.currentTraceLevel || this.currentTraceLevel == (byte) 0) && (keywords == 0L || (keywords & this.anyKeywordMask) != 0L && (keywords & this.allKeywordMask) == this.allKeywordMask);

    public static DiagnosticsEventProvider.WriteEventErrorCode GetLastWriteEventError() => DiagnosticsEventProvider.errorCode;

    private static void SetLastError(int error)
    {
      switch (error)
      {
        case 8:
          DiagnosticsEventProvider.errorCode = DiagnosticsEventProvider.WriteEventErrorCode.NoFreeBuffers;
          break;
        case 234:
        case 534:
          DiagnosticsEventProvider.errorCode = DiagnosticsEventProvider.WriteEventErrorCode.EventTooBig;
          break;
      }
    }

    [SecurityCritical]
    private static unsafe string EncodeObject(
      ref object data,
      Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventData* dataDescriptor,
      byte* dataBuffer)
    {
      dataDescriptor->Reserved = 0;
      if (data is string str1)
      {
        dataDescriptor->Size = (uint) ((str1.Length + 1) * 2);
        return str1;
      }
      if (data is IntPtr)
      {
        dataDescriptor->Size = (uint) sizeof (IntPtr);
        IntPtr* numPtr = (IntPtr*) dataBuffer;
        *numPtr = (IntPtr) data;
        dataDescriptor->DataPointer = (ulong) numPtr;
      }
      else if (data is int)
      {
        dataDescriptor->Size = 4U;
        int* numPtr = (int*) dataBuffer;
        *numPtr = (int) data;
        dataDescriptor->DataPointer = (ulong) numPtr;
      }
      else if (data is long)
      {
        dataDescriptor->Size = 8U;
        long* numPtr = (long*) dataBuffer;
        *numPtr = (long) data;
        dataDescriptor->DataPointer = (ulong) numPtr;
      }
      else if (data is uint)
      {
        dataDescriptor->Size = 4U;
        uint* numPtr = (uint*) dataBuffer;
        *numPtr = (uint) data;
        dataDescriptor->DataPointer = (ulong) numPtr;
      }
      else if (data is ulong)
      {
        dataDescriptor->Size = 8U;
        ulong* numPtr = (ulong*) dataBuffer;
        *numPtr = (ulong) data;
        dataDescriptor->DataPointer = (ulong) numPtr;
      }
      else if (data is char)
      {
        dataDescriptor->Size = 2U;
        char* chPtr = (char*) dataBuffer;
        *chPtr = (char) data;
        dataDescriptor->DataPointer = (ulong) chPtr;
      }
      else if (data is byte)
      {
        dataDescriptor->Size = 1U;
        byte* numPtr = dataBuffer;
        *numPtr = (byte) data;
        dataDescriptor->DataPointer = (ulong) numPtr;
      }
      else if (data is short)
      {
        dataDescriptor->Size = 2U;
        short* numPtr = (short*) dataBuffer;
        *numPtr = (short) data;
        dataDescriptor->DataPointer = (ulong) numPtr;
      }
      else if (data is sbyte)
      {
        dataDescriptor->Size = 1U;
        sbyte* numPtr = (sbyte*) dataBuffer;
        *numPtr = (sbyte) data;
        dataDescriptor->DataPointer = (ulong) numPtr;
      }
      else if (data is ushort)
      {
        dataDescriptor->Size = 2U;
        ushort* numPtr = (ushort*) dataBuffer;
        *numPtr = (ushort) data;
        dataDescriptor->DataPointer = (ulong) numPtr;
      }
      else if (data is float)
      {
        dataDescriptor->Size = 4U;
        float* numPtr = (float*) dataBuffer;
        *numPtr = (float) data;
        dataDescriptor->DataPointer = (ulong) numPtr;
      }
      else if (data is double)
      {
        dataDescriptor->Size = 8U;
        double* numPtr = (double*) dataBuffer;
        *numPtr = (double) data;
        dataDescriptor->DataPointer = (ulong) numPtr;
      }
      else if (data is bool)
      {
        dataDescriptor->Size = 1U;
        bool* flagPtr = (bool*) dataBuffer;
        *flagPtr = (bool) data;
        dataDescriptor->DataPointer = (ulong) flagPtr;
      }
      else if (data is Guid)
      {
        dataDescriptor->Size = (uint) sizeof (Guid);
        Guid* guidPtr = (Guid*) dataBuffer;
        *guidPtr = (Guid) data;
        dataDescriptor->DataPointer = (ulong) guidPtr;
      }
      else if (data is Decimal)
      {
        dataDescriptor->Size = 16U;
        Decimal* numPtr = (Decimal*) dataBuffer;
        *numPtr = (Decimal) data;
        dataDescriptor->DataPointer = (ulong) numPtr;
      }
      else if (data is bool)
      {
        dataDescriptor->Size = 1U;
        bool* flagPtr = (bool*) dataBuffer;
        *flagPtr = (bool) data;
        dataDescriptor->DataPointer = (ulong) flagPtr;
      }
      else
      {
        string str2 = data.ToString();
        dataDescriptor->Size = (uint) ((str2.Length + 1) * 2);
        return str2;
      }
      return (string) null;
    }

    [SecurityCritical]
    public unsafe bool WriteMessageEvent(string eventMessage, byte eventLevel, long eventKeywords)
    {
      if (eventMessage == null)
        throw Fx.Exception.AsError((Exception) new ArgumentNullException(nameof (eventMessage)));
      if (this.IsEnabled(eventLevel, eventKeywords))
      {
        if (eventMessage.Length > 32724)
        {
          DiagnosticsEventProvider.errorCode = DiagnosticsEventProvider.WriteEventErrorCode.EventTooBig;
          return false;
        }
        int error;
        fixed (char* message = eventMessage)
          error = (int) Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventWriteString(this.traceRegistrationHandle, eventLevel, eventKeywords, message);
        if (error != 0)
        {
          DiagnosticsEventProvider.SetLastError(error);
          return false;
        }
      }
      return true;
    }

    [SecurityCritical]
    public bool WriteMessageEvent(string eventMessage) => this.WriteMessageEvent(eventMessage, (byte) 0, 0L);

    [SecurityCritical]
    public unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, params object[] eventPayload)
    {
      uint error = 0;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        int userDataCount = 0;
        Guid activityId = DiagnosticsEventProvider.GetActivityId();
        DiagnosticsEventProvider.SetActivityId(ref activityId);
        if (eventPayload == null || eventPayload.Length == 0 || eventPayload.Length == 1)
        {
          string str = (string) null;
          byte* dataBuffer = stackalloc byte[16];
          Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventData eventData;
          eventData.Size = 0U;
          if (eventPayload != null && eventPayload.Length != 0)
          {
            str = DiagnosticsEventProvider.EncodeObject(ref eventPayload[0], &eventData, dataBuffer);
            userDataCount = 1;
          }
          if (eventData.Size > 65482U)
          {
            DiagnosticsEventProvider.errorCode = DiagnosticsEventProvider.WriteEventErrorCode.EventTooBig;
            return false;
          }
          if (str != null)
          {
            fixed (char* chPtr = str)
            {
              eventData.DataPointer = (ulong) chPtr;
              error = Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventWrite(this.traceRegistrationHandle, ref eventDescriptor, (uint) userDataCount, &eventData);
            }
          }
          else
            error = userDataCount != 0 ? Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventWrite(this.traceRegistrationHandle, ref eventDescriptor, (uint) userDataCount, &eventData) : Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventWrite(this.traceRegistrationHandle, ref eventDescriptor, 0U, (Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventData*) null);
        }
        else
        {
          int length = eventPayload.Length;
          if (length > 32)
            throw Fx.Exception.AsError((Exception) new ArgumentOutOfRangeException(nameof (eventPayload), SRCore.EtwMaxNumberArgumentsExceeded((object) 32)));
          uint num = 0;
          int index1 = 0;
          int[] numArray = new int[8];
          string[] strArray = new string[8];
          Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventData* userData = stackalloc Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventData[length];
          Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventData* dataDescriptor = userData;
          byte* dataBuffer = stackalloc byte[16 * length];
          for (int index2 = 0; index2 < eventPayload.Length; ++index2)
          {
            if (eventPayload[index2] != null)
            {
              string str = DiagnosticsEventProvider.EncodeObject(ref eventPayload[index2], dataDescriptor, dataBuffer);
              dataBuffer += 16;
              num += dataDescriptor->Size;
              ++dataDescriptor;
              if (str != null)
              {
                if (index1 >= 8)
                  throw Fx.Exception.AsError((Exception) new ArgumentOutOfRangeException(nameof (eventPayload), SRCore.EtwAPIMaxStringCountExceeded((object) 8)));
                strArray[index1] = str;
                numArray[index1] = index2;
                ++index1;
              }
            }
          }
          if (num > 65482U)
          {
            DiagnosticsEventProvider.errorCode = DiagnosticsEventProvider.WriteEventErrorCode.EventTooBig;
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
                          Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventData* eventDataPtr = userData;
                          if (strArray[0] != null)
                            eventDataPtr[numArray[0]].DataPointer = (ulong) chPtr1;
                          if (strArray[1] != null)
                            eventDataPtr[numArray[1]].DataPointer = (ulong) chPtr2;
                          if (strArray[2] != null)
                            eventDataPtr[numArray[2]].DataPointer = (ulong) chPtr3;
                          if (strArray[3] != null)
                            eventDataPtr[numArray[3]].DataPointer = (ulong) chPtr4;
                          if (strArray[4] != null)
                            eventDataPtr[numArray[4]].DataPointer = (ulong) chPtr5;
                          if (strArray[5] != null)
                            eventDataPtr[numArray[5]].DataPointer = (ulong) chPtr6;
                          if (strArray[6] != null)
                            eventDataPtr[numArray[6]].DataPointer = (ulong) chPtr7;
                          if (strArray[7] != null)
                            eventDataPtr[numArray[7]].DataPointer = (ulong) chPtr8;
                          error = Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventWrite(this.traceRegistrationHandle, ref eventDescriptor, (uint) length, userData);
                        }
        }
      }
      if (error == 0U)
        return true;
      DiagnosticsEventProvider.SetLastError((int) error);
      return false;
    }

    [SecurityCritical]
    public unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, string data)
    {
      uint error = 0;
      data = data ?? string.Empty;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        if (data.Length > 32724)
        {
          DiagnosticsEventProvider.errorCode = DiagnosticsEventProvider.WriteEventErrorCode.EventTooBig;
          return false;
        }
        Guid activityId = DiagnosticsEventProvider.GetActivityId();
        DiagnosticsEventProvider.SetActivityId(ref activityId);
        Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventData eventData;
        eventData.Size = (uint) ((data.Length + 1) * 2);
        eventData.Reserved = 0;
        fixed (char* chPtr = data)
        {
          eventData.DataPointer = (ulong) chPtr;
          error = Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventWrite(this.traceRegistrationHandle, ref eventDescriptor, 1U, &eventData);
        }
      }
      if (error == 0U)
        return true;
      DiagnosticsEventProvider.SetLastError((int) error);
      return false;
    }

    [SecurityCritical]
    protected internal unsafe bool WriteEvent(
      ref EventDescriptor eventDescriptor,
      int dataCount,
      IntPtr data)
    {
      Guid activityId = DiagnosticsEventProvider.GetActivityId();
      DiagnosticsEventProvider.SetActivityId(ref activityId);
      uint error = Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventWrite(this.traceRegistrationHandle, ref eventDescriptor, (uint) dataCount, (Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventData*) (void*) data);
      if (error == 0U)
        return true;
      DiagnosticsEventProvider.SetLastError((int) error);
      return false;
    }

    [SecurityCritical]
    public unsafe bool WriteTransferEvent(
      ref EventDescriptor eventDescriptor,
      Guid relatedActivityId,
      params object[] eventPayload)
    {
      uint error = 0;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        Guid activityId = DiagnosticsEventProvider.GetActivityId();
        if (eventPayload != null && eventPayload.Length != 0)
        {
          int length = eventPayload.Length;
          if (length > 32)
            throw Fx.Exception.AsError((Exception) new ArgumentOutOfRangeException(nameof (eventPayload), SRCore.EtwMaxNumberArgumentsExceeded((object) 32)));
          uint num = 0;
          int index1 = 0;
          int[] numArray = new int[8];
          string[] strArray = new string[8];
          Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventData* userData = stackalloc Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventData[length];
          Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventData* dataDescriptor = userData;
          byte* dataBuffer = stackalloc byte[16 * length];
          for (int index2 = 0; index2 < eventPayload.Length; ++index2)
          {
            if (eventPayload[index2] != null)
            {
              string str = DiagnosticsEventProvider.EncodeObject(ref eventPayload[index2], dataDescriptor, dataBuffer);
              dataBuffer += 16;
              num += dataDescriptor->Size;
              ++dataDescriptor;
              if (str != null)
              {
                if (index1 >= 8)
                  throw Fx.Exception.AsError((Exception) new ArgumentOutOfRangeException(nameof (eventPayload), SRCore.EtwAPIMaxStringCountExceeded((object) 8)));
                strArray[index1] = str;
                numArray[index1] = index2;
                ++index1;
              }
            }
          }
          if (num > 65482U)
          {
            DiagnosticsEventProvider.errorCode = DiagnosticsEventProvider.WriteEventErrorCode.EventTooBig;
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
                          Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventData* eventDataPtr = userData;
                          if (strArray[0] != null)
                            eventDataPtr[numArray[0]].DataPointer = (ulong) chPtr1;
                          if (strArray[1] != null)
                            eventDataPtr[numArray[1]].DataPointer = (ulong) chPtr2;
                          if (strArray[2] != null)
                            eventDataPtr[numArray[2]].DataPointer = (ulong) chPtr3;
                          if (strArray[3] != null)
                            eventDataPtr[numArray[3]].DataPointer = (ulong) chPtr4;
                          if (strArray[4] != null)
                            eventDataPtr[numArray[4]].DataPointer = (ulong) chPtr5;
                          if (strArray[5] != null)
                            eventDataPtr[numArray[5]].DataPointer = (ulong) chPtr6;
                          if (strArray[6] != null)
                            eventDataPtr[numArray[6]].DataPointer = (ulong) chPtr7;
                          if (strArray[7] != null)
                            eventDataPtr[numArray[7]].DataPointer = (ulong) chPtr8;
                          error = Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventWriteTransfer(this.traceRegistrationHandle, ref eventDescriptor, ref activityId, ref relatedActivityId, (uint) length, userData);
                        }
        }
        else
          error = Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventWriteTransfer(this.traceRegistrationHandle, ref eventDescriptor, ref activityId, ref relatedActivityId, 0U, (Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventData*) null);
      }
      if (error == 0U)
        return true;
      DiagnosticsEventProvider.SetLastError((int) error);
      return false;
    }

    [SecurityCritical]
    protected unsafe bool WriteTransferEvent(
      ref EventDescriptor eventDescriptor,
      Guid relatedActivityId,
      int dataCount,
      IntPtr data)
    {
      Guid activityId = DiagnosticsEventProvider.GetActivityId();
      uint error = Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventWriteTransfer(this.traceRegistrationHandle, ref eventDescriptor, ref activityId, ref relatedActivityId, (uint) dataCount, (Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventData*) (void*) data);
      if (error == 0U)
        return true;
      DiagnosticsEventProvider.SetLastError((int) error);
      return false;
    }

    [SecurityCritical]
    private static Guid GetActivityId()
    {
      object activityId = (object) Trace.CorrelationManager.ActivityId;
      return activityId != null ? (Guid) activityId : Guid.Empty;
    }

    [SecurityCritical]
    public static void SetActivityId(ref Guid id)
    {
      int num = (int) Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventActivityIdControl(2, ref id);
    }

    [SecurityCritical]
    public static Guid CreateActivityId()
    {
      Guid ActivityId = new Guid();
      int num = (int) Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.EventActivityIdControl(3, ref ActivityId);
      return ActivityId;
    }

    private enum ActivityControl : uint
    {
      EventActivityControlGetId = 1,
      EventActivityControlSetId = 2,
      EventActivityControlCreateId = 3,
      EventActivityControlGetSetId = 4,
      EventActivityControlCreateSetId = 5,
    }

    public enum WriteEventErrorCode
    {
      NoError,
      NoFreeBuffers,
      EventTooBig,
    }
  }
}
