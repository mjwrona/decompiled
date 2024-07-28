// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IInternalCloudAgentDefinitionService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DefaultServiceImplementation(typeof (InternalCloudAgentDefinitionService))]
  public interface IInternalCloudAgentDefinitionService : IVssFrameworkService
  {
    Task<InternalCloudAgentDefinition> AddInternalCloudAgentDefinitionAsync(
      IVssRequestContext requestContext,
      InternalCloudAgentDefinition agentDefinition);

    Task<InternalCloudAgentDefinition> UpdateInternalCloudAgentDefinitionAsync(
      IVssRequestContext requestContext,
      InternalCloudAgentDefinition agentDefinition);

    Task<InternalCloudAgentDefinition> DeleteInternalCloudAgentDefinitionAsync(
      IVssRequestContext requestContext,
      InternalCloudAgentDefinition agentDefinition);

    Task<InternalCloudAgentDefinition> DeleteInternalCloudAgentDefinitionAsync(
      IVssRequestContext requestContext,
      string identifier);

    Task<InternalCloudAgentDefinition> GetInternalCloudAgentDefinitionAsync(
      IVssRequestContext requestContext,
      string identifier);

    Task<List<InternalCloudAgentDefinition>> GetInternalCloudAgentDefinitionsAsync(
      IVssRequestContext requestContext);
  }
}
