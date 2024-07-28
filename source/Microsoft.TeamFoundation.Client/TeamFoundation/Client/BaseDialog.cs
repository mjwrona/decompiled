// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.BaseDialog
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class BaseDialog : Form
  {
    private bool m_couldBeInOnLoad;
    private bool m_closeDialogOnVisibleTrue;
    private bool m_wasSizedMoved;
    private bool m_saveRestorePosition;
    private IMessageFilter m_messageFilter;
    private string m_helpTopic;
    private bool m_alwaysShowHelpButton;
    private bool m_forwardWndMsgOutsideVS;

    public BaseDialog()
      : this(false)
    {
    }

    public BaseDialog(bool forwardWndMsgOutsideVS)
    {
      this.m_alwaysShowHelpButton = true;
      this.m_helpTopic = string.Empty;
      this.m_saveRestorePosition = true;
      this.m_forwardWndMsgOutsideVS = forwardWndMsgOutsideVS;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && !this.DesignMode)
      {
        UIHost.FontChanged -= new EventHandler(this.HostFontChanged);
        this.FontChanged -= new EventHandler(this.BaseDialog_FontChanged);
        this.FormClosing -= new FormClosingEventHandler(this.BaseDialog_FormClosing);
        this.HelpButtonClicked -= new CancelEventHandler(this.BaseDialog_HelpButtonClicked);
        this.Resize -= new EventHandler(this.BaseDialog_Resize);
        this.Move -= new EventHandler(this.BaseDialog_Move);
        this.HelpRequested -= new HelpEventHandler(this.BaseDialog_HelpRequested);
      }
      base.Dispose(disposing);
    }

    public new DialogResult ShowDialog() => UIHost.ShowModalDialog((Form) this);

    public new DialogResult ShowDialog(IWin32Window parent) => UIHost.ShowModalDialog((Form) this, parent);

    private void BaseDialog_HelpRequested(object sender, HelpEventArgs e)
    {
      if (string.IsNullOrEmpty(this.HelpTopic) || !UIHost.DisplayHelp(this.HelpTopic))
        return;
      e.Handled = true;
    }

    private void BaseDialog_HelpButtonClicked(object sender, CancelEventArgs e)
    {
      if (!string.IsNullOrEmpty(this.HelpTopic))
        UIHost.DisplayHelp(this.HelpTopic);
      e.Cancel = true;
    }

    private void BaseDialog_Move(object sender, EventArgs e) => this.m_wasSizedMoved = true;

    private void BaseDialog_Resize(object sender, EventArgs e) => this.m_wasSizedMoved = true;

    private void BaseDialog_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (!this.m_saveRestorePosition || !this.m_wasSizedMoved)
        return;
      UIHost.UIConfig.SaveForm((Form) this);
    }

    protected override void OnLoad(EventArgs e)
    {
      this.SuspendLayout();
      if (!this.DesignMode)
        UIHost.InitializeContainer((ContainerControl) this);
      base.OnLoad(e);
      if (!this.DesignMode)
      {
        UIHost.FontChanged += new EventHandler(this.HostFontChanged);
        this.FontChanged += new EventHandler(this.BaseDialog_FontChanged);
        this.FormClosing += new FormClosingEventHandler(this.BaseDialog_FormClosing);
        this.HelpButtonClicked += new CancelEventHandler(this.BaseDialog_HelpButtonClicked);
        this.Resize += new EventHandler(this.BaseDialog_Resize);
        this.Move += new EventHandler(this.BaseDialog_Move);
        this.HelpRequested += new HelpEventHandler(this.BaseDialog_HelpRequested);
        this.RestoreSizePosition();
      }
      this.ResumeLayout();
    }

    protected void RestoreSizePosition()
    {
      if (this.SaveRestorePosition)
        UIHost.UIConfig.RestoreForm((Form) this);
      this.m_wasSizedMoved = false;
    }

    protected void EnsureDialogFitsInWorkingArea()
    {
      Rectangle workingArea = Screen.GetWorkingArea((Control) this);
      if (workingArea.Contains(new Rectangle(this.Location, this.Size)))
        return;
      this.Location = workingArea.Location;
      this.Size = workingArea.Size;
    }

    private void BaseDialog_FontChanged(object sender, EventArgs e) => UIHost.UpdatePrimaryFont(this.Font, (Control) this, true);

    private void HostFontChanged(object sender, EventArgs e) => this.Font = UIHost.Font;

    [Browsable(true)]
    [Description("Sets the default help topic for dialog")]
    [Category("Team Foundation")]
    public virtual string HelpTopic
    {
      get => this.m_helpTopic;
      set => this.m_helpTopic = value != null ? value : string.Empty;
    }

    [Browsable(true)]
    [Description("Determines if dialog positions will be automatically saved and restored")]
    [Category("Team Foundation")]
    [DefaultValue(true)]
    public bool SaveRestorePosition
    {
      get => this.m_saveRestorePosition;
      set => this.m_saveRestorePosition = value;
    }

    [Browsable(true)]
    [Description("Determines if we will always force showing a help button for this dialog")]
    [Category("Team Foundation")]
    [DefaultValue(true)]
    public bool AlwaysShowHelpButton
    {
      get => this.m_alwaysShowHelpButton;
      set => this.m_alwaysShowHelpButton = value;
    }

    [Browsable(false)]
    public IMessageFilter MessageFilter
    {
      get => this.m_messageFilter;
      set => this.m_messageFilter = value;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    protected override void WndProc(ref Message m)
    {
      if (m.Msg != 123 || !this.OnContextMenu(m))
        base.WndProc(ref m);
      if (!this.ShowInTaskbar && !this.m_forwardWndMsgOutsideVS)
        return;
      UIHost.OnBroadcastMessage(m.Msg, m.WParam, m.LParam);
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    protected override bool ProcessKeyPreview(ref Message m) => m.Msg == 261 && (long) m.WParam == 18L && (Control.ModifierKeys & Keys.Shift) == Keys.Shift && this.OnFocusToolbar() || base.ProcessKeyPreview(ref m);

    protected virtual bool OnContextMenu(Message m) => false;

    public event FocusToolbarEventHandler FocusToolbar;

    private bool OnFocusToolbar()
    {
      bool flag = false;
      FocusToolbarEventHandler focusToolbar = this.FocusToolbar;
      if (focusToolbar != null)
      {
        FocusToolbarEventArgs args = new FocusToolbarEventArgs();
        focusToolbar((object) this, args);
        flag = args.Handled;
      }
      return flag;
    }

    public void CloseDuringOnLoad(DialogResult result)
    {
      if (!this.m_couldBeInOnLoad)
        return;
      this.DialogResult = result;
      this.m_closeDialogOnVisibleTrue = true;
    }

    protected override void OnCreateControl()
    {
      this.m_couldBeInOnLoad = true;
      base.OnCreateControl();
      this.m_couldBeInOnLoad = false;
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
      if (this.Visible && this.m_closeDialogOnVisibleTrue)
      {
        this.m_closeDialogOnVisibleTrue = false;
        this.Close();
      }
      else
        base.OnVisibleChanged(e);
    }
  }
}
