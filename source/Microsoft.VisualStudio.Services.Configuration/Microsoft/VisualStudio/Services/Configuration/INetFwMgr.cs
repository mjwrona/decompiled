// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.INetFwMgr
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [Guid("F7898AF5-CAC4-4632-A2EC-DA06E5111AF2")]
  [TypeLibType(4160)]
  [ComImport]
  public interface INetFwMgr
  {
    [DispId(1)]
    INetFwPolicy LocalPolicy { [DispId(1), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(2)]
    NET_FW_PROFILE_TYPE_ CurrentProfileType { [DispId(2), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(3)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RestoreDefaults();

    [DispId(4)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void IsPortAllowed(
      [MarshalAs(UnmanagedType.BStr), In] string imageFileName,
      [In] NET_FW_IP_VERSION_ ipVersion,
      [In] int portNumber,
      [MarshalAs(UnmanagedType.BStr), In] string localAddress,
      [In] NET_FW_IP_PROTOCOL_ ipProtocol,
      [MarshalAs(UnmanagedType.Struct)] out object allowed,
      [MarshalAs(UnmanagedType.Struct)] out object restricted);

    [DispId(5)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void IsIcmpTypeAllowed(
      [In] NET_FW_IP_VERSION_ ipVersion,
      [MarshalAs(UnmanagedType.BStr), In] string localAddress,
      [In] byte type,
      [MarshalAs(UnmanagedType.Struct)] out object allowed,
      [MarshalAs(UnmanagedType.Struct)] out object restricted);
  }
}
