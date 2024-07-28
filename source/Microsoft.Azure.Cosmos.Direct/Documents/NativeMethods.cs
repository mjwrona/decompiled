// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.NativeMethods
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Documents
{
  internal static class NativeMethods
  {
    public static class Darwin
    {
      private const int CTL_KERN = 1;
      private const int KERN_OSRELEASE = 2;

      public static unsafe string GetKernelRelease()
      {
        int* name = stackalloc int[2];
        name[0] = 1;
        name[1] = 2;
        byte* numPtr = stackalloc byte[32];
        uint* oldlenp = stackalloc uint[1];
        oldlenp[0] = 32U;
        try
        {
          if (NativeMethods.Darwin.sysctl(name, 2U, numPtr, oldlenp, IntPtr.Zero, 0U) == 0)
          {
            if (oldlenp[0] < 32U)
              return Marshal.PtrToStringAnsi((IntPtr) (void*) numPtr, (int) oldlenp[0]);
          }
        }
        catch (Exception ex)
        {
          throw new PlatformNotSupportedException("Error reading Darwin Kernel Version", ex);
        }
        throw new PlatformNotSupportedException("Unknown error reading Darwin Kernel Version");
      }

      [DllImport("libc")]
      private static extern unsafe int sysctl(
        int* name,
        uint namelen,
        byte* oldp,
        uint* oldlenp,
        IntPtr newp,
        uint newlen);
    }

    public static class Unix
    {
      public static unsafe string GetUname()
      {
        byte* numPtr = stackalloc byte[2048];
        try
        {
          if (NativeMethods.Unix.uname((IntPtr) (void*) numPtr) == 0)
            return Marshal.PtrToStringAnsi((IntPtr) (void*) numPtr);
        }
        catch (Exception ex)
        {
          throw new PlatformNotSupportedException("Error reading Unix name", ex);
        }
        throw new PlatformNotSupportedException("Unknown error reading Unix name");
      }

      [DllImport("libc")]
      private static extern int uname(IntPtr utsname);
    }

    public static class Windows
    {
      [DllImport("ntdll")]
      private static extern int RtlGetVersion(
        out NativeMethods.Windows.RTL_OSVERSIONINFOEX lpVersionInformation);

      internal static string RtlGetVersion()
      {
        NativeMethods.Windows.RTL_OSVERSIONINFOEX lpVersionInformation = new NativeMethods.Windows.RTL_OSVERSIONINFOEX();
        lpVersionInformation.dwOSVersionInfoSize = (uint) Marshal.SizeOf<NativeMethods.Windows.RTL_OSVERSIONINFOEX>(lpVersionInformation);
        return NativeMethods.Windows.RtlGetVersion(out lpVersionInformation) == 0 ? string.Format("{0}.{1}.{2}", (object) lpVersionInformation.dwMajorVersion, (object) lpVersionInformation.dwMinorVersion, (object) lpVersionInformation.dwBuildNumber) : (string) null;
      }

      internal struct RTL_OSVERSIONINFOEX
      {
        internal uint dwOSVersionInfoSize;
        internal uint dwMajorVersion;
        internal uint dwMinorVersion;
        internal uint dwBuildNumber;
        internal uint dwPlatformId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal string szCSDVersion;
      }
    }
  }
}
