// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.InternalCloudAgentDefinitionComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class InternalCloudAgentDefinitionComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<InternalCloudAgentDefinitionComponent>(1),
      (IComponentCreator) new ComponentCreator<InternalCloudAgentDefinitionComponent2>(2)
    }, "DistributedTaskInternalCloudAgentDefinition", "DistributedTask");

    public virtual async Task<InternalCloudAgentDefinition> AddInternalCloudAgentDefintionAsync(
      InternalCloudAgentDefinition agentSpec)
    {
      InternalCloudAgentDefinitionComponent component = this;
      InternalCloudAgentDefinition cloudAgentDefinition;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AddInternalCloudAgentDefintionAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_AddInternalCloudAgentDefinition", true);
        component.BindString("@identifier", agentSpec.Identifier, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@imageLabel", agentSpec.ImageLabel, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindBoolean("@isVisible", agentSpec.IsVisible);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<InternalCloudAgentDefinition>(component.GetInternalCloudAgentDefinitionBinder());
          cloudAgentDefinition = resultCollection.GetCurrent<InternalCloudAgentDefinition>().FirstOrDefault<InternalCloudAgentDefinition>();
        }
      }
      return cloudAgentDefinition;
    }

    public virtual async Task<InternalCloudAgentDefinition> GetInternalCloudAgentDefinitionAsync(
      string identifier)
    {
      InternalCloudAgentDefinitionComponent component = this;
      InternalCloudAgentDefinition agentDefinitionAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetInternalCloudAgentDefinitionAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetInternalCloudAgentDefinitions", true);
        component.BindString("@identifier", identifier, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<InternalCloudAgentDefinition>(component.GetInternalCloudAgentDefinitionBinder());
          agentDefinitionAsync = resultCollection.GetCurrent<InternalCloudAgentDefinition>().FirstOrDefault<InternalCloudAgentDefinition>();
        }
      }
      return agentDefinitionAsync;
    }

    public virtual async Task<List<InternalCloudAgentDefinition>> GetInternalCloudAgentDefinitionsAsync()
    {
      InternalCloudAgentDefinitionComponent component = this;
      List<InternalCloudAgentDefinition> list;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetInternalCloudAgentDefinitionsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetInternalCloudAgentDefinitions", true);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<InternalCloudAgentDefinition>(component.GetInternalCloudAgentDefinitionBinder());
          list = resultCollection.GetCurrent<InternalCloudAgentDefinition>().ToList<InternalCloudAgentDefinition>();
        }
      }
      return list;
    }

    public virtual async Task<InternalCloudAgentDefinition> DeleteInternalCloudAgentDefinitionAsync(
      string identifier)
    {
      InternalCloudAgentDefinitionComponent component = this;
      InternalCloudAgentDefinition cloudAgentDefinition;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (DeleteInternalCloudAgentDefinitionAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_DeleteInternalCloudAgentDefinition", true);
        component.BindString("@identifier", identifier, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<InternalCloudAgentDefinition>(component.GetInternalCloudAgentDefinitionBinder());
          cloudAgentDefinition = resultCollection.GetCurrent<InternalCloudAgentDefinition>().FirstOrDefault<InternalCloudAgentDefinition>();
        }
      }
      return cloudAgentDefinition;
    }

    public virtual async Task<InternalCloudAgentDefinition> UpdateInternalCloudAgentDefinitionAsync(
      InternalCloudAgentDefinition agentSpec)
    {
      InternalCloudAgentDefinitionComponent definitionComponent = this;
      definitionComponent.PrepareStoredProcedure("Task.prc_UpdateInternalCloudAgentDefinition", true);
      definitionComponent.BindString("@identifier", agentSpec.Identifier, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      definitionComponent.BindString("@imageLabel", agentSpec.ImageLabel, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      definitionComponent.BindBoolean("@isVisible", agentSpec.IsVisible);
      InternalCloudAgentDefinition cloudAgentDefinition;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await definitionComponent.ExecuteReaderAsync(), definitionComponent.ProcedureName, definitionComponent.RequestContext))
      {
        resultCollection.AddBinder<InternalCloudAgentDefinition>(definitionComponent.GetInternalCloudAgentDefinitionBinder());
        cloudAgentDefinition = resultCollection.GetCurrent<InternalCloudAgentDefinition>().FirstOrDefault<InternalCloudAgentDefinition>();
      }
      return cloudAgentDefinition;
    }

    protected virtual ObjectBinder<InternalCloudAgentDefinition> GetInternalCloudAgentDefinitionBinder() => (ObjectBinder<InternalCloudAgentDefinition>) new InternalCloudAgentDefinitionBinder();
  }
}
