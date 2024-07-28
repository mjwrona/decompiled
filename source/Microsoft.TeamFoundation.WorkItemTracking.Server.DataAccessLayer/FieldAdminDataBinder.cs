// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FieldAdminDataBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class FieldAdminDataBinder : WorkItemTrackingObjectBinder<FieldAdminData>
  {
    private SqlColumnBinder FldId = new SqlColumnBinder(nameof (FldId));
    private SqlColumnBinder Type = new SqlColumnBinder(nameof (Type));
    private SqlColumnBinder ParentFldID = new SqlColumnBinder(nameof (ParentFldID));
    private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
    private SqlColumnBinder ReferenceName = new SqlColumnBinder(nameof (ReferenceName));
    private SqlColumnBinder fEditable = new SqlColumnBinder(nameof (fEditable));
    private SqlColumnBinder fSemiEditable = new SqlColumnBinder(nameof (fSemiEditable));
    private SqlColumnBinder ReportingType = new SqlColumnBinder(nameof (ReportingType));
    private SqlColumnBinder ReportingFormula = new SqlColumnBinder(nameof (ReportingFormula));
    private SqlColumnBinder ReportingName = new SqlColumnBinder(nameof (ReportingName));
    private SqlColumnBinder ReportingReferenceName = new SqlColumnBinder(nameof (ReportingReferenceName));
    private SqlColumnBinder fReportingEnabled = new SqlColumnBinder(nameof (fReportingEnabled));
    private SqlColumnBinder Cachestamp = new SqlColumnBinder("CacheStamp");
    private SqlColumnBinder fDeleted = new SqlColumnBinder(nameof (fDeleted));
    private SqlColumnBinder fSupportsTextQuery = new SqlColumnBinder(nameof (fSupportsTextQuery));

    public override FieldAdminData Bind(IDataReader reader) => new FieldAdminData()
    {
      FldId = this.FldId.GetInt32(reader),
      ParentFldId = this.ParentFldID.GetInt32(reader),
      Type = this.Type.GetInt32(reader),
      Name = this.Name.GetString(reader, true),
      ReferenceName = this.ReferenceName.GetString(reader, true),
      FEditable = this.fEditable.GetBoolean(reader),
      FSemiEditable = this.fSemiEditable.GetBoolean(reader),
      ReportingType = this.ReportingType.GetInt32(reader),
      ReportingFormula = this.ReportingFormula.GetInt32(reader),
      ReportingName = this.ReportingName.GetString(reader, true),
      ReportingReferenceName = this.ReportingReferenceName.GetString(reader, true),
      FReportingEnabled = this.fReportingEnabled.GetBoolean(reader),
      CacheStamp = this.Cachestamp.GetNullableInt64(reader),
      FDeleted = this.fDeleted.GetBoolean(reader),
      FSupportsTextQuery = this.fSupportsTextQuery.GetBoolean(reader)
    };
  }
}
