// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FieldDefinitionRecordBinder2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class FieldDefinitionRecordBinder2 : FieldDefinitionRecordBinder
  {
    public FieldDefinitionRecordBinder2()
    {
      this.IdColumn = new SqlColumnBinder("FieldId");
      this.ParentIdColumn = new SqlColumnBinder("ParentFieldId");
      this.EditableColumn = new SqlColumnBinder("IsEditable");
      this.SemiEditableColumn = new SqlColumnBinder("IsSemiEditable");
      this.ReportableColumn = new SqlColumnBinder("IsReportingEnabled");
      this.SupportsTextQueryColumn = new SqlColumnBinder("SupportsTextQuery");
    }
  }
}
