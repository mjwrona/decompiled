// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class TimelineRecord : ITimelineRecordReference
  {
    [DataMember(Name = "Issues", EmitDefaultValue = false, Order = 60)]
    private List<Issue> m_issues;
    [DataMember(Name = "Variables", EmitDefaultValue = false, Order = 80)]
    private Dictionary<string, VariableValue> m_variables;
    [DataMember(Name = "PreviousAttempts", EmitDefaultValue = false, Order = 120)]
    private List<TimelineAttempt> m_previousAttempts;

    public TimelineRecord() => this.Attempt = 1;

    private TimelineRecord(TimelineRecord recordToBeCloned)
    {
      this.Attempt = recordToBeCloned.Attempt;
      this.ChangeId = recordToBeCloned.ChangeId;
      this.CurrentOperation = recordToBeCloned.CurrentOperation;
      this.FinishTime = recordToBeCloned.FinishTime;
      this.Id = recordToBeCloned.Id;
      this.Identifier = recordToBeCloned.Identifier;
      this.LastModified = recordToBeCloned.LastModified;
      this.Location = recordToBeCloned.Location;
      this.Name = recordToBeCloned.Name;
      this.Order = recordToBeCloned.Order;
      this.ParentId = recordToBeCloned.ParentId;
      this.PercentComplete = recordToBeCloned.PercentComplete;
      this.RecordType = recordToBeCloned.RecordType;
      this.Result = recordToBeCloned.Result;
      this.ResultCode = recordToBeCloned.ResultCode;
      this.StartTime = recordToBeCloned.StartTime;
      this.State = recordToBeCloned.State;
      this.TimelineId = recordToBeCloned.TimelineId;
      this.QueueId = recordToBeCloned.QueueId;
      this.AgentSpecification = recordToBeCloned.AgentSpecification;
      this.WorkerName = recordToBeCloned.WorkerName;
      this.RefName = recordToBeCloned.RefName;
      this.ErrorCount = recordToBeCloned.ErrorCount;
      this.WarningCount = recordToBeCloned.WarningCount;
      if (recordToBeCloned.Log != null)
        this.Log = new TaskLogReference()
        {
          Id = recordToBeCloned.Log.Id,
          Location = recordToBeCloned.Log.Location
        };
      if (recordToBeCloned.Details != null)
        this.Details = new TimelineReference()
        {
          ChangeId = recordToBeCloned.Details.ChangeId,
          Id = recordToBeCloned.Details.Id,
          Location = recordToBeCloned.Details.Location
        };
      if (recordToBeCloned.Task != null)
        this.Task = recordToBeCloned.Task.Clone();
      List<Issue> issues = recordToBeCloned.m_issues;
      // ISSUE: explicit non-virtual call
      if ((issues != null ? (__nonvirtual (issues.Count) > 0 ? 1 : 0) : 0) != 0)
        this.Issues.AddRange(recordToBeCloned.Issues.Select<Issue, Issue>((Func<Issue, Issue>) (i => i.Clone())));
      List<TimelineAttempt> previousAttempts = recordToBeCloned.m_previousAttempts;
      // ISSUE: explicit non-virtual call
      if ((previousAttempts != null ? (__nonvirtual (previousAttempts.Count) > 0 ? 1 : 0) : 0) != 0)
        this.PreviousAttempts.AddRange<TimelineAttempt, IList<TimelineAttempt>>((IEnumerable<TimelineAttempt>) recordToBeCloned.PreviousAttempts);
      Dictionary<string, VariableValue> variables = recordToBeCloned.m_variables;
      // ISSUE: explicit non-virtual call
      if ((variables != null ? (__nonvirtual (variables.Count) > 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_variables = recordToBeCloned.Variables.ToDictionary<KeyValuePair<string, VariableValue>, string, VariableValue>((Func<KeyValuePair<string, VariableValue>, string>) (k => k.Key), (Func<KeyValuePair<string, VariableValue>, VariableValue>) (v => v.Value.Clone()));
    }

    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [IgnoreDataMember]
    public Guid? TimelineId { get; set; }

    [DataMember(Order = 2)]
    public Guid? ParentId { get; set; }

    [DataMember(Name = "Type", Order = 3)]
    public string RecordType { get; set; }

    [DataMember(Order = 4)]
    public string Name { get; set; }

    [DataMember(Order = 5)]
    public DateTime? StartTime { get; set; }

    [DataMember(Order = 6)]
    public DateTime? FinishTime { get; set; }

    [DataMember(Order = 7)]
    public string CurrentOperation { get; set; }

    [DataMember(Order = 8)]
    public int? PercentComplete { get; set; }

    [DataMember(Order = 9)]
    public TimelineRecordState? State { get; set; }

    [DataMember(Order = 10)]
    public TaskResult? Result { get; set; }

    [DataMember(Order = 11)]
    public string ResultCode { get; set; }

    [DataMember(Order = 12)]
    public int ChangeId { get; set; }

    [DataMember(Order = 13)]
    public DateTime LastModified { get; set; }

    [DataMember(Order = 14)]
    public string WorkerName { get; set; }

    [DataMember(Order = 15, EmitDefaultValue = false)]
    public int? Order { get; set; }

    [DataMember(Order = 16, EmitDefaultValue = false)]
    public string RefName { get; set; }

    [DataMember(Order = 20)]
    public TaskLogReference Log { get; set; }

    [DataMember(Order = 30)]
    public TimelineReference Details { get; set; }

    [DataMember(Order = 40)]
    public int? ErrorCount { get; set; }

    [DataMember(Order = 50)]
    public int? WarningCount { get; set; }

    public List<Issue> Issues
    {
      get
      {
        if (this.m_issues == null)
          this.m_issues = new List<Issue>();
        return this.m_issues;
      }
    }

    [DataMember(EmitDefaultValue = false, Order = 70)]
    public TaskReference Task { get; set; }

    [DataMember(Order = 100)]
    public Uri Location { get; set; }

    [DataMember(Order = 130)]
    public int Attempt { get; set; }

    [DataMember(Order = 131)]
    public string Identifier { get; set; }

    [DataMember(Order = 140, EmitDefaultValue = false)]
    public int? QueueId { get; set; }

    [DataMember(Order = 141, EmitDefaultValue = false)]
    public JObject AgentSpecification { get; set; }

    public IList<TimelineAttempt> PreviousAttempts
    {
      get
      {
        if (this.m_previousAttempts == null)
          this.m_previousAttempts = new List<TimelineAttempt>();
        return (IList<TimelineAttempt>) this.m_previousAttempts;
      }
    }

    public IDictionary<string, VariableValue> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (IDictionary<string, VariableValue>) this.m_variables;
      }
    }

    public TimelineRecord Clone() => new TimelineRecord(this);

    public string ToStringExtended()
    {
      try
      {
        return JsonUtility.ToString((object) new
        {
          RecordId = this.Id,
          Name = this.Name,
          ParentId = this.ParentId,
          Attempt = this.Attempt,
          RecordType = this.RecordType,
          State = this.State,
          Result = this.Result,
          ChangeId = this.ChangeId
        });
      }
      catch (Exception ex)
      {
        return string.Format("{0}() encountered {1}", (object) "ToString", (object) ex.GetType());
      }
    }
  }
}
