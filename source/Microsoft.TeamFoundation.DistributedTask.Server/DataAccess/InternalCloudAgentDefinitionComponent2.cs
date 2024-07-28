// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.InternalCloudAgentDefinitionComponent2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class InternalCloudAgentDefinitionComponent2 : InternalCloudAgentDefinitionComponent
  {
    protected bool m_209_useInternalCloudAgentDefinitionNewSearch;

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      this.m_209_useInternalCloudAgentDefinitionNewSearch = requestContext.IsFeatureEnabled("DistributedTask.UseInternalCloudAgentDefinitionNewSearch");
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
    }

    public override async Task<InternalCloudAgentDefinition> GetInternalCloudAgentDefinitionAsync(
      string identifier)
    {
      InternalCloudAgentDefinitionComponent2 component = this;
      InternalCloudAgentDefinition agentDefinitionAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetInternalCloudAgentDefinitionAsync)))
      {
        string storedProcedure = component.m_209_useInternalCloudAgentDefinitionNewSearch ? "Task.prc_GetInternalCloudAgentDefinitions" : "Task.prc_GetInternalCloudAgentDefinitions2";
        component.PrepareStoredProcedure(storedProcedure, true);
        component.BindString("@identifier", identifier, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<InternalCloudAgentDefinition>(component.GetInternalCloudAgentDefinitionBinder());
          agentDefinitionAsync = resultCollection.GetCurrent<InternalCloudAgentDefinition>().FirstOrDefault<InternalCloudAgentDefinition>();
        }
      }
      return agentDefinitionAsync;
    }

    public override async Task<List<InternalCloudAgentDefinition>> GetInternalCloudAgentDefinitionsAsync()
    {
      InternalCloudAgentDefinitionComponent2 component = this;
      List<InternalCloudAgentDefinition> list;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetInternalCloudAgentDefinitionsAsync)))
      {
        string storedProcedure = component.m_209_useInternalCloudAgentDefinitionNewSearch ? "Task.prc_GetInternalCloudAgentDefinitions" : "Task.prc_GetInternalCloudAgentDefinitions2";
        component.PrepareStoredProcedure(storedProcedure, true);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<InternalCloudAgentDefinition>(component.GetInternalCloudAgentDefinitionBinder());
          list = resultCollection.GetCurrent<InternalCloudAgentDefinition>().ToList<InternalCloudAgentDefinition>();
        }
      }
      return list;
    }
  }
}
