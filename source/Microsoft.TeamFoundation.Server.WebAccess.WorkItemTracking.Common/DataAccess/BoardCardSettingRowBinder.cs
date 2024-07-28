// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardCardSettingRowBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardCardSettingRowBinder : ObjectBinder<BoardCardSettingRow>
  {
    protected SqlColumnBinder TypeColumn = new SqlColumnBinder("Type");
    protected SqlColumnBinder FieldColumn = new SqlColumnBinder("Field");
    protected SqlColumnBinder PropertyColumn = new SqlColumnBinder("Property");
    protected SqlColumnBinder ValueColumn = new SqlColumnBinder("Value");

    protected override BoardCardSettingRow Bind() => new BoardCardSettingRow()
    {
      Type = this.TypeColumn.GetString((IDataReader) this.Reader, false),
      Field = this.FieldColumn.GetString((IDataReader) this.Reader, false),
      Property = this.PropertyColumn.GetString((IDataReader) this.Reader, false),
      Value = this.ValueColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
