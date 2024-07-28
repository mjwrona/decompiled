// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.BranchObjectOwnershipBinder15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class BranchObjectOwnershipBinder15 : BranchObjectOwnershipBinder
  {
    public BranchObjectOwnershipBinder15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override BranchObjectOwnership Bind()
    {
      BranchObjectOwnership branchObjectOwnership = new BranchObjectOwnership()
      {
        RootItem = new ItemIdentifier()
      };
      branchObjectOwnership.RootItem.ItemPathPair = this.GetItemPathPair(this.fullPath.GetServerItem(this.Reader, false));
      branchObjectOwnership.RootItem.DeletionId = this.deletionId.GetInt32((IDataReader) this.Reader);
      branchObjectOwnership.RootItem.Version = (VersionSpec) new ChangesetVersionSpec(this.version.GetInt32((IDataReader) this.Reader));
      branchObjectOwnership.RootItem.ChangeType = VersionControlSqlResourceComponent.GetChangeType((int) this.command.GetInt16((IDataReader) this.Reader));
      branchObjectOwnership.VersionedItemCount = this.itemCount.GetInt32((IDataReader) this.Reader);
      return branchObjectOwnership;
    }
  }
}
