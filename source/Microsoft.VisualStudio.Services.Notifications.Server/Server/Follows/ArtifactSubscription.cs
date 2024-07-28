// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.Follows.ArtifactSubscription
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server.Follows
{
  public class ArtifactSubscription
  {
    public ArtifactSubscription()
    {
    }

    public ArtifactSubscription(int subscriptionId, string artifactId, string artifactType)
    {
      this.SubscriptionId = subscriptionId;
      this.ArtifactId = artifactId;
      this.ArtifactType = artifactType;
    }

    public int SubscriptionId { get; set; }

    public string ArtifactId { get; set; }

    public string ArtifactType { get; set; }

    public Guid SubscriberId { get; set; }

    public string Description { get; set; }
  }
}
