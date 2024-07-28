// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskDefinitionComponent7
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskDefinitionComponent7 : TaskDefinitionComponent6
  {
    public override async Task UpdateContributionVersionAsync(
      Guid taskId,
      string contributionVersion)
    {
      TaskDefinitionComponent7 component = this;
      TaskSqlComponentBase.SqlMethodScope sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateContributionVersionAsync));
      try
      {
        component.PrepareStoredProcedure("Task.prc_UpdateContributionVersion");
        component.BindGuid("@taskId", taskId);
        component.BindString("@contributionVersion", contributionVersion, 25, true, SqlDbType.NVarChar);
        component.BindNullableDateTime2("@contributionUpdatedOn", new DateTime?(DateTime.Now));
        component.BindGuid("@writerId", component.Author);
        int num = await component.ExecuteNonQueryAsync();
      }
      finally
      {
        sqlMethodScope.Dispose();
      }
      sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope();
    }
  }
}
