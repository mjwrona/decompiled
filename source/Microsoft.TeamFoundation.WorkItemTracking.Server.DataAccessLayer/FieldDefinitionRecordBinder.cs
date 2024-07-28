// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FieldDefinitionRecordBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class FieldDefinitionRecordBinder : ObjectBinder<FieldDefinitionRecord>
  {
    protected SqlColumnBinder IdColumn;
    protected SqlColumnBinder TypeColumn = new SqlColumnBinder("Type");
    protected SqlColumnBinder ParentIdColumn;
    protected SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    protected SqlColumnBinder ReferenceNameColumn = new SqlColumnBinder("ReferenceName");
    protected SqlColumnBinder EditableColumn;
    protected SqlColumnBinder SemiEditableColumn;
    protected SqlColumnBinder ReportingTypeColumn = new SqlColumnBinder("ReportingType");
    protected SqlColumnBinder ReportingFormulaColumn = new SqlColumnBinder("ReportingFormula");
    protected SqlColumnBinder ReportingNameColumn = new SqlColumnBinder("ReportingName");
    protected SqlColumnBinder ReportingReferenceNameColumn = new SqlColumnBinder("ReportingReferenceName");
    protected SqlColumnBinder ReportableColumn;
    protected SqlColumnBinder SupportsTextQueryColumn;
    protected SqlColumnBinder IsIdentityFieldColumn = new SqlColumnBinder("IsIdentityField");
    protected SqlColumnBinder IsHistoryEnabledColumn = new SqlColumnBinder("IsHistoryEnabled");

    public FieldDefinitionRecordBinder()
    {
      this.IdColumn = new SqlColumnBinder("FldID");
      this.ParentIdColumn = new SqlColumnBinder("ParentFldID");
      this.EditableColumn = new SqlColumnBinder("fEditable");
      this.SemiEditableColumn = new SqlColumnBinder("fSemiEditable");
      this.ReportableColumn = new SqlColumnBinder("fReportingEnabled");
      this.SupportsTextQueryColumn = new SqlColumnBinder("fSupportsTextQuery");
    }

    protected override FieldDefinitionRecord Bind() => new FieldDefinitionRecord()
    {
      Id = this.IdColumn.GetInt32((IDataReader) this.Reader),
      Name = this.NameColumn.GetString((IDataReader) this.Reader, false),
      ReferenceName = this.ReferenceNameColumn.GetString((IDataReader) this.Reader, false),
      DBType = this.TypeColumn.GetInt32((IDataReader) this.Reader),
      IsReportable = this.ReportableColumn.GetBoolean((IDataReader) this.Reader),
      Editable = this.EditableColumn.GetBoolean((IDataReader) this.Reader),
      SemiEditable = this.SemiEditableColumn.GetBoolean((IDataReader) this.Reader),
      ReportingName = this.ReportingNameColumn.GetString((IDataReader) this.Reader, true),
      ReportingReferenceName = this.ReportingReferenceNameColumn.GetString((IDataReader) this.Reader, true),
      SupportsTextQuery = this.SupportsTextQueryColumn.ColumnExists((IDataReader) this.Reader) && this.SupportsTextQueryColumn.GetBoolean((IDataReader) this.Reader),
      IsIdentity = this.IsIdentityFieldColumn.ColumnExists((IDataReader) this.Reader) && this.IsIdentityFieldColumn.GetBoolean((IDataReader) this.Reader),
      IsHistoryEnabled = !this.IsHistoryEnabledColumn.ColumnExists((IDataReader) this.Reader) || this.IsHistoryEnabledColumn.GetBoolean((IDataReader) this.Reader)
    };
  }
}
