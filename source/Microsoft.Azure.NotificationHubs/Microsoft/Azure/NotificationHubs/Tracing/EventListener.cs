// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.EventListener
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  internal abstract class EventListener : IDisposable
  {
    internal volatile EventListener m_Next;
    internal static EventListener s_Listeners;
    internal static List<WeakReference> s_EventSources;

    protected EventListener()
    {
      lock (EventListener.EventListenersLock)
      {
        foreach (WeakReference eventSource in EventListener.s_EventSources)
        {
          if (eventSource.Target is EventSource target)
            target.AddListener(this);
        }
        this.m_Next = EventListener.s_Listeners;
        EventListener.s_Listeners = this;
      }
    }

    public virtual void Dispose()
    {
      lock (EventListener.EventListenersLock)
      {
        if (EventListener.s_Listeners == null)
          return;
        if (this == EventListener.s_Listeners)
        {
          EventListener.s_Listeners = this.m_Next;
        }
        else
        {
          EventListener eventListener = EventListener.s_Listeners;
          EventListener next;
          while (true)
          {
            next = eventListener.m_Next;
            if (next != null)
            {
              if (next != this)
                eventListener = next;
              else
                goto label_7;
            }
            else
              break;
          }
          return;
label_7:
          eventListener.m_Next = next.m_Next;
          EventListener.RemoveReferencesToListenerInEventSources(next);
        }
      }
    }

    public void EnableEvents(EventSource eventSource, EventLevel level) => this.EnableEvents(eventSource, level, EventKeywords.None);

    public void EnableEvents(
      EventSource eventSource,
      EventLevel level,
      EventKeywords matchAnyKeyword)
    {
      this.EnableEvents(eventSource, level, matchAnyKeyword, (IDictionary<string, string>) null);
    }

    public void EnableEvents(
      EventSource eventSource,
      EventLevel level,
      EventKeywords matchAnyKeyword,
      IDictionary<string, string> arguments)
    {
      if (eventSource == null)
        throw new ArgumentNullException(nameof (eventSource));
      EventSource.SendCommand(eventSource, this, EventCommand.Update, true, level, matchAnyKeyword, arguments);
    }

    public void DisableEvents(EventSource eventSource)
    {
      if (eventSource == null)
        throw new ArgumentNullException(nameof (eventSource));
      EventSource.SendCommand(eventSource, this, EventCommand.Update, false, EventLevel.LogAlways, EventKeywords.None, (IDictionary<string, string>) null);
    }

    protected internal virtual void OnEventSourceCreated(EventSource eventSource)
    {
    }

    protected internal abstract void OnEventWritten(EventWrittenEventArgs eventData);

    protected static int EventSourceIndex(EventSource eventSource) => eventSource.m_id;

    internal static void AddEventSource(EventSource newEventSource)
    {
      lock (EventListener.EventListenersLock)
      {
        if (EventListener.s_EventSources == null)
          EventListener.s_EventSources = new List<WeakReference>(2);
        int num = -1;
        if (EventListener.s_EventSources.Count % 64 == 63)
        {
          int count = EventListener.s_EventSources.Count;
          while (0 < count)
          {
            --count;
            WeakReference eventSource = EventListener.s_EventSources[count];
            if (!eventSource.IsAlive)
            {
              num = count;
              eventSource.Target = (object) newEventSource;
              break;
            }
          }
        }
        if (num < 0)
        {
          num = EventListener.s_EventSources.Count;
          EventListener.s_EventSources.Add(new WeakReference((object) newEventSource));
        }
        newEventSource.m_id = num;
        for (EventListener listener = EventListener.s_Listeners; listener != null; listener = listener.m_Next)
          newEventSource.AddListener(listener);
      }
    }

    private static void RemoveReferencesToListenerInEventSources(EventListener listenerToRemove)
    {
      using (List<WeakReference>.Enumerator enumerator = EventListener.s_EventSources.GetEnumerator())
      {
label_10:
        while (enumerator.MoveNext())
        {
          if (enumerator.Current.Target is EventSource target)
          {
            if (target.m_Dispatchers.m_Listener == listenerToRemove)
            {
              EventSource eventSource = target;
              eventSource.m_Dispatchers = eventSource.m_Dispatchers.m_Next;
            }
            else
            {
              EventDispatcher eventDispatcher = target.m_Dispatchers;
              EventDispatcher next;
              while (true)
              {
                next = eventDispatcher.m_Next;
                if (next != null)
                {
                  if (next.m_Listener != listenerToRemove)
                    eventDispatcher = next;
                  else
                    break;
                }
                else
                  goto label_10;
              }
              eventDispatcher.m_Next = next.m_Next;
            }
          }
        }
      }
    }

    [Conditional("DEBUG")]
    internal static void Validate()
    {
      lock (EventListener.EventListenersLock)
      {
        Dictionary<EventListener, bool> dictionary = new Dictionary<EventListener, bool>();
        for (EventListener key = EventListener.s_Listeners; key != null; key = key.m_Next)
          dictionary.Add(key, true);
        int num = -1;
        foreach (WeakReference eventSource in EventListener.s_EventSources)
        {
          ++num;
          if (eventSource.Target is EventSource target)
          {
            EventDispatcher eventDispatcher1 = target.m_Dispatchers;
            while (eventDispatcher1 != null)
              eventDispatcher1 = eventDispatcher1.m_Next;
            using (Dictionary<EventListener, bool>.KeyCollection.Enumerator enumerator = dictionary.Keys.GetEnumerator())
            {
label_15:
              while (enumerator.MoveNext())
              {
                EventListener current = enumerator.Current;
                EventDispatcher eventDispatcher2 = target.m_Dispatchers;
                while (true)
                {
                  if (eventDispatcher2.m_Listener != current)
                    eventDispatcher2 = eventDispatcher2.m_Next;
                  else
                    goto label_15;
                }
              }
            }
          }
        }
      }
    }

    internal static object EventListenersLock
    {
      get
      {
        if (EventListener.s_EventSources == null)
          Interlocked.CompareExchange<List<WeakReference>>(ref EventListener.s_EventSources, new List<WeakReference>(2), (List<WeakReference>) null);
        return (object) EventListener.s_EventSources;
      }
    }
  }
}
