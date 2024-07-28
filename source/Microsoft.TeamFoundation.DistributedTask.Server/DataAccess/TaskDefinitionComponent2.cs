// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskDefinitionComponent2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskDefinitionComponent2 : TaskDefinitionComponent
  {
    public TaskDefinitionComponent2() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

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
      TaskDefinitionComponent2 component = this;
      TaskSqlComponentBase.SqlMethodScope sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AddTaskDefinitionAsync));
      try
      {
        component.PrepareStoredProcedure("Task.prc_AddTaskDefinition");
        component.BindGuid("@taskId", taskId);
        component.BindInt("@categoryId", 0);
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

    public override List<TaskDefinitionData> GetTaskDefinitions(
      Guid? taskId,
      TaskVersion version,
      bool allVersions)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetTaskDefinitions)))
      {
        this.PrepareStoredProcedure("Task.prc_GetTaskDefinitions");
        if (taskId.HasValue)
          this.BindGuid("@taskId", taskId.Value);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskDefinitionData>((ObjectBinder<TaskDefinitionData>) new TaskDefinitionDataBinder());
          return resultCollection.GetCurrent<TaskDefinitionData>().Items;
        }
      }
    }

    public override IDictionary<Guid, IList<TaskVersion>> GetTaskVersions(IEnumerable<Guid> taskIds = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetTaskVersions)))
      {
        this.PrepareStoredProcedure("Task.prc_GetTaskVersions");
        if (taskIds != null && taskIds.Any<Guid>())
          this.BindGuidTable("@taskIdTable", taskIds);
        Dictionary<Guid, IList<TaskVersion>> taskVersions = new Dictionary<Guid, IList<TaskVersion>>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<KeyValuePair<Guid, TaskVersion>>((ObjectBinder<KeyValuePair<Guid, TaskVersion>>) new TaskVersionBinder());
          foreach (KeyValuePair<Guid, TaskVersion> keyValuePair in resultCollection.GetCurrent<KeyValuePair<Guid, TaskVersion>>())
          {
            IList<TaskVersion> taskVersionList;
            if (!taskVersions.TryGetValue(keyValuePair.Key, out taskVersionList))
            {
              taskVersionList = (IList<TaskVersion>) new List<TaskVersion>();
              taskVersions.Add(keyValuePair.Key, taskVersionList);
            }
            taskVersionList.Add(keyValuePair.Value);
          }
          return (IDictionary<Guid, IList<TaskVersion>>) taskVersions;
        }
      }
    }

    public override async Task UpdateTaskDefinitionAsync(
      Guid taskId,
      TaskVersion version,
      long containerId,
      string filePath,
      string iconPath)
    {
      TaskDefinitionComponent2 component = this;
      TaskSqlComponentBase.SqlMethodScope sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateTaskDefinitionAsync));
      try
      {
        component.PrepareStoredProcedure("Task.prc_UpdateTaskDefinition");
        component.BindGuid("@taskId", taskId);
        component.BindInt("@majorVersion", version.Major);
        component.BindInt("@minorVersion", version.Minor);
        component.BindInt("@patchVersion", version.Patch);
        component.BindBoolean("@isTest", version.IsTest);
        component.BindLong("@containerId", containerId);
        component.BindString("@filePath", filePath, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindString("@iconPath", iconPath, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
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
