// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.IDictionaryExtension
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal static class IDictionaryExtension
  {
    public static void Add(
      this IDictionary<string, object> dictionary,
      bool condition,
      string key,
      object value)
    {
      if (!condition)
        return;
      dictionary.Add(key, value);
    }
  }
}
