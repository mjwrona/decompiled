// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.IterationBinder
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
  internal class IterationBinder : ObjectBinder<Iteration>
  {
    private SqlColumnBinder ReviewIdColumn = new SqlColumnBinder("ReviewId");
    private SqlColumnBinder IterationIdColumn = new SqlColumnBinder("IterationId");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder AuthorColumn = new SqlColumnBinder("Author");
    private SqlColumnBinder CreatedDateColumn = new SqlColumnBinder("CreatedDate");
    private SqlColumnBinder UpdatedDateColumn = new SqlColumnBinder("UpdatedDate");
    private SqlColumnBinder IsUnpublishedColumn = new SqlColumnBinder("IsUnpublished");

    protected override Iteration Bind() => new Iteration()
    {
      Id = new int?(this.IterationIdColumn.GetInt32((IDataReader) this.Reader)),
      ReviewId = this.ReviewIdColumn.GetInt32((IDataReader) this.Reader),
      Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true),
      Author = new IdentityRef()
      {
        Id = this.AuthorColumn.GetGuid((IDataReader) this.Reader, false).ToString()
      },
      CreatedDate = new DateTime?(this.CreatedDateColumn.GetDateTime((IDataReader) this.Reader)),
      UpdatedDate = new DateTime?(this.UpdatedDateColumn.GetDateTime((IDataReader) this.Reader)),
      IsUnpublished = this.IsUnpublishedColumn.GetBoolean((IDataReader) this.Reader)
    };
  }
}
