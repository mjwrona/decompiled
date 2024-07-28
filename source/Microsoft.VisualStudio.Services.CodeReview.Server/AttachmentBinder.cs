// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.AttachmentBinder
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class AttachmentBinder : ObjectBinder<Attachment>
  {
    private SqlColumnBinder ReviewIdColumn = new SqlColumnBinder("ReviewId");
    private SqlColumnBinder AttachmentIdColumn = new SqlColumnBinder("AttachmentId");
    private SqlColumnBinder DisplayNameColumn = new SqlColumnBinder("DisplayName");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder AuthorColumn = new SqlColumnBinder("Author");
    private SqlColumnBinder CreatedDateColumn = new SqlColumnBinder("CreatedDate");
    private SqlColumnBinder ContentHashColumn = new SqlColumnBinder("ContentHash");

    protected override Attachment Bind() => new Attachment()
    {
      Id = this.AttachmentIdColumn.GetInt32((IDataReader) this.Reader),
      ReviewId = this.ReviewIdColumn.GetInt32((IDataReader) this.Reader),
      DisplayName = this.DisplayNameColumn.GetString((IDataReader) this.Reader, false),
      Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true),
      Author = new IdentityRef()
      {
        Id = this.AuthorColumn.GetGuid((IDataReader) this.Reader, false).ToString()
      },
      CreatedDate = new DateTime?(this.CreatedDateColumn.GetDateTime((IDataReader) this.Reader)),
      ContentHash = ReviewFileContentExtensions.ToSha1HashString(this.ContentHashColumn.GetBytes((IDataReader) this.Reader, false))
    };
  }
}
