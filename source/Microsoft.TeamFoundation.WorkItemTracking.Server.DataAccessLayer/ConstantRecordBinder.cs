// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ConstantRecordBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ConstantRecordBinder : ObjectBinder<ConstantRecord>
  {
    private SqlColumnBinder index = new SqlColumnBinder("Order");
    private SqlColumnBinder constID = new SqlColumnBinder("ConstID");
    private SqlColumnBinder displayValue = new SqlColumnBinder("DisplayPart");
    private SqlColumnBinder accountName = new SqlColumnBinder("String");
    private SqlColumnBinder sid = new SqlColumnBinder("SID");
    private SqlColumnBinder teamFoundationId = new SqlColumnBinder("TeamFoundationId");

    protected override ConstantRecord Bind()
    {
      ConstantRecord constantRecord = new ConstantRecord();
      constantRecord.Index = this.index.GetInt32((IDataReader) this.Reader);
      constantRecord.ConstantId = this.constID.GetInt32((IDataReader) this.Reader);
      constantRecord.DisplayValue = this.displayValue.GetString((IDataReader) this.Reader, false);
      constantRecord.Sid = this.sid.GetString((IDataReader) this.Reader, true);
      if (!string.IsNullOrEmpty(constantRecord.Sid))
      {
        constantRecord.AccountName = this.accountName.GetString((IDataReader) this.Reader, false);
        constantRecord.TeamFoundationId = this.teamFoundationId.GetGuid((IDataReader) this.Reader);
      }
      return constantRecord;
    }
  }
}
