// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.PlanLogTableBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class PlanLogTableBinder : ObjectBinder<PlanLogTable>
  {
    private SqlColumnBinder m_storageKey = new SqlColumnBinder("StorageKey");
    private SqlColumnBinder m_tableName = new SqlColumnBinder("TableName");
    private SqlColumnBinder m_startedOn = new SqlColumnBinder("StartedOn");
    private SqlColumnBinder m_completedOn = new SqlColumnBinder("CompletedOn");
    private SqlColumnBinder m_expiryOn = new SqlColumnBinder("ExpiryOn");
    private SqlColumnBinder m_deletedOn = new SqlColumnBinder("DeletedOn");

    protected override PlanLogTable Bind() => new PlanLogTable()
    {
      StorageKey = this.m_storageKey.GetString((IDataReader) this.Reader, false),
      TableName = this.m_tableName.GetString((IDataReader) this.Reader, false),
      StartedOn = new DateTime?(this.m_startedOn.GetDateTime((IDataReader) this.Reader)),
      CompletedOn = this.m_completedOn.GetNullableDateTime((IDataReader) this.Reader),
      ExpiryOn = new DateTime?(this.m_expiryOn.GetDateTime((IDataReader) this.Reader)),
      DeletedOn = this.m_deletedOn.GetNullableDateTime((IDataReader) this.Reader)
    };
  }
}
