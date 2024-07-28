// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.UserReportedConcernBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class UserReportedConcernBinder : ObjectBinder<UserReportedConcern>
  {
    protected SqlColumnBinder ReportedItemIdColumn = new SqlColumnBinder("ReportedItemId");
    protected SqlColumnBinder UserIdColumn = new SqlColumnBinder("UserId");
    protected SqlColumnBinder ConcernCategoryColumn = new SqlColumnBinder("ConcernCategory");
    protected SqlColumnBinder ConcernTextColumn = new SqlColumnBinder("ConcernText");
    protected SqlColumnBinder SubmittedDateColumn = new SqlColumnBinder("SubmittedDate");

    protected override UserReportedConcern Bind() => new UserReportedConcern()
    {
      ReviewId = this.ReportedItemIdColumn.GetInt64((IDataReader) this.Reader),
      UserId = this.UserIdColumn.GetGuid((IDataReader) this.Reader),
      Category = (ConcernCategory) this.ConcernCategoryColumn.GetInt32((IDataReader) this.Reader),
      ConcernText = this.ConcernTextColumn.GetString((IDataReader) this.Reader, true),
      SubmittedDate = this.SubmittedDateColumn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
