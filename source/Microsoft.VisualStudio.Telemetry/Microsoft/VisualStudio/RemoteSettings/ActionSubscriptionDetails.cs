// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.ActionSubscriptionDetails
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Telemetry.Notification;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.RemoteSettings
{
  public sealed class ActionSubscriptionDetails
  {
    public const string StartTrigger = "start";
    public const string StopTrigger = "stop";

    public bool TriggerAlways { get; internal set; }

    public bool TriggerOnSubscribe { get; internal set; }

    public TelemetryEvent TelemetryEvent { get; internal set; }

    public IEnumerable<string> RegisteredTriggerNames { get; internal set; }

    public string TriggerName { get; internal set; }

    internal IDictionary<string, int> TriggerSubscriptions { get; set; }

    internal ITelemetryNotificationService NotificationService { get; set; }

    internal object TriggerLockObject { get; set; }

    public void Unsubscribe()
    {
      if (this.NotificationService == null || this.TriggerSubscriptions == null)
        return;
      lock (this.TriggerLockObject)
      {
        if (!this.TriggerSubscriptions.ContainsKey(this.TriggerName))
          return;
        this.NotificationService.Unsubscribe(this.TriggerSubscriptions[this.TriggerName]);
        this.TriggerSubscriptions.Remove(this.TriggerName);
      }
    }

    public void UnsubscribeAll()
    {
      if (this.NotificationService == null || this.TriggerSubscriptions == null)
        return;
      lock (this.TriggerLockObject)
      {
        if (this.TriggerSubscriptions.Count <= 0)
          return;
        foreach (int subscriptionId in (IEnumerable<int>) this.TriggerSubscriptions.Values)
          this.NotificationService.Unsubscribe(subscriptionId);
        this.TriggerSubscriptions.Clear();
      }
    }
  }
}
