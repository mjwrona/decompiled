// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.NotificationEventBacklogStatus
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public class NotificationEventBacklogStatus
  {
    public NotificationEventBacklogStatus()
    {
      this.NotificationBacklogStatus = new List<Microsoft.VisualStudio.Services.Notifications.WebApi.NotificationBacklogStatus>();
      this.EventBacklogStatus = new List<Microsoft.VisualStudio.Services.Notifications.WebApi.EventBacklogStatus>();
    }

    [DataMember(EmitDefaultValue = false)]
    public List<Microsoft.VisualStudio.Services.Notifications.WebApi.NotificationBacklogStatus> NotificationBacklogStatus { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<Microsoft.VisualStudio.Services.Notifications.WebApi.EventBacklogStatus> EventBacklogStatus { get; set; }
  }
}
