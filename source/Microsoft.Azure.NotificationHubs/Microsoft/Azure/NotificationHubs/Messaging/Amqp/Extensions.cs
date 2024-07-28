// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Extensions
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp
{
  internal static class Extensions
  {
    public static string GetString(this ArraySegment<byte> binary)
    {
      StringBuilder stringBuilder = new StringBuilder(binary.Count * 2);
      for (int index = 0; index < binary.Count; ++index)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0:X2}", new object[1]
        {
          (object) binary.Array[binary.Offset + index]
        });
      return stringBuilder.ToString();
    }

    public static uint MaxFrameSize(this Open open) => open.MaxFrameSize.HasValue ? open.MaxFrameSize.Value : uint.MaxValue;

    public static ushort ChannelMax(this Open open) => open.ChannelMax.HasValue ? open.ChannelMax.Value : ushort.MaxValue;

    public static uint IdleTimeOut(this Open open) => open.IdleTimeOut.HasValue && open.IdleTimeOut.Value != 0U ? open.IdleTimeOut.Value : uint.MaxValue;

    public static ushort RemoteChannel(this Begin begin) => begin.RemoteChannel.HasValue ? begin.RemoteChannel.Value : (ushort) 0;

    public static uint HandleMax(this Begin begin) => begin.HandleMax.HasValue ? begin.HandleMax.Value : uint.MaxValue;

    public static uint OutgoingWindow(this Begin begin) => begin.OutgoingWindow.HasValue ? begin.OutgoingWindow.Value : uint.MaxValue;

    public static uint IncomingWindow(this Begin begin) => begin.IncomingWindow.HasValue ? begin.IncomingWindow.Value : uint.MaxValue;

    public static bool IsReceiver(this Attach attach) => attach.Role.Value;

    public static bool IncompleteUnsettled(this Attach attach) => attach.IncompleteUnsettled.HasValue && attach.IncompleteUnsettled.Value;

    public static ulong MaxMessageSize(this Attach attach) => attach.MaxMessageSize.HasValue ? attach.MaxMessageSize.Value : ulong.MaxValue;

    public static Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Terminus Terminus(
      this Attach attach)
    {
      return attach.IsReceiver() ? (attach.Source is Source source ? new Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Terminus(source) : (Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Terminus) null) : (attach.Target is Target target ? new Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Terminus(target) : (Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Terminus) null);
    }

    public static Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Address Address(
      this Attach attach)
    {
      return attach.IsReceiver() ? ((Source) attach.Source).Address : ((Target) attach.Target).Address;
    }

    public static bool Dynamic(this Attach attach) => attach.IsReceiver() ? ((Source) attach.Source).Dynamic() : ((Target) attach.Target).Dynamic();

    public static SettleMode SettleType(this Attach attach)
    {
      byte? nullable;
      int num1;
      if (!attach.SndSettleMode.HasValue)
      {
        num1 = 2;
      }
      else
      {
        nullable = attach.SndSettleMode;
        num1 = (int) nullable.Value;
      }
      nullable = attach.RcvSettleMode;
      int num2;
      if (!nullable.HasValue)
      {
        num2 = 0;
      }
      else
      {
        nullable = attach.RcvSettleMode;
        num2 = (int) nullable.Value;
      }
      ReceiverSettleMode receiverSettleMode = (ReceiverSettleMode) num2;
      if (num1 == 1)
        return SettleMode.SettleOnSend;
      return receiverSettleMode == ReceiverSettleMode.First ? SettleMode.SettleOnReceive : SettleMode.SettleOnDispose;
    }

    public static Attach Clone(this Attach attach) => new Attach()
    {
      LinkName = attach.LinkName,
      Role = attach.Role,
      SndSettleMode = attach.SndSettleMode,
      RcvSettleMode = attach.RcvSettleMode,
      Source = attach.Source,
      Target = attach.Target,
      Unsettled = attach.Unsettled,
      IncompleteUnsettled = attach.IncompleteUnsettled,
      InitialDeliveryCount = attach.InitialDeliveryCount,
      MaxMessageSize = attach.MaxMessageSize,
      OfferedCapabilities = attach.OfferedCapabilities,
      DesiredCapabilities = attach.DesiredCapabilities,
      Properties = attach.Properties
    };

    public static Source Clone(this Source source) => new Source()
    {
      Address = source.Address,
      Durable = source.Durable,
      ExpiryPolicy = source.ExpiryPolicy,
      Timeout = source.Timeout,
      DistributionMode = source.DistributionMode,
      FilterSet = source.FilterSet,
      DefaultOutcome = source.DefaultOutcome,
      Outcomes = source.Outcomes,
      Capabilities = source.Capabilities
    };

    public static Target Clone(this Target target) => new Target()
    {
      Address = target.Address,
      Durable = target.Durable,
      ExpiryPolicy = target.ExpiryPolicy,
      Timeout = target.Timeout,
      Capabilities = target.Capabilities
    };

    public static bool Settled(this Transfer transfer) => transfer.Settled.HasValue && transfer.Settled.Value;

    public static bool More(this Transfer transfer) => transfer.More.HasValue && transfer.More.Value;

    public static bool Resume(this Transfer transfer) => transfer.Resume.HasValue && transfer.Resume.Value;

    public static bool Aborted(this Transfer transfer) => transfer.Aborted.HasValue && transfer.Aborted.Value;

    public static bool Batchable(this Transfer transfer) => transfer.Batchable.HasValue && transfer.Batchable.Value;

    public static bool Settled(this Disposition disposition) => disposition.Settled.HasValue && disposition.Settled.Value;

    public static bool Batchable(this Disposition disposition) => disposition.Batchable.HasValue && disposition.Batchable.Value;

    public static uint LinkCredit(this Flow flow) => !flow.LinkCredit.HasValue ? uint.MaxValue : flow.LinkCredit.Value;

    public static bool Echo(this Flow flow) => flow.Echo.HasValue && flow.Echo.Value;

    public static bool Closed(this Detach detach) => detach.Closed.HasValue && detach.Closed.Value;

    public static bool Durable(this Header header) => header.Durable.HasValue && header.Durable.Value;

    public static byte Priority(this Header header) => header.Priority.HasValue ? header.Priority.Value : (byte) 0;

    public static uint Ttl(this Header header) => header.Ttl.HasValue ? header.Ttl.Value : 0U;

    public static bool FirstAcquirer(this Header header) => header.FirstAcquirer.HasValue && header.FirstAcquirer.Value;

    public static uint DeliveryCount(this Header header) => header.DeliveryCount.HasValue ? header.DeliveryCount.Value : 0U;

    public static DateTime AbsoluteExpiryTime(this Properties properties) => properties.AbsoluteExpiryTime.HasValue ? properties.AbsoluteExpiryTime.Value : new DateTime();

    public static DateTime CreationTime(this Properties properties) => properties.CreationTime.HasValue ? properties.CreationTime.Value : new DateTime();

    public static SequenceNumber GroupSequence(this Properties properties) => (SequenceNumber) (!properties.GroupSequence.HasValue ? 0U : properties.GroupSequence.Value);

    public static string TrackingId(this Properties properties)
    {
      if (properties.CorrelationId != null)
        return properties.CorrelationId.ToString();
      if (properties.MessageId != null)
        return properties.MessageId.ToString();
      Guid guid = Guid.NewGuid();
      properties.MessageId = (MessageId) guid;
      return properties.MessageId.ToString();
    }

    public static bool Dynamic(this Source source) => source.Dynamic.HasValue && source.Dynamic.Value;

    public static bool Dynamic(this Target target) => target.Dynamic.HasValue && target.Dynamic.Value;

    public static bool Durable(this Source source) => source.Durable.HasValue && source.Durable.Value == 0U;

    public static bool Durable(this Target target) => target.Durable.HasValue && target.Durable.Value == 0U;

    public static void AddProperty(this Attach attach, AmqpSymbol symbol, object value)
    {
      if (attach.Properties == null)
        attach.Properties = new Fields();
      attach.Properties.Add(symbol, value);
    }

    public static void UpsertProperty(this Attach attach, AmqpSymbol symbol, object value)
    {
      if (attach.Properties == null)
        attach.Properties = new Fields();
      attach.Properties[symbol] = value;
    }

    public static void AddProperty(this Open open, AmqpSymbol symbol, object value)
    {
      if (open.Properties == null)
        open.Properties = new Fields();
      open.Properties.Add(symbol, value);
    }

    internal static IEnumerable<ByteBuffer> GetClones(this IEnumerable<ByteBuffer> buffers)
    {
      if (buffers == null)
        return (IEnumerable<ByteBuffer>) null;
      List<ByteBuffer> clones = new List<ByteBuffer>();
      foreach (ByteBuffer buffer in buffers)
        clones.Add((ByteBuffer) buffer.Clone());
      return (IEnumerable<ByteBuffer>) clones;
    }

    internal static ByteBuffer[] ToByteBufferArray(this ArraySegment<byte>[] bufferList)
    {
      if (bufferList == null)
        return (ByteBuffer[]) null;
      ByteBuffer[] byteBufferArray = new ByteBuffer[bufferList.Length];
      for (int index = 0; index < bufferList.Length; ++index)
        byteBufferArray[index] = new ByteBuffer(bufferList[index]);
      return byteBufferArray;
    }
  }
}
