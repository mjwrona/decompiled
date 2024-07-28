// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.BaseMemoryCacheCapacityPolicy`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class BaseMemoryCacheCapacityPolicy<TKey, TValue> : IMemoryCacheCapacityPolicy<TKey, TValue>
  {
    private readonly ISizeProvider<TKey, TValue> m_sizeProvider;
    private long m_currentSize;
    private readonly Capture<long> m_maxSize;
    private int m_currentLength;
    private readonly Capture<int> m_maxLength;

    public BaseMemoryCacheCapacityPolicy(
      CaptureLength maxLength,
      CaptureSize maxSize,
      ISizeProvider<TKey, TValue> sizeProvider)
    {
      ArgumentUtility.CheckForNull<CaptureLength>(maxLength, nameof (maxLength));
      ArgumentUtility.CheckForNull<CaptureSize>(maxSize, nameof (maxSize));
      ArgumentUtility.CheckForNull<ISizeProvider<TKey, TValue>>(sizeProvider, nameof (sizeProvider));
      this.m_sizeProvider = sizeProvider;
      this.m_maxLength = (Capture<int>) maxLength;
      this.m_currentLength = 0;
      this.m_maxSize = (Capture<long>) maxSize;
      this.m_currentSize = 0L;
    }

    public int Length => this.m_currentLength;

    public long Size => this.m_currentSize;

    public bool NeedRoom(TKey key, TValue value) => this.m_currentLength >= (int) this.m_maxLength || this.m_currentSize + this.GetItemSize(key, value) > (long) this.m_maxSize;

    public bool NeedRoom(TKey key, TValue previousValue, TValue newValue) => this.m_currentSize + this.GetDeltaSize(key, previousValue, newValue) > (long) this.m_maxSize;

    public long OnEntryAdded(TKey key, TValue value)
    {
      long itemSize = this.GetItemSize(key, value);
      this.Update(1, itemSize);
      return itemSize;
    }

    public SizePair OnEntryReplaced(TKey key, TValue previousValue, TValue newValue)
    {
      this.Assert(this.m_currentLength > 0);
      long itemSize1 = this.GetItemSize(key, previousValue);
      long itemSize2 = this.GetItemSize(key, newValue);
      this.Update(0, itemSize2 - itemSize1);
      return new SizePair(itemSize1, itemSize2);
    }

    public long OnEntryRemoved(TKey key, TValue value)
    {
      long itemSize = this.GetItemSize(key, value);
      this.Update(-1, -itemSize);
      return itemSize;
    }

    public void OnCleared()
    {
      this.m_currentLength = 0;
      this.m_currentSize = 0L;
    }

    private long GetItemSize(TKey key, TValue value)
    {
      long size = this.m_sizeProvider.GetSize(key, value);
      return size >= 0L ? size : throw new ArgumentOutOfRangeException("Key/Value size is negative");
    }

    private long GetDeltaSize(TKey key, TValue previousValue, TValue newValue) => this.GetItemSize(key, newValue) - this.GetItemSize(key, previousValue);

    private void Update(int deltaLength, long deltaSize)
    {
      this.ValidatePolicyUpdate(deltaLength, deltaSize);
      this.m_currentLength += deltaLength;
      this.m_currentSize += deltaSize;
    }

    private void ValidatePolicyUpdate(int deltaLength, long deltaSize)
    {
      int num1 = this.m_currentLength + deltaLength;
      long num2 = this.m_currentSize + deltaSize;
      this.Assert(num1 >= 0 && num2 >= 0L);
      if (num1 == 0)
        this.Assert(num2 == 0L);
      if (deltaLength >= 0)
        this.Assert(num1 <= (int) this.m_maxLength);
      if (deltaSize < 0L)
        return;
      this.Assert(num2 <= (long) this.m_maxSize);
    }

    private void Assert(bool condition)
    {
      if (!condition)
        throw new InvalidOperationException(string.Format("Cache policy {0} is corrupted, currentLength={1}, currentSize={2}", (object) this.GetType().Name, (object) this.m_currentLength, (object) this.m_currentSize));
    }
  }
}
