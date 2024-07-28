// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ChangeEntryBinder
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class ChangeEntryBinder : ObjectBinder<ChangeEntry>
  {
    private SqlColumnBinder ReviewIdColumn = new SqlColumnBinder("ReviewId");
    private SqlColumnBinder IterationIdColumn = new SqlColumnBinder("IterationId");
    private SqlColumnBinder ChangeIdColumn = new SqlColumnBinder("ChangeId");
    private SqlColumnBinder BasePathColumn = new SqlColumnBinder("BasePath");
    private SqlColumnBinder BaseContentId = new SqlColumnBinder(nameof (BaseContentId));
    private SqlColumnBinder BaseContentHashColumn = new SqlColumnBinder("BaseContentHash");
    private SqlColumnBinder BaseFileServiceFileIdColumn = new SqlColumnBinder("BaseFileId");
    private SqlColumnBinder BaseFlags = new SqlColumnBinder(nameof (BaseFlags));
    private SqlColumnBinder ModifiedPathColumn = new SqlColumnBinder("ModifiedPath");
    private SqlColumnBinder ModifiedContentId = new SqlColumnBinder(nameof (ModifiedContentId));
    private SqlColumnBinder ModifiedContentHashColumn = new SqlColumnBinder("ModifiedContentHash");
    private SqlColumnBinder ModifiedFileServiceFileIdColumn = new SqlColumnBinder("ModifiedFileId");
    private SqlColumnBinder ModifiedFlags = new SqlColumnBinder(nameof (ModifiedFlags));
    private SqlColumnBinder ChangeTypeColumn = new SqlColumnBinder("ChangeType");
    private SqlColumnBinder ExtendedChangeTypeColumn = new SqlColumnBinder("ExtendedChangeType");
    private SqlColumnBinder ChangeTrackingIdColumn = new SqlColumnBinder("ChangeTrackingId");
    private SqlColumnBinder TotalChangesCountColumn = new SqlColumnBinder("TotalChangesCount");

    protected override ChangeEntry Bind()
    {
      ChangeEntry entry = new ChangeEntry()
      {
        IterationId = new int?(this.IterationIdColumn.GetInt32((IDataReader) this.Reader)),
        ChangeId = new int?(this.ChangeIdColumn.GetInt32((IDataReader) this.Reader, -1, -1)),
        Type = (ChangeType) this.ChangeTypeColumn.GetInt32((IDataReader) this.Reader),
        ExtendedChangeType = this.ExtendedChangeTypeColumn.GetString((IDataReader) this.Reader, true),
        ChangeTrackingId = this.ChangeTrackingIdColumn.GetInt32((IDataReader) this.Reader, 0, 0),
        TotalChangesCount = this.TotalChangesCountColumn.GetInt32((IDataReader) this.Reader, 0, 0)
      };
      this.PopulateFilePaths(entry, this.ReviewIdColumn.GetInt32((IDataReader) this.Reader, -1), this.BasePathColumn.GetString((IDataReader) this.Reader, true), this.BaseContentId.GetGuid((IDataReader) this.Reader, true), this.BaseContentHashColumn.GetBytes((IDataReader) this.Reader, false), this.BaseFileServiceFileIdColumn.GetInt32((IDataReader) this.Reader, -1), this.BaseFlags.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0), this.ModifiedPathColumn.GetString((IDataReader) this.Reader, true), this.ModifiedContentId.GetGuid((IDataReader) this.Reader, true), this.ModifiedContentHashColumn.GetBytes((IDataReader) this.Reader, false), this.ModifiedFileServiceFileIdColumn.GetInt32((IDataReader) this.Reader, -1), this.ModifiedFlags.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0));
      return entry;
    }

    private void PopulateFilePaths(
      ChangeEntry entry,
      int reviewId,
      string basePath,
      Guid baseContentId,
      byte[] baseContentHash,
      int baseFileId,
      byte baseFlags,
      string modifiedPath,
      Guid modifiedContentId,
      byte[] modifiedContentHash,
      int modifiedFileId,
      byte modifiedFlags)
    {
      if (!string.IsNullOrEmpty(basePath))
      {
        ChangeEntry changeEntry = entry;
        ChangeEntryFileInfo changeEntryFileInfo = new ChangeEntryFileInfo();
        changeEntryFileInfo.ReviewId = reviewId;
        changeEntryFileInfo.Path = basePath;
        changeEntryFileInfo.ContentId = baseContentId;
        changeEntryFileInfo.SHA1Hash = ReviewFileContentExtensions.ToSha1HashString(baseContentHash);
        changeEntryFileInfo.FileServiceFileId = baseFileId;
        changeEntryFileInfo.Flags = baseFlags;
        changeEntry.Base = changeEntryFileInfo;
      }
      if (string.IsNullOrEmpty(modifiedPath))
        return;
      ChangeEntry changeEntry1 = entry;
      ChangeEntryFileInfo changeEntryFileInfo1 = new ChangeEntryFileInfo();
      changeEntryFileInfo1.ReviewId = reviewId;
      changeEntryFileInfo1.Path = modifiedPath;
      changeEntryFileInfo1.ContentId = modifiedContentId;
      changeEntryFileInfo1.SHA1Hash = ReviewFileContentExtensions.ToSha1HashString(modifiedContentHash);
      changeEntryFileInfo1.FileServiceFileId = modifiedFileId;
      changeEntryFileInfo1.Flags = modifiedFlags;
      changeEntry1.Modified = changeEntryFileInfo1;
    }
  }
}
