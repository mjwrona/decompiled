// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeExtensionBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class WorkItemTypeExtensionBinder : ObjectBinder<WorkItemTypeletRecord>
  {
    protected SqlColumnBinder m_columnId = new SqlColumnBinder("Id");
    protected SqlColumnBinder m_columnProjectId = new SqlColumnBinder("ProjectId");
    protected SqlColumnBinder m_columnOwnerId = new SqlColumnBinder("OwnerId");
    protected SqlColumnBinder m_columnName = new SqlColumnBinder("Name");
    protected SqlColumnBinder m_columnDescription = new SqlColumnBinder("Description");
    protected SqlColumnBinder m_columnMarkerField = new SqlColumnBinder("MarkerField");
    protected SqlColumnBinder m_columnPredicate = new SqlColumnBinder("Predicate");
    protected SqlColumnBinder m_columnRules = new SqlColumnBinder("Rules");
    protected SqlColumnBinder m_columnForm = new SqlColumnBinder("Form");
    protected SqlColumnBinder m_columnLastChangeDate = new SqlColumnBinder("LastChangeDate");
    protected SqlColumnBinder m_columnReconciliationStatus = new SqlColumnBinder("ReconciliationStatus");
    protected SqlColumnBinder m_columnReconciliationWatermark = new SqlColumnBinder("ReconciliationWatermark");
    protected SqlColumnBinder m_columnReconciliationMessage = new SqlColumnBinder("ReconciliationMessage");

    protected IDataReader Reader => this.BaseReader;

    protected override WorkItemTypeletRecord Bind()
    {
      IDataReader reader = this.Reader;
      return new WorkItemTypeletRecord()
      {
        Id = this.m_columnId.GetGuid(reader),
        ProjectId = this.m_columnProjectId.GetGuid(reader, true),
        OwnerId = this.m_columnOwnerId.GetGuid(reader, true),
        Name = this.m_columnName.GetString(reader, false),
        Description = this.m_columnDescription.GetString(reader, true),
        MarkerField = this.m_columnMarkerField.GetInt32(reader),
        Predicate = this.m_columnPredicate.GetString(reader, true),
        Rules = this.m_columnRules.GetString(reader, true),
        Form = this.m_columnForm.GetString(reader, true),
        LastChangeDate = this.m_columnLastChangeDate.GetDateTime(reader, DateTime.MinValue),
        ReconciliationWatermark = this.m_columnReconciliationWatermark.GetGuid(reader, true),
        ReconciliationStatus = this.m_columnReconciliationStatus.GetInt32(reader),
        ReconciliationMessage = this.m_columnReconciliationMessage.GetString(reader, true)
      };
    }
  }
}
