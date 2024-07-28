// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization.AmqpContractAttribute
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
  internal sealed class AmqpContractAttribute : Attribute
  {
    public AmqpContractAttribute()
    {
      this.Encoding = EncodingType.List;
      this.Code = -1L;
    }

    public string Name { get; set; }

    public long Code { get; set; }

    public EncodingType Encoding { get; set; }

    internal ulong? InternalCode => this.Code < 0L ? new ulong?() : new ulong?((ulong) this.Code);
  }
}
