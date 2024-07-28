// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.MetaTaskDefinitionHistoryComponent3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class MetaTaskDefinitionHistoryComponent3 : MetaTaskDefinitionHistoryComponent2
  {
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
      this.BindGuid("@changedBy", changedById);
      this.BindInt("@changeType", (int) changeType);
      this.BindInt("@fileId", fileId);
      this.BindString("@comment", comment, 2048, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (AddTaskGroupRevision));
    }
  }
}
