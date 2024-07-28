// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.INetFwRules
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Collections;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [Guid("9C4C6277-5027-441E-AFAE-CA1F542DA009")]
  [TypeLibType(4160)]
  public interface INetFwRules : IEnumerable
  {
    [DispId(1)]
    int Count { [DispId(1)] get; }

    [DispId(2)]
    void Add([MarshalAs(UnmanagedType.Interface), In] INetFwRule rule);

    [DispId(3)]
    void Remove([MarshalAs(UnmanagedType.BStr), In] string Name);

    [DispId(4)]
    [return: MarshalAs(UnmanagedType.Interface)]
    INetFwRule Item([MarshalAs(UnmanagedType.BStr), In] string Name);
  }
}
