// Decompiled with JetBrains decompiler
// Type: Windows.CertEntroll.IX509SignatureInformation
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.CertEntroll
{
  [CompilerGenerated]
  [Guid("728AB33C-217D-11DA-B2A4-000E7BBB2B09")]
  [TypeIdentifier]
  [ComImport]
  public interface IX509SignatureInformation
  {
    [DispId(1610743808)]
    CObjectId HashAlgorithm { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.Interface), In] set; }
  }
}
