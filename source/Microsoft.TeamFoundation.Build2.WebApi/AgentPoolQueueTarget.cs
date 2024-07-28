// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueueTarget
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class AgentPoolQueueTarget : PhaseTarget
  {
    [DataMember(Name = "Demands", EmitDefaultValue = false)]
    private List<Demand> m_serializedDemands;
    private List<Demand> m_demands;

    public AgentPoolQueueTarget()
      : this((ISecuredObject) null)
    {
    }

    internal AgentPoolQueueTarget(ISecuredObject securedObject)
      : base(1, securedObject)
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

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<Demand>(ref this.m_serializedDemands, ref this.m_demands, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<Demand>(ref this.m_demands, ref this.m_serializedDemands);
  }
}
