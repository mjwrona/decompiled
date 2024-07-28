// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FieldUsageRecordBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class FieldUsageRecordBinder : ObjectBinder<FieldUsageRecord>
  {
    protected SqlColumnBinder FieldIdColumn;
    protected SqlColumnBinder ObjectIdColumn;
    protected SqlColumnBinder DirectObjectIdColumn;
    protected SqlColumnBinder OftenQueriedColumn;
    protected SqlColumnBinder SupportsTextQueryColumn;
    protected SqlColumnBinder CoreColumn;

    public FieldUsageRecordBinder()
    {
      this.FieldIdColumn = new SqlColumnBinder("FldID");
      this.ObjectIdColumn = new SqlColumnBinder("ObjectID");
      this.DirectObjectIdColumn = new SqlColumnBinder("DirectObjectID");
      this.OftenQueriedColumn = new SqlColumnBinder("fOftenQueried");
      this.SupportsTextQueryColumn = new SqlColumnBinder("fSupportsTextQuery");
      this.CoreColumn = new SqlColumnBinder("fCore");
    }

    protected override FieldUsageRecord Bind() => new FieldUsageRecord()
    {
      FieldId = this.FieldIdColumn.GetInt32((IDataReader) this.Reader),
      ObjectId = this.ObjectIdColumn.GetInt32((IDataReader) this.Reader),
      DirectObjectId = this.DirectObjectIdColumn.GetInt32((IDataReader) this.Reader),
      OftenQueried = this.OftenQueriedColumn.GetBoolean((IDataReader) this.Reader),
      SupportsTextQuery = this.SupportsTextQueryColumn.ColumnExists((IDataReader) this.Reader) && this.SupportsTextQueryColumn.GetBoolean((IDataReader) this.Reader),
      Core = this.CoreColumn.GetBoolean((IDataReader) this.Reader)
    };
  }
}
