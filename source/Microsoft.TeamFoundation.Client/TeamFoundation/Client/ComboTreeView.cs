// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ComboTreeView
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DesignerCategory("Code")]
  public class ComboTreeView : TreeView, IComboHostedControl, IDisposable
  {
    private const int cMinDropdownItems = 6;
    private const int cMaxDropdownItems = 16;
    private const int cTreeViewMargin = 8;
    private ComboTree m_parent;

    public ComboTreeView(ComboTree parent)
    {
      this.m_parent = parent;
      this.AutoSize = true;
      this.HotTracking = true;
      this.HideSelection = false;
      this.BorderStyle = BorderStyle.None;
      this.Location = new Point(0, 0);
      this.LostFocus += new EventHandler(this.m_trvNodes_LostFocus);
      this.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.m_trvNodes_NodeMouseClick);
      this.AfterExpand += new TreeViewEventHandler(this.m_trvNodes_AfterExpand);
      this.SetStyle(ControlStyles.Selectable, false);
      UIHost.WinformsStyler.Style((TreeView) this);
    }

    public string GetSelectedText(out int caretPos)
    {
      string selectedText = string.Empty;
      if (this.SelectedNode != null)
        selectedText = this.SelectedNode.FullPath;
      caretPos = selectedText.Length;
      return selectedText;
    }

    public bool Filter(string text) => false;

    public Size GetDesiredControlSize() => new Size(this.m_parent.Width - 8, Math.Min(16 * this.ItemHeight + 8, (this.GetTotalVisibleCount(this.Nodes) + 1) * this.ItemHeight + 8));

    public Size GetMinimumControlSize() => new Size(this.m_parent.Width - 8, 6 * this.ItemHeight + 8);

    public void HandleComboControlFocusLost()
    {
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Return || e.KeyCode == Keys.Space)
        this.SetSelection();
      else if (e.KeyCode == Keys.Tab)
      {
        this.SetSelection();
        this.m_parent.PostKey(e.KeyCode, e.KeyData);
      }
      else if (e.KeyCode == Keys.F4 || e.KeyCode == Keys.Escape)
        this.m_parent.HideDropDown();
      else if (e.Alt && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
        this.m_parent.HideDropDown();
      else
        base.OnKeyDown(e);
    }

    public override bool PreProcessMessage(ref Message msg)
    {
      if (msg.Msg == 256)
      {
        if ((int) msg.WParam == 9)
        {
          this.SetSelection();
          Microsoft.TeamFoundation.Common.Internal.NativeMethods.PostMessage(new HandleRef((object) this, this.m_parent.Handle), 256, msg.WParam, msg.LParam);
          return true;
        }
        if ((int) msg.WParam == 27)
        {
          this.m_parent.HideDropDown();
          return true;
        }
      }
      else if (msg.Msg == 258 && (int) msg.WParam == 13)
        return true;
      return base.PreProcessMessage(ref msg);
    }

    private void m_trvNodes_LostFocus(object sender, EventArgs e) => this.m_parent.HideDropDown();

    private void m_trvNodes_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      if (e.Node == null || e.X < e.Node.Bounds.Left)
        return;
      string fullPath = this.m_parent.GetValidNode(e.Node).FullPath;
      this.m_parent.SetSelection(fullPath, fullPath.Length, true);
    }

    private void m_trvNodes_AfterExpand(object sender, TreeViewEventArgs e)
    {
      int height = this.GetDesiredControlSize().Height;
      Rectangle screen1 = this.RectangleToScreen(this.m_parent.ClientRectangle);
      Rectangle screen2 = this.RectangleToScreen(this.Parent.ClientRectangle);
      Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
      if (screen2.Top < screen1.Top)
        return;
      int num = workingArea.Height - screen1.Y - screen1.Height;
      if (height >= num || screen2.Height >= height)
        return;
      this.m_parent.RecalculateSize();
    }

    private void SetSelection()
    {
      this.SelectedNode = this.m_parent.GetValidNode(this.SelectedNode);
      string fullPath = this.SelectedNode.FullPath;
      this.m_parent.SetSelection(fullPath, fullPath.Length, true);
    }

    private int GetTotalVisibleCount(TreeNodeCollection nodes)
    {
      int count = nodes.Count;
      foreach (TreeNode node in nodes)
      {
        if (node.IsExpanded)
          count += this.GetTotalVisibleCount(node.Nodes);
      }
      return count;
    }
  }
}
