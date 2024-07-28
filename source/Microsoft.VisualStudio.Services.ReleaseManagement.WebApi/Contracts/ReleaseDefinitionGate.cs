// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionGate
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ReleaseDefinitionGate : ReleaseManagementSecuredObject
  {
    public ReleaseDefinitionGate() => this.Tasks = (IList<WorkflowTask>) new List<WorkflowTask>();

    [DataMember]
    public IList<WorkflowTask> Tasks { get; set; }

    [DataMember]
    public bool IsGenerated { get; set; }

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object obj)
    {
      if (!(obj is ReleaseDefinitionGate releaseDefinitionGate) || this.IsGenerated != releaseDefinitionGate.IsGenerated)
        return false;
      IList<WorkflowTask> tasks = this.Tasks;
      IList<WorkflowTask> newGateTasks = releaseDefinitionGate.Tasks;
      if (tasks == null && newGateTasks == null)
        return true;
      return tasks != null && newGateTasks != null && tasks.Count == newGateTasks.Count && !tasks.Where<WorkflowTask>((Func<WorkflowTask, int, bool>) ((t, i) => !t.Equals((object) newGateTasks[i], false))).Any<WorkflowTask>();
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IList<WorkflowTask> tasks = this.Tasks;
      if (tasks == null)
        return;
      tasks.ForEach<WorkflowTask>((Action<WorkflowTask>) (i => i?.SetSecuredObject(token, requiredPermissions)));
    }
  }
}
