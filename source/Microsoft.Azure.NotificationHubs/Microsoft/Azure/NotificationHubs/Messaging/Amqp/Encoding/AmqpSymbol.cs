// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.AmqpSymbol
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal struct AmqpSymbol : IEquatable<AmqpSymbol>
  {
    private int valueSize;

    public AmqpSymbol(string value)
      : this()
    {
      this.Value = value;
    }

    public string Value { get; private set; }

    public int ValueSize
    {
      get
      {
        if (this.valueSize == 0)
          this.valueSize = SymbolEncoding.GetValueSize(this);
        return this.valueSize;
      }
    }

    public static implicit operator AmqpSymbol(string value) => new AmqpSymbol()
    {
      Value = value
    };

    public bool Equals(AmqpSymbol other)
    {
      if (this.Value == null && other.Value == null)
        return true;
      return this.Value != null && other.Value != null && string.Compare(this.Value, other.Value, StringComparison.Ordinal) == 0;
    }

    public override int GetHashCode() => this.Value == null ? 0 : this.Value.GetHashCode();

    public override string ToString() => this.Value;
  }
}
