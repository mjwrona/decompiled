// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.INetFwAuthorizedApplications
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [TypeLibType(4160)]
  [Guid("644EFD52-CCF9-486C-97A2-39F352570B30")]
  [ComImport]
  public interface INetFwAuthorizedApplications : IEnumerable
  {
    [DispId(1)]
    int Count { [DispId(1), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(2)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Add([MarshalAs(UnmanagedType.Interface), In] INetFwAuthorizedApplication app);

    [DispId(3)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Remove([MarshalAs(UnmanagedType.BStr), In] string imageFileName);

    [DispId(4)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    INetFwAuthorizedApplication Item([MarshalAs(UnmanagedType.BStr), In] string imageFileName);
  }
}
