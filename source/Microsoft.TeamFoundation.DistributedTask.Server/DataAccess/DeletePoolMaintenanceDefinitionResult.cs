// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.DeletePoolMaintenanceDefinitionResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class DeletePoolMaintenanceDefinitionResult
  {
    private IList<TaskAgentPoolMaintenanceDefinition> m_deletedPoolMaintenanceDefinitions;
    private IList<TaskAgentPoolMaintenanceJob> m_deletedPoolMaintenanceJobs;

    public IList<TaskAgentPoolMaintenanceDefinition> DeletedPoolMaintenanceDefinitions
    {
      get
      {
        if (this.m_deletedPoolMaintenanceDefinitions == null)
          this.m_deletedPoolMaintenanceDefinitions = (IList<TaskAgentPoolMaintenanceDefinition>) new List<TaskAgentPoolMaintenanceDefinition>();
        return this.m_deletedPoolMaintenanceDefinitions;
      }
      set => this.m_deletedPoolMaintenanceDefinitions = value;
    }

    public IList<TaskAgentPoolMaintenanceJob> DeletedPoolMaintenanceJobs
    {
      get
      {
        if (this.m_deletedPoolMaintenanceJobs == null)
          this.m_deletedPoolMaintenanceJobs = (IList<TaskAgentPoolMaintenanceJob>) new List<TaskAgentPoolMaintenanceJob>();
        return this.m_deletedPoolMaintenanceJobs;
      }
      set => this.m_deletedPoolMaintenanceJobs = value;
    }
  }
}
