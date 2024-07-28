// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ResourceLockComponent3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class ResourceLockComponent3 : ResourceLockComponent2
  {
    protected static SqlMetaData[] typ_ResourceLockRequestTableV2 = new SqlMetaData[14]
    {
      new SqlMetaData("RequestId", SqlDbType.BigInt),
      new SqlMetaData("ResourceId", SqlDbType.NVarChar, 400L),
      new SqlMetaData("ResourceType", SqlDbType.NVarChar, 64L),
      new SqlMetaData("PlanId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("NodeName", SqlDbType.NVarChar, 400L),
      new SqlMetaData("NodeAttempt", SqlDbType.Int),
      new SqlMetaData("ProjectId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("CheckRunId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("QueueTime", SqlDbType.DateTime2),
      new SqlMetaData("AssignTime", SqlDbType.DateTime2),
      new SqlMetaData("FinishTime", SqlDbType.DateTime2),
      new SqlMetaData("LockType", SqlDbType.TinyInt)
    };

    protected override ObjectBinder<ResourceLockRequest> CreateResourceLockRequestBinder() => (ObjectBinder<ResourceLockRequest>) new ResourceLockRequestBinder2();

    protected override SqlParameter BindResourceLockTable(
      string parameterName,
      IEnumerable<ResourceLockRequest> rows)
    {
      return this.BindTable(parameterName, "Task.typ_ResourceLockRequestTableV2", (rows ?? Enumerable.Empty<ResourceLockRequest>()).Select<ResourceLockRequest, SqlDataRecord>(new System.Func<ResourceLockRequest, SqlDataRecord>(((ResourceLockComponent) this).ConvertLockRequestToSqlDataRecord)));
    }

    protected override SqlDataRecord ConvertLockRequestToSqlDataRecord(ResourceLockRequest row)
    {
      SqlDataRecord record = new SqlDataRecord(ResourceLockComponent3.typ_ResourceLockRequestTableV2);
      record.SetInt64(0, row.RequestId);
      record.SetString(1, row.ResourceId, BindStringBehavior.Unchanged);
      record.SetString(2, row.ResourceType, BindStringBehavior.Unchanged);
      record.SetGuid(3, row.PlanId);
      record.SetString(4, row.NodeName, BindStringBehavior.Unchanged);
      record.SetInt32(5, row.NodeAttempt);
      record.SetGuid(6, row.ProjectId);
      record.SetGuid(7, row.CheckRunId);
      record.SetInt32(8, row.DefinitionId);
      record.SetByte(9, (byte) row.Status);
      record.SetDateTime(10, row.QueueTime);
      record.SetNullableDateTime(11, row.AssignTime);
      record.SetNullableDateTime(12, row.FinishTime);
      record.SetByte(13, (byte) row.LockType);
      return record;
    }
  }
}
