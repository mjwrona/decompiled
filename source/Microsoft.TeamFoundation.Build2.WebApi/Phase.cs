// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Phase
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class Phase : BaseSecuredObject
  {
    [DataMember(Name = "Dependencies", EmitDefaultValue = false)]
    private List<Dependency> m_serializedDependencies;
    [DataMember(Name = "Steps", EmitDefaultValue = false)]
    private List<BuildDefinitionStep> m_serializedSteps;
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private IDictionary<string, BuildDefinitionVariable> m_serializedVariables;
    private List<Dependency> m_dependencies;
    private List<BuildDefinitionStep> m_steps;
    private IDictionary<string, BuildDefinitionVariable> m_variables;

    public Phase()
    {
    }

    internal Phase(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string RefName { get; set; }

    public List<BuildDefinitionStep> Steps
    {
      get
      {
        if (this.m_steps == null)
          this.m_steps = new List<BuildDefinitionStep>();
        return this.m_steps;
      }
      set => this.m_steps = value;
    }

    public IDictionary<string, BuildDefinitionVariable> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = (IDictionary<string, BuildDefinitionVariable>) new Dictionary<string, BuildDefinitionVariable>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_variables;
      }
      set => this.m_variables = (IDictionary<string, BuildDefinitionVariable>) new Dictionary<string, BuildDefinitionVariable>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public List<Dependency> Dependencies
    {
      get
      {
        if (this.m_dependencies == null)
          this.m_dependencies = new List<Dependency>();
        return this.m_dependencies;
      }
      set => this.m_dependencies = value;
    }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string Condition { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PhaseTarget Target { get; set; }

    [DataMember]
    public BuildAuthorizationScope JobAuthorizationScope { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int JobTimeoutInMinutes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int JobCancelTimeoutInMinutes { get; set; }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      SerializationHelper.Copy<Dependency>(ref this.m_serializedDependencies, ref this.m_dependencies, true);
      SerializationHelper.Copy<BuildDefinitionStep>(ref this.m_serializedSteps, ref this.m_steps, true);
      SerializationHelper.Copy<string, BuildDefinitionVariable>(ref this.m_serializedVariables, ref this.m_variables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, true);
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      SerializationHelper.Copy<Dependency>(ref this.m_dependencies, ref this.m_serializedDependencies);
      SerializationHelper.Copy<BuildDefinitionStep>(ref this.m_steps, ref this.m_serializedSteps);
      SerializationHelper.Copy<string, BuildDefinitionVariable>(ref this.m_variables, ref this.m_serializedVariables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
