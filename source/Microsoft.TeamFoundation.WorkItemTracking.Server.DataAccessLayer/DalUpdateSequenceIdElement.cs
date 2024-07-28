// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalUpdateSequenceIdElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalUpdateSequenceIdElement : DalSqlElement
  {
    public void JoinBatch(
      ElementGroup group,
      EventType batchType,
      int seqId,
      DateTime syncTime,
      bool isTenantBackedWithAadInGroupSupport = false)
    {
      this.m_elementGroup = group;
      this.m_outputs = 0;
      this.m_index = group.AddElementToGroup(this.m_outputs);
      string sql = "UpdateSequenceId1";
      this.SqlBatch.RequestContext.Trace(900410, TraceLevel.Verbose, "Sync", "ProcessIdentityEventMessage", "JoinBatch :  updateSequenceId stored proc : {0} ", (object) sql);
      this.AppendSql(" exec dbo.[");
      this.AppendSql(sql);
      this.AppendSql("] ");
      this.AppendPartitionIdVariable();
      this.AppendSql(this.SqlBatch.AddParameterInt(seqId));
      this.AppendSql(",");
      this.AppendSql("'");
      this.AppendSql(syncTime.ToString("yyyy-MM-ddTHH:mm:ss.fff", (IFormatProvider) DateTimeFormatInfo.InvariantInfo));
      this.AppendSql("',");
      this.AppendSql(this.SqlBatch.AddParameterInt(Convert.ToInt32(Enum.Parse(typeof (EventType), batchType.ToString()), (IFormatProvider) CultureInfo.InvariantCulture)));
      if (this.Version >= 28 && this.SqlBatch.RequestContext.ServiceHost.HostType == TeamFoundationHostType.ProjectCollection)
      {
        this.AppendSql(",");
        SqlBatchBuilder sqlBatch1 = this.SqlBatch;
        Guid instanceId = this.SqlBatch.RequestContext.ServiceHost.InstanceId;
        string parameterValue1 = instanceId.ToString();
        this.AppendSql(sqlBatch1.AddParameterNVarChar(parameterValue1));
        this.AppendSql(",");
        SqlBatchBuilder sqlBatch2 = this.SqlBatch;
        instanceId = this.SqlBatch.RequestContext.ServiceHost.ParentServiceHost.InstanceId;
        string parameterValue2 = instanceId.ToString();
        this.AppendSql(sqlBatch2.AddParameterNVarChar(parameterValue2));
      }
      if (this.Version >= 29)
      {
        this.AppendSql(",");
        this.AppendSql(this.SqlBatch.AddParameterInt(isTenantBackedWithAadInGroupSupport ? 1 : 0));
      }
      this.AppendSql(Environment.NewLine);
    }
  }
}
