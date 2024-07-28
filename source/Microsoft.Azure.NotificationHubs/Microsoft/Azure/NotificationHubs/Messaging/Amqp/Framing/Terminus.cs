// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Terminus
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Terminus
  {
    private Source source;
    private Target target;

    public Terminus(Source source) => this.source = source;

    public Terminus(Target target) => this.target = target;

    public Address Address => this.source == null ? this.target.Address : this.source.Address;

    public TerminusDurability Durable => this.source != null ? (this.source.Durable.HasValue ? (TerminusDurability) this.source.Durable.Value : TerminusDurability.None) : (this.target.Durable.HasValue ? (TerminusDurability) this.target.Durable.Value : TerminusDurability.None);

    public AmqpSymbol ExpiryPolicy => this.source == null ? this.target.ExpiryPolicy : this.source.ExpiryPolicy;

    public uint? Timeout => this.source == null ? this.target.Timeout : this.source.Timeout;

    public bool? Dynamic => this.source == null ? this.target.Dynamic : this.source.Dynamic;

    public AmqpMap DynamicNodeProperties => (AmqpMap) (this.source != null ? (RestrictedMap<AmqpSymbol>) this.source.DynamicNodeProperties : (RestrictedMap<AmqpSymbol>) this.target.DynamicNodeProperties);

    public Multiple<AmqpSymbol> Capabilities => this.source == null ? this.target.Capabilities : this.source.Capabilities;
  }
}
