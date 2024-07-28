// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentReference
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAgentReference : ICloneable
  {
    [DataMember(Name = "_links", EmitDefaultValue = false)]
    private ReferenceLinks m_links;

    public TaskAgentReference()
    {
    }

    protected TaskAgentReference(TaskAgentReference referenceToBeCloned)
    {
      this.Id = referenceToBeCloned.Id;
      this.Name = referenceToBeCloned.Name;
      this.Version = referenceToBeCloned.Version;
      this.Enabled = referenceToBeCloned.Enabled;
      this.Status = referenceToBeCloned.Status;
      this.OSDescription = referenceToBeCloned.OSDescription;
      this.ProvisioningState = referenceToBeCloned.ProvisioningState;
      this.AccessPoint = referenceToBeCloned.AccessPoint;
      if (referenceToBeCloned.m_links == null)
        return;
      this.m_links = referenceToBeCloned.m_links.Clone();
    }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Version { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string OSDescription { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? Enabled { get; set; }

    [DataMember]
    public TaskAgentStatus Status { get; set; }

    [DataMember]
    public string ProvisioningState { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string AccessPoint { get; set; }

    public ReferenceLinks Links
    {
      get
      {
        if (this.m_links == null)
          this.m_links = new ReferenceLinks();
        return this.m_links;
      }
      internal set => this.m_links = value;
    }

    object ICloneable.Clone() => (object) this.Clone();

    public TaskAgentReference Clone() => new TaskAgentReference(this);
  }
}
