// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskDefinitionComponent3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskDefinitionComponent3 : TaskDefinitionComponent2
  {
    public TaskDefinitionComponent3() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public override async Task AddTaskDefinitionAsync(
      Guid taskId,
      TaskVersion version,
      string name,
      string displayName,
      byte[] metadataDocument,
      bool overwrite,
      string contributionIdentifier = null,
      string contributionVersion = null,
      bool contributionInstallComplete = true,
      DateTime? contributionUpdatedOn = null)
    {
      TaskDefinitionComponent3 component = this;
      TaskSqlComponentBase.SqlMethodScope sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AddTaskDefinitionAsync));
      try
      {
        component.PrepareStoredProcedure("Task.prc_AddTaskDefinition");
        component.BindGuid("@taskId", taskId);
        component.BindInt("@majorVersion", version.Major);
        component.BindInt("@minorVersion", version.Minor);
        component.BindInt("@patchVersion", version.Patch);
        component.BindBoolean("@isTest", version.IsTest);
        component.BindString("@name", name, 25, false, SqlDbType.NVarChar);
        component.BindString("@displayName", displayName, 40, false, SqlDbType.NVarChar);
        component.BindBinary("@metadataDocument", metadataDocument, SqlDbType.VarBinary);
        component.BindBoolean("@overwrite", overwrite);
        component.BindGuid("@writerId", component.Author);
        int num = await component.ExecuteNonQueryAsync();
      }
      finally
      {
        sqlMethodScope.Dispose();
      }
      sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope();
    }

    public override void DeleteTaskDefinition(Guid taskId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteTaskDefinition)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteTaskDefinition");
        this.BindGuid("@taskId", taskId);
        this.BindGuid("@writerId", this.Author);
        this.ExecuteNonQuery();
      }
    }
  }
}
