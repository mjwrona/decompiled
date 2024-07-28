// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamCapacityDaysOffRangeRowBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TeamCapacityDaysOffRangeRowBinder : ObjectBinder<TeamCapacityDaysOffRangeRow>
  {
    private SqlColumnBinder TeamMemberIdColumn = new SqlColumnBinder("TeamMemberId");
    private SqlColumnBinder StartTimeColumn = new SqlColumnBinder("StartTime");
    private SqlColumnBinder EndTimeColumn = new SqlColumnBinder("EndTime");

    protected override TeamCapacityDaysOffRangeRow Bind() => new TeamCapacityDaysOffRangeRow()
    {
      TeamMemberId = this.TeamMemberIdColumn.GetGuid((IDataReader) this.Reader),
      StartTime = this.StartTimeColumn.GetDateTime((IDataReader) this.Reader),
      EndTime = this.EndTimeColumn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
