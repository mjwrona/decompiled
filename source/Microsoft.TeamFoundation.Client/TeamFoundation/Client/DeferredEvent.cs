// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.DeferredEvent
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class DeferredEvent
  {
    private Control m_control;
    private MethodInvoker m_method;
    private bool m_pending;
    private MethodInvoker m_callback;
    private IAsyncResult m_asyncResult;

    public DeferredEvent(Control control, MethodInvoker method)
    {
      this.m_control = control;
      this.m_method = method;
      this.m_callback = new MethodInvoker(this.Callback);
    }

    public void Fire()
    {
      if (this.m_pending || !this.m_control.IsHandleCreated)
        return;
      this.m_pending = true;
      if (this.m_asyncResult != null)
        this.m_control.EndInvoke(this.m_asyncResult);
      this.m_asyncResult = this.m_control.BeginInvoke((Delegate) this.m_callback);
    }

    private void Callback()
    {
      this.m_pending = false;
      this.m_method();
    }
  }
}
