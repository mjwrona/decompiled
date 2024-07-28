// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ReviewBinder
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
  internal class ReviewBinder : ObjectBinder<Review>
  {
    private CodeReviewComponent m_component;
    private SqlColumnBinder ReviewIdColumn = new SqlColumnBinder("ReviewId");
    private SqlColumnBinder TitleColumn = new SqlColumnBinder("Title");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder StatusColumn = new SqlColumnBinder("Status");
    private SqlColumnBinder AuthorColumn = new SqlColumnBinder("Author");
    private SqlColumnBinder CreatedDateColumn = new SqlColumnBinder("CreatedDate");
    private SqlColumnBinder UpdatedDateColumn = new SqlColumnBinder("UpdatedDate");
    private SqlColumnBinder CompletedDateColumn = new SqlColumnBinder("CompletedDate");
    private SqlColumnBinder SourceArtifactIdColumn = new SqlColumnBinder("SourceArtifactId");
    private SqlColumnBinder IsDeletedColumn = new SqlColumnBinder("IsDeleted");
    private SqlColumnBinder CustomStorageColumn = new SqlColumnBinder("CustomStorage");
    private SqlColumnBinder DiffFileIdColumn = new SqlColumnBinder("DiffFileId");
    private SqlColumnBinder DataspaceIdColumn = new SqlColumnBinder("DataspaceId");

    public ReviewBinder(CodeReviewComponent component) => this.m_component = component;

    protected override Review Bind()
    {
      int? nullable1 = new int?();
      if (this.DiffFileIdColumn.ColumnExists((IDataReader) this.Reader))
        nullable1 = new int?(this.DiffFileIdColumn.GetInt32((IDataReader) this.Reader, 0));
      Review review1 = new Review()
      {
        Id = this.ReviewIdColumn.GetInt32((IDataReader) this.Reader),
        Title = this.TitleColumn.GetString((IDataReader) this.Reader, true),
        Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true),
        Status = new ReviewStatus?((ReviewStatus) this.StatusColumn.GetByte((IDataReader) this.Reader)),
        Author = new IdentityRef()
        {
          Id = this.AuthorColumn.GetGuid((IDataReader) this.Reader, false).ToString()
        },
        CreatedDate = new DateTime?(this.CreatedDateColumn.GetDateTime((IDataReader) this.Reader)),
        UpdatedDate = new DateTime?(this.UpdatedDateColumn.GetDateTime((IDataReader) this.Reader)),
        CompletedDate = new DateTime?(this.CompletedDateColumn.GetDateTime((IDataReader) this.Reader)),
        SourceArtifactId = this.SourceArtifactIdColumn.GetString((IDataReader) this.Reader, (string) null),
        IsDeleted = this.IsDeletedColumn.GetBoolean((IDataReader) this.Reader, false),
        CustomStorage = this.CustomStorageColumn.GetBoolean((IDataReader) this.Reader, false),
        DiffFileId = nullable1,
        ProjectId = this.GetDataspaceIdentifier(this.DataspaceIdColumn.GetInt32((IDataReader) this.Reader, 0, 0))
      };
      DateTime? nullable2 = review1.UpdatedDate;
      if (nullable2.Value == new DateTime())
      {
        Review review2 = review1;
        nullable2 = new DateTime?();
        DateTime? nullable3 = nullable2;
        review2.UpdatedDate = nullable3;
      }
      nullable2 = review1.CompletedDate;
      if (nullable2.Value == new DateTime())
      {
        Review review3 = review1;
        nullable2 = new DateTime?();
        DateTime? nullable4 = nullable2;
        review3.CompletedDate = nullable4;
      }
      return review1;
    }

    private Guid GetDataspaceIdentifier(int dataspaceId) => dataspaceId == 0 ? Guid.Empty : this.m_component.GetDataspaceIdentifier(dataspaceId);
  }
}
