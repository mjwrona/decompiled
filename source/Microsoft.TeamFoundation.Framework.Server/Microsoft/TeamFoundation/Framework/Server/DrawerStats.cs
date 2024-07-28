// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DrawerStats
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DrawerStats
  {
    public DrawerStats(string drawerName, int itemsCount)
    {
      this.DrawerName = drawerName;
      this.ItemsCount = itemsCount;
    }

    public override string ToString() => string.Format("DrawerItemsCount {0}: {1}", (object) this.DrawerName, (object) this.ItemsCount);

    public string DrawerName { get; }

    public int ItemsCount { get; }
  }
}
