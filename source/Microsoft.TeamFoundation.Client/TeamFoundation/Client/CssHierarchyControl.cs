// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CssHierarchyControl
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CssHierarchyControl : ComboTree
  {
    private ICssDataProvider m_dataProvider;
    private bool m_isRootLoaded;
    private const char cDefaultPathSplitter = '\\';
    private char m_pathSplitter = '\\';
    private ImageList m_imageList;

    public CssHierarchyControl()
    {
      this.DrawMode = DrawMode.OwnerDrawFixed;
      this.TextChanged += new EventHandler(this.OnTextChanged);
    }

    public CssHierarchyControl(ICssDataProvider dataProvider)
      : this()
    {
      this.m_dataProvider = dataProvider;
    }

    public ImageList ImageList
    {
      get => this.m_imageList;
      set
      {
        this.m_imageList = value;
        this.Tree.ImageList = value;
      }
    }

    public ICssDataProvider DataProvider
    {
      get => this.m_dataProvider;
      set
      {
        this.m_dataProvider = value;
        this.InitControl();
      }
    }

    public void Reset() => this.InitControl();

    protected char PathSplitter
    {
      get => this.m_pathSplitter;
      set
      {
        this.m_pathSplitter = value;
        this.InitControl();
      }
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
      try
      {
        base.OnDrawItem(e);
        e.DrawBackground();
        if (e.Index >= 0)
        {
          string text = this.Text;
          if (!this.m_isRootLoaded)
          {
            this.LoadRootNodes();
            this.ExpandPath();
          }
          bool enabled = this.Enabled;
          if (!enabled)
          {
            using (Brush brush = (Brush) new SolidBrush(SystemColors.Control))
              e.Graphics.FillRectangle(brush, e.Bounds);
          }
          int index = -1;
          if (this.ImageList != null)
          {
            TreeNode selectedNode = this.Tree.SelectedNode;
            index = selectedNode == null || !(selectedNode.FullPath == this.Text) ? -1 : selectedNode.ImageIndex;
          }
          if (index >= 0)
          {
            int num = (e.Bounds.Height - this.ImageList.ImageSize.Height) / 2;
            Rectangle bounds1 = e.Bounds;
            int left = bounds1.Left;
            bounds1 = e.Bounds;
            int top1 = bounds1.Top + num;
            bounds1 = e.Bounds;
            int right1 = bounds1.Left + this.ImageList.ImageSize.Width;
            bounds1 = e.Bounds;
            int bottom1 = bounds1.Top + this.ImageList.ImageSize.Height;
            Rectangle rect = Rectangle.FromLTRB(left, top1, right1, bottom1);
            int right2 = rect.Right;
            bounds1 = e.Bounds;
            int top2 = bounds1.Top + 1;
            bounds1 = e.Bounds;
            int right3 = bounds1.Right;
            bounds1 = e.Bounds;
            int bottom2 = bounds1.Bottom;
            Rectangle bounds2 = Rectangle.FromLTRB(right2, top2, right3, bottom2);
            if (index >= 0 && index < this.ImageList.Images.Count)
            {
              if (enabled)
                e.Graphics.DrawImage(this.m_imageList.Images[index], rect);
              else
                ControlPaint.DrawImageDisabled(e.Graphics, this.m_imageList.Images[index], rect.Left, rect.Top, SystemColors.Control);
            }
            TextRenderer.DrawText((IDeviceContext) e.Graphics, text, e.Font, bounds2, enabled ? e.ForeColor : SystemColors.GrayText, TextFormatFlags.TextBoxControl);
          }
          else
          {
            int left = e.Bounds.Left;
            Rectangle bounds3 = e.Bounds;
            int top = bounds3.Top + 1;
            bounds3 = e.Bounds;
            int right = bounds3.Right;
            bounds3 = e.Bounds;
            int bottom = bounds3.Bottom;
            Rectangle bounds4 = Rectangle.FromLTRB(left, top, right, bottom);
            TextRenderer.DrawText((IDeviceContext) e.Graphics, text, e.Font, bounds4, enabled ? e.ForeColor : SystemColors.GrayText, TextFormatFlags.TextBoxControl);
          }
        }
        e.DrawFocusRectangle();
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
    }

    protected override void OnDropDown(EventArgs e)
    {
      try
      {
        if (!this.m_isRootLoaded)
          this.LoadRootNodes();
        this.ExpandPath();
        base.OnDropDown(e);
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException((IWin32Window) this, ex);
      }
    }

    protected override void OnBeforeExpand(object sender, TreeViewCancelEventArgs e)
    {
      try
      {
        this.LoadChildNodes(e.Node);
        e.Node.EnsureVisible();
        TreeNode node = e.Node;
        CssNode tag = node.Tag as CssNode;
        if (tag.ImageIndexExpanded >= 0)
        {
          node.ImageIndex = tag.ImageIndexExpanded;
          node.SelectedImageIndex = tag.ImageIndexExpanded;
        }
        else
        {
          node.ImageIndex = tag.ImageIndex;
          node.SelectedImageIndex = tag.ImageIndex;
        }
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException((IWin32Window) this, ex);
      }
    }

    protected override void OnBeforeCollapse(object sender, TreeViewCancelEventArgs e)
    {
      try
      {
        TreeNode node = e.Node;
        CssNode tag = node.Tag as CssNode;
        node.ImageIndex = tag.ImageIndex;
        node.SelectedImageIndex = tag.ImageIndex;
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException((IWin32Window) this, ex);
      }
    }

    private void OnTextChanged(object sender, EventArgs e)
    {
      try
      {
        if (string.IsNullOrEmpty(this.Text))
          return;
        this.ExpandPath();
        this.Invalidate();
      }
      catch (Exception ex)
      {
        int num = (int) UIHost.ShowException(ex);
      }
    }

    private void InitControl()
    {
      if (this.m_dataProvider != null)
      {
        this.Tree.PathSeparator = this.m_pathSplitter.ToString();
        this.SetSelectedText(this.m_dataProvider.GetInitialPath(), 0);
      }
      this.m_isRootLoaded = false;
      this.Tree.Nodes.Clear();
    }

    protected void LoadRootNodes()
    {
      if (this.m_isRootLoaded || this.m_dataProvider == null)
        return;
      this.LoadChildNodes((TreeNode) null);
      this.m_isRootLoaded = true;
      if (UIHost.IsVistaOrNewer)
        return;
      this.Tree.ShowLines = this.Tree.Nodes.Count != 1 || this.Tree.Nodes[0].Nodes.Count != 0;
    }

    private void LoadChildNodes(TreeNode parentTreeNode, bool force = false)
    {
      if (this.m_dataProvider == null || !force && !this.NeedsLoading(parentTreeNode))
        return;
      TreeNodeCollection treeNodeCollection = parentTreeNode == null ? this.Tree.Nodes : parentTreeNode.Nodes;
      CssNode tag = parentTreeNode == null ? (CssNode) null : (CssNode) parentTreeNode.Tag;
      treeNodeCollection.Clear();
      CssNodeCollection childNodes = this.m_dataProvider.GetChildNodes(tag);
      bool flag = false;
      if (childNodes != null && childNodes.Count > 0)
      {
        foreach (CssNode cssNode in (List<CssNode>) childNodes)
        {
          if (cssNode is CssLoadingNode)
            flag = true;
          TreeNode wrappingTreeNode = this.GetWrappingTreeNode(cssNode);
          treeNodeCollection.Add(wrappingTreeNode);
        }
      }
      if (!flag)
        return;
      tag.Populated += (EventHandler<Exception>) ((s, e) =>
      {
        try
        {
          if (this.IsDisposed)
            return;
          if (e == null)
          {
            this.LoadChildNodes(parentTreeNode, true);
          }
          else
          {
            if (parentTreeNode.Nodes.Count != 1 || !(parentTreeNode.Nodes[0].Tag is CssLoadingNode))
              return;
            parentTreeNode.Nodes[0].Text = e.Message;
          }
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.TraceException(ex);
        }
      });
    }

    private bool NeedsLoading(TreeNode parentTreeNode)
    {
      if (parentTreeNode != null && parentTreeNode.Nodes != null && parentTreeNode.Nodes.Count == 1 && parentTreeNode.Nodes[0] is CssHierarchyControl.DummyNode)
        return true;
      return parentTreeNode == null && !this.m_isRootLoaded;
    }

    private TreeNode GetWrappingTreeNode(CssNode cssNode)
    {
      TreeNode wrappingTreeNode = new TreeNode(cssNode.Name);
      wrappingTreeNode.Name = cssNode.Name;
      wrappingTreeNode.Tag = (object) cssNode;
      wrappingTreeNode.ImageIndex = cssNode.ImageIndex;
      wrappingTreeNode.SelectedImageIndex = cssNode.ImageIndex;
      if (cssNode.HasChildren)
        wrappingTreeNode.Nodes.Add((TreeNode) new CssHierarchyControl.DummyNode());
      return wrappingTreeNode;
    }

    private void ExpandPath()
    {
      string[] strArray = this.Text.Split(new char[1]
      {
        this.m_pathSplitter
      }, StringSplitOptions.RemoveEmptyEntries);
      TreeNodeCollection nodes = this.Tree.Nodes;
      TreeNode parentTreeNode1 = (TreeNode) null;
      foreach (string key in strArray)
      {
        TreeNode parentTreeNode2 = nodes[key];
        if (parentTreeNode2 != null)
        {
          this.LoadChildNodes(parentTreeNode2);
          nodes = parentTreeNode2.Nodes;
          parentTreeNode1 = parentTreeNode2;
        }
        else
          break;
      }
      if (parentTreeNode1 != null)
      {
        parentTreeNode1.EnsureVisible();
        this.Tree.SelectedNode = parentTreeNode1;
        this.LoadChildNodes(parentTreeNode1);
        parentTreeNode1.Expand();
      }
      else
        this.ExpandDefaultRootNode();
    }

    private void ExpandDefaultRootNode()
    {
      if (this.m_dataProvider == null)
        return;
      TreeNode parentTreeNode = (TreeNode) null;
      TreeNodeCollection nodes = this.Tree.Nodes;
      if (nodes.Count == 1)
      {
        parentTreeNode = nodes[0];
      }
      else
      {
        string defaultRootNodeName = this.m_dataProvider.GetDefaultRootNodeName();
        if (defaultRootNodeName != null && defaultRootNodeName.Length > 0)
          parentTreeNode = nodes[defaultRootNodeName];
      }
      if (parentTreeNode == null)
        return;
      this.LoadChildNodes(parentTreeNode);
      parentTreeNode.Expand();
      this.Tree.SelectedNode = parentTreeNode;
    }

    private class DummyNode : TreeNode
    {
    }
  }
}
