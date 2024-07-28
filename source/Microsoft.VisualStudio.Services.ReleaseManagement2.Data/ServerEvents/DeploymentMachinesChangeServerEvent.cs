// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.ServerEvents.DeploymentMachinesChangeServerEvent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.ServerEvents
{
  [ServiceEventObject]
  public class DeploymentMachinesChangeServerEvent
  {
    [DataMember(Name = "Machines")]
    private IList<DeploymentMachineChangedData> deploymentTargetsChangedData;

    public DeploymentMachinesChangeServerEvent(
      Guid projectId,
      int deploymentGroupId,
      IList<DeploymentMachineChangedData> targetsChangedData)
    {
      this.ProjectId = projectId;
      this.DeploymentGroupId = deploymentGroupId;
      this.deploymentTargetsChangedData = targetsChangedData;
    }

    public IList<DeploymentMachineChangedData> TargetsChangedData
    {
      get
      {
        if (this.deploymentTargetsChangedData == null)
          this.deploymentTargetsChangedData = (IList<DeploymentMachineChangedData>) new List<DeploymentMachineChangedData>();
        return this.deploymentTargetsChangedData;
      }
    }

    public int DeploymentGroupId { get; }

    public Guid ProjectId { get; }
  }
}
