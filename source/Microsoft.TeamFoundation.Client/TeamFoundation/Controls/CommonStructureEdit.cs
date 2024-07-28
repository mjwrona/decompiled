// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Controls.CommonStructureEdit
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Internal;
using Microsoft.TeamFoundation.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace Microsoft.TeamFoundation.Controls
{
  public class CommonStructureEdit : UserControl
  {
    private ResourceManager _resources;
    private ContextMenu menu = new ContextMenu();
    private MenuItem menuNew = new MenuItem();
    private MenuItem menuRename = new MenuItem();
    private MenuItem menuDelete = new MenuItem();
    private MenuItem menuSeparator = new MenuItem();
    private MenuItem menuSeparatorMoveDownIndent = new MenuItem();
    private MenuItem menuSeparatorRefreshIndent = new MenuItem();
    private MenuItem menuIndent = new MenuItem();
    private MenuItem menuOutdent = new MenuItem();
    private MenuItem menuRefresh = new MenuItem();
    private MenuItem menuMoveUp = new MenuItem();
    private MenuItem menuMoveDown = new MenuItem();
    private TreeView tree = new TreeView();
    private Panel panel = new Panel();
    private IContainer components = (IContainer) new System.ComponentModel.Container();
    private ToolStripButton _addRootButton;
    private ToolStripButton _addChildButton;
    private ToolStripButton _removeButton;
    private ToolStripButton _moveUpButton;
    private ToolStripButton _moveDownButton;
    private ToolStripButton _indentButton;
    private ToolStripButton _unindentButton;
    private ToolStripSeparator _toolBarSeparator;
    private ToolStrip _treeViewToolBar;
    private string _rootNodeUri;
    private string _newNodeDefaultName = string.Empty;
    private bool _inNodeEdit;
    private bool _inCreateNewNode;
    private readonly string FIND_NODE_URI_XPATH_QUERY = ".//Node[@NodeID='{0}']";
    private ICommonStructureService css;
    private ILinking _linking;
    private TfsTeamProjectCollection _tfs;
    private string selectedName = string.Empty;
    private string selectedType = string.Empty;
    private string selectedNodeUri = string.Empty;
    private CssNode selectedCssNode;
    private XmlNode structure;
    private const string cHelpInvalidCssNodeName = "vstf.css.InvalidCssNodeName";

    public event EventHandler NodeSelectChange;

    public string RootNodeUri
    {
      get => this._rootNodeUri;
      set
      {
        this._rootNodeUri = value;
        this.RefreshView((TreeNode) null);
      }
    }

    public string SelectedNodeUri => this.selectedNodeUri;

    public TfsTeamProjectCollection TfsTeamProjectCollection
    {
      get => this._tfs;
      set
      {
        this._tfs = value;
        this.css = (ICommonStructureService) this._tfs.GetService(typeof (ICommonStructureService));
        this._linking = (ILinking) this._tfs.GetService(typeof (ILinking));
        this.RefreshView((TreeNode) null);
      }
    }

    public ICommonStructureService CommonStructureServiceProxy
    {
      get => this.css;
      set
      {
        this.css = value;
        this.RefreshView((TreeNode) null);
      }
    }

    public CommonStructureEdit()
    {
      this._resources = new ResourceManager(this.GetType().Name, this.GetType().Assembly);
      this.SuspendLayout();
      this._treeViewToolBar = new ToolStrip();
      this._addRootButton = new ToolStripButton();
      this._addChildButton = new ToolStripButton();
      this._removeButton = new ToolStripButton();
      this._moveUpButton = new ToolStripButton();
      this._moveDownButton = new ToolStripButton();
      this._indentButton = new ToolStripButton();
      this._unindentButton = new ToolStripButton();
      this._toolBarSeparator = new ToolStripSeparator();
      UIHost.WinformsStyler.Style(this._treeViewToolBar, false);
      this.menu.MenuItems.AddRange(new MenuItem[11]
      {
        this.menuNew,
        this.menuRename,
        this.menuDelete,
        this.menuSeparator,
        this.menuMoveUp,
        this.menuMoveDown,
        this.menuSeparatorMoveDownIndent,
        this.menuOutdent,
        this.menuIndent,
        this.menuSeparatorRefreshIndent,
        this.menuRefresh
      });
      this.menu.Popup += new EventHandler(this.menu_Popup);
      this.menuNew.Index = 0;
      this.menuNew.Shortcut = Shortcut.Ins;
      this.menuNew.Text = this._resources.GetString("MenuNew");
      this.menuNew.Click += new EventHandler(this.menuNew_Click);
      this.menuRename.Index = 1;
      this.menuRename.Shortcut = Shortcut.F2;
      this.menuRename.Text = this._resources.GetString("MenuRename");
      this.menuRename.Click += new EventHandler(this.menuRename_Click);
      this.menuDelete.Index = 2;
      this.menuDelete.Shortcut = Shortcut.Del;
      this.menuDelete.Text = this._resources.GetString("MenuDelete");
      this.menuDelete.Click += new EventHandler(this.menuDelete_Click);
      this.menuSeparator.Index = 3;
      this.menuSeparator.Text = "-";
      this.menuMoveUp.Index = 4;
      this.menuMoveUp.Text = this._resources.GetString("MenuMoveUp");
      this.menuMoveUp.Click += new EventHandler(this.menuMoveUp_Click);
      this.menuMoveDown.Index = 5;
      this.menuMoveDown.Text = this._resources.GetString("MenuMoveDown");
      this.menuMoveDown.Click += new EventHandler(this.menuMoveDown_Click);
      this.menuSeparatorMoveDownIndent.Index = 6;
      this.menuSeparatorMoveDownIndent.Text = "-";
      this.menuIndent.Index = 7;
      this.menuIndent.Text = this._resources.GetString("MenuIndent");
      this.menuIndent.Click += new EventHandler(this.menuIndent_Click);
      this.menuOutdent.Index = 8;
      this.menuOutdent.Text = this._resources.GetString("MenuOutdent");
      this.menuOutdent.Click += new EventHandler(this.menuOutdent_Click);
      this.menuSeparatorRefreshIndent.Index = 9;
      this.menuSeparatorRefreshIndent.Text = "-";
      this.menuRefresh.Index = 10;
      this.menuRefresh.Text = this._resources.GetString("MenuRefresh");
      this.menuRefresh.Click += new EventHandler(this.menuRefresh_Click);
      this.tree.AllowDrop = true;
      this.tree.BorderStyle = BorderStyle.None;
      this.tree.ContextMenu = this.menu;
      this.tree.HideSelection = false;
      this.tree.LabelEdit = false;
      this.tree.Location = new Point(0, 10);
      this.tree.Dock = DockStyle.Fill;
      this.tree.Size = new Size(150, 150);
      this.tree.Sorted = false;
      this.tree.TabIndex = 0;
      this.tree.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.tree_NodeMouseClick);
      this.tree.DragOver += new DragEventHandler(this.tree_DragOver);
      this.tree.AfterSelect += new TreeViewEventHandler(this.tree_AfterSelect);
      this.tree.BeforeLabelEdit += new NodeLabelEditEventHandler(this.tree_BeforeLabelEdit);
      this.tree.AfterLabelEdit += new NodeLabelEditEventHandler(this.tree_AfterLabelEdit);
      this.tree.DragEnter += new DragEventHandler(this.tree_DragEnter);
      this.tree.ItemDrag += new ItemDragEventHandler(this.tree_ItemDrag);
      this.tree.DragDrop += new DragEventHandler(this.tree_DragDrop);
      UIHost.WinformsStyler.Style(this.tree);
      this._treeViewToolBar.AutoSize = true;
      this._treeViewToolBar.Items.AddRange(new ToolStripItem[7]
      {
        (ToolStripItem) this._addChildButton,
        (ToolStripItem) this._removeButton,
        (ToolStripItem) this._toolBarSeparator,
        (ToolStripItem) this._moveUpButton,
        (ToolStripItem) this._moveDownButton,
        (ToolStripItem) this._unindentButton,
        (ToolStripItem) this._indentButton
      });
      this._treeViewToolBar.Location = new Point(0, 0);
      this._treeViewToolBar.Name = nameof (_treeViewToolBar);
      this._treeViewToolBar.Anchor = AnchorStyles.Top | AnchorStyles.Left;
      this._treeViewToolBar.Dock = DockStyle.Top;
      this._treeViewToolBar.ShowItemToolTips = true;
      this._treeViewToolBar.TabStop = true;
      this._treeViewToolBar.TabIndex = 1;
      this._treeViewToolBar.ItemClicked += new ToolStripItemClickedEventHandler(this.OnTreeViewToolBarButtonClick);
      this._treeViewToolBar.Margin = Padding.Empty;
      this._treeViewToolBar.GripStyle = ToolStripGripStyle.Hidden;
      this.Controls.Add((Control) this.tree);
      this.Controls.Add((Control) this._treeViewToolBar);
      this.Name = nameof (CommonStructureEdit);
      this.UpdateEnabledState();
      this.ResumeLayout(false);
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      ImageList imageList = new ImageList(this.components);
      imageList.ImageSize = new Size(16, 16);
      imageList.TransparentColor = Color.Magenta;
      imageList.Images.AddStrip((Image) new Bitmap(executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + ".Resources.Commands.bmp")));
      this._treeViewToolBar.ImageList = imageList;
      this._addChildButton.ToolTipText = this._resources.GetString("ToolTip_AddChild");
      this._addChildButton.AccessibleDescription = this._resources.GetString("ToolTip_AddChild");
      this._addChildButton.ImageIndex = 2;
      this._removeButton.ToolTipText = this._resources.GetString("ToolTip_Remove");
      this._removeButton.AccessibleDescription = this._resources.GetString("ToolTip_Remove");
      this._removeButton.ImageIndex = 4;
      this._moveUpButton.ToolTipText = this._resources.GetString("ToolTip_MoveUp");
      this._moveUpButton.AccessibleDescription = this._resources.GetString("ToolTip_MoveUp");
      this._moveUpButton.ImageIndex = 5;
      this._moveDownButton.ToolTipText = this._resources.GetString("ToolTip_MoveDown");
      this._moveDownButton.AccessibleDescription = this._resources.GetString("ToolTip_MoveDown");
      this._moveDownButton.ImageIndex = 6;
      this._indentButton.ToolTipText = this._resources.GetString("ToolTip_Indent");
      this._indentButton.AccessibleDescription = this._resources.GetString("ToolTip_Indent");
      this._indentButton.ImageIndex = 1;
      this._unindentButton.ToolTipText = this._resources.GetString("ToolTip_Unindent");
      this._unindentButton.AccessibleDescription = this._resources.GetString("ToolTip_Unindent");
      this._unindentButton.ImageIndex = 0;
      this.tree.ImageIndex = 0;
      this.tree.SelectedImageIndex = 0;
      this._addRootButton.AccessibilityObject.Name = "AddRoot";
      this._addChildButton.AccessibilityObject.Name = "AddChild";
      this._removeButton.AccessibilityObject.Name = "Remove";
      this._moveUpButton.AccessibilityObject.Name = "MoveUp";
      this._moveDownButton.AccessibilityObject.Name = "MoveDown";
      this._indentButton.AccessibilityObject.Name = "Indent";
      this._unindentButton.AccessibilityObject.Name = "Outdent";
    }

    public void SelectRootNode()
    {
      if (this.tree.Nodes.Count <= 0)
        return;
      this.tree.SelectedNode = this.tree.Nodes[0];
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private CssNode GetNodeInfo(XmlNode xn)
    {
      if (xn.Attributes["Name"] == null || xn.Attributes["NodeID"] == null)
        throw new ArgumentException(this._resources.GetString("InvalidXmlFormat"));
      return new CssNode(xn.Attributes["Name"].Value, xn.Attributes["NodeID"].Value);
    }

    private void RefreshView(TreeNode prevSelToRestore)
    {
      this.tree.SelectedNode = (TreeNode) null;
      this.SelectChanged((TreeViewEventArgs) null);
      this.tree.BeginUpdate();
      try
      {
        Cursor.Current = Cursors.WaitCursor;
        this.tree.LabelEdit = false;
        this.tree.Nodes.Clear();
        if (this.css == null || this._rootNodeUri == null)
          return;
        this.structure = (XmlNode) this.css.GetNodesXml(new string[1]
        {
          this._rootNodeUri
        }, true);
        this.tree.LabelEdit = true;
        TreeNode treeNodeToSelect = (TreeNode) null;
        this.tree.Nodes.Add(this.PopulateNode(this.structure.ChildNodes[0], this.FindNodeOrClosestParent(this.structure, prevSelToRestore), ref treeNodeToSelect));
        this.tree.ExpandAll();
        this._newNodeDefaultName = !TFStringComparer.CssStructureType.Equals(this.structure.ChildNodes[0].Attributes["StructureType"].Value, "ProjectModelHierarchy") ? this._resources.GetString("NewIterationNodeDefault") : this._resources.GetString("NewAreaNodeDefault");
        if (treeNodeToSelect == null)
          return;
        this.SelectNodeAndFireEvent(treeNodeToSelect);
      }
      finally
      {
        Cursor.Current = Cursors.Default;
        this.tree.EndUpdate();
      }
    }

    private TreeNode PopulateNode(XmlNode xn, XmlNode xnSelect, ref TreeNode treeNodeToSelect)
    {
      TreeNode treeNode = this.CreateTreeNode(this.GetNodeInfo(xn));
      if (xn == xnSelect)
        treeNodeToSelect = treeNode;
      foreach (XmlNode xmlNode in xn)
      {
        if (TFStringComparer.CssXmlNodeName.Equals(xmlNode.Name, "Children"))
        {
          IEnumerator enumerator = xmlNode.GetEnumerator();
          try
          {
            while (enumerator.MoveNext())
            {
              TreeNode node = this.PopulateNode((XmlNode) enumerator.Current, xnSelect, ref treeNodeToSelect);
              treeNode.Nodes.Add(node);
            }
            break;
          }
          finally
          {
            if (enumerator is IDisposable disposable)
              disposable.Dispose();
          }
        }
      }
      return treeNode;
    }

    private XmlNode FindNodeOrClosestParent(XmlNode xn, TreeNode tn)
    {
      XmlNode nodeOrClosestParent = (XmlNode) null;
      for (; tn != null; tn = tn.Parent)
      {
        string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.FIND_NODE_URI_XPATH_QUERY, (object) ((CssNode) tn.Tag).ID);
        nodeOrClosestParent = xn.SelectSingleNode(xpath);
        if (nodeOrClosestParent != null)
          break;
      }
      return nodeOrClosestParent;
    }

    private TreeNode CreateTreeNode(CssNode tag) => new TreeNode(tag.Name)
    {
      ContextMenu = this.menu,
      Tag = (object) tag
    };

    public List<string> GetCssRootNodePaths(Uri rootNodeUri)
    {
      List<string> cssRootNodePaths = new List<string>();
      try
      {
        this.structure = (XmlNode) this.css.GetNodesXml(new string[1]
        {
          rootNodeUri.OriginalString
        }, true);
      }
      catch (Exception ex)
      {
        CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
        MessageBoxOptions options = MessageBoxOptions.DefaultDesktopOnly;
        if (currentUiCulture.TextInfo.IsRightToLeft)
          options |= MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
        int num = (int) MessageBox.Show(ex.Message + "\n" + this._resources.GetString("CannotRetrieveStructure"), this._resources.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, options);
        return (List<string>) null;
      }
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      XmlDocument xmlDocument = new XmlDocument();
      using (XmlReader reader = XmlReader.Create((TextReader) new StringReader(this.structure.InnerXml), settings))
        xmlDocument.Load(reader);
      foreach (XmlNode selectNode in xmlDocument.DocumentElement.SelectNodes("descendant::Node"))
        cssRootNodePaths.Add(selectNode.Attributes["Path"].Value);
      return cssRootNodePaths;
    }

    public string ToolStripAccessibilityName
    {
      get => this._treeViewToolBar.AccessibilityObject.Name;
      set => this._treeViewToolBar.AccessibilityObject.Name = value;
    }

    private string GetUniqueNodeName()
    {
      string empty = string.Empty;
      TreeNodeCollection nodes = this.tree.SelectedNode.Nodes;
      int num = 0;
      string y;
      bool flag;
      do
      {
        y = string.Format((IFormatProvider) CultureInfo.InvariantCulture, this._newNodeDefaultName, (object) num++);
        flag = true;
        foreach (TreeNode treeNode in nodes)
        {
          if (TFStringComparer.CssTreeNodeName.Equals(treeNode.Text, y))
          {
            flag = false;
            break;
          }
        }
      }
      while (!flag);
      return y;
    }

    private void menuNew_Click(object sender, EventArgs e)
    {
      try
      {
        this.CheckPermissions("CREATE_CHILDREN");
        TreeNode treeNode = this.CreateTreeNode(new CssNode(this.GetUniqueNodeName(), string.Empty));
        this.tree.SelectedNode.Nodes.Add(treeNode);
        this.tree.SelectedNode = treeNode;
        treeNode.EnsureVisible();
        this._inCreateNewNode = true;
        treeNode.BeginEdit();
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void menuRename_Click(object sender, EventArgs e)
    {
      try
      {
        if (this.tree.SelectedNode.Parent == null)
        {
          CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
          MessageBoxOptions options = (MessageBoxOptions) 0;
          if (currentUiCulture.TextInfo.IsRightToLeft)
            options |= MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
          int num = (int) MessageBox.Show(this._resources.GetString("CannotRenameRootNode"), this._resources.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, options);
        }
        else
        {
          this.CheckPermissions("GENERIC_WRITE");
          this.tree.SelectedNode.BeginEdit();
        }
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void menuDelete_Click(object sender, EventArgs e)
    {
      try
      {
        if (this.tree.SelectedNode.Parent == null)
        {
          CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
          MessageBoxOptions options = (MessageBoxOptions) 0;
          if (currentUiCulture.TextInfo.IsRightToLeft)
            options |= MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
          int num = (int) MessageBox.Show(this._resources.GetString("CannotDeleteRootNode"), this._resources.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, options);
        }
        else
        {
          this.CheckPermissions("DELETE");
          bool flag = true;
          TreeNode parent = this.tree.SelectedNode.Parent;
          string reclassifyUri = ((CssNode) parent.Tag).ID;
          DeleteNodesDialog deleteNodesDialog = (DeleteNodesDialog) null;
          DialogResult dialogResult;
          if (flag)
          {
            deleteNodesDialog = new DeleteNodesDialog(this._tfs, reclassifyUri, this.selectedNodeUri);
            dialogResult = deleteNodesDialog.ShowDialog((IWin32Window) this);
          }
          else
            dialogResult = UIHost.ShowMessageBox((IWin32Window) this, this._resources.GetString("DeleteText"), (string) null, (string) this._resources.GetObject("DeleteNodes"), MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
          if (dialogResult == DialogResult.Cancel || deleteNodesDialog.SelectedNodeInfo == null)
            return;
          using (new AutoWaitCursor())
          {
            if (flag)
              reclassifyUri = deleteNodesDialog.SelectedNodeInfo.Uri;
            this.css.DeleteBranches(new string[1]
            {
              this.selectedNodeUri
            }, reclassifyUri);
            this.tree.SelectedNode.Remove();
            this.SelectNodeAndFireEvent(parent);
          }
        }
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowMessageBox((IWin32Window) this, ex.Message + "\n" + this._resources.GetString("CannotDeleteNode"), (string) null, ClientResources.GenericTeamFoundationCaption(), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
      }
    }

    private void ReorderNode(int increment)
    {
      try
      {
        Cursor.Current = Cursors.WaitCursor;
        this.css.ReorderNode(this.selectedNodeUri, increment);
        TreeNode selectedNode = this.tree.SelectedNode;
        TreeNode parent = selectedNode.Parent;
        int index = selectedNode.Parent.Nodes.IndexOf(selectedNode);
        TreeNode node = selectedNode.Parent.Nodes[index + increment];
        if (increment < 0)
        {
          parent.Nodes.RemoveAt(index);
          parent.Nodes.Insert(index + increment, selectedNode);
        }
        else
        {
          parent.Nodes.RemoveAt(index + increment);
          parent.Nodes.Insert(index, node);
        }
        this.SelectNodeAndFireEvent(selectedNode);
      }
      catch (Exception ex)
      {
        CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
        MessageBoxOptions options = (MessageBoxOptions) 0;
        if (currentUiCulture.TextInfo.IsRightToLeft)
          options |= MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
        int num = (int) MessageBox.Show(ex.Message + "\n" + this._resources.GetString("CannotReorderNode"), this._resources.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, options);
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }

    private void menuMoveUp_Click(object sender, EventArgs e) => this.ReorderNode(-1);

    private void menuMoveDown_Click(object sender, EventArgs e) => this.ReorderNode(1);

    private void menuIndent_Click(object sender, EventArgs e) => this.OnIndentButtonClick();

    private void menuOutdent_Click(object sender, EventArgs e) => this.OnUnindentButtonClick();

    private void menuRefresh_Click(object sender, EventArgs e)
    {
      Cursor cursor = this.Cursor;
      try
      {
        this.Cursor = Cursors.WaitCursor;
        this.RefreshView(this.tree.SelectedNode);
      }
      catch (Exception ex)
      {
        CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
        MessageBoxOptions options = MessageBoxOptions.DefaultDesktopOnly;
        if (currentUiCulture.TextInfo.IsRightToLeft)
          options |= MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
        int num = (int) MessageBox.Show(ex.Message + "\n" + this._resources.GetString("CannotRetrieveStructure"), this._resources.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, options);
      }
      finally
      {
        this.Cursor = cursor;
      }
    }

    private void OnTreeViewToolBarButtonClick(object sender, ToolStripItemClickedEventArgs e)
    {
      if (this._inNodeEdit)
        Console.Beep();
      else if (e.ClickedItem == this._addChildButton)
        this.menuNew_Click((object) this, EventArgs.Empty);
      else if (e.ClickedItem == this._removeButton)
        this.menuDelete_Click((object) this, EventArgs.Empty);
      else if (e.ClickedItem == this._moveUpButton)
        this.menuMoveUp_Click((object) this, EventArgs.Empty);
      else if (e.ClickedItem == this._unindentButton)
        this.OnUnindentButtonClick();
      else if (e.ClickedItem == this._indentButton)
      {
        this.OnIndentButtonClick();
      }
      else
      {
        if (e.ClickedItem != this._moveDownButton)
          return;
        this.menuMoveDown_Click((object) this, EventArgs.Empty);
      }
    }

    private void EnableAllMenuItems(bool enable)
    {
      foreach (MenuItem menuItem in this.menu.MenuItems)
        menuItem.Enabled = enable;
    }

    private void UpdateEnabledState()
    {
      TreeNode selectedNode = this.tree.SelectedNode;
      this._addRootButton.Enabled = false;
      if (selectedNode != null)
      {
        this._addChildButton.Enabled = true;
        this._removeButton.Enabled = this.tree.SelectedNode.Parent != null;
        this._moveUpButton.Enabled = this.tree.SelectedNode.Parent != null && this.tree.SelectedNode.Parent.Nodes.IndexOf(this.tree.SelectedNode) != 0;
        this._moveDownButton.Enabled = this.tree.SelectedNode.Parent != null && this.tree.SelectedNode.Parent.Nodes.IndexOf(this.tree.SelectedNode) != this.tree.SelectedNode.Parent.Nodes.Count - 1;
        this._indentButton.Enabled = selectedNode.PrevNode != null;
        this._unindentButton.Enabled = selectedNode.Parent != null && selectedNode.Parent.Parent != null;
      }
      else
      {
        this._addChildButton.Enabled = false;
        this._removeButton.Enabled = false;
        this._moveUpButton.Enabled = false;
        this._moveDownButton.Enabled = false;
        this._indentButton.Enabled = false;
        this._unindentButton.Enabled = false;
      }
    }

    private void OnUnindentButtonClick()
    {
      try
      {
        TreeNode selectedNode = this.tree.SelectedNode;
        if (selectedNode == null)
          return;
        TreeNode parent = selectedNode.Parent;
        if (parent == null)
          return;
        TreeNodeCollection nodes = this.tree.Nodes;
        if (parent.Parent == null)
          return;
        this.MoveTreeNode(selectedNode, parent.Parent);
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void OnIndentButtonClick()
    {
      try
      {
        TreeNode selectedNode = this.tree.SelectedNode;
        if (selectedNode == null)
          return;
        TreeNode prevNode = selectedNode.PrevNode;
        if (prevNode == null)
          return;
        this.MoveTreeNode(selectedNode, prevNode);
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void menu_Popup(object sender, EventArgs e)
    {
      this.menuRename.Enabled = this.tree.SelectedNode.Parent != null;
      this.menuDelete.Enabled = this.tree.SelectedNode.Parent != null;
      this.menuMoveUp.Enabled = this.tree.SelectedNode.Parent != null && this.tree.SelectedNode.Parent.Nodes.IndexOf(this.tree.SelectedNode) != 0;
      this.menuMoveDown.Enabled = this.tree.SelectedNode.Parent != null && this.tree.SelectedNode.Parent.Nodes.IndexOf(this.tree.SelectedNode) != this.tree.SelectedNode.Parent.Nodes.Count - 1;
      this.menuIndent.Enabled = this.tree.SelectedNode.PrevNode != null;
      this.menuOutdent.Enabled = this.tree.SelectedNode.Parent != null && this.tree.SelectedNode.Parent.Parent != null;
    }

    private void tree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
    {
      if (!this._inCreateNewNode)
      {
        try
        {
          this.CheckPermissions("GENERIC_WRITE");
        }
        catch
        {
          e.CancelEdit = true;
          return;
        }
      }
      if (e.Node.Parent == null)
      {
        e.CancelEdit = true;
      }
      else
      {
        this._inNodeEdit = true;
        IntPtr handle = Microsoft.TeamFoundation.Common.Internal.NativeMethods.SendMessage(new HandleRef((object) e.Node.TreeView, e.Node.TreeView.Handle), 4367, IntPtr.Zero, IntPtr.Zero);
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.SendMessage(new HandleRef((object) e.Node.TreeView, handle), 197, new IntPtr((int) byte.MaxValue), IntPtr.Zero);
        this.EnableAllMenuItems(false);
      }
    }

    private void tree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
    {
      this._inNodeEdit = false;
      this.EnableAllMenuItems(true);
      string name = e.Label != null ? e.Label.Trim() : (string) null;
      if (name != null)
      {
        string text = (string) null;
        if (string.IsNullOrEmpty(name))
          text = this._resources.GetString("ErrorCssNameEmpty");
        else if (CssUtils.IsReservedCssName(name))
          text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, this._resources.GetString("ErrorIsReservedCssName"), (object) name);
        else if (CssUtils.HasInvalidCssCharacters(name))
          text = this._resources.GetString("ErrorCssNameContainsInvalidCharacter");
        if (text != null)
        {
          int num = (int) UIHost.ShowMessageBox((IWin32Window) this, text, "vstf.css.InvalidCssNodeName", (string) null, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
          this.tree.SelectedNode.BeginEdit();
          e.CancelEdit = true;
          return;
        }
      }
      if (this._inCreateNewNode)
        this.CreateNodeAfterEdit(e);
      else
        this.RenameNodeAfterEdit(e);
    }

    private void CreateNodeAfterEdit(NodeLabelEditEventArgs e)
    {
      TreeNode node1 = this.tree.SelectedNode;
      CssNode tag = (CssNode) node1.Parent.Tag;
      this._inCreateNewNode = false;
      try
      {
        Cursor.Current = Cursors.WaitCursor;
        string nodeName = (e.Label != null ? e.Label : node1.Text).Trim();
        string node2 = this.css.CreateNode(nodeName, tag.ID);
        ((CssNode) e.Node.Tag).ID = node2;
        ((CssNode) e.Node.Tag).Name = nodeName;
        e.CancelEdit = true;
        e.Node.Text = nodeName;
      }
      catch (Exception ex)
      {
        e.CancelEdit = true;
        this.tree.SelectedNode = (TreeNode) null;
        TreeNode parent = node1.Parent;
        parent.Nodes.Remove(node1);
        CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
        MessageBoxOptions options = (MessageBoxOptions) 0;
        if (currentUiCulture.TextInfo.IsRightToLeft)
          options |= MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
        int num = (int) MessageBox.Show(ex.Message + "\n" + this._resources.GetString("CannotCreateNode"), this._resources.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, options);
        node1 = parent;
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
      this.BeginInvoke((Delegate) new CommonStructureEdit.TreeNodeDelegate(this.SelectNodeAndFireEvent), (object) node1);
    }

    private void RenameNodeAfterEdit(NodeLabelEditEventArgs e)
    {
      string str = e.Label != null ? e.Label.Trim() : (string) null;
      if (string.IsNullOrEmpty(str))
      {
        e.CancelEdit = true;
      }
      else
      {
        try
        {
          Cursor.Current = Cursors.WaitCursor;
          string newNodeName = str;
          this.css.RenameNode(this.selectedNodeUri, newNodeName);
          e.CancelEdit = true;
          e.Node.Text = newNodeName;
        }
        catch (Exception ex)
        {
          CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
          MessageBoxOptions options = (MessageBoxOptions) 0;
          if (currentUiCulture.TextInfo.IsRightToLeft)
            options |= MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
          int num = (int) MessageBox.Show(ex.Message + "\n" + this._resources.GetString("CannotRenameNode"), this._resources.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, options);
          e.CancelEdit = true;
        }
        finally
        {
          Cursor.Current = Cursors.Default;
        }
      }
    }

    private void SelectChanged(TreeViewEventArgs e)
    {
      if (e == null || e.Node == null || e.Node.Tag == null)
      {
        this.selectedName = string.Empty;
        this.selectedType = string.Empty;
        this.selectedNodeUri = string.Empty;
      }
      else
      {
        CssNode tag = (CssNode) e.Node.Tag;
        this.selectedCssNode = tag;
        this.selectedName = tag.Name;
        this.selectedNodeUri = tag.ID;
      }
      this.UpdateEnabledState();
    }

    private void tree_AfterSelect(object sender, TreeViewEventArgs e)
    {
      this.SelectChanged(e);
      if (this.NodeSelectChange == null)
        return;
      this.NodeSelectChange((object) this, EventArgs.Empty);
    }

    private void SelectNodeAndFireEvent(TreeNode tn)
    {
      try
      {
        this.tree.SelectedNode = tn;
        tn.EnsureVisible();
        this.tree_AfterSelect((object) this, new TreeViewEventArgs(tn));
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void MoveTreeNode(TreeNode node, TreeNode newParent)
    {
      MessageBoxOptions options = (MessageBoxOptions) 0;
      if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
        options = MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
      try
      {
        Cursor.Current = Cursors.WaitCursor;
        this.css.MoveBranch(((CssNode) node.Tag).ID, ((CssNode) newParent.Tag).ID);
        node.Remove();
        newParent.Nodes.Add(node);
        node.EnsureVisible();
        this.SelectNodeAndFireEvent(node);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.Message + "\n" + this._resources.GetString("CannotMoveNode"), this._resources.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, options);
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }

    private void tree_DragDrop(object sender, DragEventArgs e)
    {
      TreeNode nodeAt = this.tree.GetNodeAt(this.tree.PointToClient(new Point(e.X, e.Y)));
      TreeNode data = (TreeNode) e.Data.GetData(typeof (TreeNode));
      if (data == null || nodeAt == null)
        return;
      this.MoveTreeNode(data, nodeAt);
    }

    private void tree_DragEnter(object sender, DragEventArgs e) => e.Effect = e.AllowedEffect;

    private void tree_DragOver(object sender, DragEventArgs e) => this.tree.SelectedNode = this.tree.GetNodeAt(this.tree.PointToClient(new Point(e.X, e.Y)));

    private void tree_ItemDrag(object sender, ItemDragEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;
      if (e.Item == this.tree.Nodes[0])
      {
        CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
        MessageBoxOptions options = (MessageBoxOptions) 0;
        if (currentUiCulture.TextInfo.IsRightToLeft)
          options |= MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
        int num = (int) MessageBox.Show(this._resources.GetString("CannotMoveRootNode"), this._resources.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, options);
      }
      else
      {
        int num1 = (int) this.DoDragDrop(e.Item, DragDropEffects.Move);
      }
    }

    private void tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      if (e.Button != MouseButtons.Right)
        return;
      this.SelectNodeAndFireEvent(this.tree.GetNodeAt(e.X, e.Y));
    }

    private void CheckPermissions(string actionId) => ((IAuthorizationService) this._tfs.GetService(typeof (IAuthorizationService))).CheckPermission(this.selectedNodeUri, actionId);

    private delegate void TreeNodeDelegate(TreeNode tn);
  }
}
