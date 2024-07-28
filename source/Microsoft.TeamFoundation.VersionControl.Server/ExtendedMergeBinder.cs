// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ExtendedMergeBinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ExtendedMergeBinder : VersionControlObjectBinder<ExtendedMerge>
  {
    protected SqlColumnBinder sourceVersionFrom = new SqlColumnBinder("SourceVersionFrom");
    protected SqlColumnBinder ownerIdFrom = new SqlColumnBinder("OwnerIdFrom");
    protected SqlColumnBinder creationDateFrom = new SqlColumnBinder("CreationDateFrom");
    protected SqlColumnBinder commentFrom = new SqlColumnBinder("CommentFrom");
    protected SqlColumnBinder committerIdFrom = new SqlColumnBinder("CommitterIdFrom");
    protected SqlColumnBinder targetVersion = new SqlColumnBinder("TargetVersion");
    protected SqlColumnBinder ownerIdTarget = new SqlColumnBinder("OwnerIdTarget");
    protected SqlColumnBinder creationDateTarget = new SqlColumnBinder("CreationDateTarget");
    protected SqlColumnBinder commentTarget = new SqlColumnBinder("CommentTarget");
    protected SqlColumnBinder committerIdTarget = new SqlColumnBinder("CommitterIdTarget");
    protected SqlColumnBinder sourceServerItem = new SqlColumnBinder("SourceServerItem");
    protected SqlColumnBinder sourceitemVersion = new SqlColumnBinder("SourceItemVersion");
    protected SqlColumnBinder sourceDeletionId = new SqlColumnBinder("SourceDeletionId");
    protected SqlColumnBinder sourceChangeType = new SqlColumnBinder("SourceChangeType");
    protected SqlColumnBinder sourceEncoding = new SqlColumnBinder("SourceEncoding");
    protected SqlColumnBinder sourceFileLength = new SqlColumnBinder("SourceFileLength");
    protected SqlColumnBinder sourceHashValue = new SqlColumnBinder("SourceHashValue");
    protected SqlColumnBinder sourceFileId = new SqlColumnBinder("SourceFileId");
    protected SqlColumnBinder sourceItemId = new SqlColumnBinder("SourceItemId");
    protected SqlColumnBinder sourceCreationDate = new SqlColumnBinder("SourceCreationDate");
    protected SqlColumnBinder targetServerItem = new SqlColumnBinder("TargetServerItem");
    protected SqlColumnBinder targetItemVersion = new SqlColumnBinder("TargetItemVersion");
    protected SqlColumnBinder targetDeletionId = new SqlColumnBinder("TargetDeletionId");
    protected SqlColumnBinder targetChangeType = new SqlColumnBinder("TargetChangeType");
    protected SqlColumnBinder versionedItemCount = new SqlColumnBinder("VersionedItemCount");

    public ExtendedMergeBinder()
    {
    }

    public ExtendedMergeBinder(VersionControlSqlResourceComponent component)
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
      change.Item.ItemPathPair = this.GetPreDataspaceItemPathPair(this.sourceServerItem.GetServerItem(this.Reader, false));
      change.Item.Encoding = this.sourceEncoding.GetInt32((IDataReader) this.Reader);
      change.Item.ItemType = change.Item.Encoding == -3 ? ItemType.Folder : ItemType.File;
      change.Item.FileLength = this.sourceFileLength.GetInt64((IDataReader) this.Reader, 0L);
      change.Item.HashValue = VersionControlObjectBinder<ExtendedMerge>.NormalizeHashValue(this.sourceHashValue.GetBytes((IDataReader) this.Reader, true));
      change.Item.fileId = this.sourceFileId.GetInt32((IDataReader) this.Reader, 0);
      change.Item.CheckinDate = this.sourceCreationDate.GetDateTime((IDataReader) this.Reader);
      extendedMerge.SourceItem = change;
      extendedMerge.TargetItem = new ItemIdentifier();
      extendedMerge.TargetItem.ItemPathPair = this.GetPreDataspaceItemPathPair(this.targetServerItem.GetServerItem(this.Reader, false));
      extendedMerge.TargetItem.Version = (VersionSpec) new ChangesetVersionSpec(this.targetItemVersion.GetInt32((IDataReader) this.Reader));
      extendedMerge.TargetItem.DeletionId = this.targetDeletionId.GetInt32((IDataReader) this.Reader);
      extendedMerge.TargetItem.ChangeType = VersionControlSqlResourceComponent.GetChangeType((int) this.targetChangeType.GetInt16((IDataReader) this.Reader));
      extendedMerge.VersionedItemCount = this.versionedItemCount.GetInt32((IDataReader) this.Reader);
      return extendedMerge;
    }
  }
}
