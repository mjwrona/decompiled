// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CommandTarget.IOleCommandTarget
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Client.CommandTarget
{
  [ComVisible(true)]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("B722BCCB-4E68-101B-A2BC-00AA00404770")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [CLSCompliant(false)]
  [ComImport]
  public interface IOleCommandTarget
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int QueryStatus([In] ref Guid pguidCmdGroup, [In] uint cCmds, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), In, Out] OLECMD[] prgCmds, [In, Out] IntPtr pCmdText);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Exec([In] ref Guid pguidCmdGroup, [In] uint nCmdID, [In] uint nCmdexecopt, [In] IntPtr pvaIn, [In] IntPtr pvaOut);
  }
}
