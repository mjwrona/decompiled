// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.ProtocolHeader
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System;
using System.Globalization;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class ProtocolHeader : IAmqpSerializable
  {
    public static readonly ProtocolHeader Amqp100 = new ProtocolHeader(ProtocolId.Amqp, new AmqpVersion((byte) 1, (byte) 0, (byte) 0));
    public static readonly ProtocolHeader AmqpTls100 = new ProtocolHeader(ProtocolId.AmqpTls, new AmqpVersion((byte) 1, (byte) 0, (byte) 0));
    public static readonly ProtocolHeader AmqpSasl100 = new ProtocolHeader(ProtocolId.AmqpSasl, new AmqpVersion((byte) 1, (byte) 0, (byte) 0));
    private const uint AmqpPrefix = 1095586128;
    private ProtocolId protocolId;
    private AmqpVersion version;

    public ProtocolHeader()
    {
    }

    public ProtocolHeader(ProtocolId id, AmqpVersion version)
    {
      this.protocolId = id;
      this.version = version;
    }

    public ProtocolId ProtocolId => this.protocolId;

    public AmqpVersion Version => this.version;

    public int EncodeSize => 8;

    public void Encode(ByteBuffer buffer)
    {
      AmqpBitConverter.WriteUInt(buffer, 1095586128U);
      AmqpBitConverter.WriteUByte(buffer, (byte) this.protocolId);
      AmqpBitConverter.WriteUByte(buffer, this.version.Major);
      AmqpBitConverter.WriteUByte(buffer, this.version.Minor);
      AmqpBitConverter.WriteUByte(buffer, this.version.Revision);
    }

    public void Decode(ByteBuffer buffer)
    {
      if (buffer.Length < this.EncodeSize)
        throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpInsufficientBufferSize((object) this.EncodeSize, (object) buffer.Length));
      this.protocolId = AmqpBitConverter.ReadUInt(buffer) == 1095586128U ? (ProtocolId) AmqpBitConverter.ReadUByte(buffer) : throw AmqpEncoding.GetEncodingException("ProtocolName");
      this.version = new AmqpVersion(AmqpBitConverter.ReadUByte(buffer), AmqpBitConverter.ReadUByte(buffer), AmqpBitConverter.ReadUByte(buffer));
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AMQP {0} {1}", new object[2]
    {
      (object) (byte) this.protocolId,
      (object) this.version
    });

    public override bool Equals(object obj) => obj is ProtocolHeader protocolHeader && protocolHeader.protocolId == this.protocolId && protocolHeader.version.Equals(this.version);

    public override int GetHashCode() => (((int) this.protocolId << 24) + ((int) this.version.Major << 16) + ((int) this.version.Minor << 8) + (int) this.version.Revision).GetHashCode();
  }
}
