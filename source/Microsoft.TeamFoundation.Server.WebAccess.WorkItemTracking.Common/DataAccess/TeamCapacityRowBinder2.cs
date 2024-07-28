// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamCapacityRowBinder2
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TeamCapacityRowBinder2 : ObjectBinder<TeamCapacityRow2>
  {
    private SqlColumnBinder TeamIdColumn = new SqlColumnBinder("TeamId");
    private SqlColumnBinder IterationIdColumn = new SqlColumnBinder("IterationId");
    private SqlColumnBinder TeamMemberIdColumn = new SqlColumnBinder("TeamMemberId");
    private SqlColumnBinder CapacityColumn = new SqlColumnBinder("Capacity");
    private SqlColumnBinder ActivityColumn = new SqlColumnBinder("Activity");

    protected override TeamCapacityRow2 Bind()
    {
      TeamCapacityRow2 teamCapacityRow2 = new TeamCapacityRow2();
      teamCapacityRow2.TeamId = this.TeamIdColumn.GetGuid((IDataReader) this.Reader);
      teamCapacityRow2.IterationId = this.IterationIdColumn.GetGuid((IDataReader) this.Reader);
      teamCapacityRow2.TeamMemberId = this.TeamMemberIdColumn.GetGuid((IDataReader) this.Reader);
      teamCapacityRow2.Capacity = this.CapacityColumn.GetFloat((IDataReader) this.Reader, 0.0f);
      teamCapacityRow2.Activity = this.ActivityColumn.GetString((IDataReader) this.Reader, true);
      return teamCapacityRow2;
    }
  }
}
