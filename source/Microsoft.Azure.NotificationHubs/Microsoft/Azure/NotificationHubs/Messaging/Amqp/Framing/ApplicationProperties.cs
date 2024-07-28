// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.ApplicationProperties
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class ApplicationProperties : DescribedMap
  {
    public static readonly string Name = "amqp:application-properties:map";
    public static readonly ulong Code = 116;
    private PropertiesMap propMap;

    public ApplicationProperties()
      : base((AmqpSymbol) ApplicationProperties.Name, ApplicationProperties.Code)
    {
    }

    public PropertiesMap Map
    {
      get
      {
        if (this.propMap == null)
        {
          this.propMap = new PropertiesMap();
          this.propMap.SetMap(this.InnerMap);
        }
        return this.propMap;
      }
    }
  }
}
