// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.TreeNode`1
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TreeNode<T> : INotifyPropertyChanged
  {
    private T m_value;
    private TreeNode<T> m_parent;
    private object m_tag;
    private bool m_isSelected;
    private bool m_isChecked;
    private TreeNode<T>.TreeNodeChildCollection<T> m_children;

    public TreeNode(T t) => this.Value = t;

    public T Value
    {
      get => this.m_value;
      set
      {
        this.m_value = value;
        this.FireNotifyPropertyChanged(nameof (Value));
      }
    }

    public TreeNode<T> Parent
    {
      get => this.m_parent;
      set
      {
        this.m_parent = value;
        this.FireNotifyPropertyChanged(nameof (Parent));
      }
    }

    public override string ToString() => this.Value.ToString();

    public bool HasChildren => this.m_children != null && this.m_children.Count > 0;

    public TreeNodeCollection<T> ChildNodes
    {
      get
      {
        if (this.m_children == null)
        {
          this.m_children = new TreeNode<T>.TreeNodeChildCollection<T>(this);
          this.FireNotifyPropertyChanged(nameof (ChildNodes));
          this.FireNotifyPropertyChanged("HasChildren");
        }
        return (TreeNodeCollection<T>) this.m_children;
      }
    }

    public object Tag
    {
      get => this.m_tag;
      set
      {
        this.m_tag = value;
        this.FireNotifyPropertyChanged(nameof (Tag));
      }
    }

    public bool IsSelected
    {
      get => this.m_isSelected;
      set
      {
        if (value == this.m_isSelected)
          return;
        this.m_isSelected = value;
        this.FireNotifyPropertyChanged(nameof (IsSelected));
      }
    }

    public TreeNodeCollection<T> GetSelectedNodes()
    {
      TreeNodeCollection<T> selectedNodes = new TreeNodeCollection<T>();
      this.GetSelectedNodes(selectedNodes);
      return selectedNodes;
    }

    private void GetSelectedNodes(TreeNodeCollection<T> selectedNodes)
    {
      if (this.IsSelected)
        selectedNodes.Add(this);
      if (!this.HasChildren)
        return;
      foreach (TreeNode<T> childNode in (Collection<TreeNode<T>>) this.ChildNodes)
        childNode.GetSelectedNodes(selectedNodes);
    }

    public bool IsChecked
    {
      get => this.m_isChecked;
      set
      {
        if (value == this.m_isChecked)
          return;
        this.m_isChecked = value;
        this.FireNotifyPropertyChanged(nameof (IsChecked));
      }
    }

    public void SetChecked(bool isChecked, bool recursive)
    {
      this.IsChecked = isChecked;
      if (!recursive || !this.HasChildren)
        return;
      foreach (TreeNode<T> childNode in (Collection<TreeNode<T>>) this.ChildNodes)
        childNode.SetChecked(isChecked, recursive);
    }

    public TreeNodeCollection<T> GetCheckedNodes()
    {
      TreeNodeCollection<T> checkedNodes = new TreeNodeCollection<T>();
      this.GetCheckedNodes(checkedNodes);
      return checkedNodes;
    }

    private void GetCheckedNodes(TreeNodeCollection<T> checkedNodes)
    {
      if (this.IsChecked)
        checkedNodes.Add(this);
      if (!this.HasChildren)
        return;
      foreach (TreeNode<T> childNode in (Collection<TreeNode<T>>) this.ChildNodes)
        childNode.GetCheckedNodes(checkedNodes);
    }

    public TreeNode<T> Find(string nodePath, string delimiter, StringComparison comparisonType)
    {
      if (string.IsNullOrEmpty(nodePath))
        throw new ArgumentNullException(nameof (nodePath));
      if (this.m_children == null || this.m_children.Count <= 0)
        return (TreeNode<T>) null;
      string[] strArray;
      if (string.IsNullOrEmpty(delimiter))
        strArray = new string[1]{ nodePath };
      else
        strArray = nodePath.Split(new string[1]{ delimiter }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray == null || strArray.Length == 0)
        return (TreeNode<T>) null;
      TreeNodeCollection<T> treeNodeCollection = (TreeNodeCollection<T>) this.m_children;
      TreeNode<T> treeNode = (TreeNode<T>) null;
      foreach (string nodeString in strArray)
      {
        if (treeNodeCollection == null || treeNodeCollection.Count <= 0)
          return (TreeNode<T>) null;
        treeNode = treeNodeCollection.Find(nodeString, comparisonType);
        if (treeNode == null)
          return (TreeNode<T>) null;
        treeNodeCollection = treeNode.HasChildren ? treeNode.ChildNodes : (TreeNodeCollection<T>) null;
      }
      return treeNode;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void FireNotifyPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    internal class TreeNodeChildCollection<U> : TreeNodeCollection<U>
    {
      private TreeNode<U> m_parent;

      public TreeNodeChildCollection(TreeNode<U> parent) => this.m_parent = parent;

      protected override void InsertItem(int index, TreeNode<U> item)
      {
        item.Parent = this.m_parent;
        base.InsertItem(index, item);
      }

      protected override void SetItem(int index, TreeNode<U> item)
      {
        item.Parent = this.m_parent;
        base.SetItem(index, item);
      }

      protected override void RemoveItem(int index)
      {
        this[index].Parent = (TreeNode<U>) null;
        base.RemoveItem(index);
      }
    }
  }
}
