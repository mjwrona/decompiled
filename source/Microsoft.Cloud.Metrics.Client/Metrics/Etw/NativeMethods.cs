// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.Etw.NativeMethods
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Cloud.Metrics.Client.Metrics.Etw
{
  internal static class NativeMethods
  {
    internal const uint ErrorSuccess = 0;
    internal const uint ErrorInsufficientBuffer = 122;
    internal const uint ErrorAlreadyExists = 183;
    internal const uint ErrorMoreData = 234;
    internal const uint ErrorWmiGuidNotFound = 4200;
    internal const uint ErrorWmiInstanceNotFound = 4201;
    internal const ulong InvalidHandleValue = 18446744073709551615;
    internal const ulong InvalidTracehandle64 = 18446744073709551615;
    internal const ulong InvalidTracehandle32 = 4294967295;
    internal const uint ProcessTraceModeRealTime = 256;
    internal const uint ProcessTraceModeRawTimestamp = 4096;
    internal const uint ProcessTraceModeEventRecord = 268435456;
    internal const uint EventTraceRealTimeMode = 256;
    internal const uint EventTraceFileModeSequential = 1;
    internal const uint EventTracePrivateLoggerMode = 2048;
    internal const uint EventTraceIndependentSessionMode = 134217728;
    internal const uint EventTraceControlQuery = 0;
    internal const uint EventTraceControlStop = 1;
    internal const uint EventTraceControlUpdate = 2;
    internal const uint EventControlCodeEnableProvider = 1;
    internal const uint WnodeFlagTracedGuid = 131072;

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    internal static extern int StartTrace(
      out ulong sessionHandle,
      [In] string sessionName,
      [In, Out] IntPtr properties);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    internal static extern int ControlTrace(
      [In] ulong sessionHandle,
      [In] string sessionName,
      [In, Out] IntPtr properties,
      [In] uint controlCode);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    internal static extern int EnableTraceEx2(
      [In] ulong traceHandle,
      [In] ref Guid providerGuid,
      [In] uint controlCode,
      [In] byte level,
      [In] ulong matchAnyKeyword,
      [In] ulong matchAllKeyword,
      [In] uint timeoutMilliseconds,
      [In, Optional] IntPtr enableParameters);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern void ZeroMemory(IntPtr handle, uint length);

    [DllImport("advapi32.dll", EntryPoint = "OpenTraceW", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern ulong OpenTrace([In, Out] ref NativeMethods.EventTraceLogfilew traceLog);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    internal static extern int ProcessTrace(
      [In] ulong[] handleArray,
      [In] uint handleCount,
      [In] IntPtr startTime,
      [In] IntPtr endTime);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    internal static extern int CloseTrace([In] ulong traceHandle);

    [DllImport("advapi32.dll")]
    internal static extern unsafe int EnumerateTraceGuidsEx(
      NativeMethods.TraceQueryInfoClass traceQueryInfoClass,
      void* inBuffer,
      int inBufferSize,
      void* outBuffer,
      int outBufferSize,
      ref int returnLength);

    [DllImport("advapi32.dll")]
    internal static extern unsafe int QueryAllTracesW(
      [In, Out] void* propertyArray,
      [In] uint propertyArrayCount,
      [In, Out] ref uint sessionCount);

    public delegate bool EventTraceBufferCallback([In] IntPtr eventTraceLog);

    public unsafe delegate void EventRecordCallback([In] NativeMethods.EventRecord* rawData);

    public enum EtwSessionClockType : uint
    {
      Default,
      PerformanceCounter,
      SystemTime,
      CpuTimestamp,
    }

    public enum TraceControl : uint
    {
      Query,
      Stop,
      Update,
      Flush,
    }

    internal enum TraceQueryInfoClass
    {
      TraceGuidQueryList,
      TraceGuidQueryInfo,
      TraceGuidQueryProcess,
      TraceStackTracingInfo,
      TraceSystemTraceEnableFlagsInfo,
      TraceSampledProfileIntervalInfo,
      TraceProfileSourceConfigInfo,
      TraceProfileSourceListInfo,
      TracePmcEventListInfo,
      TracePmcCounterListInfo,
      MaxTraceSetInfoClass,
    }

    public struct WnodeHeader
    {
      public uint BufferSize;
      public uint ProviderId;
      public ulong HistoricalContext;
      public ulong TimeStamp;
      public Guid Guid;
      public NativeMethods.EtwSessionClockType ClientContext;
      public uint Flags;
    }

    public struct EventTraceProperties
    {
      public NativeMethods.WnodeHeader Wnode;
      public uint BufferSize;
      public uint MinimumBuffers;
      public uint MaximumBuffers;
      public uint MaximumFileSize;
      public uint LogFileMode;
      public uint FlushTimer;
      public uint EnableFlags;
      public int AgeLimit;
      public uint NumberOfBuffers;
      public uint FreeBuffers;
      public uint EventsLost;
      public uint BuffersWritten;
      public uint LogBuffersLost;
      public uint RealTimeBuffersLost;
      public IntPtr LoggerThreadId;
      public uint LogFileNameOffset;
      public uint LoggerNameOffset;
    }

    public struct EtwBufferContext
    {
      public byte ProcessorNumber;
      public byte Alignment;
      public ushort LoggerId;
    }

    public struct EventHeader
    {
      public ushort Size;
      public ushort HeaderType;
      public ushort Flags;
      public ushort EventProperty;
      public int ThreadId;
      public int ProcessId;
      public long TimeStamp;
      public Guid ProviderId;
      public ushort Id;
      public byte Version;
      public byte Channel;
      public byte Level;
      public byte Opcode;
      public ushort Task;
      public ulong Keyword;
      public int KernelTime;
      public int UserTime;
      public Guid ActivityId;
    }

    public struct EventRecord
    {
      public NativeMethods.EventHeader EventHeader;
      public NativeMethods.EtwBufferContext BufferContext;
      public ushort ExtendedDataCount;
      public ushort UserDataLength;
      public IntPtr ExtendedData;
      public IntPtr UserData;
      public IntPtr UserContext;
    }

    public struct TraceEnableInfo
    {
      public uint IsEnabled;
      public byte Level;
      public byte Reserved1;
      public ushort LoggerId;
      public uint EnableProperty;
      public uint Reserved2;
      public long MatchAnyKeyword;
      public long MatchAllKeyword;
    }

    [StructLayout(LayoutKind.Sequential, Size = 172, CharSet = CharSet.Unicode)]
    internal struct TimeZoneInformation
    {
      internal uint bias;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      internal string standardName;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.U2)]
      internal ushort[] standardDate;
      internal uint standardBias;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      internal string daylightName;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.U2)]
      internal ushort[] daylightDate;
      internal uint daylightBias;
    }

    internal struct TraceLogfileHeader
    {
      internal uint BufferSize;
      internal uint Version;
      internal uint ProviderVersion;
      internal uint NumberOfProcessors;
      internal long EndTime;
      internal uint TimerResolution;
      internal uint MaximumFileSize;
      internal uint LogFileMode;
      internal uint BuffersWritten;
      internal uint StartBuffers;
      internal uint PointerSize;
      internal uint EventsLost;
      internal uint CpuSpeedInMHz;
      internal IntPtr LoggerName;
      internal IntPtr LogFileName;
      internal NativeMethods.TimeZoneInformation TimeZone;
      internal long BootTime;
      internal long PerfFreq;
      internal long StartTime;
      internal uint ReservedFlags;
      internal uint BuffersLost;
    }

    internal struct EventTraceHeader
    {
      internal ushort Size;
      internal ushort FieldTypeFlags;
      internal byte Type;
      internal byte Level;
      internal ushort Version;
      internal int ThreadId;
      internal int ProcessId;
      internal long TimeStamp;
      internal Guid Guid;
      internal int KernelTime;
      internal int UserTime;
    }

    internal struct EventTrace
    {
      internal NativeMethods.EventTraceHeader Header;
      internal uint InstanceId;
      internal uint ParentInstanceId;
      internal Guid ParentGuid;
      internal IntPtr MofData;
      internal int MofLength;
      internal NativeMethods.EtwBufferContext BufferContext;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct EventTraceLogfilew
    {
      [MarshalAs(UnmanagedType.LPWStr)]
      internal string LogFileName;
      [MarshalAs(UnmanagedType.LPWStr)]
      internal string LoggerName;
      internal long CurrentTime;
      internal uint BuffersRead;
      internal uint LogFileMode;
      internal NativeMethods.EventTrace CurrentEvent;
      internal NativeMethods.TraceLogfileHeader LogfileHeader;
      internal NativeMethods.EventTraceBufferCallback BufferCallback;
      internal int BufferSize;
      internal int Filled;
      internal int EventsLost;
      internal NativeMethods.EventRecordCallback EventCallback;
      internal int IsKernelTrace;
      internal IntPtr Context;
    }

    internal struct TraceGuidInfo
    {
      internal uint InstanceCount;
      internal uint Reserved;
    }

    internal struct TraceProviderInstanceInfo
    {
      internal uint NextOffset;
      internal uint EnableCount;
      internal uint Pid;
      internal uint Flags;
    }
  }
}
