// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent13
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobQueueComponent13 : JobQueueComponent12
  {
    private static readonly SqlMetaData[] typ_JobDefinitionUpdateTable3 = new SqlMetaData[5]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IgnoreDormancy", SqlDbType.Bit),
      new SqlMetaData("PriorityClass", SqlDbType.Int),
      new SqlMetaData("QueueAsDormant", SqlDbType.Bit),
      new SqlMetaData("OverrideQueueTime", SqlDbType.DateTime)
    };

    protected override SqlParameter BindJobDefinitionUpdateTable(
      string parameterName,
      IEnumerable<TeamFoundationJobDefinition> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationJobDefinition>();
      System.Func<TeamFoundationJobDefinition, SqlDataRecord> selector = (System.Func<TeamFoundationJobDefinition, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(JobQueueComponent13.typ_JobDefinitionUpdateTable3);
        sqlDataRecord.SetGuid(0, row.JobId);
        sqlDataRecord.SetBoolean(1, row.IgnoreDormancy);
        if (row.PriorityClass == JobPriorityClass.None)
        {
          TeamFoundationTracingService.TraceRaw(2032129, TraceLevel.Error, "Job", nameof (JobQueueComponent13), "jobDefinition.PriorityClass is None");
          row.PriorityClass = JobPriorityClass.Normal;
        }
        sqlDataRecord.SetInt32(2, (int) row.PriorityClass);
        sqlDataRecord.SetBoolean(3, row.QueueAsDormant);
        DateTime minValue = (DateTime) SqlDateTime.MinValue;
        sqlDataRecord.SetDateTime(4, row.OverrideQueueTime < minValue ? minValue : row.OverrideQueueTime);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_JobDefinitionUpdateTable3", rows.Select<TeamFoundationJobDefinition, SqlDataRecord>(selector));
    }
  }
}
