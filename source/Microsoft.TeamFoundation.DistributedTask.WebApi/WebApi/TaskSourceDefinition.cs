// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskSourceDefinition
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskSourceDefinition : TaskSourceDefinitionBase
  {
    public TaskSourceDefinition()
    {
    }

    private TaskSourceDefinition(TaskSourceDefinition inputDefinitionToClone)
      : base((TaskSourceDefinitionBase) inputDefinitionToClone)
    {
    }

    private TaskSourceDefinition(
      TaskSourceDefinition inputDefinitionToClone,
      ISecuredObject securedObject)
      : base((TaskSourceDefinitionBase) inputDefinitionToClone, securedObject)
    {
    }

    public TaskSourceDefinition Clone() => new TaskSourceDefinition(this);

    public override TaskSourceDefinitionBase Clone(ISecuredObject securedObject) => (TaskSourceDefinitionBase) new TaskSourceDefinition(this, securedObject);
  }
}
