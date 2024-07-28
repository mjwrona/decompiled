// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.EntityDescription
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  [DataContract(Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  [KnownType(typeof (NotificationHubDescription))]
  public abstract class EntityDescription : IExtensibleDataObject
  {
    internal EntityDescription()
    {
    }

    public bool IsReadOnly { get; internal set; }

    public ExtensionDataObject ExtensionData { get; set; }

    internal virtual bool RequiresEncryption => false;

    internal virtual void OverrideEntityStatus(EntityStatus status)
    {
    }

    internal virtual void OverrideEntityAvailabilityStatus(EntityAvailabilityStatus status)
    {
    }

    internal virtual void UpdateForVersion(
      ApiVersion version,
      EntityDescription existingDescription = null)
    {
    }

    internal virtual bool IsValidForVersion(ApiVersion version) => true;

    protected void ThrowIfReadOnly()
    {
      if (this.IsReadOnly)
        throw new InvalidOperationException(SRClient.ObjectIsReadOnly);
    }
  }
}
