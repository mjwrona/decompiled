// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationArtifact
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public struct NotificationArtifact
  {
    public string Id;
    public string Type;
    public string Tool;
    public string Name;
    public string FriendlyName;

    public void CopyFrom(NotificationArtifact that)
    {
      this.Id = that.Id;
      this.Type = that.Type;
      this.Tool = that.Tool;
      this.Name = that.Name;
      this.FriendlyName = that.FriendlyName;
    }
  }
}
