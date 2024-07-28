// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.INetFwPolicy
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [TypeLibType(4160)]
  [Guid("D46D2478-9AC9-4008-9DC7-5563CE5536CC")]
  [ComImport]
  public interface INetFwPolicy
  {
    [DispId(1)]
    INetFwProfile CurrentProfile { [DispId(1), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(2)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    INetFwProfile GetProfileByType([In] NET_FW_PROFILE_TYPE_ profileType);
  }
}
