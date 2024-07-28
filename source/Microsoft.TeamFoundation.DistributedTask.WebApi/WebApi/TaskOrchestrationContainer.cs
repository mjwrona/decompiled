// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationContainer
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class TaskOrchestrationContainer : TaskOrchestrationItem, IOrchestrationProcess
  {
    [DataMember(Name = "Children")]
    private List<TaskOrchestrationItem> m_children;
    [DataMember(Name = "Data", EmitDefaultValue = false)]
    private IDictionary<string, string> m_data;

    public TaskOrchestrationContainer()
      : base(TaskOrchestrationItemType.Container)
    {
      this.ContinueOnError = true;
      this.MaxConcurrency = int.MaxValue;
    }

    [DataMember(EmitDefaultValue = false)]
    public bool Parallel { get; set; }

    public List<TaskOrchestrationItem> Children
    {
      get
      {
        if (this.m_children == null)
          this.m_children = new List<TaskOrchestrationItem>();
        return this.m_children;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public TaskOrchestrationContainer Rollback { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool ContinueOnError { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public int MaxConcurrency { get; set; }

    public IDictionary<string, string> Data
    {
      get
      {
        if (this.m_data == null)
          this.m_data = (IDictionary<string, string>) new Dictionary<string, string>();
        return this.m_data;
      }
    }

    OrchestrationProcessType IOrchestrationProcess.ProcessType => OrchestrationProcessType.Container;

    public IEnumerable<TaskOrchestrationJob> GetJobs()
    {
      TaskOrchestrationContainer orchestrationContainer = this;
      Queue<TaskOrchestrationContainer> containerQueue = new Queue<TaskOrchestrationContainer>();
      containerQueue.Enqueue(orchestrationContainer);
      while (containerQueue.Count > 0)
      {
        foreach (TaskOrchestrationItem child in containerQueue.Dequeue().Children)
        {
          switch (child.ItemType)
          {
            case TaskOrchestrationItemType.Container:
              containerQueue.Enqueue((TaskOrchestrationContainer) child);
              continue;
            case TaskOrchestrationItemType.Job:
              yield return child as TaskOrchestrationJob;
              continue;
            default:
              continue;
          }
        }
      }
    }
  }
}
