// Decompiled with JetBrains decompiler
// Type: Windows.CertEntroll.IX509ExtensionBasicConstraints
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.CertEntroll
{
  [CompilerGenerated]
  [Guid("728AB316-217D-11DA-B2A4-000E7BBB2B09")]
  [TypeIdentifier]
  [ComImport]
  public interface IX509ExtensionBasicConstraints : IX509Extension
  {
    [SpecialName]
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    sealed extern void _VtblGap1_3();

    [DispId(1610743811)]
    bool Critical { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610809344)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InitializeEncode([In] bool IsCA, [In] int PathLenConstraint);
  }
}
