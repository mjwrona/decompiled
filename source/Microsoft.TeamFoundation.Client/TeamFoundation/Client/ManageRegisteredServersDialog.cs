// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ManageRegisteredServersDialog
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  internal class ManageRegisteredServersDialog : BaseDialog
  {
    private Label _serverListLable;
    private Button _addButton;
    private Button _removeButton;
    private Button _closeButton;
    private ColumnHeader _serverName;
    private ColumnHeader _serverUrl;
    private ListView _serverList;
    private System.ComponentModel.Container components;
    private TableLayoutPanel overarchingTableLayoutPanel;
    private TfsConnection m_activeServer;
    private const string cHelpTopic = "vs.tfc.addremovetfsdialog";

    public event ServerEventHandler ServerAdded;

    public event ServerEventHandler ServerRemoved;

    public ManageRegisteredServersDialog(TfsTeamProjectCollection activeServer)
    {
      this.m_activeServer = (TfsConnection) activeServer;
      if (activeServer != null && activeServer.CatalogNode != null)
        this.m_activeServer = (TfsConnection) activeServer.ConfigurationServer;
      this.InitializeComponent();
      this.HelpTopic = "vs.tfc.addremovetfsdialog";
      List<TfsConnection> tfsConnectionList = new List<TfsConnection>();
      foreach (RegisteredConfigurationServer configurationServer in RegisteredTfsConnections.GetConfigurationServers())
        tfsConnectionList.Add((TfsConnection) TfsConfigurationServerFactory.GetConfigurationServer(configurationServer));
      foreach (RegisteredProjectCollection projectCollection in RegisteredTfsConnections.GetLegacyProjectCollections())
        tfsConnectionList.Add((TfsConnection) TfsTeamProjectCollectionFactory.GetTeamProjectCollection(projectCollection));
      foreach (TfsConnection tfsConnection in tfsConnectionList)
        this._serverList.Items.Add(new ListViewItem(new string[2]
        {
          tfsConnection.Name,
          tfsConnection.Uri.AbsoluteUri
        })
        {
          Tag = (object) tfsConnection
        });
      if (this._serverList.Items.Count > 0)
        this._serverList.Items[0].Selected = true;
      this.ServerList_SelectedIndexChanged((object) null, (EventArgs) null);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ManageRegisteredServersDialog));
      this._serverList = new ListView();
      this._serverName = new ColumnHeader();
      this._serverUrl = new ColumnHeader();
      this._serverListLable = new Label();
      this._addButton = new Button();
      this._removeButton = new Button();
      this._closeButton = new Button();
      this.overarchingTableLayoutPanel = new TableLayoutPanel();
      this.overarchingTableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      this._serverList.AllowColumnReorder = true;
      this._serverList.Columns.AddRange(new ColumnHeader[2]
      {
        this._serverName,
        this._serverUrl
      });
      this._serverList.FullRowSelect = true;
      this._serverList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
      this._serverList.HideSelection = false;
      componentResourceManager.ApplyResources((object) this._serverList, "_serverList");
      this._serverList.Margin = new Padding(0, 2, 3, 0);
      this._serverList.MultiSelect = false;
      this._serverList.Name = "_serverList";
      this.overarchingTableLayoutPanel.SetRowSpan((Control) this._serverList, 3);
      this._serverList.Sorting = SortOrder.Ascending;
      this._serverList.View = View.Details;
      this._serverList.SelectedIndexChanged += new EventHandler(this.ServerList_SelectedIndexChanged);
      componentResourceManager.ApplyResources((object) this._serverName, "_serverName");
      this._serverName.Width = DpiHelper.LogicalToDeviceUnitsX(this._serverName.Width);
      componentResourceManager.ApplyResources((object) this._serverUrl, "_serverUrl");
      this._serverUrl.Width = DpiHelper.LogicalToDeviceUnitsX(this._serverUrl.Width);
      componentResourceManager.ApplyResources((object) this._serverListLable, "_serverListLable");
      this._serverListLable.Margin = new Padding(0);
      this._serverListLable.Name = "_serverListLable";
      componentResourceManager.ApplyResources((object) this._addButton, "_addButton");
      this._addButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this._addButton.Margin = new Padding(3, 2, 0, 3);
      this._addButton.MinimumSize = new Size(75, 23);
      this._addButton.Name = "_addButton";
      this._addButton.Padding = new Padding(10, 0, 10, 0);
      this._addButton.Click += new EventHandler(this.addButton_Click);
      componentResourceManager.ApplyResources((object) this._removeButton, "_removeButton");
      this._removeButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this._removeButton.Margin = new Padding(3, 3, 0, 3);
      this._removeButton.MinimumSize = new Size(75, 23);
      this._removeButton.Name = "_removeButton";
      this._removeButton.Padding = new Padding(10, 0, 10, 0);
      this._removeButton.Click += new EventHandler(this.removeButton_Click);
      componentResourceManager.ApplyResources((object) this._closeButton, "_closeButton");
      this._closeButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this._closeButton.DialogResult = DialogResult.Cancel;
      this._closeButton.Margin = new Padding(3, 3, 0, 0);
      this._closeButton.MinimumSize = new Size(75, 23);
      this._closeButton.Name = "_closeButton";
      this._closeButton.Padding = new Padding(10, 0, 10, 0);
      this._closeButton.Click += new EventHandler(this._closeButton_Click);
      componentResourceManager.ApplyResources((object) this.overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
      this.overarchingTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
      this.overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
      this.overarchingTableLayoutPanel.Controls.Add((Control) this._serverList, 0, 1);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this._closeButton, 1, 3);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this._serverListLable, 0, 0);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this._removeButton, 1, 2);
      this.overarchingTableLayoutPanel.Controls.Add((Control) this._addButton, 1, 1);
      this.overarchingTableLayoutPanel.Margin = new Padding(12);
      this.overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
      this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
      this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
      this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
      this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
      this.AcceptButton = (IButtonControl) this._closeButton;
      componentResourceManager.ApplyResources((object) this, "$this");
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.CancelButton = (IButtonControl) this._closeButton;
      this.Controls.Add((Control) this.overarchingTableLayoutPanel);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.HelpButton = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ManageTfsList";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.overarchingTableLayoutPanel.ResumeLayout(false);
      this.overarchingTableLayoutPanel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private void ServerList_SelectedIndexChanged(object sender, EventArgs e)
    {
      try
      {
        bool flag = false;
        if (this._serverList.SelectedIndices.Count > 0)
        {
          TfsConnection tag = (TfsConnection) this._serverList.Items[this._serverList.SelectedIndices[0]].Tag;
          if (tag != null)
          {
            if (this.m_activeServer == null)
              flag = true;
            else if (!UriUtility.Equals(this.m_activeServer.Uri, tag.Uri))
              flag = true;
          }
        }
        this._removeButton.Enabled = flag;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    private void _closeButton_Click(object sender, EventArgs e) => this.Close();

    private void removeButton_Click(object sender, EventArgs e)
    {
      try
      {
        TfsConnection tag = (TfsConnection) this._serverList.Items[this._serverList.SelectedIndices[0]].Tag;
        if (UIHost.ShowMessageBox(ClientResources.RemoveServer((object) tag.Name), (string) null, ClientResources.RemoveServerTitle(), MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
          return;
        if (tag is TfsConfigurationServer configurationServer)
          TfsConfigurationServerManager.Remove(configurationServer);
        else
          RegisteredTfsConnections.UnregisterConfigurationServer(tag.Name);
        this._serverList.Items.RemoveAt(this._serverList.SelectedIndices[0]);
        if (this.ServerRemoved == null)
          return;
        this.ServerRemoved((object) this, new ServerEventArgs(tag));
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void addButton_Click(object sender, EventArgs e)
    {
      try
      {
        using (AddDomainDialog addDomainDialog = new AddDomainDialog())
        {
          addDomainDialog.OnAddServer += new OnAddServerEventHandler(this.OnAddServerValidate);
          if (addDomainDialog.ShowDialog((IWin32Window) this) != DialogResult.OK)
            return;
          TfsConnection server = addDomainDialog.Server;
          ListViewItem listViewItem = new ListViewItem(new string[2]
          {
            server.Name,
            server.Uri.AbsoluteUri
          });
          listViewItem.Tag = (object) server;
          this._serverList.Items.Insert(0, listViewItem);
          listViewItem.Selected = true;
          if (this.ServerAdded == null)
            return;
          this.ServerAdded((object) this, new ServerEventArgs(server));
        }
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void OnAddServerValidate(object sender, OnAddServerEventArgs eventArgs)
    {
      foreach (ListViewItem listViewItem in this._serverList.Items)
      {
        if ((TfsConnection) listViewItem.Tag == eventArgs.Server)
        {
          eventArgs.Cancel = true;
          string caption = ClientResources.DomainNameAlreadyExistsTitle();
          int num = (int) UIHost.ShowError((IWin32Window) this, ClientResources.DomainNameAlreadyExists((object) eventArgs.Server.Name), (string) null, caption);
          break;
        }
      }
    }
  }
}
