// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskDefinitionComponent9
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskDefinitionComponent9 : TaskDefinitionComponent8
  {
    public override List<TaskDefinitionData> GetTaskDefinitions(
      Guid? taskId,
      TaskVersion version,
      bool allVersions)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetTaskDefinitions)))
      {
        this.PrepareStoredProcedure("Task.prc_GetTaskDefinitions");
        if (taskId.HasValue)
        {
          this.BindGuid("@taskId", taskId.Value);
          if (version != (TaskVersion) null)
          {
            this.BindInt("@majorVersion", version.Major);
            this.BindInt("@minorVersion", version.Minor);
            this.BindInt("@patchVersion", version.Patch);
          }
        }
        this.BindBoolean("@allVersions", allVersions);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskDefinitionData>((ObjectBinder<TaskDefinitionData>) new TaskDefinitionDataBinder3());
          return resultCollection.GetCurrent<TaskDefinitionData>().Items;
        }
      }
    }
  }
}
