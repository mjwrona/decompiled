// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.FocusRestorer
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;

namespace Microsoft.TeamFoundation.Client
{
  internal class FocusRestorer : IDisposable
  {
    private IntPtr m_focusWnd;

    internal FocusRestorer() => this.m_focusWnd = NativeMethods.GetFocus();

    void IDisposable.Dispose()
    {
      if (this.m_focusWnd != IntPtr.Zero)
      {
        NativeMethods.SetFocus(this.m_focusWnd);
        this.m_focusWnd = IntPtr.Zero;
      }
      GC.SuppressFinalize((object) this);
    }
  }
}
