// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ExtendedMergeBinder15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ExtendedMergeBinder15 : ExtendedMergeBinder
  {
    protected SqlColumnBinder sourceItemDataspaceId = new SqlColumnBinder("SourceItemDataspaceId");

    public ExtendedMergeBinder15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override ExtendedMerge Bind()
    {
      ExtendedMerge extendedMerge = new ExtendedMerge()
      {
        SourceChangeset = new ChangesetSummary()
      };
      extendedMerge.SourceChangeset.ChangesetId = this.sourceVersionFrom.GetInt32((IDataReader) this.Reader);
      extendedMerge.SourceChangeset.Comment = this.commentFrom.GetString((IDataReader) this.Reader, true);
      extendedMerge.SourceChangeset.committerId = this.committerIdFrom.GetGuid((IDataReader) this.Reader);
      extendedMerge.SourceChangeset.CreationDate = this.creationDateFrom.GetDateTime((IDataReader) this.Reader);
      extendedMerge.SourceChangeset.ownerId = this.ownerIdFrom.GetGuid((IDataReader) this.Reader);
      extendedMerge.TargetChangeset = new ChangesetSummary();
      extendedMerge.TargetChangeset.ChangesetId = this.targetVersion.GetInt32((IDataReader) this.Reader);
      extendedMerge.TargetChangeset.Comment = this.commentTarget.GetString((IDataReader) this.Reader, true);
      extendedMerge.TargetChangeset.committerId = this.committerIdTarget.GetGuid((IDataReader) this.Reader);
      extendedMerge.TargetChangeset.CreationDate = this.creationDateTarget.GetDateTime((IDataReader) this.Reader);
      extendedMerge.TargetChangeset.ownerId = this.ownerIdTarget.GetGuid((IDataReader) this.Reader);
      Change change = new Change()
      {
        Item = {
          ChangesetId = extendedMerge.SourceChangeset.ChangesetId
        },
        ChangeType = VersionControlSqlResourceComponent.GetChangeType((int) this.sourceChangeType.GetInt16((IDataReader) this.Reader))
      };
      change.Item.DeletionId = this.sourceDeletionId.GetInt32((IDataReader) this.Reader);
      change.Item.ItemId = this.sourceItemId.GetInt32((IDataReader) this.Reader);
      change.Item.ItemDataspaceId = this.GetDataspaceIdentifier(this.sourceItemDataspaceId.GetInt32((IDataReader) this.Reader));
      change.Item.ItemPathPair = this.GetItemPathPair(this.sourceServerItem.GetServerItem(this.Reader, false));
      change.Item.Encoding = this.sourceEncoding.GetInt32((IDataReader) this.Reader);
      change.Item.ItemType = change.Item.Encoding == -3 ? ItemType.Folder : ItemType.File;
      change.Item.FileLength = this.sourceFileLength.GetInt64((IDataReader) this.Reader, 0L);
      change.Item.HashValue = VersionControlObjectBinder<ExtendedMerge>.NormalizeHashValue(this.sourceHashValue.GetBytes((IDataReader) this.Reader, true));
      change.Item.fileId = this.sourceFileId.GetInt32((IDataReader) this.Reader, 0);
      change.Item.CheckinDate = this.sourceCreationDate.GetDateTime((IDataReader) this.Reader);
      extendedMerge.SourceItem = change;
      extendedMerge.TargetItem = new ItemIdentifier();
      extendedMerge.TargetItem.ItemPathPair = this.GetItemPathPair(this.targetServerItem.GetServerItem(this.Reader, false));
      extendedMerge.TargetItem.Version = (VersionSpec) new ChangesetVersionSpec(this.targetItemVersion.GetInt32((IDataReader) this.Reader));
      extendedMerge.TargetItem.DeletionId = this.targetDeletionId.GetInt32((IDataReader) this.Reader);
      extendedMerge.TargetItem.ChangeType = VersionControlSqlResourceComponent.GetChangeType((int) this.targetChangeType.GetInt16((IDataReader) this.Reader));
      extendedMerge.VersionedItemCount = this.versionedItemCount.GetInt32((IDataReader) this.Reader);
      return extendedMerge;
    }
  }
}
