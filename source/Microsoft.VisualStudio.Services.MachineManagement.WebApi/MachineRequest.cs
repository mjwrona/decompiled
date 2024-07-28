// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineRequest
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachineRequest
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Inputs")]
    private IDictionary<string, string> m_inputs;
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Outputs")]
    private IDictionary<string, string> m_outputs;
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Secrets")]
    private IDictionary<string, string> m_secrets;
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Tags")]
    private List<string> m_tags;

    internal MachineRequest()
    {
    }

    public MachineRequest(string poolType, string poolName, Guid hostId)
    {
      this.HostId = hostId;
      this.PoolType = poolType;
      this.PoolName = poolName;
      this.IsScheduled = new bool?(false);
    }

    public MachineRequest(
      string poolType,
      string poolName,
      Guid hostId,
      string requiredResourceVersion)
    {
      this.HostId = hostId;
      this.PoolType = poolType;
      this.PoolName = poolName;
      this.IsScheduled = new bool?(false);
      this.RequiredResourceVersion = requiredResourceVersion;
    }

    [DataMember(EmitDefaultValue = false)]
    public Guid ServiceOwner { get; set; }

    [DataMember(IsRequired = true)]
    public Guid HostId { get; set; }

    [DataMember(IsRequired = true)]
    public long RequestId { get; set; }

    [DataMember(IsRequired = true)]
    public string PoolType { get; set; }

    [DataMember(IsRequired = true)]
    public string PoolName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TimeSpan? Timeout { get; set; }

    public IDictionary<string, string> Inputs
    {
      get
      {
        if (this.m_inputs == null)
          this.m_inputs = (IDictionary<string, string>) new Dictionary<string, string>();
        return this.m_inputs;
      }
      internal set => this.m_inputs = value;
    }

    public IDictionary<string, string> Outputs
    {
      get
      {
        if (this.m_outputs == null)
          this.m_outputs = (IDictionary<string, string>) new Dictionary<string, string>();
        return this.m_outputs;
      }
      internal set => this.m_outputs = value;
    }

    [DataMember(IsRequired = false)]
    public DateTime QueuedTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? AssignedTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? FinishTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? UnassignedTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? SlaStartTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri HostUri { get; set; }

    [DataMember(IsRequired = false)]
    public Guid TraceActivityId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public MachineRequestOutcome Outcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string RequiredResourceVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AssignedResourceVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? AssignmentAttempts { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool? IsScheduled { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string RequestType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? MaxParallelism { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string MachineImageLabel { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public MachineImageLabelVersion MachineImageLabelVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string OrchestrationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid SourceCorrelationId { get; set; }

    public List<string> Tags
    {
      get
      {
        if (this.m_tags == null)
          this.m_tags = new List<string>();
        return this.m_tags;
      }
      set => this.m_tags = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int WaitTimeSlaSeconds { get; set; }

    public IDictionary<string, string> Secrets
    {
      get
      {
        if (this.m_secrets == null)
          this.m_secrets = (IDictionary<string, string>) new Dictionary<string, string>();
        return this.m_secrets;
      }
      set => this.m_secrets = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string SuspiciousActivity { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string MachineInstanceName { get; set; }

    public MachineInstanceMessage GetMachineInstanceMessage() => new MachineInstanceMessage()
    {
      Body = JsonUtility.ToString((object) this),
      MessageType = "http://schemas.microsoft.com/visualstudio/2012/services/machinemanagement/ProcessRequest"
    };

    public override string ToString() => string.Format("{0}\r\nRequestId: {1}\r\nPoolName: {2}\r\nQueuedTime: {3}\r\nAssignedTime: {4}\r\nStartTime: {5}\r\nFinishTime: {6}\r\nUnassignedTime: {7}\r\nOrchestrationId: {8}\r\nInstanceName: {9}", (object) nameof (MachineRequest), (object) this.RequestId, (object) this.PoolName, (object) this.QueuedTime, (object) this.AssignedTime, (object) this.StartTime, (object) this.FinishTime, (object) this.UnassignedTime, (object) this.OrchestrationId, (object) this.MachineInstanceName);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      IDictionary<string, string> inputs = this.m_inputs;
      if ((inputs != null ? (inputs.Count == 0 ? 1 : 0) : 0) != 0)
        this.m_inputs = (IDictionary<string, string>) null;
      IDictionary<string, string> outputs = this.m_outputs;
      if ((outputs != null ? (outputs.Count == 0 ? 1 : 0) : 0) != 0)
        this.m_outputs = (IDictionary<string, string>) null;
      IDictionary<string, string> secrets = this.m_secrets;
      if ((secrets != null ? (secrets.Count == 0 ? 1 : 0) : 0) != 0)
        this.m_secrets = (IDictionary<string, string>) null;
      List<string> tags = this.m_tags;
      // ISSUE: explicit non-virtual call
      if ((tags != null ? (__nonvirtual (tags.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_tags = (List<string>) null;
    }
  }
}
