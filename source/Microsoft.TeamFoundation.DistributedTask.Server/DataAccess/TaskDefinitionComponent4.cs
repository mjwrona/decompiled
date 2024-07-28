// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskDefinitionComponent4
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
  internal class TaskDefinitionComponent4 : TaskDefinitionComponent3
  {
    public TaskDefinitionComponent4() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public override void InstallCollectionData()
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (InstallCollectionData)))
      {
        this.PrepareStoredProcedure("Task.prc_InstallCollectionData");
        this.ExecuteNonQuery();
      }
    }

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
      TaskDefinitionComponent4 component = this;
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
        component.BindString("@contributionIdentifier", contributionIdentifier, 2048, true, SqlDbType.NVarChar);
        component.BindString("@contributionVersion", contributionVersion, 2048, true, SqlDbType.NVarChar);
        component.BindGuid("@writerId", component.Author);
        int num = await component.ExecuteNonQueryAsync();
      }
      finally
      {
        sqlMethodScope.Dispose();
      }
      sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope();
    }

    public override async Task<IList<Guid>> DeleteTaskDefinitionsForExtensionAsync(
      string contributionIdentifierPrefix)
    {
      TaskDefinitionComponent4 component = this;
      IList<Guid> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (DeleteTaskDefinitionsForExtensionAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_DeleteTaskDefinitionsForExtension");
        component.BindString("@contributionIdentifierPrefix", contributionIdentifierPrefix, 2048, false, SqlDbType.NVarChar);
        component.BindGuid("@writerId", component.Author);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new TaskIdBinder());
          items = (IList<Guid>) resultCollection.GetCurrent<Guid>().Items;
        }
      }
      return items;
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
          resultCollection.AddBinder<TaskDefinitionData>((ObjectBinder<TaskDefinitionData>) new TaskDefinitionDataBinder2());
          return resultCollection.GetCurrent<TaskDefinitionData>().Items;
        }
      }
    }
  }
}
