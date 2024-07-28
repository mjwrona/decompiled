// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CustomCombo`1
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CustomCombo<T> : ComboBox where T : Control, IComboHostedControl
  {
    private const int cMinDropdownHeight = 100;
    private const long cDropDownTickDelay = 2500000;
    private ToolStripDropDown m_dropDown;
    private IComboHostedControl m_hostedControl;
    private Point m_currentPosition;
    private Timer m_moveTimer;
    private Timer m_textChangedTimer;
    private long m_lastTimeStamp;
    private int m_textChanging;
    private int m_suspendAutoComplete;
    private int m_cachedCaretPos;
    private bool m_accessibilityMode;

    public void Initialize(T control)
    {
      ArgumentUtility.CheckForNull<T>(control, nameof (control));
      this.m_hostedControl = (IComboHostedControl) control;
      control.Location = new Point(0, 0);
      control.Margin = new Padding(0);
      control.Padding = new Padding(0);
      control.Dock = DockStyle.Fill;
      ToolStripControlHost stripControlHost = new ToolStripControlHost((Control) control);
      stripControlHost.Dock = DockStyle.Fill;
      stripControlHost.Margin = new Padding(0);
      stripControlHost.Padding = new Padding(0);
      stripControlHost.AutoSize = true;
      this.m_dropDown = new ToolStripDropDown();
      this.m_dropDown.AutoClose = false;
      this.m_dropDown.LayoutStyle = ToolStripLayoutStyle.Table;
      this.m_dropDown.Items.Add((ToolStripItem) stripControlHost);
      this.m_dropDown.BackColor = SystemColors.Window;
      this.m_dropDown.Margin = new Padding(0);
      this.m_dropDown.BackColor = UIHost.Colors.ControlBackColor;
      this.m_moveTimer = new Timer();
      this.m_moveTimer.Interval = 100;
      this.m_moveTimer.Enabled = false;
      this.m_moveTimer.Tick += new EventHandler(this.MoveTimer_TimerTick);
      this.SetDelayedFiltering(true);
      this.SizeChanged += new EventHandler(this.ComboBox_SizeChanged);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.SizeChanged -= new EventHandler(this.ComboBox_SizeChanged);
        if (this.HostedDropDownControl != null)
          this.HostedDropDownControl.Dispose();
        if (this.m_dropDown != null)
        {
          this.m_dropDown.Dispose();
          this.m_dropDown = (ToolStripDropDown) null;
        }
        if (this.m_moveTimer != null)
        {
          this.m_moveTimer.Stop();
          this.m_moveTimer.Tick -= new EventHandler(this.MoveTimer_TimerTick);
          this.m_moveTimer.Dispose();
          this.m_moveTimer = (Timer) null;
        }
        this.SetDelayedFiltering(false);
      }
      base.Dispose(disposing);
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    protected override void WndProc(ref Message m)
    {
      if (m.Msg == 8465 && this.HIWORD((int) m.WParam) == 7)
      {
        if (this.m_lastTimeStamp + 1000000L >= DateTime.Now.Ticks)
          return;
        if (this.IsDroppedDown)
          this.HideDropDown();
        else
          this.ShowDropDown(true);
      }
      else
        base.WndProc(ref m);
    }

    private int HIWORD(int n) => n >> 16 & (int) ushort.MaxValue;

    public override bool PreProcessMessage(ref Message msg)
    {
      if (msg.Msg == 256)
      {
        KeyEventArgs keyEventArgs = new KeyEventArgs((Keys) (int) msg.WParam | Control.ModifierKeys);
        if (keyEventArgs.KeyCode == Keys.Escape && !keyEventArgs.Alt && !keyEventArgs.Control && !keyEventArgs.Shift && this.IsDroppedDown)
        {
          this.HideDropDown();
          return true;
        }
      }
      return base.PreProcessMessage(ref msg);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (!e.Alt && !e.Control)
      {
        switch (e.KeyCode)
        {
          case Keys.Return:
            if (this.IsDroppedDown)
            {
              int caretPos;
              this.SetSelection(this.HostedControl.GetSelectedText(out caretPos), caretPos, true);
            }
            e.Handled = true;
            break;
          case Keys.Prior:
          case Keys.Next:
          case Keys.Up:
          case Keys.Down:
            if (!this.m_accessibilityMode || e.KeyCode != Keys.Down || this.m_lastTimeStamp + 2500000L <= DateTime.Now.Ticks)
            {
              if (this.IsDroppedDown)
                this.HostedDropDownControl.Focus();
              else
                this.ShowDropDown(true);
              e.Handled = true;
              break;
            }
            break;
          case Keys.Right:
            if (this.DropDownStyle == ComboBoxStyle.DropDownList)
            {
              this.ShowDropDown(true);
              e.Handled = true;
              break;
            }
            break;
        }
      }
      if (e.Handled)
        return;
      base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
      base.OnKeyUp(e);
      this.m_cachedCaretPos = this.SelectionStart;
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
      base.OnMouseClick(e);
      this.m_cachedCaretPos = this.SelectionStart;
    }

    protected override void OnFontChanged(EventArgs e)
    {
      base.OnFontChanged(e);
      this.HostedDropDownControl.Font = this.Font;
    }

    public override string Text
    {
      set
      {
        ++this.m_suspendAutoComplete;
        try
        {
          base.Text = value;
        }
        finally
        {
          --this.m_suspendAutoComplete;
        }
      }
    }

    protected override void OnTextChanged(EventArgs e)
    {
      try
      {
        if (this.m_suspendAutoComplete > 0)
        {
          base.OnTextChanged(e);
        }
        else
        {
          if (this.m_textChanging > 0)
            return;
          this.StartFilter();
          base.OnTextChanged(e);
        }
      }
      catch (Exception ex)
      {
      }
    }

    protected override void OnLostFocus(EventArgs e)
    {
      if (!this.IsDroppedDown)
        return;
      this.HostedControl.HandleComboControlFocusLost();
      base.OnLostFocus(e);
    }

    protected override AccessibleObject CreateAccessibilityInstance()
    {
      this.m_accessibilityMode = true;
      return (AccessibleObject) new CustomCombo<T>.CustomComboAccessibilityObject(this);
    }

    private void ComboBox_SizeChanged(object sender, EventArgs e)
    {
      if (!this.IsDroppedDown)
        return;
      this.HideDropDown();
    }

    private void MoveTimer_TimerTick(object sender, EventArgs e)
    {
      if (!(this.m_currentPosition != this.PointToScreen(Point.Empty)))
        return;
      this.HideDropDown();
    }

    private void TextChangedTimer_Tick(object sender, EventArgs e)
    {
      this.m_textChangedTimer.Enabled = false;
      this.FilterItems();
    }

    private void FilterItems()
    {
      if (!this.HostedControl.Filter(this.Text) || !this.ContainsFocus)
        return;
      if (!this.IsDroppedDown)
      {
        ++this.m_textChanging;
        try
        {
          this.ShowDropDown(false);
        }
        finally
        {
          --this.m_textChanging;
        }
      }
      else
        this.RecalculateSize();
    }

    public void PostKey(Keys wParam, Keys lParam) => Microsoft.TeamFoundation.Common.Internal.NativeMethods.PostMessage(new HandleRef((object) this, this.Handle), 256, (IntPtr) (int) wParam, (IntPtr) (int) lParam);

    public void SetSelection(string selectedText, int caretPos, bool closeDropdown) => this.BeginInvoke((Delegate) (() =>
    {
      try
      {
        if (closeDropdown)
          this.HideDropDown();
        this.SetSelectedText(selectedText, caretPos);
        this.OnItemSelected(selectedText);
      }
      catch (Exception ex)
      {
      }
    }));

    public void StartFilter()
    {
      if (this.m_textChangedTimer != null)
      {
        this.m_textChangedTimer.Enabled = false;
        this.m_textChangedTimer.Enabled = true;
      }
      else
        this.FilterItems();
    }

    protected virtual void OnItemSelected(string item)
    {
    }

    public Size GetControlAreaSize()
    {
      Size desiredControlSize = this.HostedControl.GetDesiredControlSize();
      Size minimumControlSize = this.HostedControl.GetMinimumControlSize();
      return new Size(Math.Max(desiredControlSize.Width, this.Width - this.m_dropDown.Padding.Horizontal), Math.Max(100 - this.m_dropDown.Padding.Vertical, Math.Max(minimumControlSize.Height, desiredControlSize.Height)));
    }

    public bool IsDroppedDown => this.m_dropDown.Visible;

    public void HideDropDown() => this.SetDropDownVisibility(false);

    public void ShowDropDown(bool giveFocus)
    {
      this.SetDropDownVisibility(true);
      this.m_cachedCaretPos = this.SelectionStart;
      if (!giveFocus)
        return;
      this.HostedDropDownControl.Focus();
      this.SetCaretPosition(this.m_cachedCaretPos);
    }

    public void RecalculateSize()
    {
      int num = (int) this.SetDropdownSize();
    }

    public int CaretPosition => !this.ContainsFocus && this.m_cachedCaretPos >= 0 && this.m_cachedCaretPos <= this.Text.Length ? this.m_cachedCaretPos : this.SelectionStart;

    public Control HostedDropDownControl => (Control) this.m_hostedControl;

    public void SetCaretPosition(int pos)
    {
      if (this.DropDownStyle == ComboBoxStyle.DropDownList)
        return;
      this.SelectionStart = pos;
      this.SelectionLength = 0;
      this.m_cachedCaretPos = this.SelectionStart;
    }

    protected void SetDelayedFiltering(bool delayOn)
    {
      if (delayOn && this.m_textChangedTimer == null)
      {
        this.m_textChangedTimer = new Timer();
        this.m_textChangedTimer.Interval = 150;
        this.m_textChangedTimer.Enabled = false;
        this.m_textChangedTimer.Tick += new EventHandler(this.TextChangedTimer_Tick);
      }
      else
      {
        if (delayOn || this.m_textChangedTimer == null)
          return;
        this.m_textChangedTimer.Stop();
        this.m_textChangedTimer.Tick -= new EventHandler(this.TextChangedTimer_Tick);
        this.m_textChangedTimer.Dispose();
        this.m_textChangedTimer = (Timer) null;
      }
    }

    protected void SetSelectedText(string text, int caretPos)
    {
      if (text == null)
        text = string.Empty;
      if (this.DropDownStyle == ComboBoxStyle.DropDownList)
      {
        this.Items.Clear();
        this.Items.Add(text != null ? (object) text : (object) string.Empty);
      }
      this.Text = text;
      this.SetCaretPosition(caretPos);
    }

    protected IComboHostedControl HostedControl => this.m_hostedControl;

    private void SetDropDownVisibility(bool show)
    {
      if (!this.IsHandleCreated)
        return;
      this.m_lastTimeStamp = DateTime.Now.Ticks;
      if (show)
      {
        if (!this.m_dropDown.Visible)
        {
          this.HostedControl.Filter(this.Text);
          this.OnDropDown(EventArgs.Empty);
          ToolStripDropDownDirection direction = this.SetDropdownSize();
          Point screen = this.PointToScreen(Point.Empty);
          if (direction == ToolStripDropDownDirection.BelowRight)
            screen.Y += this.Height;
          this.m_dropDown.Show(screen, direction);
          this.m_moveTimer.Start();
          this.m_currentPosition = this.PointToScreen(Point.Empty);
        }
      }
      else if (this.m_dropDown.Visible)
      {
        this.m_dropDown.Close();
        this.m_moveTimer.Stop();
      }
      this.AsyncHideComboBoxDropDown();
    }

    private void AsyncHideComboBoxDropDown()
    {
      Action method = (Action) (() =>
      {
        string text = this.Text;
        int caretPosition = this.CaretPosition;
        this.DroppedDown = false;
        if (string.Equals(text, this.Text, StringComparison.Ordinal))
          return;
        this.SetSelectedText(text, caretPosition);
      });
      try
      {
        this.BeginInvoke((Delegate) method);
      }
      catch (Exception ex)
      {
      }
    }

    private Size GetDropdownSize()
    {
      Size desiredControlSize = this.HostedControl.GetDesiredControlSize();
      Size minimumControlSize = this.HostedControl.GetMinimumControlSize();
      return new Size(Math.Max(desiredControlSize.Width + this.m_dropDown.Padding.Horizontal, this.Width), Math.Max(minimumControlSize.Height + this.m_dropDown.Padding.Vertical, desiredControlSize.Height + this.m_dropDown.Padding.Vertical));
    }

    private ToolStripDropDownDirection SetDropdownSize()
    {
      Size dropdownSize = this.GetDropdownSize();
      Point location = this.HostedDropDownControl.Location;
      this.m_dropDown.MinimumSize = dropdownSize;
      this.m_dropDown.MaximumSize = dropdownSize;
      Size size = new Size(dropdownSize.Width - this.m_dropDown.Padding.Horizontal, dropdownSize.Height - this.m_dropDown.Padding.Vertical);
      this.HostedDropDownControl.MaximumSize = size;
      this.HostedDropDownControl.MinimumSize = size;
      this.HostedDropDownControl.Size = size;
      this.HostedDropDownControl.Location = location;
      return this.m_dropDown.Height < Screen.FromControl((Control) this).WorkingArea.Height - this.PointToScreen(Point.Empty).Y - this.Height ? ToolStripDropDownDirection.BelowRight : ToolStripDropDownDirection.AboveRight;
    }

    private class CustomComboAccessibilityObject : Control.ControlAccessibleObject
    {
      private CustomCombo<T> m_control;
      private const int cNumControls = 3;

      public CustomComboAccessibilityObject(CustomCombo<T> control)
        : base((Control) control)
      {
        this.m_control = control;
      }

      public override int GetChildCount() => !this.m_control.IsDroppedDown ? 2 : 3;

      public override AccessibleObject GetChild(int index) => index == 2 ? this.m_control.HostedDropDownControl.AccessibilityObject : base.GetChild(index);
    }
  }
}
