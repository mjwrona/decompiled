// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.WindowsSystemUtilizationReader
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System.Runtime.InteropServices;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class WindowsSystemUtilizationReader : SystemUtilizationReaderBase
  {
    private long lastIdleTime;
    private long lastKernelTime;
    private long lastUserTime;

    public WindowsSystemUtilizationReader()
    {
      this.lastIdleTime = 0L;
      this.lastKernelTime = 0L;
      this.lastUserTime = 0L;
    }

    protected override float GetSystemWideCpuUsageCore()
    {
      long idle;
      long kernel;
      long user;
      if (!WindowsSystemUtilizationReader.NativeMethods.GetSystemTimes(out idle, out kernel, out user))
        return float.NaN;
      long num1 = idle - this.lastIdleTime;
      long num2 = kernel - this.lastKernelTime;
      long num3 = user - this.lastUserTime;
      this.lastIdleTime = idle;
      this.lastUserTime = user;
      this.lastKernelTime = kernel;
      long num4 = num3 + num2;
      return num4 == 0L ? float.NaN : (float) (100L * (num3 + num2 - num1)) / (float) num4;
    }

    protected override long? GetSystemWideMemoryAvailabiltyCore()
    {
      WindowsSystemUtilizationReader.NativeMethods.MemoryInfo memInfo = new WindowsSystemUtilizationReader.NativeMethods.MemoryInfo();
      memInfo.dwLength = (uint) Marshal.SizeOf<WindowsSystemUtilizationReader.NativeMethods.MemoryInfo>(memInfo);
      WindowsSystemUtilizationReader.NativeMethods.GlobalMemoryStatusEx(out memInfo);
      return new long?((long) memInfo.ullAvailPhys / 1024L);
    }

    private static class NativeMethods
    {
      [DllImport("kernel32.dll", SetLastError = true)]
      internal static extern bool GetSystemTimes(out long idle, out long kernel, out long user);

      [DllImport("kernel32.dll", SetLastError = true)]
      internal static extern bool GlobalMemoryStatusEx(
        out WindowsSystemUtilizationReader.NativeMethods.MemoryInfo memInfo);

      internal struct MemoryInfo
      {
        internal uint dwLength;
        internal uint dwMemoryLoad;
        internal ulong ullTotalPhys;
        internal ulong ullAvailPhys;
        internal ulong ullTotalPageFile;
        internal ulong ullAvailPageFile;
        internal ulong ullTotalVirtual;
        internal ulong ullAvailVirtual;
        internal ulong ullAvailExtendedVirtual;
      }
    }
  }
}
