// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.Etw.EtwSessionManager
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Cloud.Metrics.Client.Metrics.Etw
{
  internal static class EtwSessionManager
  {
    private const uint MaxNameSize = 1024;
    private const uint ExtSize = 4096;
    private const uint MaxSessionsByQueryAllTraces = 64;
    private static readonly uint TracePropertiesSize;
    private static IntPtr sharedBuffer;
    private static object sharedBufferLock = new object();
    private static int sharedBufferSize;
    private static uint expectedActiveEtwSessions = 32;

    static EtwSessionManager()
    {
      EtwSessionManager.TracePropertiesSize = (uint) Marshal.SizeOf(typeof (NativeMethods.EventTraceProperties));
      EtwSessionManager.sharedBufferSize = (int) EtwSessionManager.TracePropertiesSize + 4096;
      EtwSessionManager.sharedBuffer = Marshal.AllocHGlobal(EtwSessionManager.sharedBufferSize);
    }

    public static bool Stop(string sessionName)
    {
      int errorCode;
      EtwSessionManager.ControlTrace(sessionName, NativeMethods.TraceControl.Stop, out errorCode);
      return errorCode == 0 || errorCode == 4201;
    }

    public static bool TryGetSessionProperties(
      string sessionName,
      out NativeMethods.EventTraceProperties traceProperties)
    {
      int errorCode;
      traceProperties = EtwSessionManager.ControlTrace(sessionName, NativeMethods.TraceControl.Query, out errorCode);
      return errorCode == 0;
    }

    public static unsafe string[] GetSessionNames()
    {
      lock (EtwSessionManager.sharedBufferLock)
      {
        uint sessionCount = 0;
        bool flag = true;
        int error = 0;
        while (flag)
        {
          uint num = EtwSessionManager.expectedActiveEtwSessions * (EtwSessionManager.TracePropertiesSize + 4096U);
          if ((long) EtwSessionManager.sharedBufferSize < (long) num)
            EtwSessionManager.ReallocateSharedBufferSize((int) num);
          NativeMethods.ZeroMemory(EtwSessionManager.sharedBuffer, num);
          IntPtr[] numArray = new IntPtr[(int) EtwSessionManager.expectedActiveEtwSessions];
          for (int index = 0; (long) index < (long) EtwSessionManager.expectedActiveEtwSessions; ++index)
          {
            NativeMethods.EventTraceProperties* eventTracePropertiesPtr = (NativeMethods.EventTraceProperties*) ((IntPtr) (void*) EtwSessionManager.sharedBuffer + (IntPtr) ((long) index * (long) (EtwSessionManager.TracePropertiesSize + 4096U) * 2L));
            eventTracePropertiesPtr->LoggerNameOffset = EtwSessionManager.TracePropertiesSize;
            eventTracePropertiesPtr->LogFileNameOffset = EtwSessionManager.TracePropertiesSize + 2048U;
            eventTracePropertiesPtr->Wnode.BufferSize = EtwSessionManager.TracePropertiesSize + 4096U;
            numArray[index] = (IntPtr) (void*) eventTracePropertiesPtr;
          }
          fixed (IntPtr* propertyArray = numArray)
          {
            error = NativeMethods.QueryAllTracesW((void*) propertyArray, EtwSessionManager.expectedActiveEtwSessions, ref sessionCount);
            if ((error == 234 || (int) EtwSessionManager.expectedActiveEtwSessions == (int) sessionCount) && EtwSessionManager.expectedActiveEtwSessions < 64U)
            {
              EtwSessionManager.expectedActiveEtwSessions = EtwSessionManager.expectedActiveEtwSessions < sessionCount ? sessionCount + 1U : 2U * EtwSessionManager.expectedActiveEtwSessions;
              EtwSessionManager.expectedActiveEtwSessions = Math.Max(EtwSessionManager.expectedActiveEtwSessions, 64U);
            }
            else
              flag = false;
          }
        }
        if (error != 0)
          throw new Win32Exception(error, "Error calling QueryAllTracesW (0x" + error.ToString("X8") + ")");
        string[] sessionNames = new string[(int) sessionCount];
        for (int index = 0; (long) index < (long) sessionCount; ++index)
        {
          char* chPtr = (char*) ((IntPtr) (void*) EtwSessionManager.sharedBuffer + (IntPtr) (int) ((long) index * (long) (EtwSessionManager.TracePropertiesSize + 4096U)) * 2);
          sessionNames[index] = new string(chPtr + (int) ((NativeMethods.EventTraceProperties*) chPtr)->LoggerNameOffset);
        }
        return sessionNames;
      }
    }

    public static unsafe bool GetProviderInfo(
      ulong loggerId,
      Guid providerId,
      out NativeMethods.TraceEnableInfo enableInfo)
    {
      bool providerInfo = false;
      enableInfo = new NativeMethods.TraceEnableInfo();
      lock (EtwSessionManager.sharedBufferLock)
      {
        int returnLength = 0;
        void* outBuffer = (void*) null;
        bool flag = true;
        int error = 0;
        while (flag)
        {
          outBuffer = EtwSessionManager.sharedBuffer.ToPointer();
          error = NativeMethods.EnumerateTraceGuidsEx(NativeMethods.TraceQueryInfoClass.TraceGuidQueryInfo, (void*) &providerId, sizeof (Guid), outBuffer, EtwSessionManager.sharedBufferSize, ref returnLength);
          if (error != 122)
            flag = false;
          else
            EtwSessionManager.ReallocateSharedBufferSize(returnLength);
        }
        if (error == 4200)
          return false;
        if (error != 0)
          throw new Win32Exception(error, "Error calling EnumerateTraceGuidsEx (0x" + error.ToString("X8") + ")");
        NativeMethods.TraceGuidInfo traceGuidInfo = *(NativeMethods.TraceGuidInfo*) outBuffer;
        void* voidPtr = (void*) ((NativeMethods.TraceGuidInfo*) outBuffer + 1);
        for (int index1 = 0; (long) index1 < (long) traceGuidInfo.InstanceCount; ++index1)
        {
          if (!providerInfo)
          {
            NativeMethods.TraceProviderInstanceInfo providerInstanceInfo = *(NativeMethods.TraceProviderInstanceInfo*) voidPtr;
            if (providerInstanceInfo.EnableCount > 0U)
            {
              NativeMethods.TraceEnableInfo* traceEnableInfoPtr = (NativeMethods.TraceEnableInfo*) ((NativeMethods.TraceProviderInstanceInfo*) voidPtr + 1);
              for (int index2 = 0; (long) index2 < (long) providerInstanceInfo.EnableCount; ++index2)
              {
                if ((long) traceEnableInfoPtr->LoggerId == (long) loggerId)
                {
                  enableInfo = *traceEnableInfoPtr;
                  providerInfo = true;
                  break;
                }
                ++traceEnableInfoPtr;
              }
            }
            voidPtr += ((IntPtr) providerInstanceInfo.NextOffset).ToInt64();
          }
          else
            break;
        }
      }
      return providerInfo;
    }

    public static unsafe bool TryGetCurrentFileOfSession(
      string sessionName,
      out string currentSessionFile)
    {
      currentSessionFile = (string) null;
      lock (EtwSessionManager.sharedBufferLock)
      {
        NativeMethods.EventTraceProperties structure = new NativeMethods.EventTraceProperties()
        {
          LoggerNameOffset = EtwSessionManager.TracePropertiesSize,
          LogFileNameOffset = EtwSessionManager.TracePropertiesSize + 2048U,
          Wnode = new NativeMethods.WnodeHeader()
          {
            BufferSize = EtwSessionManager.TracePropertiesSize + 4096U
          }
        };
        NativeMethods.ZeroMemory(EtwSessionManager.sharedBuffer, structure.Wnode.BufferSize);
        Marshal.StructureToPtr((object) structure, EtwSessionManager.sharedBuffer, true);
        int num = NativeMethods.ControlTrace(0UL, sessionName, EtwSessionManager.sharedBuffer, 0U);
        NativeMethods.EventTraceProperties* sharedBuffer = (NativeMethods.EventTraceProperties*) (void*) EtwSessionManager.sharedBuffer;
        if (num == 0 && sharedBuffer->LogFileNameOffset > 0U)
          currentSessionFile = new string((char*) (sharedBuffer + (int) sharedBuffer->LogFileNameOffset));
        return num == 0 && currentSessionFile != null;
      }
    }

    private static NativeMethods.EventTraceProperties ControlTrace(
      string sessionName,
      NativeMethods.TraceControl traceControl,
      out int errorCode)
    {
      lock (EtwSessionManager.sharedBufferLock)
      {
        NativeMethods.EventTraceProperties eventTraceProperties = new NativeMethods.EventTraceProperties();
        eventTraceProperties.LoggerNameOffset = EtwSessionManager.TracePropertiesSize;
        eventTraceProperties.LogFileNameOffset = EtwSessionManager.TracePropertiesSize + 2048U;
        eventTraceProperties.Wnode = new NativeMethods.WnodeHeader()
        {
          BufferSize = EtwSessionManager.TracePropertiesSize + 4096U
        };
        NativeMethods.EventTraceProperties structure = eventTraceProperties;
        NativeMethods.ZeroMemory(EtwSessionManager.sharedBuffer, structure.Wnode.BufferSize);
        Marshal.StructureToPtr((object) structure, EtwSessionManager.sharedBuffer, true);
        errorCode = NativeMethods.ControlTrace(0UL, sessionName, EtwSessionManager.sharedBuffer, (uint) traceControl);
        eventTraceProperties = (NativeMethods.EventTraceProperties) Marshal.PtrToStructure(EtwSessionManager.sharedBuffer, typeof (NativeMethods.EventTraceProperties));
        return eventTraceProperties;
      }
    }

    private static void ReallocateSharedBufferSize(int newBufferSize)
    {
      lock (EtwSessionManager.sharedBufferLock)
      {
        int num = Math.Max(EtwSessionManager.sharedBufferSize, newBufferSize);
        Marshal.FreeHGlobal(EtwSessionManager.sharedBuffer);
        EtwSessionManager.sharedBufferSize = num;
        EtwSessionManager.sharedBuffer = Marshal.AllocHGlobal(EtwSessionManager.sharedBufferSize);
      }
    }
  }
}
