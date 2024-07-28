// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.MapKey
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal struct MapKey : IEquatable<MapKey>
  {
    private object key;

    public MapKey(object key) => this.key = key;

    public object Key => this.key;

    public bool Equals(MapKey other)
    {
      if (this.key == null && other.key == null)
        return true;
      return this.key != null && other.key != null && this.key.Equals(other.key);
    }

    public override int GetHashCode() => this.key == null ? 0 : this.key.GetHashCode();

    public override string ToString() => this.key != null ? this.key.ToString() : "<null>";
  }
}
