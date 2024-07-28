// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlan
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskOrchestrationPlan : TaskOrchestrationPlanReference
  {
    private IOrchestrationProcess m_process;
    private IOrchestrationEnvironment m_processEnvironment;
    [DataMember(Name = "Environment", EmitDefaultValue = false)]
    private PlanEnvironment m_environment;
    [DataMember(Name = "Implementation", EmitDefaultValue = false)]
    private TaskOrchestrationContainer m_implementation;

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; set; }

    [DataMember]
    public TaskOrchestrationPlanState State { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskResult? Result { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResultCode { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TimelineReference Timeline { get; set; }

    public PlanEnvironment Environment
    {
      get
      {
        if (this.m_environment == null)
          this.SummonProcessEnvironment();
        return this.m_environment;
      }
      set
      {
        this.m_environment = value;
        this.m_processEnvironment = (IOrchestrationEnvironment) value;
      }
    }

    public TaskOrchestrationContainer Implementation
    {
      get
      {
        if (this.m_implementation == null)
          this.SummonProcess();
        return this.m_implementation;
      }
      set
      {
        this.m_process = (IOrchestrationProcess) value;
        this.m_implementation = value;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IOrchestrationProcess Process
    {
      get
      {
        if (this.m_process == null)
          this.SummonProcess();
        return this.m_process;
      }
      set
      {
        this.m_process = value;
        this.m_implementation = value as TaskOrchestrationContainer;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IOrchestrationEnvironment ProcessEnvironment
    {
      get
      {
        if (this.m_processEnvironment == null)
          this.SummonProcessEnvironment();
        return this.m_processEnvironment;
      }
      set
      {
        this.m_processEnvironment = value;
        this.m_environment = value as PlanEnvironment;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public Guid RequestedById { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid RequestedForId { get; set; }

    internal PlanTemplateType TemplateType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskLogReference InitializationLog { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskLogReference ExpandedYaml { get; set; }

    private void SummonProcess()
    {
      if (this.ProcessSerialized == null)
        return;
      this.Process = JsonUtility.Deserialize<IOrchestrationProcess>(this.ProcessSerialized);
      this.ProcessSerialized = (byte[]) null;
    }

    private void SummonProcessEnvironment()
    {
      if (this.ProcessEnvironmentSerialized == null)
        return;
      this.ProcessEnvironment = JsonUtility.Deserialize<IOrchestrationEnvironment>(this.ProcessEnvironmentSerialized);
      this.ProcessEnvironmentSerialized = (byte[]) null;
    }

    internal byte[] ProcessSerialized { get; set; }

    internal byte[] ProcessEnvironmentSerialized { get; set; }
  }
}
