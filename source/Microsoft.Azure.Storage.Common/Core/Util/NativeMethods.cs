// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.NativeMethods
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal static class NativeMethods
  {
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CryptAcquireContextW(
      out IntPtr hashProv,
      string pszContainer,
      string pszProvider,
      uint provType,
      uint flags);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CryptDestroyHash(IntPtr hashHandle);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CryptReleaseContext(IntPtr hashProv, int dwFlags);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CryptGetHashParam(
      IntPtr hashHandle,
      uint param,
      byte[] data,
      ref int pdwDataLen,
      uint flags);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CryptCreateHash(
      IntPtr hashProv,
      uint algId,
      IntPtr hashKey,
      uint flags,
      out IntPtr hashHandle);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CryptHashData(
      IntPtr hashHandle,
      IntPtr data,
      int dataLen,
      uint flags);
  }
}
