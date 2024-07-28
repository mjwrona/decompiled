// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTask
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class ReleaseTask : ReleaseManagementSecuredObject
  {
    public ReleaseTask() => this.Issues = new List<Issue>();

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public Guid TimelineRecordId { get; set; }

    [DataMember]
    public string Name { get; set; }

    [Obsolete("Use StartTime instead.")]
    [DataMember(EmitDefaultValue = false)]
    public DateTime? DateStarted { get; set; }

    [Obsolete("Use FinishTime instead")]
    [DataMember(EmitDefaultValue = false)]
    public DateTime? DateEnded { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; set; }

    [DataMember]
    public TaskStatus Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? PercentComplete { get; set; }

    [DataMember]
    public int? Rank { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public long LineCount { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "XML serializer cannot serialize collections/interfaces")]
    [DataMember(EmitDefaultValue = false)]
    public List<Issue> Issues { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public WorkflowTaskReference Task { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string AgentName { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "The name Url is appropariate name for this property and it signifies REST URL of resource")]
    [DataMember(EmitDefaultValue = false)]
    public string LogUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResultCode { get; set; }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.Issues?.ForEach((Action<Issue>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.Task?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
