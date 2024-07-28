// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.ScaleoutStore
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Threading;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public sealed class ScaleoutStore
  {
    private const uint _minFragmentCount = 4;
    private static readonly uint _maxFragmentSize = IntPtr.Size == 4 ? 16384U : 8192U;
    private static readonly ArraySegment<ScaleoutMapping> _emptyArraySegment = new ArraySegment<ScaleoutMapping>(new ScaleoutMapping[0]);
    private ScaleoutStore.Fragment[] _fragments;
    private readonly uint _fragmentSize;
    private long _minMessageId;
    private long _nextFreeMessageId;
    private ulong _minMappingId;
    private ScaleoutMapping _maxMapping;

    public ScaleoutStore(uint capacity)
    {
      if (capacity < 32U)
        capacity = 32U;
      uint num = Math.Max(4U, capacity / ScaleoutStore._maxFragmentSize);
      this._fragmentSize = Math.Min(checked (capacity + num - 1U) / num, ScaleoutStore._maxFragmentSize);
      this._fragments = new ScaleoutStore.Fragment[(int) checked (num + 1U)];
    }

    internal ScaleoutStore(uint capacity, uint fragmentSize)
    {
      uint num = capacity / fragmentSize;
      this._fragmentSize = fragmentSize;
      this._fragments = new ScaleoutStore.Fragment[(int) checked (num + 1U)];
    }

    internal ulong MinMappingId => this._minMappingId;

    public ScaleoutMapping MaxMapping => this._maxMapping;

    public uint FragmentSize => this._fragmentSize;

    public int FragmentCount => this._fragments.Length;

    public ulong Add(ScaleoutMapping mapping)
    {
      ulong newMessageId;
      do
        ;
      while (!this.TryAddImpl(mapping, out newMessageId));
      Interlocked.Increment(ref this._nextFreeMessageId);
      return newMessageId;
    }

    private void GetFragmentOffsets(
      ulong messageId,
      out ulong fragmentNum,
      out int idxIntoFragmentsArray,
      out int idxIntoFragment)
    {
      fragmentNum = messageId / (ulong) this._fragmentSize;
      idxIntoFragmentsArray = (int) (fragmentNum % (ulong) (uint) this._fragments.Length);
      idxIntoFragment = (int) (messageId % (ulong) this._fragmentSize);
    }

    private int GetFragmentOffset(ulong messageId) => (int) (messageId / (ulong) this._fragmentSize % (ulong) (uint) this._fragments.Length);

    private ulong GetMessageId(ulong fragmentNum, uint offset) => fragmentNum * (ulong) this._fragmentSize + (ulong) offset;

    private bool TryAddImpl(ScaleoutMapping mapping, out ulong newMessageId)
    {
      ulong fragmentNum;
      int idxIntoFragmentsArray;
      int idxIntoFragment;
      this.GetFragmentOffsets((ulong) Volatile.Read(ref this._nextFreeMessageId), out fragmentNum, out idxIntoFragmentsArray, out idxIntoFragment);
      ScaleoutStore.Fragment fragment1 = this._fragments[idxIntoFragmentsArray];
      if (fragment1 == null || fragment1.FragmentNum < fragmentNum)
      {
        bool flag = fragment1 != null && fragment1.FragmentNum < fragmentNum;
        if (idxIntoFragment == 0)
        {
          ScaleoutStore.Fragment fragment2 = new ScaleoutStore.Fragment(fragmentNum, this._fragmentSize);
          fragment2.Data[0] = mapping;
          ScaleoutStore.Fragment fragment3 = Interlocked.CompareExchange<ScaleoutStore.Fragment>(ref this._fragments[idxIntoFragmentsArray], fragment2, fragment1);
          if (fragment3 == fragment1)
          {
            newMessageId = this.GetMessageId(fragmentNum, 0U);
            fragment2.MinId = newMessageId;
            fragment2.Length = 1;
            fragment2.MaxId = this.GetMessageId(fragmentNum, this._fragmentSize - 1U);
            this._maxMapping = mapping;
            if (flag)
            {
              this._minMessageId = (long) fragment3.MaxId + 1L;
              this._minMappingId = fragment3.MaxValue.Value;
            }
            else if (idxIntoFragmentsArray == 0)
              this._minMappingId = mapping.Id;
            return true;
          }
        }
      }
      else if ((long) fragment1.FragmentNum == (long) fragmentNum)
      {
        ScaleoutMapping[] data = fragment1.Data;
        for (int offset = idxIntoFragment; offset < data.Length; ++offset)
        {
          if (Interlocked.CompareExchange<ScaleoutMapping>(ref data[offset], mapping, (ScaleoutMapping) null) == null)
          {
            newMessageId = this.GetMessageId(fragmentNum, (uint) offset);
            ++fragment1.Length;
            this._maxMapping = data[offset];
            return true;
          }
        }
      }
      newMessageId = 0UL;
      return false;
    }

    public MessageStoreResult<ScaleoutMapping> GetMessages(ulong firstMessageIdRequestedByClient)
    {
      ulong num = (ulong) Volatile.Read(ref this._nextFreeMessageId);
      if (num <= firstMessageIdRequestedByClient)
        return new MessageStoreResult<ScaleoutMapping>(firstMessageIdRequestedByClient, ScaleoutStore._emptyArraySegment, false);
      ulong fragmentNum;
      int idxIntoFragmentsArray;
      int idxIntoFragment;
      this.GetFragmentOffsets(firstMessageIdRequestedByClient, out fragmentNum, out idxIntoFragmentsArray, out idxIntoFragment);
      ScaleoutStore.Fragment fragment1 = this._fragments[idxIntoFragmentsArray];
      long messageId = (long) this.GetMessageId(fragment1.FragmentNum, 0U);
      ulong val2 = (ulong) messageId + (ulong) this._fragmentSize;
      if ((ulong) messageId <= firstMessageIdRequestedByClient && firstMessageIdRequestedByClient < val2)
      {
        int count = (int) ((long) Math.Min(num, val2) - (long) firstMessageIdRequestedByClient);
        ArraySegment<ScaleoutMapping> messages = new ArraySegment<ScaleoutMapping>(fragment1.Data, idxIntoFragment, count);
        return new MessageStoreResult<ScaleoutMapping>(firstMessageIdRequestedByClient, messages, num > val2);
      }
      ScaleoutStore.Fragment fragment2;
      while (true)
      {
        this.GetFragmentOffsets(num, out fragmentNum, out idxIntoFragmentsArray, out idxIntoFragment);
        fragment2 = this._fragments[(idxIntoFragmentsArray + 1) % this._fragments.Length];
        if (fragment2.FragmentNum >= fragmentNum)
          num = (ulong) Volatile.Read(ref this._nextFreeMessageId);
        else
          break;
      }
      return new MessageStoreResult<ScaleoutMapping>(this.GetMessageId(fragment2.FragmentNum, 0U), new ArraySegment<ScaleoutMapping>(fragment2.Data, 0, fragment2.Length), true);
    }

    public MessageStoreResult<ScaleoutMapping> GetMessagesByMappingId(ulong mappingId)
    {
      ulong minMessageId = (ulong) Volatile.Read(ref this._minMessageId);
      ScaleoutStore.Fragment fragment;
      if (this.TryGetFragmentFromMappingId(mappingId, out fragment))
      {
        int index;
        int lastSearchIndex;
        ulong lastSearchId;
        if (fragment.TrySearch(mappingId, out index, out lastSearchIndex, out lastSearchId))
        {
          int offset = index + 1;
          return this.GetMessages(this.GetMessageId(fragment.FragmentNum, (uint) offset));
        }
        if (mappingId > lastSearchId)
          ++lastSearchIndex;
        ArraySegment<ScaleoutMapping> messages = new ArraySegment<ScaleoutMapping>(fragment.Data, lastSearchIndex, fragment.Length - lastSearchIndex);
        return new MessageStoreResult<ScaleoutMapping>(this.GetMessageId(fragment.FragmentNum, (uint) lastSearchIndex), messages, true);
      }
      return mappingId < this._minMappingId || mappingId == ulong.MaxValue ? this.GetAllMessages(minMessageId) : new MessageStoreResult<ScaleoutMapping>(0UL, ScaleoutStore._emptyArraySegment, false);
    }

    private MessageStoreResult<ScaleoutMapping> GetAllMessages(ulong minMessageId)
    {
      int idxIntoFragmentsArray;
      this.GetFragmentOffsets(minMessageId, out ulong _, out idxIntoFragmentsArray, out int _);
      ScaleoutStore.Fragment fragment = this._fragments[idxIntoFragmentsArray];
      return fragment == null ? new MessageStoreResult<ScaleoutMapping>(minMessageId, ScaleoutStore._emptyArraySegment, false) : new MessageStoreResult<ScaleoutMapping>(this.GetMessageId(fragment.FragmentNum, 0U), new ArraySegment<ScaleoutMapping>(fragment.Data, 0, fragment.Length), true);
    }

    internal bool TryGetFragmentFromMappingId(ulong mappingId, out ScaleoutStore.Fragment fragment)
    {
      long num1 = this._minMessageId;
      long num2 = this._nextFreeMessageId;
      while (num1 <= num2)
      {
        int fragmentOffset = this.GetFragmentOffset((ulong) (num1 + num2) / 2UL);
        fragment = this._fragments[fragmentOffset];
        if (fragment == null)
          return false;
        long num3 = (long) mappingId;
        ulong? nullable = fragment.MinValue;
        long valueOrDefault1 = (long) nullable.GetValueOrDefault();
        if ((ulong) num3 < (ulong) valueOrDefault1 & nullable.HasValue)
        {
          num2 = (long) fragment.MinId - 1L;
        }
        else
        {
          long num4 = (long) mappingId;
          nullable = fragment.MaxValue;
          long valueOrDefault2 = (long) nullable.GetValueOrDefault();
          if ((ulong) num4 > (ulong) valueOrDefault2 & nullable.HasValue)
            num1 = (long) fragment.MaxId + 1L;
          else if (fragment.HasValue(mappingId))
            return true;
        }
      }
      fragment = (ScaleoutStore.Fragment) null;
      return false;
    }

    internal sealed class Fragment
    {
      public readonly ulong FragmentNum;
      public readonly ScaleoutMapping[] Data;
      public int Length;
      public ulong MinId;
      public ulong MaxId;

      public Fragment(ulong fragmentNum, uint fragmentSize)
      {
        this.FragmentNum = fragmentNum;
        this.Data = new ScaleoutMapping[(int) fragmentSize];
      }

      public ulong? MinValue => this.Data[0]?.Id;

      public ulong? MaxValue => (this.Length != 0 ? this.Data[this.Length - 1] : this.Data[this.Length])?.Id;

      public bool HasValue(ulong id)
      {
        long num1 = (long) id;
        ulong? minValue = this.MinValue;
        long valueOrDefault1 = (long) minValue.GetValueOrDefault();
        if (!((ulong) num1 >= (ulong) valueOrDefault1 & minValue.HasValue))
          return false;
        long num2 = (long) id;
        ulong? maxValue = this.MaxValue;
        long valueOrDefault2 = (long) maxValue.GetValueOrDefault();
        return (ulong) num2 <= (ulong) valueOrDefault2 & maxValue.HasValue;
      }

      public bool TrySearch(
        ulong id,
        out int index,
        out int lastSearchIndex,
        out ulong lastSearchId)
      {
        lastSearchIndex = 0;
        lastSearchId = id;
        int num1 = 0;
        int num2 = this.Length;
        while (num1 <= num2)
        {
          int index1 = (num1 + num2) / 2;
          ScaleoutMapping scaleoutMapping = this.Data[index1];
          lastSearchIndex = index1;
          lastSearchId = scaleoutMapping.Id;
          if (id < scaleoutMapping.Id)
            num2 = index1 - 1;
          else if (id > scaleoutMapping.Id)
            num1 = index1 + 1;
          else if ((long) id == (long) scaleoutMapping.Id)
          {
            index = index1;
            return true;
          }
        }
        index = -1;
        return false;
      }
    }
  }
}
