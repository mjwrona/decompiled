// Decompiled with JetBrains decompiler
// Type: Windows.CertEntroll.IX509Enrollment2
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.CertEntroll
{
  [CompilerGenerated]
  [Guid("728AB350-217D-11DA-B2A4-000E7BBB2B09")]
  [TypeIdentifier]
  [ComImport]
  public interface IX509Enrollment2 : IX509Enrollment
  {
    [SpecialName]
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    sealed extern void _VtblGap1_2();

    [DispId(1610743810)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InitializeFromRequest([MarshalAs(UnmanagedType.Interface), In] IX509CertificateRequest pRequest);

    [DispId(1610743811)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string CreateRequest([In] EncodingType Encoding = EncodingType.XCN_CRYPT_STRING_BASE64);

    [SpecialName]
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    sealed extern void _VtblGap2_1();

    [DispId(1610743813)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InstallResponse(
      [In] InstallResponseRestrictionFlags Restrictions,
      [MarshalAs(UnmanagedType.BStr), In] string strResponse,
      [In] EncodingType Encoding,
      [MarshalAs(UnmanagedType.BStr), In] string strPassword);

    [SpecialName]
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    sealed extern void _VtblGap3_11();

    [DispId(1610743825)]
    string CertificateFriendlyName { [DispId(1610743825), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743825), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }
  }
}
