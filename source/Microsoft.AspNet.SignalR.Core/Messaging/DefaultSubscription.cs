// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.DefaultSubscription
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Messaging
{
  internal class DefaultSubscription : Subscription
  {
    internal static string _defaultCursorPrefix = DefaultSubscription.GetCursorPrefix();
    private List<Cursor> _cursors;
    private List<Topic> _cursorTopics;
    private ulong[] _cursorsState;
    private readonly IStringMinifier _stringMinifier;

    public DefaultSubscription(
      string identity,
      IList<string> eventKeys,
      TopicLookup topics,
      string cursor,
      Func<MessageResult, object, Task<bool>> callback,
      int maxMessages,
      IStringMinifier stringMinifier,
      IPerformanceCounterManager counters,
      object state)
      : base(identity, eventKeys, callback, maxMessages, counters, state)
    {
      this._stringMinifier = stringMinifier;
      this._cursors = !string.IsNullOrEmpty(cursor) ? Cursor.GetCursors(cursor, DefaultSubscription._defaultCursorPrefix, (Func<string, object, string>) ((k, s) => DefaultSubscription.UnminifyCursor(k, s)), (object) stringMinifier) ?? this.GetCursorsFromEventKeys(this.EventKeys, topics) : this.GetCursorsFromEventKeys(this.EventKeys, topics);
      this._cursorTopics = new List<Topic>();
      if (!string.IsNullOrEmpty(cursor))
      {
        for (int index = this._cursors.Count - 1; index >= 0; --index)
        {
          Cursor cursor1 = this._cursors[index];
          if (!this.EventKeys.Contains(cursor1.Key))
          {
            this._cursors.Remove(cursor1);
          }
          else
          {
            Topic topic;
            if (!topics.TryGetValue(this._cursors[index].Key, out topic) || this._cursors[index].Id > topic.Store.GetMessageCount())
              this.UpdateCursor(cursor1.Key, 0UL);
          }
        }
      }
      for (int index = 0; index < this._cursors.Count; ++index)
        this._cursorTopics.Add((Topic) null);
    }

    private static string UnminifyCursor(string key, object state) => ((IStringMinifier) state).Unminify(key);

    public override bool AddEvent(string eventKey, Topic topic)
    {
      base.AddEvent(eventKey, topic);
      lock (this._cursors)
      {
        if (this.FindCursorIndex(eventKey) != -1)
          return false;
        this._cursors.Add(new Cursor(eventKey, DefaultSubscription.GetMessageId(topic), this._stringMinifier.Minify(eventKey)));
        this._cursorTopics.Add(topic);
        return true;
      }
    }

    public override void RemoveEvent(string eventKey)
    {
      base.RemoveEvent(eventKey);
      lock (this._cursors)
      {
        int cursorIndex = this.FindCursorIndex(eventKey);
        if (cursorIndex == -1)
          return;
        this._cursors.RemoveAt(cursorIndex);
        this._cursorTopics.RemoveAt(cursorIndex);
      }
    }

    public override void SetEventTopic(string eventKey, Topic topic)
    {
      base.SetEventTopic(eventKey, topic);
      lock (this._cursors)
      {
        int cursorIndex = this.FindCursorIndex(eventKey);
        if (cursorIndex == -1)
          return;
        this._cursorTopics[cursorIndex] = topic;
      }
    }

    public override void WriteCursor(TextWriter textWriter)
    {
      lock (this._cursors)
        Cursor.WriteCursors(textWriter, (IList<Cursor>) this._cursors, DefaultSubscription._defaultCursorPrefix);
    }

    protected override void PerformWork(
      IList<ArraySegment<Message>> items,
      out int totalCount,
      out object state)
    {
      totalCount = 0;
      lock (this._cursors)
      {
        if (this._cursorsState == null || this._cursorsState.Length != this._cursors.Count)
          this._cursorsState = new ulong[this._cursors.Count];
        for (int index1 = 0; index1 < this._cursors.Count; ++index1)
        {
          MessageStoreResult<Message> messages1 = this._cursorTopics[index1].Store.GetMessages(this._cursors[index1].Id, this.MaxMessages);
          ulong[] cursorsState = this._cursorsState;
          int index2 = index1;
          long firstMessageId = (long) messages1.FirstMessageId;
          ArraySegment<Message> messages2 = messages1.Messages;
          long count1 = (long) messages2.Count;
          long num1 = firstMessageId + count1;
          cursorsState[index2] = (ulong) num1;
          messages2 = messages1.Messages;
          if (messages2.Count > 0)
          {
            items.Add(messages1.Messages);
            ref int local = ref totalCount;
            int num2 = totalCount;
            messages2 = messages1.Messages;
            int count2 = messages2.Count;
            int num3 = num2 + count2;
            local = num3;
          }
        }
        state = (object) this._cursorsState;
      }
    }

    protected override void BeforeInvoke(object state)
    {
      lock (this._cursors)
      {
        ulong[] numArray = (ulong[]) state;
        for (int index = 0; index < this._cursors.Count; ++index)
          this._cursors[index].Id = numArray[index];
      }
    }

    private bool UpdateCursor(string key, ulong id)
    {
      lock (this._cursors)
      {
        int cursorIndex = this.FindCursorIndex(key);
        if (cursorIndex == -1)
          return false;
        this._cursors[cursorIndex].Id = id;
        return true;
      }
    }

    private int FindCursorIndex(string eventKey)
    {
      for (int index = 0; index < this._cursors.Count; ++index)
      {
        if (this._cursors[index].Key == eventKey)
          return index;
      }
      return -1;
    }

    private List<Cursor> GetCursorsFromEventKeys(IList<string> eventKeys, TopicLookup topics)
    {
      List<Cursor> cursorsFromEventKeys = new List<Cursor>(eventKeys.Count);
      foreach (string eventKey in (IEnumerable<string>) eventKeys)
      {
        Cursor cursor = new Cursor(eventKey, DefaultSubscription.GetMessageId(topics, eventKey), this._stringMinifier.Minify(eventKey));
        cursorsFromEventKeys.Add(cursor);
      }
      return cursorsFromEventKeys;
    }

    private static string GetCursorPrefix()
    {
      using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider())
      {
        byte[] data = new byte[4];
        cryptoServiceProvider.GetBytes(data);
        using (StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
        {
          Cursor.WriteUlongAsHexToBuffer((ulong) BitConverter.ToUInt32(data, 0), (TextWriter) stringWriter);
          return "d-" + stringWriter.ToString() + "-";
        }
      }
    }

    private static ulong GetMessageId(TopicLookup topics, string key)
    {
      Topic topic;
      return topics.TryGetValue(key, out topic) ? DefaultSubscription.GetMessageId(topic) : 0UL;
    }

    private static ulong GetMessageId(Topic topic) => topic == null ? 0UL : topic.Store.GetMessageCount();
  }
}
