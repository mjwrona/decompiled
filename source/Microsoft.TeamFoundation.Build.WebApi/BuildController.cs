// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildController
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class BuildController : QueueReference
  {
    public BuildController()
    {
      this.Status = ControllerStatus.Unavailable;
      this.QueueType = QueueType.BuildController;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Uri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ControllerStatus Status { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool Enabled { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CreatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime UpdatedDate { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Server { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<ShallowReference> Agents { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string CustomAssemblyPath { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int MaxConcurrentBuilds { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int QueueCount { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string StatusMessage { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string MessageQueueUrl { get; set; }
  }
}
