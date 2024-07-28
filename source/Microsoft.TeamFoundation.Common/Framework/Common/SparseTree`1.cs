// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.SparseTree`1
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SparseTree<T>
  {
    public readonly StringComparison TokenComparison;
    public readonly char TokenSeparator;
    public readonly int FixedElementLength;
    private SparseTree<T>.SparseTreeNode<T> m_rootNode;
    private int m_count;
    private bool m_sealed;

    public SparseTree(char tokenSeparator, StringComparison comparison)
      : this(tokenSeparator, -1, comparison)
    {
    }

    public SparseTree(int fixedElementLength, StringComparison comparison)
      : this(char.MinValue, fixedElementLength, comparison)
    {
    }

    public SparseTree(char tokenSeparator, int fixedElementLength, StringComparison comparison)
    {
      if (tokenSeparator == char.MinValue && fixedElementLength <= 0)
        throw new ArgumentException();
      if (tokenSeparator != char.MinValue && fixedElementLength > 0)
        throw new ArgumentException();
      if (comparison != StringComparison.OrdinalIgnoreCase && comparison != StringComparison.Ordinal)
        throw new ArgumentException();
      this.TokenSeparator = tokenSeparator;
      this.FixedElementLength = fixedElementLength;
      this.TokenComparison = comparison;
      this.m_sealed = false;
      this.Clear();
    }

    public SparseTree(SparseTree<T> source, bool deepCopy = true, bool lazyCopy = false)
      : this(source.TokenSeparator, source.FixedElementLength, source.TokenComparison)
    {
      this.m_count = source.m_count;
      if (lazyCopy & deepCopy)
        throw new ArgumentException("LazyCopy and deepCopy cannot both be true. DeepCopy makes a clone of the entire SparseTree while lazyCopy uses copy-on-write to delay copy of each node to each write operation.");
      if (lazyCopy)
      {
        source.CheckSealStatus(true);
        this.m_rootNode = source.m_rootNode.LazyCopy();
      }
      else
        this.m_rootNode = source.m_rootNode.Copy(deepCopy);
    }

    public void Clear()
    {
      this.CheckSealStatus(false);
      this.m_rootNode = new SparseTree<T>.SparseTreeNode<T>((string) null, this.TokenSeparator == char.MinValue ? (int[]) null : Array.Empty<int>(), default (T));
      this.m_count = 0;
    }

    public T GetOrAdd<X>(string token, Func<X, T> valueFactory, X valueFactoryParameter = null)
    {
      T referencedObject;
      if (token == null && this.TokenSeparator != char.MinValue)
      {
        if (this.m_rootNode.HasValue)
        {
          referencedObject = this.m_rootNode.ReferencedObject;
        }
        else
        {
          referencedObject = valueFactory(valueFactoryParameter);
          this.m_rootNode.ReferencedObject = referencedObject;
        }
      }
      else
      {
        token = this.CanonicalizeToken(token);
        SparseTree<T>.SparseTreeNode<T> closestNode;
        int insertIndex;
        bool indexIsChild;
        if (this.FindClosestNode(token, out closestNode, out insertIndex, out indexIsChild, true, !this.m_sealed))
        {
          referencedObject = closestNode.ReferencedObject;
        }
        else
        {
          this.CheckSealStatus(false);
          referencedObject = valueFactory(valueFactoryParameter);
          SparseTree<T>.SparseTreeNode<T> childNode = new SparseTree<T>.SparseTreeNode<T>(token, this.GetTokenIndices(closestNode, token), referencedObject);
          this.AddNode(closestNode, childNode, insertIndex, indexIsChild);
        }
      }
      return referencedObject;
    }

    public void Add(string token, T referencedObject) => this.Add(token, referencedObject, false);

    private void Add(string token, T referencedObject, bool overwrite)
    {
      this.CheckSealStatus(false);
      if (token == null && this.TokenSeparator != char.MinValue)
      {
        if (!overwrite && this.m_rootNode.HasValue)
          throw new ArgumentException(TFCommonResources.SparseTreeTokenAlreadyExists(), nameof (token));
        this.m_rootNode.ReferencedObject = referencedObject;
      }
      else
      {
        token = this.CanonicalizeToken(token);
        SparseTree<T>.SparseTreeNode<T> closestNode;
        int insertIndex;
        bool indexIsChild;
        if (this.FindClosestNode(token, out closestNode, out insertIndex, out indexIsChild, !overwrite, true))
        {
          if (!overwrite)
            throw new ArgumentException(TFCommonResources.SparseTreeTokenAlreadyExists(), nameof (token));
          closestNode.ReferencedObject = referencedObject;
        }
        else
        {
          SparseTree<T>.SparseTreeNode<T> childNode = new SparseTree<T>.SparseTreeNode<T>(token, this.GetTokenIndices(closestNode, token), referencedObject);
          this.AddNode(closestNode, childNode, insertIndex, indexIsChild);
        }
      }
    }

    private int[] GetTokenIndices(SparseTree<T>.SparseTreeNode<T> parent, string token)
    {
      if (this.TokenSeparator == char.MinValue)
        return (int[]) null;
      int length1 = parent.TokenIndices.Length;
      int startIndex1 = length1 == 0 ? 0 : parent.Token.Length + 1;
      do
      {
        int num = token.IndexOf(this.TokenSeparator, startIndex1);
        if (num < 0)
          num = token.Length;
        ++length1;
        startIndex1 = num + 1;
      }
      while (startIndex1 < token.Length);
      int[] destinationArray = new int[length1];
      Array.Copy((Array) parent.TokenIndices, (Array) destinationArray, parent.TokenIndices.Length);
      int length2 = parent.TokenIndices.Length;
      int startIndex2 = length2 == 0 ? 0 : parent.Token.Length + 1;
      do
      {
        destinationArray[length2++] = startIndex2;
        int num = token.IndexOf(this.TokenSeparator, startIndex2);
        if (num < 0)
          num = token.Length;
        startIndex2 = num + 1;
      }
      while (startIndex2 < token.Length);
      return destinationArray;
    }

    private void AddNode(
      SparseTree<T>.SparseTreeNode<T> parentNode,
      SparseTree<T>.SparseTreeNode<T> childNode,
      int insertIndex,
      bool indexIsChild)
    {
      if (parentNode.Children == null)
        parentNode.Children = new List<SparseTree<T>.SparseTreeNode<T>>();
      if (indexIsChild)
      {
        int num = this.EndRangeSeek(parentNode, childNode.Token, childNode.Token.Length);
        childNode.Children = new List<SparseTree<T>.SparseTreeNode<T>>(num - insertIndex + 1);
        for (int index = insertIndex; index <= num; ++index)
          childNode.Children.Add(parentNode.Children[index]);
        parentNode.Children.RemoveRange(insertIndex, num - insertIndex + 1);
      }
      parentNode.Children.Insert(insertIndex, childNode);
      ++this.m_count;
    }

    public bool Remove(string token, bool recurse)
    {
      this.CheckSealStatus(false);
      token = token != null ? this.CanonicalizeToken(token) : throw new ArgumentNullException(nameof (token));
      SparseTree<T>.SparseTreeNode<T> closestNode;
      SparseTree<T>.SparseTreeNode<T> immediateParent;
      int insertIndex;
      int indexInParent;
      bool indexIsChild;
      if (this.FindClosestNode(token, out closestNode, out immediateParent, out insertIndex, out indexInParent, out indexIsChild, copyPath: true))
      {
        immediateParent.Children.RemoveAt(indexInParent);
        if (!recurse)
        {
          if (closestNode.ChildCount > 0)
            immediateParent.Children.InsertRange(indexInParent, (IEnumerable<SparseTree<T>.SparseTreeNode<T>>) closestNode.Children);
          --this.m_count;
        }
        else
          this.m_count -= SparseTree<T>.CountNode(closestNode);
        return true;
      }
      if (!(recurse & indexIsChild))
        return false;
      int num = this.EndRangeSeek(closestNode, token, token.Length);
      for (int index = insertIndex; index <= num; ++index)
        this.m_count -= SparseTree<T>.CountNode(closestNode.Children[index]);
      closestNode.Children.RemoveRange(insertIndex, num - insertIndex + 1);
      return true;
    }

    public T this[string token]
    {
      get
      {
        T referencedObject;
        if (!this.TryGetValue(token, out referencedObject))
          throw new KeyNotFoundException();
        return referencedObject;
      }
      set => this.Add(token, value, true);
    }

    public bool TryGetValue(string token, out T referencedObject) => this.TryGetValue(token, true, out referencedObject);

    public bool TryGetValue(string token, bool exactMatch, out T referencedObject)
    {
      if (token == null && this.TokenSeparator != char.MinValue)
      {
        if (this.m_rootNode.HasValue)
        {
          referencedObject = this.m_rootNode.ReferencedObject;
          return true;
        }
        referencedObject = default (T);
        return false;
      }
      token = this.CanonicalizeToken(token);
      SparseTree<T>.SparseTreeNode<T> closestNode;
      if (this.FindClosestNode(token, out closestNode))
      {
        referencedObject = closestNode.ReferencedObject;
        return true;
      }
      if (exactMatch || closestNode == this.m_rootNode)
      {
        referencedObject = default (T);
        return false;
      }
      referencedObject = closestNode.ReferencedObject;
      return true;
    }

    public bool EnumAndEvaluateParents(
      string token,
      EnumParentsOptions options,
      SparseTree<T>.EnumNodeCallback evaluate)
    {
      if (token != null || !this.m_rootNode.HasValue)
        token = this.CanonicalizeToken(token);
      bool parents = true;
      string noChildrenBelow = (string) null;
      if (token != null)
        parents = this.EnumAndEvaluateParentsHelper(this.m_rootNode, token, options, evaluate, out noChildrenBelow);
      if (parents && this.m_rootNode != null && this.m_rootNode.HasValue)
        parents = evaluate(this.m_rootNode.Token, this.m_rootNode.ReferencedObject, noChildrenBelow, token == null);
      return parents;
    }

    private bool EnumAndEvaluateParentsHelper(
      SparseTree<T>.SparseTreeNode<T> node,
      string token,
      EnumParentsOptions options,
      SparseTree<T>.EnumNodeCallback evaluate,
      out string noChildrenBelow)
    {
      bool parentsHelper = true;
      bool isExactMatch;
      int index = this.Seek(node, token, token.Length, out isExactMatch, out bool _);
      if (index < 0)
      {
        noChildrenBelow = this.GetNoChildrenBelowToken(token, options, node, false);
        if ((EnumParentsOptions.IncludeSparseNodes & options) != EnumParentsOptions.None)
        {
          parentsHelper = evaluate(token, default (T), noChildrenBelow, true);
          noChildrenBelow = (string) null;
          string tokenToChop = token;
          while (parentsHelper && this.ChopToken(ref tokenToChop, node.Token))
            parentsHelper = evaluate(tokenToChop, default (T), noChildrenBelow, false);
        }
      }
      else
      {
        SparseTree<T>.SparseTreeNode<T> child = node.Children[index];
        if (isExactMatch)
          noChildrenBelow = this.GetNoChildrenBelowToken(token, options, child, true);
        else
          parentsHelper = this.EnumAndEvaluateParentsHelper(child, token, options, evaluate, out noChildrenBelow);
        if (parentsHelper)
        {
          parentsHelper = evaluate(child.Token, child.ReferencedObject, noChildrenBelow, isExactMatch);
          noChildrenBelow = (string) null;
        }
        if ((EnumParentsOptions.IncludeSparseNodes & options) != EnumParentsOptions.None)
        {
          string token1 = child.Token;
          while (parentsHelper && this.ChopToken(ref token1, node.Token))
            parentsHelper = evaluate(token1, default (T), noChildrenBelow, false);
        }
      }
      return parentsHelper;
    }

    private bool ChopToken(ref string tokenToChop, string stopAtToken)
    {
      bool flag = false;
      if (this.TokenSeparator != char.MinValue)
      {
        int length = tokenToChop.LastIndexOf(this.TokenSeparator);
        if (length >= 0 && (stopAtToken == null || length > stopAtToken.Length))
        {
          tokenToChop = tokenToChop.Substring(0, length);
          flag = true;
        }
      }
      else if (stopAtToken.Length + this.FixedElementLength < tokenToChop.Length)
      {
        tokenToChop = tokenToChop.Substring(0, tokenToChop.Length - this.FixedElementLength);
        flag = true;
      }
      return flag;
    }

    public IEnumerable<EnumeratedSparseTreeNode<T>> EnumParents(
      string token,
      EnumParentsOptions options)
    {
      if ((EnumParentsOptions.IncludeSparseNodes & options) != EnumParentsOptions.None)
        throw new NotSupportedException();
      if (token != null || !this.m_rootNode.HasValue)
        token = this.CanonicalizeToken(token);
      string noChildrenBelow = (string) null;
      List<EnumeratedSparseTreeNode<T>> enumeratedSparseTreeNodeList = new List<EnumeratedSparseTreeNode<T>>();
      if (this.m_rootNode != null && this.m_rootNode.HasValue)
        enumeratedSparseTreeNodeList.Add(new EnumeratedSparseTreeNode<T>(this.m_rootNode.Token, this.m_rootNode.ReferencedObject, this.m_rootNode.ChildCount > 0, (string) null, token == null));
      if (token != null)
      {
        bool isExactMatch = true;
        SparseTree<T>.SparseTreeNode<T> sparseTreeNode = this.m_rootNode;
        do
        {
          int index = this.Seek(sparseTreeNode, token, token.Length, out isExactMatch, out bool _);
          if (index < 0)
          {
            noChildrenBelow = this.GetNoChildrenBelowToken(token, options, sparseTreeNode, false);
            goto label_12;
          }
          else
          {
            sparseTreeNode = sparseTreeNode.Children[index];
            enumeratedSparseTreeNodeList.Add(new EnumeratedSparseTreeNode<T>(sparseTreeNode.Token, sparseTreeNode.ReferencedObject, sparseTreeNode.ChildCount > 0, (string) null, isExactMatch));
          }
        }
        while (!isExactMatch);
        noChildrenBelow = this.GetNoChildrenBelowToken(token, options, sparseTreeNode, true);
      }
label_12:
      enumeratedSparseTreeNodeList.Reverse();
      if (enumeratedSparseTreeNodeList.Count > 0)
      {
        EnumeratedSparseTreeNode<T> enumeratedSparseTreeNode = enumeratedSparseTreeNodeList[0];
        enumeratedSparseTreeNodeList[0] = new EnumeratedSparseTreeNode<T>(enumeratedSparseTreeNode.Token, enumeratedSparseTreeNode.ReferencedObject, enumeratedSparseTreeNode.HasChildren, noChildrenBelow, enumeratedSparseTreeNode.IsExactMatch);
      }
      return (IEnumerable<EnumeratedSparseTreeNode<T>>) enumeratedSparseTreeNodeList;
    }

    private string GetNoChildrenBelowToken(
      string token,
      EnumParentsOptions options,
      SparseTree<T>.SparseTreeNode<T> currentNode,
      bool isExactMatch)
    {
      if (isExactMatch)
      {
        if ((EnumParentsOptions.IncludeAdditionalData & options) != EnumParentsOptions.None && currentNode.ChildCount == 0)
          return currentNode.Token;
      }
      else if ((EnumParentsOptions.IncludeAdditionalData & options) != EnumParentsOptions.None)
      {
        if (currentNode.ChildCount <= 0)
          return currentNode.Token;
        int num1 = 0;
        int endIndex = int.MaxValue;
        int num2 = currentNode.Token != null ? currentNode.Token.Length : -1;
        int length = currentNode.Token != null ? currentNode.Token.Length : 0;
        while ((num2 = this.GetNextTokenLength(token, num2)) >= 0)
        {
          num1 = ~this.Seek(currentNode, token, num2, num1, endIndex);
          if (num1 != currentNode.Children.Count && this.IsSubItem(currentNode.Children[num1].Token, token, length, num2))
            endIndex = this.EndRangeSeek(currentNode, token, num2, num1, endIndex);
          else
            break;
        }
        if (num2 >= 0)
          return token.Substring(0, num2);
      }
      return (string) null;
    }

    private int GetNextTokenLength(string token, int length)
    {
      if (length >= token.Length)
        return -1;
      if (this.TokenSeparator != char.MinValue)
      {
        ++length;
        int nextTokenLength = token.IndexOf(this.TokenSeparator, length);
        if (nextTokenLength < 0)
          nextTokenLength = token.Length;
        return nextTokenLength;
      }
      return length < 0 ? 0 : length + this.FixedElementLength;
    }

    public IEnumerable<T> EnumParentsReferencedObjects(string token, EnumParentsOptions options)
    {
      foreach (EnumeratedSparseTreeNode<T> enumParent in this.EnumParents(token, options))
        yield return enumParent.ReferencedObject;
    }

    public IEnumerable<EnumeratedSparseTreeNode<T>> EnumSubTree(
      string token,
      EnumSubTreeOptions options,
      int depth)
    {
      Stack<SparseTree<T>.SparseTreeNode<T>> nodeStack = new Stack<SparseTree<T>.SparseTreeNode<T>>();
      SparseTree<T>.SparseTreeNode<T> node = this.m_rootNode;
      int startIndex = 0;
      int endIndex = node.ChildCount - 1;
      if (token != null)
      {
        token = this.CanonicalizeToken(token);
        if (this.FindClosestNode(token, out node, out startIndex))
        {
          if ((EnumSubTreeOptions.EnumerateSubTreeRoot & options) != EnumSubTreeOptions.None)
            yield return node.Enumerate(true);
          startIndex = 0;
          endIndex = node.ChildCount - 1;
        }
        else
          endIndex = this.EndRangeSeek(node, token, token.Length);
      }
      else if ((EnumSubTreeOptions.EnumerateSubTreeRoot & options) != EnumSubTreeOptions.None && this.m_rootNode.HasValue)
        yield return this.m_rootNode.Enumerate(true);
      if (depth > 0 && node.ChildCount > 0)
      {
        for (int index = endIndex; index >= startIndex; --index)
          nodeStack.Push(node.Children[index]);
      }
      node = (SparseTree<T>.SparseTreeNode<T>) null;
      if (token == null && depth > 0 && depth < int.MaxValue)
        --depth;
      while (nodeStack.Count > 0)
      {
        node = nodeStack.Pop();
        if (depth >= int.MaxValue || depth >= this.GetDepth(node.Token, token != null ? token.Length : 0))
        {
          yield return node.Enumerate(false);
          if (node.ChildCount > 0)
          {
            for (int index = node.Children.Count - 1; index >= 0; --index)
              nodeStack.Push(node.Children[index]);
          }
          node = (SparseTree<T>.SparseTreeNode<T>) null;
        }
      }
    }

    private int GetDepth(string token, int startIndex)
    {
      if (this.TokenSeparator == char.MinValue)
        return (token.Length - startIndex) / this.FixedElementLength;
      int depth = 0;
      for (; (startIndex = token.IndexOf(this.TokenSeparator, startIndex)) >= 0; ++startIndex)
        ++depth;
      return depth;
    }

    public IEnumerable<T> EnumSubTreeReferencedObjects(
      string token,
      EnumSubTreeOptions options,
      int depth)
    {
      foreach (EnumeratedSparseTreeNode<T> enumeratedSparseTreeNode in this.EnumSubTree(token, options, depth))
        yield return enumeratedSparseTreeNode.ReferencedObject;
    }

    private static int CountNode(SparseTree<T>.SparseTreeNode<T> nodeToCount)
    {
      if (nodeToCount.ChildCount == 0)
        return 1;
      int num = 0;
      Stack<SparseTree<T>.SparseTreeNode<T>> sparseTreeNodeStack = new Stack<SparseTree<T>.SparseTreeNode<T>>();
      sparseTreeNodeStack.Push(nodeToCount);
      while (sparseTreeNodeStack.Count > 0)
      {
        SparseTree<T>.SparseTreeNode<T> sparseTreeNode = sparseTreeNodeStack.Pop();
        if (sparseTreeNode.ChildCount > 0)
        {
          foreach (SparseTree<T>.SparseTreeNode<T> child in sparseTreeNode.Children)
            sparseTreeNodeStack.Push(child);
        }
        ++num;
      }
      return num;
    }

    private string CanonicalizeToken(string token)
    {
      if (this.TokenSeparator != char.MinValue)
      {
        int canonicalTokenLength = this.GetCanonicalTokenLength(token);
        if (canonicalTokenLength < token.Length)
          return token.Substring(0, canonicalTokenLength);
      }
      else if (token.Length % this.FixedElementLength != 0)
        throw new ArgumentException(nameof (token));
      return token;
    }

    private int GetCanonicalTokenLength(string token)
    {
      int length = token.Length;
      while (length > 0 && (int) token[length - 1] == (int) this.TokenSeparator)
        --length;
      return length;
    }

    private bool IsSubItem(string item, string parent, int startIndex) => this.IsSubItem(item, parent, startIndex, parent.Length);

    private bool IsSubItem(string item, string parent, int startIndex, int parentLength) => parent == null || string.Compare(item, startIndex, parent, startIndex, parentLength - startIndex, this.TokenComparison) == 0 && (this.TokenSeparator == char.MinValue || item.Length > parentLength && (int) item[parentLength] == (int) this.TokenSeparator);

    private bool FindClosestNode(string token, out SparseTree<T>.SparseTreeNode<T> closestNode) => this.FindClosestNode(token, out closestNode, out int _);

    private bool FindClosestNode(
      string token,
      out SparseTree<T>.SparseTreeNode<T> closestNode,
      out int insertIndex)
    {
      return this.FindClosestNode(token, out closestNode, out insertIndex, out bool _);
    }

    private bool FindClosestNode(
      string token,
      out SparseTree<T>.SparseTreeNode<T> closestNode,
      out int insertIndex,
      out bool indexIsChild,
      bool hintEndRange = false,
      bool copyPath = false)
    {
      return this.FindClosestNode(token, out closestNode, out SparseTree<T>.SparseTreeNode<T> _, out insertIndex, out int _, out indexIsChild, hintEndRange, copyPath);
    }

    private bool FindClosestNode(
      string token,
      out SparseTree<T>.SparseTreeNode<T> closestNode,
      out SparseTree<T>.SparseTreeNode<T> immediateParent,
      out int insertIndex,
      out int indexInParent,
      out bool indexIsChild,
      bool hintEndRange = false,
      bool copyPath = false)
    {
      indexIsChild = false;
      indexInParent = -1;
      immediateParent = (SparseTree<T>.SparseTreeNode<T>) null;
      SparseTree<T>.SparseTreeNode<T> node = this.m_rootNode;
      int index;
      while (true)
      {
        bool isExactMatch;
        index = this.Seek(node, token, token.Length, out isExactMatch, out indexIsChild, hintEndRange: hintEndRange);
        if (!isExactMatch)
        {
          if (index >= 0)
          {
            immediateParent = node;
            node = node.Children[index];
            if (copyPath && node.Sealed)
            {
              node = node.LazyCopy();
              immediateParent.Children[index] = node;
            }
            indexInParent = index;
            indexIsChild = false;
          }
          else
            break;
        }
        else
          goto label_7;
      }
      closestNode = node;
      insertIndex = ~index;
      return false;
label_7:
      immediateParent = node;
      closestNode = node.Children[index];
      if (copyPath && closestNode.Sealed)
      {
        closestNode = closestNode.LazyCopy();
        immediateParent.Children[index] = closestNode;
      }
      insertIndex = -1;
      indexInParent = index;
      indexIsChild = false;
      return true;
    }

    private void CheckSealStatus(bool expectedStatus)
    {
      if (this.m_sealed != expectedStatus)
        throw new InvalidOperationException(expectedStatus ? "Source SparseTree needs to be sealed before doing a lazy copy." : "It is illegal to change the source tree directly once the tree is sealed.");
    }

    private int Seek(
      SparseTree<T>.SparseTreeNode<T> node,
      string token,
      int tokenLength,
      int startIndex = 0,
      int endIndex = 2147483647)
    {
      return this.Seek(node, token, tokenLength, out bool _, out bool _, startIndex, endIndex);
    }

    private int Seek(
      SparseTree<T>.SparseTreeNode<T> node,
      string token,
      int tokenLength,
      out bool isExactMatch,
      out bool indexIsChild,
      int startIndex = 0,
      int endIndex = 2147483647,
      bool hintEndRange = false)
    {
      return this.TokenSeparator != char.MinValue ? this.TokenSeparatorSeek(node, token, tokenLength, out isExactMatch, out indexIsChild, startIndex, endIndex, hintEndRange) : this.FixedLengthSeek(node, token, tokenLength, out isExactMatch, out indexIsChild, startIndex, endIndex, hintEndRange);
    }

    private int EndRangeSeek(
      SparseTree<T>.SparseTreeNode<T> node,
      string token,
      int tokenLength,
      int startIndex = 0,
      int endIndex = 2147483647)
    {
      return this.TokenSeparator != char.MinValue ? this.TokenSeparatorEndRangeSeek(node, token, tokenLength, startIndex, endIndex) : this.FixedLengthEndRangeSeek(node, token, tokenLength, startIndex, endIndex);
    }

    private int TokenSeparatorSeek(
      SparseTree<T>.SparseTreeNode<T> node,
      string token,
      int tokenLength,
      out bool isExactMatch,
      out bool indexIsChild,
      int startIndex = 0,
      int endIndex = 2147483647,
      bool hintEndRange = false)
    {
      int num1 = startIndex;
      int num2 = node.ChildCount - 1;
      string token1 = node.Token;
      int length = token1 != null ? token1.Length : 0;
      int token2Count = tokenLength - length;
      if (node != this.m_rootNode)
      {
        ++length;
        --token2Count;
      }
      indexIsChild = false;
      if (endIndex < num2)
        num2 = endIndex;
      while (num1 <= num2)
      {
        int index;
        if (hintEndRange)
        {
          index = num2;
          hintEndRange = false;
        }
        else
          index = (num2 - num1) / 2 + num1;
        SparseTree<T>.SparseTreeNode<T> child = node.Children[index];
        bool outOfParts;
        int num3 = this.TokenSeparatorCompare(child, node.TokenIndices.Length, child.TokenIndices.Length - node.TokenIndices.Length, token, length, token2Count, out outOfParts);
        if (num3 < 0)
        {
          if (outOfParts)
          {
            isExactMatch = false;
            return index;
          }
          num1 = index + 1;
        }
        else if (num3 > 0)
        {
          indexIsChild = outOfParts;
          num2 = index - 1;
        }
        else
        {
          isExactMatch = true;
          return index;
        }
      }
      isExactMatch = false;
      return ~num1;
    }

    private int TokenSeparatorCompare(
      SparseTree<T>.SparseTreeNode<T> node,
      int elementIndex,
      int elementCount,
      string token2,
      int token2Index,
      int token2Count,
      out bool outOfParts)
    {
      outOfParts = false;
      int num1 = elementIndex + elementCount;
      int num2;
      int num3;
      for (num2 = token2Index + token2Count; elementIndex < num1 && token2Index <= num2; token2Index = num3 + 1)
      {
        int num4 = elementIndex >= node.TokenIndices.Length - 1 ? node.Token.Length : node.TokenIndices[elementIndex + 1] - 1;
        num3 = token2.IndexOf(this.TokenSeparator, token2Index, num2 - token2Index);
        if (num3 < 0)
          num3 = num2;
        int num5 = num4 - node.TokenIndices[elementIndex];
        int num6 = num3 - token2Index;
        int num7 = string.Compare(node.Token, node.TokenIndices[elementIndex], token2, token2Index, num5 < num6 ? num5 : num6, this.TokenComparison);
        if (num7 != 0)
          return num7;
        if (num5 != num6)
          return num5 - num6;
        ++elementIndex;
      }
      if (elementIndex < num1)
      {
        outOfParts = true;
        return 1;
      }
      if (token2Index > num2)
        return 0;
      outOfParts = true;
      return -1;
    }

    private int TokenSeparatorEndRangeSeek(
      SparseTree<T>.SparseTreeNode<T> node,
      string token,
      int tokenLength,
      int startIndex = 0,
      int endIndex = 2147483647)
    {
      int num1 = startIndex;
      int num2 = node.ChildCount - 1;
      string token1 = node.Token;
      int length = token1 != null ? token1.Length : 0;
      int token2Count = tokenLength - length;
      if (node != this.m_rootNode)
      {
        ++length;
        --token2Count;
      }
      if (endIndex < num2)
        num2 = endIndex;
      while (num1 <= num2)
      {
        int index = (num2 - num1) / 2 + num1;
        SparseTree<T>.SparseTreeNode<T> child = node.Children[index];
        int num3 = this.TokenSeparatorEndRangeCompare(child, node.TokenIndices.Length, child.TokenIndices.Length - node.TokenIndices.Length, token, length, token2Count);
        if (num3 < 0)
        {
          num1 = index + 1;
        }
        else
        {
          if (num3 <= 0)
            throw new InvalidOperationException();
          num2 = index - 1;
        }
      }
      return num1 - 1;
    }

    private int TokenSeparatorEndRangeCompare(
      SparseTree<T>.SparseTreeNode<T> node,
      int elementIndex,
      int elementCount,
      string token2,
      int token2Index,
      int token2Count)
    {
      bool outOfParts;
      int num = this.TokenSeparatorCompare(node, elementIndex, elementCount, token2, token2Index, token2Count, out outOfParts);
      if (outOfParts || num == 0)
        num = -1;
      return num;
    }

    private int FixedLengthSeek(
      SparseTree<T>.SparseTreeNode<T> node,
      string token,
      int tokenLength,
      out bool isExactMatch,
      out bool indexIsChild,
      int startIndex = 0,
      int endIndex = 2147483647,
      bool hintEndRange = false)
    {
      int num1 = startIndex;
      int num2 = node.ChildCount - 1;
      string token1 = node.Token;
      int length = token1 != null ? token1.Length : 0;
      int token2Count = tokenLength - length;
      indexIsChild = false;
      if (endIndex < num2)
        num2 = endIndex;
      while (num1 <= num2)
      {
        int index;
        if (hintEndRange)
        {
          index = num2;
          hintEndRange = false;
        }
        else
          index = (num2 - num1) / 2 + num1;
        string token2 = node.Children[index].Token;
        bool outOfParts;
        int num3 = this.FixedLengthCompare(token2, length, token2.Length - length, token, length, token2Count, out outOfParts);
        if (num3 < 0)
        {
          if (outOfParts)
          {
            isExactMatch = false;
            return index;
          }
          num1 = index + 1;
        }
        else if (num3 > 0)
        {
          indexIsChild = outOfParts;
          num2 = index - 1;
        }
        else
        {
          isExactMatch = true;
          return index;
        }
      }
      isExactMatch = false;
      return ~num1;
    }

    private int FixedLengthCompare(
      string token1,
      int token1Index,
      int token1Count,
      string token2,
      int token2Index,
      int token2Count,
      out bool outOfParts)
    {
      outOfParts = false;
      int num = string.Compare(token1, token1Index, token2, token2Index, token1Count < token2Count ? token1Count : token2Count, this.TokenComparison);
      if (num == 0)
      {
        num = token1Count - token2Count;
        if (num != 0)
          outOfParts = true;
      }
      return num;
    }

    private int FixedLengthEndRangeSeek(
      SparseTree<T>.SparseTreeNode<T> node,
      string token,
      int tokenLength,
      int startIndex = 0,
      int endIndex = 2147483647)
    {
      int num1 = startIndex;
      int num2 = node.ChildCount - 1;
      string token1 = node.Token;
      int length = token1 != null ? token1.Length : 0;
      int token2Count = tokenLength - length;
      if (endIndex < num2)
        num2 = endIndex;
      while (num1 <= num2)
      {
        int index = (num2 - num1) / 2 + num1;
        string token2 = node.Children[index].Token;
        int num3 = this.FixedLengthEndRangeCompare(token2, length, token2.Length - length, token, length, token2Count);
        if (num3 < 0)
        {
          num1 = index + 1;
        }
        else
        {
          if (num3 <= 0)
            throw new InvalidOperationException();
          num2 = index - 1;
        }
      }
      return num1 - 1;
    }

    private int FixedLengthEndRangeCompare(
      string token1,
      int token1Index,
      int token1Count,
      string token2,
      int token2Index,
      int token2Count)
    {
      if (token1Count > token2Count)
        token1Count = token2Count;
      int num = this.FixedLengthCompare(token1, token1Index, token1Count, token2, token2Index, token2Count, out bool _);
      if (num == 0)
        num = -1;
      return num;
    }

    public int Count => this.m_count + (this.m_rootNode.HasValue ? 1 : 0);

    public void Seal() => this.m_sealed = true;

    public delegate bool EnumNodeCallback(
      string token,
      T referencedObject,
      string noChildrenBelow,
      bool isExactMatch);

    private class SparseTreeNode<X>
    {
      public readonly string Token;
      public readonly int[] TokenIndices;
      public X ReferencedObject;
      public List<SparseTree<T>.SparseTreeNode<X>> Children;
      public bool Sealed;

      public SparseTreeNode(string token, int[] tokenIndices, X referencedObject)
      {
        this.Token = token;
        this.TokenIndices = tokenIndices;
        this.ReferencedObject = referencedObject;
        this.Children = (List<SparseTree<T>.SparseTreeNode<X>>) null;
      }

      public int ChildCount => this.Children != null ? this.Children.Count : 0;

      public EnumeratedSparseTreeNode<X> Enumerate(bool isExactMatch) => new EnumeratedSparseTreeNode<X>(this.Token, this.ReferencedObject, this.ChildCount > 0, (string) null, isExactMatch);

      public SparseTree<T>.SparseTreeNode<X> Copy(bool deepCopy)
      {
        SparseTree<T>.SparseTreeNode<X> target1 = SparseTree<T>.SparseTreeNode<X>.CopyNode(this, deepCopy);
        Queue<SparseTree<T>.SparseTreeNode<X>.CopyFrame> copyFrameQueue = new Queue<SparseTree<T>.SparseTreeNode<X>.CopyFrame>();
        if (this.Children != null)
          copyFrameQueue.Enqueue(new SparseTree<T>.SparseTreeNode<X>.CopyFrame(this, target1));
        while (copyFrameQueue.Count > 0)
        {
          SparseTree<T>.SparseTreeNode<X>.CopyFrame copyFrame = copyFrameQueue.Dequeue();
          copyFrame.Target.Children = new List<SparseTree<T>.SparseTreeNode<X>>(copyFrame.Source.Children.Count);
          foreach (SparseTree<T>.SparseTreeNode<X> child in copyFrame.Source.Children)
          {
            SparseTree<T>.SparseTreeNode<X> target2 = SparseTree<T>.SparseTreeNode<X>.CopyNode(child, deepCopy);
            copyFrame.Target.Children.Add(target2);
            if (child.Children != null)
              copyFrameQueue.Enqueue(new SparseTree<T>.SparseTreeNode<X>.CopyFrame(child, target2));
          }
        }
        return target1;
      }

      public SparseTree<T>.SparseTreeNode<X> LazyCopy()
      {
        SparseTree<T>.SparseTreeNode<X> sparseTreeNode = SparseTree<T>.SparseTreeNode<X>.CopyNode(this, false);
        if (this.ChildCount > 0)
        {
          foreach (SparseTree<T>.SparseTreeNode<X> child in this.Children)
            child.Sealed = true;
          sparseTreeNode.Children = new List<SparseTree<T>.SparseTreeNode<X>>((IEnumerable<SparseTree<T>.SparseTreeNode<X>>) this.Children);
        }
        return sparseTreeNode;
      }

      internal bool HasValue
      {
        get
        {
          if (!typeof (X).IsValueType)
            return (object) this.ReferencedObject != null;
          ref X local1 = ref this.ReferencedObject;
          if ((object) default (X) == null)
          {
            X x = local1;
            local1 = ref x;
          }
          __Boxed<T> local2 = (object) default (T);
          return !local1.Equals((object) local2);
        }
      }

      private static SparseTree<T>.SparseTreeNode<X> CopyNode(
        SparseTree<T>.SparseTreeNode<X> source,
        bool deepCopy)
      {
        X referencedObject = source.ReferencedObject;
        if (deepCopy && referencedObject is ICloneable cloneable)
          referencedObject = (X) cloneable.Clone();
        return new SparseTree<T>.SparseTreeNode<X>(source.Token, source.TokenIndices, referencedObject);
      }

      private struct CopyFrame
      {
        public readonly SparseTree<T>.SparseTreeNode<X> Source;
        public readonly SparseTree<T>.SparseTreeNode<X> Target;

        public CopyFrame(
          SparseTree<T>.SparseTreeNode<X> source,
          SparseTree<T>.SparseTreeNode<X> target)
        {
          this.Source = source;
          this.Target = target;
        }
      }
    }
  }
}
