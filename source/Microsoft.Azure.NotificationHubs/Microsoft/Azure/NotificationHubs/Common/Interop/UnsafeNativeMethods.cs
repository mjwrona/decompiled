// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Common.Interop
{
  [SuppressUnmanagedCodeSecurity]
  internal static class UnsafeNativeMethods
  {
    public const string KERNEL32 = "kernel32.dll";
    public const string ADVAPI32 = "advapi32.dll";
    public const string WS2_32 = "ws2_32.dll";
    public const int ERROR_SUCCESS = 0;
    public const int ERROR_INVALID_HANDLE = 6;
    public const int ERROR_OUTOFMEMORY = 14;
    public const int ERROR_MORE_DATA = 234;
    public const int ERROR_ARITHMETIC_OVERFLOW = 534;
    public const int ERROR_NOT_ENOUGH_MEMORY = 8;
    public const int ERROR_OPERATION_ABORTED = 995;
    public const int ERROR_IO_PENDING = 997;
    public const int ERROR_NO_SYSTEM_RESOURCES = 1450;
    public const int STATUS_PENDING = 259;
    public const int WSAACCESS = 10013;
    public const int WSAEMFILE = 10024;
    public const int WSAEMSGSIZE = 10040;
    public const int WSAEADDRINUSE = 10048;
    public const int WSAEADDRNOTAVAIL = 10049;
    public const int WSAENETDOWN = 10050;
    public const int WSAENETUNREACH = 10051;
    public const int WSAENETRESET = 10052;
    public const int WSAECONNABORTED = 10053;
    public const int WSAECONNRESET = 10054;
    public const int WSAENOBUFS = 10055;
    public const int WSAESHUTDOWN = 10058;
    public const int WSAETIMEDOUT = 10060;
    public const int WSAECONNREFUSED = 10061;
    public const int WSAEHOSTDOWN = 10064;
    public const int WSAEHOSTUNREACH = 10065;

    [SecurityCritical]
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    public static extern SafeWaitHandle CreateWaitableTimer(
      IntPtr mustBeZero,
      bool manualReset,
      string timerName);

    [SecurityCritical]
    [DllImport("kernel32.dll")]
    public static extern bool SetWaitableTimer(
      SafeWaitHandle handle,
      ref long dueTime,
      int period,
      IntPtr mustBeZero,
      IntPtr mustBeZeroAlso,
      bool resume);

    [SecurityCritical]
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern int QueryPerformanceCounter(out long time);

    [SecurityCritical]
    [DllImport("kernel32.dll")]
    public static extern uint GetSystemTimeAdjustment(
      out int adjustment,
      out uint increment,
      out uint adjustmentDisabled);

    [SecurityCritical]
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern void GetSystemTimeAsFileTime(out long time);

    [DllImport("ws2_32.dll", SetLastError = true)]
    internal static extern unsafe int WSARecv(
      IntPtr handle,
      UnsafeNativeMethods.WSABuffer* buffers,
      int bufferCount,
      out int bytesTransferred,
      ref int socketFlags,
      NativeOverlapped* nativeOverlapped,
      IntPtr completionRoutine);

    [DllImport("ws2_32.dll", SetLastError = true)]
    internal static extern unsafe bool WSAGetOverlappedResult(
      IntPtr socketHandle,
      NativeOverlapped* overlapped,
      out int bytesTransferred,
      bool wait,
      out uint flags);

    [SecurityCritical]
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetComputerNameEx(
      [In] ComputerNameFormat nameType,
      [MarshalAs(UnmanagedType.LPTStr), In, Out] StringBuilder lpBuffer,
      [In, Out] ref int size);

    [SecurityCritical]
    internal static string GetComputerName(ComputerNameFormat nameType)
    {
      int size = 0;
      if (!UnsafeNativeMethods.GetComputerNameEx(nameType, (StringBuilder) null, ref size))
      {
        int lastWin32Error = Marshal.GetLastWin32Error();
        if (lastWin32Error != 234)
          throw Fx.Exception.AsError((Exception) new Win32Exception(lastWin32Error));
      }
      if (size < 0)
        Fx.AssertAndThrow("GetComputerName returned an invalid length: " + (object) size);
      StringBuilder lpBuffer = new StringBuilder(size);
      if (!UnsafeNativeMethods.GetComputerNameEx(nameType, lpBuffer, ref size))
        throw Fx.Exception.AsError((Exception) new Win32Exception(Marshal.GetLastWin32Error()));
      return lpBuffer.ToString();
    }

    [SecurityCritical]
    [DllImport("kernel32.dll")]
    internal static extern bool IsDebuggerPresent();

    [SecurityCritical]
    [DllImport("kernel32.dll")]
    internal static extern void DebugBreak();

    [SecurityCritical]
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    internal static extern void OutputDebugString(string lpOutputString);

    [SecurityCritical]
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    internal static extern unsafe uint EventRegister(
      [In] ref Guid providerId,
      [In] UnsafeNativeMethods.EtwEnableCallback enableCallback,
      [In] void* callbackContext,
      [In, Out] ref long registrationHandle);

    [SecurityCritical]
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    internal static extern uint EventUnregister([In] long registrationHandle);

    [SecurityCritical]
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    internal static extern unsafe uint EventWrite(
      [In] long registrationHandle,
      [In] ref System.Diagnostics.Eventing.EventDescriptor eventDescriptor,
      [In] uint userDataCount,
      [In] UnsafeNativeMethods.EventData* userData);

    [SecurityCritical]
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    internal static extern unsafe uint EventWriteTransfer(
      [In] long registrationHandle,
      [In] ref System.Diagnostics.Eventing.EventDescriptor eventDescriptor,
      [In] ref Guid activityId,
      [In] ref Guid relatedActivityId,
      [In] uint userDataCount,
      [In] UnsafeNativeMethods.EventData* userData);

    [SecurityCritical]
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    internal static extern unsafe uint EventWriteString(
      [In] long registrationHandle,
      [In] byte level,
      [In] long keywords,
      [In] char* message);

    [SecurityCritical]
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    internal static extern uint EventActivityIdControl([In] int ControlCode, [In, Out] ref Guid ActivityId);

    [SecurityCritical]
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool ReportEvent(
      SafeHandle hEventLog,
      ushort type,
      ushort category,
      uint eventID,
      byte[] userSID,
      ushort numStrings,
      uint dataLen,
      HandleRef strings,
      byte[] rawData);

    [SecurityCritical]
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern SafeEventLogWriteHandle RegisterEventSource(
      string uncServerName,
      string sourceName);

    internal static unsafe bool HasOverlappedIoCompleted(NativeOverlapped* overlapped) => overlapped->InternalLow != (IntPtr) 259;

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct EventData
    {
      [FieldOffset(0)]
      internal ulong DataPointer;
      [FieldOffset(8)]
      internal uint Size;
      [FieldOffset(12)]
      internal int Reserved;
    }

    [SecurityCritical]
    internal unsafe delegate void EtwEnableCallback(
      [In] ref Guid sourceId,
      [In] int isEnabled,
      [In] byte level,
      [In] long matchAnyKeywords,
      [In] long matchAllKeywords,
      [In] void* filterData,
      [In] void* callbackContext);

    internal struct WSABuffer
    {
      public int length;
      public IntPtr buffer;
    }
  }
}
