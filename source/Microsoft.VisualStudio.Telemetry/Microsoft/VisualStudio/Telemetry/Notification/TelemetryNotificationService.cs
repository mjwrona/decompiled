// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Notification.TelemetryNotificationService
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry.Notification
{
  public class TelemetryNotificationService : ITelemetryNotificationService, ISetTelemetrySession
  {
    private static Lazy<ITelemetryNotificationService> defaultLazy;
    private const string TelemetryNotificationBaseEventName = "VS/Core/TelemetryNotification";
    internal const string TelemetryNotificationFilterFaultEventName = "VS/Core/TelemetryNotification/FilterFault";
    internal const string TelemetryNotificationHandlerFaultEventName = "VS/Core/TelemetryNotification/HandlerFault";
    private readonly object lockObject = new object();
    private readonly Lazy<IDictionary<int, TelemetryNotificationService.Subscription>> subscriptionsLazy = new Lazy<IDictionary<int, TelemetryNotificationService.Subscription>>((Func<IDictionary<int, TelemetryNotificationService.Subscription>>) (() => (IDictionary<int, TelemetryNotificationService.Subscription>) new Dictionary<int, TelemetryNotificationService.Subscription>()));
    private ITelemetryNotificationProvider provider;
    private ITelemetryTestChannel channel;
    private int lastSubscriptionId;
    private ConcurrentQueue<TelemetryEvent> queueTelemetryEvents = new ConcurrentQueue<TelemetryEvent>();
    internal AsyncManualResetEvent EventNewItemAvailableForNotification = new AsyncManualResetEvent();
    private CancellationTokenSource cancellationTokenSource;

    public static ITelemetryNotificationService Default => TelemetryNotificationService.defaultLazy.Value;

    static TelemetryNotificationService() => TelemetryNotificationService.Initialize();

    internal static void Initialize() => TelemetryNotificationService.defaultLazy = new Lazy<ITelemetryNotificationService>((Func<ITelemetryNotificationService>) (() => (ITelemetryNotificationService) new TelemetryNotificationService((ITelemetryNotificationProvider) new TelemetryNotificationProvider((TelemetrySession) null))));

    internal TelemetryNotificationService(ITelemetryNotificationProvider provider)
    {
      provider.RequiresArgumentNotNull<ITelemetryNotificationProvider>(nameof (provider));
      this.provider = provider;
    }

    public int Subscribe(
      ITelemetryEventMatch eventMatch,
      Action<TelemetryEvent> handler,
      bool singleNotification = true)
    {
      eventMatch.RequiresArgumentNotNull<ITelemetryEventMatch>(nameof (eventMatch));
      handler.RequiresArgumentNotNull<Action<TelemetryEvent>>(nameof (handler));
      TelemetryNotificationService.Subscription subscription = new TelemetryNotificationService.Subscription()
      {
        Handler = handler,
        Match = eventMatch,
        SingleNotification = singleNotification
      };
      lock (this.lockObject)
      {
        ++this.lastSubscriptionId;
        this.AttachChannel();
        this.Subscriptions[this.lastSubscriptionId] = subscription;
        return this.lastSubscriptionId;
      }
    }

    public void Unsubscribe(int subscriptionId)
    {
      lock (this.lockObject)
      {
        if (!this.Subscriptions.Remove(subscriptionId) || this.Subscriptions.Count != 0)
          return;
        this.DetachChannel();
      }
    }

    private IDictionary<int, TelemetryNotificationService.Subscription> Subscriptions => this.subscriptionsLazy.Value;

    private void AttachChannel()
    {
      lock (this.lockObject)
      {
        if (this.channel != null)
          return;
        this.channel = (ITelemetryTestChannel) new NotificationTelemetryChannel(new Action<TelemetryEvent>(this.OnPostEvent));
        this.provider.AttachChannel(this.channel);
        if (this.cancellationTokenSource != null && !this.cancellationTokenSource.IsCancellationRequested)
          return;
        this.cancellationTokenSource = new CancellationTokenSource();
        Task.Run((Func<Task>) (async () => await this.ListenForEventsInQueue()));
      }
    }

    private void DetachChannel()
    {
      lock (this.lockObject)
      {
        if (this.channel == null)
          return;
        this.provider.DetachChannel(this.channel);
        this.channel = (ITelemetryTestChannel) null;
        this.cancellationTokenSource.Cancel();
        this.EventNewItemAvailableForNotification.Set();
      }
    }

    private async Task ListenForEventsInQueue()
    {
      while (!this.cancellationTokenSource.IsCancellationRequested)
      {
        await this.EventNewItemAvailableForNotification.WaitAsync();
        this.EventNewItemAvailableForNotification.Reset();
        while (this.queueTelemetryEvents.Count<TelemetryEvent>() > 0)
        {
          TelemetryEvent result = (TelemetryEvent) null;
          if (this.queueTelemetryEvents.TryDequeue(out result))
            this.ProcessPostedEvents(result);
        }
      }
    }

    private void OnPostEvent(TelemetryEvent telemetryEvent)
    {
      this.queueTelemetryEvents.Enqueue(telemetryEvent);
      this.EventNewItemAvailableForNotification.Set();
    }

    private void ProcessPostedEvents(TelemetryEvent telemetryEvent)
    {
      KeyValuePair<int, TelemetryNotificationService.Subscription>[] array;
      lock (this.lockObject)
        array = this.Subscriptions.ToArray<KeyValuePair<int, TelemetryNotificationService.Subscription>>();
      foreach (KeyValuePair<int, TelemetryNotificationService.Subscription> keyValuePair in array)
      {
        TelemetryNotificationService.Subscription subscription = keyValuePair.Value;
        bool flag = false;
        try
        {
          flag = subscription.Match.IsEventMatch(telemetryEvent);
        }
        catch (Exception ex)
        {
          this.PostFaultEvent("VS/Core/TelemetryNotification/FilterFault", subscription.Match, ex);
          this.Unsubscribe(keyValuePair.Key);
        }
        if (flag)
        {
          if (subscription.SingleNotification)
            this.Unsubscribe(keyValuePair.Key);
          try
          {
            TelemetryEvent telemetryEvent1 = telemetryEvent.CloneTelemetryEvent();
            subscription.Handler(telemetryEvent1);
          }
          catch (Exception ex)
          {
            this.PostFaultEvent("VS/Core/TelemetryNotification/HandlerFault", subscription.Match, ex);
          }
        }
      }
    }

    private void PostFaultEvent(
      string eventName,
      ITelemetryEventMatch eventMatch,
      Exception exception)
    {
      string description;
      try
      {
        description = JsonConvert.SerializeObject((object) eventMatch, Formatting.None);
      }
      catch (Exception ex)
      {
        description = ex.Message;
      }
      this.provider.PostFaultEvent(eventName, description, exception);
    }

    public void SetSession(TelemetrySession session)
    {
      lock (this.lockObject)
      {
        if (this.Subscriptions.Count > 0)
          throw new InvalidOperationException("Cannot set the session after subscriptions have been made");
        this.provider = (ITelemetryNotificationProvider) new TelemetryNotificationProvider(session);
      }
    }

    private struct Subscription
    {
      public Action<TelemetryEvent> Handler;
      public ITelemetryEventMatch Match;
      public bool SingleNotification;
    }
  }
}
