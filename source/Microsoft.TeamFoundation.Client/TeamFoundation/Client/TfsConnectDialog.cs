// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsConnectDialog
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.Internal.Performance;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  internal class TfsConnectDialog : BaseDialog
  {
    private TfsConnectDataSource m_dataSource;
    private bool m_isUpdating;
    private bool m_suspendItemCheckEvent;
    private TfsConnectDataSource.ServerNode m_lastAddedServer;
    private bool m_autoCommitConnectionChanges;
    private CheckedListBox m_teamProjectsCheckedListBox;
    private ListBox m_teamProjectsListBox;
    private HashSet<INotifyPropertyChanged> m_hookedCollectionsEvent;
    private VssClientCredentialStorage m_credentialStorage = new VssClientCredentialStorage();
    private Dictionary<Uri, string> m_connectionUserMap = new Dictionary<Uri, string>(UriUtility.AbsoluteUriStringComparer);
    private const string FORM_CHOOSE_PROJECT_MULTISELECT = "ConnectToTfsMultiSelect";
    private const string FORM_CHOOSE_PROJECT_SINGLESELECT = "ConnectToTfsSingleSelect";
    private const string FORM_NO_PROJECT = "ConnectToTfsNoProjectList";
    private const int cProgressSpeed = 100;
    private const string cConnectDialogSubKey = "TeamFoundation\\ConnectDialog";
    private const string cSplitterDistanceValueName = "SplitterDistance";
    private IContainer components;
    private TableLayoutPanel m_mainPanel;
    private TableLayoutPanel m_domainPanel;
    private ComboBox m_domainComboBox;
    private Button m_serversButton;
    private TableLayoutPanel m_commandButtonsPanel;
    private Button m_connectButton;
    private Button m_cancelButton;
    private SplitContainer m_middlePanel;
    private Panel m_directoryPanel;
    private Label m_directoryLabel;
    private Panel m_teamProjectsPanel;
    private Label m_teamProjectsLabel;
    private Label m_selectServerLabel;
    private InformationBar m_directoryInfoBar;
    private InformationBar m_projectsInfoBar;
    private CheckBox m_projectSelectAll;
    private Panel m_selectAllPanel;
    private TreeView m_directoryTree;
    private ImageList m_directoryImageList;
    private TableLayoutPanel m_bottomPanel;
    private LinkLabel m_userLabel;
    private ToolTip m_toolTip;

    public TfsConnectDialog(TfsConnectDataSource dataSource)
    {
      this.HelpTopic = "vs.tfc.connecttotfsdialog";
      this.m_dataSource = dataSource;
      this.m_autoCommitConnectionChanges = true;
      this.m_hookedCollectionsEvent = new HashSet<INotifyPropertyChanged>();
      this.InitializeComponent();
      this.SetupControls();
    }

    public static void CommitConnectionChange(
      TfsTeamProjectCollection collection,
      bool storeCredentials)
    {
      if (collection == null)
        return;
      TfsConfigurationServer configurationServer = collection.ConfigurationServer;
      if (configurationServer != null)
      {
        if (storeCredentials)
          TfsConfigurationServerManager.SwitchUser(configurationServer);
        else
          TfsConfigurationServerFactory.ReplaceConfigurationServer(configurationServer);
      }
      TfsTeamProjectCollectionFactory.ReplaceTeamProjectCollection(collection);
      RegisteredTfsConnections.RegisterProjectCollection(collection);
    }

    public string AcceptButtonText
    {
      get => this.m_connectButton.Text;
      set => this.m_connectButton.Text = value;
    }

    public bool AutoCommitConnectionChanges
    {
      get => this.m_autoCommitConnectionChanges;
      set => this.m_autoCommitConnectionChanges = value;
    }

    public event CancelEventHandler AcceptButtonClick;

    private void SelectedServerChanged(object sender, EventArgs e)
    {
      try
      {
        if (this.m_isUpdating)
        {
          this.UpdateUserLabel();
        }
        else
        {
          this.m_dataSource.SelectedServer = this.m_domainComboBox.SelectedItem as TfsConnectDataSource.ServerNode;
          this.HandleNodeChange(TfsConnectDialog.DialogPanel.Directory, (TfsConnectDataSource.ContainerNode) this.m_dataSource.SelectedServer);
          this.UpdateUserLabel();
        }
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void SelectedDirectoryNodeChanged(object sender, EventArgs e)
    {
      try
      {
        this.HandleNodeChange(TfsConnectDialog.DialogPanel.Projects, (TfsConnectDataSource.ContainerNode) this.m_dataSource.SelectedCollection);
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void ServerNodeStateChanged(object sender, PropertyChangedEventArgs e)
    {
      if (this.m_dataSource.SelectedServer == null || !(e.PropertyName == "State") || this.m_dataSource.SelectedServer != sender)
        return;
      this.SelectedServerChanged((object) this, EventArgs.Empty);
    }

    private void TPCNodeStateChanged(object sender, PropertyChangedEventArgs e)
    {
      if (this.m_dataSource.SelectedServer == null || !(e.PropertyName == "State") || this.m_dataSource.SelectedCollection == null || this.m_dataSource.SelectedCollection != sender)
        return;
      this.SelectedDirectoryNodeChanged((object) this, EventArgs.Empty);
    }

    private void m_teamProjectsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
    {
      try
      {
        if (this.m_suspendItemCheckEvent)
          return;
        if (e.NewValue == CheckState.Unchecked)
          this.m_dataSource.SelectedProjects.Remove(this.m_teamProjectsCheckedListBox.Items[e.Index] as TfsConnectDataSource.TeamProjectNode);
        else
          this.m_dataSource.SelectedProjects.Add(this.m_teamProjectsCheckedListBox.Items[e.Index] as TfsConnectDataSource.TeamProjectNode);
        int count = this.m_teamProjectsCheckedListBox.Items.Count;
        for (int index = 0; index < count; ++index)
        {
          if (index != e.Index && this.m_teamProjectsCheckedListBox.GetItemChecked(index) != (e.NewValue == CheckState.Checked))
          {
            this.m_projectSelectAll.CheckState = CheckState.Indeterminate;
            return;
          }
        }
        this.m_projectSelectAll.CheckState = e.NewValue;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    private void m_projectSelectAll_Click(object sender, EventArgs e)
    {
      try
      {
        this.m_suspendItemCheckEvent = true;
        int count = this.m_teamProjectsCheckedListBox.Items.Count;
        for (int index = 0; index < count; ++index)
          this.m_teamProjectsCheckedListBox.SetItemChecked(index, this.m_projectSelectAll.Checked);
        if (this.m_projectSelectAll.Checked)
        {
          foreach (TfsConnectDataSource.TeamProjectNode selected in (Collection<TfsConnectDataSource.INode>) this.m_dataSource.SelectedCollection)
          {
            if (!this.m_dataSource.SelectedProjects.Contains(selected))
              this.m_dataSource.SelectedProjects.Add(selected);
          }
        }
        else
          this.m_dataSource.SelectedProjects.Clear();
        if (this.m_teamProjectsCheckedListBox.SelectedIndex >= 0)
          this.m_teamProjectsCheckedListBox.SetSelected(this.m_teamProjectsCheckedListBox.SelectedIndex, false);
        this.m_suspendItemCheckEvent = false;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    private void m_projectsInfoBar_LabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      try
      {
        this.m_dataSource.SelectedCollection.Refresh();
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void m_directoryInfoBar_LabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      try
      {
        if (this.m_dataSource.SelectedServer == null)
          return;
        this.m_dataSource.SelectedServer.Refresh();
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void m_userLabel_LabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      try
      {
        if (this.m_dataSource.SelectedServer == null)
          return;
        this.m_dataSource.SelectedServer.RefreshWithNewCredentials();
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void m_connectButton_Click(object sender, EventArgs e)
    {
      try
      {
        if (this.m_dataSource.SelectedCollection == null)
          return;
        if (this.AcceptButtonClick != null)
        {
          CancelEventArgs e1 = new CancelEventArgs(false);
          this.AcceptButtonClick((object) this, e1);
          if (e1.Cancel)
          {
            this.DialogResult = DialogResult.None;
            return;
          }
          this.DialogResult = DialogResult.OK;
        }
        if (this.m_autoCommitConnectionChanges)
          TfsConnectDialog.CommitConnectionChange(this.m_dataSource.SelectedCollection.Server, this.m_dataSource.SelectedServer.HasNewCredentials);
        this.Close();
      }
      catch (Exception ex)
      {
        this.DialogResult = DialogResult.None;
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void m_serversButton_Click(object sender, EventArgs e)
    {
      try
      {
        using (ManageRegisteredServersDialog registeredServersDialog = new ManageRegisteredServersDialog(this.m_dataSource.ActiveServer))
        {
          registeredServersDialog.ServerAdded += new ServerEventHandler(this.ServerAdded);
          registeredServersDialog.ServerRemoved += new ServerEventHandler(this.ServerRemoved);
          int num = (int) registeredServersDialog.ShowDialog((IWin32Window) this);
        }
        if (this.m_lastAddedServer != null)
        {
          this.m_dataSource.SelectedServer = this.m_lastAddedServer;
          this.m_lastAddedServer = (TfsConnectDataSource.ServerNode) null;
        }
        this.PopulateServerList();
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void ServerAdded(object sender, ServerEventArgs sae) => this.m_lastAddedServer = this.m_dataSource.AddServer(sae.Server);

    private void ServerRemoved(object sender, ServerEventArgs sae)
    {
      TfsConnectDataSource.ServerNode serverNode = (TfsConnectDataSource.ServerNode) null;
      foreach (TfsConnectDataSource.ServerNode registeredServer in (Collection<TfsConnectDataSource.ServerNode>) this.m_dataSource.RegisteredServers)
      {
        if (VssStringComparer.DomainName.Compare(registeredServer.Name, sae.Server.Name) == 0)
        {
          serverNode = registeredServer;
          break;
        }
      }
      if (this.m_domainComboBox.SelectedItem == serverNode)
      {
        if (this.m_lastAddedServer == serverNode)
          this.m_lastAddedServer = (TfsConnectDataSource.ServerNode) null;
        this.m_dataSource.SelectedServer = (TfsConnectDataSource.ServerNode) null;
      }
      this.m_dataSource.RegisteredServers.Remove(serverNode);
    }

    private void m_directoryTree_AfterSelect(object sender, TreeViewEventArgs e)
    {
      try
      {
        this.m_dataSource.SelectedDirectoryNode = this.m_directoryTree.SelectedNode != null ? this.m_directoryTree.SelectedNode.Tag as TfsConnectDataSource.ContainerNode : (TfsConnectDataSource.ContainerNode) null;
        this.SelectedDirectoryNodeChanged((object) this, EventArgs.Empty);
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void m_teamProjectsListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      try
      {
        if (this.m_suspendItemCheckEvent)
          return;
        this.m_dataSource.SelectedProjects.Clear();
        this.m_dataSource.SelectedProjects.Add(this.m_teamProjectsListBox.SelectedItem as TfsConnectDataSource.TeamProjectNode);
        this.EnableDisableConnectButton();
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void TfsConnectDialog_FormClosed(object sender, FormClosedEventArgs e)
    {
      try
      {
        if (!this.m_dataSource.CanSelectProject)
          return;
        this.StoredSplitterDistance = DpiHelper.DeviceToLogicalUnitsX(this.m_middlePanel.SplitterDistance);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    private void m_teamProjectsListBox_DoubleClick(object sender, EventArgs e)
    {
      try
      {
        if (this.m_teamProjectsListBox.IndexFromPoint(this.m_teamProjectsListBox.PointToClient(Control.MousePosition)) < 0)
          return;
        this.m_connectButton.PerformClick();
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    private void m_projectSelectAll_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
    {
      try
      {
        if (e.KeyData != Keys.Down || sender != this.m_projectSelectAll)
          return;
        this.m_teamProjectsCheckedListBox.SetSelected(0, true);
        this.m_teamProjectsCheckedListBox.TopIndex = 0;
        this.m_teamProjectsCheckedListBox.Focus();
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    private void m_teamProjectsCheckedListBox_KeyDown(object sender, KeyEventArgs e)
    {
      try
      {
        if (e.KeyData != Keys.Up || this.m_teamProjectsCheckedListBox.SelectedIndex != 0)
          return;
        this.m_teamProjectsCheckedListBox.SetSelected(0, false);
        this.m_projectSelectAll.Focus();
        e.Handled = true;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.StyleControls();
      this.PopulateServerList();
      if (this.m_dataSource.CanSelectProject && this.StoredSplitterDistance > 0)
        this.m_middlePanel.SplitterDistance = DpiHelper.LogicalToDeviceUnitsX(this.StoredSplitterDistance);
      this.EnableDisableConnectButton();
    }

    private int StoredSplitterDistance
    {
      get
      {
        int splitterDistance = 0;
        try
        {
          using (RegistryKey userRegistryRoot = UIHost.UserRegistryRoot)
          {
            using (RegistryKey registryKey = userRegistryRoot.OpenSubKey("TeamFoundation\\ConnectDialog"))
            {
              if (registryKey != null)
              {
                splitterDistance = (int) registryKey.GetValue("SplitterDistance", (object) 0);
                if (splitterDistance >= 0)
                {
                  if (splitterDistance <= 10000)
                    goto label_14;
                }
                splitterDistance = 0;
              }
            }
          }
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.TraceException(ex);
        }
label_14:
        return splitterDistance;
      }
      set
      {
        try
        {
          using (RegistryKey userRegistryRoot = UIHost.UserRegistryRoot)
          {
            using (RegistryKey subKey = userRegistryRoot.CreateSubKey("TeamFoundation\\ConnectDialog"))
              subKey?.SetValue("SplitterDistance", (object) value);
          }
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.TraceException(ex);
        }
      }
    }

    private void HandleNodeChange(
      TfsConnectDialog.DialogPanel panel,
      TfsConnectDataSource.ContainerNode selectedNode)
    {
      if (selectedNode == null)
        this.SetPanelStatus(panel, panel == TfsConnectDialog.DialogPanel.Directory ? Microsoft.TeamFoundation.Client.Internal.ClientResources.TfsConnectDialogSelectServerToViewCollections() : Microsoft.TeamFoundation.Client.Internal.ClientResources.TfsConnectDialogSelectCollectionToViewProjects());
      else if (selectedNode.State == RetrievablePropertyState.Uninitialized)
      {
        this.HookEvent((INotifyPropertyChanged) selectedNode, panel == TfsConnectDialog.DialogPanel.Directory ? new PropertyChangedEventHandler(this.ServerNodeStateChanged) : new PropertyChangedEventHandler(this.TPCNodeStateChanged));
        this.SetPanelWorking(panel);
        selectedNode.Refresh();
      }
      else if (selectedNode.State == RetrievablePropertyState.Working)
        this.SetPanelWorking(panel);
      else if (selectedNode.State == RetrievablePropertyState.Info)
        this.SetPanelStatus(panel, selectedNode.InfoMessage);
      else if (selectedNode.State == RetrievablePropertyState.Error)
      {
        TeamFoundationServerUnauthorizedException error = selectedNode.Error as TeamFoundationServerUnauthorizedException;
        TfsConnectDataSource.ServerNode serverNode = selectedNode as TfsConnectDataSource.ServerNode;
        bool flag = false;
        if (serverNode != null && serverNode.CredentialsSwitchingServer != null)
        {
          serverNode.CredentialsSwitchingServer = (TfsConnection) null;
          flag = error != null && error.AuthenticationError == TeamFoundationAuthenticationError.Cancelled && serverNode != null && serverNode.Server != null && serverNode.Server.HasAuthenticated;
        }
        if (flag)
        {
          serverNode.Refresh();
        }
        else
        {
          this.SetPanelError(panel, selectedNode.Error, selectedNode.Name);
          CodeMarkers.Instance.CodeMarker(9802);
        }
      }
      else
      {
        if (panel == TfsConnectDialog.DialogPanel.Directory)
        {
          TfsConnectDataSource.ServerNode node = selectedNode as TfsConnectDataSource.ServerNode;
          if (node.CredentialsSwitchingServer != null)
          {
            node.Server = node.CredentialsSwitchingServer;
            node.HasNewCredentials = true;
            node.CredentialsSwitchingServer = (TfsConnection) null;
          }
          this.UpdateDirectoryNodes(node);
          this.SelectedDirectoryNodeChanged((object) this, EventArgs.Empty);
        }
        else
          this.UpdateTeamProjectsList((IList<TfsConnectDataSource.INode>) selectedNode);
        CodeMarkers.Instance.CodeMarker(9802);
      }
      this.EnableDisableConnectButton();
    }

    private void HookEvent(INotifyPropertyChanged source, PropertyChangedEventHandler handler)
    {
      if (this.m_hookedCollectionsEvent.Contains(source))
        return;
      this.m_hookedCollectionsEvent.Add(source);
      source.PropertyChanged += handler;
    }

    private void UpdateDirectoryNodes(TfsConnectDataSource.ServerNode node)
    {
      this.ClearPanels();
      this.m_directoryTree.Nodes.Clear();
      TreeNode treeNodeCollection = this.AddINodeToTreeNodeCollection(this.m_directoryTree.Nodes, (TfsConnectDataSource.ContainerNode) node);
      bool flag = true;
      foreach (TreeNode node1 in this.m_directoryTree.Nodes)
      {
        if (node1.Nodes.Count != 0)
        {
          flag = false;
          break;
        }
      }
      this.m_directoryTree.ShowRootLines = !flag;
      this.m_directoryTree.Visible = true;
      this.m_directoryTree.SelectedNode = treeNodeCollection;
    }

    private TreeNode AddINodeToTreeNodeCollection(
      TreeNodeCollection treeNodes,
      TfsConnectDataSource.ContainerNode parentNode)
    {
      TreeNode treeNodeCollection = (TreeNode) null;
      List<TreeNode> treeNodeList = new List<TreeNode>(parentNode.Count);
      foreach (TfsConnectDataSource.ContainerNode containerNode in (Collection<TfsConnectDataSource.INode>) parentNode)
      {
        TreeNode treeNode = new TreeNode(containerNode.Name);
        if (containerNode == this.m_dataSource.SelectedDirectoryNode)
          treeNodeCollection = treeNode;
        treeNode.Tag = (object) containerNode;
        treeNode.ImageIndex = 0;
        treeNode.SelectedImageIndex = 0;
        treeNodeList.Add(treeNode);
      }
      treeNodeList.Sort((Comparison<TreeNode>) ((n1, n2) => string.Compare(n1.ToString(), n2.ToString(), StringComparison.CurrentCultureIgnoreCase)));
      treeNodes.AddRange(treeNodeList.ToArray());
      return treeNodeCollection;
    }

    private void UpdateTeamProjectsList(IList<TfsConnectDataSource.INode> projects)
    {
      this.ClearTeamProjectsPanel();
      try
      {
        this.m_suspendItemCheckEvent = true;
        if (this.m_dataSource.CanSelectMultipleProjects)
        {
          this.m_teamProjectsCheckedListBox.Items.Clear();
          foreach (TfsConnectDataSource.TeamProjectNode project in (IEnumerable<TfsConnectDataSource.INode>) projects)
            this.m_teamProjectsCheckedListBox.Items.Add((object) project);
          for (int index = 0; index < this.m_teamProjectsCheckedListBox.Items.Count; ++index)
          {
            if (this.m_dataSource.SelectedProjects.Contains(this.m_teamProjectsCheckedListBox.Items[index] as TfsConnectDataSource.TeamProjectNode))
              this.m_teamProjectsCheckedListBox.SetItemChecked(index, true);
          }
          this.m_projectSelectAll.Enabled = this.m_teamProjectsCheckedListBox.Items.Count > 0;
          this.m_projectSelectAll.CheckState = this.m_dataSource.SelectedCollection.Count != this.m_dataSource.SelectedProjects.Count ? (this.m_dataSource.SelectedProjects.Count != 0 ? CheckState.Indeterminate : CheckState.Unchecked) : CheckState.Checked;
          this.m_teamProjectsCheckedListBox.Visible = true;
          this.m_selectAllPanel.Visible = true;
        }
        else if (this.m_dataSource.CanSelectProject)
        {
          this.m_teamProjectsListBox.Items.Clear();
          foreach (TfsConnectDataSource.TeamProjectNode project in (IEnumerable<TfsConnectDataSource.INode>) projects)
            this.m_teamProjectsListBox.Items.Add((object) project);
          if (this.m_dataSource.SelectedProjects.Count > 0)
          {
            int num = this.m_teamProjectsListBox.Items.IndexOf((object) this.m_dataSource.SelectedProjects[0]);
            if (num >= 0)
              this.m_teamProjectsListBox.SelectedIndex = num;
          }
          this.m_teamProjectsListBox.Visible = true;
        }
        this.EnableDisableConnectButton();
      }
      finally
      {
        this.m_suspendItemCheckEvent = false;
      }
    }

    private void PopulateServerList()
    {
      this.m_isUpdating = true;
      bool flag = this.m_domainComboBox.SelectedItem != this.m_dataSource.SelectedServer || this.m_domainComboBox.SelectedItem == null;
      this.m_domainComboBox.Items.Clear();
      this.m_domainComboBox.DisplayMember = "Name";
      foreach (object registeredServer in (Collection<TfsConnectDataSource.ServerNode>) this.m_dataSource.RegisteredServers)
        this.m_domainComboBox.Items.Add(registeredServer);
      this.m_domainComboBox.SelectedItem = (object) this.m_dataSource.SelectedServer;
      this.m_isUpdating = false;
      if (!flag)
        return;
      this.SelectedServerChanged((object) this, EventArgs.Empty);
    }

    private void StyleControls()
    {
      UIHost.WinformsStyler.Style(this.m_directoryTree);
      this.m_teamProjectsPanel.BackColor = this.m_directoryTree.BackColor;
      this.m_directoryPanel.BackColor = this.m_directoryTree.BackColor;
      this.m_selectAllPanel.BackColor = SystemColors.Control;
      this.m_projectsInfoBar.InfoBackColor = this.m_directoryTree.BackColor;
      this.m_directoryInfoBar.InfoBackColor = this.m_directoryTree.BackColor;
    }

    private void SetupControls()
    {
      this.ClearUserLabel();
      this.m_userLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.m_userLabel_LabelLinkClicked);
      if (this.m_dataSource.CanSelectProject)
        this.Name = this.m_dataSource.CanSelectMultipleProjects ? "ConnectToTfsMultiSelect" : "ConnectToTfsSingleSelect";
      else
        this.Name = "ConnectToTfsNoProjectList";
      if (!this.m_dataSource.CanSelectProject)
      {
        this.Text = Microsoft.TeamFoundation.Client.Internal.ClientResources.TfsConnectDialogTitleConnectToServer();
        this.m_middlePanel.Panel2Collapsed = true;
        this.m_middlePanel.Panel1MinSize = 0;
        this.m_middlePanel.Panel2MinSize = 0;
        this.MinimumSize = new Size(320, 350).LogicalToDeviceUnits();
      }
      else
      {
        this.m_middlePanel.Panel2Collapsed = false;
        this.MinimumSize = new Size(550, 350).LogicalToDeviceUnits();
        int width = this.m_middlePanel.Width;
        int deviceUnitsX1 = DpiHelper.LogicalToDeviceUnitsX(235);
        int deviceUnitsX2 = DpiHelper.LogicalToDeviceUnitsX(200);
        int num = deviceUnitsX1 + deviceUnitsX2;
        if (width - num > width / 5)
        {
          this.m_middlePanel.Panel1MinSize = deviceUnitsX1;
          this.m_middlePanel.Panel2MinSize = deviceUnitsX2;
        }
        else
        {
          this.m_middlePanel.Panel1MinSize = 4 * width / 10;
          this.m_middlePanel.Panel2MinSize = 3 * width / 10;
        }
        if (this.m_dataSource.CanSelectMultipleProjects)
        {
          this.m_teamProjectsCheckedListBox = new CheckedListBox();
          this.m_teamProjectsCheckedListBox.Name = "m_teamProjectsCheckedListBox";
          this.m_teamProjectsCheckedListBox.Dock = DockStyle.Fill;
          this.m_teamProjectsCheckedListBox.CheckOnClick = true;
          this.m_teamProjectsCheckedListBox.ItemCheck += new ItemCheckEventHandler(this.m_teamProjectsCheckedListBox_ItemCheck);
          this.m_teamProjectsCheckedListBox.BorderStyle = BorderStyle.None;
          this.m_teamProjectsCheckedListBox.Margin = new Padding(0);
          this.m_teamProjectsCheckedListBox.Padding = new Padding(2);
          this.m_teamProjectsCheckedListBox.IntegralHeight = false;
          this.m_teamProjectsCheckedListBox.Sorted = true;
          this.m_teamProjectsCheckedListBox.TabIndex = 7;
          this.m_teamProjectsCheckedListBox.KeyDown += new KeyEventHandler(this.m_teamProjectsCheckedListBox_KeyDown);
          this.m_teamProjectsPanel.Controls.Add((Control) this.m_teamProjectsCheckedListBox);
          this.m_teamProjectsPanel.Controls.Add((Control) this.m_selectAllPanel);
        }
        else
        {
          this.m_teamProjectsListBox = new ListBox();
          this.m_teamProjectsListBox.Name = "m_teamProjectsListBox";
          this.m_teamProjectsListBox.Dock = DockStyle.Fill;
          this.m_teamProjectsListBox.SelectedIndexChanged += new EventHandler(this.m_teamProjectsListBox_SelectedIndexChanged);
          this.m_teamProjectsListBox.BorderStyle = BorderStyle.None;
          this.m_teamProjectsListBox.Margin = new Padding(0);
          this.m_teamProjectsListBox.Padding = new Padding(2);
          this.m_teamProjectsListBox.IntegralHeight = false;
          this.m_teamProjectsListBox.Sorted = true;
          this.m_teamProjectsListBox.TabIndex = 7;
          this.m_teamProjectsListBox.DoubleClick += new EventHandler(this.m_teamProjectsListBox_DoubleClick);
          this.m_teamProjectsPanel.Controls.Add((Control) this.m_teamProjectsListBox);
          this.m_teamProjectsPanel.Controls.Remove((Control) this.m_selectAllPanel);
        }
      }
      if (!this.m_dataSource.CanSelectServer)
      {
        this.m_serversButton.Enabled = false;
        this.m_domainComboBox.Enabled = false;
        this.m_directoryTree.Enabled = false;
      }
      this.ClearPanels();
      this.m_directoryImageList.Images.AddStrip((Image) Microsoft.TeamFoundation.Client.Internal.ClientResources.Manager.GetObject("ConnectDialogImages"));
      DpiHelper.LogicalToDeviceUnits(ref this.m_directoryImageList);
      this.m_directoryTree.ImageList = this.m_directoryImageList;
    }

    private void UpdateUserLabel()
    {
      if (this.m_dataSource.SelectedServer == null)
      {
        this.ClearUserLabel();
      }
      else
      {
        string str1 = string.Empty;
        string str2 = string.Empty;
        if (this.m_dataSource.SelectedServer.Server.HasAuthenticated)
        {
          TeamFoundationIdentity identity;
          this.m_dataSource.SelectedServer.Server.GetAuthenticatedIdentity(out identity);
          str1 = identity.DisplayName;
          str2 = this.m_dataSource.SelectedServer.Server.IsHostedServer ? identity.GetAttribute("Account", string.Empty) : IdentityHelper.GetDomainUserName(identity);
        }
        else if (this.m_connectionUserMap.TryGetValue(this.m_dataSource.SelectedServer.Server.Uri, out str1))
        {
          str2 = str1;
        }
        else
        {
          try
          {
            str1 = str2 = this.m_credentialStorage.GetTokenProperty(this.m_dataSource.SelectedServer.Server.Uri, "UserName");
            this.m_connectionUserMap[this.m_dataSource.SelectedServer.Server.Uri] = str1;
          }
          catch (Exception ex)
          {
            TeamFoundationTrace.TraceException(ex);
          }
        }
        if (this.m_dataSource.SelectedServer.Server.HasAuthenticated && this.m_dataSource.SelectedServer.Server.IsHostedServer || !this.m_dataSource.SelectedServer.Server.HasAuthenticated && !string.IsNullOrEmpty(str1))
        {
          string labelText;
          string toolTipText;
          this.GetUserLabelTexts(str2, str1, out labelText, out toolTipText);
          labelText = Microsoft.TeamFoundation.Client.Internal.ClientResources.TfsConnectDialogUserLabelFormat((object) labelText, (object) Microsoft.TeamFoundation.Client.Internal.ClientResources.TfsConnectDialogSwitchUser());
          LinkLabel.Link link = new LinkLabel.Link(labelText.IndexOf(Microsoft.TeamFoundation.Client.Internal.ClientResources.TfsConnectDialogSwitchUser()), Microsoft.TeamFoundation.Client.Internal.ClientResources.TfsConnectDialogSwitchUser().Length);
          this.m_userLabel.Text = labelText;
          this.m_toolTip.SetToolTip((Control) this.m_userLabel, toolTipText);
          this.m_userLabel.Links.Clear();
          this.m_userLabel.Links.Add(link);
        }
        else
        {
          string labelText;
          string toolTipText;
          this.GetUserLabelTexts(str1, str2, out labelText, out toolTipText);
          this.m_userLabel.Text = labelText;
          this.m_toolTip.SetToolTip((Control) this.m_userLabel, toolTipText);
          this.m_userLabel.Links.Clear();
        }
        this.m_userLabel.Enabled = true;
      }
    }

    private void ClearUserLabel()
    {
      this.m_userLabel.Links.Clear();
      this.m_userLabel.Text = string.Empty;
      this.m_toolTip.SetToolTip((Control) this.m_userLabel, (string) null);
      this.m_userLabel.Enabled = false;
    }

    private void GetUserLabelTexts(
      string nameOnLabel,
      string nameOnToolTip,
      out string labelText,
      out string toolTipText)
    {
      labelText = nameOnLabel;
      toolTipText = nameOnToolTip;
      if (nameOnLabel == null || nameOnLabel.Length <= 32)
        return;
      labelText = Microsoft.TeamFoundation.Client.Internal.ClientResources.TruncatedStringFormat((object) nameOnLabel.Substring(0, 32));
      if (string.Equals(nameOnLabel, nameOnToolTip, StringComparison.OrdinalIgnoreCase))
        return;
      toolTipText = Microsoft.TeamFoundation.Client.Internal.ClientResources.TfsConnectDialogUserLabelFormat((object) nameOnToolTip, (object) nameOnLabel);
    }

    private void EnableDisableConnectButton()
    {
      if (this.m_dataSource.CanSelectProject && !this.m_dataSource.CanSelectMultipleProjects)
        this.m_connectButton.Enabled = this.m_dataSource.SelectedProjects != null && this.m_dataSource.SelectedProjects.Count > 0;
      else
        this.m_connectButton.Enabled = this.m_dataSource.SelectedCollection != null && this.m_dataSource.SelectedCollection.State == RetrievablePropertyState.Retrieved;
    }

    private void SetPanelStatus(TfsConnectDialog.DialogPanel panel, string text)
    {
      if (panel == TfsConnectDialog.DialogPanel.Projects)
      {
        this.ClearTeamProjectsPanel();
        this.m_projectsInfoBar.Icon = InformationBar.IconType.Info;
        this.m_projectsInfoBar.Text = text;
        this.m_projectsInfoBar.Visible = true;
      }
      else
      {
        this.ClearPanels();
        this.m_directoryInfoBar.Icon = InformationBar.IconType.Info;
        this.m_directoryInfoBar.Text = text;
        this.m_directoryInfoBar.Visible = true;
      }
    }

    private void SetPanelWorking(TfsConnectDialog.DialogPanel panel)
    {
      if (panel == TfsConnectDialog.DialogPanel.Projects)
      {
        this.ClearTeamProjectsPanel();
        this.m_projectsInfoBar.StartMarquee(100);
        this.m_projectsInfoBar.Text = Microsoft.TeamFoundation.Client.Internal.ClientResources.TfsConnectDialogWorking();
        this.m_projectsInfoBar.Visible = true;
      }
      else
      {
        this.ClearPanels();
        this.m_directoryInfoBar.StartMarquee(100);
        this.m_directoryInfoBar.Text = Microsoft.TeamFoundation.Client.Internal.ClientResources.TfsConnectDialogWorking();
        this.m_directoryInfoBar.Visible = true;
      }
    }

    private void SetPanelError(
      TfsConnectDialog.DialogPanel panel,
      Exception error,
      string serverName)
    {
      if (panel == TfsConnectDialog.DialogPanel.Projects)
      {
        if (this.m_dataSource.CanSelectProject)
        {
          this.ClearTeamProjectsPanel();
          this.SetInfobarError(this.m_projectsInfoBar, error, serverName);
        }
        else
        {
          if (UIHost.ShowMessageBox(ConnectFailureReason.GetReason(error).GetErrorMessage(serverName), (string) null, (string) null, MessageBoxButtons.RetryCancel, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button2) != DialogResult.Retry)
            return;
          this.m_dataSource.SelectedCollection.Refresh();
        }
      }
      else
      {
        this.ClearPanels();
        this.SetInfobarError(this.m_directoryInfoBar, error, serverName);
      }
    }

    private void SetInfobarError(InformationBar bar, Exception error, string serverName)
    {
      bar.Icon = InformationBar.IconType.Error;
      string errorMessage = ConnectFailureReason.GetReason(error).GetErrorMessage(serverName);
      int start = errorMessage.Length + 1;
      string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", (object) errorMessage, (object) Microsoft.TeamFoundation.Client.Internal.ClientResources.TfsConnectDialogRefresh());
      bar.Text = str;
      int length = Microsoft.TeamFoundation.Client.Internal.ClientResources.TfsConnectDialogRefresh().Length;
      LinkLabel.Link link = new LinkLabel.Link(start, length);
      bar.LabelLinks.Add(link);
      bar.Text = str;
      bar.Visible = true;
    }

    private void ClearInfobar(InformationBar bar)
    {
      bar.Visible = false;
      bar.Text = string.Empty;
      bar.StopMarquee();
      bar.Icon = InformationBar.IconType.None;
      bar.LabelLinks.Clear();
    }

    private void ClearTeamProjectsPanel()
    {
      if (this.m_dataSource.CanSelectMultipleProjects)
      {
        this.m_teamProjectsCheckedListBox.Items.Clear();
        this.m_teamProjectsCheckedListBox.Visible = false;
      }
      else if (this.m_dataSource.CanSelectProject)
      {
        this.m_teamProjectsListBox.Items.Clear();
        this.m_teamProjectsListBox.Visible = false;
      }
      this.m_selectAllPanel.Visible = false;
      this.ClearInfobar(this.m_projectsInfoBar);
    }

    private void ClearPanels()
    {
      this.ClearTeamProjectsPanel();
      this.ClearInfobar(this.m_directoryInfoBar);
      this.m_directoryTree.Visible = false;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (TfsConnectDialog));
      this.m_middlePanel = new SplitContainer();
      this.m_directoryPanel = new Panel();
      this.m_directoryTree = new TreeView();
      this.m_directoryImageList = new ImageList(this.components);
      this.m_directoryInfoBar = new InformationBar();
      this.m_directoryLabel = new Label();
      this.m_teamProjectsPanel = new Panel();
      this.m_selectAllPanel = new Panel();
      this.m_projectSelectAll = new CheckBox();
      this.m_projectsInfoBar = new InformationBar();
      this.m_teamProjectsLabel = new Label();
      this.m_mainPanel = new TableLayoutPanel();
      this.m_domainPanel = new TableLayoutPanel();
      this.m_serversButton = new Button();
      this.m_domainComboBox = new ComboBox();
      this.m_selectServerLabel = new Label();
      this.m_commandButtonsPanel = new TableLayoutPanel();
      this.m_connectButton = new Button();
      this.m_cancelButton = new Button();
      this.m_bottomPanel = new TableLayoutPanel();
      this.m_userLabel = new LinkLabel();
      this.m_toolTip = new ToolTip(this.components);
      this.m_middlePanel.Panel1.SuspendLayout();
      this.m_middlePanel.Panel2.SuspendLayout();
      this.m_middlePanel.SuspendLayout();
      this.m_directoryPanel.SuspendLayout();
      this.m_teamProjectsPanel.SuspendLayout();
      this.m_selectAllPanel.SuspendLayout();
      this.m_mainPanel.SuspendLayout();
      this.m_domainPanel.SuspendLayout();
      this.m_bottomPanel.SuspendLayout();
      this.m_commandButtonsPanel.SuspendLayout();
      this.SuspendLayout();
      this.m_middlePanel.Name = "m_middlePanel";
      componentResourceManager.ApplyResources((object) this.m_middlePanel, "m_middlePanel");
      this.m_middlePanel.Panel1.Controls.Add((Control) this.m_directoryPanel);
      this.m_middlePanel.Panel1.Controls.Add((Control) this.m_directoryLabel);
      componentResourceManager.ApplyResources((object) this.m_middlePanel.Panel1, "m_middlePanel.Panel1");
      this.m_middlePanel.Panel2.Controls.Add((Control) this.m_teamProjectsPanel);
      this.m_middlePanel.Panel2.Controls.Add((Control) this.m_teamProjectsLabel);
      componentResourceManager.ApplyResources((object) this.m_middlePanel.Panel2, "m_middlePanel.Panel2");
      componentResourceManager.ApplyResources((object) this.m_directoryPanel, "m_directoryPanel");
      this.m_directoryPanel.BorderStyle = BorderStyle.FixedSingle;
      this.m_directoryPanel.Controls.Add((Control) this.m_directoryTree);
      this.m_directoryPanel.Controls.Add((Control) this.m_directoryInfoBar);
      this.m_directoryPanel.Name = "m_directoryPanel";
      this.m_directoryTree.BorderStyle = BorderStyle.None;
      componentResourceManager.ApplyResources((object) this.m_directoryTree, "m_directoryTree");
      this.m_directoryTree.HideSelection = false;
      this.m_directoryTree.ImageList = this.m_directoryImageList;
      this.m_directoryTree.Name = "m_directoryTree";
      this.m_directoryTree.AfterSelect += new TreeViewEventHandler(this.m_directoryTree_AfterSelect);
      this.m_directoryImageList.ColorDepth = ColorDepth.Depth24Bit;
      componentResourceManager.ApplyResources((object) this.m_directoryImageList, "m_directoryImageList");
      this.m_directoryImageList.TransparentColor = System.Drawing.Color.Magenta;
      this.ApplyInfoBarColors(this.m_directoryInfoBar);
      this.m_directoryInfoBar.AutoGrowMaxHeight = 0;
      this.m_directoryInfoBar.BorderPadding = new Padding(0);
      this.m_directoryInfoBar.BorderSides = BorderPanel.Sides.None;
      this.m_directoryInfoBar.BorderStyle = ButtonBorderStyle.Solid;
      componentResourceManager.ApplyResources((object) this.m_directoryInfoBar, "m_directoryInfoBar");
      this.m_directoryInfoBar.Icon = InformationBar.IconType.None;
      this.m_directoryInfoBar.MinimumSize = new Size(100, 23);
      this.m_directoryInfoBar.Name = "m_directoryInfoBar";
      this.m_directoryInfoBar.ProgressBarValue = 0;
      this.m_directoryInfoBar.WrapText = true;
      this.m_directoryInfoBar.LabelLinkClicked += new LinkLabelLinkClickedEventHandler(this.m_directoryInfoBar_LabelLinkClicked);
      componentResourceManager.ApplyResources((object) this.m_directoryLabel, "m_directoryLabel");
      this.m_directoryLabel.Name = "m_directoryLabel";
      componentResourceManager.ApplyResources((object) this.m_teamProjectsPanel, "m_teamProjectsPanel");
      this.m_teamProjectsPanel.BorderStyle = BorderStyle.FixedSingle;
      this.m_teamProjectsPanel.Controls.Add((Control) this.m_selectAllPanel);
      this.m_teamProjectsPanel.Controls.Add((Control) this.m_projectsInfoBar);
      this.m_teamProjectsPanel.Name = "m_teamProjectsPanel";
      componentResourceManager.ApplyResources((object) this.m_selectAllPanel, "m_selectAllPanel");
      this.m_selectAllPanel.Controls.Add((Control) this.m_projectSelectAll);
      this.m_selectAllPanel.Name = "m_selectAllPanel";
      componentResourceManager.ApplyResources((object) this.m_projectSelectAll, "m_projectSelectAll");
      this.m_projectSelectAll.Name = "m_projectSelectAll";
      this.m_projectSelectAll.UseVisualStyleBackColor = true;
      this.m_projectSelectAll.PreviewKeyDown += new PreviewKeyDownEventHandler(this.m_projectSelectAll_PreviewKeyDown);
      this.m_projectSelectAll.Click += new EventHandler(this.m_projectSelectAll_Click);
      this.m_projectsInfoBar.AutoGrowMaxHeight = 0;
      this.ApplyInfoBarColors(this.m_projectsInfoBar);
      this.m_projectsInfoBar.BorderPadding = new Padding(0);
      this.m_projectsInfoBar.BorderSides = BorderPanel.Sides.None;
      this.m_projectsInfoBar.BorderStyle = ButtonBorderStyle.Solid;
      componentResourceManager.ApplyResources((object) this.m_projectsInfoBar, "m_projectsInfoBar");
      this.m_projectsInfoBar.Icon = InformationBar.IconType.None;
      this.m_projectsInfoBar.MinimumSize = new Size(100, 23);
      this.m_projectsInfoBar.Name = "m_projectsInfoBar";
      this.m_projectsInfoBar.ProgressBarValue = 0;
      this.m_projectsInfoBar.WrapText = true;
      this.m_projectsInfoBar.LabelLinkClicked += new LinkLabelLinkClickedEventHandler(this.m_projectsInfoBar_LabelLinkClicked);
      componentResourceManager.ApplyResources((object) this.m_teamProjectsLabel, "m_teamProjectsLabel");
      this.m_teamProjectsLabel.Name = "m_teamProjectsLabel";
      componentResourceManager.ApplyResources((object) this.m_mainPanel, "m_mainPanel");
      this.m_mainPanel.Controls.Add((Control) this.m_domainPanel, 0, 0);
      this.m_mainPanel.Controls.Add((Control) this.m_middlePanel, 0, 1);
      this.m_mainPanel.Controls.Add((Control) this.m_bottomPanel, 0, 2);
      this.m_mainPanel.Name = "m_mainPanel";
      this.m_mainPanel.AutoSize = true;
      componentResourceManager.ApplyResources((object) this.m_domainPanel, "m_domainPanel");
      this.m_domainPanel.Controls.Add((Control) this.m_serversButton, 1, 1);
      this.m_domainPanel.Controls.Add((Control) this.m_domainComboBox, 0, 1);
      this.m_domainPanel.Controls.Add((Control) this.m_selectServerLabel, 0, 0);
      this.m_domainPanel.Name = "m_domainPanel";
      componentResourceManager.ApplyResources((object) this.m_serversButton, "m_serversButton");
      this.m_serversButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.m_serversButton.MinimumSize = new Size(75, 23);
      this.m_serversButton.Name = "m_serversButton";
      this.m_serversButton.UseVisualStyleBackColor = true;
      this.m_serversButton.Click += new EventHandler(this.m_serversButton_Click);
      componentResourceManager.ApplyResources((object) this.m_domainComboBox, "m_domainComboBox");
      this.m_domainComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      this.m_domainComboBox.FormattingEnabled = true;
      this.m_domainComboBox.Name = "m_domainComboBox";
      this.m_domainComboBox.Sorted = true;
      this.m_domainComboBox.SelectedValueChanged += new EventHandler(this.SelectedServerChanged);
      componentResourceManager.ApplyResources((object) this.m_selectServerLabel, "m_selectServerLabel");
      this.m_domainPanel.SetColumnSpan((Control) this.m_selectServerLabel, 2);
      this.m_selectServerLabel.Name = "m_selectServerLabel";
      componentResourceManager.ApplyResources((object) this.m_bottomPanel, "m_bottomPanel");
      this.m_bottomPanel.Controls.Add((Control) this.m_userLabel, 0, 0);
      this.m_bottomPanel.Controls.Add((Control) this.m_commandButtonsPanel, 1, 0);
      this.m_bottomPanel.Name = "m_bottomPanel";
      componentResourceManager.ApplyResources((object) this.m_userLabel, "m_userLabel");
      this.m_userLabel.Name = "m_userLabel";
      componentResourceManager.ApplyResources((object) this.m_commandButtonsPanel, "m_commandButtonsPanel");
      this.m_commandButtonsPanel.Controls.Add((Control) this.m_connectButton, 0, 0);
      this.m_commandButtonsPanel.Controls.Add((Control) this.m_cancelButton, 1, 0);
      this.m_commandButtonsPanel.Name = "m_commandButtonsPanel";
      componentResourceManager.ApplyResources((object) this.m_connectButton, "m_connectButton");
      this.m_connectButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.m_connectButton.MinimumSize = new Size(75, 23);
      this.m_connectButton.Name = "m_connectButton";
      this.m_connectButton.UseVisualStyleBackColor = true;
      this.m_connectButton.Click += new EventHandler(this.m_connectButton_Click);
      componentResourceManager.ApplyResources((object) this.m_cancelButton, "m_cancelButton");
      this.m_cancelButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.m_cancelButton.DialogResult = DialogResult.Cancel;
      this.m_cancelButton.MinimumSize = new Size(75, 23);
      this.m_cancelButton.Name = "m_cancelButton";
      this.m_cancelButton.UseVisualStyleBackColor = true;
      componentResourceManager.ApplyResources((object) this, "$this");
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoSizeMode = AutoSizeMode.GrowOnly;
      this.AutoSize = true;
      this.CancelButton = (IButtonControl) this.m_cancelButton;
      this.AcceptButton = (IButtonControl) this.m_connectButton;
      this.Controls.Add((Control) this.m_mainPanel);
      this.HelpButton = true;
      this.KeyPreview = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (TfsConnectDialog);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.SizeGripStyle = SizeGripStyle.Show;
      this.FormClosed += new FormClosedEventHandler(this.TfsConnectDialog_FormClosed);
      this.m_middlePanel.Panel1.ResumeLayout(false);
      this.m_middlePanel.Panel1.PerformLayout();
      this.m_middlePanel.Panel2.ResumeLayout(false);
      this.m_middlePanel.Panel2.PerformLayout();
      this.m_middlePanel.ResumeLayout(false);
      this.m_directoryPanel.ResumeLayout(false);
      this.m_teamProjectsPanel.ResumeLayout(false);
      this.m_teamProjectsPanel.PerformLayout();
      this.m_selectAllPanel.ResumeLayout(false);
      this.m_selectAllPanel.PerformLayout();
      this.m_mainPanel.ResumeLayout(false);
      this.m_mainPanel.PerformLayout();
      this.m_domainPanel.ResumeLayout(false);
      this.m_domainPanel.PerformLayout();
      this.m_bottomPanel.ResumeLayout(false);
      this.m_bottomPanel.PerformLayout();
      this.m_commandButtonsPanel.ResumeLayout(false);
      this.m_commandButtonsPanel.PerformLayout();
      this.ResumeLayout(false);
    }

    private void ApplyInfoBarColors(InformationBar infoBar)
    {
      infoBar.BorderColor = SystemColors.ControlDark;
      infoBar.InnerColor = SystemColors.Info;
      infoBar.UseInnerColor = true;
      infoBar.WarningForeColor = SystemColors.ControlText;
      infoBar.InfoForeColor = SystemColors.ControlText;
    }

    private enum DialogPanel
    {
      Directory,
      Projects,
    }
  }
}
