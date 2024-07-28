// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.Etw.RawListener
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.Cloud.Metrics.Client.Metrics.Etw
{
  internal sealed class RawListener : IDisposable
  {
    private static readonly bool Is64BitProcess = sizeof (IntPtr) == 8;
    private readonly ulong[] traceHandles;
    private readonly NativeMethods.EventTraceLogfilew[] traceLogs;

    private RawListener(
      IEnumerable<string> sessionOrEtlFiles,
      NativeMethods.EventRecordCallback eventRecordCallback,
      NativeMethods.EventTraceBufferCallback eventBufferCallback,
      bool isFileTrace,
      bool useRawTimestamps = true)
    {
      List<string> stringList = sessionOrEtlFiles != null ? sessionOrEtlFiles.Where<string>((Func<string, bool>) (trace => !string.IsNullOrEmpty(trace))).ToList<string>() : throw new ArgumentNullException(nameof (sessionOrEtlFiles));
      if (stringList.Count < 1)
        throw new ArgumentException("At least one non-null, non-empty session or etl file name must be provided");
      if (eventRecordCallback == null && eventBufferCallback == null)
        throw new ArgumentException("At least one of the callbacks must be specified");
      int index = 0;
      this.traceHandles = new ulong[stringList.Count];
      this.traceLogs = new NativeMethods.EventTraceLogfilew[stringList.Count];
      foreach (string str in stringList)
      {
        this.traceLogs[index].EventCallback = eventRecordCallback;
        this.traceLogs[index].BufferCallback = eventBufferCallback;
        this.traceLogs[index].LogFileMode = (uint) (268435456 | (useRawTimestamps ? 4096 : 0));
        if (isFileTrace)
        {
          this.traceLogs[index].LogFileName = str;
        }
        else
        {
          this.traceLogs[index].LoggerName = str;
          this.traceLogs[index].LogFileMode |= 256U;
        }
        this.traceLogs[index].LogFileMode |= 134217728U;
        ulong num = NativeMethods.OpenTrace(ref this.traceLogs[index]);
        this.traceHandles[index++] = (RawListener.Is64BitProcess || num != (ulong) uint.MaxValue) && (!RawListener.Is64BitProcess || num != ulong.MaxValue) ? num : throw new Win32Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "OpenTrace call for trace '{0}' failed.", new object[1]
        {
          (object) str
        }));
      }
    }

    public static RawListener CreateRealTimeListener(
      string sessionName,
      NativeMethods.EventRecordCallback eventRecordCallback,
      NativeMethods.EventTraceBufferCallback eventBufferCallback,
      bool useRawTimestamps = true)
    {
      if (string.IsNullOrEmpty(sessionName))
        throw new ArgumentException("Session name cannot be null or empty", nameof (sessionName));
      return new RawListener((IEnumerable<string>) new string[1]
      {
        sessionName
      }, eventRecordCallback, eventBufferCallback, false, useRawTimestamps);
    }

    public static RawListener CreateEtlFileListener(
      IEnumerable<string> etlFiles,
      NativeMethods.EventRecordCallback eventRecordCallback,
      NativeMethods.EventTraceBufferCallback eventBufferCallback,
      bool useRawTimestamps = true)
    {
      List<string> sessionOrEtlFiles = etlFiles != null ? etlFiles.Where<string>((Func<string, bool>) (etl => !string.IsNullOrEmpty(etl))).ToList<string>() : throw new ArgumentNullException(nameof (etlFiles));
      if (sessionOrEtlFiles.Count < 1)
        throw new ArgumentException("At least one non-null and non-empty etl file name must be provided");
      return new RawListener((IEnumerable<string>) sessionOrEtlFiles, eventRecordCallback, eventBufferCallback, true, useRawTimestamps);
    }

    public void Process()
    {
      int error = NativeMethods.ProcessTrace(this.traceHandles, (uint) this.traceHandles.Length, IntPtr.Zero, IntPtr.Zero);
      if (error != 0)
        throw new Win32Exception(error);
    }

    public void Dispose()
    {
      foreach (ulong traceHandle in this.traceHandles)
        NativeMethods.CloseTrace(traceHandle);
    }
  }
}
