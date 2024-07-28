// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.QueuedBuildModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Server;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  [DataContract]
  internal class QueuedBuildModel
  {
    public QueuedBuildModel(QueuedBuild build, string controller)
    {
      this.Id = build.Id;
      this.Definition = BuildDetailModel.GetDefinitionName(build.Definition);
      this.DefinitionStatus = build.Definition != null ? build.Definition.QueueStatus : DefinitionQueueStatus.Enabled;
      this.Priority = build.Priority;
      this.QueuedDate = build.QueueTime;
      this.RequestedBy = build.RequestedForDisplayName;
      this.Status = build.Status.ToString();
      this.QueueStatus = build.Status;
      this.Reason = build.Reason;
      this.Controller = controller;
      if (build.BuildUris.Count > 0)
        this.Uri = build.BuildUris.Last<string>();
      this.CustomGetVersion = build.CustomGetVersion;
      this.ShelvesetName = build.ShelvesetName;
    }

    [DataMember(Name = "id", EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(Name = "definition", EmitDefaultValue = false)]
    public string Definition { get; set; }

    [DataMember(Name = "definitionStatus", EmitDefaultValue = true)]
    public DefinitionQueueStatus DefinitionStatus { get; set; }

    [DataMember(Name = "priority", EmitDefaultValue = false)]
    public QueuePriority Priority { get; set; }

    [DataMember(Name = "controller", EmitDefaultValue = false)]
    public string Controller { get; set; }

    [DataMember(Name = "queuedDate", EmitDefaultValue = false)]
    public DateTime QueuedDate { get; set; }

    [DataMember(Name = "requestedBy", EmitDefaultValue = false)]
    public string RequestedBy { get; set; }

    [DataMember(Name = "status", EmitDefaultValue = false)]
    public string Status { get; set; }

    [DataMember(Name = "queueStatus", EmitDefaultValue = false)]
    public QueueStatus QueueStatus { get; set; }

    [DataMember(Name = "reason", EmitDefaultValue = false)]
    public BuildReason Reason { get; set; }

    [DataMember(Name = "uri", EmitDefaultValue = false)]
    public string Uri { get; set; }

    [DataMember(Name = "customGetVersion", EmitDefaultValue = false)]
    public string CustomGetVersion { get; set; }

    [DataMember(Name = "shelvesetName", EmitDefaultValue = false)]
    public string ShelvesetName { get; set; }
  }
}
