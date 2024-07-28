// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.MessageId
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System;
using System.Globalization;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal abstract class MessageId
  {
    public abstract int EncodeSize { get; }

    public static implicit operator MessageId(ulong value) => (MessageId) new MessageId.MessageIdUlong(value);

    public static implicit operator MessageId(Guid value) => (MessageId) new MessageId.MessageIdUuid(value);

    public static implicit operator MessageId(ArraySegment<byte> value) => (MessageId) new MessageId.MessageIdBinary(value);

    public static implicit operator MessageId(string value) => (MessageId) new MessageId.MessageIdString(value);

    public static int GetEncodeSize(MessageId messageId) => messageId != null ? messageId.EncodeSize : 1;

    public static void Encode(ByteBuffer buffer, MessageId messageId)
    {
      if (messageId == null)
        AmqpEncoding.EncodeNull(buffer);
      else
        messageId.OnEncode(buffer);
    }

    public static MessageId Decode(ByteBuffer buffer)
    {
      object obj = AmqpEncoding.DecodeObject(buffer);
      switch (obj)
      {
        case null:
          return (MessageId) null;
        case ulong num:
          return (MessageId) num;
        case Guid guid:
          return (MessageId) guid;
        case ArraySegment<byte> arraySegment:
          return (MessageId) arraySegment;
        case string _:
          return (MessageId) (string) obj;
        default:
          throw new NotSupportedException(obj.GetType().ToString());
      }
    }

    public abstract void OnEncode(ByteBuffer buffer);

    private sealed class MessageIdUlong : MessageId
    {
      private ulong id;

      public MessageIdUlong(ulong id) => this.id = id;

      public override int EncodeSize => AmqpCodec.GetULongEncodeSize(new ulong?(this.id));

      public override void OnEncode(ByteBuffer buffer) => AmqpCodec.EncodeULong(new ulong?(this.id), buffer);

      public override bool Equals(object obj) => obj is MessageId.MessageIdUlong messageIdUlong && (long) this.id == (long) messageIdUlong.id;

      public override string ToString() => this.id.ToString((IFormatProvider) CultureInfo.InvariantCulture);

      public override int GetHashCode() => this.id.GetHashCode();
    }

    private sealed class MessageIdUuid : MessageId
    {
      private Guid id;

      public MessageIdUuid(Guid id) => this.id = id;

      public override int EncodeSize => AmqpCodec.GetUuidEncodeSize(new Guid?(this.id));

      public override void OnEncode(ByteBuffer buffer) => AmqpCodec.EncodeUuid(new Guid?(this.id), buffer);

      public override bool Equals(object obj) => obj is MessageId.MessageIdUuid messageIdUuid && this.id == messageIdUuid.id;

      public override string ToString() => this.id.ToString();

      public override int GetHashCode() => this.id.GetHashCode();
    }

    private sealed class MessageIdBinary : MessageId
    {
      private ArraySegment<byte> id;

      public MessageIdBinary(ArraySegment<byte> id) => this.id = id;

      public override int EncodeSize => AmqpCodec.GetBinaryEncodeSize(this.id);

      public override void OnEncode(ByteBuffer buffer) => AmqpCodec.EncodeBinary(this.id, buffer);

      public override bool Equals(object obj) => obj is MessageId.MessageIdBinary messageIdBinary && ByteArrayComparer.Instance.Equals(this.id, messageIdBinary.id);

      public override int GetHashCode() => ByteArrayComparer.Instance.GetHashCode(this.id);

      public override string ToString() => this.id.GetString();
    }

    private sealed class MessageIdString : MessageId
    {
      private string id;

      public MessageIdString(string id) => this.id = id;

      public override int EncodeSize => AmqpCodec.GetStringEncodeSize(this.id);

      public override void OnEncode(ByteBuffer buffer) => AmqpCodec.EncodeString(this.id, buffer);

      public override bool Equals(object obj) => obj is MessageId.MessageIdString messageIdString && this.id.Equals(messageIdString.id);

      public override string ToString() => this.id;

      public override int GetHashCode() => this.id.GetHashCode();
    }
  }
}
