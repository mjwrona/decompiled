// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.TimelineRecord
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
  public sealed class TimelineRecord : BaseSecuredObject
  {
    [DataMember(Name = "_links", EmitDefaultValue = false)]
    private ReferenceLinks m_links;
    [DataMember(Name = "Issues", EmitDefaultValue = false, Order = 60)]
    private List<Issue> m_issues;
    [DataMember(Name = "PreviousAttempts", EmitDefaultValue = false)]
    private List<TimelineAttempt> m_previousAttempts;

    public TimelineRecord()
    {
    }

    internal TimelineRecord(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public Guid? ParentId { get; set; }

    [DataMember(Name = "Type")]
    public string RecordType { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public DateTime? StartTime { get; set; }

    [DataMember]
    public DateTime? FinishTime { get; set; }

    [DataMember]
    public string CurrentOperation { get; set; }

    [DataMember]
    public int? PercentComplete { get; set; }

    [DataMember]
    public TimelineRecordState? State { get; set; }

    [DataMember]
    public TaskResult? Result { get; set; }

    [DataMember]
    public string ResultCode { get; set; }

    [DataMember]
    public int ChangeId { get; set; }

    [DataMember]
    public DateTime LastModified { get; set; }

    [DataMember]
    public string WorkerName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? QueueId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Order { get; set; }

    [DataMember]
    public TimelineReference Details { get; set; }

    [DataMember]
    public int? ErrorCount { get; set; }

    [DataMember]
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

    [DataMember]
    public Uri Url { get; set; }

    [DataMember]
    public BuildLogReference Log { get; set; }

    [DataMember]
    public TaskReference Task { get; set; }

    [DataMember]
    public int Attempt { get; set; }

    [DataMember]
    public string Identifier { get; set; }

    public IList<TimelineAttempt> PreviousAttempts
    {
      get
      {
        if (this.m_previousAttempts == null)
          this.m_previousAttempts = new List<TimelineAttempt>();
        return (IList<TimelineAttempt>) this.m_previousAttempts;
      }
    }

    public ReferenceLinks Links
    {
      get
      {
        if (this.m_links == null)
          this.m_links = new ReferenceLinks();
        return this.m_links;
      }
    }
  }
}
