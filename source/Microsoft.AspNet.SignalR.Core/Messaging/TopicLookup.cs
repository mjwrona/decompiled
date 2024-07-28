// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.TopicLookup
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public sealed class TopicLookup : IEnumerable<KeyValuePair<string, Topic>>, IEnumerable
  {
    private readonly ConcurrentDictionary<string, Topic> _topics = new ConcurrentDictionary<string, Topic>();
    private readonly ConcurrentDictionary<string, Topic> _groupTopics = new ConcurrentDictionary<string, Topic>((IEqualityComparer<string>) new SipHashBasedStringEqualityComparer());

    public int Count => this._topics.Count + this._groupTopics.Count;

    public Topic this[string key]
    {
      get
      {
        Topic topic;
        return this.TryGetValue(key, out topic) ? topic : (Topic) null;
      }
    }

    public bool ContainsKey(string key) => PrefixHelper.HasGroupPrefix(key) ? this._groupTopics.ContainsKey(key) : this._topics.ContainsKey(key);

    public bool TryGetValue(string key, out Topic topic) => PrefixHelper.HasGroupPrefix(key) ? this._groupTopics.TryGetValue(key, out topic) : this._topics.TryGetValue(key, out topic);

    public IEnumerator<KeyValuePair<string, Topic>> GetEnumerator() => this._topics.Concat<KeyValuePair<string, Topic>>((IEnumerable<KeyValuePair<string, Topic>>) this._groupTopics).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public bool TryRemove(string key)
    {
      Topic topic;
      return PrefixHelper.HasGroupPrefix(key) ? this._groupTopics.TryRemove(key, out topic) : this._topics.TryRemove(key, out topic);
    }

    public Topic GetOrAdd(string key, Func<string, Topic> factory) => PrefixHelper.HasGroupPrefix(key) ? this._groupTopics.GetOrAdd(key, factory) : this._topics.GetOrAdd(key, factory);

    public void Clear()
    {
      this._topics.Clear();
      this._groupTopics.Clear();
    }
  }
}
