// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.DataAccess.ApprovalComponent7
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Approval.Server.DataAccess
{
  internal class ApprovalComponent7 : ApprovalComponent6
  {
    protected static readonly SqlMetaData[] ApprovalConfigTableType2 = new SqlMetaData[5]
    {
      new SqlMetaData("ApprovalId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("StepId", SqlDbType.Int),
      new SqlMetaData("Comment", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("DeferredTo", SqlDbType.DateTime2)
    };

    protected override void BindApprovalUpdateParametersTable(
      IEnumerable<ApprovalUpdateParameters> updateParameters)
    {
      this.BindTable("@updateParameters", "PipelinePolicy.typ_ApprovalUpdateParametersTable2", updateParameters.Select<ApprovalUpdateParameters, SqlDataRecord>(new System.Func<ApprovalUpdateParameters, SqlDataRecord>(((ApprovalComponent) this).ConvertToApprovalConfigTableSqlDataRecord)));
    }

    protected override ApprovalStepBinder GetApprovalStepBinder() => (ApprovalStepBinder) new ApprovalStepBinder2();

    protected override SqlDataRecord ConvertToApprovalConfigTableSqlDataRecord(
      ApprovalUpdateParameters row)
    {
      SqlDataRecord record = new SqlDataRecord(ApprovalComponent7.ApprovalConfigTableType2);
      record.SetGuid(0, row.ApprovalId);
      record.SetNullableInt32(1, row.StepId);
      record.SetNullableString(2, row.Comment);
      record.SetByte(3, (byte) row.Status.Value);
      record.SetNullableDateTime(4, row.DeferredTo);
      return record;
    }
  }
}
