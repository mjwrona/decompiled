// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ExternalDeployment
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  [ClientIncludeModel]
  public class ExternalDeployment
  {
    [DataMember]
    public IEnumerable<int> RelatedWorkItemIds { get; set; }

    [DataMember]
    public Guid ArtifactId { get; set; }

    [DataMember]
    public ExternalPipeline Pipeline { get; set; }

    [DataMember]
    public ExternalEnvironment Environment { get; set; }

    [DataMember]
    public int RunId { get; set; }

    [DataMember]
    public int SequenceNumber { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string Status { get; set; }

    [DataMember]
    public string Group { get; set; }

    [DataMember]
    public Uri Url { get; set; }

    [DataMember]
    public DateTime StatusDate { get; set; }

    [DataMember]
    public Guid CreatedBy { get; set; }
  }
}
