// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.StatusBinder
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
  internal class StatusBinder : ObjectBinder<Status>
  {
    private SqlColumnBinder ReviewIdColumn = new SqlColumnBinder("ReviewId");
    private SqlColumnBinder IterationIdColumn = new SqlColumnBinder("IterationId");
    private SqlColumnBinder StatusIdColumn = new SqlColumnBinder("StatusId");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder GenreColumn = new SqlColumnBinder("Genre");
    private SqlColumnBinder StateColumn = new SqlColumnBinder("State");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder TargetUrlColumn = new SqlColumnBinder("TargetUrl");
    private SqlColumnBinder CreatedDateColumn = new SqlColumnBinder("CreatedDate");
    private SqlColumnBinder UpdatedDateColumn = new SqlColumnBinder("UpdatedDate");
    private SqlColumnBinder AuthorColumn = new SqlColumnBinder("Author");
    private SqlColumnBinder PropertyIdColumn = new SqlColumnBinder("PropertyId");

    protected override Status Bind()
    {
      int? nullable1 = new int?(this.IterationIdColumn.GetInt32((IDataReader) this.Reader, -1));
      Status status1 = new Status();
      status1.Id = this.StatusIdColumn.GetInt32((IDataReader) this.Reader);
      status1.ReviewId = this.ReviewIdColumn.GetInt32((IDataReader) this.Reader);
      int? nullable2 = nullable1;
      int num = -1;
      status1.IterationId = nullable2.GetValueOrDefault() == num & nullable2.HasValue ? new int?() : nullable1;
      status1.Context = new StatusContext()
      {
        Name = this.NameColumn.GetString((IDataReader) this.Reader, false),
        Genre = this.GenreColumn.GetString((IDataReader) this.Reader, true)
      };
      status1.State = (MetaState) this.StateColumn.GetByte((IDataReader) this.Reader);
      status1.Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true);
      status1.TargetUrl = this.TargetUrlColumn.GetString((IDataReader) this.Reader, true);
      status1.CreatedDate = new DateTime?(this.CreatedDateColumn.GetDateTime((IDataReader) this.Reader));
      status1.UpdatedDate = new DateTime?(this.UpdatedDateColumn.GetDateTime((IDataReader) this.Reader));
      Status status2 = status1;
      if (this.AuthorColumn.ColumnExists((IDataReader) this.Reader) && !this.AuthorColumn.IsNull((IDataReader) this.Reader))
        status2.Author = new IdentityRef()
        {
          Id = this.AuthorColumn.GetGuid((IDataReader) this.Reader, false).ToString()
        };
      if (this.PropertyIdColumn.ColumnExists((IDataReader) this.Reader) && !this.PropertyIdColumn.IsNull((IDataReader) this.Reader))
        status2.PropertyId = new long?(this.PropertyIdColumn.GetInt64((IDataReader) this.Reader));
      return status2;
    }
  }
}
