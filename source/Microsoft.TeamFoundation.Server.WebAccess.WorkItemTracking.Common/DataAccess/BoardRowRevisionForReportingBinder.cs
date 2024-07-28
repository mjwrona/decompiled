// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardRowRevisionForReportingBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardRowRevisionForReportingBinder : ObjectBinder<BoardRowRevisionForReportingTable>
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder BoardIdColumn = new SqlColumnBinder("BoardId");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder OrderColumn = new SqlColumnBinder("Order");
    private SqlColumnBinder RevisedDateColumn = new SqlColumnBinder("RevisedDate");
    private SqlColumnBinder DeletedColumn = new SqlColumnBinder("Deleted");
    private SqlColumnBinder WatermarkColumn = new SqlColumnBinder("Watermark");
    private SqlColumnBinder ChangedDateColumn = new SqlColumnBinder("ChangedDate");
    private SqlColumnBinder BoardExtensionIdColumn = new SqlColumnBinder("BoardExtensionId");
    private SqlColumnBinder BoardCategoryReferenceNameColumn = new SqlColumnBinder("BoardCategoryReferenceName");
    private SqlColumnBinder TeamIdColumn = new SqlColumnBinder("TeamId");

    protected override BoardRowRevisionForReportingTable Bind()
    {
      BoardRowRevisionForReportingTable forReportingTable = new BoardRowRevisionForReportingTable();
      forReportingTable.Id = new Guid?(this.IdColumn.GetGuid((IDataReader) this.Reader));
      forReportingTable.BoardId = new Guid?(this.BoardIdColumn.GetGuid((IDataReader) this.Reader));
      forReportingTable.Name = this.NameColumn.GetString((IDataReader) this.Reader, true);
      forReportingTable.Order = this.OrderColumn.GetInt32((IDataReader) this.Reader);
      forReportingTable.RevisedDate = new DateTime?(this.RevisedDateColumn.GetDateTime((IDataReader) this.Reader));
      forReportingTable.Deleted = this.DeletedColumn.GetBoolean((IDataReader) this.Reader);
      forReportingTable.Watermark = this.WatermarkColumn.GetInt32((IDataReader) this.Reader);
      forReportingTable.ChangedDate = this.ChangedDateColumn.GetDateTime((IDataReader) this.Reader);
      forReportingTable.BoardExtensionId = this.BoardExtensionIdColumn.GetGuid((IDataReader) this.Reader);
      forReportingTable.BoardCategoryReferenceName = this.BoardCategoryReferenceNameColumn.GetString((IDataReader) this.Reader, true);
      forReportingTable.TeamId = this.TeamIdColumn.GetGuid((IDataReader) this.Reader);
      return forReportingTable;
    }
  }
}
