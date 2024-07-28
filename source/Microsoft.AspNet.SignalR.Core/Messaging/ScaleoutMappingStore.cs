// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.ScaleoutMappingStore
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public class ScaleoutMappingStore
  {
    private ScaleoutStore _store;
    private readonly uint _maxMessages;

    public ScaleoutMappingStore()
      : this((int) ushort.MaxValue)
    {
    }

    public ScaleoutMappingStore(int maxMessages)
    {
      this._maxMessages = (uint) maxMessages;
      this._store = new ScaleoutStore(this._maxMessages);
    }

    public void Add(ulong id, ScaleoutMessage message, IList<LocalEventKeyInfo> localKeyInfo)
    {
      if (this.MaxMapping != null && id < this.MaxMapping.Id)
        this._store = new ScaleoutStore(this._maxMessages);
      long num = (long) this._store.Add(new ScaleoutMapping(id, message, localKeyInfo));
    }

    public ScaleoutMapping MaxMapping => this._store.MaxMapping;

    public IEnumerator<ScaleoutMapping> GetEnumerator(ulong id) => (IEnumerator<ScaleoutMapping>) new ScaleoutMappingStore.ScaleoutStoreEnumerator(this._store, this._store.GetMessagesByMappingId(id));

    private struct ScaleoutStoreEnumerator : IEnumerator<ScaleoutMapping>, IDisposable, IEnumerator
    {
      private readonly WeakReference _storeReference;
      private MessageStoreResult<ScaleoutMapping> _result;
      private int _offset;
      private int _length;
      private ulong _nextId;

      public ScaleoutStoreEnumerator(
        ScaleoutStore store,
        MessageStoreResult<ScaleoutMapping> result)
        : this()
      {
        this._storeReference = new WeakReference((object) store);
        this.Initialize(result);
      }

      public ScaleoutMapping Current => this._result.Messages.Array[this._offset];

      public void Dispose()
      {
      }

      object IEnumerator.Current => (object) this.Current;

      public bool MoveNext()
      {
        ++this._offset;
        if (this._offset < this._length)
          return true;
        if (!this._result.HasMoreData)
          return false;
        ScaleoutStore target = (ScaleoutStore) this._storeReference.Target;
        if (target == null)
          return false;
        this.Initialize(target.GetMessages(this._nextId));
        ++this._offset;
        return this._offset < this._length;
      }

      public void Reset() => throw new NotSupportedException();

      private void Initialize(MessageStoreResult<ScaleoutMapping> result)
      {
        this._result = result;
        this._offset = this._result.Messages.Offset - 1;
        ArraySegment<ScaleoutMapping> messages = this._result.Messages;
        int offset = messages.Offset;
        messages = this._result.Messages;
        int count = messages.Count;
        this._length = offset + count;
        this._nextId = this._result.FirstMessageId + (ulong) this._result.Messages.Count;
      }
    }
  }
}
