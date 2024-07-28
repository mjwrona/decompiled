// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HierarchicalStore`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HierarchicalStore<T> : SparseTree<T>, ITokenStore<T>
  {
    public HierarchicalStore(
      char separatorValue,
      int fixedElementLength,
      StringComparison tokenComparison)
      : base(separatorValue, fixedElementLength, tokenComparison)
    {
    }

    private HierarchicalStore(IVssRequestContext requestContext, HierarchicalStore<T> source)
      : base((SparseTree<T>) source, false, true)
    {
    }

    public bool IsSubItem(string item, string parent)
    {
      if (this.TokenSeparator == char.MinValue)
        return item.StartsWith(parent, this.TokenComparison);
      if (parent == null)
        return true;
      if (item == null || !item.StartsWith(parent, this.TokenComparison))
        return false;
      return item.Length == parent.Length || parent.Length > 0 && (int) parent[parent.Length - 1] == (int) this.TokenSeparator || (int) item[parent.Length] == (int) this.TokenSeparator;
    }

    public bool HasSubItem(string token) => this.EnumSubTree(token, EnumSubTreeOptions.EnumerateSubTreeRoot, int.MaxValue).Any<EnumeratedSparseTreeNode<T>>();

    public ITokenStore<T> Copy(IVssRequestContext requestContext) => (ITokenStore<T>) new HierarchicalStore<T>(requestContext, this);

    IEnumerable<T> ITokenStore<T>.EnumSubTree(string token, bool enumSubTreeRoot)
    {
      HierarchicalStore<T> hierarchicalStore = this;
      EnumSubTreeOptions options = EnumSubTreeOptions.None;
      if (enumSubTreeRoot)
        options |= EnumSubTreeOptions.EnumerateSubTreeRoot;
      foreach (EnumeratedSparseTreeNode<T> enumeratedSparseTreeNode in hierarchicalStore.EnumSubTree(token, options, int.MaxValue))
        yield return enumeratedSparseTreeNode.ReferencedObject;
    }

    void ITokenStore<T>.EnumAndEvaluateParents(
      string token,
      bool includeSparseNodes,
      Func<string, T, string, bool, bool> evaluate)
    {
      EnumParentsOptions options = EnumParentsOptions.IncludeAdditionalData;
      if (includeSparseNodes)
        options |= EnumParentsOptions.IncludeSparseNodes;
      this.EnumAndEvaluateParents(token, options, new SparseTree<T>.EnumNodeCallback(evaluate.Invoke));
    }

    int ITokenStore<T>.get_Count() => this.Count;

    bool ITokenStore<T>.TryGetValue(string token, out T referencedObject) => this.TryGetValue(token, out referencedObject);

    T ITokenStore<T>.GetOrAdd<X>(string token, Func<X, T> valueFactory, X valueFactoryParameter = null) => this.GetOrAdd<X>(token, valueFactory, valueFactoryParameter);

    bool ITokenStore<T>.Remove(string token, bool recurse) => this.Remove(token, recurse);

    T ITokenStore<T>.get_Item(string token) => this[token];

    void ITokenStore<T>.set_Item(string token, T value) => this[token] = value;

    void ITokenStore<T>.Clear() => this.Clear();

    void ITokenStore<T>.Seal() => this.Seal();
  }
}
