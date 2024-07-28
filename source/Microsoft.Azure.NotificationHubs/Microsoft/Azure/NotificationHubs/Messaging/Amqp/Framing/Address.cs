// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Address
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal abstract class Address
  {
    public abstract int EncodeSize { get; }

    public static implicit operator Address(string value) => (Address) new Address.AddressString(value);

    public static int GetEncodeSize(Address address) => address != null ? address.EncodeSize : 1;

    public static void Encode(ByteBuffer buffer, Address address)
    {
      if (address == null)
        AmqpEncoding.EncodeNull(buffer);
      else
        address.OnEncode(buffer);
    }

    public static Address Decode(ByteBuffer buffer)
    {
      object obj = AmqpEncoding.DecodeObject(buffer);
      if (obj == null)
        return (Address) null;
      return obj is string ? (Address) (string) obj : throw new NotSupportedException(obj.GetType().ToString());
    }

    public abstract void OnEncode(ByteBuffer buffer);

    private sealed class AddressString : Address
    {
      private string address;

      public AddressString(string id) => this.address = id;

      public override int EncodeSize => AmqpCodec.GetStringEncodeSize(this.address);

      public override void OnEncode(ByteBuffer buffer) => AmqpCodec.EncodeString(this.address, buffer);

      public override string ToString() => this.address;
    }
  }
}
