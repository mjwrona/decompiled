// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Controls.DeleteNodesDialog
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Server;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Controls
{
  internal class DeleteNodesDialog : BaseDialog
  {
    private NodeInfo _selectedNodeInfo;
    private ComponentResourceManager resources;
    private IContainer components;
    private TableLayoutPanel tableLayoutPanel1;
    private Label Info_label;
    private Label Select_node_text;
    private Button ok_button;
    private Button cancel_button;
    private CssDataProvider provider;
    private CssHierarchyControl _cssPicker;
    private PictureBox pictureBox1;
    private TableLayoutPanel Ok_cancel_tableLayoutPanel;

    public DeleteNodesDialog(
      TfsTeamProjectCollection tfs,
      string reclassifyUri,
      string skipNodeUri)
    {
      this.InitializeComponent();
      this.pictureBox1.Image = (Image) SystemIcons.Warning.ToBitmap();
      this.pictureBox1.Size = this.pictureBox1.Image.Size;
      NodeInfo node = ((ICommonStructureService) tfs.GetService(typeof (ICommonStructureService))).GetNode(reclassifyUri);
      this.provider = new CssDataProvider(tfs, node.ProjectUri, node.StructureType, node.Path, new string[1]
      {
        skipNodeUri
      });
      this._cssPicker.DataProvider = (ICssDataProvider) this.provider;
      this.HelpTopic = "vs.tfc.deletenodesdialog";
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      Size size = this.Size;
      this.AutoSize = false;
      this.tableLayoutPanel1.AutoSize = false;
      this.tableLayoutPanel1.Dock = DockStyle.Fill;
      this.tableLayoutPanel1.MaximumSize = new Size(0, 0);
      this.MinimumSize = size;
    }

    public NodeInfo SelectedNodeInfo => this._selectedNodeInfo;

    private void ok_button_Click(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(this._cssPicker.Text))
        this._selectedNodeInfo = this.provider.TryGetCssNodeInfo(this._cssPicker.Text);
      if (this._selectedNodeInfo != null)
      {
        this.DialogResult = DialogResult.OK;
      }
      else
      {
        CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
        MessageBoxOptions messageBoxOptions1 = MessageBoxOptions.DefaultDesktopOnly;
        if (currentUiCulture.TextInfo.IsRightToLeft)
        {
          MessageBoxOptions messageBoxOptions2 = messageBoxOptions1 | MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
        }
        int num = (int) UIHost.ShowMessageBox((IWin32Window) this, ClientResources.EnterValidPath(), (string) null, ClientResources.GenericTeamFoundationCaption(), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
      }
    }

    private void cancel_button_Click(object sender, EventArgs e) => this.DialogResult = DialogResult.Cancel;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.resources = new ComponentResourceManager(typeof (DeleteNodesDialog));
      this.tableLayoutPanel1 = new TableLayoutPanel();
      this.Ok_cancel_tableLayoutPanel = new TableLayoutPanel();
      this.cancel_button = new Button();
      this.ok_button = new Button();
      this.Info_label = new Label();
      this.Select_node_text = new Label();
      this._cssPicker = new CssHierarchyControl();
      this.pictureBox1 = new PictureBox();
      this.tableLayoutPanel1.SuspendLayout();
      this.Ok_cancel_tableLayoutPanel.SuspendLayout();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.SuspendLayout();
      this.resources.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
      this.tableLayoutPanel1.Controls.Add((Control) this.Ok_cancel_tableLayoutPanel, 0, 3);
      this.tableLayoutPanel1.Controls.Add((Control) this.Info_label, 1, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.Select_node_text, 0, 1);
      this.tableLayoutPanel1.Controls.Add((Control) this._cssPicker, 0, 2);
      this.tableLayoutPanel1.Controls.Add((Control) this.pictureBox1, 0, 0);
      this.tableLayoutPanel1.MaximumSize = new Size(287, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.resources.ApplyResources((object) this.Ok_cancel_tableLayoutPanel, "Ok_cancel_tableLayoutPanel");
      this.tableLayoutPanel1.SetColumnSpan((Control) this.Ok_cancel_tableLayoutPanel, 2);
      this.Ok_cancel_tableLayoutPanel.Controls.Add((Control) this.cancel_button, 1, 0);
      this.Ok_cancel_tableLayoutPanel.Controls.Add((Control) this.ok_button, 0, 0);
      this.Ok_cancel_tableLayoutPanel.Name = "Ok_cancel_tableLayoutPanel";
      this.resources.ApplyResources((object) this.cancel_button, "cancel_button");
      this.cancel_button.Name = "cancel_button";
      this.cancel_button.UseVisualStyleBackColor = true;
      this.cancel_button.Click += new EventHandler(this.cancel_button_Click);
      this.resources.ApplyResources((object) this.ok_button, "ok_button");
      this.ok_button.Name = "ok_button";
      this.ok_button.UseVisualStyleBackColor = true;
      this.ok_button.Click += new EventHandler(this.ok_button_Click);
      this.resources.ApplyResources((object) this.Info_label, "Info_label");
      this.Info_label.Name = "Info_label";
      this.resources.ApplyResources((object) this.Select_node_text, "Select_node_text");
      this.tableLayoutPanel1.SetColumnSpan((Control) this.Select_node_text, 2);
      this.Select_node_text.Name = "Select_node_text";
      this.resources.ApplyResources((object) this._cssPicker, "_cssPicker");
      this.tableLayoutPanel1.SetColumnSpan((Control) this._cssPicker, 2);
      this._cssPicker.FormattingEnabled = true;
      this._cssPicker.Name = "_cssPicker";
      this.resources.ApplyResources((object) this.pictureBox1, "pictureBox1");
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.TabStop = false;
      this.AcceptButton = (IButtonControl) this.ok_button;
      this.resources.ApplyResources((object) this, "$this");
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.cancel_button;
      this.Controls.Add((Control) this.tableLayoutPanel1);
      this.HelpButton = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (DeleteNodesDialog);
      this.ShowIcon = false;
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.Ok_cancel_tableLayoutPanel.ResumeLayout(false);
      this.Ok_cancel_tableLayoutPanel.PerformLayout();
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.ResumeLayout(false);
    }
  }
}
