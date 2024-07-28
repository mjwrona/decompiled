// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notification.RecipientMetadata
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notification
{
  [DataContract]
  public class RecipientMetadata
  {
    [DataMember]
    public Guid RecipientId { get; set; }

    [DataMember]
    public long IdOfMostRecentNotification { get; set; }

    [DataMember]
    public long IdOfMostRecentSeenNotification { get; set; }

    [DataMember]
    public int NumberOfUnseenNotifications { get; set; }

    public RecipientMetadata()
    {
    }

    public RecipientMetadata(
      Guid recipientId,
      long highestUnseenNotificationId,
      int unseenNotificationCount)
    {
      this.RecipientId = recipientId;
      this.IdOfMostRecentNotification = highestUnseenNotificationId;
      this.NumberOfUnseenNotifications = unseenNotificationCount;
    }
  }
}
