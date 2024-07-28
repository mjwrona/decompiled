// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Win32WindowWrapper
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class Win32WindowWrapper : System.Windows.Forms.IWin32Window
  {
    private IntPtr m_hwnd;

    public Win32WindowWrapper(IntPtr handle) => this.m_hwnd = handle;

    public static Win32WindowWrapper FromVisual(Visual visual)
    {
      IntPtr handle = IntPtr.Zero;
      if (PresentationSource.FromVisual(visual) != null)
      {
        HwndSource hwndSource = (HwndSource) PresentationSource.FromVisual(visual);
        if (hwndSource != null)
          handle = hwndSource.Handle;
      }
      return new Win32WindowWrapper(handle);
    }

    IntPtr System.Windows.Forms.IWin32Window.Handle => this.m_hwnd;
  }
}
