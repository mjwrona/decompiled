// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamCapacityDaysOffRangeRowBinder2
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TeamCapacityDaysOffRangeRowBinder2 : ObjectBinder<TeamCapacityDaysOffRangeRow2>
  {
    private SqlColumnBinder TeamIdColumn = new SqlColumnBinder("TeamId");
    private SqlColumnBinder IterationIdColumn = new SqlColumnBinder("IterationId");
    private SqlColumnBinder TeamMemberIdColumn = new SqlColumnBinder("TeamMemberId");
    private SqlColumnBinder StartTimeColumn = new SqlColumnBinder("StartTime");
    private SqlColumnBinder EndTimeColumn = new SqlColumnBinder("EndTime");

    protected override TeamCapacityDaysOffRangeRow2 Bind()
    {
      TeamCapacityDaysOffRangeRow2 daysOffRangeRow2 = new TeamCapacityDaysOffRangeRow2();
      daysOffRangeRow2.TeamId = this.TeamIdColumn.GetGuid((IDataReader) this.Reader);
      daysOffRangeRow2.IterationId = this.IterationIdColumn.GetGuid((IDataReader) this.Reader);
      daysOffRangeRow2.TeamMemberId = this.TeamMemberIdColumn.GetGuid((IDataReader) this.Reader);
      daysOffRangeRow2.StartTime = this.StartTimeColumn.GetDateTime((IDataReader) this.Reader);
      daysOffRangeRow2.EndTime = this.EndTimeColumn.GetDateTime((IDataReader) this.Reader);
      return daysOffRangeRow2;
    }
  }
}
