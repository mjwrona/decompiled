// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.ScaleoutSubscription
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Tracing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public class ScaleoutSubscription : Subscription
  {
    private const string _scaleoutCursorPrefix = "s-";
    private readonly IList<ScaleoutMappingStore> _streams;
    private readonly List<Cursor> _cursors;
    private readonly TraceSource _trace;

    public ScaleoutSubscription(
      string identity,
      IList<string> eventKeys,
      string cursor,
      IList<ScaleoutMappingStore> streams,
      Func<MessageResult, object, Task<bool>> callback,
      int maxMessages,
      ITraceManager traceManager,
      IPerformanceCounterManager counters,
      object state)
      : base(identity, eventKeys, callback, maxMessages, counters, state)
    {
      if (streams == null)
        throw new ArgumentNullException(nameof (streams));
      if (traceManager == null)
        throw new ArgumentNullException(nameof (traceManager));
      this._streams = streams;
      List<Cursor> cursors;
      if (string.IsNullOrEmpty(cursor))
      {
        cursors = new List<Cursor>();
      }
      else
      {
        cursors = Cursor.GetCursors(cursor, "s-");
        if (cursors == null)
          cursors = new List<Cursor>();
        else if (cursors.Count != this._streams.Count)
          cursors.Clear();
      }
      if (cursors.Count == 0)
      {
        for (int streamIndex = 0; streamIndex < this._streams.Count; ++streamIndex)
          this.AddCursorForStream(streamIndex, cursors);
      }
      this._cursors = cursors;
      this._trace = traceManager["SignalR." + typeof (ScaleoutSubscription).Name];
    }

    public override void WriteCursor(TextWriter textWriter) => Cursor.WriteCursors(textWriter, (IList<Cursor>) this._cursors, "s-");

    protected override void PerformWork(
      IList<ArraySegment<Message>> items,
      out int totalCount,
      out object state)
    {
      ulong?[] nullableArray = new ulong?[this._cursors.Count];
      totalCount = 0;
      IEnumerator<Tuple<ScaleoutMapping, int>> enumerator = this.GetMappings().GetEnumerator();
      while (totalCount < this.MaxMessages && enumerator.MoveNext())
      {
        ScaleoutMapping mapping = enumerator.Current.Item1;
        int streamIndex = enumerator.Current.Item2;
        ulong? nullable1 = nullableArray[streamIndex];
        if (nullable1.HasValue)
        {
          long id = (long) mapping.Id;
          ulong? nullable2 = nullable1;
          long valueOrDefault = (long) nullable2.GetValueOrDefault();
          if (!((ulong) id > (ulong) valueOrDefault & nullable2.HasValue))
            continue;
        }
        ulong messages = this.ExtractMessages(streamIndex, mapping, items, ref totalCount);
        nullableArray[streamIndex] = new ulong?(messages);
      }
      state = (object) nullableArray;
    }

    protected override void BeforeInvoke(object state)
    {
      ulong?[] nullableArray = (ulong?[]) state;
      for (int index = 0; index < this._cursors.Count; ++index)
      {
        ulong? nullable = nullableArray[index];
        if (nullable.HasValue)
        {
          Cursor cursor = this._cursors[index];
          this._trace.TraceVerbose("Setting cursor {0} value {1} to {2} [{3}]", (object) index, (object) cursor.Id, (object) nullable.Value, (object) this.Identity);
          cursor.Id = nullable.Value;
        }
      }
    }

    private IEnumerable<Tuple<ScaleoutMapping, int>> GetMappings()
    {
      ScaleoutSubscription scaleoutSubscription = this;
      List<ScaleoutSubscription.CachedStreamEnumerator> enumerators = new List<ScaleoutSubscription.CachedStreamEnumerator>();
      int count = scaleoutSubscription._streams.Count;
      for (int index = 0; index < scaleoutSubscription._streams.Count; ++index)
        enumerators.Add(new ScaleoutSubscription.CachedStreamEnumerator(scaleoutSubscription._streams[index].GetEnumerator(scaleoutSubscription._cursors[index].Id), index));
      int counter = 0;
      while (enumerators.Count > 0)
      {
        ScaleoutMapping scaleoutMapping = (ScaleoutMapping) null;
        ScaleoutSubscription.CachedStreamEnumerator streamEnumerator1 = (ScaleoutSubscription.CachedStreamEnumerator) null;
        for (int index = enumerators.Count - 1; index >= 0; --index)
        {
          ++counter;
          ScaleoutSubscription.CachedStreamEnumerator streamEnumerator2 = enumerators[index];
          ScaleoutMapping mapping;
          if (streamEnumerator2.TryMoveNext(out mapping))
          {
            if (scaleoutMapping == null || mapping.ServerCreationTime < scaleoutMapping.ServerCreationTime)
            {
              scaleoutMapping = mapping;
              streamEnumerator1 = streamEnumerator2;
            }
          }
          else
            enumerators.RemoveAt(index);
        }
        if (scaleoutMapping != null)
        {
          streamEnumerator1.ClearCachedValue();
          yield return Tuple.Create<ScaleoutMapping, int>(scaleoutMapping, streamEnumerator1.StreamIndex);
        }
      }
      // ISSUE: explicit non-virtual call
      scaleoutSubscription._trace.TraceEvent(TraceEventType.Verbose, 0, string.Format("End of mappings (connection ID: {0}). Total mappings processed: {1}", (object) __nonvirtual (scaleoutSubscription.Identity), (object) counter));
    }

    private ulong ExtractMessages(
      int streamIndex,
      ScaleoutMapping mapping,
      IList<ArraySegment<Message>> items,
      ref int totalCount)
    {
      lock (this.EventKeys)
      {
        for (int index1 = 0; index1 < this.EventKeys.Count; ++index1)
        {
          string eventKey = this.EventKeys[index1];
          for (int index2 = 0; index2 < mapping.LocalKeyInfo.Count; ++index2)
          {
            LocalEventKeyInfo localEventKeyInfo = mapping.LocalKeyInfo[index2];
            MessageStore<Message> messageStore = localEventKeyInfo.MessageStore;
            if (messageStore != null && localEventKeyInfo.Key.Equals(eventKey, StringComparison.OrdinalIgnoreCase))
            {
              MessageStoreResult<Message> messages1 = messageStore.GetMessages(localEventKeyInfo.Id, 1);
              ArraySegment<Message> messages2 = messages1.Messages;
              if (messages2.Count > 0)
              {
                messages2 = messages1.Messages;
                Message[] array = messages2.Array;
                messages2 = messages1.Messages;
                int offset = messages2.Offset;
                Message message = array[offset];
                if (message.StreamIndex == streamIndex)
                {
                  items.Add(messages1.Messages);
                  ref int local = ref totalCount;
                  int num1 = totalCount;
                  messages2 = messages1.Messages;
                  int count = messages2.Count;
                  int num2 = num1 + count;
                  local = num2;
                  TraceSource trace = this._trace;
                  object[] objArray = new object[5];
                  messages2 = messages1.Messages;
                  objArray[0] = (object) messages2.Count;
                  objArray[1] = (object) mapping.Id;
                  objArray[2] = (object) localEventKeyInfo.Key;
                  objArray[3] = (object) localEventKeyInfo.Id;
                  objArray[4] = (object) streamIndex;
                  trace.TraceVerbose("Adding {0} message(s) for mapping id: {1}, event key: '{2}', event id: {3}, streamIndex: {4}", objArray);
                  if (message.MappingId > mapping.Id)
                  {
                    this._trace.TraceEvent(TraceEventType.Verbose, 0, string.Format("Extracted additional messages, updating cursor to new Mapping ID: {0}", (object) message.MappingId));
                    return message.MappingId;
                  }
                }
                else
                  this._trace.TraceInformation("Stream index mismatch. Mapping id: {0}, event key: '{1}', event id: {2}, message.StreamIndex: {3}, streamIndex: {4}", (object) mapping.Id, (object) localEventKeyInfo.Key, (object) localEventKeyInfo.Id, (object) message.StreamIndex, (object) streamIndex);
              }
            }
          }
        }
      }
      return mapping.Id;
    }

    private void AddCursorForStream(int streamIndex, List<Cursor> cursors)
    {
      ScaleoutMapping maxMapping = this._streams[streamIndex].MaxMapping;
      ulong num = ulong.MaxValue;
      string key = streamIndex.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (maxMapping != null)
        num = maxMapping.Id;
      long id = (long) num;
      Cursor cursor = new Cursor(key, (ulong) id);
      cursors.Add(cursor);
    }

    private class CachedStreamEnumerator
    {
      private readonly IEnumerator<ScaleoutMapping> _enumerator;
      private ScaleoutMapping _cachedValue;

      public CachedStreamEnumerator(IEnumerator<ScaleoutMapping> enumerator, int streamIndex)
      {
        this._enumerator = enumerator;
        this.StreamIndex = streamIndex;
      }

      public int StreamIndex { get; private set; }

      public bool TryMoveNext(out ScaleoutMapping mapping)
      {
        mapping = (ScaleoutMapping) null;
        if (this._cachedValue != null)
        {
          mapping = this._cachedValue;
          return true;
        }
        if (!this._enumerator.MoveNext())
          return false;
        mapping = this._enumerator.Current;
        this._cachedValue = mapping;
        return true;
      }

      public void ClearCachedValue() => this._cachedValue = (ScaleoutMapping) null;
    }
  }
}
