// Decompiled with JetBrains decompiler
// Type: Windows.CertEntroll.IAlternativeNames
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.CertEntroll
{
  [CompilerGenerated]
  [Guid("728AB314-217D-11DA-B2A4-000E7BBB2B09")]
  [DefaultMember("ItemByIndex")]
  [TypeIdentifier]
  [ComImport]
  public interface IAlternativeNames : IEnumerable
  {
    [SpecialName]
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    sealed extern void _VtblGap1_3();

    [DispId(2)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Add([MarshalAs(UnmanagedType.Interface), In] CAlternativeName pVal);
  }
}
