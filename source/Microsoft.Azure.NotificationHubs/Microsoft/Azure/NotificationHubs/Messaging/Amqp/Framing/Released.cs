// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Released
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Released : Outcome
  {
    public static readonly string Name = "amqp:released:list";
    public static readonly ulong Code = 38;
    private const int Fields = 0;

    public Released()
      : base((AmqpSymbol) Released.Name, Released.Code)
    {
    }

    protected override int FieldCount => 0;

    public override string ToString() => "released()";

    protected override void OnEncode(ByteBuffer buffer)
    {
    }

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
    }

    protected override int OnValueSize() => 0;
  }
}
