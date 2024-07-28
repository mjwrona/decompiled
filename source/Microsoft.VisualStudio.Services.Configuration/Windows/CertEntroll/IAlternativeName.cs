// Decompiled with JetBrains decompiler
// Type: Windows.CertEntroll.IAlternativeName
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.CertEntroll
{
  [CompilerGenerated]
  [Guid("728AB313-217D-11DA-B2A4-000E7BBB2B09")]
  [TypeIdentifier]
  [ComImport]
  public interface IAlternativeName
  {
    [DispId(1610743808)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InitializeFromString([In] AlternativeNameType Type, [MarshalAs(UnmanagedType.BStr), In] string strValue);

    [DispId(1610743809)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InitializeFromRawData([In] AlternativeNameType Type, [In] EncodingType Encoding, [MarshalAs(UnmanagedType.BStr), In] string strRawData);

    [SpecialName]
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    sealed extern void _VtblGap1_1();

    [DispId(1610743811)]
    AlternativeNameType Type { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
  }
}
