// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.AddDomainDialog
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.Internal.Performance;
using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  internal class AddDomainDialog : BaseDialog
  {
    private Label label1;
    private GroupBox _details;
    private Label _portNumber;
    private Label _protocol;
    private Button _okButton;
    private Button _cancelButton;
    private TextBox _serverName;
    private TextBox _portNumberText;
    private RadioButton _httpButton;
    private RadioButton _httpsButton;
    private TfsConnection _server;
    private string _errorCaption;
    private TableLayoutPanel okCancelTableLayoutPanel;
    private TableLayoutPanel connectionDetailsTableLayoutPanel;
    private TableLayoutPanel overarchingTableLayoutPanel;
    private Label _previewLabel;
    private TextBox _pathText;
    private Label _path;
    private TableLayoutPanel _previewPanel;
    private TextBox _previewText;
    private Cursor _originalCursor;
    private AddDomainDialogDataSource _datasource;
    private System.ComponentModel.Container components;

    public event OnAddServerEventHandler OnAddServer;

    public AddDomainDialog()
    {
      this._datasource = new AddDomainDialogDataSource((BaseDialog) this);
      this.InitializeComponent();
      this.InvalidateDataSource();
      this._datasource.PropertyChanged += new PropertyChangedEventHandler(this.Datasource_PropertyChanged);
      this._errorCaption = (UIHost.EnvironmentFlags & RuntimeEnvironmentFlags.Vsip) != RuntimeEnvironmentFlags.None ? ClientResources.MicrosoftVisualStudioCaption() : ClientResources.GenericTeamFoundationCaption();
      this.HelpTopic = "vs.tfc.connecttotfsdialog";
    }

    public TfsConnection Server => this._server;

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.components != null)
          this.components.Dispose();
        if (this._datasource != null)
          this._datasource.StopConnect();
      }
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (AddDomainDialog));
      this.label1 = new Label();
      this._serverName = new TextBox();
      this._details = new GroupBox();
      this.connectionDetailsTableLayoutPanel = new TableLayoutPanel();
      this._pathText = new TextBox();
      this._path = new Label();
      this._protocol = new Label();
      this._httpsButton = new RadioButton();
      this._portNumberText = new TextBox();
      this._httpButton = new RadioButton();
      this._portNumber = new Label();
      this._previewLabel = new Label();
      this._okButton = new Button();
      this._cancelButton = new Button();
      this.okCancelTableLayoutPanel = new TableLayoutPanel();
      this.overarchingTableLayoutPanel = new TableLayoutPanel();
      this._previewPanel = new TableLayoutPanel();
      this._previewText = new TextBox();
      this._details.SuspendLayout();
      this.connectionDetailsTableLayoutPanel.SuspendLayout();
      this.okCancelTableLayoutPanel.SuspendLayout();
      this.overarchingTableLayoutPanel.SuspendLayout();
      this._previewPanel.SuspendLayout();
      this.SuspendLayout();
      componentResourceManager.ApplyResources((object) this.label1, "label1");
      this.label1.FlatStyle = FlatStyle.System;
      this.label1.Name = "label1";
      componentResourceManager.ApplyResources((object) this._serverName, "_serverName");
      this._serverName.Name = "_serverName";
      this._serverName.TextChanged += new EventHandler(this.ServerNameChanged);
      componentResourceManager.ApplyResources((object) this._details, "_details");
      this._details.Controls.Add((Control) this.connectionDetailsTableLayoutPanel);
      this._details.Name = "_details";
      this._details.TabStop = false;
      componentResourceManager.ApplyResources((object) this.connectionDetailsTableLayoutPanel, "connectionDetailsTableLayoutPanel");
      this.connectionDetailsTableLayoutPanel.Controls.Add((Control) this._pathText, 1, 0);
      this.connectionDetailsTableLayoutPanel.Controls.Add((Control) this._path, 0, 0);
      this.connectionDetailsTableLayoutPanel.Controls.Add((Control) this._protocol, 0, 2);
      this.connectionDetailsTableLayoutPanel.Controls.Add((Control) this._httpsButton, 2, 2);
      this.connectionDetailsTableLayoutPanel.Controls.Add((Control) this._portNumberText, 1, 1);
      this.connectionDetailsTableLayoutPanel.Controls.Add((Control) this._httpButton, 1, 2);
      this.connectionDetailsTableLayoutPanel.Controls.Add((Control) this._portNumber, 0, 1);
      this.connectionDetailsTableLayoutPanel.Name = "connectionDetailsTableLayoutPanel";
      this.connectionDetailsTableLayoutPanel.SetColumnSpan((Control) this._pathText, 2);
      componentResourceManager.ApplyResources((object) this._pathText, "_pathText");
      this._pathText.Name = "_pathText";
      this._pathText.TextChanged += new EventHandler(this.PathChanged);
      componentResourceManager.ApplyResources((object) this._path, "_path");
      this._path.FlatStyle = FlatStyle.System;
      this._path.Name = "_path";
      componentResourceManager.ApplyResources((object) this._protocol, "_protocol");
      this._protocol.FlatStyle = FlatStyle.System;
      this._protocol.Name = "_protocol";
      componentResourceManager.ApplyResources((object) this._httpsButton, "_httpsButton");
      this._httpsButton.Name = "_httpsButton";
      this._httpsButton.CheckedChanged += new EventHandler(this.IsHttpsChanged);
      componentResourceManager.ApplyResources((object) this._portNumberText, "_portNumberText");
      this._portNumberText.Name = "_portNumberText";
      this._portNumberText.TextChanged += new EventHandler(this.PortChanged);
      componentResourceManager.ApplyResources((object) this._httpButton, "_httpButton");
      this._httpButton.Checked = true;
      this._httpButton.Name = "_httpButton";
      this._httpButton.TabStop = true;
      this._httpButton.CheckedChanged += new EventHandler(this.IsHttpsChanged);
      componentResourceManager.ApplyResources((object) this._portNumber, "_portNumber");
      this._portNumber.FlatStyle = FlatStyle.System;
      this._portNumber.Name = "_portNumber";
      componentResourceManager.ApplyResources((object) this._previewLabel, "_previewLabel");
      this._previewLabel.FlatStyle = FlatStyle.System;
      this._previewLabel.Name = "_previewLabel";
      componentResourceManager.ApplyResources((object) this._okButton, "_okButton");
      this._okButton.MinimumSize = new Size(75, 23);
      this._okButton.Name = "_okButton";
      this._okButton.Click += new EventHandler(this._okButton_Click);
      componentResourceManager.ApplyResources((object) this._cancelButton, "_cancelButton");
      this._cancelButton.DialogResult = DialogResult.Cancel;
      this._cancelButton.MinimumSize = new Size(75, 23);
      this._cancelButton.Name = "_cancelButton";
      componentResourceManager.ApplyResources((object) this.okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
      this.okCancelTableLayoutPanel.Controls.Add((Control) this._okButton, 0, 0);
      this.okCancelTableLayoutPanel.Controls.Add((Control) this._cancelButton, 1, 0);
      this.okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
      componentResourceManager.ApplyResources((object) this.overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.label1, 0, 0);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this._serverName, 0, 1);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this._details, 0, 2);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this._previewPanel, 0, 3);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this.okCancelTableLayoutPanel, 0, 4);
      this.overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
      componentResourceManager.ApplyResources((object) this._previewPanel, "_previewPanel");
      this._previewPanel.Controls.Add((Control) this._previewText, 1, 0);
      this._previewPanel.Controls.Add((Control) this._previewLabel, 0, 0);
      this._previewPanel.Name = "_previewPanel";
      componentResourceManager.ApplyResources((object) this._previewText, "_previewText");
      this._previewText.Name = "_previewText";
      this._previewText.ReadOnly = true;
      this._previewText.TabStop = false;
      this.AcceptButton = (IButtonControl) this._okButton;
      componentResourceManager.ApplyResources((object) this, "$this");
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this._cancelButton;
      this.Controls.Add((Control) this.overarchingTableLayoutPanel);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.HelpButton = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (AddDomainDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this._details.ResumeLayout(false);
      this._details.PerformLayout();
      this.connectionDetailsTableLayoutPanel.ResumeLayout(false);
      this.connectionDetailsTableLayoutPanel.PerformLayout();
      this.okCancelTableLayoutPanel.ResumeLayout(false);
      this.okCancelTableLayoutPanel.PerformLayout();
      this.overarchingTableLayoutPanel.ResumeLayout(false);
      this.overarchingTableLayoutPanel.PerformLayout();
      this._previewPanel.ResumeLayout(false);
      this._previewPanel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
      this._originalCursor = this.Cursor;
    }

    private void InvalidateDataSource()
    {
      this._portNumberText.Text = this._datasource.Port;
      this._pathText.Text = this._datasource.Path;
      this._previewText.Text = this._datasource.Preview;
      this._httpButton.Checked = !this._datasource.IsHttps;
      this._httpsButton.Checked = this._datasource.IsHttps;
      this._serverName.Text = this._datasource.ServerName;
      this._okButton.Enabled = this._datasource.IsInputValid;
    }

    private void Datasource_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "Port":
          this._portNumberText.Text = this._datasource.Port;
          break;
        case "Preview":
          this._previewText.Text = this._datasource.Preview;
          break;
        case "IsUrlEntered":
          this._details.Enabled = !this._datasource.IsUrlEntered;
          break;
        case "IsHostedUrl":
          this._details.Enabled = !this._datasource.IsHostedUrl;
          break;
        case "IsInputValid":
          this._okButton.Enabled = this._datasource.IsInputValid;
          break;
      }
    }

    private void RestoreCursor() => this.Cursor = this._originalCursor;

    private void SetWaitCursor()
    {
      this.Cursor = Cursors.WaitCursor;
      this._cancelButton.Cursor = this._originalCursor;
    }

    private void UseWaitCursorAndDisableControls()
    {
      this.SetWaitCursor();
      this._serverName.Enabled = false;
      this._details.Enabled = false;
      this._okButton.Enabled = false;
    }

    private void RestoreCursorAndEnableControls()
    {
      this.RestoreCursor();
      this._serverName.Enabled = true;
      this._details.Enabled = true;
      this._okButton.Enabled = true;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      Size size1 = this._protocol.Size;
      int width1 = size1.Width;
      size1 = this._httpButton.Size;
      int width2 = size1.Width;
      int num1 = width1 + width2;
      size1 = this._httpsButton.Size;
      int width3 = size1.Width;
      int width4 = num1 + width3 + this._protocol.Margin.Horizontal + this._httpButton.Margin.Horizontal + this._httpsButton.Margin.Horizontal + this.connectionDetailsTableLayoutPanel.Margin.Horizontal + this.overarchingTableLayoutPanel.Margin.Horizontal;
      Size size2 = this._protocol.Size;
      int width5 = size2.Width;
      size2 = this._httpButton.Size;
      int width6 = size2.Width;
      int num2 = width5 + width6;
      size2 = this._httpsButton.Size;
      int width7 = size2.Width;
      int width8 = num2 + width7 + this._protocol.Margin.Horizontal + this._httpButton.Margin.Horizontal + this._httpsButton.Margin.Horizontal + this.connectionDetailsTableLayoutPanel.Margin.Horizontal;
      this.overarchingTableLayoutPanel.MinimumSize = new Size(width4, this.overarchingTableLayoutPanel.Size.Height);
      this.connectionDetailsTableLayoutPanel.MinimumSize = new Size(width8, this.connectionDetailsTableLayoutPanel.Size.Height);
      this.MinimumSize = this.overarchingTableLayoutPanel.MinimumSize;
    }

    protected override void OnClosed(EventArgs e)
    {
      this._datasource.StopConnect();
      base.OnClosed(e);
    }

    private void _okButton_Click(object sender, EventArgs e)
    {
      try
      {
        this.UseWaitCursorAndDisableControls();
        CodeMarkers.Instance.CodeMarker(9839);
        this._datasource.StartConnect(new Action<TfsConnection, Exception>(this.HandleConnectionCompleted));
      }
      catch (Exception ex)
      {
        this.RestoreCursorAndEnableControls();
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void GetMessageAndHelpTopicFromException(
      Exception ex,
      TfsConnection server,
      out string reason,
      out string helpTopic)
    {
      ConnectFailureReason reason1 = ConnectFailureReason.GetReason(ex);
      reason = reason1.GetErrorMessage(server.Name);
      helpTopic = reason1.HelpTopic;
      if (ex is TeamFoundationServerNotSupportedException)
      {
        reason = ex.Message;
      }
      else
      {
        if (reason1.Category != ConnectFailureCategory.ConnectFailure && reason1.Category != ConnectFailureCategory.Unknown)
          return;
        if (ex is WebException || ex.InnerException is WebException)
        {
          reason = ClientResources.ConnectToTfs_AddServer_UnableToConnect_WithTechnicalInfo((object) server.Name, (object) server.Uri.AbsoluteUri, ex is WebException ? (object) ex.Message : (object) ex.InnerException.Message);
        }
        else
        {
          if (ex is TeamFoundationServiceUnavailableException)
            return;
          reason = ClientResources.ConnectToTfs_AddServer_UnableToConnect((object) server.Name, (object) server.Uri.AbsoluteUri);
        }
      }
    }

    private void HandleConnectionCompleted(TfsConnection server, Exception error)
    {
      if (this.Disposing || this.IsDisposed)
        return;
      this.RestoreCursorAndEnableControls();
      if (error != null)
      {
        string reason;
        string helpTopic;
        this.GetMessageAndHelpTopicFromException(error, server, out reason, out helpTopic);
        int num = (int) UIHost.ShowMessageBox((IWin32Window) this, reason, helpTopic, this._errorCaption, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
      }
      else
        this.OnServerAdded(server);
    }

    private void OnServerAdded(TfsConnection server)
    {
      if (this.OnAddServer != null)
      {
        OnAddServerEventArgs e = new OnAddServerEventArgs(server);
        this.OnAddServer((object) this, e);
        if (e.Cancel)
          return;
      }
      if (server is TfsConfigurationServer configurationServer)
        RegisteredTfsConnections.RegisterConfigurationServer(configurationServer);
      else
        RegisteredTfsConnections.RegisterProjectCollection(server as TfsTeamProjectCollection);
      this._server = server;
      this.DialogResult = DialogResult.OK;
      this.Close();
      CodeMarkers.Instance.CodeMarker(9840);
    }

    private void ServerNameChanged(object sender, EventArgs e)
    {
      try
      {
        this._datasource.ServerName = this._serverName.Text;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    private void PortChanged(object sender, EventArgs e)
    {
      try
      {
        this._datasource.Port = this._portNumberText.Text;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    private void PathChanged(object sender, EventArgs e)
    {
      try
      {
        this._datasource.Path = this._pathText.Text;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    private void IsHttpsChanged(object sender, EventArgs e)
    {
      try
      {
        this._datasource.IsHttps = this._httpsButton.Checked;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }
  }
}
