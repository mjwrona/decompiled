// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamFieldRowBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TeamFieldRowBinder : ObjectBinder<TeamFieldRow>
  {
    protected SqlColumnBinder TeamFieldValueColumn = new SqlColumnBinder("TeamFieldValue");
    protected SqlColumnBinder IncludeChildrenColumn = new SqlColumnBinder("IncludeChildren");
    protected SqlColumnBinder OrderColumn = new SqlColumnBinder("Order");

    protected override TeamFieldRow Bind() => new TeamFieldRow()
    {
      TeamFieldValue = this.TeamFieldValueColumn.GetString((IDataReader) this.Reader, false),
      IncludeChildren = this.IncludeChildrenColumn.GetBoolean((IDataReader) this.Reader),
      Order = this.OrderColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
