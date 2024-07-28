// Decompiled with JetBrains decompiler
// Type: Nest.NativeMethods
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.InteropServices;

namespace Nest
{
  internal static class NativeMethods
  {
    public static class Windows
    {
      [DllImport("ntdll")]
      private static extern int RtlGetVersion(
        out NativeMethods.Windows.RTL_OSVERSIONINFOEX lpVersionInformation);

      internal static string RtlGetVersion()
      {
        NativeMethods.Windows.RTL_OSVERSIONINFOEX lpVersionInformation = new NativeMethods.Windows.RTL_OSVERSIONINFOEX();
        lpVersionInformation.dwOSVersionInfoSize = (uint) Marshal.SizeOf<NativeMethods.Windows.RTL_OSVERSIONINFOEX>(lpVersionInformation);
        return NativeMethods.Windows.RtlGetVersion(out lpVersionInformation) == 0 ? string.Format("Microsoft Windows {0}.{1}.{2}", (object) lpVersionInformation.dwMajorVersion, (object) lpVersionInformation.dwMinorVersion, (object) lpVersionInformation.dwBuildNumber) : (string) null;
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
