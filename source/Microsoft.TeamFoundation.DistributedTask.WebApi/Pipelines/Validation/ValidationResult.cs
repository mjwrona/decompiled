// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation.ValidationResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ValidationResult
  {
    [DataMember(Name = "Errors", EmitDefaultValue = false)]
    private List<PipelineValidationError> m_errors;
    [DataMember(Name = "ReferencedResources", EmitDefaultValue = false)]
    private PipelineResources m_referencedResources;
    [DataMember(Name = "UnauthorizedResources", EmitDefaultValue = false)]
    private PipelineResources m_unauthorizedResources;

    public PipelineEnvironment Environment { get; internal set; }

    public IList<PipelineValidationError> Errors
    {
      get
      {
        if (this.m_errors == null)
          this.m_errors = new List<PipelineValidationError>();
        return (IList<PipelineValidationError>) this.m_errors;
      }
    }

    public PipelineResources ReferencedResources
    {
      get
      {
        if (this.m_referencedResources == null)
          this.m_referencedResources = new PipelineResources();
        return this.m_referencedResources;
      }
    }

    public PipelineResources UnauthorizedResources
    {
      get
      {
        if (this.m_unauthorizedResources == null)
          this.m_unauthorizedResources = new PipelineResources();
        return this.m_unauthorizedResources;
      }
    }

    internal void AddQueueReference(int id, string name)
    {
      if (id != 0)
      {
        this.ReferencedResources.Queues.Add(new AgentQueueReference()
        {
          Id = id
        });
      }
      else
      {
        if (string.IsNullOrEmpty(name))
          return;
        ISet<AgentQueueReference> queues = this.ReferencedResources.Queues;
        AgentQueueReference agentQueueReference = new AgentQueueReference();
        agentQueueReference.Name = (ExpressionValue<string>) name;
        queues.Add(agentQueueReference);
      }
    }

    internal void AddPoolReference(int id, string name)
    {
      if (id != 0)
      {
        this.ReferencedResources.Pools.Add(new AgentPoolReference()
        {
          Id = id
        });
      }
      else
      {
        if (string.IsNullOrEmpty(name))
          return;
        ISet<AgentPoolReference> pools = this.ReferencedResources.Pools;
        AgentPoolReference agentPoolReference = new AgentPoolReference();
        agentPoolReference.Name = (ExpressionValue<string>) name;
        pools.Add(agentPoolReference);
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<PipelineValidationError> errors = this.m_errors;
      // ISSUE: explicit non-virtual call
      if ((errors != null ? (__nonvirtual (errors.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_errors = (List<PipelineValidationError>) null;
      PipelineResources referencedResources = this.m_referencedResources;
      if ((referencedResources != null ? (referencedResources.Count == 0 ? 1 : 0) : 0) != 0)
        this.m_referencedResources = (PipelineResources) null;
      PipelineResources unauthorizedResources = this.m_unauthorizedResources;
      if ((unauthorizedResources != null ? (unauthorizedResources.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_unauthorizedResources = (PipelineResources) null;
    }
  }
}
