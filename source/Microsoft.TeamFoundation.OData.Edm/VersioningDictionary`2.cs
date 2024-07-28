// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.VersioningDictionary`2
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  internal abstract class VersioningDictionary<TKey, TValue>
  {
    protected readonly Func<TKey, TKey, int> CompareFunction;

    protected VersioningDictionary(Func<TKey, TKey, int> compareFunction) => this.CompareFunction = compareFunction;

    public static VersioningDictionary<TKey, TValue> Create(Func<TKey, TKey, int> compareFunction) => (VersioningDictionary<TKey, TValue>) new VersioningDictionary<TKey, TValue>.EmptyVersioningDictionary(compareFunction);

    public abstract VersioningDictionary<TKey, TValue> Set(TKey keyToSet, TValue newValue);

    public abstract VersioningDictionary<TKey, TValue> Remove(TKey keyToRemove);

    public TValue Get(TKey key)
    {
      TValue obj;
      if (this.TryGetValue(key, out obj))
        return obj;
      throw new KeyNotFoundException(key.ToString());
    }

    public abstract bool TryGetValue(TKey key, out TValue value);

    internal sealed class EmptyVersioningDictionary : VersioningDictionary<TKey, TValue>
    {
      public EmptyVersioningDictionary(Func<TKey, TKey, int> compareFunction)
        : base(compareFunction)
      {
      }

      public override VersioningDictionary<TKey, TValue> Set(TKey keyToSet, TValue newValue) => (VersioningDictionary<TKey, TValue>) new VersioningDictionary<TKey, TValue>.OneKeyDictionary(this.CompareFunction, keyToSet, newValue);

      public override VersioningDictionary<TKey, TValue> Remove(TKey keyToRemove) => throw new KeyNotFoundException(keyToRemove.ToString());

      public override bool TryGetValue(TKey key, out TValue value)
      {
        value = default (TValue);
        return false;
      }
    }

    internal sealed class OneKeyDictionary : VersioningDictionary<TKey, TValue>
    {
      private readonly TKey key;
      private readonly TValue value;

      public OneKeyDictionary(Func<TKey, TKey, int> compareFunction, TKey key, TValue value)
        : base(compareFunction)
      {
        this.key = key;
        this.value = value;
      }

      public override VersioningDictionary<TKey, TValue> Set(TKey keyToSet, TValue newValue) => this.CompareFunction(keyToSet, this.key) == 0 ? (VersioningDictionary<TKey, TValue>) new VersioningDictionary<TKey, TValue>.OneKeyDictionary(this.CompareFunction, keyToSet, newValue) : (VersioningDictionary<TKey, TValue>) new VersioningDictionary<TKey, TValue>.TwoKeyDictionary(this.CompareFunction, this.key, this.value, keyToSet, newValue);

      public override VersioningDictionary<TKey, TValue> Remove(TKey keyToRemove)
      {
        if (this.CompareFunction(keyToRemove, this.key) == 0)
          return (VersioningDictionary<TKey, TValue>) new VersioningDictionary<TKey, TValue>.EmptyVersioningDictionary(this.CompareFunction);
        throw new KeyNotFoundException(keyToRemove.ToString());
      }

      public override bool TryGetValue(TKey key, out TValue value)
      {
        if (this.CompareFunction(key, this.key) == 0)
        {
          value = this.value;
          return true;
        }
        value = default (TValue);
        return false;
      }
    }

    internal sealed class TwoKeyDictionary : VersioningDictionary<TKey, TValue>
    {
      private readonly TKey firstKey;
      private readonly TValue firstValue;
      private readonly TKey secondKey;
      private readonly TValue secondValue;

      public TwoKeyDictionary(
        Func<TKey, TKey, int> compareFunction,
        TKey firstKey,
        TValue firstValue,
        TKey secondKey,
        TValue secondValue)
        : base(compareFunction)
      {
        this.firstKey = firstKey;
        this.firstValue = firstValue;
        this.secondKey = secondKey;
        this.secondValue = secondValue;
      }

      public override VersioningDictionary<TKey, TValue> Set(TKey keyToSet, TValue newValue)
      {
        if (this.CompareFunction(keyToSet, this.firstKey) == 0)
          return (VersioningDictionary<TKey, TValue>) new VersioningDictionary<TKey, TValue>.TwoKeyDictionary(this.CompareFunction, keyToSet, newValue, this.secondKey, this.secondValue);
        return this.CompareFunction(keyToSet, this.secondKey) == 0 ? (VersioningDictionary<TKey, TValue>) new VersioningDictionary<TKey, TValue>.TwoKeyDictionary(this.CompareFunction, this.firstKey, this.firstValue, keyToSet, newValue) : (VersioningDictionary<TKey, TValue>) new VersioningDictionary<TKey, TValue>.TreeDictionary(this.CompareFunction, this.firstKey, this.firstValue, this.secondKey, this.secondValue, keyToSet, newValue);
      }

      public override VersioningDictionary<TKey, TValue> Remove(TKey keyToRemove)
      {
        if (this.CompareFunction(keyToRemove, this.firstKey) == 0)
          return (VersioningDictionary<TKey, TValue>) new VersioningDictionary<TKey, TValue>.OneKeyDictionary(this.CompareFunction, this.secondKey, this.secondValue);
        if (this.CompareFunction(keyToRemove, this.secondKey) == 0)
          return (VersioningDictionary<TKey, TValue>) new VersioningDictionary<TKey, TValue>.OneKeyDictionary(this.CompareFunction, this.firstKey, this.firstValue);
        throw new KeyNotFoundException(keyToRemove.ToString());
      }

      public override bool TryGetValue(TKey key, out TValue value)
      {
        if (this.CompareFunction(key, this.firstKey) == 0)
        {
          value = this.firstValue;
          return true;
        }
        if (this.CompareFunction(key, this.secondKey) == 0)
        {
          value = this.secondValue;
          return true;
        }
        value = default (TValue);
        return false;
      }
    }

    internal sealed class TreeDictionary : VersioningDictionary<TKey, TValue>
    {
      private const int MaxTreeHeight = 10;
      private readonly VersioningTree<TKey, TValue> tree;

      public TreeDictionary(
        Func<TKey, TKey, int> compareFunction,
        TKey firstKey,
        TValue firstValue,
        TKey secondKey,
        TValue secondValue,
        TKey thirdKey,
        TValue thirdValue)
        : base(compareFunction)
      {
        this.tree = new VersioningTree<TKey, TValue>(firstKey, firstValue, (VersioningTree<TKey, TValue>) null, (VersioningTree<TKey, TValue>) null).SetKeyValue(secondKey, secondValue, this.CompareFunction).SetKeyValue(thirdKey, thirdValue, this.CompareFunction);
      }

      public TreeDictionary(
        Func<TKey, TKey, int> compareFunction,
        VersioningTree<TKey, TValue> tree)
        : base(compareFunction)
      {
        this.tree = tree;
      }

      public override VersioningDictionary<TKey, TValue> Set(TKey keyToSet, TValue newValue) => this.tree.Height > 10 ? (VersioningDictionary<TKey, TValue>) new VersioningDictionary<TKey, TValue>.HashTreeDictionary(this.CompareFunction, this.tree, keyToSet, newValue) : (VersioningDictionary<TKey, TValue>) new VersioningDictionary<TKey, TValue>.TreeDictionary(this.CompareFunction, this.tree.SetKeyValue(keyToSet, newValue, this.CompareFunction));

      public override VersioningDictionary<TKey, TValue> Remove(TKey keyToRemove) => (VersioningDictionary<TKey, TValue>) new VersioningDictionary<TKey, TValue>.TreeDictionary(this.CompareFunction, this.tree.Remove(keyToRemove, this.CompareFunction));

      public override bool TryGetValue(TKey key, out TValue value)
      {
        if (this.tree != null)
          return this.tree.TryGetValue(key, this.CompareFunction, out value);
        value = default (TValue);
        return false;
      }
    }

    internal sealed class HashTreeDictionary : VersioningDictionary<TKey, TValue>
    {
      private const int HashSize = 17;
      private readonly VersioningTree<TKey, TValue>[] treeBuckets;

      public HashTreeDictionary(
        Func<TKey, TKey, int> compareFunction,
        VersioningTree<TKey, TValue> tree,
        TKey key,
        TValue value)
        : base(compareFunction)
      {
        this.treeBuckets = new VersioningTree<TKey, TValue>[17];
        this.SetKeyValues(tree);
        this.SetKeyValue(key, value);
      }

      public HashTreeDictionary(
        Func<TKey, TKey, int> compareFunction,
        VersioningTree<TKey, TValue>[] trees,
        TKey key,
        TValue value)
        : base(compareFunction)
      {
        this.treeBuckets = (VersioningTree<TKey, TValue>[]) trees.Clone();
        this.SetKeyValue(key, value);
      }

      public HashTreeDictionary(
        Func<TKey, TKey, int> compareFunction,
        VersioningTree<TKey, TValue>[] trees,
        TKey key)
        : base(compareFunction)
      {
        this.treeBuckets = (VersioningTree<TKey, TValue>[]) trees.Clone();
        this.RemoveKey(key);
      }

      public override VersioningDictionary<TKey, TValue> Set(TKey keyToSet, TValue newValue) => (VersioningDictionary<TKey, TValue>) new VersioningDictionary<TKey, TValue>.HashTreeDictionary(this.CompareFunction, this.treeBuckets, keyToSet, newValue);

      public override VersioningDictionary<TKey, TValue> Remove(TKey keyToRemove) => (VersioningDictionary<TKey, TValue>) new VersioningDictionary<TKey, TValue>.HashTreeDictionary(this.CompareFunction, this.treeBuckets, keyToRemove);

      public override bool TryGetValue(TKey key, out TValue value)
      {
        VersioningTree<TKey, TValue> treeBucket = this.treeBuckets[VersioningDictionary<TKey, TValue>.HashTreeDictionary.GetBucket(key)];
        if (treeBucket != null)
          return treeBucket.TryGetValue(key, this.CompareFunction, out value);
        value = default (TValue);
        return false;
      }

      private void SetKeyValue(TKey keyToSet, TValue newValue)
      {
        int bucket = VersioningDictionary<TKey, TValue>.HashTreeDictionary.GetBucket(keyToSet);
        if (this.treeBuckets[bucket] == null)
          this.treeBuckets[bucket] = new VersioningTree<TKey, TValue>(keyToSet, newValue, (VersioningTree<TKey, TValue>) null, (VersioningTree<TKey, TValue>) null);
        else
          this.treeBuckets[bucket] = this.treeBuckets[bucket].SetKeyValue(keyToSet, newValue, this.CompareFunction);
      }

      private void SetKeyValues(VersioningTree<TKey, TValue> tree)
      {
        if (tree == null)
          return;
        this.SetKeyValue(tree.Key, tree.Value);
        this.SetKeyValues(tree.LeftChild);
        this.SetKeyValues(tree.RightChild);
      }

      private void RemoveKey(TKey keyToRemove)
      {
        int bucket = VersioningDictionary<TKey, TValue>.HashTreeDictionary.GetBucket(keyToRemove);
        if (this.treeBuckets[bucket] == null)
          throw new KeyNotFoundException(keyToRemove.ToString());
        this.treeBuckets[bucket] = this.treeBuckets[bucket].Remove(keyToRemove, this.CompareFunction);
      }

      private static int GetBucket(TKey key)
      {
        int num = key.GetHashCode();
        if (num < 0)
          num = -num;
        return num % 17;
      }
    }
  }
}
