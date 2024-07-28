// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.EnumeratedSparseTreeNode`1
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public struct EnumeratedSparseTreeNode<X>
  {
    public string Token;
    public X ReferencedObject;
    public bool HasChildren;
    public string NoChildrenBelow;
    public bool IsExactMatch;

    public EnumeratedSparseTreeNode(
      string token,
      X referencedObject,
      bool hasChildren,
      string noChildrenBelow,
      bool isExactMatch)
    {
      this.Token = token;
      this.ReferencedObject = referencedObject;
      this.HasChildren = hasChildren;
      this.NoChildrenBelow = noChildrenBelow;
      this.IsExactMatch = isExactMatch;
    }
  }
}
