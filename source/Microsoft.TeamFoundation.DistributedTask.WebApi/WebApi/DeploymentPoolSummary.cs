// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentPoolSummary
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class DeploymentPoolSummary
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "DeploymentGroups")]
    private IList<DeploymentGroupReference> m_deploymentGroups;

    [DataMember]
    public TaskAgentPoolReference Pool { get; internal set; }

    [DataMember]
    public int OnlineAgentsCount { get; internal set; }

    [DataMember]
    public int OfflineAgentsCount { get; internal set; }

    [DataMember]
    public EnvironmentResourceReference Resource { get; internal set; }

    public IList<DeploymentGroupReference> DeploymentGroups
    {
      get
      {
        if (this.m_deploymentGroups == null)
          this.m_deploymentGroups = (IList<DeploymentGroupReference>) new List<DeploymentGroupReference>();
        return this.m_deploymentGroups;
      }
      internal set => this.m_deploymentGroups = value;
    }
  }
}
