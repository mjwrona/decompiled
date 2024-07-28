// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskDefinitionComponent6
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskDefinitionComponent6 : TaskDefinitionComponent5
  {
    public override async Task AddTaskDefinitionHistoryAsync(
      TaskDefinitionStatus taskDefinitionStatus,
      Guid? taskId = null,
      TaskVersion version = null,
      Dictionary<string, string> contributions = null)
    {
      TaskDefinitionComponent6 component = this;
      TaskSqlComponentBase.SqlMethodScope sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AddTaskDefinitionHistoryAsync));
      try
      {
        component.PrepareStoredProcedure("Task.prc_AddTaskDefinitionHistory");
        component.BindNullableGuid("@taskId", taskId);
        int parameterValue1 = 0;
        int parameterValue2 = 0;
        int parameterValue3 = 0;
        if (version != (TaskVersion) null)
        {
          parameterValue1 = version.Major;
          parameterValue2 = version.Minor;
          parameterValue3 = version.Patch;
        }
        component.BindInt("@majorVersion", parameterValue1);
        component.BindInt("@minorVersion", parameterValue2);
        component.BindInt("@patchVersion", parameterValue3);
        component.BindKeyValuePairStringTable("@contributions", (IEnumerable<KeyValuePair<string, string>>) contributions);
        component.BindInt("@taskDefinitionStatus", (int) taskDefinitionStatus);
        int num = await component.ExecuteNonQueryAsync();
      }
      finally
      {
        sqlMethodScope.Dispose();
      }
      sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope();
    }

    public override IList<TaskDefinitionData> UpdateContributionInstallComplete(
      IDictionary<string, string> contributionIds)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateContributionInstallComplete)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateContributionInstallComplete");
        this.BindKeyValuePairStringTable("@contributionIdentifers", (IEnumerable<KeyValuePair<string, string>>) contributionIds);
        this.BindGuid("@writerId", this.Author);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskDefinitionData>((ObjectBinder<TaskDefinitionData>) new TaskContributionDataBinder());
          return (IList<TaskDefinitionData>) resultCollection.GetCurrent<TaskDefinitionData>().Items;
        }
      }
    }
  }
}
