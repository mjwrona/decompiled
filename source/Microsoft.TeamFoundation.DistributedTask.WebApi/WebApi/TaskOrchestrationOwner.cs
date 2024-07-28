// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationOwner
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskOrchestrationOwner : ICloneable
  {
    [DataMember(Name = "_links")]
    private ReferenceLinks m_links;

    public TaskOrchestrationOwner()
    {
    }

    private TaskOrchestrationOwner(TaskOrchestrationOwner ownerToBeCloned)
    {
      this.Id = ownerToBeCloned.Id;
      this.Name = ownerToBeCloned.Name;
      this.m_links = ownerToBeCloned.Links.Clone();
    }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    public ReferenceLinks Links
    {
      get
      {
        if (this.m_links == null)
          this.m_links = new ReferenceLinks();
        return this.m_links;
      }
    }

    public TaskOrchestrationOwner Clone() => new TaskOrchestrationOwner(this);

    object ICloneable.Clone() => (object) this.Clone();
  }
}
