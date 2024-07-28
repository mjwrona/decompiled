// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.BaseUserControl
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class BaseUserControl : UserControl
  {
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnCreateControl()
    {
      if (!this.DesignMode)
        UIHost.InitializeContainer((ContainerControl) this);
      base.OnCreateControl();
      if (this.DesignMode)
        return;
      UIHost.FontChanged += new EventHandler(this.HostFontChanged);
      this.FontChanged += new EventHandler(this.BaseUserControl_FontChanged);
    }

    private void BaseUserControl_FontChanged(object sender, EventArgs e) => UIHost.UpdatePrimaryFont(this.Font, (Control) this, true);

    protected virtual void HostFontChanged(object sender, EventArgs e) => this.Font = UIHost.Font;

    protected override void Dispose(bool disposing)
    {
      if (disposing && !this.DesignMode)
        UIHost.FontChanged -= new EventHandler(this.HostFontChanged);
      base.Dispose(disposing);
    }
  }
}
