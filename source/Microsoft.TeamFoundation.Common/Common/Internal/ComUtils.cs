// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.ComUtils
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ComUtils
  {
    public static bool Succeeded(int hr) => hr >= 0;

    public static bool Failed(int hr) => hr < 0;

    public static int ThrowOnFailure(int hr) => ComUtils.ThrowOnFailure(hr, (int[]) null);

    public static int ThrowOnFailure(int hr, params int[] expectedHRFailure)
    {
      if (ComUtils.Failed(hr) && (expectedHRFailure == null || Array.IndexOf<int>(expectedHRFailure, hr) < 0))
        Marshal.ThrowExceptionForHR(hr);
      return hr;
    }

    public enum OleDispatchError
    {
      DISP_E_UNKNOWNINTERFACE = -2147352575, // 0x80020001
      DISP_E_MEMBERNOTFOUND = -2147352573, // 0x80020003
      DISP_E_PARAMNOTFOUND = -2147352572, // 0x80020004
      DISP_E_TYPEMISMATCH = -2147352571, // 0x80020005
      DISP_E_UNKNOWNNAME = -2147352570, // 0x80020006
      DISP_E_NONAMEDARGS = -2147352569, // 0x80020007
      DISP_E_BADVARTYPE = -2147352568, // 0x80020008
      DISP_E_EXCEPTION = -2147352567, // 0x80020009
      DISP_E_OVERFLOW = -2147352566, // 0x8002000A
      DISP_E_BADINDEX = -2147352565, // 0x8002000B
      DISP_E_UNKNOWNLCID = -2147352564, // 0x8002000C
      DISP_E_ARRAYISLOCKED = -2147352563, // 0x8002000D
      DISP_E_BADPARAMCOUNT = -2147352562, // 0x8002000E
      DISP_E_PARAMNOTOPTIONAL = -2147352561, // 0x8002000F
      DISP_E_BADCALLEE = -2147352560, // 0x80020010
      DISP_E_NOTACOLLECTION = -2147352559, // 0x80020011
      DISP_E_DIVBYZERO = -2147352558, // 0x80020012
      DISP_E_BUFFERTOOSMALL = -2147352557, // 0x80020013
    }
  }
}
