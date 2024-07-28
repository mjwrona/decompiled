// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.InternalCloudAgentDefinitionService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class InternalCloudAgentDefinitionService : 
    IInternalCloudAgentDefinitionService,
    IVssFrameworkService
  {
    public async Task<InternalCloudAgentDefinition> AddInternalCloudAgentDefinitionAsync(
      IVssRequestContext requestContext,
      InternalCloudAgentDefinition agentDefinition)
    {
      InternalCloudAgentDefinition cloudAgentDefinition;
      using (InternalCloudAgentDefinitionComponent component = requestContext.CreateComponent<InternalCloudAgentDefinitionComponent>())
        cloudAgentDefinition = await component.AddInternalCloudAgentDefintionAsync(agentDefinition);
      return cloudAgentDefinition;
    }

    public Task<InternalCloudAgentDefinition> DeleteInternalCloudAgentDefinitionAsync(
      IVssRequestContext requestContext,
      InternalCloudAgentDefinition agentDefinition)
    {
      return this.DeleteInternalCloudAgentDefinitionAsync(requestContext, agentDefinition.Identifier);
    }

    public async Task<InternalCloudAgentDefinition> DeleteInternalCloudAgentDefinitionAsync(
      IVssRequestContext requestContext,
      string identifier)
    {
      InternalCloudAgentDefinition cloudAgentDefinition;
      using (InternalCloudAgentDefinitionComponent component = requestContext.CreateComponent<InternalCloudAgentDefinitionComponent>())
        cloudAgentDefinition = await component.DeleteInternalCloudAgentDefinitionAsync(identifier);
      return cloudAgentDefinition;
    }

    public async Task<InternalCloudAgentDefinition> GetInternalCloudAgentDefinitionAsync(
      IVssRequestContext requestContext,
      string identifier)
    {
      InternalCloudAgentDefinition agentDefinitionAsync;
      using (InternalCloudAgentDefinitionComponent component = requestContext.CreateComponent<InternalCloudAgentDefinitionComponent>())
        agentDefinitionAsync = await component.GetInternalCloudAgentDefinitionAsync(identifier);
      return agentDefinitionAsync;
    }

    public async Task<List<InternalCloudAgentDefinition>> GetInternalCloudAgentDefinitionsAsync(
      IVssRequestContext requestContext)
    {
      List<InternalCloudAgentDefinition> definitionsAsync;
      using (InternalCloudAgentDefinitionComponent component = requestContext.CreateComponent<InternalCloudAgentDefinitionComponent>())
        definitionsAsync = await component.GetInternalCloudAgentDefinitionsAsync();
      return definitionsAsync;
    }

    public async Task<InternalCloudAgentDefinition> UpdateInternalCloudAgentDefinitionAsync(
      IVssRequestContext requestContext,
      InternalCloudAgentDefinition agentDefinition)
    {
      InternalCloudAgentDefinition cloudAgentDefinition;
      using (InternalCloudAgentDefinitionComponent component = requestContext.CreateComponent<InternalCloudAgentDefinitionComponent>())
        cloudAgentDefinition = await component.UpdateInternalCloudAgentDefinitionAsync(agentDefinition);
      return cloudAgentDefinition;
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
