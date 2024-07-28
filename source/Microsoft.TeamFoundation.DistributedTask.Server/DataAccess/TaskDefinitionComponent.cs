// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskDefinitionComponent
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
  internal class TaskDefinitionComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[9]
    {
      (IComponentCreator) new ComponentCreator<TaskDefinitionComponent>(1),
      (IComponentCreator) new ComponentCreator<TaskDefinitionComponent2>(2),
      (IComponentCreator) new ComponentCreator<TaskDefinitionComponent3>(3),
      (IComponentCreator) new ComponentCreator<TaskDefinitionComponent4>(4),
      (IComponentCreator) new ComponentCreator<TaskDefinitionComponent5>(5),
      (IComponentCreator) new ComponentCreator<TaskDefinitionComponent6>(6),
      (IComponentCreator) new ComponentCreator<TaskDefinitionComponent7>(7),
      (IComponentCreator) new ComponentCreator<TaskDefinitionComponent8>(8),
      (IComponentCreator) new ComponentCreator<TaskDefinitionComponent9>(9)
    }, "DistributedTaskDefinition", "DistributedTask");

    public TaskDefinitionComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual async Task AddTaskDefinitionAsync(
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
      TaskDefinitionComponent component = this;
      TaskSqlComponentBase.SqlMethodScope sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AddTaskDefinitionAsync));
      try
      {
        component.PrepareStoredProcedure("prc_AddTaskDefinition");
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
        int num = await component.ExecuteNonQueryAsync();
      }
      finally
      {
        sqlMethodScope.Dispose();
      }
      sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope();
    }

    public virtual void DeleteTaskDefinition(Guid taskId) => throw new ServiceVersionNotSupportedException();

    public virtual List<TaskDefinitionData> GetTaskDefinitions(
      Guid? taskId,
      TaskVersion version,
      bool allVersions)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetTaskDefinitions)))
      {
        this.PrepareStoredProcedure("prc_GetTaskDefinitions");
        if (taskId.HasValue)
          this.BindGuid("@taskId", taskId.Value);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskDefinitionData>((ObjectBinder<TaskDefinitionData>) new TaskDefinitionDataBinder());
          return resultCollection.GetCurrent<TaskDefinitionData>().Items;
        }
      }
    }

    public virtual IDictionary<Guid, IList<TaskVersion>> GetTaskVersions(IEnumerable<Guid> taskIds = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetTaskVersions)))
      {
        this.PrepareStoredProcedure("prc_GetTaskVersions");
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

    public virtual async Task UpdateTaskDefinitionAsync(
      Guid taskId,
      TaskVersion version,
      long containerId,
      string filePath,
      string iconPath)
    {
      TaskDefinitionComponent component = this;
      TaskSqlComponentBase.SqlMethodScope sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateTaskDefinitionAsync));
      try
      {
        component.PrepareStoredProcedure("prc_UpdateTaskDefinition");
        component.BindGuid("@taskId", taskId);
        component.BindInt("@majorVersion", version.Major);
        component.BindInt("@minorVersion", version.Minor);
        component.BindInt("@patchVersion", version.Patch);
        component.BindBoolean("@isTest", version.IsTest);
        component.BindLong("@containerId", containerId);
        component.BindString("@filePath", filePath, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindString("@iconPath", iconPath, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        int num = await component.ExecuteNonQueryAsync();
      }
      finally
      {
        sqlMethodScope.Dispose();
      }
      sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope();
    }

    public virtual void InstallCollectionData()
    {
    }

    public virtual void DeleteTaskDefinitions(List<Guid> taskIds)
    {
    }

    public virtual Task<IList<Guid>> DeleteTaskDefinitionsForExtensionAsync(
      string contributionIdentifierPrefix)
    {
      return Task.FromResult<IList<Guid>>((IList<Guid>) Array.Empty<Guid>());
    }

    public virtual IList<TaskDefinitionData> UpdateContributionInstallComplete(
      IDictionary<string, string> contributionIds)
    {
      return (IList<TaskDefinitionData>) new List<TaskDefinitionData>();
    }

    public virtual Task AddTaskDefinitionHistoryAsync(
      TaskDefinitionStatus taskDefinitionStatus,
      Guid? taskId = null,
      TaskVersion version = null,
      Dictionary<string, string> contributions = null)
    {
      return Task.CompletedTask;
    }

    public virtual Task UpdateContributionVersionAsync(Guid taskId, string contributionVersion) => Task.CompletedTask;

    public virtual Task<IList<TaskDefinitionData>> GetTaskDefinitionsForExtensionAsync(
      string extensionIdentifier)
    {
      return Task.FromResult<IList<TaskDefinitionData>>((IList<TaskDefinitionData>) Array.Empty<TaskDefinitionData>());
    }
  }
}
