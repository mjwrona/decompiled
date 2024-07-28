// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildAgent
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public sealed class BuildAgent
  {
    public BuildAgent() => this.Status = AgentStatus.Offline;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Controller { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Server { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Uri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [Key]
    public int Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildDirectory { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public AgentStatus Status { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string StatusMessage { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool Enabled { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string MessageQueueUrl { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CreatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime UpdatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ReservedForBuild { get; set; }
  }
}
