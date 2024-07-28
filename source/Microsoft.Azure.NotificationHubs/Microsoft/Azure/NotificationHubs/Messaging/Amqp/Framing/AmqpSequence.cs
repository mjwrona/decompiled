// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.AmqpSequence
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System.Collections;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class AmqpSequence : DescribedList
  {
    public static readonly string Name = "amqp:amqp-sequence:list";
    public static readonly ulong Code = 118;
    private IList innerList;

    public AmqpSequence()
      : this((IList) new System.Collections.Generic.List<object>())
    {
    }

    public AmqpSequence(IList innerList)
      : base((AmqpSymbol) AmqpSequence.Name, AmqpSequence.Code)
    {
      this.innerList = innerList;
    }

    public IList List => this.innerList;

    protected override int FieldCount => this.innerList.Count;

    public override string ToString() => "sequence()";

    protected override int OnValueSize() => ListEncoding.GetValueSize(this.innerList);

    protected override void OnEncode(ByteBuffer buffer)
    {
      foreach (object inner in (IEnumerable) this.innerList)
        AmqpEncoding.EncodeObject(inner, buffer);
    }

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
      for (int index = 0; index < count; ++index)
        this.innerList.Add(AmqpEncoding.DecodeObject(buffer));
    }
  }
}
