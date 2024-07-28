// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.PhaseExecutionState
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [DataContract]
  [JsonConverter(typeof (SimpleJsonConverter<PhaseExecutionState>))]
  public class PhaseExecutionState : IGraphNode
  {
    [DataMember(Name = "Jobs", EmitDefaultValue = false)]
    private List<JobExecutionState> m_jobs;
    [DataMember(Name = "Dependencies", EmitDefaultValue = false)]
    private GraphDependencies m_dependencies;
    [DataMember(Name = "SatisfiedDependencies", EmitDefaultValue = false)]
    private List<PhaseDependency> m_satisifiedDependencies;
    [DataMember(Name = "UnsatisfiedDependencies", EmitDefaultValue = false)]
    private List<PhaseDependency> m_unsatisfiedDependencies;

    [JsonConstructor]
    public PhaseExecutionState() => this.Attempt = 1;

    public PhaseExecutionState(PhaseNode phase)
    {
      this.Attempt = 1;
      if (phase == null)
        return;
      this.Name = phase.Name;
      this.Condition = phase.Condition;
      this.Skip = phase.Skip;
      if (phase is Phase phase1)
      {
        PhaseTarget target = phase1.Target;
        this.Target = target != null ? target.TrimForExecution() : (PhaseTarget) null;
      }
      else if (phase is ProviderPhase providerPhase)
        this.Provider = providerPhase.Provider;
      if (phase.DependsOn.Count <= 0)
        return;
      this.m_dependencies = new GraphDependencies();
      this.m_dependencies.Unsatisfied.UnionWith((IEnumerable<string>) phase.DependsOn);
    }

    public PhaseExecutionState(PhaseAttempt attempt)
      : this(attempt.Phase.Definition)
    {
      PhaseInstance phase = attempt.Phase;
      this.Name = phase.Name;
      this.Attempt = phase.Attempt;
      this.FinishTime = phase.FinishTime;
      this.StartTime = phase.StartTime;
      this.State = phase.State;
      this.Result = phase.Result;
      foreach (JobAttempt jobAttempt in attempt.Jobs.Where<JobAttempt>((Func<JobAttempt, bool>) (x => x.Job.State != PipelineState.Completed)))
        this.Jobs.Add(new JobExecutionState(jobAttempt.Job));
    }

    [DefaultValue(1)]
    [DataMember(EmitDefaultValue = false)]
    public int Attempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Condition { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PhaseTarget Target { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Provider { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool ContinueOnError { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool FailFast { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int MaxConcurrency { get; set; }

    public IList<JobExecutionState> Jobs
    {
      get
      {
        if (this.m_jobs == null)
          this.m_jobs = new List<JobExecutionState>();
        return (IList<JobExecutionState>) this.m_jobs;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; set; }

    [DataMember]
    public PipelineState State { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskResult? Result { get; set; }

    [DataMember]
    public bool Skip { get; set; }

    public GraphDependencies Dependencies
    {
      get
      {
        if (this.m_dependencies == null)
          this.m_dependencies = new GraphDependencies();
        return this.m_dependencies;
      }
    }

    public void CopyFrom(PhaseExecutionState source)
    {
      this.Attempt = source.Attempt;
      this.StartTime = source.StartTime;
      this.FinishTime = source.FinishTime;
      this.State = source.State;
      this.Result = source.Result;
      this.Skip = source.Skip;
      this.Jobs.Clear();
      this.Jobs.AddRange<JobExecutionState, IList<JobExecutionState>>((IEnumerable<JobExecutionState>) source.Jobs);
    }

    public PhaseInstance ToInstance()
    {
      PhaseInstance instance = new PhaseInstance();
      instance.Attempt = this.Attempt;
      instance.FinishTime = this.FinishTime;
      instance.Name = this.Name;
      instance.StartTime = this.StartTime;
      instance.State = this.State;
      instance.Result = this.Result;
      return instance;
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      if (this.m_unsatisfiedDependencies == null && this.m_satisifiedDependencies == null)
        return;
      this.m_dependencies = new GraphDependencies();
      List<PhaseDependency> satisifiedDependencies = this.m_satisifiedDependencies;
      // ISSUE: explicit non-virtual call
      if ((satisifiedDependencies != null ? (__nonvirtual (satisifiedDependencies.Count) > 0 ? 1 : 0) : 0) != 0)
        this.m_dependencies.Satisfied.AddRange<string, ISet<string>>(this.m_satisifiedDependencies.Select<PhaseDependency, string>((Func<PhaseDependency, string>) (x => x.Scope)));
      List<PhaseDependency> unsatisfiedDependencies = this.m_unsatisfiedDependencies;
      // ISSUE: explicit non-virtual call
      if ((unsatisfiedDependencies != null ? (__nonvirtual (unsatisfiedDependencies.Count) > 0 ? 1 : 0) : 0) != 0)
        this.m_dependencies.Unsatisfied.AddRange<string, ISet<string>>(this.m_unsatisfiedDependencies.Select<PhaseDependency, string>((Func<PhaseDependency, string>) (x => x.Scope)));
      this.m_satisifiedDependencies = (List<PhaseDependency>) null;
      this.m_unsatisfiedDependencies = (List<PhaseDependency>) null;
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<JobExecutionState> jobs = this.m_jobs;
      // ISSUE: explicit non-virtual call
      if ((jobs != null ? (__nonvirtual (jobs.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_jobs = (List<JobExecutionState>) null;
      if (this.m_dependencies == null || this.m_dependencies.Satisfied.Count != 0 || this.m_dependencies.Unsatisfied.Count != 0)
        return;
      this.m_dependencies = (GraphDependencies) null;
    }
  }
}
