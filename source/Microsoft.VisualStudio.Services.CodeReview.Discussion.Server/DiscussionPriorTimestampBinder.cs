// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.DiscussionPriorTimestampBinder
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  public class DiscussionPriorTimestampBinder : ObjectBinder<Tuple<int, DateTime>>
  {
    private SqlColumnBinder DiscussionIdColumn = new SqlColumnBinder("DiscussionId");
    private SqlColumnBinder priorLastUpdatedDate = new SqlColumnBinder("PriorLastUpdatedDate");

    protected override Tuple<int, DateTime> Bind() => new Tuple<int, DateTime>(this.DiscussionIdColumn.GetInt32((IDataReader) this.Reader, int.MinValue, int.MinValue), this.priorLastUpdatedDate.GetDateTime((IDataReader) this.Reader));
  }
}
