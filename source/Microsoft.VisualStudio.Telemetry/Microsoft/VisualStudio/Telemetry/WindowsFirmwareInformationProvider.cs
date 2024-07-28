// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsFirmwareInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VisualStudio.Telemetry
{
  internal static class WindowsFirmwareInformationProvider
  {
    internal static byte[] GetSystemFirmwareTable(
      NativeMethods.FirmwareTableProviderSignature provider,
      string table)
    {
      if (table.Length != 4 || table.Any<char>((Func<char, bool>) (c => c > '\u007F')))
        throw new ArgumentException("Table names must consist of 4 Ascii charactors", nameof (table));
      int table1 = (int) table[3] << 24 | (int) table[2] << 16 | (int) table[1] << 8 | (int) table[0];
      return WindowsFirmwareInformationProvider.GetSystemFirmwareTable(provider, table1);
    }

    internal static byte[] GetSystemFirmwareTable(
      NativeMethods.FirmwareTableProviderSignature provider,
      int table)
    {
      int systemFirmwareTable = NativeMethods.GetSystemFirmwareTable(provider, table, IntPtr.Zero, 0);
      IntPtr num = systemFirmwareTable > 0 ? Marshal.AllocHGlobal(systemFirmwareTable) : throw new ExternalException(string.Format("There was an error obtaining the firmware table '{0}' from {1}.", (object) table, (object) provider), Marshal.GetLastWin32Error());
      try
      {
        NativeMethods.GetSystemFirmwareTable(provider, table, num, systemFirmwareTable);
        byte[] destination = (byte[]) null;
        if (Marshal.GetLastWin32Error() == 0)
        {
          destination = new byte[systemFirmwareTable];
          Marshal.Copy(num, destination, 0, systemFirmwareTable);
        }
        return destination;
      }
      finally
      {
        Marshal.FreeHGlobal(num);
      }
    }

    internal static string[] EnumSystemFirmwareTables(
      NativeMethods.FirmwareTableProviderSignature provider)
    {
      int length = NativeMethods.EnumSystemFirmwareTables(provider, IntPtr.Zero, 0);
      byte[] numArray = length > 0 ? new byte[length] : throw new ExternalException(string.Format("There was an error enumerating the firmware tables from {0}.", (object) provider), Marshal.GetLastWin32Error());
      IntPtr num = Marshal.AllocHGlobal(length);
      try
      {
        NativeMethods.EnumSystemFirmwareTables(provider, num, length);
        Marshal.Copy(num, numArray, 0, length);
      }
      finally
      {
        Marshal.FreeHGlobal(num);
      }
      string[] strArray = new string[length / 4];
      for (int index = 0; index < strArray.Length; ++index)
        strArray[index] = Encoding.ASCII.GetString(numArray, 4 * index, 4);
      return strArray;
    }
  }
}
