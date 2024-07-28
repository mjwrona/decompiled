// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.DictionaryStoredIdentityEventHandler`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class DictionaryStoredIdentityEventHandler<TKey, TEventArgs> : 
    IDictionaryStoredIdentityEventHandler<TKey, TEventArgs>,
    IIdentityEventHandler<TEventArgs>
    where TEventArgs : EventArgs
  {
    private ConcurrentDictionary<TKey, EventHandler<TEventArgs>> concurrentDictionary = new ConcurrentDictionary<TKey, EventHandler<TEventArgs>>();

    public int Count => this.concurrentDictionary.Count;

    public void AddHandler(EventHandler<TEventArgs> eventHandler) => throw new NotSupportedException("Use dictionary based method with key, value parameters.");

    public void RemoveHandler(EventHandler<TEventArgs> eventHandler) => throw new NotSupportedException("Use dictionary based method with key as parameter.");

    public void AddHandler(TKey key, EventHandler<TEventArgs> eventHandler) => this.concurrentDictionary.AddOrUpdate(key, eventHandler, (Func<TKey, EventHandler<TEventArgs>, EventHandler<TEventArgs>>) ((a, b) => eventHandler));

    public void RemoveHandler(TKey key) => this.concurrentDictionary.TryRemove(key, out EventHandler<TEventArgs> _);

    public void RaiseEvents(object sender, TEventArgs eventArgs)
    {
      foreach (EventHandler<TEventArgs> eventHandler in (IEnumerable<EventHandler<TEventArgs>>) this.concurrentDictionary.Values)
        eventHandler((object) this, eventArgs);
    }

    public bool ContainsEvents() => this.Count != 0;
  }
}
