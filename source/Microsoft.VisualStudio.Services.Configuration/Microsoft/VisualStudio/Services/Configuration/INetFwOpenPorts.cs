// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.INetFwOpenPorts
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [TypeLibType(4160)]
  [Guid("C0E9D7FA-E07E-430A-B19A-090CE82D92E2")]
  [ComImport]
  public interface INetFwOpenPorts : IEnumerable
  {
    [DispId(1)]
    int Count { [DispId(1), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(2)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Add([MarshalAs(UnmanagedType.Interface), In] INetFwOpenPort port);

    [DispId(3)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Remove([In] int portNumber, [In] NET_FW_IP_PROTOCOL_ ipProtocol);

    [DispId(4)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    INetFwOpenPort Item([In] int portNumber, [In] NET_FW_IP_PROTOCOL_ ipProtocol);
  }
}
