// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ReviewFileContentInfoBinder
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class ReviewFileContentInfoBinder : ObjectBinder<ReviewFileContentInfo>
  {
    private SqlColumnBinder ReviewIdColumn = new SqlColumnBinder("ReviewId");
    private SqlColumnBinder ContentIdColumn = new SqlColumnBinder("ContentId");
    private SqlColumnBinder ContentHashColumn = new SqlColumnBinder("ContentHash");
    private SqlColumnBinder FileServiceFileIdColumn = new SqlColumnBinder("FileServiceFileId");
    private SqlColumnBinder NeedsCleanupColumn = new SqlColumnBinder("NeedsCleanup");
    private SqlColumnBinder FlagsColumn = new SqlColumnBinder("Flags");

    protected override ReviewFileContentInfo Bind() => new ReviewFileContentInfo()
    {
      ReviewId = this.ReviewIdColumn.GetInt32((IDataReader) this.Reader),
      ContentId = this.ContentIdColumn.GetGuid((IDataReader) this.Reader, false),
      SHA1Hash = ReviewFileContentExtensions.ToSha1HashString(this.ContentHashColumn.GetBytes((IDataReader) this.Reader, false)),
      FileServiceFileId = this.FileServiceFileIdColumn.GetInt32((IDataReader) this.Reader, 0),
      NeedsCleanup = this.NeedsCleanupColumn.GetBoolean((IDataReader) this.Reader),
      Flags = this.FlagsColumn.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0)
    };
  }
}
