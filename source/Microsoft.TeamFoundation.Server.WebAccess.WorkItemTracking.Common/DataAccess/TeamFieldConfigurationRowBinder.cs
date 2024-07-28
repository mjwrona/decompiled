// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TeamFieldConfigurationRowBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TeamFieldConfigurationRowBinder : ObjectBinder<TeamFieldRow>
  {
    private SqlColumnBinder TeamIdColumn = new SqlColumnBinder("TeamId");
    private SqlColumnBinder TeamFieldValueColumn = new SqlColumnBinder("TeamFieldValue");
    private SqlColumnBinder IncludeChildrenColumn = new SqlColumnBinder("IncludeChildren");

    protected override TeamFieldRow Bind() => new TeamFieldRow()
    {
      TeamId = this.TeamIdColumn.GetGuid((IDataReader) this.Reader),
      TeamFieldValue = this.TeamFieldValueColumn.GetString((IDataReader) this.Reader, false),
      IncludeChildren = this.IncludeChildrenColumn.GetBoolean((IDataReader) this.Reader)
    };

    protected virtual TeamFieldRow Bind(IDataReader reader) => new TeamFieldRow()
    {
      TeamId = this.TeamIdColumn.GetGuid(reader),
      TeamFieldValue = this.TeamFieldValueColumn.GetString(reader, false),
      IncludeChildren = this.IncludeChildrenColumn.GetBoolean(reader)
    };

    public IEnumerable<TeamFieldRow> BindAll(IDataReader reader)
    {
      while (reader.Read())
        yield return this.Bind(reader);
    }
  }
}
