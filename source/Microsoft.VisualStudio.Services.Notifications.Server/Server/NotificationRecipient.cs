// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationRecipient
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class NotificationRecipient : ICloneable
  {
    public Guid Id { get; set; }

    [JsonIgnore]
    public Microsoft.VisualStudio.Services.Identity.Identity Identity { get; set; }

    public string Address { get; set; }

    public object Clone() => (object) new NotificationRecipient()
    {
      Id = this.Id,
      Identity = this.Identity?.Clone(),
      Address = this.Address
    };

    public override bool Equals(object obj)
    {
      bool flag = base.Equals(obj);
      NotificationRecipient notificationRecipient = obj as NotificationRecipient;
      if (!flag && notificationRecipient != null && this.Id.Equals(notificationRecipient.Id) && string.Equals(this.Address, notificationRecipient.Address))
        flag = true;
      return flag;
    }

    public override int GetHashCode() => this.Id.GetHashCode() + this.Address.SafeGetHashCode<string>();
  }
}
