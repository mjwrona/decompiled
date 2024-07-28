// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.VersioningTree`2
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  internal class VersioningTree<TKey, TValue>
  {
    public readonly TKey Key;
    public readonly TValue Value;
    public readonly int Height;
    public readonly VersioningTree<TKey, TValue> LeftChild;
    public readonly VersioningTree<TKey, TValue> RightChild;

    public VersioningTree(
      TKey key,
      TValue value,
      VersioningTree<TKey, TValue> leftChild,
      VersioningTree<TKey, TValue> rightChild)
    {
      this.Key = key;
      this.Value = value;
      this.Height = VersioningTree<TKey, TValue>.Max(VersioningTree<TKey, TValue>.GetHeight(leftChild), VersioningTree<TKey, TValue>.GetHeight(rightChild)) + 1;
      this.LeftChild = leftChild;
      this.RightChild = rightChild;
    }

    public TValue GetValue(TKey key, Func<TKey, TKey, int> compareFunction)
    {
      TValue obj;
      if (this.TryGetValue(key, compareFunction, out obj))
        return obj;
      throw new KeyNotFoundException(key.ToString());
    }

    public bool TryGetValue(TKey key, Func<TKey, TKey, int> compareFunction, out TValue value)
    {
      VersioningTree<TKey, TValue> versioningTree1;
      for (VersioningTree<TKey, TValue> versioningTree2 = this; versioningTree2 != null; versioningTree2 = versioningTree1)
      {
        int num = compareFunction(key, versioningTree2.Key);
        if (num == 0)
        {
          value = versioningTree2.Value;
          return true;
        }
        if (num >= 0)
        {
          versioningTree1 = versioningTree2.RightChild;
        }
        else
        {
          VersioningTree<TKey, TValue> versioningTree3 = versioningTree1 = versioningTree2.LeftChild;
        }
      }
      value = default (TValue);
      return false;
    }

    public VersioningTree<TKey, TValue> SetKeyValue(
      TKey key,
      TValue value,
      Func<TKey, TKey, int> compareFunction)
    {
      VersioningTree<TKey, TValue> leftChild1 = this.LeftChild;
      VersioningTree<TKey, TValue> rightChild1 = this.RightChild;
      int num1 = compareFunction(key, this.Key);
      if (num1 < 0)
      {
        if (VersioningTree<TKey, TValue>.GetHeight(leftChild1) <= VersioningTree<TKey, TValue>.GetHeight(rightChild1))
          return new VersioningTree<TKey, TValue>(this.Key, this.Value, VersioningTree<TKey, TValue>.SetKeyValue(leftChild1, key, value, compareFunction), rightChild1);
        int num2 = compareFunction(key, leftChild1.Key);
        VersioningTree<TKey, TValue> leftChild2 = num2 < 0 ? VersioningTree<TKey, TValue>.SetKeyValue(leftChild1.LeftChild, key, value, compareFunction) : leftChild1.LeftChild;
        VersioningTree<TKey, TValue> rightChild2 = new VersioningTree<TKey, TValue>(this.Key, this.Value, num2 > 0 ? VersioningTree<TKey, TValue>.SetKeyValue(leftChild1.RightChild, key, value, compareFunction) : leftChild1.RightChild, rightChild1);
        return new VersioningTree<TKey, TValue>(num2 == 0 ? key : leftChild1.Key, num2 == 0 ? value : leftChild1.Value, leftChild2, rightChild2);
      }
      if (num1 == 0)
        return new VersioningTree<TKey, TValue>(key, value, leftChild1, rightChild1);
      if (VersioningTree<TKey, TValue>.GetHeight(leftChild1) >= VersioningTree<TKey, TValue>.GetHeight(rightChild1))
        return new VersioningTree<TKey, TValue>(this.Key, this.Value, leftChild1, VersioningTree<TKey, TValue>.SetKeyValue(rightChild1, key, value, compareFunction));
      int num3 = compareFunction(key, rightChild1.Key);
      VersioningTree<TKey, TValue> leftChild3 = new VersioningTree<TKey, TValue>(this.Key, this.Value, leftChild1, num3 < 0 ? VersioningTree<TKey, TValue>.SetKeyValue(rightChild1.LeftChild, key, value, compareFunction) : rightChild1.LeftChild);
      VersioningTree<TKey, TValue> rightChild3 = num3 > 0 ? VersioningTree<TKey, TValue>.SetKeyValue(rightChild1.RightChild, key, value, compareFunction) : rightChild1.RightChild;
      return new VersioningTree<TKey, TValue>(num3 == 0 ? key : rightChild1.Key, num3 == 0 ? value : rightChild1.Value, leftChild3, rightChild3);
    }

    public VersioningTree<TKey, TValue> Remove(TKey key, Func<TKey, TKey, int> compareFunction)
    {
      int num = compareFunction(key, this.Key);
      if (num < 0)
      {
        if (this.LeftChild == null)
          throw new KeyNotFoundException(key.ToString());
        return new VersioningTree<TKey, TValue>(this.Key, this.Value, this.LeftChild.Remove(key, compareFunction), this.RightChild);
      }
      if (num == 0)
      {
        if (this.LeftChild == null)
          return this.RightChild;
        if (this.RightChild == null)
          return this.LeftChild;
        return this.LeftChild.Height < this.RightChild.Height ? this.LeftChild.MakeRightmost(this.RightChild) : this.RightChild.MakeLeftmost(this.LeftChild);
      }
      if (this.RightChild == null)
        throw new KeyNotFoundException(key.ToString());
      return new VersioningTree<TKey, TValue>(this.Key, this.Value, this.LeftChild, this.RightChild.Remove(key, compareFunction));
    }

    private static VersioningTree<TKey, TValue> SetKeyValue(
      VersioningTree<TKey, TValue> me,
      TKey key,
      TValue value,
      Func<TKey, TKey, int> compareFunction)
    {
      return me == null ? new VersioningTree<TKey, TValue>(key, value, (VersioningTree<TKey, TValue>) null, (VersioningTree<TKey, TValue>) null) : me.SetKeyValue(key, value, compareFunction);
    }

    private static int GetHeight(VersioningTree<TKey, TValue> tree) => tree != null ? tree.Height : 0;

    private static int Max(int x, int y) => x <= y ? y : x;

    private VersioningTree<TKey, TValue> MakeLeftmost(VersioningTree<TKey, TValue> leftmost) => this.LeftChild == null ? new VersioningTree<TKey, TValue>(this.Key, this.Value, leftmost, this.RightChild) : new VersioningTree<TKey, TValue>(this.Key, this.Value, this.LeftChild.MakeLeftmost(leftmost), this.RightChild);

    private VersioningTree<TKey, TValue> MakeRightmost(VersioningTree<TKey, TValue> rightmost) => this.RightChild == null ? new VersioningTree<TKey, TValue>(this.Key, this.Value, this.LeftChild, rightmost) : new VersioningTree<TKey, TValue>(this.Key, this.Value, this.LeftChild, this.RightChild.MakeRightmost(rightmost));
  }
}
