// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueTarget
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class AgentPoolQueueTarget : PhaseTarget
  {
    [DataMember(Name = "Demands", EmitDefaultValue = false)]
    private List<Demand> m_serializedDemands;
    private List<Demand> m_demands;

    public AgentPoolQueueTarget()
      : base(1)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public AgentPoolQueue Queue { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public AgentSpecification AgentSpecification { get; set; }

    public List<Demand> Demands
    {
      get
      {
        if (this.m_demands == null)
          this.m_demands = new List<Demand>();
        return this.m_demands;
      }
      set => this.m_demands = new List<Demand>((IEnumerable<Demand>) value);
    }

    [DataMember]
    public AgentTargetExecutionOptions ExecutionOptions { get; set; }

    [DataMember]
    public bool AllowScriptsAuthAccessOption { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public WorkspaceOptions Workspace { get; set; }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<Demand>(ref this.m_serializedDemands, ref this.m_demands, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<Demand>(ref this.m_demands, ref this.m_serializedDemands);
  }
}
