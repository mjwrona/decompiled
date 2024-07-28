// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.TreeNodeCollection`1
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TreeNodeCollection<T> : ObservableCollection<TreeNode<T>>
  {
    public TreeNode<T> Find(string nodeString, StringComparison comparisonType)
    {
      foreach (TreeNode<T> treeNode in (Collection<TreeNode<T>>) this)
      {
        if (string.Equals(nodeString, treeNode.ToString(), comparisonType))
          return treeNode;
      }
      return (TreeNode<T>) null;
    }
  }
}
