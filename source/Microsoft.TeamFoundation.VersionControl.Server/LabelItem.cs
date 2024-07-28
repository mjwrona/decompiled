// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LabelItem
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal struct LabelItem
  {
    public readonly int ItemId;
    public readonly string ServerItem;
    public readonly int VersionFrom;
    public readonly bool FromChildLabel;

    public LabelItem(int itemId, string serverItem, int versionFrom, bool fromChildLabel)
    {
      this.ItemId = itemId;
      this.ServerItem = serverItem;
      this.VersionFrom = versionFrom;
      this.FromChildLabel = fromChildLabel;
    }
  }
}
