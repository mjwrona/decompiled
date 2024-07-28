// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.IRowSetsNative
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  [ComVisible(false)]
  [Guid("3ca9b98e-eea9-4179-97e5-8acc774385c2")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [ComImport]
  public interface IRowSetsNative
  {
    [return: MarshalAs(UnmanagedType.IUnknown)]
    object GetRowSet([MarshalAs(UnmanagedType.I4)] MetadataRowSetNames name);
  }
}
