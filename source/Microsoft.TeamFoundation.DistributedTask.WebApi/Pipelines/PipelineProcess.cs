// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineProcess
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PipelineProcess : IOrchestrationProcess
  {
    [DataMember(Name = "Stages", EmitDefaultValue = false)]
    private List<Stage> m_stages;
    [DataMember(Name = "Phases", EmitDefaultValue = false)]
    private List<PhaseNode> m_phases;

    [JsonConstructor]
    public PipelineProcess()
    {
    }

    public PipelineProcess(IList<PhaseNode> phases)
    {
      Stage defaultStage = PipelineProcess.CreateDefaultStage();
      defaultStage.Phases.AddRange<PhaseNode, IList<PhaseNode>>((IEnumerable<PhaseNode>) phases ?? Enumerable.Empty<PhaseNode>());
      this.Stages.Add(defaultStage);
    }

    public PipelineProcess(IList<Stage> stages)
    {
      if (stages == null || stages.Count <= 0)
        return;
      this.m_stages = new List<Stage>((IEnumerable<Stage>) stages);
    }

    public IList<Stage> Stages
    {
      get
      {
        if (this.m_stages == null)
          this.m_stages = new List<Stage>();
        return (IList<Stage>) this.m_stages;
      }
    }

    OrchestrationProcessType IOrchestrationProcess.ProcessType => OrchestrationProcessType.Pipeline;

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      List<PhaseNode> phases = this.m_phases;
      // ISSUE: explicit non-virtual call
      if ((phases != null ? (__nonvirtual (phases.Count) > 0 ? 1 : 0) : 0) == 0)
        return;
      Stage defaultStage = PipelineProcess.CreateDefaultStage();
      defaultStage.Phases.AddRange<PhaseNode, IList<PhaseNode>>((IEnumerable<PhaseNode>) this.m_phases);
      this.m_phases = (List<PhaseNode>) null;
      this.Stages.Add(defaultStage);
    }

    private static Stage CreateDefaultStage() => new Stage()
    {
      Name = PipelineConstants.DefaultJobName
    };

    public IGraphNode GetNodeAtPath(IList<string> path)
    {
      IList<string> source = path;
      int? nullable1 = source != null ? new int?(source.Count<string>()) : new int?();
      IGraphNode nodeAtPath = (IGraphNode) null;
      int? nullable2 = nullable1;
      int num1 = 0;
      if (nullable2.GetValueOrDefault() > num1 & nullable2.HasValue)
      {
        nodeAtPath = (IGraphNode) this.Stages.FirstOrDefault<Stage>((Func<Stage, bool>) (x => string.Equals(x.Name, path[0], StringComparison.OrdinalIgnoreCase) || string.Equals(x.Name, PipelineConstants.DefaultJobName, StringComparison.OrdinalIgnoreCase)));
        nullable2 = nullable1;
        int num2 = 1;
        if (nullable2.GetValueOrDefault() > num2 & nullable2.HasValue && nodeAtPath != null)
        {
          nodeAtPath = (IGraphNode) (nodeAtPath as Stage).Phases.FirstOrDefault<PhaseNode>((Func<PhaseNode, bool>) (x => string.Equals(x.Name, path[1], StringComparison.OrdinalIgnoreCase) || string.Equals(x.Name, PipelineConstants.DefaultJobName, StringComparison.OrdinalIgnoreCase)));
          nullable2 = nullable1;
          int num3 = 2;
          if (nullable2.GetValueOrDefault() > num3 & nullable2.HasValue)
            nodeAtPath = (IGraphNode) null;
        }
      }
      return nodeAtPath;
    }
  }
}
