// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamIdIterationIdWatermarkPairTypeRowBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TeamIdIterationIdWatermarkPairTypeRowBinder : 
    ObjectBinder<TeamIdIterationIdWatermarkPairTypeRow>
  {
    private SqlColumnBinder TeamIdColumn = new SqlColumnBinder("TeamId");
    private SqlColumnBinder IterationIdColumn = new SqlColumnBinder("IterationId");
    private SqlColumnBinder WatermarkColumn = new SqlColumnBinder("Watermark");

    protected override TeamIdIterationIdWatermarkPairTypeRow Bind() => new TeamIdIterationIdWatermarkPairTypeRow()
    {
      TeamId = this.TeamIdColumn.GetGuid((IDataReader) this.Reader),
      IterationId = this.IterationIdColumn.GetGuid((IDataReader) this.Reader),
      Watermark = this.WatermarkColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
