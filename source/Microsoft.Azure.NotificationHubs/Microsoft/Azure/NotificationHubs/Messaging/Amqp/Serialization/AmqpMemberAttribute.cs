// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization.AmqpMemberAttribute
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
  internal sealed class AmqpMemberAttribute : Attribute
  {
    private int? order;
    private bool? mandatory;

    public string Name { get; set; }

    public int Order
    {
      get => !this.order.HasValue ? 0 : this.order.Value;
      set => this.order = new int?(value);
    }

    public bool Mandatory
    {
      get => this.mandatory.HasValue && this.mandatory.Value;
      set => this.mandatory = new bool?(value);
    }

    internal int? InternalOrder => this.order;

    internal bool? InternalMandatory => this.mandatory;
  }
}
