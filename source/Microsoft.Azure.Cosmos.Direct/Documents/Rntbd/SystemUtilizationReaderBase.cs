// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.SystemUtilizationReaderBase
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal abstract class SystemUtilizationReaderBase
  {
    private static readonly Lazy<SystemUtilizationReaderBase> singletonInstance = new Lazy<SystemUtilizationReaderBase>(new Func<SystemUtilizationReaderBase>(SystemUtilizationReaderBase.Create), LazyThreadSafetyMode.ExecutionAndPublication);
    private static SystemUtilizationReaderBase singletonOverride;

    public static SystemUtilizationReaderBase SingletonInstance
    {
      get
      {
        SystemUtilizationReaderBase singletonOverride;
        return (singletonOverride = SystemUtilizationReaderBase.singletonOverride) != null ? singletonOverride : SystemUtilizationReaderBase.singletonInstance.Value;
      }
    }

    internal static void ApplySingletonOverride(SystemUtilizationReaderBase readerOverride) => SystemUtilizationReaderBase.singletonOverride = readerOverride;

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Intentional catch-all-rethrow here t log exception")]
    public float GetSystemWideCpuUsage()
    {
      try
      {
        return this.GetSystemWideCpuUsageCore();
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceError("Reading the system-wide CPU usage failed. Exception: {0}", (object) ex);
        return float.NaN;
      }
    }

    public long? GetSystemWideMemoryAvailabilty()
    {
      try
      {
        return this.GetSystemWideMemoryAvailabiltyCore();
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceError("Reading the system-wide Memory availability failed. Exception: {0}", (object) ex);
        return new long?();
      }
    }

    private static SystemUtilizationReaderBase Create()
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        return (SystemUtilizationReaderBase) new WindowsSystemUtilizationReader();
      return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? (SystemUtilizationReaderBase) new LinuxSystemUtilizationReader() : (SystemUtilizationReaderBase) new UnsupportedSystemUtilizationReader();
    }

    protected abstract float GetSystemWideCpuUsageCore();

    protected abstract long? GetSystemWideMemoryAvailabiltyCore();
  }
}
