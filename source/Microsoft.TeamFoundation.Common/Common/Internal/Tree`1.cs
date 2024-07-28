// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.Tree`1
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class Tree<T>
  {
    public Tree() => this.HiddenRoot = new TreeNode<T>(default (T));

    private TreeNode<T> HiddenRoot { get; set; }

    public void Clear() => this.HiddenRoot.ChildNodes.Clear();

    public TreeNodeCollection<T> RootNodes => this.HiddenRoot.ChildNodes;

    public TreeNode<T> Find(string nodePath, string delimiter, StringComparison comparisonType) => this.HiddenRoot.Find(nodePath, delimiter, comparisonType);

    public TreeNodeCollection<T> GetSelectedNodes() => this.HiddenRoot.GetSelectedNodes();

    public TreeNodeCollection<T> GetCheckedNodes() => this.HiddenRoot.GetCheckedNodes();
  }
}
