// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.BranchObjectOwnership
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class BranchObjectOwnership
  {
    private ItemIdentifier m_rootItem;
    private int m_count;

    public ItemIdentifier RootItem
    {
      get => this.m_rootItem;
      set => this.m_rootItem = value;
    }

    public int VersionedItemCount
    {
      get => this.m_count;
      set => this.m_count = value;
    }
  }
}
