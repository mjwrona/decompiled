// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.IRowSetNative
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  [ComVisible(false)]
  [Guid("b23fa78e-ad7b-4efc-a96c-b56a22031f67")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  internal interface IRowSetNative
  {
    int GetRowCount();

    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
    string[] GetColumns();

    void GetRow(int row, IntPtr p, int esz);
  }
}
