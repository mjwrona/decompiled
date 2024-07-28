// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ComboTree
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.ComponentModel;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DesignerCategory("Code")]
  public class ComboTree : CustomCombo<ComboTreeView>
  {
    private ComboTreeView m_trvNodes;

    public ComboTree()
    {
      this.m_trvNodes = new ComboTreeView(this);
      this.m_trvNodes.BeforeExpand += new TreeViewCancelEventHandler(this.OnBeforeExpand);
      this.m_trvNodes.BeforeCollapse += new TreeViewCancelEventHandler(this.OnBeforeCollapse);
      this.Initialize(this.m_trvNodes);
    }

    public TreeNodeCollection TreeNodes => this.m_trvNodes.Nodes;

    public TreeView Tree => (TreeView) this.m_trvNodes;

    public event TreeViewCancelEventHandler BeforeExpandTreeNode;

    public event TreeViewCancelEventHandler BeforeCollapseTreeNode;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.m_trvNodes != null)
        this.m_trvNodes.Dispose();
      base.Dispose(disposing);
    }

    protected internal virtual TreeNode GetValidNode(TreeNode selectedNode) => selectedNode;

    protected virtual void OnBeforeExpand(object sender, TreeViewCancelEventArgs e)
    {
      if (this.BeforeExpandTreeNode == null)
        return;
      this.BeforeExpandTreeNode(sender, e);
    }

    protected virtual void OnBeforeCollapse(object sender, TreeViewCancelEventArgs e)
    {
      if (this.BeforeCollapseTreeNode == null)
        return;
      this.BeforeCollapseTreeNode(sender, e);
    }
  }
}
