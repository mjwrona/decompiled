// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Utilities.Internal.MacNativeMethods
// Assembly: Microsoft.VisualStudio.Utilities.Internal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E02F1555-E063-4795-BC05-853CA7424F51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Utilities.Internal.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Utilities.Internal
{
  public static class MacNativeMethods
  {
    [DllImport("/usr/lib/libSystem.dylib")]
    private static extern int sysctlbyname(
      [MarshalAs(UnmanagedType.LPStr)] string property,
      IntPtr output,
      IntPtr oldLen,
      IntPtr newp,
      uint newlen);

    [DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
    private static extern int Gestalt(int selector, out int result);

    [DllImport("libc")]
    private static extern int uname(IntPtr buf);

    public static void GetSystemInfo(ref MacNativeMethods.SystemInfo info)
    {
      info.HardwareLogicalCpuSize = MacNativeMethods.SysctlValueAsInt("hw.logicalcpu_max");
      info.HardwareMachine = MacNativeMethods.SysctlValueAsString("hw.machine");
      info.HardwareMemorySize = MacNativeMethods.SysctlValueAsInt64("hw.memsize");
      info.HardwareModel = MacNativeMethods.SysctlValueAsString("hw.model");
      info.HardwarePhysicalCpuSize = MacNativeMethods.SysctlValueAsInt("hw.physicalcpu_max");
      info.MachineCpuBrandString = MacNativeMethods.SysctlValueAsString("machdep.cpu.brand_string");
    }

    public static void GetOSVersionInfo(ref MacNativeMethods.OSVersionInfo info)
    {
      info.OSVersion = MacNativeMethods.SysctlValueAsString("kern.osversion");
      info.MajorVersion = MacNativeMethods.Gestalt("sys1");
      info.MinorVersion = MacNativeMethods.Gestalt("sys2");
      info.BuildNumber = MacNativeMethods.Gestalt("sys3");
    }

    internal static bool IsRunningOnMac()
    {
      IntPtr num = IntPtr.Zero;
      try
      {
        num = Marshal.AllocHGlobal(8192);
        if (MacNativeMethods.uname(num) == 0)
        {
          if (Marshal.PtrToStringAnsi(num) == "Darwin")
            return true;
        }
      }
      catch
      {
      }
      finally
      {
        if (num != IntPtr.Zero)
          Marshal.FreeHGlobal(num);
      }
      return false;
    }

    private static int Gestalt(string selector)
    {
      int result;
      int num = MacNativeMethods.Gestalt((int) selector[3] | (int) selector[2] << 8 | (int) selector[1] << 16 | (int) selector[0] << 24, out result);
      if (num != 0)
        throw new Exception(string.Format("Error reading gestalt for selector '{0}': {1}", (object) selector, (object) num));
      return result;
    }

    private static int SysctlValueAsInt(string name)
    {
      IntPtr num = MacNativeMethods.SysctlGetValue(name);
      try
      {
        return Marshal.ReadInt32(num);
      }
      catch
      {
      }
      finally
      {
        if (num != IntPtr.Zero)
          Marshal.FreeHGlobal(num);
      }
      return 0;
    }

    private static long SysctlValueAsInt64(string name)
    {
      IntPtr num = MacNativeMethods.SysctlGetValue(name);
      try
      {
        return Marshal.ReadInt64(num);
      }
      catch
      {
      }
      finally
      {
        if (num != IntPtr.Zero)
          Marshal.FreeHGlobal(num);
      }
      return 0;
    }

    private static string SysctlValueAsString(string name)
    {
      IntPtr num = MacNativeMethods.SysctlGetValue(name);
      try
      {
        return Marshal.PtrToStringAnsi(num);
      }
      catch
      {
      }
      finally
      {
        if (num != IntPtr.Zero)
          Marshal.FreeHGlobal(num);
      }
      return string.Empty;
    }

    private static IntPtr SysctlGetValue(string name)
    {
      IntPtr num = Marshal.AllocHGlobal(4);
      Marshal.WriteInt32(num, 0);
      try
      {
        MacNativeMethods.sysctlbyname(name, IntPtr.Zero, num, IntPtr.Zero, 0U);
        int cb = Marshal.ReadInt32(num);
        IntPtr output = cb >= 1 ? Marshal.AllocHGlobal(cb) : throw new Exception("sysctl: unknown oid '" + name + "'");
        if (MacNativeMethods.sysctlbyname(name, output, num, IntPtr.Zero, 0U) != 0)
          throw new Exception("sysctlbyname failed for " + name);
        return output;
      }
      finally
      {
        Marshal.FreeHGlobal(num);
      }
    }

    public struct SystemInfo
    {
      public string HardwareMachine;
      public string HardwareModel;
      public long HardwareMemorySize;
      public int HardwarePhysicalCpuSize;
      public int HardwareLogicalCpuSize;
      public string MachineCpuBrandString;
    }

    public struct OSVersionInfo
    {
      public int MajorVersion;
      public int MinorVersion;
      public int BuildNumber;
      public string OSVersion;
    }
  }
}
