// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Properties
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Properties : DescribedList
  {
    public static readonly string Name = "amqp:properties:list";
    public static readonly ulong Code = 115;
    private static readonly string MessageIdName = "message-id";
    private static readonly string UserIdName = "user-id";
    private static readonly string ToName = "to";
    private static readonly string SubjectName = "subject";
    private static readonly string ReplyToName = "reply-to";
    private static readonly string CorrelationIdName = "correlation-id";
    private static readonly string ContentTypeName = "content-type";
    private static readonly string ContentEncodingName = "content-encoding";
    private static readonly string AbsoluteExpiryTimeName = "absolute-expiry-time";
    private static readonly string CreationTimeName = "creation-time";
    private static readonly string GroupIdName = "group-id";
    private static readonly string GroupSequenceName = "group-sequence";
    private static readonly string ReplyToGroupIdName = "reply-to-group-id";
    private const int Fields = 13;

    public Properties()
      : base((AmqpSymbol) Properties.Name, Properties.Code)
    {
    }

    public MessageId MessageId { get; set; }

    public ArraySegment<byte> UserId { get; set; }

    public Address To { get; set; }

    public string Subject { get; set; }

    public Address ReplyTo { get; set; }

    public MessageId CorrelationId { get; set; }

    public AmqpSymbol ContentType { get; set; }

    public AmqpSymbol ContentEncoding { get; set; }

    public DateTime? AbsoluteExpiryTime { get; set; }

    public DateTime? CreationTime { get; set; }

    public string GroupId { get; set; }

    public uint? GroupSequence { get; set; }

    public string ReplyToGroupId { get; set; }

    protected override int FieldCount => 13;

    public IDictionary<string, object> ToDictionary()
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      dictionary.Add(this.MessageId != null, Properties.MessageIdName, (object) this.MessageId);
      dictionary.Add(this.UserId.Array != null, Properties.UserIdName, (object) this.UserId);
      dictionary.Add(this.To != null, Properties.ToName, (object) this.To);
      dictionary.Add(this.Subject != null, Properties.SubjectName, (object) this.Subject);
      dictionary.Add(this.ReplyTo != null, Properties.ReplyToName, (object) this.ReplyTo);
      dictionary.Add(this.CorrelationId != null, Properties.CorrelationIdName, (object) this.CorrelationId);
      dictionary.Add(this.ContentType.Value != null, Properties.ContentTypeName, (object) this.ContentType);
      dictionary.Add(this.ContentEncoding.Value != null, Properties.ContentEncodingName, (object) this.ContentEncoding);
      dictionary.Add(this.AbsoluteExpiryTime.HasValue, Properties.AbsoluteExpiryTimeName, (object) this.AbsoluteExpiryTime);
      dictionary.Add(this.CreationTime.HasValue, Properties.CreationTimeName, (object) this.CreationTime);
      dictionary.Add(this.GroupId != null, Properties.GroupIdName, (object) this.GroupId);
      dictionary.Add(this.GroupSequence.HasValue, Properties.GroupSequenceName, (object) this.GroupSequence);
      dictionary.Add(this.ReplyToGroupId != null, Properties.ReplyToGroupIdName, (object) this.ReplyToGroupId);
      return (IDictionary<string, object>) dictionary;
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder("properties(");
      int count = 0;
      this.AddFieldToString(this.MessageId != null, sb, Properties.MessageIdName, (object) this.MessageId, ref count);
      this.AddFieldToString(this.UserId.Array != null, sb, Properties.UserIdName, (object) this.UserId, ref count);
      this.AddFieldToString(this.To != null, sb, Properties.ToName, (object) this.To, ref count);
      this.AddFieldToString(this.Subject != null, sb, Properties.SubjectName, (object) this.Subject, ref count);
      this.AddFieldToString(this.ReplyTo != null, sb, Properties.ReplyToName, (object) this.ReplyTo, ref count);
      this.AddFieldToString(this.CorrelationId != null, sb, Properties.CorrelationIdName, (object) this.CorrelationId, ref count);
      this.AddFieldToString(this.ContentType.Value != null, sb, Properties.ContentTypeName, (object) this.ContentType, ref count);
      this.AddFieldToString(this.ContentEncoding.Value != null, sb, Properties.ContentEncodingName, (object) this.ContentEncoding, ref count);
      this.AddFieldToString(this.AbsoluteExpiryTime.HasValue, sb, Properties.AbsoluteExpiryTimeName, (object) this.AbsoluteExpiryTime, ref count);
      this.AddFieldToString(this.CreationTime.HasValue, sb, Properties.CreationTimeName, (object) this.CreationTime, ref count);
      this.AddFieldToString(this.GroupId != null, sb, Properties.GroupIdName, (object) this.GroupId, ref count);
      this.AddFieldToString(this.GroupSequence.HasValue, sb, Properties.GroupSequenceName, (object) this.GroupSequence, ref count);
      this.AddFieldToString(this.ReplyToGroupId != null, sb, Properties.ReplyToGroupIdName, (object) this.ReplyToGroupId, ref count);
      sb.Append(')');
      return sb.ToString();
    }

    protected override void OnEncode(ByteBuffer buffer)
    {
      MessageId.Encode(buffer, this.MessageId);
      AmqpCodec.EncodeBinary(this.UserId, buffer);
      Address.Encode(buffer, this.To);
      AmqpCodec.EncodeString(this.Subject, buffer);
      Address.Encode(buffer, this.ReplyTo);
      MessageId.Encode(buffer, this.CorrelationId);
      AmqpCodec.EncodeSymbol(this.ContentType, buffer);
      AmqpCodec.EncodeSymbol(this.ContentEncoding, buffer);
      AmqpCodec.EncodeTimeStamp(this.AbsoluteExpiryTime, buffer);
      AmqpCodec.EncodeTimeStamp(this.CreationTime, buffer);
      AmqpCodec.EncodeString(this.GroupId, buffer);
      AmqpCodec.EncodeUInt(this.GroupSequence, buffer);
      AmqpCodec.EncodeString(this.ReplyToGroupId, buffer);
    }

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
      if (count-- > 0)
        this.MessageId = MessageId.Decode(buffer);
      if (count-- > 0)
        this.UserId = AmqpCodec.DecodeBinary(buffer);
      if (count-- > 0)
        this.To = Address.Decode(buffer);
      if (count-- > 0)
        this.Subject = AmqpCodec.DecodeString(buffer);
      if (count-- > 0)
        this.ReplyTo = Address.Decode(buffer);
      if (count-- > 0)
        this.CorrelationId = MessageId.Decode(buffer);
      if (count-- > 0)
        this.ContentType = AmqpCodec.DecodeSymbol(buffer);
      if (count-- > 0)
        this.ContentEncoding = AmqpCodec.DecodeSymbol(buffer);
      if (count-- > 0)
        this.AbsoluteExpiryTime = AmqpCodec.DecodeTimeStamp(buffer);
      if (count-- > 0)
        this.CreationTime = AmqpCodec.DecodeTimeStamp(buffer);
      if (count-- > 0)
        this.GroupId = AmqpCodec.DecodeString(buffer);
      if (count-- > 0)
        this.GroupSequence = AmqpCodec.DecodeUInt(buffer);
      if (count-- <= 0)
        return;
      this.ReplyToGroupId = AmqpCodec.DecodeString(buffer);
    }

    protected override int OnValueSize() => MessageId.GetEncodeSize(this.MessageId) + AmqpCodec.GetBinaryEncodeSize(this.UserId) + Address.GetEncodeSize(this.To) + AmqpCodec.GetStringEncodeSize(this.Subject) + Address.GetEncodeSize(this.ReplyTo) + MessageId.GetEncodeSize(this.CorrelationId) + AmqpCodec.GetSymbolEncodeSize(this.ContentType) + AmqpCodec.GetSymbolEncodeSize(this.ContentEncoding) + AmqpCodec.GetTimeStampEncodeSize(this.AbsoluteExpiryTime) + AmqpCodec.GetTimeStampEncodeSize(this.CreationTime) + AmqpCodec.GetStringEncodeSize(this.GroupId) + AmqpCodec.GetUIntEncodeSize(this.GroupSequence) + AmqpCodec.GetStringEncodeSize(this.ReplyToGroupId);
  }
}
