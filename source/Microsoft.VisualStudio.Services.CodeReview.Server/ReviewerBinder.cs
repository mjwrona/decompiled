// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ReviewerBinder
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
  internal class ReviewerBinder : ObjectBinder<Reviewer>
  {
    private const int NullableOrMissingColumnValue = -1;
    private SqlColumnBinder ReviewIdColumn = new SqlColumnBinder("ReviewId");
    private SqlColumnBinder ReviewerIdColumn = new SqlColumnBinder("ReviewerId");
    private SqlColumnBinder ReviewerTypeColumn = new SqlColumnBinder("ReviewerType");
    private SqlColumnBinder ReviewerStateColumn = new SqlColumnBinder("ReviewerState");
    private SqlColumnBinder CreatedDateColumn = new SqlColumnBinder("CreatedDate");
    private SqlColumnBinder ModifiedDateColumn = new SqlColumnBinder("ModifiedDate");
    private SqlColumnBinder IterationIdColumn = new SqlColumnBinder("IterationId");

    protected override Reviewer Bind()
    {
      short int16 = this.ReviewerStateColumn.GetInt16((IDataReader) this.Reader);
      Reviewer reviewer1 = new Reviewer()
      {
        Identity = new IdentityRef()
        {
          Id = this.ReviewerIdColumn.GetGuid((IDataReader) this.Reader, false).ToString()
        },
        ReviewerStateId = new short?(ReviewerExtensions.GetDefaultState(int16).Value),
        Kind = new ReviewerKind?((ReviewerKind) this.ReviewerTypeColumn.GetInt16((IDataReader) this.Reader)),
        IterationId = new int?(this.IterationIdColumn.GetInt32((IDataReader) this.Reader, -1, -1)),
        CreatedDate = new DateTime?(this.CreatedDateColumn.GetDateTime((IDataReader) this.Reader)),
        ModifiedDate = new DateTime?(this.ModifiedDateColumn.GetDateTime((IDataReader) this.Reader)),
        ReviewId = this.ReviewIdColumn.GetInt32((IDataReader) this.Reader, -1, -1)
      };
      if (reviewer1.CreatedDate.Value == new DateTime())
        reviewer1.CreatedDate = new DateTime?();
      if (reviewer1.ModifiedDate.Value == new DateTime())
        reviewer1.ModifiedDate = new DateTime?();
      int? nullable1 = reviewer1.IterationId;
      int num = -1;
      if (nullable1.GetValueOrDefault() == num & nullable1.HasValue)
      {
        Reviewer reviewer2 = reviewer1;
        nullable1 = new int?();
        int? nullable2 = nullable1;
        reviewer2.IterationId = nullable2;
      }
      return reviewer1;
    }
  }
}
