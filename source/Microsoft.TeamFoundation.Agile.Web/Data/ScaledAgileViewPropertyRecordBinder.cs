// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.ScaledAgileViewPropertyRecordBinder
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Agile.Web.Data
{
  internal class ScaledAgileViewPropertyRecordBinder : ObjectBinder<ScaledAgileViewPropertyRecord>
  {
    private SqlColumnBinder ViewIdColumn = new SqlColumnBinder("ViewId");
    private SqlColumnBinder TeamIdColumn = new SqlColumnBinder("TeamId");
    private SqlColumnBinder WorkItemTypeNameColumn = new SqlColumnBinder("WorkItemTypeName");

    protected override ScaledAgileViewPropertyRecord Bind() => new ScaledAgileViewPropertyRecord()
    {
      ViewId = this.ViewIdColumn.GetGuid((IDataReader) this.Reader),
      TeamId = this.TeamIdColumn.GetGuid((IDataReader) this.Reader),
      WorkItemTypeName = this.WorkItemTypeNameColumn.GetString((IDataReader) this.Reader, false)
    };
  }
}
