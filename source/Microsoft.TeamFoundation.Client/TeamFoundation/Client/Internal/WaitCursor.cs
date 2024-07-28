// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Internal.WaitCursor
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WaitCursor : IDisposable
  {
    private Cursor m_oldCursor;

    protected WaitCursor()
    {
    }

    void IDisposable.Dispose()
    {
      GC.SuppressFinalize((object) this);
      this.Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_oldCursor != (Cursor) null)
      {
        Cursor.Current = this.m_oldCursor;
        this.m_oldCursor = (Cursor) null;
      }
      else
        Cursor.Current = Cursors.Default;
    }

    protected void SetDefaultWaitCursor()
    {
      this.m_oldCursor = Cursor.Current;
      Cursor.Current = Cursors.WaitCursor;
    }
  }
}
