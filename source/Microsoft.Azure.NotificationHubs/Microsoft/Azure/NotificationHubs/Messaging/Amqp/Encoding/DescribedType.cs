// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.DescribedType
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal class DescribedType
  {
    public DescribedType(object descriptor, object value)
    {
      this.Descriptor = descriptor;
      this.Value = value;
    }

    public object Descriptor { get; set; }

    public object Value { get; set; }

    public override string ToString() => this.Descriptor.ToString();
  }
}
