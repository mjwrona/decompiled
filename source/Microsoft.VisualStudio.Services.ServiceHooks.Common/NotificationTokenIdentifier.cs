// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.NotificationTokenIdentifier
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public class NotificationTokenIdentifier
  {
    public Guid SubscriptionId;
    public Guid SubscriberId;
    public Guid ClientAppId;

    public string LookupKey
    {
      get => string.Join("/", (object) this.SubscriberId, (object) this.ClientAppId);
      set
      {
        if (value == null)
        {
          this.SubscriberId = Guid.Empty;
          this.ClientAppId = Guid.Empty;
        }
        else
        {
          string[] strArray = value.Split('/');
          this.SubscriberId = Guid.Parse(strArray[0]);
          this.ClientAppId = Guid.Parse(strArray[1]);
        }
      }
    }
  }
}
