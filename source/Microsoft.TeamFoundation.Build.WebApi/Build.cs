// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Build
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public sealed class Build
  {
    private List<int> m_queueIds = new List<int>();

    public Build() => this.Reason = BuildReason.Manual;

    [DataMember(IsRequired = true)]
    public string Uri { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [Key]
    public int Id { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildNumber { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime StartTime { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime FinishTime { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildReason Reason { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildStatus Status { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Quality { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string DropLocation { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DropLocationReference Drop { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public LogLocationReference Log { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string SourceGetVersion { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef LastChangedBy { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool? RetainIndefinitely { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool HasDiagnostics { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Definition { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public QueueReference Queue { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<RequestReference> Requests { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Project { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ProcessParameters { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildPhaseStatus CompilationStatus { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildPhaseStatus TestStatus { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string DropUrl { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string LogLocation { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime LastChangedDate { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string LabelName { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string InformationNodesLocation { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsDeleted { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public long? ContainerId { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Controller { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool KeepForever { get; set; }
  }
}
