// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.INetFwServices
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [Guid("79649BB4-903E-421B-94C9-79848E79F6EE")]
  [TypeLibType(4160)]
  [ComImport]
  public interface INetFwServices : IEnumerable
  {
    [DispId(1)]
    int Count { [DispId(1), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(2)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    INetFwService Item([In] NET_FW_SERVICE_TYPE_ svcType);
  }
}
