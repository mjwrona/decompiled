// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.MetaTaskDefinitionHistoryComponent4
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class MetaTaskDefinitionHistoryComponent4 : MetaTaskDefinitionHistoryComponent3
  {
    public override List<MetaTaskDefinitionRevisionData> GetTaskGroupHistory(
      Guid projectId,
      Guid definitionId)
    {
      this.TraceEnter(0, nameof (GetTaskGroupHistory));
      this.PrepareStoredProcedure("Task.prc_GetMetaTaskDefinitionHistory", projectId);
      this.BindGuid("@definitionId", definitionId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<MetaTaskDefinitionRevisionData>((ObjectBinder<MetaTaskDefinitionRevisionData>) new MetaTaskDefinitionRevisionDataBinder2());
        this.TraceLeave(0, nameof (GetTaskGroupHistory));
        return resultCollection.GetCurrent<MetaTaskDefinitionRevisionData>().Items;
      }
    }

    public override MetaTaskDefinitionRevisionData GetTaskGroupRevision(
      Guid projectId,
      Guid definitionId,
      int definitionRevision)
    {
      this.TraceEnter(0, nameof (GetTaskGroupRevision));
      this.PrepareStoredProcedure("Task.prc_GetMetaTaskDefinitionRevision", projectId);
      this.BindGuid("@definitionId", definitionId);
      this.BindInt("@definitionRevision", definitionRevision);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<MetaTaskDefinitionRevisionData>((ObjectBinder<MetaTaskDefinitionRevisionData>) new MetaTaskDefinitionRevisionDataBinder2());
        this.TraceLeave(0, nameof (GetTaskGroupRevision));
        return resultCollection.GetCurrent<MetaTaskDefinitionRevisionData>().FirstOrDefault<MetaTaskDefinitionRevisionData>();
      }
    }

    public override void AddTaskGroupRevision(
      Guid projectId,
      Guid definitionId,
      Guid changedById,
      int revision,
      int majorVersion,
      int fileId,
      string comment,
      AuditAction changeType)
    {
      this.TraceEnter(0, nameof (AddTaskGroupRevision));
      this.PrepareStoredProcedure("Task.prc_AddMetaTaskDefinitionRevision", projectId);
      this.BindGuid("@definitionId", definitionId);
      this.BindInt("@definitionRevision", revision);
      this.BindInt("@majorVersion", majorVersion);
      this.BindGuid("@changedBy", changedById);
      this.BindInt("@changeType", (int) changeType);
      this.BindInt("@fileId", fileId);
      this.BindString("@comment", comment, 2048, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (AddTaskGroupRevision));
    }
  }
}
