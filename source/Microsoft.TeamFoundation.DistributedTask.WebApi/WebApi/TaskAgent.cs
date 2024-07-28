// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgent
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAgent : TaskAgentReference, ICloneable
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Properties")]
    private PropertiesCollection m_properties;
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "SystemCapabilities")]
    private Dictionary<string, string> m_systemCapabilities;
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "UserCapabilities")]
    private Dictionary<string, string> m_userCapabilities;

    internal TaskAgent()
    {
    }

    public TaskAgent(string name) => this.Name = name;

    internal TaskAgent(TaskAgentReference reference)
      : base(reference)
    {
    }

    private TaskAgent(TaskAgent agentToBeCloned)
      : base((TaskAgentReference) agentToBeCloned)
    {
      this.CreatedOn = agentToBeCloned.CreatedOn;
      this.MaxParallelism = agentToBeCloned.MaxParallelism;
      this.StatusChangedOn = agentToBeCloned.StatusChangedOn;
      if (agentToBeCloned.AssignedRequest != null)
        this.AssignedRequest = agentToBeCloned.AssignedRequest.Clone();
      if (agentToBeCloned.Authorization != null)
        this.Authorization = agentToBeCloned.Authorization.Clone();
      if (agentToBeCloned.m_properties != null && agentToBeCloned.m_properties.Count > 0)
        this.m_properties = new PropertiesCollection((IDictionary<string, object>) agentToBeCloned.m_properties);
      if (agentToBeCloned.m_systemCapabilities != null && agentToBeCloned.m_systemCapabilities.Count > 0)
        this.m_systemCapabilities = new Dictionary<string, string>((IDictionary<string, string>) agentToBeCloned.m_systemCapabilities, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (agentToBeCloned.m_userCapabilities != null && agentToBeCloned.m_userCapabilities.Count > 0)
        this.m_userCapabilities = new Dictionary<string, string>((IDictionary<string, string>) agentToBeCloned.m_userCapabilities, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (agentToBeCloned.PendingUpdate == null)
        return;
      this.PendingUpdate = agentToBeCloned.PendingUpdate.Clone();
    }

    [DataMember]
    public int? MaxParallelism { get; set; }

    [DataMember]
    public DateTime CreatedOn { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StatusChangedOn { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskAgentJobRequest AssignedRequest { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskAgentJobRequest LastCompletedRequest { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskAgentAuthorization Authorization { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskAgentUpdate PendingUpdate { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskAgentCloudRequest AssignedAgentCloudRequest { get; internal set; }

    public IDictionary<string, string> SystemCapabilities
    {
      get
      {
        if (this.m_systemCapabilities == null)
          this.m_systemCapabilities = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (IDictionary<string, string>) this.m_systemCapabilities;
      }
    }

    public IDictionary<string, string> UserCapabilities
    {
      get
      {
        if (this.m_userCapabilities == null)
          this.m_userCapabilities = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (IDictionary<string, string>) this.m_userCapabilities;
      }
    }

    public PropertiesCollection Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new PropertiesCollection();
        return this.m_properties;
      }
      internal set => this.m_properties = value;
    }

    object ICloneable.Clone() => (object) this.Clone();

    public TaskAgent Clone() => new TaskAgent(this);
  }
}
