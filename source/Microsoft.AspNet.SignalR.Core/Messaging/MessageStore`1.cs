// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.MessageStore`1
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Threading;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public sealed class MessageStore<T> where T : class
  {
    private static readonly uint _minFragmentCount = 4;
    private static readonly uint _maxFragmentSize = IntPtr.Size == 4 ? 16384U : 8192U;
    private static readonly ArraySegment<T> _emptyArraySegment = new ArraySegment<T>(new T[0]);
    private readonly uint _offset;
    private MessageStore<T>.Fragment[] _fragments;
    private readonly uint _fragmentSize;
    private long _nextFreeMessageId;

    public MessageStore(uint capacity, uint offset)
    {
      if (capacity < 32U)
        capacity = 32U;
      this._offset = offset;
      uint num = Math.Max(MessageStore<T>._minFragmentCount, capacity / MessageStore<T>._maxFragmentSize);
      this._fragmentSize = Math.Min(checked (capacity + num - 1U) / num, MessageStore<T>._maxFragmentSize);
      this._fragments = new MessageStore<T>.Fragment[(int) checked (num + 1U)];
    }

    public MessageStore(uint capacity)
      : this(capacity, 0U)
    {
    }

    public ulong GetMessageCount() => (ulong) Volatile.Read(ref this._nextFreeMessageId);

    public ulong Add(T message)
    {
      ulong newMessageId;
      do
        ;
      while (!this.TryAddImpl(message, out newMessageId));
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

    private ulong GetMessageId(ulong fragmentNum, uint offset) => fragmentNum * (ulong) this._fragmentSize + (ulong) offset;

    public MessageStoreResult<T> GetMessages(ulong firstMessageId, int maxMessages) => this.GetMessagesImpl(firstMessageId, maxMessages);

    private MessageStoreResult<T> GetMessagesImpl(
      ulong firstMessageIdRequestedByClient,
      int maxMessages)
    {
      ulong num = (ulong) Volatile.Read(ref this._nextFreeMessageId);
      if (num <= firstMessageIdRequestedByClient)
        return new MessageStoreResult<T>(firstMessageIdRequestedByClient, MessageStore<T>._emptyArraySegment, false);
      ulong fragmentNum;
      int idxIntoFragmentsArray;
      int idxIntoFragment;
      this.GetFragmentOffsets(firstMessageIdRequestedByClient, out fragmentNum, out idxIntoFragmentsArray, out idxIntoFragment);
      MessageStore<T>.Fragment fragment1 = this._fragments[idxIntoFragmentsArray];
      long messageId1 = (long) this.GetMessageId(fragment1.FragmentNum, this._offset);
      ulong val2 = (ulong) messageId1 + (ulong) this._fragmentSize;
      if ((ulong) messageId1 <= firstMessageIdRequestedByClient && firstMessageIdRequestedByClient < val2)
      {
        int count = Math.Min((int) ((long) Math.Min(num, val2) - (long) firstMessageIdRequestedByClient), maxMessages);
        ArraySegment<T> messages = new ArraySegment<T>(fragment1.Data, idxIntoFragment, count);
        return new MessageStoreResult<T>(firstMessageIdRequestedByClient, messages, num > val2);
      }
      MessageStore<T>.Fragment fragment2;
      while (true)
      {
        this.GetFragmentOffsets(num, out fragmentNum, out idxIntoFragmentsArray, out idxIntoFragment);
        fragment2 = this._fragments[(idxIntoFragmentsArray + 1) % this._fragments.Length];
        if (fragment2.FragmentNum >= fragmentNum)
          num = (ulong) Volatile.Read(ref this._nextFreeMessageId);
        else
          break;
      }
      long messageId2 = (long) this.GetMessageId(fragment2.FragmentNum, this._offset);
      int count1 = Math.Min(maxMessages, fragment2.Data.Length);
      ArraySegment<T> messages1 = new ArraySegment<T>(fragment2.Data, 0, count1);
      return new MessageStoreResult<T>((ulong) messageId2, messages1, true);
    }

    private bool TryAddImpl(T message, out ulong newMessageId)
    {
      ulong fragmentNum;
      int idxIntoFragmentsArray;
      int idxIntoFragment;
      this.GetFragmentOffsets((ulong) Volatile.Read(ref this._nextFreeMessageId), out fragmentNum, out idxIntoFragmentsArray, out idxIntoFragment);
      MessageStore<T>.Fragment fragment1 = this._fragments[idxIntoFragmentsArray];
      if (fragment1 == null || fragment1.FragmentNum < fragmentNum)
      {
        if (idxIntoFragment == 0)
        {
          MessageStore<T>.Fragment fragment2 = new MessageStore<T>.Fragment(fragmentNum, this._fragmentSize);
          fragment2.Data[0] = message;
          if (Interlocked.CompareExchange<MessageStore<T>.Fragment>(ref this._fragments[idxIntoFragmentsArray], fragment2, fragment1) == fragment1)
          {
            newMessageId = this.GetMessageId(fragmentNum, this._offset);
            return true;
          }
        }
      }
      else if ((long) fragment1.FragmentNum == (long) fragmentNum)
      {
        T[] data = fragment1.Data;
        for (int offset = idxIntoFragment; offset < data.Length; ++offset)
        {
          if ((object) Interlocked.CompareExchange<T>(ref data[offset], message, default (T)) == null)
          {
            newMessageId = this.GetMessageId(fragmentNum, (uint) offset);
            return true;
          }
        }
      }
      newMessageId = 0UL;
      return false;
    }

    private sealed class Fragment
    {
      public readonly ulong FragmentNum;
      public readonly T[] Data;

      public Fragment(ulong fragmentNum, uint fragmentSize)
      {
        this.FragmentNum = fragmentNum;
        this.Data = new T[(int) fragmentSize];
      }
    }
  }
}
