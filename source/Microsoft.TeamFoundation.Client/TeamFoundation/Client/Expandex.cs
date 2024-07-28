// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Expandex
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
  public class Expandex : UserControl
  {
    private Panel m_panel;
    private ExpanderButton m_expander;
    private IContainer components;
    private Size m_expandedPanelSize;
    private int m_heightPad;

    public Expandex()
    {
      this.InitializeComponent();
      this.m_expander.ExpandEvent += new EventHandler<CancelEventArgs>(this.m_expander_ExpandEvent);
      this.m_expander.ContractEvent += new EventHandler<CancelEventArgs>(this.m_expander_ContractEvent);
      this.m_expandedPanelSize = this.m_panel.Size;
      this.Expanded = true;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    [Category("Appearance")]
    [Description("Is the control expanded or contracted")]
    [Browsable(true)]
    [DefaultValue(true)]
    public bool Expanded
    {
      get => this.m_expander.Expanded;
      set => this.m_expander.Expanded = value;
    }

    [Category("Appearance")]
    [Description("Text value to right of expand/contract button")]
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override string Text
    {
      get => this.m_expander.Text;
      set => this.m_expander.Text = value;
    }

    public Panel ExpandedPanel => this.m_panel;

    public Size ExpandedPanelSize
    {
      get => this.m_expandedPanelSize;
      set
      {
        this.m_expandedPanelSize = value;
        this.Size = new Size(value.Width, value.Height + this.HeightPad);
      }
    }

    public int HeightFromExpandedPanelHeight(int epHeight) => epHeight + this.HeightPad;

    private void m_expander_ExpandEvent(object sender, CancelEventArgs e)
    {
      this.Size = new Size(this.m_expandedPanelSize.Width, this.m_expandedPanelSize.Height + this.HeightPad);
      this.m_panel.Enabled = true;
      if (this.m_panel.Controls.Count <= 0)
        return;
      this.m_panel.Controls[0].Focus();
    }

    private void m_expander_ContractEvent(object sender, CancelEventArgs e)
    {
      this.m_heightPad = this.Height - this.m_panel.Height;
      Size size = this.Size;
      int width = size.Width;
      size = this.m_expander.Size;
      int height = size.Height;
      this.Size = new Size(width, height);
      this.m_panel.Enabled = false;
    }

    [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
    protected override bool ProcessMnemonic(char charCode)
    {
      if (!Control.IsMnemonic(charCode, this.Text) || !this.CanSelect || this.ContainsFocus)
        return base.ProcessMnemonic(charCode);
      if (!this.m_expander.Expanded)
        this.m_expander.Expanded = true;
      this.SelectNextControl((Control) this.m_expander, true, true, true, true);
      return true;
    }

    private int HeightPad => this.m_expander.Expanded ? this.Height - this.m_panel.Height : this.m_heightPad;

    private void InitializeComponent()
    {
      this.m_panel = new Panel();
      this.m_expander = new ExpanderButton();
      this.SuspendLayout();
      this.m_panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.m_panel.Location = new Point(19, DpiHelper.LogicalToDeviceUnitsY(24));
      this.m_panel.Name = "m_panel";
      this.m_panel.Size = new Size(288, 85);
      this.m_panel.TabIndex = 1;
      this.m_panel.Margin = new Padding(0);
      this.m_expander.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.m_expander.AutoCheck = false;
      this.m_expander.Expanded = false;
      this.m_expander.Location = new Point(0, 0);
      this.m_expander.Name = "m_expander";
      this.m_expander.Size = new Size(307, 24).LogicalToDeviceUnits();
      this.m_expander.TabIndex = 0;
      this.Controls.Add((Control) this.m_panel);
      this.Controls.Add((Control) this.m_expander);
      this.Name = nameof (Expandex);
      this.Size = new Size(324, 85 + DpiHelper.LogicalToDeviceUnitsY(24));
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
