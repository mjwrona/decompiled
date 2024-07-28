// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.WorkItemTypeRowBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class WorkItemTypeRowBinder : ObjectBinder<WorkItemTypeRow>
  {
    private SqlColumnBinder WorkItemTypeColumn = new SqlColumnBinder("WorkItemType");
    private SqlColumnBinder CategoryRefNameColumn = new SqlColumnBinder("CategoryRefName");
    private SqlColumnBinder PluralDisplayNameColumn = new SqlColumnBinder("PluralDisplayName");

    protected override WorkItemTypeRow Bind() => new WorkItemTypeRow()
    {
      WorkItemType = (WorkItemTypeEnum) this.WorkItemTypeColumn.GetByte((IDataReader) this.Reader),
      CategoryRefName = this.CategoryRefNameColumn.GetString((IDataReader) this.Reader, false),
      PluralDisplayName = this.PluralDisplayNameColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
