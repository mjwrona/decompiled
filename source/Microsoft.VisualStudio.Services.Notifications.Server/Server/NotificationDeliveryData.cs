// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationDeliveryData
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public struct NotificationDeliveryData
  {
    public string FromAddress;
    public NotificationArtifact Artifact;
    public List<EventScope> Scopes;
    public IdentityRef Initiator;
    public string Action;

    public void AddScopes(params object[] scopes)
    {
      if (this.Scopes == null)
        this.Scopes = new List<EventScope>();
      if (scopes == null || scopes.Length == 0)
        return;
      foreach (object scope in scopes)
      {
        if (scope is EventScope eventScope)
          this.Scopes.Add(eventScope);
      }
    }

    public void CopyFrom(NotificationDeliveryData that)
    {
      this.FromAddress = that.FromAddress;
      this.Artifact.CopyFrom(that.Artifact);
      this.Action = that.Action;
      this.Scopes = that.Scopes;
      this.Initiator = that.Initiator;
    }
  }
}
