// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskInputDefinition
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskInputDefinition : TaskInputDefinitionBase
  {
    public TaskInputDefinition()
    {
    }

    private TaskInputDefinition(TaskInputDefinition inputDefinitionToClone)
      : base((TaskInputDefinitionBase) inputDefinitionToClone)
    {
    }

    private TaskInputDefinition(
      TaskInputDefinition inputDefinitionToClone,
      ISecuredObject securedObject)
      : base((TaskInputDefinitionBase) inputDefinitionToClone, securedObject)
    {
    }

    public TaskInputDefinition Clone() => new TaskInputDefinition(this);

    public override TaskInputDefinitionBase Clone(ISecuredObject securedObject) => base.Clone(securedObject);
  }
}
