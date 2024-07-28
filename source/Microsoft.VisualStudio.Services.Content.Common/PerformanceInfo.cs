// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.PerformanceInfo
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class PerformanceInfo
  {
    [DllImport("psapi.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetPerformanceInfo(
      out PerformanceInfo.PerformanceInformation PerformanceInformation,
      [In] int Size);

    public static bool TryGetPhysicalFreeMemoryPercentage(out int value)
    {
      PerformanceInfo.PerformanceInformation PerformanceInformation = new PerformanceInfo.PerformanceInformation();
      if (PerformanceInfo.GetPerformanceInfo(out PerformanceInformation, Marshal.SizeOf<PerformanceInfo.PerformanceInformation>(PerformanceInformation)))
      {
        long int64_1 = PerformanceInformation.PhysicalAvailable.ToInt64();
        long int64_2 = PerformanceInformation.PhysicalTotal.ToInt64();
        value = (int) (int64_1 * 100L / int64_2);
        return true;
      }
      value = 0;
      return false;
    }

    private struct PerformanceInformation
    {
      public int Size;
      public IntPtr CommitTotal;
      public IntPtr CommitLimit;
      public IntPtr CommitPeak;
      public IntPtr PhysicalTotal;
      public IntPtr PhysicalAvailable;
      public IntPtr SystemCache;
      public IntPtr KernelTotal;
      public IntPtr KernelPaged;
      public IntPtr KernelNonPaged;
      public IntPtr PageSize;
      public int HandlesCount;
      public int ProcessCount;
      public int ThreadCount;
    }
  }
}
