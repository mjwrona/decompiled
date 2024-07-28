// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildRequest
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class BuildRequest : RequestReference
  {
    public BuildRequest() => this.Builds = new List<ShallowReference>();

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Uri { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public QueueReference Queue { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Definition { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<ShallowReference> Builds { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string CustomGetVersion { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string RequestDropLocation { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public QueuePriority Priority { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int QueuePosition { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime QueueTime { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildReason Reason { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef RequestedBy { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ShelvesetName { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public QueueStatus Status { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TeamProjectReference Project { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid BatchId { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Controller { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int DefinitionId { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DefinitionQueueStatus? DefinitionStatus { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public GetOption GetOption { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ProcessParameters { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TeamProject { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }
  }
}
