// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime.GraphNodeInstance`1
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime
{
  [DataContract]
  public abstract class GraphNodeInstance<TNode> : IGraphNodeInstance where TNode : IGraphNode
  {
    [DataMember(Name = "Outputs", EmitDefaultValue = false)]
    private VariablesDictionary m_outputs;

    private protected GraphNodeInstance() => this.Attempt = 1;

    private protected GraphNodeInstance(
      string name,
      int attempt,
      TNode definition,
      TaskResult result)
    {
      this.Name = name;
      this.Attempt = attempt;
      this.Definition = definition;
      this.State = PipelineState.NotStarted;
      this.Result = new TaskResult?(result);
    }

    [DataMember(EmitDefaultValue = false)]
    public string Identifier { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int Attempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; set; }

    public IDictionary<string, VariableValue> Outputs
    {
      get
      {
        if (this.m_outputs == null)
          this.m_outputs = new VariablesDictionary();
        return (IDictionary<string, VariableValue>) this.m_outputs;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public PipelineState State { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskResult? Result { get; set; }

    public TNode Definition { get; internal set; }

    internal TimelineRecord Record { get; }

    public bool SecretsAccessed
    {
      get
      {
        VariablesDictionary outputs = this.m_outputs;
        return outputs != null && outputs.SecretsAccessed.Count > 0;
      }
    }

    public void ResetSecretsAccessed() => this.m_outputs?.SecretsAccessed.Clear();

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      VariablesDictionary outputs = this.m_outputs;
      if ((outputs != null ? (outputs.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_outputs = (VariablesDictionary) null;
    }
  }
}
