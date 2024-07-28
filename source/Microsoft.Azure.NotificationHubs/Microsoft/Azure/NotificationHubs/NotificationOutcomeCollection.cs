// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.NotificationOutcomeCollection
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [CollectionDataContract(Name = "NotificationOutcomeCollection", ItemName = "Outcome", KeyName = "Name", ValueName = "Count", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public sealed class NotificationOutcomeCollection : Dictionary<string, long>
  {
    public NotificationOutcomeCollection()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
    }

    internal NotificationOutcomeCollection(IEnumerable<string> notificationOutcomes)
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      foreach (string notificationOutcome in notificationOutcomes)
      {
        if (this.ContainsKey(notificationOutcome))
          ++this[notificationOutcome];
        else
          this[notificationOutcome] = 1L;
      }
    }

    internal void AddRange(IEnumerable<string> notificationOutcomes)
    {
      foreach (string notificationOutcome in notificationOutcomes)
      {
        if (this.ContainsKey(notificationOutcome))
          ++this[notificationOutcome];
        else
          this[notificationOutcome] = 1L;
      }
    }

    internal NotificationOutcomeCollection Add(NotificationOutcomeCollection outcomeCollection)
    {
      foreach (KeyValuePair<string, long> outcome in (Dictionary<string, long>) outcomeCollection)
      {
        if (this.ContainsKey(outcome.Key))
          this[outcome.Key] = this[outcome.Key] + outcome.Value;
        else
          this[outcome.Key] = outcome.Value;
      }
      return this;
    }

    internal static NotificationOutcomeCollection Rollup(
      IEnumerable<NotificationOutcomeCollection> outcomeCollection)
    {
      NotificationOutcomeCollection outcomeCollection1 = new NotificationOutcomeCollection();
      foreach (Dictionary<string, long> outcome in outcomeCollection)
      {
        foreach (KeyValuePair<string, long> keyValuePair in outcome)
        {
          if (outcomeCollection1.ContainsKey(keyValuePair.Key))
            outcomeCollection1[keyValuePair.Key] = outcomeCollection1[keyValuePair.Key] + keyValuePair.Value;
          else
            outcomeCollection1[keyValuePair.Key] = keyValuePair.Value;
        }
      }
      return outcomeCollection1;
    }
  }
}
