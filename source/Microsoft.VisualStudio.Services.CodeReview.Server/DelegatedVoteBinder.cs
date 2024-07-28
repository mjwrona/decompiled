// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.DelegatedVoteBinder
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class DelegatedVoteBinder : ObjectBinder<DelegatedVote>
  {
    private SqlColumnBinder ReviewIdColumn = new SqlColumnBinder("ReviewId");
    private SqlColumnBinder ReviewerIdColumn = new SqlColumnBinder("ReviewerId");
    private SqlColumnBinder VotedForIdColumn = new SqlColumnBinder("VotedForId");

    protected override DelegatedVote Bind() => new DelegatedVote(this.ReviewIdColumn.GetInt32((IDataReader) this.Reader, -1, -1), this.ReviewerIdColumn.GetGuid((IDataReader) this.Reader, false), this.VotedForIdColumn.GetGuid((IDataReader) this.Reader, false));
  }
}
