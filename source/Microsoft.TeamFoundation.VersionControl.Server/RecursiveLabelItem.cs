// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.RecursiveLabelItem
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal struct RecursiveLabelItem
  {
    public readonly bool Active;
    public readonly int VersionFrom;
    public readonly string ServerItem;

    public RecursiveLabelItem(bool active, string serverItem, int versionFrom)
    {
      this.Active = active;
      this.VersionFrom = versionFrom;
      this.ServerItem = serverItem;
    }
  }
}
